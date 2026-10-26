# 레이저 파워 매핑 및 보정 사용자 매뉴얼

> API 기준: Sirius3 Core/UI 1.17.0 (공개 기능)

[English](PowermapUserManual.md) | [한국어](PowermapUserManual.koKR.md) | [전체 안내서](README.koKR.md)

본 문서는 Sirius3의 공개 PowerMap API와 WinForms/WPF 컨트롤을 설명합니다.

## 1. 개요

PowerMap은 레이저에 지시한 출력 X와 실측 광학 출력 Y 사이의 비선형 관계를 보정합니다. 두 값의 단위는 W입니다. 매핑은 X/Y 측정값을 기록하고, `LookUp`은 요청한 실측 출력 Y를 얻는 데 필요한 입력 X를 역산합니다.

각 카테고리는 독립적인 측정 테이블을 가집니다. 주파수, 레이저 헤드 또는 다른 보정 조건을 카테고리로 구분할 수 있습니다. 예를 들어 문자열 `"50000"`은 50 kHz에서 측정한 테이블을 식별할 수 있지만, 카테고리 키 자체가 레이저 주파수를 설정하지는 않습니다.

## 2. 구성 요소와 매핑 절차

- `IPowerMap`: 카테고리별 테이블과 역산 조회를 관리합니다.
- `IPowerMeter`: 광학 출력을 측정합니다.
- `ILaserPowerControl`: 출력 명령을 적용하며 연결된 PowerMap으로 보정할 수 있습니다.

1. Scanner, Laser, PowerMeter를 연결하고 설정합니다. 센서를 사용할 측정 위치에 배치하고 냉각 상태와 측정 범위를 확인합니다.
2. PowerMapControl에서 Mapping을 시작합니다. 카테고리, 입력 출력 범위와 단계 수를 선택합니다. 각 단계에서 레이저가 출력되며 실측 출력이 기록됩니다.
3. 아래 설명에 따라 전역 보간 모드를 선택한 뒤, 실제 사용할 목표 출력에서 Verify를 실행합니다. 실측값과 허용 오차 범위를 비교합니다.
4. 출력에 드리프트가 발생하면 Compensate를 실행합니다. 이 작업은 출력을 측정하여 테이블을 갱신하며 레이저가 출력될 수 있습니다.

예열과 안정화 시간은 `Config.PowerMapPreHeatTimeMs`와 `Config.PowerMapHoldTimeMs`로 설정합니다. `Config.PowerMapInRangeThreshold`는 검증에 사용할 허용 범위를 백분율로 지정합니다. 보정 품질은 실제 측정으로 확인해야 하며, 그래프가 매끄럽다는 사실만으로 출력 정확도가 보장되지는 않습니다.

## 3. 전역 Linear/PCHIP 보간

`Config.PowerMapInterpolationMode`는 기본 제공 PowerMap 인스턴스와 모든 카테고리에 적용하는 하나의 전역 설정입니다. 카테고리별 측정값은 독립적이지만 보간 모드를 카테고리마다 지정하지는 않습니다. 보정 출력을 검증하거나 사용하기 전에 모드를 설정합니다.

| 모드 | 동작 |
|---|---|
| `PowerMapCurveModes.Linear` | 인접 측정점을 직선 구간으로 연결합니다. 기본값입니다. |
| `PowerMapCurveModes.Pchip` | 측정점을 통과하는 형태 보존 3차 보간을 사용합니다. 측정점이 두 개이면 선형 보간과 같습니다. |

```csharp
using SpiralLab.Sirius3;
using SpiralLab.Sirius3.PowerMap;

Config.PowerMapInterpolationMode = PowerMapCurveModes.Pchip;
// 기본 구간별 선형 보간을 사용하려면 다음과 같이 설정합니다.
// Config.PowerMapInterpolationMode = PowerMapCurveModes.Linear;
```

PCHIP은 측정된 증가·감소 특성을 유지합니다. 측정 노이즈를 자동으로 평활화하거나 증가 곡선으로 강제하지 않으며, 여러 역산 해가 발생하는 특성도 제거하지 않습니다.

WinForms와 WPF PowerMapControl은 선택된 전역 모드로 보정 곡선을 표시합니다. 외부에서 직접 구현한 `IPowerMap`은 해당 구현체의 조회 동작을 사용합니다.

## 4. 공개 LookUp API와 다중 해

`map.IsLookUpEnable = true`로 보정을 활성화합니다. 공개 API는 다음과 같습니다.

```csharp
bool success = map.LookUp(category, targetYWatt,
    out double xWatt, out double xLeftWatt, out double xRightWatt);
```

- `targetYWatt`: 요청한 실측 출력 Y이며 단위는 W입니다.
- `xWatt`: 선택된 입력 X이며 소수점 세 자리로 반올림합니다.
- `xLeftWatt`, `xRightWatt`: 선택한 해를 포함하는 원본 측정 구간의 X 값입니다.
- 해가 여러 개이면 기본 정책은 가장 작은 X를 선택합니다. 목표 Y와 같은 수평 구간에서는 왼쪽 끝점을 선택합니다.
- 최소 두 개의 측정점이 필요합니다. 카테고리가 없거나 목표 출력이 보정 범위를 벗어나면 실패합니다. PCHIP은 반올림한 입력이 예측 허용오차를 만족하지 못하는 경우에도 실패합니다. 반환값이 true인지 확인한 후 출력 인자를 사용합니다.

다음 예제는 설명용 데이터를 입력하고 계산만 수행하며 레이저를 동작시키지 않습니다.

```csharp
using System;
using SpiralLab.Sirius3;
using SpiralLab.Sirius3.PowerMap;

Config.PowerMapInterpolationMode = PowerMapCurveModes.Pchip;
IPowerMap map = new PowerMapDefault(0, "Calibration");
map.IsLookUpEnable = true;
map.Update("50000", 0, 0);
map.Update("50000", 10, 10);
map.Update("50000", 20, 0);

if (map.LookUp("50000", 5, out double x, out double left, out double right))
    Console.WriteLine($"X={x:F3} W, interval=[{left:F3}, {right:F3}] W");
else
    throw new InvalidOperationException("Power lookup failed.");
```

이 테이블은 Y = 5 W에 대해 두 개의 해를 가집니다. PCHIP의 기본 반환값은 [0, 10] W 구간의 X = 2.929 W입니다. 전역 모드를 Linear로 바꾸면 X = 5.000 W를 반환합니다. 작은 입력 선택은 기본 규칙이며, 히스테리시스나 출력 감소 특성이 있는 레이저에서 어느 물리적 구간이 적합한지를 판정하는 기능은 아닙니다.

`Update`는 X와 Y를 소수점 세 자리로 반올림하며 같은 X가 있으면 측정값을 교체합니다. `Query`는 저장된 X 키의 값을 조회하며 보간하지 않습니다. `IsLookUpEnable = false`이면 역산 보정을 우회합니다.

## 5. 곡선과 Power 미리보기

WinForms와 WPF PowerMapControl은 `Config.PowerMapInterpolationMode`에 따라 측정점과 보정 곡선을 표시합니다. 이 전역 설정은 애플리케이션에서 지정하며, 컨트롤에는 별도의 곡선 설정·조회 창을 제공하지 않습니다. 역산 조회에는 기본 최소 X 선택 정책을 사용합니다.

`EntityPen`에 PowerMap이 연결되어 있고 유효한 `PowerMapCategory`가 설정되어 있으면 PropertyGrid의 Power 편집기에서 해당 카테고리의 작은 그래프를 미리 볼 수 있습니다. ManualControl도 연결된 맵과 선택한 카테고리에 대한 미리보기를 제공합니다. 미리보기는 곡선과 목표·해 오버레이를 표시하며, 파워 값을 바꾸거나 레이저 명령을 실행하지 않습니다. 숫자 파워 편집은 계속 사용할 수 있습니다.

## 6. 실제 가공에 보정 적용

레이저의 `ILaserPowerControl.PowerMap`에 맵을 연결하고 `map.IsLookUpEnable`을 활성화한 뒤, 정상적인 레이저·스캐너 리스트 처리 과정에서 정확한 카테고리 키를 `ListPower(targetWatt, category)`에 전달합니다. 목표값은 원하는 출력 Y이며, 파워 명령을 리스트에 기록하기 전에 PowerMap이 보정 입력 X를 계산합니다. `CtlPower(targetWatt, category)`는 즉시 제어 명령입니다.

편집기 문서에서는 해당 `EntityPen`에 맵과 일치하는 `PowerMapCategory`를 연결합니다. 카테고리 이름만 지정해도 맵이 연결되는 것은 아닙니다. 애플리케이션의 정상적인 장치 초기화, 리스트 제출과 완료 확인 절차를 따릅니다. 조회와 명령의 반환값을 확인해야 하며, 제어기 타이밍과 출력 동작은 레이저, 스캐너 및 파워 제어 방식 설정에 따라 달라집니다.

## 7. 테이블 저장과 로드

`PowerMapSerializer.Save`와 `PowerMapSerializer.Open`은 기존 카테고리별 X/Y 테이블 형식을 사용합니다. `.map` 파일에는 원본 측정값을 저장하며, 보간 모드·곡선 설정·계산된 계수는 저장하지 않습니다.

```csharp
if (!PowerMapSerializer.Save("calibration.map", map))
    throw new InvalidOperationException("Power-map save failed.");

IPowerMap loaded = new PowerMapDefault(1, "Loaded calibration");
if (!PowerMapSerializer.Open("calibration.map", loaded))
    throw new InvalidOperationException("Power-map load failed.");
loaded.IsLookUpEnable = true;
```

로드는 애플리케이션의 현재 전역 보간 모드를 유지하며 테이블에서 곡선을 다시 만들고 기본 최소 X 선택 정책을 사용합니다. 애플리케이션은 `.map` 파일과 별도로 전역 보간 모드를 설정해야 합니다.

로드에 실패하면 기존 테이블과 실행 중 설정은 유지됩니다. 맵이 동작 중이지 않을 때 저장하거나 로드합니다.

## 8. 1:1 리셋과 측정 주의사항

`Reset1to1(category, maxXWatt)`는 양수인 최대 입력값에 대해 해당 카테고리를 (0, 0)과 (maxXWatt, maxXWatt)의 두 점으로 교체합니다. 이는 Y = X 테이블로 되돌리는 기능이며 레이저를 측정하거나 재보정하지 않습니다. 보정 활성화·우회는 `IsLookUpEnable`로 선택합니다.

- Mapping, Verify와 Compensate는 레이저를 출력할 수 있습니다. 장비의 정상 작업 절차를 따르고 센서의 허용 출력·에너지 범위를 준수합니다.
- 충분한 예열·안정화 시간을 확보하고 파워 미터의 측정 단위와 범위가 올바른지 확인합니다.
- 주파수, 광학 경로와 측정 위치를 실제 가공 조건과 일치시킵니다. 매핑 전에 스캐너 보정과 광학계 상태를 확인합니다.
- 보간 모드나 측정값을 변경하면 사용할 목표 파워에서 다시 검증합니다. 광학계 오염과 레이저 에이징은 기존 보정의 유효성에 영향을 줄 수 있습니다.

## 9. 관련 문서

- [WinForms 연동](WinForms.koKR.md)
- [WPF 연동](WPF.koKR.md)
- [English manual](PowermapUserManual.md)

---
2026 Copyright (c) SpiralLAB. All rights reserved.
