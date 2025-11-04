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
        public static string ConfigFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini");

        /// <summary>
        /// Create devices (like as <c>IScanner</c>, <c>ILaser</c>, <c>IMarker</c>, <c>IPowerMeter</c>, ...)
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
        /// Dispose resources (like as <c>IScanner</c>, <c>ILaser</c>, <c>IMarker</c>, <c>IPowerMeter</c>, ...)
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
            NativeMethods.WriteIni<string>(ConfigFileName, $"LASER{index}", "POWERMAP_FILE", fileNameOnly);
        }

    }
}
