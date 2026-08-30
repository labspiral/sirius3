# IDocument & Recipe Management User Manual

> Reference version: Sirius3 1.12.3 (public Release features)

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

The document provides up to four pages. Page is the top-level run unit that separates different drawings or processes. If you change the active Page in the Editor, the Layer and Entity of that Page will be edited.

The `LayerFirst` and `OffsetFirst` settings in MarkerRtc change the repeated order when processing multiple Page/Offset. The same data can be “Take all layers and then offset” or “Take all offset and then layer” according to the settings, so set the actual production order first.

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
- `ActImport`: Import DXF/DWG, HPGL/PLT, Gerber/Excellon, G-code/NGC, image, STL/OBJ/PLY/STP·STEP etc.

`UI.Config.ImportMergeDistance` is a common permissible distance that connects the close endpoint of a vector path. If too large, the unrelated path can be combined, so set it on the basis of the drawing units and real intervals.

## 9. Selection and Editing Actions

If possible, change the document by using the `Act*` method. This path will be updated together with select, Undo/Redo, TreeView, PropertyGrid and redirect.

- Choose: `Selected`, `SubSelected`, `ActSelectAll`
- Edited by: `ActAdd`, `ActRemove`, `ActCopy`, `ActPaste`
- Convert: `ActTranslate`, `ActRotate`, `ActScale`, `ActAlignTo`
- Structure: `ActMixedGroup`, `ActUniformGroup`, `ActUngroup`
- Order: `ActMoveUp`, `ActMoveDown`
- Form: `ActReverse`, `ActSlice`, `ActRegen`

If the Property setter set the right ModifyFlag, the PropertyGrid editing will automatically reproduce the required shape. If you have changed the field or subarrangement directly, use the Setter and `ActRegen` that corresponds to the contract.

## 10. Coordinates and ITransformable

Since Sirius3 1.12.1, `ITransformable` exposes path start and end points in three coordinate spaces.

- `OriginalIn/Out`: original coordinates
- `ModelIn/Out`: Your own ModelMatrix applied coordinates
- `RealIn/Out`: World coordinates accumulated to parents

MatrixStack and Marker Offset are further applied in the processing phase. if the screen coordinates and the RTC coordinates are different, first check which step values are compared.

## 11. Events and Cleanup

You can subscribe to the status changes as `OnNew`, `OnBeforeOpen`, `OnAfterOpen`, `OnBeforeSave`, `OnAfterSave`, `OnSelected`, `OnChildChanged`, `OnPageChanged`, `OnPropertyChanged`, `OnSimulationStarted`, `OnSimulationEnded`.

The code that created the document directly must call `Dispose()` when it is no longer used. Connected View and Device’s closing responsibility must be clearly managed by the code that created each.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
