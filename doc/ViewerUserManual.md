# ViewerControl User Manual

> Reference version: Sirius3 1.12.3 (public Release features)


## 1. Overview

`ViewerControl` is a lightweight OpenGL visualization control for monitoring laser-processing state or inspecting drawings in view-only mode.
The editing function is excluded, so there is no risk of drawing mutation due to errors, and it uses system resources efficiently.

## 2. Document Injection

Like EditorControl, ViewerControl also infuses data from the outside through the `Document` property.

- Real-time collaboration: When you connect the `IDocument` that is being edited in the Editor to the Viewer, the Viewer screen will also be synchronized and updated in real-time when an object is modified or added in the Editor.
- Multi-view configuration: Exhibit multiple viewers to share one document, each of which can intrinsically monitor the processing status in different camera corners (flat, side, ISO, etc.).

## 3. Key Features

- Read-only mode: The `IsAllowEdit` property is internally fixed to `false`, and the drawing is not changed by mouse drag or click.
- Advanced Camera Animation:
  - Auto Rotating: Automatically rotating the camera to a specific object center, allowing you to review the processing in multiple dimensions.
  - Zoom Fit: One-click button to draw the whole or selected objects on the screen tightly.
- Status display: You can receive the start/final event of the processing markers (`Marker`) to automatically update the screen or display the processing time.

## 4. Usage Scenarios

- Installation Operator Screen: The main editing window is hidden, exhibit only ViewerControl, so that the processing processing is safely checked.
- 3D monitoring: 3D processing (e.g. Z-Defocus processing) is used as an assistant monitor to verify the intrusive orbit.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
