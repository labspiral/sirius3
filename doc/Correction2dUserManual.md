# 2D Scanner Field Correction User Manual

> Reference version: Sirius3 1.12.3 (public Release features)

## 1. Why Field Correction Is Necessary

Galvanometer-mirror rotation is not perfectly proportional to linear motion on the work surface. F-theta lens nonlinearity, optical-axis decentering, scan-head installation angle, and lens tolerances all introduce position-dependent error. Adjusting KFactor corrects the overall scale, but barrel, pincushion, skew, and local distortion remain. 2D Field Correction reduces these nonlinear errors using XY deviations measured at multiple positions across the scan field.

## 2. Prerequisites

- Initiated RTC and scan heads
- The original `.ctb` or `.ct5` correction file that corresponds to the current optic system
- Grid test patterns that can be processed with low output
- A vision system, measuring microscope, or other equipment capable of measuring actual coordinates
- The way to keep the original/measure data/results files separately

Field-correction work may emit a real laser beam. Apply the equipment safety procedure and low-power test conditions before starting.

## 3. Source, Target and KFactor

- Source File: The original correction file you are currently using
- Target File: The way to save the results into a new file
- KFactor: bits/mm unit coordinates

The conversion form is **Controller position (bit) = User input position (mm) × KFactor (bits/mm)**. Make sure KFactor and Source correction files are the same optical field standards.

## 4. Measurement Grid

1. Select the full number Rows/Columns (e.g. 5×5, 9×9, 17×17).
2. Set the Row/Column Interval (mm).
3. Start the Grid with Reset This task deletes the existing cell data, so export it first.
4. Processing the Grid of the theory coordinates.
5. Collect real coordinates with vision or measurement equipment.

If you have more points, you can describe the local defect in more detail, but the impact of the measurement time and the wrong point is also increased. It is safe to increase the required area only gradually, starting with the Strong Grid.

## 5. Entering Error Data

The 2D correction grid in Sirius3 1.12.3 can differentiate dx/dy as **discovered or sheltered**.

- `0.010 -0.005`
- `0.010, -0.005`

dx/dy should fix the difference between the theory and the measurement point to the direction required by the API. When using the “measurement value – theoretical value” provided by the external vision, first unite the coordinate direction, camera rotation, reverse and unit. It is advisable to put the known defect in the center and four corners and then verify the code and then import the entire data.

In 1.12.3, the cell color according to the defect distance is re- displayed. color is an assistant means to find the ideal points, and whether the accurate permission is judged by the numbers and the process gap.

## 6. Create RtcCorrection2D with external vision data

The developer can arrange the vision coordinates into a line-line order and then add theoretical coordinates and relative defects to `RtcCorrection2D`. Grid's source point, line-line direction, column direction and interval must match the UI indication. Failure point, duplicate point, NaN/Infinity, unit disagreement first passed and then open the 2D correction WinForms in `UI.Config.OnScannerFieldCorrection2DShow` to review, store and apply.

## 7. Conversion and Application

1. Backup the measurement data to Import/Export.
2. Create a target file with a converter.
3. Check the error message and the result file.
4. Apply to RTC Correction Table and select it.
5. Repeat the same grid and check the rest.

Not just the fact that the file was created, the correction was successful. check the applied Table number, the current Primary Head, KFactor, the axis direction and the actual residues together.

## 8. Cautions

- Do not cover the original correction file.
- If you change the grid structure and reset, the existing data will be deleted.
- One or two big ideals can distort the whole fixing side.
- Do not apply the Matrix twice between the camera coordinates and the scanner coordinates.
- The correction results are only valid for the combination of the scanhead lens, work distance and installation state.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
