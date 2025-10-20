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
using Microsoft.Extensions.Logging;
using SpiralLab.Sirius3;
using SpiralLab.Sirius3.Mathematics;
using SpiralLab.Sirius3.Scanner.Rtc.SyncAxis;
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

            btnSetVelocity.Click += BtnSetVelocity_Click;
            btnActualVelocity.Click += BtnActualVelocity_Click;
            btnPositionDependent.Click += BtnPositionDependent_Click;
            btnSpotDistance.Click += BtnSpotDistance_Click;
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

        private void CreateEntity()
        {
            var document = siriusEditorControl1.Document;
            var text = EntityFactory.CreateText("Arial",
              FontStyle.Regular,
              $"AaBbCcDdEeFfGg{Environment.NewLine}0123456789{Environment.NewLine}!@#$%^&*()-+<>",
              10);
            text.FontHorizontalAlignment = StringAlignment.Center;
            text.FontVerticalAlignment = StringAlignment.Center;

            document.ActivePage.ActiveLayer.AddChild(text);
        }
        private void BtnSetVelocity_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            document.ActNew();

            // create measurement begin
            var begin = EntityFactory.CreateMeasurementBegin(
                10 * 1000,
                new MeasurementChannels[] {
                    MeasurementChannels.SampleX,
                    MeasurementChannels.SampleY,
                    MeasurementChannels.LaserOn,
                    MeasurementChannels.ExtAO1,
                },
                "Set velocity + analog1"
                );
            document.ActivePage.ActiveLayer.AddChild(begin);

            CreateEntity();

            var end = EntityFactory.CreateMeasurementEnd();
            document.ActivePage.ActiveLayer.AddChild(end);

            Debug.Assert(document.ActivePage.ActiveLayer.PenColor == Color.White);

            // Find layer pen for 'White'
            document.FindByLayerPenColor(System.Drawing.Color.White, out var layerPenWhite);

            //Set veloticy + analog output
            layerPenWhite.IsALC = true;
            layerPenWhite.AlcSignal = AutoLaserControlSignals.Analog1;
            layerPenWhite.AlcMode = AutoLaserControlModes.SetVelocity;
            layerPenWhite.AlcPercentage100 = 5; //5V
            layerPenWhite.AlcMinValue = 4; // 4V
            layerPenWhite.AlcMaxValue = 6; //6V
            layerPenWhite.AlcByPositionTable.Clear();
        }

        private void BtnActualVelocity_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            document.ActNew();

            // create measurement begin
            var begin = EntityFactory.CreateMeasurementBegin(
                10 * 1000,
                new MeasurementChannels[] {
                    MeasurementChannels.SampleX,
                    MeasurementChannels.SampleY,
                    MeasurementChannels.LaserOn,
                    MeasurementChannels.OutputPeriod,
                },
                "Actual velocity + frequency"
                );
            document.ActivePage.ActiveLayer.AddChild(begin);

            CreateEntity();

            var end = EntityFactory.CreateMeasurementEnd();
            document.ActivePage.ActiveLayer.AddChild(end);

            Debug.Assert(document.ActivePage.ActiveLayer.PenColor == Color.White);

            // Find layer pen for 'White'
            document.FindByLayerPenColor(System.Drawing.Color.White, out var layerPenWhite);

            // Actual velocity + frequency
            layerPenWhite.IsALC = true;
            layerPenWhite.AlcSignal = AutoLaserControlSignals.Frequency;
            layerPenWhite.AlcMode = AutoLaserControlModes.ActualVelocity; // Only for iDRIVE scanner products 
            layerPenWhite.AlcPercentage100 = 50 * 1000; //50KHz
            layerPenWhite.AlcMinValue = 40 * 1000; //Lower cut off frequency : 40KHz
            layerPenWhite.AlcMaxValue = 60 * 1000; //Upper cut off frequency : 60KHz
            layerPenWhite.AlcByPositionTable.Clear();
        }

        private void BtnPositionDependent_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            document.ActNew();

            // create measurement begin
            var begin = EntityFactory.CreateMeasurementBegin(
                10 * 1000,
                new MeasurementChannels[] {
                    MeasurementChannels.SampleX,
                    MeasurementChannels.SampleY,
                    MeasurementChannels.LaserOn,
                    MeasurementChannels.ExtAO1,
                },
                "Set velocity + analog + position dependent"
                );
            document.ActivePage.ActiveLayer.AddChild(begin);

            CreateEntity();

            var end = EntityFactory.CreateMeasurementEnd();
            document.ActivePage.ActiveLayer.AddChild(end);

            Debug.Assert(document.ActivePage.ActiveLayer.PenColor == Color.White);

            // Find layer pen for 'White'
            document.FindByLayerPenColor(System.Drawing.Color.White, out var layerPenWhite);

            // Set veloticy + analog output 
            layerPenWhite.IsALC = true;
            layerPenWhite.AlcSignal = AutoLaserControlSignals.Analog1;
            layerPenWhite.AlcMode = AutoLaserControlModes.SetVelocity;
            layerPenWhite.AlcPercentage100 = 5; //5V
            layerPenWhite.AlcMinValue = 4; // 4V
            layerPenWhite.AlcMaxValue = 6; //6V
            // Distance(or radius) (mm), scale (0~4)
            var kvList = new List<KeyValuePair<double, double>>();
            kvList.Add(new KeyValuePair<double, double>(5, 0.9));
            kvList.Add(new KeyValuePair<double, double>(10, 1));
            kvList.Add(new KeyValuePair<double, double>(15, 1.1));
            kvList.Add(new KeyValuePair<double, double>(20, 1.2));
            kvList.Add(new KeyValuePair<double, double>(25, 1.3));
            kvList.Add(new KeyValuePair<double, double>(30, 1.4));
            kvList.Add(new KeyValuePair<double, double>(35, 1.5));
            kvList.Add(new KeyValuePair<double, double>(40, 1.6));
            kvList.Add(new KeyValuePair<double, double>(50, 2.0));
            // + Position dependent
            layerPenWhite.AlcByPositionTable = kvList;
        }

        private void BtnSpotDistance_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            document.ActNew();

            // create measurement begin
            var begin = EntityFactory.CreateMeasurementBegin(
                10 * 1000,
                new MeasurementChannels[] {
                    MeasurementChannels.SampleX,
                    MeasurementChannels.SampleY,
                    MeasurementChannels.LaserOn,
                    //MeasurementChannels.SpotDistance,
                },
                "Spot distance"
                );
            document.ActivePage.ActiveLayer.AddChild(begin);

            CreateEntity();

            var end = EntityFactory.CreateMeasurementEnd();
            document.ActivePage.ActiveLayer.AddChild(end);

            Debug.Assert(document.ActivePage.ActiveLayer.PenColor == Color.White);

            // Find layer pen for 'White'
            document.FindByLayerPenColor(System.Drawing.Color.White, out var layerPenWhite);

            // Actual velocity + spot distance control
            layerPenWhite.IsALC = true;
            layerPenWhite.AlcByPositionTable.Clear();
            layerPenWhite.AlcSignal = AutoLaserControlSignals.SpotDistance; //RTC6 + SCANAhead
            layerPenWhite.AlcMode = AutoLaserControlModes.ActualVelocityWithSCANAhead;
            double spotDistance = 0.1; 
            layerPenWhite.AlcPercentage100 = spotDistance;
            //layerPenWhite.AlcMinValue = 0; // not used at SpotDistance
            //layerPenWhite.AlcMaxValue = 0; // not used at SpotDistance
            layerPenWhite.AlcByPositionTable.Clear();
        }

    }
}
