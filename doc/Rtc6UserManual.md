# SCANLAB RTC6 Controller User Manual


## 1. 개요 (Overview)

RTC6는 SCANLAB의 차세대 주력 컨트롤러로, RTC5의 모든 기능을 계승하면서 대폭 확장된 메모리와 분해능, 그리고 혁신적인 SCANAhead 기술을 제공합니다. 
초정밀 3D 가공 및 초고속 excelliSCAN 스캐너 제어에 최적화되어 있습니다.

## 2. 향상된 하드웨어 사양 (Enhanced Specs)

- 제어 분해능: XY축 및 Z축 모두 20비트 (1,048,576 steps) 지원. (RTC5 대비 Z축 정밀도 16배 향상)
- 메모리: 최대 8MB (리스트 버퍼 2^21). 대규모 도면 처리에 매우 유리합니다.
- 보정 테이블: 최대 8개의 테이블 상주 가능. (RTC5 대비 2배)
- 측정 채널: 8개의 하드웨어 측정 채널 제공.

## 3. 전용 고급 기능 (Advanced RTC6-Only Features)

- SCANAhead: excelliSCAN 시스템과 결합하여 경로를 미리 분석하고 지연 시간(Delays)을 자동으로 제로화하는 차세대 제어 기술.
- 정밀 타임베이스: 레이저 지연 및 시프트 제어 분해능이 1/64µs (~15.6ns)로 극대화되어 극초단파(USP) 레이저 제어에 최적입니다.
- UFPM (Universal Fast Pixel Marking): 초고속 비트맵 가공 및 래스터 이미징 지원.
- 확장된 MoF: 보다 복잡한 다축 동기화(Fly extension) 및 McBSP 등 연동 가공 지원.

## 4. 시스템 인터페이스 (Connectivity)

- 이더넷(Ethernet) 및 PCI Express 인터페이스 지원.
- 원격 가공(Remote Processing)을 위한 강력한 네트워크 레이어 제공.

## 5. RTC5와의 호환성 (Compatibility)

- Sirius3 프레임워크 내에서 RTC5와 대부분의 API 수준 호환성을 유지하므로, 기존 코드를 최소한의 수정으로 RTC6 시스템으로 마이그레이션할 수 있습니다.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
