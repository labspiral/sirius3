using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace Demos
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
#if NET8_0_OR_GREATER
            ApplicationConfiguration.Initialize();
#endif
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            string locale = "en-US";
            //string locale = "ko-KR";
            //string locale = "zh-CN";
            //string locale = "ja-JP";
            //string locale = "de-DE";
            var cultureInfo = new CultureInfo(locale);
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;

            SpiralLab.Sirius3.Core.Initialize();
            try
            {
                using (var form = new MainForm())
                    Application.Run(form);
            }
            finally
            {
                SpiralLab.Sirius3.Core.Cleanup();
            }
        }
    }
}
