# SiriusEditorControl User Manual

> 기준 버전: Sirius3 1.12.3 (공개 Release 기능)

## 1. 통합 편집기

`SiriusEditorControl`은 EditorControl, Page/Layer TreeView, PropertyGrid와 Scanner·Laser·Marker·I/O·PowerMeter UI를 한 화면에 묶는 공개 WinForms 컨트롤입니다. 공개 `beginner`와 `editor_ui` 데모의 소스를 복사해 메뉴·탭·장치 UI를 제품에 맞게 바꿀 수 있습니다.

## 2. Page 탭

Page 1~4는 서로 다른 도면 또는 공정을 분리합니다. 활성 Page를 바꾸면 Editor, TreeView와 Marker의 기본 대상이 함께 바뀝니다. F5는 현재 Page를 실제로 실행할 수 있습니다.

## 3. Block 탭

Block은 재사용할 마스터 형상을 관리하고 BlockInsert는 Page에 배치된 참조입니다. 반복 로고나 배열에 적합하지만 Block의 ModelMatrix, Insert의 ModelMatrix와 Marker Offset이 중복되지 않게 확인하십시오.

## 4. Entity Pen 탭

개체 색상별 Power, PowerMax, PowerMapCategory, Frequency, PulseWidth, Mark/Jump Speed, 지연, Raster, Hard Jump와 Wobbel을 편집합니다. 개체 가공 중 리스트 명령으로 적용됩니다.

## 5. Layer Pen 탭

Layer 시작 전에 적용할 Sky Writing, ALC, Variable Polygon/Jump Delay, SCANAhead 연계와 syncAXIS 조건을 편집합니다. Layer 순서를 바꾸면 이 제어 조건의 적용 순서도 달라집니다.

## 6. PropertyGrid

Editor 또는 TreeView에서 개체를 선택하면 형상, Text, Hatch, Transform과 Pen 속성이 표시됩니다. 속성 이름·분류·설명을 검색할 수 있고 `CTRL+F`로 검색창에 포커스를 둘 수 있습니다. 다중 선택에서는 공통 속성만 표시됩니다.

## 7. Scanner 탭

RTC 상태, KFactor, 보정 파일과 Table, Laser Mode, Delay, I/O, Measurement와 지원 확장 기능을 확인합니다. RTC6에서는 SCANAhead, Auto Delay와 Preview Time을 설치된 스캔헤드 구성에 맞춰 사용합니다.

## 8. Laser 탭

Laser Ready, 최대 출력, Power Control 방식과 제조사별 설정을 확인합니다. Laser를 등록하면 EntityPen의 PowerMax와 PowerMap 연결을 갱신합니다. 수동 출력 명령은 실제 출사로 이어질 수 있습니다.

## 9. Marker 탭

대상 Page/Layer/Offset, 반복 순서, Preview, Start, Stop, Reset과 진행 상태를 관리합니다. `LayerFirst`와 `OffsetFirst`는 Layer와 Offset 중 어느 축을 바깥쪽 반복으로 둘지 정합니다. 시작 전 Ready/Busy/Error와 현재 대상 수를 확인하십시오.

## 10. 장치 등록

`RegisterDevices`로 Scanner, Laser, PowerMeter, Extension/LASER-port DInput·DOutput, Marker와 Remote를 연결합니다. 데모는 `config*.ini`를 읽어 Factory로 장치를 만들고 등록한 뒤 Marker Ready를 확인합니다.

종료 시 실행 중인 Marker를 먼저 멈추고 생성한 Device를 해제한 뒤 `Core.Cleanup()`을 호출하십시오.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
