# System Log Monitoring & Logger Events User Manual

> 기준 버전: Sirius3 1.12.3 (공개 Release 기능)


## 1. 개요 (Overview)

LogControl은 Sirius3 프레임워크 내부에서 발생하는 모든 시스템 이벤트, 오류, 경고 및 디버그 정보를 실시간으로 수집하여 화면에 표시하는 컨트롤입니다. 
멀티스레드 환경에서 안전하게 로그를 수집하며, 가독성을 높이기 위한 색상 구분 및 필터링 기능을 제공합니다.

## 2. 주요 UI 기능 (UI Features)

- 로그 리스트뷰 (ListView):
  - Time: 로그 발생 시간 (HH:mm:ss.fff 단위).
  - Level: 로그의 심각도 (Debug, Information, Warning, Error, Critical).
  - Message: 발생한 상세 내용. (여러 줄 메시지 지원)
- 필터링 및 검색:
  - Category 필터: 특정 레벨(예: Error만 보기)의 로그만 골라낼 수 있습니다.
  - 텍스트 검색 (Search): 메시지 내용 중 특정 키워드가 포함된 로그만 실시간 필터링합니다. (2글자 이상 입력 시 동작)
- 조작 버튼:
  - Clear: 현재 화면의 로그와 대기 중인 로그 큐를 모두 비웁니다.
  - Open Folder: 로그 파일이 실제로 저장되는 하드디스크 폴더를 엽니다.
  - Ctrl + C: 선택한 로그 항목들을 탭 구분자 형태의 텍스트로 클립보드에 복사합니다.

## 3. 로거 이벤트 구조 (Logger Event Architecture)

Sirius3의 로그 시스템은 '발행-구독(Pub-Sub)' 모델을 따릅니다.

- 이벤트 발생: 프레임워크 내부 어디서든 `Logger.Log(LogLevel, "message")`를 호출하면 로그가 생성됩니다.
- 이벤트 구독: `LogControl`은 로드 시점에 `SpiralLab.Sirius3.Config.OnLogged` 이벤트를 구독합니다.
  - `OnLogged` 델리게이트 형식: `Action<LogLevel, string>`
- 스레드 안전성 (Thread-Safety):
  - 가공 마커나 하드웨어 제어 스레드는 UI 스레드와 별개로 동작합니다.
  - 로그 메시지는 즉시 `ConcurrentQueue`에 안전하게 저장됩니다.
  - UI 업데이트 타이머(100ms 주기)가 UI 스레드에서 큐를 비우며 화면을 갱신하므로, 크로스 스레드 예외 없이 안정적인 출력이 가능합니다.

## 4. 로그 레벨 및 색상 가이드 (Log Levels)

- Error / Critical (Red): 시스템 정지, 하드웨어 에러, 통신 실패 등 즉각적인 조치가 필요한 상황.
- Warning (Yellow): 가공 중 주의 사항, 잠재적 문제, 비정상적 설정값 알림.
- Information (Default): 일반적인 작업 시작/종료, 설정 변경 등의 정상 상태 정보.
- Debug (Default): 개발 및 문제 분석을 위한 상세 추적 정보.

## 5. 주요 설정 파라미터 (Configurations)

로그의 동작 방식은 `SpiralLab.Sirius3.Config` 클래스의 다음 속성들에 의해 결정됩니다.
- MaxLogItems: 화면에 유지할 최대 로그 개수. 초과 시 가장 오래된 로그부터 자동 삭제됩니다.
- LogPath: 로그 파일이 물리적으로 기록되는 경로.

## 6. 주의 사항 (Cautions)

- 대량의 로그 발생: 초당 수천 건 이상의 로그가 지속적으로 발생할 경우 UI 렌더링 성능에 영향을 줄 수 있으므로 가급적 필요한 정보 위주로 로그 레벨을 관리하십시오.
- 검색 모드: 검색어가 입력된 상태에서는 새로운 로그가 발생해도 조건에 맞지 않으면 화면에 보이지 않을 수 있습니다. 모든 로그를 보려면 검색창을 비우십시오.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
