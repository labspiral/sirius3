using System.Globalization;

namespace Demos
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
#if NET8_0_OR_GREATER
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
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

            // single devices set
            //Application.Run(new Form1());
            
            //or for multiple devices set
            Application.Run(new Form2());

            SpiralLab.Sirius3.Core.Cleanup();
        }
    }
}