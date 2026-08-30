# SCANLAB RTC6 Controller User Manual

> Reference version: Sirius3 1.12.3 (public Release features)

## 1. RTC6 Role

RTC6 converts geometry and process parameters prepared in Windows into deterministic scanner, laser, and I/O commands that run independently of normal Windows scheduling. Sirius3 uses RTC6/RTC6e as its primary controller generation.

Successful list submission means that RTC accepted the command and started execution. Confirm actual completion with `Ready`, `Busy`, and `Error` state, job status, and device logs.

## 2. Coordinates and KFactor

- RTC6 represents the scan field in high-resolution controller-bit coordinates.
- The unit of `KFactor` is **bits/mm**.
- The conversion is **Controller position (bits) = user position (mm) × KFactor (bits/mm)**.
- For example, if the KFactor is 10,000 bits/mm, 12.5 mm will be converted to 125,000 bits before the field correction.
- `2<sup>20</sup> ÷ FOV` is only a scale illustration. The actual value must match the RTC generation, coordinate range, correction file, optical field, and axis direction.

KFactor sets the overall scan-field scale. Barrel, pincushion, trapezoidal, and local optical distortions are corrected separately with 2D or 3D correction data.

## 3. Jump, Mark and Microstep

- `Jump`: Move to the next start point with the laser gate inactive.
- `Mark`: Move along a straight segment with `LASERON` active.
- `Arc`: Mark an arc defined by the current position, center, and angle.

RTC6 divides Jump, Mark, and Arc motion into **position setpoints at 10 µs intervals (Microsteps)** and sends them to the scan head. At a commanded speed `v`, the distance between setpoints is `Δs = v × 10 µs`. Microstep is the RTC motion time unit; it is distinct from the Sirius3 `MicroVector` raster mode and from laser pulse width.

## 4. List Buffer

The RTC6 Single List capacity used by Sirius3 is **2<sup>23</sup> commands**. Geometry, pen, delay, laser, I/O, measurement, and termination commands share this capacity, so retain adequate headroom.

- `Single`: Create and run one limited RTC list.
- `Auto`: Sirius3 splits a long logical job across native lists, fills the buffers, and executes them in sequence.
- `Ctl*`: An immediate command that changes RTC control state.
- `List*`: A buffered command recorded between `ListBegin` and `ListEnd` and executed in order by RTC.

## 5. Laser and Scanner Delays

Scanner mirrors and laser sources do not respond instantaneously to commands. Laser delays align actual emission start and end with scanner position; scanner delays give the mirrors time to reach the commanded position and velocity.

1. Prepare a straight-line and corner test pattern at low output in a controlled area.
2. Tune Laser On/Off Delay so line starts and ends are neither clipped nor overexposed.
3. Tune Jump Delay to the lowest value that avoids an unstable start after a Jump.
4. Tune Mark Delay and Polygon Delay to reduce excess energy at line ends and corners between consecutive Mark segments.
5. Change one parameter at a time and inspect repeated marks under magnification or with measurement equipment.

RTC6's laser delay resolution is 1/64 μs and the scanner's move setup is 10 μs interval. Do not confuse the input unit with the actual application resolution.

## 6. Variable Polygon Delay

Variable Polygon Delay applies a part of Polygon Delay according to the angle to the corner in one Polyline **continuous Mark linear**, which does not mean how much the object has turned on the screen.

In the default behavior, it adds a delay to the corner, so the output time increases. `EdgeLevel` is a separate protection setting that allows you to delete the laser from the corner near the reverse and complete the current Polyline. check the column accumulation and corner shape with the test pattern to set the values.

## 7. Sky Writing and SCANAhead

Sky Writing adds the acceleration and acceleration range to the front-back of the processing line to pass the actual Mark range at the target speed. in Sirius3, it uses the Sky Writing settings in `EntityLayerPen`.

- Mode 1/2: Run-in/Run-out and Sky Jump before the vector.
- Mode 3: Select whether to apply Sky Writing depending on the corner angle.
- Mode 4: Mode 3 is the RTC6 mode that allows short List commands that are supported on the basis of Mode 3.

SCANAhead analyses the future trail in the compatible RTC6 and excelliSCAN configuration and automatically calculates delays. `Preview Time` is a time window to look at the future routes, and the bigger it is not always a good value. In Develop Mode it is converted to 10 μs Tick, and in Load Mode it uses the parameters stored in the controller, so it does not apply Preview Time·Vmax·Amax entries. Use the validated values in accordance with the installed RTC6 DLL, firmware, scan heads and correction files.

## 8. Wobbel

Wobbel synthesizes cyclic movements, such as one-tunn·8 characters, in the basic Mark path to change processing width and energy distribution. in `EntityPen` it sets `WobbelShape`, `WobbelFrequency`, `WobbelParallel`, `WobbelPerpendicular`.

- If the parallel and perpendicular expansion are equal, it becomes the Ellipse close to the circular.
- If the two spots are different, it becomes a puzzle.
- If the parallel extension is 0 it becomes a vertical Sine line route in the direction of progress.
- The combination with Sky Writing, Pixel Output, and Raster may be limited, so check the actual RTC6 function flags and logs.

## 9. LASER Port Signals

The accurate pin behavior of `LASER1`, `LASER2`, `LASERON` will vary depending on the Laser Mode, polarity, pulse width/frequency settings and linearity.

- `LASERON`: Primarily used as the laser-enable gate during Mark segments.
- `LASER1`, `LASER2`: Laser Mode is used as a pulse, frequency and mode signal.
- LASER-port `DIGITAL IN1`: Counts external laser synchronization pulses for features such as `PixelPulses`. A physical connection between the **laser SYNC OUT and DIGITAL IN1** is required.

Do not connect the laser until you have verified voltage levels, polarity, reference ground, and the input edge.

## 10. Field Correction and Status Checks

Due to the nonlinearity of the F-theta lens, mirror corner, lightweight predisposition, scanhead installation failure, the command coordinates and the actual processing location may not match. First match the full scanning with the right KFactor and then reduce the position-by-location failure with the 2D/3D correction file.

After RTC6 start, check the correction file and table selection, Laser Mode, Scanner Ready, Error Code, Analog and Digital I/O status. Sirius3 1.12.3 read the RTC6 status and Analog I/O as a generated controller API and reliably deals with Ethernet connection errors and end-of-state timers.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
