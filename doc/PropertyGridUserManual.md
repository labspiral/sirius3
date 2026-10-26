# PropertyGrid User Manual

> API reference: Sirius3 Core/UI 1.17.0 (public features)

[English](PropertyGridUserManual.md) | [한국어](PropertyGridUserManual.koKR.md) | [All guides](README.md)

## 1. Select the target

Select a Page, Layer, entity, EntityPen, EntityLayerPen or device in the editor
or TreeView. PropertyGrid shows that target's supported settings. Check the
selected name and values before editing; a blank grid usually means there is no
applicable selection. Multi-selection exposes common editable properties.

## 2. Find a setting

Ctrl+F focuses property search. Search names, categories or descriptions, and
clear the search to restore the full list. A filtered-out property is not deleted.
Device-specific properties may be hidden/read-only when the registered device
does not support them. Descriptions use the chosen UI language and explain units,
prerequisites and related settings.

## 3. Apply values and previews

Enter the value in its displayed unit and finish the edit with Enter or the editor's
accept command. Check the resulting value when range limits or controller-resolution
conversion apply. Geometry edits refresh the drawing and participate in document
Undo/Redo when performed through the editor.

Dropdowns may offer Apply preview. Accept commits the change; cancel or close
without acceptance restores the prior value. For multi-selection, check all
intended targets. Text fields keep their own caret, typing and editing keys;
move focus back to the viewport before using drawing shortcuts.

## 4. Common groups

| Group | Examples |
|---|---|
| Basic / transform | Name, render/mark permission, position, rotation, scale |
| Geometry / text / hatch | Dimensions, text content, character spacing, hatch list |
| EntityPen | Power, frequency, speed, delay, raster and point settings |
| EntityLayerPen | ALC, Sky Writing, variable delay and supported controller modes |

Use [Pen](PenUserManual.md) and [LayerPen](LayerPenUserManual.md) for processing
relationships. A change to appearance does not necessarily change laser output.

## 5. Frameworks and operation state

WinForms and WPF expose the same document/device property workflow with their
native controls. The description area and dropdowns follow the host's font/DPI.
Editing is restricted while the marker is busy. Verify the selected device and
its readiness before starting processing.

See [Editor](EditorUserManual.md), [TreeView](TreeviewUserManual.md),
[WinForms](WinForms.md), [WPF](WPF.md) and [Font Editor](FontEditorUserManual.md).


---
2026 Copyright (c) SpiralLAB. All rights reserved.
