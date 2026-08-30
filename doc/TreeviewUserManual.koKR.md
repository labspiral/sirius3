# TreeView Page Control User Manual

> 기준 버전: Sirius3 1.12.3 (공개 Release 기능)

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

방향키를 제외한 `CTRL` 명령과 F1/F2/F4/F5/F6/F8 같은 View 단축키는 TreeView에서도 Editor/Marker로 전달됩니다. Sirius3 1.11.14부터 동일 키를 두 경로에서 중복 전달하지 않아 `CTRL+V`가 두 번 붙여넣거나 F5 취소 뒤 확인 창이 다시 열리는 문제를 막습니다.

| 키 | 동작 |
|---|---|
| `CTRL+C/X/V` | 복사 / 잘라내기 / 붙여넣기 |
| `CTRL+Z/Y` | Undo / Redo |
| `CTRL+R/M` | 렌더링 / 가공 허용 토글 |
| `CTRL+F` | 선택 대상 화면 맞춤 |
| `F1` | 시뮬레이션 |
| `F4` | Preview |
| `F5` | 현재 Page 실제 가공 시작 |
| `F6` / `F8` | Stop / Reset |

F5는 TreeView에 포커스가 있어도 실제 Marker Start로 이어질 수 있습니다.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
