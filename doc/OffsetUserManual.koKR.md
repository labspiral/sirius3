# Marker Offset 사용자 매뉴얼

> API 기준: Sirius3 Core/UI 1.17.0 (공개 기능)

[English](OffsetUserManual.md) | [한국어](OffsetUserManual.koKR.md) | [전체 안내서](README.koKR.md)

## 1. 가공 위치별 좌표와 방향

`SpiralLab.Sirius3.Mathematics.Offset`은 값 형식입니다. `marker.Offsets`는 문서 형상을 영구 변경하지 않고 여러 가공 위치를 지정합니다. Marker가 변환 스택으로 각 Offset을 적용하며 `WorkingSet.Offset`과 `WorkingSet.OffsetIndex`는 현재 처리 중인 항목입니다.

| 멤버 | 역할 |
|---|---|
| Translate.X / Y | X/Y 방향 이동량이며 단위는 mm입니다. |
| Translate.Z | Z 방향 이동량이며 단위는 mm입니다. 호환되는 3D 가공 경로가 필요하며 이 값 자체가 기계 스테이지를 움직이지는 않습니다. |
| AngleZ | Z축 회전각이며 단위는 도입니다. 양수는 반시계 방향입니다. |
| Scale | 축별 배율이며 생성자는 각 축을 1로 초기화합니다. |
| ExtensionData | 위치별 문자열 정보입니다. Offset 텍스트 변환의 데이터를 제공하며 좌표에는 영향을 주지 않습니다. |
| ToMatrix | Sirius3의 행 벡터 규약에서 배율 → Z축 회전 → 이동을 결합하는 행렬 속성입니다. |

예를 들어 `new Offset(10, 20, 3, 90, "Part A")`는 배율 1에서 로컬 점 `(1, 0, 0)`을 `(10, 21, 3)`으로 변환합니다. 개체·부모 변환도 Marker Offset과 함께 적용되므로 같은 배치를 개체와 Offset에 중복 적용하지 않습니다.

항등 Offset은 `Offset.Zero` 또는 생성자를 사용합니다. `default(Offset)`은 Scale까지 0으로 초기화되어 항등 변환이 아닙니다. 배열 항목을 편집할 때에는 복사한 값을 변경한 뒤 다시 대입합니다.

## 2. 위치 지정

```csharp
using SpiralLab.Sirius3.Mathematics;

Offset[] locations =
{
    new Offset(-10, 0) { ExtensionData = "MyBarcode|OFFSET 11|MyText|OFFSET 12" },
    new Offset(+10, 0) { ExtensionData = "MyBarcode|OFFSET 21|MyText|OFFSET 22" }
};
// 초기화된 Marker가 대기 상태일 때 지정합니다.
// marker.Offsets = locations;
```

LayerFirst는 한 Offset에서 레이어들을 처리한 뒤 다음 Offset으로 이동합니다. OffsetFirst는 한 레이어를 여러 Offset에서 처리한 뒤 다음 레이어로 이동합니다. 선택한 Marker의 순회·이벤트 순서를 확인합니다. `OffsetIndex`는 현재 배열 항목이며 완성품 개수가 아닙니다. [Marker](MarkerUserManual.koKR.md)를 참고합니다.

## 3. Offset 텍스트 변환 선택

텍스트·바코드 개체의 `IsAllowConvert = true`, `TextConverter = TextConverters.Offset`를 지정합니다. 가공 중 현재 Offset의 `ExtensionData`를 읽습니다. 이름·값 데이터에서는 개체의 `Name`이 조회 키입니다. 미리보기의 대체 문자열이나 `ConvertedText`만으로 해당 Offset의 가공 완료를 판단하지 않습니다.

공개 [editor_barcode_textconvert](../demos/editor_barcode_textconvert/Form1.cs)는 `MyBarcode`와 `MyText`에 이 변환기를 적용하고 위의 두 위치를 사용합니다.

| 위치 | MyBarcode | MyText |
|---|---|---|
| X = −10 mm | OFFSET 11 | OFFSET 12 |
| X = +10 mm | OFFSET 21 | OFFSET 22 |

## 4. 일반 문자열과 이름·값 쌍

설정한 구분자를 포함하지 않고 시작 부분이 JSON 객체 표시도 아니면 문자열 전체를 모든 Offset 변환 개체에 사용합니다. 예를 들어 `ExtensionData = "PART-001"`은 두 개체 모두에 `PART-001`을 제공합니다.

기본 `UI.Config.RemoteSeparator`는 `|`입니다. 이름과 값을 번갈아 기록합니다.

```text
MyBarcode|DM-001|MyText|TEXT-001
```

구분자 파서는 현재 구분자로 분리하고 빈 토큰을 제거합니다. 토큰은 비어 있지 않은 짝수 개여야 합니다. 이름의 앞뒤 공백은 제거하고 값의 공백은 유지합니다. 이름은 대소문자를 구분하지 않으며 같은 이름이 반복되면 마지막 쌍을 사용합니다.

| ExtensionData | MyText 개체의 결과 |
|---|---|
| `mytext\|A` | `A` |
| ` MyText \| A ` | 공백을 포함한 ` A ` |
| `MyText\|A\|MyText\|B` | `B` |
| `MyBarcode\|A` | 이름이 없어 변환 실패 |
| `MyText\|A\|extra` | 토큰이 홀수 개여서 변환 실패 |

구분자 방식에서 빈 값을 사용하면 빈 토큰 제거 때문에 쌍의 경계가 바뀔 수 있습니다. 구분자를 이스케이프하는 문법이 없으므로 값에 구분자가 있으면 JSON을 사용합니다. 데이터가 비었거나 변환값이 비어 있거나 누락되면 변환 실패로 가공을 중단합니다.

## 5. JSON 데이터

```json
{"MyBarcode":"DM|001","MyText":"TEXT-001"}
```

앞쪽 공백을 제거한 문자열이 `{`로 시작하면 JSON 문자열 사전으로 파싱합니다. JSON 키는 대소문자를 구분하므로 `MyText`와 `mytext`는 서로 다릅니다. 구분자 방식과의 차이입니다. 키가 없으면 변환 실패입니다. 잘못된 JSON은 구분자 파싱을 다시 시도하므로 이 동작에 의존하지 말고 생성 단계에서 JSON을 검증합니다.

구분자 방식에서는 현재 설정한 구분자만 적용합니다. `&`나 `;`가 자동 대체 구분자는 아닙니다. 외부 프로그램의 데이터 생성에도 같은 설정을 사용합니다.

## 6. 가공과 소유권

시작 전에 Offset과 데이터를 준비·검증합니다. Marker가 Busy인 동안 위치나 데이터를 바꾸지 않습니다. Marker 이벤트, 완료 상태와 변환 오류를 확인합니다. Offset은 값으로 복사되며 장치·문서 수명은 호스트가 소유합니다.

관련 문서: [TextConvert](TextConvert.koKR.md), [텍스트](TextUserManual.koKR.md), [바코드](BarcodeUserManual.koKR.md), [MoF](MarkingOnTheFlyUserManual.koKR.md).

---
2026 Copyright (c) SpiralLAB. All rights reserved.
