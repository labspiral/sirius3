# syncAXIS (XL-SCAN) Coordinated Motion User Manual


## 1. 개요 (Overview)

syncAXIS(XL-SCAN)는 SCANLAB RTC6 컨트롤러, excelliSCAN 스캐너, 그리고 ACS 모션 컨트롤러를 결합하여 스캐너와 스테이지 간의 완벽한 동기화 구동을 실현하는 솔루션입니다. 
물리적인 스캐너 필드(FOV)의 한계를 넘어 스테이지 전체 가공 영역을 하나의 거대한 가상 이미지 필드로 처리합니다.

## 2. 주요 동작 모드 (Motion Types)

사용자는 가공 목적에 따라 다음과 같은 협조 구동 방식을 선택할 수 있습니다.
- Scanner Only: 스테이지는 고정하고 스캔 헤드만 움직여 가공합니다.
- Stage Only: 스캐너 미러는 고정하고 스테이지의 이동만으로 가공합니다.
- Stage + Scanner: 스캐너의 고속 동특성과 스테이지의 넓은 가공 영역을 결합합니다. 자동 궤적 계획 엔진이 부하를 분산 처리합니다.

## 3. 핵심 제어 파라미터 (Key Parameters)

- BandWidth (Hz): 스테이지와 스캐너 간의 부하 분담 비율을 결정합니다. 값이 높을수록 스테이지가 더 많이 기여하며, 낮을수록 스캐너가 더 정밀한 움직임을 담당합니다. (최소 0.23Hz)
- Motion Mode (Follow/Unfollow): 스테이지가 스캐너의 경로를 실시간으로 추종할지 여부를 결정합니다.
- Heuristic Index: 설정 XML에 정의된 동적 감속 함수를 선택하여 코너 구간 등의 속도를 최적화합니다.

## 4. 시스템 상태 모니터링 (Status)

syncAXIS 인스턴스는 다음과 같은 신호등 색상으로 상태를 표시합니다.
- Green: 정상 동작 및 가공 준비 완료.
- Yellow: 초기화 진행 중 또는 일시적 대기 상태.
- Red: 시스템 정지 및 에러 발생 (스테이지 통신 오류 등).

## 5. 고급 마킹 기능 (Advanced Marking)

- Dashed Mark: 선분이나 호 가공 시 지정한 길이에 맞춰 레이저를 주기적으로 온/오프하는 기능을 지원합니다.
- Trajectory Planning: 동특성(Acceleration, Jerk) 제한을 준수하면서 최적의 속도 프로파일을 하드웨어적으로 자동 생성합니다.

## 6. 주의 사항 (Cautions)

- 하드웨어 구성품 중 하나라도 통신이 단절되면(RTC6 <-> ACS) 즉시 가공이 중단됩니다.
- 전체 시스템 설정은 외부 XML 파일(`ConfigXMLFile`)을 통해 관리되며, 이 파일의 물리적 경로가 올바르게 지정되어야 초기화에 성공합니다.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
