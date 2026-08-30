# SiriusMultiEditorControl User Manual

> 기준 버전: Sirius3 1.12.3 (공개 Release 기능)

## 1. 용도

`SiriusMultiEditorControl`은 하나의 `IDocument`와 편집 화면을 공유하면서 1~4개의 Scanner/Laser/Marker 장치 세트를 인덱스로 전환하는 공개 WinForms 컨트롤입니다. 여러 개의 레시피를 동시에 편집하는 컨트롤이 아니라, 같은 레시피를 여러 장치 구성에 연결해 상태를 확인하고 실행할 때 사용합니다.

공개 데모 `editor_multiple2`와 `editor_ui`에서 초기화, 장치 등록과 UI 소스 복사 기반 커스터마이징을 확인할 수 있습니다.

## 2. 초기화 순서

1. `Core.Initialize()`를 호출합니다.
2. `config*.ini`를 읽어 각 인덱스의 Scanner, Laser, PowerMeter, DIO, Marker, Remote를 생성합니다.
3. `MaxDeviceCounts`를 1~4 범위로 설정합니다.
4. 각 세트를 `RegisterDevices(index, ...)`로 등록합니다.
5. `SwitchDevices(index)`로 화면에 표시할 활성 세트를 선택합니다.
6. 종료 시 Marker와 Device를 중지·해제하고 `Core.Cleanup()`을 호출합니다.

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
- `DisposeDevices()`는 등록한 장치를 정리하는 명시적 동작입니다. UI Control의 일반 Dispose와 혼동하지 마십시오.
- 실제 Marker Start 단축키는 선택된 활성 장치를 동작시킬 수 있습니다.

## 6. Multi-Beam과의 차이

`SiriusMultiEditorControl`은 여러 독립 장치 세트를 UI에서 전환하는 컨트롤입니다. 하나의 레이저 소스를 두 RTC가 AOM과 Token으로 공유하는 Multi-Beam은 `IRtcMultiBeam`과 `RtcMultiBeamHelper`가 담당하는 별도 하드웨어 구조입니다.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
