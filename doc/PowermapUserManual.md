# Laser Power Mapping & Compensation User Manual

> Reference version: Sirius3 1.16.0 (public Release features)

This guide covers the public Sirius3 PowerMap API and its WinForms/WPF controls.

## 1. Overview

PowerMap compensates for the nonlinear relationship between commanded laser power X and measured optical power Y. Both values are expressed in watts. Calibration records X/Y measurements; `LookUp` finds the input X needed for a requested output Y.

Each category stores an independent measurement table. A category can identify a frequency, laser head or other calibration condition. For example, the string `"50000"` can identify the table measured at 50 kHz; the category key itself does not set the laser frequency.

## 2. Components and Calibration Workflow

- `IPowerMap`: manages category tables and inverse lookup.
- `IPowerMeter`: measures optical output.
- `ILaserPowerControl`: applies power commands and can use an assigned PowerMap for compensation.

1. Connect and configure the scanner, laser and power meter. Place the sensor at the intended measurement position and confirm its cooling and measurement range.
2. Start Mapping in PowerMapControl. Select the category, input-power range and number of steps. The operation emits laser power at each step and records the measured output.
3. Choose the global interpolation mode described below, then run Verify at the target powers used by the application. Compare actual measurements with the permitted error range.
4. Run Compensate when output has drifted. This operation measures output and updates the table; it can emit laser power.

Use `Config.PowerMapPreHeatTimeMs` and `Config.PowerMapHoldTimeMs` to allow warm-up and settling. `Config.PowerMapInRangeThreshold` specifies the percentage threshold used by verification. Calibration quality must be checked by measurement; a smooth graph alone does not prove output accuracy.

## 3. Global Linear/PCHIP Interpolation

`Config.PowerMapInterpolationMode` is one global setting for all built-in PowerMap instances and all their categories. Categories have separate measurements, but do not have separate interpolation modes. Set the mode before validating or using compensated power.

| Mode | Behavior |
|---|---|
| `PowerMapCurveModes.Linear` | Joins adjacent measurements with straight segments. This is the default. |
| `PowerMapCurveModes.Pchip` | Uses shape-preserving cubic interpolation through the measurements. With two points, it is linear. |

```csharp
using SpiralLab.Sirius3;
using SpiralLab.Sirius3.PowerMap;

Config.PowerMapInterpolationMode = PowerMapCurveModes.Pchip;
// To use the default piecewise linear interpolation:
// Config.PowerMapInterpolationMode = PowerMapCurveModes.Linear;
```

PCHIP preserves changes in the measured trend. It does not automatically smooth measurement noise, force an increasing curve or remove multiple inverse solutions.

WinForms and WPF PowerMapControl use the selected global mode for the displayed calibration curve. Custom implementations of `IPowerMap` retain their own lookup behavior.

## 4. Public LookUp API and Multiple Solutions

Enable compensation with `map.IsLookUpEnable = true`. The public API remains:

```csharp
bool success = map.LookUp(category, targetYWatt,
    out double xWatt, out double xLeftWatt, out double xRightWatt);
```

- `targetYWatt`: requested measured output Y, in W.
- `xWatt`: selected input X, rounded to three decimal places.
- `xLeftWatt`, `xRightWatt`: original measurement X values bounding the selected solution.
- The default policy selects the smallest X when more than one solution exists. For a flat segment equal to the target Y, it selects the left endpoint.
- At least two measurements are required. Lookup fails for an unavailable category or a target outside the calibrated output range. PCHIP also reports failure when the rounded input cannot meet its prediction tolerance. Check the returned Boolean before using the output values.

The following example only inserts illustrative data and performs a calculation; it does not operate a laser:

```csharp
using System;
using SpiralLab.Sirius3;
using SpiralLab.Sirius3.PowerMap;

Config.PowerMapInterpolationMode = PowerMapCurveModes.Pchip;
IPowerMap map = new PowerMapDefault(0, "Calibration");
map.IsLookUpEnable = true;
map.Update("50000", 0, 0);
map.Update("50000", 10, 10);
map.Update("50000", 20, 0);

if (map.LookUp("50000", 5, out double x, out double left, out double right))
    Console.WriteLine($"X={x:F3} W, interval=[{left:F3}, {right:F3}] W");
else
    throw new InvalidOperationException("Power lookup failed.");
```

This table has two solutions for Y = 5 W. With PCHIP, the default result is X = 2.929 W in the [0, 10] W interval. Changing the global mode to Linear gives X = 5.000 W. Selecting the smaller input is the default rule; it does not establish which physical branch is suitable for a laser with hysteresis or output foldback.

`Update` rounds X and Y to three decimal places and replaces the measurement when the same X already exists. `Query` retrieves a stored X key; it does not interpolate. `IsLookUpEnable = false` bypasses inverse compensation.

## 5. Curves and Power Previews

WinForms and WPF PowerMapControl display measurements and calibration curves using `Config.PowerMapInterpolationMode`. Configure this global setting in the application; the control does not provide a separate curve settings or lookup dialog. Inverse lookup uses the default smallest-X selection.

When an `EntityPen` has a PowerMap assigned and a valid `PowerMapCategory`, its PropertyGrid Power editor provides a compact preview of that category. ManualControl provides the corresponding preview for its assigned map and selected category. The preview shows the curve and target/solution overlay without changing power or issuing a laser command. Numeric power editing remains available.

## 6. Applying Compensation During Processing

Assign the map to the laser's `ILaserPowerControl.PowerMap`, enable `map.IsLookUpEnable`, and pass the exact category key to `ListPower(targetWatt, category)` in the normal laser/scanner list workflow. The target is the desired output Y; the PowerMap calculates the compensated input X before the power command is recorded in the list. `CtlPower(targetWatt, category)` is the immediate-control counterpart.

In an editor document, assign the map and matching `PowerMapCategory` to the relevant `EntityPen`. A category name alone does not connect a map. Follow the application's normal device initialization, list submission and completion handling. Check lookup and command return values; controller timing and output behavior depend on the configured laser, scanner and power-control method.

## 7. Saving and Loading Tables

`PowerMapSerializer.Save` and `PowerMapSerializer.Open` use the existing category X/Y table format. `.map` files contain the original measurements. Interpolation mode, curve settings and calculated coefficients are not stored.

```csharp
if (!PowerMapSerializer.Save("calibration.map", map))
    throw new InvalidOperationException("Power-map save failed.");

IPowerMap loaded = new PowerMapDefault(1, "Loaded calibration");
if (!PowerMapSerializer.Open("calibration.map", loaded))
    throw new InvalidOperationException("Power-map load failed.");
loaded.IsLookUpEnable = true;
```

Loading preserves the application's current global interpolation mode and rebuilds curves from the table using the default smallest-X selection. The application must configure its global mode independently of the `.map` file.

A failed load leaves the existing table and runtime settings unchanged. Save or load only while the map is idle.

## 8. 1:1 Reset and Measurement Precautions

`Reset1to1(category, maxXWatt)` replaces that category with two points, (0, 0) and (maxXWatt, maxXWatt), for a positive maximum input. It restores a Y = X table rather than measuring or recalibrating the laser. Use `IsLookUpEnable` to enable or bypass compensation.

- Mapping, Verify and Compensate can emit laser power. Use the equipment's normal operating procedure and keep the sensor within its permitted power and energy range.
- Allow sufficient warm-up and settling time; confirm that the power meter's measurement units and range are correct.
- Keep frequency, optical path and measurement position consistent with production conditions. Check scanner correction and the optical system before calibration.
- Validate useful target powers after changing the interpolation mode or updating measurements. Optical contamination and laser aging can invalidate an earlier calibration.

## 9. Related Guides

- [WinForms integration](WinForms.md)
- [WPF integration](WPF.md)
- [한국어 매뉴얼](PowermapUserManual.koKR.md)

---
2026 Copyright (c) SpiralLAB. All rights reserved.
