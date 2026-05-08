using System.Diagnostics;
using System.Globalization;

using SpiralLab.Sirius3.Document;
using SpiralLab.Sirius3.Entity;
using SpiralLab.Sirius3.Entity.Hatch;
using SpiralLab.Sirius3.IO;
using SpiralLab.Sirius3.Laser;
using SpiralLab.Sirius3.Marker;
using SpiralLab.Sirius3.PowerMeter;
using SpiralLab.Sirius3.Scanner.Rtc;
using SpiralLab.Sirius3.UI.WinForms;

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

            ApplyPenValues();

            while (!terminated)
            {
                Console.WriteLine("");
                Console.WriteLine("Console Document Demo - (c)SpiralLAB");
                Console.WriteLine("1. Marker start");
                Console.WriteLine("2. Marker stop");
                Console.WriteLine("3. Open document");
                Console.WriteLine("4. Save document");
                Console.WriteLine("V. Viewer");
                Console.WriteLine("E. Editor");
                Console.WriteLine("Q. Quit");
                Console.Write("Select Item : ");

                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.D1:
                        Console.WriteLine("");
                        CreateMarkerStart();
                        break;
                    case ConsoleKey.D2:
                        Console.WriteLine("");
                        CreateMarkerStop();
                        break;
                    case ConsoleKey.D3:
                        {
                            Console.WriteLine("");
                            var ofd = new OpenFileDialog();
                            ofd.Title = "Open Sirius3 File";
                            ofd.Filter = "sirius3 file (*.sirius3)|*.sirius3";
                            ofd.DefaultExt = "sirius3";
                            DialogResult result = ofd.ShowDialog();
                            if (result != DialogResult.OK)
                                break;
                            document.ActOpen(ofd.FileName);
                            document.ActRegen();
                            marker?.Ready(document);
                        }
                        break;
                    case ConsoleKey.D4:
                        {
                            Console.WriteLine("");
                            var sfd = new SaveFileDialog();
                            sfd.Title = "Save Sirius3 File";
                            sfd.Filter = "sirius3 file (*.sirius3)|*.sirius3";
                            sfd.DefaultExt = "sirius3";
                            DialogResult result = sfd.ShowDialog();
                            if (result != DialogResult.OK)
                                break;
                            document.ActSave(sfd.FileName);
                        }
                        break;

                    case ConsoleKey.V:
                        {
                            Console.WriteLine("");
                            var dynamicForm = new Form();
                            dynamicForm.SuspendLayout();
                            dynamicForm.AutoScaleDimensions = new SizeF(6F, 13F);
                            dynamicForm.AutoScaleMode = AutoScaleMode.Font;
                            dynamicForm.Font = new Font("Segoe UI", 8.25F);
                            dynamicForm.Text = "ViewerControl - (c)SpiralLab";
                            dynamicForm.Size = new Size(800, 600);
                            dynamicForm.StartPosition = FormStartPosition.WindowsDefaultLocation;

                            var viewerControl = new SpiralLab.Sirius3.UI.WinForms.ViewerControl();
                            viewerControl.AliasName = "MyView";
                            viewerControl.Dock = DockStyle.Fill;
                            viewerControl.Document = document;
                            dynamicForm.Controls.Add(viewerControl);
                            dynamicForm.ResumeLayout(false);

                            Application.Run(dynamicForm);
                        }
                        break;

                    case ConsoleKey.E:
                        {
                            Console.WriteLine("");
                            var dynamicForm = new Form();
                            dynamicForm.SuspendLayout();
                            dynamicForm.AutoScaleDimensions = new SizeF(6F, 13F);
                            dynamicForm.AutoScaleMode = AutoScaleMode.Font;
                            dynamicForm.Font = new Font("Segoe UI", 8.25F);
                            dynamicForm.Text = "EditorControl - (c)SpiralLab";
                            dynamicForm.Size = new Size(800, 600);
                            dynamicForm.StartPosition = FormStartPosition.WindowsDefaultLocation;

                            var editorControl = new SpiralLab.Sirius3.UI.WinForms.EditorControl();
                            editorControl.Dock = DockStyle.Fill;
                            editorControl.Document = document;
                            dynamicForm.Controls.Add(editorControl);
                            dynamicForm.ResumeLayout(false);

                            Application.Run(dynamicForm);
                        }
                        break;

                    case ConsoleKey.Q:
                        terminated = true;
                        break;
                }
                Console.WriteLine("");
            }
            CreateMarkerStop();

            DestroyDevices();

            document?.Dispose();

            SpiralLab.Sirius3.Core.Cleanup();
        }

        /// <summary>
        /// Create Document
        /// </summary>
        static void CreateDocument()
        {
            if (null != document)
            {
                document.OnAfterOpen -= Document_OnAfterOpen;
                document.OnAfterSave -= Document_OnAfterSave;
            }

            document?.Dispose();
            document = DocumentFactory.CreateDefault();

            document.OnAfterOpen += Document_OnAfterOpen;
            document.OnAfterSave += Document_OnAfterSave;
        }

        private static void Document_OnAfterSave(IDocument document, string fileName)
        {
            Console.Title = $"Saved: {fileName}";
        }

        private static void Document_OnAfterOpen(IDocument document, string fileName)
        {
            Console.Title = $"Opened: {fileName}";
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
            var color = SpiralLab.Sirius3.UI.Config.EntityPenColors[0];// Color.White; 

            {
                var entity = EntityFactory.CreateDataMatrix("0123456789", EntityBarcode2DBase.Barcode2DCells.Dots, 10, 10);
                entity.PenColor = color;  
                entity.CellDot.DotFactor = 2;
                entity.IsReversed = true;
                entity.Translate(0, 10);
                document.ActAdd(entity);
            }

            {
                var entity = new EntitySiriusText("ocra.cxf", EntitySiriusText.LetterSpaces.Variable, 0.2, 0.5, 1, "0123456789", 2);
                entity.PenColor = color;
                var hatch = HatchFactory.CreateLine(90, 0.1);
                entity.PenColor = Color.White;
                hatch.Joint = HatchJoints.Miter;
                hatch.Exclude = 0.05;
                hatch.IsZigZag = true;
                hatch.Order = HatchOrders.Descending;
                hatch.Sort = HatchSorts.Nearest;
                entity.HatchMarkOption = HatchMarkOptions.HatchFirst;
                entity.AddHatch(hatch);
                entity.Translate(0, -12.5);
                document.ActAdd(entity);
            }

            document.ActRegen();
        }

        static void ApplyPenValues()
        {
            {
                var color = SpiralLab.Sirius3.UI.Config.EntityPenColors[0];// Color.White; 

                bool success = document.FindByEntityPenColor(color, out var pen);
                Debug.Assert(success);

                pen.Frequency = 50_000;
                pen.PulseWidth = 2;
                pen.Power = 2.0;
                pen.JumpSpeed = 500;
                pen.MarkSpeed = 500;

                pen.RasterMode = RasterModes.JumpAndShoot;
                pen.PixelTime = 200;
            }

            {
                var color = document.ActivePage.ActiveLayer.PenColor;
                bool success = document.FindByLayerPenColor(color, out var pen);
                Debug.Assert(success);

                pen.IsSkyWritingEnabled = false;

                pen.IsALC = false;

                pen.IsVariablePolygonDelay = true;
                pen.VariablePolygonDelayEdgeLevel = 100;
            }
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
            Console.Title = $"Started ...";
        }

        /// <summary>
        /// Event fired when marker has ended
        /// </summary>
        /// <param name="marker"></param>
        /// <param name="success"></param>
        /// <param name="timeSpan"></param>
        private static void Marker_OnEnded(IMarker marker, bool success, TimeSpan? timeSpan)
        {
            if (success)
                Console.Title = $"Ended : {timeSpan.Value.TotalSeconds:F1} sec";
            else
                Console.Title = $"Failed";
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