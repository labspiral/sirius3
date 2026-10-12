# SiriusEditorControl User Manual

> Reference version: Sirius3 1.16.0 (public Release features)

[English](SiriusEditorUserManual.md) | [한국어](SiriusEditorUserManual.koKR.md)

## 1. Integrated Editor

`SiriusEditorControl` is available for WinForms and WPF. It combines EditorControl, the Page/Layer TreeView, PropertyGrid, and Scanner, Laser, Marker, I/O, and PowerMeter UI on one screen. Use `beginner` and `editor_ui` for WinForms or `wpf_editor_ui` for WPF customization.

## 2. Page Tab

Page 1 to 4 separates different drawings or processes.When you change the active Page, the default objects of Editor, TreeView and Marker are changed together.F5 can actually run the current Page.

## 3. Block Tab

Block manages the master shape to be reused and BlockInsert is a reference placed on the Page. It is suitable for repeated logos or arrangements, but make sure that the ModelMatrix of the Block, the ModelMatrix of the Insert and the Marker Offset are not duplicated.

## 4. Entity Pen Tab

Object colour-by-colour Power, PowerMax, PowerMapCategory, Frequency, PulseWidth, Mark/Jump Speed, Delay, Raster, Hard Jump and Wobbel.

## 5. Layer Pen Tab

Layer edits the Sky Writing, ALC, Variable Polygon/Jump Delay, SCANAhead connection and syncAXIS conditions to be applied before starting.

## 6. PropertyGrid

When you select an object in Editor or TreeView, the image, Text, Hatch, Transform and Pen properties will be displayed. you can search for the properties name, class, description, and you can focus on the search window with `CTRL+F`. in multiple choices only common properties will be displayed.

## 7. Scanner Tab

Check the RTC status, KFactor, correction files and Table, Laser Mode, Delay, I/O, Measurement and support extensions. in RTC6, SCANAhead, Auto Delay and Preview Time are used in accordance with the installed scan heads configuration.

## 8. Laser Tab

Check Laser Ready, maximum output, Power Control method and manufacturer-specific settings. Registering the Laser renews the connection to PowerMax and PowerMap in EntityPen. Manual output command can lead to real output.

## 9. Marker Tab

Goal Page/Layer/Offset, Repeat Order, Preview, Start, Stop, Reset and Progress status. `LayerFirst` and `OffsetFirst` set any axis of Layer and Offset to external repetition. Check Ready/Busy/Error and current target number before starting.

## 10. Device Registration

`RegisterDevices` connects Scanner, Laser, PowerMeter, Extension/LASER-port DInput·DOutput, Marker and Remote. The demo read `config*.ini` to create and register the device as Factory and then check Marker Ready.

Before releasing the editor or document, await `editor.DisposeDevices()` on the owning UI thread. In 1.15.0 this returns `Task` and awaits the registered MCP server and remote before releasing devices. A separately owned server requires `await server.DisposeAsync()` first. Dispose owned controls/documents once, then call `Core.Cleanup()` from the application lifetime owner.

## 11. Framework selection and HiDPI

Package controls use `SpiralLab.Sirius3.UI.WinForms` or `SpiralLab.Sirius3.UI.WPF`; public source copies use `Demos`. WPF controls initialize their required theme resources. WPF Multi-Beam navigation is shown only for an assigned `IRtcMultiBeam`, and virtual/vendor laser panels follow the registered laser or custom WPF UI factory.

WinForms hosts select `PerMonitorV2`, and the demo forms use `AutoScaleMode.Dpi` with `AutoScaleDimensions = (96, 96)`. WPF hosts use a DPI-aware executable manifest and device-independent XAML layout. Follow the [WinForms](WinForms.md) or [WPF](WPF.md) guide for framework-specific startup, registration, and closing examples.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
