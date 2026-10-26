# ViewerControl 사용 설명서

> API 기준: Sirius3 Core/UI 1.17.0 (공개 기능)

[English](ViewerUserManual.md) | [한국어](ViewerUserManual.koKR.md) | [전체 안내서](README.koKR.md)

## 1. 문서 보기

ViewerControl은 뷰포트에서 형상을 편집하지 않고 도면이나 가공 경로를 확인하는
컨트롤입니다. 호스트의 `IDocument`를 `Document`에 연결합니다. 편집기와 여러 Viewer가
문서를 공유하면 다른 화면의 변경도 반영됩니다. Viewer 자체가 장비 인터록이나
외부 문서 변경을 잠그는 기능은 아닙니다.

## 2. 카메라와 외관

휠은 포인터 기준 확대·축소, 가운데 드래그는 평행 이동, 오른쪽 드래그는 원근 카메라
회전입니다. 화면 맞춤으로 확인할 영역을 표시합니다. 여러 창에서 서로 다른 카메라로
상면·측면을 확인할 수 있습니다. 메시 색·텍스처는 화면 외관이며 실제 가공은 준비한
경로와 Pen을 따릅니다.

## 3. 연결과 종료

WinForms와 WPF 모두 Viewer를 제공합니다. [WinForms](WinForms.koKR.md),
[WPF](WPF.koKR.md)에 따라 DPI와 프로그램 수명을 구성합니다. 모든 컨트롤의 연결을
해제할 때까지 공유 문서를 유지하고, 이후 호스트가 소유한 문서를 한 번 해제합니다.
Viewer도 해당 창을 닫을 때 해제합니다.

편집은 [EditorControl](EditorUserManual.koKR.md)을 사용합니다.
[화면·카메라](ViewUserManual.koKR.md), [시뮬레이션](SimulationUserManual.koKR.md),
[Font Editor](FontEditorUserManual.koKR.md)에서 관련 작업을 확인하십시오.


---
2026 Copyright (c) SpiralLAB. All rights reserved.
