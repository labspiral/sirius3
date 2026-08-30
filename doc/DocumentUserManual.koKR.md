# IDocument & Recipe Management User Manual

> 기준 버전: Sirius3 1.12.3 (공개 Release 기능)

## 1. IDocument

`IDocument`는 Sirius3 레시피의 Page, Layer, EntityPen, EntityLayerPen, Block과 선택 상태를 관리합니다. 편집기와 TreeView는 같은 Document를 바라보며, Marker는 선택한 Page와 Layer 순서에 따라 이 데이터를 RTC 리스트로 변환합니다.

## 2. ViewerControl과 EditorControl에 Document 연결

하나의 `IDocument`는 응용 프로그램 코드에서 직접 처리할 수 있고, 필요할 때 `ViewerControl` 또는 `EditorControl`의 `Document` 속성에 할당해 화면에 표시할 수 있습니다.

- `ViewerControl`: Document를 표시하고 Camera, 선택, Preview와 상태 확인 중심으로 사용합니다.
- `EditorControl`: 같은 Document에 Entity 생성, 삭제, 변환, Undo/Redo 같은 편집 기능을 제공합니다.
- 두 Control에 같은 `IDocument` 인스턴스를 할당할 수도 있습니다. 한쪽에서 `Act*` 메서드로 바꾼 내용과 선택 상태는 Document Event를 통해 다른 Control에도 전달됩니다.

```csharp
using SpiralLab.Sirius3.Document;
using SpiralLab.Sirius3.UI.WinForms;

IDocument document = DocumentFactory.CreateDefault();

var viewerControl = new ViewerControl
{
    AliasName = "Production View",
    Dock = DockStyle.Fill,
    Document = document
};

var editorControl = new EditorControl
{
    Dock = DockStyle.Fill,
    Document = document
};
```

Control은 Document를 할당할 때 필요한 Event를 자동으로 연결하고, 다른 Document를 할당하거나 Control이 Dispose될 때 기존 Event 연결을 해제합니다. Control을 닫는 것만으로 할당된 `IDocument`가 Dispose되지는 않습니다. Document를 만든 코드가 모든 Control에서 분리한 뒤 직접 Dispose하십시오.

```csharp
viewerControl.Document = null;
editorControl.Document = null;
document.Dispose();
```

같은 Document를 여러 WinForms Control에서 사용할 때는 모두 같은 UI Thread에서 다루십시오. 한 Control에서 시뮬레이션이나 편집 중인 Document를 다른 Thread에서 동시에 변경하면 안 됩니다. 실제 가공 기능이 필요하면 Control의 `Marker`도 등록하고, Document를 열거나 구조를 크게 변경한 뒤 `Marker.Ready(document)`를 다시 호출합니다.

## 3. console_document 데모 흐름

공개 `demos/console_document/Program.cs`는 UI 편집기 없이 Document API를 사용하는 과정과, 같은 Document를 필요할 때 Viewer 또는 Editor에 연결하는 과정을 함께 보여줍니다.

1. `Core.Initialize()`를 호출합니다.
2. `DocumentFactory.CreateDefault()`로 기본 Document를 만들고 `OnAfterOpen`, `OnAfterSave` Event를 연결합니다.
3. `EditorHelper.CreateDevices(...)`로 RTC, Laser, DIO, PowerMeter, Marker를 만들고 `marker.Ready(document, null, rtc, laser, powerMeter)`로 등록합니다.
4. `EntityFactory`로 Data Matrix를 만들고 `EntitySiriusText`와 Hatch를 구성한 뒤 `document.ActAdd(entity)`로 추가합니다.
5. 모든 Entity를 추가한 뒤 `document.ActRegen()`을 호출합니다.
6. `FindByEntityPenColor`와 `FindByLayerPenColor`로 Pen을 찾아 출력, 속도, Raster, Sky Writing, Variable Polygon Delay 값을 설정합니다.
7. `ActOpen`, `ActSave`로 `.sirius3` 파일을 처리합니다. 파일을 연 뒤에는 `ActRegen()`과 `marker.Ready(document)`를 다시 실행합니다.
8. 사용자가 Viewer를 선택하면 `ViewerControl.Document = document`, Editor를 선택하면 `EditorControl.Document = document`로 같은 Document를 할당합니다.
9. 실제 가공 전 `marker.Reset()`, `marker.Ready(document)`, `marker.Start()` 순서로 실행합니다.
10. 종료할 때 Marker를 중지하고 장치, Document, Core 순서로 정리합니다.

다음은 데모의 핵심 부분을 줄인 예입니다.

```csharp
IDocument document = DocumentFactory.CreateDefault();

var entity = EntityFactory.CreateDataMatrix(
    "0123456789",
    EntityBarcode2DBase.Barcode2DCells.Dots,
    10,
    10);
entity.PenColor = SpiralLab.Sirius3.UI.Config.EntityPenColors[0];
document.ActAdd(entity);
document.ActRegen();

using (var form = new Form { Text = "Sirius3 Viewer" })
using (var viewer = new ViewerControl { Dock = DockStyle.Fill })
{
    viewer.Document = document;
    form.Controls.Add(viewer);
    form.ShowDialog();
    viewer.Document = null;
}

document.Dispose();
```

데모는 Viewer와 Editor Form을 차례로 열어 개념을 보여줍니다. 일반 WinForms 제품에서는 하나의 기본 `Application.Run(...)` Message Loop 안에서 Control을 배치하고 `Show` 또는 `ShowDialog`를 사용하십시오.

## 4. Page

문서는 최대 4개의 Page를 제공합니다. Page는 서로 다른 도면이나 공정을 분리하는 최상위 실행 단위입니다. Editor에서 활성 Page를 바꾸면 해당 Page의 Layer와 Entity가 편집 대상이 됩니다.

MarkerRtc의 `LayerFirst`와 `OffsetFirst` 설정은 여러 Page/Offset을 가공할 때 반복 순서를 바꿉니다. 같은 데이터라도 설정에 따라 “Layer를 모두 처리한 뒤 다음 Offset” 또는 “Offset을 모두 처리한 뒤 다음 Layer”가 될 수 있으므로 실제 생산 순서를 먼저 정하십시오.

## 5. Layer

각 Page는 여러 Layer를 가지며, TreeView에 표시된 순서가 가공 순서에 영향을 줍니다. Layer를 시작하기 전에 EntityLayerPen의 제어 조건을 적용하고, 그 안의 Entity를 순서대로 처리합니다.

- Layer의 Mark 허용 여부가 꺼져 있으면 해당 Layer를 건너뜁니다.
- Entity의 Mark 허용 여부가 꺼져 있으면 그 Entity만 건너뜁니다.
- Layer 순서를 바꾸면 레이저 조건, Sky Writing, ALC와 장치 동작 순서도 바뀔 수 있습니다.

## 6. Block

Block은 재사용 가능한 마스터 형상이고 BlockInsert는 해당 형상을 배치하는 참조입니다. 같은 로고나 패턴을 반복할 때 메모리와 편집량을 줄일 수 있습니다. Block 자체의 형상, BlockInsert의 ModelMatrix, Marker Offset이 순서대로 누적되므로 중복 이동·회전·배율 적용에 주의하십시오.

## 7. EntityPen과 EntityLayerPen

- EntityPen: 개체 가공 시 리스트 명령으로 적용되는 Power, Speed, Delay, Raster, Wobbel 등
- EntityLayerPen: 레이어 처리 전에 제어 상태로 적용되는 Sky Writing, ALC, Variable Delay 등

PropertyGrid에서 개체를 선택하면 개체 속성과 연결된 펜을 확인할 수 있습니다. 레이저를 교체하면 `PowerMax`, PowerMap과 지원 속성이 다시 연결되었는지 확인하십시오.

## 8. 파일과 가져오기

- `ActNew`: 새 문서와 기본 Layer 생성
- `ActOpen`, `ActSave`: `.sirius3` 레시피 열기/저장
- `ActImport`: DXF/DWG, HPGL/PLT, Gerber/Excellon, G-code/NGC, 이미지, STL/OBJ/PLY/STP·STEP 등 가져오기

`UI.Config.ImportMergeDistance`는 벡터 경로의 가까운 끝점을 연결하는 공통 허용 거리입니다. 너무 크면 관련 없는 경로가 합쳐질 수 있으므로 도면 단위와 실제 간격을 기준으로 정하십시오.

## 9. 선택과 편집 Action

문서 변경은 가능하면 `Act*` 메서드로 수행하십시오. 이 경로는 선택, Undo/Redo, TreeView, PropertyGrid와 다시 그리기를 함께 갱신합니다.

- 선택: `Selected`, `SubSelected`, `ActSelectAll`
- 편집: `ActAdd`, `ActRemove`, `ActCopy`, `ActPaste`
- 변환: `ActTranslate`, `ActRotate`, `ActScale`, `ActAlignTo`
- 구조: `ActMixedGroup`, `ActUniformGroup`, `ActUngroup`
- 순서: `ActMoveUp`, `ActMoveDown`
- 형상: `ActReverse`, `ActSlice`, `ActRegen`

Property setter가 올바른 ModifyFlag를 설정한 경우 PropertyGrid 편집은 필요한 형상을 자동 재생성합니다. 직접 필드나 하위 배열을 바꿨다면 계약에 맞는 Setter와 `ActRegen`을 사용하십시오.

## 10. 좌표와 ITransformable

Sirius3 1.12.1부터 `ITransformable`은 경로 시작·끝점을 세 좌표 단계로 제공합니다.

- `OriginalIn/Out`: 원본 좌표
- `ModelIn/Out`: 자신의 ModelMatrix 적용 좌표
- `RealIn/Out`: 부모까지 누적한 World 좌표

MatrixStack과 Marker Offset은 가공 단계에서 추가로 적용됩니다. 화면 좌표와 RTC 좌표가 다르면 어느 단계의 값을 비교하는지 먼저 확인하십시오.

## 11. 이벤트와 종료

`OnNew`, `OnBeforeOpen`, `OnAfterOpen`, `OnBeforeSave`, `OnAfterSave`, `OnSelected`, `OnChildChanged`, `OnPageChanged`, `OnPropertyChanged`, `OnSimulationStarted`, `OnSimulationEnded`로 상태 변화를 구독할 수 있습니다.

Document를 직접 생성한 코드는 더 이상 사용하지 않을 때 `Dispose()`를 호출해야 합니다. 연결한 View와 Device의 종료 책임은 각각을 만든 코드가 명확히 관리하십시오.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
