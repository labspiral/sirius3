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
            btnGridCloud.Click += BtnGridCloud_Click;
            btnFieldCorrection3D.Click += BtnFieldCorrection_Click;
            btnRevertFieldCorrection.Click += BtnRevertFieldCorrection_Click;
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
            document.ActNew();

            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sample\\Nefertiti_face.stl");
            if (!File.Exists(fileName)) 
                return;

            document.ActImport(fileName, out var mesh);
            //or
            //EntityFactory.CreateMesh(fileName, out var mesh);
            //document.ActivePage.ActiveLayer.AddChild(mesh);

            document.ActSelect(mesh);
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

            // grid interval for hit-test 
            // smaller value cause performance drop during 'RtcCalibrationLibrary.PointsCloudCalibration'
            const double interval = 1;// 0.3; 

            if (!document.ActGridCloud(mesh, interval, out DVec3[] vertices, out DVec3[] normals))
                return;

            // create points entity
            var points = new EntityPoints(vertices, normals);
            // get real dimension of points entity
            mesh.CalcuateRealMinMax(out var realMin, out var realMax);
            double width = realMax.X - realMin.X;
            double height = realMax.Y - realMin.Y;
            points.Translate(0, -height, 0);

            document.ActivePage?.ActiveLayer?.AddChild(points);

            // create(or prepare) mark entity hover cloud
            var text = new EntitySiriusText("ocra.cxf", EntitySiriusText.LetterSpaces.Variable, 0.2, 0.5, 1, "AaBbGg 012", 10);
            text.Translate(0, -height, 0);
            document.ActivePage?.ActiveLayer?.AddChild(text);

            document.ActSelect(points);
        }

        private void BtnFieldCorrection_Click(object sender, EventArgs e)
        {
            var rtc = siriusEditorControl1.Scanner as IRtc;
            //Debug.Assert(rtc.Is3D); 

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
                // it takes heavy time for calaulation
                // do works as async if you want
                if (!RtcCalibrationLibrary.PointsCloudCalibration(vertices.ToArray(), inputCtFileName, null, newCtFileName, out var returnCode))
                    return;
            }
            finally
            {
                Cursor = Cursors.Default;
            }

            LoadAndSelectCorrectionFile(rtc, newCtFileName);
            document.ActRemove(points);
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
            if (success)
                MessageBox.Show($"New 3D calibration has applied: {newCtFileName} at {targetTable}");
            else
                MessageBox.Show($"Fail to load and select 3D calibration: {newCtFileName} at {targetTable}");

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
                MessageBox.Show($"3D calibration has reset to original(or default) correction table");
            return success;
        }
    }
}
