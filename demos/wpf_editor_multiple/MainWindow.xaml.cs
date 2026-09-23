using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using SpiralLab.Sirius3.Document;
using SpiralLab.Sirius3.IO;
using SpiralLab.Sirius3.Laser;
using SpiralLab.Sirius3.Marker;
using SpiralLab.Sirius3.PowerMeter;
using SpiralLab.Sirius3.Scanner.Rtc;
using SpiralLab.Sirius3.UI.WPF;

namespace Demos
{
    /// <summary>Hosts two independent WPF editors and device systems.</summary>
    public partial class MainWindow : Window
    {
        private const int EditorCount = 2;
        private readonly SiriusEditorControl[] editors;
        private bool devicesInitialized;
        private bool initializationFailed;
        private bool editorsDisposed;

        /// <summary>Initializes the WPF multiple-editor demo window.</summary>
        public MainWindow()
        {
            InitializeComponent();
            editors = new[] { editor1, editor2 };
        }

        private void EditorTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!ReferenceEquals(e.Source, editorTabs) || editor1 == null || editor2 == null)
                return;

            var selectedIndex = editorTabs.SelectedIndex;
            editor1.Visibility = selectedIndex == 0 ? Visibility.Visible : Visibility.Collapsed;
            editor2.Visibility = selectedIndex == 1 ? Visibility.Visible : Visibility.Collapsed;
            if (selectedIndex < 0 || selectedIndex >= EditorCount)
                return;
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(() =>
            {
                if (!editorsDisposed && selectedIndex == editorTabs.SelectedIndex)
                    editors[selectedIndex].View?.DoRender();
            }));
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (devicesInitialized || initializationFailed || editorsDisposed)
                return;

            try
            {
                for (var index = 0; index < EditorCount; index++)
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

                    editors[index].RegisterDevices(
                        rtc, laser, powerMeter,
                        diExt1, diLaserPort,
                        doExt1, doExt2, doLaserPort,
                        marker);
                }

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

            try { DisposeEditors(); }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Sirius3 cleanup failed",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            try { DisposeEditors(); }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Sirius3 cleanup failed",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        internal void DisposeEditors()
        {
            if (editorsDisposed)
                return;
            editorsDisposed = true;

            var failures = new List<Exception>();
            var documents = editors
                .Select(editor => editor.Document)
                .Where(document => document != null)
                .Distinct()
                .ToArray();

            foreach (var editor in editors)
            {
                try { editor.DisposeDevices(); }
                catch (Exception exception) { failures.Add(exception); }
            }
            foreach (var editor in editors)
            {
                try { editor.Dispose(); }
                catch (Exception exception) { failures.Add(exception); }
            }
            foreach (IDocument document in documents)
            {
                try { document.Dispose(); }
                catch (Exception exception) { failures.Add(exception); }
            }

            if (failures.Count > 0)
                throw new AggregateException("Editor cleanup failed.", failures);
        }
    }
}
