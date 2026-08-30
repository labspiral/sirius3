# EntityFactory & Entity User Manual

> Reference version: Sirius3 1.12.3 (public Release features)

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

### Fixed Spacing in Sirius3 1.12.3

`EntityText`, `EntitySiriusText`, Circular Variation and `EntityImageText` place all the characters, including empty and missing glyphs, in the fixed intervals in the same width cells.

- `FixedGlyphWidth`: Vector Text's letter cell width
- `FixedGlyphWidthPixels`: ImageText’s text cell width (pixel)
- If the value is 0 you select the automatic width in the setup sample of the detected text rights.
- `IsGlyphWidthFit = false`: Keeping the glyph crossover
- `IsGlyphWidthFit = true`: customize the graph that can be drawn to the cell width
- Fixed does not apply `TargetWidth`/`TargetWidthPixels`, `WordSpacing`, Auto Kerning

`EntityImageText` preserves the physical character-cell size in 1.12.3 while eliminating top-to-bottom transparent artifacts.

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

In addition to the default Mesh and STL/OBJ/PLY/STP·STEP imports, the following models and Factory have been added to Sirius3 1.12.3.

| The object. | Factory | Key entry. |
|---|---|---|
| `EntityPlane` | `CreatePlane` | center, law line, width, height |
| `EntityPyramid` | `CreatePyramid` | Basic point, width, depth, height |
| `EntityTorus` | `CreateTorus` | Central, big ring, small ring, can be split |
| `EntityNURBSSurface` | `CreateNURBS3D` | Control Points, Degree, Knot, Sampling |

These objects can also be added in the Editor's 3D Object Generation menu. Mesh is a visualized surface, so real processing requires a Slice or route conversion.

## 8. Layer, Group, Block

- Layer: A run unit that connects objects and layer conditions within Page
- MixedGroup: Move, rotate, or scale different entity types together
- UniformGroup: Connecting mass objects of the same rendering structure to optimize performance
- Block / BlockInsert: See one master shape in multiple locations

Check the coordinate conversion one-step in so that the `ModelMatrix` and Offset/MatrixStack of the Marker are not duplicated.

## 9. Importing External Files

You can import DXF/DWG, HPGL/PLT, Gerber/Excellon, G-code/NGC, images and 3D files. `UI.Config.ImportMergeDistance` is a common permissible distance to connect the close end points of DXF, DWG, HPGL, PLT, and `UI.Config.IsImportColorPreserved` will change the original color to the closest EntityPen color to keep the original color.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
