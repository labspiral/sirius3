# SIRIUS3 Multi-Beam Laser Control System User Manual

> 기준 버전: Sirius3 1.12.3 (공개 Release 기능)

## 1. 개요

Multi-Beam은 하나의 레이저 소스를 두 스캔헤드가 공유하는 장치 구성입니다. 두 RTC와 두 AOM을 사용하며, 교차 연결된 Token DIO로 레이저 사용권을 한 Head씩 전달합니다.

## 2. 구성 요소

| 구성 | 역할 |
|---|---|
| RTC/Head 1 | 첫 번째 스캔헤드 이동과 AOM/Token 제어 |
| RTC/Head 2 | 두 번째 스캔헤드 이동과 AOM/Token 제어 |
| Laser Source | 두 Head가 공유하는 광원 |
| AOM 1/2 | 출사 광로를 선택 |
| Token DI/DO | 두 RTC가 상대의 레이저 사용 여부를 확인 |

Token과 AOM은 소프트웨어 안전 장치만이 아닙니다. 실제 광학 차단, 인터록과 장비 비상 정지는 별도로 구성해야 합니다.

## 3. 가공 모드

- Head1: 첫 번째 Head만 사용
- Head2: 두 번째 Head만 사용
- Both: 두 Head가 Jump 구간에서 Token을 교환하며 번갈아 사용
- Reset/None: AOM과 Token을 비활성 상태로 복귀

Both Mode에서 Preferred Side는 두 Head가 동시에 시작할 때 먼저 Token을 받을 Head를 정합니다. 전체 작업을 항상 먼저 끝낸다는 의미는 아닙니다.

## 4. 작업 전 점검

1. 두 RTC, Laser와 Marker가 Ready인지 확인합니다.
2. `CheckPins`로 Token DO→상대 DI의 양방향 연결을 검사합니다.
3. AOM 0차/1차 Voltage, Digital Enable Bit와 Hold Time을 확인합니다.
4. Mode와 Preferred Side를 선택합니다.
5. `ReadyMode` 후 각 Head의 상태 표시를 확인합니다.
6. 저출력 시험으로 한 시점에 한 광로만 열리는지 확인합니다.

## 5. Both Mode 동작

각 Head는 가공 구간 전에 상대 Token이 해제될 때까지 기다리고 자신의 Token을 켠 뒤 AOM 광로를 엽니다. 다음 위치로 Jump할 때 AOM과 Token을 해제해 상대 Head가 사용할 수 있게 합니다. 첫 Jump가 동시에 시작되면 짧은 Guard Wait로 Preferred Side의 우선순위를 보장합니다.

Sirius3 1.11.14부터 JumpAndShoot의 반복 Token 교환에서 대기 일부를 실제 Jump와 겹쳐 불필요한 정지 시간을 줄였습니다.

## 6. 상태와 오류

- Ready 꺼짐: Abort/Error 뒤 `ResetMode`와 `ReadyMode`를 다시 실행
- Token 대기: 상대 Head의 Busy/Error와 Token 출력 확인
- 한 Head만 출력: Mode, AOM Voltage/Bit, 광학 정렬 확인
- 동시 출력 의심: 즉시 작업을 중지하고 하드웨어 인터록과 배선을 검사

## 7. 관련 공개 예제

`editor_multibeam2`와 공통 `multibeamhelper.cs`는 `config_multibeam.ini`를 읽어 두 RTC, AOM/Token과 Marker를 초기화하는 흐름을 보여 줍니다. 데모의 Bit Mask, Voltage, KFactor는 설치 장비에 맞게 바꿔야 합니다.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
