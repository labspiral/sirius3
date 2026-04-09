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

            this.btnCreateEntities.Click += BtnCreateEntities_Click;
            this.btnReferenceRun.Click += BtnReferenceRun_Click;
            this.btnStartStop.Click += BtnStartStop_Click;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            EditorHelper.CreateDevices(out IRtc rtc, out ILaser laser, out IDInput dInExt1, out IDInput dInLaserPort, out IDOutput dOutExt1, out IDOutput dOutExt2, out IDOutput dOutLaserPort, out IPowerMeter powerMeter, out IMarker marker);

            // MoF 라이센스가 있는지 여부
            // Need to MoF at library option.
            //Core.License(out var licenseInfo);
            //Debug.Assert(licenseInfo.IsMoFLicensed);

            // IRtcMoF 사용하기 위한 준비가 되었는지 여부 확인
            Debug.Assert(rtc.IsMoF);
            var rtcMoF = rtc as IRtcMoF;
            Debug.Assert(rtcMoF != null);
            rtcMoF.OnEncoderSignalError += OnEncoderSignalError;
            rtcMoF.OnOutOfVirtualImageField += OnOutOfVirtualImageField;
            Debug.Assert(rtcMoF.EncXCountsPerMm != 0);
            Debug.Assert(rtcMoF.EncYCountsPerMm != 0);

            // IRtcInterrupt 사용하기 위해 RTC 카드가 지원하는지 여부 확인
            var rtcInterrupt = rtc as IRtcInterrupt;
            Debug.Assert(rtcInterrupt != null);
            rtcInterrupt.OnInterrupt -= RtcInterrupt_OnInterrupt;
            rtcInterrupt.OnInterrupt += RtcInterrupt_OnInterrupt;


            // 엔코더 보정이 필요한 경우 보정 테이블 설정
            //rtcMoF.CtlMoFCompensateTable( ... );


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

        private void BtnCreateEntities_Click(object sender, EventArgs e)
        {
            var marker = siriusEditorControl1.Marker;
            if (marker.IsBusy)
                return;

            var rtc = siriusEditorControl1.Scanner as IRtc;
            var document = siriusEditorControl1.Document;
            document.ActNew();

            var rnd = new Random((int)DateTime.Now.Ticks);
            for (int i = 0; i < 30; i++)
            {
                double r = rnd.NextDouble()* 2 + 0.5;
                var arc = EntityFactory.CreateArc(new DVec3(0, 0, 0), r, 0, 360);
                double tx = rnd.NextDouble() * 80.0 - 40.0;
                double ty = rnd.NextDouble() * 80.0 - 40.0;
                arc.Translate(tx, ty, 0);
               
                document.ActivePage?.ActiveLayer?.AddChild(arc);
            }

            marker.OnBeforeEntity -= Marker_OnBeforeEntity;
            marker.OnBeforeEntity += Marker_OnBeforeEntity;

            marker.OnAfterEntity -= Marker_OnAfterEntity;
            marker.OnAfterEntity += Marker_OnAfterEntity;

            siriusEditorControl1.View?.DoRender();
        }

        private void BtnReferenceRun_Click(object sender, EventArgs e)
        {
            var marker = siriusEditorControl1.Marker;
            if (marker.IsBusy)
                return;

            // Must be moved xy stage center as scanner center before start mark !
            // REFERENCE RUN 
            // Stage.Move(0, 0); // Move stage to origin (scanner center)
            // ...

            Thread.Sleep(1_000);

            // WAIT FOR DONE and then reset x,y encoders as 0,0
            var rtc = siriusEditorControl1.Scanner as IRtc;
            var rtcMoF = rtc as IRtcMoF;
            rtcMoF.CtlMoFEncoderReset(0, 0);
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
                // Must be moved xy stage center as scanner center before start mark !

                marker.Reset();
                marker.Ready(siriusEditorControl1.Document);
                marker.Start(document.Page); // current page
            }
        }

        private bool Marker_OnBeforeEntity(IMarker marker, IEntity entity)
        {
            bool success = true;
            var rtc = marker.Rtc;
            var rtcMoF = rtc as IRtcMoF;
            var rtcInterrupt = rtc as IRtcInterrupt;

            if (entity is EntityArc entityArc)
            {
                if (entityArc.CalcuateRealMinMax(out var realMin, out var realMax))
                {
                    var realCenter = (realMin + realMax) * 0.5;
                    // breakpoint entity before arc with entity.Id
                    success &= rtcInterrupt.ListBreakPoint(entityArc.Id);

                    // 스캔 헤드는 고정이고 X,Y 스테이지가 이동한다고 가정
                    // X 스테이지가 X+ 방향으로 이동하면 스캔 헤드(RTC) 의 엔코더는 X+ 값 방향으로 증가함
                    // Y 스테이지가 Y+ 방향으로 이동하면 스캔 헤드(RTC) 의 엔코더는 Y+ 값 방향으로 증가함
                    // 해당 개체의 중심 근처에 스테이지가 이동할때 까지 대기
                    // 해당 개체의 중심 위치로 이동하기 위해서는 원점중심에서 -cx, -cy 만큼 이동필요
                    // -1 ~ 1 mm 범위 내에 들어올 때 까지 대기
                    DVec2 threshold = new DVec2(1, 1); 
                    // MoF wait by range
                    success &= rtcMoF.ListMoFWaitRange(-realCenter.Xy - threshold, -realCenter.Xy + threshold);

                    // MoF begin with no encoder reset
                    success &= rtcMoF.ListMoFBegin(false);
                }
            }
            return success;
        }

        private bool Marker_OnAfterEntity(IMarker marker, IEntity entity)
        {
            bool success = true;
            var rtc = marker.Rtc;
            var rtcMoF = rtc as IRtcMoF;

            if (entity is EntityArc entityArc)
            {
                // MoF end
                success &= rtcMoF.ListMoFEnd(DVec2.Zero);
            }
            return success;
        }

        private bool RtcInterrupt_OnInterrupt(IRtcInterrupt rtcInterrupt, long waitID)
        {
            // RTC list is excuting but paused !
            var document = siriusEditorControl1.Document;
            if (document.FindById(waitID, out var foundedEntity))
            {
                if (foundedEntity is EntityArc entityArc)
                { 
                    if (entityArc.CalcuateRealMinMax(out var min, out var max))
                    {
                        var realCenter = (min + max) * 0.5;
                        // Move your stage to realCenter
                        // Stage.Move(realCenter.X, realCenter.Y);

                        Thread.Sleep(1_000);

                        return true; // 'True' means resume list executing 
                    }
                }
            }

            this.BeginInvoke(new MethodInvoker(() =>
            {
                MessageBox.Show(this, $"Invalid interrupt ? Wait id: {waitID}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }));
            return false; 
        }
    }
}
