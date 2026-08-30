# ULTRA SHORT PULSE LASER with PULSE PICKING AND SYNCHRONIZATION

> Reference version: Sirius3 1.12.3 (public Release features)

This guide explains how to use RTC6 **Pulse Picking** to control ultrashort-pulse (USP) laser sources such as femtosecond and picosecond lasers.

## 1. Pulse Picking Laser Mode

It is a mode that must be used to extract (picking) only the pulse needed for processing during the basic Seeder (hour) of the piston laser that spreads to dozens of MHz.
You can use the Divider inside the RTC6 board to select the pulse at the desired Repetition Rate.

### Operating Principle and signal output
*   **LASERON Signal:** is ON when Laser Active is active.
*   **LASER1 port:** Basic Clock and synchronized transformation signals come out from the laser source.
*   **LASER2 port:** Depending on the set divider, the pulse extracted per Ninth pulse of the LASER1 signal comes out.
*   **Standby:** While `LASERON` is OFF, LASER1 and LASER2 output standby pulses with the same phase. Pulse picking is not applied in this state.

---

## 2. Main Features and API Usage

### 2.1 Enable Pulse Picking (`CtlPulsePickingMode`)
Set to extract a specific pulse depending on the set distribution cost `N`.

```csharp
// N=2 when set: 1 pulse output with LASER1 pulse 2 per LASER2 (50% split)
// N=10 when set: 1 pulse output with LASER1 pulse 10 per LASER2 (10% square)
uint divider = 10; 
rtc.CtlPulsePickingMode(divider);
```

*   * The variable (`no`): *
    *   `0`: Pulse Picking Disconnected. through the LASER2 port, the LASERON signal comes out in the same way.
    *   `1 ~ 63`: Pulse Picking Distribution Cost Set. Every Ninth pulse of LASER1 is outputed by LASER2.
*   **Specifications:** When this feature is activated, the laser mode (0-6), set by the existing `CtlLaserMode`, will be ignored.

### 2.2 Configure a Fixed Pulse Width (`CtlPulsePickingConstantLength`)
It is the ability to fix the length of the picked pulse (LASER2 port) independently and consistently, regardless of the LASER1 setting.

```csharp
bool enable = true;
double pulseWidth = 0.5; // 0.5 usec (500ns)
rtc.CtlPulsePickingConstantLength(enable, pulseWidth);
```

*   **Actions:** Basically, the pulse width of LASER2 follows the pulse width of LASER1, but when this function is activated, you can only separate the real picking and outgoing pulse width.
*   **Using:** Uses when precise pulse width control is required in accordance with the USP laser trigger signal requirements.

### 2.3 LASER1 Output Pulse Synchronization (`CtlLASER1Synchronization`)
It is the ability to accurately synchronize the output time of the LASER1 signal generated inside the RTC board according to the external laser clock signal (DIGITAL IN1).

```csharp
bool enable = true;
double delayTime = 0.1; // 0.1 usec (100ns)
rtc.CtlLASER1Synchronization(enable, delayTime);
```

*   **Operation:** The RTC6 board does not immediately issue the LASER1 pulse, and with the **DIGITAL IN1** pin of the LASER connector, it expands and expands until the new external clock pulse is detected.
*   The setup: *
    *   `delayTime`: It is an additional delay time until the actual pulse output after external signal detection. The laser output cycle should be shorter.
    *   **Edge settings:** The elevation/down edge distinction of the external clock can be changed through the `Rtc6LaserControlSignal.Bit.ExtSignalPulseRisingEdge` settings.
*   * Attention to: *
    *   The `Rtc6LaserControlSignal.Bit.OutputSynchronization` (Scanner Movement Synchronization) function is not available at the same time.
    *   The SYNC OUT signal from the external laser source must be connected to the DIGITAL IN1 pin of the RTC6 LASER connector.


---

## 3. Signal Connection and Interface (Wiring)

The common interface connection between the USP laser source and the RTC6 board is as follows:

| The RTC6 port | Laser source input/output | Explanation |
| :--- | :--- | :--- |
| **LASER1** | **Sync / Clock** | Basic trigger signal synchronized with the inner clock of the laser source |
| **LASER2** | **Trigger / Picked** | Real Picked Processing Pulse Trigger Signal |
| **LASERON** | **Gate / Enable** | Gate signals allowing laser output at the time of processing |

---

## 4. Programming Example

Here is the full example code to start the RTC6 control object and set the USP mode.

```csharp
// RTC6 Scanner Object Generation (Basic Laser Mode Set)
var rtc = ScannerFactory.CreateRtc6(0, kFactor, LaserModes.Mode4, RtcSignalLevels.ActiveHigh, RtcSignalLevels.ActiveHigh, "correction.ct5");

// USP pulse picking set (N = 4 minutes)
rtc.CtlPulsePickingMode(4);

// 3. (optional) Fix the width of the picked pulse to 200ns
rtc.CtlPulsePickingConstantLength(true, 0.2);

// 4. processing parameters set (frequency, delay time, etc.)
rtc.CtlFrequency(100000, 0.1); // 100KHz, 0.1us pulse width
rtc.CtlDelay(100, 200, 100, 100, 100);

// 5 – Marking
rtc.ListBegin()
rtc.ListJumpTo ...
rtc.ListMarkTo...
rtc.ListEnd()
rtc.ListExecute()
```

---

## 5. Cautions and Tips

1.  **Laser Mode Recovery:** To disable the pulse picking mode and return to the usual mode, you need to call the standard laser mode settings API again, such as 'CtlLaserMode(LaserModes.Mode4)'.
2.  **Q-Switch Delay:** In the pulse picking mode, the Q-Switch delay settings are also valid via 'CtlQSwitchDelay'.
3.  **FPK (First Pulse Killer):** Please note that the FPK signal via 'CtlFirstPulseKiller' is not output when using the pulse picking mode.
4.  **Standby Pulse:** The pulse output and width through 'CtlStandBy' in the atmospheric state must be adjusted according to the safety regulations of the laser source.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
