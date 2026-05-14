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
    /// Marking On-The-Fly (MoF) with RTC Interrupt (Break point) demo
    /// MoF와 RTC 인터럽트(중단점)를 이용한 데모
    /// </summary>
    public partial class Form1 : Form
    {
        /// <summary>
        /// Form constructor
        /// 폼 생성자
        /// </summary>
        public Form1()
        {
            // Initialize SIRIUS3 library
            // SIRIUS3 라이브러리 초기화
            SpiralLab.Sirius3.Core.Initialize();

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

            // Attach button click events
            // 버튼 클릭 이벤트 연결
            this.btnCreateEntities.Click += BtnCreateEntities_Click;
            this.btnReferenceRun.Click += BtnReferenceRun_Click;
            this.btnStartStop.Click += BtnStartStop_Click;
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

            // MoF license check (optional)
            // MoF 라이선스가 있는지 여부 확인 (선택 사항)
            // Need to MoF at library option.
            //Core.License(out var licenseInfo);
            //Debug.Assert(licenseInfo.IsMoFLicensed);

            // Check if RTC card is ready for MoF(aka. Processing on the fly)
            // IRtcMoF 사용하기 위한 준비가 되었는지 여부 확인
            Debug.Assert(rtc.IsMoF);
            var rtcMoF = rtc as IRtcMoF;
            Debug.Assert(rtcMoF != null);
            // Register MoF related events
            // MoF 관련 이벤트 등록
            rtcMoF.OnEncoderSignalError += OnEncoderSignalError;
            rtcMoF.OnOutOfVirtualImageField += OnOutOfVirtualImageField;
            // Ensure encoder counts are configured in config.ini
            // 단위 mm 당 엔코더 펄스 수가 설정되어 있어야 함
            Debug.Assert(rtcMoF.EncXCountsPerMm != 0);
            Debug.Assert(rtcMoF.EncYCountsPerMm != 0);

            // Check if RTC card supports interrupts
            // IRtcInterrupt 사용하기 위해 RTC 카드가 지원하는지 여부 확인
            var rtcInterrupt = rtc as IRtcInterrupt;
            Debug.Assert(rtcInterrupt != null);
            rtcInterrupt.OnInterrupt -= RtcInterrupt_OnInterrupt;
            rtcInterrupt.OnInterrupt += RtcInterrupt_OnInterrupt;

            // Set encoder compensation table if needed
            // 엔코더 보정이 필요한 경우 보정 테이블 설정
            //rtcMoF.CtlMoFCompensateTable( ... );

            // Register devices to control
            // 컨트롤에 장치 등록
            siriusEditorControl1.RegisterDevices(rtc, laser, powerMeter, dInExt1, dInLaserPort, dOutExt1, dOutExt2, dOutLaserPort, marker);

            // Ready marker
            // 마커 준비
            marker.Ready(siriusEditorControl1.Document, siriusEditorControl1.View, rtc, laser, powerMeter);
        }

        /// <summary>
        /// Event handler for encoder signal errors
        /// 엔코더 신호 오류 이벤트 핸들러
        /// </summary>
        /// <param name="rtcMoF"></param>
        /// <param name="rtcMarkingInfo"></param>
        private void OnEncoderSignalError(IRtcMoF rtcMoF, IRtcMarkingInfo rtcMarkingInfo)
        {
            if (rtcMarkingInfo is Rtc6MarkingInfo rtc6MarkingInfo)
            {
                // For RTC6 card / RTC6 전용 에러 비트 처리
                if (rtc6MarkingInfo.Contains(Rtc6MarkingInfo.Bit.Signal1EncoderXTooShort)) { }
                if (rtc6MarkingInfo.Contains(Rtc6MarkingInfo.Bit.Signal1EncoderYTooShort)) { }
                if (rtc6MarkingInfo.Contains(Rtc6MarkingInfo.Bit.Signal2EncoderXTooShort)) { }
                if (rtc6MarkingInfo.Contains(Rtc6MarkingInfo.Bit.Signal2EncoderYTooShort)) { }
                if (rtc6MarkingInfo.Contains(Rtc6MarkingInfo.Bit.WrongSignalSequenceEncoderX)) { }
                if (rtc6MarkingInfo.Contains(Rtc6MarkingInfo.Bit.WrongSignalSequenceEncoderY)) { }
            }
            else if (rtcMarkingInfo is Rtc5MarkingInfo rtc5MarkingInfo)
            {
                // For RTC5 card / RTC5 전용 에러 비트 처리
                if (rtc5MarkingInfo.Contains(Rtc5MarkingInfo.Bit.Signal1EncoderXTooShort)) { }
                if (rtc5MarkingInfo.Contains(Rtc5MarkingInfo.Bit.Signal1EncoderYTooShort)) { }
                if (rtc5MarkingInfo.Contains(Rtc5MarkingInfo.Bit.Signal2EncoderXTooShort)) { }
                if (rtc5MarkingInfo.Contains(Rtc5MarkingInfo.Bit.Signal2EncoderYTooShort)) { }
                if (rtc5MarkingInfo.Contains(Rtc5MarkingInfo.Bit.WrongSignalSequenceEncoderX)) { }
                if (rtc5MarkingInfo.Contains(Rtc5MarkingInfo.Bit.WrongSignalSequenceEncoderY)) { }
            }
            else if (rtcMarkingInfo is Rtc4MarkingInfo rtc4MarkingInfo)
            {
                // For RTC4 card / RTC4는 미지원
            }

            this.Invoke(new MethodInvoker(() =>
            {
                MessageBox.Show(this, $"Encoder Signal Error: {rtcMarkingInfo.ToString()}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }));

        }

        /// <summary>
        /// Event handler for virtual image field overflow/underflow
        /// 가상 이미지 필드 범위 초과 오류 이벤트 핸들러
        /// </summary>
        /// <param name="rtcMoF"></param>
        /// <param name="rtcMarkingInfo"></param>
        private void OnOutOfVirtualImageField(IRtcMoF rtcMoF, IRtcMarkingInfo rtcMarkingInfo)
        {
            if (rtcMarkingInfo is Rtc6MarkingInfo rtc6MarkingInfo)
            {
                // For RTC6 card / RTC6 전용 범위 오류 처리
                if (rtc6MarkingInfo.Contains(Rtc6MarkingInfo.Bit.MoFOverflowInXDirection)) { }
                if (rtc6MarkingInfo.Contains(Rtc6MarkingInfo.Bit.MoFUnderflowInXDirection)) { }
                if (rtc6MarkingInfo.Contains(Rtc6MarkingInfo.Bit.MoFOverflowInYDirection)) { }
                if (rtc6MarkingInfo.Contains(Rtc6MarkingInfo.Bit.MoFUnderflowInYDirection)) { }
            }
            else if (rtcMarkingInfo is Rtc5MarkingInfo rtc5MarkingInfo)
            {
                // For RTC5 card / RTC5 전용 범위 오류 처리
                if (rtc5MarkingInfo.Contains(Rtc5MarkingInfo.Bit.MoFOverflowInXDirection)) { }
                if (rtc5MarkingInfo.Contains(Rtc5MarkingInfo.Bit.MoFUnderflowInXDirection)) { }
                if (rtc5MarkingInfo.Contains(Rtc5MarkingInfo.Bit.MoFOverflowInYDirection)) { }
                if (rtc5MarkingInfo.Contains(Rtc5MarkingInfo.Bit.MoFUnderflowInYDirection)) { }
            }
            else if (rtcMarkingInfo is Rtc4MarkingInfo rtc4MarkingInfo)
            {
                // For RTC4 card / RTC4는 가상 이미지 필드 미지원
            }

            this.Invoke(new MethodInvoker(() =>
            {
                MessageBox.Show(this, $"Out of Virtual Image Field: {rtcMarkingInfo.ToString()}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);                
            }));
        }

        /// <summary>
        /// Create test entities and attach event handlers
        /// 테스트용 엔티티 생성 및 이벤트 핸들러 연결
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCreateEntities_Click(object sender, EventArgs e)
        {
            var marker = siriusEditorControl1.Marker;
            if (marker.IsBusy)
                return;

            var rtc = siriusEditorControl1.Scanner as IRtc;
            var document = siriusEditorControl1.Document;
            document.ActNew();

            // Create random arcs
            // 랜덤 원호 생성
            var rnd = new Random((int)DateTime.Now.Ticks);
            for (int i = 0; i < 30; i++)
            {
                double r = rnd.NextDouble()* 2 + 0.5;
                var arc = EntityFactory.CreateArc(new DVec3(0, 0, 0), r, 0, 360);
                double tx = rnd.NextDouble() * 80.0 - 40.0;
                double ty = rnd.NextDouble() * 80.0 - 40.0;
                arc.Translate(tx, ty, 0);
               
                document.ActAdd(arc);
            }

            // Register marker internal events for MoF synchronization
            // MoF 동기화를 위한 마커 내부 이벤트 등록
            marker.OnBeforeEntity -= Marker_OnBeforeEntity;
            marker.OnBeforeEntity += Marker_OnBeforeEntity;

            marker.OnAfterEntity -= Marker_OnAfterEntity;
            marker.OnAfterEntity += Marker_OnAfterEntity;

            siriusEditorControl1.View?.DoRender();
        }

        /// <summary>
        /// Stage Reference Run (Simulated)
        /// 스테이지 원점 복귀 (시뮬레이션)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            // 가동 완료 후 x,y 엔코더를 0,0으로 리셋
            var rtc = siriusEditorControl1.Scanner as IRtc;
            var rtcMoF = rtc as IRtcMoF;
            rtcMoF.CtlMoFEncoderReset(0, 0);
        }

        /// <summary>
        /// Start or Stop marking process
        /// 마킹 시작 또는 중지
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnStartStop_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            var marker = siriusEditorControl1.Marker;

            if (marker.IsBusy)
            {
                // Stop marking
                // 마킹 중지
                marker.Stop();
                marker.Reset();
            }
            else
            {
                // Must be moved xy stage center as scanner center before start mark !
                // 시작 전 XY 스테이지가 스캐너 중심에 위치해야 합니다.

                marker.Reset();
                marker.Ready(siriusEditorControl1.Document);
                marker.Start(document.Page); // current page
            }
        }

        /// <summary>
        /// Called before marking each entity to synchronize with moving stage
        /// 각 엔티티 마킹 전 호출 (이동 중인 스테이지와의 동기화 로직)
        /// </summary>
        /// <param name="marker"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        private bool Marker_OnBeforeEntity(IMarker marker, IEntity entity)
        {
            bool success = true;
            var rtc = marker.Scanner as IRtc;
            var rtcMoF = rtc as IRtcMoF;
            var rtcInterrupt = rtc as IRtcInterrupt;

            if (entity is EntityArc entityArc)
            {
                if (entityArc.CalcuateRealMinMax(out var realMin, out var realMax))
                {
                    var realCenter = (realMin + realMax) * 0.5;
                    // Create breakpoint before arc using entity ID
                    // 원호 가공 전 엔티티 ID를 사용하여 중단점(BreakPoint) 생성
                    success &= rtcInterrupt.ListBreakPoint(entityArc.Id);

                    // Assume scan head is fixed and XY stage is moving
                    // As X stage moves in X+ direction, RTC encoder value increases in X+ direction
                    // As Y stage moves in Y+ direction, RTC encoder value increases in Y+ direction
                    // Wait until the stage moves near the center of the entity
                    // To move to the center position of the entity, a move of -cx, -cy from the origin is required
                    // Wait until it comes within -1 ~ 1 mm range
                    // 스캔 헤드는 고정이고 X,Y 스테이지가 이동한다고 가정
                    // X 스테이지가 X+ 방향으로 이동하면 스캔 헤드(RTC) 의 엔코더는 X+ 값 방향으로 증가함
                    // Y 스테이지가 Y+ 방향으로 이동하면 스캔 헤드(RTC) 의 엔코더는 Y+ 값 방향으로 증가함
                    // 해당 개체의 중심 근처에 스테이지가 이동할때 까지 대기
                    // 해당 개체의 중심 위치로 이동하기 위해서는 원점중심에서 -cx, -cy 만큼 이동필요
                    // -1 ~ 1 mm 범위 내에 들어올 때 까지 대기
                    DVec2 threshold = new DVec2(1, 1); 
                    // MoF wait by coordinate range
                    // 지정된 범위 내로 엔코더 값이 들어올 때까지 대기
                    success &= rtcMoF.ListMoFWaitRange(-realCenter.Xy - threshold, -realCenter.Xy + threshold);

                    // MoF begin (false: no encoder reset)
                    // MoF 시작 (false: 엔코더 리셋 없음)
                    success &= rtcMoF.ListMoFBegin(false);
                }
            }
            return success;
        }

        /// <summary>
        /// Called after marking each entity
        /// 각 엔티티 마킹 후 호출 (MoF 종료)
        /// </summary>
        /// <param name="marker"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        private bool Marker_OnAfterEntity(IMarker marker, IEntity entity)
        {
            bool success = true;
            var rtc = marker.Scanner as IRtc;
            var rtcMoF = rtc as IRtcMoF;

            if (entity is EntityArc entityArc)
            {
                // MoF end tracking
                // MoF 추적 종료
                success &= rtcMoF.ListMoFEnd(DVec2.Zero);
            }
            return success;
        }

        /// <summary>
        /// Event handler called when RTC list execution reaches a break point
        /// RTC 리스트 실행 중 중단점에 도달했을 때 호출되는 이벤트 핸들러
        /// </summary>
        /// <param name="rtcInterrupt"></param>
        /// <param name="waitID"></param>
        /// <returns></returns>
        private bool RtcInterrupt_OnInterrupt(IRtcInterrupt rtcInterrupt, long waitID)
        {
            // Note: RTC list execution is currently paused!
            // 참고: 현재 RTC 리스트 실행이 중단된 상태입니다!
            var document = siriusEditorControl1.Document;
            // Find entity by the break point ID
            // 중단점 ID로 엔티티 검색
            if (document.FindById(waitID, out var foundedEntity))
            {
                if (foundedEntity is EntityArc entityArc)
                { 
                    if (entityArc.CalcuateRealMinMax(out var min, out var max))
                    {
                        var realCenter = (min + max) * 0.5;
                        // Move your external stage to the entity's real center
                        // 스테이지를 해당 엔티티의 실제 중심 위치로 이동
                        // Stage.Move(realCenter.X, realCenter.Y);

                        Thread.Sleep(1_000); // Simulated work delay / 가공 전 작업 지연 시뮬레이션

                        // Return 'True' to resume list execution
                        // 'True' 리턴 시 가공 리스트 실행 재개
                        return true; 
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
