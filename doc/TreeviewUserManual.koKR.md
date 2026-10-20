# TreeView Page Control User Manual

> API 기준: Sirius3 Core/UI 1.17 (공개 기능)

[English](TreeviewUserManual.md) | [한국어](TreeviewUserManual.koKR.md) | [전체 안내서](README.koKR.md)

## 1. 계층과 가공 순서

`TreeViewPageControl`은 한 Page의 Layer, Group, BlockInsert와 Entity를 계층으로 보여 줍니다. 위·아래 이동과 Drag & Drop으로 구조를 바꾸면 Marker의 실제 순서도 바뀔 수 있습니다.

## 2. 선택과 동기화

TreeView에서 노드를 선택하면 Editor 선택과 PropertyGrid가 갱신되고, Editor에서 선택해도 해당 노드가 동기화됩니다. 노드를 두 번 클릭하면 해당 개체에 Zoom Fit합니다.

- Bold: Active Layer
- 취소선: `IsAllowMark = false`
- 회색: `IsAllowRender = false`

## 3. 구조 편집

- Layer 추가
- Mixed/Uniform Group 생성과 Ungroup
- Block/BlockInsert 변환
- Up/Down으로 실행 순서 변경
- Drag & Drop으로 Layer 이동 또는 Layer 안의 Entity 재배치

서로 다른 계층 수준을 섞거나 Layer를 Entity 아래에 넣는 잘못된 Drop은 거부됩니다. 대량 노드는 `UI.Config.MaxTreeNodeItems` 기준으로 확장 확인을 표시할 수 있습니다.

## 4. TreeView 포커스의 방향키

TreeView에 포커스가 있으면 **수정키가 함께 눌려도 Up/Down/Left/Right는 모두 노드 탐색과 접기·펼치기에 사용**됩니다. 따라서 Editor의 `CTRL+화살표` 개체 이동은 실행되지 않습니다. 개체를 키보드로 이동하려면 Editor를 먼저 클릭해 포커스를 옮기십시오.

## 5. 전달되는 단축키

TreeView에서도 방향키를 제외한 Editor·Marker 단축키를 전달합니다. 처리된 단축키는 한 번만 실행하며, 같은 키로 작업을 중복 실행하거나 다른 노드를 검색하지 않습니다.

| 키 | 동작 |
|---|---|
| `CTRL+C/X/V` | 복사 / 잘라내기 / 붙여넣기 |
| `CTRL+Z/Y` | Undo / Redo |
| `R` / `CTRL+R` | 표시 허용 전환 |
| `M` / `CTRL+M` | 마킹 허용 전환 |
| `F` / `CTRL+F` | 선택 대상 화면 맞춤, 선택이 없으면 활성 Layer 전체 맞춤 |
| `V` / `Shift+V` | 다음 / 이전 Grip |
| `G` | Grip 자석 스냅 전환 |
| `F1` | 시뮬레이션 |
| `F4` | Preview |
| `F5` | 현재 Page 실제 가공 시작 |
| `F6` / `F8` | Stop / Reset |

F5는 TreeView에 포커스가 있어도 실제 Marker Start로 이어질 수 있습니다.

문자 단축키는 WinForms·WPF의 Page·Block·Substrate·Wafer 트리에서 동일하게
동작합니다. 처리한 문자 키로 다른 노드를 검색하지 않습니다. 텍스트 입력 칸은
자체 편집 키를 유지하고 Tab은 일반 포커스 이동에 사용합니다. Grip의 V/Shift+V
탐색은 뷰포트에서 사용하십시오. M은 마킹 허용 상태를 바꾸어 Undo/Redo에 기록하며
실제 마킹을 시작하지 않습니다.

## 6. HiDPI와 프레임워크 지원

WinForms 1.15.0은 DPI 변경 후 처음 표시하는 페이지까지 현재 DPI에 맞게 TreeView 행과 Node 스타일을 측정합니다. `UI.Config.TreeviewFontSize`는 논리적인 포인트 크기이며 UI 스레드에서 바꾸면 기존 Tree도 갱신합니다. WPF는 자체 Tree Adapter와 장치 독립 레이아웃을 제공합니다. [WinForms](WinForms.koKR.md)와 [WPF](WPF.koKR.md)를 참고합니다.



뷰포트에서 Tab/Shift+Tab으로 Move·Vertex·Scale 모드를 변경합니다. 트리의 Tab은 포커스를 이동합니다.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
