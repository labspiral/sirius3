using SpiralLab.Sirius3;

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

            // Initialize before creating the editor; clean up after releasing its GL resources.
            // 편집기를 생성하기 전에 초기화하고 GL 리소스를 해제한 후 정리합니다.
            if (!Core.Initialize()) return;
            try
            {
                using (var form = new Form1()) Application.Run(form);
            }
            finally { Core.Cleanup(); }
        }
    }
}
