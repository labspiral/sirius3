# EntityLayerPen User Guide

> Reference version: Sirius3 1.12.3 (public Release features)

`EntityLayerPen` configures RTC control state before processing a Layer. It selects Layer-wide behavior such as ALC (Automatic Laser Control), Sky Writing, Variable Polygon/Jump Delay, and the syncAXIS motion mode.

For per-entity laser output, frequency, pulse width, speed, conventional delays, Raster, Wobbel, and SCANAhead list settings, see [PenUserManual.md](PenUserManual.md).

## EntityLayerPen Role

`EntityPen` and `EntityLayerPen` are applied at different stages.

| Aspect | EntityPen | EntityLayerPen |
|---|---|---|
| Selection key | Entity `PenColor` | Layer `PenColor` |
| Application point | While entity commands are appended to the RTC list buffer | Before list-buffer generation starts for the Layer |
| Command type | List commands such as `ListLaserPower`, `ListDelay`, `ListSpeed` | Control commands such as `CtlSkyWriting`, `CtlAlc`, `CtlDelayVariable` |
| Typical features | Output, pulse, speed, conventional delays, Raster, Wobbel | ALC, Sky Writing, Variable Delay, syncAXIS |

The Marker finds the `EntityLayerPen` whose color matches the Layer's `PenColor`, applies it, and then records that Layer's entities in the RTC list buffer. The Layer pen therefore selects a Layer-wide processing mode; it is not merely a display color.

## Configuring properties in the editor

1. Select the Layer in TreeView.
2. Review or change the Layer's `PenColor` in PropertyGrid.
3. Select the `EntityLayerPen` with the same color.
4. Configure ALC, Sky Writing, Variable Delay, or syncAXIS properties.
5. Confirm that each entity in the Layer uses the intended `EntityPen`.
6. Confirm that the Marker's `LayerFirst` or `OffsetFirst` execution order matches the intended process.
7. With laser emission safely inhibited, run Preview or Simulation.
8. Use low output on a test workpiece to inspect Layer boundaries and start/end quality.

PropertyGrid shows only features supported by the registered RTC. For example, RTC4 does not expose Sky Writing or ALC. On RTC6, enabling SCANAhead Auto Delay can hide manual Variable Delay settings.

## Key Properties at a Glance

| Category | Main Properties | Purpose |
|---|---|---|
| ALC | `IsALC`, `AlcSignal`, `AlcMode`, `AlcModeExtensionBits` | Automatically modify the laser control signal depending on speed or location. |
| ALC range | `AlcPercentage100`, `AlcMinValue`, `AlcMaxValue` | Set the 100% reference value and the permitted output range. |
| Position-dependent ALC | `AlcByPositionTable` | Adjust ALC output as a function of position within the scan field. |
| Sky Writing | `IsSkyWritingEnabled`, `SkyWritingMode`, `TimeLag`, `LaserOnShift`, `Prev`, `Post`, `AngularLimit` | Add lead-in and lead-out motion so marking starts and ends at a more stable scanner velocity. |
| Variable Polygon Delay | `IsVariablePolygonDelay`, `VariablePolygonDelayEdgeLevel` | Adjust Polygon Delay according to the corner angle between consecutive Mark segments. |
| Variable Jump Delay | `IsVariableJumpDelay`, `VariableJumpDelayMin`, `VariableJumpDelayLimitLength` | Adjust Jump Delay according to Jump distance. |
| syncAXIS | `MotionType`, `BandWidth` | Select scanner-only, stage-only, or coordinated scanner-and-stage motion for the Layer. |

## Identification and Common Properties

### Name and Description

`Name` is the name to be displayed on the LayerPen list and `Description` is a note that describes the layer process mode. If you write colours and roles together, such as `White - Outline ALC`, `Yellow - Sky Writing`, it is easy to check the layer structure.

### PenColor

`PenColor` is the exact ARGB color that connects Layer and EntityLayerPen. In the default editing screen, it can only be displayed for reading for consistency of identification values, and the developer can specify it in the LayerPen generating event. The color that looks similar on the screen is a different pen if the ARGB values are different.

### ExtensionData

The user application is the extension data that keeps additional information such as the recipe key, the process step or the external device conditions. in `OnMarkLayerPen` you can read and choose the user Control command, but the default Marker does not interpret the meaning of it.

### IsAllowMark and Repeats

`EntityControl` The inherited value, however, is hidden in the standard PropertyGrid of EntityLayerPen and does not determine whether the layer is running or the number of repetitions. `EntityLayer` Use the settings.

## ALC Overview

ALC automatically changes an RTC output signal according to scanner speed or position. When the scanner slows at a line start, line end, or corner, ALC can reduce excessive energy accumulation and provide more uniform marking as speed changes.

You need to separate the next three.

- `AlcSignal`: Select which RTC output to change.
- `AlcMode`: Select which speed to change according to the standard.
- `AlcModeExtensionBits`: Choose additional calculation conditions, such as SCANAhead, MoF, etc.

ALC is used in the RTC5/RTC6 series implementing `IRtcAutoLaserControl` and is not available in the RTC4.

## ALC Activation

### IsALC

If `IsALC` is `true`, you will apply the ALC position table and signal and mode settings to RTC before you start Layer. If `false`, you will not use ALC in that Layer.

After the processing of one Layer, the markers will delete the ALC position table and disable the ALC. The next Layer will re-apply its `EntityLayerPen` settings, so don't rely on the ALC condition of the front layer to happen accidentally.

## ALC Output Signal

### AlcSignal

| Value | Exit and Unit. | The main concerns and attention. |
|---|---|---|
| `Disabled` | No output. | Apply only location adjustments or use when you disable ALC. |
| `Analog1` | 0~10 V | Control the analog output input of the laser. It is suitable when changing the output century whileining frequency and pulse width. |
| `Analog2` | 0~10 V | Use the second analog channel. check the line and input impedance. |
| `ExtDO8` | 0 to 255. | Control the laser or external device with the 8-bit digital output of EXTENSION2. |
| `PulseWidth` | µs | LASER1/LASER2 changes the pulse width according to speed. the laser must support a fast and linear PWM response. |
| `Frequency` | Hz | Changes the frequency of repetition of the laser within the permissible range. Changes the frequency also change the pulse interval so check the effect of the process. |
| `ExtDO16` | 0 to 65535 | Control external devices with the 16-bit digital output of EXTENSION1. |
| `SpotDistance` | mm | RTC6 SCANAhead controls pulse spacing along the path. Requires a PoD-capable laser or another supported external-trigger interface. |

When using `Analog1`, `Analog2`, `ExtDO8`, or `ExtDO16`, verify the RTC-to-laser voltage range, bit width, polarity, common ground, and safety interlocks.

## ALC Reference Speed

### AlcMode

| Value | Standards | time of use. |
|---|---|---|
| `Disabled` | No speed. | `AlcByPositionTable`,000 can be used when making location dependency adjustments. |
| `SetVelocity` | Speed ordered by RTC. | It can also be used in scan heads without digital location feedback. Real acceleration response does not reflect. |
| `ActualVelocity` | The real speed of scanhead. | Adjust from the compatible scan heads that provide iDRIVE feedback to the actual decrease in the start and end and corner. |
| `EncoderSpeed` | The external encoder speed. | It is used when controlling the speed of the conveyer or external movements based on the scanner. It requires a separate encoder speed setting. |

`ActualVelocity` does not act only by selecting the name. Compatible scan heads, RTC settings, feedback communications and firmware must be all ready. `EncoderSpeed` must first set the encoder source and cash value with `IRtcAutoLaserControl.CtlAlcEncoderSpeed`.

## ALC output range

### AlcPercentage100

The scanner speed is the output value used when it is 100% of the standard speed. the unit varies depending on `AlcSignal`.

- Analog: V
- PulseWidth: µs
- Frequency: Hz
- ExtDO8/ExtDO16: the exact output value

In `SpotDistance` this value is not used and the actual interval is specified as `EntityPen.SpotDistanceSCANa`.

### AlcMinValue and AlcMaxValue

ALC limits the output calculated not to exceed the scope of validity of the device. the minimum value should not be set lower than the bottom limit that the laser can operate stable, and the maximum value should not exceed the permissible scope of the laser input and RTC output.

For example, if the minimum frequency is too low when using the Frequency Signal, the laser emission may become unstable. In the Analog Signal, you need to check separately whether the input voltage and the actual output are linear.

## ALC Extensions

### AlcModeExtensionBits

In PropertyGrid, you can select multiple bits together. in the code, you directly specify the `AlcModeExtensionBits` bit flag, or add bit to the `AlcModeExtension` collection, which is the target of sequencing.

| byt | Purpose | Requirements or Conditions. |
|---|---|---|
| `None` | Do not use extension function. | Use the basic ALC. |
| `EncoderSpeedAddition` | Combin scanner speed and encoder speed into vectors. | You need an active MoF session. |
| `SCANAhead` | SCANAhead Preview Time and orbit information are used to calculate ALC. | It requires RTC6, SCANahead options, compatible heads and Auto Delay. |
| `InverseSpeedCorrection` | F-Theta lens and field position differences. | Real correction data and optical configuration verification are required. |
| `BackwardTransformation` | Converts speed after rotation and linear transforms back to the original coordinate basis. | Verify the supported RTC6 range. |
| `SkyWritingSDC` | In the speed range of Sky Writing, it alsoins Spot Distance Control. | The configuration of SCANAhead and SpotDistance is required first. |

The extension bit can be used together if the purpose is not overwhelmed, but it does not mean that the device supports all combinations. check the SCANLAB system package, RTC firmware, scan heads and laser input conditions together.

## Position-dependent ALC Correction

### AlcByPositionTable

Depending on the circumference from the field center, the ALC output specifies a multiplied distribution, which is used when the same command value is due to the lens and optical path when the same command value creates different results by location.

- Key: Circuit in the field center, unit mm
- Value: Output Division, 1.0 is 100%
- Validity range: 0 to 4
- Maximum number of points: 50
- Effective Field Size: Effective Field Size: Effective Field Size: Effective Field Size: Effective Field Size: Effective Field Size: Effective Field Size: Effective Field Size: Effective Field Size: Effective Field Size: Effective Field Size: Effective Field Size: Effective Field Size: Effective Field Size: Effective Field Size: Effective Field Size: Effective Field Size: Effective Field Size

RTC sets the tables in a round order and scores between points. If there is no 0% or 150% endpoint, it supplements the limit value, and if there is only one valid point, it applies the same distribution to the entire range. Items outside the range are ignored, and if there are items but there is no one valid point, the settings may fail.

```csharp
layerPen.IsALC = true;
layerPen.AlcSignal = AutoLaserControlSignals.Analog1;
layerPen.AlcMode = AutoLaserControlModes.Disabled;
layerPen.AlcByPositionTable = new List<KeyValuePair<double, double>>
{
    new KeyValuePair<double, double>(0.0, 1.00),
    new KeyValuePair<double, double>(10.0, 1.05),
    new KeyValuePair<double, double>(20.0, 1.12)
};
```

The values above illustrate the format; they are not calibration data. Measure output at each position with a PowerMeter and verify the result before and after compensation on a separate test workpiece. RTC5 Frequency/HalfPeriod modes use an inverse relationship, whereas RTC6 uses direct values, so revalidate the table whenever the controller generation changes.

## Spot Distance Control

Spot Distance Control keeps pulse spacing along the path stable while the scanner accelerates or decelerates. It does not set the optical laser spot diameter.

### Prerequisites

```csharp
layerPen.IsALC = true;
layerPen.AlcSignal = AutoLaserControlSignals.SpotDistance;
layerPen.AlcMode = AutoLaserControlModes.ActualVelocity;
layerPen.AlcModeExtensionBits =
    AutoLaserControlModeExtensions.Bit.SCANAhead;

entityPen.SpotDistanceSCANa = 0.01; // 10 µm
```

We need all the following conditions.

- RTC6 and SCANahead options
- Compatible ExcelliSCAN or intelliSCAN IV series scan heads
- Activation of SCANAhead Auto Delay
- Laser that supports PoD or Calendar Energy external trigger
- Right external trigger line between RTC and laser
- Minimum and maximum repetition frequency of the laser.

To keep the scale even during Sky Writing, add the `SkyWritingSDC` bit. If the speed is near to zero, the demand frequency is also near to zero, and the higher the speed, the demand frequency is greater, so the common fixed frequency laser makes it difficult to handle the entire range in a stable manner.

## Sky Writing Overview

Sky Writing adds run-in and run-out motion before and after a Mark vector. Laser emission begins after the scanner approaches the target Mark speed, then the scanner decelerates after the marked segment. This improves line-start, line-end, and sharp-corner quality.

It is useful for the small line, short line, small line and a lot of direction shift, which is difficult to match start and end with only the normal delay value. The additional movement is added, so the processing time and the necessary movement space are increased.

Sky Writing is used in RTC5 or higher implementing `IRtcSkyWriting` and Mode4 is used in RTC6.

## Sky Writing Modes

### IsSkyWritingEnabled

`true` applies the modes and parameters selected before Layer starts. `false` will set the Marker to `Deactivate` so that the Sky Writing of the previous Layer does not follow.

### SkyWritingMode

| Mode | Behavior | Suitable use |
|---|---|---|
| `Mode1` | Add forerun and return moves in front of Mark and perform slowing and retrace behind Mark. Real run-in acts twice as `Prev` and run-out acts twice as time as `Post`. | The most basic start-end quality verification |
| `Mode2` | Connect run-in and run-out locations with Sky Writing Jump instead of forerun/retrace. | Keeping accuracy while reducing the return time in Mode1 |
| `Mode3` | When the direction change between consecutive Mark is greater than `AngularLimit`, it acts in Mode2 mode, and the ordinary Polygon Delay is used for smooth connections. | When the quality and time are balanced in a lot of corner Polyline |
| `Mode4` | In addition to Mode3, it allows the short List command between Jump and Mark. | When using a complex list in RTC6 |

In the start and end of the Polyline, in the Jump, you can perform the basic Sky Writing behavior for Mode3/Mode4 as well as the quality of the start and end.

## Sky Writing Parameters

### TimeLag

The tracking delay between the scanner's command position and the actual position is a value expressed in time and the unit is μs. It is generally 0 to 10,000 μs range and is not practically activated at less than 0.25 μs.

In conventional Sky Writing, `TimeLag` defines the laser-off reference and `TimeLag + LaserOnShift` defines the laser-on reference. SCANAhead Auto Delay uses Preview Time and automatically calculated timing, so `TimeLag`, `Prev`, and `Post` may be hidden in PropertyGrid or may not directly determine timing.

### LaserOnShift

Adjusts the calculated Sky Writing laser-on time, in µs.

- Exit is delayed.
- The output is advanced.

`Prev` can be configured with 0.5 µs resolution on RTC5 and 0.015625 µs resolution on RTC6. It does not behave like `EntityPen.LaserOnDelay` or `LaserOffDelay`; do not mix the two timing systems while tuning.

### Prev

Run-in time reserved before marking, in µs. In Mode 1, the lead-in and return motion make the actual run-in time twice the configured value; Modes 2–4 use the configured value directly. It is normally handled in 10 µs increments. A value that is too small can start emission before target speed is reached; a value that is too large increases motion distance and cycle time.

### Post

The time of run-out setup to be secured after Mark ends and the unit is μs. In Mode1, due to slowing and reversal, the real run-out time is twice the setup value, and in Mode2~Mode4, the setup value is used as well. If too small, the endpoint quality may be worse, and if too big, the processing time will increase.

`Prev` and `Post` use the range of 0 to 655,350 μs, and 655,350 can be used as a special value for RTC to automatically select the default value based on `TimeLag`.

### AngularLimit

In Mode3/Mode4, Sky Writing applies the direction change criterion between the continuous Mark and the unit is the degree. the range is 0 to 180° and the standard default value is 90°.

This angle does not mean how much the scanner rotates, but is a direction change in the corner that the two Mark segments that lead to the laser to meet. For a more concrete connection than the standard, the ordinary Polygon Delay is used, and for the larger rotating corner, the Sky Writing is applied.

## Sky Writing Optimization

1. Generally `EntityPen` has a stable result first.
2. Enable Sky Writing to Mode1 and match `TimeLag` to the actual follow-up delays of the scanhead.
3. View the line start and end and adjust `LaserOnShift`.
4. `Prev`, `Post` is sufficiently secured and then reduced in the scope ofining quality.
5. If processing time is important, compare Mode2.
6. In Polyline with a lot of direction switching, Mode3 and `AngularLimit` are adjusted.
7. Use Mode4 only when the RTC6 complex list is needed.
8. Click SCANAhead Auto Delay or SDC and then check the full result again.

## Variable Polygon Delay

### Operating Principle

When `IsVariablePolygonDelay` is activated, it automatically reduces or increases `EntityPen.ScannerPolygonDelay` depending on the corner angle between the consecutive Mark.

`Scale = 1 − cos(φ)`

Here `φ` is the direction change angle in which two consecutive Mark segments meet.

| Change of direction. | The Division | Results |
|---:|---:|---|
| 0° | 0 | It is close to the straight line, so there is little additional Polygon Delay. |
| 90° | 1 | The same value as `ScannerPolygonDelay` is applied. |
| 180° | 2 | The reverse corner can be applied up to twice. |

This feature is not always the ability to turn the laser off from the corner. The more the direction changes significantly, the more time of delay is added, which ensures the scanner time to follow the new direction, and the more time of the laser output in the corner.

### VariablePolygonDelayEdgeLevel

If the calculated Variable Polygon Delay is greater than this criterion, RTC will turn the laser off behind `LaserOffDelay` and start again like a new Polyline.

This value should be two times smaller than the fixed `EntityPen.ScannerPolygonDelay` and will work on the actual quarter standard. even in a small corner, if too low, the laser can often disappear and the line can be cut off, and if too high, the heat can accumulate excessively in a rapid corner.

### Example

- The long straight line is divided into several short segments: it can reduce unnecessary delays.
- 90° corner repeated contexts: maintain a standard similar to the fixed Polygon Delay.
- Very urgent return: Over the Edge Level, the laser is turned off and processed on a new path to reduce excessive corner output.

## Variable Jump Delay

When the Jump distance is short, the delay depends on the length of movement so that you don’t wait for a long fixed Jump Delay every time.

### IsVariableJumpDelay

Variable Jump Delay applies only to RTCs implementing `IRtcVariableDelay`.

### VariableJumpDelayMin

The minimum delay to apply to Jump with a very short or length close to 0 and the unit is μs. It is usually set to be smaller or equal to the current `EntityPen.ScannerJumpDelay`.

### VariableJumpDelayLimitLength

The Jump distance to reach the fixed `EntityPen.ScannerJumpDelay` and the unit is mm.

- Very Short Jump: `VariableJumpDelayMin`
- Between 0 and Limit Length: Distance Ratio
- Limit Length: `EntityPen.ScannerJumpDelay`

Like a short hatch line moves, a small jump can reduce the time in the process that repeats. too small minimum delays can make the starting point of the next Mark errors and vibrations, so check the actual movement length separately.

## Relationship Between SCANAhead and Variable Delay

RTC6 SCANAhead Auto Delay automatically calculates delays with Preview Time and scanhead parameters, in this state it does not use manual Variable Polygon/Jump Delay together and the related properties are hidden in PropertyGrid.

After activating Auto Delay, optimize to the following order.

1. Make sure that RTC6 recognizes scan heads compatible with the SCANahead option.
2. In the scan header Pre-configuration, make sure the Preview Time, maximum speed and acceleration values are loaded correctly.
3. Check `IsActivateAutoDelays` and `IsSCANAhead`.
4. Start the SCANAhead Shift and Scale in `EntityPen` from the default value.
5. Trajectory ACK status and the actual start-end-end quality.
6. Adjust the Laser On/Off Shift and Scale to a small step only if necessary.

Preview Time indicates how pre-calculated the future trajectory will be delivered to the scanhead, and SCANAhead is the core input of automatic delay. It does not mean the same as the usual Laser/Scanner Delay, so don't replace it with the manual delay value.

## syncAXIS Motion Mode

syncAXIS is the ability to co-drive the transfer stages and scanners in a wide work area using a common `MarkerRtc` and a separate syncAXIS Marker.

### MotionType

| Value | Behavior |
|---|---|
| `ScannerOnly` | Move the scanner only. |
| `StageOnly` | Move the stage only. |
| `StageAndScanner` | The stage is responsible for big movements and the scanner supplements the fast domestic movement. |

### BandWidth

In `StageAndScanner`, the frequency criterion that defines the division of action between the stage and the scanner and the unit is Hz. The higher the value the stage is responsible for the faster orbit component, and the lower the scanner is responsible for the higher high frequency component. The value less than 0.23 Hz is not available.

BandWidth increases does not always improve performance. you need to consider the mass of the stage, the maximum speed and speed, the device power and the scanner field range together.

### Prerequisites

- SyncAXIS system pre-setup
- RTC6 syncAXIS license
- SCANLAB USB Dungeon
- Axial configuration, Stage Value and Coordinate Setup File
- Verify the setup with [`demos/console_syncaxis_setup`](../demos/console_syncaxis_setup)
- Marker and `IRtcSyncAxis` registration for syncAXIS

For more details, see [Rtc6SyncaxisUserManual.md](Rtc6SyncaxisUserManual.md).

## LayerFirst and OffsetFirst

Marker provides two orders when processing multiple Offset and Layer. even the same document varies according to the order when `EntityLayerPen` is applied and column accumulation, and the number of device conversions.

### LayerFirst

Process all Layers in each Offset and then go to the next Offset.

```text
Offset 1: Layer 1 → Layer 2 → Layer 3
Offset 2: Layer 1 → Layer 2 → Layer 3
```

The LayerPen will be reapplicated before each Offset/Layer combination starts, which is suitable when you need to process multiple layer processes on a consecutive basis in the same Offset location.

### OffsetFirst

Process one Layer in all Offset and then move to the next Layer.

```text
Layer 1: Offset 1 → Offset 2
Layer 2: Offset 1 → Offset 2
Layer 3: Offset 1 → Offset 2
```

After applying LayerPen, all of the Offset of that Layer are configured into one processing bond. It is suitable whenining the same layer conditions and processing multiple locations first.

### Relationship with Repeats

The number of repeats is determined by `EntityLayerPen.Repeats` not, but by `EntityLayer.Repeats`. Do not rely on LayerPen's hidden `Repeats` values. In repeated processing, the heat accumulation and cooling time between layers must be examined separately.

## Default Values

Sirius3 Editor is a representative value applied when creating a default LayerPen. It does not mean a security value that fits your device and processes.

| Property | Default |
|---|---:|
| `IsALC` | false |
| `AlcSignal`, `AlcMode` | Disabled |
| `AlcModeExtensionBits` | None |
| `AlcPercentage100`, `AlcMinValue`, `AlcMaxValue` | 0 |
| `AlcByPositionTable` | The empty list. |
| `IsSkyWritingEnabled` | false |
| `SkyWritingMode` | Mode3 |
| `TimeLag` | 250 µs |
| `LaserOnShift` | 0 µs |
| `Prev` | 300 µs |
| `Post` | 200 µs |
| `AngularLimit` | 90° |
| `MotionType` | ScannerOnly |
| `BandWidth` | 2 Hz |
| `IsVariablePolygonDelay` | true |
| `VariablePolygonDelayEdgeLevel` | 150 µs |
| `IsVariableJumpDelay` | false |
| `VariableJumpDelayMin` | 50 µs |
| `VariableJumpDelayLimitLength` | 0.5 mm |

## Developer: Change Default EntityLayerPen Values

If you subscribe to `SpiralLab.Sirius3.UI.Config.OnCreateLayerPen` before creating or initiating a Document, you can create a color-by-colored default LayerPen directly.

```csharp
SpiralLab.Sirius3.UI.Config.OnCreateLayerPen += CreateLayerPen;

private EntityLayerPen CreateLayerPen(IDocument document, Color color)
{
    return new EntityLayerPen
    {
        Name = color.ToKnownColor().ToString(),
        PenColor = color,
        Description = color.ToString(),
        IsALC = false,
        IsSkyWritingEnabled = false,
        SkyWritingMode = SkyWritingModes.Mode3,
        TimeLag = 250,
        Prev = 300,
        Post = 200,
        AngularLimit = 90,
        IsVariablePolygonDelay = true,
        VariablePolygonDelayEdgeLevel = 150,
        IsVariableJumpDelay = false,
        VariableJumpDelayMin = 50,
        VariableJumpDelayLimitLength = 0.5
    };
}
```

The event is static, so unsubscribe when the form or service closes. If the Document already exists before the subscription is added, existing pens do not receive the new defaults automatically.

## Developer: Customize Control Commands with OnMarkLayerPen

The default Marker applies the control command to the next order about before starting Layer.

1. `IRtcSkyWriting.CtlSkyWriting`
2. `IRtcAutoLaserControl.CtlAlcByPositionTable`
3. `IRtcAutoLaserControl.CtlAlc`
4. `IRtcVariableDelay.CtlDelayVariable`
5. `IRtcSyncAxis.CtlMotionType` and `CtlBandWidth` if needed
6. RTC and Laser List Begin
7. EntityPen and Object List Command Records in Layer

When you subscribe to `IMarker.OnMarkLayerPen` it will not be added to the default processing, but it will completely replace the default processing. User processor must verify the interface currently supported by RTC, and apply all the necessary control commands and then return the success.

```csharp
marker.OnMarkLayerPen += (currentMarker, pen) =>
{
    var rtc = currentMarker.Scanner as IRtc;
    if (rtc == null)
        return false;

    bool ok = true;

    if (rtc is IRtcSkyWriting skyWriting)
    {
        if (pen.IsSkyWritingEnabled)
        {
            double cosineLimit = Math.Cos(
                Helper.DegToRad(pen.AngularLimit));
            ok &= skyWriting.CtlSkyWriting(
                pen.SkyWritingMode,
                pen.LaserOnShift,
                pen.TimeLag,
                pen.Prev,
                pen.Post,
                cosineLimit);
        }
        else
        {
            ok &= skyWriting.CtlSkyWriting(
                SkyWritingModes.Deactivate, 0, 0, 0, 0, 0);
        }
    }

    if (rtc is IRtcVariableDelay variableDelay)
    {
        ok &= variableDelay.CtlDelayVariable(
            pen.IsVariablePolygonDelay,
            pen.VariablePolygonDelayEdgeLevel,
            pen.IsVariableJumpDelay,
            pen.VariableJumpDelayMin,
            pen.VariableJumpDelayLimitLength);
    }

    return ok;
};
```

The above example processes only Sky Writing and Variable Delay. If you use ALC or syncAXIS in a project, the default settings are missing. For full implementation, see [`demos/editor_pen/Form1.cs`](../demos/editor_pen/Form1.cs)'s `Marker_OnMarkLayerPen`. The event is called from the Marker task thread, so don't change the WinForms control directly.

## Demo: editor_pen

[`demos/editor_pen`](../demos/editor_pen) is a standard example that configures all the main properties of EntityLayerPen into code and converts it into the RTC Control command.

### Initialization Flow

1. The project copies the common [`demos/config.ini`](../demos/config.ini) to `config.ini` in the output folder.
2. Start the library with `Core.Initialize()`.
3. `EditorHelper.CreateDevices` read the INI settings and generates RTC, Laser, DIO, PowerMeter and Marker.
4. Register the device to the editor with `SiriusEditorControl.RegisterDevices`.
5. Complete the processing with `marker.Ready(document, view, rtc, laser, powerMeter)`.
6. When closing, fix the device and the UI and call `Core.Cleanup()`.

### Key Code

- `Config_OnCreateLayerPen`: ALC, Sky Writing, Variable Delay, syncAXIS
- `Marker_OnMarkLayerPen`: Support Interface Check and Control Command Apply
- `Config_OnCreateEntityPen`: Set List for the objects in Layer
- Page 1/Page 2 Processing button: Check the page-by-page running in the same Document

This demo subscribes both `OnMarkLayerPen` and `OnMarkEntityPen`.The two events replace the default Marker processing, so it’s useful to compare if there’s no missing features when creating a custom Marker.

## Demo: editor_pen_multiple

[`demos/editor_pen_multiple`](../demos/editor_pen_multiple) shows multiple EntityPen settings selected by entity color within one Layer. EntityLayerPen maintains the Layer-wide control state, while EntityPen changes the output and speed for each side of the rectangle.

You can check the following through this distinction.

- The full layer mode, such as ALC/Sky Writing, is configured once in EntityLayerPen.
- Variable values, such as output and MarkSpeed, are converted to EntityPen color.
- You can use the Measurement Begin/End object to measure whether LayerPen and EntityPen settings are reflected in the real signal.

Measurement channels register `LaserOn`, `SampleX`, `SampleY`, `PulseLength` with 10 kHz sampling. for more information, see [MeasurementUserManual.md](MeasurementUserManual.md).

## Demo: editor_scanahead_sdc

[`demos/editor_scanahead_sdc`](../demos/editor_scanahead_sdc) is a representative example that combines EntityLayerPen ALC with EntityPen Spot Distance Control.

```csharp
layerPenWhite.IsALC = true;
layerPenWhite.AlcByPositionTable.Clear();
layerPenWhite.AlcSignal = AutoLaserControlSignals.SpotDistance;
layerPenWhite.AlcMode = AutoLaserControlModes.ActualVelocity;
layerPenWhite.AlcModeExtension.Clear();
layerPenWhite.AlcModeExtension.Add(
    AutoLaserControlModeExtensions.Bit.SCANAhead);

entityPenWhite.SpotDistanceSCANa = 0.01; // 10 µm
```

The extension bits you can choose in the example are as follows.

- `SkyWritingSDC`: Sky Writing keeps SDC even in the speed range
- `EncoderSpeedAddition`: Combination of encoder speed in active MoF
- `InverseSpeedCorrection`: Field position-by-speed advance repair
- `BackwardTransformation`: Coordinate Conversion Backback Speed Conversion to Pre-Standard

This demo presupposes the actual RTC6 SCANahead option and the scan heads that are compatible. `rtc6.IsActivateAutoDelays = true` and `rtc.IsSCANAhead` verification must be advanced, and virtual RTC alone cannot verify the actual laser pulse interval.

## Feature Support Reference

| Function | RTC4/RTC4e | RTC5/RTC5e | RTC6/RTC6e | RTC6 syncAXIS |
|---|---|---|---|---|
| ALC | Unknown | support | support | Common syncAXIS path unsupported |
| Spot Distance + SCANAhead | Unknown | Unknown | Options and compatible heads need. | Star Confirmation. |
| Sky Writing Mode1~3 | Unknown | support | support | Common syncAXIS path unsupported |
| Sky Writing Mode4 | Unknown | Unknown | support | Common syncAXIS path unsupported |
| Variable Polygon Delay | support | support | support | Common syncAXIS path unsupported |
| Variable Jump Delay | support | support | support | Common syncAXIS path unsupported |
| `MotionType`, `BandWidth` | Unknown | Unknown | General RTC route missions | support |

Virtual RTC is useful in editing and simulation, but does not prove real ports, laser responses, licenses and scanhead tracking performance.

## Troubleshooting

### LayerPen values change but are not applied

- Make sure Layer's `PenColor` and registered `EntityLayerPen.PenColor` are exactly the same.
- Check that the Marker is `Ready` with the same Document and device.
- Make sure that the `OnMarkLayerPen` subscriber is not replacing the default processing.
- Make sure that the RTC implements the required interface.

### Output does not change after enabling ALC

- Check the combination of `IsALC`, `AlcSignal`, `AlcMode`.
- `AlcPercentage100`, make sure that the unit of the minimum/maximum value corresponds to the signal.
- Check the analog/digital port line and laser input settings.
- If `ActualVelocity` is, check the iDRIVE feedback and compatible scan heads.

### Configuration fails after adding the position-correction table

- Make sure the circumference is within the valid field range.
- Check if the rate is 0 to 4.
- Check if there are 50 points.
- Remove duplicate or incorrect entries, then dispose them in reverse order.

### Lines become longer or processing time increases significantly after enabling Sky Writing

Additional run-in/run-out movements may be the normal result. Reduce `Prev`, `Post` in the quality-managed range and compare Mode2 or Mode3. If you are using Auto Delay, check the Preview Time and SCANAhead settings first.

### Corners are too dark or contain gaps

- `EntityPen.ScannerPolygonDelay` Check it first.
- Check whether `VariablePolygonDelayEdgeLevel` is so high that output remains excessive, or so low that the effect is negligible at smaller corners.
- Do not randomly adjust Sky Writing and Variable Polygon Delay at the same time, but verify one by one.

### syncAXIS properties are visible but do not work

Check syncAXIS pre-setup, license, SCANLAB USB dongle, axis settings file, syncAXIS Marker and `IRtcSyncAxis` registration. in common `MarkerRtc` syncAXIS drive is not performed.

## Pre-application Checklist

- Layer color and EntityLayerPen color exactly match.
- Currently, RTC supports ALC, Sky Writing, Variable Delay or syncAXIS interfaces.
- The unit of the ALC signal and port lines are right.
- The feedback of the compatible scan heads is normal when using ActualVelocity.
- When using Spot Distance, we checked the SCANAhead, Auto Delay, PoD laser and external triggers.
- Sky Writing run-in and run-out motion remains inside the equipment's safe working area.
- We tested Variable Delay in real Jump length and corner angle.
- LayerFirst/OffsetFirst order and `EntityLayer.Repeats` match the process intended.
- Verify Layer transitions and start/end quality at low output on a test workpiece.

## Related Documents

- [PenUserManual.md](PenUserManual.md): EntityPen output, pulse, speed, delay, Raster, Wobbel, SCANAhead
- [MarkerUserManual.md](MarkerUserManual.md): Page, Layer, Offset and Marker Run Order
- [Rtc6UserManual.md](Rtc6UserManual.md): RTC6 hardware, port and basic command
- [Rtc6SyncaxisUserManual.md](Rtc6SyncaxisUserManual.md): syncAXIS Pre-setup and operation
- [MeasurementUserManual.md](MeasurementUserManual.md): measurement channel registration and sampling
- [Sirius3UIConfigUserManual.md](Sirius3UIConfigUserManual.md): Editor Basic LayerPen Setup

---

2026 Copyright (c) SpiralLAB. All rights reserved.
