# SpiralLab.Sirius3 MoF (Marking on the Fly) 통합 사용자 매뉴얼

> API 기준: Sirius3 Core/UI 1.17.0 (공개 기능)

[English](MarkingOnTheFlyUserManual.md) | [한국어](MarkingOnTheFlyUserManual.koKR.md) | [전체 안내서](README.koKR.md)


이 매뉴얼은 외부 엔코더나 절대 위치 신호(McBSP)에 동기화하여 이동 중인 작업물에 레이저 가공을 수행하는 Sirius3의 두 가지 주요 인터페이스인 IRtcMoF와 IRtcMoFExtension에 대해 설명합니다.


## 1. IRtcMoF (표준 MoF 인터페이스)

가장 일반적인 산업용 이동 가공 인터페이스로, RTC4, RTC5, RTC6 모든 하드웨어에서 지원됩니다.

### 주요 기능
- 1D Linear: 컨베이어 벨트 방향으로의 직선 위치 보정.
- Rotary: 회전 테이블이나 원통형 부품의 회전에 따른 회전 행렬(Matrix) 보정.
- 2D Stage: XY 스테이지의 엔코더 2개를 동시에 추적하여 대면적 가공 지원.
- 2D 보정 테이블: 스테이지의 기계적 오차를 쌍선형 보간(Bilinear Interpolation)으로 실시간 보상.

### 주요 메서드
- ListMoFBegin: 선형 MoF 가공 시작.
- ListMoFRotaryBegin: 회전 MoF 가공 시작.
- CtlMoFCompensateTable: 스테이지 오차 보정용 2D 테이블 로드.
- CtlMoFEncoderReset: 하드웨어 엔코더 카운터 초기화 및 기준점 동기화.


## 2. IRtcMoFExtension (고급 Fly Extension 인터페이스)

RTC6 전용 기능으로, 로봇 팔이나 3D 자유 곡면 가공 등 복잡하고 정밀한 추적이 필요한 환경에 최적화되어 있습니다.

### 주요 기능
- 다축 동시 추적: 최대 4개 축(X, Y, Z, Rotary)을 독립적 또는 동시에 추적 가능.
- McBSP 절대 위치 추적: 엔코더 펄스 방식이 아닌 외부 제어기(PLC/PC)로부터 10µs마다 절대 좌표를 직접 수신.
- 고급 파킹(Parking): 가공 대기 중 스캐너를 안전 구역으로 치우고 가상 필드 이탈(Clipping)을 방지.
- 다중 전송 모드: 레이저 파워나 외부 센서 데이터(온도, 거리 등)를 실시간으로 모니터링 및 반영.

### 주요 메서드
- CtlMoFExtInitialize: 하드웨어 및 McBSP 통신 초기화.
- ListMoFExtPark / Return: 궤적 중간에 스캐너 주차 및 오차 없는 복귀.
- CtlMoFExtMcBSPSetMultiIn: 외부 파워 제어 및 센서 모니터링 모드 설정.
- ListMoFExtWait1DAxis / 2DAxes: 특정 엔코더 위치 도달 시까지 리스트 실행 대기.


## 3. 인터페이스 비교: IRtcMoF vs IRtcMoFExtension


| 비교 항목 	| IRtcMoF (표준) 		| IRtcMoFExtension (고급) 		|
|		|				|					|	
| 지원 하드웨어 	| RTC4, RTC5, RTC6 		| RTC6 전용				|
| 최대 추적 축 	| 최대2축 (XY 또는 Rotary)	| 최대 3축 (XYZ 또는 Rotary)		|
| 입력 소스 	| RS-422 엔코더 펄스 전용 		| 엔코더 펄스 또는 McBSP 절대 위치		|
| 주요 강점 	| 설정이 간편하며 보편적임 		| 로봇 연동 및 3D 입체 추적 가능 		|
| 특수 기능 	| 2D 스테이지 정밀 오차 보정 	| 고급 파킹, 실시간 센서 모니터링 		|
| 가상 필드 크기	| RTC4 (16비트, 가상필드 없음), RTC5 (24비트, 16배), RTC6 (29비트, 512배)	|


### 장단점 비교
- IRtcMoF
  - 장점: RTC4/5 등 구형 장비와 호환성이 좋으며, 일반적인 컨베이어나 XY 스테이지 설정이 매우 간단합니다.
  - 단점: 3축(Z축 포함) 동시 추적이 어렵고, 로봇 팔과 같은 절대 좌표 기반 제어에는 한계가 있습니다.
- IRtcMoFExtension (RTC6 전용)
  - 장점: 로봇 팔이나 3D 스테이지와의 완벽한 동기화가 가능하며, 가공 중 레이저 파워를 외부 데이터에 맞게 실시간으로 가변할 수 있습니다.
  - 단점: RTC6 하드웨어가 필수이며, McBSP 통신 규격 설정 등 초기 구성 난이도가 높습니다.



## 4. Use Case (활용 사례)


### Case A: 표준 컨베이어 마킹 (IRtcMoF 사용)
- 상황: 일정한 속도로 흐르는 음료수 캔에 유통기한 마킹.
- 방법: 컨베이어 엔코더 1개를 RTC에 연결하고 `ListMoFBegin` 호출.
- 결과: 벨트 속도가 미세하게 변하더라도 글자가 압축되거나 늘어지지 않고 정확한 위치에 기록됨.

### Case B: 대면적 PCB 분할 가공 (IRtcMoF 사용)
- 상황: 스캐너 영역(100x100mm)보다 큰 500mm PCB 가공.
- 방법: PCB를 XY 스테이지에 올리고 2D 보정 테이블 적용 후 `ListMoFBegin`으로 2축 동시 추적 가공.
- 결과: 스테이지 이동 오차가 제거된 정밀한 대면적 가공 실현.

### Case C: 로봇 팔 기반 3D 차체 용접 (IRtcMoFExtension 사용)
- 상황: 로봇 팔이 스캐너를 들고 굴곡진 자동차 차체 표면을 따라 이동하며 용접.
- 방법: 로봇 제어기가 McBSP를 통해 실시간 XYZ 좌표를 RTC6로 전송. `ListMoFExtBegin`으로 3D Fly 가공 수행.
- 결과: 로봇의 이동 궤적과 스캐너의 가공 궤적이 10µs 단위로 완벽하게 일치하여 복잡한 곡면에서도 균일한 품질 유지.

### Case D: 원통형 텀블러 이미지 마킹 (IRtcMoFExtension 사용)
- 상황: 텀블러를 회전시키며 고해상도 비트맵 이미지를 옆면에 마킹.
- 방법: 회전 모터의 각도를 원주율(π)을 이용해 선형 거리로 치환. `ListMoFExtWait1DAxis`로 다음 라인 위치를 정밀하게 대기하며 한 줄씩 가공.
- 결과: 회전 오차 누적 없이 원통 표면에 왜곡 없는 이미지 가공 가능.

## 5. 공개 editor_mof_* 데모

다섯 데모는 Demos 네임스페이스의 편집 가능한 소스를 제공하며 Sirius3 NuGet 패키지를 사용합니다. 공통 [helper.cs](../demos/helper.cs)로 설정을 읽고 장치를 생성·초기화한 뒤 RegisterDevices·Marker.Ready로 연결합니다. Form1 진입 파일은 서로 다른 컨트롤러 순서를 보여 줍니다.

| 데모 | 핵심 순서·확장 지점 |
|---|---|
| [editor_mof_xy](../demos/editor_mof_xy/Form1.cs) | CreateMoFBegin(XY, true), 엔코더 좌표 대기, 벡터 개체, CreateMoFEnd 순서입니다. Reset으로 작업의 엔코더 기준을 정합니다. |
| [editor_mof_xy_raster](../demos/editor_mof_xy_raster/Form1.cs) | OnBeforeRasterLine으로 래스터 행별 처리합니다. 변환된 시작점과 래스터 간격을 기준으로 행마다 엔코더 위치를 기다립니다. |
| [editor_mof_trigger](../demos/editor_mof_trigger/Form1.cs) | DI0 Rising 대기 → MoF 시작·Reset → 바코드·텍스트 → MoF 종료 → User Event 순서입니다. Event 텍스트 변환으로 일련번호를 제공하며 예제는 Layer를 100회 반복합니다. |
| [editor_mof_offsets](../demos/editor_mof_offsets/Form1.cs) | Marker Offset과 변환된 개체 경계 기반 위치 대기를 결합합니다. Begin(false)로 Offset마다 엔코더를 리셋하지 않고 계속 같은 기준을 사용합니다. |
| [editor_mof_interrupt](../demos/editor_mof_interrupt/Form1.cs) | Breakpoint에서 목록을 대기시키고 IRtcInterrupt Handler가 호스트 작업 후 true를 반환하여 재개합니다. 예제의 1초 Sleep은 작업 지연을 표현합니다. |

### XY·래스터 위치 대기

명령 기록 전에 mm당 엔코더 카운트와 이동 방향을 설정합니다. 대기 좌표는 모터 이동 명령이 아니므로 실제 엔코더 또는 설정한 엔코더 시뮬레이션이 진행해야 합니다. 음의 X 방향 예제에서는 Under/Over 조건도 이동 방향과 일치해야 합니다. 표시 좌표만 바꾸면 대기 조건이 계속 충족되지 않을 수 있습니다.

래스터 데모는 문서·Pen의 RasterMode와 행 Callback을 결합합니다. 개체·Marker 배치를 포함한 실제 행 시작점과 간격으로 대기 위치를 계산합니다. 행 Callback에서 느린 UI·파일 작업을 피합니다. 화면 렌더링만으로 동기 래스터 가공을 검증할 수는 없습니다.

### 트리거·일련번호·Offset

Trigger 데모는 DInput.OnChanged가 아니라 컨트롤러 목록에서 DI0 Edge를 기다립니다. 바코드·텍스트 모두 TextConverters.Event를 사용하며 Marker 변환·User Event로 일련번호 상태를 갱신합니다. 호스트 이벤트만으로 마지막 물리 펄스가 끝났다고 판단하지 않습니다. 데이터 수락, 컨트롤러 완료와 외부 ACK 순서를 함께 정합니다. Marker 방식을 변경할 때 이벤트와 레이어 반복 경계를 검토합니다.

Offsets 데모는 여러 배치에서 엔코더 원점을 유지합니다. Offset 배치와 변환된 경계를 한 번씩 적용하고 엔코더·문서 좌표 규약을 맞춥니다. ExtensionData로 위치별 텍스트·바코드 값을 제공할 수 있습니다. [Offset](OffsetUserManual.koKR.md)을 참고합니다.

### Interrupt와 컨트롤러 조건

Interrupt Handler는 Windows 스케줄링 지연이 있는 호스트 Callback입니다. 예제 Sleep을 실제 외부 작업의 완료·실패 처리로 바꿉니다. 일시 정지한 상태의 연동용이며 짧은 입력 펄스 검출용이 아닙니다. 정밀한 입력 순서는 컨트롤러 Edge 대기를 사용합니다. 현재 RTC4·4e 전용 Edge Helper는 대기 명령을 기록하지 않으므로 이 데모 흐름에는 검증한 Level 대기 조합 또는 RTC5·6을 사용합니다. [디지털 I/O](DigitalIOUserManual.koKR.md)를 참고합니다.

IRtcMoF·IRtcMoFExtension·IRtcInterrupt 지원, 컨트롤러 세대, SDK·펌웨어, 활성 MoF 옵션·라이선스, 엔코더 극성·mm당 카운트·가상 필드 제한을 확인합니다. 예제의 Debug Assert는 생산 환경 검증이 아닙니다. OnEncoderSignalError·OnOutOfVirtualImageField, 중단·오류를 처리하고 종료 시 호스트 작업을 중지·해제합니다.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
