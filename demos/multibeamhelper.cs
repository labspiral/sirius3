/*
 * 
 *                                                            ,--,      ,--,                              
 *             ,-.----.                                     ,---.'|   ,---.'|                              
 *   .--.--.   \    /  \     ,---,,-.----.      ,---,       |   | :   |   | :      ,---,           ,---,.  
 *  /  /    '. |   :    \ ,`--.' |\    /  \    '  .' \      :   : |   :   : |     '  .' \        ,'  .'  \ 
 * |  :  /`. / |   |  .\ :|   :  :;   :    \  /  ;    '.    |   ' :   |   ' :    /  ;    '.    ,---.' .' | 
 * ;  |  |--`  .   :  |: |:   |  '|   | .\ : :  :       \   ;   ; '   ;   ; '   :  :       \   |   |  |: | 
 * |  :  ;_    |   |   \ :|   :  |.   : |: | :  |   /\   \  '   | |__ '   | |__ :  |   /\   \  :   :  :  / 
 *  \  \    `. |   : .   /'   '  ;|   |  \ : |  :  ' ;.   : |   | :.'||   | :.'||  :  ' ;.   : :   |    ;  
 *   `----.   \;   | |`-' |   |  ||   : .  / |  |  ;/  \   \'   :    ;'   :    ;|  |  ;/  \   \|   :     \ 
 *   __ \  \  ||   | ;    '   :  ;;   | |  \ '  :  | \  \ ,'|   |  ./ |   |  ./ '  :  | \  \ ,'|   |   . | 
 *  /  /`--'  /:   ' |    |   |  '|   | ;\  \|  |  '  '--'  ;   : ;   ;   : ;   |  |  '  '--'  '   :  '; | 
 * '--'.     / :   : :    '   :  |:   ' | \.'|  :  :        |   ,/    |   ,/    |  :  :        |   |  | ;  
 *   `--'---'  |   | :    ;   |.' :   : :-'  |  | ,'        '---'     '---'     |  | ,'        |   :   /   
 *             `---'.|    '---'   |   |.'    `--''                              `--''          |   | ,'    
 *               `---`            `---'                                                        `----'   
 * 
 * 2026 Copyright to (c)SpiralLAB. All rights reserved.
 * Description : Configuration multibeam helper by ini file for Demo editor projects
 * Author : hong chan, choi / hcchoi@spirallab.co.kr (http://spirallab.co.kr)
 * 
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using SpiralLab.Sirius3.IO;
using SpiralLab.Sirius3.Scanner.Rtc;
using SpiralLab.Sirius3.Marker;
using SpiralLab.Sirius3.PowerMeter;
using SpiralLab.Sirius3.Laser;
using SpiralLab.Sirius3.Scanner;
using SpiralLab.Sirius3.Scanner.Rtc.SyncAxis;
using SpiralLab.Sirius3.PowerMap;
using SpiralLab.Sirius3.Document;
using SpiralLab.Sirius3.Entity;
using SpiralLab.Sirius3.Entity.Hatch;
using SpiralLab.Sirius3.Remote;
using SpiralLab.Sirius3.UI.WinForms;

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

// Cannot convert null literal to non-nullable reference type.
#pragma warning disable CS8625 

namespace Demos
{
    /// <summary>
    /// Demo editor helper
    /// </summary>
    public static class EditorHelper
    {
        /// <summary>
        /// Your config ini file
        /// </summary>
        public static string ConfigFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config_multibeam.ini");

        /// <summary>
        /// Create devices (like as <c>IScanner</c>, <c>ILaser</c>, <c>IMarker</c>, <c>IPowerMeter</c>, ...)
        /// </summary>
        /// <param name="index">Index (assign value if using multiple devices) (0,1,2,...)</param>
        /// <param name="multiBeamIndex">Multibeam index (0,1,2,3)
        /// <br/>
        /// Pair0: 0, 1 <br/>
        /// Pair1: 2, 3 <br/>
        /// Pair2: 4, 5 <br/>
        /// Pair3: 6, 7 <br/>
        /// </param>
        /// <param name="laser"><c>ILaser</c></param>
        /// <param name="rtcMultiBeam"><c>IRtcMultiBeam</c></param>
        /// <param name="dInExt1">RTC D.Input EXTENSION1 port</param>
        /// <param name="dInLaserPort">RTC D.Input LASER port</param>
        /// <param name="dOutExt1">RTC D.Output EXTENSION1 port</param>
        /// <param name="dOutExt2">RTC D.Output EXTENSION2 port</param>
        /// <param name="dOutLaserPort">RTC D.Output LASER port</param>
        /// <param name="powerMeter"><c>IPowerMeter</c></param>
        /// <param name="marker"><c>IMarker</c></param>
        /// <returns>Success or failed</returns>
        public static bool CreateDevices(int index, int multiBeamIndex, ILaser laser, out IRtcMultiBeam rtcMultiBeam, out IDInput dInExt1, out IDInput dInLaserPort, out IDOutput dOutExt1, out IDOutput dOutExt2, out IDOutput dOutLaserPort, out IPowerMeter powerMeter, out IMarker marker)
        {
            rtcMultiBeam = null;
            dInExt1 = null;
            dInLaserPort = null;
            dOutExt1 = null;
            dOutExt2 = null;
            dOutLaserPort = null;         
            powerMeter = null;
            marker = null;

            bool success = true;

            #region Initialize RTC controller
            string rtcType = NativeMethods.ReadIni(ConfigFileName, $"RTC{index}", "TYPE", "rtcvirtualmultimeam");
            int rtcId = NativeMethods.ReadIni<int>(ConfigFileName, $"RTC{index}", "ID", index);

            // FOV size (mm)
            var fov = NativeMethods.ReadIni<double>(ConfigFileName, $"RTC{index}", "FOV", 100.0);
            // Resolution : bits/mm (= kfactor)
            // RTC5,6 using 20 bits resolution
            var kfactor = Math.Pow(2, 20) / fov;
            // Field correction file path: \correction\cor_1to1.ct5
            // Default (1:1) correction file
            string correctionFile = NativeMethods.ReadIni(ConfigFileName, $"RTC{index}", "CORRECTION", "cor_1to1.ct5");
            string correctionPath = Path.Combine(SpiralLab.Sirius3.Config.CorrectionPath, correctionFile);
            RtcSignalLevels signalLevelLaser12 = NativeMethods.ReadIni(ConfigFileName, $"RTC{index}", "SIGNALLEVEL_LASER12", "High") == "High" ? RtcSignalLevels.ActiveHigh : RtcSignalLevels.ActiveLow;
            RtcSignalLevels signalLevelLaserOn = NativeMethods.ReadIni(ConfigFileName, $"RTC{index}", "SIGNALLEVEL_LASERON", "High") == "High" ? RtcSignalLevels.ActiveHigh : RtcSignalLevels.ActiveLow;
            string sLaserMode = NativeMethods.ReadIni(ConfigFileName, $"RTC{index}", "LASERMODE", "Yag1");
            LaserModes laserMode = (LaserModes)Enum.Parse(typeof(LaserModes), sLaserMode);
            
            string ipAddress, subnetMask;
            switch (rtcType.Trim().ToLower())
            {
                default:
                case "rtcvirtualmultimeam":
                    rtcMultiBeam = ScannerFactory.CreateRtcVirtualMultiBeam(index, multiBeamIndex, kfactor, laserMode, signalLevelLaser12, signalLevelLaserOn, correctionPath); 
                    break;
                case "rtc4multibeam":
                    rtcMultiBeam = ScannerFactory.CreateRtc4MultiBeam(index, multiBeamIndex, kfactor, laserMode, correctionPath);
                    break;
                case "rtc5multibeam":
                    rtcMultiBeam = ScannerFactory.CreateRtc5MultiBeam(index, multiBeamIndex, kfactor, laserMode, signalLevelLaser12, signalLevelLaserOn, correctionPath);
                    break;
                case "rtc6multibeam":
                    rtcMultiBeam = ScannerFactory.CreateRtc6MultiBeam(index, multiBeamIndex, kfactor, laserMode, signalLevelLaser12, signalLevelLaserOn, correctionPath);
                    break;
                case "rtc6ethernetmultibeam":
                    ipAddress = NativeMethods.ReadIni(ConfigFileName, $"RTC{index}", "IP_ADDRESS", "192.168.0.100");
                    subnetMask = NativeMethods.ReadIni(ConfigFileName, $"RTC{index}", "SUBNET_MASK", "255.255.255.0");
                    rtcMultiBeam = ScannerFactory.CreateRtc6EthernetMultiBeam(index, multiBeamIndex, ipAddress, subnetMask, kfactor, laserMode, signalLevelLaser12, signalLevelLaserOn, correctionPath);
                    break;
            }

            // Initialize RTC controller
            var rtc = rtcMultiBeam as IRtc;
            success &= rtc.Initialize();
            Debug.Assert(success);

            rtcMultiBeam.TokenBitMask = (ushort)(0x01 << NativeMethods.ReadIni(ConfigFileName, $"RTC{index}", "TOKEN_BIT", 0));
            rtcMultiBeam.AOMBitMask = (ushort)(0x01 << NativeMethods.ReadIni(ConfigFileName, $"RTC{index}", "AOM_BIT", 1));
            rtcMultiBeam.AOMChannel = NativeMethods.ReadIni<ExtensionChannels>(ConfigFileName, $"RTC{index}", "AOM_CHANNEL", ExtensionChannels.ExtAO1);
            rtcMultiBeam.AOM0OrderVoltage = NativeMethods.ReadIni(ConfigFileName, $"RTC{index}", "AOM_0_ODR_V", 0);
            rtcMultiBeam.AOM1stOrderVoltage = NativeMethods.ReadIni(ConfigFileName, $"RTC{index}", "AOM_1_ODR_V", 5);
            rtcMultiBeam.AOMHoldMsec = NativeMethods.ReadIni(ConfigFileName, $"RTC{index}", "AOM_HOLD_MSEC", 0.01);


            double fpk = NativeMethods.ReadIni(ConfigFileName, $"RTC{index}", "FPK", 0.0);
            double qSwitchDelay = NativeMethods.ReadIni(ConfigFileName, $"RTC{index}", "QSWITCH_DELAY", 0.0);

            success &= rtc.CtlFirstPulseKiller(fpk);

            if (rtc is IRtcExtension rtcExtension)
                success &= rtcExtension.CtlQSwitchDelay(qSwitchDelay);

            // Create GPIO at RTC 
            // 16 bits input at extension 1 port
            dInExt1 = IOFactory.CreateInputExtension1(rtc);
            // 16 bits output at extension 1 port
            dOutExt1 = IOFactory.CreateOutputExtension1(rtc);
            success &= dInExt1.Initialize();
            success &= dOutExt1.Initialize();
        
            // 8 bits output at extension 2 port
            dOutExt2 = IOFactory.CreateOutputExtension2(rtc);
            // 2 bits input at laser port
            dInLaserPort = IOFactory.CreateInputLaserPort(rtc);
            // 2 bits output at laser port
            dOutLaserPort = IOFactory.CreateOutputLaserPort(rtc);

            success &= dInLaserPort.Initialize();
            success &= dOutExt2.Initialize();
            success &= dOutLaserPort.Initialize();


            // Set FOV area: WxH, it will be drawn as red square
            //siriusEditorControl1.EditorCtrl.View.FovArea = new DVec3(200, 200, 0);
            //SpiralLab.Sirius3.Winforms.Config.ViewFovSize = new SizeF(fov, fov);

            // To check out of range for jump and mark x,y locations
            //rtc.FieldSizeLimit = new SizeF(fov, fov);
         
            // Default frequency and pulse width: 50KHz, 2 usec 
            success &= rtc.CtlFrequency(50 * 1000, 2);
            // Default jump and mark speed: 500 mm/s
            success &= rtc.CtlSpeed(500, 500);

            rtc.OnCorrectionTable += Rtc_OnCorrectionTable;
            #endregion

            #region Initialize Powermeter
            var enablePowerMeter = NativeMethods.ReadIni<int>(ConfigFileName, $"POWERMETER{index}", "ENABLE", 0);
            if (0 != enablePowerMeter)
            {
                var powerMeterType = NativeMethods.ReadIni(ConfigFileName, $"POWERMETER{index}", "TYPE", "Virtual");
                var powerMeterSerialNo = NativeMethods.ReadIni(ConfigFileName, $"POWERMETER{index}", "SERIAL_NO", string.Empty);
                var powerMeterCOMPort = NativeMethods.ReadIni<int>(ConfigFileName, $"POWERMETER{index}", "SERIAL_PORT", 0);
                switch (powerMeterType.Trim().ToLower())
                {
                    default:
                    case "virtual":
                        var laserVirtualMaxPower = NativeMethods.ReadIni<double>(ConfigFileName, $"LASER{index}", "MAXPOWER", 10);
                        powerMeter = PowerMeterFactory.CreateVirtual(index, laserVirtualMaxPower);
                        break;
                    case "ophirphotonics":
                        powerMeter = PowerMeterFactory.CreateOphirPhotonics(index, powerMeterSerialNo);
                        break;
                    case "coherentpowermax":
                        powerMeter = PowerMeterFactory.CreateCoherentPowerMax(index, powerMeterCOMPort);
                        break;
                    case "thorlabs":
                        if (powerMeterCOMPort > 0)
                        {
                            // by COM port communication
                            powerMeter = PowerMeterFactory.CreateThorlabs(index, powerMeterCOMPort);
                        }
                        else
                        {
                            // by USB communication
                            powerMeter = PowerMeterFactory.CreateThorlabs(index, powerMeterSerialNo);
                        }
                        break;
                    case "gentec-eo":
                        powerMeter = PowerMeterFactory.CreateGentecEO(index, powerMeterCOMPort);
                        break;
                }
                success &= powerMeter.Initialize();
                // uncomment to auto start 
                //success &= powerMeter.CtlStart();
            }
            #endregion

            #region Initialize PowerMap 
            var powerControl = laser as ILaserPowerControl;
            var enablePowerMap = NativeMethods.ReadIni<int>(ConfigFileName, $"LASER{index}", "POWERMAP_ENABLE", 0);
            if (0 != enablePowerMap)
            {
                var powerMap = PowerMapFactory.CreateDefault(index, $"MAP{index}");
                
                powerMap.OnOpened += PowerMap_OnMappingOpened;
                powerMap.OnSaved += PowerMap_OnMappingSaved;
                var powerMapFile = NativeMethods.ReadIni<string>(ConfigFileName, $"LASER{index}", "POWERMAP_FILE", string.Empty);
                var powerMapFullPath = Path.Combine(SpiralLab.Sirius3.Config.PowerMapPath, powerMapFile);
                if (File.Exists(powerMapFullPath))
                    success &= PowerMapSerializer.Open(powerMapFullPath, powerMap);
                else
                {
                    //reset as 1 to 1 if you want
                    //powerMap.Reset1to1("10000", laser.MaxPowerWatt);
                    //powerMap.Reset1to1("50000", laser.MaxPowerWatt);
                }
                if (null != powerControl)
                {
                    powerControl.PowerMap = powerMap;
                    // Enable lookup powermap table 
                    powerMap.IsEnableLookUp = true;
                }
            }
            // Assign RTC into laser source
            laser.Scanner = rtc;
            // Initialize laser source
            success &= laser.Initialize();

            // Set Default Power
            var laserMaxPower = laser.MaxPowerWatt;
            var laserDefaultPower = NativeMethods.ReadIni<double>(ConfigFileName, $"LASER{index}", "DEFAULT_POWER", laserMaxPower * 0.05);
            if (null != powerControl)
                success &= powerControl.CtlPower(laserDefaultPower);
            #endregion

            #region Marker
            switch (rtcType.Trim().ToLower())
            {
                default:
                case "rtcvirtualmultibeam":
                    marker = MarkerFactory.CreateVirtual(index);
                    break;
                case "rtc4multibeam":
                case "rtc5multibeam":
                case "rtc6multibeam":
                case "rtc6ethernetmultibeam":
                    marker = MarkerFactory.CreateRtc(index);
                    //marker = MarkerFactory.CreateRtcFast(index);
                    //or your custom marker
                    break;
            }
            success &= marker.Initialize();
            #endregion


            #region Multibeam Helper
            //  RTC <-> RTC PIN connection (for token bit) are valid ?
            //success &= RtcMultiBeamHelper.CheckPins(0);
            //Debug.Assert(success);
            #endregion

            return success;
        }

        /// <summary>
        /// Create laser source by ini file configuration
        /// </summary>
        /// <param name="index"></param>
        /// <param name="laser"></param>
        /// <returns></returns>
        public static bool CreateLaser(int index, out ILaser laser)
        {
            laser = null;

            bool success = true;

            #region Initialize Laser source
            var laserType = NativeMethods.ReadIni(ConfigFileName, $"LASER{index}", "TYPE", "Virtual");
            var laserMaxPower = NativeMethods.ReadIni<double>(ConfigFileName, $"LASER{index}", "MAXPOWER", 10);
            var laserCOMPort = NativeMethods.ReadIni<int>(ConfigFileName, $"LASER{index}", "COM_PORT", 1);
            var laserIPaddress = NativeMethods.ReadIni<string>(ConfigFileName, $"LASER{index}", "IP_ADDRESS", string.Empty);
            var rtcAnalogPort = NativeMethods.ReadIni<int>(ConfigFileName, $"LASER{index}", "ANALOG_PORT", 1);
            var virtuaLaserPowerControl = NativeMethods.ReadIni(ConfigFileName, $"LASER{index}", "POWERCONTROL", "Unknown");
            switch (laserType.Trim().ToLower())
            {
                default:
                case "virtual":
                    switch (virtuaLaserPowerControl.Trim().ToLower())
                    {
                        default:
                        case "unknown":
                            laser = LaserFactory.CreateVirtual(index, laserMaxPower, PowerControlMethods.Unknown);
                            break;
                        case "analog1":
                            {
                                var voltageMin = NativeMethods.ReadIni<double>(ConfigFileName, $"LASER{index}", "POWERCONTROL_VOLTAGE_MIN", 0);
                                var voltageMax = NativeMethods.ReadIni<double>(ConfigFileName, $"LASER{index}", "POWERCONTROL_VOLTAGE_MAX", 10);
                                laser = LaserFactory.CreateVirtualAnalog(index, laserMaxPower, 1, voltageMin, voltageMax);
                            }
                            break;
                        case "analog2":
                            {
                                var voltageMin = NativeMethods.ReadIni<double>(ConfigFileName, $"LASER{index}", "POWERCONTROL_VOLTAGE_MIN", 0);
                                var voltageMax = NativeMethods.ReadIni<double>(ConfigFileName, $"LASER{index}", "POWERCONTROL_VOLTAGE_MAX", 10);
                                laser = LaserFactory.CreateVirtualAnalog(index, laserMaxPower, 2, voltageMin, voltageMax);
                            }
                            break;
                        case "frequency":
                            var freqMin = NativeMethods.ReadIni<double>(ConfigFileName, $"LASER{index}", "POWERCONTROL_FREQUENCY_MIN", 40000);
                            var freqMax = NativeMethods.ReadIni<double>(ConfigFileName, $"LASER{index}", "POWERCONTROL_FREQUENCY_MAX", 50000);
                            laser = LaserFactory.CreateVirtualFrequency(index, laserMaxPower, freqMin, freqMax);
                            break;
                        case "dutycycle":
                            var dutyCycleMin = NativeMethods.ReadIni<double>(ConfigFileName, $"LASER{index}", "POWERCONTROL_DUTYCYCLE_MIN", 0);
                            var dutyCycleMax = NativeMethods.ReadIni<double>(ConfigFileName, $"LASER{index}", "POWERCONTROL_DUTYCYCLE_MAX", 99);
                            laser = LaserFactory.CreateVirtualDutyCycle(index, laserMaxPower, dutyCycleMin, dutyCycleMax);
                            break;
                        case "digitalbits16":
                            var dOut16Min = NativeMethods.ReadIni<ushort>(ConfigFileName, $"LASER{index}", "POWERCONTROL_DO16_MIN", 0);
                            var dOut16Max = NativeMethods.ReadIni<ushort>(ConfigFileName, $"LASER{index}", "POWERCONTROL_DO16_MAX", 65535);
                            laser = LaserFactory.CreateVirtualDO16Bits(index, laserMaxPower, dOut16Min, dOut16Max);
                            break;
                        case "digitalbits8":
                            var dOut8Min = NativeMethods.ReadIni<ushort>(ConfigFileName, $"LASER{index}", "POWERCONTROL_DO8_MIN", 0);
                            var dOut8Max = NativeMethods.ReadIni<ushort>(ConfigFileName, $"LASER{index}", "POWERCONTROL_DO8_MAX", 255);
                            laser = LaserFactory.CreateVirtualDO8Bits(index, laserMaxPower, dOut8Min, dOut8Max);
                            break;
                    }
                    break;
                case "advancedoptowaveaopico":
                    laser = LaserFactory.CreateAdvancedOptoWaveAOPico(index, $"LASER{index}", laserCOMPort, laserMaxPower);
                    break;
                case "advancedoptowaveaopicoprecision":
                    laser = LaserFactory.CreateAdvancedOptoWaveAOPicoPrecision(index, $"LASER{index}", laserCOMPort, laserMaxPower);
                    break;
                case "advancedoptowavefotia":
                    laser = LaserFactory.CreateAdvancedOptoWaveFotia(index, $"LASER{index}", laserCOMPort, laserMaxPower);
                    break;
                case "coherentavialx":
                    laser = LaserFactory.CreateCoherentAviaLX(index, $"LASER{index}", laserCOMPort, laserMaxPower);
                    break;
                case "coherentdiamondcseries":
                    laser = LaserFactory.CreateCoherentDiamondCSeries(index, $"LASER{index}", laserMaxPower);
                    break;
                case "ipgylptyped":
                    laser = LaserFactory.CreateIPGYLPTypeD(index, $"LASER{index}", laserCOMPort, laserMaxPower);
                    break;
                case "ipgylptypee":
                    laser = LaserFactory.CreateIPGYLPTypeE(index, $"LASER{index}", laserCOMPort, laserMaxPower);
                    break;
                case "ipgylpulpn":
                    laser = LaserFactory.CreateIPGYLPULPN(index, $"LASER{index}", laserCOMPort, laserMaxPower);
                    break;
                case "ipgylpn":
                    laser = LaserFactory.CreateIPGYLPN(index, $"LASER{index}", laserCOMPort, laserMaxPower, rtcAnalogPort);
                    break;
                case "jpttypee":
                    laser = LaserFactory.CreateJPTTypeE(index, $"LASER{index}", laserCOMPort, laserMaxPower);
                    break;
                case "photonicsindustrydx":
                    laser = LaserFactory.CreatePhotonicsIndustryDX(index, $"LASER{index}", laserCOMPort, laserMaxPower);
                    break;
                case "photonicsindustryrghaio":
                    laser = LaserFactory.CreatePhotonicsIndustryRGHAIO(index, $"LASER{index}", laserCOMPort, laserMaxPower);
                    break;
                case "spectraphysicshippo":
                    laser = LaserFactory.CreateSpectraPhysicsHippo(index, $"LASER{index}", laserCOMPort, laserMaxPower);
                    break;
                case "spectraphysicstalon":
                    laser = LaserFactory.CreateSpectraPhysicsTalon(index, $"LASER{index}", laserCOMPort, laserMaxPower);
                    break;
                case "spig4":
                    laser = LaserFactory.CreateSPIG4(index, $"LASER{index}", laserCOMPort, laserMaxPower);
                    break;
            }

            var laserPowerControlDelay = NativeMethods.ReadIni<double>(ConfigFileName, $"LASER{index}", "POWERCONTROL_DELAY", 0);
            laser.PowerControlDelayTime = laserPowerControlDelay;

            success &= laser.Initialize();
            #endregion

            return success;
        }

        /// <summary>
        /// Create remote device
        /// </summary>
        /// <param name="marker"><c>IMarker</c></param>
        /// <param name="remote">Created <c>IRemote</c></param>
        /// <param name="index">Index (assign value if using multiple devices) (0,1,2,...)</param>
        /// <returns>Success or failed</returns>
        public static bool CreateRemote(IMarker marker, out IRemote remote, int index = 0)
        {
            remote = null;

            bool success = true;

            #region Remote
            //var enableRemote = NativeMethods.ReadIni<int>(ConfigFileName, $"REMOTE{index}", "ENABLE", 0);
            //if (0 != enableRemote)
            {
                string protocol = NativeMethods.ReadIni<string>(ConfigFileName, $"REMOTE{index}", $"PROTOCOL", "tcpip");
                switch (protocol.ToLower().Trim())
                {
                    default:
                    case "virtual":
                        remote = RemoteFactory.CreateVirtual(index, "Virtual", marker);
                        break;
                    case "tcp":
                    case "tcpip":
                        int tcpPort = NativeMethods.ReadIni<int>(ConfigFileName, $"REMOTE{index}", $"TCP_PORT", 5001);
                        remote = RemoteFactory.CreateTcpServer(index, "TCP/IP", marker, tcpPort);
                        break;
                    case "rs232":
                    case "rs232c":
                    case "serial":
                        int serialPort = NativeMethods.ReadIni<int>(ConfigFileName, $"REMOTE{index}", $"SERIAL_PORT", 1);
                        int serialBaudRate = NativeMethods.ReadIni<int>(ConfigFileName, $"REMOTE{index}", $"SERIAL_BAUDRATE", 57600);
                        remote = RemoteFactory.CreateSerial(index, "Serial", marker, serialPort, serialBaudRate);
                        break;
                    case "web":
                    case "websocket":
                        string prefix = NativeMethods.ReadIni<string>(ConfigFileName, $"REMOTE{index}", $"WEB_PREFIX", "http://*:8080");
                        remote = RemoteFactory.CreateWebSocket(index, "Web", marker, new string[] { prefix });
                        break;
                    case "mqtt":
                    case "iot":
                        string brokerAddress = NativeMethods.ReadIni<string>(ConfigFileName, $"REMOTE{index}", $"BROKER_ADDRESS", "127.0.0.1");
                        int brokerPort = NativeMethods.ReadIni<int>(ConfigFileName, $"REMOTE{index}", $"BROKER_PORT", 1883);
                        string topicSubject = NativeMethods.ReadIni<string>(ConfigFileName, $"REMOTE{index}", $"TOPIC_SUBJECT", "sirius3/cmd");
                        string topicPublish = NativeMethods.ReadIni<string>(ConfigFileName, $"REMOTE{index}", $"TOPIC_PUBLISH", "sirius3/response");
                        remote = RemoteFactory.CreateMqtt(index, "Web", marker, brokerAddress, brokerPort, topicSubject, topicPublish);
                        break;
                }

                //success = await remote.Start();
                _ = remote.Start();
            }
            #endregion

            return success;
        }

        private static void Rtc_OnCorrectionTable(IRtc rtc, CorrectionTables correctionTable, string fileName)
        {
            //if (correctionTable == CorrectionTables.Table1)
            //{
            //    var index = rtc.Index;
            //    var fileNameOnly = Path.GetFileName(fileName);
            //    NativeMethods.WriteIni<string>(ConfigFileName, $"RTC{index}", "CORRECTION", fileNameOnly);
            //}
        }

        internal static void PowerMap_OnMappingOpened(IPowerMap powerMap, string fileName)
        {
            //var index = powerMap.Index;
            //var name = Path.GetFileName(fileName);
            //NativeMethods.WriteIni<string>(ConfigFileName, $"LASER{index}", "POWERMAP_FILE", name);
            // ...
        }

        internal static void PowerMap_OnMappingSaved(IPowerMap powerMap, string fileName)
        {
            var index = powerMap.Index;
            // File path should be in "powermap\"
            var fileNameOnly = Path.GetFileName(fileName);
            //NativeMethods.WriteIni<string>(ConfigFileName, $"LASER{index}", "POWERMAP_FILE", fileNameOnly);
            NativeMethods.WriteIni<string>(ConfigFileName, $"LASER0", "POWERMAP_FILE", fileNameOnly);
        }

    }
}
