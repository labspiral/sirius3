# SpiralLab.Sirius3 MoF (Marking on the Fly) Integrated User Guide

> Reference version: Sirius3 1.12.3 (public Release features)


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
