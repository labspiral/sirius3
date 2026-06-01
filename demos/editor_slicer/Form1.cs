using System;
using System.Text;
using Microsoft.Extensions.Logging;

using SpiralLab.Sirius3.Document;
using SpiralLab.Sirius3.Scanner;
using SpiralLab.Sirius3.IO;
using SpiralLab.Sirius3.Scanner.Rtc;
using SpiralLab.Sirius3.PowerMeter;
using SpiralLab.Sirius3.Laser;
using SpiralLab.Sirius3.Marker;
using SpiralLab.Sirius3.Entity;
using SpiralLab.Sirius3.Entity.Hatch;
using SpiralLab.Sirius3.Mathematics;

#if OPENTK3
using OpenTK;
using DVec2 = OpenTK.Vector2d;
using DVec3 = OpenTK.Vector3d;
using DVec4 = OpenTK.Vector4d;
using DMat3 = OpenTK.Matrix3d;
using DMat4 = OpenTK.Matrix4d;
#elif OPENTK4
using OpenTK.Mathematics;
using DVec2 = OpenTK.Mathematics.Vector2d;
using DVec3 = OpenTK.Mathematics.Vector3d;
using DVec4 = OpenTK.Mathematics.Vector4d;
using DMat3 = OpenTK.Mathematics.Matrix3d;
using DMat4 = OpenTK.Mathematics.Matrix4d;
#endif

namespace Demos
{
    /// <summary>
    /// Form1
    /// </summary>
    public partial class Form1 : Form
    {
        /// <summary>
        /// Form constructor
        /// 폼 생성자
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
            this.FormClosing += (s, e) =>
            {
                var dlgResult = MessageBox.Show(this, $"Do you really want to terminate program ?", "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dlgResult != DialogResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }
                // Dispose instances 
                // 인스턴스 해제 
                siriusEditorControl1.DisposeDevices();

                // Dispose document
                // 문서 해제
                var doc = siriusEditorControl1.Document;
                siriusEditorControl1.Document = null;
                doc?.Dispose();

                // Clean up SIRIUS3 library
                // SIRIUS3 라이브러리 정리
                SpiralLab.Sirius3.Core.Cleanup();
            };

            btnLoad3DModel.Click += BtnLoad3DModel_Click;
            btnSliceContours.Click += BtnSliceContours_Click;
            btnHatchGenerate.Click += BtnAddHatch_Click;
            btnSimulationStart.Click += BtnSimulationStart_Click;
            btnSimulationStop.Click += BtnSimulationStop_Click;

            nudSlice.ValueChanged += NudSlice_ValueChanged;
        }

        /// <summary>
        /// Form load
        /// 폼 로드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            // Create devices
            // 장치 생성
            EditorHelper.CreateDevices(out IRtc rtc, out ILaser laser, out IDInput dInExt1, out IDInput dInLaserPort, out IDOutput dOutExt1, out IDOutput dOutExt2, out IDOutput dOutLaserPort, out IPowerMeter powerMeter, out IMarker marker);
            
            siriusEditorControl1.RegisterDevices(rtc, laser, powerMeter, dInExt1, dInLaserPort, dOutExt1, dOutExt2, dOutLaserPort, marker);

            marker.Ready(siriusEditorControl1.Document, siriusEditorControl1.View, rtc, laser, powerMeter);
        }

        private void BtnLoad3DModel_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            document.ActNew();

            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sample\\stl\\Nefertiti_face.stl");
            if (!File.Exists(fileName)) 
                return;

            document.ActImport(fileName, out var mesh);
            //or
            //EntityFactory.CreateMesh(fileName, out var mesh);
            //document.ActivePage.ActiveLayer.AddChild(mesh);

            siriusEditorControl1.View?.DoRender();

            document.ActSelect(mesh);

            EntityModelBase.CalculateRealMinMax( new IEntity[] { mesh }, out var realMin, out var realMax);
            
            nudMin.Value = (decimal)realMin.Z;
            nudMax.Value = (decimal)realMax.Z;

            nudSlice.Minimum = (decimal)realMin.Z;
            nudSlice.Maximum = (decimal)realMax.Z;
            nudSlice.Increment = 0.1M; // 0.1 mm step

            nudSlice.Value = (decimal)(Math.Round((realMin.Z + realMax.Z) / 2.0, 1));

        }

        private void SlicePreview()
        {
            var document = siriusEditorControl1.Document;
            if (1 != document.Selected.Length)
                return;
            var entity = document.Selected[0];
            var mesh = entity as EntityMesh;
            if (null == mesh) 
                return;

            mesh.IsAllowSlice = true; // !mesh.IsAllowSlice;

            mesh.CalculateRealMinMax(out var min, out var max);
            double sliceZ = (double)nudSlice.Value;
            mesh.SliceZ = sliceZ;

            siriusEditorControl1.View?.DoRender();
        }

        private void NudSlice_ValueChanged(object sender, EventArgs e)
        {
            SlicePreview();
        }

        private void BtnSliceContours_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            if (1 != document.Selected.Length)
                return;
            var entity = document.Selected[0];
            var mesh = entity as EntityMesh;
            if (null == mesh)
                return;

            double z = mesh.SliceZ;
            if (document.ActSlice(mesh, z, out var group))
            {
                mesh.CalculateRealMinMax(out var min, out var max);
                var width = max.X - min.X;
                var height = max.Y - min.Y;
                group.Translate(0, height, 0);
                document.ActSelect(group);
                
                siriusEditorControl1.View?.DoRender();
            }
        }

        private void BtnAddHatch_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            if (1 != document.Selected.Length)
                return;
            var entity = document.Selected[0];
            var hatchable = entity as IHatchable;
            if (null == hatchable)
                return;

            var rnd = new Random((int)DateTime.Now.Ticks);
            var angle = rnd.NextDouble() * 180 - 90;
            var interval = rnd.NextDouble() / 2.0 + 0.02;

            // line hatch
            var hatch = HatchFactory.CreateLine(angle, interval);
            //or polygon hatch
            //var hatch = HatchFactory.CreatePolygon(interval);
            hatch.Joint = HatchJoints.Miter;
            hatch.Exclude = 0.05;
            hatch.IsZigZag = true;
            hatch.Sort = HatchSorts.None;
            //hatch.Sort = HatchSorts.Near; // nearest. greedy 
            //hatch.Sort = HatchSorts.Global; //slow calculation but mark time optimized

            var index = (int)(rnd.NextDouble() * SpiralLab.Sirius3.UI.Config.EntityPenColors.Length);
            hatch.ModelColor = SpiralLab.Sirius3.UI.Config.EntityPenColors[index].ToDVec3();

            hatchable.AddHatch(hatch);
            //or
            //document.ActAddHatch(hatchable, hatch);

            hatchable.HatchMarkOption = HatchMarkOptions.HatchFirst;

            // regenerate hatch within entity
            document.ActRegen();

            siriusEditorControl1.View?.DoRender();
        }

        private void BtnSimulationStart_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            var marker = siriusEditorControl1.Marker;
            if (1 != document.Selected.Length)
                return;
            var entity = document.Selected[0];
            var markerable = entity as IMarkerable;
            if (null == markerable)
                return;
            
            document.ActSimulateStart(siriusEditorControl1.View, new IEntity[] { entity }, marker, IDocument.SimulationSpeeds.Fast);
        }
        private void BtnSimulationStop_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            document.ActSimulateStop();
        }
       
    }
}
