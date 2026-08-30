# SCANLAB RTC4 Controller User Manual

> 기준 버전: Sirius3 1.12.3 (공개 Release 기능)

## 1. RTC4의 위치

RTC4/RTC4e는 16-bit 좌표계와 비교적 작은 리스트를 사용하는 이전 세대 컨트롤러입니다. Sirius3는 RTC4를 계속 지원하지만, RTC5/RTC6와 이름이 비슷한 명령이라도 지원 인터페이스·단위·펌웨어 조건이 다를 수 있습니다.

## 2. 핵심 사양

- XY/Z 좌표: 16-bit
- Sirius3 Single List 용량: 8,000개 명령
- 보정 테이블: 2개
- 측정 채널: 2개
- 스캐너 시간축: 10 µs 단위

리스트에는 이동 명령뿐 아니라 레이저, 지연, I/O와 종료 명령도 들어갑니다. 긴 작업은 `ListBufferTypes.Auto`를 사용하고 버퍼에 여유를 남기십시오.

## 3. Sirius3에서 지원하는 기능

- 기본 `Jump`, `Mark`, `Arc`, Raster, Timed Marking
- 2nd Head와 3D 옵션
- Classic MoF
- Variable Jump/Polygon Delay
- RTC4 Wobbel
- 측정, 조건부 I/O, Interrupt

RTC4 구현은 `IRtcSkyWriting`, `IRtcAutoLaserControl`, `IRtcSerialComm`, `IRtcStepper`, `IRtcSCANAhead`를 제공하지 않습니다. RTC5/RTC6용 설정을 RTC4에 그대로 적용하지 마십시오.

## 4. KFactor와 보정

`KFactor`의 단위는 bits/mm이며 **Controller 위치(bit) = 사용자 입력 위치(mm) × KFactor(bits/mm)** 로 변환합니다. KFactor는 전체 축척을 맞추고, `.ctb` 보정 파일은 필드 위치에 따라 달라지는 렌즈·스캐너 왜곡을 줄입니다.

## 5. 레이저 포트와 안전

`LASER1`, `LASER2`, `LASERON`의 기능과 극성은 Laser Mode 및 보드 설정에 따라 달라집니다. RTC5/RTC6의 핀 동작을 그대로 가정하지 말고 RTC4 세대의 하드웨어 매뉴얼, 실제 배선, 전압 레벨과 오실로스코프 측정으로 확인하십시오.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
