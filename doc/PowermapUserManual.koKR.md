# Laser Power Mapping & Compensation User Manual

> 기준 버전: Sirius3 1.12.3 (공개 Release 기능)


## 1. 개요 (Overview)

본 매뉴얼은 레이저 소스의 제어 신호(입력값, X)와 실제 출력되는 광학 파워(실측값, Y) 사이의 비선형성을 보정하기 위한 Sirius3 PowerMap 시스템의 사용 방법을 설명합니다. 외부 파워 미터(Power Meter)를 연동하여 정밀한 출력 테이블을 생성하고, 이를 실제 가공에 적용하여 전 영역에서 균일한 가공 품질을 확보할 수 있습니다.

## 2. 시스템 구성 및 작동 원리 (System Components)

- IPowerMap: 입력값(X) 대비 실측값(Y)의 룩업 테이블(LUT)을 관리하는 핵심 엔진입니다.
- IPowerMeter: 레이저 출력을 정밀하게 계측하는 하드웨어(Ophir, Coherent, Thorlabs 등)입니다.
- ILaserPowerControl: 생성된 매핑 데이터를 사용하여 실제 가공 시 출력을 조절합니다.
- 작동 원리: 
  - 매핑(Mapping): 제어 명령(X)을 단계별로 높여가며 실제 출력(Y)을 측정하여 저장합니다.
  - 조회(LookUp): 사용자가 목표 파워(Y)를 지시하면, 테이블을 역산하여 정확한 제어값(X)을 도출합니다.

## 3. 매핑 및 보정 절차 (Calibration Workflow)


Step 1: 하드웨어 준비
- 레이저 출력부 앞에 파워 미터 센서를 설치합니다.
- UI에서 Scanner, Laser, PowerMeter 인스턴스가 정상 연결되었는지 확인합니다.

Step 2: 파워 매핑 (Mapping Start)
- 'Power Map Start'를 클릭하여 매핑 범위를 설정합니다. (Min/Max Watt, Step 수)
- 카테고리 설정: 주파수(Frequency)별로 레이저 특성이 다르므로, 주파수 단위로 카테고리를 나누어 관리하는 것을 권장합니다.
- 진행: 레이저가 단계별로 출력되며 파워 미터의 실측값이 자동으로 수집되어 차트에 기록됩니다.

Step 3: 파워 검증 (Verify)
- 생성된 매핑 데이터가 현재 장비 상태와 일치하는지 확인합니다.
- 설정된 목표 파워를 출력했을 때, 실측값이 허용 오차 범위 내에 들어오는지 체크합니다.

Step 4: 파워 보상 (Compensate)
- 장시간 사용에 따른 에이징(Aging)으로 파워가 감소한 경우 수행합니다.
- 기존 매핑 데이터를 기준으로 편차를 계산하여 테이블을 최신 상태로 업데이트(Closed-loop)합니다.

## 4. 실제 가공에 적용 (Real-time Processing)

생성된 매핑 데이터는 가공 리스트 실행 시 실시간으로 조회(LookUp)됩니다.

- 룩업 활성화: `PowerMap.IsEnableLookUp = true` 설정이 필요합니다.
- 리스트 명령 사용: 
  - 가공 리스트 내에서 `ListPower(targetWatt, category)` 함수를 호출합니다.
  - 소프트웨어는 지정된 'category' 테이블에서 'targetWatt'가 출력되기 위한 최적의 제어 신호값을 실시간으로 계산하여 RTC 제어기에 전달합니다.
- 장점: 10µs 단위의 리스트 실행 주기와 동기화되어, 선분마다 다른 출력을 지시해도 끊김 없이 정확한 보정값이 반영됩니다.

## 5. 주요 기능 설명 (Key Features)

- Category 관리: 다중 스캔 헤드, 파장 변환(SHG/THG) 등 물리적 조건별로 독립적인 매핑 테이블을 운영할 수 있습니다.
- 1:1 Reset: 매핑 데이터 없이 입력값 그대로 출력하도록 초기화합니다 (Y=X).
- Data Persistence: 매핑 결과는 `.map` 파일로 저장 및 로드할 수 있어 장비별 관리가 용이합니다.

## 6. 주의 사항 (Cautions)

- 매핑 중에는 센서 보호를 위해 냉각 상태를 확인하고, 최대 허용 에너지를 초과하지 마십시오.
- Pre-Heat Time: 정확한 계측을 위해 레이저가 안정화될 수 있는 예열 시간(Settling time)을 충분히 설정하십시오.
- K-Factor 및 광학계: 스캐너 보정(Field Correction)이 선행되어야 하며, 광학계 오염 시 매핑 데이터의 신뢰성이 저하될 수 있습니다.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
