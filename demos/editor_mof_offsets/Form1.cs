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
    /// Marking On-The-Fly (MoF) with multiple offsets demo (Conveyor style)
    /// 다중 오프셋을 이용한 MoF(이동 중 가공) 데모 (컨베이어 방식)
    /// </summary>
    public partial class Form1 : Form
    {
        // Number of offsets to generate for test
        // 생성할 테스트 오프셋 개수
        const int offsetCounts = 1_000; 

        // Processed count (using free variable)
        // 가공 완료 개수 (자유 변수 사용)
        uint processCounts = 0;

        /// <summary>
        /// Form constructor
        /// 폼 생성자
        /// </summary>
        public Form1()
        {
            InitializeComponent();

            // Form load event
            // 폼 로드 이벤트
            this.Load += Form1_Load;

            this.FormClosing += (s, e) =>
            {
                // Confirm before closing
                // 종료 전 사용자 확인
                var dlgResult = MessageBox.Show(this, $"Do you really want to terminate program ?", "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dlgResult != DialogResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }

                // Dispose hardware devices
                // 장치 리소스 해제
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
            this.btnStartStop.Click += BtnStartStop_Click;
            this.btnSimulateEncoder.Click += BtnSimulateEncoder_Click;
            this.btnResetEncoder.Click += BtnResetEncoder_Click;
        }

        /// <summary>
        /// Form load
        /// 폼 로드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            // Create hardware devices (RTC, Laser, IO, PowerMeter, Marker)
            // 하드웨어 장치 생성
            EditorHelper.CreateDevices(out IRtc rtc, out ILaser laser, out IDInput dInExt1, out IDInput dInLaserPort, out IDOutput dOutExt1, out IDOutput dOutExt2, out IDOutput dOutLaserPort, out IPowerMeter powerMeter, out IMarker marker);

            // MoF(aka. Processing on the fly) option must be enabled on the RTC card 
            // RTC 카드에 MoF 옵션 활성화 여부 확인
            Debug.Assert(rtc.IsMoF);
            var rtcMoF = rtc as IRtcMoF;
            Debug.Assert(rtcMoF != null);

            // Register devices to control
            // 컨트롤에 장치 등록
            siriusEditorControl1.RegisterDevices(rtc, laser, powerMeter, dInExt1, dInLaserPort, dOutExt1, dOutExt2, dOutLaserPort, marker);

            // Register event: free variable change (used as process counter)
            // 자유 변수 변경 이벤트 등록 (가공 카운트용)
            if (rtc is IRtcFreeVariable rtcFreeVariable)
                rtcFreeVariable.OnFreeVariableChanged += OnFreeVariableChanged;

            // Encoder error event
            // 엔코더 에러 이벤트
            rtcMoF.OnEncoderSignalError += OnEncoderSignalError;

            // Virtual field overflow/underflow event
            // 가상 영역 벗어남 이벤트
            rtcMoF.OnOutOfVirtualImageField += OnOutOfVirtualImageField;

            // Ready marker with initial document and view
            // 마커 준비
            marker.Ready(siriusEditorControl1.Document, siriusEditorControl1.View, rtc, laser, powerMeter);
        }

        /// <summary>
        /// Event handler for RTC free variable change
        /// 자유 변수 변경 이벤트 핸들러
        /// </summary>
        /// <param name="rtcFreeVariable"></param>
        /// <param name="no"></param>
        /// <param name="data"></param>
        private void OnFreeVariableChanged(IRtcFreeVariable rtcFreeVariable, uint no, uint data)
        {
            // Update processed count in UI
            // UI에 가공 카운트 실시간 업데이트
            if (nudCounts.InvokeRequired)
            {
                nudCounts.BeginInvoke((MethodInvoker)(() => nudCounts.Value = data));
            }
            else
            {
                nudCounts.Value = data;
            }
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
                // RTC6 specific error handling / RTC6 전용 에러 처리
            }
            else if (rtcMarkingInfo is Rtc5MarkingInfo rtc5MarkingInfo)
            {
                // RTC5 specific error handling / RTC5 전용 에러 처리
            }

            this.Invoke(new MethodInvoker(() =>
            {
                MessageBox.Show(this, $"Encoder Signal Error: {rtcMarkingInfo.ToString()}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }));
        }

        /// <summary>
        /// Event handler for virtual field range errors
        /// 가상 영역 경계 오류 이벤트 핸들러
        /// </summary>
        /// <param name="rtcMoF"></param>
        /// <param name="rtcMarkingInfo"></param>
        private void OnOutOfVirtualImageField(IRtcMoF rtcMoF, IRtcMarkingInfo rtcMarkingInfo)
        {
            var rtc = rtcMoF as IRtc;
            // Force stop marking on virtual field error
            // 가상 영역 오류 발생 시 강제 정지
            rtc.CtlAbort();

            this.Invoke(new MethodInvoker(() =>
            {
                MessageBox.Show(this, $"Out of Virtual Image Field: {rtcMarkingInfo.ToString()}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }));
        }

        /// <summary>
        /// Create test entities and initialize offsets
        /// 테스트 엔티티 생성 및 오프셋 초기화
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCreateEntities_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            document.ActNew();

            // Create sample entities
            // 샘플 엔티티 생성
            CreateEntities();

            // Apply pen parameter values
            // 펜 파라메터 값 설정
            EditPenValues();
        }

        /// <summary>
        /// Create entities and generate multiple offsets
        /// 엔티티 생성 및 다중 오프셋 생성
        /// </summary>
        void CreateEntities()
        {
            var document = siriusEditorControl1.Document;
            var marker = siriusEditorControl1.Marker;
            var rtc = siriusEditorControl1.Scanner as IRtc;

            // Check if RTC card is ready for MoF(aka. Processing on the fly)
            // IRtcMoF 사용하기 위한 준비가 되었는지 여부 확인
            Debug.Assert(rtc.IsMoF);

            // Create a small arc as target entity
            // 가공 대상인 작은 원 생성
            var entity = EntityFactory.CreateArc(DVec2.Zero, 0.2, 0, 360);
            entity.PenColor = Color.White;
            document.ActAdd(entity);

            siriusEditorControl1.View?.DoRender();

            var rtcMoF = rtc as IRtcMoF;
            Debug.Assert(rtcMoF != null);
            Debug.Assert(rtcMoF.EncXCountsPerMm != 0);

            var rnd = new Random((int)DateTime.Now.Ticks);
   
            // Generate multiple random offsets for conveyor simulation
            // 컨베이어 시뮬레이션을 위한 다중 랜덤 오프셋 생성
            var offsets = new List<Offset>(offsetCounts);
            for (int i = 0; i < offsetCounts; i++)
            {
                // X: generated in -200mm ~ 0 range (left side space) / X: -200~0mm 범위(좌측 공간) 생성
                // Y: use -25% ~ 25% of FOV / Y: FOV의 25% 이내 사용
                offsets.Add(new Offset(rnd.NextDouble() * 199.0 - 200.0, (rnd.NextDouble() * rtc.EffectiveFieldSize.Y  - rtc.EffectiveFieldSize.Y / 2.0) * 0.5));
            }

            // Sort offsets by X descending (important for conveyor direction logic)
            // X 기준 내림차순 정렬 (컨베이어 이동 방향 추종을 위해 중요)
            marker.Offsets = offsets
                .OrderByDescending(o => o.Translate.X)
                .ToArray();

            // Register marker internal event handlers
            // 마커 내부 이벤트 핸들러 등록
            marker.OnBeforeEntity -= Marker_OnBeforeEntity;
            marker.OnBeforeEntity += Marker_OnBeforeEntity;

            marker.OnAfterEntity -= Marker_OnAfterEntity;
            marker.OnAfterEntity += Marker_OnAfterEntity;

            // Zoom to fit for target entity
            // 개체에 맞춰 화면 줌
            siriusEditorControl1.View?.ActiveCamera?.ZoomFit(siriusEditorControl1.View, entity );
            siriusEditorControl1.View?.DoRender();
        }

        /// <summary>
        /// Configure pen parameters for MoF
        /// MoF를 위한 펜 파라메터 설정
        /// </summary>
        void EditPenValues()
        {
            var document = siriusEditorControl1.Document;
            var layerPenColor = SpiralLab.Sirius3.UI.Config.LayerPenColors[0]; // Color.White
            document.FindByLayerPenColor(layerPenColor, out var layerPen);

            // SDC (Spot Distance Control) and SCANAhead configuration for excelliSCAN
            // excelliSCAN 헤드 및 SDC(등간격 제어) 설정
            layerPen.IsALC = true; 
            // Actual velocity + spot distance control
            layerPen.AlcByPositionTable.Clear();
            layerPen.AlcSignal = AutoLaserControlSignals.SpotDistance; //RTC6 + SCANAhead
            layerPen.AlcMode = AutoLaserControlModes.ActualVelocity;
            layerPen.AlcModeExtension.Clear();

            // excelliSCAN 헤드 및 SDC(등간격 제어) 사용 시 필수.
            // Tracking Error 대신 프리뷰 타임(Preview Time)을 기반으로 제어.
            layerPen.AlcModeExtension.Add(AutoLaserControlModeExtensions.Bit.SCANAhead);

            // Sky Writing 중 SDC 유지. 스카이 라이팅(가감속 구간) 동작 중에도 SDC 알고리즘을 유지.
            // 벡터의 시작과 끝부분에서도 펄스 간격을 정밀하게 유지합니다. SCANahead 활성화가 필요. (RTC6 전용).
            layerPen.AlcModeExtension.Add(AutoLaserControlModeExtensions.Bit.SkyWritingSDC); 

            // 엔코더 속도 합산. 스캐너 속도에 엔코더 속도를 벡터 합산.
            // 이동하는 물체를 가공하는 MoF(Marking On-the-Fly) 공정에서 사용.
            //layerPen.AlcModeExtension.Add(AutoLaserControlModeExtensions.Bit.EncoderSpeedAddition); 

            // 역 속도 보정. F-Theta 렌즈 왜곡으로 인해 발생하는 위치별 선속도 차이를 보정.
            // 보정 테이블을 사용하여 각속도를 실제 필드 상의 속도로 변환.
            //layerPen.AlcModeExtension.Add(AutoLaserControlModeExtensions.Bit.InverseSpeedCorrection); 

            // 역 좌표 변환. 좌표 변환(회전, 행렬 등)이 적용된 경우, 피드백 속도를 역변환.
            // 레이저 제어가 변환 전의 원본 도면 속도를 기준으로 수행되도록 보장. (RTC6 전용)
            //layerPen.AlcModeExtension.Add(AutoLaserControlModeExtensions.Bit.BackwardTransformation); 

            // Find entity pen for 'White'
            var entityPenColor = SpiralLab.Sirius3.UI.Config.EntityPenColors[0]; // Color.White
            document.FindByEntityPenColor(entityPenColor, out var entityPen);
            entityPen.JumpSpeed = 2_000;
            entityPen.MarkSpeed = 1_000;
            // Spot Distance
            var spotDistance = 0.01; //10 um
            entityPen.SpotDistanceSCANa = spotDistance;
        }

        /// <summary>
        /// Called before each entity marking in the list
        /// 각 엔티티 마킹 전 호출 (MoF 시작 및 대기 로직)
        /// </summary>
        /// <param name="marker"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        private bool Marker_OnBeforeEntity(IMarker marker, IEntity entity)
        {
            var rtc = marker.Scanner as IRtc;
            var rtcMoF = rtc as IRtcMoF;
            Debug.Assert(rtcMoF != null);

            bool success = true;

            // Start MoF at the first offset index
            // 첫 번째 오프셋 인덱스에서 MoF 시작
            if (0 == marker.WorkingSet.OffsetIndex)
            {
                // MoF begin (false: no encoder reset) / MoF 시작 (false: 엔코더 리셋 없음)
                success &= rtcMoF.ListMoFBegin(false);
            }

            if (success)
            {
                if (entity is ITransformable transformable)
                {
                    if (transformable.CalculateRealMinMax(out var realMin, out var realMax))
                    {
                        var offset = marker.WorkingSet.Offset.Translate;
                        var originXShift = -1; // origin shift for wait condition / 대기 조건을 위한 원점 시프트

                        // Wait until the object reaches the scanner center area
                        // 객체가 스캐너 중심 영역에 도달할 때까지 대기
                        // Wait condition: EncX > -(Max.X + Offset.X)
                        // 가공 데이타는 좌측 공간(X: -200mm ~ 0) 에 있고, 스테이지(혹은 컨베이어)가 좌->우 지속적으로 이동중(스캐너 입장에서는 엔코더가 X+ 방향으로 증가) 
                        // 최종 대기 위치는 개체의 최대 우측값(Max. x) - Offset 이 되어야 한다.
                        // '최종 대기 위치 = 개체의 우측 끝단(Max. x)이 스캐너 중심에 도달' 했다는 의미.
                        success &= rtcMoF.ListMoFWait(RtcEncoders.EncX, -(realMax.X + offset.X) + originXShift, RtcEncoderWaitConditions.Over);
                        // 범위로 기다리기
                        //// or 대기 범위를 지정해 처리도 가능. (주의. 대기 범위내에 들어올떄까지 무한히 대기하게됨)
                        //success &= rtcMoF.ListMoFWaitRange(-(realMin.Xy + offset.Xy), -(realMax.Xy + offset.Xy));
                    }
                }
            }
            return success;
        }

        /// <summary>
        /// Called after each entity marking in the list
        /// 각 엔티티 마킹 후 호출 (카운트 업데이트 및 MoF 종료)
        /// </summary>
        /// <param name="marker"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        private bool Marker_OnAfterEntity(IMarker marker, IEntity entity)
        {
            var rtc = marker.Scanner as IRtc;
            var rtcMoF = rtc as IRtcMoF;
            var rtcFreeVariable = rtc as IRtcFreeVariable;
            Debug.Assert(rtcMoF != null);

            bool success = true;

            // Increase process count and write to free variable
            // 가공 카운트 증가 및 자유 변수에 기록
            processCounts++;
            success &= rtcFreeVariable.ListWriteVariable(0, processCounts);

            // End MoF tracking after processing the last offset
            // 마지막 오프셋 가공 후 MoF 추적 종료
            if (marker.WorkingSet.OffsetIndex == (marker.Offsets.Length - 1))
            {
                // MoF end and jump to origin / MoF 종료 및 원점으로 이동
                success &= rtcMoF.ListMoFEnd(DVec2.Zero);
            }
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
            var rtc = siriusEditorControl1.Scanner as IRtc;
            var rtcFreeVariable = rtc as IRtcFreeVariable;
            
            Debug.Assert(rtc.IsMoF);
            var rtcMoF = rtc as IRtcMoF;

            // Ensure only single layer is used for this MoF strategy
            // 이 MoF 방식은 단일 레이어 사용을 권장함
            Debug.Assert(document.ActivePage.Layers.ChildrenCount == 1); 

            if (marker.IsBusy)
            {
                // Stop marking process
                // 마킹 공정 중지
                marker.Stop();
                marker.Reset();
            }
            else
            {
                marker.Reset();
                marker.Ready(siriusEditorControl1.Document);

                if (marker is MarkerRtc markerRtc)
                    // Use OffsetFirst procedure for conveyor tracking
                    // 컨베이어 트래킹을 위해 OffsetFirst 절차 사용
                    markerRtc.MarkProcedure = MarkerRtc.MarkProcedures.OffsetFirst;

                // Move scanner mirrors to origin
                // 스캐너 미러 원점 이동
                rtc.CtlMoveTo(DVec2.Zero);

                // Reset process counter
                // 가공 카운터 초기화
                processCounts = 0;
                rtcFreeVariable?.CtlWriteVariable(0, processCounts);

                // Start marking current page
                // 가공 시작
                marker.Start(document.Page);
            }
        }

        /// <summary>
        /// Toggle simulated encoder movement
        /// 가상 엔티티 이동(엔코더 시뮬레이션) 토글
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSimulateEncoder_Click(object sender, EventArgs e)
        {
            var marker = siriusEditorControl1.Marker;
            var rtc = siriusEditorControl1.Scanner as IRtc;
            var rtcMoF = rtc as IRtcMoF;

            if (marker.IsBusy)
                return;

            if (rtcMoF.EncXApproxSpeed == 0)
            {
                // Start simulated encoder movement at +5 mm/s (X direction)
                //
                //             SCANNER (Fixed)
                //                 |
                //                 |
                //                 |
                //                 |
                //                \|/
                //                 .
                //      [ >>>>>>>>>>>>>>>>>>>>> ]   MOVING X+
                rtcMoF.CtlMoFEncoderSpeed(+5, 0);
            }
            else
            {
                // Stop simulated encoder movement
                // 가상 엔티티 이동 중지
                rtcMoF.CtlMoFEncoderSpeed(0, 0);
            }
        }

        /// <summary>
        /// Reset encoder positions to zero
        /// 엔코더 위치 초기화
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnResetEncoder_Click(object sender, EventArgs e)
        {
            var marker = siriusEditorControl1.Marker;
            if (marker.IsBusy)
                return;

            var rtc = siriusEditorControl1.Scanner as IRtc;
            var rtcMoF = rtc as IRtcMoF;

            // Reset encoders to origin
            // 엔코더 위치를 0으로 리셋
            rtcMoF.CtlMoFEncoderReset();
        }
    }
}
