# ViewerControl User Manual

> API reference: Sirius3 Core/UI 1.17.0 (public features)

[English](ViewerUserManual.md) | [한국어](ViewerUserManual.koKR.md) | [All guides](README.md)

## 1. View a document

Use ViewerControl to inspect drawings or monitor a processing path without
editing geometry in the viewport. Assign the host's `IDocument` to `Document`.
An editor and multiple viewers can share it; edits made elsewhere are reflected
in the viewers. A viewer is not an interlock or a lock on external document changes.

## 2. Cameras and appearance

Wheel zooms around the pointer, middle drag pans and right drag rotates a
perspective camera. Use Fit to inspect the intended region. Choose different
cameras for top/side inspection in separate windows. Imported mesh colors and
textures affect visualization, while machining follows prepared paths and pens.

## 3. Integration and cleanup

Both WinForms and WPF provide viewer controls. Configure DPI and the application
lifetime as described in [WinForms](WinForms.md) and [WPF](WPF.md). Keep a shared
document alive until every control has detached, then dispose the host-owned
document once. Dispose each viewer when its window closes.

For editing, use [EditorControl](EditorUserManual.md). See
[view/camera](ViewUserManual.md), [simulation](SimulationUserManual.md) and
[Font Editor](FontEditorUserManual.md) for the related workflows.


---
2026 Copyright (c) SpiralLAB. All rights reserved.
