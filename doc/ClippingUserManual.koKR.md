# 문서 클리핑 사용자 매뉴얼

> 기준 버전: Sirius3 1.16.0 (공개 Release 기능)

[English](ClippingUserManual.md) | [한국어](ClippingUserManual.koKR.md)

## 1. 원본과 영역 선택

`IClippable`은 월드 좌표 `Frustum` 내부의 독립 형상을 생성합니다. `IDocument.ActClip`은 비어 있지 않은 영역별 결과 그룹을 활성 레이어에 추가하며, 결과 추가와 선택적 원본 삭제를 한 번의 Undo/Redo로 처리합니다. WinForms/WPF 편집기의 클리핑 명령 또는 공개 [editor_clip](../demos/editor_clip/Form1.cs) 예제를 사용합니다.

문서가 소유한 지원 엔티티를 선택합니다. `EntityLayer`의 내용을 자를 때는 해당 레이어 하나만 전달합니다. 부모와 자식이 함께 선택되면 부모를 한 번만 처리합니다. 월드 영역에는 원본 계층의 변환이 반영되며 대상 계층에는 고정 정렬과 역변환 가능한 표준 변환이 필요합니다.

## 2. 경로, 채워진 영역과 해치

| 옵션 | 동작 |
|---|---|
| `ClipModes.Paths` (기본값) | 영역 안의 경로 구간을 유지합니다. 열린 경로는 열린 상태이며 지원하는 원본 곡선 유형을 유지합니다. |
| `ClipModes.ClosedRegions` | 평면의 채워진 영역을 교차하고 절단 경계를 포함합니다. 원본은 공통 로컬 XY 평면에 있어야 합니다. |
| `IncludeHatches` (기본 `true`) | 기존 활성 HatchLine/HatchPolygon 경로를 독립 엔티티로 포함합니다. 원본 해치 설정을 결과에서 재생성하지 않습니다. |
| `Fill` | ClosedRegions의 입력 채움 규칙이며 기본값은 `HatchFills.EvenOdd`입니다. |
| `Winding` | 로컬 XY 결과 방향이며 기본값은 외곽 CCW와 구멍 CW입니다. |

래스터 클리핑은 픽셀 간격과 위치를 유지하고 유효 픽셀 영역으로 자르며, 후속 클리핑과 가공에서 투명 픽셀을 제외합니다. 중첩이 0이면 인접 이미지 타일의 렌더링 경계를 샘플 이동 없이 유지합니다.

## 3. Undo/Redo를 지원하는 결과 추가

다음 코드는 초기화된 문서에서 평면 `IClippable` 엔티티가 선택된 상황을 전제로 합니다. `DVec2`는 프로젝트의 OpenTK 3/4 double 벡터 별칭입니다.

```csharp
var regions = new[]
{
    Frustum.Build(new DVec2(-10, -10), new DVec2(10, 10))
};
var options = ClipOptions.Default;
options.Mode = ClipModes.ClosedRegions;
options.IncludeHatches = true;
bool success = document.ActClip(document.Selected, regions, options, isRemove: false);
```

성공한 결과는 문서가 소유하고 Clip1, Clip2 등의 이름을 사용합니다. `isRemove: false`이면 원본을 유지합니다. 기본 `true`는 결과를 만든 원본만 삭제하며, 레이어 클리핑은 레이어 자체를 유지하고 기존 자식 전체를 제거합니다. false 반환은 잘못된 입력, 계산 실패 또는 빈 결과를 의미하며 계산 실패 시 원본은 유지됩니다. 동기 호출 중 원본 형상·소유 계층과 활성 대상을 바꾸지 않습니다.

`IClippable.Clip`을 직접 호출하면 원본은 바뀌지 않고 호출자 소유 결과를 반환합니다. 모든 결과를 문서에 추가하거나 해제합니다. 문서 편집과 Undo/Redo에는 폐기 예정 `ClipHelper` 대신 `ActClip`을 사용합니다.

## 4. 편집기 작업 순서

편집기 또는 TreeView에서 객체나 레이어를 선택한 뒤 클리핑을 엽니다. 격자의 영역 크기와 중첩을 설정하고 영역을 미리 본 다음 형상·해치 옵션과 원본 유지 여부를 선택합니다. 한 번 적용하고 결과 그룹, 경계와 해치 경로를 검토한 뒤 가공 작업을 준비합니다. 레이어를 선택해도 페이지 내 레이어 자체는 제거하지 않습니다.

관련 문서: [문서](DocumentUserManual.koKR.md), [엔티티](EntityUserManual.koKR.md), [WinForms](WinForms.koKR.md), [WPF](WPF.koKR.md).

## 관련 기능

[Offset 좌표와 문자열 파싱](OffsetUserManual.koKR.md), [텍스트](TextUserManual.koKR.md), [바코드](BarcodeUserManual.koKR.md), [해치](HatchUserManual.koKR.md), [전체 EntityFactory](EntityUserManual.koKR.md)를 함께 참고합니다.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
