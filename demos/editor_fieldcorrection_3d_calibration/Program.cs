namespace Demos
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            SpiralLab.Sirius3.Core.Initialize();


            Application.Run(new Form1());


            SpiralLab.Sirius3.Core.Cleanup();
        }
    }
}