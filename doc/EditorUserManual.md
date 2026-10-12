# EditorControl User Manual

> Reference version: Sirius3 1.16.0 (public Release features)

[English](EditorUserManual.md) | [한국어](EditorUserManual.koKR.md)

## 1. Purpose

`EditorControl` is available for WinForms and WPF and is a control that displays the entities in an `IDocument` through an OpenGL editor and supports selection, translation, rotation, scaling, creation, and deletion. When the assigned Document changes, the control detaches events from the previous Document and subscribes to selection and change events on the new one.

## 2. Mouse Operations

- Click to the left: Selection of objects
- Shift + left click: Choose Add
- Ctrl + left click: Select Toughened Lack
- Alt + left click: Except for choice
- Track in the middle: Edit the screen move
- Wheel: extended and reduced
- Right drawing: Perspective Camera rotation
- Choose by pressing Space: Choose the sub-candidate of overlaped objects

During processing, selection and data changes are locked, but extension, reduction and screen movements are still available.

## 3. Entity Creation

The generating menu provides Point, Line, Arc, Polyline, Text, Barcode, Image, Group and 3D objects. Sirius3 1.13.0 has added Plane, Pyramid, Torus, NURBS Surface. The generated objects enter the current Active Layer of the Page and can immediately edit detailed properties in PropertyGrid.

## 4. Editing Shortcuts

| Key | Action |
|---|---|
| `CTRL+C` / `CTRL+X` / `CTRL+V` | Copy / Cut / Put in the mouse position |
| `CTRL+Z` / `CTRL+Y` | Undo / Redo |
| `CTRL+A` | Selection of Active Layer |
| `CTRL+Delete` | Remove the choice. |
| `CTRL+H` / `CTRL+SHIFT+H` | Align to the XY / XYZ origin |
| `CTRL+R` / `CTRL+M` | Toggle rendering / marking permission |
| `CTRL+F` | Fit the selected entities or Active Layer to the view |
| `CTRL+E` / `CTRL+Q` | Next / Previous Camera |

`CTRL+Arrow`, `CTRL+ALT+Arrow`, and `CTRL+ALT+SHIFT+Arrow` move by the distances configured in `KeyboardTransitXYCtrl`, `KeyboardTransitXYCtrlAlt`, and `KeyboardTransitXYCtrlAltShift`. The defaults are 1 mm, 0.1 mm, and 0.01 mm.

`CTRL+[`/`]`, `CTRL+ALT+[`/`]`, and `CTRL+ALT+SHIFT+[`/`]` rotate by the configured 90°, 10°, and 1° increments.

## 5. Simulation and Marker Key

| Key | Action |
|---|---|
| `F1` / `CTRL+F1` / `CTRL+ALT+F1` | Fast / Normal / Slow Simulation |
| `ESC` | Simulation or drag cancellation. |
| `F2` | Script Properties |
| `F4` | Marker Preview |
| `F5` | Start actual marking of the current Page |
| `F6` | Marker Stop |
| `F8` | Marker Reset |

This key follows the `UI.Config.Keyboard*` settings, especially F5 can operate real hardware, so check the work area, interlock, laser and active device and then use it.

## 6. Latest Stability Improvements

Since Sirius3 1.11.14, Editor and TreeView prevent the same shortcut from being handled twice and prevent repeated F5 confirmation dialogs. When the PropertyGrid search box has focus, `CTRL+F` searches properties instead of fitting the editor view; check the current focus when the shortcut appears to behave differently.

## 7. HiDPI and framework integration

With 1.15.0, configure the WinForms executable for Per-Monitor V2 and its demo host form for DPI scaling at a 96-DPI design baseline. WPF uses device-independent layout and an executable DPI manifest. See [WinForms](WinForms.md) and [WPF](WPF.md); the UI DLL does not select the host process DPI mode.

TreeView directional keys remain navigation commands while the tree has focus. Give the editor focus before using entity movement shortcuts. WPF routes keyboard input through its preview events; avoid duplicating the same command in additional host handlers.

In 1.16.0, the editor clipping command divides selected entities or one layer into regions. Review region size/overlap, Paths/ClosedRegions mode, hatch inclusion and source removal before applying; the operation supports Undo/Redo. See [clipping](ClippingUserManual.md). WPF marking confirmation uses the Sirius3 theme and localized Start/Cancel buttons.

## 8. Camera navigation shortcuts

With focus in the editor viewport, the following keys operate the camera:

| Keys | Action |
|---|---|
| ↑ / ↓ / ← / → | Move along the camera's Up/Right directions. |
| Alt + ↑ / ↓ | Move forward/backward along the camera's Front direction. |
| Alt + ← / → | Turn about the camera position's Z axis in 5° steps. |
| Ctrl + + / − | Zoom in/out; keypad Add/Subtract also work. |
| Ctrl + F | Fit selected entities, or the active layer when nothing is selected. |
| Ctrl + E / Q | Next/previous camera. |

Camera translation uses a step related to the visible height (2%, minimum 0.1). It is distinct from the configured millimetre step for moving entities. Bare arrows operate the camera; Ctrl+arrows operate selected entities. Ctrl+Alt and Ctrl+Alt+Shift choose the finer configured entity movement steps. Ctrl+Shift+arrows is not an additional movement mode.

`[` rotates selected entities clockwise and `]` counterclockwise when used with Ctrl, Ctrl+Alt or Ctrl+Alt+Shift. Defaults are 90°, 10° and 1°, respectively; the KeyboardRotate settings control these values.

TreeView arrow keys navigate the tree when that control owns focus. PropertyGrid text editing/search and host controls consume their own keys. Move focus to the viewport before using its commands. The WPF editing path also supports Delete; Ctrl+Delete is the documented viewport deletion shortcut. See [TreeView](TreeviewUserManual.md) for tree commands.

`UI.Config.KeyboardHelpMessage` shows the current configured help. F1 / Ctrl+F1 / Ctrl+Alt+F1 select Fast / Normal / Slow path simulation by default. Simulation is display-only; the configured Marker Start key (default F5) can operate real hardware. Preview, Stop, Reset and Script keys also follow Keyboard settings. Keep host handlers from invoking the same command twice.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
