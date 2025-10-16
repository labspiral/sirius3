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
 * 2025 Copyright to (c)SpiralLAB. All rights reserved.
 * Description : editor helper
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
    /// <summary>
    /// Demo editor helper
    /// </summary>
    public static class EditorHelper
    {
        /// <summary>
        /// Your config ini file
        /// </summary>
        public static string ConfigFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini");
        // Config file for XL-SCAN (syncAXIS)
        //public static string ConfigFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config_syncaxis.ini");

        /// <summary>
        /// Create devices (like as <c>IScanner</c>, <c>ILaser</c>, ...)
        /// </summary>
        /// <param name="rtc"><c>IScanner</c></param>
        /// <param name="laser"><c>ILaser</c></param>
        /// <param name="dInExt1">RTC D.Input EXTENSION1 port</param>
        /// <param name="dInLaserPort">RTC D.Input LASER port</param>
        /// <param name="dOutExt1">RTC D.Output EXTENSION1 port</param>
        /// <param name="dOutExt2">RTC D.Output EXTENSION2 port</param>
        /// <param name="dOutLaserPort">RTC D.Output LASER port</param>
        /// <param name="powerMeter"><c>IPowerMeter</c></param>
        /// <param name="marker"><c>IMarker</c></param>
        /// <param name="index">Index (assign value if using multiple devices) (0,1,2,...)</param>
        /// <returns>Success or failed</returns>
        public static bool CreateDevices(out IRtc rtc, out ILaser laser, out IDInput dInExt1, out IDInput dInLaserPort, out IDOutput dOutExt1, out IDOutput dOutExt2, out IDOutput dOutLaserPort, out IPowerMeter powerMeter, out IMarker marker, int index = 0)
        {
            rtc = null;
            dInExt1 = null;
            dInLaserPort = null;
            dOutExt1 = null;
            dOutExt2 = null;
            dOutLaserPort = null;
            laser = null;
            powerMeter = null;
            marker = null;

            bool success = true;

            #region Initialize RTC controller
            string rtcType = NativeMethods.ReadIni(ConfigFileName, $"RTC{index}", "TYPE", "Rtc6");

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
                case "virtual":
                    rtc = ScannerFactory.CreateVirtual(index, kfactor, laserMode, signalLevelLaser12, signalLevelLaserOn, correctionPath);
                    break;
                case "rtc4":
                    // RTC4 using 16 bits resolution
                    kfactor = Math.Pow(2, 16) / fov;
                    correctionFile = NativeMethods.ReadIni(ConfigFileName, $"RTC{index}", "CORRECTION", "cor_1to1.ctb");
                    rtc = ScannerFactory.CreateRtc4(index, kfactor, laserMode, correctionPath);
                    break;
                case "rtc4e":
                    ipAddress = NativeMethods.ReadIni(ConfigFileName, $"RTC{index}", "IP_ADDRESS", "192.168.0.100");
                    subnetMask = NativeMethods.ReadIni(ConfigFileName, $"RTC{index}", "SUBNET_MASK", "255.255.255.0");
                    rtc = ScannerFactory.CreateRtc4Ethernet(index, ipAddress, subnetMask, kfactor, laserMode, correctionPath);
                    break;
                case "rtc5":
                    rtc = ScannerFactory.CreateRtc5(index, kfactor, laserMode, signalLevelLaser12, signalLevelLaserOn, correctionPath);
                    break;
                case "rtc6":
                    rtc = ScannerFactory.CreateRtc6(index, kfactor, laserMode, signalLevelLaser12, signalLevelLaserOn, correctionPath);
                    break;
                case "rtc6e":
                    ipAddress = NativeMethods.ReadIni(ConfigFileName, $"RTC{index}", "IP_ADDRESS", "192.168.0.100");
                    subnetMask = NativeMethods.ReadIni(ConfigFileName, $"RTC{index}", "SUBNET_MASK", "255.255.255.0");
                    rtc = ScannerFactory.CreateRtc6Ethernet(index, ipAddress, subnetMask, kfactor, laserMode, signalLevelLaser12, signalLevelLaserOn, correctionPath);
                    break;
                case "syncaxis":
                    string configXmlFileName = NativeMethods.ReadIni(ConfigFileName, $"RTC{index}", "CONFIG_XML", string.Empty);
                    string configXmlFilePath = Path.Combine(SpiralLab.Sirius3.Config.SyncAxisPath, configXmlFileName);
                    rtc = ScannerFactory.CreateRtc6SyncAxis(index, configXmlFilePath);
                    break;
            }

            // Initialize RTC controller
            success &= rtc.Initialize();
            Debug.Assert(success);

            // Load 2nd correction file into TABLE2 if assigned
            string correction2File = NativeMethods.ReadIni(ConfigFileName, $"RTC{index}", "CORRECTION2", string.Empty);
            string correction2Path = Path.Combine(SpiralLab.Sirius3.Config.CorrectionPath, correction2File);
            if (File.Exists(correction2Path))
            {
                // Load another correction file on Table2 
                success &= rtc.CtlLoadCorrectionFile(CorrectionTables.Table2, correction2Path);
                // Select Table1 on Primary(1st) head
                // Select Table2 on Secondary(2nd) head
                success &= rtc.CtlSelectCorrection(CorrectionTables.Table1, CorrectionTables.Table2);
            }

            // Create GPIO at RTC 
            // 16 bits input at extension 1 port
            dInExt1 = IOFactory.CreateInputExtension1(rtc);
            // 16 bits output at extension 1 port
            dOutExt1 = IOFactory.CreateOutputExtension1(rtc);
            success &= dInExt1.Initialize();
            success &= dOutExt1.Initialize();

            if (rtc is Rtc6SyncAxis)
            {
                // syncAXIS has no more
            }
            else
            {
                // 8 bits output at extension 2 port
                dOutExt2 = IOFactory.CreateOutputExtension2(rtc);
                // 2 bits input at laser port
                dInLaserPort = IOFactory.CreateInputLaserPort(rtc);
                // 2 bits output at laser port
                dOutLaserPort = IOFactory.CreateOutputLaserPort(rtc);

                success &= dInLaserPort.Initialize();
                success &= dOutExt2.Initialize();
                success &= dOutLaserPort.Initialize();
            }
            Debug.Assert(success);

            // Set FOV area: WxH, it will be drawn as red square
            //siriusEditorControl1.EditorCtrl.View.FovArea = new DVec3(200, 200, 0);
            //SpiralLab.Sirius3.Winforms.Config.ViewFovSize = new SizeF(fov, fov);

            // To check out of range for jump and mark x,y locations
            //rtc.FieldSizeLimit = new SizeF(fov, fov);

            // 2nd Head
            var rtc2ndHead = rtc as IRtc2ndHead;
            if (null != rtc2ndHead)
            {
                // Primary head base offset
                var dx1 = NativeMethods.ReadIni<double>(ConfigFileName, $"RTC{index}", "PRIMARY_BASE_OFFSET_X");
                var dy1 = NativeMethods.ReadIni<double>(ConfigFileName, $"RTC{index}", "PRIMARY_BASE_OFFSET_Y");
                var angle1 = NativeMethods.ReadIni<double>(ConfigFileName, $"RTC{index}", "PRIMARY_BASE_OFFSET_ANGLE");
                rtc2ndHead.PrimaryHeadBaseOffset = new SpiralLab.Sirius3.Mathematics.Offset(dx1, dy1, 0, angle1);

                int enable2ndHead = NativeMethods.ReadIni<int>(ConfigFileName, $"RTC{index}", $"SECONDARY_HEAD_ENABLE");
                if (0 != enable2ndHead)
                {
                    var secondaryCorrectionFileName = NativeMethods.ReadIni<string>(ConfigFileName, $"RTC{index}", "SECONDARY_CORRECTION");
                    var secondaryCorrectionFullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "correction", secondaryCorrectionFileName);
                    success &= rtc.CtlLoadCorrectionFile(CorrectionTables.Table2, secondaryCorrectionFullPath);
                    success &= rtc.CtlSelectCorrection(rtc.PrimaryHeadTable, CorrectionTables.Table2);
                    
                    var distX = NativeMethods.ReadIni<double>(ConfigFileName, $"RTC{index}", "PRIMARY_TO_SECONDARY_DISTANCE_X");
                    var distY = NativeMethods.ReadIni<double>(ConfigFileName, $"RTC{index}", "PRIMARY_TO_SECONDARY_DISTANCE_Y");
                    
                    // Distance from primary to secondary head
                    rtc2ndHead.DistanceToSecondaryHead = new DVec2(distX, distY);

                    // Secondary head base offset
                    var dx2 = NativeMethods.ReadIni<double>(ConfigFileName, $"RTC{index}", "SECONDARY_BASE_OFFSET_X");
                    var dy2 = NativeMethods.ReadIni<double>(ConfigFileName, $"RTC{index}", "SECONDARY_BASE_OFFSET_Y");
                    var angle2 = NativeMethods.ReadIni<double>(ConfigFileName, $"RTC{index}", "SECONDARY_BASE_OFFSET_ANGLE");
                    rtc2ndHead.SecondaryHeadBaseOffset = new SpiralLab.Sirius3.Mathematics.Offset(dx2, dy2, 0, angle2);
                }
            }

            // 3D
            var rtc3D = rtc as IRtc3D;
            if (null != rtc3D)
            {
                var kzScaleStr = NativeMethods.ReadIni<string>(ConfigFileName, $"RTC{index}", "KZ_SCALE", "1,1");
                var scaleTokens = kzScaleStr.Split(',');
                Debug.Assert(2 == scaleTokens.Length);
                rtc3D.KZScale = new DVec2(double.Parse(scaleTokens[0]), double.Parse(scaleTokens[1]));
            }

            // MoF 
            var rtcMoF = rtc as IRtcMoF;
            if (null != rtcMoF)
            {
                rtcMoF.EncXCountsPerMm = NativeMethods.ReadIni<int>(ConfigFileName, $"RTC{index}", "MOF_X_ENC_COUNTS_PER_MM", 0);
                rtcMoF.EncYCountsPerMm = NativeMethods.ReadIni<int>(ConfigFileName, $"RTC{index}", "MOF_X_ENC_COUNTS_PER_MM", 0);
                rtcMoF.EncCountsPerRevolution = NativeMethods.ReadIni<int>(ConfigFileName, $"RTC{index}", "MOF_ANGULAR_ENC_COUNTS_PER_REVOLUTION", 0);
                var trackingError = NativeMethods.ReadIni<int>(ConfigFileName, $"RTC{index}", "MOF_TRACKING_ERROR", 250);
                rtcMoF.CtlMofTrackingError(trackingError, trackingError);
            }

            // Default frequency and pulse width: 50KHz, 2 usec 
            success &= rtc.CtlFrequency(50 * 1000, 2);
            // Default jump and mark speed: 500 mm/s
            success &= rtc.CtlSpeed(500, 500);
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
                        var laserVirtualMaxPower = NativeMethods.ReadIni<float>(ConfigFileName, $"LASER{index}", "MAXPOWER", 10);
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
                }
                success &= powerMeter.Initialize();
                Debug.Assert(success);
                // uncomment to auto start 
                //success &= powerMeter.CtlStart();
            }
            #endregion

            #region Initialize Laser source
            var laserType = NativeMethods.ReadIni(ConfigFileName, $"LASER{index}", "TYPE", "Virtual");
            var laserMaxPower = NativeMethods.ReadIni<float>(ConfigFileName, $"LASER{index}", "MAXPOWER", 10);
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
                            laser = LaserFactory.CreateVirtual(index, laserMaxPower);
                            break;
                        case "analog1":
                            {
                                var voltageMin = NativeMethods.ReadIni<float>(ConfigFileName, $"LASER{index}", "POWERCONTROL_VOLTAGE_MIN", 0);
                                var voltageMax = NativeMethods.ReadIni<float>(ConfigFileName, $"LASER{index}", "POWERCONTROL_VOLTAGE_MAX", 10);
                                laser = LaserFactory.CreateVirtualAnalog(index, laserMaxPower, 1, voltageMin, voltageMax);
                            }
                            break;
                        case "analog2":
                            {
                                var voltageMin = NativeMethods.ReadIni<float>(ConfigFileName, $"LASER{index}", "POWERCONTROL_VOLTAGE_MIN", 0);
                                var voltageMax = NativeMethods.ReadIni<float>(ConfigFileName, $"LASER{index}", "POWERCONTROL_VOLTAGE_MAX", 10);
                                laser = LaserFactory.CreateVirtualAnalog(index, laserMaxPower, 2, voltageMin, voltageMax);
                            }
                            break;
                        case "frequency":
                            var freqMin = NativeMethods.ReadIni<float>(ConfigFileName, $"LASER{index}", "POWERCONTROL_FREQUENCY_MIN", 40000);
                            var freqMax = NativeMethods.ReadIni<float>(ConfigFileName, $"LASER{index}", "POWERCONTROL_FREQUENCY_MAX", 50000);
                            laser = LaserFactory.CreateVirtualFrequency(index, laserMaxPower, freqMin, freqMax);
                            break;
                        case "dutycycle":
                            var dutyCycleMin = NativeMethods.ReadIni<float>(ConfigFileName, $"LASER{index}", "POWERCONTROL_DUTYCYCLE_MIN", 0);
                            var dutyCycleMax = NativeMethods.ReadIni<float>(ConfigFileName, $"LASER{index}", "POWERCONTROL_DUTYCYCLE_MAX", 99);
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
            if (powerMeter != null)
            {
                if (powerMeter is PowerMeterVirtual powerMeterVirtual)
                {
                    powerMeterVirtual.Laser = laser;
                }
            }
            var laserPowerControlDelay = NativeMethods.ReadIni<float>(ConfigFileName, $"LASER{index}", "POWERCONTROL_DELAY", 0);
            laser.PowerControlDelayTime = laserPowerControlDelay;
            
            // Initialize PowerMap 
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
            Debug.Assert(success);

            // Set Default Power
            var laserDefaultPower = NativeMethods.ReadIni<float>(ConfigFileName, $"LASER{index}", "DEFAULT_POWER", 1);
            if (null != powerControl)
                success &= powerControl.CtlPower(laserDefaultPower);
            Debug.Assert(success);
            #endregion

            #region Marker
            switch (rtcType.Trim().ToLower())
            {
                default:
                case "virtual":
                    marker = MarkerFactory.CreateVirtual(index);
                    break;
                case "rtc4":
                case "rtc5":
                case "rtc6":
                case "rtc6e":
                    marker = MarkerFactory.CreateRtc(index);
                    //marker = MarkerFactory.CreateRtcFast(index);
                    //or your custom marker
                    break;
                case "syncaxis":
                    marker = MarkerFactory.CreateSyncAxis(index);
                    break;
            }            
            #endregion

            return success;
        }

        /// <summary>
        /// Dispose resources (like as <c>IScanner</c>, <c>ILaser</c>, <c>IMarker</c>,<c>IPowerMeter</c>, <c>IRemote</c> ...)
        /// </summary>
        /// <param name="rtc"><c>IScanner</c></param>
        /// <param name="laser"><c>ILaser</c></param>
        /// <param name="powerMeter"><c>IPowerMeter</c></param>
        /// <param name="dInExt1">RTC D.Input EXTENSION1 port</param>
        /// <param name="dInLaserPort">RTC D.Input LASER port</param>
        /// <param name="dOutExt1">RTC D.Output EXTENSION1 port</param>
        /// <param name="dOutExt2">RTC D.Output EXTENSION2 port</param>
        /// <param name="dOutLaserPort">RTC D.Output LASER port</param>
        /// <param name="marker"><c>IMarker</c></param>
        public static void DestroyDevices(IRtc rtc, ILaser laser, IDInput dInExt1, IDInput dInLaserPort, IDOutput dOutExt1, IDOutput dOutExt2, IDOutput dOutLaserPort, IPowerMeter powerMeter, IMarker marker)
        {
            marker?.Dispose();
            //powerMap?.Dispose();
            powerMeter?.Dispose();
            dInExt1?.Dispose();
            dInLaserPort?.Dispose();
            dOutExt1?.Dispose();
            dOutExt2?.Dispose();
            dOutLaserPort?.Dispose();
            laser?.Dispose();
            rtc?.Dispose();
        }

        private static void PowerMap_OnMappingOpened(IPowerMap powerMap, string fileName)
        {
            //var index = powerMap.Index;
            //var name = Path.GetFileName(fileName);
            //NativeMethods.WriteIni<string>(ConfigFileName, $"LASER{index}", "POWERMAP_FILE", name);

        }
        private static void PowerMap_OnMappingSaved(IPowerMap powerMap, string fileName)
        {
            var index = powerMap.Index;
            // File path should be in "bin\powermap"
            var fileNameOnly = Path.GetFileName(fileName);
            NativeMethods.WriteIni<string>(ConfigFileName, $"LASER{index}", "POWERMAP_FILE", fileNameOnly);
        }

        /// <summary>
        /// Create test entities at <c>IDocument</c>
        /// </summary>
        /// <param name="rtc"><c>IRtc</c></param>
        /// <param name="document"><c>IDocument</c></param>
        /// <returns>Success or failed</returns>
        public static void CreateTestEntities(IRtc rtc, IDocument document)
        {
            points_testcase(document);
            line_arc_testcase(document);
            triangle_rectangle_testcase(document);
            polyline2d_testcase(document);
            polyline3d_testcase(document);
            hatch_testcase(document);
            bezierSpline_testcase(document);
            catmullRomSpline_testcase(document);
            text_testcase(document);
            image_testcase(document);
            gridcloud_testcase(document);
            large_lines_testcase(document);
            barcode_testcase(document);
            groupIngroup_testcase(document);
            sphere_testcase(document);
            cube_cylinder_testcase(document);
            block_insert_testcase(document);
            //gerber_testcase(document);
            zpl_testcase(document);
        }

        #region Testcases (Samples)

        /// <summary>
        /// Adds a random point cloud entity.
        /// </summary>
        private static void points_testcase(IDocument document)
        {

            var rng = new Random((int)DateTime.Now.Ticks);

            int VERT_COUNT = 100 + (int)(rng.NextDouble() * 100);
            var tempVerts = new List<Vector3d>(VERT_COUNT);
            for (int v = 0; v < VERT_COUNT; v++)
            {
                double x = rng.NextDouble() * 6.0 - 3.0;
                double y = rng.NextDouble() * 6.0 - 3.0;
                double z = rng.NextDouble();
                tempVerts.Add(new Vector3d(x, y, z));
            }

            var points = new EntityPoints(tempVerts)
            {
                ColorMode = EntityModelBase.ColorModes.Model,
                ModelColor = new Vector3d(rng.NextDouble() + 0.8, rng.NextDouble() * 0.5, rng.NextDouble() + 0.4)
            };

            double tx = rng.NextDouble() * 100.0 - 50.0;
            double ty = rng.NextDouble() * 100.0 - 50.0;
            double tz = rng.NextDouble() * 10.0;
            points.Translate(tx, ty, tz);

            document.ActivePage?.ActiveLayer?.AddChild(points);
        }

        /// <summary>
        /// Adds line, arc, trepan samples with random transforms.
        /// </summary>
        private static void line_arc_testcase(IDocument document)
        {
            var rng = new Random((int)DateTime.Now.Ticks);

            {
                var entity = new EntityLine(new Vector3d(0, 0, 0), new Vector3d(10, 10, 1));
                document.ActivePage?.ActiveLayer?.AddChild(entity);
            }

            {
                var entity = new EntityArc(new Vector3d(0, 0, 0), 5);
                double rx = rng.NextDouble() * 10 - 5.0;
                double ry = rng.NextDouble() * 10 - 5.0;
                double rz = rng.NextDouble() * 10 - 5.0;
                entity.Rotate(rx, ry, rz);

                double tx = rng.NextDouble() * 100.0 - 50.0;
                double ty = rng.NextDouble() * 100.0 - 50.0;
                double tz = rng.NextDouble() * 100.0 - 10.0;
                entity.Translate(tx, ty, tz);

                document.ActivePage?.ActiveLayer?.AddChild(entity);
            }

            {
                var entity = new EntityTrepan(Vector3d.Zero, 5, 10, 10);
                double tx = rng.NextDouble() * 100.0 - 50.0;
                double ty = rng.NextDouble() * 100.0 - 50.0;
                double tz = rng.NextDouble() * 10.0;
                entity.Translate(tx, ty, tz);

                document.ActivePage?.ActiveLayer?.AddChild(entity);
            }
        }

        /// <summary>
        /// Adds triangle/rectangle/cross with random transforms.
        /// </summary>
        private static void triangle_rectangle_testcase(IDocument document)
        {
            var rng = new Random((int)DateTime.Now.Ticks);

            {
                var entity = new EntityTriangle(new Vector3d(0, 0, 0), 3, 2);
                entity.Rotate(rng.NextDouble() * 10 - 5.0, rng.NextDouble() * 10 - 5.0, rng.NextDouble() * 10 - 5.0);
                entity.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 10.0);
                document.ActivePage?.ActiveLayer?.AddChild(entity);
            }

            {
                var entity = new EntityRectangle(new Vector3d(0, 0, 0), 4, 3);
                entity.Rotate(rng.NextDouble() * 10 - 5.0, rng.NextDouble() * 10 - 5.0, rng.NextDouble() * 10 - 5.0);
                entity.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 10.0);
                document.ActivePage?.ActiveLayer?.AddChild(entity);
            }

            {
                var entity = new EntityCross(Vector3d.Zero, 10, 10, 2);
                entity.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 10.0);
                document.ActivePage?.ActiveLayer?.AddChild(entity);
            }
        }

        /// <summary>
        /// Adds hatch examples on rectangle/cross/polylines.
        /// </summary>
        private static void hatch_testcase(IDocument document)
        {
            var rng = new Random((int)DateTime.Now.Ticks);

            {
                var entity = new EntityRectangle(new Vector3d(0, 0, 0), 4, 3);
                entity.Rotate(rng.NextDouble() * 10 - 5.0, rng.NextDouble() * 10 - 5.0, rng.NextDouble() * 10 - 5.0);
                entity.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 10.0);
                entity.AddHatch(HatchFactory.CreateLine(30, 0.2));
                entity.AddHatch(HatchFactory.CreateLine(120, 0.2));
                document.ActivePage?.ActiveLayer?.AddChild(entity);
            }

            {
                var entity = new EntityCross(Vector3d.Zero, 10, 10, 2);
                entity.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 10.0);
                entity.AddHatch(HatchFactory.CreatePolygon(0.1));
                document.ActivePage?.ActiveLayer?.AddChild(entity);
            }

            {
                const int ENTITY_COUNT = 5;
                for (int i = 0; i < ENTITY_COUNT; i++)
                {
                    int VERT_COUNT = 3 + (int)(rng.NextDouble() * 5);
                    var tempVerts = new List<Vertex2D>(VERT_COUNT);
                    for (int v = 0; v < VERT_COUNT; v++)
                    {
                        double x = rng.NextDouble() * 10.0 - 5.0;
                        double y = rng.NextDouble() * 10.0 - 5.0;
                        double b = rng.NextDouble();
                        tempVerts.Add(new Vertex2D(x, y, b));
                    }

                    var poly = new EntityPolyline2D(tempVerts, true)
                    {
                        ColorMode = EntityModelBase.ColorModes.Model,
                        ModelColor = new Vector3d(rng.NextDouble() + 0.4, rng.NextDouble() * 0.5, rng.NextDouble() + 0.4)
                    };

                    poly.Rotate(rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0);
                    poly.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 10.0);
                    poly.AddHatch(HatchFactory.CreateLine(45, 0.2, 0.1));

                    document.ActivePage?.ActiveLayer?.AddChild(poly);
                }
            }
        }

        /// <summary>
        /// Adds multiple cubes and cylinders with random transforms.
        /// </summary>
        private static void cube_cylinder_testcase(IDocument document)
        {
            var rng = new Random((int)DateTime.Now.Ticks);
            const int ENTITY_COUNT = 5;

            for (int i = 0; i < ENTITY_COUNT; i++)
            {
                var cube = new EntityCube(Vector3d.Zero, rng.NextDouble() * 5, rng.NextDouble() * 6, rng.NextDouble() * 2)
                {
                    ColorMode = EntityModelBase.ColorModes.Model,
                    ModelColor = new Vector3d(rng.NextDouble() + 0.8, rng.NextDouble() * 0.5, rng.NextDouble())
                };
                cube.Rotate(rng.NextDouble() * 60.0 - 30.0, rng.NextDouble() * 60.0 - 30.0, rng.NextDouble() * 60.0 - 30.0);
                cube.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 10.0);
                document.ActivePage?.ActiveLayer?.AddChild(cube);

                var cyl = new EntityCylinder(Vector3d.Zero, rng.NextDouble() * 10, rng.NextDouble() * 10)
                {
                    ColorMode = EntityModelBase.ColorModes.Model,
                    ModelColor = new Vector3d(rng.NextDouble() * 0.5, rng.NextDouble() * 0.7, rng.NextDouble() + 0.5)
                };
                cyl.Rotate((float)(rng.NextDouble() * 60.0 - 30.0), (float)(rng.NextDouble() * 60.0 - 30.0), (float)(rng.NextDouble() * 60.0 - 30.0));
                cyl.Translate((float)(rng.NextDouble() * 100.0 - 50.0), (float)(rng.NextDouble() * 100.0 - 50.0), (float)(rng.NextDouble() * 100.0 - 10.0));
                document.ActivePage?.ActiveLayer?.AddChild(cyl);
            }
        }

        /// <summary>
        /// Adds two large grid-cloud entities for a height map example.
        /// </summary>
        private static void gridcloud_testcase(IDocument document)
        {
            var rng = new Random((int)DateTime.Now.Ticks);
            const int COLS = 1024;
            const int ROWS = 768;
            const double INTERVAL = 0.05;

            EntityGrids reference = null;
            EntityGrids measured = null;

            // Reference
            {
                var zDepths = new List<double>(ROWS * COLS);
                var center = new Vector2d(COLS / 2f * INTERVAL, ROWS / 2f * INTERVAL);
                double amplitude = 0.5f;
                float wavelength = 5f;
                double phaseOffset = 0f;

                for (int y = 0; y < ROWS; y++)
                {
                    for (int x = 0; x < COLS; x++)
                    {
                        var pos = new Vector2d(x * INTERVAL, y * INTERVAL);
                        double dist = (pos - center).Length;
                        double z = amplitude * Math.Sin((2 * Math.PI * dist / wavelength) + phaseOffset);
                        zDepths.Add(z);
                    }
                }

                var minZ = zDepths.Min();
                var maxZ = zDepths.Max();
                var pointsCloud = new EntityGrids(ROWS, COLS, INTERVAL, zDepths, new Vector2d(minZ + 2, maxZ + 2));
                pointsCloud.Translate(-100, 0, 2);
                document.ActivePage?.ActiveLayer?.AddChild(pointsCloud);
                reference = pointsCloud;
            }

            // Measured
            {
                var zDepths = new List<double>(ROWS * COLS);
                var center = new Vector2d(COLS / 2f * INTERVAL, ROWS / 2f * INTERVAL);
                double amplitude = 0.5f;
                double wavelength = 5f;
                double phaseOffset = 0f;

                for (int y = 0; y < ROWS; y++)
                {
                    for (int x = 0; x < COLS; x++)
                    {
                        var pos = new Vector2d(x * INTERVAL, y * INTERVAL);
                        double dist = (pos - center).Length;
                        double z = amplitude * Math.Sin((2 * Math.PI * dist / wavelength) + phaseOffset);
                        zDepths.Add(z + 0.02f);
                    }
                }

                var minZ = zDepths.Min();
                var maxZ = zDepths.Max();
                var pointsCloud = new EntityGrids(ROWS, COLS, INTERVAL, zDepths, new Vector2d(minZ + 5, maxZ + 5))
                {
                    ColorMode = EntityModelBase.ColorModes.PerVertex
                };
                pointsCloud.Translate(100, 0, 5);
                document.ActivePage?.ActiveLayer?.AddChild(pointsCloud);
                measured = pointsCloud;
            }
        }

        /// <summary>
        /// Adds random closed 2D polylines with transforms.
        /// </summary>
        private static void polyline2d_testcase(IDocument document)
        {
            var rng = new Random((int)DateTime.Now.Ticks);
            const int ENTITY_COUNT = 5;

            for (int i = 0; i < ENTITY_COUNT; i++)
            {
                int VERT_COUNT = 3 + (int)(rng.NextDouble() * 5);
                var tempVerts = new List<Vertex2D>(VERT_COUNT);
                for (int v = 0; v < VERT_COUNT; v++)
                {
                    double x = rng.NextDouble() * 10.0 - 5.0;
                    double y = rng.NextDouble() * 10.0 - 5.0;
                    double b = rng.NextDouble();
                    tempVerts.Add(new Vertex2D(x, y, b));
                }

                var poly = new EntityPolyline2D(tempVerts, true)
                {
                    ColorMode = EntityModelBase.ColorModes.Model,
                    ModelColor = new Vector3d(rng.NextDouble() + 0.4, rng.NextDouble() * 0.5, rng.NextDouble() + 0.4)
                };

                poly.Rotate(rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0);
                poly.Scale(rng.NextDouble() * 2.0 + 0.5, rng.NextDouble() * 2.0 + 0.5, rng.NextDouble() * 2.0 + 0.5);
                poly.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 10.0);

                document.ActivePage?.ActiveLayer?.AddChild(poly);
            }
        }

        /// <summary>
        /// Adds random closed 3D polylines with transforms.
        /// </summary>
        private static void polyline3d_testcase(IDocument document)
        {
            var rng = new Random((int)DateTime.Now.Ticks);
            const int ENTITY_COUNT = 5;

            for (int i = 0; i < ENTITY_COUNT; i++)
            {
                int VERT_COUNT = 3 + (int)(rng.NextDouble() * 5);
                var tempVerts = new List<Vector3d>(VERT_COUNT);
                for (int v = 0; v < VERT_COUNT; v++)
                {
                    double x = rng.NextDouble() * 10.0 - 5.0;
                    double y = rng.NextDouble() * 10.0 - 5.0;
                    double z = rng.NextDouble() * 10.0 - 5.0;
                    tempVerts.Add(new Vector3d(x, y, z));
                }

                var poly = new EntityPolyline3D(tempVerts, true)
                {
                    ColorMode = EntityModelBase.ColorModes.Model,
                    ModelColor = new Vector3d(rng.NextDouble() + 0.4, rng.NextDouble() * 0.5, rng.NextDouble() + 0.4)
                };

                poly.Rotate(rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0);
                poly.Scale(rng.NextDouble() * 2.0 + 0.5, rng.NextDouble() * 2.0 + 0.5, rng.NextDouble() * 2.0 + 0.5);
                poly.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 10.0);

                document.ActivePage?.ActiveLayer?.AddChild(poly);
            }
        }

        /// <summary>
        /// Adds random Bezier spline examples with transforms.
        /// </summary>
        private static void bezierSpline_testcase(IDocument document)
        {
            var rng = new Random((int)DateTime.Now.Ticks);
            const int ENTITY_COUNT = 5;

            for (int i = 0; i < ENTITY_COUNT; i++)
            {
                int VERT_COUNT = 3 + (int)(rng.NextDouble() * 5);
                var tempVerts = new List<Vector3d>(VERT_COUNT);
                for (int v = 0; v < VERT_COUNT; v++)
                {
                    double x = rng.NextDouble() * 10.0 - 5.0;
                    double y = rng.NextDouble() * 10.0 - 5.0;
                    double z = rng.NextDouble() * 10.0 - 5.0;
                    tempVerts.Add(new Vector3d(x, y, z));
                }

                var spline = new EntityBezierSpline(tempVerts)
                {
                    ColorMode = EntityModelBase.ColorModes.Model,
                    ModelColor = new Vector3d(rng.NextDouble() + 0.4, rng.NextDouble() * 0.5, rng.NextDouble() + 0.4)
                };

                spline.Rotate(rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0);
                spline.Scale(rng.NextDouble() * 2.0 + 0.5, rng.NextDouble() * 2.0 + 0.5, rng.NextDouble() * 2.0 + 0.5);
                spline.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 10.0);

                document.ActivePage?.ActiveLayer?.AddChild(spline);
            }
        }

        /// <summary>
        /// Adds random Catmull-Rom spline examples with transforms.
        /// </summary>
        private static void catmullRomSpline_testcase(IDocument document)
        {
            var rng = new Random((int)DateTime.Now.Ticks);
            const int ENTITY_COUNT = 5;

            for (int i = 0; i < ENTITY_COUNT; i++)
            {
                int VERT_COUNT = 5 + (int)(rng.NextDouble() * 5);
                var tempVerts = new List<Vector3d>(VERT_COUNT);
                for (int v = 0; v < VERT_COUNT; v++)
                {
                    double x = rng.NextDouble() * 10.0 - 5.0;
                    double y = rng.NextDouble() * 10.0 - 5.0;
                    double z = rng.NextDouble() * 10.0 - 5.0;
                    tempVerts.Add(new Vector3d(x, y, z));
                }

                var spline = new EntityCatmullRomSpline(tempVerts, false)
                {
                    ColorMode = EntityModelBase.ColorModes.Model,
                    ModelColor = new Vector3d(rng.NextDouble() + 0.4, rng.NextDouble() * 0.5, rng.NextDouble() + 0.4)
                };

                spline.Rotate(rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0);
                spline.Scale(rng.NextDouble() * 2.0 + 0.5, rng.NextDouble() * 2.0 + 0.5, rng.NextDouble() * 2.0 + 0.5);
                spline.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 10.0);

                document.ActivePage?.ActiveLayer?.AddChild(spline);
            }
        }

        /// <summary>
        /// Adds multiple text variants (GDI, image, circular, cxf) with transforms.
        /// </summary>
        private static void text_testcase(IDocument document)
        {
            var rng = new Random((int)DateTime.Now.Ticks);

            {
                var text = new EntityText("Arial", FontStyle.Regular, $"0123456789{Environment.NewLine}AaBbFfGgHhJj{Environment.NewLine}~!@#$%^&*()_+", 10);
                text.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 10.0);
                document.ActivePage?.ActiveLayer?.AddChild(text);
            }

            {
                var text = new EntityText("Segoe UI", FontStyle.Regular, $"스파이럴랩{Environment.NewLine}SIRIUS3{Environment.NewLine}개발자 버전", 12);
                text.Rotate(rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0);
                text.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 10.0);
                document.ActivePage?.ActiveLayer?.AddChild(text);
            }

            {
                var text = new EntityImageText(
                    "Segoe UI",
                    FontStyle.Regular,
                    $"0123456789{Environment.NewLine}AaBbFfGgHhJj{Environment.NewLine}~!@#$%^&*()_+",
                    50, 1, true, 20);

                text.Rotate(rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0);
                text.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 10.0);
                document.ActivePage?.ActiveLayer?.AddChild(text);
            }

            {
                var text = new EntityCircularText("Segoe UI", FontStyle.Regular, TextCircularDirections.ClockWise, 30, 90,
                    $"0123456789{Environment.NewLine}AaBbFfGgHhJj{Environment.NewLine}~!@#$%^&*()_+", 5);

                text.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 10.0);
                document.ActivePage?.ActiveLayer?.AddChild(text);
            }

            {
                var text = new EntitySiriusText("romans2.cxf", EntitySiriusText.LetterSpaces.Fixed, 0, 1, 0.5,
                    $"0123456789{Environment.NewLine}AaBbFfGgHhJj{Environment.NewLine}~!@#$%^&*()_+", 10);

                text.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 10.0);
                document.ActivePage?.ActiveLayer?.AddChild(text);
            }
        }

        /// <summary>
        /// Creates a nested group containing multiple polylines and sub-groups.
        /// </summary>
        private static void groupIngroup_testcase(IDocument document)
        {
            var rng = new Random((int)DateTime.Now.Ticks);
            var group = new EntityGroup { Name = "TestGroup" };

            for (int i = 0; i < 5; i++)
            {
                int VERT_COUNT = 3 + (int)(rng.NextDouble() * 5);
                var tempVerts = new List<Vertex2D>(VERT_COUNT);
                for (int v = 0; v < VERT_COUNT; v++)
                {
                    double x = rng.NextDouble() * 20.0 - 10.0;
                    double y = rng.NextDouble() * 20.0 - 10.0;
                    double b = rng.NextDouble() * 0.1;
                    tempVerts.Add(new Vertex2D(x, y, b));
                }

                var poly = new EntityPolyline2D(tempVerts, true)
                {
                    ColorMode = EntityModelBase.ColorModes.Model,
                    ModelColor = new Vector3d(rng.NextDouble() + 0.4, rng.NextDouble() * 0.5, rng.NextDouble() + 0.4)
                };

                poly.Rotate(rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0);
                poly.Scale(rng.NextDouble() * 2.0 + 0.5, rng.NextDouble() * 2.0 + 0.5, rng.NextDouble() * 2.0 + 0.5);
                poly.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 10.0);

                group.AddChild(poly);
            }

            for (int i = 0; i < 2; i++)
            {
                var subGroup = new EntityGroup(2) { Name = $"SubGroup{i}" };

                int VERT_COUNT = 5 + (int)(rng.NextDouble() * 5);
                var tempVerts = new List<Vertex2D>(VERT_COUNT);
                for (int v = 0; v < VERT_COUNT; v++)
                {
                    double x = rng.NextDouble() * 20.0 - 10.0;
                    double y = rng.NextDouble() * 20.0 - 10.0;
                    double b = rng.NextDouble() * 0.1;
                    tempVerts.Add(new Vertex2D(x, y, b));
                }

                var poly = new EntityPolyline2D(tempVerts, true)
                {
                    ColorMode = EntityModelBase.ColorModes.Model,
                    ModelColor = new Vector3d(rng.NextDouble() + 0.4, rng.NextDouble() * 0.5, rng.NextDouble() + 0.4)
                };

                poly.Rotate(rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0);
                poly.Scale(rng.NextDouble() * 2.0 + 0.5, rng.NextDouble() * 2.0 + 0.5, rng.NextDouble() * 2.0 + 0.5);
                poly.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 10.0);

                subGroup.AddChild(poly);
                group.AddChild(subGroup);
            }

            document.ActivePage?.ActiveLayer?.AddChild(group);
        }

        /// <summary>
        /// Adds a set of spheres with Z height-map coloring.
        /// </summary>
        private static void sphere_testcase(IDocument document)
        {
            var rng = new Random((int)DateTime.Now.Ticks);
            const int ENTITY_COUNT = 100;
            var group = new EntityGroup();

            for (int i = 0; i < ENTITY_COUNT; i++)
            {
                var entity = new EntitySphere(new Vector3d(0, 0, 0), 3)
                {
                    Segments = 24,
                    ColorMode = EntityModelBase.ColorModes.ZHeightMap,
                    ZRange = new Vector2d(-5, 5)
                };

                entity.Rotate(rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0);
                entity.Scale(rng.NextDouble() * 2.0 + 0.5, rng.NextDouble() * 2.0 + 0.5, rng.NextDouble() * 2.0 + 0.5);
                entity.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0 + 100, rng.NextDouble() * 10.0 - 5.0);

                group.AddChild(entity);
            }

            document.ActivePage?.ActiveLayer?.AddChild(group);
        }

        /// <summary>
        /// Creates a block from an entity and inserts multiple block instances.
        /// </summary>
        private static void block_insert_testcase(IDocument document)
        {
            var rng = new Random((int)DateTime.Now.Ticks);

            {
                var entity = new EntitySpiral(Vector3d.Zero, 5, 10, 2, 5, true);
                document.ActBlock(new IEntity[] { entity }, "Block1");
            }

            {
                double dx = 0;
                double dy = 0;
                List<IEntity> entities = new(3 * 5);

                for (int y = 0; y < 2; y++)
                {
                    for (int x = 0; x < 5; x++)
                    {
                        var insert = new EntityBlockInsert($"BlockInsert{x},{y}", "Block1", new Vector3d(dx, dy - 50, 0));
                        insert.Rotate(rng.NextDouble() * 60.0 - 30.0, rng.NextDouble() * 60.0 - 30.0, rng.NextDouble() * 60.0 - 30.0);
                        insert.Scale(rng.NextDouble() + 0.2, rng.NextDouble() + 0.2, rng.NextDouble() + 0.2);

                        entities.Add(insert);
                        dx += 10;
                    }
                    dx = 0;
                    dy += 11;
                }

                document.ActivePage?.ActiveLayer?.AddChildren(entities.ToArray());
            }
        }

        /// <summary>
        /// Adds an image entity if the sample image exists.
        /// </summary>
        private static void image_testcase(IDocument document)
        {
            var rng = new Random((int)DateTime.Now.Ticks);
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sample\\lena.bmp");
            if (!File.Exists(fileName)) return;

            var image = new EntityImage(fileName, 10);
            image.Rotate(rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0);
            image.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 10.0);

            document.ActivePage?.ActiveLayer?.AddChild(image);
        }

        /// <summary>
        /// (Optional) Demonstrates adding Gerber entities (paths are placeholders).
        /// </summary>
        private static void gerber_testcase(IDocument document)
        {
            // Update the file paths below before enabling.
            // var gerber1 = new EntityGerber(@"C:\Path\LED-seven-segment.GBS");
            // document.ActivePage?.ActiveLayer?.AddChild(gerber1);
        }

        /// <summary>
        /// Adds several large line batches to test performance.
        /// </summary>
        private static void large_lines_testcase(IDocument document)
        {
            // Pack 1
            {
                const int LINE_COUNT = 10000;
                const double LINE_LENGTH = 10;
                const double LINE_GAP = 0.01;
                List<Vector3d> lines = new(LINE_COUNT * 2);

                double dx = -80;
                double dy = -10;
                for (int i = 0; i < LINE_COUNT; i++)
                {
                    var start = new Vector3d(0 + dx, LINE_GAP * i + dy, 0);
                    var end = new Vector3d(LINE_LENGTH + dx, LINE_GAP * i + dy, 0);
                    lines.Add(start);
                    lines.Add(end);
                }
                var entity = new EntityLines(lines) { Alpha = 0.9f };
                document.ActivePage?.ActiveLayer?.AddChild(entity);
            }

            // Pack 2
            {
                const int LINE_COUNT = 1000;
                const double LINE_LENGTH = 5;
                const double LINE_GAP = 0.05;
                List<Vector3d> lines = new(LINE_COUNT * 2);

                double dx = 80;
                double dy = -10;
                for (int i = 0; i < LINE_COUNT; i++)
                {
                    var start = new Vector3d(0 + dx, LINE_GAP * i + dy, 0);
                    var end = new Vector3d(LINE_LENGTH + dx, LINE_GAP * i + dy, 0);
                    lines.Add(start);
                    lines.Add(end);
                }
                var entity = new EntityLines(lines);
                entity.Translate(0, 0, 1);
                document.ActivePage?.ActiveLayer?.AddChild(entity);
            }

            // Pack 3 (Z stacked)
            {
                const int LINE_COUNT = 100;
                const double LINE_LENGTH = 20;
                const double LINE_GAP = 0.05;
                List<Vector3d> lines = new(LINE_COUNT * 2);

                double dx = -10;
                double dy = -60;
                for (int i = 0; i < LINE_COUNT; i++)
                {
                    var start = new Vector3d(0 + dx, 0 + dy, LINE_GAP * i);
                    var end = new Vector3d(LINE_LENGTH + dx, 0 + dy, LINE_GAP * i);
                    lines.Add(start);
                    lines.Add(end);
                }
                var entity = new EntityLines(lines);
                entity.Translate(0, 0, 1);
                document.ActivePage?.ActiveLayer?.AddChild(entity);
            }
        }

        /// <summary>
        /// Adds 1D/2D barcode examples with transforms and hatch.
        /// </summary>
        private static void barcode_testcase(IDocument document)
        {
            var rng = new Random((int)DateTime.Now.Ticks);

            {
                var entity = new EntityBarcode1D("1234567890", EntityBarcode1D.Barcode1DFormats.Code128, 5, 5, 1);
                entity.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 10.0 - 2.0);
                document.ActivePage?.ActiveLayer?.AddChild(entity);
            }

            {
                var entity = new EntityQRCode("01234567890123456789", EntityBarcode2DBase.Barcode2DCells.Lines, 5, 5, 5);
                entity.Rotate(rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0);
                entity.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 10.0 - 2.0);
                document.ActivePage?.ActiveLayer?.AddChild(entity);
            }

            {
                var entity = new EntityDataMatrix("01234567890123456789", EntityBarcode2DBase.Barcode2DCells.Dots, 5, 5, 5);
                entity.Rotate(rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0);
                entity.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 10.0 - 2.0);
                document.ActivePage?.ActiveLayer?.AddChild(entity);
            }

            {
                var entity = new EntityPDF417("01234567890123456789", EntityBarcode2DBase.Barcode2DCells.Outline, 1, 5, 5);
                entity.Rotate(rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0);
                entity.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 10.0 - 2.0);

                var hatch = HatchFactory.CreateLine(45, 0.1);
                entity.AddHatch(hatch);

                document.ActivePage?.ActiveLayer?.AddChild(entity);
            }
        }

        /// <summary>
        /// Adds a sample ZPL label as an image entity.
        /// </summary>
        private static void zpl_testcase(IDocument document)
        {
            var sb = new StringBuilder();
            sb.Append("^XA");
            sb.Append("^FX Top section with logo, name and address.");
            sb.Append("^CF0,60");
            sb.Append("^FO50,50^GB100,100,100^FS");
            sb.Append("^FO75,75^FR^GB100,100,100^FS");
            sb.Append("^FO93,93^GB40,40,40^FS");
            sb.Append("^FO220,50^FDIntershipping, Inc.^FS");
            sb.Append("^CF0,30");
            sb.Append("^FO220,115^FD1000 Shipping Lane^FS");
            sb.Append("^FO220,155^FDShelbyville TN 38102^FS");
            sb.Append("^FO220,195^FDUnited States (USA)^FS");
            sb.Append("^FO50,250^GB700,3,3^FS");
            sb.Append("^FX Second section with recipient address and permit information.");
            sb.Append("^CFA,30");
            sb.Append("^FO50,300^FDJohn Doe^FS");
            sb.Append("^FO50,340^FD100 Main Street^FS");
            sb.Append("^FO50,380^FDSpringfield TN 39021^FS");
            sb.Append("^FO50,420^FDUnited States (USA)^FS");
            sb.Append("^CFA,15");
            sb.Append("^FO600,300^GB150,150,3^FS");
            sb.Append("^FO638,340^FDPermit^FS");
            sb.Append("^FO638,390^FD123456^FS");
            sb.Append("^FO50,500^GB700,3,3^FS");
            sb.Append("^FX Third section with bar code.");
            sb.Append("^BY5,2,270");
            sb.Append("^FO100,550^BC^FD12345678^FS");
            sb.Append("^FX Fourth section (the two boxes on the bottom).");
            sb.Append("^FO50,900^GB700,250,3^FS");
            sb.Append("^FO400,900^GB3,250,3^FS");
            sb.Append("^CF0,40");
            sb.Append("^FO100,960^FDCtr. X34B-1^FS");
            sb.Append("^FO100,1010^FDREF1 F00B47^FS");
            sb.Append("^FO100,1060^FDREF2 BL4H8^FS");
            sb.Append("^CF0,190");
            sb.Append("^FO470,955^FDCA^FS");
            sb.Append("^XZ");

            var zplText = sb.ToString();
            var entity = new EntityImageZPL(4 * 25.4, 6 * 25.4, zplText, EntityImageZPL.DotsPerMMs.Dots8_203DPI);
            document.ActivePage?.ActiveLayer?.AddChild(entity);
        }

        #endregion

    }
}
