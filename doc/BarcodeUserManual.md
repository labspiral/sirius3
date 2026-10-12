# Barcode Entities User Manual

> Reference version: Sirius3 1.16.0 (public Release features)

[English](BarcodeUserManual.md) | [한국어](BarcodeUserManual.koKR.md)

## 1. Classes and factories

`EntityBarcode1D_v2` is the concrete 1D class; the lowercase `v2` is part of its C# name. `EntityBarcode2DBase` is an abstract base that provides common cell geometry, hatching, conversion, rendering and marking for its concrete formats.

| Entity | Creation | Format-specific settings |
|---|---|---|
| `EntityBarcode1D_v2` | `EntityFactory.CreateBarcodeV2` | `Format`, `DotDimension`, `QuiteZone`, GS1/encoding |
| `EntityDataMatrix` | `CreateDataMatrix` | `IsSquare`, minimum `DotDimension`, `IsC40`, `IsCompactEncoding` |
| `EntityQRCode` | `CreateQRCode` | `QrVersion`, L/M/Q/H error correction, output sampling |
| `EntityPDF417` | `CreatePDF417` | `IsCompact`, L0–L8/AUTO error correction, actual matrix size |
| `EntityAztec` | `CreateAztec` | `Layers`, error-correction percentage, output sampling |

The four 2D classes derive from EntityBarcode2DBase. `CreateBarcode` returns the separate older `EntityBarcode1D`; use `CreateBarcodeV2` for the cell-based API described here. Factories create caller-owned, unattached entities. Document insertion uses `ActAdd` and transfers ownership.

## 2. Content, GS1 and encoding

These classes implement `ITextConvertible`. Set `SourceText` for ordinary content, or enable `IsAllowConvert` and a TextConverter for dynamic data. ConvertedText is resolved for the marking invocation; empty/invalid data is logged and must not be treated as a usable barcode. See [TextConverters](TextConvert.md).

`IsAllowEncoding` defaults to false. In that state, the encoder's default character handling is used and the configured Encoding is retained but ignored. Enable it to request a supported explicit encoding, such as UTF-8 where the selected format/runtime supports it. ECI and decoded content can differ with the selected encoding; compare with the intended reader.

`IsGS1Format` enables the format's GS1 encoding policy. The implementation recognizes group-separator notation such as `<GS>` and `(,)`. This option does not validate all application-identifier business rules. Preserve required separators and check the actual decoded data. `QuiteZone` is the exact public property spelling for the quiet-zone margin; it is not a misspelling to change in C# code.

## 3. Width, height and generated matrix dimensions

`Width` and `Height` are persistent maximum machining extents in local millimeters. With IsFixedAspectRatio enabled, the encoded matrix is fitted proportionally inside both limits. Fitting does not rewrite those requested dimensions, so changing symbol shape and regenerating does not progressively shrink the stored request.

Without aspect fitting, independent dimensions can produce different X/Y pitch. Logical square Data Matrix or QR data does not itself establish physically square modules. Quiet zones, matrix dimensions and motif subdivision affect generated geometry. Entity/group transforms also affect world-space extents; inspect the final transformed result.

| Class | Requested versus generated dimension |
|---|---|
| 1D v2 | `DotDimension` requests output dimensions; a zero component allows automatic sizing. `DotDimensionAdjusted` reports the actual matrix. |
| Data Matrix | `DotDimension` is a normalized minimum from the square/rectangular symbol family; content may require a larger symbol. `DotDimensionAdjusted` reports its actual logical size. |
| QR | `DotDimension` requests output sampling, while `QrVersion` and content determine logical modules. Read `DotDimensionAdjusted` for actual output. |
| PDF417 | `DotDimension` is a generated read-only result, not a requested minimum. |
| Aztec | `DotDimension` requests sampling; Layers, content and error correction determine the structural size. Read `DotDimensionAdjusted`. |

Adjusted dimensions exclude additional quiet-zone and cell DotFactor expansion, are not serialized and are zero before generation or after failure. Do not feed them back as the next requested dimensions. Read the properties appropriate to the concrete class.

## 4. Cell modes and machining

Both families declare their own cell enum: `EntityBarcode1D_v2.Barcode1DCells` and `EntityBarcode2DBase.Barcode2DCells`.

| Mode | Generated machining representation |
|---|---|
| `Dots` | Points in active cells; point rendering and marking use the same local coordinates. |
| `Lines` | Line patterns controlled by CellLine direction, subdivision and zigzag settings. |
| `Outline` | Filled-module boundary contours, suitable for region hatching. |
| `Circles` | Circle motifs configured through CellCircle. |
| `Squares` | Square motifs configured through CellSquare. |

`CellDot`, `CellLine`, `CellOutLine`, `CellCircle` and `CellSquare` expose mode-specific settings. DotFactor changes subdivision where supported; circle radius/square scale and traversal settings change motifs. Their settings belong to the receiving barcode. Clone settings for independent objects and regenerate after editing.

Dots are point machining: EntityPen PixelTime/PixelPulses and supported scanner/laser behavior determine exposure. Display PointSize is not the physical laser spot diameter. `IsInvertCell` reverses active/inactive cells and can substantially change the region being machined.

## 5. 1D formats

The v2 format enum includes `CODABAR`, `Code39`, `Code93`, `Code128`, `EAN8`, `EAN13`, `ITF`, `MSI`, `UPCA`, `UPCE`, `UPCEAN_EXT` and `PLESSEY`. Content restrictions and check-digit handling depend on the selected encoder. UPC/EAN extensions are a distinct format; verify main-code and supplemental-code use with the target reader.

New v2 objects default to Code128, Dots, 20 × 4 mm and independent aspect sizing. Choose the representation explicitly for the intended process.

```csharp
using SpiralLab.Sirius3.Entity;
using SpiralLab.Sirius3.Entity.Hatch;

using (var barcode = EntityFactory.CreateBarcodeV2(
    "LOT-0001", EntityBarcode1D_v2.Barcode1DCells.Outline,
    EntityBarcode1D_v2.Barcode1DFormats.Code128, 20, 5))
{
    barcode.IsFixedAspectRatio = false;
    barcode.AddHatch(HatchFactory.CreateLine(90, 0.1, 0.02));
    barcode.HatchMarkOption = HatchMarkOptions.HatchOnly;
    if (!barcode.Regen())
        throw new System.InvalidOperationException("1D barcode regeneration failed.");
}
```

This standalone example owns and disposes its entity. When attaching it to a document, retain it under document ownership instead.

## 6. Data Matrix, QR, PDF417 and Aztec

Data Matrix defaults to the square family. Set IsSquare false for supported rectangular symbols. Minimum-size selection uses discrete supported dimensions, rather than arbitrary logical row/column counts. C40 and compact encoding are mutually exclusive policies; neither is a character-set or error-correction setting.

QR's nullable QrVersion chooses automatic or specified version. Its ErrorCorrectionLevel enum is L, M, Q, H; new objects default to H. Version/content and error correction determine logical modules, independently of requested output sampling.

PDF417 is a stacked symbol. IsCompact changes its encoding representation; ErrorCorrectionLevel uses L0 through L8 or AUTO, which is the default. Its actual DotDimension is available after regeneration.

Aztec Layers 0 selects a suitable count automatically; -1 through -4 force compact layers and 1 through 32 force full layers. ErrorCorrectionLevel defaults to 33 and is clamped to 25–100. A forced size can fail when it cannot hold content and recovery data.

```csharp
using (var barcode = EntityFactory.CreateDataMatrix(
    "SIRIUS3", EntityBarcode2DBase.Barcode2DCells.Outline, 12, 6))
{
    barcode.IsSquare = false;
    barcode.IsFixedAspectRatio = true;
    barcode.AddHatch(HatchFactory.CreateLine(0, 0.1, 0.02));
    barcode.HatchMarkOption = HatchMarkOptions.HatchOnly;
    if (!barcode.Regen())
        throw new System.InvalidOperationException("Data Matrix regeneration failed.");
}
```

```csharp
using (var barcode = EntityFactory.CreateQRCode(
    "SIRIUS3", EntityBarcode2DBase.Barcode2DCells.Outline, 10, 10))
{
    barcode.ErrorCorrectionLevel = EntityQRCode.ErrorCorrectionLevels.M;
    barcode.IsAllowEncoding = true;
    barcode.Encoding = "UTF-8";
    if (!barcode.Regen())
        throw new System.InvalidOperationException("QR regeneration failed.");
}
```

## 7. Hatching, clipping and regeneration

Dots/Lines are open point/line representations. Closed modes expose contour regions where supported. For solid code areas, use Outline and a hatch pattern; HatchOnly suppresses a separately marked boundary. Keep holes, quiet zones and combined touching-cell topology intact. See [hatching](HatchUserManual.md).

Geometry-affecting edits require Regen/ActRegen before rendering, picking or marking the new content. Encoding failures retire old cell geometry, bounds and hatch results while preserving user settings for a later valid value. Inspect the Boolean result and logs instead of relying on the previous visible code.

IClippable supports the generated representation. ActClip can preserve existing hatch paths through IncludeHatches and adds results with Undo/Redo. A clipped portion is geometry; it is not automatically a complete decodable symbol. See [clipping](ClippingUserManual.md).

## 8. Extending the barcode implementation

EntityBarcode2DBase supplies common geometry and interfaces. A concrete implementation overrides `EncodeAndCreateBitMatrix()` for its encoder and implements its cloning behavior. Preserve valid matrix dimensions, content validation, serialized settings, ModifyFlag updates, cell ownership and failure cleanup. Keep source-order, render, contour, picking and marking representations consistent.

Inspect the public DataMatrix/QR/PDF417/Aztec implementations for examples. Extend the entity/encoder layer independently from scanner command execution; use existing marker/pen contracts for output. The common base is abstract and cannot be instantiated directly.

## 9. Editing and checking results

Select the barcode in the editor or TreeView and edit its format, data, dimensions, cells and hatch settings in PropertyGrid. Check static and converted content, quiet-zone margins, physical X/Y proportions, outline/hatch/point modes and selected EntityPen. Validate the actual produced symbol with the application's reader and process conditions.

Public examples: [editor_barcode](../demos/editor_barcode/Form1.cs), [editor_barcode_textconvert](../demos/editor_barcode_textconvert/Form1.cs). Related guides: [entities](EntityUserManual.md), [text](TextUserManual.md), [pens](PenUserManual.md), [PropertyGrid](PropertyGridUserManual.md).

---
2026 Copyright (c) SpiralLAB. All rights reserved.
