# SpiralLab.Sirius3.Config 설정 가이드

> 기준 버전: Sirius3 1.12.3 (공개 Release 기능)

## 1. 역할과 설정 시점

`SpiralLab.Sirius3.Config`는 Core 라이브러리 전체에 적용되는 정적 설정을 제공합니다. 로그, 파일 경로, 좌표 계산, RTC 계측, PowerMap처럼 UI와 관계없이 장치 및 가공 동작에 영향을 주는 값이 이 클래스에 모여 있습니다.

경로와 로그 정책은 가능하면 `Core.Initialize()` 전에 지정하십시오. 가공 중에 값을 바꾸면 이미 생성된 장치나 계산 결과에는 반영되지 않을 수 있습니다.

```csharp
using CoreConfig = SpiralLab.Sirius3.Config;

CoreConfig.LogPath = @"D:\SiriusData\Logs";
CoreConfig.CorrectionPath = @"D:\SiriusData\Correction";
CoreConfig.PowerMapPath = @"D:\SiriusData\PowerMap";
CoreConfig.IsLogToConsole = true;
CoreConfig.LogMaxArchiveDays = 30;

bool initialized = SpiralLab.Sirius3.Core.Initialize(
    minLogLevel: "Information",
    maxLogArchiveDays: CoreConfig.LogMaxArchiveDays);
if (!initialized)
    throw new InvalidOperationException("Sirius3 초기화에 실패했습니다.");
```

`Config`는 정적 설정 클래스이므로 인스턴스를 만들어 값을 보관하는 용도가 아닙니다. `SpiralLab.Sirius3.UI.Config`와 이름이 같으므로 위 예제처럼 별칭을 사용하면 코드의 의미가 분명해집니다.

## 2. 버전 정보

| 항목 | 기본값 또는 반환값 | 설명 |
|---|---:|---|
| `AssemblyName` | `SpiralLab.Sirius3.dll` | Core 어셈블리 파일명 상수 |
| `AssemblyVersion` | 실행 중인 어셈블리 버전 | `Major.Minor.Build` 형식의 읽기 전용 값 |

## 3. 로그

| 설정 | 기본값 | 설명과 적용 시점 |
|---|---:|---|
| `MaxLogItems` | `10,000` | WinForms 로그 컨트롤이 화면에 유지할 최대 항목 수입니다. 파일 로그 보관량과는 다릅니다. |
| `IsLogEnable` | `true` | Sirius3 내부 로그 발생 여부입니다. 문제 분석을 위해 운영 환경에서도 유지하는 편이 좋습니다. |
| `IsLogToConsole` | `true` | 콘솔 출력 여부입니다. GUI 응용 프로그램에서는 필요에 따라 끌 수 있습니다. |
| `LogMaxArchiveDays` | `90`일 | 보관 로그의 최대 유지 기간입니다. `Core.Initialize`의 인수로 같은 정책을 전달하십시오. |
| `MinimumLogLevel` | `Core.Initialize`에서 결정 | 현재 최소 로그 수준을 나타냅니다. 외부에서 직접 설정하는 값이 아닙니다. |
| `OnLogged` | 이벤트 | 새 로그의 `LogLevel`과 메시지를 응용 프로그램으로 전달합니다. |

```csharp
CoreConfig.OnLogged += (level, message) =>
{
    // UI에 반영할 때는 해당 UI 스레드로 전환합니다.
    Console.WriteLine($"[{level}] {message}");
};
```

장치 초기화 실패를 분석할 때는 마지막 Error만 보지 말고 바로 앞의 Information과 Warning도 함께 보존하십시오. 반복되는 프레임·포인트 단위 로그를 직접 추가하면 성능과 파일 크기에 영향을 줄 수 있습니다.

## 4. 파일과 도구 경로

지정하지 않은 경로는 `AppDomain.CurrentDomain.BaseDirectory` 아래의 기본 폴더를 사용합니다. 서비스, Visual Studio, 배포 프로그램은 실행 기준 폴더가 다를 수 있으므로 제품에서는 절대 경로 사용을 권장합니다.

| 설정 | 기본 하위 경로 | 용도 |
|---|---|---|
| `LogPath` | `siriuslogs` | Sirius3 로그 파일 |
| `MeasurementPath` | `measurement` | RTC 고속 계측 및 진단 데이터 |
| `CorrectionPath` | `correction` | 스캐너 보정 파일(`.ct5`, `.ctb`)과 보정 도구의 기준 폴더 |
| `CorreXionProProgramPath` | `correction\CorreXionPro.exe` | SCANLAB CorreXion Pro 실행 파일 |
| `StretchCorreXion5ProgramPath` | `correction\stretchcorreXion5.exe` | Stretch correction 도구 실행 파일 |
| `CorrectionFileCoverterProgramPath` | `correction\CorrectionFileConverter.exe` | 보정 파일 변환 도구 실행 파일 |
| `PowerMapPath` | `powermap` | PowerMap 매핑·검증·보정 데이터 |
| `SyncAxisPath` | `syncaxis` | syncAXIS 설정의 루트 폴더 |
| `SyncAxisViewerProgramPath` | `syncaxis\Tools\syncAXIS_Viewer\syncAXIS_Viewer.exe` | syncAXIS Viewer 실행 파일 |
| `SyncAxisSimulateFilePath` | `siriuslogs` | syncAXIS 시뮬레이션 출력 로그 |
| `RecipePath` | `recipe` | Sirius3 문서 및 레시피 파일 |
| `ScriptPath` | `script` | SimpleScript C# 소스 파일 |

경로를 바꿀 때는 폴더 존재 여부, 서비스 계정의 읽기·쓰기 권한, 외부 도구의 실제 설치 위치를 응용 프로그램 시작 단계에서 확인하십시오. 경로 문자열을 바꾸는 것만으로 외부 도구가 설치되거나 라이선스가 활성화되지는 않습니다.

## 5. 숫자 표시와 경로 생성

| 설정 | 기본값 | 설명과 주의점 |
|---|---:|---|
| `DecimalPrecision` | `3` | UI에서 실수를 표시할 소수 자릿수입니다. 3이면 mm 기준 0.001 mm까지 표시합니다. 내부 계산 정밀도를 낮추는 값은 아닙니다. |
| `MergeDistance` | `0.001` mm | 같은 위치에 가까운 연속 Jump/Mark 명령을 합치는 거리 임계값입니다. 불필요한 중복 이동과 시작·끝점의 과출사를 줄이는 데 사용됩니다. |
| `MinStepDistance` | `0.1` mm | Arc, Spline, Ellipse, Hatch 등을 선분 명령으로 나눌 때 사용하는 최소 길이입니다. |
| `VirtualJumpAndMarkAccScale` | `1.2` | 가상 RTC의 Jump/Mark 가속도 시뮬레이션 배율입니다. 실제 RTC 튜닝값을 바꾸지 않습니다. |

`MergeDistance`가 너무 크면 서로 다른 짧은 이동이 하나로 취급될 수 있습니다. `MinStepDistance`를 너무 작게 하면 곡선 근사는 세밀해지지만 RTC 리스트 명령 수와 준비 시간이 증가합니다. 실제 광학계, 스폿 크기, 속도, RTC 리스트 여유를 함께 고려해 검증하십시오.

## 6. CharacterSet

| 설정 | 기본값 | 설명 |
|---|---:|---|
| `CharacterSetMaxSerialNoUpdateTime` | `50` ms | CharacterSet에서 최대 일련번호를 확인하는 갱신 주기입니다. |

주기를 짧게 하면 UI 갱신은 빨라지지만 확인 작업이 더 자주 실행됩니다. 대량 문서에서는 실제 처리량을 확인한 뒤 조정하십시오.

## 7. Measurement

| 설정 | 기본값 | 설명 |
|---|---:|---|
| `MeasurementPlotMode` | `PlotModes.TimeChart` | 계측 데이터를 표시할 기본 Plot 방식입니다. |
| `MeasurementLaserOnFactor` | `1` | 계측 데이터의 LASER ON 채널에 적용할 변환 배율입니다. |
| `MeasurementPath` | `measurement` | 원본 및 변환된 계측 데이터를 저장할 폴더입니다. |

실제 Sampling 주기와 Channel은 등록한 RTC의 계측 인터페이스와 Measurement UI에서 정합니다. `MeasurementLaserOnFactor`는 표시·변환 배율이며 입력 전압 범위나 장치 보호 한계를 대신하지 않습니다.

## 8. PowerMap

| 설정 | 기본값 | 설명 |
|---|---:|---|
| `PowerMapPreHeatTimeMs` | `10,000` ms | Mapping, Verify, Compensate 전에 레이저를 안정화하는 예열 시간 |
| `PowerMapHoldTimeMs` | `5,000` ms | 각 출력 조건에서 안정된 출력을 유지하는 시간 |
| `PowerMapInRangeThreshold` | `5.0` % | 측정값을 목표 범위 안으로 판단하는 편차 |
| `PowerMapOutOfRangeThreshold` | `20.0` % | 큰 편차 오류로 판단하는 기준 |
| `PowerMapCompensateRetryCounts` | `2`회 | 자동 출력 보정의 최대 재시도 횟수 |

```csharp
CoreConfig.PowerMapPreHeatTimeMs = 15_000;
CoreConfig.PowerMapHoldTimeMs = 3_000;
CoreConfig.PowerMapInRangeThreshold = 3.0;
CoreConfig.PowerMapOutOfRangeThreshold = 15.0;
CoreConfig.PowerMapCompensateRetryCounts = 2;
```

이 값은 장비 안전 한계가 아닙니다. Laser와 PowerMeter의 정격, 냉각 조건, 측정기 응답 시간, 차광 및 인터록 조건을 먼저 따르십시오. 실제 출력을 사용하는 Mapping과 Compensate는 작업 영역이 안전하고 장치가 준비된 상태에서만 실행합니다.

## 9. 초기화와 종료 예제

```csharp
using CoreConfig = SpiralLab.Sirius3.Config;

bool coreInitialized = false;
try
{
    CoreConfig.LogPath = @"D:\SiriusData\Logs";
    CoreConfig.MeasurementPath = @"D:\SiriusData\Measurement";
    CoreConfig.CorrectionPath = @"D:\SiriusData\Correction";
    CoreConfig.RecipePath = @"D:\SiriusData\Recipe";
    CoreConfig.ScriptPath = @"D:\SiriusData\Script";

    coreInitialized = SpiralLab.Sirius3.Core.Initialize("Information", 30);
    if (!coreInitialized)
        throw new InvalidOperationException("Sirius3 초기화 실패");

    // RTC, Laser, DIO, PowerMeter, Marker를 생성하고 사용합니다.
}
finally
{
    // 생성한 장치와 문서를 먼저 Dispose합니다.
    if (coreInitialized)
        SpiralLab.Sirius3.Core.Cleanup();
}
```

`Core.Initialize()`가 성공한 경우에만 대응하는 `Core.Cleanup()`을 호출하십시오. 장치와 문서는 해당 객체의 `Dispose()`를 먼저 호출한 뒤 Core를 종료하는 순서가 안전합니다.

## 10. 변경값 기록 권장사항

Config 값은 프로세스 전체에 영향을 줍니다. 제품에서는 시작 시 적용한 값을 로그에 남기고, 레시피별 값과 전역 Config 값을 구분하십시오. 특히 다음 값은 결과 재현에 중요합니다.

- `MergeDistance`, `MinStepDistance`
- Measurement Plot 및 LASER ON 배율
- PowerMap 시간·허용 편차·재시도 수
- 보정 파일, PowerMap, 레시피 및 스크립트 경로
- 최소 로그 수준과 로그 보관 기간

---
2026 Copyright (c) SpiralLAB. All rights reserved.
