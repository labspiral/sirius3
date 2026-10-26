# IDocument & Recipe Management User Manual

> API 기준: Sirius3 Core/UI 1.17.0 (공개 기능)

[English](DocumentUserManual.md) | [한국어](DocumentUserManual.koKR.md) | [전체 안내서](README.koKR.md)

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

문서는 `DocumentPages.Page1`부터 `Page4`까지 4개의 페이지를 제공합니다.
각 페이지는 자신의 레이어를 가지며 일반 편집의 대상은 `ActivePage.ActiveLayer`입니다.
통합 편집기는 페이지별 `TreeViewPageControl` 탭 4개를 제공합니다.

```csharp
document.Page = DocumentPages.Page2;
// ActivePage도 document.DocumentData.Pages[1]로 바뀝니다.
```

`Page`를 지정하면 해당 `ActivePage`가 자동으로 선택됩니다. 반대로 같은 문서의
기존 페이지를 `ActivePage`에 지정하면 `Page`도 바뀝니다. 두 속성을 함께 대입할
필요 없이 하나만 지정합니다.

`MarkerRtc.MarkProcedure`는 **선택한 페이지 안에서** 레이어와 Offset의 반복 순서를
바꿉니다. LayerFirst는 한 Offset의 모든 레이어를 마친 뒤 다음 Offset으로 이동하고,
OffsetFirst는 한 레이어를 모든 Offset에서 처리한 뒤 다음 레이어로 이동합니다.
구체적인 순서는 [Offset 가공 순서](OffsetUserManual.koKR.md)를 참고합니다.

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
- `ActImport`: SVG, DXF/DWG, HPGL/PLT, Gerber/Excellon, G-code/NGC, 이미지와 STL/OBJ/PLY/STP·STEP/3MF 메쉬 가져오기

`UI.Config.ImportMergeDistance`는 벡터 경로의 가까운 끝점을 연결하는 공통 허용 거리입니다. 너무 크면 관련 없는 경로가 합쳐질 수 있으므로 도면 단위와 실제 간격을 기준으로 정하십시오.

### SVG 및 3MF 가져오기

`ActImport`는 `.svg`를 편집 가능한 `EntitySvg`/`EntityContour` 벡터 계층으로, `.3mf`를 `EntityMesh`로 가져옵니다. SVG는 96 pixels/inch와 Y축 반전으로 mm 좌표를 생성하며 선·호·베지어/NURBS 원본을 유지합니다. 단색 채움과 선 두께·끝점·모서리는 영역으로 처리합니다. 텍스트, 이미지, 마스크, 필터, clip path, marker, 점선과 non-scaling stroke는 지원하지 않으므로 외곽선으로 변환하거나 지원 형식으로 내보냅니다.

SVG 선 해치는 기본적으로 생성하지 않습니다. 새 가져오기 전에 `UI.Config.IsSvgImportHatchEnabled = true`를 설정하면 `UI.Config.SvgImportHatchInterval`(기본 0.1 mm) 간격의 HatchLine을 추가합니다. 뒤에 그린 불투명 흰색은 기본적으로 앞의 형상을 잘라내며, `EntitySvg.WhiteIsKnockout`으로 이 정책을 바꿀 수 있습니다.

3MF는 빌드된 삼각형과 컴포넌트 상대 배치를 mm로 변환하고 RGB 색상, 기본 재질과 diffuse 텍스처를 유지합니다. 복합/다중 속성 재질과 비메쉬 형상은 지원하지 않습니다. 빌드 인스턴스 확장 후 최대 2,000,000개의 삼각형과 내장 이미지당 64 MiB의 인코딩 데이터 제한을 적용합니다. 가져온 메쉬의 재질은 표시용이며 실제 가공 경로는 Slice 등으로 준비합니다.

```csharp
if (!document.ActImport("drawing.svg", out var imported))
    throw new InvalidOperationException("SVG import failed; check the Sirius3 log.");
```

성공한 결과는 문서가 소유합니다. 반환값과 로그를 확인하고 실제 크기, 채움·구멍과 텍스처를 검토한 뒤 가공 데이터를 사용합니다. 설정은 [UI Config](Sirius3UIConfigUserManual.koKR.md)를 참고합니다.

## 9. 선택과 편집 Action

문서 변경은 가능하면 `Act*` 메서드로 수행하십시오. 이 경로는 선택, Undo/Redo, TreeView, PropertyGrid와 다시 그리기를 함께 갱신합니다.

- 선택: `Selected`, `SubSelected`, `ActSelectAll`
- 편집: `ActAdd`, `ActRemove`, `ActCopy`, `ActPaste`
- 변환: `ActTranslate`, `ActRotate`, `ActScale`, `ActAlignTo`
- 구조: `ActMixedGroup`, `ActUniformGroup`, `ActUngroup`
- 순서: `ActMoveUp`, `ActMoveDown`
- 형상: `ActReverse`, `ActSlice`, `ActRegen`

Property setter가 올바른 ModifyFlag를 설정한 경우 PropertyGrid 편집은 필요한 형상을 자동 재생성합니다. 직접 필드나 하위 배열을 바꿨다면 계약에 맞는 Setter와 `ActRegen`을 사용하십시오.

`ActClip`은 문서 소유 `IClippable` 원본을 월드 좌표의 Frustum 영역으로 나누고 결과 추가와 선택적 원본 삭제를 한 번의 Undo/Redo로 처리합니다. 레이어를 원본으로 사용할 때는 해당 레이어 하나만 전달합니다. [클리핑 매뉴얼](ClippingUserManual.koKR.md)과 [editor_clip 데모](../demos/editor_clip/Form1.cs)를 참고합니다.

## 10. 좌표와 ITransformable

Sirius3 1.12.1부터 `ITransformable`은 경로 시작·끝점을 세 좌표 단계로 제공합니다.

- `OriginalIn/Out`: 원본 좌표
- `ModelIn/Out`: 자신의 ModelMatrix 적용 좌표
- `RealIn/Out`: 부모까지 누적한 World 좌표

MatrixStack과 Marker Offset은 가공 단계에서 추가로 적용됩니다. 화면 좌표와 RTC 좌표가 다르면 어느 단계의 값을 비교하는지 먼저 확인하십시오.

## 11. 이벤트와 종료

`OnNew`, `OnBeforeOpen`, `OnAfterOpen`, `OnBeforeSave`, `OnAfterSave`, `OnSelected`, `OnChildChanged`, `OnPageChanged`, `OnPropertyChanged`, `OnSimulationStarted`, `OnSimulationEnded`로 상태 변화를 구독할 수 있습니다.

Document를 직접 생성한 코드는 더 이상 사용하지 않을 때 `Dispose()`를 호출해야 합니다. 연결한 View와 Device의 종료 책임은 각각을 만든 코드가 명확히 관리하십시오.

## 12. 1.15.0의 UI 호스트 종료

편집기의 `DisposeDevices()`는 이제 `Task`를 반환합니다. UI Dispatcher가 유효한 동안 등록된 Remote/MCP 작업을 정리하도록 편집기나 Document를 해제하기 전에 완료를 기다립니다. 별도 소유 서버는 먼저 `DisposeAsync()`를 호출하고 기다립니다. 공유 Document는 다른 Viewer에서 연결을 해제한 후 한 번만 폐기합니다. 이 과정이 끝난 뒤 Application에서 `Core.Cleanup()`을 호출합니다. 종료 이벤트 패턴은 [WinForms](WinForms.koKR.md)와 [WPF](WPF.koKR.md)를 참고합니다.

## PDF 페이지 가져오기

PDF는 편집 가능한 벡터 문자·경로 대신 페이지 래스터 이미지(`EntityImagePDF`)로
가져오며 첫 페이지로 시작합니다. 이미지를 선택한 뒤 PropertyGrid에서 `PageNumber`
(1부터 시작), `RenderDpi`(36~600, 기본 150)를 설정합니다. 페이지·해상도를 바꾸려면
원본 PDF가 필요합니다. 잘못된 페이지나 읽기 실패 시 이전 페이지 이미지를 유지합니다.
Width/Height는 실제 mm 치수이며 DPI를 높이면 세부 표현과 메모리 사용량이 증가합니다.

```csharp
var image = EntityFactory.CreatePDFImage(@"C:\Drawings\label.pdf", 1, 20);
if (!document.ActAdd(image))
    image.Dispose();
```

래스터 처리는 [이미지 개체](EntityUserManual.koKR.md), 영역별 출력은
[클리핑](ClippingUserManual.koKR.md)을 참고하십시오.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
