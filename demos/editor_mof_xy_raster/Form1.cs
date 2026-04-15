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
            this.FormClosing += (s, e) =>
            {
                var dlgResult = MessageBox.Show(this, $"Do you really want to terminate program ?", "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dlgResult != DialogResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }
                // Dispose instances 
                siriusEditorControl1.DisposeDevices();

                // Dispose document
                var doc = siriusEditorControl1.Document;
                siriusEditorControl1.Document = null;
                doc?.Dispose();


                // Clean up SIRIUS3 library
                SpiralLab.Sirius3.Core.Cleanup();
            };

            this.btnCreateEntities_Eventhandler.Click += BtnCreateEntities_Eventhandler_Click;
            this.btnStartStop.Click += BtnStartStop_Click;
            this.btnStartEncoderSimulation.Click += BtnStartEncoderSimulation_Click;
            this.btnStopEncoderSimulation.Click += BtnStopEncoderSimulation_Click;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            EditorHelper.CreateDevices(out IRtc rtc, out ILaser laser, out IDInput dInExt1, out IDInput dInLaserPort, out IDOutput dOutExt1, out IDOutput dOutExt2, out IDOutput dOutLaserPort, out IPowerMeter powerMeter, out IMarker marker);

            var rtcMoF = rtc as IRtcMoF;
            Debug.Assert(rtcMoF != null);
            rtcMoF.OnEncoderSignalError += OnEncoderSignalError;
            rtcMoF.OnOutOfVirtualImageField += OnOutOfVirtualImageField;

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

        private void BtnCreateEntities_Eventhandler_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            document.ActNew();

            CreateEntities();

            CreateEventhandler();
        }

        void CreateEntities()
        {
            var document = siriusEditorControl1.Document;
            var marker = siriusEditorControl1.Marker;
            var view = siriusEditorControl1.View;
            var rtc = siriusEditorControl1.Scanner as IRtc;

            // Create mof begin 
            // with encoder reset
            // Also, need to MoF at library option.
            //Debug.Assert(rtc.IsMoF);
            //Core.License(out var licenseInfo);
            //Debug.Assert(licenseInfo.IsMoFLicensed);


            /*      
             *                           |
             *                           |
             *                           |
             *                           |
             *                           | _________
             *                           | |       |
             *                           | |       | 
             *  <= ENC-   ---------------+-|-IMAGE |--------    
             *                           | |       |                
             *                           | |_______| 
             *                           |
             *                           |
             *                           |
             *                           |
             *                           |
             *  <= MOVING DIRECTION 
             *  
             */


            var mofBegin = EntityFactory.CreateMoFBegin(RtcMoFModes.XY, true);
            document.ActivePage?.ActiveLayer?.AddChild(mofBegin);

            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sample\\image\\imagekorea.jpg");
            if (!File.Exists(fileName)) return;
            var image = EntityFactory.CreateImage(fileName, 20);
            var penColor = SpiralLab.Sirius3.UI.Config.EntityPenColors[0]; // White
            image.PenColor = penColor;
            image.AlignmentXY = AlignmentXYs.MiddleLeft;
            image.MajorColor = MajorColors.Black;
            document.ActivePage?.ActiveLayer?.AddChild(image);
            // X 방향으로 +1mm 위치로 이동
            image.Translate(1, 0); 


            var mofEnd = EntityFactory.CreateMoFEnd(DVec2.Zero);
            document.ActivePage?.ActiveLayer?.AddChild(mofEnd);

            siriusEditorControl1.View?.DoRender();


            document.FindByEntityPenColor(penColor, out var pen);
            //pen.Frequency =
            //pen.PulseWidth =
            //pen.Power = 
            //pen.JumpSpeed = 1_000;
            //pen.MarkSpeed = 1_000;
            //pen.ScannerJumpDelay = ;
            //pen.ScannerMarkDelay = ;
            //pen.ScannerPolygonDelay = ;
            //pen.LaserOnDelay = ;
            //pen.LaserOffDelay = ;

            pen.RasterDirection = EntityPen.RasterDirections.Vertical;
            pen.RasterMode = RasterModes.JumpAndShoot;
            pen.IsRasterZigZag = true;
            pen.PixelTime = 100; // 100usec
            // LASER PORt의 D.IN1 을 레이저의 SYNC OUT 동기화 신호 입력을 시키면
            // 레이저 펄스 개수를 지정할수있음
            //pen.PixelPulses = 20;
            //pen.IsPixelPulsesExit = true;


            Debug.Assert(rtc.IsMoF);
            var rtcMoF = rtc as IRtcMoF;
            Debug.Assert(rtcMoF != null);
            Debug.Assert(rtcMoF.EncXCountsPerMm != 0);
            //Debug.Assert(rtcMoF.EncYCountsPerMm != 0);

            // 만약 텀블와 같은 원통을 눕혀 옆면을 회전시키면서 MoF 가공하려고 한다면
            // 원통의 둘레 길이는 l = 반지름 * 2*Math.PI 이므로,
            // EncXCountsPerMm 값에 '엔코더 개수 / 원통 둘레 길이' 을 계산해 입력해 주고
            // MoF 대기 조건을 원통의 가장 높은 위치 (초점 위치) 가 되도록 ListMoFWait 함수를 이용한다.
            // rtcMoF.EncXCountsPerMm = 
        }

        void CreateEventhandler()
        {
            var document = siriusEditorControl1.Document;
            var marker = siriusEditorControl1.Marker;
            var view = siriusEditorControl1.View;
            var rtc = siriusEditorControl1.Scanner as IRtc;

            marker.OnBeforeRasterLine -= Marker_OnBeforeRasterLine;
            marker.OnBeforeRasterLine += Marker_OnBeforeRasterLine;
            marker.OnAfterRasterLine -= Marker_OnAfterRasterLine;
            marker.OnAfterRasterLine += Marker_OnAfterRasterLine;
        }

        private bool Marker_OnBeforeRasterLine(IMarker marker, IEntity entity, EntityPen.RasterDirections dir, RasterModes mode, double usec, DVec2 start, DVec2 pitch, uint arg8, ExtensionChannels channel)
        {
            bool success = true;
            var rtc = marker.Scanner as IRtc;
            var rtcMoF = rtc as IRtcMoF;
            var transformedStart = start.Transform(rtc.MatrixStack.ToResult); // 실제(행렬 스택이 모두 적용된) 시작 위치
            var transformedPitch = pitch.Transform(rtc.MatrixStack.ToResult); // 실제(행렬 스택이 모두 적용된) 픽셀 피치

            // 최대한 스캐너의 중심에서 수직 선분이 가공되도록 하는 조건 설정
            // start 의 X 값이 n 번째 줄(Raster)에 해당하므로, 스캐너의 중심 근처에 올때까지 대기한다.
            success &= rtcMoF.ListMoFWait(RtcEncoders.EncX, -transformedStart.X + transformedPitch.X, RtcEncoderWaitConditions.Under);

            return success;
        }

        private bool Marker_OnAfterRasterLine(IMarker marker, IEntity entity, EntityPen.RasterDirections dir, RasterModes mode)
        {
            bool success = true;
            var rtc = marker.Scanner as IRtc;
            var rtcMoF = rtc as IRtcMoF;

            // 다음 가공 시작 지점으로 미리 점프 시켜 놓을수도 있다
            //success &= rtc.ListJumpTo() ;

            return success;
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

            Debug.Assert(rtc.IsMoF);
            var rtcMoF = rtc as IRtcMoF;

            // Start simulated encoders as x= -1, y=0 mm/s
            rtcMoF.CtlMoFEncoderSpeed(-1, 0);
            // or
            // Edit 'Simulated x speed at MoF = -1' at propertygrid of scanner(RTC) page
            // and
            // Marker.Start
        }

        private void BtnStopEncoderSimulation_Click(object sender, EventArgs e)
        {
            var rtc = siriusEditorControl1.Scanner as IRtc;

            Debug.Assert(rtc.IsMoF);
            var rtcMoF = rtc as IRtcMoF;

            // Deactivated simulated encoders 
            rtcMoF.CtlMoFEncoderSpeed(0, 0);
            // Reset encoders
            rtcMoF.CtlMoFEncoderReset();
        }
    }
}
