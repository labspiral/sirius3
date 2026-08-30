# View System & OpenGL Rendering User Manual

> Reference version: Sirius3 1.12.3 (public Release features)


## 1. Overview

The Sirius3 View system uses the OpenGL 3.3 core profile to visualize tens of thousands of vector primitives in real time at 60 fps or higher on suitable hardware.
The system consists of Surface responsible for screen output, View managing visual logic, and Resource managing GPU resources.

## 2. Core Components


[WinFormsGLSurface: Hardware Connection Layer]
- Role: WinForms is a bridge that connects the `GLControl` and OpenGL context.
- Main Functions:
  - Convert mouse and keyboard events into OpenGL coordinates.
  - `MakeCurrent()`: Stream-based OpenGL context activation control.
  - `Invalidate()`: screen update request and `SwapBuffers` perform.
- Important: `MakeCurrent()` must necessarily be called only on the UI thread. Access to GPU resources directly from the background thread is prohibited.

[ViewBase: Visual Logic Manager]
- Role: Combines the camera, lights, Document drawing, and marker state into one scene.
- Main Functions:
  - Camera Control: 2D (Professional) and 3D (Professional) Camera Conversion and Zoom Fit Function.
  - Interaction: Drag entities with the mouse and select entities with a frustum.
  - Visualization Assistant: FOV area display, checkboard, coordinate axis rendering.
  - Simulation: Show the real-time animation guide of the laser processing pathway.

[GLResource: GPU Resource Manager]
- Role: Manage the Vertex data of the individual entity (Entity) as a GPU memory (VRAM).
- Synchronization mechanisms (`SyncWith`):
  - Only when the entity's `GeometryVersion` has been changed, the GPU buffer (VAO, VBO, EBO) is updated to prevent unnecessary data transfer.
  - Buffer the position, color, normal, and texture (UV) data independently.

[Shaders: Rendering Pipeline]
- Role: Manage the high-speed calculation program (GLSL) running in the GPU.
- Provided by Shader:
  - General: Standard shader for ordinary vectors and mesh entities, including lighting and clipping effects.
  - Font: High-resolution SDF (Signed Distance Field) based text rendering.
  - Plane: for work flat and grid rendering.

## 3. Rendering Workflow

The process in which the processing data is displayed on the screen is as follows.

1) Data Change: The user changes the properties of the object.
2) Reproduction: `GeometryVersion` increases when calling `document.ActRegen()`.
3) Synchronization: At the next frame rendering, `GLResource.SyncWith()` detects the version difference and upload new data to the GPU.
4) Rendering: `GLResource.Render()` is called to send the model/view/projection and lighting parameters to the shader and run `GL.DrawElements`.

## 4. View Interactions

- Mouse wheels: extended / reduced (based on the locations of the crossing).
- Mouse Uclick Drag: Camera Rotation (3D Mode Only).
- Mouse wheel click drag: screen parallel move (Panning).
- Space Bar: Activate the sub entity (Group internal objects) choice mode.
- Ctrl + F: Selected object or full drawing screen customized (Zoom Fit).

## 5. Developer Notes

- Resource cleanup: `GLResource` owns unmanaged GPU resources (VRAM). Call `Dispose()` when an entity is deleted or the Document is closed to prevent resource leaks.
- Rendering performance: When rendering the same mass of objects, use `EntityUniformGroup` to minimize the number of draw calls.
- Transparency Control: The `EntityModelBase.Alpha` property allows you to control the transparency of the object.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
