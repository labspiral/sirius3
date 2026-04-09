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
using System.Diagnostics;
using SpiralLab.Sirius3;


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
            this.FormClosing += (s, e) =>
            {
                var dlgResult = MessageBox.Show(this, $"Do you really want to terminate program ?", "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dlgResult != DialogResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }

                // Dispose document
                var doc = siriusEditorControl1.Document;
                siriusEditorControl1.Document = null;
                doc?.Dispose();

                // Dispose instances 
                siriusEditorControl1.DisposeDevices();

                // Clean up SIRIUS3 library
                SpiralLab.Sirius3.Core.Cleanup();
            };

            btnLoad3DModel.Click += BtnLoad3DModel_Click;
            btnGridCloud.Click += BtnGridCloud_Click;
            btnPointsCloudCalibrationAndApply.Click += BtnPointsCloudCalibrationAndApply_Click;
            btnRevertFieldCorrection.Click += BtnRevertFieldCorrection_Click;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            EditorHelper.CreateDevices(out IRtc rtc, out ILaser laser, out IDInput dInExt1, out IDInput dInLaserPort, out IDOutput dOutExt1, out IDOutput dOutExt2, out IDOutput dOutLaserPort, out IPowerMeter powerMeter, out IMarker marker);

            // Need to option for 3D
            Debug.Assert(rtc is IRtc3D);
            Debug.Assert(rtc.Is3D);

            var inputCtFileName = rtc.CorrectionFiles[(int)rtc.PrimaryHeadTable].FileName;
            Debug.Assert(inputCtFileName.Contains("D3_"));

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
            document.ActNew();

            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sample\\stl\\Nefertiti_face.stl");
            if (!File.Exists(fileName)) 
                return;

            document.ActImport(fileName, out var mesh);
            //or
            //EntityFactory.CreateMesh(fileName, out var mesh);
            //document.ActivePage.ActiveLayer.AddChild(mesh);

            document.ActSelect(mesh);
            siriusEditorControl1.View?.DoRender();
        }

        private void BtnGridCloud_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            if (1 != document.Selected.Length)
                return;
            var entity = document.Selected[0];
            var mesh = entity as EntityMesh;
            if (null == mesh) 
                return;

            // Extract pointscloud by each vertex
            //if (!document.ActPointCloud(mesh, -DVec3.UnitZ, out var rayOriginOffset, out List<DVec3> vertices)
            //    return;

            // Extract pointscloud by using fixed grids interval
            // Smaller value cause performance drop during 'RtcCalibrationLibrary.PointsCloudCalibration'
            const double interval = 1;// 0.3; 

            if (!document.ActGridCloud(mesh, interval, out DVec3[] vertices, out DVec3[] normals))
                return;

            // Create pointscloud as points entity
            //var points = new EntityPoints(vertices);
            var points = new EntityPoints(vertices, normals);
            // Get real dimension of points entity
            mesh.CalcuateRealMinMax(out var realMin, out var realMax);
            double width = realMax.X - realMin.X;
            double height = realMax.Y - realMin.Y;
            points.Translate(0, -height, 0);

            // Not allow hit test 
            points.IsAllowHitTest = false;

            // Add points entity into layer
            document.ActivePage?.ActiveLayer?.AddChild(points);

            // Create and prepare entity for mark hover pointscloud
            var text = new EntitySiriusText("ocra.cxf", EntitySiriusText.LetterSpaces.Variable, 0.2, 0.5, 1, "AaBbGg 012", 10);
            text.Translate(0, -height, 0);
            document.ActivePage?.ActiveLayer?.AddChild(text);

            document.ActSelect(points);
            siriusEditorControl1.View?.DoRender();
        }

        private void BtnPointsCloudCalibrationAndApply_Click(object sender, EventArgs e)
        {
            var rtc = siriusEditorControl1.Scanner as IRtc;
            Debug.Assert(rtc.Is3D); 

            var document = siriusEditorControl1.Document;
            if (1 != document.Selected.Length)
                return;
            var entity = document.Selected[0];
            var points = entity as EntityPoints;
            if (null == points)
                return;

            if (!points.CalculateTransformedVerticesAndNormals(out var vertices, out var normals))
                return;

            var inputCtFileName = rtc.CorrectionFiles[(int)rtc.PrimaryHeadTable].FileName;
            string dirName = Path.GetDirectoryName(inputCtFileName);
            string fileName = Path.GetFileNameWithoutExtension(inputCtFileName);
            var newCtFileName = Path.Combine(dirName, $"{fileName}_PointsCloud.ct5");
            try
            {
                Cursor = Cursors.WaitCursor;
                // It takes heavy time for calaulation if too many points are exist.
                if (!RtcCalibrationLibrary.PointsCloudCalibration(vertices.ToArray(), inputCtFileName, null, newCtFileName, out var returnCode))
                    return;
            }
            finally
            {
                Cursor = Cursors.Default;
            }

            LoadAndSelectCorrectionFile(rtc, newCtFileName);
            document.ActRemove(points);
            siriusEditorControl1.View?.DoRender();
        }

        private void BtnRevertFieldCorrection_Click(object sender, EventArgs e)
        {
            var rtc = siriusEditorControl1.Scanner as IRtc;
            RevertCorrectionFile(rtc);
        }

        private bool LoadAndSelectCorrectionFile(IRtc rtc, string newCtFileName)
        {
            bool success = true;
            CorrectionTables targetTable = CorrectionTables.None;
            switch (rtc.RtcCard)
            {
                case RtcCards.Rtc5:
                    targetTable = CorrectionTables.Table4;
                    success &= rtc.CtlLoadCorrectionFile(targetTable, newCtFileName);
                    // select new correction table at primary/secondary head
                    success &= rtc.CtlSelectCorrection(targetTable, targetTable);
                    break;
                case RtcCards.Rtc6:
                    targetTable = CorrectionTables.Table8;
                    success &= rtc.CtlLoadCorrectionFile(targetTable, newCtFileName);
                    // select new correction table at primary/secondary head
                    success &= rtc.CtlSelectCorrection(targetTable, targetTable);
                    break;
                default:
                    throw new InvalidOperationException();
            }
            var rtc3D = rtc as IRtc3D;
            if (success)
            {
                var coeff = rtc3D.CoeffABC;
                var stretchFactor = rtc3D.StretchFactor;
                MessageBox.Show(this, $"New 3D calibration has applied: {newCtFileName} at {targetTable}{Environment.NewLine}Coefficient A,B,C: {coeff}{Environment.NewLine}Stretch factor: {rtc3D.StretchFactor}", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
                MessageBox.Show(this, $"Fail to load and select 3D calibration: {newCtFileName} at {targetTable}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            return success;
        }
        private bool RevertCorrectionFile(IRtc rtc)
        {
            bool success = true;
            switch (rtc.RtcCard)
            {
                case RtcCards.Rtc5:
                    success &= rtc.CtlSelectCorrection(CorrectionTables.Table1);
                    break;
                case RtcCards.Rtc6:
                    success &= rtc.CtlSelectCorrection(CorrectionTables.Table1);
                    break;
                default:
                    throw new InvalidOperationException();
            }
            if (success)
                MessageBox.Show(this, $"3D calibration has reset to original(or default) correction table", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return success;
        }
    }
}
