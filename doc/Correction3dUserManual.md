# Integrated 3D Calibration User Manual

> Reference version: Sirius3 1.12.3 (public Release features)


## 1. Overview

This manual describes how to use the Sirius3 3D Integrated Calibration Windows form using the SCANLAB RTC Controller and CalibrationLibrary v1.4.1.1.
3D correction is not only the geometric error correction of the scanner, but also an essential procedure to keep the laser focus (Focus) stable in the entire work area.

## 2. Prerequisites

- The RTC controller and the 3D scanner (e.g. varioSCAN) must be connected normally.
- It requires a default correction file (.ctb or .ct5) and a README text file for that file.
- You need to know the exact K-Factor (bits/mm) value.

## 3. Step-by-Step Procedure

Correction is recommended to go in the following order, and every time each step is completed, you need to press the 'Apply' button to reflect on the hardware, the accuracy of the next step will be increased.

Step 1: Beam Tilt Calibration
- Correction of the scanner's scanning and central scanning errors in varioSCAN.
- In the upper and lower plates, each measure the center point position to enter the gap (dx, dy) and the distance (Height) between the two plates.
- After clicking 'Calibrate', press 'Apply' to reflect when successful.

Step 2: XY Field Calibration (2D Field Correction)
- Correction of the scanner’s geometric errors (Scale, Rotation, Pincushion, etc.).
- After processing the grid pattern, measure and enter the actual processing position (Measured X, Y) for each target coordinate (Target X, Y).
- After 'Calibrate', press 'Apply' (Source File will then be automatically updated to the created file.)

Step 3: Focus Calibration at Z=0
- The basic work level (Z=0) adjusts the microscope so that the focus is constant in the entire area.
- In several points, change the value of A (Z control bit) and enter the most clear-focused value.
- When a value is modified, it is automatically called IRtc3D.CtlLoadZTable (A, 0, 0) internally.
- When selecting a cell, the scanner automatically moves to that location, and you can control the A value to check in real time.

Step 4: Focus Coeff A, B, C Calibration
- Calculates the focus-curve coefficients (`Zout = A + B·l + C·l²`) as a function of travel distance.
- Find the optimal focus control value (A) in various Z heights.
- When you select the position value X, Y, Z, the scanner’s position moves automatically, and the value L (Focal Length Deviation bits) is calculated automatically using IRtc3D.CtlZDistance.
- When a value is modified, it is automatically called IRtc3D.CtlLoadZTable (A, 0, 0) internally.


Step 5: Stretch Calibration
- If you do not use the Telecentric lens, it is necessary to adjust the stretch.
- Adjust the change in processing size (Scale) according to the change in height.
- In Z+, Z- level, each process the test pattern and enter the measurement value.

## 4. Key Features

- Open/Save (mouse click): You can save or re-call the measurement data entered as a text file.
- Manual Control: You can manually control the laser and scanner through the right panel to perform the test processing.
- Live Update: In the Focus-related tab, the Z position of the scanner will change in real time as soon as you modify the grid cell value.

## 5. Cautions

- Do not manipulate the scanner or hardware forced during the correction process.
- When you press 'Apply', the current charged correction file will apply to the actual hardware, and the subsequent correction will be accumulated based on this file.
- If the K-Factor values are not accurate, all the calculation results are erroneous.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
