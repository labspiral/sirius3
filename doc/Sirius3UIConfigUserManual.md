# SpiralLab.Sirius3.UI.Config Settings Guide

> Reference version: Sirius3 1.12.3 (public Release features)

## 1. Role and Configuration Timing

`SpiralLab.Sirius3.UI.Config` is the static configuration surface for Document, Editor, TreeView, PropertyGrid, import, text, Marker, and WinForms extension behavior. Set these values before creating UI controls and Documents so newly created objects use the intended policy.

Core settings and names are the same, so using the pseudonyms can reduce confusion.

```csharp
using CoreConfig = SpiralLab.Sirius3.Config;
using UIConfig = SpiralLab.Sirius3.UI.Config;

CoreConfig.DecimalPrecision = 3;
UIConfig.UnReDoSize = 50;
UIConfig.ImportMergeDistance = 0.001;
UIConfig.KeyboardMarkerStart = SpiralLab.Sirius3.View.GLKeys.F5;

if (!SpiralLab.Sirius3.Core.Initialize())
    throw new InvalidOperationException("Sirius3 initialization failed.");
```

Config values apply to the whole process.You must not expect different values for each editor in a program that uses multiple editors.The already generated or cache fonts, pen, shapes and UIs may not be automatically re-generated even if you change the settings.

## 2. Document and Undo/Redo

| Setting | Default | Explanation and point. |
|---|---:|---|
| `MaxPages` | `4` | The maximum number of pages that one document can have. read only. |
| `IsUnReDoEnable` | `true` | Undo/Redo records are used. the property name uses the letter defined in the API. |
| `UnReDoSize` | `30` | The number of Undo/Redo records to be stored. `0` has no number limit, so check the memory use together. |
| `IsFileSaveWithImage` | `false` | `IDocument.ActSave` decides whether Snapshot images will be stored in the same location. View and storage folder requires the authorization to write. |

Undo/Redo acts based on the `IDocument.Act*` task. If you change the Page, Layer or Entity Collection directly, you may not guarantee the same status for history and TreeView, so use the Public Document task method.

## 3. File Format and JSON Converter

| Property | Default or action. | Explanation |
|---|---|---|
| `AssemblyName` | `SpiralLab.Sirius3.UI.dll` | UI Assembly Files |
| `AssemblyVersion` | The running assembly version. | `Major.Minor.Build` format only read value |
| `FileOpenFilters` | Sirius3, DXF, DWG | Open the dialog box Filter |
| `FileImportFilters` | Sirius3, STL, OBJ, PLY, DXF, DWG, image, HPGL/PLT, Gerber, G-code | Importation of the dialog box Filter |
| `FileSaveFilters` | `.sirius3` | The dialog box Filter. |
| `FileImportImageFilters` | JPG, BMP, PNG, GIF, TIFF | Imaging the image Filter |
| `FileMeasurementFilter` | `.txt` | Measurement File Filter |
| `IsCompressedFileFormat` | Release `true`, Debug `false` | Decide whether to save the new `.sirius3` file in a compressed format. distinguish from the ability to open the existing file. |
| `JsonExternalConvertes` | `List<JsonConverter>` | This is a list of Newtonsoft.Json Converter used when rendering customized formats. |

Custom Converter is registered before opening the Document.

```csharp
using UIConfig = SpiralLab.Sirius3.UI.Config;

UIConfig.JsonExternalConvertes.Add(new MyEntityJsonConverter());
// After Run DocumentSerializer.Open or IDocument.ActOpen
```

Do not duplicate the Converter to process the same format. If the Converter’s `CanConvert` range is excessively implemented, it can affect the linearization of the Sirius3 default format.

## 4. Paths

| Setting | Basic route. | Explanation |
|---|---|---|
| `SiriusFontPath` | `siriusfonts` | `.cxf`, `.lff`, `.fnt`, `.dot` Sirius Font file location |

The default path is based on the application's Base Directory.When you put the distribution file in a different location, specify the absolute path before you first call the Font and check the read permits.

## 5. Parallel Processing

| Setting | Default | Explanation and limitation. |
|---|---:|---|
| `MaxDegreeOfParallelism` | 50 % of the logical processor number, at least 1 | AABB Tree, Gerber Tessellation, Hatch, Grip Parsing, Contour Processing, Point Ranking, etc. are the maximum parallel running numbers. If you specify a value less than 1, `ArgumentOutOfRangeException` occurs. |

It doesn’t always speed up to make the value bigger. UI responsibility, memory bandwidth, and other tasks that are executed at the same time, take into account and measure it as a representative document.

## 6. TreeView

| Setting | Default | Explanation |
|---|---:|---|
| `MaxTreeNodeItems` | `10,000` | Maximum items to be processed under one TreeView Node |
| `TreeviewFontSize` | `8.25` pt | TreeView Node font size |
| `TreeviewNodeDefaultFont` | Segoe UI Regular | Ordinary Node fonts, only to read outside |
| `TreeviewNodeBoldFont` | Segoe UI Bold | Node fonts, only to read outside. |
| `TreeviewNodeStrikeOutFont` | Segoe UI Strikeout | Unactivity in the back of the cancellation line font, only read outside. |
| `TreeviewNodeBoldAndStrikeOutFont` | Segoe UI Bold + Strikeout | Sign and cancellation, read outside. |

If you change `TreeviewFontSize` while running, the already created font objects will not be automatically re-generated. specify before starting, or expressly re-configurate the related resources with TreeView after changing.

## 7. Editor

| Setting | Default | Explanation |
|---|---:|---|
| `MaxSubSelectionItems` | `50` | Maximum number to list the Selected Group Entity in the menu |
| `IsHideEditorToolBars` | `false` | Public EditorControl tool collection display policy |

Growing `MaxSubSelectionItems` in a mass group increases the time and number of items to update the selection menu.

## 8. EntityPen and EntityLayerPen

| Set up or event. | Default | Explanation |
|---|---|---|
| `EntityPenColors` | White, Yellow, Orange, Red, Cyan, Lime, Magenta, Brown, Purple, Blue | Basic 10 Colour Settings Connecting Entity and `EntityPen` |
| `LayerPenColors` | `EntityPenColors` Like here. | Color arrangements used in `EntityLayerPen` |
| `IsConvertToControllerResolution` | `false` | Decide whether the device dependence value of the Pen will be treated according to the Connected Controller resolution. It is separate from the KFactor coordinate conversion. |
| `OnCreateEntityPen` | No connected handler. | The new `EntityPen` makes the Power, Frequency, PulseWidth, Delay, Speed, Raster, SCANAhead, and Wobbel default values as user code. |
| `OnCreateLayerPen` | No connected handler. | The new `EntityLayerPen` makes the default values ALC, Sky Writing, Variable Polygon/Jump Delay, and syncAXIS as user code. |

The Pen Generation Event is registered before the Document or Editor creates the default Pen. The Handler must return one fully initiated Pen, and the Entity's `PenColor` must match the color of `EntityPenColors`.

```csharp
using System.Drawing;
using SpiralLab.Sirius3.Document;
using SpiralLab.Sirius3.Entity;
using UIConfig = SpiralLab.Sirius3.UI.Config;

UIConfig.OnCreateEntityPen += CreateEntityPen;

static EntityPen CreateEntityPen(IDocument document, Color color)
{
    return new EntityPen
    {
        Name = color.ToKnownColor().ToString(),
        PenColor = color,
        Power = 1,
        Frequency = 50_000,
        PulseWidth = 2,
        JumpSpeed = 500,
        MarkSpeed = 500,
        ScannerJumpDelay = 250,
        ScannerMarkDelay = 150,
        ScannerPolygonDelay = 100
    };
}
```

In public demo's `editor_pen` and `editor_pen_multiple`, you can use this event to check the flow that creates the default value of Pen.

## 9. View and Simulation

| Setting | Default | Range or action. |
|---|---:|---|
| `SelectedLineWidth` | `2` | Sub-selection entity line thickness and point size |
| `DragMousePixel` | `5` px | Mouse Drag to judge. |
| `RayHitTestPixelSize` | `4` px | The screen size permitted by Ray Hit Test |
| `FrustumHitTestPixelSize` | `1` px | Frustum Hit Test Screen Size |
| `SimulationMarkerPixelSize` | `15` px | Simulation target diameter, limited to at least 3 px |
| `SimulationBeamPixelWidth` | `20` px | Simulation Beam Start Dimension, Limited to at least 3 px |
| `SimulationDebrisEnabled` | `true` | Simulation Target About Use Debris Effect |
| `SimulationDebrisMaxParticles` | `16` | The number of particles at the same time, limited to 1 to 128. |
| `SimulationDebrisSpreadPixelRadius` | `10` px | Debris spread circle, at least 1 px |
| `SimulationDebrisLifetimeMilliseconds` | `1,500` ms | Debris average maintenance time, limited to 100-10,000 ms |
| `CameraZoomFitSteps` | `30` | Zoom Fit Camera Conversion |

Target, Beam, Debris size is based on the screen pixels, so the camera zoom changes and the visible size remains in general. This effect is a visual simulation and does not represent the real Laser Spot size or energy.

## 10. Text and Font

| Setting | Default | Explanation |
|---|---|---|
| `InstalledFontNames` | List of Fonts. | The first time you read, the first time you read, the first time you read. |
| `ImageTextClearColor` | `Color.Black` | `EntityImageText` background color |
| `ImageTextFillBrush` | `Brushes.White` | `EntityImageText` Brush |
| `ImageTextPenColor` | `Color.White` | `EntityImageText` corner line color |
| `ImageTextRenderingHint` | `SingleBitPerPixel` | GDI Text Rendering |
| `ImageTextSmoothingMode` | `None` | GDI Smoothing Method |
| `ImageTextPixelOffsetMode` | `HighQuality` | The Pixel Offset |
| `FontDefault` | `Segoe UI` | `EntityText` Windows Font |
| `SiriusFontDefault` | `romans2.cxf` | `EntitySiriusText` basic Sirius Font file |
| `SiriusFontCapitalSample` | `@567890ABHWMZQ0()` | Representative characters to calculate the large font height and fixed cell width in Sirius Font |

In binary Raster processing, the combination of `SingleBitPerPixelGridFit`, `SmoothingMode.None`, `PixelOffsetMode.None` is generally clear. in the gray thread variation Raster, consider the combination of `AntiAliasGridFit`, `SmoothingMode.AntiAlias`, `PixelOffsetMode.HighQuality`. after changing settings, re-create Text Entity or `Regen()` to update the shape and Raster data.

### Fixed Text Sample

When automatically estimating the glyph width in Fixed intervals, use the following settings.

| Setting | Basic Sample |
|---|---|
| `FixedTextHangulFallbackSample` | `대한민국스파이럴랩옳닳흙깊` |
| `FixedTextChineseFallbackSample` | `中文汉字國語測試永高低上下左右鼎鬱龘` |
| `FixedTextJapaneseFallbackSample` | Japanese Gana-Hanga representative list |
| `FixedTextLatinFallbackSample` | `HMQXWgyjpq` |
| `FixedTextCyrillicFallbackSample` | `ШЖФфрудцщ` |
| `FixedTextArabicFallbackSample` | Arabic text representative. |
| `FixedTextDevanagariFallbackSample` | DeVanagari representative. |
| `FixedTextBengaliFallbackSample` | The Goliath representative. |
| `FixedTextGreekFallbackSample` | `ΗΜΩβγμρφψξ` |
| `FixedTextHebrewFallbackSample` | `אבגךםןףץ` |
| `FixedTextThaiFallbackSample` | The Thai Post. |
| `FixedTextTamilFallbackSample` | `ழளறஞ` |
| `FixedTextTeluguFallbackSample` | Telugu Post Representative. |

Indicate the letters that represent the font and set of characters you use in the product, and set it before you first call the font. Sample is a value for estimating the cell width and assisted Line Metric, not the real output string.

## 11. Common imports

| Setting | Default | Explanation and limitation. |
|---|---:|---|
| `ImportMergeDistance` | `0.001` | The maximum distance to connect the end points of DXF, DWG, HPGL, PLT Path. DXF/DWG is the original coordinate unit, HPGL/PLT is applied after mm conversion. `0` connects only the exact matching end points. Sound, NaN, Infinity are ignored. |
| `IsImportColorPreserved` | `false` | If `true` keeps the DXF, DWG, Gerber original colour. If `false` changes RGB distance to the closest `EntityPenColors` colour and helps the Pen connection. |

Growing the Merge Distance may make the near separate Contour mistaken. check the coordinate unit and the minimum shaped interval of the real file and then adjust it.

## 12. DXF and DWG

| Setting | Default | Explanation |
|---|---:|---|
| `DxfSplineToPolygonalCounts` | `6` | DXF Spline to Polyline |
| `DxfTextDefaultFont` | `Arial` | Alternative fonts used when importing DXF/DWG Text |
| `IsDxfWithUniformGroup` | `true` | Connect the same Primitive to `EntityUniformGroup` to determine whether to increase Rendering efficiency |
| `ODAConverterPath` | Automatic search results. | It is the ODA File Converter path to convert DWG or DXF version. It is only read and found in the Registry and standard installation folder. If not found, it may be `null`. |

ODA File Converter is a separate installation program. There is a `ODAConverterPath` value, so not all DWG versions and files are converted normally, so check the conversion results and logs.

## 13. 3D Mesh

| Setting | Default | Explanation |
|---|---:|---|
| `GridCloudInterval` | `0.5` | Create Grid Cloud in 3D Mesh |

The smaller the intervals, the number of samples, the memory use and the calculation time will increase. Select based on the model size and the required Z resolution.

## 14. Gerber

| Setting | Default | Explanation |
|---|---:|---|
| `IsGerberPrecombinePolygons` | `false` | Decide whether to Union/Merge in the transversal or transversal Polygon import phase. using it, you can give data, but the processing time will increase. |
| `IsGerberTessellation` | `false` | Fill closed regions by tessellating them into triangles. |
| `IsGerberWithUniformGroup` | `true` | Decide whether to bind the same Primitive to `EntityUniformGroup` for fast Rendering. |

Precombine and Tessellation vary greatly in time depending on the file size and the form complexity. check the import speed, display, Hatch and the real processing path together.

## 15. Editor Shortcut Settings

### Movement Increments

| Setting | Default | Movement increment |
|---|---:|---|
| `KeyboardTransitXYCtrl` | `1` mm | `Ctrl` + direction key |
| `KeyboardTransitXYCtrlAlt` | `0.1` mm | `Ctrl` + `Alt` + direction key |
| `KeyboardTransitXYCtrlAltShift` | `0.01` mm | `Ctrl` + `Alt` + `Shift` + direction key |

### Rotation Increment

| Setting | Default | Rotation increment |
|---|---:|---|
| `KeyboardRotateCtrl` | `90`° | `Ctrl` + `[` or `]` |
| `KeyboardRotateCtrlAlt` | `10`° | `Ctrl` + `Alt` + `[` or `]` |
| `KeyboardRotateCtrlAltShift` | `1`° | `Ctrl` + `Alt` + `Shift` + `[` or `]` |

### Execution Keys

| Setting | Default | Action |
|---|---:|---|
| `KeyboardSimulationStart` | `F1` | Start simulation; use `Ctrl` or `Ctrl+Alt` to change speed and `Esc` to stop |
| `KeyboardShowScript` | `F2` | Show the Script Objects in PropertyGrid |
| `KeyboardMarkerPreview` | `F4` | Scanner Preview |
| `KeyboardMarkerStart` | `F5` | Current Page's Real Marker Start |
| `IsShowMessageBoxWhenMarkerStart` | `true` | Marker Start check window before running. |
| `KeyboardMarkerStop` | `F6` | Marker Stop |
| `KeyboardMarkerReset` | `F8` | Marker Reset |
| `KeyboardHelpMessage` | Generated text | Read-only help showing the current movement and rotation increments and execution keys |

When TreeView has focus, arrow-key combinations navigate nodes. `KeyboardMarkerStart` is not a virtual command; it can start real hardware marking. Configure authorization, interlocks, and the equipment emergency-stop procedure before disabling the confirmation dialog.

## 16. ZPL

| Setting | Default | Explanation |
|---|---|---|
| `ZPLService` | `ZPLServices.BinaryKits` | Offline BinaryKits or Network Labelary Rendering |
| `ZPLBinaryKitsDefaultFont` | `Arial Narrow;Arial;Helvetica` | ZPL Font Identifier Local Font Candidate Order for `0` |
| `ZPLBinaryKitsFonts` | Identification of the Dictionary | `K`, `1`, `A` and others connect the ZPL Font ID and Printer Font name to the local Font candidate |
| `ZPLLabelaryAPIURIFormat` | Labelary API URI format | Labelary request address Template |

The candidate fonts are divided into `;`, `|`, `,` and use the first Fonts installed. When you choose Labelary, you need a network and ZPL data will be transferred to external services, so first check the product's security and network policy.

In the public `editor_zpl` demo, you can check the default font and individual candidate settings.

## 17. Marker

| Setting | Default | Explanation |
|---|---:|---|
| `MarkPreviewRepeats` | `50` | Scanner Preview Route Repeat |
| `MarkPreviewSpeed` | `1,000` mm/s | Jump/Mark speed used for preview |
| `IsMarkArcsIntoLines` | `true` | The Arc-related routes are processed as `ListMarkTo` linear in the `MinStepDistance` standard. `false` uses `ListArcTo` in the Support Scanner. |

Preview is different from laser output processing, but the scanner can actually move. Start with a sufficiently low speed and a safe number of repetitions. Arc command support and accuracy will verify the implementation of the connected RTC/Scanner.

## 18. MoF Extension

| Setting | Default | Explanation |
|---|---:|---|
| `MoFExtMcBSPFrequency` | `8,000,000` Hz | The McBSP frequency to request from MoF Extension |

Make sure the connected RTC, MoF Extension devices and Firmware support the same communication conditions. Only frequency changes will not automatically adjust the line, Clock Source or device settings.

## 19. Stepper

| Setting | Default | Explanation |
|---|---:|---|
| `StepperReferenceRunTimeOut` | `30` seconds | Stepper Reference Run finished waiting time, UI range 1 to 120 seconds |

Timeout is not the same as the device protection function that forced Motor to stop safety. Configure the Limit Sensor, Reference direction, Movement range and Emergency Stop separately on the device side. Public `editor_steppermotor` demo uses this value to wait for Reference Run.

## 20. Scanner Jog

| Setting | Default | Explanation |
|---|---:|---|
| `ScannerJogDistance` | `5` mm | Distance of one scanner jog; UI range 0.1 to 100 mm |

Verify from a small value that does not exceed the validity area of the correction field and optical system. Jog can move the real scanner.

## 21. Remote Protocol

| Setting | Default | Explanation |
|---|---|---|
| `RemoteSeparator` | `|` | Difference between order and receipt. |
| `RemoteTerminator` | `;` | The end of the order. |
| `RemoteOk` | `OK` | Successful response. |
| `RemoteNG` | `NG` | failure response. |
| `RemoteReady` | `Ready` | Ready to respond. |
| `RemoteNotReady` | `NotReady` | Unprepared response. |
| `RemoteBusy` | `Busy` | Response in operation. |
| `RemoteError` | `Error` | The error response. |

If you change the divider and response string, change the transmission side and the receiver side together. The recipient buffer can follow several commands or one command can be divided, so you assemble the full Frame according to the Terminator standard and then Parsing.

## 22. WinForms Custom UI Event

| Event | Returns | Purpose |
|---|---|---|
| `OnCreateLaserUI` | `Control` | Insert Control for specific `ILaser` in the Editor's Laser Tab |
| `OnCreateScannerUI` | `Control` | Enter Control for specific `IScanner` in the Scanner Tab |
| `OnCreateMarkerUI` | `Control` | Enter Control for specific `IMarker` in the Marker Tab |
| `OnScannerFieldCorrection2DShow` | `RtcCorrection2D` | External Vision Failure Data Created 2D Correction Structure Correction to Form |
| `OnScannerFieldCorrection2DApply` | `bool` | The created 2D correction file is directly applied by the user. after completing the application, `true` returns |
| `OnScannerFieldCorrection3DShow` | No | 3D Correction UI Display Action Extension |
| `OnCreateGrids` | `IEntity` | Create Grid Entity with the input value of the Grid Form |

```csharp
using System.Windows.Forms;
using SpiralLab.Sirius3.Laser;
using UIConfig = SpiralLab.Sirius3.UI.Config;

UIConfig.OnCreateLaserUI += CreateLaserControl;

static Control CreateLaserControl(ILaser laser)
{
    return new MyLaserControl
    {
        LaserSource = laser
    };
}
```

The event is registered once before you create the Editor, and disable the same Handler when it is no longer used. The Control created by the Handler must follow the UI Thread rules, and do not run long device communications or file processing directly from the UI Thread. When you disable the Control, the event subscription and Timer will also be arranged together.

In the public `editor_laser_ui` demo, you can see how to customize the Laser UI connection using `OnCreateLaserUI`.

## 23. Application Order Example

```csharp
using CoreConfig = SpiralLab.Sirius3.Config;
using UIConfig = SpiralLab.Sirius3.UI.Config;

bool coreInitialized = false;
try
{
    // 1.Core and UI set up
    CoreConfig.LogPath = @"D:\SiriusData\Logs";
    UIConfig.SiriusFontPath = @"D:\SiriusData\Fonts";
    UIConfig.UnReDoSize = 50;
    UIConfig.ImportMergeDistance = 0.001;
    UIConfig.IsImportColorPreserved = false;
    UIConfig.MarkPreviewSpeed = 500;
    UIConfig.IsShowMessageBoxWhenMarkerStart = true;

    // Custom Factory/Event Registration
    UIConfig.OnCreateEntityPen += CreateEntityPen;
    UIConfig.OnCreateLaserUI += CreateLaserControl;

    // The Core Initiation.
    coreInitialized = SpiralLab.Sirius3.Core.Initialize();
    if (!coreInitialized)
        throw new InvalidOperationException("Sirius3 initialization failed.");

    // Create the device and document and register to EditorControl
}
finally
{
    UIConfig.OnCreateEntityPen -= CreateEntityPen;
    UIConfig.OnCreateLaserUI -= CreateLaserControl;

    // Install the device, document, and control first.
    if (coreInitialized)
        SpiralLab.Sirius3.Core.Cleanup();
}
```

When a product reads Config values from `config*.ini`, parse and validate ranges first, then apply the values in the order above. Log the final applied values at startup so behavior on customer equipment can be reproduced.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
