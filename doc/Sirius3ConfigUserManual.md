# SpiralLab.Sirius3.Config Settings Guide

> Reference version: Sirius3 1.12.3 (public Release features)

## 1. Role and Configuration Timing

`SpiralLab.Sirius3.Config` is the static configuration surface for the core library. It centralizes logging, file paths, coordinate calculations, RTC measurement, PowerMap, and other settings that affect device and marking behavior.

Configure path and logging policies before `Core.Initialize()` whenever possible. Values changed at run time may not affect devices or calculation results that have already been created.

```csharp
using CoreConfig = SpiralLab.Sirius3.Config;

CoreConfig.LogPath = @"D:\SiriusData\Logs";
CoreConfig.CorrectionPath = @"D:\SiriusData\Correction";
CoreConfig.PowerMapPath = @"D:\SiriusData\PowerMap";
CoreConfig.IsLogToConsole = true;
CoreConfig.LogMaxArchiveDays = 30;

bool initialized = SpiralLab.Sirius3.Core.Initialize(
    minLogLevel: "Information",
    maxLogArchiveDays: CoreConfig.LogMaxArchiveDays);
if (!initialized)
    throw new InvalidOperationException("Sirius3 initialization failed.");
```

`Config` is static; do not instantiate it. Because `SpiralLab.Sirius3.UI.Config` has the same class name, use aliases such as `CoreConfig` and `UIConfig` to make intent explicit.

## 2. Version Information

| Property | Default / return value | Description |
|---|---:|---|
| `AssemblyName` | `SpiralLab.Sirius3.dll` | Core assembly filename |
| `AssemblyVersion` | Running assembly version | Read-only `Major.Minor.Build` version |

## 3. Logging

| Setting | Default | Explanation and Application |
|---|---:|---|
| `MaxLogItems` | `10,000` | Maximum rows retained by the WinForms log control; independent of file-log retention. |
| `IsLogEnable` | `true` | Enables internal logging. Keep it enabled in production to support diagnostics. |
| `IsLogToConsole` | `true` | Writes log messages to the console. Disable it in GUI applications when console output is unnecessary. |
| `LogMaxArchiveDays` | `90` days | Number of days file logs are retained. Pass the same policy to `Core.Initialize`. |
| `MinimumLogLevel` | Set by `Core.Initialize` | Read-only minimum log level currently in effect. |
| `OnLogged` | Event | Raised with the `LogLevel` and message for each new log entry. |

```csharp
CoreConfig.OnLogged += (level, message) =>
{
    // When we reflect on the UI, we switch to the UI thread.
    Console.WriteLine($"[{level}] {message}");
};
```

When you analyze a failure to start your device, don’t just look at the last Error, but keep the previous Information and Warning together. Adding the repeated frame and point unit logs directly can affect performance and file size.

## 4. File and Tool Paths

Undefined routes use the default folder below `AppDomain.CurrentDomain.BaseDirectory` The service, Visual Studio, and distribution programs may be different from the default folder running, so the product recommends the use of absolute routes.

| Setting | Basic route. | Purpose |
|---|---|---|
| `LogPath` | `siriuslogs` | Sirius3 log files |
| `MeasurementPath` | `measurement` | RTC High Speed Measurement and Diagnostic Data |
| `CorrectionPath` | `correction` | Scanner correction files (`.ct5`, `.ctb`) and the standard folder of the correction tools |
| `CorreXionProProgramPath` | `correction\CorreXionPro.exe` | SCANLAB CorreXion Pro file |
| `StretchCorreXion5ProgramPath` | `correction\stretchcorreXion5.exe` | Stretch correction tool |
| `CorrectionFileCoverterProgramPath` | `correction\CorrectionFileConverter.exe` | Fixed file conversion tool running file |
| `PowerMapPath` | `powermap` | PowerMap Mapping, Verification and Adjustment Data |
| `SyncAxisPath` | `syncaxis` | Root folder in syncAXIS settings |
| `SyncAxisViewerProgramPath` | `syncaxis\Tools\syncAXIS_Viewer\syncAXIS_Viewer.exe` | SyncAXIS Viewer file |
| `SyncAxisSimulateFilePath` | `siriuslogs` | SyncAXIS Simulation Output Log |
| `RecipePath` | `recipe` | Sirius3 Documents and Recipes |
| `ScriptPath` | `script` | SimpleScript C# source file |

When changing a path, verify that the folder exists, that the service account has read/write permission, and that external tools are installed at the expected location. Changing a path string does not install an external tool or activate its license.

## 5. Numeric Formatting and Path Generation

| Setting | Default | Explanation and point. |
|---|---:|---|
| `DecimalPrecision` | `3` | It is a minor number of characters that will indicate a error in the UI. 3 will indicate up to 0.001 mm standard. Not a value that reduces the internal calculation accuracy. |
| `MergeDistance` | `0.001` mm | It is a distance limit that combines continuous Jump/Mark commands close to the same location. It is used to reduce unnecessary duplicate movements and the output of the starting and end points. |
| `MinStepDistance` | `0.1` mm | It is the minimum length used when dividing Arc, Spline, Ellipse, Hatch, etc. into linear commands. |
| `VirtualJumpAndMarkAccScale` | `1.2` | The virtual RTC's Jump/Mark acceleration is the simulation rate that does not change the actual RTC tuning value. |

If `MergeDistance` is too big, different short moves can be treated as one. If you make `MinStepDistance` too small, the curve curve will be more detailed, but the number of RTC list commands and the preparation time will increase. Consider the actual optical measurement, the switch size, the speed, and the RTC list vacancy together and verify it.

## 6. CharacterSet

| Setting | Default | Explanation |
|---|---:|---|
| `CharacterSetMaxSerialNoUpdateTime` | `50` ms | It is a renewal cycle to verify the maximum serial number in CharacterSet. |

If the cycle is shorter, the UI renewal will be faster, but the verification task will be performed more frequently. in the mass document, check the actual processing volume and then adjust it.

## 7. Measurement

| Setting | Default | Explanation |
|---|---:|---|
| `MeasurementPlotMode` | `PlotModes.TimeChart` | Basic Plot method to display measurement data. |
| `MeasurementLaserOnFactor` | `1` | The conversion rate to apply to the LASER ON channel of the measuring data. |
| `MeasurementPath` | `measurement` | The folder to save the original and converted measurement data. |

Real Sampling cycles and channels are defined in the measurement interface and Measurement UI of the registered RTC. `MeasurementLaserOnFactor` is the display and conversion rate and does not replace the input voltage range or device protection limits.

## 8. PowerMap

| Setting | Default | Explanation |
|---|---:|---|
| `PowerMapPreHeatTimeMs` | `10,000` ms | Mapping, Verify, Compensate Preheating Time to Stabilize the Laser Before Mapping, Verify, Compensate |
| `PowerMapHoldTimeMs` | `5,000` ms | Time to maintain a stable output in each output condition. |
| `PowerMapInRangeThreshold` | `5.0` % | Assessment of measurement within the target range. |
| `PowerMapOutOfRangeThreshold` | `20.0` % | judgment by big errors. |
| `PowerMapCompensateRetryCounts` | `2` Meeting | The maximum number of automatic output. |

```csharp
CoreConfig.PowerMapPreHeatTimeMs = 15_000;
CoreConfig.PowerMapHoldTimeMs = 3_000;
CoreConfig.PowerMapInRangeThreshold = 3.0;
CoreConfig.PowerMapOutOfRangeThreshold = 15.0;
CoreConfig.PowerMapCompensateRetryCounts = 2;
```

This value is not an equipment safety limit. Follow the accuracy and cooling requirements of the laser and power meter, the meter response time, laser-emission precautions, and interlock requirements. Run mapping and compensation with real output only after the work area is safe and the devices are ready.

## 9. Initialization and Cleanup Example

```csharp
using CoreConfig = SpiralLab.Sirius3.Config;

bool coreInitialized = false;
try
{
    CoreConfig.LogPath = @"D:\SiriusData\Logs";
    CoreConfig.MeasurementPath = @"D:\SiriusData\Measurement";
    CoreConfig.CorrectionPath = @"D:\SiriusData\Correction";
    CoreConfig.RecipePath = @"D:\SiriusData\Recipe";
    CoreConfig.ScriptPath = @"D:\SiriusData\Script";

    coreInitialized = SpiralLab.Sirius3.Core.Initialize("Information", 30);
    if (!coreInitialized)
        throw new InvalidOperationException("Sirius3 initialization failed.");

    // Generate and use RTC, Laser, DIO, PowerMeter, Marker.
}
finally
{
    // Create the device and the document first.
    if (coreInitialized)
        SpiralLab.Sirius3.Core.Cleanup();
}
```

Call the `Core.Cleanup()` that corresponds only if `Core.Initialize()` is successful. The device and the document are safe in order to first call the `Dispose()` of that object and then close the Core.

## 10. Recommendations for Recording Changes

Config values affect the whole process. In the product, leave the values applied at the start in the log, and separate the recipes values from the entire Config values.

- `MergeDistance`, `MinStepDistance`
- Measurement Plot and Laser On
- PowerMap time · permission deviation · reboot
- Correction files, PowerMap, recipes and script routes
- Minimum log level and log storage period.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
