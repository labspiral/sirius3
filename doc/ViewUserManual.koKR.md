# View System & OpenGL Rendering User Manual

> 기준 버전: Sirius3 1.12.3 (공개 Release 기능)


## 1. 개요 (Overview)

Sirius3의 View 시스템은 수만 개의 벡터 데이터를 실시간(60fps 이상)으로 시각화하기 위해 OpenGL 3.3 코어 프로필을 기반으로 설계되었습니다. 
본 시스템은 화면 출력을 담당하는 Surface, 시각적 논리를 관리하는 View, 그리고 GPU 자원을 관리하는 Resource로 구성됩니다.

## 2. 구성 요소별 역할 (Core Components)


[WinFormsGLSurface: 하드웨어 연결 레이어]
- 역할: WinForms의 `GLControl`과 OpenGL 컨텍스트를 연결하는 브릿지입니다.
- 주요 기능: 
  - 마우스 및 키보드 이벤트를 OpenGL 좌표계로 변환하여 전달.
  - `MakeCurrent()`: 스레드별 OpenGL 컨텍스트 활성화 제어.
  - `Invalidate()`: 화면 갱신 요청 및 `SwapBuffers` 수행.
- 중요: `MakeCurrent()`는 반드시 UI 스레드에서만 호출되어야 합니다. 백그라운드 스레드에서 직접 GPU 자원에 접근하는 것은 금지됩니다.

[ViewBase: 시각 논리 관리자]
- 역할: 카메라, 조명, 도면(Document) 및 마커 상태를 조합하여 하나의 장면(Scene)을 구성합니다.
- 주요 기능:
  - 카메라 제어: 2D(직교) 및 3D(원근) 카메라 전환 및 Zoom Fit 기능.
  - 인터랙션: 마우스 드래그를 통한 개체 이동, 러버밴드(Frustum) 선택 처리.
  - 시각화 보조: FOV 영역 표시, 그리드(Checkerboard), 좌표축(Axis) 렌더링.
  - 시뮬레이션: 레이저 가공 경로의 실시간 애니메이션 가이드 표시.

[GLResource: GPU 리소스 관리자]
- 역할: 개별 엔티티(Entity)의 정점(Vertex) 데이터를 GPU 메모리(VRAM)로 관리합니다.
- 동기화 메커니즘 (`SyncWith`):
  - 엔티티의 `GeometryVersion`이 변경되었을 때만 GPU 버퍼(VAO, VBO, EBO)를 업데이트하여 불필요한 데이터 전송을 억제합니다.
  - 정점(Position), 색상(Color), 법선(Normal), 텍스처(UV) 데이터를 독립적으로 버퍼링합니다.

[Shaders: 렌더링 파이프라인]
- 역할: GPU에서 실행되는 고속 연산 프로그램(GLSL)을 관리합니다.
- 제공 셰이더:
  - General: 일반적인 벡터 및 메시 개체를 위한 표준 셰이더 (조명, 슬라이스 효과 포함).
  - Font: 고해상도 SDF(Signed Distance Field) 기반 텍스트 렌더링 전용.
  - Plane: 작업 평면 및 그리드 렌더링용.

## 3. 렌더링 워크플로우 (Rendering Workflow)

가공 데이터가 화면에 표시되는 과정은 다음과 같습니다.

1) 데이터 변경: 사용자가 개체의 속성을 변경합니다.
2) 재생성: `document.ActRegen()` 호출 시 `GeometryVersion`이 증가합니다.
3) 동기화: 다음 프레임 렌더링 시 `GLResource.SyncWith()`가 버전 차이를 감지하고 새 데이터를 GPU로 업로드합니다.
4) 렌더링: `GLResource.Render()`가 호출되어 셰이더에 행렬(Model/View/Projection)과 조명 파라미터를 전달하고 `GL.DrawElements`를 실행합니다.

## 4. 주요 조작 단축키 (View Interactions)

- 마우스 휠: 확대 / 축소 (커서 위치 기준).
- 마우스 우클릭 드래그: 카메라 회전 (3D 모드 전용).
- 마우스 휠 클릭 드래그: 화면 평행 이동 (Panning).
- Space 바 (누른 채): 서브 엔티티(그룹 내부 개체) 선택 모드 활성화.
- Ctrl + F: 선택 개체 또는 전체 도면 화면 맞춤 (Zoom Fit).

## 5. 개발자 주의 사항 (Developer Notes)

- 리소스 해제: `GLResource`는 비관리형 자원(VRAM)을 사용하므로, 엔티티가 삭제되거나 문서가 닫힐 때 반드시 `Dispose()`가 호출되어야 메모리 누수를 방지할 수 있습니다.
- 렌더링 성능: 대량의 동일 개체를 렌더링할 때는 `EntityUniformGroup`을 사용하여 드로우 콜(Draw Call) 횟수를 최소화하십시오.
- 투명도 제어: `EntityModelBase.Alpha` 속성을 통해 개체의 투명도를 조절할 수 있습니다.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
