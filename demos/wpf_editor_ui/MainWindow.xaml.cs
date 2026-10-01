using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using SpiralLab.Sirius3.Document;
using SpiralLab.Sirius3.IO;
using SpiralLab.Sirius3.Laser;
using SpiralLab.Sirius3.Marker;
using SpiralLab.Sirius3.PowerMeter;
using SpiralLab.Sirius3.Scanner.Rtc;

namespace Demos
{
    /// <summary>Hosts the public-source WPF editor with one virtual device system.</summary>
    public partial class MainWindow : Window
    {
        private bool devicesInitialized;
        private bool initializationFailed;
        private bool editorDisposed;
        private bool shutdownStarted;
        private bool closeAfterShutdown;

        /// <summary>Initializes the public-source WPF editor demo window.</summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (devicesInitialized || initializationFailed || editorDisposed)
                return;

            try
            {
                EditorHelper.CreateDevices(
                    out IRtc rtc,
                    out ILaser laser,
                    out IDInput diExt1,
                    out IDInput diLaserPort,
                    out IDOutput doExt1,
                    out IDOutput doExt2,
                    out IDOutput doLaserPort,
                    out IPowerMeter powerMeter,
                    out IMarker marker);

                editor.RegisterDevices(
                    rtc, laser, powerMeter,
                    diExt1, diLaserPort,
                    doExt1, doExt2, doLaserPort,
                    marker);

                devicesInitialized = true;
            }
            catch (Exception exception)
            {
                initializationFailed = true;
                MessageBox.Show(this, exception.ToString(), "Device initialization failed",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
            }
        }

        private async void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (closeAfterShutdown) return;
            e.Cancel = true;
            if (shutdownStarted) return;
            if (!initializationFailed && MessageBox.Show(
                    this,
                    "Do you really want to terminate the program?",
                    "WARNING",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) != MessageBoxResult.Yes)
            {
                e.Cancel = true;
                return;
            }

            shutdownStarted = true;
            await Task.Yield();
            try { await DisposeEditorAsync(); }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Sirius3 cleanup failed",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                closeAfterShutdown = true;
                Close();
            }
        }

        internal async Task DisposeEditorAsync()
        {
            if (editorDisposed)
                return;
            editorDisposed = true;

            var document = editor.Document;
            var failures = new List<Exception>();
            try { await editor.DisposeDevices(); }
            catch (Exception exception) { failures.Add(exception); }
            try { editor.Dispose(); }
            catch (Exception exception) { failures.Add(exception); }
            try { document?.Dispose(); }
            catch (Exception exception) { failures.Add(exception); }

            if (failures.Count > 0)
                throw new AggregateException("Editor cleanup failed.", failures);
        }
    }
}
