using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

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

            Application.Run(new Form1());
        }
    }
}