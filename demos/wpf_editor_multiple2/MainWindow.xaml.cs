using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using SpiralLab.Sirius3.Document;
using SpiralLab.Sirius3.IO;
using SpiralLab.Sirius3.Laser;
using SpiralLab.Sirius3.Marker;
using SpiralLab.Sirius3.PowerMeter;
using SpiralLab.Sirius3.Scanner.Rtc;

namespace Demos
{
    /// <summary>Hosts one WPF editor that switches between two device systems.</summary>
    public partial class MainWindow : Window
    {
        private const int DeviceCount = 2;
        private bool devicesInitialized;
        private bool initializationFailed;
        private bool editorDisposed;

        /// <summary>Initializes the WPF multi-device editor demo window.</summary>
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
                multiEditor.MaxDeviceCounts = DeviceCount;

                for (var index = 0; index < DeviceCount; index++)
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
                        out IMarker marker,
                        index);

                    multiEditor.RegisterDevices(
                        index,
                        rtc, laser, powerMeter,
                        diExt1, diLaserPort,
                        doExt1, doExt2, doLaserPort,
                        marker);
                }

                if (!multiEditor.SwitchDevices(0))
                    throw new InvalidOperationException("The first device set could not be selected.");

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

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
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

            try { DisposeEditor(); }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Sirius3 cleanup failed",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            try { DisposeEditor(); }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Sirius3 cleanup failed",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        internal void DisposeEditor()
        {
            if (editorDisposed)
                return;
            editorDisposed = true;

            var document = multiEditor.Document;
            var failures = new List<Exception>();
            try { multiEditor.DisposeDevices(); }
            catch (Exception exception) { failures.Add(exception); }
            try { multiEditor.Dispose(); }
            catch (Exception exception) { failures.Add(exception); }
            try { document?.Dispose(); }
            catch (Exception exception) { failures.Add(exception); }

            if (failures.Count > 0)
                throw new AggregateException("Editor cleanup failed.", failures);
        }
    }
}
