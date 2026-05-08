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
        const int offsetCounts = 1_000; 
        uint processCounts = 0;


        public Form1()
        {
            InitializeComponent();

            // Form load event
            // 폼 로드 이벤트
            // 窗体加载事件
            this.Load += Form1_Load;

            this.FormClosing += (s, e) =>
            {
                // Confirm before closing
                // 종료 전 사용자 확인
                // 关闭前确认
                var dlgResult = MessageBox.Show(this, $"Do you really want to terminate program ?", "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dlgResult != DialogResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }
                // Dispose hardware devices
                // 장치 리소스 해제
                // 释放设备资源
                siriusEditorControl1.DisposeDevices();

                // Dispose document
                var doc = siriusEditorControl1.Document;
                siriusEditorControl1.Document = null;
                doc?.Dispose();


                // Cleanup library
                // 라이브러리 정리
                // 清理库
                SpiralLab.Sirius3.Core.Cleanup();
            };

            this.btnCreateEntities.Click += BtnCreateEntities_Click;
            this.btnStartStop.Click += BtnStartStop_Click;
            this.btnSimulateEncoder.Click += BtnSimulateEncoder_Click;
            this.btnResetEncoder.Click += BtnResetEncoder_Click;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Create all required hardware interfaces
            // 장치 인터페이스 생성
            // 创建硬件接口
            EditorHelper.CreateDevices(out IRtc rtc, out ILaser laser, out IDInput dInExt1, out IDInput dInLaserPort, out IDOutput dOutExt1, out IDOutput dOutExt2, out IDOutput dOutLaserPort, out IPowerMeter powerMeter, out IMarker marker);

            // MoF option must be enabled
            // MoF 기능 필수
            // 必须启用 MoF 功能
            Debug.Assert(rtc.IsMoF);
            var rtcMoF = rtc as IRtcMoF;
            Debug.Assert(rtcMoF != null);

            // Assign devices to editor control
            // 장치들을 컨트롤에 할당
            // 将设备绑定到控件
            siriusEditorControl1.RegisterDevices(rtc, laser, powerMeter, dInExt1, dInLaserPort, dOutExt1, dOutExt2, dOutLaserPort, marker);

            // Register event: free variable change (used as process counter)
            // 자유 변수 변경 이벤트 등록 (가공 카운트용)
            // 注册自由变量变化事件（用于加工计数）
            if (rtc is IRtcFreeVariable rtcFreeVariable)
                rtcFreeVariable.OnFreeVariableChanged += OnFreeVariableChanged;
            // Encoder error event
            // 엔코더 에러 이벤트
            // 编码器错误事件
            rtcMoF.OnEncoderSignalError += OnEncoderSignalError;

            // Virtual field overflow event
            // 가상 영역 벗어남 이벤트
            // 虚拟区域越界事件
            rtcMoF.OnOutOfVirtualImageField += OnOutOfVirtualImageField;

            marker.Ready(siriusEditorControl1.Document, siriusEditorControl1.View, rtc, laser, powerMeter);
        }

        private void OnFreeVariableChanged(IRtcFreeVariable rtcFreeVariable, uint no, uint data)
        {
            // Update processed count in UI
            // UI에 가공 카운트 표시
            // 在界面显示加工计数
            if (nudCounts.InvokeRequired)
            {
                nudCounts.BeginInvoke((MethodInvoker)(() => nudCounts.Value = data));
            }
            else
            {
                nudCounts.Value = data;
            }
        }

        private void OnEncoderSignalError(IRtcMoF rtcMoF, IRtcMarkingInfo rtcMarkingInfo)
        {
            // Handle encoder signal errors
            // 엔코더 신호 오류 처리
            // 处理编码器信号错误

            // You can analyze each bit depending on RTC version
            // RTC 버전에 따라 상세 분석 가능
            // 根据RTC版本解析错误位
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

            //var rtc = rtcMoF as IRtc;
            //// Abort list executing by forcily?
            //rtc.CtlAbort();

            this.Invoke(new MethodInvoker(() =>
            {
                MessageBox.Show(this, $"Encoder Signal Error: {rtcMarkingInfo.ToString()}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }));
        }

        private void OnOutOfVirtualImageField(IRtcMoF rtcMoF, IRtcMarkingInfo rtcMarkingInfo)
        {
            // Handle out-of-range condition
            // 가상 영역 벗어남 처리
            // 处理越界情况
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

            var rtc = rtcMoF as IRtc;
            // Force stop marking
            // 강제 정지
            // 强制停止
            rtc.CtlAbort();

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

            EditPenValues();


        }
        void CreateEntities()
        {
            var document = siriusEditorControl1.Document;
            var marker = siriusEditorControl1.Marker;
            var rtc = siriusEditorControl1.Scanner as IRtc;

            Debug.Assert(rtc.IsMoF);

            // need to MoF at library option.
            //Core.License(out var licenseInfo);
            //Debug.Assert(licenseInfo.IsMoFLicensed);

            //var entity = EntityFactory.CreateSpiralClassic(DVec3.Zero, 0.5, 1, 0, 2, true);
            var entity = EntityFactory.CreateArc(DVec2.Zero, 0.2, 0, 360);
            entity.PenColor = Color.White;
            document.ActAdd(entity);

            siriusEditorControl1.View?.DoRender();

            var rtcMoF = rtc as IRtcMoF;
            Debug.Assert(rtcMoF != null);
            Debug.Assert(rtcMoF.EncXCountsPerMm != 0);
            //Debug.Assert(rtcMoF.EncYCountsPerMm != 0);

            var rnd = new Random((int)DateTime.Now.Ticks);
   
            var offsets = new List<Offset>(offsetCounts);
            for (int i = 0; i < offsetCounts; i++)
            {
                // 오프셋 생성전에 엔코더 리셋은, 현재 스테이지(혹은 컨베이어) 위치를 스캐너 중심과 rtcMoF.CtlMofEncoderReset(); 으로 절대 엔코더 리셋 했다고 가정
                // X 는 좌측 공간(X: -200mm ~ 0) 에 데이타가 있도록 생성
                // Y 는 Fov의 -25% ~ 25% 만 사용
                offsets.Add(new Offset(rnd.NextDouble() * 199.0 - 200.0, (rnd.NextDouble() * rtc.EffectiveFieldSize.Y  - rtc.EffectiveFieldSize.Y / 2.0) * 0.5));
            }

            // Sort offsets by X descending (important for conveyor direction)
            // X 기준 내림차순 정렬 (컨베이어 방향)
            // 按X降序排序（对应传送带方向）
            marker.Offsets = offsets
                .OrderByDescending(o => o.Translate.X)
                .ToArray();

            marker.OnBeforeEntity -= Marker_OnBeforeEntity;
            marker.OnBeforeEntity += Marker_OnBeforeEntity;

            marker.OnAfterEntity -= Marker_OnAfterEntity;
            marker.OnAfterEntity += Marker_OnAfterEntity;

            // Zoom to fit for target entity
            siriusEditorControl1.View?.ActiveCamera?.ZoomFit(siriusEditorControl1.View, entity );
            siriusEditorControl1.View?.DoRender();
        }

        void EditPenValues()
        {
            var document = siriusEditorControl1.Document;
            var marker = siriusEditorControl1.Marker;
            var rtc = siriusEditorControl1.Scanner as IRtc;

            var layerPenColor = SpiralLab.Sirius3.UI.Config.LayerPenColors[0]; // Color.White
            document.FindByLayerPenColor(layerPenColor, out var layerPen);

            // Disable ALC (Auto Laser Control) for test purpose.
            layerPen.IsALC = false;
            //layerPen.IsALC = true; 

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

        private bool Marker_OnBeforeEntity(IMarker marker, IEntity entity)
        {
            var rtc = marker.Scanner as IRtc;
            var rtcMoF = rtc as IRtcMoF;
            Debug.Assert(rtcMoF != null);

            bool success = true;

            // Start MoF at first offset
            // 첫 오프셋에서 MoF 시작
            // 第一个偏移开始MoF
            if (0 == marker.WorkingSet.OffsetIndex)
            {
                // MoF 시작 및 Encoder reset
                //success &= rtcMoF.ListMoFBegin(true);
                // MoF 시작 및 No encoder reset
                success &= rtcMoF.ListMoFBegin(false);
            }

            if (success)
            {
                if (entity is ITransformable transformable)
                {
                    if (transformable.CalcuateRealMinMax(out var realMin, out var realMax))
                    {
                        var offset = marker.WorkingSet.Offset.Translate;

                        // 현재는 스캐너의 X+ 영역만 사용하므로 이를 조절해서
                        // X- 영역도 사용 되도록 대기 위치 조절용
                        var originXShift = -1;

                        // Wait until object reaches scanner center
                        // 等待物体到达扫描中心
                        // 개체의 우측끝이 스캐너 중심에 올떄 까지 기다리기
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

        private bool Marker_OnAfterEntity(IMarker marker, IEntity entity)
        {
            var rtc = marker.Scanner as IRtc;
            var rtcMoF = rtc as IRtcMoF;
            var rtcFreeVariable = rtc as IRtcFreeVariable;
            Debug.Assert(rtcMoF != null);

            bool success = true;

            // Increase process count
            // 가공 카운트 증가
            // 加工计数增加
            processCounts++;
            success &= rtcFreeVariable.ListWriteVariable(0, processCounts);

            // End MoF after last offset
            // 마지막 오프셋 이후 MoF 종료
            // 最后一个偏移后结束MoF
            if (marker.WorkingSet.OffsetIndex == (marker.Offsets.Length - 1))
            {
                // MoF 중지 및 원점으로 점프
                success &= rtcMoF.ListMoFEnd(DVec2.Zero);
            }
            return success;
        }

        private void BtnStartStop_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            var marker = siriusEditorControl1.Marker;
            var rtc = siriusEditorControl1.Scanner as IRtc;
            var rtcFreeVariable = rtc as IRtcFreeVariable;
            
            // RTC 카드에 MoF (Processing on the fly) 옵션이 반드시 필수
            Debug.Assert(rtc.IsMoF);
            var rtcMoF = rtc as IRtcMoF;
            Debug.Assert(rtcMoF != null);

            // MoF 기능은 실행 방식이 레이어 1개만 사용할수 밖에 없음
            Debug.Assert(document.ActivePage.Layers.ChildrenCount == 1); 
            Debug.Assert(marker is MarkerRtc);

            if (marker.IsBusy)
            {
                // Stop marking
                // 가공 중지
                // 停止加工
                marker.Stop();
                marker.Reset();
            }
            else
            {
                marker.Reset();
                marker.Ready(siriusEditorControl1.Document);

                if (marker is MarkerRtc markerRtc)
                    markerRtc.MarkProcedure = MarkerRtc.MarkProcedures.OffsetFirst; // 개별 레이어에 대해 오프셋 개수만큼 반복 가공하는 모드 사용 필요

                // Move scanner to origin
                // 원점 이동
                // 移动到原点
                rtc.CtlMoveTo(DVec2.Zero);

                // Reset counter
                // 카운트 초기화
                // 计数清零
                processCounts = 0;
                rtcFreeVariable?.CtlWriteVariable(0, processCounts);

                // Start marking
                // 가공 시작
                // 开始加工
                marker.Start(document.Page);
            }
        }

        private void BtnSimulateEncoder_Click(object sender, EventArgs e)
        {
            var marker = siriusEditorControl1.Marker;
            var rtc = siriusEditorControl1.Scanner as IRtc;
            Debug.Assert(rtc.IsMoF);
            var rtcMoF = rtc as IRtcMoF;

            if (marker.IsBusy)
                return;

            // Toggle simulated encoder
            // 엔코더 시뮬레이션 토글
            // 模拟编码器开关
            if (rtcMoF.EncXApproxSpeed == 0)
            {
                // Start simulated encoders as x= +5, y=0 mm/s by rtcMoF.CtlMofEncoderSpeed(5, 0);
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

                // 외부 엔코더 연결이 되어 있으면 아래 코드는 주석 처리 
                // Deactivated simulated encoders 
                rtcMoF.CtlMoFEncoderSpeed(0, 0);
            }
        }

        private void BtnResetEncoder_Click(object sender, EventArgs e)
        {
            var marker = siriusEditorControl1.Marker;
            if (marker.IsBusy)
                return;

            var rtc = siriusEditorControl1.Scanner as IRtc;
            Debug.Assert(rtc.IsMoF);
            var rtcMoF = rtc as IRtcMoF;

            // Reset encoder position
            // 엔코더 위치 초기화
            // 重置编码器位置
            rtcMoF.CtlMoFEncoderReset();
        }
    }
}
