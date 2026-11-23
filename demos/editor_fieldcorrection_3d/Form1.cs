using Microsoft.Extensions.Logging;
using SpiralLab.Sirius3;
using SpiralLab.Sirius3.Scanner.Rtc;
using SpiralLab.Sirius3.UI.WinForms;

namespace Demos
{
    public partial class Form1 : Form
    {
        private bool closing;
        private bool cleanupComplete;

        public Form1()
        {
            InitializeComponent();
            Load += InitializeDevices;
            FormClosing += DisposeEditorDevices;
            btn3DCorrection.Click += OpenCalibration;
        }

        private void InitializeDevices(object sender, EventArgs args)
        {
            try
            {
                if (!File.Exists(EditorHelper.ConfigFileName))
                {
                    ShowWarning($"Device configuration was not found:\n{EditorHelper.ConfigFileName}");
                    return;
                }

                bool initialized = EditorHelper.CreateDevices(out var rtc, out var laser, out var dInExt1, out var dInLaserPort,
                    out var dOutExt1, out var dOutExt2, out var dOutLaserPort, out var powerMeter, out var marker);
                if (!initialized || marker == null)
                {
                    marker?.Dispose();
                    dInExt1?.Dispose();
                    dInLaserPort?.Dispose();
                    dOutExt1?.Dispose();
                    dOutExt2?.Dispose();
                    dOutLaserPort?.Dispose();
                    powerMeter?.Dispose();
                    laser?.Dispose();
                    rtc?.Dispose();
                    ShowWarning("Device initialization failed. Check the demo configuration and log.");
                    return;
                }

                siriusEditorControl1.RegisterDevices(rtc, laser, powerMeter, dInExt1, dInLaserPort, dOutExt1, dOutExt2, dOutLaserPort, marker);
                if (rtc is not IRtc3D || !rtc.Is3D || !rtc.IsReady || !ReferenceEquals(marker.Scanner, rtc))
                {
                    ShowWarning("3D calibration requires a ready scanner and the RTC 3D option.");
                    return;
                }

                btn3DCorrection.Enabled = true;
            }
            catch (Exception exception)
            {
                Logger.Log(LogLevel.Error, exception, "Could not initialize the 3D calibration demo.");
                ShowWarning("Device initialization failed. Check the demo configuration and log.");
            }
        }

        private void OpenCalibration(object sender, EventArgs args)
        {
            if (closing)
                return;
            var marker = siriusEditorControl1.Marker;
            if (marker?.Scanner is not IRtc rtc || rtc is not IRtc3D || !rtc.Is3D || !rtc.IsReady ||
                rtc.PrimaryHeadTable == CorrectionTables.None)
            {
                ShowWarning("Select a correction table on a ready 3D RTC before opening 3D calibration.");
                return;
            }
            if (marker.IsBusy || rtc.IsBusy)
            {
                ShowWarning("Wait until the marker and RTC are idle before opening 3D calibration.");
                return;
            }

            using var calibration = new Correction3DRtcForm(marker);
            calibration.ShowDialog(this);
        }

        private async void DisposeEditorDevices(object sender, FormClosingEventArgs args)
        {
            if (cleanupComplete)
                return;
            args.Cancel = true;
            if (closing)
                return;

            closing = true;
            btn3DCorrection.Enabled = false;
            await Task.Yield();
            try
            {
                await siriusEditorControl1.DisposeDevices();
            }
            catch (Exception exception)
            {
                Logger.Log(LogLevel.Error, exception, "Could not dispose the 3D calibration demo devices.");
            }
            finally
            {
                var document = siriusEditorControl1.Document;
                siriusEditorControl1.Document = null;
                document?.Dispose();
                cleanupComplete = true;
                Close();
            }
        }

        private void ShowWarning(string message) =>
            System.Windows.Forms.MessageBox.Show(this, message, "3D Calibration", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }
}
