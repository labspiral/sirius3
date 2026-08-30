# EditorControl User Manual

> Reference version: Sirius3 1.12.3 (public Release features)

## 1. Purpose

`EditorControl` is a public WinForms control that displays the entities in an `IDocument` through an OpenGL editor and supports selection, translation, rotation, scaling, creation, and deletion. When the assigned Document changes, the control detaches events from the previous Document and subscribes to selection and change events on the new one.

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

The generating menu provides Point, Line, Arc, Polyline, Text, Barcode, Image, Group and 3D objects. Sirius3 1.12.3 has added Plane, Pyramid, Torus, NURBS Surface. The generated objects enter the current Active Layer of the Page and can immediately edit detailed properties in PropertyGrid.

## 4. Editing Shortcuts

| Key | Action |
|---|---|
| `CTRL+C` / `CTRL+X` / `CTRL+V` | Copy / Cut / Put in the mouse position |
| `CTRL+Z` / `CTRL+Y` | Undo / Redo |
| `CTRL+A` | Selection of Active Layer |
| `CTRL+Delete` | Remove the choice. |
| `CTRL+H` / `CTRL+SHIFT+H` | XY / XYZ / XY / XY / XY / XY |
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
| `F5` | The current page actually begins. |
| `F6` | Marker Stop |
| `F8` | Marker Reset |

This key follows the `UI.Config.Keyboard*` settings, especially F5 can operate real hardware, so check the work area, interlock, laser and active device and then use it.

## 6. Latest Stability Improvements

Since Sirius3 1.11.14, Editor and TreeView prevent the same shortcut from being handled twice and prevent repeated F5 confirmation dialogs. When the PropertyGrid search box has focus, `CTRL+F` searches properties instead of fitting the editor view; check the current focus when the shortcut appears to behave differently.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
