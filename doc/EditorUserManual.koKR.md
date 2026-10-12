# EditorControl User Manual

> 기준 버전: Sirius3 1.16.0 (공개 Release 기능)

## 1. 역할

`EditorControl`은 `IDocument`의 Entity를 OpenGL 편집기에 표시하고 선택·이동·회전·배율 변경·생성·삭제하는 WinForms/WPF 컨트롤입니다. Document를 바꾸면 기존 이벤트를 해제하고 새 Document의 선택과 변경 이벤트를 연결합니다.

## 2. 마우스 조작

- 왼쪽 클릭: 개체 선택
- Shift + 왼쪽 클릭: 선택 추가
- Ctrl + 왼쪽 클릭: 선택 토글
- Alt + 왼쪽 클릭: 선택 제외
- 가운데 드래그: 편집 화면 이동
- 휠: 확대·축소
- 오른쪽 드래그: Perspective Camera 회전
- Space를 누른 채 선택: 겹친 개체의 하위 후보 선택

가공 중에는 선택과 데이터 변경이 잠기지만 확대·축소와 화면 이동은 계속 사용할 수 있습니다.

## 3. 개체 생성

생성 메뉴는 Point, Line, Arc, Polyline, Text, Barcode, Image, Group과 3D 개체를 제공합니다. Sirius3 1.13.0에는 Plane, Pyramid, Torus, NURBS Surface가 추가되었습니다. 생성된 개체는 현재 Page의 Active Layer에 들어가며 즉시 PropertyGrid에서 세부 속성을 편집할 수 있습니다.

## 4. 편집 단축키

| 키 | 동작 |
|---|---|
| `CTRL+C` / `CTRL+X` / `CTRL+V` | 복사 / 잘라내기 / 마우스 위치에 붙여넣기 |
| `CTRL+Z` / `CTRL+Y` | Undo / Redo |
| `CTRL+A` | Active Layer의 개체 선택 |
| `CTRL+Delete` | 선택 개체 삭제 |
| `CTRL+H` / `CTRL+SHIFT+H` | XY 원점 / XYZ 원점에 정렬 |
| `CTRL+R` / `CTRL+M` | 렌더링 허용 / 가공 허용 토글 |
| `CTRL+F` | 선택 개체 또는 Active Layer 화면 맞춤 |
| `CTRL+E` / `CTRL+Q` | 다음 / 이전 Camera |

`CTRL+화살표`, `CTRL+ALT+화살표`, `CTRL+ALT+SHIFT+화살표`는 각각 `KeyboardTransitXYCtrl`, `KeyboardTransitXYCtrlAlt`, `KeyboardTransitXYCtrlAltShift` 거리만큼 이동합니다. 기본값은 1 mm, 0.1 mm, 0.01 mm입니다.

`CTRL+[`/`]`, `CTRL+ALT+[`/`]`, `CTRL+ALT+SHIFT+[`/`]`는 설정된 90°, 10°, 1° 단계로 회전합니다.

## 5. 시뮬레이션과 Marker 키

| 키 | 동작 |
|---|---|
| `F1` / `CTRL+F1` / `CTRL+ALT+F1` | Fast / Normal / Slow 시뮬레이션 |
| `ESC` | 시뮬레이션 또는 드래그 취소 |
| `F2` | Script 속성 표시 |
| `F4` | Marker Preview |
| `F5` | 현재 Page 실제 가공 시작 |
| `F6` | Marker Stop |
| `F8` | Marker Reset |

이 키는 `UI.Config.Keyboard*` 설정을 따릅니다. 특히 F5는 실제 하드웨어를 동작시킬 수 있으므로 작업 영역, 인터록, 레이저와 활성 장치를 확인한 뒤 사용하십시오.

## 6. 최신 안정성 변경

Sirius3 1.11.14부터 Editor와 TreeView가 같은 단축키를 두 번 전달하지 않도록 처리해 붙여넣기와 F5 확인 창의 중복 실행을 막습니다. PropertyGrid에 포커스가 있으면 해당 입력·검색 경로를 사용하고, Viewport의 `CTRL+F`는 화면 맞춤에 사용합니다. 포커스 위치를 구분합니다.

## 7. HiDPI와 프레임워크 통합

1.15.0에서는 WinForms 실행 파일을 Per-Monitor V2로 설정하고 데모 호스트 Form은 96 DPI 설계 기준의 DPI 자동 배율을 사용합니다. WPF는 장치 독립 레이아웃과 실행 파일 DPI manifest를 사용합니다. [WinForms](WinForms.koKR.md)와 [WPF](WPF.koKR.md)를 참고합니다. UI DLL은 호스트 프로세스의 DPI 모드를 선택하지 않습니다.

TreeView에 포커스가 있으면 방향키는 트리 탐색에 사용합니다. 개체 이동 단축키는 편집기로 포커스를 옮긴 후 사용합니다. WPF 키 입력은 Preview 이벤트 경로에서 전달하므로 호스트의 추가 Handler에서 같은 명령을 중복 실행하지 않습니다.

1.16.0에서 선택한 객체 또는 레이어를 편집기의 클리핑 명령으로 영역별로 나눌 수 있습니다. 영역 크기·중첩, 경로/폐영역 모드, 해치 포함과 원본 삭제 여부를 검토한 뒤 적용하며 Undo/Redo를 지원합니다. [클리핑](ClippingUserManual.koKR.md)을 참고합니다. WPF 가공 시작 확인창은 Sirius3의 테마와 현지화된 시작/취소 버튼을 사용합니다.

## 8. 카메라 이동 단축키

편집기 Viewport에 포커스가 있을 때 다음 키로 카메라를 조작합니다.

| 키 | 동작 |
|---|---|
| ↑ / ↓ / ← / → | Camera Up/Right 방향으로 이동합니다. |
| Alt + ↑ / ↓ | Camera Front 방향으로 전진·후진합니다. |
| Alt + ← / → | Camera 위치의 Z축을 기준으로 5°씩 회전합니다. |
| Ctrl + + / − | 확대·축소합니다. 숫자 키패드 Add/Subtract도 지원합니다. |
| Ctrl + F | 선택 개체를 맞추며 선택이 없으면 Active Layer를 맞춥니다. |
| Ctrl + E / Q | 다음·이전 Camera를 선택합니다. |

Camera 이동량은 현재 보이는 높이의 2%이며 최소 0.1입니다. 개체를 이동하는 mm 단위 설정과 별개입니다. 단독 방향키는 Camera, Ctrl+방향키는 선택 개체를 이동합니다. Ctrl+Alt와 Ctrl+Alt+Shift는 더 작은 개체 이동 설정을 선택합니다. Ctrl+Shift+방향키는 별도 이동 모드가 아닙니다.

`[`는 시계 방향, `]`는 반시계 방향 개체 회전입니다. Ctrl, Ctrl+Alt, Ctrl+Alt+Shift와 조합하며 기본값은 각각 90°, 10°, 1°입니다. KeyboardRotate 계열 설정으로 바꿀 수 있습니다.

TreeView에 포커스가 있으면 방향키는 트리를 탐색합니다. PropertyGrid의 문자 입력·검색과 호스트 Control은 자체 키를 처리합니다. Viewport 명령은 포커스를 편집기로 옮긴 뒤 사용합니다. WPF 편집 경로는 Delete도 지원하며 Ctrl+Delete는 문서화된 Viewport 삭제 단축키입니다. 트리 명령은 [TreeView](TreeviewUserManual.koKR.md)를 참고합니다.

`UI.Config.KeyboardHelpMessage`는 현재 설정의 도움말을 표시합니다. 기본 F1 / Ctrl+F1 / Ctrl+Alt+F1은 Fast / Normal / Slow 경로 시뮬레이션입니다. 시뮬레이션은 표시용이며 설정한 Marker Start 키(기본 F5)는 실제 하드웨어를 동작시킬 수 있습니다. Preview·Stop·Reset·Script 키도 Keyboard 설정을 따릅니다. 호스트 Handler에서 같은 명령을 두 번 실행하지 않습니다.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
