# SpiralLab.Sirius3 MoF (Marking on the Fly) Integrated User Guide

> Reference version: Sirius3 1.16.0 (public Release features)

[English](MarkingOnTheFlyUserManual.md) | [한국어](MarkingOnTheFlyUserManual.koKR.md)


This manual describes the two main Sirius3 interfaces, `IRtcMoF` and `IRtcMoFExtension`, which synchronize scanner motion with an external encoder or absolute-position signal (McBSP) to mark moving workpieces.


## 1. IRtcMoF (Standard MoF Interface)

As the most common industrial mobile processing interface, RTC4, RTC5, RTC6 is supported by all hardware.

### Main Functions
- 1D Linear: Correction of the direct line position in the direction of the conveyor belt.
- Rotary: Rotary line (Matrix) correction according to the rotation of a rotating table or circular parts.
- 2D Stage: Tracking two encoders of the XY Stage at the same time, supporting face-to-face processing.
- 2D correction table: Compensation of the mechanical fault of the stage with bilinear interpolation in real time.

### Main Methods
- ListMoFBegin: Linear MoF processing start.
- ListMoFRotaryBegin: Turn MoF processing start.
- CtlMoFCompensateTable: Stage error correction 2D table load.
- CtlMoFEncoderReset: Hardware Encoder Counter Initialization and Basic Point Synchronization.


## 2. IRtcMoFExtension (Advanced Fly Extension Interface)

With RTC6 features, it is optimized for complex and accurate tracking conditions, such as robotic arm or 3D free curve processing.

### Main Functions
- Multiple simultaneous tracking: up to four axes (X, Y, Z, Rotary) can be traced independently or at the same time.
- McBSP absolute position tracking: receiving the absolute coordinates directly every 10μs from the external controller (PLC/PC) not the encoder pulse method.
- Advanced parking: Clean the scanner into a safe area during processing atmosphere and prevent the virtual field distraction.
- Multi-transmission mode: Laser power or external sensor data (temperature, distance, etc.) is monitored and reflected in real time.

### Main Methods
- CtlMoFExtInitialize: hardware and McBSP communication initialization.
- ListMoFExtPark/Return: Scanner parks in the middle of the trail and returns without errors.
- CtlMoFExtMcBSPSetMultiIn: external power control and sensor monitoring mode settings.
- ListMoFExtWait1DAxis / 2DAxes: Wait to run the list until you reach a specific encoder location.


## 3. Interface Comparison: IRtcMoF vs IRtcMoFExtension


| Comparison of items. 	| IRtcMoF (Standard) 		| IRtcMoFExtension (Advanced) 		|
|		|				|					|	
| support hardware 	| RTC4, RTC5, RTC6 		| The RTC6				|
| Maximum tracking. 	| Maximum 2 axes (XY or Rotary)	| Up to 3 axes (XYZ or Rotary)		|
| Introduction to SOFT 	| RS-422 Encoder Pulse 		| Encoder pulse or McBSP absolute location		|
| The main strength. 	| It is convenient and universal. 		| Robot interaction and 3D tracking possible 		|
| Special Function 	| 2D Stage Precision Vehicle Correction 	| Advanced parking, real-time sensor monitoring 		|
| The field size.	| RTC4 (16 bits, no virtual fields), RTC5 (24 bits, 16 times), RTC6 (29 bits, 512 times)	|


### Advantages and Limitations
- IRtcMoF
  - Advantages: Good compatibility with older controllers such as RTC4/5, with straightforward configuration for ordinary conveyors or XY stages.
  - Disadvantages: It is difficult to track simultaneously with three axes (including axes), and there are boundaries in absolute coordinate-based controls, such as a robot arm.
- IRtcMoFExtension (only for RTC6)
  - Advantages: It is possible to perfectly synchronize with the robot arm or 3D stage, and the laser power during processing can be converted into real time to fit with external data.
  - Disadvantages: RTC6 hardware is essential, and the initial configuration difficulties, such as the McBSP communication standard settings, are high.



## 4. Use Cases


### Case A: Standard Conveyor Marking (Using IRtcMoF)
- Situation: Distributed marking on the drinking water cann that flows at a fixed speed.
- How: Connect 1 Conveyor Encoder to RTC and call `ListMoFBegin`.
- Result: Even if the belt speed changes mildly, the letters are compressed or not increased and recorded in the exact position.

### Case B: Large-area PCB Tiled Marking (IRtcMoF)
- Situation: 500mm PCB processing greater than the scanner area (100x100mm).
- Method: Up PCB to the XY stage and after applying the 2D correction table with `ListMoFBegin` two-shaped simultaneous tracking processing.
- Result: Stage movement defects removed accurate face-face processing realization.

### Case C: Robot-arm 3D Body Welding (IRtcMoFExtension)
- Situation: Robot arm takes the scanner and moves along the rolled car car car surface and salts.
- Method: Robot controller transmits XYZ coordinates in real time to RTC6 through McBSP. `ListMoFExtBegin` performs 3D Fly processing.
- Result: The robot trajectory and scanner marking trajectory are synchronized at 10 µs intervals, maintaining consistent quality over complex curvature.

### Case D: Cylindrical Tumbler Image Marking (IRtcMoFExtension)
- Situation: Turn the tumbler and mark the high-resolution bitmap image on the side.
- Method: Replace the corner of the rotating engine with the original rate (π) to linear distance. With `ListMoFExtWait1DAxis`, wait the next line position accurately and process one line by one.
- The result: it is possible to process the image without distortion on the rotating surface without the accumulation of rotating defects.

## 5. Public editor_mof_* demos

All five demos ship editable source in namespace Demos and consume Sirius3 NuGet packages. Their host [helper.cs](../demos/helper.cs) reads configuration, creates/initializes devices and binds them through RegisterDevices and Marker.Ready. Their Form1 entry points illustrate different controller sequences:

| Demo | Key sequence / customization |
|---|---|
| [editor_mof_xy](../demos/editor_mof_xy/Form1.cs) | CreateMoFBegin(XY, true), encoder-coordinate waits, vector entities and CreateMoFEnd. Reset establishes an encoder reference for the job. |
| [editor_mof_xy_raster](../demos/editor_mof_xy_raster/Form1.cs) | Raster image processing with OnBeforeRasterLine. Each row waits for the appropriate encoder position using the transformed start and raster pitch. |
| [editor_mof_trigger](../demos/editor_mof_trigger/Form1.cs) | DI0 rising-edge wait → MoF begin/reset → barcode/text → MoF end → user event. Event text conversion supplies serial numbers; the sample repeats the layer 100 times. |
| [editor_mof_offsets](../demos/editor_mof_offsets/Form1.cs) | Combines marker offsets and position waits based on transformed entity bounds. Begin(false) keeps the ongoing encoder reference rather than resetting it at every offset. |
| [editor_mof_interrupt](../demos/editor_mof_interrupt/Form1.cs) | Breakpoint pauses the list; an IRtcInterrupt handler performs host work and returns true to resume. The sample's one-second sleep illustrates work latency. |

### XY and raster position waits

Set encoder counts per millimetre and movement direction before recording commands. A wait coordinate is not a motor-move command: an encoder or the demo's configured encoder simulation must actually advance. In the negative-X examples, the Under/Over condition must agree with that direction; changing only the displayed coordinate can leave a wait unsatisfied.

The raster demo combines document/pen RasterMode with a row callback. Use the actual transformed row start and pitch, including entity and marker placement, when computing the wait. Avoid doing slow UI/file work in the row callback. View rendering alone does not demonstrate synchronized raster machining.

### Trigger, serial values and offsets

The trigger demo queues the DI0 edge wait on the controller, not in DInput.OnChanged. It configures both barcode and text with TextConverters.Event and updates serial state through marker conversion/user events. A host event is not necessarily proof that the final physical pulse has finished; coordinate accepted data, controller completion and an external ACK protocol. Review event and layer-repeat boundaries when changing marker strategy.

The offsets demo retains the encoder origin while processing multiple placements. Combine offset placement and transformed bounds once, and keep encoder and document coordinate conventions consistent. ExtensionData can supply different text/barcode values per location; see [Offset](OffsetUserManual.md).

### Interrupt and controller prerequisites

The interrupt handler is a host callback with Windows scheduling latency. Replace illustrative sleeps with explicit completion/failure handling for the external operation. It is suited to paused coordination, not capture of a short input pulse. Use controller edge waits for timing-sensitive input sequencing. The current RTC4/4e dedicated edge helper does not record an edge wait; use verified level-wait composition or RTC5/6 for that demo flow. See [digital I/O](DigitalIOUserManual.md).

Check IRtcMoF/IRtcMoFExtension/IRtcInterrupt support, controller generation, installed SDK/firmware, enabled MoF option/license, encoder polarity, counts per millimetre and virtual-field limits. Debug assertions in a sample are not production validation. Handle OnEncoderSignalError and OnOutOfVirtualImageField, aborts and errors, and stop/dispose host work during cleanup.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
