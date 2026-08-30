# SiriusEditorControl User Manual

> Reference version: Sirius3 1.12.3 (public Release features)

## 1. Integrated Editor

`SiriusEditorControl` is a public WinForms control that combines EditorControl, the Page/Layer TreeView, PropertyGrid, and Scanner, Laser, Marker, I/O, and PowerMeter UI on one screen. Use the public `beginner` and `editor_ui` demo sources as starting points for product-specific menus, tabs, and device UI.

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

Stop the Marker that is running at closure first, disable the Device created and then call `Core.Cleanup()`.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
