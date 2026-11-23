# 3D Scanner Calibration User Manual

> API reference: Sirius3 Core/UI 1.17.2 (public features)
> Section 6 describes the 1.17.2 calibration dialog.

[English](Correction3dUserManual.md) | [한국어](Correction3dUserManual.koKR.md) | [All guides](README.md)

## 1. Prepare the system

Use a compatible RTC, 3D focusing scanner and the matching CalibrationLibrary
runtime. Prepare the source `.ctb`/`.ct5`, its readme file and the correct
KFactor. Preserve the original and save each result under a new name.

Use the explicit movement and focus controls to make measurements. Start with the
[public demo configured for virtual devices](../demos/editor_fieldcorrection_3d/README.md)
when learning the workflow. Use the equipment's approved procedure on a real scanner.

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
use the 16-bit compatibility polynomial basis for both formats. For the same
physical focus, `L20 = 16 × L16`, `Zout20 = 16 × Zout16`, and the equivalent
coefficients are `A20 = 16 × A16`, `B20 = B16`, and `C20 = C16 / 16`. In the
measurement grid, RTC4 and RTC5 use signed 16-bit values and standard RTC6 3D
uses signed 20-bit values. Use the bit basis shown in the grid, then let the
dialog convert the measurements to the source file's basis.

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

## 6. Integrated calibration workflow

The WinForms and WPF dialogs provide the same five measurement tabs. Choose
the source table, matching README and KFactor. The upper **Load** beside the
source file selects it on the RTC and is required before using measurement
commands. Use **Add**, **Remove** and **Clear** to edit the grid. Use the grid's
right-click menu to import or export measurement files. Row selection and
cell editing prepare the data. Device actions use the explicit **Move to
selected point** and **Apply focus setting** controls on the relevant tabs.

| Step | Measurement and check |
|---|---|
| **Beam tilt** | Keep commanded X=Y=0 and measure the spot center at Z=0, then at Z=+Height, using the source table axes. Enter `Dx = X(0) − X(+Height)` and `Dy = Y(0) − Y(+Height)` in mm. Use `0 < Height ≤` the positive README Z limit. If `CALCULATION_FAILED` occurs within range, check the matched README, units and measured shift. After loading the result, repeat the two measurements. |
| **XY field** | At Z=0, distribute at least three target XY points over the field. Record the actual X/Y positions in mm with the same origin and axes as the targets. Load the result and remeasure the reference positions. |
| **Z=0 focus** | Keep the workpiece at Z=0 and sample focus across the field. A 3×3 grid is recommended. Move to each row, enter the clearest focus setting in `Zout`, and use **Apply focus setting** to test a changed value. |
| **ABC focus** | At each known height, move to the selected point to read the RTC's `L`. **Read distance L** can refresh that row separately. Tune `Zout` with XYZ and L fixed. Use at least three distinct L values after CTB/CT5 bit conversion. The `A` field in measurement files stores measured `Zout`. The fitted coefficient `A` is shown with `B` and `C` in the result. |
| **Stretch** | Use at least two distinct nonzero Z heights, with positive and negative heights where practical. Record absolute target XYZ and measured XY positions in mm. After loading, repeat the same targets and compare their XY size at each height. |

Use the last accepted table for each new measurement step. ABC and Stretch
can be performed in either order after Z=0 focus. Their Z entries are signed
absolute heights from the same Z=0 plane. Follow the machine procedure for
positioning the workpiece at each height. The scanner move command addresses
the scan head and its optical focus. The dialog restores temporary focus
settings when switching steps or closing. Beam Tilt file calculation can run
without an RTC card once the measurements are available.

### Calculate, inspect and load each result

1. Enter a distinct target path with the source extension. The active tab
   suggests a file name, and a manually entered path stays when you change tabs.
2. Click **Calculate and save file**. The dialog rereads the saved file and
   shows its path after verification. Use **File values** to compare the source
   and result. On the ABC tab, the preview shows measured points and the saved
   fit after calculation. Recalculate after changing the inputs, paths or saved
   file. Open **ABC units / bit ratios** to check the RTC and file bit bases.
3. With the marker and RTC idle, click the tab's lower **Load** and confirm.
   The result becomes the new source. A matching README is selected if present.
   For example, loading `D3_2982_BeamTilt.ct5` and opening XY field suggests
   `D3_2982_BeamTilt_XyField.ct5`. Remeasure the relevant points before continuing.

### Manual Z range and jog

With Is3D enabled and a readable matching README for the primary table,
ManualControl shows the allowed Z range. An out-of-range XYZ move is cancelled
with a warning after checking the transformed position. The jog popup shows a
vertical command-Z slider beside XY: +Z is up and −Z is down. Release to move.
XY jog retains that Z. The slider is a command in mm, not a measured position
or a mechanical Z-stage control.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
