# SCANLAB RTC6 Controller User Manual

> 기준 버전: Sirius3 1.12.3 (공개 Release 기능)

## 1. RTC6의 역할

RTC6는 Windows에서 준비한 형상과 가공 조건을 스캐너·레이저·I/O 명령으로 바꾸고, 일반적인 Windows 스케줄링과 분리된 실시간 시간축에서 실행하는 컨트롤러입니다. Sirius3에서는 RTC6/RTC6e를 기본 기준으로 설명합니다.

리스트 전송 성공은 RTC가 명령을 받아 실행을 시작했다는 뜻입니다. 실제 완료 여부는 `Ready`, `Busy`, `Error`, Job 상태와 장치 로그를 함께 확인해야 합니다.

## 2. 좌표와 KFactor

- RTC6는 스캔 필드를 고해상도 Controller Bit 좌표로 처리합니다.
- `KFactor`의 단위는 **bits/mm**입니다.
- 변환식은 **Controller 위치(bit) = 사용자 입력 위치(mm) × KFactor(bits/mm)** 입니다.
- 예를 들어 KFactor가 10,000 bits/mm이면 12.5 mm는 필드 보정 전에 125,000 bit로 변환됩니다.
- `2<sup>20</sup> ÷ FOV`는 축척을 이해하기 위한 예일 뿐입니다. 실제 값은 RTC 세대, 좌표 범위, 보정 파일, 광학 필드와 축 방향에 맞춰 정해야 합니다.

KFactor는 전체 축척을 맞추는 값입니다. 렌즈와 스캐너에서 생기는 배럴·핀쿠션·기울어짐·국부 왜곡은 2D 또는 3D 보정 파일로 따로 보정합니다.

## 3. Jump, Mark, Arc와 Microstep

- `Jump`: 레이저 활성 신호를 끈 상태로 다음 시작점까지 이동합니다.
- `Mark`: `LASERON`을 활성화하고 직선 경로를 가공합니다.
- `Arc`: 현재 위치에서 중심과 각도를 사용해 원호를 가공합니다.

RTC6는 Jump, Mark, Arc를 **10 µs 간격의 위치 설정값(Microstep)** 으로 나누어 스캔헤드에 출력합니다. 일정 속도 `v`에서 설정점 간 거리는 `Δs = v × 10 µs`입니다. 여기서 Microstep은 RTC의 이동 시간 단위이며 Sirius3의 `MicroVector` 래스터 방식이나 레이저 펄스 폭과는 다른 개념입니다.

## 4. 리스트 버퍼

Sirius3에서 사용하는 RTC6 Single List 용량은 **2<sup>23</sup>개 명령**입니다. 형상 명령뿐 아니라 펜, 지연, 레이저, I/O, 대기와 종료 명령도 같은 용량을 사용하므로 최대치까지 채우지 말고 여유를 남겨야 합니다.

- `Single`: 하나의 유한한 RTC 리스트를 작성하고 실행합니다.
- `Auto`: Sirius3가 네이티브 리스트를 교대로 채우고 실행해 긴 논리 작업을 이어 갑니다.
- `Ctl*`: RTC 상태를 즉시 바꾸는 제어 명령입니다.
- `List*`: `ListBegin`과 `ListEnd` 사이에 기록되며 RTC 리스트 버퍼에서 순서대로 실행됩니다.

## 5. 레이저 지연과 스캐너 지연

스캐너 미러와 레이저 소스는 명령을 받은 즉시 이상적으로 반응하지 않습니다. 레이저 지연은 실제 출사 시작·종료를 이동점과 맞추고, 스캐너 지연은 미러가 목표 위치와 속도를 따라갈 시간을 제공합니다.

1. 낮은 출력과 안전한 시험편으로 직선·모서리 시험 패턴을 준비합니다.
2. 레이저 On/Off Delay를 조정해 선의 시작과 끝이 잘리거나 늘어나지 않게 합니다.
3. Jump Delay는 점프 뒤 선 시작점이 흔들리지 않는 최소값을 찾습니다.
4. Mark Delay와 Polygon Delay는 선 끝과 연속 Mark 모서리의 과다 출사를 줄이는 방향으로 조정합니다.
5. 한 번에 한 항목만 바꾸고, 확대 사진이나 계측기로 반복 확인합니다.

RTC6의 레이저 지연 분해능은 1/64 µs이며 스캐너 이동 설정점은 10 µs 간격입니다. 입력 단위와 실제 적용 해상도를 혼동하지 마십시오.

## 6. Variable Polygon Delay

Variable Polygon Delay는 하나의 Polyline에서 **연속된 Mark 선분이 만나는 모서리**에 Polygon Delay의 일부를 각도에 따라 적용합니다. 개체가 화면에서 얼마나 회전했는지를 뜻하지 않습니다.

기본 동작에서는 레이저를 끄지 않고 모서리에 지연을 더하므로 출사 시간이 늘어납니다. 반전에 가까운 모서리에서 레이저를 끄고 현재 Polyline을 끝낼 수 있는 `EdgeLevel`은 별도의 보호 설정입니다. 시험 패턴으로 열 누적과 모서리 형상을 확인해 값을 정하십시오.

## 7. Sky Writing과 SCANAhead

Sky Writing은 가공선 앞뒤에 레이저를 끈 가속·감속 구간을 추가해 실제 Mark 구간을 목표 속도로 통과하게 합니다. Sirius3에서는 `EntityLayerPen`의 Sky Writing 설정을 사용합니다.

- Mode 1/2: 벡터 전후의 Run-in/Run-out과 Sky Jump를 사용합니다.
- Mode 3: 모서리 각도에 따라 Sky Writing 적용 여부를 선택합니다.
- Mode 4: Mode 3을 기반으로 지원되는 짧은 List 명령을 시퀀스 안에 허용하는 RTC6 모드입니다.

SCANAhead는 호환 RTC6와 excelliSCAN 구성에서 미래 궤적을 분석해 지연을 자동 계산합니다. `Preview Time`은 미래 경로를 살펴보는 시간 창이며, 클수록 항상 좋은 값은 아닙니다. Develop Mode에서는 10 µs Tick으로 변환되고, Load Mode에서는 컨트롤러에 저장된 파라미터를 사용하므로 Preview Time·Vmax·Amax 입력을 적용하지 않습니다. 설치된 RTC6 DLL, 펌웨어, 스캔헤드와 보정 파일에 맞춰 검증된 값을 사용하십시오.

## 8. Wobbel

Wobbel은 기본 Mark 경로에 원·타원·8자 등의 주기 운동을 합성해 가공 폭과 에너지 분포를 바꿉니다. `EntityPen`에서 `WobbelShape`, `WobbelFrequency`, `WobbelParallel`, `WobbelPerpendicular`를 설정합니다.

- Parallel과 Perpendicular 진폭이 같으면 원형에 가까운 Ellipse가 됩니다.
- 두 진폭이 다르면 타원이 됩니다.
- Parallel 진폭이 0이면 진행 방향에 수직인 Sine 계열 경로가 됩니다.
- Sky Writing, Pixel Output, Raster와의 조합은 제한될 수 있으므로 실제 RTC6 기능 플래그와 로그를 확인하십시오.

## 9. LASER 포트 신호

`LASER1`, `LASER2`, `LASERON`의 정확한 핀 동작은 선택한 Laser Mode, 극성, 펄스 폭/주파수 설정과 배선에 따라 달라집니다.

- `LASERON`: Mark 구간의 출사 허용 창을 나타내는 게이트로 주로 사용합니다.
- `LASER1`, `LASER2`: Laser Mode에 따라 펄스·주파수·모드 신호로 사용합니다.
- LASER 포트의 `DIGITAL IN1`: `PixelPulses`처럼 외부 레이저 동기 펄스를 계수할 때 사용합니다. 이 기능은 **레이저 SYNC OUT과 DIGITAL IN1의 물리적 연결**이 필수입니다.

전압 레벨, 극성, 기준 접지와 입력 Edge를 확인하지 않은 상태에서 레이저를 연결하지 마십시오.

## 10. 필드 보정과 상태 확인

F-theta 렌즈의 비선형성, 미러 각도, 광축 편심, 스캔헤드 장착 오차 때문에 명령 좌표와 실제 가공 위치는 일치하지 않을 수 있습니다. 먼저 올바른 KFactor로 전체 축척을 맞춘 뒤 2D/3D 보정 파일로 위치별 오차를 줄입니다.

RTC6 초기화 후에는 보정 파일과 Table 선택, Laser Mode, Scanner Ready, Error Code, 아날로그·디지털 I/O 상태를 확인하십시오. Sirius3 1.12.3은 RTC6 상태와 아날로그 I/O를 세대에 맞는 컨트롤러 API로 읽고 Ethernet 연결 오류와 종료 중 상태 타이머를 안정적으로 처리합니다.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
