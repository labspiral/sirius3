# SCANLAB RTC4 Controller User Manual

> Reference version: Sirius3 1.12.3 (public Release features)

## 1. Location of RTC4

RTC4/RTC4e is an earlier-generation controller with a 16-bit coordinate system and comparatively small list buffers. Sirius3 continues to support RTC4, but commands with names similar to RTC5/RTC6 may differ in available interfaces, units, and firmware requirements.

## 2. Core Specifications

- XY/Z coordinates: 16 bits
- Sirius 3 Single List Capacity: 8,000 commands
- The table: 2
- Measurement channels: 2
- Scanner time: 10 μs unit

The list enters not only the move command, but also the laser, delay, I/O and closing command. Long tasks use `ListBufferTypes.Auto` and leave the buffer free.

## 3. Features Features Supported by Sirius3

- Basic `Jump`, `Mark`, `Arc`, Raster, Timed Marking
- 2nd Head and 3D Options
- Classic MoF
- Variable Jump/Polygon Delay
- RTC4 Wobbel
- Measurement, Conditional I/O, Interrupt

RTC4 implementation does not provide `IRtcSkyWriting`, `IRtcAutoLaserControl`, `IRtcSerialComm`, `IRtcStepper`, `IRtcSCANAhead`. Do not apply the settings for RTC5/RTC6 to RTC4.

## 4. KFactor and Correction

The unit of `KFactor` is bits/mm and **Controller position (bit) = User input position (mm) × KFactor (bits/mm)**. KFactor adjusts the entire shrimp, and the `.ctb` correction file reduces the lens and scanner distortion depending on the field position.

## 5. Laser port and safety.

The functionality and polarity of `LASER1`, `LASER2`, `LASERON` vary depending on the Laser Mode and board settings. Do not assume the pin behavior of RTC5/RTC6 and check it by the hardware manual of the RTC4 generation, the real line, the voltage level and the oscilloscope measurement.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
