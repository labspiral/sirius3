# ILaser와 사용자 Laser UI 매뉴얼

> API 기준: Sirius3 Core/UI 1.17.0 (공개 기능)

[English](LaserUsermanual.md) | [한국어](LaserUsermanual.koKR.md) | [전체 안내서](README.koKR.md)

## 1. ILaser와 LaserBase

SpiralLab.Sirius3.Laser.ILaser는 장치 등록·Marker가 사용하는 공통 소스 계약입니다. 식별 정보, 상태, Scanner 연결, capability, 초기화, 즉시 중단·리셋, 목록 기록 수명, 속성 알림과 해제를 제공합니다.

| 멤버·그룹 | 역할 |
|---|---|
| Index, Name, LaserType | 장치 식별과 소스 유형입니다. |
| IsReady, IsBusy, IsWarning, IsError | 실제 소스·드라이버 상태를 보고합니다. |
| MaxPowerWatt, LastPowerWatt | 정격·설정 최대값과 마지막 지령값입니다. 지령값은 PowerMeter 실측값이 아닙니다. |
| Scanner | 지원하는 RTC 동기 출력에 사용하는 Scanner입니다. |
| IsPowerControl, PowerControlMethod, PowerControlDelayTime | 출력 제어 지원·방식·지연을 설정합니다. |
| IsGuideControl | Guide 지원 여부입니다. |
| Initialize, CtlAbort, CtlReset | 통신 초기화, 즉시 중단과 상태 리셋입니다. |
| ListBegin, ListEnd | Marker의 Scanner·Laser 목록 기록 수명에 참여합니다. |
| SyncRoot, PropertyChanged, Dispose | 동기화, 상태 알림과 명시적 정리입니다. |

ILaser를 직접 구현할 수 있습니다. LaserBase 상속은 공통 정보, 동기화된 상태 알림, Scanner 연결·수명을 재사용합니다. 추상 Initialize·CtlAbort·CtlReset은 구현해야 하며 자체 버퍼 전송이 필요하면 목록 기록도 재정의합니다. Core 어댑터는 UI Assembly와 독립적으로 유지합니다.

## 2. 출력·Guide 확장 인터페이스

| 인터페이스 | 멤버·역할 |
|---|---|
| ILaserPowerControl | PowerMap, CtlPower(targetWatt, category), ListPower(targetWatt, category)입니다. |
| ILaserGuideControl | IsGuideOn과 CtlGuide(bool)입니다. |

CtlPower는 즉시 제어이며 ListPower는 활성 Scanner·Laser 목록에 출력 변경을 기록하여 컨트롤러 실행 중 적용합니다. 두 메서드는 W 단위와 선택적 보정 카테고리를 받습니다. 실제 Analog·Digital·Serial·Vendor 전송을 구현하고 수행할 수 없으면 실패를 반환합니다.

PowerMap은 설정·활성화 시 요청 광학 출력에 필요한 소스 지령값을 계산합니다. 카테고리는 측정 테이블이며 주파수를 자동 지정하지 않습니다. 사용자 데모에는 별도 IsLookUpPowerMap gate도 있습니다. LookUp 반환값과 어댑터 제어 방식을 확인합니다. [PowerMap](PowermapUserManual.koKR.md)을 참고합니다.

Guide는 별도 capability입니다. 인터페이스 구현과 IsGuideControl 표시가 일치해야 합니다. UI checkbox는 인터록이나 주 레이저 OFF 확인을 대신하지 않습니다. 장치의 실제 Guide·주 출력 관계를 구현합니다.

## 3. 사용자 소스 구현

공개 [laser_source.cs](../demos/editor_laser_ui/laser_source.cs)는 Demos.MyLaserSource가 LaserBase, ILaserPowerControl, ILaserGuideControl을 상속·구현합니다. UserDefined1, 정격 출력, 선택적 PowerMap, Analog 포트·전압 환산, Guide 상태와 즉시·목록 출력 메서드를 보여줍니다.

이를 어댑터 구조로 사용합니다. 실제 장치는 소프트웨어 flag만으로 Ready를 선언하지 말고 통신·상태 전이를 구현합니다. 출력 비활성 상태로 초기화하고 설정 범위·Scanner capability를 확인하며 전송 실패를 전달합니다. 재현 가능한 로그와 부분 초기화 후에도 반복 가능한 정리를 제공합니다.

초기화 전에 Index, Name, 정격 MaxPowerWatt와 Scanner를 설정합니다. 지원하는 기능에 맞게 구현 내부에서 capability flag를 설정합니다. 어댑터 소유 통신·리소스만 해제하며 연결한 Scanner·공유 PowerMap은 별도 소유자를 유지합니다.

## 4. WinForms 전용 패널 추가

[LaserUIControl.cs](../demos/editor_laser_ui/LaserUIControl.cs), [Designer](../demos/editor_laser_ui/LaserUIControl.Designer.cs)와 resource 파일에 공개 패널 소스가 있습니다. LaserSource로 비소유 ILaser를 받고 속성·상태를 표시하며 capability 인터페이스로 지원 동작을 수행합니다.

Editor의 Laser 대입·RegisterDevices 전에 UI.Config.OnCreateLaserUI를 등록합니다. 형식은 Func<ILaser, System.Windows.Forms.Control>입니다.

데모 호스트 안의 패널 Factory 예제입니다.

```csharp
using SpiralLab.Sirius3.Laser;

System.Windows.Forms.Control CreateLaserPanel(ILaser source)
{
    return new Demos.LaserUIControl { LaserSource = source };
}

SpiralLab.Sirius3.UI.Config.OnCreateLaserUI += CreateLaserPanel;
// Assign/register the initialized source while the handler is attached.
// Detach the same handler when the owning host is closing.
SpiralLab.Sirius3.UI.Config.OnCreateLaserUI -= CreateLaserPanel;
```

WinForms는 하나의 Factory를 사용합니다. 등록한 Handler는 내장 패널 선택을 대체하며 null을 반환해도 자동으로 내장 패널을 선택하지 않습니다. 호스트가 연결할 모든 소스에 적절한 Control을 반환합니다. 데모의 공통 패널은 MyLaserSource로 강제 변환하지 않고 ILaser를 표시합니다.

## 5. 초기화·등록 순서

1. Core를 초기화하고 호스트 config·Factory로 Scanner와 관련 장치를 생성·초기화합니다.
2. 맞는 Index, 정격 출력, Scanner와 검증된 설정으로 사용자 Laser를 생성하고 초기화합니다.
3. Editor에 소스를 연결하기 전에 패널 생성 Handler를 등록합니다.
4. 같은 사용자 소스로 장치 전체를 RegisterDevices에 등록합니다.
5. Marker 준비 상태와 로그를 확인합니다. 별도 Ready 호출도 같은 사용자 소스를 사용해야 합니다.

RegisterDevices는 문서·View·장치를 Marker에 연결합니다. Helper가 만든 뒤 교체한 소스도 호스트 소유이므로 더 이상 사용하지 않으면 해제합니다. 사용자 Laser 등록 후 폐기한 원래 Laser로 Marker를 다시 연결하지 않습니다.

공개 [Form1.cs](../demos/editor_laser_ui/Form1.cs)는 사용자 구현 경계와 Handler 연결을 보여줍니다. 예제의 기본 소스 생성은 실제 정격 출력, 초기화와 일치하는 등록·Ready 참조로 보완해야 하며 장비 설정이 완료된 형태로 간주하지 않습니다.

## 6. 네이티브 WPF UI

WPF의 UI.Config.OnCreateLaserUIWPF는 System.Windows.Controls.Control을 반환합니다. 소스 대입 전에 등록하고 비소유 ILaser에 연결한 WPF 패널을 반환합니다. null이면 내장 WPF 선택을 계속할 수 있습니다. WinForms·WPF Handler를 분리하고 WPF 이벤트에 WinForms Control을 반환하지 않습니다.

Sirius3 Control이 내장 WPF 리소스를 초기화합니다. 사용자 패널은 이벤트·Dispatcher 작업·Timer를 소유하며 연결한 Laser를 자동 소유하지 않습니다. [WPF](WPF.koKR.md)를 참고합니다.

## 7. 상태 갱신·종료

PropertyChanged로 소유 UI 스레드에서 상태를 갱신합니다. UI·Render 스레드의 Driver probing·Blocking I/O를 피하고 값 갱신 재진입을 막으며 실제 Ready·capability에 맞춰 동작을 활성화합니다. 구독 해제를 위해 Handler 참조를 보존합니다.

호스트의 static UI 생성 Handler를 해제하고 패널 작업을 중지합니다. editor.DisposeDevices로 등록한 Remote·MCP·장치 정리를 기다린 뒤 소유 Control·Document를 해제하고 Core를 정리합니다. Laser Dispose는 자체 통신·리소스만 해제하며 호출자 소유 Scanner·UI를 해제하지 않습니다.

## 8. 공개 소스·관련 문서

[editor_laser_ui](../demos/editor_laser_ui/editor_laser_ui.csproj)는 Demos 네임스페이스의 공개 소스와 NuGet 소비 경계를 유지합니다. 수정하려면 구현·Control·Designer·resource를 한 세트로 가져옵니다. 1.16.0의 공통 호스트 API는 WinForms·WPF를 지원합니다.

관련 문서: [가상 Laser](LaserVirtualUserManual.koKR.md), [Pen](PenUserManual.koKR.md), [Marker](MarkerUserManual.koKR.md), [WinForms](WinForms.koKR.md), [디지털 I/O](DigitalIOUserManual.koKR.md).

---
2026 Copyright (c) SpiralLAB. All rights reserved.
