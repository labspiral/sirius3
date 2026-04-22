# IDocument & Recipe Management User Manual


## 1. 개요 (Overview)

IDocument는 Sirius3 프레임워크에서 레이저 가공 레시피를 관리하는 최상위 컨테이너입니다. 
도면 내의 모든 기하학적 엔티티(Entity), 가공 파라미터(Pen), 논리적 페이지(Page), 그리고 블록(Block) 리소스를 중앙에서 제어하며, 
파일 입출력 및 사용자 인터랙션(선택, 변환)을 담당합니다.

## 2. 데이터 구조 (Data Structure)

문서의 데이터는 `IDocumentData` 객체 내에 계층적으로 저장됩니다.
- Pages: 최대 4개의 논리적 가공 페이지(Page1~4)와 특수 뷰(Block, Wafer, Substrate)를 지원합니다.
- Layers: 각 페이지는 여러 개의 레이어를 가지며, 가공 순서는 트리 구조의 아래에서 위 방향으로 진행됩니다.
- Pens: 가공 조건을 정의하는 엔티티 펜과 레이어 펜 컬렉션을 보유합니다.
- Blocks: 재사용 가능한 마스터 도형 정의(Block)를 관리합니다.

## 3. 라이프사이클 관리 (Lifecycle - 중요)

- 인스턴스 생성: `IDocument` 인스턴스는 필요에 따라 여러 개를 동시에 운용할 수 있습니다.
- 자원 해제: ★중요★ 라이브러리는 문서 인스턴스를 자동으로 Dispose하지 않습니다. 외부 사용자는 문서가 더 이상 필요하지 않을 때 반드시 `document.Dispose()`를 호출하여 메모리와 GPU 리소스를 명시적으로 해제해야 합니다.

## 4. 파일 조작 및 가져오기 (File & Import)

- ActNew: 새로운 빈 문서를 초기화합니다. (초기화 후 레이어 개체가 자동 추가됩니다.)
- ActOpen / ActSave: 전용 형식(*.sirius3)으로 레시피를 저장하거나 불러옵니다.
- ActImport: 다양한 외부 CAD/이미지 파일을 현재 레이어로 가져옵니다.
  - 벡터: *.dxf, *.dwg, *.plt (dwg 및 dxf 포맷은 ODA Converter 설치시 지원됨)
  - 이미지: *.png, *.jpg, *.bmp, *.tif (래스터 마킹용)
  - 3D: *.stl, *.obj, *.ply (3D 메시 가공용)
  - PCB: *.gbr, *.gtl 등 (Gerber/Drill 가공용)

## 5. 선택 및 히트 테스트 (Selection & Hit-test)

- Selected: 선택된 최상위 개체들의 배열입니다.
- SubSelected: 그룹이나 블록 내부에 포함된 개별 자식 개체가 선택되었을 때의 배열입니다.
- IsAllowHitTest: 가공 중이나 특정 상황에서 마우스에 의한 개체 선택을 차단하려면 이 속성을 사용하십시오.

## 6. 주요 편집 액션 (Common Actions)

문서의 모든 변경은 `Act`로 시작하는 메서드를 통해 수행하는 것이 권장됩니다.
- 그룹화: `ActMixedGroup` (서로 다른 타입 혼합) 또는 `ActUniformGroup` (동일 타입 대량 고속 렌더링용)을 사용합니다.
- 정렬 및 변환: `ActAlignTo`, `ActTranslate`, `ActRotate`, `ActScale` 등을 통해 정밀한 위치 제어가 가능합니다.
- 순서 변경: `ActMoveUp`, `ActMoveDown` 등을 통해 가공 우선순위를 조정합니다.
- 반전 및 슬라이스: `ActReverse` (경로 방향 반전), `ActSlice` (3D 메시를 특정 높이에서 절단) 기능을 제공합니다.

## 7. 재생성 및 동기화 (Regeneration)

- ActRegen: ★필수★ 엔티티의 속성(예: 원의 반지름, 텍스트 내용 등)을 변경한 후에는 반드시 이 메서드를 호출해야 합니다. 
  그래야만 변경된 데이터가 물리적인 벡터 궤적 및 GPU 버퍼로 업데이트되어 화면에 올바르게 표시됩니다.

## 8. 시뮬레이션 (Simulation)

- ActSimulateStart: 실제 하드웨어 출력 없이 소프트웨어 상에서 레이저 헤드의 이동 경로와 속도를 시각화합니다.
- 속도 조절: `Slow`, `Normal`, `Fast` 모드를 지원합니다.

## 9. 이벤트 핸들링 (Event Handling)

문서의 상태 변화를 실시간으로 감지하려면 다음 이벤트를 구독하십시오.
- OnNew: 새(New) 문서를 생성할때 발생.
- OnBeforeOpen, OnAfterOpen: 문서를 개방(Open)할때 발생.
- OnBeforeSave, OnAfterSave: 문서를 저장(Open)할때 발생.
- OnSelected: 선택 영역 변경 시 발생 (속성창 갱신용).
- OnChildChanged: 개체가 추가, 삭제, 이동될 때 발생 (트리뷰 갱신용).
- OnPageChanged: 문서내의 작업중인 대상 페이지(IPage) 가 변경될때 발생.
- OnPropertyChanged: 문서나 개체의 속성 값이 변경될 때 발생.
- OnSimulationStarted, OnSimulationEnded: 시뮬레이션 가공이 시작, 중지될때 발생.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
