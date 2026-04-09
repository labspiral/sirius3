using System.Globalization;

using SpiralLab.Sirius3.Document;
using SpiralLab.Sirius3.Entity;
using SpiralLab.Sirius3.Entity.Hatch;
using SpiralLab.Sirius3.IO;
using SpiralLab.Sirius3.Laser;
using SpiralLab.Sirius3.Marker;
using SpiralLab.Sirius3.PowerMeter;
using SpiralLab.Sirius3.Scanner.Rtc;

namespace Demos
{
    internal static class Program
    {

        static IDocument document;
        static IRtc rtc;
        static ILaser laser;
        static IDInput dInExt1;
        static IDInput dInLaserPort;
        static IDOutput dOutExt1;
        static IDOutput dOutExt2;
        static IDOutput dOutLaserPort;
        static IPowerMeter powerMeter;
        static IMarker marker;

        static bool terminated = false;


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

            SpiralLab.Sirius3.Core.Initialize();

            CreateDocument();
        
            CreateDevices();

            CreateEntities();

            while (!terminated)
            {
                Console.WriteLine("");
                Console.WriteLine("Console Document Demo - (c)SpiralLAB");
                Console.WriteLine("1. Marker start");
                Console.WriteLine("2. Marker stop");
                Console.WriteLine("3. Open document");
                Console.WriteLine("Q. Quit");
                Console.Write("Select Item : ");

                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.D1:
                        CreateMarkerStart();
                        break;
                    case ConsoleKey.D2:
                        CreateMarkerStop();
                        break;
                    case ConsoleKey.D3:
                        var dlg = new OpenFileDialog();
                        dlg.Title = "Open Document File";
                        dlg.Filter = "sirius3 file (*.sirius3)|*.sirius3";
                        dlg.DefaultExt = "sirius3";
                        DialogResult result = dlg.ShowDialog();
                        if (result != DialogResult.OK)
                            break;
                        document.ActOpen(dlg.FileName);
                        document.ActRegen();
                        marker?.Ready(document);
                        break;
                    case ConsoleKey.Q:
                        terminated = true;
                        break;
                }
                Console.WriteLine("");
            }
            CreateMarkerStop();

            DestroyDevices();
            SpiralLab.Sirius3.Core.Cleanup();
        }

        /// <summary>
        /// Create Document
        /// </summary>
        static void CreateDocument()
        {
            document?.Dispose();
            document = DocumentFactory.CreateDefault();
        }

        /// <summary>
        /// Create Devices
        /// </summary>
        static void CreateDevices()
        {
            //DestroyDevices();
            EditorHelper.CreateDevices(out rtc, out laser, out dInExt1, out dInLaserPort, out dOutExt1, out dOutExt2, out dOutLaserPort, out powerMeter, out marker);

            marker.OnStarted -= Marker_OnStarted;
            marker.OnStarted += Marker_OnStarted;
            marker.OnEnded -= Marker_OnEnded;
            marker.OnEnded += Marker_OnEnded;

            marker.Ready(document, null, rtc, laser, powerMeter);
        }

        /// <summary>
        /// Create Sample Entities
        /// </summary>
        static void CreateEntities()
        {
            {
                var entity = EntityFactory.CreateDataMatrix("0123456789", EntityBarcode2DBase.Barcode2DCells.Dots, 10, 10);
                entity.CellDot.DotFactor = 2;
                entity.IsReversed = true;
                entity.Translate(0, 10);
                document.ActivePage?.ActiveLayer?.AddChild(entity);
            }

            {
                var entity = new EntitySiriusText("ocra.cxf", EntitySiriusText.LetterSpaces.Variable, 0.2, 0.5, 1, "0123456789", 2);
                var hatch = HatchFactory.CreateLine(90, 0.1);
                hatch.Joint = HatchJoints.Miter;
                hatch.Exclude = 0.05;
                hatch.IsZigZag = true;
                hatch.Order = HatchOrders.Descending;
                hatch.Sort = HatchSorts.Nearest;
                entity.HatchMarkOption = HatchMarkOptions.HatchFirst;
                entity.AddHatch(hatch);
                entity.Translate(0, -12.5);
                document.ActivePage?.ActiveLayer?.AddChild(entity);
            }

            document.ActRegen();
        }

        /// <summary>
        /// Marker Start
        /// </summary>
        static void CreateMarkerStart()
        {
            marker?.Reset();
            marker?.Ready(document);
            marker?.Start();
        }

        /// <summary>
        /// Event fired when marker has started
        /// </summary>
        /// <param name="marker"></param>
        private static void Marker_OnStarted(IMarker marker)
        {
            Console.Title = "Started ...";
        }

        /// <summary>
        /// Event fired when marker has ended
        /// </summary>
        /// <param name="marker"></param>
        /// <param name="success"></param>
        /// <param name="timeSpan"></param>
        private static void Marker_OnEnded(IMarker marker, bool success, TimeSpan? timeSpan)
        {
            Console.Title = "Ended !";
        }

        /// <summary>
        /// Marker stop
        /// </summary>
        static void CreateMarkerStop()
        {
            marker?.Stop();
        }

        /// <summary>
        /// Destory Devices
        /// </summary>
        static void DestroyDevices()
        {
            marker?.Dispose();
            marker = null;
            dInExt1?.Dispose();
            dInExt1 = null;
            dInLaserPort?.Dispose();
            dInLaserPort = null;
            dOutExt1?.Dispose();
            dOutExt1 = null;
            dOutExt2?.Dispose();
            dOutExt2 = null;
            dOutLaserPort?.Dispose();
            dOutLaserPort = null;
            powerMeter?.Dispose();
            powerMeter = null;
            laser?.Dispose();
            laser = null;
            rtc?.Dispose();
            rtc= null;
        }
    }
}