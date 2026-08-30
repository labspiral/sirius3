# SCANLAB RTC5 Controller User Manual

> 기준 버전: Sirius3 1.12.3 (공개 Release 기능)

## 1. RTC5의 위치

RTC5/RTC5e는 20-bit XY 좌표계와 고급 가공 기능을 제공하는 컨트롤러입니다. RTC4보다 리스트와 기능이 확장되지만, RTC6 전용 SCANAhead와 Fly Extension은 지원하지 않습니다.

## 2. 핵심 사양

- XY 좌표: 20-bit, Z 좌표: 16-bit
- Sirius3 Single List 용량: 2<sup>20</sup>개 명령
- 보정 테이블: 4개
- 측정 채널: 4개
- 스캐너 이동 설정점: 10 µs 단위
- 레이저 지연: 0.5 µs 단위

Single List에는 형상뿐 아니라 펜·레이저·I/O·대기·종료 명령도 들어갑니다. 긴 작업은 `ListBufferTypes.Auto`와 Job 상태를 사용해 전송과 실제 완료를 구분하십시오.

## 3. Sirius3에서 지원하는 기능

- Jump, Mark, Arc, Raster, Timed Marking
- 2nd Head, 3D, Classic MoF
- Variable Jump/Polygon Delay와 Jump Mode
- Sky Writing, Wobbel, ALC
- Character Set, Measurement, Free Variable
- RTC Serial Communication, Interrupt, Stepper

RTC5는 `IRtcSCANAhead`와 `IRtcMoFExtension`을 제공하지 않습니다. RTC6용 SCANAhead·Fly Extension 코드를 이름이 비슷하다는 이유로 이식하지 마십시오.

## 4. KFactor와 보정

`KFactor`의 단위는 bits/mm이며 **Controller 위치(bit) = 사용자 입력 위치(mm) × KFactor(bits/mm)** 로 변환합니다. KFactor는 전체 축척을, `.ct5`/`.ctb` 보정표는 위치별 비선형 왜곡을 보상합니다.

## 5. 지연과 고급 기능

Laser On/Off, Jump, Mark, Polygon Delay를 실제 스캔헤드와 레이저 응답에 맞춰 조정하십시오. Sky Writing과 ALC는 각 기능의 전제 조건과 조합 제한을 확인해야 하며, Wobbel은 EntityPen에서 설정합니다. 한 번에 한 값만 바꾸고 저출력 시험 패턴으로 검증하는 것이 안전합니다.

## 6. 인터페이스와 포트

RTC5는 LASER 포트, 확장 I/O, Scan Head 포트, MoF/Encoder, RS-232, Stepper 기능을 제공할 수 있습니다. 실제 사용 가능 여부는 카드 옵션, 펌웨어, 케이블과 라이선스에 따라 달라집니다. 핀 전압·극성·접지를 확인한 뒤 연결하십시오.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
