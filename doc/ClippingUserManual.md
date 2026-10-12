# Document Clipping User Manual

> Reference version: Sirius3 1.16.0 (public Release features)

[English](ClippingUserManual.md) | [한국어](ClippingUserManual.koKR.md)

## 1. Select sources and regions

`IClippable` creates independent geometry inside a world-space `Frustum`. `IDocument.ActClip` adopts those results into the active layer as one group per nonempty region. It combines insertion and optional removal of contributing sources into one Undo/Redo action. Use the clipping command in the WinForms or WPF editor, or the public [editor_clip](../demos/editor_clip/Form1.cs) example.

Select document-owned supported entities. Pass an `EntityLayer` alone to clip its contents. When a parent and its child are both selected, the parent is processed once. World-space regions account for the source hierarchy's transforms; the target hierarchy requires fixed alignment and invertible standard transforms.

## 2. Paths, filled regions and hatches

| Option | Behavior |
|---|---|
| `ClipModes.Paths` (default) | Retains path portions inside the region; open paths remain open and supported native curve types are retained. |
| `ClipModes.ClosedRegions` | Intersects filled planar regions and includes cutting boundaries; sources must share a local XY plane. |
| `IncludeHatches` (default `true`) | Includes existing active HatchLine/HatchPolygon paths as independent entities; results do not regenerate the source hatch settings. |
| `Fill` | Interprets input regions in ClosedRegions; default `HatchFills.EvenOdd`. |
| `Winding` | Sets local-XY output orientation; default outer CCW and holes CW. |

Raster clipping retains the pixel pitch and positions, crops to active pixels, and ignores transparent pixels in subsequent clipping and marking. With zero overlap, adjacent image tiles retain their rendered boundaries without shifting samples.

## 3. Add results with Undo/Redo

The following fragment assumes an initialized document with selected planar `IClippable` entities. `DVec2` denotes the project's OpenTK 3/4 double-vector alias.

```csharp
var regions = new[]
{
    Frustum.Build(new DVec2(-10, -10), new DVec2(10, 10))
};
var options = ClipOptions.Default;
options.Mode = ClipModes.ClosedRegions;
options.IncludeHatches = true;
bool success = document.ActClip(document.Selected, regions, options, isRemove: false);
```

On success, results are document-owned and named Clip1, Clip2, etc. `isRemove: false` retains sources. The default `true` removes only sources that produced results; clipping a layer retains the layer itself and removes all of its original children. A false return indicates invalid input, failed calculation or no retained results; failed calculation leaves sources unchanged. Keep source geometry, ownership and the active target unchanged during this synchronous operation.

Calling `IClippable.Clip` directly leaves the source unchanged and returns caller-owned entities. Adopt or dispose every result. Use `ActClip` for document editing and Undo/Redo instead of the deprecated `ClipHelper` API.

## 4. Editor workflow

Select the explained object or layer in the editor/TreeView, then open clipping. Set the grid's area size and overlap, preview the regions, choose the geometry and hatch options, and decide whether to retain the source. Apply once and inspect resulting groups, boundaries and hatch paths before preparing a marking job. Selecting a layer does not remove its page identity.

Related guides: [documents](DocumentUserManual.md), [entities](EntityUserManual.md), [WinForms](WinForms.md), [WPF](WPF.md).

## Related features

See [Offset coordinates and data parsing](OffsetUserManual.md), [text](TextUserManual.md), [barcodes](BarcodeUserManual.md), [hatching](HatchUserManual.md) and [complete EntityFactory](EntityUserManual.md).

---
2026 Copyright (c) SpiralLAB. All rights reserved.
