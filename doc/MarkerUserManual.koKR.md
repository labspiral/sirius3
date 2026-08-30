# RTC Laser Marker Control User Manual

> 기준 버전: Sirius3 1.12.3 (공개 Release 기능)


## 1. 개요 (Overview)

본 매뉴얼은 SCANLAB RTC 제어기를 사용하여 레이저 가공(마킹)을 수행하는 Sirius3 MarkerRtc 및 MarkerRtcControl의 사용 방법을 설명합니다. 이 시스템은 도면에 배치된 엔티티(도형)들을 실제 가공 리스트로 변환하여 RTC 카드로 전송하고 실행하는 역할을 합니다.

## 2. 마킹 대상 설정 (Mark Targets)

가공할 엔티티의 범위를 선택할 수 있습니다.
- All (전체): 현재 문서 페이지 내의 모든 가공 가능한 엔티티를 순차적으로 마킹합니다.
- Selected (선택): 사용자가 마우스로 선택한 특정 엔티티들만 마킹합니다. (미리보기 시 유용)

## 3. 마킹 절차 및 순서 (Mark Procedures) - 중요

다중 오프셋(배열 가공) 시 가공 효율을 결정하는 중요한 설정입니다.
- Layer First (레이어 우선): 
  - 방식: 오프셋 1번 위치에서 전체 레이어 가공 -> 오프셋 2번 위치에서 전체 레이어 가공...
  - 특징: 한 제품을 완전히 가공한 후 다음 제품으로 이동하는 일반적인 방식입니다.
- Offset First (오프셋 우선): 
  - 방식: 레이어 1번을 모든 오프셋 위치에서 가공 -> 레이어 2번을 모든 오프셋 위치에서 가공...
  - 특징: 툴 교체(공구 교환)나 특정 파라미터 유지가 중요한 경우 사용됩니다.

## 4. 하드웨어 상태 및 안전 확인 (Health Checks)

가공 시작 전 RTC 카드 및 스캐너의 물리적 상태를 자동으로 체크하도록 설정할 수 있습니다.
- Check Temp: 스캐너 헤드의 온도가 정상 범위인지 확인합니다.
- Check Power: 스캐너 전원 공급이 안정적인지 확인합니다.
- Check Position Ack: 스캐너가 지령된 궤적을 정상적으로 추종하는지(오차 범위 내) 확인합니다.

## 5. 주요 기능 버튼 (Action Buttons)

- Start: 설정된 조건에 따라 실제 레이저 마킹을 시작합니다.
- Stop: 진행 중인 가공을 즉시 중단합니다. (RTC List Abort 명령 수행)
- Preview: 실제 레이저를 쏘지 않고 가이드 레이저(Red Pointer)를 사용하여 선택한 엔티티의 외곽 영역(Bounding Box)을 반복 표시합니다. 위치 확인 시 필수 기능입니다.
- Reset: RTC 카드 및 레이저 소스의 에러 상태를 초기화합니다.

## 6. 리스트 버퍼 및 측정 (List & Measurement)

- List Buffer: 'Auto' 또는 'Single' 버퍼 모드를 선택합니다. 대량의 데이터 가공 시 하드웨어 메모리 활용 방식을 결정합니다.
- Measurement Plot: 가공 중 발생하는 스캐너 위치, 속도, 레이저 상태 등의 실시간 데이터를 RTC 내부 메모리에 기록하고, 가공 완료 후 그래프로 시각화합니다. 가공 품질 분석 및 디버깅에 매우 유용합니다.

## 7. 상태 표시등 (Status Indicators)

- Ready: 하드웨어가 초기화되어 가공 준비가 된 상태 (녹색).
- Busy: 현재 가공이 진행 중이거나 리스트가 실행 중인 상태 (황색 점멸).
- Error: 하드웨어 또는 소프트웨어에 치명적인 오류가 발생한 상태 (적색).

## 8. 주의 사항 (Cautions)

- 가공 중(Busy 상태)에는 마킹 절차나 대상을 변경할 수 없습니다.
- 'Stop' 클릭 시 스캐너는 원점(0,0)으로 복귀하며, 가공 중이던 리스트는 취소됩니다.
- 실제 가공 전 반드시 'Preview' 기능을 통해 가공 위치와 범위를 육안으로 확인하십시오.

## 9. 스크립트 (Script)

- ScriptingInstance: 'ScriptFactory.Create' 를 이용해 외부 스크립트 객체를 생성하여 지정가능합니다.
- 텍스트 및 바코드 개체들에 대해 TextConverters.SimpleScript 를 사용할 경우 이 스크립트를 이용해 문자열 변환이 자동 지원됩니다.
- 편집기(EditorControl) 에서 단축키(F2)를 통해 ScriptingInstance 의 속성값 확인 및 편집이 가능합니다.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
