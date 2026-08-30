# RTC Stepper Motor Control User Manual

> Reference version: Sirius3 1.12.3 (public Release features)


## 1. Overview

This guide explains how Sirius3 `StepperControl` drives up to two motors through the Stepper Port on a SCANLAB RTC5/6 controller. Typical uses include rotary-index motion and step-and-repeat positioning of an XY stage.

## 2. System Preparation and Setup (Setup)

- The STEPPER port on the back of the RTC card must be connected with a steper driver.
- Setup of unit measurement: Supports the MilliMeter (linear axis) or Degree (conversion) units for each separation.
- Scale Factor: The number of steps required to move 1mm (or 360 degrees) must be accurately set and the coordinate movement is possible.

## 3. Status Monitoring (Status Indicators)

You can check the real-time state of each engine at the top of the UI.
- Pos/Vel: Current absolute position and motion speed.
- Busy: Locations move or climb when the processing command is being performed.
- Init: The reference run is ongoing.
- Limit: When the limit switch is detected, it scrolls in red.
- Home (L): Indicates that homing completed successfully and the coordinate system was initialized.

## 4. Reference Run / Homing

For accurate position control, complete a reference run (homing) before operation.
- First navigation (Vel 1): move to the specified direction and browse the limit switch.
- Separation and Secondary Navigation (Vel 2): After switch detection, it is out as much as the determined tolerance (Tolerance), then low-speed accurately re-switch to establish the source point position.
- Home Position: Enter the coordinate value to be given to that point after the navigation is completed.

## 5. Movement Control

- Enable: Recognize or block the drive stream of the engine. (not that the fixed turn is dissolved when check off)
- Disable Switch: If you need an infinite rotation of 360 degrees, like a rotation, you can set it to ignore the limit switch detection.
- Move List: You can manage coordinates and speeds you frequently use up to 20 lists.
  - Absolute: Go to the set source point standard to the absolute coordinate.
  - Relative: Move by the specified distance from the current position.
  - Drag-and-Drop: It is possible to draw and change the item order in the list by mouse.

## 6. Safety Warnings (Safe Warnings)

1) Step-out Note: The RTC card’s Step-Control feature does not include the ‘Ramp-down’ feature.
A sharp speed change can cause the engine’s deforestation, so use the filter function of the hardware driver itself or set the appropriate speed in the software.
2) Emergency Abortion Independence: The processing stop command (CtlAbort) of the RTC card does not stop the operations of the steper engine.
In emergency situations, you must click on the 'STOP' button separately or call the `CtlStepperMoveStop` command to force the motor pulse output to block.
3) Holding Torque Disappearance: When the engine Enable is disabled (Reset), the current in the engine is blocked and the power to keep the position disappears.
At the time of the Zack (direct) control, the axis may be flowed by the wire, so be careful (confirm whether there is a brake function).

## 7. Data Persistence

- The input source point return parameters and Move List data are automatically stored in the Windows Register and will be retained even when the program is restarted.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
