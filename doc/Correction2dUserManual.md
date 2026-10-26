# 2D Scanner Field Correction User Manual

> API reference: Sirius3 Core/UI 1.17.0 (public features)

[English](Correction2dUserManual.md) | [한국어](Correction2dUserManual.koKR.md) | [All guides](README.md)

## 1. Why correction is needed

1. A 2D scan head uses X and Y mirrors to steer the beam. Mirror angle and the
   position on a flat work surface are not perfectly proportional.
2. With ordinary focusing geometry, distance from the optical axis follows
   approximately `r = f × tan(θ)`, where `f` is focal length and `θ` is beam angle.
   An f-theta lens is designed to bring this closer to `r = f × θ`. It reduces
   distortion, but installation, alignment and lens tolerances leave residual errors.
3. A measured correction table, such as a `.ct5` file for a compatible RTC,
   compensates for those remaining errors over the field. `KFactor` sets the
   overall bits/mm scale; it does not replace field correction.

Use the correction file supplied for the actual head, lens and working distance.
See [calibration overview](CalibrationUserManual.md) for the wider workflow.

## 2. Prepare and mark a measurement grid

1. Preserve the source correction file and create a different target filename.
   Confirm the selected head and the file's KFactor before measuring.
2. Use `CreateGridsForm` to prepare a grid pattern, then use the same rows,
   columns and spacing in the correction form. Grid spacing is in mm.
3. Mark and measure the reference positions using the equipment's approved
   low-power procedure. Record actual XY coordinates with a calibrated camera
   or measuring instrument. Resetting the grid clears existing measurement data.

A coarse grid is easier to validate. Add more points where the remaining error
requires them; a denser grid also increases measurement effort and the influence
of incorrect samples.

## 3. Enter the measured error

The form displays **dx = measured X − reference X** and
**dy = measured Y − reference Y**, in mm. For example, reference `(10, 5)` and
measured `(10.010, 4.995)` give `dx = 0.010`, `dy = -0.005`.

Both whitespace and a comma can separate the values:

```text
0.010 -0.005
0.010, -0.005
```

Map camera coordinates into the scanner coordinate system first. Match axis
direction, rotation, mirroring, origin and units. Keep the same row/column order
as the reference grid and reject missing, duplicate or nonfinite measurements.
Cell colors highlight large errors; use numeric tolerances to accept the result.

## 4. Convert, apply and verify

1. Export the measurements, then select the matching source correction file,
   KFactor and new target file. Convert creates the corrected file.
2. Check conversion success before Apply loads the result into the RTC table.
   Verify the active head and table number.
3. Mark and measure the same grid again. Compare the remaining XY errors with
   the process tolerance; file creation alone does not prove correction accuracy.

Changing the head, lens, working distance or mechanical alignment may invalidate
the measurements. Keep the original file and each verified result separately.
Use [3D calibration](Correction3dUserManual.md) when focus and field size must
also remain correct at different heights.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
