using System;

using SpiralLab.Sirius3.Document;
using SpiralLab.Sirius3.Scanner;
using SpiralLab.Sirius3.IO;
using SpiralLab.Sirius3.Scanner.Rtc;
using SpiralLab.Sirius3.PowerMeter;
using SpiralLab.Sirius3.Laser;
using SpiralLab.Sirius3.Marker;
using SpiralLab.Sirius3.Entity;
using System.Text;
using SpiralLab.Sirius3.Entity.Hatch;
using System.Diagnostics;
using SpiralLab.Sirius3.Scanner.Rtc.SyncAxis;

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
            this.btnMark.Click += BtnMark_Click;
            this.btnMarkSw1.Click += BtnMarkSw1_Click;
            this.btnMarkSw2.Click += BtnMarkSw2_Click;
            this.btnMarkSw3.Click += BtnMarkSw3_Click;
            this.btnMarkSw4.Click += BtnMarkSw4_Click;

            this.btnWobbelEllipse.Click += BtnWobbelEllipse_Click;
            this.btnWobbelParallel8.Click += BtnWobbelParallel8_Click;
            this.btnWobbelPerpendicular8.Click += BtnWobbelPerpendicular8_Click;
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

            MarkerOption();

            marker.Ready(siriusEditorControl1.Document, siriusEditorControl1.View, rtc, laser, powerMeter);
        }

        void MarkerOption()
        {
            var marker = siriusEditorControl1.Marker;
            if (marker is MarkerRtc markerRtc)
            {
                //default is 'True'
                markerRtc.IsMeasurementPlot = true;
            }
        }

        void CreateRectangleAndMeasurement(string title)
        {
            var document = siriusEditorControl1.Document;
            document.ActNew();

            // create measurement begin
            var begin = EntityFactory.CreateMeasurementBegin(
                10 * 1000,
                new MeasurementChannels[] {
                    MeasurementChannels.LaserOn,
                    MeasurementChannels.SampleX,
                    MeasurementChannels.SampleY,
                    MeasurementChannels.SampleZ,
                },
                title
                );
            document.ActivePage?.ActiveLayer?.AddChild(begin);
            var layerPenColor = SpiralLab.Sirius3.UI.Config.LayerPenColors[1]; // Color.Yellow
            document.ActivePage.ActiveLayer.PenColor = layerPenColor;


            var scannerPenColor = SpiralLab.Sirius3.UI.Config.ScannerPenColors[1]; // Color.Yellow
            // create rectangle
            var rectangle = EntityFactory.CreateRectangle(DVec2.Zero, 50, 50);
            rectangle.PenColor = scannerPenColor;
            document.ActivePage?.ActiveLayer?.AddChild(rectangle);

            // create circle
            var circle = EntityFactory.CreateArc(DVec2.Zero, 25, 0, 360);
            circle.PenColor = scannerPenColor;
            document.ActivePage?.ActiveLayer?.AddChild(circle);

            // create measurement end
            var end = EntityFactory.CreateMeasurementEnd();
            document.ActivePage?.ActiveLayer?.AddChild(end);

            // config scanner pen parameters
            document.FindByScannerPenColor(scannerPenColor, out var scannerPen);
            scannerPen.JumpSpeed = 3_000;
            scannerPen.MarkSpeed = 3_000;
        }

        private void BtnMark_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            var marker = siriusEditorControl1.Marker;

            CreateRectangleAndMeasurement("No skywriting");

            var layerPenColor = SpiralLab.Sirius3.UI.Config.LayerPenColors[1]; // Color.Yellow
            document.FindByLayerPenColor(layerPenColor, out var layerPen);
            layerPen.IsSkyWritingEnabled = false; //disable sky-writing

            var scannerPenColor = SpiralLab.Sirius3.UI.Config.ScannerPenColors[1]; // Color.Yellow
            document.FindByScannerPenColor(scannerPenColor, out var scannerPen);
            scannerPen.IsWobbelEnabled = false; //disable wobbel

            marker.Ready(document);
            marker.Page = document.Page;
            marker.Start();
        }

        private void BtnMarkSw1_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            var marker = siriusEditorControl1.Marker;
            Debug.Assert(marker.Rtc is not Rtc4);

            CreateRectangleAndMeasurement("skywriting 1");

            var layerPenColor = SpiralLab.Sirius3.UI.Config.LayerPenColors[1]; // Color.Yellow
            document.FindByLayerPenColor(layerPenColor, out var layerPen);
            layerPen.IsSkyWritingEnabled = true;
            layerPen.SkyWritingMode = SkyWritingModes.Mode1;
            layerPen.TimeLag = 250;
            layerPen.LaserOnShift = 0;
            layerPen.Prev = 1000;
            layerPen.Post = 1000;
            //layerPen.AngularLimit = 89;

            var scannerPenColor = SpiralLab.Sirius3.UI.Config.ScannerPenColors[1]; // Color.Yellow
            document.FindByScannerPenColor(scannerPenColor, out var scannerPen);
            scannerPen.IsWobbelEnabled = false;

            marker.Ready(document);
            marker.Page = document.Page;
            marker.Start();
        }

        private void BtnMarkSw2_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            var marker = siriusEditorControl1.Marker;
            Debug.Assert(marker.Rtc is not Rtc4);

            CreateRectangleAndMeasurement("skywriting 2");

            var layerPenColor = SpiralLab.Sirius3.UI.Config.LayerPenColors[1]; // Color.Yellow
            document.FindByLayerPenColor(layerPenColor, out var layerPen);
            layerPen.IsSkyWritingEnabled = true;
            layerPen.SkyWritingMode = SkyWritingModes.Mode2;
            layerPen.TimeLag = 250;
            layerPen.LaserOnShift = 0;
            layerPen.Prev = 1000;
            layerPen.Post = 1000;
            //layerPen.AngularLimit = 89;

            var scannerPenColor = SpiralLab.Sirius3.UI.Config.ScannerPenColors[1]; // Color.Yellow
            document.FindByScannerPenColor(scannerPenColor, out var scannerPen);
            scannerPen.IsWobbelEnabled = false;

            marker.Ready(document);
            marker.Page = document.Page;
            marker.Start();
        }

        private void BtnMarkSw3_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            var marker = siriusEditorControl1.Marker;
            Debug.Assert(marker.Rtc is not Rtc4);

            CreateRectangleAndMeasurement("skywriting 3");

            var layerPenColor = SpiralLab.Sirius3.UI.Config.LayerPenColors[1]; // Color.Yellow
            document.FindByLayerPenColor(layerPenColor, out var layerPen);
            layerPen.IsSkyWritingEnabled = true;
            layerPen.SkyWritingMode = SkyWritingModes.Mode3;
            layerPen.TimeLag = 250;
            layerPen.LaserOnShift = 0;
            layerPen.Prev = 2000 * 0.15;
            layerPen.Post = 2000 * 0.1;
            layerPen.AngularLimit = 89;

            var scannerPenColor = SpiralLab.Sirius3.UI.Config.ScannerPenColors[1]; // Color.Yellow
            document.FindByScannerPenColor(scannerPenColor, out var scannerPen);
            scannerPen.IsWobbelEnabled = false;

            marker.Ready(document);
            marker.Page = document.Page;
            marker.Start();
        }

        private void BtnMarkSw4_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            var marker = siriusEditorControl1.Marker;
            Debug.Assert(marker.Rtc is not Rtc4);
            Debug.Assert(marker.Rtc is Rtc6);

            CreateRectangleAndMeasurement("skywriting 4");

            var layerPenColor = SpiralLab.Sirius3.UI.Config.LayerPenColors[1]; // Color.Yellow
            document.FindByLayerPenColor(layerPenColor, out var layerPen);
            layerPen.IsSkyWritingEnabled = true;
            layerPen.SkyWritingMode = SkyWritingModes.Mode4;
            layerPen.TimeLag = 250;
            layerPen.LaserOnShift = 0;
            layerPen.Prev = 2000 * 0.15;
            layerPen.Post = 2000 * 0.1;
            layerPen.AngularLimit = 89;

            var scannerPenColor = SpiralLab.Sirius3.UI.Config.ScannerPenColors[1]; // Color.Yellow
            document.FindByScannerPenColor(scannerPenColor, out var scannerPen);
            scannerPen.IsWobbelEnabled = false;

            marker.Ready(document);
            marker.Page = document.Page;
            marker.Start();
        }

        private void BtnWobbelEllipse_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            var marker = siriusEditorControl1.Marker;

            CreateRectangleAndMeasurement("wobbel (ellipse)");

            var layerPenColor = SpiralLab.Sirius3.UI.Config.LayerPenColors[1]; // Color.Yellow
            document.FindByLayerPenColor(layerPenColor, out var layerPen);
            layerPen.IsSkyWritingEnabled = false;

            var scannerPenColor = SpiralLab.Sirius3.UI.Config.ScannerPenColors[1]; // Color.Yellow
            document.FindByScannerPenColor(scannerPenColor, out var scannerPen);
            scannerPen.WobbelShape = WobbelShapes.Ellipse;
            scannerPen.IsWobbelEnabled = true;
            scannerPen.WobbelFrequency = 200;
            scannerPen.WobbelPerpendicular = 0.5;
            scannerPen.WobbelParallel = 0.25;

            marker.Ready(document);
            marker.Page = document.Page;
            marker.Start();
        }
        private void BtnWobbelParallel8_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            var marker = siriusEditorControl1.Marker;
            Debug.Assert(marker.Rtc is not Rtc4);

            CreateRectangleAndMeasurement("wobbel (parallel8)");

            var layerPenColor = SpiralLab.Sirius3.UI.Config.LayerPenColors[1]; // Color.Yellow
            document.FindByLayerPenColor(layerPenColor, out var layerPen);
            layerPen.IsSkyWritingEnabled = false;

            var scannerPenColor = SpiralLab.Sirius3.UI.Config.ScannerPenColors[1]; // Color.Yellow
            document.FindByScannerPenColor(scannerPenColor, out var scannerPen);
            scannerPen.WobbelShape = WobbelShapes.Parallel8;
            scannerPen.IsWobbelEnabled = true;
            scannerPen.WobbelFrequency = 200;
            scannerPen.WobbelPerpendicular = 0.5;
            scannerPen.WobbelParallel = 0.25;

            marker.Ready(document);
            marker.Page = document.Page;
            marker.Start();
        }

        private void BtnWobbelPerpendicular8_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            var marker = siriusEditorControl1.Marker;
            Debug.Assert(marker.Rtc is not Rtc4);

            CreateRectangleAndMeasurement("wobbel (perpendicular8)");

            var layerPenColor = SpiralLab.Sirius3.UI.Config.LayerPenColors[1]; // Color.Yellow
            document.FindByLayerPenColor(layerPenColor, out var layerPen);
            layerPen.IsSkyWritingEnabled = false;

            var scannerPenColor = SpiralLab.Sirius3.UI.Config.ScannerPenColors[1]; // Color.Yellow
            document.FindByScannerPenColor(scannerPenColor, out var scannerPen);
            scannerPen.WobbelShape = WobbelShapes.Perpendicular8;
            scannerPen.IsWobbelEnabled = true;
            scannerPen.WobbelFrequency = 200;
            scannerPen.WobbelPerpendicular = 0.5;
            scannerPen.WobbelParallel = 0.25;

            marker.Ready(document);
            marker.Page = document.Page;
            marker.Start();
        }
    }
}
