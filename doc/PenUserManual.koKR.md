# EntityPen 사용자 설명서

> 기준 버전: Sirius3 1.12.3 (공개 Release 기능)

`EntityPen`은 개별 개체를 가공할 때 사용할 레이저 출력, 펄스, 스캐너 속도, 지연 시간과 선택 기능을 한곳에 모아 둔 설정입니다. 편집기에서 개체의 선 색을 바꾸면 같은 색의 `EntityPen`이 연결되며, Marker는 해당 색을 처음 만날 때 RTC 리스트 버퍼에 펜 설정 명령을 기록합니다.

레이어 전체에 먼저 적용되는 ALC, Sky Writing, Variable Delay, syncAXIS 동작 모드는 [LayerPenUserManual.md](LayerPenUserManual.koKR.md)를 참고하십시오.

## EntityPen이 적용되는 위치

한 개체가 실제로 가공되기까지의 흐름은 다음과 같습니다.

1. 개체의 `PenColor`를 확인합니다.
2. Document에서 같은 색의 `EntityPen`을 찾습니다.
3. 직전에 사용한 펜과 색이 달라졌다면 레이저 출력, 지연 시간, 속도 등의 List 명령을 RTC 리스트 버퍼에 기록합니다.
4. 이어서 개체의 Jump, Mark, Arc 또는 Raster 명령을 기록합니다.
5. 완성된 리스트 버퍼를 RTC가 순서대로 실행합니다.

같은 펜 색의 개체가 연속되면 불필요한 펜 명령을 반복해서 기록하지 않습니다. 서로 다른 색을 짧은 간격으로 지나치게 자주 교차 배치하면 리스트 명령 수가 늘어날 수 있으므로, 같은 공정 조건의 개체는 가능한 한 같은 색으로 묶는 편이 좋습니다.

> `EntityPen`의 색은 화면 장식용 색상이 아니라 가공 조건을 찾는 키입니다. 임의의 ARGB 색을 사용하면 같은 색으로 등록된 펜이 없을 수 있습니다.

## 편집기에서 설정하는 방법

1. 편집기에서 가공할 개체를 선택합니다.
2. PropertyGrid에서 개체의 `PenColor`를 원하는 색으로 지정합니다.
3. TreeView 또는 펜 편집 화면에서 같은 색의 `EntityPen`을 선택합니다.
4. PropertyGrid에서 출력, 펄스, 속도, 지연 시간과 선택 기능을 설정합니다.
5. Marker 및 Scanner 탭에서 장치가 준비되었는지 확인합니다.
6. 레이저 출사를 막은 안전 상태에서 Preview 또는 시뮬레이션으로 이동 경로를 먼저 확인합니다.
7. 시험편에서 낮은 출력으로 결과를 확인한 뒤 공정값을 단계적으로 조정합니다.

PropertyGrid에는 연결된 RTC와 레이저가 지원하는 항목만 표시됩니다. SCANAhead Auto Delay를 사용하면 일반 레이저·스캐너 지연 항목처럼 더 이상 직접 사용하지 않는 속성은 숨겨질 수 있습니다.

## 주요 속성 한눈에 보기

| 분류 | 주요 속성 | 역할 |
|---|---|---|
| 레이저 출력 | `PowerMax`, `Power`, `PowerPercentage`, `PowerMapCategory` | 정격 출력과 실제 명령 출력을 결정합니다. |
| 펄스 | `Frequency`, `PulseWidth`, `PulsePeriod`, `PulsePitch`, `PulseDutyCycle` | 반복 주파수, 펄스 폭과 공간 간격을 설정합니다. |
| 레이저 지연 | `LaserOnDelay`, `LaserOffDelay` | 스캐너 이동과 레이저 출사 시점을 맞춥니다. |
| 스캐너 | `JumpSpeed`, `MarkSpeed`, `ScannerJumpDelay`, `ScannerMarkDelay`, `ScannerPolygonDelay` | 비가공 이동, 가공 이동과 각 동작의 안정화 시간을 설정합니다. |
| Hard Jump | `IsHardJump` | 지원 RTC에서 일반 Jump 대신 Hard Jump를 사용합니다. |
| Raster | `RasterMode`, `PixelTime`, `PixelPulses`, `PixelPeriod` 등 | 이미지 픽셀을 Jump and Shoot 또는 Micro Vector 방식으로 가공합니다. |
| Wobbel | `IsWobbelEnabled`, `WobbelShape`, `WobbelFrequency`, 진폭 속성 | 기본 Mark 경로에 주기적인 횡·종 방향 운동을 합성합니다. |
| SCANAhead | `LaserOnShiftSCANa`, `LaserOffShiftSCANa`, Scale 속성, `SpotDistanceSCANa` | SCANAhead 궤적 예측과 출사 시점·등간격 펄스를 조정합니다. |
| syncAXIS | `MinMarkSpeed`, `ApproxBlendLimit` | 스캐너와 스테이지가 함께 움직일 때 저속과 블렌딩을 제한합니다. |

## 식별 정보와 공통 속성

### Name과 Description

`Name`은 펜 목록에 표시할 이름이고 `Description`은 공정 조건이나 용도를 기록하는 설명입니다. 기본 펜은 색상 이름을 사용하지만, 실제 프로젝트에서는 `White - Outline`, `Yellow - Hatch`처럼 역할이 드러나는 설명을 함께 기록하면 문서 검토와 현장 인수인계가 쉬워집니다.

### PenColor

`PenColor`는 개체와 EntityPen을 연결하는 정확한 ARGB 색입니다. 기본 편집 화면에서는 식별값의 일관성을 위해 읽기 전용으로 표시될 수 있으며, 개발자는 펜 생성 이벤트에서 지정할 수 있습니다. 화면에서 비슷해 보이는 색이라도 ARGB 값이 다르면 다른 펜으로 처리됩니다.

### ExtensionData

사용자 애플리케이션이 공정 번호, 재료 코드 또는 외부 레시피 키 같은 추가 정보를 보관하는 확장 데이터입니다. `OnMarkEntityPen`에서 읽어 사용자 규칙을 적용할 수 있지만, 기본 Marker는 그 의미를 해석하지 않습니다.

### IsAllowMark와 Repeats

`EntityControl`에서 상속된 값이지만 EntityPen의 표준 PropertyGrid에서는 숨겨지며 펜 실행 여부나 반복 횟수를 결정하는 용도로 사용하지 않습니다. 개체의 가공 허용 여부와 반복은 해당 Entity/Layer 및 Marker 설정에서 관리하십시오.

## 레이저 출력

### PowerMax

`PowerMax`는 연결된 `ILaser.MaxPowerWatt`에서 가져오는 레이저 정격 최대 출력이며 단위는 W입니다. 읽기 전용 값이므로 사용자가 직접 설정하지 않습니다. 값이 0이거나 레이저가 아직 연결되지 않았다면 출력 백분율을 실제 W 값으로 변환할 기준이 없습니다.

### Power

`Power`는 가공에 요청할 실제 출력이며 단위는 W입니다. `PowerMax`가 0보다 클 때는 최대 출력보다 큰 값을 입력해도 `PowerMax` 이하로 제한됩니다.

다음 항목을 함께 확인하십시오.

- 레이저가 실제로 지원하는 최소·최대 출력 범위
- 선택한 주파수와 펄스 폭에서 허용되는 평균 출력
- 소재 손상, 발화 및 반사광 위험
- PowerMap 보정 사용 여부

### PowerPercentage

`PowerPercentage`는 `Power`를 `PowerMax`에 대한 백분율로 보여 주고 설정하는 보조 속성입니다.

`Power = PowerMax × PowerPercentage ÷ 100`

예를 들어 `PowerMax`가 20 W이고 `PowerPercentage`가 25%이면 `Power`는 5 W입니다. `PowerMax`가 유효하지 않으면 백분율을 W로 환산할 수 없으므로 먼저 레이저 등록 상태를 확인하십시오.

### PowerMapCategory

`PowerMapCategory`는 출력 보정에 사용할 분류 이름입니다. 연결된 레이저가 `ILaserPowerControl.PowerMap`을 제공하고, 선택한 이름이 `IPowerMap.Categories`에 등록되어 있어야 합니다. 렌즈, 파장, 광학 경로 또는 공정 헤드가 달라 보정 곡선을 구분해야 할 때 사용합니다.

보정값 생성, 검증, 보상 원리와 사용자 정의 `IPowerMap`은 [PowermapUserManual.md](PowermapUserManual.koKR.md)를 참고하십시오.

## 펄스와 주파수

### Frequency

`Frequency`는 초당 펄스 반복 횟수이며 단위는 Hz입니다. 주파수가 높아지면 같은 속도에서 펄스 간 거리는 짧아집니다. 단, 레이저의 출력 제어 방식이 Frequency 기반이면 이 값은 단순 반복률이 아니라 출력 명령의 일부가 될 수 있습니다.

### PulseWidth

`PulseWidth`는 한 펄스가 활성 상태로 유지되는 시간이며 단위는 µs입니다. 레이저의 출력 제어 방식이 Duty Cycle 기반이면 `ILaser` 구현이 출력값을 펄스 폭으로 변환할 수 있으므로, 최종 출력 파형은 레이저 구현과 장치 사양을 함께 확인해야 합니다.

### PulsePeriod

`PulsePeriod`는 한 펄스 주기의 길이이며 단위는 µs입니다.

`PulsePeriod = 10⁶ ÷ Frequency`

예를 들어 50 kHz는 20 µs 주기에 해당합니다. `PulsePeriod`를 변경하면 내부적으로 `Frequency`가 다시 계산됩니다.

### PulsePitch

`PulsePitch`는 일정한 속도로 Mark할 때 인접 펄스 사이의 이론적인 거리이며 단위는 µm입니다.

`PulsePitch = MarkSpeed × 1000 ÷ Frequency`

예를 들어 `MarkSpeed = 500 mm/s`, `Frequency = 50 kHz`이면 펄스 간격은 약 10 µm입니다. 실제 소재 위의 간격은 스캐너 가감속, 레이저 응답, SCANAhead/ALC 설정과 광학계의 영향을 받습니다.

### PulseDutyCycle

`PulseDutyCycle`은 한 주기에서 레이저 펄스가 차지하는 비율입니다.

`PulseDutyCycle = PulseWidth ÷ PulsePeriod × 100`

듀티비가 장치 허용 범위를 넘지 않도록 레이저 제조사 사양을 확인하십시오. 이 값을 설정하면 `PulseWidth`가 다시 계산됩니다.

### RTC 시간 해상도

`SpiralLab.Sirius3.UI.Config.IsConvertToControllerResolution`이 활성화되면 PropertyGrid에서 읽는 값이 RTC가 표현할 수 있는 시간 단위에 맞춰집니다.

| 항목 | RTC4/RTC4e | RTC5 | RTC6/RTC6e/Virtual |
|---|---:|---:|---:|
| 펄스 주기 | 0.25 µs | 0.03125 µs | 0.03125 µs |
| 펄스 폭 | 0.125 µs | 0.015625 µs | 0.015625 µs |
| 레이저 지연 | 1 µs | 0.5 µs | 0.015625 µs |
| 스캐너 지연 | 10 µs | 10 µs | 10 µs |

입력값과 다시 표시된 값이 조금 다른 것은 오류가 아니라 컨트롤러 해상도에 맞춘 결과일 수 있습니다.

## 스캐너 속도

### JumpSpeed

`JumpSpeed`는 레이저를 끈 상태에서 다음 가공 위치로 이동하는 속도이며 단위는 mm/s입니다. 너무 높으면 이동 시간이 짧아지지만 스캐너의 추종 오차와 진동이 커질 수 있습니다.

### MarkSpeed

`MarkSpeed`는 레이저를 출사하면서 경로를 따라가는 속도이며 단위는 mm/s입니다. 출력, 주파수, 펄스 폭과 함께 단위 길이당 에너지를 결정합니다. 속도만 낮추면 같은 위치에 더 많은 에너지가 들어갈 수 있으므로 출력과 펄스 간격도 함께 검토하십시오.

## 레이저와 스캐너 지연

레이저는 전기 신호를 받은 즉시 안정된 광출력을 만들지 못하고, 스캐너 미러도 명령 위치에 즉시 도달하지 못합니다. 지연값은 이 서로 다른 응답 시간을 맞추기 위한 값입니다.

### LaserOnDelay

Mark 시작점에서 스캐너 이동과 레이저 켜짐을 맞춥니다.

- 너무 작음: 시작점이 진하게 타거나 스캐너가 안정되기 전에 출사될 수 있습니다.
- 너무 큼: 선의 시작 부분이 짧아지거나 빠질 수 있습니다.

### LaserOffDelay

Mark 끝점에서 레이저가 꺼지는 시점을 맞춥니다.

- 너무 작음: 선의 끝부분이 부족할 수 있습니다.
- 너무 큼: 끝점이 진해지거나 과가공될 수 있습니다.

### ScannerJumpDelay

Jump가 끝난 뒤 다음 명령을 시작하기 전 안정화 시간입니다.

- 너무 작음: 다음 선의 시작점이 흔들리거나 위치가 벗어날 수 있습니다.
- 너무 큼: 전체 가공 시간이 불필요하게 늘어납니다.

### ScannerMarkDelay

한 Mark가 끝난 뒤 후속 이동을 시작하기 전의 안정화 시간입니다.

- 너무 작음: 끝점에서 미러가 충분히 따라오지 못해 선이 휘거나 짧아질 수 있습니다.
- 너무 큼: 가공 시간이 늘고 끝점에 열이 집중될 수 있습니다.

### ScannerPolygonDelay

레이저가 켜진 상태에서 연속된 두 Mark가 만나는 꺾인 모서리에 추가하는 시간입니다. 레이저를 끄는 설정이 아닙니다. 방향이 크게 바뀌는 모서리에서 스캐너가 새 방향을 따라갈 시간을 확보하고, 그동안 모서리의 출사 시간을 늘립니다.

- 너무 작음: 모서리가 둥글어지거나 경로 안쪽으로 잘릴 수 있습니다.
- 너무 큼: 모서리가 과하게 진해지고 열이 집중되며 가공 시간이 늘어납니다.

### 지연값 최적화 순서

1. Wobbel, Sky Writing, SCANAhead Auto Delay와 ALC를 끕니다.
2. 낮은 출력과 반복 가능한 시험 도형을 준비합니다.
3. JumpSpeed와 MarkSpeed를 실제 공정값으로 먼저 고정합니다.
4. 선 시작과 끝을 보며 `LaserOnDelay`, `LaserOffDelay`를 조정합니다.
5. 분리된 선 사이의 위치 안정성을 보며 `ScannerJumpDelay`를 조정합니다.
6. 선 끝과 연속 모서리를 보며 `ScannerMarkDelay`, `ScannerPolygonDelay`를 조정합니다.
7. 선택 기능을 하나씩 다시 켜고 결과를 재확인합니다.

SCANAhead Auto Delay가 활성화된 시스템에서는 기존 수동 지연값과 자동 보상을 동시에 임의 조정하지 마십시오. 일반 지연 속성이 편집기에서 숨겨지는 것은 Auto Delay 경로를 사용한다는 뜻입니다.

## Hard Jump

`IsHardJump`는 지원 RTC에서 일반 Jump 명령을 Hard Jump로 변환합니다. Hard Jump는 RTC의 10 µs Microstep 기반 궤적 보간을 거치지 않고 목표 위치로 빠르게 이동한 뒤 `ScannerJumpDelay`만큼 기다립니다.

짧은 비가공 이동이 매우 많은 공정에서 시간을 줄일 수 있지만, 이동 궤적과 가속 제한을 자동으로 보장하지 않습니다. 큰 이동 거리나 높은 속도에서는 미러 진동, 오버슈트 또는 위치 추종 오류가 커질 수 있습니다.

- `IRtcJumpMode`를 구현하는 RTC에서만 동작합니다.
- RTC5/RTC6 계열에서 사용할 수 있으며 RTC4 계열에서는 사용할 수 없습니다.
- 먼저 일반 Jump로 안정적인 조건을 만든 뒤 짧은 이동부터 비교하십시오.
- 사용 시 `ScannerJumpDelay`를 다시 최적화해야 합니다.

## Raster 가공

Raster는 이미지 픽셀을 순서대로 스캔하여 밝기 또는 픽셀별 조건을 레이저 출력으로 표현합니다. `EntityImage`, Raster 모드 바코드처럼 픽셀 기반 개체에 사용합니다.

### RasterMode

#### JumpAndShoot

각 픽셀 위치로 Jump한 뒤 정지 상태에서 레이저를 출사합니다.

- 위치 정확도를 우선할 때 적합합니다.
- 픽셀마다 Jump하므로 처리 시간은 길어질 수 있습니다.
- `JumpSpeed`, `PixelTime`, `PixelPulses`, `IsPixelPulsesExit`가 핵심입니다.
- 일반적인 픽셀 출사는 LASERON 신호를 `PixelTime` 동안 사용합니다.

#### MicroVector

짧은 벡터를 연속적으로 이동하면서 픽셀을 처리합니다. RTC의 Micro Vector 명령을 사용하며 일반적으로 LASERON은 활성 상태를 유지하고 LASER1 펄스 폭 또는 확장 아날로그 출력으로 픽셀 값을 표현합니다.

- JumpAndShoot보다 빠를 수 있지만 위치 정확도와 스캐너 추종 상태를 확인해야 합니다.
- `PixelPeriod`와 `PixelTime`으로 픽셀 주기를 구성합니다.
- `PixelTime`은 `PixelPeriod`보다 작아야 합니다.
- ALC, Timed Mark, Wobbel, Sky Writing과 함께 사용하지 않습니다.
- syncAXIS 경로에서는 MicroVector를 사용하지 않습니다.
- `IRtcRaster`를 지원하는 RTC에서만 동작합니다.

### PixelTime

픽셀당 레이저 활성 시간이며 단위는 µs입니다. 이미지에서는 픽셀 밝기 0~1이 `PixelTime`에 반영됩니다. 너무 길면 픽셀 번짐과 열 누적이 생기고, 너무 짧으면 레이저가 충분히 응답하지 못할 수 있습니다.

### PixelPeriod

MicroVector에서 다음 픽셀로 진행하는 전체 주기이며 단위는 µs입니다. 픽셀 간격이 알려져 있을 때의 이론 속도는 다음과 같습니다.

`속도(mm/s) = 픽셀 간격(mm) ÷ (PixelPeriod(µs) × 10⁻⁶)`

### PixelChannel

MicroVector 픽셀값을 출력할 채널입니다. `ExtAO1`, `ExtAO2`와 같은 확장 아날로그 채널을 사용할 때는 RTC 확장 출력 포트와 레이저 입력의 전압 범위, 극성, 접지를 확인하십시오.

### RasterDirection

- `Horizontal`: 수평 스캔 라인을 사용하고 아래쪽에서 위쪽으로 줄을 진행합니다.
- `Vertical`: 수직 스캔 라인을 사용하고 왼쪽에서 오른쪽으로 줄을 진행합니다.

### IsRasterZigZag

활성화하면 한 줄은 정방향, 다음 줄은 역방향으로 가공해 줄 사이의 복귀 이동을 줄입니다. 장비의 양방향 시간 오차가 크면 줄이 어긋날 수 있으므로 단방향 결과와 비교하십시오.

### PixelPulses

`PixelPulses`는 JumpAndShoot에서 한 픽셀을 외부 레이저 동기 펄스 수로 완료하는 기능입니다.

- `0`: 일반 방식으로 `PixelTime` 동안 LASERON을 유지합니다.
- `1`~`65535`: `PixelTime` 안에 지정한 수의 외부 펄스 에지를 기다립니다.
- 지정 펄스를 받기 전에는 LASERON이 활성화되지 않으며, 펄스가 없으면 해당 픽셀은 출사되지 않습니다.
- 목표 수보다 많은 펄스는 무시됩니다.
- `IsPixelPulsesExit = true`이면 목표 펄스를 모두 받는 즉시 다음 픽셀로 진행합니다.
- `false`이면 목표 펄스를 받았더라도 `PixelTime`이 끝날 때까지 기다립니다.

이 기능에는 외부 레이저 동기 신호 연결이 반드시 필요합니다. 레이저의 `SYNC OUT`을 RTC LASER 포트의 `DIGITAL IN1`에 연결하고 TTL 레벨, 신호 접지와 활성 에지를 확인하십시오. 활성 에지는 `IRtcSignalLevel.CtlLaserControlSignal` 설정과 일치해야 합니다. RTC5 이상과 JumpAndShoot에서 사용합니다.

## Wobbel

Wobbel은 기본 Mark 경로를 따라가면서 작은 주기 운동을 합성하는 기능입니다. 선폭 확대, 에너지 분산, 용융 풀 제어 또는 표면 질감 조정에 사용합니다.

### IsWobbelEnabled

Wobbel 사용 여부입니다. 사용하지 않을 때 Marker는 `ListWobbelEnd`를 기록해 이전 펜의 Wobbel 상태가 다음 개체로 이어지지 않도록 합니다.

### WobbelShape

| 형상 | 설명 | 주의사항 |
|---|---|---|
| `Ellipse` | 진행 방향과 수직·평행 방향의 진폭을 합성합니다. 두 진폭이 같으면 원형에 가깝고, 다르면 타원형이 됩니다. | 평행 진폭이 0이면 진행 방향에 수직인 사인 운동이 됩니다. |
| `Perpendicular8` | 진행 방향의 수직 방향을 중심으로 8자 형태를 만듭니다. | RTC5 이상에서 사용합니다. |
| `Parallel8` | 진행 방향과 평행한 방향을 중심으로 8자 형태를 만듭니다. | RTC5 이상에서 사용합니다. |
| `Defined` | RTC에 미리 정의한 사용자 Wobbel 형상을 사용합니다. | 먼저 `IRtcWobbel.ListWobbelDefine`으로 형상을 등록해야 합니다. |

RTC4는 기본 Wobbel 기능만 지원하며, 형상별 지원 범위는 RTC와 펌웨어에 따라 다릅니다.

### WobbelFrequency

Wobbel 반복 주파수이며 단위는 Hz입니다. 양수와 음수는 회전 방향을 바꾸는 데 사용되며, 구현상 양수는 시계 방향, 음수는 반시계 방향입니다. 절대값은 1000 Hz 미만이어야 하고, 스캐너의 추종 가능한 주파수보다 낮게 설정해야 합니다.

### WobbelPerpendicular와 WobbelParallel

- `WobbelPerpendicular`: 기본 경로의 진행 방향에 수직인 진폭, 단위 mm
- `WobbelParallel`: 기본 경로의 진행 방향과 평행한 진폭, 단위 mm

진폭과 주파수를 동시에 크게 올리면 스캐너가 요구 궤적을 따라가지 못할 수 있습니다. 낮은 주파수와 작은 진폭부터 시작하여 실제 스캐너 상태와 가공 폭을 확인하십시오.

## SCANAhead

SCANAhead는 RTC6가 스캐너의 미래 궤적을 미리 계산하고, 호환 스캔헤드의 상태를 반영해 레이저 출사와 운동을 정밀하게 맞추는 기능입니다.

### 사용 조건

- RTC6와 SCANahead 옵션
- 호환되는 excelliSCAN 또는 intelliSCAN IV 계열 스캔헤드
- 호환되는 RTC/스캔헤드 펌웨어와 시스템 패키지
- SCANAhead에 맞는 Auto Delay 설정
- `rtc.IsSCANAhead == true` 확인

SCANAhead에서는 일반 `PositionAck`가 아니라 궤적 추종 상태인 `TrAck` 의미로 동작합니다. 추종 오류가 래치되면 `IRtc.CtlReset()`이 필요할 수 있습니다.

### LaserOnShiftSCANa와 LaserOffShiftSCANa

SCANAhead가 계산한 기준 시점에서 레이저 켜짐과 꺼짐을 이동합니다.

- 음수 `LaserOnShiftSCANa`: 레이저를 더 일찍 켭니다.
- 양수 `LaserOnShiftSCANa`: 레이저를 더 늦게 켭니다.
- 음수 `LaserOffShiftSCANa`: 레이저를 더 일찍 끕니다.
- 양수 `LaserOffShiftSCANa`: 레이저를 더 늦게 끕니다.

단위는 µs이며 RTC6 해상도는 0.015625 µs입니다.

### CornerScaleSCANa

연속된 Mark가 만나는 모서리의 궤적을 조정하는 비율입니다. 100%는 모서리 형상을 우선하고, 값을 낮추면 더 부드럽고 빠른 궤적을 허용합니다. 실제 Sky Writing 구간에서는 적용되지 않습니다.

### EndScaleSCANa

Mark 끝점의 정확도와 처리 시간 사이의 비율입니다. 100%는 끝점 정확도를 우선하고, 값을 낮추면 종료 동작이 빨라질 수 있지만 끝점 오차가 커질 수 있습니다. Sky Writing 중에도 적용됩니다.

### AccScaleSCANa

가감속 구간 중 레이저가 출사되는 시간 비율입니다. 100%는 가감속 전 구간의 출사를 허용하고, 값을 낮추면 시작과 끝의 출사 구간이 줄어 선 길이가 짧아질 수 있습니다. 실제 Sky Writing 구간에서는 적용되지 않습니다.

### SpotDistanceSCANa

등간격 펄스 제어에서 이동 경로를 따라 펄스를 발생시킬 거리이며 단위는 mm입니다. 광학적인 레이저 스폿 직경이 아닙니다. 0으로 설정하면 SDC 거리 명령을 사용하지 않습니다.

이 기능을 사용하려면 `EntityLayerPen`에서 다음 조건을 함께 구성해야 합니다.

- `IsALC = true`
- `AlcSignal = SpotDistance`
- `AlcMode = ActualVelocity`
- `AlcModeExtension`에 `SCANAhead` 추가
- SCANAhead Auto Delay 활성화
- 레이저의 외부 PoD 또는 등간격 펄스 입력 연결

Sky Writing 중에도 SDC를 유지하려면 `SkyWritingSDC` 확장 비트가 추가로 필요합니다. 전체 설정은 [LayerPenUserManual.md](LayerPenUserManual.koKR.md)의 ALC 절을 참고하십시오.

## syncAXIS용 개체 속성

### MinMarkSpeed

스캐너와 스테이지를 함께 운용할 때 허용할 최소 Mark 속도이며 단위는 mm/s입니다. 0보다 큰 값일 때만 `IRtcSyncAxis.ListSpeedMinMark` 명령이 기록됩니다.

### ApproxBlendLimit

근사 블렌딩 허용 한계이며 단위는 mm입니다. 0보다 큰 값일 때만 `IRtcSyncAxis.ListApproxBlendLimit` 명령이 기록됩니다.

두 속성은 일반 `MarkerRtc`가 아니라 syncAXIS용 Marker와 `IRtcSyncAxis`에서 사용합니다. 사전 셋업, 라이선스와 SCANLAB USB 동글이 필요하므로 [Rtc6SyncaxisUserManual.md](Rtc6SyncaxisUserManual.koKR.md)를 먼저 확인하십시오.

## 표준 기본값

Sirius3 편집기가 기본 펜을 만들 때 적용하는 대표값입니다. 장치와 공정에 맞는 안전값을 의미하지는 않습니다.

| 속성 | 기본값 |
|---|---:|
| `Power` | 1 W |
| `Frequency` | 50,000 Hz |
| `PulseWidth` | 2 µs |
| `LaserOnDelay`, `LaserOffDelay` | 0 µs |
| `ScannerJumpDelay` | 250 µs |
| `ScannerMarkDelay` | 150 µs |
| `ScannerPolygonDelay` | 100 µs |
| `JumpSpeed`, `MarkSpeed` | 500 mm/s |
| `IsHardJump` | false |
| `RasterMode` | JumpAndShoot |
| `RasterDirection` | Horizontal |
| `IsRasterZigZag` | true |
| `PixelTime` | 100 µs |
| `PixelPulses` | 0 |
| `IsPixelPulsesExit` | true |
| `PixelPeriod` | 200 µs |
| `PixelChannel` | ExtAO2 |
| SCANAhead Shift | 0 µs |
| SCANAhead Scale | 100% |
| `SpotDistanceSCANa` | 0 mm |
| `IsWobbelEnabled` | false |
| `WobbelFrequency` | 100 Hz |
| Wobbel 진폭 | 0.5 mm / 0.5 mm |
| `WobbelShape` | Ellipse |
| `MinMarkSpeed`, `ApproxBlendLimit` | 0 |

## 개발자: 기본 EntityPen 생성값 바꾸기

`SpiralLab.Sirius3.UI.Config.OnCreateEntityPen`을 Document를 만들거나 초기화하기 전에 구독하면 색상별 기본 펜을 직접 만들 수 있습니다. 편집기가 사용하는 각 `Config.EntityPenColors`에 대해 콜백이 호출됩니다.

```csharp
SpiralLab.Sirius3.UI.Config.OnCreateEntityPen += CreateEntityPen;

private EntityPen CreateEntityPen(IDocument document, Color color)
{
    return new EntityPen
    {
        Name = color.ToKnownColor().ToString(),
        PenColor = color,
        Description = color.ToString(),
        Power = 1,
        Frequency = 50_000,
        PulseWidth = 2,
        JumpSpeed = 500,
        MarkSpeed = 500,
        ScannerJumpDelay = 250,
        ScannerMarkDelay = 150,
        ScannerPolygonDelay = 100
    };
}
```

이벤트는 정적 이벤트이므로 폼이나 서비스가 종료될 때 구독을 해제하여 오래된 인스턴스가 유지되지 않도록 하십시오.

## 개발자: OnMarkEntityPen으로 List 명령 사용자 정의

기본 Marker는 펜 색이 바뀔 때 대략 다음 순서로 List 명령을 기록합니다.

1. `IRtc.ListLaserPower`
2. `IRtc.ListDelay`
3. SCANAhead List 설정
4. Hard Jump 설정
5. `IRtc.ListSpeed`
6. Wobbel 시작 또는 종료
7. syncAXIS 최소 속도와 블렌딩 설정

`IMarker.OnMarkEntityPen`을 구독하면 이 기본 처리에 추가되는 것이 아니라 기본 처리를 완전히 대신합니다. 따라서 사용자 처리기는 필요한 모든 명령을 기록하고 각 호출의 성공 여부를 반환해야 합니다.

```csharp
marker.OnMarkEntityPen += (currentMarker, pen) =>
{
    var rtc = currentMarker.Scanner as IRtc;
    if (rtc == null || currentMarker.Laser == null)
        return false;

    // 공정 조건에 따라 요청 출력을 제한하거나 교체할 수 있습니다.
    double requestedPower = Math.Min(pen.Power, 5.0);

    bool ok = rtc.ListLaserPower(
        currentMarker.Laser,
        pen.Frequency,
        pen.PulseWidth,
        requestedPower,
        pen.PowerMapCategory);

    ok &= rtc.ListDelay(
        pen.LaserOnDelay,
        pen.LaserOffDelay,
        pen.ScannerJumpDelay,
        pen.ScannerMarkDelay,
        pen.ScannerPolygonDelay);

    ok &= rtc.ListSpeed(pen.JumpSpeed, pen.MarkSpeed);
    return ok;
};
```

위 예제처럼 `OnMarkEntityPen`에서 `Power`를 바꾸어 사용자 공정 규칙을 적용할 수 있습니다. 단, SCANAhead, Wobbel, Hard Jump 또는 syncAXIS를 사용하는 문서라면 해당 기본 명령도 처리기에 포함해야 합니다. 이벤트는 Marker 작업 스레드에서 호출되므로 WinForms 컨트롤은 직접 변경하지 말고 UI 디스패치를 사용하십시오.

완전한 구현은 [`demos/editor_pen/Form1.cs`](../demos/editor_pen/Form1.cs)의 `Marker_OnMarkEntityPen`을 참고하십시오.

## 데모: editor_pen

[`demos/editor_pen`](../demos/editor_pen)은 EntityPen과 EntityLayerPen의 전체 속성을 코드로 초기화하고 Marker 이벤트에서 컨트롤·리스트 명령으로 변환하는 기준 예제입니다.

### 초기화 흐름

1. 프로젝트가 공통 [`demos/config.ini`](../demos/config.ini)를 출력 폴더의 `config.ini`로 복사합니다.
2. `Core.Initialize()`로 Sirius3 라이브러리를 초기화합니다.
3. `EditorHelper.CreateDevices`가 INI 설정을 읽고 RTC, Laser, DIO, PowerMeter와 Marker를 생성합니다.
4. `SiriusEditorControl.RegisterDevices`로 편집기에 장치를 등록합니다.
5. `marker.Ready(document, view, rtc, laser, powerMeter)`로 가공 준비를 완료합니다.
6. 종료 시 생성한 장치와 UI를 정리하고 `Core.Cleanup()`을 호출합니다.

### 확인할 코드

- `Config_OnCreateEntityPen`: 속성별 기본값 구성
- `BtnPrepare_Click`: 서로 다른 페이지와 펜 색을 가진 개체 생성
- `Marker_OnMarkEntityPen`: `EntityPen`을 RTC List 명령으로 변환
- `BtnMarkPage1_Click`, `BtnMarkPage2_Click`: 페이지 선택 가공

## 데모: editor_pen_multiple

[`demos/editor_pen_multiple`](../demos/editor_pen_multiple)은 한 도형의 구간마다 다른 펜 색을 배정하여 여러 공정 조건을 연속으로 전환합니다.

| 색 | `PowerPercentage` | `MarkSpeed` |
|---|---:|---:|
| White | 25% | 100 mm/s |
| Yellow | 50% | 500 mm/s |
| Orange | 75% | 1,000 mm/s |
| Red | 100% | 2,000 mm/s |

모든 펜의 `JumpSpeed`는 1,000 mm/s로 설정합니다. 사각형의 네 변이 각각 다른 `PenColor`를 사용하므로 Marker가 구간 경계에서 EntityPen List 명령을 갱신하는 과정을 확인할 수 있습니다.

또한 `EntityFactory.CreateMeasurementBegin`으로 10 kHz 측정을 시작하고 `LaserOn`, `SampleX`, `SampleY`, `PulseLength` 채널을 수집한 뒤 Measurement End 개체로 종료합니다. 설정한 펜 전환이 실제 신호와 위치에 어떻게 반영되는지 검증하는 데 활용할 수 있습니다. 자세한 측정 절차는 [MeasurementUserManual.md](MeasurementUserManual.koKR.md)를 참고하십시오.

## 데모: editor_scanahead_sdc

[`demos/editor_scanahead_sdc`](../demos/editor_scanahead_sdc)은 RTC6 SCANAhead와 Spot Distance Control을 함께 구성합니다.

핵심 흐름은 다음과 같습니다.

1. RTC6와 `rtc.IsSCANAhead`를 확인합니다.
2. `rtc6.IsActivateAutoDelays = true`로 Auto Delay를 활성화합니다.
3. `PositionACKLimit`을 필요에 맞게 설정합니다. 예제의 0.01 mm는 10 µm입니다.
4. White `EntityLayerPen`에 ALC `SpotDistance`, `ActualVelocity`, `SCANAhead` 확장 비트를 설정합니다.
5. White `EntityPen.SpotDistanceSCANa`를 0.01 mm로 설정합니다.

이 데모는 SCANAhead 옵션과 호환 스캔헤드가 있는 실제 RTC6 시스템을 전제로 합니다. Virtual RTC에서 속성값을 설정할 수 있다는 사실만으로 실제 SCANAhead/SDC 동작이 검증되지는 않습니다.

## 기능 지원 참고표

| 기능 | RTC4/RTC4e | RTC5/RTC5e | RTC6/RTC6e | RTC6 syncAXIS |
|---|---|---|---|---|
| 기본 출력·속도·지연 | 지원 | 지원 | 지원 | 지원 |
| Hard Jump | 미지원 | 지원 | 지원 | 미지원 |
| Raster JumpAndShoot | 지원 | 지원 | 지원 | 지원 |
| Raster MicroVector | 지원 | 지원 | 지원 | 사용하지 않음 |
| Wobbel 기본 형상 | 지원 | 지원 | 지원 | 미지원 |
| Wobbel 8자/Defined | 제한적 | 지원 | 지원 | 미지원 |
| SCANAhead | 미지원 | 미지원 | 옵션 및 호환 헤드 필요 | 미지원 |
| `MinMarkSpeed`, `ApproxBlendLimit` | 미지원 | 미지원 | 일반 RTC 경로 미사용 | 지원 |

실제 지원 여부는 카드 세대만으로 결정되지 않습니다. RTC 옵션, 펌웨어, 스캔헤드 종류, 레이저 인터페이스와 라이선스를 함께 확인하십시오. RTC6 기본 구조와 포트는 [Rtc6UserManual.md](Rtc6UserManual.koKR.md)를 참고하십시오.

## 문제 해결

### 개체 색을 바꿨는데 가공 조건이 변하지 않습니다

- 개체의 `PenColor`와 Document에 등록된 `EntityPen.PenColor`가 정확히 같은지 확인합니다.
- `marker.Ready`가 현재 Document와 장치를 사용해 다시 호출되었는지 확인합니다.
- `OnMarkEntityPen` 구독자가 기본 처리를 대체하고 있지 않은지 확인합니다.

### 출력 백분율을 바꿔도 Power가 변하지 않습니다

- 레이저가 편집기에 등록되었는지 확인합니다.
- `ILaser.MaxPowerWatt`와 `PowerMax`가 0보다 큰지 확인합니다.
- `PowerMapCategory`가 실제 등록된 분류인지 확인합니다.

### 입력한 펄스나 지연값이 조금 달라집니다

RTC 시간 해상도에 맞춰 반올림된 결과일 수 있습니다. `Config.IsConvertToControllerResolution`과 RTC 세대를 확인하십시오.

### Raster 줄이 번갈아 어긋납니다

`IsRasterZigZag`를 끄고 단방향 결과와 비교합니다. 양방향 레이저 지연, 스캐너 추종 및 픽셀 주기를 점검합니다.

### PixelPulses에서 레이저가 나오지 않습니다

레이저 `SYNC OUT`과 RTC LASER 포트 `DIGITAL IN1`의 물리 연결, TTL 레벨, 공통 접지, 활성 에지를 확인합니다. `PixelPulses = 0`인 일반 방식과 비교하여 문제 범위를 좁히십시오.

### SCANAhead 속성이 보이지 않거나 동작하지 않습니다

RTC6 카드, SCANahead 옵션, 호환 스캔헤드, 시스템 패키지와 펌웨어, `IsSCANAhead`, Auto Delay 활성 상태를 차례로 확인합니다.

## 적용 전 점검표

- 개체 색과 EntityPen 색이 정확히 일치합니다.
- `Power`가 레이저와 소재의 안전 범위 안에 있습니다.
- 주파수, 펄스 폭과 듀티비가 레이저 사양을 만족합니다.
- 속도를 바꾼 뒤 레이저·스캐너 지연을 다시 확인했습니다.
- Raster 외부 펄스 사용 시 SYNC OUT 배선과 신호 레벨을 확인했습니다.
- Wobbel 진폭·주파수가 스캐너 추종 범위 안에 있습니다.
- SCANAhead/SDC의 하드웨어 옵션과 LayerPen ALC 조건을 확인했습니다.
- 낮은 출력의 시험편과 안전한 차광 상태에서 먼저 검증했습니다.

## 관련 문서

- [LayerPenUserManual.md](LayerPenUserManual.koKR.md): EntityLayerPen, ALC, Sky Writing, Variable Delay, syncAXIS 동작 모드
- [MarkerUserManual.md](MarkerUserManual.koKR.md): Page, Layer, Offset과 Marker 실행 순서
- [PowermapUserManual.md](PowermapUserManual.koKR.md): 출력 Mapping, Verify, Compensate
- [Rtc6UserManual.md](Rtc6UserManual.koKR.md): RTC6 하드웨어와 기본 동작
- [Rtc6SyncaxisUserManual.md](Rtc6SyncaxisUserManual.koKR.md): syncAXIS 셋업과 운용
- [MeasurementUserManual.md](MeasurementUserManual.koKR.md): 측정 채널과 샘플링
- [Sirius3UIConfigUserManual.md](Sirius3UIConfigUserManual.koKR.md): 편집기 기본 펜과 표시 설정

---

2026 Copyright (c) SpiralLAB. All rights reserved.
