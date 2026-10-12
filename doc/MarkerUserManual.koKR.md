# RTC Laser Marker Control User Manual

> 기준 버전: Sirius3 1.16.0 (공개 Release 기능)

[English](MarkerUserManual.md) | [한국어](MarkerUserManual.koKR.md)


## 1. 개요 (Overview)

본 매뉴얼은 SCANLAB RTC 제어기를 사용하여 레이저 가공(마킹)을 수행하는 Sirius3 MarkerRtc 및 MarkerRtcControl의 사용 방법을 설명합니다. 이 시스템은 도면에 배치된 엔티티(도형)들을 실제 가공 리스트로 변환하여 RTC 카드로 전송하고 실행하는 역할을 합니다.

## 2. 마킹 대상 설정 (Mark Targets)

가공할 엔티티의 범위를 선택할 수 있습니다.
- All (전체): 현재 문서 페이지 내의 모든 가공 가능한 엔티티를 순차적으로 마킹합니다.
- Selected (선택): 사용자가 마우스로 선택한 특정 엔티티들만 마킹합니다. (미리보기 시 유용)

## 3. 마킹 절차 및 순서 (Mark Procedures) - 중요

다중 오프셋(배열 가공) 시 가공 효율을 결정하는 중요한 설정입니다.
- Layer First (레이어 우선): 
  - 방식: 오프셋 1번 위치에서 전체 레이어 가공 -> 오프셋 2번 위치에서 전체 레이어 가공...
  - 특징: 한 제품을 완전히 가공한 후 다음 제품으로 이동하는 일반적인 방식입니다.
- Offset First (오프셋 우선): 
  - 방식: 레이어 1번을 모든 오프셋 위치에서 가공 -> 레이어 2번을 모든 오프셋 위치에서 가공...
  - 특징: 툴 교체(공구 교환)나 특정 파라미터 유지가 중요한 경우 사용됩니다.

## 4. 하드웨어 상태 및 안전 확인 (Health Checks)

가공 시작 전 RTC 카드 및 스캐너의 물리적 상태를 자동으로 체크하도록 설정할 수 있습니다.
- Check Temp: 스캐너 헤드의 온도가 정상 범위인지 확인합니다.
- Check Power: 스캐너 전원 공급이 안정적인지 확인합니다.
- Check Position Ack: 스캐너가 지령된 궤적을 정상적으로 추종하는지(오차 범위 내) 확인합니다.

## 5. 주요 기능 버튼 (Action Buttons)

- Start: 설정된 조건에 따라 실제 레이저 마킹을 시작합니다.
- Stop: 진행 중인 가공을 즉시 중단합니다. (RTC List Abort 명령 수행)
- Preview: 실제 레이저를 쏘지 않고 가이드 레이저(Red Pointer)를 사용하여 선택한 엔티티의 외곽 영역(Bounding Box)을 반복 표시합니다. 위치 확인 시 필수 기능입니다.
- Reset: RTC 카드 및 레이저 소스의 에러 상태를 초기화합니다.

## 6. 리스트 버퍼 및 측정 (List & Measurement)

- List Buffer: 'Auto' 또는 'Single' 버퍼 모드를 선택합니다. 대량의 데이터 가공 시 하드웨어 메모리 활용 방식을 결정합니다.
- Measurement Plot: 가공 중 발생하는 스캐너 위치, 속도, 레이저 상태 등의 실시간 데이터를 RTC 내부 메모리에 기록하고, 가공 완료 후 그래프로 시각화합니다. 가공 품질 분석 및 디버깅에 매우 유용합니다.

## 7. 상태 표시등 (Status Indicators)

- Ready: 하드웨어가 초기화되어 가공 준비가 된 상태 (녹색).
- Busy: 현재 가공이 진행 중이거나 리스트가 실행 중인 상태 (황색 점멸).
- Error: 하드웨어 또는 소프트웨어에 치명적인 오류가 발생한 상태 (적색).

## 8. 주의 사항 (Cautions)

- 가공 중(Busy 상태)에는 마킹 절차나 대상을 변경할 수 없습니다.
- 'Stop' 클릭 시 스캐너는 원점(0,0)으로 복귀하며, 가공 중이던 리스트는 취소됩니다.
- 실제 가공 전 반드시 'Preview' 기능을 통해 가공 위치와 범위를 육안으로 확인하십시오.

## 9. 스크립트 (Script)

- ScriptingInstance: 'ScriptFactory.Create' 를 이용해 외부 스크립트 객체를 생성하여 지정가능합니다.
- 텍스트 및 바코드 개체들에 대해 TextConverters.SimpleScript 를 사용할 경우 이 스크립트를 이용해 문자열 변환이 자동 지원됩니다.
- 편집기(EditorControl) 에서 단축키(F2)를 통해 ScriptingInstance 의 속성값 확인 및 편집이 가능합니다.

## 10. MarkerFactory와 구현 선택

`MarkerFactory`는 인스턴스를 생성합니다. 장치 초기화, Scanner 소유권 인수나 가공 시작은 수행하지 않습니다. 초기화한 Scanner에 맞는 Marker를 선택하고 Editor의 `RegisterDevices`·Marker `Ready` 흐름에서 같은 Document/View/Scanner/Laser/PowerMeter를 연결합니다.

| 팩토리 / 인스턴스 | 가공 구조 | 용도와 차이 |
|---|---|---|
| CreateVirtual → MarkerVirtual | 물리 RTC 실행 없는 소프트웨어 처리 | 장치 없는 순회·호스트 개발용이며 하드웨어 타이밍 검증은 아닙니다. |
| CreateRtc → MarkerRtc | 레이어·Offset 경계에서 RTC 목록 시작·종료·실행 | 일반 RTC4/5/6 흐름이며 레이어별 ALC 처리를 제공합니다. |
| CreateRtcFast → MarkerRtcFast | 순회를 더 큰 논리 RTC 목록으로 결합 | 제출 비용을 줄이며 MarkerRtc의 레이어별 ALC 설정은 제공하지 않습니다. 버퍼·이벤트 경계를 확인합니다. |
| CreateSyncAxis → MarkerSyncAxis | IRtcSyncAxis의 레이어 Motion Type별 Job과 Laser 목록 결합 | Scanner·Stage 동기 가공용이며 호환 컨트롤러·Job 설정이 필요합니다. |
| CreateAuto(IRtc) | IRtcSyncAxis → SyncAxis, RtcVirtual → Virtual, 그 밖에는 Rtc | RtcFast를 자동 선택하지 않습니다. |

```csharp
using SpiralLab.Sirius3.Marker;

// 생성만 수행합니다. 시작 전에 초기화한 장치를 연결합니다.
IMarker marker = MarkerFactory.CreateRtc(0);
IMarker fastMarker = MarkerFactory.CreateRtcFast(0);
IMarker syncMarker = MarkerFactory.CreateSyncAxis(0);
```

Fast는 제출 방식의 차이이며 물리 스캔이 반드시 빨라진다는 보장은 아닙니다. 구현을 바꿀 때 필요한 레이어별 설정과 컨트롤러 capability를 확인합니다. 일반 RTC Marker를 syncAXIS Job Marker 대신 사용하지 않습니다. 팩토리 선택만으로 Scanner·Stage·Laser 운전 준비가 끝나지는 않습니다.

`Start`가 작업을 받아들였다고 가공이 완료된 것은 아닙니다. `IsBusy`, 오류와 `OnStarted`·`OnEnded`로 수명·완료를 확인합니다. 호스트 이벤트 Handler는 실시간 하드웨어 트리거가 아닙니다.

## 11. 공개 Marker 소스

공개 [editor_marker](../demos/editor_marker/editor_marker.csproj)는 [Form1.cs](../demos/editor_marker/Form1.cs)와 MyMarkerRtc, MyMarkerRtcFast, MyMarkerSyncAxis 사용자 구현 소스를 제공합니다. Demos 네임스페이스의 NuGet 소비 프로젝트입니다. 호스트는 일반 RTC에 MyMarkerRtc, syncAXIS에 MyMarkerSyncAxis, RtcVirtual에 내장 Virtual Marker를 선택합니다. MyMarkerRtcFast 소스도 제공하지만 기본 선택은 아닙니다.

이 소스로 순회, 목록·Job 경계와 Hook을 확인합니다. 수정할 때 초기화, capability 검사, 실패·중단 처리와 장치 소유권을 유지합니다. 복사한 구현은 설치 SDK와 현재 라이브러리 버전에 맞춰 검토합니다.

## 12. Offset 위치와 위치별 데이터

`Offsets`는 Translate.X/Y/Z(mm), AngleZ(도, 양수는 반시계 방향), Scale로 위치를 지정합니다. ExtensionData는 위치별 문자열 정보이며 좌표를 바꾸지 않습니다. Z는 좌표 이동량이며 자동 Stage 이동 요청이 아닙니다. 대기 상태에서 Offset을 준비하고 개체 배치 변환을 중복 적용하지 않습니다.

`TextConverters.Offset`에서 일반 ExtensionData 문자열은 모든 변환 개체에 같은 값을 제공합니다. `MyBarcode|A|MyText|B`는 개체 Name별 값을 지정하고 구분자를 포함한 값은 JSON으로 표현할 수 있습니다. 구분자 이름은 대소문자를 무시하며 JSON 키는 구분합니다. 구분자 방식의 반복 이름은 마지막 쌍을 사용합니다. 파싱·좌표 예제와 변환 순서는 [Offset](OffsetUserManual.koKR.md)을 참고합니다.

관련 문서: [syncAXIS](MarkerSyncaxisUserManual.koKR.md), [Laser 확장](LaserUsermanual.koKR.md), [디지털 I/O](DigitalIOUserManual.koKR.md).

---
2026 Copyright (c) SpiralLAB. All rights reserved.
