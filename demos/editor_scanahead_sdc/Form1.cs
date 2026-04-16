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

        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();

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
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            EditorHelper.CreateDevices(out IRtc rtc, out ILaser laser, out IDInput dInExt1, out IDInput dInLaserPort, out IDOutput dOutExt1, out IDOutput dOutExt2, out IDOutput dOutLaserPort, out IPowerMeter powerMeter, out IMarker marker);

            // SCANAhead 사용하기 위해 RTC6 카드와 SCANahead 옵션이 장착되어 있어야 합니다.
            // 또한 excelliSCAN (or intelliSCAN IV) 스캔헤드를 사용해야 합니다.
            Debug.Assert(rtc is Rtc6);

            var rtc6 = rtc as Rtc6;
            Debug.Assert(rtc6 != null);
            
            Debug.Assert(rtc.IsSCANAhead);

            // Activate auto delays 
            // 스캔 헤드로 부터 Pre-configuration (SCANAhead 를 위한 Preview time, Velocity max, Acc max 파라메터들)이 Load 됨

            rtc6.IsActivateAutoDelays = true;
            //or rtc6.CtlDelayAutoByScanAhead(true);
            
            // 활성화 되었는지 재차 확인
            Debug.Assert(rtc6.IsActivateAutoDelays);


            // Trajectory Acknowledge 
            // Trajectory Acknowledge OK 여부를 rtc.CtlGetStatus(RtcStatus.PositionAckOK) 함수로 확인할 수 있습니다.
            // (참고) SCANAhead 의 경우 Position Ack OK 상태가 Trajectory ACK Ok 로 처리됩니다.
            //
            // 또한 Trajectory ACK 상태가 실패(0 인 상태) 인 경우, 에러 상태가 자동 리셋되지 않고 레치 상태로 유지되기 때문에
            // IRtc.CtlReset() 을 사용해야 에러 상태의 해제가 가능합니다.
            //
            // (참고) rtc6.PositionACKLimit 값을 통해 해당 Threshold 값을 설정할수있습니다.
            // 기본값은 전체 FOV 영역의 0.28% 이며, 스캐너의 전원이 리셋될때 마다 초기화됩니다.
            // 만약 스캐너의 Effective FOV 범위가 100mm 인 경우, 0.28mm 로 설정됩니다.
            // 이 값을 Actual-Command 차이값이 범위를 넘는지 여부를 모니터링 할기 위해 더 작게 설정할 수 있습니다.
            rtc6.PositionACKLimit = 0.01; //10um

            // 이 상태(Rtc6 + SCANAhead + Auto delays)로 SiriusEditorControl 에 Rtc 가 할당되면,
            // 아래 항목들이 사용되지 않으므로 속성창에서 Invisible 상태가 됩니다. 
            //
            // EntityPen :
            //  LaserOnShiftSCANa, LaserOnShiftSCANa 항목이 보임
            //  ScannerJumpDelay,  ScannerMarkDelay, ScannerPolygonDelay, LaserOnDelay, LaserOffDelay 않보임
            // EntityLayerPen :
            //  IsVariablePolygonDelay, VariablePolygonDelayEdgeLevel 않보임
            //  IsVariableJumpDelay, VariableJumpDelayMin, VariableJumpDelayLimitLength 않보임
            
            siriusEditorControl1.RegisterDevices(rtc, laser, powerMeter, dInExt1, dInLaserPort, dOutExt1, dOutExt2, dOutLaserPort, marker);

            var document = siriusEditorControl1.Document;

            marker.Ready(document, siriusEditorControl1.View, rtc, laser, powerMeter);

            // Find layer pen for 'White'
            document.FindByLayerPenColor(System.Drawing.Color.White, out var layerPenWhite);

            // Actual velocity + spot distance control
            layerPenWhite.IsALC = true;
            layerPenWhite.AlcByPositionTable.Clear();
            layerPenWhite.AlcSignal = AutoLaserControlSignals.SpotDistance; //RTC6 + SCANAhead
            layerPenWhite.AlcMode = AutoLaserControlModes.ActualVelocity;
            layerPenWhite.AlcModeExtension.Clear();

            // excelliSCAN 헤드 및 SDC(등간격 제어) 사용 시 필수.
            // Tracking Error 대신 프리뷰 타임(Preview Time)을 기반으로 제어.
            layerPenWhite.AlcModeExtension.Add(AutoLaserControlModeExtensions.Bit.SCANAhead);

            // Sky Writing 중 SDC 유지. 스카이 라이팅(가감속 구간) 동작 중에도 SDC 알고리즘을 유지.
            // 벡터의 시작과 끝부분에서도 펄스 간격을 정밀하게 유지합니다. SCANahead 활성화가 필요. (RTC6 전용).
            //layerPenWhite.AlcModeExtension.Add(AutoLaserControlModeExtensions.Bit.SkyWritingSDC); 

            // 엔코더 속도 합산. 스캐너 속도에 엔코더 속도를 벡터 합산.
            // 이동하는 물체를 가공하는 MoF(Marking On-the-Fly) 공정에서 사용.
            //layerPenWhite.AlcModeExtension.Add(AutoLaserControlModeExtensions.Bit.EncoderSpeedAddition); 

            // 역 속도 보정. F-Theta 렌즈 왜곡으로 인해 발생하는 위치별 선속도 차이를 보정.
            // 보정 테이블을 사용하여 각속도를 실제 필드 상의 속도로 변환.
            //layerPenWhite.AlcModeExtension.Add(AutoLaserControlModeExtensions.Bit.InverseSpeedCorrection); 

            // 역 좌표 변환. 좌표 변환(회전, 행렬 등)이 적용된 경우, 피드백 속도를 역변환.
            // 레이저 제어가 변환 전의 원본 도면 속도를 기준으로 수행되도록 보장. (RTC6 전용)
            //layerPenWhite.AlcModeExtension.Add(AutoLaserControlModeExtensions.Bit.BackwardTransformation); 

            // Find entity pen for 'White'
            var spotDistance = 0.01; //10 um
            document.FindByEntityPenColor(System.Drawing.Color.White, out var entityPenWhite);
            entityPenWhite.SpotDistanceSCANa = spotDistance;
        }

    }
}
