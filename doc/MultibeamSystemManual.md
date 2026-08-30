# SIRIUS3 Multi-Beam Laser Control System User Manual

> Reference version: Sirius3 1.12.3 (public Release features)

## 1. Overview

Multi-Beam is a device configuration in which two scan heads share one laser source. It uses two RTC controllers and two AOMs, with cross-connected Token DIO signals transferring laser-use permission between the heads.

## 2. Components

| Configuration | Purpose |
|---|---|
| RTC/Head 1 | First scan heads move and AOM/Token control |
| RTC/Head 2 | Second scanhead move and AOM/Token control |
| Laser Source | The two heads are shared. |
| AOM 1/2 | Choose the outlet. |
| Token DI/DO | Check whether the two RTCs use the opponent laser. |

Token and AOM are not substitutes for hardware safety devices. Configure optical shutters, interlocks, and equipment emergency stops separately.

## 3. Processing Modes

- Head1: Use only the first Head
- Head2: Use only the second Head
- Both: The two heads alternate using Token handoff within the Jump interval.
- Reset/None: Return AOM and Token to disabled

In Both Mode, the Preferred Side sets the head to receive the token first when the two heads start at the same time, which does not always mean that you finish the whole task first.

## 4. Pre-operation Checklist

1. Make sure both RTC, Laser and Marker are Ready.
2. With `CheckPins` check the two-way connection of the Token DO → relative DI.
3. Check AOM 0 / 1 Voltage, Digital Enable Bit and Hold Time.
4. Select Mode and Preferred Side.
5. After `ReadyMode` check the status indication of each Head.
6. Make sure that the output test only opens one light at one point.

## 5. The Both Mode

Each Head waits until the relative Token is dissolved before the processing interval, then taps its own Token and then opens the AOM light. When Jumping to the next position, it dissolves the AOM and Token so that the relative Head is available. When the first Jump begins at the same time, the short Guard Wait guarantees the preferred side priority.

Since Sirius3 1.11.14, part of the wait time during repeated JumpAndShoot Token handoff is overlapped with an actual Jump, reducing unnecessary idle time.

## 6. Status and error.

- Ready Discharge: Abort/Error Back to `ResetMode` and `ReadyMode`
- Token Waiting: Check the Busy/Error and Token Output of the Relative Head
- One Head Only Output: Mode, AOM Voltage/Bit, Optical Setting Check
- Simultaneous output suspicion: immediately stop work and check hardware interroks and wires

## 7. Related Public Demo

`editor_multibeam2` and the shared `multibeamhelper.cs` read `config_multibeam.ini` and show how to initialize both RTC controllers, AOM/Token control, and the Marker. Replace the demo bit masks, voltages, and KFactor values with validated values for the installed equipment.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
