# Integrated 3D Calibration User Manual

> 기준 버전: Sirius3 1.12.3 (공개 Release 기능)


## 1. 개요 (Overview)

본 매뉴얼은 SCANLAB RTC 제어기 및 CalibrationLibrary v1.4.1.1을 사용하는 Sirius3 3D 통합 보정(Calibration) 윈폼의 사용 방법을 설명합니다. 
3D 보정은 스캐너의 기하학적 오차 보정뿐만 아니라, 전 작업 영역에서 레이저 초점(Focus)을 일정하게 유지하기 위한 필수 절차입니다.

## 2. 준비 사항 (Prerequisites)

- RTC 제어기 및 3D 스캐너(예: varioSCAN)가 정상 연결되어야 합니다.
- 기본 보정 파일(.ctb 또는 .ct5)과 해당 파일의 README 텍스트 파일이 필요합니다.
- 정확한 K-Factor (bits/mm) 값을 알고 있어야 합니다.

## 3. 보정 절차 (Step-by-Step Procedure)

보정은 아래의 순서대로 진행하는 것을 권장하며, 각 단계가 완료될 때마다 'Apply' 버튼을 눌러 하드웨어에 반영해야 다음 단계의 정밀도가 높아집니다.

Step 1: Beam Tilt Calibration (빔 틸트 보정)
- 스캐너의 광축과 varioSCAN의 중심축 정렬 오차를 보정합니다.
- 상단 평면과 하단 평면에서 각각 중심점 위치를 측정하여 오차(dx, dy)와 두 평면 사이의 거리(Height)를 입력합니다.
- 'Calibrate' 클릭 후 성공 시 'Apply'를 눌러 반영합니다.

Step 2: XY Field Calibration (2D 필드 보정)
- 스캐너의 기하학적 오차(Scale, Rotation, Pincushion 등)를 보정합니다.
- 격자(Grid) 패턴을 가공한 후, 각 타겟 좌표(Target X, Y)에 대한 실제 가공 위치(Measured X, Y)를 측정하여 입력합니다.
- 'Calibrate' 후 'Apply'를 누릅니다. (이후 Source File이 생성된 파일로 자동 갱신됩니다.)

Step 3: Focus Calibration at Z=0 (Z=0 평면 초점 보정)
- 기본 작업 평면(Z=0) 전체 영역에서 초점이 일정하게 맺히도록 미세 조정합니다.
- 여러 지점에서 A(Z 제어 비트) 값을 변경하며 가장 선명한 초점이 맺히는 값을 찾아 입력합니다.
- A 값 수정시 내부적으로  IRtc3D.CtlLoadZTable(A, 0, 0) 이 자동 호출됩니다.
- 셀을 선택하면 스캐너가 해당 위치로 자동 이동하며, A값을 조절하여 실시간으로 확인 가능합니다.

Step 4: Focus Coeff A,B,C Calibration (ABC 계수 보정)
- Z축 이동 거리에 따른 초점 변화 곡선(Zout = A + Bl + Cl<sup>2</sup>)의 계수를 산출합니다.
- 다양한 Z 높이에서 최적의 초점 제어값(A)을 찾습니다.
- X,Y,Z 위치값 선택시 스캐너 위치가 자동 이동되며, IRtc3D.CtlZDistance 를 이용해 L(Focal Length Deviation bits) 값이 자동 계산됩니다.
- A 값 수정시 내부적으로  IRtc3D.CtlLoadZTable(A, 0, 0) 이 자동 호출됩니다.


Step 5: Stretch Calibration (Z 볼륨 스트레치 보정)
- 텔레센트릭(Telecentric) 렌즈를 사용하지 않을 경우 스트레치 보정이 반드시 필요합니다.
- Z축 높이 변화에 따른 가공 크기(Scale)의 변화를 보정합니다.
- Z+, Z- 평면에서 각각 테스트 패턴을 가공하고 측정값을 입력합니다.

## 4. 주요 기능 설명 (Key Features)

- Open/Save (마우스 우클릭): 입력한 측정 데이터를 텍스트 파일로 저장하거나 다시 불러올 수 있습니다.
- Manual Control: 우측 패널을 통해 레이저 및 스캐너를 수동으로 제어하여 테스트 가공을 수행할 수 있습니다.
- Live Update: Focus 관련 탭에서는 그리드 셀 값을 수정하는 즉시 스캐너의 Z 위치가 실시간으로 변경됩니다.

## 5. 주의 사항 (Cautions)

- 보정 절차 중 스캐너나 하드웨어를 강제로 조작하지 마십시오.
- 'Apply'를 누르면 현재 로드된 보정 파일이 실제 하드웨어에 적용되며, 이후의 보정은 이 파일을 기준으로 누적 진행됩니다.
- K-Factor 값이 정확하지 않으면 모든 계산 결과에 오차가 발생합니다.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
