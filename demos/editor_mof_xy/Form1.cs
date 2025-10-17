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

            this.btnCreateEntities.Click += BtnCreateEntities_Click;
            this.btnStartStop.Click += BtnStartStop_Click;
            this.btnStartEncoderSimulation.Click += BtnStartEncoderSimulation_Click;
            this.btnStopEncoderSimulation.Click += BtnStopEncoderSimulation_Click;
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

        private void BtnCreateEntities_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            document.ActNew();

            CreateEntities();
        }

        void CreateEntities()
        {
            var document = siriusEditorControl1.Document;
            var marker = siriusEditorControl1.Marker;
            var rtc = siriusEditorControl1.Scanner as IRtc;

            // Create mof begin 
            // with encoder reset
            var mofBegin = EntityFactory.CreateMoFBegin(RtcMoFModes.XY, true);
            document.ActivePage?.ActiveLayer?.AddChild(mofBegin);

            /*      
                    *                     |
                    *                     |
                    *                     |
                    *                     |
                    *     . .             |
                    *      .        |     |
                    *       .       | |   |
                    *     .         | | | |
                    *  ----.--▯--@--|-|-|-+-------------------    => ENC +
                    *       .       | | | |                       => MOVING DIRECTION 
                    *     .         | |   |
                    *      .        |     |
                    *        .            |
                    *      .              |
                    *                     |
                    *                     |
                    *                     |
                    */
            // adjust RtcEncoderWaitConditions contition to marks at scanner center area

            double x1 = -1;
            var mofWait1 = EntityFactory.CreateMoFWait(RtcEncoders.EncX, RtcEncoderWaitConditions.Over, -x1);
            document.ActivePage?.ActiveLayer?.AddChild(mofWait1);
            var line1 = EntityFactory.CreateLine(x1, 10, x1, -10);
            document.ActivePage?.ActiveLayer?.AddChild(line1);

            double x2 = -5;
            var mofWait2 = EntityFactory.CreateMoFWait(RtcEncoders.EncX, RtcEncoderWaitConditions.Over, -x2);
            document.ActivePage?.ActiveLayer?.AddChild(mofWait2);
            var line2 = EntityFactory.CreateLine(x2, 15, x2, -15);
            document.ActivePage?.ActiveLayer?.AddChild(line2);

            double x3 = -10;
            var mofWait3 = EntityFactory.CreateMoFWait(RtcEncoders.EncX, RtcEncoderWaitConditions.Over, -x3);
            document.ActivePage?.ActiveLayer?.AddChild(mofWait3);
            var line3 = EntityFactory.CreateLine(x3, 20, x3, -20);
            document.ActivePage?.ActiveLayer?.AddChild(line3);

            double x4 = -15;
            var mofWait4 = EntityFactory.CreateMoFWait(RtcEncoders.EncX, RtcEncoderWaitConditions.Over, -x4);
            document.ActivePage?.ActiveLayer?.AddChild(mofWait4);
            var spiral = EntityFactory.CreateSpiral(x4, 0, 2, 4, 0, 5, true);
            document.ActivePage?.ActiveLayer?.AddChild(spiral);

            double x5 = -20;
            var mofWait5 = EntityFactory.CreateMoFWait(RtcEncoders.EncX, RtcEncoderWaitConditions.Over, -x5);
            document.ActivePage?.ActiveLayer?.AddChild(mofWait5);
            var dataMatrix = EntityFactory.CreateDataMatrix("SIRIUS2", EntityBarcode2DBase.Barcode2DCells.Outline, 3, 4, 4);
            dataMatrix.RotateZ(90);
            dataMatrix.Name = "MyBarcode1";
            dataMatrix.Translate(x5, -10);
            document.ActivePage?.ActiveLayer?.AddChild(dataMatrix);

            var text = EntityFactory.CreateText("Arial", FontStyle.Bold, "SIRIUS2", 4);
            text.RotateZ(90);
            text.Translate(x5, 10);
            document.ActivePage?.ActiveLayer?.AddChild(text);

            double x6 = -40;
            var mofWait6 = EntityFactory.CreateMoFWait(RtcEncoders.EncX, RtcEncoderWaitConditions.Over, -x6);
            document.ActivePage?.ActiveLayer?.AddChild(mofWait6);
            double xRange = 2;
            double yRange = 30;
            var rnd = new Random();
            var pts = new List<DVec2>(20);
            for (int i = 0; i < 20; i++)
            {
                double x = x6 + rnd.NextDouble() * (xRange + xRange) - xRange;
                double y = rnd.NextDouble() * (yRange + yRange) - yRange;
                pts.Add(new DVec2(x, y));
            }
            var points = EntityFactory.CreatePoints(pts);
            document.ActivePage?.ActiveLayer?.AddChild(points);

            var mofEnd = EntityFactory.CreateMoFEnd(DVec2.Zero);
            document.ActivePage?.ActiveLayer?.AddChild(mofEnd);

            Debug.Assert(rtc.IsMoF);
            var rtcMoF = rtc as IRtcMoF;
            Debug.Assert(rtcMoF != null);
            Debug.Assert(rtcMoF.EncXCountsPerMm != 0);
            //Debug.Assert(rtcMoF.EncYCountsPerMm != 0);
        }

        private void BtnStartStop_Click(object sender, EventArgs e)
        {
            var marker = siriusEditorControl1.Marker;

            if (marker.IsBusy)
            {
                marker.Stop();
                marker.Reset();
            }
            else
            {
                marker.Reset();
                marker.Ready(siriusEditorControl1.Document);
                //marker.Page = DocumentPages.Page1;
                marker.Start();
            }
        }

        private void BtnStartEncoderSimulation_Click(object sender, EventArgs e)
        {
            var rtc = siriusEditorControl1.Scanner as IRtc;
            if (rtc.IsBusy)
                return;

            Debug.Assert(rtc.IsMoF);
            var rtcMoF = rtc as IRtcMoF;

            // Start simulated encoders as x= 1, y=0 mm/s by rtcMoF.CtlMofEncoderSpeed(1, 0);
            rtcMoF.CtlMofEncoderSpeed(1, 0);
            // or
            // Edit 'Simulated x speed at MoF = 1' at propertygrid of scanner(RTC) page
            // and
            // Marker.Start
        }

        private void BtnStopEncoderSimulation_Click(object sender, EventArgs e)
        {
            var rtc = siriusEditorControl1.Scanner as IRtc;

            Debug.Assert(rtc.IsMoF);
            var rtcMoF = rtc as IRtcMoF;

            // Deactivated simulated encoders 
            rtcMoF.CtlMofEncoderSpeed(0, 0);

        }

    }
}
