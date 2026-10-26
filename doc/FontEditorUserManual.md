# Font Editor User Manual

> API reference: Sirius3 Core/UI 1.17.0 (public features)

[English](FontEditorUserManual.md) | [한국어](FontEditorUserManual.koKR.md) | [All guides](README.md)

## 1. Open Font Editor

In the WinForms or WPF editor, right-click the viewport and choose **Font Edit**
below **Stitched image**. Select a SiriusText entity first to open its font file.
Font editing is independent of the drawing document and does not operate marking
devices. Its Undo/Redo history applies to font edits, separately from document history.

Use **Open** for an existing file or **New** for CXF, LFF or DOT. Existing EO
ASCII/binary FNT files can be opened and edited; FNT is not offered in the New menu.
The window title shows the font name and extension. Resolve any unsaved changes
before opening another font or closing the window.

## 2. Formats and units

| Format | Editing and storage |
|---|---|
| CXF | Vector paths in mm; supported native lines, arcs and polylines are retained. |
| LFF | Vector polylines and bulge arcs; unedited character references are preserved. |
| EO ASCII / binary FNT | Integer-encoded line paths. Unsupported imported curves are approximated and rounded to the format's storage grid. |
| DOT | Integer pixel cells. The pixel/mm display toggle changes coordinates shown, while saved physical pixel pitch is preserved. |

Y=0 is the baseline. BBox defines the permitted font area; ascent, descent and
x-height must fit it. Coordinate and metric entry uses mm for vectors and integer
pixel coordinates in DOT pixel view. Display rounding does not reduce source precision.
DOT cells remain on the integer grid and are displayed without antialiasing.

## 3. Characters, paths and vertices

The left **Characters** panel selects a character; the center canvas edits it;
the right table shows ordered paths and vertices. Resize the dividers as needed.
Latin characters are shown initially; choose a BMP block or enter a character/code
to navigate. Ctrl+PageUp/PageDown changes character.
Arrow keys in Characters navigate that panel rather than moving the canvas.

| Character state | Meaning |
|---|---|
| Empty / gray | No usable character geometry |
| Editing / blue | Changed relative to the saved data |
| Complete / green | Marked complete at its current geometry |
| Saved / dark slate | Matches saved character data |

The amber border shows the active character. **Complete** marks progress; use
**Save As** to write a file. Space width is configured by SiriusText rather than
by adding a normal space glyph here.

## 4. Draw and edit

Choose Select, Line, Polyline, Circle, Ellipse, Arc or DOT as supported by the format.
Empty vector characters begin with Polyline; DOT characters begin with pixel drawing.
Click a DOT cell to toggle it. Circle, Ellipse and Arc are separate tools:
Ellipse uses a center and bounding corner; Arc uses center, start and end, with
Ctrl reversing its sweep. For Polyline, A switches line/arc construction, Enter
commits accepted points, Backspace returns a stage and Escape cancels.

Click a path or vertex to select it; Ctrl/Shift extends the selection. Drag a
selected item to move it, or drag empty canvas for box selection. The vertex
table permits coordinate/bulge edits, point insertion/removal, path reversal and
open/closed state changes. Drag rows to reorder vertices within a path; select a
whole path to change path order. Undo/Redo restores these edits.

New points and translations stay within BBox. Ordinary movement stops at its
boundary. Use **Clip** explicitly for geometry already outside it; inspect the
result before saving. CXF/LFF retain native supported arcs. FNT curve approximation
uses 0.01% of ascent (BBox height when ascent is zero), plus storage-grid rounding.

## 5. Grips, snapping and navigation

On the selected vector path, **Tab** cycles Move → Vertex → Size and **Shift+Tab**
reverses the cycle. Unsupported modes are skipped; a straight two-point path has
Move and Vertex. Selecting a different path/character starts in Move.

| Mode | Action |
|---|---|
| Move | Move the entire path using its center/bounds grips, or rotate it about Z |
| Vertex | Edit source vertices and bulges |
| Size | Resize the whole path |

V/Shift+V selects the next/previous available grip. Read the upper-left
coordinate, size, angle or Bulge value while dragging. Escape cancels the preview;
a completed drag creates one Undo action. Whole-path Move/Size applies to the
whole path, including Move rotation, even after selecting only some table rows. Font path sizing edits source coordinates and keeps the normal size-grip opacity.

**G** toggles magnetic attraction to points and guides, enabled by default. Its
3 baseline-pixel picking distance scales with display DPI and is independent of
zoom. It is separate from the movement/angle grid. Position/size-related movement
is continuous without modifiers where supported. Ctrl / Ctrl+Alt / Ctrl+Alt+Shift
uses the host's default **1 / 0.1 / 0.01 mm** intervals; rotation uses
**90 / 10 / 1°**. Shift drag locks the nearest axis, Alt alone locks vertical
movement, and X/Y explicitly constrains it. Ctrl combinations select grid spacing
without adding an axis lock. DOT retains mandatory integer cells.

Wheel zooms at the pointer; middle/right drag pans. F, Ctrl+F or Home fits the
view. Plain arrows pan the canvas; Ctrl combinations with arrows move the selected
vertices to the next configured grid boundary. Ctrl+[ / ] rotates clockwise /
counterclockwise with the corresponding configured angular interval.

## 6. BBox, metrics, scaling and guides

Open **BBox / Metrics** (Ctrl+Shift+M) to edit the font name, limits, ascent,
descent and x-height. Reducing BBox can require clipping affected characters;
review the choice before applying. The Scale tab accepts a scale factor or target
reference height. It changes the whole font as one Undo transaction. DOT scaling
preserves integer cell indexes while changing physical pitch.

Use Guides, Shift+H or Shift+V for horizontal/vertical guides. Drag a selected
guide, press F2 for its exact position, or Delete to remove it. Ctrl+Shift+H clears
guides. Guides are application-wide view preferences and are not font-file content.

## 7. Preview, import and TTF conversion

Ctrl+Shift+P opens **Text Preview**. Enter a test string and adjust the actual
SiriusText settings, including spacing, fixed-cell width, glyph fitting and
multiline layout. Check capitals, descenders, whitespace and the intended size.
The preview window has its own Fit/view controls and optional Stay on top.

Ctrl+I imports DXF geometry into the current character. Review units, BBox,
path order and any format conversion before accepting it.

Ctrl+Shift+C opens **Font Converter** for installed TrueType fonts.

1. Select the font and its Regular/Bold/Italic/Bold Italic style. The output list
   contains **CXF, LFF and DOT**, with **CXF** selected initially. Basic Latin is
   the initial character block. EO ASCII/binary FNT output is not offered here;
   existing FNT files can still be opened and edited.
2. For CXF/LFF, set the reference height in mm and maximum curve-segment length
   (default 0.02 mm). Approximation error is 0.01% of the height. BBox width **0**
   means automatic; a positive override must contain the converted geometry.
3. For DOT, set the integer raster dimensions and **Dot factor** to adjust pixel
   density. Pixel weight is fixed to **Normal**, so there is no separate Weight
   selector. Font style remains available and changes the source glyph shape.

The required converter executables must be present in the application's Sirius
font folder. See [font files and text entities](TextUserManual.md) for reuse.

## 8. Save and reuse

Ctrl+S and Ctrl+Shift+S both run **Save As**. Existing destination bytes are backed
up to `.bak`; older backups are retained separately. A failed save preserves the
original. If the file was changed externally, reopen/reconcile it instead of
overwriting it with a stale session. Keep an independent copy before major changes.

After saving, use the file with EntitySiriusText or EntityCircularSiriusText. Matching
text already open in the application refreshes after a successful save/conversion.
Saving a font is outside document Undo/Redo. Store the font beside the recipe or
in the configured Sirius font folder so another installation can resolve it.

## 9. Main shortcuts

| Keys | Action |
|---|---|
| Ctrl+N / O / S | New / open / Save As |
| Ctrl+Z / Y | Undo / redo |
| V (without selected-path grips) / L / P / C / Alt+E / A / D | Select / line / polyline / circle / ellipse / arc / DOT |
| Tab / Shift+Tab | Next / previous available grip mode |
| V / Shift+V | Next / previous grip |
| G | Magnetic snapping on/off |
| F / Ctrl+F / Home | Fit |
| Ctrl+Shift+P / M / R / C | Text Preview / metrics / font scale / TTF converter |
| Ctrl+I | Import DXF |
| F4 / Shift+F4 | Show order / jump paths |
| Ctrl+Shift+X | DOT pixel/mm coordinate display |
| F1 | Keyboard help |

Drawing letters and grip shortcuts belong to the canvas. Text fields and table
cells retain normal typing/editing keys. F4 controls order display here; it is
not the drawing editor's device-preview command.

## 10. Application integration

Use `SpiralLab.Sirius3.UI.FontEditor.FontEditorForm` as a modeless window or host
`FontEditorControl` in a WinForms container. Its public asynchronous methods are
`OpenFontAsync`, `NewFontAsync`, `SaveFontAsync` and `ConfirmCloseAsync`.

```csharp
var window = new SpiralLab.Sirius3.UI.FontEditor.FontEditorForm();
window.Show(this); // this is the owning WinForms Form
bool opened = await window.Editor.OpenFontAsync(@"C:\Fonts\romans.cxf");
```

Call these operations on the owning UI thread. For a custom embedded host, await
`ConfirmCloseAsync()` from a cancellable closing path before disposing the
control. WPF users can open the same editor from the viewport context menu.
See [text](TextUserManual.md), [Editor](EditorUserManual.md),
[WinForms](WinForms.md) and [WPF](WPF.md).




## Creation and Vertex-mode keys

Hold Shift alone during Ellipse creation for a circular preview. The constraint ends with creation; Ctrl+Shift and Alt+Shift do not impose a ratio.

In canvas Vertex mode, Insert splits the pointed line/arc and Delete removes the hovered or V-selected vertex. Keep at least two vertices in open paths and three in closed paths, preserving bulges and Undo/Redo. The vertex table and other modes retain selection deletion, midpoint insertion and path continuation. DOT and guide deletion retain their existing behavior.

Help displays `Tab: Move mode` / `Tab: Vertex mode` / `Tab: Size mode` and `V: Next grip`. Shift+Tab selects the previous mode, Shift+V the previous grip, and Alt+V adds a vertical guide. Auxiliary Bulge controls are smaller and more transparent than source vertices.

Move, Vertex and Size markers use 8.8 logical pixels; auxiliary controls use 7.2 pixels, and active emphasis adds 3.2 pixels. These are 20% smaller while retaining the relative sizes. Display DPI scales the markers; the magnetic/picking margin remains unchanged. Move/Size handles and square marker orientation follow object rotation. FontEditor resizes rotated paths along their local axes while keeping the opposite anchor fixed.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
