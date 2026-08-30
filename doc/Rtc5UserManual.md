# SCANLAB RTC5 Controller User Manual

> Reference version: Sirius3 1.12.3 (public Release features)

## 1. Location of RTC5

RTC5/RTC5e provides a 20-bit XY coordinate system and more advanced marking features than RTC4. It offers larger lists and broader capability than RTC4, but does not support RTC6-only SCANAhead or Fly Extension features.

## 2. Core Specifications

- XY coordinates: 20-bit, Z coordinates: 16-bit
- Sirius3 Single List Capacity: 2<sup>20</sup> Command
- The table: 4
- Measurement Channels: 4
- Scanner move setup: 10 μs unit
- Laser delay: 0.5 μs unit

The Single List includes not only the shapes but also the Pen Laser I/O Atmospheric End Command. Long tasks use `ListBufferTypes.Auto` and Job status to distinguish the transmission from the actual completion.

## 3. Features Features Supported by Sirius3

- Jump, Mark, Arc, Raster, Timed Marking
- 2nd Head, 3D, Classic MoF
- Variable Jump/Polygon Delay and Jump Mode
- Sky Writing, Wobbel, ALC
- Character Set, Measurement, Free Variable
- RTC Serial Communication, Interrupt, Stepper

RTC5 does not provide `IRtcSCANAhead` and `IRtcMoFExtension`. Do not translate the SCANAhead·Fly Extension code for RTC6 because the name is similar.

## 4. KFactor and Correction

The unit of `KFactor` is bits/mm and **Controller position (bit) = User input position (mm) × KFactor (bits/mm)**. The KFactor is the full shrinking, and the `.ct5`/`.ctb` correction table is the compensation for the nonlinear distortion by location.

## 5. Delays and Advanced Features

Adjust Laser On/Off, Jump, Mark, and Polygon Delay to the real scan heads and laser responses. Sky Writing and ALC must verify the pre-conditions and combination limits of each function, and Wobbel is set in EntityPen. It is safe to change only one value at a time and verify it as a low-power test pattern.

## 6. Interfaces and Ports

RTC5 can provide LASER ports, extension I/O, Scan Head ports, MoF/Encoder, RS-232, Stepper features. The actual availability will vary depending on card options, firmware, cable and license.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
