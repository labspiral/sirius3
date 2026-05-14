using System;

using SpiralLab.Sirius3.Document;
using SpiralLab.Sirius3.Scanner;
using SpiralLab.Sirius3.IO;
using SpiralLab.Sirius3.Scanner.Rtc;
using SpiralLab.Sirius3.PowerMeter;
using SpiralLab.Sirius3.Laser;
using SpiralLab.Sirius3.Marker;
using SpiralLab.Sirius3.Entity;
using System.Text;
using SpiralLab.Sirius3.Entity.Hatch;
using Microsoft.Extensions.Logging;
using SpiralLab.Sirius3;
using SpiralLab.Sirius3.Mathematics;
using SpiralLab.Sirius3.Scanner.Rtc.SyncAxis;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

#if OPENTK3
using OpenTK;
using DVec2 = OpenTK.Vector2d;
using DVec3 = OpenTK.Vector3d;
using DVec4 = OpenTK.Vector4d;
using DMat3 = OpenTK.Matrix3d;
using DMat4 = OpenTK.Matrix4d;
#elif OPENTK4
using OpenTK.Mathematics;
using DVec2 = OpenTK.Mathematics.Vector2d;
using DVec3 = OpenTK.Mathematics.Vector3d;
using DVec4 = OpenTK.Mathematics.Vector4d;
using DMat3 = OpenTK.Mathematics.Matrix3d;
using DMat4 = OpenTK.Mathematics.Matrix4d;
#endif

namespace Demos
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            // Initialize SIRIUS3 library
            SpiralLab.Sirius3.Core.Initialize();

            InitializeComponent();
            this.Load += Form1_Load;
            this.FormClosing += (s, e) =>
            {
                var dlgResult = MessageBox.Show(this, $"Do you really want to terminate program ?", "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dlgResult != DialogResult.Yes)
                {
                    // is marker need to abort ?
                    e.Cancel = true;
                    return;
                }
                // Dispose instances 
                siriusEditorControl1.DisposeDevices();

                // Dispose document
                var doc = siriusEditorControl1.Document;
                siriusEditorControl1.Document = null;
                doc?.Dispose();

                // Clean up SIRIUS3 library
                SpiralLab.Sirius3.Core.Cleanup();
            };
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Create devices
            CreateDevices();

            // Create entities for test
            CreateEntities();
        }


        void CreateDevices()
        {
            bool success = true;

            // 0 : first rtc card
            var index = 0;

            // scanner card controller
            var fov = 100.0;    // field of view (100mm)
            var kfactor = Math.Round(Math.Pow(2, 20) / fov); // kfactor = bits/mm (20bits resolution = 2^20 for RTC6)
            LaserModes laserMode = LaserModes.Yag1; // output signals timing for LASER1, LASER2 and LASER ON at RTC card
            RtcSignalLevels signalLevelLaser12 = RtcSignalLevels.ActiveHigh; // output signal level for LASER1 and LASER2 at RTC card
            RtcSignalLevels signalLevelLaserOn = RtcSignalLevels.ActiveHigh; // output signal level for LASER ON at RTC card
            string correctionPath = Path.Combine(SpiralLab.Sirius3.Config.CorrectionPath, "cor_1to1.ct5"); // *.ct5 for RTC5,6 card (*.ctb for RTC4 card)
            //var rtc = ScannerFactory.CreateRtc4(index, kfactor, laserMode, correctionPath);
            //var rtc = ScannerFactory.CreateRtc4Ethernet(index, "192,168,0,100", "255.255.255.0", kfactor, laserMode, correctionPath);
            //var rtc = ScannerFactory.CreateRtc5(index, kfactor, laserMode, signalLevelLaser12, signalLevelLaserOn, correctionPath);
            var rtc = ScannerFactory.CreateRtc6(index, kfactor, laserMode, signalLevelLaser12, signalLevelLaserOn, correctionPath); // create Rtc6 card instance
            //var rtc = ScannerFactory.CreateRtc6Ethernet(index, "192,168,0,100", "255.255.255.0", kfactor, laserMode, signalLevelLaser12, signalLevelLaserOn, correctionPath); // create Rtc6 card instance
            success &= rtc.Initialize(); // initialize the card
            Debug.Assert(success);

            // laser source device
            var laserMaxPower = 10.0; // laser max output power (W)
            var laser = LaserFactory.CreateVirtual(index, laserMaxPower, PowerControlMethods.Unknown); // create virtual laser instance for test purpose
            //var laser = LaserFactory.CreateVirtualAnalog(index, laserMaxPower, analog1, voltageMin, voltageMax); // create virtual analog output laser instance for test purpose
            //var laser = LaserFactory.CreateVirtualDutyCycle(index, laserMaxPower, dutyCycleMin, dutyCycleMax); // create virtual duty cycle output laser instance for test purpose
            //var laser = LaserFactory.CreateVirtualDO8Bits(index, laserMaxPower, dOut8Min, dOut8Max); // create virtual DO8Bits output laser instance for test purpose
            //var laser = LaserFactory.Create for target vender product ...
            laser.Scanner = rtc; // assign scanner instance to laser
            success &= laser.Initialize(); // initialize the laser
            Debug.Assert(success);

            // DIOs at RTC card 
            // D/O 16bit at extension1 port 
            var dIExt1 = IOFactory.CreateInputExtension1(rtc);
            success &= dIExt1.Initialize();
            Debug.Assert(success);
            siriusEditorControl1.DIExt1 = dIExt1;

            // D/I 16bit at extension1 port 
            var dOExt1 = IOFactory.CreateOutputExtension1(rtc);
            success &= dOExt1.Initialize();
            Debug.Assert(success);
            siriusEditorControl1.DOExt1 = dOExt1;

            // D/O 8bit at extension1 port 
            var dOExt2 = IOFactory.CreateOutputExtension2(rtc);
            success &= dOExt2.Initialize();
            Debug.Assert(success);
            siriusEditorControl1.DOExt2 = dOExt2;

            // Above RTC5 only
            // D/O 2bit at laser port 
            var dILaserPort = IOFactory.CreateInputLaserPort(rtc);
            success &= dILaserPort.Initialize();
            Debug.Assert(success);

            // Above RTC5 only
            // D/I 2bit at laser port 
            var dOLaserPort = IOFactory.CreateOutputLaserPort(rtc);
            success &= dOLaserPort.Initialize();
            Debug.Assert(success);
            //siriusEditorControl1.DOLaserPort = dOLaserPort;

            // powermeter device
            var powerMeter = PowerMeterFactory.CreateVirtual(index, laserMaxPower); // create virtual powermeter instance for test purpose
            //var powerMeter = PowerMeterFactory.CreateCoherentPowerMax(index, COMPORT);
            //var powerMeter = PowerMeterFactory.CreateOphirPhotonics(index, SERIALNO);
            //var powerMeter = PowerMeterFactory.CreateGentecEO(index, COMPORT);
            powerMeter.Laser = laser;
            success &= powerMeter.Initialize();
            Debug.Assert(success);

            // marker
            var marker = MarkerFactory.CreateRtc(index); // create marker instance 
            //var marker = MarkerFactory.CreateRtcFast(index);
            //var marker = MarkerFactory.CreateSyncAxis(index);
            success &= marker.Initialize();
            Debug.Assert(success);

            siriusEditorControl1.RegisterDevices(rtc, laser, powerMeter, dIExt1, dILaserPort, dOExt1, dOExt2, dOLaserPort, marker);
        }

        /// <summary>
        /// Create entities
        /// 엔티티 생성
        /// </summary>
        void CreateEntities()
        {
            var document = siriusEditorControl1.Document;
            //document.ActNew();

            // Create Arc
            // 호 생성
            {
                var entity = EntityFactory.CreateArc(DVec2.Zero, 1, 0, 360);
                document.ActAdd(entity);
            }

            // Create Line
            // 선 생성
            {
                var entity = EntityFactory.CreateLine(new DVec3(10, 1, 0), new DVec3(12, 10, 1));
                document.ActAdd(entity);
            }

            // Create Arc
            // 호 생성
            {
                var entity = EntityFactory.CreateArc(new DVec3(-15, 25, 0), 3, 0, 180);
                document.ActAdd(entity);
            }

            // Create Rectangle
            // 사각형 생성
            {
                var entity = EntityFactory.CreateRectangle(new DVec3(12, 8, 0), 3, 2);
                document.ActAdd(entity);
            }

            // Create Polyline
            // 폴리라인 생성
            {
                var rnd = new Random((int)DateTime.Now.Ticks);
                int count = 3 + (int)(rnd.NextDouble() * 5);
                var vertices = new List<Vertex2D>(count);
                for (int v = 0; v < count; v++)
                {
                    double x = rnd.NextDouble() * 50.0 - 25.0;
                    double y = rnd.NextDouble() * 50.0 - 25.0;
                    double b = rnd.NextDouble() * 0.2;
                    vertices.Add(new Vertex2D(x, y, b));
                }
                var entity = EntityFactory.CreatePolyline2D(vertices, true);
                document.ActAdd(entity);
            }

            // Create Text
            // 텍스트 생성
            {
                var entity = EntityFactory.CreateText("Arial", FontStyle.Regular, $"0123456789{Environment.NewLine}AaBbFfGgHhJj{Environment.NewLine}~!@#$%^&*()_+", 3);
                entity.Translate(5, -30);
                document.ActAdd(entity);
            }

            // Create QR Code
            // QR 코드 생성
            {
                var entity = EntityFactory.CreateQRCode("01234567890123456789", EntityBarcode2DBase.Barcode2DCells.Lines, 5, 5);
                entity.CellLine.DotFactor = 5;
                entity.Translate(10, 20);
                document.ActAdd(entity);
            }

            // Create Data Matrix
            // 데이터 매트릭스 생성
            {
                var entity = EntityFactory.CreateDataMatrix("01234567890123456789", EntityBarcode2DBase.Barcode2DCells.Squares, 10, 10);
                entity.CellDot.DotFactor = 1;
                entity.Translate(-10, 20);
                var hatch = HatchFactory.CreateLine(45, 0.02, 0.02);
                entity.AddHatch(hatch);
                entity.HatchMarkOption = HatchMarkOptions.HatchFirst;
                document.ActAdd(entity);
            }

            // Create Spiral
            // 나선 생성
            {
                var entity = EntityFactory.CreateSpiralClassic(DVec3.Zero, 10, 8, 2, 10, true);
                entity.Translate(-20, -30);
                document.ActAdd(entity);
            }
        }

        void StartMarker()
        {
            var document = siriusEditorControl1.Document;
            var marker = siriusEditorControl1.Marker;
            if (marker.IsBusy)
                return;

            marker.Reset();

            //var offsets = new List<Offset>();
            //offsets.Add(new Offset(-10, 0));
            //offsets.Add(new Offset(10, 0));
            //marker.Offsets = offsets.ToArray();

            marker.Start(document.Page);
        }

        void StopMarker()
        {
            var marker = siriusEditorControl1.Marker;
            marker.Stop();
        }
    }
}
