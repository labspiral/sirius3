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

            // To use SCANahead, an RTC6 card with the SCANahead option must be equipped.
            // Also, an excelliSCAN (or intelliSCAN IV) scan head must be used.
            // SCANAhead 사용하기 위해 RTC6 카드와 SCANahead 옵션이 장착되어 있어야 합니다.
            // 또한 excelliSCAN (or intelliSCAN IV) 스캔헤드를 사용해야 합니다.
            Debug.Assert(rtc is Rtc6);

            var rtc6 = rtc as Rtc6;
            Debug.Assert(rtc6 != null);

            // Check if RTC card is ready for SCANAhead
            // SCANAhead 사용하기 위한 준비가 되었는지 여부 확인
            Debug.Assert(rtc.IsSCANAhead);

            // Activate auto delays 
            // Pre-configurations (Preview time, Velocity max, Acc max parameters for SCANAhead) are loaded from the scan head.
            // Activate auto delays 
            // 스캔 헤드로 부터 Pre-configuration (SCANAhead 를 위한 Preview time, Velocity max, Acc max 파라메터들)이 Load 됨

            rtc6.IsActivateAutoDelays = true;
            // or rtc6.CtlDelayAutoByScanAhead(true);
            
            // Re-verify if activated
            // 활성화 되었는지 재차 확인
            Debug.Assert(rtc6.IsActivateAutoDelays);

            // Trajectory Acknowledge 
            // You can check the Trajectory Acknowledge OK status using the rtc.CtlGetStatus(RtcStatus.PositionAckOK) function.
            // (Note) In the case of SCANAhead, the Position Ack OK status is treated as Trajectory ACK Ok.
            //
            // If the Trajectory ACK status fails (status is 0), the error status is not automatically reset but remains in a latched state.
            // Therefore, IRtc.CtlReset() must be used to clear the error status.
            //
            // (Note) You can set the corresponding threshold value through the rtc6.PositionACKLimit value.
            // The default value is 0.28% of the total FOV area, and it is initialized every time the scanner power is reset.
            // If the scanner's effective FOV range is 100mm, it is set to 0.28mm.
            // This value can be set smaller to monitor whether the difference between Actual and Command positions exceeds the range.
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
            rtc6.PositionACKLimit = 0.01; // 10um

            // When RTC is assigned to SiriusEditorControl in this state (Rtc6 + SCANAhead + Auto delays),
            // the following items are not used and will be invisible in the property grid.
            //
            // EntityPen:
            //  LaserOnShiftSCANa, LaserOffShiftSCANa items are visible
            //  ScannerJumpDelay, ScannerMarkDelay, ScannerPolygonDelay, LaserOnDelay, LaserOffDelay are invisible
            //
            // EntityLayerPen:
            //  IsVariablePolygonDelay, VariablePolygonDelayEdgeLevel are invisible
            //  IsVariableJumpDelay, VariableJumpDelayMin, VariableJumpDelayLimitLength are invisible
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

            marker.Ready(siriusEditorControl1.Document, siriusEditorControl1.View, rtc, laser, powerMeter);
        }
    }
}
