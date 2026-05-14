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
    /// <summary>
    /// Form1
    /// </summary>
    public partial class Form1 : Form
    {
        const int instanceCount = 2;
        const int multibeamPairIndex = 0;

        readonly RadioButton[] modeRadioButtons;
        readonly RadioButton[] sideRadioButtons;

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
                var dlgResult = System.Windows.Forms.MessageBox.Show(this, $"Do you really want to terminate program ?", "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dlgResult != DialogResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }

                // Dispose instances 
                // 인스턴스 해제 
                siriusMultiEditorControl1.DisposeDevices();

                // Dispose document
                // 문서 해제
                var doc = siriusMultiEditorControl1.Document;
                siriusMultiEditorControl1.Document = null;
                doc?.Dispose();

                // Clean up SIRIUS3 library
                // SIRIUS3 라이브러리 정리
                SpiralLab.Sirius3.Core.Cleanup();
            };

            modeRadioButtons = new[] { rbModeNone, rbModeHead1, rbModeHead2, rbModeBoth };
            for (int i = 0; i < modeRadioButtons.Length; i++)
            {
                modeRadioButtons[i].CheckedChanged += RbMode_CheckedChanged;
            }

            sideRadioButtons = new[] { rbHead1Side, rbHead2Side };
            for (int i = 0; i < sideRadioButtons.Length; i++)
            {
                sideRadioButtons[i].CheckedChanged += RbSide_CheckedChanged;
            }

            this.btnCheckPins.Click += BtnCheckPins_Click;
            this.btnReady.Click += BtnReady_Click;
            this.btnStart.Click += BtnStart_Click;
            this.btnStop.Click += BtnStop_Click;
            this.btnReset.Click += BtnReset_Click;
        }

        /// <summary>
        /// Form load
        /// 폼 로드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            // Need to equipped with 2instances and multibeam option at library option.
            //Core.License(out var licenseInfo);
            //Debug.Assert(licenseInfo.RtcLicenseMax == instanceCount);
            //Debug.Assert(licenseInfo.IsMultiBeamLicensed);

            // Initialize RTC MultiBeam instances (MultiBeamIndex: 0, 1 for Pair 0)
            // RTC 멀티빔 인스턴스 초기화 (멀티빔 인덱스 0, 1은 페어 0을 형성함) (멀티빔 인덱스 2, 3은 페어 1을 형성함)
            // 初始化 RTC 多光束实例（多光束索引 0、1 组成第 0 对）
            siriusMultiEditorControl1.MaxDeviceCounts = instanceCount;

            // single laser source
            ILaser laser = null;
            EditorHelper.CreateLaser(0, out laser);

            // two scanheads (with 2 rtc cards)
            {
                int index = 0;
                // Create devices
                // 장치 생성
                EditorHelper.CreateDevices(index, 0, laser, out var rtcMultiBeam, out IDInput dInExt1, out IDInput dInLaserPort, out IDOutput dOutExt1, out IDOutput dOutExt2, out IDOutput dOutLaserPort, out IPowerMeter powerMeter, out IMarker marker);
                siriusMultiEditorControl1.RegisterDevices(index, rtcMultiBeam, laser, powerMeter, dInExt1, dInLaserPort, dOutExt1, dOutExt2, dOutLaserPort, marker);
                marker.Ready(siriusMultiEditorControl1.Document, siriusMultiEditorControl1.View, rtcMultiBeam, laser, null);

                RenameDIOs(dInExt1, dOutExt1, siriusMultiEditorControl1);
            }

            {
                int index = 1;
                // Create devices
                // 장치 생성
                EditorHelper.CreateDevices(index, 1, laser, out var rtcMultiBeam, out IDInput dInExt1, out IDInput dInLaserPort, out IDOutput dOutExt1, out IDOutput dOutExt2, out IDOutput dOutLaserPort, out IPowerMeter powerMeter, out IMarker marker);
                siriusMultiEditorControl1.RegisterDevices(index, rtcMultiBeam, laser, powerMeter, dInExt1, dInLaserPort, dOutExt1, dOutExt2, dOutLaserPort, marker);
                marker.Ready(siriusMultiEditorControl1.Document, siriusMultiEditorControl1.View, rtcMultiBeam, laser, null);

                RenameDIOs(dInExt1, dOutExt1, siriusMultiEditorControl1);
            }

        }

        void RenameDIOs(IDInput dInExt1, IDOutput dOutExt1, SiriusMultiEditorControl siriusMultiEditorControl)
        {
            dInExt1.ChannelNames = new string[1][] {
                new string[16] {
                 "MY TOKEN",
                    "D01",
                    "D02",
                    "D03",
                    "D04",
                    "D05",
                    "D06",
                    "D07",
                    "D08",
                    "D09",
                    "D10",
                    "D11",
                    "D12",
                    "D13",
                    "D14",
                    "D15",
                }};

            dOutExt1.ChannelNames = new string[1][] {
                new string[16] {
                    "PEER TOKEN",
                    "AOM",
                    "D02",
                    "D03",
                    "D04",
                    "D05",
                    "D06",
                    "D07",
                    "D08",
                    "D09",
                    "D10",
                    "D11",
                    "D12",
                    "D13",
                    "D14",
                    "D15",
                }};

            //siriusMultiEditorControl.DIRtcCtrl.AnalogNames[0] = 
            //siriusMultiEditorControl.DIRtcCtrl.AnalogNames[1] = 

            siriusMultiEditorControl.DORtcCtrl.AnalogNames[0] = "AOM";
            siriusMultiEditorControl.DORtcCtrl.AnalogNames[1] = "LASER POWER";
        }

        private void RbMode_CheckedChanged(object sender, EventArgs e)
        {
            var rb = sender as RadioButton;
            if (rb == null || !rb.Checked) return;

            RtcMultiBeamHelper.MultiBeamModes mode = RtcMultiBeamHelper.MultiBeamModes.None;
            if (rb == rbModeNone)
                // Safety State (Route laser to Beam Dump)
                // 안전 상태 (레이저를 빔 덤프 경로로 설정)
                // 安全状态（将激光引导至光束卸载区路径）
                mode = RtcMultiBeamHelper.MultiBeamModes.None;
            else if (rb == rbModeHead1)
                // Manual Head Selection (Switch AOM to Head 1 path)
                // 수동 헤드 선택 (AOM을 헤드 1 경로로 스위칭)
                // 手动选择头（将 AOM 切换到 1 号头路径）
                mode = RtcMultiBeamHelper.MultiBeamModes.Head1;
            else if (rb == rbModeHead2)
                // Manual Head Selection (Switch AOM to Head 2 path)
                // 수동 헤드 선택 (AOM을 헤드 2 경로로 스위칭)
                // 手动选择头（将 AOM 切换到 2 号头路径）
                mode = RtcMultiBeamHelper.MultiBeamModes.Head2;
            else if (rb == rbModeBoth)
                // Set Multi-Beam Processing Mode (Pair Index: 0, Mode: Both for exclusive sequential marking)
                // 멀티빔 가공 모드 설정 (페어 인덱스: 0, 모드: Both - 배타적 순차 가공)
                // 设置多光束加工模式（对索引：0，模式：Both - 互斥顺序加工）

                // Exclusive Marking Logic (Each RTC board exchanges tokens via 4-Way Handshake)
                // 배타적 가공 로직 (각 RTC 보드는 4-Way Handshake를 통해 토큰을 교환함)
                // 互斥加工逻辑（每个 RTC 板卡通过 4-Way Handshake 交换令牌）

                // Note: RtcMultiBeamHelper.Modes.Both ensures that ListJumpTo internally handles token acquisition/release.
                // 참고: Modes.Both 설정 시 ListJumpTo 내부에서 토큰 획득/해제 핸드셰이크가 자동으로 수행됩니다.
                // 注意：设置 Modes.Both 时，ListJumpTo 内部会自动执行令牌获取/释放握手。
                mode = RtcMultiBeamHelper.MultiBeamModes.Both;

            RtcMultiBeamHelper.MultiBeamPreperSides side = RtcMultiBeamHelper.MultiBeamPreperSides.Head1;
            if (rbHead1Side.Checked) 
                side = RtcMultiBeamHelper.MultiBeamPreperSides.Head1;
            else if (rbHead2Side.Checked) 
                side = RtcMultiBeamHelper.MultiBeamPreperSides.Head2;

            if (!RtcMultiBeamHelper.SetMode(multibeamPairIndex, mode, side))
            {

            }
        }

        private void RbSide_CheckedChanged(object sender, EventArgs e)
        {
            var rb = sender as RadioButton;
            if (rb == null || !rb.Checked) return;

            var mode = RtcMultiBeamHelper.GetMode(multibeamPairIndex);

            RtcMultiBeamHelper.MultiBeamPreperSides side = RtcMultiBeamHelper.MultiBeamPreperSides.Head1;
            if (rbHead1Side.Checked) side = RtcMultiBeamHelper.MultiBeamPreperSides.Head1;
            else if (rbHead2Side.Checked) side = RtcMultiBeamHelper.MultiBeamPreperSides.Head2;

            if (!RtcMultiBeamHelper.SetMode(multibeamPairIndex, mode, side))
            {
            }
        }

        private void BtnCheckPins_Click(object sender, EventArgs e)
        {
            if (RtcMultiBeamHelper.CheckPins(multibeamPairIndex))
                System.Windows.Forms.MessageBox.Show(this, $"PIN CONNECTION ARE OK !", "INFORMATION", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            else
                System.Windows.Forms.MessageBox.Show(this, $"PIN CONNECTION ARE NOT OK !", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void BtnReady_Click(object sender, EventArgs e)
        {
            if (!RtcMultiBeamHelper.ReadyMode(multibeamPairIndex))
            {
                var form = new SpiralLab.Sirius3.UI.WinForms.MessageBox(
                          $"Fail to ready multibeam pair",
                          "MultiBeam",
                          MessageBoxButtons.OK);
                form.ShowDialog(this);
            }
        }

        private async void BtnStart_Click(object sender, EventArgs e)
        {
            if (!RtcMultiBeamHelper.IsReady(multibeamPairIndex))
            {
                var form = new SpiralLab.Sirius3.UI.WinForms.MessageBox(
                          $"Multibeam Pair is not ready ?.\rTry ready at first",
                          "MultiBeam",
                          MessageBoxButtons.OK);
                form.ShowDialog(this);
                return;
            }

            var mode = RtcMultiBeamHelper.GetMode(multibeamPairIndex);
            bool success = true;

            switch (mode)
            {
                case RtcMultiBeamHelper.MultiBeamModes.None:
                    break;
                case RtcMultiBeamHelper.MultiBeamModes.Head1:
                    {
                        var form = new SpiralLab.Sirius3.UI.WinForms.MessageBox(
                            $"Do you want to start for {mode} ?",
                            "MultiBeam",
                            MessageBoxButtons.YesNo);
                        if (DialogResult.Yes != form.ShowDialog(this))
                            return;
                    }
                    success &= await siriusMultiEditorControl1.Markers[0].Start();
                    break;
                case RtcMultiBeamHelper.MultiBeamModes.Head2:
                    {
                        var form = new SpiralLab.Sirius3.UI.WinForms.MessageBox(
                            $"Do you want to start for {mode} ?",
                            "MultiBeam",
                            MessageBoxButtons.YesNo);
                        if (DialogResult.Yes != form.ShowDialog(this))
                            return;
                    }
                    success &= await siriusMultiEditorControl1.Markers[1]?.Start();
                    break;
                case RtcMultiBeamHelper.MultiBeamModes.Both:
                    {
                        var form = new SpiralLab.Sirius3.UI.WinForms.MessageBox(
                            $"Do you want to start for {mode} ?",
                            "MultiBeam",
                            MessageBoxButtons.YesNo);
                        if (DialogResult.Yes != form.ShowDialog(this))
                            return;
                    }
                    var task1 = siriusMultiEditorControl1.Markers[0]?.Start();
                    var task2 = siriusMultiEditorControl1.Markers[1]?.Start();
                    
                    await Task.WhenAll(task1, task2); // 둘 다 끝날 때까지 대기
                    bool result1 = await task1;
                    bool result2 = await task2;
                    success &= result1;
                    success &= result2;
                    break;
            }
        }

        private async void BtnStop_Click(object sender, EventArgs e)
        {
            var mode = RtcMultiBeamHelper.GetMode(multibeamPairIndex);

            bool success = true;
            switch (mode)
            {
                case RtcMultiBeamHelper.MultiBeamModes.None:
                    break;
                case RtcMultiBeamHelper.MultiBeamModes.Head1:
                    success &= await siriusMultiEditorControl1.Markers[0]?.Stop();
                    break;
                case RtcMultiBeamHelper.MultiBeamModes.Head2:

                    success &= await siriusMultiEditorControl1.Markers[1]?.Stop();
                    break;
                case RtcMultiBeamHelper.MultiBeamModes.Both:
                    var t1 = siriusMultiEditorControl1.Markers[0]?.Stop();
                    var t2 = siriusMultiEditorControl1.Markers[1]?.Stop();

                    await Task.WhenAll(t1, t2);

                    bool result1 = await t1;
                    bool result2 = await t2;
                    success &= result1 && result2;
                    break;
            }
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            var mode = RtcMultiBeamHelper.GetMode(multibeamPairIndex);
            bool success = true;

            switch (mode)
            {
                case RtcMultiBeamHelper.MultiBeamModes.None:
                    break;
                case RtcMultiBeamHelper.MultiBeamModes.Head1:
                    success &= siriusMultiEditorControl1.Markers[0].Reset();
                    break;
                case RtcMultiBeamHelper.MultiBeamModes.Head2:
                    success &= siriusMultiEditorControl1.Markers[1].Reset();
                    break;
                case RtcMultiBeamHelper.MultiBeamModes.Both:
                    success &= siriusMultiEditorControl1.Markers[0].Reset();
                    success &= siriusMultiEditorControl1.Markers[1].Reset();
                    break;
            }
        }
    }
}
