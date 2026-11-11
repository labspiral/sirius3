# 문서 클리핑 사용자 매뉴얼

> API 기준: Sirius3 Core/UI 공개 기능

[English](ClippingUserManual.md) | [한국어](ClippingUserManual.koKR.md) | [전체 안내서](README.koKR.md)

## 1. 원본과 영역 선택

`IClippable`은 월드 좌표 `Frustum` 내부의 독립 형상을 생성합니다. `IDocument.ActClip`은 비어 있지 않은 영역별 결과 그룹을 활성 레이어에 추가하며, 결과 추가와 선택적 원본 삭제를 한 번의 Undo/Redo로 처리합니다. WinForms/WPF 편집기의 클리핑 명령 또는 공개 [editor_clip](../demos/editor_clip/Form1.cs) 예제를 사용합니다.

문서가 소유한 지원 엔티티를 선택합니다. `EntityLayer`의 내용을 자를 때는 해당 레이어 하나만 전달합니다. 부모와 자식이 함께 선택되면 부모를 한 번만 처리합니다. 월드 영역에는 원본 계층의 변환이 반영되며 대상 계층에는 고정 정렬과 역변환 가능한 표준 변환이 필요합니다.

## 2. 경로, 채워진 영역과 해치

| 옵션 | 동작 |
|---|---|
| `ClipModes.Paths` (기본값) | 영역 안의 경로 구간을 유지합니다. 열린 경로는 열린 상태이며 지원하는 원본 곡선 유형을 유지합니다. |
| `ClipModes.ClosedRegions` | 평면의 채워진 영역을 교차하고 절단 경계를 포함합니다. 원본은 공통 로컬 XY 평면에 있어야 합니다. |
| `IsIncludeHatches` (기본 `true`) | 기존 활성 HatchLine/HatchPolygon 경로를 독립 엔티티로 포함합니다. 원본 해치 설정을 결과에서 재생성하지 않습니다. |
| `IsIncludeOnce` (기본 `true`) | 순서가 있는 `ActClip` 호출에서 공유 점, 직선 경로 구간, 활성 래스터 픽셀을 처음 포함하는 영역에만 배정합니다. `false`이면 포함하는 모든 영역에 유지합니다. |
| `Fill` | ClosedRegions의 입력 채움 규칙이며 기본값은 `HatchFills.EvenOdd`입니다. |
| `Winding` | 로컬 XY 결과 방향이며 기본값은 외곽 CCW와 구멍 CW입니다. |

래스터 클리핑은 픽셀 간격과 위치를 유지하고 유효 픽셀 영역으로 자르며, 후속 클리핑과 가공에서 투명 픽셀을 제외합니다. 중첩이 0이면 인접 이미지 타일의 렌더링 경계를 샘플 이동 없이 유지합니다. `IsIncludeOnce`를 켜도 교차하는 선 조각은 공유 끝점에서 이어집니다. 단일 영역을 직접 전달하는 `IClippable.Clip` 호출에는 이 옵션이 적용되지 않습니다.

### 경계 데이터의 소속 영역과 순서

인접한 두 타일의 공통 경계에 있는 점·일치하는 직선 구간·래스터 샘플은 두 영역에 모두 포함될 수 있습니다. `ClipOptions.IsIncludeOnce = true`는 `ActClip`에 전달한 순서에서 처음 포함하는 영역에만 이 데이터를 배정합니다. 예를 들어 X=0인 점은 아래 배열의 첫 타일에 속하며 영역 순서를 바꾸면 소속도 바뀝니다.

```csharp
var orderedRegions = new[]
{
    Frustum.Build(new DVec2(-10, -10), new DVec2(0, 10)),
    Frustum.Build(new DVec2(0, -10), new DVec2(10, 10))
};
var boundaryOptions = ClipOptions.Default;
boundaryOptions.Mode = ClipModes.Paths;
boundaryOptions.IsIncludeOnce = true;
bool clipped = document.ActClip(document.Selected, orderedRegions,
    boundaryOptions, isRemove: false);
```

WinForms와 WPF의 클리핑 대화상자에서 같은 옵션을 제공합니다. 공유 데이터의 중복을 피하려면 켜고 포함하는 모든 영역에 사본이 필요할 때만 끕니다. 영역 순서에 따른 배정 규칙이며 레시피 전체의 겹치는 개체를 모두 제거하는 기능은 아닙니다. 결과를 가공하기 전에 그룹 경계를 확인하고 Undo/Redo로 검토합니다.

## 3. Undo/Redo를 지원하는 결과 추가

다음 코드는 초기화된 문서에서 평면 `IClippable` 엔티티가 선택된 상황을 전제로 합니다. `DVec2`는 프로젝트의 OpenTK 3/4 double 벡터 별칭입니다.

```csharp
var regions = new[]
{
    Frustum.Build(new DVec2(-10, -10), new DVec2(10, 10))
};
var options = ClipOptions.Default;
options.Mode = ClipModes.ClosedRegions;
options.IsIncludeHatches = true;
bool success = document.ActClip(document.Selected, regions, options, isRemove: false);
```

성공한 결과는 문서가 소유하고 Clip1, Clip2 등의 이름을 사용합니다. `isRemove: false`이면 원본을 유지합니다. 기본 `true`는 결과를 만든 원본만 삭제하며, 레이어 클리핑은 레이어 자체를 유지하고 기존 자식 전체를 제거합니다. false 반환은 잘못된 입력, 계산 실패 또는 빈 결과를 의미하며 계산 실패 시 원본은 유지됩니다. 동기 호출 중 원본 형상·소유 계층과 활성 대상을 바꾸지 않습니다.

`IClippable.Clip`을 직접 호출하면 원본은 바뀌지 않고 호출자 소유 결과를 반환합니다. 모든 결과를 문서에 추가하거나 해제합니다. 문서 편집과 Undo/Redo에는 폐기 예정 `ClipHelper` 대신 `ActClip`을 사용합니다.

## 4. 편집기 작업 순서

편집기 또는 TreeView에서 객체나 레이어를 선택한 뒤 클리핑을 엽니다. 격자의 영역 크기와 중첩을 설정하고 영역을 미리 본 다음 형상·해치 옵션과 원본 유지 여부를 선택합니다. 한 번 적용하고 결과 그룹, 경계와 해치 경로를 검토한 뒤 가공 작업을 준비합니다. 레이어를 선택해도 페이지 내 레이어 자체는 제거하지 않습니다. 격자 중첩은 선택한 전체 외곽 범위를 넘지 않으며, 한 칸짜리 격자에는 실제 중첩이 없습니다. WPF 대화상자도 WinForms와 같은 격자·클리핑 옵션을 제공합니다.

관련 문서: [문서](DocumentUserManual.koKR.md), [엔티티](EntityUserManual.koKR.md), [WinForms](WinForms.koKR.md), [WPF](WPF.koKR.md).

## 관련 기능

[Offset 좌표와 문자열 파싱](OffsetUserManual.koKR.md), [텍스트](TextUserManual.koKR.md), [바코드](BarcodeUserManual.koKR.md), [해치](HatchUserManual.koKR.md), [전체 EntityFactory](EntityUserManual.koKR.md)를 함께 참고합니다.

## 이미지 영역 확인

이미지 밖에 있는 영역은 결과를 만들지 않으며 원본 제거는 결과를 만든 원본에
적용됩니다. 부분 래스터 클리핑은 픽셀 위치를 유지한 독립 결과를 만듭니다. 회전·반사한
이미지도 겹침 0의 경계와 남은 픽셀을 확인하고 영역이 잘못되었으면 Undo로 복원합니다.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
