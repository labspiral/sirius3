# SpiralLab.Sirius3 MoF (Marking on the Fly) 통합 사용자 매뉴얼

> 기준 버전: Sirius3 1.12.3 (공개 Release 기능)


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
