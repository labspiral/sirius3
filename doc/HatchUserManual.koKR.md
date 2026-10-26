# IHatch 해치 기능 사용자 매뉴얼

> API 기준: Sirius3 Core/UI 1.17.0 (공개 기능)

[English](HatchUserManual.md) | [한국어](HatchUserManual.koKR.md) | [전체 안내서](README.koKR.md)

## 1. 패턴, 엔티티와 생성 경로

`SpiralLab.Sirius3.Entity.Hatch`의 `IHatch`는 하나의 채움 패턴을 정의합니다. `HatchLine`은 평행 선분을, `HatchPolygon`은 안쪽으로 연속 오프셋한 폴리곤 셸을 생성합니다. 두 클래스는 `HatchBase`를 상속하며 속성 알림, 복제와 명시적 해제를 지원합니다.

`SpiralLab.Sirius3.Entity`의 `IHatchable`은 닫힌 컨투어를 제공하고 해치 목록을 소유하는 엔티티 측 인터페이스입니다. 닫힌 벡터 개체, 윤곽선 텍스트, 바코드와 지원 그룹 등이 구현합니다. 선택한 개체의 실제 인터페이스와 닫힌 형상을 확인합니다. `EntityImageText`는 `IHatchable` 벡터 채움 대신 래스터 가공을 사용합니다.

엔티티 재생성은 원본 형상과 컨투어를 준비하고 변경된 패턴을 재생성하여 결과 경로를 게시합니다. 렌더링은 이 경로를 표시하며 Marker는 이후 해당 Pen과 변환을 적용하여 명령을 제출합니다. 패턴 생성이나 재생성만으로 가공을 시작하지 않습니다.

## 2. IHatch 멤버

| 멤버 | 계약 |
|---|---|
| `Name` | IHatch에서는 읽기 전용이며 내장 HatchBase에서는 편집할 수 있습니다. |
| `ModelColor` | IHatch에서는 읽기 전용이며 내장 패턴에서는 표시·Pen 색을 편집합니다. 초기 색은 `UI.Config.EntityPenColors[0]`입니다. |
| `HatchResult` | 생성 경로를 포함하는 패턴 소유 IEntity의 빌린 참조입니다. 생성 전에는 null일 수 있습니다. |
| `HatchRepeats` | 채움 반복 횟수이며 기본값은 1입니다. 양수를 사용하며 내장 PropertyGrid 범위는 1–100입니다. |
| `IsModified` | 생성 경로의 재생성이 필요함을 나타냅니다. 형상에 영향을 주는 패턴 설정자가 이 값을 설정합니다. |
| `Tag` | 응용 프로그램 데이터이며 내장 JSON 직렬화에서 제외합니다. |
| `PropertyChanged` | INotifyPropertyChanged에서 상속한 속성 알림입니다. |
| `Regen(hatchable, contours, owner)` | 닫힌 로컬 XY 컨투어에서 채움 경로를 생성하고 Boolean 결과를 반환합니다. |
| `Clone()` / `Dispose()` | 내장 패턴과 생성 결과의 독립 복제본을 만들거나 패턴과 결과를 해제합니다. |

패턴을 엔티티에 추가한 뒤 엔티티를 재생성하는 방식을 권장합니다. `IHatch.Regen`을 직접 호출하면 유효한 로컬 bounds와 구멍을 포함한 닫힌 XY 컨투어를 제공해야 합니다. 입력 컨투어는 호출자가 소유하며 변경하지 않습니다.

## 3. HatchLine 설정

표는 `new HatchLine()`의 기본값입니다. `HatchFactory.CreateLine(angle, interval, exclude = 0.05, isZigZag = true)`은 전달한 설정을 적용합니다. 인수 순서는 **각도, 간격**입니다.

| 설정 | 기본값 | 효과 |
|---|---:|---|
| `Angle` | 0° | 원본 로컬 XY 평면에서 각 선의 방향입니다. |
| `Interval` | 0.05 mm | 선에 수직인 기준 간격입니다. 0.001 mm보다 작은 값은 0.001 mm로 보정합니다. |
| `Exclude` | 0.05 mm | 구멍 경계를 포함한 안쪽 여백입니다. 큰 값은 좁은 영역을 소멸시킬 수 있습니다. |
| `Alignment` | `Center` | 투영 범위 안에서 선 시퀀스의 배치를 정합니다. |
| `Shift` | 0 mm | 수직축 방향의 주기적 위상입니다. 엔티티 위치나 선을 따르는 이동값이 아닙니다. |
| `IsZigZag` | `true` | 연속 해치 밴드의 방향을 번갈아 바꿉니다. |
| `Order` | `Ascending` | 낮은 투영 좌표에서 높은 좌표로 진행합니다. Descending은 기본 순서를 뒤집습니다. |
| `Sort` | `Nearest` | 경로 순서 최적화 방식을 정합니다. |
| `MinAdjacentDistance` | 0 mm | 짧은 점프에 최적화 패널티를 추가합니다. 경로를 삭제하거나 형상을 바꾸지 않습니다. |
| `Fill` | `EvenOdd` | 경계 오프셋·선 교차 전에 채워진 영역을 판정합니다. |
| `Joint` | `Square` | 경계 오프셋의 모서리 형상입니다. |
| `IsSnapToGrid` | `false` | 첫 수직 좌표를 그리드에 맞추고 선 시퀀스를 투영 범위 안에 유지합니다. |
| `SnapToGridInterval` | 0.001 mm | 그리드 간격이며 해치 선 간격과 별개입니다. |

`Alignment.None`은 최소 투영 좌표에서 시작합니다. Center는 기준 간격을 유지하고 시퀀스를 중앙에 배치합니다. Fit은 투영 범위에 맞게 실제 간격을 다시 계산합니다. 그리드 스냅은 경계 안에 유지하기 위해 선 수를 줄일 수 있습니다.

간격과 여백은 로컬 mm 기준입니다. 엔티티·그룹의 배율 변환은 실제 가공 경로도 확대·축소하며 카메라 확대는 표시만 바꿉니다. `Exclude = 0`이어도 작은 수치적 경계 여백이 남으므로 외곽선을 그대로 재현하는 명령은 아닙니다.

## 4. HatchPolygon 설정

| 설정 | 기본값 | 효과 |
|---|---:|---|
| `Interval` | 0.1 mm | 연속 셸 사이의 안쪽 오프셋이며 최소값은 0.001 mm입니다. |
| `Exclude` | 0 mm | 초기 여백입니다. 0일 때는 첫 셸 전에 Interval 크기의 사전 인셋도 수행합니다. |
| `Joint` | `Square` | 매 인셋의 모서리 형상입니다. |
| `Order` | `Descending` | 기본 셸 순서는 외부→내부이며 Ascending은 내부 셸부터 시작합니다. |
| `Sort` | `Nearest` | 경로·그룹의 점프를 줄이도록 순서를 정합니다. |
| `MinArea` | 0.005 mm² | 절대 다각형 면적을 기준으로 작은 셸 경로를 제외합니다. |

`HatchFactory.CreatePolygon(interval, exclude = 0)`은 간격과 여백을 지정합니다. MinArea는 생성자가 초기화하며 이후 Interval을 바꿔도 자동으로 다시 계산하지 않습니다. 0이어도 구현의 작은 간격 종속 면적 허용값을 적용합니다. 큰 값은 유효한 작은 형상도 제거할 수 있습니다.

폴리곤 해치는 NonZero로 입력 영역을 판정합니다. HatchPolygon에는 Fill, Angle, Alignment 속성이 없습니다. 셸은 컨투어 형상을 따라 생성하며 원형에 한정되지 않습니다.

## 5. 채움 규칙, 구멍, 모서리와 정렬

| HatchFills | 내부 판정 |
|---|---|
| `EvenOdd` | 교차 횟수의 홀짝으로 판정하며 중첩 영역은 채움과 빈 영역을 번갈아 사용합니다. |
| `NonZero` | winding number가 0이 아닌 영역입니다. 컨투어 방향이 구멍·중첩 영역에 영향을 줍니다. |
| `Positive` / `Negative` | 양수 / 음수 winding number 영역만 사용합니다. |

`GetOriginalContours`는 채워진 영역의 위상 관계를 제공합니다. 비슷하게 보이는 외곽선 목록만으로 충분하지 않습니다. 열린 경로를 강제로 닫지 않습니다. 일반적인 NonZero 영역은 외곽과 구멍의 방향을 반대로 구성합니다. 메쉬 Slice 결과는 외곽 CCW·구멍 CW입니다. 중첩으로 구멍을 정의할 때 Contour.NormalizeAndNest를 사용할 수 있지만 입력 방향 자체에 의미가 있으면 채움 판정 전에 정규화하지 않습니다.

Miter, Square, Bevel, Round는 오프셋의 모서리 처리를 선택합니다. 여백·Joint 변경 후 예각, 좁은 형상과 구멍을 확인합니다.

Sort.None은 패턴 생성기의 기본 순서를 사용합니다. Nearest와 Global은 휴리스틱 정렬이며 Global도 수학적인 최단 경로를 보장하지 않습니다. 최적화 작업량은 제한하며 생성 경로 전체를 유지합니다. 기본 밴드·셸 순서가 중요하면 None을 사용하고 최적화를 켜면 실제 진행 순서를 확인합니다.

## 6. 외곽선·채움 가공 순서와 Pen

| HatchMarkOptions | 엔티티 가공 순서 |
|---|---|
| `HatchLast` | 외곽선→채움이며 일반적인 내장 해치 가능 엔티티의 기본값입니다. |
| `HatchFirst` | 채움→외곽선입니다. |
| `HatchOnly` | 외곽선을 제외하고 채움만 가공합니다. |

HatchMarkOption은 엔티티 설정이며 HatchLine.Order와 HatchPolygon.Order는 각 패턴 설정입니다. Hatches 목록은 패턴 순서를 정하고 HatchRepeats는 각 패턴을 반복합니다. 엔티티와 Marker 반복 설정은 별도 범위를 사용합니다.

채움을 사용하려면 IsAllowHatch를 켭니다. 외곽선 가공을 생략하려면 HatchOnly를 사용합니다. 엔티티의 IsAllowMark를 끄면 채움을 포함한 가공을 제외합니다. 활성 채움 경로가 없는 HatchOnly 엔티티에는 가공할 채움 경로가 없습니다.

패턴의 ModelColor를 문서에서 사용할 EntityPen 색과 맞추고 해당 Pen의 Power, Speed와 공정 조건을 설정합니다. 해치 Pen과 외곽선 Pen을 다르게 사용할 수 있습니다. [EntityPen](PenUserManual.koKR.md)과 [Marker](MarkerUserManual.koKR.md)를 참고합니다.

## 7. 문서 작업으로 패턴 추가

예제는 초기화되고 대기 상태인 문서와 문서가 소유한 고정된 target을 전제로 하며 소유 UI 스레드에서 실행합니다. ActAddHatch는 일반 문서 작업에서 재생성·알림을 수행하고 Undo/Redo를 지원합니다.

```csharp
using System.Linq;
using SpiralLab.Sirius3.Entity;
using SpiralLab.Sirius3.Entity.Hatch;

if (target is IHatchable hatchable && hatchable.IsClosed)
{
    var hatch = HatchFactory.CreateLine(angle: 45, interval: 0.1, exclude: 0.05);
    hatch.Alignment = HatchAlignments.Center;
    hatch.Sort = HatchSorts.None;
    hatch.HatchRepeats = 1;

    bool added = document.ActAddHatch(target, hatch);
    if (!added && !hatchable.Hatches.Contains(hatch))
        hatch.Dispose();
}
```

목록 포함 여부를 확인하여 연결 후 작업이 실패한 패턴을 임의로 해제하지 않습니다. Boolean 결과와 로그를 확인합니다. 성공 후에는 엔티티·문서 이력이 소유하므로 연결한 인스턴스를 호출자의 using 범위에서 해제하지 않습니다.

두 방향으로 채우려면 두 패턴을 한 문서 작업으로 추가합니다.

```csharp
if (target is IHatchable hatchable && hatchable.IsClosed)
{
    IHatch[] patterns =
    {
        HatchFactory.CreateLine(0, 0.1, 0.05),
        HatchFactory.CreateLine(90, 0.1, 0.05)
    };
    bool added = document.ActAddHatches(target, patterns);
    if (!added)
        foreach (var pattern in patterns)
            if (!hatchable.Hatches.Contains(pattern)) pattern.Dispose();
}
```

ActInsertHatch, ActRemoveHatch, ActRemoveHatches, ActMoveHatches, ActReplaceHatches, ActHatchClear는 해당 목록 작업을 문서 이력으로 처리합니다. HatchMarkOption 등 엔티티 속성 편집에는 ActPropertyChanged를 사용합니다.

직접 IHatchable.AddHatch·컬렉션 메서드를 사용하면 문서 추가 전 호출자 소유 엔티티를 준비할 수 있습니다. 문서 Undo/Redo 작업은 생성하지 않으며 파라미터·목록 변경 후 엔티티를 재생성합니다. 패턴 설정자를 호출하는 것만으로 가공 작업을 제출하지 않습니다.

## 8. WinForms와 WPF 편집

Editor 또는 TreeView에서 엔티티를 선택하고 해치를 활성화한 뒤 PropertyGrid의 Hatches 편집기를 엽니다. Line 또는 Polygon을 추가하고 그 패턴을 선택하여 설정합니다. Remove/Clear와 Up/Down으로 목록을 관리합니다.

WinForms의 Apply는 드롭다운을 유지하며 미리보기를 갱신합니다. 수락하면 편집을 확정하고 미리보기 후 수락하지 않고 닫으면 이전 값을 복원합니다. WPF는 분리된 복제 목록을 편집하고 수락할 때 반영합니다. 두 프레임워크 모두 엔티티 재생성과 문서 편집 흐름을 사용합니다. [WinForms](WinForms.koKR.md), [WPF](WPF.koKR.md), [PropertyGrid](PropertyGridUserManual.koKR.md)를 참고합니다.

## 9. 재생성, 결과와 소유권

형상·해치 변경 후 재생성하고 반환값을 확인합니다. Marker는 준비가 완료된 해치 데이터를 사용하며 변경된 결과를 출력 중 재생성하는 대신 오류로 보고합니다. 빈 형상·열린 경로는 생성 채움만 폐기하고 패턴 설정은 보존하여 이후 유효한 형상에서 다시 생성합니다.

HatchResult는 빌린 참조입니다. 다른 엔티티에 그대로 연결하거나 별도로 Dispose하지 않습니다. 내장 Clone은 패턴의 형상 설정과 독립 생성 결과를 복제합니다. 사용자 지정 Name·Tag가 필요하면 명시적으로 복사합니다. 생성 결과와 런타임 Tag·IsModified는 일반 해치 파라미터로 저장하지 않으며 문서 재생성에서 경로를 다시 만듭니다.

엔티티는 연결한 패턴을 소유합니다. 직접 Remove/Replace는 제거한 패턴을 해제하지 않으며 Undo/Redo 소유자가 사용할 수 있습니다. ClearHatches(true)는 제거한 패턴을 해제하고 false는 호출자·이력에 정리를 맡깁니다. 독립 엔티티 사이에 하나의 변경 가능한 패턴을 공유하지 말고 복제합니다.


## 10. SVG, 클리핑과 문제 확인

새 SVG 가져오기는 UI.Config.IsSvgImportHatchEnabled가 true이면 SvgImportHatchInterval(기본 0.1 mm)과 그 절반의 경계 여백으로 HatchLine을 추가할 수 있습니다. 새 가져오기에만 적용하며 기존 문서 패턴은 독립적으로 편집합니다. [가져오기 설정](Sirius3UIConfigUserManual.koKR.md)을 참고합니다.

ClipOptions.IncludeHatches는 기존 활성 HatchLine/HatchPolygon 가공 경로를 독립 클리핑 결과로 포함합니다. 각 타일에서 설정을 다시 적용하지 않고 캡처한 경로를 유지합니다. [클리핑](ClippingUserManual.koKR.md)을 참고합니다.

채움이 없으면 닫힌·비어 있지 않은 형상, 활성화, 채움 규칙·구멍 방향, 여백, 폴리곤 최소 면적, 양의 유한 간격, 재생성 결과와 Pen을 확인합니다. 지나치게 조밀한 선 시퀀스는 할당 전에 거부합니다. 미리보기만으로 공정 에너지나 장치 실행을 검증할 수는 없습니다.

공개 예제: [editor_hatch](../demos/editor_hatch/Form1.cs), [editor_clip](../demos/editor_clip/Form1.cs). 관련 문서: [엔티티](EntityUserManual.koKR.md), [텍스트](TextUserManual.koKR.md), [바코드](BarcodeUserManual.koKR.md).

---
2026 Copyright (c) SpiralLAB. All rights reserved.
