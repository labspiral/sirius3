# RTC Field Correction & 3D Calibration User Manual

> Reference version: Sirius3 1.12.3 (public Release features)


## 1. Overview

This manual describes how to use RtcCalibrationLibrary, which is provided to maximize the processing accuracy of the SCANLAB RTC controller.
This library provides 2D correction to adjust the geometric distortion of the scanner and 3D correction to maintain equal focus and size throughout the area during 3D processing.

## 2. Activation of the Library (Activation)

- It requires SCANLAB CalibrationLibrary Runtime and Sirius3 distribution files.
- Activation values are managed within Sirius3 and are not the public `Config` property. Do not record the activation code in the application code or settings file.
- When the API returns `ACTIVATION_CODE_INVALID`, check the installed Sirius3/CalibrationLibrary version and the distribution file and contact the supplier.

## 3. 2D Field Correction (XY Field Correction)

As the most basic adjustment, the scanner adjusts Scale, Rotation, Trapezoid, and Pincushion distortions.
- Method: Call the `XyCalibration` method.
- Required data:
  - Target Points: Theoretical coordinates (mm) with the processing command.
  - Measured Points: Coordinates (mm) measured at the actual marked positions.
  - K-Factor: the bits/mm of the correction file that is being used.
- Result: Based on the input source correction file (.ct5/.ctb), a new target correction file is created with error correction.

## 4. 3D Calibration Step-by-Step

It is strongly recommended to carry out the correction in the following order for perfect 3D processing.

Step 1: Check the K-Factor
- Use `GetCalibrationFactor` to verify the default scale coefficient of the corrected file.

Step 2: Beam Tilt Calibration
- Corrects axial misalignment between the scanner and varioSCAN. Enter the center offset (`dx`, `dy`) at the upper and lower planes.

Step 3: XY Field Correction
- Do 2D correction to ensure the geometric accuracy of the basic flat.

Step 4: Focus Calibration at Z=0
- Adjust the concentration to be stable at all points of the basic work level (Z=0).

Step 5: Focus Coeff A,B,C Calibration
- Extract the coefficient of the focus change curve (Zout = A + Bl + Cl2) according to the Zq movement to adjust the focus accuracy within the 3D volume.

Step 6: Stretch Calibration
- Finally adjust the variation of the processing size (Scale) according to the change in the height.

## 5. Specialized Geometries

In addition to the flat, you can create an optimized adjustment file on the surface of a specific shape.
- Points Cloud: Create a correction file adapted to the customized free curve (Point Cloud).
- Cylinder: optimized adjustment to the surface of the rounded processing material.
- Cone: optimized adjustment to the coronary processing surface.
- Plane: Correction of new processing plates that are swallowed or moved.

## 6. Utilities

- Get/Set Coefficient: Read or modify the A, B, and C focus coefficients directly within the correction file.
- Get/Set Stretch Factor: X, Manages the Stretch Factor.

## 7. Error Handling

If the work fails, you can identify the cause through `SlscErrorCodes`.
- NO_ERROR 0 : Success
- ACTIVATION_CODE_INVALID (1): License activation code error.
- COULD_NOT_OPEN_CORR_FILE (4): file path or access authorization problem.
- MISSING_README_PARAMS (11): There is no ReadMe text file matching the correction file or the content is defective.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
