# Laser Power Mapping & Compensation User Manual

> Reference version: Sirius3 1.12.3 (public Release features)


## 1. Overview

This guide explains how Sirius3 PowerMap compensates for nonlinearity between a laser source's control signal (input X) and measured optical power (output Y). An external power meter is used to build a calibrated output table, which can then be applied during marking to improve power consistency across the operating range.

## 2. System Components

- IPowerMap is the core engine to manage the LUT tables (LUTs) of the input value (X) compared to the exhaust value (Y).
- IPowerMeter is a hardware that measures laser output accurately (Ophir, Coherent, Thorlabs, etc.).
- ILaserPowerControl: Use the generated maping data to control output during actual processing.
- principle of operation:
  - Mapping (Mapping): Steps by step increases the control command (X) and measures and saves the actual output (Y).
  - LookUp: When a user instructs the target power (Y), the table is refined to extract the exact control value (X).

## 3. Calibration Workflow


Step 1: Hardware Preparation
- Install the power meter sensor in front of the laser output.
- In the UI, make sure the Scanner, Laser, and PowerMeter instances are connected normally.

Step 2: Power Mapping (Mapping Start)
- Click 'Power Map Start' to set the map range. (Min/Max Watt, Step Number)
- Categories Settings: The frequency has different laser characteristics, so it is recommended to divide and manage the categories into frequency units.
- Progress: The laser comes out step by step, and the power meter's exhaust values are automatically collected and recorded on the chart.

Step 3: Power Verification (Verify)
- Verify that the generated mapping data matches the current equipment state.
- When the set target power is outputed, check if the exhaust value enters the permissible error range.

Step 4: Power Compensation (Compensate)
- It is done if the power is reduced by long-term use.
- Starting from existing mapping data, calculate deviation and update the table to the current state (closed loop).

## 4. Real Time Processing

The maping data generated will be viewed in real time (LookUp) when the processing list is run.

- Lockup activation: `PowerMap.IsEnableLookUp = true` is required.
- Use the order list:
  - In the processing list call the `ListPower(targetWatt, category)` function.
  - The software calculates the optimal control signal value for the output of 'targetWatt' in the specified 'category' table in real-time and transmits it to the RTC controller.
- Advantages: Synchronized with the list run cycle of 10μs units, and the exact correction value is reflected without interruption, even if each line instructs a different output.

## 5. Key Features

- Category management: Multi-scanning heads, wave conversions (SHG/THG) and other physical conditions can operate independent maping tables.
- 1:1 Reset: Start to get the input value out (Y=X) without maping data.
- Data Persistence: Maping results can be stored and loaded in a `.map` file, making it easy to manage by device.

## 6. Cautions

- During the maping, check the cooling state for the sensor protection, and do not exceed the maximum allowed energy.
- Pre-Heat Time: Set enough pre-heat time to stabilize the laser for accurate measurements.
- K-Factor and Optical System: Field Correction must be advanced, and optical system pollution may undermine the reliability of the maping data.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
