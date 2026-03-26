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
using SpiralLab.Sirius3.UI.WinForms;
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

        const int instanceCount = 2;
        SiriusEditorControl[] EditorControls = new SiriusEditorControl[instanceCount];
        IRtcMultiBeam[] RtcMultiBeams = new IRtcMultiBeam[instanceCount];

        public Form1()
        {
            InitializeComponent();

            EditorControls[0] = siriusEditorControl1;
            EditorControls[1] = siriusEditorControl2;

            this.Load += Form1_Load;
            this.Disposed += Form1_Disposed;

            this.btnCheckPins.Click += BtnCheckPins_Click;
            this.btnNone.Click += BtnHeadNone_Click;
            this.btnHead1.Click += BtnHead1_Click;
            this.btnHead2.Click += BtnHead2_Click;
            this.btnHead12.Click += BtnHead12_Click;
            this.btnStop.Click += BtnStop_Click;
        }


        private void Form1_Disposed(object sender, EventArgs e)
        {
            for (int i = 0; i < instanceCount; i++)
                EditorHelper.DestroyDevices(EditorControls[i]);
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            //Need to 2 instances and multibeam option at library option.
            //Core.License(out var licenseInfo);
            //Debug.Assert(licenseInfo.RtcLicenseMax == 2);
            //Debug.Assert(licenseInfo.IsMultiBeamLicensed);
            
            ILaser laser = null;

            // Initialize RTC MultiBeam instances (MultiBeamIndex: 0, 1 for Pair 0)
            // RTC 멀티빔 인스턴스 초기화 (멀티빔 인덱스 0, 1은 페어 0을 형성함)
            // 初始化 RTC 多光束实例（多光束索引 0、1 组成第 0 对）
            CreateMultiBeamDevices(0, out var rtcMultiBeam1, out IMarker marker1, 0);
            RtcMultiBeams[0] = rtcMultiBeam1;
            var rtc1 = rtcMultiBeam1 as IRtc;
            CreateLaser(out laser, rtc1);
            EditorControls[0].Scanner = rtc1;
            EditorControls[0].Laser = laser;
            EditorControls[0].Marker = marker1;

            CreateMultiBeamDevices(1, out var rtcMultiBeam2,  out IMarker marker2, 1);
            RtcMultiBeams[1] = rtcMultiBeam2;
            var rtc2 = rtcMultiBeam2 as IRtc;
            EditorControls[1].Scanner = rtc2;
            EditorControls[1].Laser = laser; //same laser source
            EditorControls[1].Marker = marker2;


            // for 1st SCAN Head with Rtc1 
            RtcMultiBeams[0].TokenWaitBitMask = 0b_0000_0000_0000_0001;
            RtcMultiBeams[0].TokenAckBitMask = 0b_0000_0000_0000_0010;
            RtcMultiBeams[0].AOMBitMask = 0b_0000_0000_0000_0100;
            RtcMultiBeams[0].AOMChannel = ExtensionChannels.ExtAO1;
            RtcMultiBeams[0].AOM0OrderVoltage = 0;
            var approxMaxWatt1 = laser.MaxPowerWatt * 0.98 * 0.85;
            RtcMultiBeams[0].AOM1stOrderVoltage = 5.0;
            RtcMultiBeams[0].AOMHoldMsec = 0.01; // 10usec


            // for 2nd SCAN Head with Rtc2
            RtcMultiBeams[1].TokenWaitBitMask = 0b_0000_0000_0000_0001;
            RtcMultiBeams[1].TokenAckBitMask = 0b_0000_0000_0000_0010;
            RtcMultiBeams[1].AOMBitMask = 0b_0000_0000_0000_0100;
            RtcMultiBeams[1].AOMChannel = ExtensionChannels.ExtAO1;
            RtcMultiBeams[1].AOM0OrderVoltage = 0;
            var approxMaxWatt2 = laser.MaxPowerWatt * 0.85;
            double approxEfficiency = (approxMaxWatt2 / laser.MaxPowerWatt); 
            RtcMultiBeams[1].AOM1stOrderVoltage = 5.0 * approxEfficiency;
            RtcMultiBeams[1].AOMHoldMsec = 0.01; // 10usec

            // To get notification for 'RtcMultiBeamHelper.Modes' has changed
            RtcMultiBeamHelper.PropertyChanged += RtcMultiBeamHelper_PropertyChanged;


            marker1.Ready(EditorControls[0].Document, EditorControls[0].View, rtc1, laser, null);
            marker2.Ready(EditorControls[1].Document, EditorControls[1].View, rtc2, laser, null);
        }

        private void RtcMultiBeamHelper_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RtcMultiBeamHelper.Modes mode = RtcMultiBeamHelper.GetMode(0);
            lblMode.Text = $"Mode: {mode.ToString()}"; 
        }

        bool CreateMultiBeamDevices(int index, out IRtcMultiBeam rtcMultiBeam, out IMarker marker, int multiBeamIndex)
        {
            rtcMultiBeam = null;
            marker = null;

            bool success = true;

            // scanner card controller
            var fov = 100.0;    // field of view (100mm)
            var kfactor = Math.Pow(2, 20) / fov; // kfactor = bits/mm (20bits resolution = 2^20 for RTC6)
            LaserModes laserMode = LaserModes.Yag1; // output signals timing for LASER1, LASER2 and LASER ON at RTC card
            RtcSignalLevels signalLevelLaser12 = RtcSignalLevels.ActiveHigh; // output signal level for LASER1 and LASER2 at RTC card
            RtcSignalLevels signalLevelLaserOn = RtcSignalLevels.ActiveHigh; // output signal level for LASER ON at RTC card
            string correctionPath = Path.Combine(SpiralLab.Sirius3.Config.CorrectionPath, "cor_1to1.ct5"); // *.ct5 for RTC5,6 card (*.ctb for RTC4 card)

            //var rtc = ScannerFactory.CreateRtc5MultiBeam(index, multiBeamIndex, kfactor, laserMode, signalLevelLaser12, signalLevelLaserOn, correctionPath); // create Rtc6 card instance
            var rtc = ScannerFactory.CreateRtc6MultiBeam(index, multiBeamIndex, kfactor, laserMode, signalLevelLaser12, signalLevelLaserOn, correctionPath); // create Rtc6 card instance
            //var rtc = ScannerFactory.CreateRtcVirtualMultiBeam(index, multiBeamIndex, kfactor, laserMode, signalLevelLaser12, signalLevelLaserOn, correctionPath); // create Rtc6 card instance
            success &= rtc.Initialize(); // initialize the card
            Debug.Assert(success);
            rtcMultiBeam = rtc;

            //var dIExt1 = IOFactory.CreateInputExtension1(rtc);
            //var dOExt1 = IOFactory.CreateOutputExtension1(rtc);
            //var dOExt2 = IOFactory.CreateOutputExtension2(rtc);
            //var dILaserPort = IOFactory.CreateInputLaserPort(rtc);
            //var dOLaserPort = IOFactory.CreateOutputLaserPort(rtc);

            // powermeter device
            //var powerMeter = PowerMeterFactory.CreateVirtual(index, laserMaxPower); // create virtual powermeter instance for test purpose
            //var powerMeter = PowerMeterFactory.CreateCoherentPowerMax(index, COMPORT);
            //var powerMeter = PowerMeterFactory.CreateOphirPhotonics(index, SERIALNO);
            //var powerMeter = PowerMeterFactory.CreateGentecEO(index, COMPORT);
            //success &= powerMeter.Initialize();
            //Debug.Assert(success);

            // marker
            marker = MarkerFactory.CreateRtc(index); // create marker instance 
            return success;
        }

        bool CreateLaser(out ILaser laser, IRtc rtc)
        {
            bool success = true;

            // laser source device
            var laserMaxPower = 10.0; // laser max output power (W)
            laser = LaserFactory.CreateVirtual(0, laserMaxPower, PowerControlMethods.Unknown); // create virtual laser instance for test purpose
            //var laser = LaserFactory.CreateVirtualAnalog(index, laserMaxPower, analog1, voltageMin, voltageMax); // create virtual analog output laser instance for test purpose
            //var laser = LaserFactory.CreateVirtualDutyCycle(index, laserMaxPower, dutyCycleMin, dutyCycleMax); // create virtual duty cycle output laser instance for test purpose
            //var laser = LaserFactory.CreateVirtualDO8Bits(index, laserMaxPower, dOut8Min, dOut8Max); // create virtual DO8Bits output laser instance for test purpose
            //var laser = LaserFactory.Create for target vender product ...

            laser.Scanner = rtc; // assign scanner instance to laser
            success &= laser.Initialize(); // initialize the laser
            Debug.Assert(success);
            
            return success;
        }

        private void BtnCheckPins_Click(object sender, EventArgs e)
        {
            if (RtcMultiBeamHelper.CheckPins(0))
                System.Windows.Forms.MessageBox.Show(this, $"PIN CONNECTION ARE OK!", "INFORMATION", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            else
                System.Windows.Forms.MessageBox.Show(this, $"PIN CONNECTION ARE NOT OK!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void BtnHeadNone_Click(object sender, EventArgs e)
        {
            // Safety State (Route laser to Beam Dump)
            // 안전 상태 (레이저를 빔 덤프 경로로 설정)
            // 安全状态（将激光引导至光束卸载区路径）
           if (!RtcMultiBeamHelper.SetMode(0, RtcMultiBeamHelper.Modes.None))
               return;       
        }

        private void BtnHead1_Click(object sender, EventArgs e)
        {
            // Manual Head Selection (Switch AOM to Head 1 path)
            // 수동 헤드 선택 (AOM을 헤드 1 경로로 스위칭)
            // 手动选择头（将 AOM 切换到 1 号头路径）
            if (!RtcMultiBeamHelper.SetMode(0, RtcMultiBeamHelper.Modes.Head1Only))
                return;

            var marker = EditorControls[0].Marker;
            if (marker.IsBusy)
                return;

            var document = EditorControls[0].Document;
         
            marker.Reset();
            marker.Ready(siriusEditorControl1.Document);

            // Start to mark current page
            marker.Start(document.Page);
        }

        private void BtnHead2_Click(object sender, EventArgs e)
        {
            // Manual Head Selection (Switch AOM to Head 2 path)
            // 수동 헤드 선택 (AOM을 헤드 2 경로로 스위칭)
            // 手动选择头（将 AOM 切换到 2 号头路径）
            if (!RtcMultiBeamHelper.SetMode(0, RtcMultiBeamHelper.Modes.Head2Only))
                return;

            var marker = EditorControls[1].Marker;
            if (marker.IsBusy)
                return;

            var document = EditorControls[1].Document;

            marker.Reset();
            marker.Ready(siriusEditorControl1.Document);

            // Start to mark current page
            marker.Start(document.Page);
        }

        private void BtnHead12_Click(object sender, EventArgs e)
        {
            // Set Multi-Beam Processing Mode (Pair Index: 0, Mode: Both for exclusive sequential marking)
            // 멀티빔 가공 모드 설정 (페어 인덱스: 0, 모드: Both - 배타적 순차 가공)
            // 设置多光束加工模式（对索引：0，模式：Both - 互斥顺序加工）

            // Exclusive Marking Logic (Each RTC board exchanges tokens via 4-Way Handshake)
            // 배타적 가공 로직 (각 RTC 보드는 4-Way Handshake를 통해 토큰을 교환함)
            // 互斥加工逻辑（每个 RTC 板卡通过 4-Way Handshake 交换令牌）

            // Note: RtcMultiBeamHelper.Modes.Both ensures that ListJumpTo internally handles token acquisition/release.
            // 참고: Modes.Both 설정 시 ListJumpTo 내부에서 토큰 획득/해제 핸드셰이크가 자동으로 수행됩니다.
            // 注意：设置 Modes.Both 时，ListJumpTo 内部会自动执行令牌获取/释放握手。
            if (!RtcMultiBeamHelper.SetMode(0, RtcMultiBeamHelper.Modes.Both))
                return;

            for (int i = 0; i < instanceCount; i++)
            {
                var marker = EditorControls[i].Marker;
                if (marker.IsBusy)
                    return;
            }

            for (int i = 0; i < instanceCount; i++)
            {
                var document = EditorControls[i].Document;
                var marker = EditorControls[i].Marker;

                marker.Reset();
                marker.Ready(siriusEditorControl1.Document);

                // Start to mark current page
                marker.Start(document.Page);
            }
        }
        private void BtnStop_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < instanceCount; i++)
            {
                var marker = EditorControls[i].Marker;

                marker.Stop();
                marker.Reset();
            }
        }
    }
}
