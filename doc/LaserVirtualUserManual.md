# Laser Power Control & RTC Integration User Manual

> Reference version: Sirius3 1.12.3 (public Release features)


## 1. Overview

This guide describes the `LaserVirtual` virtual laser-control implementation. Use it as a reference when deriving a custom laser object for a specific laser-source vendor.
The Sirius3 laser control system uses the hardware resources of the RTC controller to control the laser source output in a variety of ways.
Specifically, if combined with the 'List' command, the scanner's movement and super precision power modification synchronized to 10μs units are possible.

## 2. Power Control Methods

Depending on the interface specifications of the laser source, select and set one of the methods below.

1) Frequency variation (Frequency variation)
   - Principle: Input power (W) proportionally varies the laser output frequency (Hz).
   - Goal: CO2 or specific DPSS laser that linearly changes energy depending on frequency.
   - Hardware: RTC LASER1/2 port frequency signal.

2) Duty Cycle (Pulse Breadth Modification)
   - Principle: The frequency is fixed, and the pulse’s ON/OFF ratio (Duty %) is regulated to control the average output.
   - The most laser sources using the PWM control method.
   - Hardware: RTC LASER1/2 port’s Pulse Width signal.

3) Analog (analog tension)
   - Principle: Accept the voltage between 0 and 10V to the laser source to control the power.
   - Hardware: RTC ANALOG OUT 1 or 2 ports (12-bit disorder).

4) Digital Bits (Digital Bits)
   - Principles: Send 8 bit (0 to 255) or 16 bit (0 to 65535) digital values directly.
   - Hardware: RTC EXTENSION 1 (16-bit) or 2 (8-bit) ports.

5) RS-232 (Serial Communication)
   - Principle: RTC transmits a string of a specific format (e.g. "P50.0") through the built-in serial port.
   - Characteristics: The list command (`ListSerialWrite`) allows real-time transmission during processing.

## 3. ListPower and RTC Control Processing (Real-time Processing)

When the `ListPower(targetWatt, category)` function is called, the software creates the RTC command through the following processes internally.

Step 1: Power Mapping Lookup
- If `category` is specified, look at the power maping table in that category to calculate the `compensatedWatt` with nonlinear adjustment.

Step 2: Percentage Calculation
- `compensatedWatt / MaxPowerWatt * 100` produces control ratio between 0 and 100%.

Step 3: RTC hardware command maping
Depending on the chosen control method, the following command is recorded in the RTC list buffer.
- Frequency: `rtc.ListFrequency(newFreq, pulseWidth)`
- Duty Cycle: `rtc.ListFrequency(freq, newPulseWidth)`
- Analog: 
  - RTC5: `rtcIO.ListWriteData(ExtAO1/2, voltage)`
  - RTC6: `rtc6.ListLaserPower(ExtAO1/2, voltage)` -> Optimized Power Only Use Command
- Digital Bits: `rtcIO.ListWriteData(ExtDO8/16, bits)`
- RS-232: `rtcSerialComm.ListSerialWrite(formattedString)`

Step 4: Stabilization Wait (Settling Delay)
- Immediately after the power change, the `rtc.ListWait` command is added as much as `PowerControlDelayTime`, which ensures the laser source the time to physically change the output.

## 4. Key Advantages

- Changes without delay: Changes in power within the processing list, reflect immediate output, regardless of the performance of the PC, according to the hardware timing (10μs).
- Optimized Communications: If the same power value is directed consistently, it automatically eliminates unnecessary RTC command generation to efficiently use list memory.
- Guide Control: The `CtlGuide` feature allows you to integrate the Pre-Processed Guide Laser (Red Pointer) signal.

## 5. Cautions

- MaxPowerWatt settings: This value must match the maximum output of the real laser source, and the accurate W unit control is possible.
- Hardware Port Confirmation: When using Analog or DigitalBits, make sure the actual port number (`AnalogPortNo`, `DigitalBitsPortNo`) is correct.
- Serial communication restrictions: The RS-232 method is a slow transmission rate compared to other methods, so very short lineary processing can result in delays.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
