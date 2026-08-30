# IRtcMultiBeam Developer Manual

> Reference version: Sirius3 1.12.3 (public Release features)

## 1. System Architecture

Multi-Beam allows two RTC controllers and two scan heads to share one laser source. Two AOMs select the optical path, while cross-connected DInput/DOutput Token signals ensure that only one head uses the laser at a time.

It differs from the ability to switch independent device sets from the screen, like `SiriusMultiEditorControl`. Multi-Beam synchronizes real light source use at the List command level.

## 2. IRtcMultiBeam

Use the factory that fits the installed controller.

- `ScannerFactory.CreateRtc5MultiBeam`
- `ScannerFactory.CreateRtc6MultiBeam`
- `ScannerFactory.CreateRtc6EthernetMultiBeam`

The application provides implementation and use `RtcMultiBeamHelper` rather than directly implementing `IRtcMultiBeam` because Native RTC command, AOM order, Token expectation and error recovery must work together.

## 3. Main Properties

| Property | Purpose |
|---|---|
| `MultiBeamIndex` | 0 Based Index to Set Pair and Head Roles |
| `TokenBitMask` | Exit your own state of use and check the relative state from entering Bit |
| `AOMBitMask` | AOM Digital Enable Bit |
| `AOMChannel` | RTC Extension Analog Channel |
| `AOM0OrderVoltage` | Strengths in the light. |
| `AOM1stOrderVoltage` | Tension in the light. |
| `AOMHoldMsec` | AOM change after stabilization. |
| `ListAOM(onOff)` | In the List Buffer, AOM Bit·Voltage·Atmosphere is recorded in order |

Two instances of the same pair must have different `MultiBeamIndex` and the Token/AOM Bit must not be overlaped.

## 4. Initialization with config_multibeam.ini

Public `multibeamhelper.cs` read `config_multibeam.ini` and prepare for the following order.

1. Read the card/iternet settings of each RTC and the bits/mm unit KFactor.
2. Start the RTC and Correction Table.
3. Create Extension/LASER-port I/O.
4. Laser and Marker.
5. Set `MultiBeamIndex`, Token/AOM Mask, Channel, Voltage, Hold Time.
6. Register two instances as a pair in Helper.
7. Check the line with `CheckPins(pairIndex)`.
8. Set Mode and Preferred Side and call `ReadyMode(pairIndex)`.

You need a license that fits both the RTC instances and the Multi-Beam options. Do not judge that the actual option is activated only by the fact that the factory has returned the instances.

## 5. ReadyMode

`ReadyMode` starts AOM Voltage/Bit and Token output according to the chosen Mode.

- Head1: Head1 starts to open only the light
- Head2: Head2 is only opened.
- Both: Prepare both heads for Token handoff and give the Preferred Side priority.
- None/Reset: Return to the safe disabled state of two spots and tokens

Before the next processing, check the actual AOM/Token status with `IsInstanceReady` and, if necessary, Ready again after `ResetMode`.

## 6. List Sequence in Both Mode

1. Start the list and close the AOM.
2. Unlock the token now.
3. Jump to the next place.
4. Wait until the opposite token input is LOW.
5. Set your token output to HIGH.
6. Open the light with `ListAOM(true)` and restore EntityPen output.
7. Execute marking.
8. Close the list, or disable AOM and Token before the next handoff.

When the first Jump starts at the same time, the non-linear Head makes a decisive priority for the short Guard Wait. From Sirius3 1.11.14 to the repeated JumpAndShoot, the Token discharge expectancy is overwhelmed with the real Jump, and the cost is less short Jump is connected to reduce the unnecessary expectancy of Both Mode.

## 7. Error Handling

- `CheckPins` Failure: Do not start processing, DO→DI check the two-way line and Bit Mask
- Token Wait Continues: Busy/Error of the relative RTC, Abort status and Token DO check
- AOM output disagreement: Check Channel, 0/1 Voltage, Enable Bit, Hold Time
- One Head Only Release: Mode, Preferred Side, Laser/AOM Optical Setting and Marker Ready Check

The error recovery is `ResetMode` to lower the AOM and Token and then go to the order to re-confirm the two RTC status.

## 8. Public Demo

- `editor_multibeam2`: Two Editor/Marker and Mode Conversion UI Examples
- `editor_multiple2`: Multi-independent set of devices compared to MultiEditorControl
- Common `multibeamhelper.cs`: INI Reading, Factory Generation, Pair Registration, Wire Inspection and Ready Flow

The demo's line bits, voltages, and KFactor values are examples. Replace them with validated values for the installed circuitry and optical system.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
