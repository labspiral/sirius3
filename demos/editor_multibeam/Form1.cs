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
        const int multibeamPairIndex = 0;

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
                siriusEditorControl1.DisposeDevices();
                siriusEditorControl2.DisposeDevices();

                // Dispose document
                var doc1 = siriusEditorControl1.Document;
                siriusEditorControl1.Document = null;
                doc1?.Dispose();

                var doc2 = siriusEditorControl2.Document;
                siriusEditorControl2.Document = null;
                doc2?.Dispose();

                // Clean up SIRIUS3 library
                SpiralLab.Sirius3.Core.Cleanup();
            };

            this.btnCheckPins.Click += BtnCheckPins_Click;
            this.btnNone.Click += BtnHeadNone_Click;
            this.btnHead1.Click += BtnHead1_Click;
            this.btnHead2.Click += BtnHead2_Click;
            this.btnHead12.Click += BtnHead12_Click;

            this.btnReady.Click += BtnReady_Click;
            this.btnStart.Click += BtnStart_Click;
            this.btnStop.Click += BtnStop_Click;
            this.btnReset.Click += BtnReset_Click;
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            // Need to equipped with 2instances and multibeam option at library option.
            Core.License(out var licenseInfo);
            Debug.Assert(licenseInfo.RtcLicenseMax == 2);
            Debug.Assert(licenseInfo.IsMultiBeamLicensed);


            // single laser source
            ILaser laser = null;
            EditorHelper.CreateLaser(0, out laser);

            // two scanheads (with 2 rtc cards)
            {
                int index = 0;
                EditorHelper.CreateDevices(index, 0, laser, out var rtcMultiBeam, out IDInput dInExt1, out IDInput dInLaserPort, out IDOutput dOutExt1, out IDOutput dOutExt2, out IDOutput dOutLaserPort, out IPowerMeter powerMeter, out IMarker marker);
                siriusEditorControl1.RegisterDevices(rtcMultiBeam, laser, powerMeter, dInExt1, dInLaserPort, dOutExt1, dOutExt2, dOutLaserPort, marker);
                marker.Ready(siriusEditorControl1.Document, siriusEditorControl1.View, rtcMultiBeam, laser, null);
            }

            {
                int index = 1;
                EditorHelper.CreateDevices(index, 1, laser, out var rtcMultiBeam, out IDInput dInExt1, out IDInput dInLaserPort, out IDOutput dOutExt1, out IDOutput dOutExt2, out IDOutput dOutLaserPort, out IPowerMeter powerMeter, out IMarker marker);
                siriusEditorControl2.RegisterDevices(rtcMultiBeam, laser, powerMeter, dInExt1, dInLaserPort, dOutExt1, dOutExt2, dOutLaserPort, marker);
                marker.Ready(siriusEditorControl2.Document, siriusEditorControl2.View, rtcMultiBeam, laser, null);
            }

            // To get notification for 'RtcMultiBeamHelper.Modes' has changed
            RtcMultiBeamHelper.PropertyChanged += RtcMultiBeamHelper_PropertyChanged;
            RtcMultiBeamHelper_PropertyChanged(null, null);
        }

        private void RtcMultiBeamHelper_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RtcMultiBeamHelper.MultiBeamModes mode = RtcMultiBeamHelper.GetMode(multibeamPairIndex);
            lblMode.Text = $"Mode : {mode.ToString()}"; 
        }

        private void BtnCheckPins_Click(object sender, EventArgs e)
        {
            if (RtcMultiBeamHelper.CheckPins(multibeamPairIndex))
                System.Windows.Forms.MessageBox.Show(this, $"PIN CONNECTION ARE OK !", "INFORMATION", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            else
                System.Windows.Forms.MessageBox.Show(this, $"PIN CONNECTION ARE NOT OK !", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void BtnHeadNone_Click(object sender, EventArgs e)
        {
            // Safety State (Route laser to Beam Dump)
            // 안전 상태 (레이저를 빔 덤프 경로로 설정)
            // 安全状态（将激光引导至光束卸载区路径）
           if (!RtcMultiBeamHelper.SetMode(multibeamPairIndex, RtcMultiBeamHelper.MultiBeamModes.None))
               return;       
        }

        private void BtnHead1_Click(object sender, EventArgs e)
        {
            // Manual Head Selection (Switch AOM to Head 1 path)
            // 수동 헤드 선택 (AOM을 헤드 1 경로로 스위칭)
            // 手动选择头（将 AOM 切换到 1 号头路径）
            if (!RtcMultiBeamHelper.SetMode(multibeamPairIndex, RtcMultiBeamHelper.MultiBeamModes.Head1))
                return;
        }

        private void BtnHead2_Click(object sender, EventArgs e)
        {
            // Manual Head Selection (Switch AOM to Head 2 path)
            // 수동 헤드 선택 (AOM을 헤드 2 경로로 스위칭)
            // 手动选择头（将 AOM 切换到 2 号头路径）
            if (!RtcMultiBeamHelper.SetMode(multibeamPairIndex, RtcMultiBeamHelper.MultiBeamModes.Head2))
                return;
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
            if (!RtcMultiBeamHelper.SetMode(0, RtcMultiBeamHelper.MultiBeamModes.Both))
                return;
        }

        private void BtnReady_Click(object sender, EventArgs e)
        {
            RtcMultiBeamHelper.ReadyMode(multibeamPairIndex);
        }

        private async void BtnStart_Click(object sender, EventArgs e)
        {
            if (!RtcMultiBeamHelper.IsReady(multibeamPairIndex))
            {
                using var form = new SpiralLab.Sirius3.UI.WinForms.MessageBox(
                          $"Multibeam Pair is not ready yet. Try ready at first",
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
                    success &= await siriusEditorControl1.Marker?.Start();
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
                    success &= await siriusEditorControl2.Marker?.Start();
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
                    var task1 = siriusEditorControl1.Marker?.Start();
                    var task2 = siriusEditorControl2.Marker?.Start();
                    
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
                    success &= await siriusEditorControl1.Marker?.Stop();
                    break;
                case RtcMultiBeamHelper.MultiBeamModes.Head2:

                    success &= await siriusEditorControl2.Marker?.Stop();
                    break;
                case RtcMultiBeamHelper.MultiBeamModes.Both:
                    var t1 = siriusEditorControl1.Marker?.Stop();
                    var t2 = siriusEditorControl2.Marker?.Stop();

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
                    success &= siriusEditorControl1.Marker.Reset();
                    break;
                case RtcMultiBeamHelper.MultiBeamModes.Head2:
                    success &= siriusEditorControl2.Marker.Reset();
                    break;
                case RtcMultiBeamHelper.MultiBeamModes.Both:
                    success &= siriusEditorControl1.Marker.Reset();
                    success &= siriusEditorControl2.Marker.Reset();
                    break;
            }
        }
    }
}
