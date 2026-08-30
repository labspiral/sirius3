# TextConverters & ITextConvertible Developer Manual

> Reference version: Sirius3 1.12.3 (public Release features)

## 1. Overview

`ITextConvertible` is the common interface for transforming text or barcode data immediately before marking. The `TextConverters` enum selects how the original `SourceText` is converted into `ConvertedText`.

Public `demos/editor_barcode_textconvert` creates Data Matrix and Text and then applies Event, SimpleScript, File, Offset, and Link each.

## 2. Supported Entities

The representative objects implementing `ITextConvertible` are as follows.

- `EntityText`, `EntitySiriusText` and Circular Text
- `EntityImageText`
- `EntityBarcode1D_V2`
- `EntityQRCode`, `EntityDataMatrix`, `EntityPDF417`, `EntityAztec`

Make sure your actual support is converted to `ITextConvertible`.

## 3. ITextConvertible Properties

| Property | Purpose |
|---|---|
| `IsAllowConvert` | `true` only performs dynamic conversion. |
| `TextConverter` | `Event`, `SimpleScript`, `File`, `Offset`, `Link` |
| `SourceText` | Original string, script or default value |
| `ConvertedText` | Conversion results used in real shapes and marks |
| `ExternalFile` | Text file to read in file mode. |
| `LinkEntity` | The entity name link. |

When `IsAllowConvert = true`, the application must not overwrite `ConvertedText` arbitrarily. The Marker updates the value according to the selected conversion method and regenerates the required geometry.

## 4. Conversion Timing and Failure Handling

Marker tries to convert every time the `ITextConvertible` object is processed. If the conversion results differ from the existing `ConvertedText`, then the Text/Barcode form is re-created and then the list is created.

In the following case, record the error and stop the current mark.

- Conversion result is `null` or empty string
- SimpleScript assessment fails
- The next file is no line or empty.
- Offset ExtensionData is empty
- No link or use the link.

Verify the dynamic data with Preview or Virtual Marker before running, and leave the Entity Name and Conversion method in the error log.

## 5. TextConverters.Event

In `IMarker.OnTextConvert` you see the current Marker, WorkingSet and the target object and return the string.

```csharp
document.FindByName("MyBarcode", out IEntity barcodeEntity);
var barcode = barcodeEntity as ITextConvertible;
barcode.IsAllowConvert = true;
barcode.TextConverter = TextConverters.Event;

marker.OnTextConvert -= Marker_OnTextConvert;
marker.OnTextConvert += Marker_OnTextConvert;

string Marker_OnTextConvert(IMarker activeMarker, ITextConvertible target)
{
    var entity = (IEntity)target;
    var offsetIndex = activeMarker.WorkingSet.OffsetIndex;

    return entity.Name == "MyBarcode"
        ? $"LOT-{offsetIndex:D3}-{DateTime.Now:HHmmss}"
        : target.SourceText;
}
```

This event can be called from the Marker's non-motion workflow. Do not touch the UI Control directly, do not register multiple Handler in one Marker. If there are multiple Handler, a warning will be recorded and the final return value will result.

## 6. TextConverters.SimpleScript

Evaluate `SourceText` as C# Script in `Marker.ScriptInstance`.

```csharp
document.FindByName("MyText", out IEntity textEntity);
var text = textEntity as ITextConvertible;
text.IsAllowConvert = true;
text.TextConverter = TextConverters.SimpleScript;
text.SourceText = @"Time(""HH:mm:ss"")";
```

The series yes:

```csharp
barcode.SourceText = @"NextSerialNo(1)";
```

A lot of yes:

```csharp
barcode.SourceText = @"
string date = Date(""yyMMdd"");
string serial = NextSerialNo(""D5"");
string shift = Shift(""A"", ""B"", ""C"");
return $""{date}-{serial}-{shift}"";
";
```

Script files and `IScript` implementation please see `ScriptUserManual.md` and Public `demos/editor_script`.

## 7. TextConverters.File

Read one line by one from the first line of `ExternalFile` The lines used for real marking are removed from the file, so separate the original file from the Queue file for production.

```csharp
document.FindByName("MyBarcode", out IEntity barcodeEntity);
var barcode = barcodeEntity as ITextConvertible;
barcode.IsAllowConvert = true;
barcode.TextConverter = TextConverters.File;
barcode.ExternalFile = Path.Combine(
    AppDomain.CurrentDomain.BaseDirectory,
    "test.txt");
```

When two objects share a file at the same time, each object consumes a line separately. to display the same data in Barcode and Text, one is made as Event/File Master and the other is safe to use the Link.

## 8. TextConverters.Offset

Currently I use `Marker.WorkingSet.Offset.ExtensionData`.

Single value:

```csharp
marker.Offsets = new[]
{
    new Offset(-10, 0) { ExtensionData = "LEFT" },
    new Offset(+10, 0) { ExtensionData = "RIGHT" }
};
```

Individual values:

```csharp
barcode.Name = "MyBarcode";
text.Name = "MyText";
barcode.TextConverter = TextConverters.Offset;
text.TextConverter = TextConverters.Offset;
barcode.IsAllowConvert = text.IsAllowConvert = true;

marker.Offsets = new[]
{
    new Offset(-10, 0)
    {
        ExtensionData = "MyBarcode|DM-001|MyText|TEXT-001"
    },
    new Offset(+10, 0)
    {
        ExtensionData = "MyBarcode|DM-002|MyText|TEXT-002"
    }
};
```

The default distinctor is `|` in `UI.Config.RemoteSeparator`. The conversion order may vary depending on LayerFirst/OffsetFirst, so check the actual order with the OffsetIndex in WorkingSet.

## 9. TextConverters.Link

The current `ConvertedText` of other `ITextConvertible` objects is imported as Entity Name. It is useful when keeping the Barcode and the Text that a person reads with the same value.

```csharp
document.FindByName("MyBarcode", out IEntity barcodeEntity);
var barcode = barcodeEntity as ITextConvertible;
barcode.IsAllowConvert = true;
barcode.TextConverter = TextConverters.Event;

document.FindByName("MyText", out IEntity textEntity);
var text = textEntity as ITextConvertible;
text.IsAllowConvert = true;
text.TextConverter = TextConverters.Link;
text.LinkEntity = "MyBarcode";
```

The link object must be converted earlier than the link object in the marking order, otherwise the previous value or empty value may be used. **Link → Link chain will record as a error from Sirius3 1.12.1 and stop marking.**

When multiple markers use the same Document, the Link does not read the latest `ConvertedText` throughout the Document at the time of viewing, but not a Snapshot by markers. The string and shape of the already created processing Clone will remain, even if the Master value changes.

## 10. Demo Execution Flow

The flow of `demos/editor_barcode_textconvert/Form1.cs` is as follows.

1. Create a virtual or INI setup device with `EditorHelper.CreateDevices`.
2. Register the device with `SiriusEditorControl.RegisterDevices`.
3. `Marker.Ready` Connect Document, View, RTC, Laser and PowerMeter.
4. Add `CreateDataMatrix` and `CreateText` to `MyBarcode`, `MyText`.
5. Change the TextConverter and auxiliary properties on each button.
6. Repair PropertyGrid and check the results with Marker Preview/Start.

When closing, disable the Marker/Device, Document and call `Core.Cleanup()`.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
