# SiriusEditorControl User Manual

> API 기준: Sirius3 Core/UI 1.17.0 (공개 기능)

[English](SiriusEditorUserManual.md) | [한국어](SiriusEditorUserManual.koKR.md) | [전체 안내서](README.koKR.md)

## 1. 통합 편집기

`SiriusEditorControl`은 WinForms와 WPF에서 EditorControl, Page/Layer TreeView, PropertyGrid와 Scanner·Laser·Marker·I/O·PowerMeter UI를 한 화면에 제공합니다. WinForms는 `beginner`와 `editor_ui`, WPF는 `wpf_editor_ui`를 기준으로 메뉴·탭·장치 UI를 제품에 맞게 구성합니다.

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

편집기나 Document를 해제하기 전에 소유 UI 스레드에서 `await editor.DisposeDevices()`를 호출합니다. 1.15.0에서는 `Task`를 반환하며 등록된 MCP 서버와 Remote를 먼저 비동기로 해제한 뒤 장치를 정리합니다. 별도 소유 서버는 먼저 `await server.DisposeAsync()`를 호출합니다. 소유한 Control과 Document는 한 번만 해제하고 Application 수명 관리 코드에서 `Core.Cleanup()`을 호출합니다.

## 11. 프레임워크 선택과 HiDPI

패키지 컨트롤은 `SpiralLab.Sirius3.UI.WinForms` 또는 `SpiralLab.Sirius3.UI.WPF`, 공개 소스 복사본은 `Demos`를 사용합니다. WPF 컨트롤은 필요한 테마 리소스를 자체 초기화합니다. WPF Multi-Beam 메뉴는 지정된 Scanner가 `IRtcMultiBeam`일 때만 표시하며, 가상·제조사별 Laser 패널은 등록 장치나 사용자 WPF UI Factory에 따라 선택합니다.

WinForms 호스트는 `PerMonitorV2`를 선택하고 데모 Form은 `AutoScaleMode.Dpi`와 `(96, 96)` 설계 기준을 사용합니다. WPF는 DPI-aware 실행 파일 manifest와 장치 독립 XAML 레이아웃을 사용합니다. 프레임워크별 시작·등록·종료 예제는 [WinForms](WinForms.koKR.md)와 [WPF](WPF.koKR.md) 가이드를 참고합니다.

## 개체 Grip과 Font Editor

뷰포트에서 Tab/Shift+Tab로 Move·Vertex·Size을 순환하고 V/Shift+V으로 Grip을
탐색합니다. Move는 개체 전체 이동·지원 축 회전, Size는 치수·모델 배율을 편집합니다. G는 자석 스냅, F/Ctrl+F는 화면 맞춤이며 편집은 Undo/Redo로 복원합니다.
포커스와 격자 규칙은 [편집기](EditorUserManual.koKR.md)를 참고하십시오. 우클릭 메뉴의
**Font Edit**는 별도의 폰트 파일 편집을 엽니다. [Font Editor](FontEditorUserManual.koKR.md)를 참고하십시오.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
