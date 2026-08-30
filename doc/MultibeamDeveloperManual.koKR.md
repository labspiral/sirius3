# IRtcMultiBeam Developer Manual

> 기준 버전: Sirius3 1.12.3 (공개 Release 기능)

## 1. 시스템 구조

Multi-Beam은 하나의 레이저 소스를 두 RTC와 두 스캔헤드가 공유하는 시스템입니다. 두 AOM이 광로를 선택하고, RTC 간 교차 연결된 DInput/DOutput Token 신호가 한 시점에 한 Head만 레이저를 사용하도록 조정합니다.

`SiriusMultiEditorControl`처럼 독립 장치 세트를 화면에서 전환하는 기능과는 다릅니다. Multi-Beam은 실제 광원 사용권을 List 명령 수준에서 동기화합니다.

## 2. IRtcMultiBeam

설치한 컨트롤러에 맞는 Factory를 사용합니다.

- `ScannerFactory.CreateRtc5MultiBeam`
- `ScannerFactory.CreateRtc6MultiBeam`
- `ScannerFactory.CreateRtc6EthernetMultiBeam`

응용 프로그램이 `IRtcMultiBeam`을 직접 구현하기보다 제공 구현과 `RtcMultiBeamHelper`를 사용하십시오. 네이티브 RTC 명령, AOM 순서, Token 대기와 오류 복구가 함께 동작해야 하기 때문입니다.

## 3. 주요 속성

| 속성 | 역할 |
|---|---|
| `MultiBeamIndex` | Pair와 Head 역할을 정하는 0 기반 인덱스 |
| `TokenBitMask` | 자신의 사용 상태를 출력하고 상대 상태를 입력에서 검사할 Bit |
| `AOMBitMask` | AOM Digital Enable Bit |
| `AOMChannel` | AOM 전압을 출력할 RTC Extension Analog Channel |
| `AOM0OrderVoltage` | 비가공 광로 전압 |
| `AOM1stOrderVoltage` | 가공 광로 전압 |
| `AOMHoldMsec` | AOM 전환 후 안정화 대기 |
| `ListAOM(onOff)` | List 버퍼 안에서 AOM Bit·Voltage·대기를 순서대로 기록 |

같은 Pair의 두 인스턴스는 서로 다른 `MultiBeamIndex`를 가져야 하며 Token/AOM Bit가 겹치지 않아야 합니다.

## 4. config_multibeam.ini 초기화

공개 `multibeamhelper.cs`는 `config_multibeam.ini`를 읽어 다음 순서로 준비합니다.

1. 각 RTC의 카드/이더넷 설정과 bits/mm 단위 KFactor를 읽습니다.
2. RTC와 Correction Table을 초기화합니다.
3. Extension/LASER-port I/O를 만듭니다.
4. Laser와 Marker를 만듭니다.
5. `MultiBeamIndex`, Token/AOM Mask, Channel, Voltage, Hold Time을 설정합니다.
6. 두 인스턴스를 Helper에 Pair로 등록합니다.
7. `CheckPins(pairIndex)`로 대칭 배선을 검사합니다.
8. Mode와 Preferred Side를 정하고 `ReadyMode(pairIndex)`를 호출합니다.

두 RTC 인스턴스와 Multi-Beam 옵션에 맞는 License가 필요합니다. Factory가 인스턴스를 반환했다는 사실만으로 실제 옵션이 활성화되었다고 판단하지 마십시오.

## 5. ReadyMode

`ReadyMode`는 선택한 Mode에 맞춰 AOM Voltage/Bit와 Token 출력을 초기화합니다.

- Head1: Head1 광로만 열 수 있도록 초기화
- Head2: Head2 광로만 열 수 있도록 초기화
- Both: 두 Head가 Token을 교환할 준비를 하고 Preferred Side에 초기 우선권 부여
- None/Reset: 두 광로와 Token을 안전한 비활성 상태로 복귀

Abort나 오류 뒤에는 Ready 상태가 해제될 수 있습니다. 다음 가공 전에 `IsInstanceReady`와 실제 AOM/Token 상태를 확인하고 필요하면 `ResetMode` 후 다시 Ready하십시오.

## 6. Both Mode의 List 순서

1. List 시작 시 AOM을 닫습니다.
2. 현재 Token을 해제합니다.
3. 다음 위치로 Jump합니다.
4. 상대 Token 입력이 LOW가 될 때까지 기다립니다.
5. 자신의 Token 출력을 HIGH로 설정합니다.
6. `ListAOM(true)`로 광로를 열고 EntityPen 출력을 복원합니다.
7. Mark를 실행합니다.
8. List 종료 또는 다음 교환에서 AOM과 Token을 해제합니다.

첫 Jump가 동시에 시작될 때 비선호 Head는 짧은 Guard Wait를 두어 결정적인 우선순위를 만듭니다. Sirius3 1.11.14부터 반복 JumpAndShoot에서 Token 해제 대기를 실제 Jump와 겹치고, 비용이 더 작은 짧은 Jump만 묶어 Both Mode의 불필요한 대기를 줄였습니다.

## 7. 오류 처리

- `CheckPins` 실패: 가공을 시작하지 말고 DO→DI 양방향 배선과 Bit Mask를 확인
- Token 대기 지속: 상대 RTC의 Busy/Error, Abort 상태와 Token DO를 확인
- AOM 출력 불일치: Channel, 0/1차 Voltage, Enable Bit, Hold Time을 확인
- 한 Head만 출사: Mode, Preferred Side, Laser/AOM 광학 정렬과 Marker Ready 확인

오류 복구는 `ResetMode`로 AOM과 Token을 낮춘 뒤 두 RTC 상태를 다시 확인하는 순서로 진행하십시오.

## 8. 공개 데모

- `editor_multibeam2`: 두 Editor/Marker와 Mode 전환을 포함한 UI 예제
- `editor_multiple2`: 여러 독립 장치 세트와 MultiEditorControl 비교용
- 공통 `multibeamhelper.cs`: INI 읽기, Factory 생성, Pair 등록, 배선 검사와 Ready 흐름

데모의 배선 Bit, 전압과 KFactor는 예시입니다. 설치 장비의 회로와 광학계에 맞게 검증한 값으로 바꾸십시오.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
