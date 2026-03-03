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
using SpiralLab.Sirius3.UI.WinForms;
using SpiralLab.Sirius3;
using System.Diagnostics;

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

        const int editorCount = 2;
        SiriusEditorControl[] EditorControls = new SiriusEditorControl[editorCount];
        IRtcMultiBeam[] RtcMultiBeams = new IRtcMultiBeam[editorCount];

        public Form1()
        {
            InitializeComponent();

            EditorControls[0] = siriusEditorControl1;
            EditorControls[1] = siriusEditorControl2;

            this.Load += Form1_Load;
            this.Disposed += Form1_Disposed;

            //this.BtnStartStop.Click += BtnStartStop_Click;
        }

        private void Form1_Disposed(object sender, EventArgs e)
        {
            for (int i = 0; i < editorCount; i++)
                EditorHelper.DestroyDevices(EditorControls[i]);
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            //Need to 2 instances and multibeam option at library option.
            //Core.License(out var licenseInfo);
            //Debug.Assert(licenseInfo.RtcLicenseMax == 2);
            //Debug.Assert(licenseInfo.IsMultiBeamLicensed);
            
            ILaser laser = null;

            CreateMultiBeamDevices(0, out var rtcMultiBeam1, out IMarker marker1);
            RtcMultiBeams[0] = rtcMultiBeam1;
            var rtc1 = rtcMultiBeam1 as IRtc;
            CreateLaser(out laser, rtc1);
            EditorControls[0].Scanner = rtc1;
            EditorControls[0].Laser = laser;
            EditorControls[0].Marker = marker1;
            marker1.Ready(EditorControls[0].Document, EditorControls[0].View, rtc1, laser, null);

            CreateMultiBeamDevices(1, out var rtcMultiBeam2,  out IMarker marker2);
            RtcMultiBeams[1] = rtcMultiBeam2;
            var rtc2 = rtcMultiBeam2 as IRtc;
            EditorControls[1].Scanner = rtc2;
            EditorControls[1].Laser = laser; //same laser source
            EditorControls[1].Marker = marker2;
            marker1.Ready(EditorControls[1].Document, EditorControls[1].View, rtc2, laser, null);

            // System Block Diagram
            //
            // 1. Laser beam path
            //  LASER -------> AOM1 -----------------------------> AOM2 -----------> DUMP
            //  SOURCE           .                0차              .         0차    
            //                   1차 .                       .  1차   
            //                           .                .  
            //                               .        .
            //                                   .. 
            //                               .        .
            //                           .                .
            //                       .                        .
            //                  HEAD1                           HEAD2              
            //
            // 2. Each head path
            //  +------------+-----------+-----------+-------------------------------------------+
            //    Target     | AOM1      | AOM2      | 최종 빔 상태 및 논리                       
            //  +------------+-----------+-----------+-------------------------------------------+
            //    HEAD1      | 0차 (OFF) | 1차 (ON)  | AOM1 직진, AOM2 회절->HEAD1, 누설광->Dump    
            //    HEAD2      | 1차 (ON)  | 0차 (OFF) | AOM1 회절- HEAD2, 누설광->Dump             
            //    Beam Dump  | 0차 (OFF) | 0차 (OFF) | 모든 광로 직진->최종 Beam Dump              
            //  +------------+-----------+-----------+-------------------------------------------+
            //  
            // 3. RTC EXTENSION 16 DIO PORT 
            //             R  T  C  1                    R  T  C  2        
            //    AO       DI      DO                    DI      DO          AO
            //    |   .---->0       0-------------------->0       0------.   |
            //    |   |    WAIT    NEXT                  WAIT    NEXT    |   |
            //    |   |                                                  |   |
            //    |   ----------------------------------------------------   |
            //    |   .---->1       1-------------------->1       1------.   |
            //    |   |    ACK     ACK                   ACK     ACK     |   |
            //    |   |                                                  |   |
            //    |   ----------------------------------------------------   |
            //    |         2       2-----------.         2       2------.   |
            //    |                             |                        |   |
            //    |         3       3           |         3       3      |   |
            //    |                             -------------------.     |   |
            //    |                                                |     |   |
            //    |                 |------------------------------+------   |
            //    |                 v                              v         |
            //    |         D.IN(1st ORDER)               D.IN(1st ORDER)    |
            //    --------->AOM RF DRIVER 1               AOM RF DRIVER 2<----
            //                     |                             |  
            //                   HEAD1                         HEAD2

            // for 1st SCAN Head with Rtc0 
            RtcMultiBeams[0].TokenWaitBitMask = 0b_0000_0000_0000_0001;
            RtcMultiBeams[0].TokenAckBitMask = 0b_0000_0000_0000_0010;
            RtcMultiBeams[0].AOMBitMask = 0b_0000_0000_0000_0100;
            RtcMultiBeams[0].AOMChannel = ExtensionChannels.ExtAO1;
            RtcMultiBeams[0].AOM0OrderVoltage = 0;
            var approxMaxWatt1 = laser.MaxPowerWatt * 0.98 * 0.85;
            RtcMultiBeams[0].AOM1stOrderVoltage = 5.0;
            RtcMultiBeams[0].AOMHoldMsec = 0.01; // 10usec


            // for 2nd SCAN Head with Rtc1 
            RtcMultiBeams[1].TokenWaitBitMask = 0b_0000_0000_0000_0001;
            RtcMultiBeams[1].TokenAckBitMask = 0b_0000_0000_0000_0010;
            RtcMultiBeams[1].AOMBitMask = 0b_0000_0000_0000_0100;
            RtcMultiBeams[1].AOMChannel = ExtensionChannels.ExtAO1;
            RtcMultiBeams[1].AOM0OrderVoltage = 0;
            var approxMaxWatt2 = laser.MaxPowerWatt * 0.85;
            double approxEfficient = (approxMaxWatt2 / laser.MaxPowerWatt);
            RtcMultiBeams[1].AOM1stOrderVoltage = 5.0 * approxEfficient;
            RtcMultiBeams[1].AOMHoldMsec = 0.01; // 10usec
        }

        bool CreateMultiBeamDevices(int index, out IRtcMultiBeam rtcMultiBeam, out IMarker marker)
        {
            rtcMultiBeam = null;
            marker = null;

            bool success = true;

            // scanner card controller
            var fov = 100.0;    // field of view (100mm)
            var kfactor = Math.Pow(2, 20) / fov; // kfactor = bits/mm (20bits resolution = 2^20 for RTC6)
            LaserModes laserMode = LaserModes.Yag1; // output signals timing for LASER1, LASER2 and LASER ON at RTC card
            RtcSignalLevels signalLevelLaser12 = RtcSignalLevels.ActiveHigh; // output signal level for LASER1 and LASER2 at RTC card
            RtcSignalLevels signalLevelLaserOn = RtcSignalLevels.ActiveHigh; // output signal level for LASER ON at RTC card
            string correctionPath = Path.Combine(SpiralLab.Sirius3.Config.CorrectionPath, "cor_1to1.ct5"); // *.ct5 for RTC5,6 card (*.ctb for RTC4 card)
            //var rtc = ScannerFactory.CreateRtc5MultiBeam(index, kfactor, laserMode, signalLevelLaser12, signalLevelLaserOn, correctionPath, index); // create Rtc6 card instance
            var rtc = ScannerFactory.CreateRtc6MultiBeam(index, kfactor, laserMode, signalLevelLaser12, signalLevelLaserOn, correctionPath, index); // create Rtc6 card instance
            //var rtc = ScannerFactory.CreateRtcVirtualMultiBeam(index, kfactor, laserMode, signalLevelLaser12, signalLevelLaserOn, correctionPath, index); // create Rtc6 card instance
            success &= rtc.Initialize(); // initialize the card
            Debug.Assert(success);
            rtcMultiBeam = rtc;

            //var dIExt1 = IOFactory.CreateInputExtension1(rtc);
            //var dOExt1 = IOFactory.CreateOutputExtension1(rtc);
            //var dOExt2 = IOFactory.CreateOutputExtension2(rtc);
            //var dILaserPort = IOFactory.CreateInputLaserPort(rtc);
            //var dOLaserPort = IOFactory.CreateOutputLaserPort(rtc);

            // powermeter device
            //var powerMeter = PowerMeterFactory.CreateVirtual(index, laserMaxPower); // create virtual powermeter instance for test purpose
            //var powerMeter = PowerMeterFactory.CreateCoherentPowerMax(index, COMPORT);
            //var powerMeter = PowerMeterFactory.CreateOphirPhotonics(index, SERIALNO);
            //var powerMeter = PowerMeterFactory.CreateGentecEO(index, COMPORT);
            //success &= powerMeter.Initialize();
            //Debug.Assert(success);

            // marker
            marker = MarkerFactory.CreateRtc(index); // create marker instance 
            return success;
        }

        bool CreateLaser(out ILaser laser, IRtc rtc)
        {
            bool success = true;

            // laser source device
            var laserMaxPower = 10.0; // laser max output power (W)
            laser = LaserFactory.CreateVirtual(0, laserMaxPower); // create virtual laser instance for test purpose
            //var laser = LaserFactory.CreateVirtualAnalog(index, laserMaxPower, analog1, voltageMin, voltageMax); // create virtual analog output laser instance for test purpose
            //var laser = LaserFactory.CreateVirtualDutyCycle(index, laserMaxPower, dutyCycleMin, dutyCycleMax); // create virtual duty cycle output laser instance for test purpose
            //var laser = LaserFactory.CreateVirtualDO8Bits(index, laserMaxPower, dOut8Min, dOut8Max); // create virtual DO8Bits output laser instance for test purpose
            //var laser = LaserFactory.Create for target vender product ...

            laser.Scanner = rtc; // assign scanner instance to laser
            success &= laser.Initialize(); // initialize the laser
            Debug.Assert(success);
            
            return success;
        }

        private void BtnStartStop_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < editorCount; i++)
            {
                var document = EditorControls[i].Document;
                var marker = EditorControls[i].Marker;

                if (marker.IsBusy)
                {
                    marker.Stop();
                    marker.Reset();
                }
                else
                {
                    marker.Reset();
                    marker.Ready(siriusEditorControl1.Document);

                    // Start to mark current page
                    marker.Start(document.Page);
                }
            }
        }

    }
}
