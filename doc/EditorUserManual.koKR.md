# EditorControl 사용 설명서

> API 기준: Sirius3 Core/UI 1.17 (공개 기능)

[English](EditorUserManual.md) | [한국어](EditorUserManual.koKR.md) | [전체 안내서](README.koKR.md)

## 1. 생성·선택·속성 편집

WinForms와 WPF EditorControl은 연결한 문서를 표시하고 편집합니다. 활성 Page와
Layer를 선택하고 개체를 생성하거나 가져온 뒤, 화면 또는 TreeView에서 선택하여
PropertyGrid의 값을 편집합니다. Page·트리·Pen·장치 패널이 함께 필요하면 통합
편집기를 사용합니다.

점·선·호·폴리라인·스플라인·기본 도형은 대화형으로 생성할 수 있습니다. 좌측 상단의
단계별 안내를 따르며, 여러 점을 입력하는 도구는 Enter로 완료하고 Escape로
취소합니다. 텍스트·바코드·이미지·3D 도구의 세부 값은 대화상자 또는 PropertyGrid에서
설정합니다. 완료한 편집은 Undo/Redo로 복원할 수 있습니다.

## 2. 마우스 선택과 카메라

| 조작 | 동작 |
|---|---|
| 왼쪽 클릭 / 빈 공간 드래그 | 개체 선택 / 사각 영역 선택 |
| Shift+왼쪽 클릭 | 선택에 추가 |
| Ctrl+왼쪽 클릭 | 선택 상태 반전 |
| Alt+왼쪽 클릭 | 선택에서 제외 |
| Space를 누른 상태로 선택 | 그룹 안의 하위 개체 선택 |
| 휠 | 포인터 위치를 기준으로 확대·축소 |
| 가운데 버튼 드래그 | 카메라 평행 이동 |
| 오른쪽 버튼 드래그 | 원근 카메라 회전 |

클릭 시 선택 가능한 보이는 형상 중 가장 가까운 결과를 선택합니다. 회전·반사·배율이
적용된 개체도 화면 기준 허용 범위로 선택합니다. 마킹 중에는 선택과 데이터 변경이
잠기지만 확대·축소와 카메라 이동은 사용할 수 있습니다.
포커스별 선택 동작은 [TreeView](TreeviewUserManual.koKR.md)를 참고하십시오.

## 3. Grip 편집

지원하는 개체 하나를 선택하면 편집 Grip이 표시됩니다. 새 개체를 선택하면
**Move**로 시작합니다. **Tab**는 Move → Vertex → Scale, **Shift+Tab**는 역순으로
전환하며 해당 형상이 지원하지 않는 모드는 건너뜁니다.

| 모드 | 편집 대상 |
|---|---|
| Move | 중심 또는 경계 상자를 이용한 개체 전체 이동 |
| Vertex | 원본 정점·곡선 제어점·Bulge·지원하는 원래 치수 |
| Scale | 크기·회전 및 개체가 제공하는 배율 초기화 |

Grip을 끌면서 좌측 상단의 좌표·치수·각도를 확인합니다. V/Shift+V은 사용 가능한
Grip을 다음/이전 순서로 선택합니다. 정점은 청록 원, 이동은 민트 사각형, 크기는
라임색 마름모, 곡선·회전은 파란색 표시로 구분합니다. 완료한 드래그는 Undo 한 건이며,
Escape로 취소한 미리보기는 이력을 남기지 않습니다.

**G**는 원본 정점·제어점에 대한 자석 스냅을 켜거나 끕니다(기본 켜짐).
이동·위치 및 원래 치수 Grip은 보조 키 없이 연속적으로 편집합니다.
Ctrl, Ctrl+Alt, Ctrl+Alt+Shift는 설정된 격자 간격을 선택하고, X/Y/Z는 지원하는
드래그의 축을 고정합니다. 회전은 보조 키 없이 연속적으로, 보조 키와 함께 각도
격자로 조정합니다. 다른 크기·정점 Grip은 해당 격자와 종횡비 규칙을 따르므로
현재 Grip의 화면 안내를 확인하십시오.

## 4. 키보드와 포커스

| 키 | 뷰포트에서의 동작 |
|---|---|
| Ctrl+C / X / V | 복사 / 잘라내기 / 붙여넣기 |
| Ctrl+Z / Y | 실행 취소 / 다시 실행 |
| Ctrl+A | 활성 Layer 선택 |
| Ctrl+Delete | 선택 개체 삭제 |
| F 또는 Ctrl+F | 선택 개체 맞춤, 선택이 없으면 활성 Layer 전체 맞춤 |
| Ctrl+H / Ctrl+Shift+H | 선택 개체를 XY / XYZ 원점에 정렬 |
| R 또는 Ctrl+R | 표시 허용 전환 |
| M 또는 Ctrl+M | 마킹 허용 전환 |
| Ctrl+E / Q | 다음 / 이전 카메라 |
| 방향키 | 카메라 Up/Right 방향 이동 |
| Alt+위 / 아래 | 카메라 전진 / 후진 |
| Alt+왼쪽 / 오른쪽 | Z축 기준 5° 카메라 회전 |
| Ctrl+Plus / Minus | 확대 / 축소 |

Ctrl+방향키, Ctrl+Alt+방향키, Ctrl+Alt+Shift+방향키는 선택 개체를 다음 격자 경계로
이동합니다. 기본 간격은 **1 / 0.1 / 0.01 mm**입니다. 같은 Ctrl 조합과 [ / ]는
시계/반시계 회전이며 기본 간격은 **90 / 10 / 1°**입니다. 호스트는
`UI.Config.KeyboardTransitXY*`, `KeyboardRotate*` 값으로 간격을 바꿀 수 있습니다.
TreeView 방향키는 트리를 탐색하고 PropertyGrid 입력·검색 칸은 자체 편집 키를
사용합니다. F, M, R, V/Shift+V, G는 WinForms·WPF의 Page·Block·Substrate·Wafer
트리에서도 동작합니다. 트리의 Tab은 일반 포커스 이동에 사용합니다. 개체 이동이나
Grip 탐색 전에는 뷰포트에 포커스를 두십시오.

## 5. 시뮬레이션과 장치 명령

| 기본 키 | 동작 |
|---|---|
| F1 / Ctrl+F1 / Ctrl+Alt+F1 | 빠른 / 보통 / 느린 경로 시뮬레이션 |
| Escape | 시뮬레이션 중지 또는 편집 미리보기 취소 |
| F2 | 스크립트 속성 |
| F4 | Marker 미리보기 |
| F5 | 현재 Page의 실제 마킹 시작 |
| F6 / F8 | Marker 중지 / 초기화 |

키는 호스트의 `UI.Config.Keyboard*` 설정을 따릅니다. 시뮬레이션은 화면 표시용입니다.
**Marker 미리보기는 스캐너를 움직일 수 있고 Marker Start는 실제 장비를 작동할 수
있습니다.** 사용 전 등록 장치, 작업 영역과 인터록을 확인하십시오.

## 6. 클리핑·폰트·프레임워크 연동

[클리핑](ClippingUserManual.koKR.md)으로 선택 형상 또는 Layer를 영역별로 나누고
Undo/Redo로 복원할 수 있습니다. 실행 전 겹침·해치 포함·원본 제거 여부를 확인합니다.
우클릭 메뉴의 **Font Edit**는 [Font Editor](FontEditorUserManual.koKR.md)를 엽니다.
SiriusText를 먼저 선택하면 사용 중인 폰트를 엽니다. 폰트 편집은 별도의 파일과
이력을 사용합니다. [WinForms](WinForms.koKR.md), [WPF](WPF.koKR.md),
[PropertyGrid](PropertyGridUserManual.koKR.md), [엔티티](EntityUserManual.koKR.md)를 참고하십시오.




## 생성 비율과 정점 추가·삭제

Rectangle·Ellipse·Cross 등의 대화형 크기 지정 중 Shift 단독으로 1:1 미리보기를 사용합니다. Shift를 놓으면 자유 비율로 돌아가며 생성 후 고정 비율 속성을 추가하지 않습니다. Ctrl+Shift·Alt+Shift는 비율을 제한하지 않습니다. 기존 고정 비율 속성과 이동·각도 스냅 간격은 유지합니다.

Vertex 모드에서 Polyline2D·Polyline3D와 Bezier·CatmullRom·Hermite·BSpline·NURBSpline은 Insert로 포인터 아래 구간에 정점을 추가하고, Delete로 가리키거나 V로 고른 원본 정점을 삭제합니다. 스플라인은 제어 다각형의 구간에 제어점을 추가하므로 곡선 모양이 바뀔 수 있습니다. 열린 경로는 2개, 닫힌 폴리라인은 3개, NURBS/BSpline은 차수+1개를 남깁니다. 추가·삭제는 각각 한 번의 Undo/Redo이며 개체 자체를 삭제하지 않습니다. 다른 모드와 Ctrl+Delete는 기존 개체 삭제 명령입니다.

좌측 상단에는 `Tab: Move mode`, `Tab: Vertex mode`, `Tab: Scale mode`와 `V: Next grip`을 표시합니다. 보조 제어점은 원본 정점보다 작고 채움이 더 투명합니다.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
