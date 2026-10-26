# View and camera guide

> API reference: Sirius3 Core/UI 1.17.0 (public features)

[English](ViewUserManual.md) | [한국어](ViewUserManual.koKR.md) | [All guides](README.md)

## 1. Display a document

Use an EditorControl for drawing and editing, or a ViewerControl for view-only
inspection. Both WinForms and WPF display the same document geometry. OpenGL 3.3
is required. Choose a supported display driver and test the application's actual
GPU and remote-desktop environment.

## 2. Cameras and navigation

The view provides an orthographic camera and Top, Front, Right, Back and Left
perspective cameras. Ctrl+E/Ctrl+Q cycles cameras. Wheel zoom is centered on the
pointer, middle drag pans, and right drag rotates a perspective camera.
F/Ctrl+F fits selected entities or active-layer contents. Plain arrows move the camera;
Alt+arrows adjust depth or turning. See [Editor shortcuts](EditorUserManual.md).

## 3. Visibility and appearance

The grid, axes and field-of-view aids support placement. Rendering permission
controls visibility; marking permission separately controls machining. A hidden
entity is not an editing target. `Alpha` affects appearance, not laser energy.
Imported mesh colors and diffuse textures are preview data; create slices or
other supported paths before machining a surface model.

Keep referenced OBJ/MTL/images together at import. Saved documents retain imported
texture data. If an image is missing or a model looks wrong, inspect the import
log and physical dimensions. See [entities](EntityUserManual.md).

## 4. Selection and simulation

Clicking chooses the nearest eligible visible geometry; box selection follows
the displayed geometry and enabled hatch paths. Grips use screen-sized markers
and remain usable when zoom changes. The upper-left help describes the current
mode and active parameter. Rotation arrow markers remain visible and selectable
through geometry while their circular guide paths may be occluded. Other mesh
and surface grips follow geometry visibility. [Simulation](SimulationUserManual.md) displays planned
travel and marking paths without changing machining geometry.

## 5. Multiple views and application lifetime

Several views may share a document while using different cameras. The application
must keep that document alive until every view has detached. Close/dispose each
control on its owning UI thread, await registered-device cleanup in integrated
editors, and dispose a host-owned document once after its views close.

For host DPI setup and shutdown examples, see [WinForms](WinForms.md), [WPF](WPF.md)
and [Viewer](ViewerUserManual.md).


---
2026 Copyright (c) SpiralLAB. All rights reserved.
