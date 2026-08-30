# EditorControl User Manual

> 기준 버전: Sirius3 1.12.3 (공개 Release 기능)

## 1. 역할

`EditorControl`은 `IDocument`의 Entity를 OpenGL 편집기에 표시하고 선택·이동·회전·배율 변경·생성·삭제하는 공개 WinForms 컨트롤입니다. Document를 바꾸면 기존 이벤트를 해제하고 새 Document의 선택과 변경 이벤트를 연결합니다.

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

생성 메뉴는 Point, Line, Arc, Polyline, Text, Barcode, Image, Group과 3D 개체를 제공합니다. Sirius3 1.12.3에는 Plane, Pyramid, Torus, NURBS Surface가 추가되었습니다. 생성된 개체는 현재 Page의 Active Layer에 들어가며 즉시 PropertyGrid에서 세부 속성을 편집할 수 있습니다.

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

Sirius3 1.11.14부터 Editor와 TreeView가 같은 단축키를 두 번 전달하지 않도록 처리해 붙여넣기와 F5 확인 창의 중복 실행을 막습니다. PropertyGrid 검색은 `CTRL+F`가 아니라 PropertyGrid 검색창이 포커스를 받을 때 동작하므로, Editor의 `CTRL+F` 화면 맞춤과 포커스 위치를 구분하십시오.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
