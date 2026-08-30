# EntityFactory & Entity User Manual

> 기준 버전: Sirius3 1.12.3 (공개 Release 기능)

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

### Sirius3 1.12.3 Fixed 간격

`EntityText`, `EntitySiriusText`, 원형 변형과 `EntityImageText`는 Fixed 간격에서 공백과 누락 글리프를 포함한 모든 문자를 같은 폭의 셀에 배치합니다. 내용이 바뀌어도 같은 글자 수라면 문자 위치와 전체 논리 경계가 유지됩니다.

- `FixedGlyphWidth`: Vector Text의 문자 셀 폭
- `FixedGlyphWidthPixels`: ImageText의 문자 셀 폭(pixel)
- 값이 0이면 감지한 문자권의 설정 샘플에서 자동 폭을 선택
- `IsGlyphWidthFit = false`: 글리프 종횡비를 유지
- `IsGlyphWidthFit = true`: 그릴 수 있는 글리프를 셀 폭에 맞춤
- Fixed에서는 `TargetWidth`/`TargetWidthPixels`, `WordSpacing`, Auto Kerning을 적용하지 않음

`EntityImageText`는 1.12.3에서 물리적인 문자 셀 크기는 유지하면서 위아래 투명 여백을 제거합니다.

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

기본 Mesh와 STL/OBJ/PLY/STP·STEP 가져오기 외에 Sirius3 1.12.3에서 다음 모델과 Factory가 추가되었습니다.

| 개체 | Factory | 핵심 입력 |
|---|---|---|
| `EntityPlane` | `CreatePlane` | 중심, 법선, 폭, 높이 |
| `EntityPyramid` | `CreatePyramid` | 기준점, 폭, 깊이, 높이 |
| `EntityTorus` | `CreateTorus` | 중심, 큰 반지름, 작은 반지름, 분할 수 |
| `EntityNURBSSurface` | `CreateNURBS3D` | Control Points, Degree, Knot, Sampling |

이 개체들은 편집기의 3D 개체 생성 메뉴에서도 추가할 수 있습니다. Mesh는 시각화용 표면이므로 실제 가공에는 Slice나 경로 변환이 필요합니다.

## 8. Layer, Group, Block

- Layer: Page 안에서 개체와 레이어 조건을 묶는 실행 단위
- MixedGroup: 서로 다른 개체를 함께 이동·회전·배율 변경
- UniformGroup: 같은 렌더링 구조의 대량 개체를 묶어 성능 최적화
- Block / BlockInsert: 하나의 마스터 형상을 여러 위치에서 참조

BlockInsert의 `ModelMatrix`와 Marker의 Offset/MatrixStack이 중복 적용되지 않도록 좌표 변환을 한 단계씩 확인하십시오.

## 9. 외부 파일 가져오기

DXF/DWG, HPGL/PLT, Gerber/Excellon, G-code/NGC, 이미지와 3D 파일을 가져올 수 있습니다. `UI.Config.ImportMergeDistance`는 DXF, DWG, HPGL, PLT의 가까운 끝점을 연결하는 공통 허용 거리이며, `UI.Config.IsImportColorPreserved`는 원본 색상을 유지할지 가장 가까운 EntityPen 색상으로 바꿀지 정합니다.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
