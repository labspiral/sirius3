# 바코드 개체 사용자 매뉴얼

> 기준 버전: Sirius3 1.16.0 (공개 Release 기능)

[English](BarcodeUserManual.md) | [한국어](BarcodeUserManual.koKR.md)

## 1. 클래스와 Factory

EntityBarcode1D_v2는 구체 1D 클래스이며 소문자 v2가 실제 C# 이름입니다. EntityBarcode2DBase는 구체 2D 형식이 공유하는 셀 형상, 해치, 변환, 렌더링·가공 기능을 제공하는 추상 기반 클래스입니다.

| 엔티티 | 생성 함수 | 형식별 설정 |
|---|---|---|
| EntityBarcode1D_v2 | EntityFactory.CreateBarcodeV2 | Format, DotDimension, QuiteZone, GS1·Encoding |
| EntityDataMatrix | CreateDataMatrix | IsSquare, 최소 DotDimension, IsC40, IsCompactEncoding |
| EntityQRCode | CreateQRCode | QrVersion, L/M/Q/H 오류 정정, 출력 샘플링 |
| EntityPDF417 | CreatePDF417 | IsCompact, L0–L8/AUTO 오류 정정, 실제 행렬 크기 |
| EntityAztec | CreateAztec | Layers, 오류 정정 비율, 출력 샘플링 |

네 2D 클래스는 EntityBarcode2DBase를 상속합니다. CreateBarcode는 별도 기존 EntityBarcode1D를 반환하므로 이 문서의 셀 API에는 CreateBarcodeV2를 사용합니다. Factory 결과는 문서에 연결되지 않은 호출자 소유이며 ActAdd로 문서에 추가하여 소유권을 이전합니다.

## 2. 내용, GS1과 Encoding

개체들은 ITextConvertible을 구현합니다. 정적 내용에는 SourceText를 사용하고 동적 데이터는 IsAllowConvert와 TextConverter를 설정합니다. ConvertedText는 해당 가공 호출에 맞게 결정합니다. 빈 값·잘못된 데이터는 오류를 기록하며 유효한 바코드로 취급하지 않습니다. [TextConverters](TextConvert.koKR.md)를 참고합니다.

IsAllowEncoding의 기본값은 false입니다. 이때 인코더의 기본 문자 처리를 사용하고 설정된 Encoding은 보존하지만 무시합니다. true이면 해당 형식·런타임이 지원하는 UTF-8 등 명시적 Encoding을 요청할 수 있습니다. ECI와 해독 결과가 달라질 수 있으므로 사용할 판독기로 확인합니다.

IsGS1Format은 해당 형식의 GS1 인코딩 정책을 사용합니다. 구현은 `<GS>`, `(,)` 등의 그룹 구분자 표기를 인식합니다. 모든 Application Identifier 업무 규칙을 검증하는 기능은 아닙니다. 필요한 구분자를 유지하고 해독 데이터를 확인합니다. Quiet zone 속성의 실제 이름은 QuiteZone이며 C# 코드에서 철자를 바꾸지 않습니다.

## 3. Width·Height와 행렬 크기

Width와 Height는 로컬 mm 단위의 사용자 요청 최대 가공 범위입니다. IsFixedAspectRatio를 켜면 인코딩 행렬을 두 범위 안에 비례 맞춤합니다. 맞춤 결과를 요청값에 다시 저장하지 않으므로 심벌 형태 변경·재생성으로 요청 크기가 계속 줄어들지 않습니다.

비율 맞춤을 끄면 독립 치수로 X/Y 간격이 달라질 수 있습니다. 논리적으로 정사각형인 Data Matrix·QR이라고 실제 모듈도 정사각형이라고 단정하지 않습니다. Quiet zone, 행렬 크기와 셀 세분화가 형상에 영향을 줍니다. 엔티티·그룹 변환 후의 월드 범위도 확인합니다.

| 클래스 | 요청값과 생성 결과 |
|---|---|
| 1D v2 | DotDimension은 출력 크기를 요청합니다. 0인 축은 자동 크기를 허용하며 DotDimensionAdjusted가 실제 행렬입니다. |
| Data Matrix | DotDimension은 Square·Rectangle 표준 계열로 보정한 최소값입니다. 내용에 따라 더 큰 심벌이 필요하며 DotDimensionAdjusted가 실제 논리 크기입니다. |
| QR | DotDimension은 출력 샘플링 요청입니다. 논리 모듈은 QrVersion·내용으로 정하며 실제 출력은 DotDimensionAdjusted에서 확인합니다. |
| PDF417 | DotDimension은 생성된 읽기 전용 결과이며 요청 최소값이 아닙니다. |
| Aztec | DotDimension은 샘플링 요청입니다. 구조 크기는 Layers·내용·오류 정정으로 정하며 실제 출력은 DotDimensionAdjusted입니다. |

Adjusted 결과에는 추가 Quiet zone·셀 DotFactor 확장을 포함하지 않으며 직렬화하지 않습니다. 생성 전·실패 후에는 0입니다. 이를 다음 요청값으로 되먹임하지 말고 구체 클래스에 맞는 속성을 사용합니다.

## 4. 셀 모드와 가공

각 계열의 enum은 EntityBarcode1D_v2.Barcode1DCells와 EntityBarcode2DBase.Barcode2DCells입니다.

| 모드 | 생성 가공 표현 |
|---|---|
| Dots | 활성 셀의 점입니다. 표시와 가공은 같은 로컬 좌표를 사용합니다. |
| Lines | CellLine 방향·세분화·ZigZag에 따른 선 패턴입니다. |
| Outline | 채워진 모듈의 경계 컨투어이며 영역 해치에 사용합니다. |
| Circles | CellCircle로 설정한 원형 패턴입니다. |
| Squares | CellSquare로 설정한 사각형 패턴입니다. |

CellDot, CellLine, CellOutLine, CellCircle, CellSquare는 모드별 설정을 제공합니다. 지원 모드의 DotFactor는 세분화를, 원 반경·사각형 배율·진행 설정은 패턴을 바꿉니다. 설정은 사용 중인 바코드가 소유합니다. 독립 개체에는 설정을 복제하고 편집 후 재생성합니다.

Dots는 점 가공이며 EntityPen의 PixelTime·PixelPulses와 지원 Scanner·Laser 동작이 출사를 결정합니다. 표시 PointSize는 실제 레이저 Spot 지름이 아닙니다. IsInvertCell은 활성·비활성 셀을 반전하므로 가공 영역이 크게 바뀔 수 있습니다.

## 5. 1D 형식

v2 Format enum은 CODABAR, Code39, Code93, Code128, EAN8, EAN13, ITF, MSI, UPCA, UPCE, UPCEAN_EXT, PLESSEY를 제공합니다. 입력 제한·검사 숫자 처리는 인코더별로 다릅니다. UPC/EAN 확장은 별도 형식이므로 본체·보조 코드 사용을 대상 판독기로 검증합니다.

새 v2 개체의 기본값은 Code128, Dots, 20 × 4 mm와 독립 비율 설정입니다. 공정에 필요한 셀 표현을 명시적으로 선택합니다.

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

독립 예제는 엔티티를 소유·해제합니다. 문서에 추가하면 using 범위 종료 대신 문서 소유권에 따라 유지합니다.

## 6. Data Matrix·QR·PDF417·Aztec

Data Matrix는 기본적으로 Square 계열을 사용하고 IsSquare false이면 지원 Rectangle 심벌을 선택합니다. 최소 크기는 임의 행·열이 아니라 지원하는 이산 표준 크기에서 선택합니다. C40·Compact는 서로 배타적인 인코딩 정책이며 문자셋·오류 정정 설정이 아닙니다.

QR의 nullable QrVersion은 자동 또는 지정 Version을 선택합니다. ErrorCorrectionLevel은 L, M, Q, H이며 새 개체는 H입니다. Version·내용·오류 정정이 논리 모듈을 결정하며 출력 샘플링 요청과 별개입니다.

PDF417은 적층 심벌입니다. IsCompact는 인코딩 표현을 바꾸고 ErrorCorrectionLevel은 L0–L8 또는 기본 AUTO를 사용합니다. 재생성 후 실제 DotDimension을 읽습니다.

Aztec Layers 0은 자동 선택이며 -1~-4는 Compact, 1~32는 Full을 지정합니다. ErrorCorrectionLevel은 기본 33이며 25–100으로 보정합니다. 강제 크기가 내용·복구 데이터를 수용하지 못하면 실패합니다.

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

## 7. 해치, 클리핑과 재생성

Dots·Lines는 열린 점·선 표현입니다. 지원하는 닫힌 모드는 영역 컨투어를 제공합니다. 코드 영역을 채우려면 Outline과 해치 패턴을 사용하며 HatchOnly로 별도 경계 가공을 생략합니다. 구멍, Quiet zone과 접한 셀의 통합 위상을 유지합니다. [해치](HatchUserManual.koKR.md)를 참고합니다.

형상 설정 변경 후 Regen·ActRegen을 수행하고 새 내용을 표시·선택·가공합니다. 인코딩 실패 시 이전 셀 형상, bounds와 해치 결과를 폐기하고 사용자 설정은 이후 유효한 값에 사용하도록 유지합니다. 이전 표시만으로 판단하지 말고 Boolean 결과·로그를 확인합니다.

IClippable은 생성 표현을 지원합니다. ActClip의 IncludeHatches로 기존 해치 경로를 유지하고 Undo/Redo로 결과를 추가할 수 있습니다. 잘린 일부는 형상이며 자동으로 판독 가능한 완전한 심벌이 되는 것은 아닙니다. [클리핑](ClippingUserManual.koKR.md)을 참고합니다.

## 8. 사용자 바코드 구현 확장

EntityBarcode2DBase는 공통 형상·인터페이스를 제공합니다. 구체 구현은 EncodeAndCreateBitMatrix를 재정의하여 인코더를 연결하고 복제 동작을 구현합니다. 행렬 치수, 입력 검증, 직렬화 설정, ModifyFlag, 셀 소유권과 실패 정리를 유지합니다. 원본 순서, 표시·컨투어·선택·가공 표현을 일치시킵니다.

공개 DataMatrix·QR·PDF417·Aztec 구현을 참고합니다. 엔티티·인코더 계층을 Scanner 명령 실행과 분리하여 확장하고 기존 Marker·Pen 계약으로 출력합니다. 기반 클래스는 추상이므로 직접 생성하지 않습니다.

## 9. 편집과 결과 확인

Editor·TreeView에서 바코드를 선택하고 PropertyGrid에서 Format, 데이터, 크기, Cell과 해치를 편집합니다. 정적·변환 데이터, Quiet zone, 실제 X/Y 비율, Outline·Hatch·Dots와 EntityPen을 확인합니다. 실제 생성 심벌은 사용할 판독기와 공정 조건에서 검증합니다.

공개 예제: [editor_barcode](../demos/editor_barcode/Form1.cs), [editor_barcode_textconvert](../demos/editor_barcode_textconvert/Form1.cs). 관련 문서: [엔티티](EntityUserManual.koKR.md), [텍스트](TextUserManual.koKR.md), [Pen](PenUserManual.koKR.md), [PropertyGrid](PropertyGridUserManual.koKR.md).

---
2026 Copyright (c) SpiralLAB. All rights reserved.
