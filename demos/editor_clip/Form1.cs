using System.Diagnostics;
using SpiralLab.Sirius3.IO;
using SpiralLab.Sirius3.Laser;
using SpiralLab.Sirius3.Marker;
using SpiralLab.Sirius3.PowerMeter;
using SpiralLab.Sirius3.Scanner.Rtc;
using SpiralLab.Sirius3.Entity;
using SpiralLab.Sirius3.UI.WinForms;

namespace Demos
{
    /// <summary>
    /// Loads geometry samples and clips selected entities through the shipped dialog with Undo/Redo.
    /// <para>형상 샘플을 불러오고 배포 대화상자를 통해 Undo/Redo를 지원하는 클리핑을 수행합니다.</para>
    /// </summary>
    public partial class Form1 : Form
    {
        private bool closing;

        /// <summary>Creates an editor whose devices are configured through the shared config.ini.
        /// <para>공유 config.ini로 장치를 설정하는 편집기를 생성합니다.</para></summary>
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Use the same config.ini device initialization as editor_dio.
            // editor_dio와 동일한 config.ini 장치 초기화 흐름을 사용합니다.
            EditorHelper.CreateDevices(out IRtc rtc, out ILaser laser, out IDInput dInExt1, out IDInput dInLaserPort, out IDOutput dOutExt1, out IDOutput dOutExt2, out IDOutput dOutLaserPort, out IPowerMeter powerMeter, out IMarker marker);

            editor.RegisterDevices(rtc, laser, powerMeter, dInExt1, dInLaserPort, dOutExt1, dOutExt2, dOutLaserPort, marker);

            marker.Ready(editor.Document, editor.View, rtc, laser, powerMeter);
        }

        private void Form1_Shown(object sender, EventArgs e) => samples.SelectedIndex = 0;
        private void Samples_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (samples.SelectedIndex >= 0) Run(LoadSample);
        }
        private void BtnDialog_Click(object sender, EventArgs e) => Run(ShowClipDialog, false);

        private IEntity[] ReadSelection()
        {
            var selected = editor.Document.Selected;
            if (selected.Length == 0 || selected.Any(e => !(e is IClippable) || e is EntityLayer))
                throw new ArgumentException("Select IClippable source entities in the editor or TreeView first.");
            // Do not clip a selected descendant again when its parent is selected.
            // 부모가 선택된 경우 선택된 자손을 중복 클리핑하지 않습니다.
            var set = new HashSet<IEntity>(selected);
            return selected.Distinct().Where(e =>
            {
                for (var owner = e.Owner; owner != null; owner = owner.Owner)
                    if (set.Contains(owner)) return false;
                return true;
            }).ToArray();
        }

        private void LoadSample()
        {
            var sample = (ClipSampleKinds)samples.SelectedIndex;
            var document = editor.Document;
            document.ActNew();
            var source = ClipSamples.Create(sample);
            bool submitted = false;
            try
            {
                source.Name = ClipSamples.Names[(int)sample];
                if (!source.Regen()) throw new InvalidOperationException("Sample regeneration failed.");
                // A valid detached entity transfers to the action, including rejection cleanup.
                // 유효한 독립 엔티티는 거절 시 정리를 포함하여 문서 작업에 전달합니다.
                submitted = true;
                if (!document.ActAdd(source)) throw new InvalidOperationException("Sample insertion failed.");
                document.ActSelect(source);
                document.ActUnReDoClear();
                ZoomFit();
            }
            finally { if (!submitted) source.Dispose(); }
        }

        private void ShowClipDialog()
        {
            // The shipped dialog owns its preview and supports multiple XY regions.
            // 배포 대화상자는 임시 미리보기를 소유하며 여러 XY 영역을 지원합니다.
            using (var dialog = new ClippingForm(editor.Document, ReadSelection(), editor.View))
            {
                dialog.ShowDialog(this);
            }
            RefreshView();
        }

        private void RefreshView() => editor.View?.DoRender();
        private void ZoomFit()
        {
            editor.View?.ActiveCamera?.ZoomFit(editor.View, editor.Document.ActivePage.ActiveLayer);
            RefreshView();
        }

        private void Run(Action action, bool waitCursor = true)
        {
            if (closing) return;
            bool previousWaitCursor = UseWaitCursor;
            try
            {
                if (editor.Document.IsSimulationWorking || editor.Marker?.IsBusy == true || !editor.View.IsAllowEdit)
                    throw new InvalidOperationException("Stop marking and simulation, and unlock the editor before loading samples or clipping geometry.");
                UseWaitCursor = waitCursor;
                action();
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception);
                System.Windows.Forms.MessageBox.Show(this, exception.Message, "Clipping demo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally { UseWaitCursor = previousWaitCursor; }
        }

        private async void CloseAsync(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            if (closing) return;
            closing = true;
            Enabled = false;
            await System.Threading.Tasks.Task.Yield();
            try { await editor.DisposeDevices(); }
            catch (Exception exception) { Trace.WriteLine(exception); }
            finally
            {
                FormClosing -= CloseAsync;
                Close();
            }
        }

        private void DisposeEditorDocument()
        {
            if (editor == null || editor.IsDisposed) return;
            var document = editor.Document;
            editor.Document = null;
            try { editor.Dispose(); }
            finally { document?.Dispose(); }
        }
    }
}
