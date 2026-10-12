using System.Globalization;

namespace Demos
{
    internal static class Program
    {
        [STAThread]
        static void Main()
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

            if (!SpiralLab.Sirius3.Core.Initialize())
                return;
            try
            {
                // Use Form2 for multiple device sets.
                using (var form = new Form1())
                    Application.Run(form);
            }
            finally
            {
                SpiralLab.Sirius3.Core.Cleanup();
            }
        }
    }
}