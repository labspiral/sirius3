# SCANLAB RTC4 Controller User Manual


## 1. 개요 (Overview)

RTC4는 SCANLAB의 스테디셀러 컨트롤러로, 합리적인 비용으로 안정적인 레이저 가공 성능을 제공합니다. 
RTC5/6와 같은 최신형 보드에 비해 기능적 제약은 있으나, 기본적인 2D/3D 마킹 및 MoF 가공에 충분한 사양을 갖추고 있습니다.

## 2. 주요 하드웨어 사양 (Specifications)

- 제어 분해능: XY축 16비트 (65,536 steps), Z축 16비트.
- 메모리: 최대 8,000 리스트 명령 저장 (리스트 버퍼 8KB).
- 보정 테이블: 최대 2개의 .ctb 파일을 하드웨어 메모리에 로드 가능.
- 측정 채널: 2개의 하드웨어 측정 채널 지원.

## 3. 지원 핵심 기능 (Key Features)

- 기본 마킹: 리스트 명령 기반의 벡터 및 래스터 가공.
- MoF (Marking on the Fly): 이동 중인 물체에 대한 동기화 가공 지원.
- 3D 가공: varioSCAN과 연동된 3D 보정 및 초점 제어 (3D 옵션 탑재 모델 한정).
- Wobbel: 단순 원형 또는 선형 진동 패턴 지원.

## 4. RTC5/6 대비 주요 제약 사항 (Limitations)

- 고급 보정 미지원: Skywriting(코너 보정), ALC(자동 속도-파워 변조) 등의 지능형 제어 기능을 하드웨어적으로 지원하지 않습니다.
- 타임베이스: 10µs 고정 타임베이스를 사용하므로, 초미세 타이밍 제어가 필요한 USP 레이저 가공에는 적합하지 않을 수 있습니다.
- 확장성: RS-232 시리얼 통신이나 고급 인터럽트 기능이 제한적입니다.

## 5. 하드웨어 설정 (Configuration)

- 레이저 제어 신호(LASER1, 2, LASERON)의 극성(Active High/Low)은 보드 상의 점퍼 납땜(Soldering) 상태에 따라 결정되므로 소프트웨어 설정 전 확인이 필요합니다.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
