# TextConverters & ITextConvertible Developer Manual

> 기준 버전: Sirius3 1.12.3 (공개 Release 기능)

## 1. 개요

`ITextConvertible`은 마킹 직전에 Text나 Barcode 데이터를 바꿀 수 있는 공통 인터페이스입니다. `TextConverters` enum은 원본 `SourceText`를 어떤 방식으로 `ConvertedText`로 만들지 선택합니다.

공개 `demos/editor_barcode_textconvert`는 Data Matrix와 Text를 만든 뒤 Event, SimpleScript, File, Offset, Link를 각각 적용합니다.

## 2. 적용되는 개체

`ITextConvertible`을 구현한 대표 개체는 다음과 같습니다.

- `EntityText`, `EntitySiriusText`와 원형 Text
- `EntityImageText`
- `EntityBarcode1D_V2`
- `EntityQRCode`, `EntityDataMatrix`, `EntityPDF417`, `EntityAztec`

실제 지원 여부는 개체가 `ITextConvertible`로 변환되는지 확인하십시오.

## 3. ITextConvertible 속성

| 속성 | 역할 |
|---|---|
| `IsAllowConvert` | `true`일 때만 동적 변환 실행 |
| `TextConverter` | `Event`, `SimpleScript`, `File`, `Offset`, `Link` 선택 |
| `SourceText` | 원본 문자열, Script 식 또는 기본값 |
| `ConvertedText` | 실제 형상과 마킹에 사용하는 변환 결과 |
| `ExternalFile` | File 방식에서 읽을 Text 파일 |
| `LinkEntity` | Link 방식에서 참조할 Entity Name |

`IsAllowConvert = true`이면 `ConvertedText`를 응용 프로그램이 임의로 덮어쓰지 마십시오. Marker가 선택한 변환 방식에 따라 값을 바꾸고 필요한 형상을 다시 생성합니다.

## 4. 변환 시점과 실패 처리

Marker는 `ITextConvertible` 개체를 가공할 때마다 변환을 시도합니다. 변환 결과가 기존 `ConvertedText`와 다르면 Text/Barcode 형상을 다시 생성한 뒤 리스트를 만듭니다.

다음 경우에는 오류를 기록하고 현재 마킹을 중지합니다.

- 변환 결과가 `null` 또는 빈 문자열
- SimpleScript 평가 실패
- File의 다음 줄이 없거나 빈 줄
- Offset ExtensionData가 비어 있음
- Link 대상이 없거나 대상도 Link를 사용

동적 데이터는 실행 전에 Preview 또는 가상 Marker로 검증하고, 오류 로그에 Entity Name과 변환 방식을 남기십시오.

## 5. TextConverters.Event

`IMarker.OnTextConvert`에서 현재 Marker, WorkingSet과 대상 개체를 보고 문자열을 반환합니다.

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

이 Event는 Marker의 비동기 작업 흐름에서 호출될 수 있습니다. UI Control을 직접 만지지 말고, 하나의 Marker에 여러 Handler를 중복 등록하지 마십시오. 여러 Handler가 있으면 경고가 기록되고 마지막 반환값이 결과가 됩니다.

## 6. TextConverters.SimpleScript

`SourceText`를 `Marker.ScriptInstance`의 C# Script 식으로 평가합니다.

```csharp
document.FindByName("MyText", out IEntity textEntity);
var text = textEntity as ITextConvertible;
text.IsAllowConvert = true;
text.TextConverter = TextConverters.SimpleScript;
text.SourceText = @"Time(""HH:mm:ss"")";
```

Serial 예:

```csharp
barcode.SourceText = @"NextSerialNo(1)";
```

여러 줄 예:

```csharp
barcode.SourceText = @"
string date = Date(""yyMMdd"");
string serial = NextSerialNo(""D5"");
string shift = Shift(""A"", ""B"", ""C"");
return $""{date}-{serial}-{shift}"";
";
```

Script 파일과 `IScript` 구현은 `ScriptUserManual.md` 및 공개 `demos/editor_script`를 참고하십시오.

## 7. TextConverters.File

`ExternalFile`의 첫 줄부터 한 줄씩 읽습니다. 실제 마킹에 사용한 줄은 파일에서 제거되므로 원본 파일과 생산용 Queue 파일을 분리하십시오.

```csharp
document.FindByName("MyBarcode", out IEntity barcodeEntity);
var barcode = barcodeEntity as ITextConvertible;
barcode.IsAllowConvert = true;
barcode.TextConverter = TextConverters.File;
barcode.ExternalFile = Path.Combine(
    AppDomain.CurrentDomain.BaseDirectory,
    "test.txt");
```

File을 두 개체가 동시에 공유하면 각 개체가 줄을 따로 소비합니다. 같은 데이터를 Barcode와 Text에 표시하려면 하나를 Event/File Master로 만들고 다른 하나는 Link를 사용하는 편이 안전합니다.

## 8. TextConverters.Offset

현재 `Marker.WorkingSet.Offset.ExtensionData`를 사용합니다.

단일 값:

```csharp
marker.Offsets = new[]
{
    new Offset(-10, 0) { ExtensionData = "LEFT" },
    new Offset(+10, 0) { ExtensionData = "RIGHT" }
};
```

개체별 값:

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

기본 구분자는 `UI.Config.RemoteSeparator`의 `|`입니다. LayerFirst/OffsetFirst에 따라 변환 순서가 달라질 수 있으므로 WorkingSet의 OffsetIndex로 실제 순서를 확인하십시오.

## 9. TextConverters.Link

다른 `ITextConvertible` 개체의 현재 `ConvertedText`를 Entity Name으로 가져옵니다. Barcode와 사람이 읽는 Text를 같은 값으로 유지할 때 유용합니다.

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

Link 대상은 마킹 순서에서 Link 개체보다 먼저 변환되어야 합니다. 그렇지 않으면 이전 값이나 빈 값이 사용될 수 있습니다. **Link → Link 연쇄는 Sirius3 1.12.1부터 오류로 기록하고 마킹을 중지합니다.**

여러 Marker가 같은 Document를 사용할 때 Link는 Marker별 Snapshot이 아니라 조회 시점의 Document 전역 최신 `ConvertedText`를 읽습니다. 이미 만들어진 가공 Clone의 문자열과 형상은 이후 Master 값이 바뀌어도 유지됩니다.

## 10. 데모 실행 흐름

`demos/editor_barcode_textconvert/Form1.cs`의 흐름은 다음과 같습니다.

1. `EditorHelper.CreateDevices`로 가상 또는 INI 설정 장치를 생성합니다.
2. `SiriusEditorControl.RegisterDevices`로 장치를 등록합니다.
3. `Marker.Ready`로 Document, View, RTC, Laser와 PowerMeter를 연결합니다.
4. `CreateDataMatrix`와 `CreateText`로 `MyBarcode`, `MyText`를 추가합니다.
5. 각 버튼에서 TextConverter와 보조 속성을 바꿉니다.
6. PropertyGrid를 새로 고치고 Marker Preview/Start로 결과를 확인합니다.

종료 시 Marker/Device, Document를 해제하고 `Core.Cleanup()`을 호출하십시오.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
