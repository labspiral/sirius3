# Marker Offset user manual

> Reference: Sirius3 1.16.0 public APIs and the current implementation.

[English](OffsetUserManual.md) | [한국어](OffsetUserManual.koKR.md)

## 1. Position and orientation for each processing location

`SpiralLab.Sirius3.Mathematics.Offset` is a value type. `marker.Offsets` supplies processing locations without permanently changing document geometry. The marker applies each offset through its transformation stack. `WorkingSet.Offset` and `WorkingSet.OffsetIndex` identify the currently processed entry.

| Member | Role |
|---|---|
| Translate.X / Y | Added X/Y displacement in millimetres. |
| Translate.Z | Added Z displacement in millimetres; requires a compatible 3D processing path. It does not itself move a mechanical stage. |
| AngleZ | Rotation about Z in degrees; positive angles are counterclockwise. |
| Scale | Per-axis scale, initialized to one by the constructors. |
| ExtensionData | Per-location string data, including values for Offset text conversion. It does not affect coordinates. |
| ToMatrix | Matrix property combining Scale → Z rotation → Translation in Sirius3's row-vector convention. |

For example, `new Offset(10, 20, 3, 90, "Part A")` transforms local point `(1, 0, 0)` to `(10, 21, 3)` with unit scale. Entity and parent transforms are applied in addition to the marker offset. Avoid applying the same placement to the entity and again to the offset.

Use `Offset.Zero` or a constructor for an identity offset. `default(Offset)` has zero-initialized Scale and is not the identity transform. When editing an entry, modify its value and assign it back to the array.

## 2. Assign locations

```csharp
using SpiralLab.Sirius3.Mathematics;

Offset[] locations =
{
    new Offset(-10, 0) { ExtensionData = "MyBarcode|OFFSET 11|MyText|OFFSET 12" },
    new Offset(+10, 0) { ExtensionData = "MyBarcode|OFFSET 21|MyText|OFFSET 22" }
};
// Assign locations to the initialized marker while it is idle.
// marker.Offsets = locations;
```

LayerFirst processes the layers at each location before advancing to the next offset. OffsetFirst processes a layer across the offsets before advancing to the next layer. Confirm the selected marker's traversal and event order; `OffsetIndex` identifies the current array entry, not a completed workpiece count. See [Marker](MarkerUserManual.md).

## 3. Select Offset text conversion

Set `IsAllowConvert = true` and `TextConverter = TextConverters.Offset` on text/barcode entities. Conversion reads the current offset's `ExtensionData` during marking. Entity `Name` is the lookup key for name/value data. A preview placeholder or `ConvertedText` alone does not prove that a particular offset has been processed.

The public [editor_barcode_textconvert](../demos/editor_barcode_textconvert/Form1.cs) creates `MyBarcode` and `MyText` with this converter and uses the two locations above:

| Location | MyBarcode | MyText |
|---|---|---|
| X = −10 mm | OFFSET 11 | OFFSET 12 |
| X = +10 mm | OFFSET 21 | OFFSET 22 |

## 4. Plain string and name/value pairs

If the string contains neither the configured separator nor a leading JSON object marker, the complete string is used for every Offset-converted entity. For example, `ExtensionData = "PART-001"` supplies `PART-001` to both entities.

The default `UI.Config.RemoteSeparator` is `|`. With that separator, write alternating names and values:

```text
MyBarcode|DM-001|MyText|TEXT-001
```

The delimiter parser splits using the current separator and removes empty tokens. It requires a nonempty, even token count. It trims each name, preserves value whitespace, compares names without case sensitivity, and uses the last matching pair if a name is repeated.

| ExtensionData | Result for entity MyText |
|---|---|
| `mytext\|A` | `A` |
| ` MyText \| A ` | ` A `, including spaces |
| `MyText\|A\|MyText\|B` | `B` |
| `MyBarcode\|A` | Missing key: conversion fails. |
| `MyText\|A\|extra` | Odd token count: conversion fails. |

Do not use empty delimiter values: removing empty tokens can change pair boundaries. Delimiter strings have no escaping syntax. Use JSON when a value contains the separator. Empty data or an empty/missing converted value makes conversion fail and stops the marking operation.

## 5. JSON data

```json
{"MyBarcode":"DM|001","MyText":"TEXT-001"}
```

A string whose trimmed beginning is `{` is parsed as a JSON string dictionary. JSON key lookup is case-sensitive: `MyText` and `mytext` are different keys, unlike delimiter pairs. Missing keys fail conversion. Invalid JSON falls back to delimiter parsing, so validate JSON beforehand rather than relying on that fallback.

Only the configured separator is active for delimiter pairs; `&` and `;` are not automatic alternatives. Apply the same separator when generating data in external applications.

## 6. Processing and ownership

Prepare and validate offsets before starting. Do not change locations or metadata while a marker is busy. Check marker events, completion status and conversion errors. Offset data is copied as a value; device and document lifetimes remain host-owned.

Related guides: [TextConvert](TextConvert.md), [text](TextUserManual.md), [barcodes](BarcodeUserManual.md), [MoF](MarkingOnTheFlyUserManual.md).

---
2026 Copyright (c) SpiralLAB. All rights reserved.
