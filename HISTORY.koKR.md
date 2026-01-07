# Sirius3 버전 이력

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
