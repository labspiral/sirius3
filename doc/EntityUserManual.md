# EntityFactory & Entity User Manual

> API reference: Sirius3 Core/UI 1.17.0 (public features)

[English](EntityUserManual.md) | [한국어](EntityUserManual.koKR.md) | [All guides](README.md)

## 1. Basic entity creation workflow

Create an entity with `EntityFactory`, then add it to the current Page and Layer with `document.ActAdd(entity)`. Selecting the entity in the editor or TreeView displays the entity and its associated `EntityPen` properties in PropertyGrid. After changing geometry-affecting values such as size, text, or hatch settings, the editing action uses `ActRegen` to rebuild the geometry and rendering buffers.

## 2. Basic Vector Entities

- Point / Points: A single emission point or an array of points
- Line / Lines: Single line and independent line set
- Arc / Ellipse: Around, Around, Around
- Rectangle / Triangle / Cross: Standard geometric shapes
- Polyline2D / Polyline3D: An open or closed path whose vertices are connected in sequence
- Bezier / Catmull-Rom / B-Spline / NURBS: Curve using the control point

Before hatching a closed path, verify that the actual contour is closed and has the correct winding direction. A path that merely looks closed on screen may not define a valid interior region.

## 3. Special Paths

- Trepan: hole processing path with internal, external and rotating water
- Spiral / SpiralClassic: linear path
- Lissajous: Synthesized patterns of cycle exercise
- Grid: generate massive points·cross·cross patterns

## 4. Text

- `EntityText`: Convert Windows fonts to a corner line
- `EntitySiriusText`: CXF, LFF, FNT, DOT and others based on Sirius fonts
- `EntityCircularText`, `EntityCircularSiriusText`: Placing according to the symbol
- `EntityImageText`: Make the text as a Bitmap and then Raster processing

### Fixed Spacing in Sirius3 1.13.0

`EntityText`, `EntitySiriusText`, Circular Variation and `EntityImageText` place all the characters, including empty and missing glyphs, in the fixed intervals in the same width cells.

- `FixedGlyphWidth`: Vector Text's letter cell width
- `FixedGlyphWidthPixels`: ImageText’s text cell width (pixel)
- If the value is 0 you select the automatic width in the setup sample of the detected text rights.
- `IsGlyphWidthFit = false`: Keeping the glyph crossover
- `IsGlyphWidthFit = true`: customize the graph that can be drawn to the cell width
- Fixed does not apply `TargetWidth`/`TargetWidthPixels`, `WordSpacing`, Auto Kerning

`EntityImageText` preserves the physical character-cell size in 1.13.0 while eliminating top-to-bottom transparent artifacts.

## 5. Barcode

- 1D: Code128, Code39, PLESSEY and so on
- 2D: QR, Data Matrix, PDF417, Aztec
- Cell expressions: Outline, Hatch, Dots, etc.

Difference the requested Width/Height and the actual Matrix size. erroneous or empty data records encoding errors without leaving the previous shape. Dots processing uses EntityPen's Raster and Pixel settings.

## 6. Images and Raster

- `EntityImage`: BMP, JPG, PNG etc. Bitmap
- `EntityImageText`: Bitmap Text
- `EntityImageZPL`: Convert ZPL to image
- `EntityStitchedImage`: Camera/Test Scale Image Visualization

The actual method of processing varies depending on EntityPen’s `RasterMode`, `PixelTime`, `PixelPeriod`, `PixelPulses` and direction settings.

## 7. 3D Mesh

In addition to the default Mesh and STL/OBJ/PLY/STP·STEP imports, the following models and Factory have been added to Sirius3 1.13.0.

| The object. | Factory | Key entry. |
|---|---|---|
| `EntityPlane` | `CreatePlane` | center, law line, width, height |
| `EntityPyramid` | `CreatePyramid` | Basic point, width, depth, height |
| `EntityTorus` | `CreateTorus` | Central, big ring, small ring, can be split |
| `EntityNURBSSurface` | `CreateNURBS3D` | Control Points, Degree, Knot, Sampling |

These objects can also be added in the Editor's 3D Object Generation menu. Mesh is a visualized surface, so real processing requires a Slice or route conversion.

### Mesh materials and textures in 1.15.0

- OBJ imports read MTL diffuse colors (`Kd`), diffuse maps (`map_Kd`), UV coordinates, and supported vertex colors. Keep the OBJ, MTL, and referenced images together with valid relative paths when importing.
- PLY imports preserve supported vertex/face RGB colors. A diffuse texture requires supported UV properties and a `comment TextureFile ...` reference to an existing image.
- Valid diffuse maps select `ColorModes.TexturePerVertex`; source colors without textures select `ColorModes.PerVertex`. Geometry without either uses its model color.
- Imported texture image data is retained when saving the Sirius3 document. Missing or undecodable images are reported in the log; check the imported appearance before saving.

These features apply to both WinForms and WPF visualization. Mesh appearance does not automatically assign laser power or create machining paths; use slices/path conversion and EntityPen settings for processing.

## 8. Layer, Group, Block

- Layer: A run unit that connects objects and layer conditions within Page
- MixedGroup: Move, rotate, or scale different entity types together
- UniformGroup: Connecting mass objects of the same rendering structure to optimize performance
- Block / BlockInsert: See one master shape in multiple locations

Check the coordinate conversion one-step in so that the `ModelMatrix` and Offset/MatrixStack of the Marker are not duplicated.

## 9. Importing External Files

You can import DXF/DWG, HPGL/PLT, Gerber/Excellon, G-code/NGC, images and 3D files. `UI.Config.ImportMergeDistance` is a common permissible distance to connect the close end points of DXF, DWG, HPGL, PLT, and `UI.Config.IsImportColorPreserved` will change the original color to the closest EntityPen color to keep the original color.

3MF import retains millimeter units, relative component placement, RGB colors, base materials and diffuse textures from `.3mf` build meshes. SVG imports produce an editable `EntitySvg`/`EntityContour` hierarchy. See [import requirements](DocumentUserManual.md) and [SVG settings](Sirius3UIConfigUserManual.md). Independent results from `IClippable.Clip` must be adopted or disposed by the caller; use [ActClip](ClippingUserManual.md) for document edits with Undo/Redo.

## 10. Complete EntityFactory catalog

The current public factory provides **85 method names and 125 overloads**. Creation does not insert entities into a document or start processing. Own unattached results until transferring them through a document action such as `document.ActAdd`. Control entities require the corresponding scanner capability.

### Primitives

| Method | Return type | Overloads |
|---|---|---|
| `CreateLine` | `EntityLine` | 4 |
| `CreateLines` | `EntityLines` | 2 |
| `CreatePoint` | `EntityPoint` | 4 |
| `CreatePoints` | `EntityPoints` | 2 |
| `CreateArc` | `EntityArc` | 4 |
| `CreateEllipse` | `EntityEllipse` | 2 |
| `CreateRectangle` | `EntityRectangle` | 4 |
| `CreateTriangle` | `EntityTriangle` | 4 |
| `CreateCross` | `EntityCross` | 4 |
| `CreateTrepan` | `EntityTrepan` | 4 |
| `CreateSpiral` | `EntitySpiral` | 4 |
| `CreateSpiralClassic` | `EntitySpiralClassic` | 4 |
| `CreateLissajous` | `EntityLissajous` | 1 |

### Polylines and splines

| Method | Return type | Overloads |
|---|---|---|
| `CreatePolyline3D` | `EntityPolyline3D` | 1 |
| `CreatePolyline2D` | `EntityPolyline2D` | 1 |
| `CreateBezierSpline` | `EntityBezierSpline` | 2 |
| `CreateCatmullRomSpline` | `EntityCatmullRomSpline` | 1 |
| `CreateHermiteSpline` | `EntityHermiteSpline` | 1 |
| `CreateBSpline` | `EntityBSpline` | 1 |
| `CreateNURBSpline` | `EntityNURBSpline` | 1 |

### Images and text

| Method | Return type | Overloads |
|---|---|---|
| `CreateImage` | `EntityImage` | 2 |
| `CreatePDFImage` | `EntityImagePDF` | 1 |
| `CreateImageZPL` | `EntityImageZPL` | 1 |
| `CreateImageText` | `EntityImageText` | 1 |
| `CreateText` | `EntityText` | 1 |
| `CreateCircularText` | `EntityCircularText` | 1 |
| `CreateCircularSiriusText` | `EntityCircularSiriusText` | 1 |
| `CreateSiriusText` | `EntitySiriusText` | 1 |

### Barcodes

| Method | Return type | Overloads |
|---|---|---|
| `CreateDataMatrix` | `EntityDataMatrix` | 1 |
| `CreateQRCode` | `EntityQRCode` | 1 |
| `CreatePDF417` | `EntityPDF417` | 1 |
| `CreateAztec` | `EntityAztec` | 1 |
| `CreateBarcode` | `EntityBarcode1D` | 1 |
| `CreateBarcodeV2` | `EntityBarcode1D_v2` | 1 |

### Contours, SVG and 3D

| Method | Return type | Overloads |
|---|---|---|
| `CreateContour` | `EntityContour` | 2 |
| `CreateSVG` | `bool` | 1 |
| `CreateMesh` | `bool` | 1 |
| `CreateCube` | `EntityCube` | 1 |
| `CreateCylinder` | `EntityCylinder` | 1 |
| `CreateCone` | `EntityCone` | 2 |
| `CreateSphere` | `EntitySphere` | 1 |
| `CreatePlane` | `EntityPlane` | 2 |
| `CreatePyramid` | `EntityPyramid` | 1 |
| `CreateTorus` | `EntityTorus` | 1 |
| `CreateNURBS3D` | `EntityNURBSSurface` | 1 |
| `CreateGridCloud` | `EntityGrids` | 1 |

### Containers and file import

| Method | Return type | Overloads |
|---|---|---|
| `CreateLayer` | `EntityLayer` | 1 |
| `CreateBlock` | `EntityBlock` | 1 |
| `CreateBlockInsert` | `EntityBlockInsert` | 2 |
| `CreateMixedGroup` | `EntityMixedGroup` | 1 |
| `CreateUniformGroup` | `EntityUniformGroup` | 1 |
| `CreateDxf` | `bool` | 1 |
| `CreateHPGL` | `bool` | 1 |
| `CreateGerber` | `bool` | 1 |
| `CreateGerbers` | `bool` | 1 |

### Timing, measurement and MoF

| Method | Return type | Overloads |
|---|---|---|
| `CreateDelay` | `EntityDelay` | 1 |
| `CreateJumpTo` | `EntityJump` | 2 |
| `CreateMeasurementBegin` | `EntityMeasurementBegin` | 1 |
| `CreateMeasurementEnd` | `EntityMeasurementEnd` | 1 |
| `CreateMoFBegin` | `EntityMoFBegin` | 1 |
| `CreateMoFWait` | `EntityMoFWait` | 2 |
| `CreateMoFWaitRange` | `EntityMoFWaitRange` | 1 |
| `CreateMoFEnd` | `EntityMoFEnd` | 2 |
| `CreateMoFExtBegin` | `EntityMoFExtBegin` | 1 |
| `CreateMoFExtWait1D` | `EntityMoFExtWait1D` | 1 |
| `CreateMoFExtWait2D` | `EntityMoFExtWait2D` | 1 |
| `CreateMoFExtPark` | `EntityMoFExtPark` | 1 |
| `CreateMoFExtParkReturn` | `EntityMoFExtParkReturn` | 1 |
| `CreateMoFExtEnd` | `EntityMoFExtEnd` | 1 |

### Z, correction, power and I/O

| Method | Return type | Overloads |
|---|---|---|
| `CreateZDefocus` | `EntityZDefocus` | 1 |
| `CreateZDelta` | `EntityZDelta` | 1 |
| `CreateSelectCorrectionTable` | `EntitySelectCorrectionTable` | 1 |
| `CreateRampBegin` | `EntityRampBegin` | 1 |
| `CreateRampEnd` | `EntityRampEnd` | 1 |
| `CreatePoD` | `EntityPoD` | 1 |
| `CreateWriteData` | `EntityWriteData` | 1 |
| `CreateWriteDataExt16` | `EntityWriteDataExt16` | 1 |
| `CreateWriteDataExt16Cond` | `EntityWriteDataExt16Cond` | 1 |
| `CreateWaitDataExt16Cond` | `EntityWaitDataExt16Cond` | 1 |
| `CreateWaitDataExt16EdgeCond` | `EntityWaitDataExt16EdgeCond` | 1 |
| `CreateFreeVariable` | `EntityVariable` | 1 |

### Events and stepper control

| Method | Return type | Overloads |
|---|---|---|
| `CreateUserEvent` | `EntityUserEvent` | 1 |
| `CreateBreakPoint` | `EntityBreakPoint` | 2 |
| `CreateStepperMove` | `EntityStepperMove` | 1 |
| `CreateStepperWait` | `EntityStepperWait` | 1 |

`CreateBarcode` returns the legacy `EntityBarcode1D`; `CreateBarcodeV2` returns `EntityBarcode1D_v2`. `CreateImageText` is a raster entity, unlike vector Text/SiriusText. `CreateNURBSpline` creates a curve, whereas `CreateNURBS3D` creates a surface mesh. `CreateGridCloud` creates a grid point cloud.

`CreateSVG`, `CreateMesh`, `CreateDxf`, `CreateHPGL`, `CreateGerber` and `CreateGerbers` return a success Boolean and `out` results. Do not treat them as single-entity return methods. Check failures and logs. Use `IDocument.ActImport` for editor-integrated import.

`CreateContour` supplies region contours; preserve closure, winding and nested inside/outside topology for hatching. Layers/blocks/groups have child placement and lifetime contracts; BlockInsert references a master. Displaying a basic 3D mesh does not automatically generate a laser path.

Pair MoF, Measurement and Ramp Begin/End controls in the intended order. `CreateZDefocus` and `CreateZDelta` represent different Z controls and require appropriate correction/device settings. `CreatePoD` controls output, `CreateFreeVariable` writes RTC variables, `CreateUserEvent` raises marker events, and `CreateBreakPoint` supports controller pause/resume workflows. Stepper Move/Wait requires the stepper capability.

Conditional factories `CreateWriteDataExt16Cond` and `CreateWaitDataExt16Cond` take **LOW mask, HIGH mask**. `inputHightBitMask` retains the existing public spelling. `CreateWaitDataExt16EdgeCond` takes a bit index 0–15 instead of a mask. For example, change DO4 to HIGH if DI0 is HIGH and DI1–3 are LOW:

```csharp
var conditionalOutput = EntityFactory.CreateWriteDataExt16Cond(
    0x000E, 0x0001, 0x0010, SignalEdges.High);
```

Related guides: [hatching](HatchUserManual.md), [text](TextUserManual.md), [barcodes](BarcodeUserManual.md), [digital I/O](DigitalIOUserManual.md), [MoF](MarkingOnTheFlyUserManual.md), [Offset](OffsetUserManual.md).

## 11. Overload reference

Signatures below include argument order, defaults and `out` results. `DVec2`/`DVec3` are aliases for the OpenTK double-precision vectors of the selected framework.

```csharp
EntityLine CreateLine(DVec2 start, DVec2 end);
EntityLine CreateLine(DVec3 start, DVec3 end);
EntityLine CreateLine(double startX, double startY, double endX, double endY);
EntityLine CreateLine(double startX, double startY, double startZ, double endX, double endY, double endZ);
EntityLines CreateLines(IEnumerable<DVec2> vertices);
EntityLines CreateLines(IEnumerable<DVec3> vertices);
EntityPoint CreatePoint(DVec2 location);
EntityPoint CreatePoint(double x, double y);
EntityPoint CreatePoint(DVec3 location);
EntityPoint CreatePoint(double x, double y, double z);
EntityPoints CreatePoints(IEnumerable<DVec2> locations);
EntityPoints CreatePoints(IEnumerable<DVec3> locations);
EntityArc CreateArc(DVec2 center, double radius, double startAngle, double sweepAngle, uint stepAngle = 0);
EntityArc CreateArc(double cx, double cy, double radius, double startAngle, double sweepAngle, uint stepAngle = 0);
EntityArc CreateArc(DVec3 center, double radius, double startAngle, double sweepAngle, uint stepAngle = 0);
EntityArc CreateArc(double cx, double cy, double cz, double radius, double startAngle, double sweepAngle, uint stepAngle = 0);
EntityEllipse CreateEllipse(double cx, double cy, double cz, double major, double minor);
EntityEllipse CreateEllipse(DVec2 center, double major, double minor);
EntityRectangle CreateRectangle(DVec2 center, double width, double height);
EntityRectangle CreateRectangle(double cx, double cy, double width, double height);
EntityRectangle CreateRectangle(DVec3 center, double width, double height);
EntityRectangle CreateRectangle(double cx, double cy, double cz, double width, double height);
EntityTriangle CreateTriangle(DVec2 baseCenter, double baseLength, double height);
EntityTriangle CreateTriangle(double cx, double cy, double cz, double baseLength, double height);
EntityTriangle CreateTriangle(DVec3 baseCenter, double baseLength, double height);
EntityTriangle CreateTriangle(double cx, double cy, double baseLength, double height);
EntityCross CreateCross(double cx, double cy, double width, double height, double thickness);
EntityCross CreateCross(DVec2 center, double width, double height, double thickness);
EntityCross CreateCross(double cx, double cy, double cz, double width, double height, double thickness);
EntityCross CreateCross(DVec3 center, double width, double height, double thickness);
EntityTrepan CreateTrepan(DVec2 center, double innerDiameter, double outterDiameter, int revolutions);
EntityTrepan CreateTrepan(double cx, double cy, double innerDiameter, double outterDiameter, int revolutions);
EntityTrepan CreateTrepan(DVec3 center, double innerDiameter, double outterDiameter, int revolutions);
EntityTrepan CreateTrepan(double cx, double cy, double cz, double innerDiameter, double outterDiameter, int revolutions);
EntitySpiral CreateSpiral(DVec2 center, double diameter, double depth, int turns, EntitySpiral.SpiralTypes type, bool isClosed);
EntitySpiral CreateSpiral(double cx, double cy, double diameter, double depth, int turns, EntitySpiral.SpiralTypes type, bool isClosed);
EntitySpiral CreateSpiral(DVec3 center, double diameter, double depth, int turns, EntitySpiral.SpiralTypes type, bool isClosed);
EntitySpiral CreateSpiral(double cx, double cy, double cz, double diameter, double depth, int turns, EntitySpiral.SpiralTypes type, bool isClosed);
EntitySpiralClassic CreateSpiralClassic(DVec2 center, double startDiameter, double endDiameter, double height, int revolutions, bool isClosed);
EntitySpiralClassic CreateSpiralClassic(double cx, double cy, double startDiameter, double endDiameter, double height, int revolutions, bool isClosed);
EntitySpiralClassic CreateSpiralClassic(DVec3 center, double startDiameter, double endDiameter, double height, int revolutions, bool isClosed);
EntitySpiralClassic CreateSpiralClassic(double cx, double cy, double cz, double startDiameter, double endDiameter, double height, int revolutions, bool isClosed);
EntityLissajous CreateLissajous(DVec3 center, double diameter, double depth, int turns, EntityLissajous.LissajousTypes lissajousType = EntityLissajous.LissajousTypes.π, EntityLissajous.Directions direction = EntityLissajous.Directions.Cw);
EntityPolyline3D CreatePolyline3D(IEnumerable<DVec3> vertices, bool isClosed = false);
EntityPolyline2D CreatePolyline2D(IEnumerable<Vertex2D> vertices, bool isClosed = false);
EntityBezierSpline CreateBezierSpline(IEnumerable<DVec2> controlPoints);
EntityBezierSpline CreateBezierSpline(IEnumerable<DVec3> controlPoints);
EntityCatmullRomSpline CreateCatmullRomSpline(IEnumerable<DVec3> controlPoints, bool isClosed);
EntityHermiteSpline CreateHermiteSpline(IEnumerable<DVec3> controlPoints, IEnumerable<DVec3> tangents, bool isClosed);
EntityBSpline CreateBSpline(IEnumerable<DVec3> controlPoints, IEnumerable<double> knots, int degree, bool isClosed);
EntityNURBSpline CreateNURBSpline(IEnumerable<DVec3> controlPoints, IEnumerable<double> knots, IEnumerable<double> weights, int degree, bool isClosed);
EntityImage CreateImage(string fileName, double height, double width = 0);
EntityImage CreateImage(System.Drawing.Bitmap bitmap, double height, double width = 0);
EntityImagePDF CreatePDFImage(string fileName, int pageNumber, double height, double width = 0, int dpi = 150);
EntityImageZPL CreateImageZPL(double width, double height, string zplText, EntityImageZPL.DotsPerMMs dpmm = EntityImageZPL.DotsPerMMs.Dots8_203DPI);
EntityImageText CreateImageText(string fontName, System.Drawing.FontStyle style, bool isFill, string text, uint capPixels, int outlinePixels, double height, double width = 0);
EntityText CreateText(string fontName, System.Drawing.FontStyle style, string text, double height);
EntityCircularText CreateCircularText(string fontName, System.Drawing.FontStyle style, EntityCircularText.TextCircularDirections direction, double radius, double startAngle, string text, double height);
EntityCircularSiriusText CreateCircularSiriusText(string fontName, EntityCircularSiriusText.LetterSpaces letterSpace, EntityCircularSiriusText.TextCircularDirections direction, double radius, double startAngle, string text, double height);
EntitySiriusText CreateSiriusText(string fontName, EntitySiriusText.LetterSpaces letterSpace, string text, double height);
EntityDataMatrix CreateDataMatrix(string text, EntityBarcode2DBase.Barcode2DCells cell, double width, double height);
EntityQRCode CreateQRCode(string text, EntityBarcode2DBase.Barcode2DCells cell, double width, double height);
EntityPDF417 CreatePDF417(string text, EntityBarcode2DBase.Barcode2DCells cell, double width, double height);
EntityAztec CreateAztec(string text, EntityBarcode2DBase.Barcode2DCells cell, double width, double height);
EntityBarcode1D CreateBarcode(string text, EntityBarcode1D.Barcode1DFormats format, double width, double height);
EntityBarcode1D_v2 CreateBarcodeV2(string text, EntityBarcode1D_v2.Barcode1DCells cell, EntityBarcode1D_v2.Barcode1DFormats format, double width, double height);
EntityContour CreateContour(IEnumerable<IEntity> segments, bool isClosed = false);
EntityContour CreateContour(Contour contour);
bool CreateSVG(string fileName, out EntityMixedGroup entity);
bool CreateMesh(string fileName, out EntityMesh entityMesh);
EntityCube CreateCube(DVec3 center, double width, double height, double depth = 0);
EntityCylinder CreateCylinder(DVec3 start, double radius, double depth, int segments = 24);
EntityCone CreateCone(double radiusReference, double radiusEnd, double length, int segments = 24);
EntityCone CreateCone(DVec3 reference, double radiusReference, double radiusEnd, double length, int segments = 24);
EntitySphere CreateSphere(DVec3 center, double radius, int segments = 24);
EntityPlane CreatePlane(DVec3 center, double width, double height);
EntityPlane CreatePlane(DVec3 center, DVec3 normal, double width, double height);
EntityPyramid CreatePyramid(DVec3 baseCenter, double baseWidth, double baseDepth, double height);
EntityTorus CreateTorus(DVec3 center, double majorRadius, double minorRadius, int majorSegments = 48, int minorSegments = 24);
EntityNURBSSurface CreateNURBS3D(int controlPointRows, int controlPointColumns, IEnumerable<DVec3> controlPoints, IEnumerable<double> knotsU, IEnumerable<double> knotsV, IEnumerable<double> weights, int degreeU, int degreeV, int segmentsU = 32, int segmentsV = 32, bool isClosedU = false, bool isClosedV = false);
EntityGrids CreateGridCloud(int rows, int cols, double interval, IEnumerable<double> zDepths, DVec2 zRange);
EntityLayer CreateLayer(string name);
EntityBlock CreateBlock(string blockName, IEnumerable<IEntity> entities);
EntityBlockInsert CreateBlockInsert(string blockName, string ownerBlockName, DVec3 deltaXyz);
EntityBlockInsert CreateBlockInsert(string blockName, string ownerBlockName, double dx, double dy, double dz = 0);
EntityMixedGroup CreateMixedGroup(string name, IEnumerable<IEntity> entities);
EntityUniformGroup CreateUniformGroup(string name, IEnumerable<IEntity> entities);
bool CreateDxf(string fileName, out EntityMixedGroup dxfEntity);
bool CreateHPGL(string fileName, out EntityMixedGroup hpglEntity);
bool CreateGerber(string fileName, System.Drawing.Color color, out EntityGerber gerberEntity);
bool CreateGerbers(string path, string filter, out EntityGerber[] gerberEntities);
EntityDelay CreateDelay(double mesc);
EntityJump CreateJumpTo(DVec3 jumpTo);
EntityJump CreateJumpTo(double x, double y, double z = 0);
EntityMeasurementBegin CreateMeasurementBegin(double samplingFrequency, IEnumerable<MeasurementChannels> channels, string title = "");
EntityMeasurementEnd CreateMeasurementEnd();
EntityMoFBegin CreateMoFBegin(RtcMoFModes type = RtcMoFModes.XY, bool isEncoderReset = true);
EntityMoFWait CreateMoFWait(RtcEncoders encoder, RtcEncoderWaitConditions condition, double position, bool isApplyMatrixStack = false);
EntityMoFWait CreateMoFWait(RtcEncoderWaitConditions condition, double angle);
EntityMoFWaitRange CreateMoFWaitRange(DVec2 min, DVec2 max);
EntityMoFEnd CreateMoFEnd(DVec2 jumpTo);
EntityMoFEnd CreateMoFEnd(double x, double y);
EntityMoFExtBegin CreateMoFExtBegin(bool isEncoderReset);
EntityMoFExtWait1D CreateMoFExtWait1D(MoFExtWait1DConditions condition, double position, bool keepFlyActive, bool turnLaserOff);
EntityMoFExtWait2D CreateMoFExtWait2D(MoFExtWait2DConditions condition, DVec2 rangeX, DVec2 rangeY, bool keepFlyActive, bool turnLaserOff);
EntityMoFExtPark CreateMoFExtPark(DVec2 position, bool keepFlyActive);
EntityMoFExtParkReturn CreateMoFExtParkReturn();
EntityMoFExtEnd CreateMoFExtEnd();
EntityZDefocus CreateZDefocus(double zDefocus);
EntityZDelta CreateZDelta(double zDelta);
EntitySelectCorrectionTable CreateSelectCorrectionTable(CorrectionTables primaryHead = CorrectionTables.Table1, CorrectionTables secondaryHead = CorrectionTables.None);
EntityRampBegin CreateRampBegin(AutoLaserControlSignals signal, double startingValue);
EntityRampEnd CreateRampEnd();
EntityPoD CreatePoD(double alcPercentage100, double alcMinValue, double alcMaxValue);
EntityWriteData CreateWriteData(ExtensionChannels channel, double data);
EntityWriteDataExt16 CreateWriteDataExt16(uint bitPosition, bool onOff);
EntityWriteDataExt16Cond CreateWriteDataExt16Cond(ushort inputLowBitMask, ushort inputHightBitMask, ushort outputBitMask, SignalEdges outputLevel = SignalEdges.High);
EntityWaitDataExt16Cond CreateWaitDataExt16Cond(ushort inputLowBitMask, ushort inputHightBitMask);
EntityWaitDataExt16EdgeCond CreateWaitDataExt16EdgeCond(uint bitPosition, SignalEdges signalEdge = SignalEdges.High);
EntityVariable CreateFreeVariable(uint no, uint data);
EntityUserEvent CreateUserEvent(string extensionData = "");
EntityBreakPoint CreateBreakPoint(long waitID);
EntityBreakPoint CreateBreakPoint(IEntity entity);
EntityStepperMove CreateStepperMove(StepperAxes axis, StepperMoveTypes type, double position, double velocity);
EntityStepperWait CreateStepperWait(StepperCombinedAxes axis, double msec);
```

## Point display and interactive spirals

New `EntityPoints` instances, including `EntityFactory.CreatePoints`, start with
`PointSize = 2`. This is a display setting; it does not change point coordinates
or the physical laser spot. Explicit sizes in saved entities and clones are retained.

The WinForms/WPF drawing commands create `EntitySpiral` and `EntityLissajous` with center → diameter,
and `EntitySpiralClassic` with center → start diameter → end diameter. The pointer
sets a radius and the readout reports twice that distance as the diameter in mm.
Classic accepts start diameter 0, a positive end diameter and inward/outward growth.
Backspace returns a stage, Escape cancels, and completion produces one Undo/Redo action.

| Drawing command | Other initial parameters |
|---|---|
| Spiral | Archimedean, Turns=5, Depth=2 mm, closed |
| SpiralClassic | Revolutions=10, Depth=1 mm, closed |
| Lissajous | π, Turns=10, Depth=0 mm, Ccw |

Edit these values in PropertyGrid after creation; use Depth=0 for a planar path.
Lissajous creation preserves the source XY ratio by editing native Diameter, with identity ModelScale. Factory calls continue to use the values supplied by the caller.
See [Editor](EditorUserManual.md) for drawing, selection and grip controls.

## Grip editing and PDF images

Supported `IGrippable` entities expose position, native parameters, vertices,
size and rotation according to their geometry. Use Tab/Shift+Tab, V/Shift+V and
G as described in [Editor](EditorUserManual.md). BlockInsert grips change the
placement instance; edit the block master deliberately when all instances must change.

`CreatePDFImage(fileName, pageNumber, height, width = 0, dpi = 150)` creates an
`EntityImagePDF`. Its page is rasterized, while Width/Height remain in mm.
See [PDF import](DocumentUserManual.md).

## Child containers and surface point clouds

`IHasChildren<IEntity>` describes a container such as Group or Gerber. It is not
an additional geometric shape. Document actions keep child edits in Undo/Redo.

| Container | Use |
|---|---|
| `EntityMixedGroup` | Keep different entity types together while preserving individual child behavior. |
| `EntityUniformGroup` | Group entities with compatible rendering structure for efficient repeated drawing. |
| `EntityGerber` | Retain the child geometry of an imported Gerber drawing. |

1. Import an STL or another supported mesh into the document.
2. Use `IDocument.ActPointCloud` to project source sample positions along a
   direction onto the surface, or `ActGridCloud` to sample the surface at a
   specified grid spacing in mm. A smaller spacing produces more points.
3. Inspect the points from an oblique camera view. They describe the sampled
   surface, not a filled volume. `ActGridCloud` also returns normals. The points
   can be passed to `RtcCalibrationLibrary.PointsCloudCalibration`; align units
   and coordinates first. See [3D calibration](Correction3dUserManual.md).

---
2026 Copyright (c) SpiralLAB. All rights reserved.
