# TreeView Page Control & Hierarchy Management User Manual


## 1. 개요 (Overview)

TreeViewPageControl은 Sirius3 도면(`IDocument`)의 내부 구조(레이어, 그룹, 개체)를 트리 형태로 시각화하고 관리하는 WinForms 컨트롤입니다. 
수천 개의 엔티티가 포함된 대규모 레시피에서도 원활한 탐색과 편집이 가능하도록 지연 로딩(Lazy Loading) 및 다중 선택 기능을 제공합니다.

## 2. 외부 문서 동적 바인딩 (Dynamic Binding)

- 핵심 특징: `TreeViewPageControl.Document` 속성을 통해 외부에서 생성된 `IDocument` 인스턴스를 주입받습니다.
- 문서 전환: 에디터에서 다른 레시피 파일을 열거나 문서를 교체할 때, 이 속성에 새 인스턴스를 할당하면 기존 바인딩이 자동으로 해제(`Unbind`)되고 새 문서의 계층 구조로 즉시 갱신됩니다.
- 페이지 연동: `Page` 속성(Page 1~4)을 변경하여 문서 내의 특정 논리적 페이지 데이터만 독립적으로 표시할 수 있습니다.

## 3. 계층 구조 탐색 및 조작 (Hierarchy & Navigation)

- 트리 구조: [레이어(Layer)] -> [그룹(Group) / 블록 삽입] -> [기본 도형 엔티티] 순의 계층을 가집니다.
- 실시간 동기화: 
  - 화면(OpenGL View)에서 개체를 선택하면 트리에서도 해당 노드가 선택됩니다.
  - 반대로 트리에서 노드를 선택하면 화면 상의 엔티티가 선택되며 속성창(`PropertyGrid`)이 갱신됩니다.
- 줌 피트 (Zoom Fit): 트리 노드를 더블 클릭하면 해당 개체가 화면 중앙에 꽉 차도록 카메라가 자동으로 이동합니다.

## 4. 드래그 앤 드롭 (Drag & Drop) - 중요

`MultiSelectTreeviewBinder`를 통해 강력한 순서 변경 기능을 제공합니다.
- 레이어 순서 변경: 레이어 노드를 드래그하여 가공 우선순위를 변경할 수 있습니다.
- 개체 이동: 엔티티들을 선택하여 다른 레이어로 이동시키거나, 같은 레이어 내에서 가공 순서를 앞/뒤로 바꿀 수 있습니다.
- 제약 사항: 서로 다른 레벨의 혼합 드래그나, 레이어를 엔티티 하위로 넣는 등의 논리적 오류가 있는 드롭은 자동으로 차단됩니다.

## 5. 지연 로딩 및 성능 (Lazy Loading)

대량의 데이터를 효율적으로 처리하기 위해 '지연 로딩' 기법을 사용합니다.
- 동작 방식: 노드를 확장(+)하기 전까지는 실제 하위 노드를 생성하지 않습니다.
- 경고 메시지: 특정 그룹 내의 자식 개체 수가 설정값(`Config.MaxTreeNodeItems`)을 초과할 경우, 성능 저하 방지를 위해 확장 여부를 묻는 확인창이 나타납니다.

## 6. 시각적 스타일 가이드 (Visual Styles)

트리 노드의 글꼴과 색상은 개체의 상태를 나타냅니다.
- 굵게 (Bold): 현재 선택된 활성 레이어(Active Layer)를 의미합니다.
- 취소선 (Strikeout): 가공 제외(`IsAllowMark = false`) 상태인 개체입니다.
- 회색 (Gray): 화면 렌더링 제외(`IsAllowRender = false`) 상태인 개체입니다.

## 7. 주요 툴바 기능 (Toolbar Actions)

- Layer: 새 가공 레이어를 추가합니다.
- Mixed/Uniform Group: 선택된 여러 개체를 하나의 그룹으로 묶습니다. (Uniform은 동일 타입 개체 전용 최적화 그룹)
- Un-Group: 그룹화된 개체를 해제합니다.
- Block: 선택한 개체들을 마스터 블록으로 변환하여 재사용 가능하게 만듭니다.
- Up/Down: 선택된 개체들의 가공 순서를 세밀하게 한 단계씩 조정합니다.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
