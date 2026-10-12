# EntityFactory & Entity User Manual

> 기준 버전: Sirius3 1.16.0 (공개 Release 기능)

## 1. 개체를 만드는 기본 흐름

`EntityFactory`로 개체를 만든 뒤 `document.ActAdd(entity)`로 현재 Page/Layer에 추가합니다. 편집기나 TreeView에서 개체를 선택하면 PropertyGrid에 해당 개체와 연결된 `EntityPen` 속성이 표시됩니다. 크기·텍스트·해치처럼 형상에 영향을 주는 값을 바꾼 뒤에는 편집 동작이 `ActRegen`을 통해 형상과 렌더링 버퍼를 다시 생성합니다.

## 2. 기본 벡터 개체

- Point / Points: 점 출사와 점 배열
- Line / Lines: 단일 선분과 독립 선분 집합
- Arc / Ellipse: 원호, 원, 타원
- Rectangle / Triangle / Cross: 정형 형상
- Polyline2D / Polyline3D: 정점이 순서대로 연결된 열린 경로 또는 닫힌 경로
- Bezier / Catmull-Rom / B-Spline / NURBS: 제어점을 이용한 곡선

닫힌 경로를 해치하려면 실제 Contour의 닫힘과 방향이 올바른지 확인하십시오. 화면에서 닫혀 보이는 것만으로 내부 영역이 항상 유효한 것은 아닙니다.

## 3. 특수 경로

- Trepan: 내경·외경과 회전수를 가진 홀 가공 경로
- Spiral / SpiralClassic: 나선형 경로
- Lissajous: 주기 운동을 합성한 패턴
- Grid: 점·원·십자선·격자 패턴을 대량 생성

## 4. 텍스트

- `EntityText`: Windows 폰트를 윤곽선으로 변환
- `EntitySiriusText`: CXF, LFF, FNT, DOT 등 Sirius 폰트 기반
- `EntityCircularText`, `EntityCircularSiriusText`: 원호를 따라 배치
- `EntityImageText`: 텍스트를 Bitmap으로 만든 뒤 Raster 가공

### Sirius3 1.13.0 Fixed 간격

`EntityText`, `EntitySiriusText`, 원형 변형과 `EntityImageText`는 Fixed 간격에서 공백과 누락 글리프를 포함한 모든 문자를 같은 폭의 셀에 배치합니다. 내용이 바뀌어도 같은 글자 수라면 문자 위치와 전체 논리 경계가 유지됩니다.

- `FixedGlyphWidth`: Vector Text의 문자 셀 폭
- `FixedGlyphWidthPixels`: ImageText의 문자 셀 폭(pixel)
- 값이 0이면 감지한 문자권의 설정 샘플에서 자동 폭을 선택
- `IsGlyphWidthFit = false`: 글리프 종횡비를 유지
- `IsGlyphWidthFit = true`: 그릴 수 있는 글리프를 셀 폭에 맞춤
- Fixed에서는 `TargetWidth`/`TargetWidthPixels`, `WordSpacing`, Auto Kerning을 적용하지 않음

`EntityImageText`는 1.13.0에서 물리적인 문자 셀 크기는 유지하면서 위아래 투명 여백을 제거합니다.

## 5. 바코드

- 1D: Code128, Code39, PLESSEY 등
- 2D: QR, Data Matrix, PDF417, Aztec
- Cell 표현: Outline, Hatch, Dots 등

요청한 Width/Height와 실제 Matrix 크기를 구분하십시오. 잘못되거나 빈 데이터는 이전 형상을 남기지 않고 인코딩 오류를 기록합니다. Dots 가공은 EntityPen의 Raster와 Pixel 설정을 사용합니다.

## 6. 이미지와 Raster

- `EntityImage`: BMP, JPG, PNG 등 Bitmap
- `EntityImageText`: Bitmap Text
- `EntityImageZPL`: ZPL을 이미지로 변환
- `EntityStitchedImage`: 카메라/검사용 격자 이미지 시각화

실제 가공 방식은 EntityPen의 `RasterMode`, `PixelTime`, `PixelPeriod`, `PixelPulses`와 방향 설정에 따라 달라집니다.

## 7. 3D Mesh

기본 Mesh와 STL/OBJ/PLY/STP·STEP 가져오기 외에 Sirius3 1.13.0에서 다음 모델과 Factory가 추가되었습니다.

| 개체 | Factory | 핵심 입력 |
|---|---|---|
| `EntityPlane` | `CreatePlane` | 중심, 법선, 폭, 높이 |
| `EntityPyramid` | `CreatePyramid` | 기준점, 폭, 깊이, 높이 |
| `EntityTorus` | `CreateTorus` | 중심, 큰 반지름, 작은 반지름, 분할 수 |
| `EntityNURBSSurface` | `CreateNURBS3D` | Control Points, Degree, Knot, Sampling |

이 개체들은 편집기의 3D 개체 생성 메뉴에서도 추가할 수 있습니다. Mesh는 시각화용 표면이므로 실제 가공에는 Slice나 경로 변환이 필요합니다.

### 1.15.0의 Mesh 재질과 텍스처

- OBJ는 MTL 확산 색상(`Kd`), 확산 이미지(`map_Kd`), UV와 지원되는 정점 색상을 가져옵니다. 가져올 때 OBJ·MTL·참조 이미지를 함께 유지하고 상대 경로를 확인합니다.
- PLY는 지원되는 정점·면 RGB 색상을 유지합니다. 확산 텍스처에는 지원 UV 속성과 실제 이미지 파일을 가리키는 `comment TextureFile ...` 참조가 필요합니다.
- 유효한 확산 이미지는 `ColorModes.TexturePerVertex`, 텍스처 없는 원본 색상은 `ColorModes.PerVertex`, 둘 다 없는 형상은 Model Color를 사용합니다.
- 가져온 텍스처 이미지 데이터는 Sirius3 Document 저장 시 유지됩니다. 없거나 해석할 수 없는 이미지는 로그에 기록하므로 저장 전 화면을 확인합니다.

WinForms와 WPF 시각화에 공통 적용됩니다. Mesh 표현이 레이저 Power를 지정하거나 가공 경로를 자동 생성하지는 않습니다. 실제 가공은 Slice·경로 변환과 EntityPen 설정을 사용합니다.

## 8. Layer, Group, Block

- Layer: Page 안에서 개체와 레이어 조건을 묶는 실행 단위
- MixedGroup: 서로 다른 개체를 함께 이동·회전·배율 변경
- UniformGroup: 같은 렌더링 구조의 대량 개체를 묶어 성능 최적화
- Block / BlockInsert: 하나의 마스터 형상을 여러 위치에서 참조

BlockInsert의 `ModelMatrix`와 Marker의 Offset/MatrixStack이 중복 적용되지 않도록 좌표 변환을 한 단계씩 확인하십시오.

## 9. 외부 파일 가져오기

DXF/DWG, HPGL/PLT, Gerber/Excellon, G-code/NGC, 이미지와 3D 파일을 가져올 수 있습니다. `UI.Config.ImportMergeDistance`는 DXF, DWG, HPGL, PLT의 가까운 끝점을 연결하는 공통 허용 거리이며, `UI.Config.IsImportColorPreserved`는 원본 색상을 유지할지 가장 가까운 EntityPen 색상으로 바꿀지 정합니다.

1.16.0은 `.3mf` 빌드 메쉬의 mm 단위, 상대 컴포넌트 배치, RGB 색상·기본 재질·diffuse 텍스처를 유지합니다. SVG는 래스터 이미지 대신 편집 가능한 `EntitySvg`/`EntityContour` 계층으로 가져옵니다. [가져오기 조건](DocumentUserManual.koKR.md)과 [SVG 설정](Sirius3UIConfigUserManual.koKR.md)을 참고합니다. `IClippable.Clip`이 반환한 독립 결과는 호출자가 문서에 추가하거나 해제하며, 문서 작업과 Undo/Redo에는 [ActClip](ClippingUserManual.koKR.md)을 사용합니다.

## 10. EntityFactory 전체 생성 항목

현재 공개 팩토리의 **84개 함수 이름, 124개 오버로드**를 아래에 정리합니다. 생성만으로 문서에 추가되거나 가공이 시작되지는 않습니다. 독립 결과를 소유하다가 `document.ActAdd` 등의 문서 작업으로 소유권을 이전합니다. 제어 엔티티는 해당 Scanner capability가 필요합니다.

### 기본 형상

| 함수 | 반환 형식 | 오버로드 수 |
|---|---|---|
| `CreateLine` | `EntityLine` | 4 |
| `CreateLines` | `EntityLines` | 2 |
| `CreatePoint` | `EntityPoint` | 4 |
| `CreatePoints` | `EntityPoints` | 2 |
| `CreateArc` | `EntityArc` | 4 |
| `CreateEllipse` | `EntityEllipse` | 2 |
| `CreateRectangle` | `EntityRectangle` | 4 |
| `CreateTriangle` | `EntityTriangle` | 4 |
| `CreateCross` | `EntityCross` | 4 |
| `CreateTrepan` | `EntityTrepan` | 4 |
| `CreateSpiral` | `EntitySpiral` | 4 |
| `CreateSpiralClassic` | `EntitySpiralClassic` | 4 |
| `CreateLissajous` | `EntityLissajous` | 1 |

### 폴리라인·스플라인

| 함수 | 반환 형식 | 오버로드 수 |
|---|---|---|
| `CreatePolyline3D` | `EntityPolyline3D` | 1 |
| `CreatePolyline2D` | `EntityPolyline2D` | 1 |
| `CreateBezierSpline` | `EntityBezierSpline` | 2 |
| `CreateCatmullRomSpline` | `EntityCatmullRomSpline` | 1 |
| `CreateHermiteSpline` | `EntityHermiteSpline` | 1 |
| `CreateBSpline` | `EntityBSpline` | 1 |
| `CreateNURBSpline` | `EntityNURBSpline` | 1 |

### 이미지·텍스트

| 함수 | 반환 형식 | 오버로드 수 |
|---|---|---|
| `CreateImage` | `EntityImage` | 2 |
| `CreateImageZPL` | `EntityImageZPL` | 1 |
| `CreateImageText` | `EntityImageText` | 1 |
| `CreateText` | `EntityText` | 1 |
| `CreateCircularText` | `EntityCircularText` | 1 |
| `CreateCircularSiriusText` | `EntityCircularSiriusText` | 1 |
| `CreateSiriusText` | `EntitySiriusText` | 1 |

### 바코드

| 함수 | 반환 형식 | 오버로드 수 |
|---|---|---|
| `CreateDataMatrix` | `EntityDataMatrix` | 1 |
| `CreateQRCode` | `EntityQRCode` | 1 |
| `CreatePDF417` | `EntityPDF417` | 1 |
| `CreateAztec` | `EntityAztec` | 1 |
| `CreateBarcode` | `EntityBarcode1D` | 1 |
| `CreateBarcodeV2` | `EntityBarcode1D_v2` | 1 |

### 윤곽·SVG·3D

| 함수 | 반환 형식 | 오버로드 수 |
|---|---|---|
| `CreateContour` | `EntityContour` | 2 |
| `CreateSVG` | `bool` | 1 |
| `CreateMesh` | `bool` | 1 |
| `CreateCube` | `EntityCube` | 1 |
| `CreateCylinder` | `EntityCylinder` | 1 |
| `CreateCone` | `EntityCone` | 2 |
| `CreateSphere` | `EntitySphere` | 1 |
| `CreatePlane` | `EntityPlane` | 2 |
| `CreatePyramid` | `EntityPyramid` | 1 |
| `CreateTorus` | `EntityTorus` | 1 |
| `CreateNURBS3D` | `EntityNURBSSurface` | 1 |
| `CreateGridCloud` | `EntityGrids` | 1 |

### 컨테이너·파일 가져오기

| 함수 | 반환 형식 | 오버로드 수 |
|---|---|---|
| `CreateLayer` | `EntityLayer` | 1 |
| `CreateBlock` | `EntityBlock` | 1 |
| `CreateBlockInsert` | `EntityBlockInsert` | 2 |
| `CreateMixedGroup` | `EntityMixedGroup` | 1 |
| `CreateUniformGroup` | `EntityUniformGroup` | 1 |
| `CreateDxf` | `bool` | 1 |
| `CreateHPGL` | `bool` | 1 |
| `CreateGerber` | `bool` | 1 |
| `CreateGerbers` | `bool` | 1 |

### 시간·측정·MoF

| 함수 | 반환 형식 | 오버로드 수 |
|---|---|---|
| `CreateDelay` | `EntityDelay` | 1 |
| `CreateJumpTo` | `EntityJump` | 2 |
| `CreateMeasurementBegin` | `EntityMeasurementBegin` | 1 |
| `CreateMeasurementEnd` | `EntityMeasurementEnd` | 1 |
| `CreateMoFBegin` | `EntityMoFBegin` | 1 |
| `CreateMoFWait` | `EntityMoFWait` | 2 |
| `CreateMoFWaitRange` | `EntityMoFWaitRange` | 1 |
| `CreateMoFEnd` | `EntityMoFEnd` | 2 |
| `CreateMoFExtBegin` | `EntityMoFExtBegin` | 1 |
| `CreateMoFExtWait1D` | `EntityMoFExtWait1D` | 1 |
| `CreateMoFExtWait2D` | `EntityMoFExtWait2D` | 1 |
| `CreateMoFExtPark` | `EntityMoFExtPark` | 1 |
| `CreateMoFExtParkReturn` | `EntityMoFExtParkReturn` | 1 |
| `CreateMoFExtEnd` | `EntityMoFExtEnd` | 1 |

### Z·보정·출력·I/O

| 함수 | 반환 형식 | 오버로드 수 |
|---|---|---|
| `CreateZDefocus` | `EntityZDefocus` | 1 |
| `CreateZDelta` | `EntityZDelta` | 1 |
| `CreateSelectCorrectionTable` | `EntitySelectCorrectionTable` | 1 |
| `CreateRampBegin` | `EntityRampBegin` | 1 |
| `CreateRampEnd` | `EntityRampEnd` | 1 |
| `CreatePoD` | `EntityPoD` | 1 |
| `CreateWriteData` | `EntityWriteData` | 1 |
| `CreateWriteDataExt16` | `EntityWriteDataExt16` | 1 |
| `CreateWriteDataExt16Cond` | `EntityWriteDataExt16Cond` | 1 |
| `CreateWaitDataExt16Cond` | `EntityWaitDataExt16Cond` | 1 |
| `CreateWaitDataExt16EdgeCond` | `EntityWaitDataExt16EdgeCond` | 1 |
| `CreateFreeVariable` | `EntityVariable` | 1 |

### 이벤트·스테퍼 제어

| 함수 | 반환 형식 | 오버로드 수 |
|---|---|---|
| `CreateUserEvent` | `EntityUserEvent` | 1 |
| `CreateBreakPoint` | `EntityBreakPoint` | 2 |
| `CreateStepperMove` | `EntityStepperMove` | 1 |
| `CreateStepperWait` | `EntityStepperWait` | 1 |

`CreateBarcode`는 기존 `EntityBarcode1D`, `CreateBarcodeV2`는 `EntityBarcode1D_v2`를 반환합니다. `CreateImageText`는 래스터 개체이며 벡터 Text/SiriusText와 다릅니다. `CreateNURBSpline`은 곡선, `CreateNURBS3D`는 표면 메쉬입니다. `CreateGridCloud`는 격자 점 구름입니다.

`CreateSVG`, `CreateMesh`, `CreateDxf`, `CreateHPGL`, `CreateGerber`, `CreateGerbers`는 성공 여부와 `out` 결과를 반환합니다. 반환 형식이 `bool`인 함수를 단일 엔티티 생성 함수로 취급하지 않습니다. 실패 시 로그와 결과를 확인합니다. 편집기 통합 파일 가져오기는 `IDocument.ActImport`를 사용합니다.

`CreateContour`는 해치할 영역의 윤곽을 제공합니다. 닫힘·방향·내외부 중첩을 유지합니다. Layer/Block/Group은 자식 배치와 수명이 있으며 BlockInsert는 마스터 참조입니다. 기본 3D 메쉬의 표시가 자동 레이저 경로를 만들지는 않습니다.

MoF·Measurement·Ramp는 Begin/End 순서를 맞춥니다. `CreateZDefocus`와 `CreateZDelta`는 서로 다른 Z 제어 개체이며 보정 테이블·장치 설정을 확인합니다. `CreatePoD`는 출력 제어, `CreateFreeVariable`은 RTC 변수, `CreateUserEvent`는 Marker 이벤트, `CreateBreakPoint`는 지원 컨트롤러의 중단·재개 흐름입니다. Stepper Move/Wait는 스테퍼 capability가 필요합니다.

조건부 팩토리 `CreateWriteDataExt16Cond`와 `CreateWaitDataExt16Cond`는 **LOW 마스크, HIGH 마스크** 순서입니다. `inputHightBitMask`는 공개 매개변수의 기존 철자입니다. `CreateWaitDataExt16EdgeCond`에는 마스크 대신 비트 번호 0–15를 전달합니다. 예를 들어 DI0=HIGH, DI1~3=LOW일 때 DO4=HIGH로 변경하는 개체는 다음과 같습니다.

```csharp
var conditionalOutput = EntityFactory.CreateWriteDataExt16Cond(
    0x000E, 0x0001, 0x0010, SignalEdges.High);
```

관련 문서: [해치](HatchUserManual.koKR.md), [텍스트](TextUserManual.koKR.md), [바코드](BarcodeUserManual.koKR.md), [디지털 I/O](DigitalIOUserManual.koKR.md), [MoF](MarkingOnTheFlyUserManual.koKR.md), [Offset](OffsetUserManual.koKR.md).

## 11. 오버로드 참조

아래 서명은 인자 순서, 기본값과 `out` 결과를 포함합니다. `DVec2`·`DVec3`는 사용 중인 프레임워크의 OpenTK 이중 정밀도 벡터 별칭입니다.

```csharp
EntityLine CreateLine(DVec2 start, DVec2 end);
EntityLine CreateLine(DVec3 start, DVec3 end);
EntityLine CreateLine(double startX, double startY, double endX, double endY);
EntityLine CreateLine(double startX, double startY, double startZ, double endX, double endY, double endZ);
EntityLines CreateLines(IEnumerable<DVec2> vertices);
EntityLines CreateLines(IEnumerable<DVec3> vertices);
EntityPoint CreatePoint(DVec2 location);
EntityPoint CreatePoint(double x, double y);
EntityPoint CreatePoint(DVec3 location);
EntityPoint CreatePoint(double x, double y, double z);
EntityPoints CreatePoints(IEnumerable<DVec2> locations);
EntityPoints CreatePoints(IEnumerable<DVec3> locations);
EntityArc CreateArc(DVec2 center, double radius, double startAngle, double sweepAngle, uint stepAngle = 0);
EntityArc CreateArc(double cx, double cy, double radius, double startAngle, double sweepAngle, uint stepAngle = 0);
EntityArc CreateArc(DVec3 center, double radius, double startAngle, double sweepAngle, uint stepAngle = 0);
EntityArc CreateArc(double cx, double cy, double cz, double radius, double startAngle, double sweepAngle, uint stepAngle = 0);
EntityEllipse CreateEllipse(double cx, double cy, double cz, double major, double minor);
EntityEllipse CreateEllipse(DVec2 center, double major, double minor);
EntityRectangle CreateRectangle(DVec2 center, double width, double height);
EntityRectangle CreateRectangle(double cx, double cy, double width, double height);
EntityRectangle CreateRectangle(DVec3 center, double width, double height);
EntityRectangle CreateRectangle(double cx, double cy, double cz, double width, double height);
EntityTriangle CreateTriangle(DVec2 baseCenter, double baseLength, double height);
EntityTriangle CreateTriangle(double cx, double cy, double cz, double baseLength, double height);
EntityTriangle CreateTriangle(DVec3 baseCenter, double baseLength, double height);
EntityTriangle CreateTriangle(double cx, double cy, double baseLength, double height);
EntityCross CreateCross(double cx, double cy, double width, double height, double thickness);
EntityCross CreateCross(DVec2 center, double width, double height, double thickness);
EntityCross CreateCross(double cx, double cy, double cz, double width, double height, double thickness);
EntityCross CreateCross(DVec3 center, double width, double height, double thickness);
EntityTrepan CreateTrepan(DVec2 center, double innerDiameter, double outterDiameter, int revolutions);
EntityTrepan CreateTrepan(double cx, double cy, double innerDiameter, double outterDiameter, int revolutions);
EntityTrepan CreateTrepan(DVec3 center, double innerDiameter, double outterDiameter, int revolutions);
EntityTrepan CreateTrepan(double cx, double cy, double cz, double innerDiameter, double outterDiameter, int revolutions);
EntitySpiral CreateSpiral(DVec2 center, double diameter, double depth, int turns, EntitySpiral.SpiralTypes type, bool isClosed);
EntitySpiral CreateSpiral(double cx, double cy, double diameter, double depth, int turns, EntitySpiral.SpiralTypes type, bool isClosed);
EntitySpiral CreateSpiral(DVec3 center, double diameter, double depth, int turns, EntitySpiral.SpiralTypes type, bool isClosed);
EntitySpiral CreateSpiral(double cx, double cy, double cz, double diameter, double depth, int turns, EntitySpiral.SpiralTypes type, bool isClosed);
EntitySpiralClassic CreateSpiralClassic(DVec2 center, double startDiameter, double endDiameter, double height, int revolutions, bool isClosed);
EntitySpiralClassic CreateSpiralClassic(double cx, double cy, double startDiameter, double endDiameter, double height, int revolutions, bool isClosed);
EntitySpiralClassic CreateSpiralClassic(DVec3 center, double startDiameter, double endDiameter, double height, int revolutions, bool isClosed);
EntitySpiralClassic CreateSpiralClassic(double cx, double cy, double cz, double startDiameter, double endDiameter, double height, int revolutions, bool isClosed);
EntityLissajous CreateLissajous(DVec3 center, double diameter, double depth, int turns, EntityLissajous.LissajousTypes lissajousType = EntityLissajous.LissajousTypes.π, EntityLissajous.Directions direction = EntityLissajous.Directions.Cw);
EntityPolyline3D CreatePolyline3D(IEnumerable<DVec3> vertices, bool isClosed = false);
EntityPolyline2D CreatePolyline2D(IEnumerable<Vertex2D> vertices, bool isClosed = false);
EntityBezierSpline CreateBezierSpline(IEnumerable<DVec2> controlPoints);
EntityBezierSpline CreateBezierSpline(IEnumerable<DVec3> controlPoints);
EntityCatmullRomSpline CreateCatmullRomSpline(IEnumerable<DVec3> controlPoints, bool isClosed);
EntityHermiteSpline CreateHermiteSpline(IEnumerable<DVec3> controlPoints, IEnumerable<DVec3> tangents, bool isClosed);
EntityBSpline CreateBSpline(IEnumerable<DVec3> controlPoints, IEnumerable<double> knots, int degree, bool isClosed);
EntityNURBSpline CreateNURBSpline(IEnumerable<DVec3> controlPoints, IEnumerable<double> knots, IEnumerable<double> weights, int degree, bool isClosed);
EntityImage CreateImage(string fileName, double height, double width = 0);
EntityImage CreateImage(System.Drawing.Bitmap bitmap, double height, double width = 0);
EntityImageZPL CreateImageZPL(double width, double height, string zplText, EntityImageZPL.DotsPerMMs dpmm = EntityImageZPL.DotsPerMMs.Dots8_203DPI);
EntityImageText CreateImageText(string fontName, System.Drawing.FontStyle style, bool isFill, string text, uint capPixels, int outlinePixels, double height, double width = 0);
EntityText CreateText(string fontName, System.Drawing.FontStyle style, string text, double height);
EntityCircularText CreateCircularText(string fontName, System.Drawing.FontStyle style, EntityCircularText.TextCircularDirections direction, double radius, double startAngle, string text, double height);
EntityCircularSiriusText CreateCircularSiriusText(string fontName, EntityCircularSiriusText.LetterSpaces letterSpace, EntityCircularSiriusText.TextCircularDirections direction, double radius, double startAngle, string text, double height);
EntitySiriusText CreateSiriusText(string fontName, EntitySiriusText.LetterSpaces letterSpace, string text, double height);
EntityDataMatrix CreateDataMatrix(string text, EntityBarcode2DBase.Barcode2DCells cell, double width, double height);
EntityQRCode CreateQRCode(string text, EntityBarcode2DBase.Barcode2DCells cell, double width, double height);
EntityPDF417 CreatePDF417(string text, EntityBarcode2DBase.Barcode2DCells cell, double width, double height);
EntityAztec CreateAztec(string text, EntityBarcode2DBase.Barcode2DCells cell, double width, double height);
EntityBarcode1D CreateBarcode(string text, EntityBarcode1D.Barcode1DFormats format, double width, double height);
EntityBarcode1D_v2 CreateBarcodeV2(string text, EntityBarcode1D_v2.Barcode1DCells cell, EntityBarcode1D_v2.Barcode1DFormats format, double width, double height);
EntityContour CreateContour(IEnumerable<IEntity> segments, bool isClosed = false);
EntityContour CreateContour(Contour contour);
bool CreateSVG(string fileName, out EntityMixedGroup entity);
bool CreateMesh(string fileName, out EntityMesh entityMesh);
EntityCube CreateCube(DVec3 center, double width, double height, double depth = 0);
EntityCylinder CreateCylinder(DVec3 start, double radius, double depth, int segments = 24);
EntityCone CreateCone(double radiusReference, double radiusEnd, double length, int segments = 24);
EntityCone CreateCone(DVec3 reference, double radiusReference, double radiusEnd, double length, int segments = 24);
EntitySphere CreateSphere(DVec3 center, double radius, int segments = 24);
EntityPlane CreatePlane(DVec3 center, double width, double height);
EntityPlane CreatePlane(DVec3 center, DVec3 normal, double width, double height);
EntityPyramid CreatePyramid(DVec3 baseCenter, double baseWidth, double baseDepth, double height);
EntityTorus CreateTorus(DVec3 center, double majorRadius, double minorRadius, int majorSegments = 48, int minorSegments = 24);
EntityNURBSSurface CreateNURBS3D(int controlPointRows, int controlPointColumns, IEnumerable<DVec3> controlPoints, IEnumerable<double> knotsU, IEnumerable<double> knotsV, IEnumerable<double> weights, int degreeU, int degreeV, int segmentsU = 32, int segmentsV = 32, bool isClosedU = false, bool isClosedV = false);
EntityGrids CreateGridCloud(int rows, int cols, double interval, IEnumerable<double> zDepths, DVec2 zRange);
EntityLayer CreateLayer(string name);
EntityBlock CreateBlock(string blockName, IEnumerable<IEntity> entities);
EntityBlockInsert CreateBlockInsert(string blockName, string ownerBlockName, DVec3 deltaXyz);
EntityBlockInsert CreateBlockInsert(string blockName, string ownerBlockName, double dx, double dy, double dz = 0);
EntityMixedGroup CreateMixedGroup(string name, IEnumerable<IEntity> entities);
EntityUniformGroup CreateUniformGroup(string name, IEnumerable<IEntity> entities);
bool CreateDxf(string fileName, out EntityMixedGroup dxfEntity);
bool CreateHPGL(string fileName, out EntityMixedGroup hpglEntity);
bool CreateGerber(string fileName, System.Drawing.Color color, out EntityGerber gerberEntity);
bool CreateGerbers(string path, string filter, out EntityGerber[] gerberEntities);
EntityDelay CreateDelay(double mesc);
EntityJump CreateJumpTo(DVec3 jumpTo);
EntityJump CreateJumpTo(double x, double y, double z = 0);
EntityMeasurementBegin CreateMeasurementBegin(double samplingFrequency, IEnumerable<MeasurementChannels> channels, string title = "");
EntityMeasurementEnd CreateMeasurementEnd();
EntityMoFBegin CreateMoFBegin(RtcMoFModes type = RtcMoFModes.XY, bool isEncoderReset = true);
EntityMoFWait CreateMoFWait(RtcEncoders encoder, RtcEncoderWaitConditions condition, double position, bool isApplyMatrixStack = false);
EntityMoFWait CreateMoFWait(RtcEncoderWaitConditions condition, double angle);
EntityMoFWaitRange CreateMoFWaitRange(DVec2 min, DVec2 max);
EntityMoFEnd CreateMoFEnd(DVec2 jumpTo);
EntityMoFEnd CreateMoFEnd(double x, double y);
EntityMoFExtBegin CreateMoFExtBegin(bool isEncoderReset);
EntityMoFExtWait1D CreateMoFExtWait1D(MoFExtWait1DConditions condition, double position, bool keepFlyActive, bool turnLaserOff);
EntityMoFExtWait2D CreateMoFExtWait2D(MoFExtWait2DConditions condition, DVec2 rangeX, DVec2 rangeY, bool keepFlyActive, bool turnLaserOff);
EntityMoFExtPark CreateMoFExtPark(DVec2 position, bool keepFlyActive);
EntityMoFExtParkReturn CreateMoFExtParkReturn();
EntityMoFExtEnd CreateMoFExtEnd();
EntityZDefocus CreateZDefocus(double zDefocus);
EntityZDelta CreateZDelta(double zDelta);
EntitySelectCorrectionTable CreateSelectCorrectionTable(CorrectionTables primaryHead = CorrectionTables.Table1, CorrectionTables secondaryHead = CorrectionTables.None);
EntityRampBegin CreateRampBegin(AutoLaserControlSignals signal, double startingValue);
EntityRampEnd CreateRampEnd();
EntityPoD CreatePoD(double alcPercentage100, double alcMinValue, double alcMaxValue);
EntityWriteData CreateWriteData(ExtensionChannels channel, double data);
EntityWriteDataExt16 CreateWriteDataExt16(uint bitPosition, bool onOff);
EntityWriteDataExt16Cond CreateWriteDataExt16Cond(ushort inputLowBitMask, ushort inputHightBitMask, ushort outputBitMask, SignalEdges outputLevel = SignalEdges.High);
EntityWaitDataExt16Cond CreateWaitDataExt16Cond(ushort inputLowBitMask, ushort inputHightBitMask);
EntityWaitDataExt16EdgeCond CreateWaitDataExt16EdgeCond(uint bitPosition, SignalEdges signalEdge = SignalEdges.High);
EntityVariable CreateFreeVariable(uint no, uint data);
EntityUserEvent CreateUserEvent(string extensionData = "");
EntityBreakPoint CreateBreakPoint(long waitID);
EntityBreakPoint CreateBreakPoint(IEntity entity);
EntityStepperMove CreateStepperMove(StepperAxes axis, StepperMoveTypes type, double position, double velocity);
EntityStepperWait CreateStepperWait(StepperCombinedAxes axis, double msec);
```

---
2026 Copyright (c) SpiralLAB. All rights reserved.
