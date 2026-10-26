# RTC Field Correction and 3D Calibration

> API reference: Sirius3 Core/UI 1.17.0 (public features)

[English](CalibrationUserManual.md) | [한국어](CalibrationUserManual.koKR.md) | [All guides](README.md)

## 1. Choose the correction task

| Task | What it corrects | Guide |
|---|---|---|
| 2D field correction | XY scale, rotation and position-dependent field distortion | [2D correction](Correction2dUserManual.md) |
| 3D calibration | Beam tilt, focus over the field, focus versus height and height-dependent size | [3D calibration](Correction3dUserManual.md) |
| Surface calibration | Focus for a measured/imported plane, cylinder, cone or freeform point cloud | [Surface points](EntityUserManual.md) |

`RtcCalibrationLibrary` wraps SCANLAB CalibrationLibrary. Use the matching
runtime and distribution files. Activation is managed by Sirius3; it is not a
public Config setting. Do not place activation codes in application source.

## 2. Match coordinates before calculating

1. A reference point is the position commanded to the scanner. A measured point
   is where the resulting mark was actually found. Both must use the same mm
   coordinate system, origin, axis directions and measurement plane.
2. Camera pixels must first be converted to that scanner coordinate system.
   Keep each measured point paired with its own reference point; matching only
   the number of samples is insufficient.
3. Use the source correction file and its KFactor, plus its readme file where
   the CalibrationLibrary operation requires one. Preserve a separate target file.

`XyCalibration` fits correction from paired reference/measured XY points. A
successful return and output file confirm calculation; remeasurement determines
whether the result meets the required accuracy.

## 3. Follow the 3D sequence

1. Check the source KFactor, then correct beam tilt and the XY field.
2. Calibrate focus over the Z=0 plane, then fit the focus coefficients A/B/C at
   several heights. Calibrate stretch when field size changes with height.
3. Apply each accepted result before measuring the next stage, so later samples
   use the correction file actually loaded on the controller.

`FocusCalibrationAtZ0`, `FocusCalibrationCoeffABC`, `GetCoefficient` and
`PointsCloudCalibration` are public wrapper operations. Their inputs differ;
use the [3D workflow](Correction3dUserManual.md) instead of treating every value
as an XY offset or a Z distance in mm.

## 4. Surface-specific correction and verification

A point cloud describes XYZ positions on a surface. `PointsCloudCalibration`
uses those positions to create a surface-specific correction file. Cylinder,
cone and plane workflows use their corresponding geometry parameters.

Check the returned error code, target file, active correction table, head,
KFactor and working distance. Keep original files and measurements. If the
runtime reports an activation or file compatibility error, resolve that error
before applying a result to the equipment.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
