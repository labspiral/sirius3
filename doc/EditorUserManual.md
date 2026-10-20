# EditorControl User Manual

> API reference: Sirius3 Core/UI 1.17 (public features)

[English](EditorUserManual.md) | [한국어](EditorUserManual.koKR.md) | [All guides](README.md)

## 1. Create, select and edit

WinForms and WPF EditorControl display and edit an assigned document. Choose the
active page and layer, create or import an entity, select it in the viewport or
TreeView, then edit its values in PropertyGrid. Use the integrated editor when
you also need page, tree, pen and device panels.

Supported drawing tools create points, lines, arcs, polylines, splines and basic
shapes interactively. Follow the upper-left stage instructions; Enter completes
supported multi-point tools and Escape cancels an unfinished operation. Text,
barcode, image and 3D tools expose their detailed settings through dialogs or
PropertyGrid. Completed edits participate in Undo/Redo.

## 2. Mouse selection and camera

| Input | Action |
|---|---|
| Left click / empty-space drag | Select an entity / select within a rectangle |
| Shift + left click | Add to the selection |
| Ctrl + left click | Toggle selection |
| Alt + left click | Exclude from the selection |
| Hold Space while selecting | Select a child inside a group |
| Wheel | Zoom around the pointer |
| Middle drag | Pan the camera |
| Right drag | Rotate a perspective camera |

Click selection chooses the nearest eligible visible geometry. Rotated, reflected
and scaled entities use the same screen-based picking tolerance. Selection and
data editing are locked while marking is busy; zoom and camera navigation remain
available. See [TreeView](TreeviewUserManual.md) for focus-dependent selection.

## 3. Grip editing

Select one supported entity to display its editing grips. A new entity selection
starts in **Move**. Press **Tab** for Move → Vertex → Scale and **Shift+Tab** for the
reverse order; modes unavailable for that shape are skipped.

| Mode | Edit |
|---|---|
| Move | Position or bounding-box movement of the whole entity |
| Vertex | Source vertices, curve controls, bulges and supported native dimensions |
| Scale | Size and rotation; reset scale when the entity exposes that action |

Drag a grip and inspect the live coordinate, dimension or angle in the upper-left
help. V / Shift+V moves to the next / previous available grip. Cyan circles
indicate source vertices, mint squares movement, lime diamonds size, and blue
markers curve/rotation controls. A committed drag creates one Undo action; Escape
cancels its preview without changing history.

**G** toggles magnetic attraction to source/control points (on by default).
Movement/position and native-dimension grips are continuous without modifiers.
Ctrl, Ctrl+Alt and Ctrl+Alt+Shift select configured grid intervals; X/Y/Z constrain
supported entity drags. Rotation is continuous without modifiers and uses the
configured angular grid with modifiers. Other size/source grips follow their
supported grid and aspect-ratio rules. Check the displayed help for the active grip.

## 4. Keyboard commands and focus

| Keys | Action in the viewport |
|---|---|
| Ctrl+C / X / V | Copy / cut / paste |
| Ctrl+Z / Y | Undo / redo |
| Ctrl+A | Select the active layer |
| Ctrl+Delete | Delete selected entities |
| F or Ctrl+F | Fit selected entities, or active-layer contents when nothing is selected |
| Ctrl+H / Ctrl+Shift+H | Center selection on XY / XYZ origin |
| R or Ctrl+R | Toggle rendering permission |
| M or Ctrl+M | Toggle marking permission |
| Ctrl+E / Q | Next / previous camera |
| Arrows | Move camera along its Up/Right axes |
| Alt+Up / Down | Move camera forward / backward |
| Alt+Left / Right | Turn camera about Z in 5° steps |
| Ctrl+Plus / Minus | Zoom in / out |

Ctrl+arrows, Ctrl+Alt+arrows and Ctrl+Alt+Shift+arrows move the selection to the
next grid boundary. Default intervals are **1 / 0.1 / 0.01 mm**. The corresponding
Ctrl+[ / ] combinations rotate clockwise / counterclockwise; defaults are
**90 / 10 / 1°**. The host can change these `UI.Config.KeyboardTransitXY*` and
`KeyboardRotate*` values. TreeView arrows navigate the tree, and PropertyGrid
text/search fields keep their own editing keys. F, M, R, V/Shift+V and G also work
in all Page, Block, Substrate and Wafer trees in WinForms and WPF. Tab in a tree
keeps normal focus navigation. Focus the viewport to move entities or navigate grips.

## 5. Simulation and device commands

| Default key | Action |
|---|---|
| F1 / Ctrl+F1 / Ctrl+Alt+F1 | Fast / normal / slow path simulation |
| Escape | Stop simulation or cancel an editing preview |
| F2 | Script properties |
| F4 | Marker preview |
| F5 | Start actual marking of the current page |
| F6 / F8 | Marker stop / reset |

These keys follow the host's `UI.Config.Keyboard*` settings. Simulation is
display-only. **Marker preview can move the scanner, and Marker Start can operate
real hardware.** Check the configured devices, work area and interlocks before use.

## 6. Clip, fonts and integration

Use [Clipping](ClippingUserManual.md) to split selected geometry or a layer into
regions with Undo/Redo. Review overlap, hatch inclusion and source removal first.
Use **Font Edit** in the context menu to open [Font Editor](FontEditorUserManual.md);
selecting SiriusText first opens its font. Font editing has its own file/history
workflow. See [WinForms](WinForms.md), [WPF](WPF.md),
[PropertyGrid](PropertyGridUserManual.md) and [entities](EntityUserManual.md).




## Creation ratio and vertex insertion/removal

Hold Shift alone while sizing an interactive Rectangle, Ellipse, Cross or similar shape for a 1:1 preview. Release Shift for free proportions; no new fixed-ratio property is persisted after creation. Ctrl+Shift and Alt+Shift do not constrain ratios. Existing fixed-ratio properties and movement/angle snap intervals remain effective.

In Vertex mode, Polyline2D/3D and Bezier, CatmullRom, Hermite, BSpline and NURBSpline support Insert on the pointed segment and Delete on the hovered or V-selected source vertex. Spline insertion edits its control-polygon segment and can change the curve shape. Keep at least two points in an open path, three in a closed polyline, and degree+1 in NURBS/BSpline. Each insertion/removal is one Undo/Redo action and never deletes the entity. Other modes and Ctrl+Delete retain entity deletion.

Upper-left help shows `Tab: Move mode`, `Tab: Vertex mode`, `Tab: Scale mode` and `V: Next grip`. Auxiliary controls are smaller and more transparent than source vertices.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
