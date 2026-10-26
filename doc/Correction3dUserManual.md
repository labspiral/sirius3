# 3D Scanner Calibration User Manual

> API reference: Sirius3 Core/UI 1.17.0 (public features)

[English](Correction3dUserManual.md) | [한국어](Correction3dUserManual.koKR.md) | [All guides](README.md)

## 1. Prepare the system

Use a compatible RTC, 3D focusing scanner and the matching CalibrationLibrary
runtime. Prepare the source `.ctb`/`.ct5`, its readme file and the correct
KFactor. Preserve the original and save each result under a new name.

The calibration UI can move the scanner and change focus while you edit or
select measurements. Use the equipment's approved calibration procedure.

## 2. Measure in this order

1. **Beam tilt and XY field:** measure the center at two known heights to
   correct beam tilt, then mark and measure the XY grid. Apply each accepted
   result before continuing. See [2D field correction](Correction2dUserManual.md).
2. **Focus at Z=0 and A/B/C:** find the best focus across the reference plane,
   then at several known heights. Fit the focus curve from these samples.
3. **Stretch:** for optics whose field size changes with height, measure test
   patterns above and below the reference plane and correct the scale change.

## 3. Understand coefficients A, B and C

The focus table follows `Zout = A + B × l + C × l²`.

| Value | Meaning |
|---|---|
| `l` | The focal-length-deviation input in controller units; it is not directly a height in mm. |
| `A` | The output offset at `l = 0`. |
| `B` | The slope: how much output changes per unit of `l`. |
| `C` | The curvature: how the slope changes as `l` grows. |

Imagine a graph with **l** on the horizontal axis and **Zout** on the vertical
axis. A moves the curve up/down. With C=0 the curve is a straight line whose
slope is B; nonzero C bends it into a parabola. The controller converts the
requested position to its focus input, so do not substitute Z in mm for l.
`FocusCalibrationCoeffABC` fits measured input/output pairs; `GetCoefficient`
reads the coefficients from a correction file.

Keep the numeric basis with the data: calibration measurement arrays use 16-bit
values for CTB and 20-bit values for CT5. Returned correction-file A/B/C coefficients
use the 16-bit compatibility polynomial basis for both formats. Do not mix these
coefficients with raw 20-bit inputs or substitute mm heights into that polynomial.

## 4. Use an imported surface

1. Import the STL or another supported mesh using `IDocument.ActImport`.
2. Extract surface samples with `ActPointCloud` or `ActGridCloud`. Grid spacing
   is in mm; use a smaller spacing where surface curvature requires more detail.
   Inspect the resulting points from an oblique view.
3. Align the model with the real workpiece coordinate system, then pass the XYZ
   samples to `RtcCalibrationLibrary.PointsCloudCalibration` with the source
   correction/readme files and a separate target path. Check its return code
   and verify focus on the real surface before using the result for production.

The point cloud represents a surface, not the interior volume. Normals returned
by `ActGridCloud` help inspect the samples; the calibration operation takes the
positions. See [entity point-cloud extraction](EntityUserManual.md).

## 5. Save, apply and verify

Save measurement data before changing a grid. Apply only a successfully
generated result, then verify the selected head/table and repeat representative
measurements. A changed lens, working distance or installation requires another
check. File creation alone does not establish focus or dimensional accuracy.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
