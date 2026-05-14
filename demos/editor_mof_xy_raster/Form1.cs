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
    /// <summary>
    /// Marking On-The-Fly (MoF) with Raster image demo
    /// 이미지 래스터 마킹을 포함한 MoF(이동 중 가공) 데모
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
                    // is marker need to abort ?
                    // 마킹 중단 여부 확인 ?
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
            this.btnCreateEntities_Eventhandler.Click += BtnCreateEntities_Eventhandler_Click;
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
            // Register MoF related event handlers
            // MoF 관련 이벤트 핸들러 등록
            rtcMoF.OnEncoderSignalError += OnEncoderSignalError;
            rtcMoF.OnOutOfVirtualImageField += OnOutOfVirtualImageField;

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
            // Analyze error bits depending on RTC card version
            // RTC 버전에 따른 에러 비트 분석 및 처리
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
            // Analyze out-of-range status bits
            // 범위 초과 상태 비트 분석
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
        /// Create test entities and initialize event handlers
        /// 테스트용 엔티티 생성 및 이벤트 핸들러 초기화
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCreateEntities_Eventhandler_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            document.ActNew();

            // Create sample entities
            // 샘플 엔티티 생성
            CreateEntities();

            // Initialize marker event handlers for synchronization
            // 동기화를 위한 마커 이벤트 핸들러 초기화
            CreateEventhandler();
        }

        /// <summary>
        /// Create entities (Image with MoF control)
        /// 엔티티 생성 (MoF 제어를 포함한 이미지)
        /// </summary>
        void CreateEntities()
        {
            var document = siriusEditorControl1.Document;
            var marker = siriusEditorControl1.Marker;
            var view = siriusEditorControl1.View;
            var rtc = siriusEditorControl1.Scanner as IRtc;

            // Create mof begin 
            // with encoder reset
            // 외부 엔코더 추종을 시작하고 현재 위치를 0으로 리셋하는 MoF 시작 객체 추가

            /*      
             * Layout Diagram / 가공 레이아웃 구성:
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
             *  <= MOVING DIRECTION / 객체 이동 방향
             *  
             */

            var mofBegin = EntityFactory.CreateMoFBegin(RtcMoFModes.XY, true);
            document.ActAdd(mofBegin);

            // Load and create image entity
            // 이미지 로드 및 엔티티 생성
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sample\\image\\imagekorea.jpg");
            if (!File.Exists(fileName)) return;
            var image = EntityFactory.CreateImage(fileName, 20);
            var penColor = SpiralLab.Sirius3.UI.Config.EntityPenColors[0]; // White / 흰색
            image.PenColor = penColor;
            image.AlignmentXY = AlignmentXYs.MiddleLeft;
            image.MajorColor = MajorColors.Black;
            document.ActAdd(image);
            
            // Translate for starting position offset
            // 가공 시작 위치를 위해 +1mm 이동
            image.Translate(1, 0); 

            // Create MoF End entity with jump back to origin
            // 외부 엔코더 추종을 중단하고 원점으로 이동하는 MoF 종료 객체 추가
            var mofEnd = EntityFactory.CreateMoFEnd(DVec2.Zero);
            document.ActAdd(mofEnd);

            siriusEditorControl1.View?.DoRender();

            // Configure pen for Jump and Shoot raster mode
            // 점프 및 가공(Jump and Shoot) 래스터 모드를 위한 펜 설정
            document.FindByEntityPenColor(penColor, out var pen);
            // pen.Frequency = ...
            // pen.Power = ...
            // pen.JumpSpeed = 1_000;
            // pen.MarkSpeed = 1_000;

            pen.RasterDirection = EntityPen.RasterDirections.Vertical;
            pen.RasterMode = RasterModes.JumpAndShoot;
            pen.IsRasterZigZag = true;
            pen.PixelTime = 100; // 100usec

            // Check if RTC card is ready for MoF(aka. Processing on the fly)
            // IRtcMoF 사용하기 위한 준비가 되었는지 여부 확인
            Debug.Assert(rtc.IsMoF);
            var rtcMoF = rtc as IRtcMoF;
            Debug.Assert(rtcMoF != null);
            // Ensure encoder counts per mm is configured
            // 단위 mm 당 엔코더 펄스 수가 설정되어 있어야 함
            Debug.Assert(rtcMoF.EncXCountsPerMm != 0);

            // Tip: For rotary MoF (e.g., cylinder), set EncXCountsPerMm = (Total Pulses) / (Circumference)
            // 참고: 원통형 회전 가공 시, EncXCountsPerMm = (총 펄스 수) / (둘레 길이) 로 설정하십시오.
        }

        /// <summary>
        /// Register raster line event handlers for synchronization
        /// 동기화를 위한 래스터 라인 이벤트 핸들러 등록
        /// </summary>
        void CreateEventhandler()
        {
            var document = siriusEditorControl1.Document;
            var marker = siriusEditorControl1.Marker;

            // Hook events for fine-grained control over each raster line
            // 개별 래스터 라인에 대한 정밀 제어를 위해 이벤트 연결
            marker.OnBeforeRasterLine -= Marker_OnBeforeRasterLine;
            marker.OnBeforeRasterLine += Marker_OnBeforeRasterLine;
            marker.OnAfterRasterLine -= Marker_OnAfterRasterLine;
            marker.OnAfterRasterLine += Marker_OnAfterRasterLine;
        }

        /// <summary>
        /// Called before marking each vertical raster line
        /// 각 수직 래스터 라인 가공 전 호출 (스캐너 중심 동기화 로직)
        /// </summary>
        /// <param name="marker"></param>
        /// <param name="entity"></param>
        /// <param name="dir"></param>
        /// <param name="mode"></param>
        /// <param name="usec"></param>
        /// <param name="start"></param>
        /// <param name="pitch"></param>
        /// <param name="arg8"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        private bool Marker_OnBeforeRasterLine(IMarker marker, IEntity entity, EntityPen.RasterDirections dir, RasterModes mode, double usec, DVec2 start, DVec2 pitch, uint arg8, ExtensionChannels channel)
        {
            bool success = true;
            var rtc = marker.Scanner as IRtc;
            var rtcMoF = rtc as IRtcMoF;
            // Calculate transformed position considering the full matrix stack
            // 행렬 스택이 적용된 실제 시작 위치 및 픽셀 피치 계산
            var transformedStart = start.Transform(rtc.MatrixStack.ToResult); 
            var transformedPitch = pitch.Transform(rtc.MatrixStack.ToResult); 

            // Wait until the current raster line reaches the scanner's center area
            // 현재 래스터 라인이 스캐너 중심 근처에 도달할 때까지 대기
            // This ensures optimal beam quality at the center of the field
            // 최대한 스캐너의 중심에서 수직 선분이 가공되도록 대기 조건을 설정합니다.
            success &= rtcMoF.ListMoFWait(RtcEncoders.EncX, -transformedStart.X + transformedPitch.X, RtcEncoderWaitConditions.Under);

            return success;
        }

        /// <summary>
        /// Called after marking each raster line
        /// 각 래스터 라인 가공 후 호출
        /// </summary>
        /// <param name="marker"></param>
        /// <param name="entity"></param>
        /// <param name="dir"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        private bool Marker_OnAfterRasterLine(IMarker marker, IEntity entity, EntityPen.RasterDirections dir, RasterModes mode)
        {
            bool success = true;
            var rtc = marker.Scanner as IRtc;
            var rtcMoF = rtc as IRtcMoF;

            // Optional: Pre-jump to the next marking start position
            // 다음 가공 시작 지점으로 미리 점프하여 효율을 높일 수 있습니다.
            // success &= rtc.ListJumpTo(...) ;

            return success;
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
                // Force stop marking process
                // 가공 중이면 강제 중단
                marker.Stop();
                marker.Reset();
            }
            else
            {
                marker.Reset();
                marker.Ready(siriusEditorControl1.Document);
                // Start marking current page
                // 현재 페이지 마킹 시작
                marker.Start(document.Page); 
            }
        }

        /// <summary>
        /// Toggle simulated encoder movement for testing
        /// 엔코더 시뮬레이션(가상 이동) 시작
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnStartEncoderSimulation_Click(object sender, EventArgs e)
        {
            var rtc = siriusEditorControl1.Scanner as IRtc;
            if (rtc.IsBusy)
                return;

            // Check if RTC card is ready for MoF(aka. Processing on the fly)
            // IRtcMoF 사용하기 위한 준비가 되었는지 여부 확인
            Debug.Assert(rtc.IsMoF);
            var rtcMoF = rtc as IRtcMoF;

            // Start simulated X movement at -1mm/s
            // X 축 방향 -1mm/s 가상 속도로 엔코더 시뮬레이션 시작
            rtcMoF.CtlMoFEncoderSpeed(-1, 0);
        }

        /// <summary>
        /// Stop simulated encoder and reset positions
        /// 엔코더 시뮬레이션 중지 및 위치 리셋
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnStopEncoderSimulation_Click(object sender, EventArgs e)
        {
            var rtc = siriusEditorControl1.Scanner as IRtc;

            // Check if RTC card is ready for MoF(aka. Processing on the fly)
            // IRtcMoF 사용하기 위한 준비가 되었는지 여부 확인
            Debug.Assert(rtc.IsMoF);
            var rtcMoF = rtc as IRtcMoF;

            // Stop simulated movement (set speed to 0)
            // 가상 속도를 0으로 하여 엔코더 시뮬레이션 중지
            rtcMoF.CtlMoFEncoderSpeed(0, 0);
            // Reset encoders to origin
            // 엔코더 위치 리셋
            rtcMoF.CtlMoFEncoderReset();
        }
    }
}
