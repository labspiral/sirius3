using System;
using System.Globalization;
using System.Threading;
using System.Windows;
using SpiralLab.Sirius3.UI.WPF;

namespace Demos
{
    /// <summary>WPF application host for the Sirius3 entity editor demo.</summary>
    public partial class App : Application
    {
        private bool coreInitialized;
        private Form1 editorWindow;

        /// <inheritdoc />
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            string locale = "en-US";
            //string locale = "ko-KR";
            //string locale = "zh-CN";
            //string locale = "ja-JP";
            //string locale = "de-DE";
            var culture = new CultureInfo(locale);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            try
            {
                // A false return means evaluation mode, not an unusable runtime.
                SpiralLab.Sirius3.Core.Initialize();
                coreInitialized = true;
                WPFThemeManager.Initialize();

                var window = new Form1();
                editorWindow = window;
                MainWindow = window;
                window.Show();
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    exception.ToString(),
                    "Sirius3 initialization failed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Shutdown(-1);
            }
        }

        /// <inheritdoc />
        protected override void OnExit(ExitEventArgs e)
        {
            try
            {
                // Show/startup failures may bypass the window's Closed event.
                editorWindow?.DisposeEditor();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Sirius3 cleanup failed",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            try
            {
                if (coreInitialized)
                {
                    coreInitialized = false;
                    SpiralLab.Sirius3.Core.Cleanup();
                }
            }
            finally
            {
                base.OnExit(e);
            }
        }
    }
}
