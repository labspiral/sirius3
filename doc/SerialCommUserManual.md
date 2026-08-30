# RTC RS-232 Serial Communication User Manual

> Reference version: Sirius3 1.12.3 (public Release features)


## 1. Overview

This manual explains how Sirius3 `SerialCommControl` communicates with external devices such as laser sources, sensors, and automation equipment through the RS-232C port built into SCANLAB RTC5/6 controllers.

## 2. RTC: Real-time Advantage (Real-time Advantage)

Unlike the usual PC serial ports, the serial communication of the RTC card can send the direct transmission command (`ListSerialWrite`) within the 'List' command.

- Super accuracy synchronization: Without the Jitter or Latency of the Windows operating system, you can send commands to the external device in accordance with the exact moment when the scanner moves or the laser is activated (10μs cycle unit).
- Independent execution: Once the processing list is downloaded to the RTC board, the data will be transmitted at the time specified by the DSP of the board, regardless of the condition or load of the PC.
- Use Examples: It is used when changing the laser source parameters in accordance with the exact runtime of the RTC List, or when sending a completed signal to the automation facility immediately after a particular area processing.
- Note: It is useful in special cases where you need to send specific commands to the laser source in real time from RTC, not the usual serial communication method, in accordance with the exact list commands run time.

## 3. Differences from and Constraints of PC Serial Communication

RTC-board serial communication is implemented in hardware, so it offers a smaller and more strictly defined feature set than general-purpose PC serial communication.

- Fixed format (Fixed 8-N-1):
  - Settings: Only the communication rate (Baud rate) can be changed.
  - Fixed framing: 8 data bits, 1 start bit, 1 stop bit, and no parity. The connected device must support this format.
- Acceptance of the buffer:
  - The receiving buffer on the RTC board offers only a 256 characters ring buffer.
  - If you don’t read the data immediately, you’re very likely to lose the data due to the buffer overrun.
- Method of polling:
  - It does not support hardware interrupts or event callbacks.
  - In Sirius3 UI, the data is read and notified as an event through periodic polling, but when implemented directly from the user logic, it is necessary to periodically call `CtlSerialRead`.

## 4. Key Features

- ASCII/HEX mode: Text-based commands and binary (Hex) data can be selected and transmitted.
- Line Ending Settings: You can choose between CR(\r), LF(\n), CRLF(\r\n) or No to notify the end of the command.
- Data Monitor is:
  - Sent (transmission): You can check out the data history through the hardware.
  - Received (received): Showing data from external devices in real time.
- Busy state check: in the processing processing (Busy) state, manual transmission (`CtlSerialWrite`) may be limited.

## 5. Cautions

- Mass data transfer delays: If you send a long string with the control command (`CtlSerialWrite`), the running flow of the PC software may be temporarily blocked (Block) until the transfer is completed.
- Verify the communication standard: RTC5/6 uses RS-232C. An incompatible voltage level or pinout can prevent communication or damage hardware.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
