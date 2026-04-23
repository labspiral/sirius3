# Sirius3 버전 이력

## v1.8.1 (2026.4.22)
- added) Remote 통신 지원
    - 외부 통신으로 레시피 변경, 개체의 속성 조회 및 변경, 마커 가공 시작, 정지, 리셋 명령, 가공 오프셋 설정을 지원하기 위한 IRemote 인터페이스 추가
    - 시리얼 통신 지원됨
    - TCP/IP 통신 지원됨
    - 웹소켓 통신 지원됨
    - MQTT 통신 지원됨
- added) Script 데이타 변환 지원
    - 외부 C# 스크립트 파일을 이용해 가공중 Text 데이타 실시간 변경지원
    - TextConverter.SimpleScript 인 경우 동작
    - Script 폴더에 사용자 작성 C# 스크립트 사용 가능    
    - IMarker 에 ScriptInstance 항목추가
- added) SEMI OCR 폰트
    - .dot 폰트 파일 추가
    - SiriusText 개체를 이용한 도트 폰트 지원
- fixed) IDocument
    - FindByName 검색 오류 수정
- fixed) MultiBeamControl
    - 버튼 토글 상태 오류 수정
- fixed) 시뮬레이션 중 PropertyGrid 컨트롤 편집 제한

## v1.7.1 (2026.4.16)
- updated) RTC6 v1.24.0 패키지
    - 2026.3.31 릴리즈 버전으로 업데이트
- added) Rtc4MultiBeam
    - RTC4 도 멀티빔 옵션 지원
- fixed) IMarker 
    - 비동기 처리 지원 (오픈 소스 변경됨)
    - 쓰레드 대신 태스크 사용으로 변경 및 상속 구현하는 방식으로 리팩토링
- fixed) IRtcMultiBeam
    - RTC 간 배타적 동기 제어 검증 완료
    - SiriusEditorContol 검증 완료
        - 2개의 서로 다른 가공 데이타 + 2개의 서로 다른 펜 조합 지원 
    - SiriusMultiEditorControl 검증 완료
        - 1개의 동일한 가공 데이타 + 1개의 서로 다른 펜 조합 
- added) LogControl
    - 로그 메시지 필터 및 검색 기능 추가
- fixed) Shader
    - 콘솔 환경에서 뷰(View)에 개체가 렌더링 되지 않는 문제
    - 다중 뷰 대상 별 Shader 관리 지원
- fixed) Correction 3D 
    - Coeff A,B,C 계수에 대한 16, 20 비트 해상도 처리
    - Correction3DRtcForm 을 이용한 데이타 조작 개선
- fixed) 메모리 누수 
- fixed) SnapShot 을 통한 뷰 이미지 저장 버그 수정
- fixed) C# 스크립트 실행 속도 개선

## v1.6.1 (2026.4.9)
- added) ViewerControl 
    - 사용자 컨트롤 추가
    - 하나의 문서를 뷰어와 에디터에 동시 렌더링 지원
    - Document 와 단일 View 간 1:1 연결 제한 삭제   
    - 외부에서 Document 생성 및 변경 지원
- fixed) IRtc3D
    - RtcCalibrationLibrary 기반의 향상된 3D 보정 절차 지원
        - 1. Beam tile calibration: RtcCalibrationLibrary.BeamTiltCalibration
        - 2. 2D field correction: RtcCalibrationLibrary.XyCalibration
        - 3. Focus calibration at z=0: RtcCalibrationLibrary.FocusCalibrationAtZ0
        - 4. Focus calibration for coefficient A,B,C: RtcCalibrationLibrary.FocusCalibrationCoeffABC
        - 5. Stretch calibration for Z volume: RtcCalibrationLibrary.StretchCalibration
    - RtcCorrection3D 삭제 : RtcCalibrationLibrary 으로 대체 사용
    - KZScale 삭제 : RtcCalibrationLibrary 의 Focus 보상 대체 사용
    - ZOffset 삭제 : MatrixStack 의 Translate Z 로 대체 사용
- added) EntityPoint 개체 추가
- added) EntityBarcode1D_V2 개체 추가
    - 2D 바코드와 유사하게 다양한 셀 타입 지원
    - 도트, 라인, 해치 등 조합 가능
- added) 아래 개체들의 정점 목록에 대한 Open, Save 지원
    - EntityPoints
    - EntityPolyline2D
    - EntityPolyline3D
    - OffsetControl 사용자 컨트롤

## v1.5.4 (2026.4.2)
- fixed) 핫픽스
    - 디자인 타임에 SiriusEditorControl 사용자 컨트롤 생성시 발생하는 예외
    - 디자인 타임에 SiriusMultiEditorControl 사용자 컨트롤 생성시 발생하는 예외     
 
## v1.5.3 (2026.4.1)
- fixed) IDocument
    - IDocument 와 IView 간 상호 연결 설정 추가
- added) IPowerMeter
    - 파워모드, 에너지 모드로 분리 및 MeasureUnits 추가
- fixed) Rtc6
    - IsActivateAutoDelays 속성 변경시 이벤트 통지
    - IsActivateAutoDelays 속성 변경시 EntityPen, EntityLayerPen 에 항목들 visible 처리 
- fixed) EntityBarcode2D
    - CellDot 타입으로 가공시 EntityPen 의 Raster 항목을 통해 가공이 처리되도록 변경
- fixed) IRtcMultiBeam
    - 배타적 토큰(Token) 처리 검증 완료
- fixed) IRtcCorrection2D, IRtcCorrection3D
    - 스캔헤드에 설정된 내부행렬(회전 등)을 사용해 Raw 데이타 연산 처리 지원

## v1.5.2 (2026.3.27)
- added) 스테퍼 모터 제어 지원
    - RTC5,6 의 스테퍼 단자를 통한 외부 스탭모터 제어 기능 추가
    - IRtcStepper 인터페이스 추가
    - StepperControl 사용자 컨트롤 UI 추가
    - 스텝 모터의 절대, 상대 좌표 이동 지원
- added) 시리얼 통신 지원
    - RTC5,6 의 RS232 단자를 통한 통신 기능 추가
    - IRtcSerialComm 인터페이스 추가
    - SerialCommControl 사용자 컨트롤 UI 추가
    - 레이저 탭에서 송수신 데이타 (바이너리) 모니터링 가능
    - OnSerialReceived 이벤트 추가
- added) Fly Extension 개선
    - RTC6 전용 Marking on the fly 확장 기능 개선
    - IRtcMoFExtension 인터페이스 리팩토링
    - 3축 조합 (X,Y,Z 혹은 회전축) 지원
    - McBSP 통신 지원
- fixed) 사용자 컨트롤 UI 리팩토링
    - OffsetControl
    - MarkerControl
    - ScannerControl 
    - LaserControl
- fixed) 행렬 스택
    - MatrixStack 의 BaseMatrix 삭제 
    - IRtc.CtlMatrix, ListMatrix 사용으로 통합 지원
- fixed) 파워메터
    - CoherentPowerMax, GentecEO 장치에서 파워값 읽기 오류 수정
- added) SiriusEditorControl    
    - 외부 .sirius3 파일을 가져와 현재 문서의 레이어로 추가
    - 레이어 개체에서 사용중인 펜 색상이 트리뷰 에서 출력됨
- added) 편집기 메뉴에 배열 붙혀넣기 추가
 
## v1.4.1 (2026.3.10)
- added) 웹 서버를 통한 문서화 제공
    - 온라인 웹사이트: https://spirallab.co.kr/sirius3/doc 사용 가능
    - sirius3\doc\sirius3_doc_버전.zip 압축을 풀고 'start_doc.bat' 배치파일 실행으로도 사용가능
- added) 마우스를 이용한 값 편집
    - PropertyGrid 마우스 오른쪽 버튼를 누른채 좌우로 드래그 이동시 값 증가, 감소 지원
- fixed) Rtc6
    - ListLaserOn 사용시 외부 레이저 소스의 SYNC OUT 을 입력해 펄스 개수 카운트시 대기시간이 10배로 잘못 처리되는 버그
- fixed) ListLaserOn(msec)
    - 시간 종료후 레이저 오프(Laser Off Delay) 시간 자동 삽입하도록 개선
- fixed) IRtcJumpMode 인터페이스 일부 변경
- fixed) EntityPen
    - Power, PowerPercentage,PowerMapCategory 값이 보이지 않는 문제
- fixed) SiriusEditorControl
    - 4 개의 페이지(Page)를 모두 기본적으로 사용 가능함
    - WaferMap 및 Substratemap 는 비활성화

## v1.4.0 (2026.3.3)
- added) .net9.0-windows, .net10.0-windows 개발환경 추가
- added) 외부 레이저 소스의 동기 신호에 의한 펄스 개수 출력
    - LASER 커넥터의 DIGITAL IN1 으로 외부 동기 신호 입력
    - IRtc.ListLaserOn(대기 시간, 펄스 개수, 펄스 개수 종료) 
    - EntityPen 펜의 PixelPulses, IsPixelPulsesExit 값으로 설정 가능
        - 0: 기존처럼 픽셀 시간동안 LASERON 출력
        - 1~65535: 픽셀 시간 동안 외부 동기 신호를 펄스수 만큼 대기하며 LASERON 출력
        - IsPixelPulsesExit 사용시 외부 동기 신호 개수가 PixelPulses 설정값에 도달시 즉시 종료하고 다음 리스트 명령으로 이동됨
- added) (experimental) IRtcMultiBeam 인터페이스
    - 하나의 레이저 소스 + 2 개의 RTC + 2개의 AOM RF 드라이버를 이용한 멀티빔 시스템
    - Rtc6MultiBeam 
- added) EntityPoints 
    - Sort 함수를 통한 최단 경로 최적화 지원 
- added) IRtcIO 인터페이스
- fixed) EntityWaitDataExt16Cond, EntityWaitDataExt16EdgeCond, EntityWriteDataExt16, EntityWriteDataExt16Cond
    - bitmask 를 문자열 대신 ushort 타입으로 처리
- fixed) SiriusEditorControl 컨트롤
    - 디자인 타임에 추가할때 발생하는 예외
    - 컨트롤이 컨트롤 비하인드에서 생성시 OpenGL 이 초기화 되지 않아 발생하는 예외 수정
    - 컨트롤이 Load 시점에 Document 를 ActNew 강제하는 코드 삭제
- fixed) 라이센스
    - 최대 허용 인스턴스 개수를 초과 혹은 옵션이 없는 경우
    - 이전: 사용 불가
    - 변경: 30 분간 평가 모드 활성화

## v1.3.2 (2026.2.20)
- fixed) Automatic Laser Control 의 확장 모드 지원
    - Actual Velocity + Encoder + SCANAhead +  Inverse Speed Correction +  Backward Transformation + SDC + SkyWriting 신호 조합 사용가능
    - EntityLayerPen 속성에 PoD 목록에서 확장 모드 조합 설정 지원
    - EntityPoD 가 추가됨
- fixed) EntityPen 
    - SDC 기능을 위한 Spot distance 값 설정이 SpotDistanceSCANa 에서 지원됩니다.
- added) IRtcMoF 
    - 엔코더 신호 이상 알림 이벤트 지원 : IRtcMoF.OnEncoderSignalError 이벤트
    - 가상 영역을 벋어날 경우 알림 이벤트 지원 : IRtcMoF.OnOutOfVirtualImageField
    - 엔코더 신호 필터 설정 지원 (RTC6 전용) : CtlMoFEncoderFilter 함수를 사용해 노이즈가 많을때 신호의 산술 평균 사용하거나, 4MHz 이상의 고속 지원
    - 엔코더 값 조회시 절대 위치,  상대 위치를 각각 조회가능합니다.
    - OnEncoderChanged 이벤트 인자가 추가됨
- fixed) IRtcWaitID 를 IRtcInterrupt 로 이름 변경

## v1.3.1 (2026.2.9)
- added) IRtcSCANAhead 인터페이스
    - EntityPen 에 SCANAhead 용 항목(Corner, End, Acc Scale) 추가 
    - Position(or Trajectory) Acknowledge Limit 값 설정 지원 (초기값 : 전체 위치 범위의 0.28%)
    - RTC6 + SCANAhead 사용시 Trajectory ACK Limit 값을 의미함
- added) IRtcWaitID 인터페이스

## v1.3.0 (2026.2.5)
- added) EntityPolyline2D, EntityPolyline3D 
    - 정점 목록 편집기 추가
- added) SiriusMultiEditorControl 컨트롤
    - 하나의 문서 + 다중 디바이스 처리 지원
- added) EntityLayerPen 
    - 펜 값을 편집하고 도움을 주는 UI 추가
- replace) 외부 gnuplot 프로그램 삭제 및 자체 plot 내장
- fixed) scanner jog 출력 문제 해결
- added) .dwg 임포트 지원
    - ODA converter 는 사용자가 별도 설치 필요 (https://www.opendesign.com/guestfiles/oda_file_converter) 
    - .dwg, .dxf 파일 처리시 ODA converter 를 추가적으로 사용가능하도록 개선
- license) 라이센스 정책 변경
    - 3D 옵션이 삭제되고 기본 제공으로 변경
    - syncAXIS 인스턴스가 옵션으로 변경

## v1.2.7 (2026.1.26)
- added) EntityLayerPen 에 Variable Delays 기능 추가
    - Variable polygon delay: 꺽이는 각도에 따라 가변적인 폴리곤 지연시간 설정 (기본값: 활성화)
    - Variable jump delay: 점프 거리에 따라 가변적인 점프 지연시간 설정
- fixed) RTC6 에서 Skywriting 사용 LaserOnShift 값이 너무 작게 설정되는 문제
- fixed) Config.IsMarkArcsIntoLines 
    - True : 호(EntityArc)와 폴리라인(EntityPolyline2D)의 곡선 가공시 직선(ListMarkTo)으로 분해되어 처리됨
    - False: 호(EntityArc)와 폴리라인(EntityPolyline2D)의 곡선 가공시 호(ListArcTo)로 처리됨
- fixed) Contour 추출시 IsClosed 값이 잘못 계산되는 문제 수정
- fixed) Config.EntityPenColors, Config.LayerPenColors 편집 지원
- fixed) 시뮬레이션 객체에 대한 ActRemove 실패
 
## v1.2.6 (2026.1.21)
- added) 타원(ellipse) 개체 추가
- added) EntityLine, EntityArc, EntityPolyline2D
    - Automatic laser control(defined vector) 지원을 위한 RampFactor 속성 추가됨
- added) IHatch.HatchRepeats 반복 회수 추가
- fixed) EntityPen, EntityLayerPen 값이 잘못 출력되는 문제
- fixed) PowerMap CtlCompensate 애서 측정값이 범위를 벋어난 경우  
    - 기존: 좌, 우측 범위에 대한 재 측정 방식 
    - 변경: 측정된 데이타를 해당 구간에 즉시 업데이트 
- fixed) IMarker.Preview
    - 기존: 선택 개체들을 감싸는 전체 사각형 표시
    - 변경: 선택 개체들의 개별 외곽 사각형을 모두 표시

## v1.2.5 (2026.1.15)
- added) ClipHelper 추가됨 
- added) 스페이스바를 누른채로 선택시 하부 개체 선택 모드 활성화
- fixed) IHitTestable 광선검출 개선
   - Config.RayHitTestPixelSize: 동적 거리값을 이용해 검출 기능 향상
- fixed) IMarker
   - MarkTargets.Selected 인 경우 재귀적으로 자식 개체 처리하도록 개선
- updated) zxing v0.16.11 업데이트
- updated) clipper2 v.2.0.0 업데이트
	 
## v1.2.4 (2026.1.7)
- added) 단축키 추가
   - CTRL + R: 렌더링 여부를 토글
   - CTRL + M: 마킹 여부를 토글
   - 렌더링, 마킹 여부 토글시 트리 노드의 폰트(혹은 색상)가 정상적으로 변경됨
- added) IRtcFreeVariable.OnFreeVariableChanged 이벤트 추가
   - FreeVariable 값이 변경될때 발생 
- added) Config.GridCloudInterval 추가
   - IDocument.ActGridCloud 함수 호출시 사용됨
- fixed) 거버 파일 파서의 성능이 향상(시간 단축)됨
- fixed) 개체 선택(hittest)시 보다 자세한 정보를 제공
   - IDocument.SubHitEntities 은 그룹 구조의 선택시 하부 개체들을 반환
- added) 신규 ActHitTest 함수 추가
- fixed) ActUngroup 사용시 잘못된 빈 트리노드로 인한 예외 해결

## v1.0.1 (2025.12.22)
- added) .chm 도움말 파일
- added) ActExpand 함수
   - 거리에 따른 경로 확대(or 축소) 지원
- added) Gentec-EO 파워메터 장치 지원
- updated) PowerMeterOphir 장치가 StarLab v3.93 버전 사용하도록 업데이트
- fixed) hatch joints 열거형
- fixed) IDocument.FindByLayerUsedPenColors 수정됨
- fixed) Marker.EntityWork 처리시 로그 메시지 추가
 
## v0.9.3 (2025.12.5)
- added) 확대 맞춤 
   - 트리 노드를 더블클릭시 적용됨
   - 파일 열기 되었을때 적용됨
- added) 신규 TextConverters.Offset 추가됨
   - Offset.ExtensionData 값이 사용됨
- fixed) 거버 파일
   - added) UI.Config.IsGerberWithUniformGroup 으로 고속 렌더링 지원
   - fixed) UI.Config.IsGerberTessellation 사용시 테셀레이션 문제 
- renamed) scanner pen 이름을 entity pen 으로 개명

## 이하 이력 정보는 HISTORY.md 파일 참고
