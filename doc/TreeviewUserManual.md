# TreeView Page Control User Manual

> Reference version: Sirius3 1.16.0 (public Release features)

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

## 5. Transmitted shortcut.

Except the direction key, the `CTRL` command and the View Shortcuts like F1/F2/F4/F5/F6/F8 are also transmitted to the Editor/Marker in TreeView. From Sirius3 1.11.14, the same key is not transmitted twice on the two routes, preventing duplicate paste actions and repeated F5 confirmation dialogs after cancellation.

| Key | Action |
|---|---|
| `CTRL+C/X/V` | Copy / Cut / Paste |
| `CTRL+Z/Y` | Undo / Redo |
| `CTRL+R/M` | Toggle rendering / marking permission |
| `CTRL+F` | Choosing to fit. |
| `F1` | The simulation |
| `F4` | Preview |
| `F5` | The current page actually begins. |
| `F6` / `F8` | Stop / Reset |

The F5 is focused on TreeView, but it can lead to a real Marker Start.

## 6. HiDPI and framework support

WinForms 1.15.0 measures TreeView rows and node styles for the current DPI, including pages first displayed after a DPI change. `UI.Config.TreeviewFontSize` remains a logical point size and can refresh existing trees when changed on the UI thread. WPF supplies its own tree adapter and device-independent layout. See [WinForms](WinForms.md) and [WPF](WPF.md).

---
2026 Copyright (c) SpiralLAB. All rights reserved.
