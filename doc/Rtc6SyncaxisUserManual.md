# RTC6 syncAXIS (XL-SCAN) Setup and Usage Guide

> Reference version: Sirius3 1.12.3 (public Release features)

## 1. syncAXIS Role

syncAXIS (XL-SCAN) plans and executes RTC6 scan-head motion together with an ACS motion controller and XY stage as one job. The scanner handles fast motion within a small field, while the stage handles wide-area travel.

Creating an RTC6 object alone does not make syncAXIS ready. Complete device installation, wiring, licensing, an equipment-specific `syncAXISConfig.xml`, stage error mapping, scan-head correction, and Simulation verification first.

## 2. Required Setup Before Execution

If the following item is not ready, do not switch to Hardware Mode.

### 2.1 Software and Runtime Environment

- The `Rtc6SyncAxis` in Sirius3 presupposes x64 Runtime.
- Use syncAXIS Software Package, DLL, XML Schema, and RTC6 Program File.
- The RTC6 `.dll`, `.rbf`, `.out`, `.dat` required for the syncAXIS application must use the combination provided in the same syncAXIS Package.
- The `syncAXISConfig.xml` and the CT5, Program File, and Log routes in which this file is referred must be accessed by the application account.
- The Sirius3 License must include the syncAXIS function.

### 2.2 SCANLAB USB Dongle

You need to connect the SCANLAB USB Dongle to the USB Port of your PC. The Dongle may record the number of available syncAXIS Instance and related Options. The Simulation Initialization also verifies the Dongle so you can’t miss the Dongle simply because it doesn’t move the hardware.

If there is a `InvalidOrMissingDongle` or instance excess error, check the following.

- Dongle is connected and normally recognized in Windows
- Are other syncAXIS processes on your PC currently using Instance?
- Confirm that the dongle options and licensed instance count match the equipment.
- SyncAXIS Option in Sirius3 License is activated.

### 2.3 Hardware

- RTC6 PCI Express or supported RTC6 Ethernet Board
- ExcelliSCAN Scan Head and specified Objective, Power, Refrigeration and Working Distance
- ACS Motion Controller, EtherCAT Configuration, Motor Drive and XY Stage
- Stage Limit, Reference Sensor, Emergency Stop and Safety Circuit
- Right Cable and Pin Assignment between RTC6 and Scan Head, SL2-100/EtherCAT Converter, Laser
- Laser emission, interlocks, safe marking, and protective equipment

Complete Positioning Stage Error Mapping before Scan Head Field correction whenever possible. Establishing the Stage coordinate system first allows the rotation and position offset between the Scan Head and Stage coordinate systems to be corrected accurately.

## 3. syncAXISConfig.xml Is Equipment-specific

Validate the customer-specific `syncAXISConfig.xml` supplied by SCANLAB against the actual equipment. The package template is a reference and must not be used to drive hardware as-is.

The subjects to check are the following.

| Difference | Verify the item. |
|---|---|
| common path. | `BaseDirectoryPath`, `ProgramFileDirectory`, CT5 path, Log path |
| Execute Mode | `SimulationMode`, initial Operation Mode |
| RTC6 | Board Serial Number, Use Connector, Program File, Head Configuration |
| ACS | Controller IP Address, `SlecEtherCATNodeID`, Stage Axis X/Y Connect |
| Stage | Available Work Area, Maximum Speed, Acceleration, Jerk, Error Mapping |
| Scanner | Available Working Field, Head and Objective, CT5 Correction File |
| Safety Monitoring | `MonitoringLevel`, `DynamicViolationReaction` and each Limit |
| Laser | Signal Level, Pin, Power/Pulse Conditions, Laser Timing |

Do not confuse the SLEC Unit ID/FOLLOWCH values written in `SlecEtherCATNodeID` and ACS. This is the common cause of hardware start-up errors.

### 3.1 BaseDirectoryPath

`BaseDirectoryPath` should actually refer to the task folder to run and refer syncAXIS files. If you use the Build Output folder like the comments of the `console_syncaxis_setup` demo, it also specifies the real output path in XML.

```xml
<cfg:BaseDirectoryPath>C:\YourApplication\bin</cfg:BaseDirectoryPath>
```

`ProgramFileDirectory` specifies the RTC6 `ProgramFiles` absolute route for the same syncAXIS Software Package. if you have moved the folder, check again whether all the relative and absolute routes in XML are valid.

### 3.2 XML verification

Install Package uses the corresponding Version XML Schema and syncAXIS Configurator to verify the graphics, tag, units and range. XML does not mean that even passing the Schema scan is in accordance with the specifications of the actual Stage, Scanner, Laser. Contrast the device manufacturer’s specifications and the actual Wiring separately.

## 4. Sirius3 folder and config_syncaxis.ini

Public demo finds XML under `SpiralLab.Sirius3.Config.SyncAxisPath` The default value is the `syncaxis` folder in the application Base Directory.

The key items of `demos/config_syncaxis.ini` are as follows.

```ini
[RTC0]
TYPE = SyncAxis
CONFIG_XML = syncAXISConfig.xml
```

`EditorHelper.CreateDevices` read the `CONFIG_XML` value and combines the path as follows:

```csharp
string configXmlFilePath = Path.Combine(
    SpiralLab.Sirius3.Config.SyncAxisPath,
    configXmlFileName);

IRtc rtc = ScannerFactory.CreateRtc6SyncAxis(rtcId, configXmlFilePath);
bool success = rtc.Initialize();
```

Thus, the basic deployment structure is the following.

```text
Application.exe
config_syncaxis.ini
syncaxis/
  syncAXISConfig.xml
  Tools/
    syncAXIS_Viewer/
```

INI tells you which XML to choose, and the actual Stage, Scanner, Laser configuration and Security Limit are defined by XML.

## 5. Direct Creation and Initialization

`demos/console_syncaxis_setup/Program.cs` is an example of creating `Rtc6SyncAxis` directly without using `EditorHelper`.

```csharp
using SpiralLab.Sirius3.Scanner.Rtc;

bool coreInitialized = false;
Rtc6SyncAxis rtc = null;

try
{
    coreInitialized = SpiralLab.Sirius3.Core.Initialize();
    if (!coreInitialized)
        throw new InvalidOperationException("Sirius3 initialization failed.");

    if (!SpiralLab.Sirius3.Core.IsRunningPlatform64)
        throw new PlatformNotSupportedException("syncAXIS requires an x64 runtime.");

    string xml = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        "syncaxis",
        "syncAXISConfig.xml");

    if (!File.Exists(xml))
        throw new FileNotFoundException("The syncAXIS configuration file was not found.", xml);

    rtc = ScannerFactory.CreateRtc6SyncAxis(0, xml);
    if (!rtc.Initialize())
        throw new InvalidOperationException("syncAXIS initialization failed.");

    // First, verify Job only in the Simulation Mode.
    if (!rtc.CtlSimulationMode(true))
        throw new InvalidOperationException("Failed to switch to Simulation Mode.");
}
finally
{
    rtc?.Dispose();
    if (coreInitialized)
        SpiralLab.Sirius3.Core.Cleanup();
}
```

`Initialize()` read XML, creates Native Configuration Handle, prepares Callback and the internal state. Successful return means that you read XML and created the Software Instance. Hardware is safe, Calibration quality or does not prove real processing completion.

## 6. Run in Simulation Mode First

The first XML or Job you use must run in the `SimulationMode = true` state. SCANLAB Installation Manual also requires you to simulate all routes before using the real Laser, Scan Head, Stage and verify the Limit breach.

### 6.1 MotionTypes

| Value | Behavior |
|---|---|
| `MotionTypes.ScannerOnly` | Fix Stage and use Scanner. |
| `MotionTypes.StageOnly` | Fix the scanner and use Stage. |
| `MotionTypes.StageAndScanner` | Distribute the path to Scanner and Stage |

```csharp
bool success = rtc.ListBegin(MotionTypes.StageAndScanner);
success &= rtc.ListJumpTo(new DVec2(-20, 20));
success &= rtc.ListMarkTo(new DVec2(20, 20));
success &= rtc.ListMarkTo(new DVec2(20, -20));
success &= rtc.ListMarkTo(new DVec2(-20, -20));
success &= rtc.ListMarkTo(new DVec2(-20, 20));
success &= rtc.ListJumpTo(DVec2.Zero);
success &= rtc.ListEnd();
if (success)
    success = rtc.ListExecute(false);
```

Check all return values immediately after calling if the middle list command fails, the incomplete list will not run.

### 6.2 Review Simulation Results

`console_syncaxis_setup` provides the following information.

- `JobHistory` results and time of execution
- Scanner use rate, maximum Position·Velocity·Acceleration
- Stage usability, maximum Position·Velocity·Acceleration·Jerk
- Simulation file created in `Config.SyncAxisSimulateFilePath`
- Open the results with `Config.SyncAxisViewerProgramPath` syncAXIS Viewer

In the Viewer, check the Scanner/Stage Position, Speed and Dynamic Limit breaches. If any position or Dynamic Violation remains, modify XML or Job and repeat Simulation.

## 7. Conditions for Switching to Hardware Mode

`rtc.CtlSimulationMode(false)` calling is a simple mode change request. This calling has been successful and the hardware is not safe prepared. it will only switch after you have verified all the following conditions.

1. There are no Position and Dynamic Violation in the Simulation results of the same XML and Job.
2. The power and communication of RTC6, Scan Head, ACS Controller, Stage, Laser are normal.
3. I checked the ACS IP, RTC6 Serial Number, `SlecEtherCATNodeID`, Stage Axis, Program File and CT5 routes in XML.
4. Stage Reference and Error Mapping are completed.
5. There are no people in the safe area, and there are no obstacles in the range of movement.
6. Laser emission controls, interlocks, emergency stops, and protective equipment are ready.
7. With low speed and limited output, you are ready to perform the step-by-step hardware verification procedure.

Hardware start-up is safe to first check the communication and Reference Run, and then expand the range to the Laser signals, ScannerOnly, StageOnly, StageAndScanner order.

## 8. console_syncaxis_setup menu and verification items

| Key | The demo action. | Purpose of use. |
|---|---|---|
| `S` | Busy, NoError and Internal Error | Check the start and job status. |
| `R` | `CtlReset()` | Remove the error cause and reset the status. |
| `J` | `CtlSimulationMode(true)` | The Simulation Mode |
| `H` | `CtlSimulationMode(false)` | Convert Hardware Mode only from approved installations. |
| `F` / `U` | Follow / Unfollow | Check the stage. |
| `V` | SyncAXIS Viewer | The simulation results. |
| `C` | The Last Job Characteristic Output | Scanner/Stage Similarity Limit |
| `O` | Scanner and Stage to the source point. | Hardware mode moves the real axis. |
| `F1`~`F3` | Quadratic: ScannerOnly / StageOnly / Complex | Motion Type Route Verification |
| `F4`~`F6` | Source: ScannerOnly / StageOnly / Complex | Arc and Motion Type Verification |
| `F7` | Scanner Calibration Pattern | Scan Head Field Pattern |
| `F8` | Laser Delay Pattern | `LaserSwitchOffsetTime`, `LaserPreTriggerTime` optimization |
| `F9` | Scanner/Stage Calibration | Confirm the statistic accuracy of Circle and Cross Grid |
| `F10` | System Delay Pattern | Scanner and Stage motivation delay verification |
| `Esc` | `CtlAbort()` | Does not replace the equipment's independent emergency stop |

`O`, `F1`~`F10`, `H` can control the real Stage, Scanner and Laser in the Hardware Mode. Do not read the code or run it just because the key is displayed on the UI.

## 9. Recommended Optimization Order

When you connect the flow in the SCANLAB Installation Manual to the F7 to F10 features in the demo, the following order is made.

1. Stage Error Mapping is completed by Stage manufacturer's procedure.
2. In Simulation, see Position·Velocity·Acceleration·Jerk in representative `StageAndScanner` Job.
3. Run the Laser Delay Pattern under lower conditions to find the starting values of `LaserSwitchOffsetTime` and `LaserPreTriggerTime`.
4. `ScannerOnly` Calibration Grid is processed and measured to create an optimized CT5.
5. After the correction, Mirror Positioning changes, so check the Laser Delay again.
6. Process `StageOnly` Cross Grid and `ScannerOnly` Circle Grid in the same set to verify the rotation, scale and position match.
7. System Delay Pattern in four directions ensures that Scanner and Stage are synchronized over time.
8. Finally, measure a wide range of Combined Motion Accuracy and record approval criteria.

Correction Pattern predicts static accuracy than high production speed and processes at low speed. Correction files do not cover the existing normal files, but generate and verify them with a separate name and then install them.

## 10. Status and Error Handling

```csharp
if (rtc.CtlGetStatus(RtcStatus.Busy))
    Console.WriteLine("syncAXIS is busy");

if (!rtc.CtlGetStatus(RtcStatus.NoError) &&
    rtc.CtlGetInternalErrMsg(out var errors))
{
    foreach (var error in errors)
        Console.WriteLine($"[{error.Key}] {error.Value}");
}
```

The syncAXIS function provides a call-by-called Return Code. If you diagnose multiple Return Code together with Bitwise OR, you may lose the first failure function, so check immediately after each call and record the function name, Job ID, Mode and original Code.

If the initiation fails, check in the next order.

1. XML file exists or Schema error
2. SCANLAB USB Dongle and Instance
3. Sirius3 syncAXIS License Option
4. `BaseDirectoryPath`, `ProgramFileDirectory`, CT5 and Log routes
5. ACS IP and EtherCAT Communication
6. RTC6 Serial Number, Program File, Firmware and Power
7. Connecting `SlecEtherCATNodeID` and Stage Axis
8. Scanning Head, Stage and Laser Cable
9. Original errors in syncAXIS Log and `CtlGetInternalErrMsg`

Not correcting the cause of error. `CtlReset()` Do not repeat it.

## 11. BandWidth and Motion Mode

- `BandWidth`: is a Trajectory Parameter that determines the distribution of the path between the Scanner and the Stage. the homogeneity of the machine axis and the Scanner Field should be considered together and the random big values do not guarantee better results.
- `MotionModes.Follow`: Stage is configured to follow the Scanner path.
- `MotionModes.Unfollow`: A behavior that does not use follow-up.
- `Trajectory`: Jump/Mark Speed, Stage/Scanner Limit, Laser Timing, etc. Includes the conditions required for Job Planning.

After changing the value, Simulate the same Job again and compare Job Characteristic and Viewer results.

## 12. References

- `demos/console_syncaxis_setup/Program.cs`: Startup, Mode Conversion, Motion Type Quadratic, Calibration and Delay Pattern, Status and Viewer
- `demos/editor_syncaxis/Form1.cs`: Editor-based device registration using `config_syncaxis.ini`
- `doc/SCANLAB/syncAXIS_V1.8.0_Installation_en-US.pdf`: Hardware installation, XML verification, Simulation first initial installation, Hardware verification, correction and motivation verification
- `doc/SCANLAB/syncAXIS_V1.8.0_API_en-US.pdf`: `slsc_cfg_*`, `slsc_list_*`, `slsc_ctrl_*`, Job, Buffer, Callback and Error Contract

If the installed syncAXIS software package, DLL, firmware, and manual versions differ, follow the documentation and headers distributed with that installed version.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
