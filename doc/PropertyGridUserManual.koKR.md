# PropertyGrid Control & Entity Inspector User Manual

> 기준 버전: Sirius3 1.12.3 (공개 Release 기능)

## 1. PropertyGrid의 역할

PropertyGrid는 편집기나 TreeView에서 선택한 Page, Layer, Entity, EntityPen, EntityLayerPen과 장치 설정을 분류별로 보여주고 편집합니다. 값을 바꾸면 Document의 변경 알림과 필요한 형상 재생성이 이어져 편집기에 반영됩니다.

## 2. 선택과 표시

- 한 개체 선택: 해당 개체의 모든 공개 편집 속성 표시
- 여러 개체 선택: 공통 속성만 표시하고 한 번의 변경을 선택 개체 전체에 적용
- Layer/Pen 선택: 개체 형상 대신 레이어 또는 가공 조건 표시
- 장치 변경: 연결된 RTC·Laser가 지원하지 않는 속성은 숨기거나 읽기 전용으로 표시

개체를 설명하거나 값을 비교할 때는 TreeView나 편집기에서 대상을 먼저 선택해 PropertyGrid가 채워졌는지 확인하십시오.

## 3. 속성 검색

Sirius3 1.11.14부터 속성 **이름, 분류, 설명**을 검색할 수 있습니다.

- `CTRL+F`: 검색창으로 이동
- 검색어 입력: 일치하는 속성만 표시
- Clear 버튼: 검색 조건을 한 번에 지움

검색 중 보이지 않는 속성은 삭제된 것이 아니라 필터에서 제외된 것일 수 있습니다. 장치 지원 여부와 검색어를 함께 확인하십시오.

## 4. 주요 분류

- Basic: 이름, ID, 가공/렌더링 허용
- Transform: 위치, 회전, 배율, ModelMatrix
- Geometry/Text/Hatch: 크기, 정점, 텍스트, 해치
- Laser: Power, PowerMax, PowerMapCategory, Frequency, PulseWidth
- Scanner: Mark/Jump Speed, Laser/Scanner Delay, Hard Jump
- Raster: RasterMode, PixelTime, PixelPeriod, PixelPulses
- Wobbel: Shape, Frequency, Parallel/Perpendicular 진폭
- Layer Advanced: Sky Writing, Variable Delay, ALC, SCANAhead 연계
- syncAXIS: MotionType, Bandwidth 등 협조 모션 설정

## 5. 값 변경과 범위

숫자 편집기는 허용 범위를 벗어난 값을 속성의 최소·최대값 안으로 조정할 수 있습니다. 입력값이 그대로 남았다고 가정하지 말고 편집 완료 후 표시값과 로그를 확인하십시오.

`UI.Config.IsConvertToControllerResolution`을 켜면 EntityPen/EntityLayerPen의 시간·주파수 값이 연결된 RTC의 적용 해상도에 맞춰 표시될 수 있습니다. 이 옵션은 좌표 KFactor 변환과 별개입니다.

## 6. 다국어 설명

Core와 UI Config, PropertyGrid 이름·분류·설명은 선택한 언어 리소스를 사용합니다. Sirius3 1.11.14부터 관련 설정, 주의사항과 적용 순서를 여러 줄로 다시 표시합니다. 다른 언어로 바꾼 뒤에는 PropertyGrid를 새로 열거나 선택을 갱신해 표시를 확인하십시오.

## 7. 가공 중 제한

Marker가 Busy이면 형상과 공정 조건 편집을 제한합니다. 확대·축소와 화면 이동은 가능하지만 선택과 데이터 변경은 잠깁니다. F5는 실제 Marker Start로 이어질 수 있으므로 작업 영역과 레이저 안전 상태를 확인한 뒤 사용하십시오.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
