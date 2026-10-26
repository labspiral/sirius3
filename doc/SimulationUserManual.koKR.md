# [사용자 메뉴얼] 가공 경로 시뮬레이션 (Simulation)

> API 기준: Sirius3 Core/UI 1.17.0 (공개 기능)

[English](SimulationUserManual.md) | [한국어](SimulationUserManual.koKR.md) | [전체 안내서](README.koKR.md)


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


## 3. 편집기 명령

F1 / Ctrl+F1 / Ctrl+Alt+F1은 빠른 / 보통 / 느린 시뮬레이션입니다. 시뮬레이션
도구 모음 버튼과 속도 보조 키도 사용할 수 있으며 Escape로 중지합니다. 시작 키는
`UI.Config.KeyboardSimulationStart` 설정을 따릅니다. 뷰포트 또는 트리에 포커스를
두십시오. 텍스트 입력 칸은 일반 편집 명령을 사용합니다.

시뮬레이션은 화면 표시용입니다. 스캐너 이동·실제 장비 동작이 가능한 Marker 미리보기와
Marker Start 명령과 구분하십시오.

## 4. 시뮬레이션 상태 확인

   - IDocument.IsSimulationWorking: 현재 시뮬레이션이 동작 중인지 여부를 반환합니다. 이 상태가 true일 때는 일반적인 편집 작업이 제한될 수 있습니다.

   - Sirius3 1.11.10 이상에서는 경로 마커가 화면에서 일정한 크기로 보이고, Beam과 선택형 Debris 효과로 Jump/Mark 흐름을 구분할 수 있습니다.
   - Sirius3 1.11.14 이상에서는 ESC가 시뮬레이션을 즉시 중지하며, 종료 정리 중 Virtual RTC Abort가 반복되거나 정상적인 Virtual Laser 중지가 오류로 기록되지 않습니다.


## 5. 공개 API 예제

```csharp
if (!document.IsSimulationWorking && document.Selected.Length > 0)
    await document.ActSimulateStart(view, document.Selected, marker,
        IDocument.SimulationSpeeds.Normal);

// 사용자가 중지를 요청하면 실행합니다.
document.ActSimulateStop();
```

프로그램에 연결한 View와 준비된 Marker를 전달합니다. 선택 경로와 가공 순서를
시뮬레이션하며 레시피 데이터를 바꾸지 않습니다.

## 6. 주의 사항

   - 엔티티 선택 필수: 시뮬레이션을 실행하려면 하나 이상의 엔티티가 선택되어 있어야 합니다. (Document.Selected 참조)
   - 장치 설정: 시뮬레이션은 내부 마커(IMarker) 로직을 사용하므로, 마커 객체가 정상적으로 할당되어 있어야 합니다.
   - 렌더링 성능: Slow 모드에서는 레이저의 On/Off 상태와 점프 구간을 매우 정밀하게 확인할 수 있으나, 전체 경로가 긴 경우 시간이 오래 걸릴 수 있습니다.
