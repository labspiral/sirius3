# IHatch and Hatchable Entities User Manual

> API reference: Sirius3 Core/UI 1.17.0 (public features)

[English](HatchUserManual.md) | [한국어](HatchUserManual.koKR.md) | [All guides](README.md)

## 1. Patterns, entities and generated paths

`IHatch`, in `SpiralLab.Sirius3.Entity.Hatch`, describes one fill pattern. `HatchLine` generates parallel line segments; `HatchPolygon` generates successively inset polygon shells. Both derive from `HatchBase` and support property notifications, cloning and deterministic disposal.

`IHatchable`, in `SpiralLab.Sirius3.Entity`, is the entity-side contract that supplies closed contours and owns a list of patterns. Examples include closed vector entities, outline text, barcode entities and supported groups. Check the interface and actual closed geometry of the selected object. `EntityImageText` uses raster processing rather than `IHatchable` vector fill.

Entity regeneration prepares source geometry and contours, regenerates modified patterns and publishes their generated paths. Rendering displays those paths; the marker subsequently submits them with the applicable pens and transforms. Creating or regenerating a pattern does not start marking.

## 2. IHatch members

| Member | Contract |
|---|---|
| `Name` | Read-only through `IHatch`; built-in `HatchBase` exposes an editable name. |
| `ModelColor` | Read-only through `IHatch`; built-in patterns allow editing the display/pen color. The initial color comes from `UI.Config.EntityPenColors[0]`. |
| `HatchResult` | Borrowed, pattern-owned `IEntity` containing generated paths; may be null before successful generation. |
| `HatchRepeats` | Fill pass count, default 1. Use a positive count; the built-in PropertyGrid range is 1–100. |
| `IsModified` | Signals that generated paths need rebuilding. Geometry-affecting pattern setters set this flag. |
| `Tag` | Application data; excluded from built-in JSON serialization. |
| `PropertyChanged` | Property notification inherited from `INotifyPropertyChanged`. |
| `Regen(hatchable, contours, owner)` | Generates fill from closed local-XY contours; returns a Boolean result. |
| `Clone()` / `Dispose()` | Create an independent built-in pattern/result copy, or release the pattern and its generated result. |

Prefer attaching a pattern to an entity and regenerating the entity. Direct `IHatch.Regen` callers must supply valid local bounds and closed XY contours, including holes. Source contours remain caller-owned and unchanged.

## 3. HatchLine settings

The defaults below are those of `new HatchLine()`. `HatchFactory.CreateLine(angle, interval, exclude = 0.05, isZigZag = true)` overrides its supplied settings. The argument order is **angle, then interval**.

| Setting | Default | Effect |
|---|---:|---|
| `Angle` | 0° | Direction of each line in the source's local XY plane. |
| `Interval` | 0.05 mm | Nominal spacing perpendicular to the lines. Values below 0.001 mm are clamped to 0.001 mm. |
| `Exclude` | 0.05 mm | Inward boundary clearance, including hole boundaries; excessive clearance can eliminate narrow regions. |
| `Alignment` | `Center` | Places the line sequence within the projected range. |
| `Shift` | 0 mm | Periodic phase along the perpendicular axis; does not move the entity or slide paths along their length. |
| `IsZigZag` | `true` | Alternates the direction of successive hatch bands. |
| `Order` | `Ascending` | Traverses projected bands from lower to higher coordinates; `Descending` reverses this base order. |
| `Sort` | `Nearest` | Chooses path-order optimization. |
| `MinAdjacentDistance` | 0 mm | Adds an optimization penalty to short jumps; does not remove paths or change their shapes. |
| `Fill` | `EvenOdd` | Interprets the filled region before boundary offset and line intersection. |
| `Joint` | `Square` | Corner treatment during boundary offsets. |
| `IsSnapToGrid` | `false` | Aligns the first perpendicular coordinate to a grid while keeping the sequence inside its projected range. |
| `SnapToGridInterval` | 0.001 mm | Grid step; distinct from hatch line spacing. |

`Alignment.None` starts from the minimum projected coordinate. `Center` retains the nominal interval and centers the sequence. `Fit` recalculates the actual interval to fit the projected range. Grid snapping can reduce the line count to keep the sequence within bounds.

Spacing and clearance are defined in local millimeters. Entity/group scaling also scales the resulting machining geometry; camera zoom only changes its display. Even `Exclude = 0` retains a small numerical boundary clearance, so it is not a command to reproduce the outline exactly.

## 4. HatchPolygon settings

| Setting | Default | Effect |
|---|---:|---|
| `Interval` | 0.1 mm | Inward offset between successive shells; minimum 0.001 mm. |
| `Exclude` | 0 mm | Initial clearance. At zero, an interval-sized pre-inset is also performed before the first shell. |
| `Joint` | `Square` | Corner treatment on every inset. |
| `Order` | `Descending` | Base shell sequence goes from outside to inside; `Ascending` starts with inner shells. |
| `Sort` | `Nearest` | Orders paths/groups to reduce jumps. |
| `MinArea` | 0.005 mm² | Filters tiny shell paths by absolute polygon area. |

`HatchFactory.CreatePolygon(interval, exclude = 0)` supplies the spacing and clearance. `MinArea` is initialized by the constructor; changing `Interval` later does not automatically recalculate it. A value of zero still uses the implementation's small interval-dependent area tolerance. Large values can remove valid small features.

Polygon fill resolves input regions with `NonZero`; `HatchPolygon` has no `Fill`, `Angle` or `Alignment` property. Its shells follow the contour shape rather than being restricted to circles.

## 5. Fill rules, holes, joints and sorting

| `HatchFills` | Inside-region test |
|---|---|
| `EvenOdd` | Odd crossing parity; nested loops alternate filled and empty regions. |
| `NonZero` | Non-zero winding number; contour directions affect holes and overlapping regions. |
| `Positive` / `Negative` | Positive / negative winding number only. |

`GetOriginalContours` supplies filled-region topology. A visually similar list of outlines is not sufficient. Keep open paths open. For a normal NonZero region, use opposite directions for outer boundaries and holes. Mesh slices already use outer CCW/hole CW winding. `Contour.NormalizeAndNest` is appropriate when nesting defines holes; do not normalize intentionally directional input before interpreting its fill rule.

`Miter`, `Square`, `Bevel` and `Round` select offset corner treatment. Inspect acute corners, narrow features and holes after changing clearance or joints.

`Sort.None` uses the pattern generator's base traversal. `Nearest` and `Global` apply heuristic ordering; `Global` does not guarantee the mathematically shortest route. Optimization work is bounded and retains all generated paths. Choose `None` when the base band/shell sequence matters, and inspect the actual order when optimization is enabled.

## 6. Boundary/fill marking order and pens

| `HatchMarkOptions` | Entity processing order |
|---|---|
| `HatchLast` | Outline, then fill; default for common built-in hatchable entities. |
| `HatchFirst` | Fill, then outline. |
| `HatchOnly` | Fill without outline. |

`HatchMarkOption` belongs to the entity; `HatchLine.Order` and `HatchPolygon.Order` belong to individual patterns. The `Hatches` list orders patterns, and `HatchRepeats` repeats each pattern. Entity and marker repetition settings have their own scopes.

Keep `IsAllowHatch` enabled to use fill. To suppress outline marking, select `HatchOnly`; disabling the entity's `IsAllowMark` disables its marking, including fill. A HatchOnly entity with no active generated fill has no fill paths to mark.

Match the pattern's `ModelColor` to the intended document `EntityPen` and configure that pen's power, speed and other process parameters. A different hatch pen can be used from the outline pen. See [EntityPen](PenUserManual.md) and [markers](MarkerUserManual.md).

## 7. Add patterns through document actions

These fragments assume an initialized, idle document and a stable, document-owned `target`, and run on its owning UI thread. `ActAddHatch` regenerates and notifies the document through the normal action and supports Undo/Redo.

```csharp
using System.Linq;
using SpiralLab.Sirius3.Entity;
using SpiralLab.Sirius3.Entity.Hatch;

if (target is IHatchable hatchable && hatchable.IsClosed)
{
    var hatch = HatchFactory.CreateLine(angle: 45, interval: 0.1, exclude: 0.05);
    hatch.Alignment = HatchAlignments.Center;
    hatch.Sort = HatchSorts.None;
    hatch.HatchRepeats = 1;

    bool added = document.ActAddHatch(target, hatch);
    if (!added && !hatchable.Hatches.Contains(hatch))
        hatch.Dispose();
}
```

The membership check avoids disposing a pattern already attached if an action fails after attachment. Inspect the Boolean result and logs. After successful attachment, the entity/document history owns the pattern; do not wrap that attached instance in a caller's `using` scope.

For two directional passes, add two separate patterns as one document action:

```csharp
if (target is IHatchable hatchable && hatchable.IsClosed)
{
    IHatch[] patterns =
    {
        HatchFactory.CreateLine(0, 0.1, 0.05),
        HatchFactory.CreateLine(90, 0.1, 0.05)
    };
    bool added = document.ActAddHatches(target, patterns);
    if (!added)
        foreach (var pattern in patterns)
            if (!hatchable.Hatches.Contains(pattern)) pattern.Dispose();
}
```

`ActInsertHatch`, `ActRemoveHatch`, `ActRemoveHatches`, `ActMoveHatches`, `ActReplaceHatches` and `ActHatchClear` provide corresponding history-aware operations. Use `ActPropertyChanged` for entity property edits such as `HatchMarkOption`.

Direct `IHatchable.AddHatch`/collection methods are useful while preparing a caller-owned entity before document insertion. They do not create document Undo/Redo actions; regenerate the entity after parameter or collection changes. Direct pattern setters do not by themselves submit a marking job.

## 8. Editing in WinForms and WPF

Select the entity in the editor or TreeView, enable hatching, open the `Hatches` PropertyGrid editor, add a Line or Polygon pattern, then select that pattern to edit its settings. Remove/Clear and Up/Down manage the pattern list.

WinForms supports Apply preview while the dropdown remains open. Accept commits the edit; closing without acceptance restores the previous value after a preview. WPF edits an isolated cloned list and commits it on acceptance. Both use the entity's regeneration path and document editing workflow. See [WinForms](WinForms.md), [WPF](WPF.md) and [PropertyGrid](PropertyGridUserManual.md).

## 9. Regeneration, results and ownership

Regenerate after changing shape or hatch settings and inspect the return value. The marker requires clean prepared hatch data and reports an error for modified results instead of generating them during output. Empty/open geometry retires derived fill while retaining pattern settings for later valid geometry.

`HatchResult` is borrowed. Do not attach it directly to another entity or dispose it independently. Built-in `Clone()` copies the pattern's geometric settings and an independent generated result; copy custom `Name`/`Tag` metadata explicitly when required. Generated results and runtime `Tag`/`IsModified` are not saved as ordinary hatch parameters; document regeneration reconstructs derived paths.

An entity owns attached patterns. Direct Remove/Replace methods do not dispose removed patterns; an Undo/Redo owner may still need them. `ClearHatches(true)` disposes removed patterns, while `ClearHatches(false)` leaves their cleanup to the caller/history. Do not share one mutable pattern instance among independently owned entities; clone it.


## 10. SVG, clipping and diagnosis

New SVG imports can add HatchLine when `UI.Config.IsSvgImportHatchEnabled = true`, using `SvgImportHatchInterval` (default 0.1 mm) and half that interval as boundary clearance. The policy applies to new imports; existing document patterns remain editable independently. See [import settings](Sirius3UIConfigUserManual.md).

`ClipOptions.IncludeHatches` includes existing active HatchLine/HatchPolygon machining paths as independent clipped entities. It preserves captured paths rather than reapplying hatch parameters to every tile. See [clipping](ClippingUserManual.md).

If fill is missing, check closed/nonempty geometry, enabled hatching, fill/winding and holes, clearance, polygon minimum area, positive finite spacing, regeneration results and pen selection. Excessively dense line sequences are rejected before allocation. A smooth preview alone does not validate process energy or device execution.

Public examples: [editor_hatch](../demos/editor_hatch/Form1.cs), [editor_clip](../demos/editor_clip/Form1.cs). Related guides: [entities](EntityUserManual.md), [text](TextUserManual.md), [barcodes](BarcodeUserManual.md).

---
2026 Copyright (c) SpiralLAB. All rights reserved.
