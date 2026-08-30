# EntityLayerPen 사용자 설명서

> 기준 버전: Sirius3 1.12.3 (공개 Release 기능)

`EntityLayerPen`은 한 Layer의 가공을 시작하기 전에 RTC 컨트롤 상태로 적용되는 설정입니다. ALC(Automatic Laser Control), Sky Writing, Variable Polygon/Jump Delay와 syncAXIS 동작 모드를 레이어 단위로 선택할 수 있습니다.

개별 개체의 레이저 출력, 주파수, 펄스 폭, 속도, 일반 지연, Raster, Wobbel과 SCANAhead List 설정은 [PenUserManual.md](PenUserManual.koKR.md)를 참고하십시오.

## EntityLayerPen의 역할

`EntityPen`과 `EntityLayerPen`은 적용 시점이 다릅니다.

| 구분 | EntityPen | EntityLayerPen |
|---|---|---|
| 연결 기준 | 개체의 `PenColor` | Layer의 `PenColor` |
| 적용 시점 | 개체 가공 명령을 RTC 리스트 버퍼에 기록하는 중 | 해당 Layer의 리스트 버퍼 생성을 시작하기 전 |
| 명령 성격 | `ListLaserPower`, `ListDelay`, `ListSpeed` 등의 List 명령 | `CtlSkyWriting`, `CtlAlc`, `CtlDelayVariable` 등의 Control 명령 |
| 대표 기능 | 출력, 펄스, 속도, 일반 지연, Raster, Wobbel | ALC, Sky Writing, Variable Delay, syncAXIS 동작 모드 |

Marker는 Layer의 `PenColor`와 같은 색의 `EntityLayerPen`을 찾아 적용한 뒤, 그 Layer 안의 개체들을 RTC 리스트 버퍼에 기록합니다. 따라서 레이어용 펜은 단순 표시 색상이 아니라 레이어 가공 모드를 선택하는 키입니다.

## 편집기에서 설정하는 방법

1. TreeView에서 설정할 Layer를 선택합니다.
2. PropertyGrid에서 Layer의 `PenColor`를 확인하거나 변경합니다.
3. 같은 색의 `EntityLayerPen`을 선택합니다.
4. ALC, Sky Writing, Variable Delay 또는 syncAXIS 속성을 설정합니다.
5. Layer 안의 개체들이 사용할 `EntityPen`도 각각 확인합니다.
6. Marker의 `LayerFirst` 또는 `OffsetFirst` 처리 순서가 공정 의도와 맞는지 확인합니다.
7. 레이저 출사를 막은 안전 상태에서 Preview 또는 시뮬레이션을 실행합니다.
8. 시험편에서 낮은 출력으로 레이어 경계와 시작·끝 품질을 확인합니다.

PropertyGrid에는 현재 RTC가 구현하는 기능만 표시됩니다. 예를 들어 RTC4에서는 Sky Writing과 ALC가 표시되지 않으며, SCANAhead Auto Delay가 활성화된 RTC6에서는 수동 Variable Delay 항목이 숨겨집니다.

## 주요 속성 한눈에 보기

| 분류 | 주요 속성 | 역할 |
|---|---|---|
| ALC | `IsALC`, `AlcSignal`, `AlcMode`, `AlcModeExtensionBits` | 속도나 위치에 따라 레이저 제어 신호를 자동 변조합니다. |
| ALC 범위 | `AlcPercentage100`, `AlcMinValue`, `AlcMaxValue` | 100% 기준값과 출력 제한 범위를 설정합니다. |
| 위치 보정 | `AlcByPositionTable` | 필드 중심에서의 반경에 따라 ALC 출력 배율을 보정합니다. |
| Sky Writing | `IsSkyWritingEnabled`, `SkyWritingMode`, `TimeLag`, `LaserOnShift`, `Prev`, `Post`, `AngularLimit` | 선의 시작·끝과 급격한 방향 전환에서 스캐너가 안정된 속도로 출사하도록 보조 이동을 추가합니다. |
| Variable Polygon Delay | `IsVariablePolygonDelay`, `VariablePolygonDelayEdgeLevel` | 연속 Mark 사이 모서리 각도에 따라 Polygon Delay를 조절합니다. |
| Variable Jump Delay | `IsVariableJumpDelay`, `VariableJumpDelayMin`, `VariableJumpDelayLimitLength` | Jump 길이에 따라 Jump Delay를 조절합니다. |
| syncAXIS | `MotionType`, `BandWidth` | Layer를 스캐너, 스테이지 또는 두 장치의 협조 구동으로 처리합니다. |

## 식별 정보와 공통 속성

### Name과 Description

`Name`은 LayerPen 목록에 표시할 이름이고 `Description`은 레이어 공정 모드를 설명하는 메모입니다. `White - Outline ALC`, `Yellow - Sky Writing`처럼 색과 역할을 함께 적으면 레이어 구조를 확인하기 쉽습니다.

### PenColor

`PenColor`는 Layer와 EntityLayerPen을 연결하는 정확한 ARGB 색입니다. 기본 편집 화면에서는 식별값의 일관성을 위해 읽기 전용으로 표시될 수 있으며, 개발자는 LayerPen 생성 이벤트에서 지정할 수 있습니다. 화면에서 비슷해 보이는 색이라도 ARGB 값이 다르면 다른 펜입니다.

### ExtensionData

사용자 애플리케이션이 레시피 키, 공정 단계 또는 외부 장치 조건 같은 추가 정보를 보관하는 확장 데이터입니다. `OnMarkLayerPen`에서 읽어 사용자 Control 명령을 선택할 수 있지만, 기본 Marker는 그 의미를 해석하지 않습니다.

### IsAllowMark와 Repeats

`EntityControl`에서 상속된 값이지만 EntityLayerPen의 표준 PropertyGrid에서는 숨겨지며 레이어 실행 여부나 반복 횟수를 결정하지 않습니다. 레이어 실행 허용과 반복은 `EntityLayer`의 설정을 사용하십시오.

## ALC 개요

ALC는 스캐너 속도 또는 위치에 따라 RTC 출력 신호를 자동으로 바꾸는 기능입니다. 시작·끝이나 모서리에서 스캐너가 느려질 때 레이저 에너지가 한곳에 과도하게 쌓이는 현상을 줄이고, 이동 속도가 변해도 더 균일한 가공 결과를 얻는 데 사용합니다.

다음 세 가지를 구분해서 설정해야 합니다.

- `AlcSignal`: 어떤 RTC 출력을 바꿀지 선택합니다.
- `AlcMode`: 어떤 속도를 기준으로 바꿀지 선택합니다.
- `AlcModeExtensionBits`: SCANAhead, MoF 등 추가 계산 조건을 선택합니다.

ALC는 `IRtcAutoLaserControl`을 구현하는 RTC5/RTC6 계열에서 사용합니다. RTC4 계열에서는 사용할 수 없습니다.

## ALC 활성화

### IsALC

`IsALC`가 `true`이면 Layer를 시작하기 전에 ALC 위치 테이블과 신호·모드 설정을 RTC에 적용합니다. `false`이면 해당 Layer에서 ALC를 사용하지 않습니다.

Marker는 한 Layer의 가공이 끝나면 ALC 위치 테이블을 지우고 ALC를 비활성화합니다. 다음 Layer는 자신의 `EntityLayerPen` 설정을 다시 적용하므로, 앞 레이어의 ALC 조건이 우연히 이어지는 것에 의존하지 마십시오.

## ALC 출력 신호

### AlcSignal

| 값 | 출력과 단위 | 주요 용도와 주의사항 |
|---|---|---|
| `Disabled` | 출력 없음 | 위치 보정표만 적용하거나 ALC를 비활성화할 때 사용합니다. |
| `Analog1` | 0~10 V | 레이저의 아날로그 출력 입력을 제어합니다. 주파수·펄스 폭을 유지하면서 출력 세기를 바꿀 때 적합합니다. |
| `Analog2` | 0~10 V | 두 번째 아날로그 채널을 사용합니다. 배선과 입력 임피던스를 확인하십시오. |
| `ExtDO8` | 0~255 정수 | EXTENSION2의 8비트 디지털 출력으로 레이저 또는 외부 장치를 제어합니다. |
| `PulseWidth` | µs | LASER1/LASER2 펄스 폭을 속도에 따라 바꿉니다. 레이저가 빠르고 선형적인 PWM 응답을 지원해야 합니다. |
| `Frequency` | Hz | 허용 범위 안에서 레이저 반복 주파수를 바꿉니다. 주파수 변화는 펄스 간격도 바꾸므로 공정 영향을 확인하십시오. |
| `ExtDO16` | 0~65535 정수 | EXTENSION1의 16비트 디지털 출력으로 외부 장치를 제어합니다. |
| `SpotDistance` | mm | RTC6 SCANAhead에서 경로상의 펄스 간격을 일정하게 제어합니다. PoD 또는 등가 외부 트리거 기능이 있는 레이저가 필요합니다. |

`Analog1`, `Analog2`, `ExtDO8`, `ExtDO16`을 사용할 때는 RTC 포트와 레이저 입력 사이의 전압 범위, 비트 폭, 극성, 공통 접지와 안전 인터록을 확인하십시오.

## ALC 기준 속도

### AlcMode

| 값 | 기준 | 사용 시점 |
|---|---|---|
| `Disabled` | 속도 보정 없음 | `AlcByPositionTable`만으로 위치 종속 보정을 할 때 사용할 수 있습니다. |
| `SetVelocity` | RTC에 명령한 속도 | 디지털 위치 피드백이 없는 스캔헤드에서도 사용할 수 있습니다. 실제 가감속 응답은 반영하지 않습니다. |
| `ActualVelocity` | 스캔헤드의 실제 피드백 속도 | iDRIVE 피드백을 제공하는 호환 스캔헤드에서 시작·끝과 모서리의 실제 감속까지 보정합니다. |
| `EncoderSpeed` | 외부 엔코더 속도 | 스캐너보다 컨베이어 또는 외부 이동체의 속도를 기준으로 제어할 때 사용합니다. 별도의 엔코더 속도 설정이 필요합니다. |

`ActualVelocity`는 이름만 선택한다고 동작하는 것이 아닙니다. 호환 스캔헤드, RTC 설정, 피드백 통신과 펌웨어가 모두 준비되어 있어야 합니다. `EncoderSpeed`는 `IRtcAutoLaserControl.CtlAlcEncoderSpeed`로 엔코더 소스와 환산값을 먼저 설정해야 합니다.

## ALC 출력 범위

### AlcPercentage100

스캐너 속도가 기준 속도의 100%일 때 사용할 출력값입니다. 단위는 `AlcSignal`에 따라 달라집니다.

- Analog: V
- PulseWidth: µs
- Frequency: Hz
- ExtDO8/ExtDO16: 정수 출력값

`SpotDistance`에서는 이 값이 사용되지 않으며 실제 간격은 `EntityPen.SpotDistanceSCANa`로 지정합니다.

### AlcMinValue와 AlcMaxValue

ALC가 계산한 출력이 장치의 유효 범위를 벗어나지 않도록 제한합니다. 최소값은 레이저가 안정적으로 동작할 수 있는 하한보다 낮게 설정하지 말고, 최대값은 레이저 입력과 RTC 출력의 허용 범위를 넘지 않도록 하십시오.

예를 들어 Frequency 신호를 사용할 때 최소 주파수를 너무 낮게 두면 레이저 발진이 불안정해질 수 있습니다. Analog 신호에서는 입력 전압과 실제 광출력이 선형인지 별도로 확인해야 합니다.

## ALC 확장 기능

### AlcModeExtensionBits

PropertyGrid에서는 여러 비트를 함께 선택할 수 있습니다. 코드에서는 `AlcModeExtensionBits` 비트 플래그를 직접 지정하거나, 직렬화 대상인 `AlcModeExtension` 컬렉션에 비트를 추가합니다.

| 비트 | 역할 | 필수 조건 또는 주의사항 |
|---|---|---|
| `None` | 확장 기능을 사용하지 않습니다. | 기본 ALC만 사용합니다. |
| `EncoderSpeedAddition` | 스캐너 속도와 엔코더 속도를 벡터로 합산합니다. | 활성 MoF 세션이 필요합니다. |
| `SCANAhead` | SCANAhead Preview Time과 궤적 정보를 ALC 계산에 사용합니다. | RTC6, SCANahead 옵션, 호환 헤드와 Auto Delay가 필요합니다. |
| `InverseSpeedCorrection` | F-Theta 렌즈와 필드 위치에 따른 선속도 차이를 역보정합니다. | 실제 보정 데이터와 광학 구성 검증이 필요합니다. |
| `BackwardTransformation` | 회전·행렬 변환이 적용된 경로의 속도를 원래 좌표계 기준으로 역변환합니다. | RTC6 지원 범위를 확인하십시오. |
| `SkyWritingSDC` | Sky Writing의 가감속 구간에서도 Spot Distance Control을 유지합니다. | SCANAhead와 SpotDistance 구성이 먼저 필요합니다. |

확장 비트는 목적이 겹치지 않는 경우 함께 사용할 수 있지만, 장치가 모든 조합을 지원한다는 뜻은 아닙니다. SCANLAB 시스템 패키지, RTC 펌웨어, 스캔헤드와 레이저 입력 조건을 함께 확인하십시오.

## 위치 종속 ALC 보정

### AlcByPositionTable

필드 중심으로부터의 반경에 따라 ALC 출력에 곱할 배율을 지정합니다. 렌즈와 광학 경로 때문에 같은 명령값이 위치별로 다른 결과를 만드는 경우에 사용합니다.

- 키: 필드 중심에서의 반경, 단위 mm
- 값: 출력 배율, 1.0은 100%
- 유효 배율 범위: 0~4
- 최대 유효 지점 수: 50개
- 유효 반경 범위: Effective Field Size의 X 크기를 기준으로 환산한 0~150%

RTC는 표를 반경 순서로 정렬하고 지점 사이를 보간합니다. 0% 또는 150% 끝점이 없으면 경계값을 보완하며, 유효한 지점이 하나뿐이면 전체 범위에 같은 배율을 적용합니다. 범위를 벗어난 항목은 무시되고, 항목은 있으나 유효한 지점이 하나도 없으면 설정이 실패할 수 있습니다.

```csharp
layerPen.IsALC = true;
layerPen.AlcSignal = AutoLaserControlSignals.Analog1;
layerPen.AlcMode = AutoLaserControlModes.Disabled;
layerPen.AlcByPositionTable = new List<KeyValuePair<double, double>>
{
    new KeyValuePair<double, double>(0.0, 1.00),
    new KeyValuePair<double, double>(10.0, 1.05),
    new KeyValuePair<double, double>(20.0, 1.12)
};
```

위 값은 형식을 보여 주기 위한 예이며 실제 보정값이 아닙니다. PowerMeter로 위치별 출력을 측정하고, 보정 적용 전후를 별도 시험편에서 검증하십시오. RTC5에서 Frequency/HalfPeriod 계열을 사용할 때는 역수 관계가 적용되고 RTC6에서는 직접 값이 적용되므로 카드 세대가 바뀌면 표를 다시 검증해야 합니다.

## Spot Distance Control

Spot Distance Control은 스캐너가 가속하거나 감속해도 경로 위의 펄스 간격을 일정하게 유지하는 기능입니다. 광학적인 스폿 직경을 지정하는 기능이 아닙니다.

### 필수 구성

```csharp
layerPen.IsALC = true;
layerPen.AlcSignal = AutoLaserControlSignals.SpotDistance;
layerPen.AlcMode = AutoLaserControlModes.ActualVelocity;
layerPen.AlcModeExtensionBits =
    AutoLaserControlModeExtensions.Bit.SCANAhead;

entityPen.SpotDistanceSCANa = 0.01; // 10 µm
```

다음 조건도 모두 필요합니다.

- RTC6와 SCANahead 옵션
- 호환되는 excelliSCAN 또는 intelliSCAN IV 계열 스캔헤드
- SCANAhead Auto Delay 활성화
- PoD 또는 일정 에너지 외부 트리거를 지원하는 레이저
- RTC와 레이저 사이의 올바른 외부 트리거 배선
- 레이저의 최소·최대 반복 주파수 안에 드는 속도와 간격

Sky Writing 중에도 등간격을 유지하려면 `SkyWritingSDC` 비트를 추가합니다. 속도가 0에 가까워지면 요구 주파수도 0에 가까워지고, 속도가 높을수록 요구 주파수가 커지므로 일반적인 고정 주파수 레이저로는 전체 범위를 안정적으로 처리하기 어렵습니다.

## Sky Writing 개요

Sky Writing은 Mark 벡터 앞뒤에 레이저를 끈 보조 이동을 추가합니다. 스캐너가 목표 Mark 속도에 가까워진 뒤 출사를 시작하고, 가공 구간을 지난 뒤 감속하도록 하여 선의 시작·끝과 급격한 모서리 품질을 개선합니다.

일반 지연값만으로 시작·끝을 맞추기 어려운 미세 선, 짧은 선, 작은 호와 방향 전환이 많은 Polyline에 유용합니다. 보조 이동이 추가되므로 가공 시간과 필요한 이동 공간은 늘어납니다.

Sky Writing은 `IRtcSkyWriting`을 구현하는 RTC5 이상에서 사용하며 Mode4는 RTC6에서 사용합니다.

## Sky Writing 모드

### IsSkyWritingEnabled

`true`이면 선택한 모드와 파라미터를 Layer 시작 전에 적용합니다. `false`이면 Marker가 `Deactivate`로 설정하여 이전 Layer의 Sky Writing이 이어지지 않도록 합니다.

### SkyWritingMode

| 모드 | 동작 | 적합한 경우 |
|---|---|---|
| `Mode1` | Mark 앞에 forerun과 복귀 동작을 추가하고 Mark 뒤에 감속과 retrace를 수행합니다. 실제 run-in은 `Prev`의 2배, run-out은 `Post`의 2배 시간으로 동작합니다. | 가장 기본적인 시작·끝 품질 검증 |
| `Mode2` | forerun/retrace 대신 Sky Writing Jump로 run-in과 run-out 위치를 연결합니다. | 정밀도를 유지하면서 Mode1의 되돌림 시간을 줄일 때 |
| `Mode3` | 연속 Mark 사이의 방향 변화가 `AngularLimit`보다 클 때 Mode2 방식으로 동작하고, 완만한 연결에는 일반 Polygon Delay를 사용합니다. | 모서리가 많은 Polyline에서 품질과 시간을 균형 있게 맞출 때 |
| `Mode4` | Mode3에 더해 Jump와 Mark 사이의 짧은 List 명령을 허용합니다. | RTC6에서 복합 리스트를 사용할 때 |

Polyline의 시작과 끝, Jump로 끊긴 구간에서는 Mode3/Mode4도 시작·끝 품질을 위해 기본 Sky Writing 동작을 수행할 수 있습니다.

## Sky Writing 파라미터

### TimeLag

스캐너의 명령 위치와 실제 위치 사이의 추종 지연을 시간으로 표현한 값이며 단위는 µs입니다. 일반적으로 0~10,000 µs 범위이며 0.25 µs 미만에서는 실질적으로 활성화되지 않습니다.

일반 Sky Writing에서 레이저 꺼짐 기준은 `TimeLag`, 켜짐 기준은 `TimeLag + LaserOnShift` 관계로 계산됩니다. SCANAhead Auto Delay에서는 Preview Time과 자동 계산 결과를 사용하므로 `TimeLag`, `Prev`, `Post`가 PropertyGrid에서 숨겨지거나 직접 타이밍을 결정하지 않습니다.

### LaserOnShift

Sky Writing이 계산한 레이저 켜짐 시점을 보정하며 단위는 µs입니다.

- 양수: 출사를 늦춥니다.
- 음수: 출사를 앞당깁니다.

음수 값은 사용할 수 있는 `Prev` 구간보다 과도하게 앞당길 수 없습니다. RTC5 해상도는 0.5 µs, RTC6 해상도는 0.015625 µs입니다. Sky Writing 중에는 일반 `EntityPen.LaserOnDelay`와 `LaserOffDelay`가 같은 방식으로 동작하지 않으므로 두 체계를 혼합해 조정하지 마십시오.

### Prev

Mark 시작 전에 확보할 run-in 설정 시간이며 단위는 µs입니다. Mode1에서는 forerun과 복귀 동작 때문에 실제 run-in 시간이 설정값의 2배이고, Mode2~Mode4에서는 설정값을 그대로 사용합니다. 일반적으로 10 µs 단위로 처리됩니다. 너무 작으면 목표 속도에 도달하기 전에 출사될 수 있고, 너무 크면 이동 거리와 시간이 늘어납니다.

### Post

Mark 종료 뒤 확보할 run-out 설정 시간이며 단위는 µs입니다. Mode1에서는 감속과 되돌림 때문에 실제 run-out 시간이 설정값의 2배이고, Mode2~Mode4에서는 설정값을 그대로 사용합니다. 너무 작으면 끝점 품질이 나빠질 수 있고, 너무 크면 가공 시간이 늘어납니다.

`Prev`와 `Post`는 0~655,350 µs 범위를 사용하며 655,350은 RTC가 `TimeLag`를 기준으로 자동 기본값을 선택하는 특별값으로 사용할 수 있습니다.

### AngularLimit

Mode3/Mode4에서 Sky Writing을 적용할 연속 Mark 사이의 방향 변화 기준이며 단위는 도입니다. 범위는 0~180°이고 표준 기본값은 90°입니다.

이 각도는 스캐너가 얼마나 회전하는지를 뜻하는 것이 아니라, 레이저를 켠 채 이어지는 두 Mark 세그먼트가 만나는 모서리의 방향 변화입니다. 기준보다 완만한 연결에는 일반 Polygon Delay를 사용하고, 더 크게 꺾이는 모서리에는 Sky Writing을 적용합니다.

## Sky Writing 최적화 순서

1. 일반 `EntityPen` 속도로 안정적인 결과를 먼저 만듭니다.
2. Sky Writing을 Mode1로 켜고 `TimeLag`를 스캔헤드의 실제 추종 지연에 맞춥니다.
3. 선 시작과 끝을 보며 `LaserOnShift`를 조정합니다.
4. `Prev`, `Post`를 충분히 확보한 뒤 품질을 유지하는 범위에서 줄입니다.
5. 가공 시간이 중요하면 Mode2를 비교합니다.
6. 방향 전환이 많은 Polyline에서는 Mode3과 `AngularLimit`을 조정합니다.
7. RTC6 복합 리스트가 필요할 때만 Mode4를 사용합니다.
8. SCANAhead Auto Delay 또는 SDC를 켠 뒤 전체 결과를 다시 확인합니다.

## Variable Polygon Delay

### 동작 원리

`IsVariablePolygonDelay`를 켜면 연속된 Mark 사이 모서리 각도에 따라 `EntityPen.ScannerPolygonDelay`를 자동으로 줄이거나 늘립니다.

`배율 = 1 − cos(φ)`

여기서 `φ`는 연속된 두 Mark 세그먼트가 만나는 방향 변화 각도입니다.

| 방향 변화 | 배율 | 결과 |
|---:|---:|---|
| 0° | 0 | 직선에 가까우므로 추가 Polygon Delay가 거의 없습니다. |
| 90° | 1 | `ScannerPolygonDelay`와 같은 값이 적용됩니다. |
| 180° | 2 | 되돌아가는 모서리이므로 최대 2배까지 적용될 수 있습니다. |

이 기능은 모서리에서 레이저를 항상 끄는 기능이 아닙니다. 방향이 크게 바뀔수록 지연 시간을 추가하여 스캐너가 새 방향을 따라갈 시간을 확보하고, 그동안 모서리의 레이저 출사 시간이 늘어납니다.

### VariablePolygonDelayEdgeLevel

모서리에서 레이저를 계속 켜 둘 수 있는 최대 지연 기준이며 단위는 µs입니다. 계산된 Variable Polygon Delay가 이 기준 이상이면 RTC는 `LaserOffDelay` 뒤에 레이저를 끄고 새 Polyline처럼 다시 시작합니다.

이 값은 고정 `EntityPen.ScannerPolygonDelay`의 2배보다 작아야 실제 분기 기준으로 작동합니다. 너무 낮으면 작은 모서리에서도 레이저가 자주 꺼져 선이 끊길 수 있고, 너무 높으면 급격한 모서리에 열이 과도하게 쌓일 수 있습니다.

### 적용 예

- 긴 직선이 여러 짧은 세그먼트로 나뉜 도형: 불필요한 지연을 줄일 수 있습니다.
- 90° 모서리가 반복되는 윤곽: 고정 Polygon Delay와 비슷한 기준을 유지합니다.
- 매우 급한 되돌림: Edge Level을 넘으면 레이저를 끄고 새 경로로 처리해 과도한 모서리 출사를 줄입니다.

## Variable Jump Delay

Jump 거리가 짧을 때 긴 고정 Jump Delay를 매번 기다리지 않도록 이동 길이에 따라 지연을 보간합니다.

### IsVariableJumpDelay

Variable Jump Delay 사용 여부입니다. `IRtcVariableDelay`를 구현하는 RTC에서만 적용됩니다.

### VariableJumpDelayMin

매우 짧거나 길이가 0에 가까운 Jump에 적용할 최소 지연이며 단위는 µs입니다. 일반적으로 현재 `EntityPen.ScannerJumpDelay`보다 작거나 같게 설정합니다.

### VariableJumpDelayLimitLength

고정 `EntityPen.ScannerJumpDelay`에 도달할 Jump 거리이며 단위는 mm입니다.

- 매우 짧은 Jump: `VariableJumpDelayMin`
- 0과 Limit Length 사이: 거리 비례 보간
- Limit Length 이상: `EntityPen.ScannerJumpDelay`

짧은 해치 선 사이 이동처럼 작은 Jump가 반복되는 공정에서 시간을 줄일 수 있습니다. 너무 작은 최소 지연은 다음 Mark의 시작점 오차와 진동을 만들 수 있으므로 실제 이동 길이별로 검증하십시오.

## SCANAhead와 Variable Delay의 관계

RTC6 SCANAhead Auto Delay는 Preview Time과 스캔헤드 파라미터로 지연을 자동 계산합니다. 이 상태에서는 수동 Variable Polygon/Jump Delay를 함께 사용하지 않으며 관련 속성이 PropertyGrid에서 숨겨집니다.

Auto Delay를 활성화한 뒤에는 다음 순서로 최적화하십시오.

1. RTC6가 SCANahead 옵션과 호환 스캔헤드를 인식하는지 확인합니다.
2. 스캔헤드 Pre-configuration에서 Preview Time, 최대 속도와 가속도 값이 올바르게 로드되었는지 확인합니다.
3. `IsActivateAutoDelays`와 `IsSCANAhead`를 확인합니다.
4. `EntityPen`의 SCANAhead Shift와 Scale을 기본값에서 시작합니다.
5. Trajectory ACK 상태와 실제 시작·끝·모서리 품질을 확인합니다.
6. 필요할 때만 Laser On/Off Shift와 Scale을 작은 단계로 조정합니다.

Preview Time은 미래 궤적을 얼마나 미리 계산해 스캔헤드에 전달할지를 나타내며, SCANAhead 자동 지연의 핵심 입력입니다. 일반 Laser/Scanner Delay와 같은 의미가 아니므로 수동 지연값으로 치환하지 마십시오.

## syncAXIS 동작 모드

syncAXIS는 넓은 작업 영역에서 이송 스테이지와 스캐너를 협조 구동하는 기능입니다. 일반 `MarkerRtc`와 별도의 syncAXIS용 Marker를 사용합니다.

### MotionType

| 값 | 동작 |
|---|---|
| `ScannerOnly` | 스캔헤드만 이동합니다. |
| `StageOnly` | 이송 스테이지만 이동합니다. |
| `StageAndScanner` | 스테이지가 큰 이동을 담당하고 스캐너가 빠른 국부 이동을 보완합니다. |

### BandWidth

`StageAndScanner`에서 스테이지와 스캐너 사이의 동작 분담을 정하는 주파수 기준이며 단위는 Hz입니다. 값이 높을수록 스테이지가 더 빠른 궤적 성분까지 담당하고, 낮을수록 스캐너가 더 많은 고주파 성분을 담당합니다. 0.23 Hz 미만의 값은 사용할 수 없습니다.

BandWidth를 높인다고 항상 성능이 좋아지는 것은 아닙니다. 스테이지의 질량, 최대 속도·가속도, 기구 공진과 스캐너 필드 범위를 함께 고려해야 합니다.

### 사용 전 준비

- syncAXIS 시스템 사전 셋업
- RTC6 syncAXIS 라이선스
- SCANLAB USB 동글
- 축 구성, 스테이지 환산값과 좌표계 설정 파일
- [`demos/console_syncaxis_setup`](../demos/console_syncaxis_setup)을 이용한 설정 확인
- syncAXIS용 Marker와 `IRtcSyncAxis` 등록

자세한 절차는 [Rtc6SyncaxisUserManual.md](Rtc6SyncaxisUserManual.koKR.md)를 참고하십시오.

## LayerFirst와 OffsetFirst 실행 순서

Marker는 여러 Offset과 Layer를 처리할 때 두 가지 순서를 제공합니다. 같은 문서라도 순서에 따라 `EntityLayerPen`이 적용되는 시점과 열 누적, 장치 전환 횟수가 달라집니다.

### LayerFirst

각 Offset에서 모든 Layer를 차례로 처리한 다음 다음 Offset으로 이동합니다.

```text
Offset 1: Layer 1 → Layer 2 → Layer 3
Offset 2: Layer 1 → Layer 2 → Layer 3
```

각 Offset/Layer 조합을 시작하기 전에 해당 LayerPen이 다시 적용됩니다. 같은 Offset 위치에서 여러 레이어 공정을 연속 처리해야 할 때 적합합니다.

### OffsetFirst

한 Layer를 모든 Offset에서 처리한 다음 다음 Layer로 이동합니다.

```text
Layer 1: Offset 1 → Offset 2
Layer 2: Offset 1 → Offset 2
Layer 3: Offset 1 → Offset 2
```

LayerPen을 적용한 뒤 해당 Layer의 모든 Offset을 하나의 처리 묶음으로 구성합니다. 같은 레이어 조건을 유지하며 여러 위치를 먼저 처리할 때 적합합니다.

### Repeats와의 관계

반복 횟수는 `EntityLayerPen.Repeats`가 아니라 `EntityLayer.Repeats`에서 결정됩니다. LayerPen의 숨겨진 `Repeats` 값에 의존하지 마십시오. 반복 가공에서는 열 누적과 레이어 간 냉각 시간을 별도로 검토해야 합니다.

## 표준 기본값

Sirius3 편집기가 기본 LayerPen을 만들 때 적용하는 대표값입니다. 장치와 공정에 맞는 안전값을 의미하지는 않습니다.

| 속성 | 기본값 |
|---|---:|
| `IsALC` | false |
| `AlcSignal`, `AlcMode` | Disabled |
| `AlcModeExtensionBits` | None |
| `AlcPercentage100`, `AlcMinValue`, `AlcMaxValue` | 0 |
| `AlcByPositionTable` | 빈 목록 |
| `IsSkyWritingEnabled` | false |
| `SkyWritingMode` | Mode3 |
| `TimeLag` | 250 µs |
| `LaserOnShift` | 0 µs |
| `Prev` | 300 µs |
| `Post` | 200 µs |
| `AngularLimit` | 90° |
| `MotionType` | ScannerOnly |
| `BandWidth` | 2 Hz |
| `IsVariablePolygonDelay` | true |
| `VariablePolygonDelayEdgeLevel` | 150 µs |
| `IsVariableJumpDelay` | false |
| `VariableJumpDelayMin` | 50 µs |
| `VariableJumpDelayLimitLength` | 0.5 mm |

## 개발자: 기본 EntityLayerPen 생성값 바꾸기

`SpiralLab.Sirius3.UI.Config.OnCreateLayerPen`을 Document 생성 또는 초기화 전에 구독하면 색상별 기본 LayerPen을 직접 만들 수 있습니다.

```csharp
SpiralLab.Sirius3.UI.Config.OnCreateLayerPen += CreateLayerPen;

private EntityLayerPen CreateLayerPen(IDocument document, Color color)
{
    return new EntityLayerPen
    {
        Name = color.ToKnownColor().ToString(),
        PenColor = color,
        Description = color.ToString(),
        IsALC = false,
        IsSkyWritingEnabled = false,
        SkyWritingMode = SkyWritingModes.Mode3,
        TimeLag = 250,
        Prev = 300,
        Post = 200,
        AngularLimit = 90,
        IsVariablePolygonDelay = true,
        VariablePolygonDelayEdgeLevel = 150,
        IsVariableJumpDelay = false,
        VariableJumpDelayMin = 50,
        VariableJumpDelayLimitLength = 0.5
    };
}
```

이벤트는 정적 이벤트이므로 폼이나 서비스가 종료될 때 구독을 해제하십시오. Document가 이미 만들어진 뒤 구독하면 기존 펜에는 새 기본값이 적용되지 않을 수 있습니다.

## 개발자: OnMarkLayerPen으로 Control 명령 사용자 정의

기본 Marker는 Layer를 시작하기 전에 대략 다음 순서로 컨트롤 명령을 적용합니다.

1. `IRtcSkyWriting.CtlSkyWriting`
2. `IRtcAutoLaserControl.CtlAlcByPositionTable`
3. `IRtcAutoLaserControl.CtlAlc`
4. `IRtcVariableDelay.CtlDelayVariable`
5. `IRtcSyncAxis.CtlMotionType`와 필요 시 `CtlBandWidth`
6. RTC와 레이저의 List Begin
7. Layer 안의 EntityPen과 개체 List 명령 기록

`IMarker.OnMarkLayerPen`을 구독하면 기본 처리에 추가되는 것이 아니라 기본 처리를 완전히 대신합니다. 사용자 처리기는 현재 RTC가 지원하는 인터페이스를 확인하고, 필요한 모든 컨트롤 명령을 적용한 뒤 성공 여부를 반환해야 합니다.

```csharp
marker.OnMarkLayerPen += (currentMarker, pen) =>
{
    var rtc = currentMarker.Scanner as IRtc;
    if (rtc == null)
        return false;

    bool ok = true;

    if (rtc is IRtcSkyWriting skyWriting)
    {
        if (pen.IsSkyWritingEnabled)
        {
            double cosineLimit = Math.Cos(
                Helper.DegToRad(pen.AngularLimit));
            ok &= skyWriting.CtlSkyWriting(
                pen.SkyWritingMode,
                pen.LaserOnShift,
                pen.TimeLag,
                pen.Prev,
                pen.Post,
                cosineLimit);
        }
        else
        {
            ok &= skyWriting.CtlSkyWriting(
                SkyWritingModes.Deactivate, 0, 0, 0, 0, 0);
        }
    }

    if (rtc is IRtcVariableDelay variableDelay)
    {
        ok &= variableDelay.CtlDelayVariable(
            pen.IsVariablePolygonDelay,
            pen.VariablePolygonDelayEdgeLevel,
            pen.IsVariableJumpDelay,
            pen.VariableJumpDelayMin,
            pen.VariableJumpDelayLimitLength);
    }

    return ok;
};
```

위 예제는 Sky Writing과 Variable Delay만 처리합니다. ALC 또는 syncAXIS를 사용하는 프로젝트에서 그대로 사용하면 해당 기본 설정이 빠집니다. 전체 구현은 [`demos/editor_pen/Form1.cs`](../demos/editor_pen/Form1.cs)의 `Marker_OnMarkLayerPen`을 참고하십시오. 이벤트는 Marker 작업 스레드에서 호출되므로 WinForms 컨트롤을 직접 변경하지 마십시오.

## 데모: editor_pen

[`demos/editor_pen`](../demos/editor_pen)은 EntityLayerPen의 모든 주요 속성을 코드로 구성하고, 이를 RTC Control 명령으로 변환하는 기준 예제입니다.

### 초기화 흐름

1. 프로젝트가 공통 [`demos/config.ini`](../demos/config.ini)를 출력 폴더의 `config.ini`로 복사합니다.
2. `Core.Initialize()`로 라이브러리를 초기화합니다.
3. `EditorHelper.CreateDevices`가 INI 설정을 읽고 RTC, Laser, DIO, PowerMeter와 Marker를 생성합니다.
4. `SiriusEditorControl.RegisterDevices`로 장치를 편집기에 등록합니다.
5. `marker.Ready(document, view, rtc, laser, powerMeter)`로 가공 준비를 완료합니다.
6. 종료 시 장치와 UI를 정리하고 `Core.Cleanup()`을 호출합니다.

### 확인할 코드

- `Config_OnCreateLayerPen`: ALC, Sky Writing, Variable Delay, syncAXIS 기본값 구성
- `Marker_OnMarkLayerPen`: 지원 인터페이스 검사와 Control 명령 적용
- `Config_OnCreateEntityPen`: Layer 안의 개체가 사용할 List 설정
- Page 1/Page 2 가공 버튼: 같은 Document에서 페이지별 실행 확인

이 데모는 `OnMarkLayerPen`과 `OnMarkEntityPen`을 모두 구독합니다. 두 이벤트가 기본 Marker 처리를 대체하므로, 사용자 정의 Marker를 만들 때 빠진 기능이 없는지 비교하는 데 유용합니다.

## 데모: editor_pen_multiple

[`demos/editor_pen_multiple`](../demos/editor_pen_multiple)은 하나의 Layer 안에서 개체 색에 따라 여러 EntityPen을 바꾸는 예제입니다. LayerPen은 레이어 전체의 Control 상태를 유지하고, EntityPen은 사각형 각 변의 출력과 속도를 바꿉니다.

이 구분을 통해 다음을 확인할 수 있습니다.

- ALC/Sky Writing 같은 레이어 전체 모드는 EntityLayerPen에서 한 번 구성합니다.
- 출력과 MarkSpeed처럼 구간별로 바뀌는 값은 EntityPen 색으로 전환합니다.
- Measurement Begin/End 개체를 이용해 LayerPen과 EntityPen 설정이 실제 신호에 반영되는지 측정할 수 있습니다.

측정 채널은 10 kHz 샘플링으로 `LaserOn`, `SampleX`, `SampleY`, `PulseLength`를 등록합니다. 자세한 측정 방법은 [MeasurementUserManual.md](MeasurementUserManual.koKR.md)를 참고하십시오.

## 데모: editor_scanahead_sdc

[`demos/editor_scanahead_sdc`](../demos/editor_scanahead_sdc)은 EntityLayerPen ALC와 EntityPen Spot Distance를 함께 설정하는 대표 예제입니다.

```csharp
layerPenWhite.IsALC = true;
layerPenWhite.AlcByPositionTable.Clear();
layerPenWhite.AlcSignal = AutoLaserControlSignals.SpotDistance;
layerPenWhite.AlcMode = AutoLaserControlModes.ActualVelocity;
layerPenWhite.AlcModeExtension.Clear();
layerPenWhite.AlcModeExtension.Add(
    AutoLaserControlModeExtensions.Bit.SCANAhead);

entityPenWhite.SpotDistanceSCANa = 0.01; // 10 µm
```

예제에서 추가로 선택할 수 있는 확장 비트는 다음과 같습니다.

- `SkyWritingSDC`: Sky Writing 가감속 구간에서도 SDC 유지
- `EncoderSpeedAddition`: 활성 MoF에서 엔코더 속도 합산
- `InverseSpeedCorrection`: 필드 위치별 선속도 역보정
- `BackwardTransformation`: 좌표 변환 전 기준으로 피드백 속도 역변환

이 데모는 실제 RTC6 SCANahead 옵션과 호환 스캔헤드를 전제로 합니다. `rtc6.IsActivateAutoDelays = true`와 `rtc.IsSCANAhead` 확인이 선행되어야 하며, Virtual RTC만으로 실제 레이저 펄스 간격을 검증할 수는 없습니다.

## 기능 지원 참고표

| 기능 | RTC4/RTC4e | RTC5/RTC5e | RTC6/RTC6e | RTC6 syncAXIS |
|---|---|---|---|---|
| ALC | 미지원 | 지원 | 지원 | 일반 syncAXIS 경로 미지원 |
| Spot Distance + SCANAhead | 미지원 | 미지원 | 옵션 및 호환 헤드 필요 | 별도 구성 확인 |
| Sky Writing Mode1~3 | 미지원 | 지원 | 지원 | 일반 syncAXIS 경로 미지원 |
| Sky Writing Mode4 | 미지원 | 미지원 | 지원 | 일반 syncAXIS 경로 미지원 |
| Variable Polygon Delay | 지원 | 지원 | 지원 | 일반 syncAXIS 경로 미지원 |
| Variable Jump Delay | 지원 | 지원 | 지원 | 일반 syncAXIS 경로 미지원 |
| `MotionType`, `BandWidth` | 미지원 | 미지원 | 일반 RTC 경로 미사용 | 지원 |

Virtual RTC는 편집과 시뮬레이션에 유용하지만 실제 포트, 레이저 응답, 라이선스와 스캔헤드 추종 성능을 증명하지 않습니다.

## 문제 해결

### LayerPen 값을 바꿨는데 적용되지 않습니다

- Layer의 `PenColor`와 등록된 `EntityLayerPen.PenColor`가 정확히 같은지 확인합니다.
- 현재 Marker가 같은 Document와 장치로 `Ready` 상태인지 확인합니다.
- `OnMarkLayerPen` 구독자가 기본 처리를 대체하고 있지 않은지 확인합니다.
- 해당 RTC가 필요한 인터페이스를 구현하는지 확인합니다.

### ALC를 켜도 출력이 변하지 않습니다

- `IsALC`, `AlcSignal`, `AlcMode` 조합을 확인합니다.
- `AlcPercentage100`, 최소·최대값의 단위가 신호와 맞는지 확인합니다.
- 아날로그/디지털 포트 배선과 레이저 입력 설정을 확인합니다.
- `ActualVelocity`라면 iDRIVE 피드백과 호환 스캔헤드를 확인합니다.

### 위치 보정표를 넣은 뒤 설정이 실패합니다

- 반경이 유효 필드 범위 안에 있는지 확인합니다.
- 배율이 0~4 범위인지 확인합니다.
- 유효 지점이 50개 이하인지 확인합니다.
- 중복 또는 잘못된 항목을 제거하고 반경 순서로 정리합니다.

### Sky Writing을 켠 뒤 선이 길어지거나 시간이 크게 늘어납니다

보조 run-in/run-out 이동이 추가된 정상적인 결과일 수 있습니다. `Prev`, `Post`를 품질이 유지되는 범위에서 줄이고 Mode2 또는 Mode3을 비교하십시오. Auto Delay를 사용 중이면 Preview Time과 SCANAhead 설정을 먼저 확인합니다.

### 모서리가 너무 진하거나 끊깁니다

- `EntityPen.ScannerPolygonDelay`를 먼저 확인합니다.
- `VariablePolygonDelayEdgeLevel`이 너무 높아 과도한 출사가 유지되거나, 너무 낮아 작은 모서리에서도 레이저가 꺼지지 않는지 비교합니다.
- Sky Writing과 Variable Polygon Delay를 동시에 임의 조정하지 말고 하나씩 검증합니다.

### syncAXIS 속성이 보이지만 동작하지 않습니다

syncAXIS 사전 셋업, 라이선스, SCANLAB USB 동글, 축 설정 파일, syncAXIS용 Marker와 `IRtcSyncAxis` 등록을 확인하십시오. 일반 `MarkerRtc`에서는 syncAXIS 구동을 수행하지 않습니다.

## 적용 전 점검표

- Layer 색과 EntityLayerPen 색이 정확히 일치합니다.
- 현재 RTC가 ALC, Sky Writing, Variable Delay 또는 syncAXIS 인터페이스를 지원합니다.
- ALC 신호의 단위와 포트 배선이 맞습니다.
- ActualVelocity 사용 시 호환 스캔헤드의 피드백이 정상입니다.
- Spot Distance 사용 시 SCANAhead, Auto Delay, PoD 레이저와 외부 트리거를 확인했습니다.
- Sky Writing 보조 이동이 장비의 안전 영역 안에 있습니다.
- Variable Delay를 실제 Jump 길이와 모서리 각도로 시험했습니다.
- LayerFirst/OffsetFirst 순서와 `EntityLayer.Repeats`가 공정 의도와 맞습니다.
- 낮은 출력의 시험편에서 레이어 전환과 시작·끝 품질을 확인했습니다.

## 관련 문서

- [PenUserManual.md](PenUserManual.koKR.md): EntityPen 출력, 펄스, 속도, 지연, Raster, Wobbel, SCANAhead
- [MarkerUserManual.md](MarkerUserManual.koKR.md): Page, Layer, Offset과 Marker 실행 순서
- [Rtc6UserManual.md](Rtc6UserManual.koKR.md): RTC6 하드웨어, 포트와 기본 명령
- [Rtc6SyncaxisUserManual.md](Rtc6SyncaxisUserManual.koKR.md): syncAXIS 사전 셋업과 운용
- [MeasurementUserManual.md](MeasurementUserManual.koKR.md): 측정 채널 등록과 샘플링
- [Sirius3UIConfigUserManual.md](Sirius3UIConfigUserManual.koKR.md): 편집기 기본 LayerPen 설정

---

2026 Copyright (c) SpiralLAB. All rights reserved.
