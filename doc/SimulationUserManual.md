# [사용자 메뉴얼] 가공 경로 시뮬레이션 (Simulation)


## 1. 개요

  시뮬레이션 기능은 실제 레이저 출력을 하지 않고, 소프트웨어 화면(OpenGL View) 상에서 가공 경로와 스캐너의 움직임을 시각적으로 확인하기 위한 기능입니다. 
  가공 순서, 점프 경로, 가공 속도감 등을 사전에 검토할 수 있습니다.



## 2. 시뮬레이션 실행 (IDocument.ActSimulateStart)

  문서 객체(IDocument)를 통해 시뮬레이션을 제어합니다.

  메서드 상세
   - ActSimulateStart(view, entities, marker, simulationSpeed)
       - view: 시뮬레이션이 출력될 IView 객체
       - entities: 시뮬레이션 대상 엔티티 배열 (선택된 엔티티만 시뮬레이션 가능)
       - marker: 마커 장치 추상화 인터페이스
       - simulationSpeed: 시뮬레이션 재생 속도 (Slow, Normal, Fast)

  시뮬레이션 중지 (ActSimulateStop)
   - 실행 중인 시뮬레이션을 즉시 중단하고 뷰를 원래 상태로 복구합니다.


## 3. EditorControl UI 및 단축키 활용

  EditorControl 상단의 툴바 버튼과 키 조합을 통해 시뮬레이션 속도를 조절하거나 제어할 수 있습니다.

  툴바 버튼: btnSimulation (아이콘 형태)
   - 기본 클릭: 시뮬레이션을 시작하거나, 이미 동작 중인 경우 중지합니다.
   - 속도 제어 (조합키): 버튼을 클릭할 때 누르고 있는 키에 따라 속도가 결정됩니다.
       - 클릭 (기본): Fast (빠름) 속도로 실행
       - Ctrl + 클릭: Normal (보통) 속도로 실행
       - Ctrl + Alt + 클릭: Slow (느림) 속도로 실행

  키보드 단축키 및 제어
   - 시뮬레이션 시작: 일반적으로 툴바의 버튼을 통해 실행합니다.
   - 중지 및 취소 (ESC 키):
       - 시뮬레이션이 진행 중일 때 ESC 키를 누르면 즉시 시뮬레이션이 중단됩니다.
       - EditorControl은 내부적으로 키 입력을 감지하여 Document.ActSimulateStop()을 호출하도록 설계되어 있습니다.


## 4. 시뮬레이션 상태 확인

   - IDocument.IsSimulationWorking: 현재 시뮬레이션이 동작 중인지 여부를 반환합니다. 이 상태가 true일 때는 일반적인 편집 작업이 제한될 수 있습니다.


## 5. 사용 예시 (C#)

  시뮬레이션 시작 코드 예시

   1 // 선택된 엔티티들에 대해 보통 속도로 시뮬레이션 시작
   2 if (!document.IsSimulationWorking)
   3 {
   4     var selected = document.Selected;
   5     if (selected.Length > 0)
   6     {
   7         await document.ActSimulateStart(view, selected, marker, IDocument.SimulationSpeeds.Normal);
   8     }
   9 }

  시뮬레이션 중지 코드 예시

   1 // 실행 중인 시뮬레이션 중지
   2 document.ActSimulateStop();
   3 



## 6. 주의 사항

   - 엔티티 선택 필수: 시뮬레이션을 실행하려면 하나 이상의 엔티티가 선택되어 있어야 합니다. (Document.Selected 참조)
   - 장치 설정: 시뮬레이션은 내부 마커(IMarker) 로직을 사용하므로, 마커 객체가 정상적으로 할당되어 있어야 합니다.
   - 렌더링 성능: Slow 모드에서는 레이저의 On/Off 상태와 점프 구간을 매우 정밀하게 확인할 수 있으나, 전체 경로가 긴 경우 시간이 오래 걸릴 수 있습니다.
