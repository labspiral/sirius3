# SpiralLab.Sirius3.UI.Config 설정 가이드

> 기준 버전: Sirius3 1.12.3 (공개 Release 기능)

## 1. 역할과 설정 시점

`SpiralLab.Sirius3.UI.Config`는 Document, Editor, TreeView, PropertyGrid, Import, Text, Marker와 WinForms 확장 지점에 적용되는 정적 설정입니다. UI Control과 Document를 만들기 전에 값을 지정하면 새로 생성되는 개체가 같은 정책을 사용합니다.

Core 설정과 이름이 같으므로 별칭을 사용하면 혼동을 줄일 수 있습니다.

```csharp
using CoreConfig = SpiralLab.Sirius3.Config;
using UIConfig = SpiralLab.Sirius3.UI.Config;

CoreConfig.DecimalPrecision = 3;
UIConfig.UnReDoSize = 50;
UIConfig.ImportMergeDistance = 0.001;
UIConfig.KeyboardMarkerStart = SpiralLab.Sirius3.View.GLKeys.F5;

if (!SpiralLab.Sirius3.Core.Initialize())
    throw new InvalidOperationException("Sirius3 초기화에 실패했습니다.");
```

Config 값은 프로세스 전체에 적용됩니다. 여러 편집기를 사용하는 프로그램에서 편집기마다 다른 값을 기대하면 안 됩니다. 이미 생성되거나 캐시된 글꼴, Pen, 형상 및 UI는 설정을 바꿔도 자동으로 다시 생성되지 않을 수 있습니다.

## 2. Document와 Undo/Redo

| 설정 | 기본값 | 설명과 주의점 |
|---|---:|---|
| `MaxPages` | `4` | 한 Document가 가질 수 있는 최대 Page 수입니다. 읽기 전용입니다. |
| `IsUnReDoEnable` | `true` | Undo/Redo 기록 사용 여부입니다. 속성명은 API에 정의된 철자를 그대로 사용합니다. |
| `UnReDoSize` | `30` | 보관할 Undo/Redo 이력 수입니다. `0`은 개수 제한 없음이므로 메모리 사용량을 함께 확인합니다. |
| `IsFileSaveWithImage` | `false` | `IDocument.ActSave` 시 같은 위치에 Snapshot 이미지도 저장할지 결정합니다. View와 저장 폴더 쓰기 권한이 필요합니다. |

Undo/Redo는 `IDocument.Act*` 작업을 기준으로 동작합니다. Page, Layer 또는 Entity Collection을 직접 변경하면 이력과 TreeView가 같은 상태를 보장하지 못할 수 있으므로 공개 Document 작업 메서드를 사용하십시오.

## 3. 파일 형식과 JSON 변환기

| 항목 | 기본값 또는 동작 | 설명 |
|---|---|---|
| `AssemblyName` | `SpiralLab.Sirius3.UI.dll` | UI 어셈블리 파일명 상수 |
| `AssemblyVersion` | 실행 중인 어셈블리 버전 | `Major.Minor.Build` 형식의 읽기 전용 값 |
| `FileOpenFilters` | Sirius3, DXF, DWG | 열기 대화상자 Filter 상수 |
| `FileImportFilters` | Sirius3, STL, OBJ, PLY, DXF, DWG, 이미지, HPGL/PLT, Gerber, G-code | 가져오기 대화상자 Filter 상수 |
| `FileSaveFilters` | `.sirius3` | 저장 대화상자 Filter 상수 |
| `FileImportImageFilters` | JPG, BMP, PNG, GIF, TIFF | 이미지 가져오기 Filter 상수 |
| `FileMeasurementFilter` | `.txt` | Measurement 파일 Filter 상수 |
| `IsCompressedFileFormat` | Release `true`, Debug `false` | 새 `.sirius3` 파일을 압축 형식으로 저장할지 결정합니다. 기존 파일을 여는 기능과는 구분하십시오. |
| `JsonExternalConvertes` | 빈 `List<JsonConverter>` | 사용자 정의 형식을 역직렬화할 때 사용할 Newtonsoft.Json Converter 목록입니다. |

사용자 정의 Converter는 Document를 열기 전에 등록합니다.

```csharp
using UIConfig = SpiralLab.Sirius3.UI.Config;

UIConfig.JsonExternalConvertes.Add(new MyEntityJsonConverter());
// 이후 DocumentSerializer.Open 또는 IDocument.ActOpen 실행
```

같은 형식을 처리하는 Converter를 중복 등록하지 마십시오. Converter의 `CanConvert` 범위를 지나치게 넓게 구현하면 Sirius3 기본 형식의 역직렬화에 영향을 줄 수 있습니다.

## 4. 경로

| 설정 | 기본 하위 경로 | 설명 |
|---|---|---|
| `SiriusFontPath` | `siriusfonts` | `.cxf`, `.lff`, `.fnt`, `.dot` Sirius Font 파일 위치 |

기본 경로는 응용 프로그램의 Base Directory를 기준으로 합니다. 배포 파일을 다른 위치에 둘 때는 Font를 처음 불러오기 전에 절대 경로를 지정하고 읽기 권한을 확인하십시오.

## 5. 병렬 처리

| 설정 | 기본값 | 설명과 제한 |
|---|---:|---|
| `MaxDegreeOfParallelism` | 논리 프로세서 수의 50%, 최소 1 | AABB Tree, Gerber Tessellation, Hatch, 글리프 Parsing, Contour 처리, Point 정렬 등의 최대 병렬 실행 수입니다. 1보다 작은 값을 지정하면 `ArgumentOutOfRangeException`이 발생합니다. |

값을 크게 한다고 항상 빨라지지는 않습니다. UI 응답성, 메모리 대역폭, 동시에 실행되는 다른 작업을 고려해 대표 문서로 측정하십시오.

## 6. TreeView

| 설정 | 기본값 | 설명 |
|---|---:|---|
| `MaxTreeNodeItems` | `10,000` | 한 TreeView Node 아래에서 처리할 최대 항목 기준 |
| `TreeviewFontSize` | `8.25` pt | TreeView Node 글꼴 크기 |
| `TreeviewNodeDefaultFont` | Segoe UI Regular | 일반 Node 글꼴, 외부에서는 읽기 전용 |
| `TreeviewNodeBoldFont` | Segoe UI Bold | 강조 Node 글꼴, 외부에서는 읽기 전용 |
| `TreeviewNodeStrikeOutFont` | Segoe UI Strikeout | 비활성 상태 등에 쓰는 취소선 글꼴, 외부에서는 읽기 전용 |
| `TreeviewNodeBoldAndStrikeOutFont` | Segoe UI Bold + Strikeout | 강조와 취소선을 함께 쓰는 글꼴, 외부에서는 읽기 전용 |

`TreeviewFontSize`를 실행 중에 바꿔도 이미 만들어진 Font 객체는 자동으로 다시 생성되지 않습니다. 시작 전에 지정하거나, 변경 후 TreeView와 관련 자원을 명시적으로 다시 구성하십시오.

## 7. Editor

| 설정 | 기본값 | 설명 |
|---|---:|---|
| `MaxSubSelectionItems` | `50` | 선택된 Group 내부 Entity를 메뉴로 나열할 최대 개수 |
| `IsHideEditorToolBars` | `false` | 공개 EditorControl의 도구 모음 표시 정책 |

대량 Group에서 `MaxSubSelectionItems`를 크게 하면 선택 메뉴를 갱신하는 시간과 항목 수가 증가합니다.

## 8. EntityPen과 EntityLayerPen

| 설정 또는 이벤트 | 기본값 | 설명 |
|---|---|---|
| `EntityPenColors` | White, Yellow, Orange, Red, Cyan, Lime, Magenta, Brown, Purple, Blue | Entity와 `EntityPen`을 연결하는 기본 10색 배열 |
| `LayerPenColors` | `EntityPenColors`와 같음 | `EntityLayerPen`에 사용하는 색상 배열 |
| `IsConvertToControllerResolution` | `false` | Pen의 장치 종속 값을 연결된 Controller 해상도에 맞춰 다룰지 결정합니다. KFactor 좌표 변환과는 별개입니다. |
| `OnCreateEntityPen` | 연결된 Handler 없음 | 새 `EntityPen`의 Power, Frequency, PulseWidth, Delay, Speed, Raster, SCANAhead, Wobbel 기본값을 사용자 코드로 만듭니다. |
| `OnCreateLayerPen` | 연결된 Handler 없음 | 새 `EntityLayerPen`의 ALC, Sky Writing, Variable Polygon/Jump Delay, syncAXIS 기본값을 사용자 코드로 만듭니다. |

Pen 생성 이벤트는 Document나 Editor가 기본 Pen을 만들기 전에 등록하십시오. Handler는 완전히 초기화된 Pen 하나를 반환해야 하며, Entity의 `PenColor`가 `EntityPenColors`의 색과 일치해야 같은 Pen을 찾을 수 있습니다.

```csharp
using System.Drawing;
using SpiralLab.Sirius3.Document;
using SpiralLab.Sirius3.Entity;
using UIConfig = SpiralLab.Sirius3.UI.Config;

UIConfig.OnCreateEntityPen += CreateEntityPen;

static EntityPen CreateEntityPen(IDocument document, Color color)
{
    return new EntityPen
    {
        Name = color.ToKnownColor().ToString(),
        PenColor = color,
        Power = 1,
        Frequency = 50_000,
        PulseWidth = 2,
        JumpSpeed = 500,
        MarkSpeed = 500,
        ScannerJumpDelay = 250,
        ScannerMarkDelay = 150,
        ScannerPolygonDelay = 100
    };
}
```

공개 데모의 `editor_pen`과 `editor_pen_multiple`에서 이 이벤트를 사용해 Pen 기본값을 만드는 흐름을 확인할 수 있습니다.

## 9. View와 Simulation

| 설정 | 기본값 | 범위 또는 동작 |
|---|---:|---|
| `SelectedLineWidth` | `2` | 하위 선택 Entity의 선 두께 및 Point 크기 |
| `DragMousePixel` | `5` px | Mouse Drag로 판단할 이동 임계값 |
| `RayHitTestPixelSize` | `4` px | Ray Hit Test의 화면상 허용 크기 |
| `FrustumHitTestPixelSize` | `1` px | Frustum Hit Test의 화면상 허용 크기 |
| `SimulationMarkerPixelSize` | `15` px | 시뮬레이션 표적 지름, 최소 3 px로 제한 |
| `SimulationBeamPixelWidth` | `20` px | 시뮬레이션 Beam 시작부 지름, 최소 3 px로 제한 |
| `SimulationDebrisEnabled` | `true` | 시뮬레이션 표적 주변 Debris 효과 사용 여부 |
| `SimulationDebrisMaxParticles` | `16` | 동시 Particle 수, 1~128로 제한 |
| `SimulationDebrisSpreadPixelRadius` | `10` px | Debris 확산 반경, 최소 1 px |
| `SimulationDebrisLifetimeMilliseconds` | `1,500` ms | Debris 평균 유지 시간, 100~10,000 ms로 제한 |
| `CameraZoomFitSteps` | `30` | Zoom Fit Camera 전환 단계 수 |

표적, Beam, Debris 크기는 화면 픽셀을 기준으로 하므로 Camera Zoom이 바뀌어도 보이는 크기가 대체로 유지됩니다. 이 효과는 시각적 시뮬레이션이며 실제 Laser Spot 크기나 에너지를 나타내지 않습니다.

## 10. Text와 Font

| 설정 | 기본값 | 설명 |
|---|---|---|
| `InstalledFontNames` | OS 설치 Font 목록 | 처음 접근할 때 읽어 캐시하는 읽기 전용 목록 |
| `ImageTextClearColor` | `Color.Black` | `EntityImageText` 배경색 |
| `ImageTextFillBrush` | `Brushes.White` | `EntityImageText` 채우기 Brush |
| `ImageTextPenColor` | `Color.White` | `EntityImageText` 윤곽선 색 |
| `ImageTextRenderingHint` | `SingleBitPerPixel` | GDI Text Rendering 방식 |
| `ImageTextSmoothingMode` | `None` | GDI Smoothing 방식 |
| `ImageTextPixelOffsetMode` | `HighQuality` | Pixel Offset 방식 |
| `FontDefault` | `Segoe UI` | `EntityText` 기본 Windows Font |
| `SiriusFontDefault` | `romans2.cxf` | `EntitySiriusText` 기본 Sirius Font 파일 |
| `SiriusFontCapitalSample` | `@567890ABHWMZQ0()` | Sirius Font의 대문자 높이와 Fixed 셀 폭을 계산할 대표 문자 |

이진 Raster 가공에는 일반적으로 `SingleBitPerPixelGridFit`, `SmoothingMode.None`, `PixelOffsetMode.None` 조합이 선명합니다. 회색조 변조 Raster에는 `AntiAliasGridFit`, `SmoothingMode.AntiAlias`, `PixelOffsetMode.HighQuality` 조합을 검토하십시오. 설정을 바꾼 뒤 Text Entity를 다시 생성하거나 `Regen()`하여 형상과 Raster 데이터를 갱신합니다.

### Fixed Text 문자권 Sample

Fixed 간격에서 글리프 폭을 자동 추정할 때 다음 설정을 사용합니다.

| 설정 | 기본 Sample |
|---|---|
| `FixedTextHangulFallbackSample` | `대한민국스파이럴랩옳닳흙깊` |
| `FixedTextChineseFallbackSample` | `中文汉字國語測試永高低上下左右鼎鬱龘` |
| `FixedTextJapaneseFallbackSample` | 일본어 가나·한자 대표 문자열 |
| `FixedTextLatinFallbackSample` | `HMQXWgyjpq` |
| `FixedTextCyrillicFallbackSample` | `ШЖФфрудцщ` |
| `FixedTextArabicFallbackSample` | 아랍 문자 대표 문자열 |
| `FixedTextDevanagariFallbackSample` | 데바나가리 대표 문자열 |
| `FixedTextBengaliFallbackSample` | 벵골어 대표 문자열 |
| `FixedTextGreekFallbackSample` | `ΗΜΩβγμρφψξ` |
| `FixedTextHebrewFallbackSample` | `אבגךםןףץ` |
| `FixedTextThaiFallbackSample` | 태국 문자 대표 문자열 |
| `FixedTextTamilFallbackSample` | `ழளறஞ` |
| `FixedTextTeluguFallbackSample` | 텔루구 문자 대표 문자열 |

제품에서 사용하는 Font와 문자 집합을 대표하는 글자를 지정하고, 해당 Font를 처음 불러오기 전에 설정하십시오. Sample은 실제 출력 문자열이 아니라 셀 폭과 보조 Line Metric을 추정하기 위한 값입니다.

## 11. 공통 Import

| 설정 | 기본값 | 설명과 제한 |
|---|---:|---|
| `ImportMergeDistance` | `0.001` | DXF, DWG, HPGL, PLT Path의 끝점을 연결할 최대 거리입니다. DXF/DWG는 원본 좌표 단위, HPGL/PLT는 mm 변환 후 적용됩니다. `0`이면 정확히 일치하는 끝점만 연결합니다. 음수, NaN, Infinity는 무시됩니다. |
| `IsImportColorPreserved` | `false` | `true`이면 DXF, DWG, Gerber 원본 색을 유지합니다. `false`이면 RGB 거리가 가장 가까운 `EntityPenColors` 색으로 바꿔 Pen 연결을 돕습니다. |

Merge Distance를 크게 하면 가까운 별도 Contour가 잘못 연결될 수 있습니다. 실제 파일의 좌표 단위와 최소 형상 간격을 확인한 뒤 조정하십시오.

## 12. DXF와 DWG

| 설정 | 기본값 | 설명 |
|---|---:|---|
| `DxfSplineToPolygonalCounts` | `6` | DXF Spline을 Polyline으로 근사할 때의 분할 기준 |
| `DxfTextDefaultFont` | `Arial` | DXF/DWG Text를 가져올 때 사용할 대체 Font |
| `IsDxfWithUniformGroup` | `true` | 동일 Primitive를 `EntityUniformGroup`으로 묶어 Rendering 효율을 높일지 결정 |
| `ODAConverterPath` | 자동 검색 결과 | DWG 또는 DXF 버전 변환에 사용할 ODA File Converter 경로입니다. 읽기 전용이며 Registry와 표준 설치 폴더에서 찾습니다. 찾지 못하면 `null`일 수 있습니다. |

ODA File Converter는 별도 설치 프로그램입니다. `ODAConverterPath` 값이 있다고 해서 모든 DWG 버전과 파일이 정상 변환되는 것은 아니므로 변환 결과와 로그를 확인하십시오.

## 13. 3D Mesh

| 설정 | 기본값 | 설명 |
|---|---:|---|
| `GridCloudInterval` | `0.5` | 3D Mesh에서 Grid Cloud를 생성할 간격 |

간격이 작을수록 표본 수, 메모리 사용량과 계산 시간이 증가합니다. 모델 크기와 필요한 Z 해상도를 기준으로 선택하십시오.

## 14. Gerber

| 설정 | 기본값 | 설명 |
|---|---:|---|
| `IsGerberPrecombinePolygons` | `false` | 겹치거나 교차하는 Polygon을 가져오기 단계에서 Union/Merge할지 결정합니다. 사용하면 데이터가 줄 수 있지만 처리 시간이 늘어납니다. |
| `IsGerberTessellation` | `false` | 닫힌 영역을 채우기용 Triangle로 Tessellation할지 결정합니다. |
| `IsGerberWithUniformGroup` | `true` | 빠른 Rendering을 위해 동일 Primitive를 `EntityUniformGroup`으로 묶을지 결정합니다. |

Precombine과 Tessellation은 파일 크기와 형상 복잡도에 따라 시간이 크게 달라집니다. 가져오기 속도, 표시, Hatch와 실제 가공 경로를 함께 확인하십시오.

## 15. Editor 단축키 설정

### 이동 간격

| 설정 | 기본값 | 단축키 |
|---|---:|---|
| `KeyboardTransitXYCtrl` | `1` mm | `Ctrl` + 방향키 |
| `KeyboardTransitXYCtrlAlt` | `0.1` mm | `Ctrl` + `Alt` + 방향키 |
| `KeyboardTransitXYCtrlAltShift` | `0.01` mm | `Ctrl` + `Alt` + `Shift` + 방향키 |

### 회전 각도

| 설정 | 기본값 | 단축키 |
|---|---:|---|
| `KeyboardRotateCtrl` | `90`° | `Ctrl` + `[` 또는 `]` |
| `KeyboardRotateCtrlAlt` | `10`° | `Ctrl` + `Alt` + `[` 또는 `]` |
| `KeyboardRotateCtrlAltShift` | `1`° | `Ctrl` + `Alt` + `Shift` + `[` 또는 `]` |

### 실행 키

| 설정 | 기본값 | 동작 |
|---|---:|---|
| `KeyboardSimulationStart` | `F1` | 시뮬레이션 시작. `Ctrl`, `Ctrl+Alt` 조합으로 속도를 바꾸며 `Esc`로 중지 |
| `KeyboardShowScript` | `F2` | Script 객체를 PropertyGrid에 표시 |
| `KeyboardMarkerPreview` | `F4` | Scanner Preview |
| `KeyboardMarkerStart` | `F5` | 현재 Page의 실제 Marker Start |
| `IsShowMessageBoxWhenMarkerStart` | `true` | Marker Start 단축키 실행 전에 확인 창 표시 |
| `KeyboardMarkerStop` | `F6` | Marker Stop |
| `KeyboardMarkerReset` | `F8` | Marker Reset |
| `KeyboardHelpMessage` | 동적 문자열 | 현재 이동·회전 간격과 실행 키를 포함한 읽기 전용 도움말 |

TreeView에 Focus가 있으면 방향키 조합은 Node 탐색에 사용될 수 있습니다. `KeyboardMarkerStart`는 가상 명령이 아니라 실제 장치 가공을 시작할 수 있으므로 확인 창을 끄기 전에 제품 자체의 권한, 인터록, 비상 정지 절차를 마련하십시오.

## 16. ZPL

| 설정 | 기본값 | 설명 |
|---|---|---|
| `ZPLService` | `ZPLServices.BinaryKits` | 오프라인 BinaryKits 또는 네트워크 Labelary Rendering 선택 |
| `ZPLBinaryKitsDefaultFont` | `Arial Narrow;Arial;Helvetica` | ZPL Font 식별자 `0`에 사용할 로컬 Font 후보 순서 |
| `ZPLBinaryKitsFonts` | 식별자별 후보 Dictionary | `K`, `1`, `A` 등 ZPL Font 식별자와 Printer Font 이름을 로컬 Font 후보에 연결 |
| `ZPLLabelaryAPIURIFormat` | Labelary API URI 형식 | Labelary 요청 주소 Template |

후보 Font는 `;`, `|`, `,`로 구분하며 설치된 첫 Font를 사용합니다. Labelary를 선택하면 네트워크가 필요하고 ZPL 데이터가 외부 서비스로 전달되므로 제품의 보안 및 네트워크 정책을 먼저 확인하십시오.

공개 `editor_zpl` 데모에서 기본 Font와 식별자별 후보 설정을 확인할 수 있습니다.

## 17. Marker

| 설정 | 기본값 | 설명 |
|---|---:|---|
| `MarkPreviewRepeats` | `50` | Scanner Preview 경로 반복 횟수 |
| `MarkPreviewSpeed` | `1,000` mm/s | Preview Jump/Mark 이동 속도 |
| `IsMarkArcsIntoLines` | `true` | Arc와 관련 경로를 `MinStepDistance` 기준의 `ListMarkTo` 선분으로 처리합니다. `false`이면 지원 Scanner에서 `ListArcTo`를 사용합니다. |

Preview는 Laser 출사 가공과 다르지만 Scanner가 실제로 움직일 수 있습니다. 충분히 낮은 속도와 안전한 반복 수로 시작하십시오. Arc 명령 지원 여부와 정확도는 연결된 RTC/Scanner 구현을 확인합니다.

## 18. MoF Extension

| 설정 | 기본값 | 설명 |
|---|---:|---|
| `MoFExtMcBSPFrequency` | `8,000,000` Hz | MoF Extension에서 요청할 McBSP 주파수 |

연결된 RTC, MoF Extension 장치와 Firmware가 같은 통신 조건을 지원하는지 확인하십시오. 주파수 변경만으로 배선, Clock Source 또는 장치 설정이 자동으로 맞춰지지 않습니다.

## 19. Stepper

| 설정 | 기본값 | 설명 |
|---|---:|---|
| `StepperReferenceRunTimeOut` | `30`초 | Stepper Reference Run의 완료 대기 시간, UI 범위 1~120초 |

Timeout은 Motor를 강제로 안전 정지시키는 장치 보호 기능과 동일하지 않습니다. Limit Sensor, Reference 방향, 이동 범위와 Emergency Stop을 장치 측에서 별도로 구성하십시오. 공개 `editor_steppermotor` 데모는 이 값을 사용해 Reference Run을 기다립니다.

## 20. Scanner Jog

| 설정 | 기본값 | 설명 |
|---|---:|---|
| `ScannerJogDistance` | `5` mm | Scanner Jog Form의 한 번 이동 거리, UI 범위 0.1~100 mm |

보정 Field와 광학계의 유효 영역을 넘지 않는 작은 값부터 검증하십시오. Jog는 실제 Scanner를 움직일 수 있습니다.

## 21. Remote Protocol

| 설정 | 기본값 | 설명 |
|---|---|---|
| `RemoteSeparator` | `|` | 명령과 인수 사이의 구분 문자열 |
| `RemoteTerminator` | `;` | 한 명령의 종료 문자열 |
| `RemoteOk` | `OK` | 성공 응답 |
| `RemoteNG` | `NG` | 실패 응답 |
| `RemoteReady` | `Ready` | 준비 상태 응답 |
| `RemoteNotReady` | `NotReady` | 준비되지 않은 상태 응답 |
| `RemoteBusy` | `Busy` | 실행 중 상태 응답 |
| `RemoteError` | `Error` | 오류 상태 응답 |

구분자와 응답 문자열을 바꾸면 송신 측과 수신 측을 함께 변경하십시오. 수신 Buffer에 여러 명령이 이어지거나 한 명령이 나뉘어 들어올 수 있으므로 Terminator 기준으로 완전한 Frame을 조립한 뒤 Parsing합니다.

## 22. WinForms 사용자 정의 UI 이벤트

| 이벤트 | 반환값 | 용도 |
|---|---|---|
| `OnCreateLaserUI` | `Control` | 특정 `ILaser`용 Control을 Editor의 Laser Tab에 삽입 |
| `OnCreateScannerUI` | `Control` | 특정 `IScanner`용 Control을 Scanner Tab에 삽입 |
| `OnCreateMarkerUI` | `Control` | 특정 `IMarker`용 Control을 Marker Tab에 삽입 |
| `OnScannerFieldCorrection2DShow` | `RtcCorrection2D` | 외부 Vision 오차 데이터로 만든 2D 보정 구조를 보정 Form에 전달 |
| `OnScannerFieldCorrection2DApply` | `bool` | 생성된 2D 보정 파일을 사용자가 직접 적용. 적용을 완료했으면 `true` 반환 |
| `OnScannerFieldCorrection3DShow` | 없음 | 3D 보정 UI 표시 동작 확장 |
| `OnCreateGrids` | `IEntity` | Grid Form의 입력값으로 사용자 정의 Grid Entity 생성 |

```csharp
using System.Windows.Forms;
using SpiralLab.Sirius3.Laser;
using UIConfig = SpiralLab.Sirius3.UI.Config;

UIConfig.OnCreateLaserUI += CreateLaserControl;

static Control CreateLaserControl(ILaser laser)
{
    return new MyLaserControl
    {
        LaserSource = laser
    };
}
```

이벤트는 Editor를 만들기 전에 한 번 등록하고, 더 이상 사용하지 않을 때 같은 Handler를 해제하십시오. Handler가 만든 Control은 UI Thread 규칙을 따라야 하며, 긴 장치 통신이나 파일 처리를 UI Thread에서 직접 실행하지 마십시오. Control을 해제할 때 이벤트 구독과 Timer도 함께 정리합니다.

공개 `editor_laser_ui` 데모에서 `OnCreateLaserUI`를 이용한 사용자 정의 Laser UI 연결 방법을 확인할 수 있습니다.

## 23. 적용 순서 예제

```csharp
using CoreConfig = SpiralLab.Sirius3.Config;
using UIConfig = SpiralLab.Sirius3.UI.Config;

bool coreInitialized = false;
try
{
    // 1. Core와 UI 전역 설정
    CoreConfig.LogPath = @"D:\SiriusData\Logs";
    UIConfig.SiriusFontPath = @"D:\SiriusData\Fonts";
    UIConfig.UnReDoSize = 50;
    UIConfig.ImportMergeDistance = 0.001;
    UIConfig.IsImportColorPreserved = false;
    UIConfig.MarkPreviewSpeed = 500;
    UIConfig.IsShowMessageBoxWhenMarkerStart = true;

    // 2. 사용자 정의 Factory/Event 등록
    UIConfig.OnCreateEntityPen += CreateEntityPen;
    UIConfig.OnCreateLaserUI += CreateLaserControl;

    // 3. Core 초기화
    coreInitialized = SpiralLab.Sirius3.Core.Initialize();
    if (!coreInitialized)
        throw new InvalidOperationException("Sirius3 초기화 실패");

    // 4. 장치와 Document를 만들고 EditorControl에 등록
}
finally
{
    UIConfig.OnCreateEntityPen -= CreateEntityPen;
    UIConfig.OnCreateLaserUI -= CreateLaserControl;

    // 장치, Document, Control을 먼저 Dispose합니다.
    if (coreInitialized)
        SpiralLab.Sirius3.Core.Cleanup();
}
```

Config 값을 `config*.ini`에서 읽는 제품이라면 Parsing과 범위 검사를 먼저 끝낸 뒤 위 순서로 적용하십시오. 적용한 최종값을 시작 로그에 남기면 고객 장비의 동작을 재현하기 쉽습니다.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
