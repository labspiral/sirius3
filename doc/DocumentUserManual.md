# IDocument & Recipe Management User Manual

> API reference: Sirius3 Core/UI 1.17.0 (public features)

[English](DocumentUserManual.md) | [한국어](DocumentUserManual.koKR.md) | [All guides](README.md)

## 1. IDocument

`IDocument` manages the Pages, Layers, EntityPen entries, EntityLayerPen entries, Blocks, and selection state in a Sirius3 recipe. The editor and TreeView reference the same Document, while the Marker converts its data into RTC list commands in the selected Page and Layer order.

## 2. Connect Document to ViewerControl and EditorControl

One `IDocument` can be handled directly in application code and assigned to the `Document` property of a `ViewerControl` or `EditorControl` whenever it needs to be displayed.

- `ViewerControl`: Displays a Document and provides camera, selection, preview, and status functions.
- `EditorControl`: Displays the same Document and adds entity creation, deletion, transform, and Undo/Redo editing functions.
- The same `IDocument` instance can be assigned to both controls. Changes made through an `Act*` method and selection changes are propagated to the other control through Document events.

```csharp
using SpiralLab.Sirius3.Document;
using SpiralLab.Sirius3.UI.WinForms;

IDocument document = DocumentFactory.CreateDefault();

var viewerControl = new ViewerControl
{
    AliasName = "Production View",
    Dock = DockStyle.Fill,
    Document = document
};

var editorControl = new EditorControl
{
    Dock = DockStyle.Fill,
    Document = document
};
```

A control connects the required events when a Document is assigned. Assigning another Document or disposing the control disconnects the previous event handlers. Closing a control does not dispose the assigned `IDocument`; detach the Document from every control and dispose it explicitly in the code that created it.

```csharp
viewerControl.Document = null;
editorControl.Document = null;
document.Dispose();
```

When the same Document is used by multiple WinForms controls, access it from the same UI thread. Do not simulate or edit it concurrently from another thread. For real marking, register the `Marker` with the control and call `Marker.Ready(document)` again after opening the Document or making a substantial structural change.

## 3. console_document demo flow

Public `demos/console_document/Program.cs` shows the process of using the Document API without the UI Editor, and the process of connecting to the Viewer or Editor when the same Document is needed.

1. Call `Core.Initialize()`.
2. Create a default document with `DocumentFactory.CreateDefault()` and connect `OnAfterOpen`, `OnAfterSave` Event.
3. Create RTC, Laser, DIO, PowerMeter, Marker with `EditorHelper.CreateDevices(...)` and register with `marker.Ready(document, null, rtc, laser, powerMeter)`.
4. Create the Data Matrix with `EntityFactory`, configure `EntitySiriusText` and Hatch and then add it to `document.ActAdd(entity)`.
5. Add all entities and then call `document.ActRegen()`.
6. In `FindByEntityPenColor` and `FindByLayerPenColor`, find the Pen and set output, speed, Raster, Sky Writing, and Variable Polygon Delay values.
7. Process the `.sirius3` file with `ActOpen`, `ActSave`. After opening the file, run `ActRegen()` and `marker.Ready(document)` again.
8. If the user has chosen a view. `ViewerControl.Document = document` Choose the Editor. `EditorControl.Document = document` Give the same document.
9. Real processing before `marker.Reset()`, `marker.Ready(document)`, `marker.Start()` run in order.
10. On shutdown, stop the Marker and dispose devices and the Document before calling `Core.Cleanup()`.

The next example is the reduction of the core part of the demo.

```csharp
IDocument document = DocumentFactory.CreateDefault();

var entity = EntityFactory.CreateDataMatrix(
    "0123456789",
    EntityBarcode2DBase.Barcode2DCells.Dots,
    10,
    10);
entity.PenColor = SpiralLab.Sirius3.UI.Config.EntityPenColors[0];
document.ActAdd(entity);
document.ActRegen();

using (var form = new Form { Text = "Sirius3 Viewer" })
using (var viewer = new ViewerControl { Dock = DockStyle.Fill })
{
    viewer.Document = document;
    form.Controls.Add(viewer);
    form.ShowDialog();
    viewer.Document = null;
}

document.Dispose();
```

The demo opens the Viewer and Editor Form in turn and shows the concept. In the common WinForms products, place the Control within one default `Application.Run(...)` Message Loop and use `Show` or `ShowDialog`.

## 4. Page

A document contains four pages, `DocumentPages.Page1` through `Page4`. Each page
owns its layers; `ActivePage.ActiveLayer` is the destination for ordinary editing.
The integrated editor provides four `TreeViewPageControl` page tabs.

```csharp
document.Page = DocumentPages.Page2;
// ActivePage now refers to document.DocumentData.Pages[1].
```

Setting `Page` also selects the corresponding `ActivePage`. Assigning an existing
page from the same document to `ActivePage` updates `Page` in the other direction.
Set one property; there is no need to assign both.

`MarkerRtc.MarkProcedure` changes the order of layers and offsets **within the
selected page**. LayerFirst completes all layers at one offset before moving to
the next offset. OffsetFirst completes one layer at every offset before moving
to the next layer. See [offset execution order](OffsetUserManual.md).

## 5. Layer

Each Page has several Layers, and the order displayed in TreeView affects the processing order. Apply the control conditions of EntityLayerPen before starting the Layer, and process entity within it according to the order.

- If the Layer's Mark permission is disabled, it will cross that Layer.
- If the entity’s Mark permission is disabled, the entity only passes.
- Changing Layer order can also change the laser conditions, Sky Writing, ALC and device behavior order.

## 6. Block

Block is a reuseable master shape and BlockInsert is a reference to placing the shape. It can reduce memory and editing when repeating the same logo or pattern. The shape of the block itself, the ModelMatrix of BlockInsert, and the Marker Offset are accumulated in order, so please pay attention to the application of repeated movements, turns and divisions.

## 7. EntityPen and EntityLayerPen

- EntityPen: Power, Speed, Delay, Raster, Wobbel, etc.
- EntityLayerPen: Sky Writing, ALC, Variable Delay, etc.

If you select an object in PropertyGrid, you can check the pen connected to the object properties. If you replace the laser, make sure the `PowerMax`, PowerMap and support properties are re-connected.

## 8. Files and Import

- `ActNew`: Create new documents and basic layer
- `ActOpen`, `ActSave`: `.sirius3` Recipe Open/Save
- `ActImport`: Import SVG, DXF/DWG, HPGL/PLT, Gerber/Excellon, G-code/NGC, images and STL/OBJ/PLY/STP·STEP/3MF meshes.

`UI.Config.ImportMergeDistance` is a common permissible distance that connects the close endpoint of a vector path. If too large, the unrelated path can be combined, so set it on the basis of the drawing units and real intervals.

### SVG and 3MF import

`ActImport` imports `.svg` as an editable `EntitySvg`/`EntityContour` vector hierarchy and `.3mf` as `EntityMesh`. SVG coordinates become millimeters using 96 pixels/inch and a reflected Y axis; native line, arc and Bezier/NURBS sources are retained. Solid fills and stroke widths, caps and joins form regions. Text, images, masks, filters, clip paths, markers, dashed and non-scaling strokes are unsupported; convert them to supported outlines before import.

SVG line hatching is disabled by default. Set `UI.Config.IsSvgImportHatchEnabled = true` before a new import to add HatchLine at `UI.Config.SvgImportHatchInterval` (default 0.1 mm). Later opaque white paint cuts out earlier geometry by default; `EntitySvg.WhiteIsKnockout` controls this policy.

3MF converts built triangles and relative component placement to millimeters, retaining RGB colors, base materials and diffuse textures. Composite/multiple-property materials and non-mesh geometry are unsupported. Limits are 2,000,000 expanded build triangles and 64 MiB of encoded data per embedded image. Mesh materials affect visualization; prepare machining paths separately, for example by slicing.

```csharp
if (!document.ActImport("drawing.svg", out var imported))
    throw new InvalidOperationException("SVG import failed; check the Sirius3 log.");
```

Successful results are document-owned. Check the Boolean result and logs, then inspect dimensions, filled regions, holes and textures before using the machining data. See [UI Config](Sirius3UIConfigUserManual.md) for import settings.

## 9. Selection and Editing Actions

If possible, change the document by using the `Act*` method. This path will be updated together with select, Undo/Redo, TreeView, PropertyGrid and redirect.

- Choose: `Selected`, `SubSelected`, `ActSelectAll`
- Edited by: `ActAdd`, `ActRemove`, `ActCopy`, `ActPaste`
- Convert: `ActTranslate`, `ActRotate`, `ActScale`, `ActAlignTo`
- Structure: `ActMixedGroup`, `ActUniformGroup`, `ActUngroup`
- Order: `ActMoveUp`, `ActMoveDown`
- Form: `ActReverse`, `ActSlice`, `ActRegen`

If the Property setter set the right ModifyFlag, the PropertyGrid editing will automatically reproduce the required shape. If you have changed the field or subarrangement directly, use the Setter and `ActRegen` that corresponds to the contract.

`ActClip` divides document-owned `IClippable` sources into world-space Frustum regions and combines result insertion and optional source removal in one Undo/Redo action. Pass a layer alone when clipping an `EntityLayer`. See the [clipping guide](ClippingUserManual.md) and [editor_clip demo](../demos/editor_clip/Form1.cs).

## 10. Coordinates and ITransformable

Since Sirius3 1.12.1, `ITransformable` exposes path start and end points in three coordinate spaces.

- `OriginalIn/Out`: original coordinates
- `ModelIn/Out`: Your own ModelMatrix applied coordinates
- `RealIn/Out`: World coordinates accumulated to parents

MatrixStack and Marker Offset are further applied in the processing phase. if the screen coordinates and the RTC coordinates are different, first check which step values are compared.

## 11. Events and Cleanup

You can subscribe to the status changes as `OnNew`, `OnBeforeOpen`, `OnAfterOpen`, `OnBeforeSave`, `OnAfterSave`, `OnSelected`, `OnChildChanged`, `OnPageChanged`, `OnPropertyChanged`, `OnSimulationStarted`, `OnSimulationEnded`.

The code that created the document directly must call `Dispose()` when it is no longer used. Connected View and Device’s closing responsibility must be clearly managed by the code that created each.

## 12. UI host shutdown

An editor's `DisposeDevices()` returns `Task`. Await it before closing the editor or disposing its document so registered remote/MCP work finishes while the UI dispatcher is still available. If the host owns a separate server, await its `DisposeAsync()` first. Detach other viewers from a shared document and dispose it once. Finish these steps before the application calls `Core.Cleanup()`. See [WinForms](WinForms.md) and [WPF](WPF.md) for closing-event patterns.

## PDF page import

PDF files are imported as raster page images (`EntityImagePDF`), not editable
vector text/paths. Import starts with page 1. Select the image and change
`PageNumber` (one-based) and `RenderDpi` (36–600, default 150) in PropertyGrid.
Keep the source PDF available when changing pages or resolution. Invalid or
unreadable pages leave the previous page image intact. Width/Height are physical
mm dimensions; increasing DPI increases raster detail and memory use.

```csharp
var image = EntityFactory.CreatePDFImage(@"C:\Drawings\label.pdf", 1, 20);
if (!document.ActAdd(image))
    image.Dispose();
```

See [image entities](EntityUserManual.md) for raster processing and
[clipping](ClippingUserManual.md) for regional output.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
