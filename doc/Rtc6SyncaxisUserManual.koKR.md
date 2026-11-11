# RTC6 syncAXIS(XL-SCAN) 설정 및 사용 가이드

> API 기준: Sirius3 Core/UI 1.17.1 (공개 기능)

[English](Rtc6SyncaxisUserManual.md) | [한국어](Rtc6SyncaxisUserManual.koKR.md) | [전체 안내서](README.koKR.md)

## 1. syncAXIS의 역할

syncAXIS(XL-SCAN)는 RTC6와 Scan Head, ACS Motion Controller 및 XY Stage의 경로를 하나의 Job으로 계획하고 실행합니다. 작은 Field 안의 고속 이동은 Scanner가 담당하고, 넓은 범위의 이동은 Stage가 담당하도록 경로를 분해할 수 있습니다.

syncAXIS는 RTC6 객체만 생성한다고 바로 사용할 수 있는 기능이 아닙니다. 장치 설치, 배선, License, 고객 장비에 맞춘 `syncAXISConfig.xml`, Stage Error Mapping, Scan Head 보정 및 Simulation 검증이 먼저 끝나야 합니다.

## 2. 실행 전 필수 셋업

다음 항목이 준비되지 않았으면 Hardware Mode로 전환하지 마십시오.

### 2.1 Software와 실행 환경

- Windows x64 응용 프로그램을 사용합니다. Sirius3의 `Rtc6SyncAxis`는 x64 Runtime을 전제로 합니다.
- 설치 장비와 일치하는 syncAXIS Software Package, DLL, XML Schema, RTC6 Program File을 사용합니다.
- RTC6 Driver와 Firmware/DLL 세대를 확인합니다. syncAXIS 응용 프로그램에 필요한 RTC6 `.dll`, `.rbf`, `.out`, `.dat`는 같은 syncAXIS Package에서 제공된 조합을 사용해야 합니다.
- `syncAXISConfig.xml`과 이 파일이 참조하는 CT5, Program File, Log 경로에 응용 프로그램 계정이 접근할 수 있어야 합니다.
- Sirius3 License에 syncAXIS 기능이 포함되어야 합니다.

### 2.2 SCANLAB USB Dongle

SCANLAB USB Dongle을 PC의 USB Port에 연결해야 합니다. Dongle에는 사용할 수 있는 syncAXIS Instance 수와 관련 Option이 기록될 수 있습니다. Simulation 초기화도 Dongle을 확인하므로 단순히 Hardware를 움직이지 않는다는 이유로 Dongle을 생략할 수 없습니다.

`InvalidOrMissingDongle` 또는 Instance 수 초과 오류가 발생하면 다음을 확인합니다.

- Dongle이 연결되어 있고 Windows에서 정상 인식되는지
- 현재 PC에서 다른 syncAXIS Process가 Instance를 사용 중인지
- 장비에 필요한 Instance 수와 Dongle Option이 일치하는지
- Sirius3 License의 syncAXIS Option이 활성화되어 있는지

### 2.3 Hardware

- RTC6 PCI Express 또는 지원되는 RTC6 Ethernet Board와 필요한 Option
- excelliSCAN Scan Head와 지정된 Objective, 전원, 냉각 및 Working Distance
- ACS Motion Controller, EtherCAT 구성, Motor Drive 및 XY Stage
- Stage Limit, Reference Sensor, Emergency Stop과 안전 회로
- RTC6와 Scan Head, SL2-100/EtherCAT Converter, Laser 사이의 올바른 Cable과 Pin Assignment
- Laser, 차광, Interlock 및 안전한 가공 시편

Positioning Stage의 Error Mapping은 Scan Head Field 보정보다 먼저 완료하는 것이 좋습니다. Stage 좌표계가 확정되어야 Scan Head 좌표계와 Stage 좌표계 사이의 회전 및 위치 차이를 올바르게 보정할 수 있습니다.

## 3. syncAXISConfig.xml은 장비별 파일입니다

SCANLAB이 제공한 고객용 `syncAXISConfig.xml`도 실제 장비와 대조해 확인해야 합니다. Package에 포함된 Template는 참고용이며 그대로 Hardware를 구동하는 파일이 아닙니다.

반드시 확인할 대표 항목은 다음과 같습니다.

| 구분 | 확인 항목 |
|---|---|
| 공통 경로 | `BaseDirectoryPath`, `ProgramFileDirectory`, CT5 경로, Log 경로 |
| 실행 Mode | `SimulationMode`, 초기 Operation Mode |
| RTC6 | Board Serial Number, 사용 Connector, Program File, Head 구성 |
| ACS | Controller IP Address, `SlecEtherCATNodeID`, Stage Axis X/Y 연결 |
| Stage | 사용 가능 Work Area, 최대 Speed, Acceleration, Jerk, Error Mapping |
| Scanner | 유효 Working Field, Head 및 Objective, CT5 Correction File |
| 안전 감시 | `MonitoringLevel`, `DynamicViolationReaction`과 각 Limit |
| Laser | Signal Level, Pin, Power/Pulse 조건, Laser Timing |

`SlecEtherCATNodeID`와 ACS에서 쓰는 SLEC Unit ID/FOLLOWCH 값을 혼동하지 마십시오. Hardware 초기화 오류의 흔한 원인입니다.

### 3.1 BaseDirectoryPath

`BaseDirectoryPath`는 실제로 syncAXIS 파일을 실행하고 참조하는 작업 폴더를 가리켜야 합니다. `console_syncaxis_setup` 데모의 주석처럼 빌드 출력 폴더를 사용한다면 XML에도 그 실제 출력 경로를 지정합니다.

```xml
<cfg:BaseDirectoryPath>C:\YourApplication\bin</cfg:BaseDirectoryPath>
```

`ProgramFileDirectory`에는 같은 syncAXIS Software Package의 RTC6 `ProgramFiles` 절대 경로를 지정합니다. 폴더를 이동했다면 XML의 상대·절대 경로가 모두 유효한지 다시 확인하십시오.

### 3.2 XML 검증

설치 Package의 해당 Version XML Schema와 syncAXIS Configurator를 사용해 문법, Tag, 단위와 범위를 확인합니다. XML이 Schema 검사를 통과해도 실제 Stage, Scanner, Laser의 정격과 일치한다는 뜻은 아닙니다. 장치 제조사의 사양과 실제 Wiring을 별도로 대조하십시오.

## 4. Sirius3 폴더와 config_syncaxis.ini

공개 데모는 `SpiralLab.Sirius3.Config.SyncAxisPath` 아래에서 XML을 찾습니다. 기본값은 응용 프로그램 Base Directory의 `syncaxis` 폴더입니다.

`demos/config_syncaxis.ini`의 핵심 항목은 다음과 같습니다.

```ini
[RTC0]
TYPE = SyncAxis
CONFIG_XML = syncAXISConfig.xml
```

`EditorHelper.CreateDevices`는 `CONFIG_XML` 값을 읽고 다음과 같이 경로를 조합합니다.

```csharp
string configXmlFilePath = Path.Combine(
    SpiralLab.Sirius3.Config.SyncAxisPath,
    configXmlFileName);

IRtc rtc = ScannerFactory.CreateRtc6SyncAxis(rtcId, configXmlFilePath);
bool success = rtc.Initialize();
```

따라서 기본 배포 구조는 다음과 같습니다.

```text
Application.exe
config_syncaxis.ini
syncaxis/
  syncAXISConfig.xml
  Tools/
    syncAXIS_Viewer/
```

INI는 어떤 XML을 선택할지 알려주고, 실제 Stage·Scanner·Laser 구성과 안전 Limit은 XML이 정의합니다.

## 5. 직접 생성과 초기화

`demos/console_syncaxis_setup/Program.cs`는 `EditorHelper`를 사용하지 않고 `Rtc6SyncAxis`를 직접 만드는 예입니다.

이 예를 실행하기 전에 XML에 `SimulationMode=true`를 지정합니다. Initialize 이후의 모드 변경으로 최초 하드웨어 연결을 되돌릴 수는 없습니다.

```csharp
using System;
using System.IO;
using SpiralLab.Sirius3.Scanner;
using SpiralLab.Sirius3.Scanner.Rtc;

bool coreInitialized = false;
Rtc6SyncAxis rtc = null;

try
{
    coreInitialized = SpiralLab.Sirius3.Core.Initialize();
    if (!coreInitialized)
        throw new InvalidOperationException("Sirius3 초기화 실패");

    if (!SpiralLab.Sirius3.Core.IsRunningPlatform64)
        throw new PlatformNotSupportedException("syncAXIS는 x64 Runtime이 필요합니다.");

    string xml = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        "syncaxis",
        "syncAXISConfig.xml");

    if (!File.Exists(xml))
        throw new FileNotFoundException("syncAXIS 설정 파일을 찾을 수 없습니다.", xml);

    rtc = ScannerFactory.CreateRtc6SyncAxis(0, xml);
    if (!rtc.Initialize())
        throw new InvalidOperationException("syncAXIS 초기화 실패");

    // Initialize 전에 XML에 SimulationMode=true를 지정해야 합니다.
    if (!rtc.CtlSimulationMode(true))
        throw new InvalidOperationException("Simulation Mode 전환 실패");
}
finally
{
    rtc?.Dispose();
    if (coreInitialized)
        SpiralLab.Sirius3.Core.Cleanup();
}
```

`Initialize()`는 XML을 읽고 Native Configuration Handle을 만들며 Callback과 내부 상태를 준비합니다. 성공 반환은 XML을 읽고 Software Instance를 만들었다는 뜻입니다. Hardware 안전, Calibration 품질 또는 실제 가공 완료를 증명하지 않습니다.

## 6. Simulation을 먼저 실행합니다

처음 사용하는 XML이나 Job은 반드시 `SimulationMode = true` 상태에서 실행합니다. SCANLAB Installation Manual도 실제 Laser, Scan Head, Stage를 사용하기 전에 모든 경로를 Simulation하고 Limit 위반을 확인하도록 요구합니다.

### 6.1 MotionTypes

| 값 | 동작 |
|---|---|
| `MotionTypes.ScannerOnly` | Stage를 고정하고 Scanner만 사용 |
| `MotionTypes.StageOnly` | Scanner를 고정하고 Stage만 사용 |
| `MotionTypes.StageAndScanner` | 경로를 Scanner와 Stage에 분배 |

```csharp
bool success = rtc.ListBegin(MotionTypes.StageAndScanner);
success &= rtc.ListJumpTo(new DVec2(-20, 20));
success &= rtc.ListMarkTo(new DVec2(20, 20));
success &= rtc.ListMarkTo(new DVec2(20, -20));
success &= rtc.ListMarkTo(new DVec2(-20, -20));
success &= rtc.ListMarkTo(new DVec2(-20, 20));
success &= rtc.ListJumpTo(DVec2.Zero);
success &= rtc.ListEnd();
if (success)
    success = rtc.ListExecute(false);
```

모든 반환값을 호출 직후 확인하십시오. 중간 List 명령이 실패했으면 불완전한 List를 실행하지 않습니다.

### 6.2 Simulation 결과 확인

`console_syncaxis_setup`은 다음 정보를 제공합니다.

- `JobHistory`의 실행 결과와 실행 시간
- Scanner 사용률, 최대 Position·Velocity·Acceleration
- Stage 사용률, 최대 Position·Velocity·Acceleration·Jerk
- `Config.SyncAxisSimulateFilePath`에 생성된 Simulation 파일
- `Config.SyncAxisViewerProgramPath`의 syncAXIS Viewer로 결과 열기

Viewer에서 Scanner/Stage Position, 속도와 동적 Limit 위반을 확인합니다. Position 또는 Dynamic Violation이 하나라도 남아 있으면 XML이나 Job을 수정하고 Simulation을 반복하십시오.

<a id="syncaxis-setup"></a>

### 6.3 ScannerControl의 syncAXIS 셋업

Sirius3 1.17.1은 WinForms와 WPF의 Scanner 탭에서 동일한 여섯 셋업 단계를 제공합니다. 호스트에서 `IRtcSyncAxis` 스캐너와 해당 `MarkerSyncAxis`를 초기화·등록하고 에디터의 `IDocument`를 연결해야 합니다. 해당 스캐너를 할당하면 셋업 탭이 표시됩니다. 독립 `ScannerControl`은 `Document`와 `Marker`도 지정해야 하며, 탭 표시만으로 장치를 초기화하거나 소유하지 않습니다.

상단 버튼은 **현재 시뮬레이션 또는 하드웨어 모드**를 표시하고 `IsSimulationMode`로 전환합니다. 하드웨어 전환에는 확인이 필요합니다. **Initialize**는 유휴 스캐너를 현재 XML로 명시적으로 재초기화하며 파일에 저장된 모드도 다시 적용합니다. Run은 스캐너를 자동 초기화하지 않습니다. XML의 작업 디렉터리는 존재하는 절대 경로여야 하며 대응하는 프로그램·보정 파일과 쓰기 가능한 시뮬레이션 출력 폴더에 접근할 수 있어야 합니다.

#### 생성·편집·실행과 반복

1. **Create** 확인 전에 현재 레시피를 저장합니다. 생성은 `IDocument.ActNew()`로 모든 Page와 펜을 초기화하고 활성 Page의 빈 기본 레이어를 교체하여 선택한 패턴만 만듭니다. 생산 레시피에 덧붙이지 않으며 Undo로 ActNew 이전 문서를 복구할 수 없습니다.
2. 생성된 개체와 펜을 EditorControl·PropertyGrid에서 검토하고 편집합니다. 크기와 관련 입력은 단계별로 제공됩니다. 격자 크기는 마커 직경이고 **Interval**은 mm 단위의 중심 간격으로 직경보다 커야 합니다. 채운 원에는 해치 간격 0.1 mm, 경계 제외 0.05 mm인 선 해치가 포함됩니다.
3. 해당 단계의 **Run**을 누릅니다. 시뮬레이션 작업이 성공적으로 완료되면 syncAXIS Viewer가 자동으로 열립니다. 별도 Viewer 버튼은 없으며 중지·실패한 작업에서는 Viewer가 열리지 않습니다. 하드웨어 모드의 Run은 빨간 글씨로 표시되고 레이저 출력이나 스테이지·스캐너 이동 전에 확인을 받습니다.
4. 시뮬레이션 결과를 검토한 뒤 승인된 실장비 절차에서만 가공 결과를 측정합니다. **Stop**은 현재 작업을 중단합니다. 오류 원인을 제거한 다음 유휴 상태에서 **Reset**을 사용합니다. 하단에는 소요 시간, 동작 상태, Ready, Busy, Error 및 **Errors** 오류 목록이 표시됩니다.
5. 단계 목표를 만족할 때까지 반복합니다. 문서 형상이나 펜을 편집하면 이전 시뮬레이션 검증이 무효가 되므로 편집한 패턴을 다시 시뮬레이션합니다. 생성 입력값을 바꾸었다면 Create가 다시 필요하며 문서도 다시 초기화됩니다. XML 변경이나 신규 CT5 선택 후에는 Initialize로 재적용하고 결과를 확인합니다. 하드웨어 Run 전에는 검토한 문서와 설정을 그대로 유지합니다.

#### 여섯 단계와 목표

| 단계 | 동작 모드 | 입력값과 확인할 결과 |
|---|---|---|
| 1. 첫 시험 가공 | StageAndScanner | 기본 변 길이 100 mm인 네 선의 사각형입니다. 위치·크기와 전체 경로를 확인합니다. |
| 2. 레이저 지연 | ScannerOnly | 편집 가능한 H 하나를 마커 오프셋으로 반복합니다. 행·열별 지연을 조정해 획 끝을 정렬합니다. 자세한 설정은 아래에 설명합니다. |
| 3. 스캐너 보정 격자 | ScannerOnly | 기본 직경 0.3 mm, 간격 9 mm, 11 × 11입니다. 행·열은 3~99의 홀수이며 증감은 2입니다. 채운 원·원·동심원·십자·점 표식을 선택하고 측정 후 외부에서 신규 CT5를 생성·선택합니다. |
| 4. 정적 정밀도 | StageOnly·ScannerOnly 개별 생성 | 스테이지 십자와 스캐너 원을 별도 Create 버튼으로 만듭니다. 기본 직경 0.3 mm, 간격 2 mm, 15 × 15이며 행·열은 3~31의 홀수입니다. 방향 식별을 위해 한쪽 모서리 표식이 생략됩니다. 같은 좌표에서 기준 표식의 상대 위치를 비교합니다. |
| 5. 동기화 | StageAndScanner | 짧은 선 44개의 간략한 편집용 배치이며 기본 명목 크기는 100입니다. X/Y 범위는 약 −45.833~+45.833 mm입니다. 아래의 제한 사항을 확인합니다. |
| 6. 복합 정밀도 | StageOnly·ScannerOnly·StageAndScanner | 기준 격자 두 종류와 복합 격자를 각각 생성하며 한 번에 하나만 준비합니다. 기본 직경 0.3 mm, 간격 9 mm, 11 × 11이고 행·열은 3~31입니다. 작업 영역에서 정적 기준과 복합 가공 결과를 비교합니다. |

스캐너 보정 격자에 짝수 개수를 입력하면 바로 아래 홀수로 조정하며 WPF의 위·아래 방향키도 2씩 변경합니다. 정밀도 단계의 각 Create는 현재 문서를 교체하므로 기준 레시피를 보존하려면 각각 저장합니다. 미리보기는 가공 형상을 나타내고 옆의 설명은 어떤 축을 움직이는지 구분합니다.

#### 레이저 지연의 행·열과 레이어 펜

Create는 ScannerOnly H 하나를 만들고 선택한 행·열·간격으로 `IMarker.Offsets`를 즉시 설정합니다. 다른 단계의 Create는 이전 위치에서 새 패턴이 반복되지 않도록 오프셋을 비웁니다. H 반높이 기본값은 1 mm, 배열은 11 × 11, 간격은 2.5 mm, 스캐너 속도는 500 mm/s입니다.

- X는 오른쪽으로 증가합니다. `EntityLayerPen.SyncAxisLaserSwitchOffsetTime`은 µs 단위의 시작값이며 열 증가량을 더합니다. 기본 시작값 −40 µs, 열당 증가량 +5 µs입니다.
- Y는 위쪽으로 증가합니다. `EntityLayerPen.SyncAxisLaserPreTriggerTime`은 µs 단위의 시작값이며 행 증가량을 더합니다. 기본 시작값 −10 µs, 행당 증가량 +2 µs입니다.
- 좌하단 H에는 두 시작값이 적용됩니다. `EntityLayerPen.SyncAxisLaserDelaySpeed`는 mm/s 단위의 시작 스캐너 속도입니다.

탐색은 syncAXIS LayerFirst 처리의 `IMarker.OnBeforeOffset`·`OnAfterOffset`을 사용하며 이후 스캐너 궤적을 복원합니다. Run을 반복해도 이전 증가량이 누적되지 않고 문서의 레이어 펜 시작값에서 다시 시작합니다. 이 값을 편집해 탐색 범위를 조정하고 다시 시뮬레이션합니다. [레이어 펜](LayerPenUserManual.koKR.md)과 [오프셋 콜백](OffsetUserManual.koKR.md#offset-callbacks)을 참고합니다.

#### 보정 파일과 동기화 검증의 범위

이 화면은 CT5를 계산하지 않습니다. ScannerOnly 격자를 생성·가공·측정하고 외부 보정 소프트웨어에서 신규 `.ct5`를 만듭니다. 설정된 스캔 헤드와 **Select ct5**로 파일을 선택한 뒤 **Initialize**를 눌러 XML 변경을 적용합니다. 선택은 외부 파일을 보존하면서 XML의 보정 참조를 바꾸며, 실행 중인 RTC 테이블을 즉시 로드하지 않습니다. 적용 후 레이저 지연과 잔류 오차를 다시 확인합니다. [2D 보정](Correction2dUserManual.koKR.md)을 참고합니다.

동기화의 패턴 크기는 배치 비율을 정하는 명목값입니다. 반폭은 `크기 × 11/24`이며 `크기/2`가 아니므로 100을 입력하면 전체 폭은 약 91.667 mm입니다. 공급사 설치 절차는 100 mm 크기를 지정하지 않습니다. 전체 CHECK_SYSTEMDELAYS 검사는 네 방향 각각 선 쌍 21개, 스테이지 가속 이동과 반전을 사용합니다. UI의 선 44개는 전체 측정 절차를 수행하거나 동기화 정밀도를 입증하지 않습니다. 시스템 지연을 측정할 때는 장비의 전체 절차를 사용합니다.

[MCP 안내서](MCP.koKR.md#syncaxis-setup)는 AI 클라이언트에서 같은 패턴을 준비하는 방법을 설명합니다. 준비·시뮬레이션 검증 상태는 등록 장치의 세션에 속하며 UI에서 수동으로 만든 도면이 해당 MCP 세션의 준비 패턴으로 자동 등록되지는 않습니다.

## 7. Hardware Mode 전환 조건

`rtc.CtlSimulationMode(false)` 호출은 단순한 Mode 변경 요청입니다. 이 호출이 성공했다고 Hardware가 안전하게 준비된 것은 아닙니다. 다음 조건을 모두 확인한 뒤에만 전환합니다.

1. 동일한 XML과 Job의 Simulation 결과에 Position 및 Dynamic Violation이 없습니다.
2. RTC6, Scan Head, ACS Controller, Stage, Laser의 전원과 통신이 정상입니다.
3. XML의 ACS IP, RTC6 Serial Number, `SlecEtherCATNodeID`, Stage Axis, Program File과 CT5 경로를 확인했습니다.
4. Stage Reference와 Error Mapping이 완료되었습니다.
5. 안전 영역에 사람이 없고, 이동 범위에 장애물이 없습니다.
6. Laser 차광, Interlock, Emergency Stop과 보호 장비가 준비되었습니다.
7. 낮은 속도와 제한된 출력으로 단계별 Hardware 확인 절차를 수행할 준비가 되어 있습니다.

Hardware 초기화는 먼저 통신과 Reference Run을 확인하고, 그 다음 Laser 신호, ScannerOnly, StageOnly, StageAndScanner 순서로 범위를 넓히는 것이 안전합니다. 처음부터 넓은 복합 경로를 실행하지 마십시오.

## 8. console_syncaxis_setup 메뉴와 검증 항목

| 키 | 데모 동작 | 사용 목적 |
|---|---|---|
| `S` | Busy, NoError 및 내부 Error 조회 | 초기화와 Job 상태 확인 |
| `R` | `CtlReset()` | 오류 원인을 제거한 뒤 상태 Reset |
| `J` | `CtlSimulationMode(true)` | Simulation Mode 전환 |
| `H` | `CtlSimulationMode(false)` | 승인된 셋업에서만 Hardware Mode 전환 |
| `F` / `U` | Follow / Unfollow | Stage 추종 방식 확인 |
| `V` | syncAXIS Viewer 실행 | Simulation 결과 확인 |
| `C` | 마지막 Job Characteristic 출력 | Scanner/Stage 동특성 Limit 확인 |
| `O` | Scanner와 Stage를 원점으로 이동 | Hardware Mode에서는 실제 축 이동 |
| `F1`~`F3` | 사각형: ScannerOnly / StageOnly / 복합 | Motion Type별 경로 검증 |
| `F4`~`F6` | 원: ScannerOnly / StageOnly / 복합 | Arc와 Motion Type 검증 |
| `F7` | Scanner Calibration Pattern | Scan Head Field 보정용 Pattern |
| `F8` | Laser Delay Pattern | `LaserSwitchOffsetTime`, `LaserPreTriggerTime` 최적화 |
| `F9` | Scanner/Stage Calibration 비교 | Circle과 Cross Grid의 정적 정확도 확인 |
| `F10` | System Delay Pattern | Scanner와 Stage 동기 지연 확인 |
| `Esc` | `CtlAbort()` | 긴급 중단 요청. 장비의 독립 Emergency Stop을 대신하지 않음 |

`O`, `F1`~`F10`, `H`는 Hardware Mode에서 실제 Stage, Scanner와 Laser를 제어할 수 있습니다. 코드를 읽거나 UI에 키가 표시된다는 이유만으로 실행하지 마십시오.

## 9. 권장 최적화 순서

SCANLAB Installation Manual의 흐름과 데모의 F7~F10 기능을 연결하면 다음 순서가 됩니다.

1. Stage 제조사의 절차로 Stage Error Mapping을 완료합니다.
2. Simulation에서 대표 `StageAndScanner` Job의 Position·Velocity·Acceleration·Jerk를 확인합니다.
3. 낮은 조건으로 Laser Delay Pattern을 실행해 `LaserSwitchOffsetTime`과 `LaserPreTriggerTime`의 시작값을 찾습니다.
4. `ScannerOnly` Calibration Grid를 가공하고 측정해 최적화된 CT5를 만듭니다. Stage는 원점에 고정합니다.
5. 보정 후 Mirror Positioning이 바뀌므로 Laser Delay를 다시 확인합니다.
6. 같은 시편에 `StageOnly` Cross Grid와 `ScannerOnly` Circle Grid를 가공해 회전, Scale과 위치 일치를 확인합니다.
7. 네 방향의 System Delay Pattern으로 Scanner와 Stage가 시간상 동기화되는지 확인합니다.
8. 마지막으로 넓은 범위의 Combined Motion Accuracy를 측정하고 승인 기준을 기록합니다.

보정용 Pattern은 높은 생산 속도보다 정적 정확도를 우선해 낮은 속도로 가공합니다. 보정 파일은 기존 정상 파일을 덮어쓰지 말고 별도 이름으로 생성·검증한 뒤 설치하십시오.

## 10. Status와 Error 처리

```csharp
if (rtc.CtlGetStatus(RtcStatus.Busy))
    Console.WriteLine("syncAXIS is busy");

if (!rtc.CtlGetStatus(RtcStatus.NoError) &&
    rtc.CtlGetInternalErrMsg(out var errors))
{
    foreach (var error in errors)
        Console.WriteLine($"[{error.Key}] {error.Value}");
}
```

syncAXIS 함수는 호출별 Return Code를 제공합니다. 여러 Return Code를 Bitwise OR로 합쳐 진단하면 최초 실패 함수를 잃을 수 있으므로 각 호출 직후 검사하고 함수명, Job ID, Mode와 원래 Code를 기록하십시오.

초기화가 실패하면 다음 순서로 확인합니다.

1. XML 파일 존재 여부와 Schema 오류
2. SCANLAB USB Dongle 및 Instance 수
3. Sirius3 syncAXIS License Option
4. `BaseDirectoryPath`, `ProgramFileDirectory`, CT5 및 Log 경로
5. ACS IP와 EtherCAT 통신
6. RTC6 Serial Number, Program File, Firmware와 전원
7. `SlecEtherCATNodeID`와 Stage Axis 연결
8. Scan Head, Stage 및 Laser Cable
9. syncAXIS Log와 `CtlGetInternalErrMsg`의 원래 오류

오류 원인을 고치지 않은 채 `CtlReset()`만 반복하지 마십시오.

## 11. BandWidth와 Motion Mode

- `BandWidth`: Scanner와 Stage 사이의 경로 분배를 결정하는 Trajectory Parameter입니다. 기계 축의 동특성과 Scanner Field를 함께 고려해야 하며 임의의 큰 값이 더 좋은 결과를 보장하지 않습니다.
- `MotionModes.Follow`: Stage가 Scanner 경로를 추종하도록 구성합니다.
- `MotionModes.Unfollow`: 추종을 사용하지 않는 동작입니다.
- `Trajectory`: Jump/Mark Speed, Stage/Scanner Limit, Laser Timing 등 Job 계획에 필요한 조건을 포함합니다.

값을 바꾼 뒤에는 같은 Job을 다시 Simulation하고 Job Characteristic과 Viewer 결과를 비교하십시오.

## 12. 참고 자료

- `demos/console_syncaxis_setup/Program.cs`: 초기화, Mode 전환, Motion Type별 사각형·원, Calibration과 Delay Pattern, Status 및 Viewer
- `demos/editor_syncaxis/Form1.cs`: `config_syncaxis.ini`를 사용한 Editor 기반 장치 등록
- `doc/SCANLAB/syncAXIS_V1.8.0_Installation_en-US.pdf`: Hardware 설치, XML 점검, Simulation 우선 초기 셋업, Hardware 확인, 보정 및 동기 검증
- `doc/SCANLAB/syncAXIS_V1.8.0_API_en-US.pdf`: `slsc_cfg_*`, `slsc_list_*`, `slsc_ctrl_*`, Job, Buffer, Callback와 Error 계약

설치 장비의 syncAXIS Software Package, DLL, Firmware와 Manual Version이 다르면 배포된 Version의 문서와 Header를 우선하십시오.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
