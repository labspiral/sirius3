# EntityPen User Guide

> Reference version: Sirius3 1.12.3 (public Release features)

`EntityPen` groups the laser output, pulse, scanner speed, delay, and optional features used to mark an individual entity. An entity's line color selects the `EntityPen` with the same color. The Marker writes that pen's settings to the RTC list buffer the first time it encounters the color.

For ALC, Sky Writing, Variable Delay, and syncAXIS behavior modes that are first applied to the entire layer, see [LayerPenUserManual.md](LayerPenUserManual.md).

## Where EntityPen Is Applied

The processing flow for one entity is as follows:

1. Read the entity's `PenColor`.
2. Find the `EntityPen` with the same color in the Document.
3. If it differs from the previously active pen, append list commands for laser output, delays, speed, and other settings to the RTC list buffer.
4. Append the entity's Jump, Mark, Arc, or Raster command.
5. RTC executes the completed list buffer in order.

When entities with the same pen color are consecutive, redundant pen commands are not written repeatedly. Frequently alternating colors increases the number of list commands, so assign the same color to entities that share process conditions whenever practical.

> `EntityPen` color is a lookup key for process conditions, not merely a display color. If an arbitrary ARGB value does not exactly match a registered pen, no matching pen is found.

## Configuring properties in the editor

1. Choose the object to process in the editor.
2. In PropertyGrid, specify the object's `PenColor` as the desired color.
3. In the TreeView or Pen Editing screen, select the same color `EntityPen`.
4. In PropertyGrid, configure output, pulse, speed, delay, and optional features.
5. Confirm that the required devices are ready on the Marker and Scanner tabs.
6. With laser emission safely inhibited, inspect the motion path using Preview or Simulation first.
7. Verify the result with low output on a test workpiece, then tune the process parameters gradually.

PropertyGrid shows only features supported by the registered RTC and laser. When SCANAhead Auto Delay is active, conventional laser- and scanner-delay properties that are no longer used directly can be hidden.

## Key Properties at a Glance

| Category | Main Properties | Purpose |
|---|---|---|
| Laser output | `PowerMax`, `Power`, `PowerPercentage`, `PowerMapCategory` | Define rated output and the commanded output used for marking. |
| Pulse | `Frequency`, `PulseWidth`, `PulsePeriod`, `PulsePitch`, `PulseDutyCycle` | Configure repetition frequency, pulse width, and pulse spacing. |
| Laser timing | `LaserOnDelay`, `LaserOffDelay` | Align scanner motion with the start and end of laser emission. |
| Scanner | `JumpSpeed`, `MarkSpeed`, `ScannerJumpDelay`, `ScannerMarkDelay`, `ScannerPolygonDelay` | Configure non-marking motion, marking motion, and settling time for each operation. |
| Hard Jump | `IsHardJump` | Use Hard Jump instead of a normal Jump on supported RTC controllers. |
| Raster | `RasterMode`, `PixelTime`, `PixelPulses`, `PixelPeriod` etc. | Process image pixels with JumpAndShoot or MicroVector. |
| Wobbel | `IsWobbelEnabled`, `WobbelShape`, `WobbelFrequency`, amplitude properties | Superimpose periodic transverse and longitudinal motion on the base Mark path. |
| SCANAhead | `LaserOnShiftSCANa`, `LaserOffShiftSCANa`, scale properties, `SpotDistanceSCANa` | Adjust trajectory prediction, laser emission points, and distance-based pulse spacing. |
| syncAXIS | `MinMarkSpeed`, `ApproxBlendLimit` | Limit low speed and blending when the scanner and stages move together. |

## Identification and Common Properties

### Name and Description

`Name` is the name to be displayed on the pen list and `Description` is the description that records the process conditions or use. The basic pen uses the color name, but in real projects, when recording the description that shows the role like `White - Outline`, `Yellow - Hatch`, it makes it easier to review the document and acquire the site.

### PenColor

`PenColor` is the exact ARGB color that connects the object to EntityPen. In the default editing screen, it can be displayed as read only for consistency of identification values, and the developer can be specified in the pen generating event. Even the color that looks similar on the screen, the ARGB values are processed with different pen.

### ExtensionData

The user application is the extension data that keeps additional information such as the process number, the material code or the external recipe key. In `OnMarkEntityPen` you can read and apply the user rules, but the default Marker does not interpret the meaning of it.

### IsAllowMark and Repeats

The value is inherited from `EntityControl` but is hidden in the standard PropertyGrid of EntityPen and is not used for the purpose of determining whether the pen is running or the number of repetitions.

## Laser output.

### PowerMax

`PowerMax` is the maximum laser quality output imported from the connected `ILaser.MaxPowerWatt` and the unit is W. It is a read-only value, so the user does not set it directly. If the value is 0 or the laser has not yet been connected, there is no criterion to convert the output percentage to the actual W value.

### Power

`Power` is the actual output to be requested for processing and the unit is W. When `PowerMax` is greater than 0 it is limited to below `PowerMax`, even if you enter a value greater than the maximum output.

Check the next item together.

- The minimum and maximum output range that the laser actually supports
- The average permitted output in the selected frequency and pulse width.
- Material damage, evaporation and reflective light risks
- Use of PowerMap

### PowerPercentage

`PowerPercentage` is a subsidiary property that shows and sets `Power` as a percentage for `PowerMax`.

`Power = PowerMax × PowerPercentage ÷ 100`

For example, if `PowerMax` is 20 W and `PowerPercentage` is 25%, `Power` is 5 W. If `PowerMax` is not valid, the percentage cannot be refunded to W, so first check the laser registration status.

### PowerMapCategory

`PowerMapCategory` is the classification name to be used for output correction. the connected laser must provide `ILaserPowerControl.PowerMap` and the selected name must be registered in `IPowerMap.Categories`. it is used when the lens, wave, optical path or process heads need to distinguish the correction curve differently.

To create, verify, reward principles and customize `IPowerMap` please see [PowermapUserManual.md](PowermapUserManual.md).

## Pulse and Frequency.

### Frequency

`Frequency` is the number of pulse repetitions per second and the unit is Hz. If the frequency increases, the distance between the pulse is shorter at the same speed. Only, if the laser output control method is based on Frequency, this value can be part of the output command, not a simple repetition rate.

### PulseWidth

`PulseWidth` is the time when one pulse remains active and the unit is μs. If the laser output control method is based on the Duty Cycle, the implementation of `ILaser` can convert the output value to the pulse width, the final output wave must be verified together with the laser implementation and the device specifications.

### PulsePeriod

`PulsePeriod` is the length of a pulse cycle and the unit is μs.

`PulsePeriod = 10⁶ ÷ Frequency`

For example, 50 kHz corresponds to the 20 μs cycle. If you change `PulsePeriod`, `Frequency` is re-calculated internally.

### PulsePitch

`PulsePitch` is the theoretical distance between the nearby pulse when marked at a fixed speed and the unit is μm.

`PulsePitch = MarkSpeed × 1000 ÷ Frequency`

For example, if `MarkSpeed = 500 mm/s`, `Frequency = 50 kHz`, the pulse interval is about 10 μm. The interval above the actual material is influenced by the scanner acceleration, laser response, SCANAhead/ALC settings and optical system.

### PulseDutyCycle

`PulseDutyCycle` is the percentage of the laser pulse in one cycle.

`PulseDutyCycle = PulseWidth ÷ PulsePeriod × 100`

Check the Laser Manufacturer Specifications so that the DutyVie does not exceed the device's permission range.When this value is set, `PulseWidth` will be re-calculated.

### RTC time resolution.

When `SpiralLab.Sirius3.UI.Config.IsConvertToControllerResolution` is activated, the value you read in PropertyGrid is adjusted to the time unit that RTC can express.

| Property | RTC4/RTC4e | RTC5 | RTC6/RTC6e/Virtual |
|---|---:|---:|---:|
| The pulse. | 0.25 µs | 0.03125 µs | 0.03125 µs |
| The pulse. | 0.125 µs | 0.015625 µs | 0.015625 µs |
| Laser timing | 1 µs | 0.5 µs | 0.015625 µs |
| Scanner is over. | 10 µs | 10 µs | 10 µs |

The input value is a little different from the remarked value is not a mistake, but a result that fits the controller resolution.

## Scanner speed.

### JumpSpeed

`JumpSpeed` is the non-marking speed from the end of one laser path to the next marking position, in mm/s. Higher values reduce travel time but can increase scanner tracking error and vibration.

### MarkSpeed

`MarkSpeed` is the speed that follows the path while the laser is output and the unit is mm/s. It determines the energy per unit length with the output, frequency, and pulse width. If you only lower the speed, more energy can enter the same position, so check output and pulse interval together.

## Laser and Scanner Delays

The laser does not create a stable output immediately receiving the electric signal, and the scanner mirror does not reach the command position immediately. The delay value is the value to match this different response time.

### LaserOnDelay

Aligns laser turn-on with scanner motion at the start of a Mark segment.

- Too small: The start point can burn dark, or emission can begin before the scanner stabilizes.
- Too large: The beginning of the line can become short or disappear.

### LaserOffDelay

Aligns laser turn-off at the end of a Mark segment.

- Too small: The end of the line may be incomplete.
- Too large: The end point can become too dark or overprocessed.

### ScannerJumpDelay

It is stabilization time before you start the next command after the jump is finished.

- Too small: The starting point of the next line may be trembling or the position may be out of.
- Too much: the entire processing time increases unnecessarily.

### ScannerMarkDelay

It is the stabilization time before the follow-up movement begins after one mark is over.

- Too small: from the end point the mirror does not follow enough, so the line may be swollen or short.
- Too much: the processing time can increase and the heat can be concentrated at the end point.

### ScannerPolygonDelay

Adds dwell time at a corner where two consecutive Mark segments meet while the laser remains on. It does not turn the laser off. At a sharp direction change, the delay gives the scanner time to follow the new direction and increases laser emission time at the corner.

- Too small: Corners can become rounded or cut inside the intended path.
- Too large: Corners become excessively dark, heat is concentrated, and processing time increases.

### Delay Optimization Order

1. Disable Wobbel, Sky Writing, SCANAhead Auto Delay, and ALC.
2. Prepare a repeatable test pattern and use low laser output.
3. Set JumpSpeed and MarkSpeed to the actual process values first.
4. Inspect line starts and ends while tuning `LaserOnDelay` and `LaserOffDelay`.
5. Inspect positional settling between separate lines while tuning `ScannerJumpDelay`.
6. Inspect line ends and consecutive corners while tuning `ScannerMarkDelay` and `ScannerPolygonDelay`.
7. Reactivate the selection function one by one and confirm the results.

In a system with SCANAhead Auto Delay activated, do not randomly adjust the existing manual delay values and the automatic reward at the same time. The general delay properties are hidden in the editor means you use the Auto Delay path.

## Hard Jump

`IsHardJump` converts the ordinary Jump command from support RTC to Hard Jump. Hard Jump does not pass the 10 μs Microstep-based orbit range of RTC, and then moves quickly to the target location and then awaits as much as `ScannerJumpDelay`.

Hard Jump can reduce the time spent on many short non-marking moves, but it does not automatically enforce trajectory and acceleration limits. Long moves or high speeds can increase mirror vibration, overshoot, or tracking error.

- Acts only in RTC implementing `IRtcJumpMode`.
- Available in the RTC5/RTC6 series and not available in the RTC4 series.
- First create a stable condition with a common Jump and then compare it from a short move.
- You need to re-optimize `ScannerJumpDelay` when used.

## Raster processing

Raster scans the image pixels in order to express the luminous or pixels-specific conditions as a laser output. `EntityImage`, Raster mode is used for pixels-based objects, like the barcode.

### RasterMode

#### JumpAndShoot

Jump to each pixel position and then release the laser in the stop state.

- It is suitable when priority position accuracy.
- Jump every pixel, so the processing time can be longer.
- `JumpSpeed`, `PixelTime`, `PixelPulses`, `IsPixelPulsesExit` are the core.
- The usual piksel output uses the LASERON signal during `PixelTime`.

#### MicroVector

Use the Micro Vector command of RTC and usually LASERON keeps the active state and expresses the pixels values with a LASER1 pulse width or extended analog output.

- It may be faster than JumpAndShoot, but you need to check the location accuracy and scanner tracking status.
- `PixelPeriod` and `PixelTime` make up the pixel cycle.
- `PixelTime` should be smaller than `PixelPeriod`.
- Not used with ALC, Timed Mark, Wobbel, Sky Writing.
- The syncAXIS path does not use MicroVector.
- It only works on RTC that supports `IRtcRaster`.

### PixelTime

Laser active time per pixel, in µs. For images, pixel intensity from 0 to 1 scales `PixelTime`. Excessive values cause pixel blooming and heat accumulation; values that are too short may not allow the laser to respond fully.

### PixelPeriod

The entire cycle from MicroVector to the next pixel and the unit is μs. The theoretical speed when the pixel interval is known is as follows.

`Speed (mm/s) = Pixel spacing (mm) ÷ (PixelPeriod (µs) × 10⁻⁶)`

### PixelChannel

Selects the channel used to output MicroVector pixel values. When using extended analog channels such as `ExtAO1` or `ExtAO2`, verify the voltage range, polarity, and ground connection between the RTC extension output and the laser input.

### RasterDirection

- `Horizontal`: Use the horizontal scan line and move the line from the bottom to the top.
- `Vertical`: Use the vertical scanning line and move the line from the left to the right.

### IsRasterZigZag

When enabled, one raster row runs forward and the next runs in reverse, reducing return travel between rows. If forward and reverse timing differs significantly on the equipment, rows can become misaligned; compare the result with unidirectional rastering.

### PixelPulses

`PixelPulses` completes one JumpAndShoot pixel after counting the configured number of external laser synchronization pulses.

- `0`: Keep the LASERON during `PixelTime` in a common way.
- `1`~`65535`: Expect the number of external pulse edges specified within `PixelTime`.
- LASERON is not enabled until the specified pulse count is received. If no pulse arrives, the pixel is not emitted.
- More impulses than the target number are ignored.
- If `IsPixelPulsesExit = true` receives all the target impulses, it goes to the next pixel immediately.
- If `false` has been targeted, it will wait until `PixelTime` ends.

This feature requires an external laser motivation signal connection. Connect the `SYNC OUT` of the laser to the `DIGITAL IN1` of the RTC LASER port and check the TTL level, the signal layer and the active axis. The active axis must match the `IRtcSignalLevel.CtlLaserControlSignal` settings. RTC5 or more and use in JumpAndShoot.

## Wobbel

Wobbel is the ability to synthesize small cycle movements while following the basic Mark path, which is used for linear width expansion, energy distribution, welding full control or surface texture adjustment.

### IsWobbelEnabled

When not used, the Marker records `ListWobbelEnd` so that the Wobbel status of the previous pen does not lead to the next object.

### WobbelShape

| Shape | Description | Cautions |
|---|---|---|
| `Ellipse` | It synthesizes the progressive direction and the vertical and parallel direction; if the two progressive are the same, they are close to the round, and if different, they are the thunder. | If the parallel expansion is 0 it becomes a vertical sign movement in the direction of progress. |
| `Perpendicular8` | Create a form of eight characters in the center of the vertical direction of progress. | Used in RTC5 or more. |
| `Parallel8` | Create a form of eight characters in the center of progress and parallel direction. | Used in RTC5 or more. |
| `Defined` | Use the predefined user Wobbel format to RTC. | First you need to register with `IRtcWobbel.ListWobbelDefine`. |

RTC4 only supports the default Wobbel function, and the form-specific support range varies depending on RTC and firmware.

### WobbelFrequency

Wobbel is the repeated frequency and the unit is Hz. The water and the water are used to change the rotation direction, the implementation of the water is the hour direction, the water is the half hour direction. The absolute value must be less than 1000 Hz, and the scanner must be set lower than the traceable frequency.

### WobbelPerpendicular and WobbelParallel

- `WobbelPerpendicular`: vertical expansion in the direction of the main route, unit mm
- `WobbelParallel`: Progressive direction of the basic route and parallel expansion, unit mm

If you increase the spread and frequency significantly at the same time, the scanner may not follow the required trajectory, starting from low frequencies and small spread to check the actual scanner state and processing width.

## SCANAhead

SCANAhead is a feature in which RTC6 pre-calculates the future trajectory of the scanner and reflects the state of the compatible scanhead to accurately match the laser output and movement.

### Conditions of use

- RTC6 and SCANahead options
- Compatible ExcelliSCAN or intelliSCAN IV series scan heads
- Compatible RTC/Scanhead firmware and system package
- Auto Delay Set for SCANAhead
- `rtc.IsSCANAhead == true` check

In SCANAhead, it acts in the meaning of `TrAck`, not the usual `PositionAck` but the orbit tracking state. If the tracking error is scattered, `IRtc.CtlReset()` may be needed.

### LaserOnShiftSCANa and LaserOffShiftSCANa

Shifts laser turn-on and turn-off relative to the reference points calculated by SCANAhead.

- `LaserOnShiftSCANa`: Take the laser earlier.
- Water `LaserOnShiftSCANa`: The laser gets late.
- `LaserOffShiftSCANa`: Take the laser earlier.
- Water `LaserOffShiftSCANa`: The laser gets late.

The unit is μs and the RTC6 resolution is 0.015625 μs.

### CornerScaleSCANa

Continuous Mark is the ratio to adjust the trajectory of the corner that encounters. 100% prioritizes the corner shape, and if you lower the value, it allows more soft and faster trajectory. It is not applied in the real Sky Writing range.

### EndScaleSCANa

Mark is the ratio between the accuracy of the endpoint and the time of processing. 100% is the endpoint accuracy priority, and if you lower the value, the endpoint behavior can be faster, but the endpoint failure can be greater.

### AccScaleSCANa

It is the percentage of time when the laser is out of the speed range. 100% allows the out of the pre-speed range, and if you lower the value, the out of the start and end range can be reduced and the line length can be shorter. It is not applied to the real Sky Writing range.

### SpotDistanceSCANa

Distance between emitted pulses along the motion path, in mm. It is not the optical laser spot diameter. A value of 0 disables the SDC distance command.

To use this feature, you need to configure the following conditions in `EntityLayerPen` together.

- `IsALC = true`
- `AlcSignal = SpotDistance`
- `AlcMode = ActualVelocity`
- Add `SCANAhead` to `AlcModeExtension`
- Activation of SCANAhead Auto Delay
- External PoD of the laser or interim pulse input connection

To maintain SDC during Sky Writing, you need an additional `SkyWritingSDC` extension bit. For full settings, see the ALC section in [LayerPenUserManual.md](LayerPenUserManual.md).

## Properties for syncAXIS

### MinMarkSpeed

The minimum Mark speed is permitted when operating the scanner and stage together and the unit is mm/s. Only when the value is greater than 0 the `IRtcSyncAxis.ListSpeedMinMark` command is recorded.

### ApproxBlendLimit

Current blending is permissible limit and the unit is mm. Only when the value is greater than 0 the command `IRtcSyncAxis.ListApproxBlendLimit` is recorded.

Both properties are not common `MarkerRtc` but are used in the Marker for syncAXIS and `IRtcSyncAxis`. Pre-setup, license and SCANLAB USB dongles are required, so check [Rtc6SyncaxisUserManual.md](Rtc6SyncaxisUserManual.md) first.

## Default Values

Sirius3 Editor is a representative value applied when creating a default pen. It does not mean a security value that fits your device and process.

| Property | Default |
|---|---:|
| `Power` | 1 W |
| `Frequency` | 50,000 Hz |
| `PulseWidth` | 2 µs |
| `LaserOnDelay`, `LaserOffDelay` | 0 µs |
| `ScannerJumpDelay` | 250 µs |
| `ScannerMarkDelay` | 150 µs |
| `ScannerPolygonDelay` | 100 µs |
| `JumpSpeed`, `MarkSpeed` | 500 mm/s |
| `IsHardJump` | false |
| `RasterMode` | JumpAndShoot |
| `RasterDirection` | Horizontal |
| `IsRasterZigZag` | true |
| `PixelTime` | 100 µs |
| `PixelPulses` | 0 |
| `IsPixelPulsesExit` | true |
| `PixelPeriod` | 200 µs |
| `PixelChannel` | ExtAO2 |
| SCANAhead Shift | 0 µs |
| SCANAhead Scale | 100% |
| `SpotDistanceSCANa` | 0 mm |
| `IsWobbelEnabled` | false |
| `WobbelFrequency` | 100 Hz |
| The Wobbel | 0.5 mm / 0.5 mm |
| `WobbelShape` | Ellipse |
| `MinMarkSpeed`, `ApproxBlendLimit` | 0 |

## Developer: Basic EntityPen Generation Value Change

If you subscribe to `SpiralLab.Sirius3.UI.Config.OnCreateEntityPen` before you create or initiate a Document, you can create a color-by-colored default pen directly. a callback will be called for each `Config.EntityPenColors` that the editor uses.

```csharp
SpiralLab.Sirius3.UI.Config.OnCreateEntityPen += CreateEntityPen;

private EntityPen CreateEntityPen(IDocument document, Color color)
{
    return new EntityPen
    {
        Name = color.ToKnownColor().ToString(),
        PenColor = color,
        Description = color.ToString(),
        Power = 1,
        Frequency = 50_000,
        PulseWidth = 2,
        JumpSpeed = 500,
        MarkSpeed = 500,
        ScannerJumpDelay = 250,
        ScannerMarkDelay = 150,
        ScannerPolygonDelay = 100
    };
}
```

The event is static. Unsubscribe when the form or service closes so old instances are not retained.

## Developer: Customize the list command with OnMarkEntityPen

The default Marker records the List command in the next order when the pen color changes.

1. `IRtc.ListLaserPower`
2. `IRtc.ListDelay`
3. Scanahead List
4. The Hard Jump
5. `IRtc.ListSpeed`
6. Wobbel start or end
7. syncAXIS Minimum Speed and Blending Settings

When you subscribe to `IMarker.OnMarkEntityPen` it will not be added to this default processing, but it will completely replace the default processing. therefore, the user processor must record all the necessary commands and return the success of each call.

```csharp
marker.OnMarkEntityPen += (currentMarker, pen) =>
{
    var rtc = currentMarker.Scanner as IRtc;
    if (rtc == null || currentMarker.Laser == null)
        return false;

    // You can limit or replace the request output according to the procedural conditions.
    double requestedPower = Math.Min(pen.Power, 5.0);

    bool ok = rtc.ListLaserPower(
        currentMarker.Laser,
        pen.Frequency,
        pen.PulseWidth,
        requestedPower,
        pen.PowerMapCategory);

    ok &= rtc.ListDelay(
        pen.LaserOnDelay,
        pen.LaserOffDelay,
        pen.ScannerJumpDelay,
        pen.ScannerMarkDelay,
        pen.ScannerPolygonDelay);

    ok &= rtc.ListSpeed(pen.JumpSpeed, pen.MarkSpeed);
    return ok;
};
```

Like the above example, you can change `Power` in `OnMarkEntityPen` to apply the user process rules. Only, if you are a document using SCANAhead, Wobbel, Hard Jump or syncAXIS, the default command should also be included in the processor. The event is called from the Marker task thread so WinForms controls don't change directly and use the UI dispatch.

For full implementation, see [`demos/editor_pen/Form1.cs`](../demos/editor_pen/Form1.cs)'s `Marker_OnMarkEntityPen`.

## Demo: editor_pen

[`demos/editor_pen`](../demos/editor_pen) is a standard example of initiating the entire property of EntityPen and EntityLayerPen in code and converting it from the Marker event into the control list command.

### Initialization Flow

1. The project copies the common [`demos/config.ini`](../demos/config.ini) to `config.ini` in the output folder.
2. Start the Sirius3 library with `Core.Initialize()`.
3. `EditorHelper.CreateDevices` read the INI settings and generates RTC, Laser, DIO, PowerMeter and Marker.
4. Register the device to the editor with `SiriusEditorControl.RegisterDevices`.
5. Complete the processing with `marker.Ready(document, view, rtc, laser, powerMeter)`.
6. Clean the device and UI generated at closure and call `Core.Cleanup()`.

### Key Code

- `Config_OnCreateEntityPen`: Configuration of properties by default value
- `BtnPrepare_Click`: Generate objects with different pages and pen color
- `Marker_OnMarkEntityPen`: Convert `EntityPen` to RTC List Command
- `BtnMarkPage1_Click`, `BtnMarkPage2_Click`: Page Selection Processing

## Demo: editor_pen_multiple

[`demos/editor_pen_multiple`](../demos/editor_pen_multiple) allocates different pen colors for each range of one type, converting multiple process conditions in a row.

| Color | `PowerPercentage` | `MarkSpeed` |
|---|---:|---:|
| White | 25% | 100 mm/s |
| Yellow | 50% | 500 mm/s |
| Orange | 75% | 1,000 mm/s |
| Red | 100% | 2,000 mm/s |

Each pen's `JumpSpeed` is set to 1,000 mm/s. The four variables of the square each use different `PenColor` so that the Marker can check the process of renewing the EntityPen List command in the range boundary.

It also starts the 10 kHz measurement with `EntityFactory.CreateMeasurementBegin` and collects the `LaserOn`, `SampleX`, `SampleY`, `PulseLength` channels and then ends with the Measurement End object. The set pen can be used to verify how the conversion reflects the real signal and location. For more information, see [MeasurementUserManual.md](MeasurementUserManual.md) for measurement procedures.

## Demo: editor_scanahead_sdc

[`demos/editor_scanahead_sdc`](../demos/editor_scanahead_sdc) combines RTC6 SCANAhead and Spot Distance Control.

The core flow is the following.

1. Check RTC6 and `rtc.IsSCANAhead`.
2. Activate Auto Delay with `rtc6.IsActivateAutoDelays = true`.
3. Set the `PositionACKLimit` according to your needs. 0.01 mm of the example is 10 μm.
4. Set the ALC `SpotDistance`, `ActualVelocity`, `SCANAhead` extension bits on White `EntityLayerPen`.
5. Set the White `EntityPen.SpotDistanceSCANa` to 0.01 mm.

This demo predicts a real RTC6 system with the SCANAhead option and a compatible scanhead. The fact that the properties value can be set in Virtual RTC does not verify the actual SCANAhead/SDC behavior.

## Feature Support Reference

| Function | RTC4/RTC4e | RTC5/RTC5e | RTC6/RTC6e | RTC6 syncAXIS |
|---|---|---|---|---|
| Basic output speed. | support | support | support | support |
| Hard Jump | Unknown | support | support | Unknown |
| Raster JumpAndShoot | support | support | support | support |
| Raster MicroVector | support | support | support | not used. |
| Wobbel basic forms | support | support | support | Unknown |
| Wobbel 8 characters/Defined | The limits | support | support | Unknown |
| SCANAhead | Unknown | Unknown | Options and compatible heads need. | Unknown |
| `MinMarkSpeed`, `ApproxBlendLimit` | Unknown | Unknown | General RTC route missions | support |

The actual support will not be determined only by the card generation. Check the RTC options, firmware, scan heads type, laser interface and licenses together. RTC6 basic structure and ports please see [Rtc6UserManual.md](Rtc6UserManual.md).

## Troubleshooting

### Change the color of the object but the processing conditions don't change.

- Make sure the object's `PenColor` and the `EntityPen.PenColor` registered in the Document are exactly the same.
- Make sure `marker.Ready` is re-called using the current Document and device.
- Make sure that the `OnMarkEntityPen` subscriber is not replacing the default processing.

### If you change the output percentage, power will not change.

- Make sure the laser is registered in the editor.
- Make sure `ILaser.MaxPowerWatt` and `PowerMax` are greater than 0.
- Make sure `PowerMapCategory` is a real registered class.

### Entered pulse or delay values differ slightly

The result may be rebounded according to the RTC time resolution. check the `Config.IsConvertToControllerResolution` and RTC generation.

### Raster line is opposed.

Disconnect `IsRasterZigZag` and compare it with one-way results. check the two-way laser delay, scanner tracking and the pixels cycle.

### PixelPulses does not have a laser.

Check the physical connection from the laser `SYNC OUT` to RTC LASER-port `DIGITAL IN1`, including TTL level, common ground, and active edge. Compare with `PixelPulses = 0` to isolate the problem from the ordinary timing method.

### SCANAhead properties are not visible or operate.

RTC6 cards, SCANahead options, compatible scan heads, system packages and firmware, `IsSCANAhead`, Auto Delay are activated.

## Pre-application Checklist

- The object color and EntityPen color exactly match.
- `Power` is within the safety range of the laser and material.
- Frequency, pulse width and dutivity satisfy the laser specifications.
- After changing speed, we checked the laser scanner delay again.
- When using the external pulse of Raster, the SYNC OUT line and signal levels were verified.
- The Wobbel expansion and frequency is within the scanner tracking range.
- Checked the hardware options for SCANAhead/SDC and LayerPen ALC conditions.
- Verify first with low output and laser emission safely controlled.

## Related Documents

- [LayerPenUserManual.md](LayerPenUserManual.md): EntityLayerPen, ALC, Sky Writing, Variable Delay, syncAXIS mode
- [MarkerUserManual.md](MarkerUserManual.md): Page, Layer, Offset and Marker Run Order
- [PowermapUserManual.md](PowermapUserManual.md): Mapping output, Verify, Compensate
- [Rtc6UserManual.md](Rtc6UserManual.md): RTC6 hardware and basic behavior
- [Rtc6SyncaxisUserManual.md](Rtc6SyncaxisUserManual.md): syncAXIS setup and operation
- [MeasurementUserManual.md](MeasurementUserManual.md): measurement channels and sampling
- [Sirius3UIConfigUserManual.md](Sirius3UIConfigUserManual.md): Editor's default pen and display settings

---

2026 Copyright (c) SpiralLAB. All rights reserved.
