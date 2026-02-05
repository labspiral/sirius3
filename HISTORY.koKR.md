# Sirius3 버전 이력

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
