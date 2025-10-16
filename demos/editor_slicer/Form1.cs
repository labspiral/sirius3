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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;

            btnLoad3DModel.Click += BtnLoad3DModel_Click;
            btnSlicePreview.Click += BtnSlicePreview_Click;
            btnSliceContours.Click += BtnSliceContours_Click;
            btnHatchGenerate.Click += BtnAddHatch_Click;
            btnSimulationStart.Click += BtnSimulationStart_Click;
            btnSimulationStop.Click += BtnSimulationStop_Click;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            EditorHelper.CreateDevices(out IRtc rtc, out ILaser laser, out IDInput dInExt1, out IDInput dInLaserPort, out IDOutput dOutExt1, out IDOutput dOutExt2, out IDOutput dOutLaserPort, out IPowerMeter powerMeter, out IMarker marker);

            siriusEditorControl1.Scanner = rtc;

            siriusEditorControl1.Laser = laser;

            siriusEditorControl1.DIExt1 = dInExt1;
            siriusEditorControl1.DOExt1 = dOutExt1;
            siriusEditorControl1.DOExt2 = dOutExt2;
            siriusEditorControl1.DILaserPort = dInLaserPort;
            siriusEditorControl1.DOLaserPort = dOutLaserPort;

            siriusEditorControl1.PowerMeter = powerMeter;

            siriusEditorControl1.Marker = marker;

            marker.Ready(siriusEditorControl1.Document, siriusEditorControl1.View, rtc, laser, powerMeter);
        }

        private void BtnLoad3DModel_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;

            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sample\\Nefertiti_face.stl");
            if (!File.Exists(fileName)) 
                return;

            document.ActImport(fileName, out var mesh);
            //or
            //EntityFactory.CreateMesh(fileName, out var mesh);
            //document.ActivePage.ActiveLayer.AddChild(mesh);

            document.ActSelect(mesh);
        }

        private void BtnSlicePreview_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            if (1 != document.Selected.Length)
                return;
            var entity = document.Selected[0];
            var mesh = entity as EntityMesh;
            if (null == mesh) 
                return;

            mesh.IsAllowSlice = true; // !mesh.IsAllowSlice;

            if (mesh.SliceZ == 0)
            {
                mesh.CalcuateRealMinMax(out var min, out var max);
                double sliceZ = ((min + max) / 2.0).Z;
                mesh.SliceZ = sliceZ;
            }
            siriusEditorControl1.View?.DoRender();
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
                mesh.CalcuateRealMinMax(out var min, out var max);
                var width = max.X - min.X;
                var height = max.Y - min.Y;
                group.Translate(0, height, 0);
                document.ActSelect(group);
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

            var rng = new Random((int)DateTime.Now.Ticks);
            var angle = rng.NextDouble() * 180;
            var interval = rng.NextDouble() / 2.0 + 0.02;

            var hatch = HatchFactory.CreateLine(angle, interval);
            hatch.Joint = HatchJoints.Miter;
            hatch.Exclude = 0.05;
            hatch.IsZigZag = true;
            hatch.Sort = HatchSorts.Line;
            //hatch.Sort = HatchSorts.Near; // nearest. greedy 
            //hatch.Sort = HatchSorts.Global; //slow calculation but mark time optimized

            var index = (int)(rng.NextDouble() * SpiralLab.Sirius3.UI.Config.ScannerPenColors.Length);
            hatch.ModelColor = SpiralLab.Sirius3.UI.Config.ScannerPenColors[index].ToDVec3();

            hatchable.AddHatch(hatch);
            hatchable.HatchMarkOption = HatchMarkOptions.HatchFirst;

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
            
            document.ActSimulateStart(new IEntity[] { entity }, marker, IDocument.SimulationSpeeds.Fast);
        }
        private void BtnSimulationStop_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            document.ActSimulateStop();
        }
       
    }
}
