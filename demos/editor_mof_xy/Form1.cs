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
using System.Diagnostics;
using SpiralLab.Sirius3;
using SpiralLab.Sirius3.Entity.Hatch;

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

            this.btnCreateEntities.Click += BtnCreateEntities_Click;
            this.btnStartStop.Click += BtnStartStop_Click;
            this.btnStartEncoderSimulation.Click += BtnStartEncoderSimulation_Click;
            this.btnStopEncoderSimulation.Click += BtnStopEncoderSimulation_Click;
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

            var rtcMoF = rtc as IRtcMoF;
            Debug.Assert(rtcMoF != null);
            rtcMoF.OnEncoderSignalError += OnEncoderSignalError;
            rtcMoF.OnOutOfVirtualImageField += OnOutOfVirtualImageField;

            siriusEditorControl1.RegisterDevices(rtc, laser, powerMeter, dInExt1, dInLaserPort, dOutExt1, dOutExt2, dOutLaserPort, marker);

            marker.Ready(siriusEditorControl1.Document, siriusEditorControl1.View, rtc, laser, powerMeter);
        }
        private void OnEncoderSignalError(IRtcMoF rtcMoF, IRtcMarkingInfo rtcMarkingInfo)
        {
            if (rtcMarkingInfo is Rtc6MarkingInfo rtc6MarkingInfo)
            {
                // For RTC6 card
                if (rtc6MarkingInfo.Contains(Rtc6MarkingInfo.Bit.Signal1EncoderXTooShort))
                {

                }
                if (rtc6MarkingInfo.Contains(Rtc6MarkingInfo.Bit.Signal1EncoderYTooShort))
                {

                }
                if (rtc6MarkingInfo.Contains(Rtc6MarkingInfo.Bit.Signal2EncoderXTooShort))
                {

                }
                if (rtc6MarkingInfo.Contains(Rtc6MarkingInfo.Bit.Signal2EncoderYTooShort))
                {

                }
                if (rtc6MarkingInfo.Contains(Rtc6MarkingInfo.Bit.WrongSignalSequenceEncoderX))
                {

                }
                if (rtc6MarkingInfo.Contains(Rtc6MarkingInfo.Bit.WrongSignalSequenceEncoderY))
                {

                }
            }
            else if (rtcMarkingInfo is Rtc5MarkingInfo rtc5MarkingInfo)
            {
                // For RTC5 card
                if (rtc5MarkingInfo.Contains(Rtc5MarkingInfo.Bit.Signal1EncoderXTooShort))
                {

                }
                if (rtc5MarkingInfo.Contains(Rtc5MarkingInfo.Bit.Signal1EncoderYTooShort))
                {

                }
                if (rtc5MarkingInfo.Contains(Rtc5MarkingInfo.Bit.Signal2EncoderXTooShort))
                {

                }
                if (rtc5MarkingInfo.Contains(Rtc5MarkingInfo.Bit.Signal2EncoderYTooShort))
                {

                }
                if (rtc5MarkingInfo.Contains(Rtc5MarkingInfo.Bit.WrongSignalSequenceEncoderX))
                {

                }
                if (rtc5MarkingInfo.Contains(Rtc5MarkingInfo.Bit.WrongSignalSequenceEncoderY))
                {

                }
            }
            else if (rtcMarkingInfo is Rtc4MarkingInfo rtc4MarkingInfo)
            {
                // For RTC4 card
                // Not supported  
            }

            this.Invoke(new MethodInvoker(() =>
            {
                MessageBox.Show(this, $"Encoder Signal Error: {rtcMarkingInfo.ToString()}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);                
            }));

        }

        private void OnOutOfVirtualImageField(IRtcMoF rtcMoF, IRtcMarkingInfo rtcMarkingInfo)
        {
            if (rtcMarkingInfo is Rtc6MarkingInfo rtc6MarkingInfo)
            {
                // For RTC6 card
                if (rtc6MarkingInfo.Contains(Rtc6MarkingInfo.Bit.MoFOverflowInXDirection))
                {

                }
                if (rtc6MarkingInfo.Contains(Rtc6MarkingInfo.Bit.MoFUnderflowInXDirection))
                {

                }
                if (rtc6MarkingInfo.Contains(Rtc6MarkingInfo.Bit.MoFOverflowInYDirection))
                {

                }
                if (rtc6MarkingInfo.Contains(Rtc6MarkingInfo.Bit.MoFUnderflowInYDirection))
                {

                }

            }
            else if (rtcMarkingInfo is Rtc5MarkingInfo rtc5MarkingInfo)
            {
                // For RTC5 card
                if (rtc5MarkingInfo.Contains(Rtc5MarkingInfo.Bit.MoFOverflowInXDirection))
                {

                }
                if (rtc5MarkingInfo.Contains(Rtc5MarkingInfo.Bit.MoFUnderflowInXDirection))
                {

                }
                if (rtc5MarkingInfo.Contains(Rtc5MarkingInfo.Bit.MoFOverflowInYDirection))
                {

                }
                if (rtc5MarkingInfo.Contains(Rtc5MarkingInfo.Bit.MoFUnderflowInYDirection))
                {

                }
            }
            else if (rtcMarkingInfo is Rtc4MarkingInfo rtc4MarkingInfo)
            {
                // For RTC4 card
                // Not supported (no virtual image field)
            }

            this.Invoke(new MethodInvoker(() =>
            {
                MessageBox.Show(this, $"Out of Virtual Image Field: {rtcMarkingInfo.ToString()}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }));
        }

        private void BtnCreateEntities_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            document.ActNew();

            CreateEntities();
        }

        /// <summary>
        /// Create entities
        /// 엔티티 생성
        /// </summary>
        void CreateEntities()
        {
            var document = siriusEditorControl1.Document;
            var marker = siriusEditorControl1.Marker;
            var rtc = siriusEditorControl1.Scanner as IRtc;

            // Create mof begin 
            // with encoder reset
            // Also, need to MoF at library option.
            //Debug.Assert(rtc.IsMoF);
            //Core.License(out var licenseInfo);
            //Debug.Assert(licenseInfo.IsMoFLicensed);

            var mofBegin = EntityFactory.CreateMoFBegin(RtcMoFModes.XY, true);
            document.ActAdd(mofBegin);

            /*      
             *      
             *      
             *      
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

            // Adjust RtcEncoderWaitConditions condition to marks at scanner center area
            double x1 = -1;
            var mofWait1 = EntityFactory.CreateMoFWait(RtcEncoders.EncX, RtcEncoderWaitConditions.Over, -x1);
            document.ActAdd(mofWait1);
            var line1 = EntityFactory.CreateLine(x1, 10, x1, -10);
            document.ActAdd(line1);

            double x2 = -5;
            var mofWait2 = EntityFactory.CreateMoFWait(RtcEncoders.EncX, RtcEncoderWaitConditions.Over, -x2);
            document.ActAdd(mofWait2);
            var line2 = EntityFactory.CreateLine(x2, 15, x2, -15);
            document.ActAdd(line2);

            double x3 = -10;
            var mofWait3 = EntityFactory.CreateMoFWait(RtcEncoders.EncX, RtcEncoderWaitConditions.Over, -x3);
            document.ActAdd(mofWait3);
            var line3 = EntityFactory.CreateLine(x3, 20, x3, -20);
            document.ActAdd(line3);

            double x4 = -15;
            var mofWait4 = EntityFactory.CreateMoFWait(RtcEncoders.EncX, RtcEncoderWaitConditions.Over, -x4);
            document.ActAdd(mofWait4);
            var spiral = EntityFactory.CreateSpiral(x4, 0, 2, 0, 5, EntitySpiral.SpiralTypes.Archimedean, true);
            document.ActAdd(spiral);

            double x5 = -20;
            var mofWait5 = EntityFactory.CreateMoFWait(RtcEncoders.EncX, RtcEncoderWaitConditions.Over, -x5);
            document.ActAdd(mofWait5);
            var dataMatrix = EntityFactory.CreateDataMatrix("SIRIUS3", EntityBarcode2DBase.Barcode2DCells.Outline, 4, 4);
            dataMatrix.RotateZ(90);
            dataMatrix.Name = "MyBarcode1";
            dataMatrix.Translate(x5, -10);
            document.ActAdd(dataMatrix);

            var text = EntityFactory.CreateText("Arial", FontStyle.Bold, "SIRIUS3", 4);
            text.RotateZ(90);
            text.Translate(x5, 10);
            document.ActAdd(text);

            double x6 = -40;
            var mofWait6 = EntityFactory.CreateMoFWait(RtcEncoders.EncX, RtcEncoderWaitConditions.Over, -x6);
            document.ActAdd(mofWait6);
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
            document.ActAdd(points);

            var mofEnd = EntityFactory.CreateMoFEnd(DVec2.Zero);
            document.ActAdd(mofEnd);

            siriusEditorControl1.View?.DoRender();

            // Check if RTC card is ready for MoF(aka. Processing on the fly)
            // IRtcMoF 사용하기 위한 준비가 되었는지 여부 확인
            Debug.Assert(rtc.IsMoF);

            var rtcMoF = rtc as IRtcMoF;
            Debug.Assert(rtcMoF != null);
            Debug.Assert(rtcMoF.EncXCountsPerMm != 0);
            //Debug.Assert(rtcMoF.EncYCountsPerMm != 0);
        }

        private void BtnStartStop_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
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
                marker.Start(document.Page); // current page
            }
        }

        private void BtnStartEncoderSimulation_Click(object sender, EventArgs e)
        {
            var rtc = siriusEditorControl1.Scanner as IRtc;
            if (rtc.IsBusy)
                return;

            // Check if RTC card is ready for MoF(aka. Processing on the fly)
            // IRtcMoF 사용하기 위한 준비가 되었는지 여부 확인
            Debug.Assert(rtc.IsMoF);
            var rtcMoF = rtc as IRtcMoF;

            // Start simulated encoders as x= 1, y=0 mm/s by rtcMoF.CtlMofEncoderSpeed(1, 0);
            rtcMoF.CtlMoFEncoderSpeed(1, 0);
            // or
            // Edit 'Simulated x speed at MoF = 1' at propertygrid of scanner(RTC) page
            // and
            // Marker.Start
        }

        private void BtnStopEncoderSimulation_Click(object sender, EventArgs e)
        {
            var rtc = siriusEditorControl1.Scanner as IRtc;

            // Check if RTC card is ready for MoF(aka. Processing on the fly)
            // IRtcMoF 사용하기 위한 준비가 되었는지 여부 확인
            Debug.Assert(rtc.IsMoF);
            var rtcMoF = rtc as IRtcMoF;

            // Deactivated simulated encoders 
            rtcMoF.CtlMoFEncoderSpeed(0, 0);
            // Reset encoders
            rtcMoF.CtlMoFEncoderReset();
        }
    }
}
