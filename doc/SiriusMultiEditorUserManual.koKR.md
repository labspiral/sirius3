# SiriusMultiEditorControl User Manual

> API 기준: Sirius3 Core/UI 1.17.0 (공개 기능)

[English](SiriusMultiEditorUserManual.md) | [한국어](SiriusMultiEditorUserManual.koKR.md) | [전체 안내서](README.koKR.md)

## 1. 용도

`SiriusMultiEditorControl`은 하나의 `IDocument`와 편집 화면을 공유하면서 1~4개의 Scanner/Laser/Marker 장치 세트를 인덱스로 전환하는 WinForms/WPF 컨트롤입니다. 여러 개의 레시피를 동시에 편집하는 컨트롤이 아니라, 같은 레시피를 여러 장치 구성에 연결해 상태를 확인하고 실행할 때 사용합니다.

WinForms는 `editor_multiple2`와 `editor_ui`, WPF는 `wpf_editor_multiple2`와 `wpf_editor_ui`에서 초기화, 장치 등록과 소스 커스터마이징을 확인할 수 있습니다.

## 2. 초기화 순서

1. `Core.Initialize()`를 호출합니다.
2. `config*.ini`를 읽어 각 인덱스의 Scanner, Laser, PowerMeter, DIO, Marker, Remote를 생성합니다.
3. `MaxDeviceCounts`를 1~4 범위로 설정합니다.
4. 각 세트를 `RegisterDevices(index, ...)`로 등록합니다.
5. `SwitchDevices(index)`로 화면에 표시할 활성 세트를 선택합니다.
6. 편집기나 Document를 해제하기 전에 소유 UI 스레드에서 `DisposeDevices()`를 호출하고 완료를 기다립니다. Control과 Document를 정리한 뒤 Application에서 `Core.Cleanup()`을 호출합니다.

## 3. RegisterDevices

`RegisterDevices`는 Scanner, Laser, PowerMeter, Extension/LASER-port DInput·DOutput, Marker와 선택형 Remote를 같은 인덱스에 묶습니다. 내부적으로 해당 Marker의 `Ready`도 호출하므로 등록 후 다시 중복 호출하지 말고 `marker.IsReady`와 로그를 확인하십시오.

배열 속성은 같은 인덱스를 한 세트로 사용합니다.

- `Scanners[index]`
- `Lasers[index]`
- `PowerMeters[index]`
- `Markers[index]`
- DInput/DOutput 및 Remote 배열

## 4. SwitchDevices

`SwitchDevices(index)`는 활성 Scanner/Laser/Marker를 하위 UI 컨트롤과 PropertyGrid에 다시 연결합니다. Document와 선택 개체는 유지되지만, 활성 장치가 지원하는 PropertyGrid 항목과 상태 표시는 달라질 수 있습니다.

전환 성공 여부와 현재 인덱스 변경 이벤트를 확인하십시오. 실행 중인 Marker가 있으면 Document 편집이나 장치 전환이 제한될 수 있습니다.

## 5. 안전과 종료

- 여러 장치가 같은 Document를 공유하므로 한 Marker가 Busy일 때 공정 데이터를 바꾸지 마십시오.
- 각 세트의 KFactor, 보정 파일, Laser Mode, PowerMax와 PowerMap이 올바른지 인덱스별로 확인하십시오.
- 1.15.0의 `DisposeDevices()`는 `Task`를 반환합니다. 등록된 MCP 서버, Remote와 장치 세트 해제가 끝날 때까지 기다린 뒤 UI Control이나 Document를 해제합니다. 일반 Control `Dispose()`는 장치 해제를 대신하지 않습니다.
- 실제 Marker Start 단축키는 선택된 활성 장치를 동작시킬 수 있습니다.

## 6. Multi-Beam과의 차이

`SiriusMultiEditorControl`은 여러 독립 장치 세트를 UI에서 전환하는 컨트롤입니다. 하나의 레이저 소스를 두 RTC가 AOM과 Token으로 공유하는 Multi-Beam은 `IRtcMultiBeam`과 `RtcMultiBeamHelper`가 담당하는 별도 하드웨어 구조입니다.

## 7. WinForms와 WPF 호스트

두 프레임워크는 인덱스별 장치 등록과 비동기 해제 계약을 공유하며 컨트롤 형식과 레이아웃 방식은 다릅니다. WinForms 데모 Form은 96 DPI의 `AutoScaleMode.Dpi`, WPF Window는 장치 독립 레이아웃과 Per-Monitor V2 manifest를 사용합니다. [WinForms](WinForms.koKR.md)와 [WPF](WPF.koKR.md)를 참고합니다. 다중 편집기의 `MCPServer`에 서버를 지정하면 `DisposeDevices()`가 장치 세트보다 먼저 서버를 해제합니다.

## 개체 Grip과 Font Editor

뷰포트에서 Tab/Shift+Tab로 Move·Vertex·Size을 순환하고 V/Shift+V으로 Grip을
탐색합니다. Move는 개체 전체 이동·지원 축 회전, Size는 치수·모델 배율을 편집합니다. G는 자석 스냅, F/Ctrl+F는 화면 맞춤이며 편집은 Undo/Redo로 복원합니다.
포커스와 격자 규칙은 [편집기](EditorUserManual.koKR.md)를 참고하십시오. 우클릭 메뉴의
**Font Edit**는 별도의 폰트 파일 편집을 엽니다. [Font Editor](FontEditorUserManual.koKR.md)를 참고하십시오.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
