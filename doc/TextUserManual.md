# Text Entities User Manual

> Reference version: Sirius3 1.16.0 (public Release features)

[English](TextUserManual.md) | [한국어](TextUserManual.koKR.md)

## 1. Choose the text representation

All five text families use the common document, transforms, pens and `ITextConvertible` workflow. Their font sources and machining geometry differ.

| Entity | Font source and output | Fill and factory |
|---|---|---|
| `EntityText` | Installed Windows font; straight vector glyph outlines | `IHatchable`; `CreateText` |
| `EntitySiriusText` | Sirius font file; straight stroke/polyline or DOT point geometry | `IHatchable` for closed outlines; `CreateSiriusText` |
| `EntityImageText` | Installed Windows font rendered to a bitmap | Raster/pixel processing; `CreateImageText` |
| `EntityCircularText` | Installed Windows font; vector glyphs arranged around a circle | `IHatchable`; `CreateCircularText` |
| `EntityCircularSiriusText` | Sirius font file; glyph paths/points arranged around a circle | Closed-outline hatching where available; `CreateCircularSiriusText` |

Names such as Text and SiriusText in the editor refer to these `Entity*` classes. `EntityFactory` creates unattached, caller-owned entities. Configure and regenerate them, then add them through `document.ActAdd(entity)` for document ownership and Undo/Redo. An unattached instance must be disposed by its creator.

## 2. Source text and dynamic conversion

`SourceText` is the input string. With conversion disabled, ordinary text editing updates the generated text. With `IsAllowConvert = true`, `TextConverter` resolves `ConvertedText` through Event, SimpleScript, File, Offset or Link during marking.

Conversion-enabled marking uses a private entity clone so each invocation keeps its resolved text and prepared geometry. Callbacks receive the original object and may execute concurrently for independent markers. A displayed `ConvertedText` is the latest display value, not proof of successful machining. Do not assign it manually while conversion is enabled.

File conversion consumes input lines, and Link reads the named master's current converted value; arrange master/slave order explicitly. See [TextConverters](TextConvert.md) for conversion APIs and failure handling.

## 3. Windows-font vector text

`EntityText` uses `FontName`, `FontStyle` and `CapHeight`. CapHeight is a font reference height in millimeters: font ascent is mapped to it, so the visible uppercase outline height can differ between fonts. Font availability and glyph coverage on the target PC affect the output.

| Setting | Meaning |
|---|---|
| `FontHorizontalAlignment` / `FontVerticalAlignment` | Alignment within the text layout; separate from entity/group transforms. |
| `LetterSpace` | `Auto`, `Variable` or `Fixed`. |
| `TrackingFactor` | Auto-mode spacing between shaped text-cluster origins; 1 is unchanged, supported range 0.75–4. Glyphs are not stretched by tracking. |
| `LetterSpacing` | Additional character gap in mm for Variable; gap after the fixed character width in Fixed. Auto ignores it. |
| `WordSpacing` / `LineSpacing` | Word spacing and additional line gap, in mm. |
| `TargetWidth` | Total target width for Auto/Variable; 0 uses automatic width. Fixed ignores it. |
| `IsDistributed` | In Auto mode, expands advancing cluster gaps toward positive TargetWidth without stretching glyphs. |

Distribution needs at least two advancing clusters and never compresses text. A text already wider than the requested distribution width is not forced to become narrower. Tracking/distributed text is limited to 10,000 UTF-16 code units. Ordinary TargetWidth fitting can scale width when distribution is disabled.

## 4. Fixed character positions and glyph width

| Setting | Meaning |
|---|---|
| `FixedGlyphWidth` | Character-position width in mm. At 0, configured font/script metric samples determine an automatic width. |
| `FixedGlyphMaxWidth` | Read-only natural maximum glyph width from the most recent successful Fixed regeneration; not the requested cell width. |
| `IsGlyphWidthFit` | Horizontally fits each drawable glyph to the fixed width. Default false preserves natural glyph proportions. |

Fixed spacing keeps character positions stable, including spaces and missing glyph positions. Without glyph fitting, a narrow assigned character width does not guarantee that a naturally wide glyph stays inside that width. Fixed ignores TargetWidth. Check regenerated bounds rather than equating character count with visible outline width.

```csharp
using System.Drawing;
using SpiralLab.Sirius3.Entity;

using (var text = EntityFactory.CreateText("Arial", FontStyle.Regular, "LOT 001", 5))
{
    text.LetterSpace = EntityText.LetterSpaces.Fixed;
    text.FixedGlyphWidth = 3;
    text.IsGlyphWidthFit = true;
    if (!text.Regen())
        throw new System.InvalidOperationException("Text regeneration failed.");
}
```

This is a standalone configuration example: its using scope owns the entity. When adding it to a document, transfer ownership instead of disposing it at the end of that scope.

## 5. Sirius font text

`FontName` selects a file under `UI.Config.SiriusFontPath`. CXF/LFF provide vector glyph paths; DOT provides supported point glyphs. EO `.fnt` support is included in normal builds and can be disabled in a custom library build. Check the font's supported characters and actual path closure; a stroke font does not automatically define filled glyph interiors.

Sirius text supports `LetterSpaces.Variable` and `Fixed`. In Variable mode, `IsAutoKerning` is enabled by default for new objects and derives spacing from cached glyph geometry, including the distance between final scaled outline segments. `LetterSpacing` remains the requested gap in output millimeters. Fixed positions do not use auto kerning. `TargetWidth` applies to Variable only.

`FixedGlyphWidth`, `FixedGlyphMaxWidth` and `IsGlyphWidthFit` provide the corresponding fixed-position behavior. `DotFactor` adjusts DOT-font point generation; these points use pixel/dot pen conditions rather than closed-region fill.

```csharp
using (var text = EntityFactory.CreateSiriusText(
    "romans2.cxf", EntitySiriusText.LetterSpaces.Variable, "SIRIUS3", 5))
{
    text.IsAutoKerning = true;
    text.LetterSpacing = 0.2;
    if (!text.Regen())
        throw new System.InvalidOperationException("Check the Sirius font file and glyphs.");
}
```

The font file must exist at the configured path; the factory does not install it.

## 6. Circular text

`Radius` is the positive baseline radius in mm. `BaseAngle` is the reference angle in degrees used with `FontHorizontalAlignment`. `Direction` uses the class's `TextCircularDirections.ClockWise` or `CounterClockWise` enum.

Each newline starts another concentric text line. The first line retains Radius; later lines advance toward the glyph side, outward for clockwise and inward for counter-clockwise text. `LineSpacing` adds radial gap. Ensure inner lines still have a usable positive radius.

Windows-font circular text has Auto/Variable/Fixed spacing. Circular Sirius text has Variable/Fixed and Sirius-font auto kerning. Each class declares its own spacing/direction enum; use the enum belonging to that class.

```csharp
using (var text = EntityFactory.CreateCircularText(
    "Arial", System.Drawing.FontStyle.Regular,
    EntityCircularText.TextCircularDirections.ClockWise,
    20, 90, "SIRIUS3", 3))
{
    text.ObliqueAngle = 10;
    if (!text.Regen())
        throw new System.InvalidOperationException("Circular text regeneration failed.");
}
```

`ObliqueAngle` adds glyph slant. Straight text and ImageText slant their generated text while retaining their alignment workflow; circular variants slant each glyph along its local tangent rather than shearing the whole circle. Circular slant accepts -85° through 85°.

## 7. ImageText: pixels and physical size

ImageText separates bitmap layout from machining dimensions.

| Pixel setting | Physical/output setting |
|---|---|
| `CapHeightPixels` | Font reference height used to render the bitmap; distinct from physical Height. |
| `TargetWidthPixels` | Pixel layout width in Auto/Variable; ignored by Fixed. |
| `FixedGlyphWidthPixels` / `IsGlyphWidthFit` | Fixed pixel character positions and optional glyph-width fitting. |
| `LetterSpacing`, `WordSpacing`, `LineSpacing` | Pixel spacing values; do not copy mm values from vector text directly. |
| `Width`, `Height` | Millimeter output dimensions. Width 0 derives width from Height and layout; positive Width is independent of Height. |
| `IsFill`, `OutlinePixelSize` | Filled bitmap glyphs or rasterized outline thickness in pixels. |
| `MajorColor` | Selects the image's major color for raster/pixel processing. |

ImageText has no IsFixedAspectRatio property. Its logical layout and text margins are distinct from the opaque-pixel bounds. Raising pixel resolution changes sampling and data volume; it does not itself increase physical Height.

```csharp
using (var text = EntityFactory.CreateImageText(
    "Arial", System.Drawing.FontStyle.Regular, true, "SIRIUS3",
    capPixels: 64, outlinePixels: 1, height: 5, width: 0))
{
    if (!text.Regen())
        throw new System.InvalidOperationException("ImageText regeneration failed.");
}
```

ImageText marking uses the associated EntityPen's `RasterMode`, `RasterDirection`, `IsRasterZigZag`, `PixelTime`, `PixelPulses` and related settings. It does not become a vector hatch by enabling a text fill option. See [pens](PenUserManual.md).

## 8. Hatching, clipping and transforms

Windows outline text can be filled with HatchLine/HatchPolygon. Sirius text needs valid closed contours for fill; stroke/DOT fonts can remain line/point machining paths. `HatchMarkOption` selects boundary/fill order and HatchOnly suppresses outline marking. See [hatching](HatchUserManual.md).

Text geometry, character layout and hatch results regenerate together. Original/local dimensions differ from model-transformed and accumulated world dimensions. Parent transforms and marker offsets affect the final machining coordinates; camera zoom does not change geometry.

The text families implement IClippable. Outline text produces independent clipped vector geometry, while ImageText uses raster clipping that retains sample positions. Document ActClip adopts results with Undo/Redo; it does not turn every result back into dynamically converted text. See [clipping](ClippingUserManual.md).

## 9. Editing and checking results

Create the intended text type in the editor, select it, then use PropertyGrid for content, font, spacing, alignment, hatch or raster settings. Use document actions for edits and regenerate after changing settings programmatically. Check return values and logs for missing fonts, unsupported glyphs or invalid geometry.

Inspect spaces, missing characters, multiline layout, circle direction, fixed positions, final bounds, fill holes and the selected pen. Save/reopen preserves text parameters while generated geometry is rebuilt. Dispose caller-owned standalone entities and keep document-owned entities alive until document cleanup.

Public examples: [editor_entity](../demos/editor_entity/Form1.cs), [editor_barcode_textconvert](../demos/editor_barcode_textconvert/Form1.cs). Related guides: [entities](EntityUserManual.md), [barcodes](BarcodeUserManual.md), [WinForms](WinForms.md), [WPF](WPF.md).

---
2026 Copyright (c) SpiralLAB. All rights reserved.
