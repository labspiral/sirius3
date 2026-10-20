# TreeView Page Control User Manual

> API reference: Sirius3 Core/UI 1.17 (public features)

[English](TreeviewUserManual.md) | [한국어](TreeviewUserManual.koKR.md) | [All guides](README.md)

## 1. Hierarchy and Processing Order

`TreeViewPageControl` displays the Layers, Groups, BlockInsert entries, and entities in one Page as a hierarchy. Moving nodes up or down or reorganizing them with drag-and-drop also changes the actual Marker execution order.

## 2. Selection and Synchronization

When you select the nodes in TreeView, the Editor selection and PropertyGrid are updated, and when you select in Editor, the nodes are synchronized.

- Bold: Active Layer
- Strikethrough: `IsAllowMark = false`
- Gray: `IsAllowRender = false`

## 3. Structure Editing

- Additional Layer
- Mixed/Uniform Group Creation and Ungroup
- Block/Block Insert Conversion
- Change the order to up/down
- Move Layers with drag-and-drop, or move entities between Layers

Mixing different layer levels or placing the Layer below the Entity is rejected. Mass nodes may display extension confirmation according to the `UI.Config.MaxTreeNodeItems` standard.

## 4. Arrow Keys When TreeView Has Focus

When TreeView has focus, Up/Down/Left/Right navigate, collapse, or expand nodes even when a modifier key is held. Therefore, the editor's `CTRL+Arrow` entity movement shortcut does not run. Click the editor first to move focus before using keyboard movement shortcuts.

## 5. Editor and marker shortcuts

TreeView forwards non-directional editor and marker shortcuts. Each handled shortcut runs once, preventing duplicate actions or native node searches from the same key.

| Key | Action |
|---|---|
| `CTRL+C/X/V` | Copy / Cut / Paste |
| `CTRL+Z/Y` | Undo / Redo |
| `R` / `CTRL+R` | Toggle rendering permission |
| `M` / `CTRL+M` | Toggle marking permission |
| `F` / `CTRL+F` | Fit the selection, or active-layer contents if nothing is selected |
| `V` / `Shift+V` | Next / previous Grip |
| `G` | Toggle magnetic Grip snap |
| `F1` | The simulation |
| `F4` | Preview |
| `F5` | Start actual marking of the current page |
| `F6` / `F8` | Stop / Reset |

F5 can start real marking while TreeView has focus. Check the active devices and work area before use.

These letter shortcuts work in the Page, Block, Substrate and Wafer trees in
WinForms and WPF. A handled letter does not also search for another node. Text
inputs keep their editing keys, and Tab keeps normal focus navigation; focus the
viewport to use V/Shift+V for Grip navigation. M changes marking permission
and adds an Undo/Redo action; it does not start marking.

## 6. HiDPI and framework support

WinForms 1.15.0 measures TreeView rows and node styles for the current DPI, including pages first displayed after a DPI change. `UI.Config.TreeviewFontSize` remains a logical point size and can refresh existing trees when changed on the UI thread. WPF supplies its own tree adapter and device-independent layout. See [WinForms](WinForms.md) and [WPF](WPF.md).



In the viewport, Tab/Shift+Tab changes Move/Vertex/Scale modes. Tab in the tree moves focus.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
