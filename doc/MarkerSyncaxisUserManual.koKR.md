# SyncAXIS Integrated Marker Control User Manual

> 기준 버전: Sirius3 1.12.3 (공개 Release 기능)


## 1. 개요 (Overview)

본 매뉴얼은 SCANLAB SyncAXIS 하드웨어를 제어하여 스캐너(Scanner)와 모션 스테이지(Stage)를 초정밀 동기화하여 가공하는 Sirius3 MarkerSyncAxis 및 MarkerSyncAxisControl의 사용 방법을 설명합니다. SyncAXIS는 대면적 가공이나 연속 이송 가공 시 스캐너와 스테이지를 하나의 가상 축으로 묶어 제어하는 시스템입니다.

## 2. 시스템 상태 확인 (Operation Status) - SyncAXIS 전용

일반적인 Ready/Busy/Error 상태 외에 SyncAXIS의 내부 제어 상태를 색상으로 표시합니다.
- Dark Gray (Unknown): 시스템이 초기화되지 않았거나 통신 불능 상태.
- Red (Stop): 시스템에 치명적인 에러가 발생하여 정지된 상태.
- Yellow (Warning): 주의 상태이며, 특정 조건이 만족되지 않음.
- Green (OK): 모든 동기화 준비가 완료되어 가공이 가능한 상태.

## 3. 가공 설정 (Targets & Procedures)

- Mark Target (가공 대상):
  - All: 도면 내의 모든 엔티티를 가공합니다.
  - Selected: 선택된 특정 엔티티들만 동기화 가공을 수행합니다.
- Mark Procedure (가공 순서):
  - Layer First: 각 오프셋(제품 위치)으로 스테이지를 이동한 후 해당 위치에서 모든 레이어를 가공합니다.
  - Offset First: 하나의 레이어를 모든 제품 위치에서 가공한 후 다음 레이어로 넘어갑니다.

## 4. 주요 기능 버튼 (Action Buttons)

- Start: 스캐너와 스테이지의 동기화 가공 리스트를 생성하여 실행합니다.
- Stop: 진행 중인 모든 동기화 모션을 즉시 중단합니다.
- Preview: 가이드 레이저를 사용하여 스테이지가 이동할 전체 범위와 스캐너 가공 영역의 외곽을 미리 표시합니다.
- Reset: SyncAXIS 컨트롤러 및 레이저의 에러 상태를 초기화합니다.

## 5. 시뮬레이션 및 시각화 (Simulation & Plot)

SyncAXIS는 가공 전 실제 경로를 예측하기 위해 시뮬레이션 기능을 자주 사용합니다.
- Measurement Plot: 이 옵션이 활성화된 경우, 시뮬레이션 모드에서 생성된 가공 궤적 데이터(.txt)를 가공 완료 직후 'SyncAXIS Viewer'를 통해 그래프로 그려줍니다. 이를 통해 스테이지의 가속도 한계나 스캐너의 가공 속도 적절성을 미리 검증할 수 있습니다.

## 6. 가공 프로세스 흐름 (Processing Flow)

1) 하드웨어를 SyncAXIS 모드로 초기화합니다. (Operation Status가 Green인지 확인)
2) 가공할 도면과 오프셋(배열) 정보를 설정합니다.
3) 'Preview'를 통해 스테이지의 이동 반경에 충돌 위험이 없는지 확인합니다.
4) 'Start'를 클릭하면 스테이지와 스캐너가 실시간 동기화(10µs)되어 가공을 시작합니다.

## 7. 주의 사항 (Cautions)

- SyncAXIS 마커는 일반 RTC 전용 하드웨어(RTC5/6 단독)에는 연결할 수 없습니다.
- SCANLAB 의 SyncAXIS 전용 동글키 + ACS 모션 제어기 + excelliSCAN 고 같은 시스템 조합이 준비되어 있어야 합니다.
- SyncAXIS 셋업 메뉴얼에 따른 다양한 환경 설정이 모두 미리 완료되어야 합니다.
- 가공 중 스테이지의 물리적 한계(Soft Limit)를 벗어나는 경로가 포함될 경우 하드웨어 에러가 발생하므로 시뮬레이션을 통해 사전에 검증하십시오.
- 스테이지와 스캐너의 동기화 정밀도를 위해 전용 보정 파일(xml 또는 .ct5)이 정확히 로드되어 있어야 합니다.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
