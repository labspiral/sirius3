# 텍스트 개체 사용자 매뉴얼

> API 기준: Sirius3 Core/UI 1.17.0 (공개 기능)

[English](TextUserManual.md) | [한국어](TextUserManual.koKR.md) | [전체 안내서](README.koKR.md)

## 1. 텍스트 표현 방식 선택

다섯 텍스트 계열은 문서, 변환, Pen과 ITextConvertible 흐름을 공유하며 글꼴 원본과 가공 형상이 다릅니다.

| 엔티티 | 글꼴 원본·출력 | 채움·Factory |
|---|---|---|
| `EntityText` | Windows 설치 글꼴의 직선 배치 벡터 윤곽선 | IHatchable; CreateText |
| `EntitySiriusText` | Sirius 글꼴 파일의 직선 배치 stroke/polyline 또는 DOT 점 | 닫힌 윤곽선의 IHatchable; CreateSiriusText |
| `EntityImageText` | Windows 설치 글꼴을 비트맵으로 생성 | 래스터·픽셀 가공; CreateImageText |
| `EntityCircularText` | Windows 글꼴의 벡터 글리프를 원주에 배치 | IHatchable; CreateCircularText |
| `EntityCircularSiriusText` | Sirius 글꼴 경로·점을 원주에 배치 | 가능한 닫힌 윤곽선 채움; CreateCircularSiriusText |

편집기의 Text·SiriusText 등은 위 Entity 클래스를 가리킵니다. EntityFactory는 문서에 연결되지 않은 호출자 소유 엔티티를 생성합니다. 설정·재생성 후 document.ActAdd(entity)로 문서 소유권과 Undo/Redo를 적용합니다. 추가하지 않은 인스턴스는 생성자가 해제합니다.

## 2. 원본 문자열과 동적 변환

SourceText는 입력 문자열입니다. 변환을 끄면 일반 텍스트 편집으로 생성 문자열을 갱신합니다. IsAllowConvert가 true이면 TextConverter가 Event, SimpleScript, File, Offset, Link 방식으로 가공 중 ConvertedText를 결정합니다.

변환 활성 가공은 독립 엔티티 복제본을 사용하여 각 호출의 문자열과 준비된 형상을 유지합니다. 콜백에는 원본이 전달되며 독립 Marker에서 동시에 실행될 수 있습니다. 표시된 ConvertedText는 최신 표시값이며 실제 가공 성공을 뜻하지 않습니다. 변환을 켠 동안 수동 대입하지 않습니다.

File은 입력 줄을 소비하고 Link는 지정한 마스터의 현재 변환값을 읽으므로 마스터·종속 개체 순서를 정합니다. API와 실패 처리는 [TextConverters](TextConvert.koKR.md)를 참고합니다.

## 3. Windows 글꼴 벡터 텍스트

EntityText는 FontName, FontStyle, CapHeight를 사용합니다. CapHeight는 mm 단위 글꼴 기준 높이이며 글꼴 ascent를 이 높이에 맞춥니다. 실제 대문자 외곽 높이는 글꼴별로 다를 수 있습니다. 대상 PC의 설치 글꼴과 지원 글리프가 결과에 영향을 줍니다.

| 설정 | 의미 |
|---|---|
| FontHorizontalAlignment / FontVerticalAlignment | 텍스트 레이아웃 내부 정렬이며 엔티티·그룹 변환과 별개입니다. |
| LetterSpace | Auto, Variable, Fixed를 선택합니다. |
| TrackingFactor | Auto에서 shaped text cluster 원점 간격을 조정합니다. 1은 기본값이며 범위는 0.75–4입니다. 글리프 자체를 늘리지 않습니다. |
| LetterSpacing | Variable에서는 추가 문자 간격, Fixed에서는 고정 문자 폭 뒤의 간격이며 mm 단위입니다. Auto는 무시합니다. |
| WordSpacing / LineSpacing | 단어 간격과 추가 줄 간격이며 mm 단위입니다. |
| TargetWidth | Auto·Variable의 전체 목표 폭입니다. 0은 자동 폭이며 Fixed는 무시합니다. |
| IsDistributed | Auto에서 양수 TargetWidth까지 문자 묶음 간격을 늘려 분산하며 글리프를 늘리지 않습니다. |

분산에는 전진 폭이 있는 문자 묶음이 두 개 이상 필요하며 텍스트를 압축하지 않습니다. 이미 목표 폭보다 넓은 텍스트를 강제로 줄이지 않습니다. Tracking·분산 텍스트는 UTF-16 단위 10,000개로 제한합니다. 분산을 끈 일반 TargetWidth 맞춤은 폭 배율을 적용할 수 있습니다.

## 4. 고정 문자 위치와 글리프 폭

| 설정 | 의미 |
|---|---|
| FixedGlyphWidth | mm 단위 문자 위치 폭입니다. 0이면 설정된 글꼴·문자권 샘플로 자동 폭을 계산합니다. |
| FixedGlyphMaxWidth | 최근 성공한 Fixed 재생성의 자연 최대 글리프 폭이며 요청 문자 셀 폭과 별개인 읽기 전용 결과입니다. |
| IsGlyphWidthFit | 그릴 수 있는 글리프를 고정 폭에 수평으로 맞춥니다. 기본 false는 자연 비율을 유지합니다. |

Fixed는 공백·누락 글리프의 위치를 포함한 문자 위치를 유지합니다. 폭 맞춤을 끄고 좁은 폭을 지정하면 넓은 글리프가 그 폭 안에 들어간다고 보장할 수 없습니다. Fixed는 TargetWidth를 무시합니다. 문자 수만으로 외곽선 폭을 계산하지 말고 재생성 bounds를 확인합니다.

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

이 예제는 독립 설정용이며 using 범위가 엔티티를 소유합니다. 문서에 추가할 때는 소유권을 이전하고 범위 종료로 해당 엔티티를 해제하지 않습니다.

## 5. Sirius 글꼴 텍스트

FontName은 UI.Config.SiriusFontPath 아래의 파일을 선택합니다. CXF·LFF는 벡터 글리프 경로를, DOT는 지원 점 글리프를 제공합니다. EO .fnt는 일반 빌드에 포함되며 사용자 라이브러리 빌드에서 끌 수 있습니다. 지원 문자와 경로 닫힘을 확인합니다. Stroke 글꼴이 자동으로 채워진 글리프 내부를 정의하지는 않습니다.

Sirius 텍스트는 Variable·Fixed를 지원합니다. 새 개체의 Variable에서는 IsAutoKerning이 기본적으로 켜지며 최종 배율의 윤곽선 선분 간 거리를 포함한 캐시 형상으로 간격을 계산합니다. LetterSpacing은 출력 mm 단위 요청 간격입니다. Fixed 위치는 AutoKerning을 사용하지 않고 TargetWidth는 Variable에만 적용합니다.

FixedGlyphWidth, FixedGlyphMaxWidth, IsGlyphWidthFit은 같은 고정 위치 기능을 제공합니다. DotFactor는 DOT 점 생성을 조정하며 이 점은 폐영역 채움 대신 픽셀·점 Pen 조건을 사용합니다.

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

글꼴 파일은 설정 경로에 있어야 하며 Factory가 설치하지 않습니다.

## 6. 원형 텍스트

Radius는 mm 단위의 양수 기준선 반지름입니다. BaseAngle은 FontHorizontalAlignment가 사용하는 도 단위 기준 각도입니다. Direction에는 각 클래스의 TextCircularDirections.ClockWise 또는 CounterClockWise를 사용합니다.

줄 바꿈은 동심 텍스트 줄을 추가합니다. 첫 줄은 Radius를 유지하고 다음 줄은 글리프 쪽으로 진행합니다. 시계 방향은 바깥쪽, 반시계 방향은 안쪽으로 진행하며 LineSpacing이 추가 반경 간격입니다. 안쪽 줄의 반지름이 유효한 양수인지 확인합니다.

Windows 원형 텍스트는 Auto·Variable·Fixed를, 원형 Sirius 텍스트는 Variable·Fixed와 Sirius AutoKerning을 지원합니다. 각 클래스는 고유한 간격·방향 enum을 선언하므로 해당 클래스의 enum을 사용합니다.

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

ObliqueAngle은 글리프 기울기를 추가합니다. 직선 텍스트·ImageText는 정렬 흐름을 유지하며 생성 문자열을 기울이고, 원형 계열은 원 전체를 전단하지 않고 각 글리프를 로컬 접선 방향으로 기울입니다. 원형 기울기 범위는 -85°~85°입니다.

## 7. ImageText의 픽셀과 출력 크기

ImageText는 비트맵 레이아웃과 실제 가공 치수를 구분합니다.

| 픽셀 설정 | 출력 설정·의미 |
|---|---|
| CapHeightPixels | 비트맵 글꼴 기준 높이이며 실제 Height와 별개입니다. |
| TargetWidthPixels | Auto·Variable의 픽셀 레이아웃 폭이며 Fixed는 무시합니다. |
| FixedGlyphWidthPixels / IsGlyphWidthFit | 고정 픽셀 문자 위치와 선택적 글리프 폭 맞춤입니다. |
| LetterSpacing, WordSpacing, LineSpacing | 픽셀 간격입니다. 벡터 텍스트의 mm 값을 그대로 복사하지 않습니다. |
| Width, Height | mm 단위 출력 치수입니다. Width 0은 Height·레이아웃에서 폭을 계산하며 양수 Width는 Height와 독립적입니다. |
| IsFill, OutlinePixelSize | 채워진 비트맵 글리프 또는 픽셀 단위 윤곽 두께입니다. |
| MajorColor | 래스터·픽셀 처리에서 사용할 이미지 주 색상입니다. |

ImageText에는 IsFixedAspectRatio 속성이 없습니다. 논리 레이아웃·텍스트 여백과 불투명 픽셀 bounds를 구분합니다. 픽셀 해상도를 높이면 샘플링·데이터량을 바꾸며 실제 Height가 자동 증가하지 않습니다.

```csharp
using (var text = EntityFactory.CreateImageText(
    "Arial", System.Drawing.FontStyle.Regular, true, "SIRIUS3",
    capPixels: 64, outlinePixels: 1, height: 5, width: 0))
{
    if (!text.Regen())
        throw new System.InvalidOperationException("ImageText regeneration failed.");
}
```

가공에는 연결한 EntityPen의 RasterMode, RasterDirection, IsRasterZigZag, PixelTime, PixelPulses 등이 적용됩니다. 텍스트 채움 설정으로 벡터 해치로 전환되지는 않습니다. [Pen](PenUserManual.koKR.md)을 참고합니다.

## 8. 해치, 클리핑과 좌표 변환

Windows 윤곽 텍스트에는 HatchLine·HatchPolygon을 적용할 수 있습니다. Sirius 텍스트의 채움에는 유효한 닫힌 컨투어가 필요하며 Stroke·DOT 글꼴은 선·점 가공 경로로 남을 수 있습니다. HatchMarkOption은 외곽선·채움 순서를 정하고 HatchOnly는 외곽선 가공을 생략합니다. [해치](HatchUserManual.koKR.md)를 참고합니다.

텍스트 형상, 문자 배치와 해치 결과는 함께 재생성합니다. 원본·로컬 치수와 자체 모델 변환·누적 월드 치수는 다릅니다. 부모 변환과 Marker Offset은 최종 가공 좌표에 적용하고 카메라 확대는 형상을 바꾸지 않습니다.

텍스트 계열은 IClippable을 구현합니다. 윤곽 텍스트는 독립 벡터 클리핑 결과를, ImageText는 샘플 위치를 유지한 래스터 결과를 만듭니다. 문서 ActClip은 Undo/Redo로 결과를 추가하며 모든 결과를 동적 변환 텍스트로 다시 만들지는 않습니다. [클리핑](ClippingUserManual.koKR.md)을 참고합니다.

## 9. 편집과 결과 확인

편집기에서 필요한 텍스트 유형을 생성·선택한 뒤 PropertyGrid에서 내용, 글꼴, 간격, 정렬, 해치·래스터를 편집합니다. 문서 작업으로 편집하고 코드로 설정을 바꾸면 재생성합니다. 글꼴·글리프 누락이나 잘못된 형상은 반환값과 로그로 확인합니다.

공백, 누락 문자, 여러 줄, 원형 방향, 고정 위치, 최종 bounds, 채움 구멍과 Pen을 확인합니다. 저장·재열기는 텍스트 파라미터를 유지하고 형상을 다시 생성합니다. 호출자 소유 독립 엔티티는 해제하고 문서 소유 엔티티는 문서 정리까지 유지합니다.

공개 예제: [editor_entity](../demos/editor_entity/Form1.cs), [editor_barcode_textconvert](../demos/editor_barcode_textconvert/Form1.cs). 관련 문서: [엔티티](EntityUserManual.koKR.md), [바코드](BarcodeUserManual.koKR.md), [WinForms](WinForms.koKR.md), [WPF](WPF.koKR.md).

## Font Editor로 폰트 편집

편집기 우클릭 메뉴의 **Font Edit**로 CXF·LFF·DOT 폰트를 생성·편집하거나 기존
EO FNT 파일을 엽니다. SiriusText를 먼저 선택하면 해당 폰트를 엽니다. 문자·경로·
메트릭과 문자열 미리보기를 편집한 뒤 다른 이름으로 저장합니다. 저장하면 현재 프로그램의
같은 폰트를 사용하는 텍스트가 갱신됩니다. 폰트 파일 변경은 자체 이력을 사용하며 문서의
Undo/Redo에 포함되지 않습니다. [Font Editor](FontEditorUserManual.koKR.md)를 참고하십시오.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
