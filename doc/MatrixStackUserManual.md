# Coordinate Transformation & Matrix Stack User Manual

> Reference version: Sirius3 1.12.3 (public Release features)


## 1. Overview

Sirius3 uses a hierarchical coordinate-transformation system to manage complex marking paths efficiently and predictably.
It is essential to separate and manage the flexible drawing placement at user (UI/Recipe) level and the physical adjustment at hardware (RTC/Scanner) level.

## 2. MatrixStack (Software-level Transform)

MatrixStack is a 4x4 linear stack in a Push/Pop mode similar to OpenGL.

[Specificities and advantages]
- Layer Processing: In complex structures leading to parts > sub parts > individual designs, the coordinates of each step can be managed independently.
- 3D Space Conversion: RTC hardware perfectly calculates the hard to handle 3D rotating (Roll, Pitch, Yaw) and rotating calculations in PC software unit.
- Value of IRtc.MatrixStack: Without modifying the processing drawing (Entity) itself, you can immediately change the location and corner of the entire drawing by just pushing the row on the stack immediately before processing (e.g., useful in multi-range tray processing).

## 3. Integration with the Offset Structure

Offset is a tool that helps users who are not familiar with linear math also intuitively manipulate the coordinates.

- Main properties: Translate (translation), AngleZ (rotation), Scale (scaling).
- ToMatrix calculation: the numbers entered by the user are automatically converted into a 4x4 sequence in the `Scale -> RotateZ -> Translate` order.
- Use: Loan the measurement values of the vision system (X, Y, Theta failure) to the Offset, and by making it `MatrixStack.Push(offset.ToMatrix)` it makes it easy to implement Part Displacement in real time.

## 4. RTC Internal Matrix (Hardware Internal Matrix)

The IRtc interface provides the properties `MatrixPrimaryInternal` (1 head) and `MatrixSecondaryInternal` (2 head).
This is a sequence processed at the internal hardware level of the RTC controller.

[MatrixStack vs. Internal Matrix Difference]
- MatrixStack (UI/PC): for the logical placement of the processing recipes. perform 4x4 conversion using the CPU/GPU resources of the PC.
- Internal Matrix (hardware): Adjusts physical equipment conditions. During marking, the RTC DSP applies the 2×2 transform and offset in real time at 10 µs intervals.

## 5. Best Practices

To maximize processing accuracy and system performance, divide the roles as follows:

[MatrixStack is recommended]
- The layout of my objects, the placement of grouped parts.
- Real-time orbit conversion by vision correction.

[Internal Matrix is recommended - effective!]
- Physical rotation adjustment of the scanner head: If the scanner head is installed with 90 or 180 degrees rotation on the device, you can set only the internal line without having to correct the software drawing.
- Flip customization: If the converter’s move direction and the scanner axis are opposite during the MoF (Marking on the Fly) processing, it is the fastest and safest to reverse the axis in the hardware sequence.
- Multi-head alignment: When aligning two scan heads to the same coordinate system, configure each head's Internal matrix independently to compensate for physical offsets.

The conclusion
Use MatrixStack for the logical layout of geometry. Use the RTC Internal Matrix for physical equipment alignment and hardware-specific rotation or axis reversal; this best matches the intended separation of responsibilities and reduces software processing.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
