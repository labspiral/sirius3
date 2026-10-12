# Sirius3 WinForms 통합 가이드

> 기준 버전: Sirius3 1.16.0 (공개 Release 기능)

[English](WinForms.md) | [한국어](WinForms.koKR.md) | [WPF](WPF.koKR.md)

## 1. 패키지와 데모 프로젝트

공개 NuGet 패키지 `SpiralLab.Sirius3`, `SpiralLab.Sirius3.UI`, `SpiralLab.Sirius3.Dependencies`를 설치합니다. 1.15.0은 WinForms HiDPI와 Remote/MCP 비동기 해제를 지원합니다. Dependencies 패키지가 출력으로 복사하는 런타임 파일도 함께 배포합니다.

공개 데모는 Visual Studio 2022 이상과 `net481;net8.0-windows;net9.0-windows`를 사용합니다. 라이브러리는 `net10.0-windows`도 지원하지만 현재 공개 데모의 대상에는 포함되지 않습니다.

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFrameworks>net481;net8.0-windows;net9.0-windows</TargetFrameworks>
    <UseWindowsForms>true</UseWindowsForms>
    <LangVersion>latest</LangVersion>
    <ApplicationHighDpiMode>PerMonitorV2</ApplicationHighDpiMode>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="SpiralLab.Sirius3" Version="1.16.0" />
    <PackageReference Include="SpiralLab.Sirius3.UI" Version="1.16.0" />
    <PackageReference Include="SpiralLab.Sirius3.Dependencies" Version="1.16.0" />
  </ItemGroup>
</Project>
```

| 데모 | 진입 파일과 용도 |
|---|---|
| [beginner](../demos/beginner/Form1.cs) | `Program.cs`, `Form1.cs`, `Form1.Designer.cs`: 기본 편집기와 명시적 장치 생성. 실행 전 Scanner Factory를 확인합니다. |
| [editor_ui](../demos/editor_ui/Form1.cs) | 단일 장치용 `Form1`, 다중 장치용 `Form2`, `Demos` 네임스페이스의 편집 가능한 컨트롤 소스 |
| [editor_multiple2](../demos/editor_multiple2/Form1.cs) | 인덱스별 장치 등록과 전환 |
| [editor_viewer](../demos/editor_viewer/Form1.cs) | Document를 공유하는 별도 Viewer 창 |
| [editor_remote_mcp](../demos/editor_remote_mcp/MainForm.cs) | 인증된 HTTP MCP 서버와 비동기 종료 |

패키지 컨트롤의 네임스페이스는 `SpiralLab.Sirius3.UI.WinForms`입니다. `editor_ui`의 소스 예제는 `Demos`를 사용하며 계속 NuGet 패키지를 참조합니다.

1.16.0 패키지는 SVG 벡터와 3MF 메쉬 가져오기, `IDocument.ActClip` 클리핑, 전역 `Config.PowerMapInterpolationMode`의 Linear/PCHIP 보간을 제공합니다. 공개 데모는 `Version="1.*"` 참조를 유지하므로 업데이트 후 NuGet을 다시 복원하고 실제 해석된 버전을 확인합니다. 자세한 사용법은 [문서 가져오기](DocumentUserManual.koKR.md), [클리핑](ClippingUserManual.koKR.md), [PowerMap](PowermapUserManual.koKR.md)을 참고합니다.

`editor_ui` 소스를 복사할 때 단일·멀티 컨트롤의 `.cs`, `.Designer.cs`, `.resx`, 공통 `EditorControlSupport.cs`와 사용한 Resources를 함께 유지합니다. 공개 소스는 `Demos` 네임스페이스와 NuGet 참조를 유지하며 라이브러리의 내부 메서드 대신 공개 API를 사용합니다.

## 2. 폼 생성 전 DPI 설정

.NET 8/9는 `ApplicationHighDpiMode=PerMonitorV2`를 생성된 `ApplicationConfiguration.Initialize()`에서 적용합니다. Form이나 Control을 만들기 전에 호출합니다. 지역화된 컨트롤을 만들기 전에 UI Culture도 선택합니다.

```csharp
[STAThread]
static void Main()
{
#if NET8_0_OR_GREATER
    ApplicationConfiguration.Initialize();
#else
    Application.EnableVisualStyles();
    Application.SetCompatibleTextRenderingDefault(false);
#endif
    if (!SpiralLab.Sirius3.Core.Initialize())
        return;
    try
    {
        using (var form = new MainForm())
            Application.Run(form);
    }
    finally
    {
        SpiralLab.Sirius3.Core.Cleanup();
    }
}
```

이 진입점은 Core 초기화와 정리를 소유합니다. Form은 종료를 허용하기 전에 비동기 장치 해제를 완료해야 합니다. Form에서 Core 초기화나 정리를 중복 호출하지 않습니다.

`net481` 실행 파일의 `App.config`에는 다음 섹션을 추가합니다.

```xml
<configuration>
  <System.Windows.Forms.ApplicationConfigurationSection>
    <add key="DpiAwareness" value="PerMonitorV2" />
  </System.Windows.Forms.ApplicationConfigurationSection>
</configuration>
```

실행 파일 manifest에는 `supportedOS` ID `{8e0f7a12-bfb3-4fe8-b9a5-48fd50a15a9a}`로 Windows 10 호환성을 선언합니다. Framework DPI 설정은 app.config에서 선택하고 manifest에 경쟁하는 DPI 선언을 추가하지 않습니다. 일반적인 서명되지 않은 데모는 `uiAccess="false"`를 사용합니다.

공개 데모에는 [Directory.Build.targets](../demos/Directory.Build.targets), [WinForms.App.net481.config](../demos/WinForms.App.net481.config), [WinForms.app.manifest](../demos/WinForms.app.manifest)가 공통 Framework 설정을 적용합니다. 기존 개별 manifest는 유지합니다. 데모 하나를 다른 디렉터리로 복사한다면 이 설정도 복사하거나 재구성합니다. Framework 전용 설정 섹션을 modern .NET의 config에 넣지 않습니다. [Microsoft Framework HiDPI 설정](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/high-dpi-support-in-windows-forms)을 참고하십시오.

## 3. 메인 폼 디자인과 사용자 레이아웃

메인 폼의 디자인 파일은 96 DPI를 설계 기준으로 사용합니다.

```csharp
this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
```

두 값을 함께 변경합니다. `(6F, 13F)`는 이전 글꼴 기반 설계 기준입니다. 추가 Form과 동적으로 여는 Viewer 창에도 같은 DPI 기준을 적용합니다. 크기를 조절할 편집기 패널에는 `Dock`과 `Anchor`를 지정하고, 사용자 버튼과 입력 영역에는 `TableLayoutPanel`이나 `FlowLayoutPanel`을 사용합니다. 선택 대화상자는 내용에 맞춰 크기를 정하고 크기 조절이 가능한 창에는 사용할 수 있는 최소 크기를 지정합니다.

글꼴 크기는 논리적인 포인트 단위로 유지합니다. Sirius3가 소유한 도구 모음 이미지, 탭, TreeView 행, PropertyGrid 설명 영역과 값 편집기는 DPI에 맞게 처리하므로 화면 배율을 다시 곱하지 않습니다. 사용자 정의 그리기 영역은 현재 DPI로 직접 측정해야 합니다.

프로세스 DPI 모드는 실행 파일이 결정합니다. UI DLL을 참조하는 것만으로 호스트의 DPI 모드가 설정되지는 않습니다. 기존 Font 기반 제품은 설계 기준을 변경하기 전에 자체 레이아웃을 확인합니다.

## 4. Document와 장치 등록

편집기를 만들기 전에 Core를 초기화합니다. `config*.ini`를 읽고 [helper.cs](../demos/helper.cs)의 데모 Factory로 Scanner, Laser, PowerMeter, DIO와 Marker를 생성하고 초기화합니다. `beginner`는 장치를 직접 생성합니다. 장비 없이 UI를 확인할 때는 가상 장치 Factory를 사용합니다.

장치 초기화에 성공한 뒤 소유 UI 스레드에서 전체 장치 세트를 등록합니다. 다음 코드의 변수는 호스트가 소유하며 초기화를 마친 장치 인스턴스입니다.

```csharp
editor.RegisterDevices(rtc, laser, powerMeter,
    diExt1, diLaserPort, doExt1, doExt2, doLaserPort, marker);
if (!marker.IsReady)
    throw new InvalidOperationException("Marker is not ready.");
```

`RegisterDevices`는 Document와 View를 연결하고 `Marker.Ready`를 호출합니다. `Ready`를 반복하기보다 결과 상태와 로그를 확인합니다. `SiriusMultiEditorControl`은 `RegisterDevices(index, ...)`로 각 세트를 등록한 뒤 `SwitchDevices(index)`로 선택합니다.

편집기나 TreeView에서 개체를 선택한 뒤 PropertyGrid를 편집합니다. Document Action을 사용해 Undo/Redo와 형상 재생성을 유지합니다. 공유 Document는 하나의 소유자가 해제하며 다른 Viewer가 사용하는 동안 해제하지 않습니다.

## 5. MCP와 비동기 종료

장치 등록 후 MCP를 생성합니다. `MCPFactory.CreateServer`는 서버를 생성하고 `await server.Start()`가 인증된 HTTP를 시작합니다. `AccessMode`를 명시하고 편집기가 해제할 서버는 `editor.MCPServer`에 지정합니다. 클라이언트 설정은 [MCP 가이드](MCP.koKR.md), Socket·Serial·WebSocket·MQTT 명령은 [원격 프로토콜](RemoteProtocolManual.koKR.md)을 참고합니다. Remote/MCP 사용 여부는 라이선스 옵션을 따릅니다.

1.15.0의 `DisposeDevices()`는 `Task`를 반환합니다. 편집기나 Document를 해제하기 전에 소유 UI 스레드에서 호출하고 완료를 기다립니다. 등록된 MCP 서버와 Remote를 먼저 비동기로 해제한 뒤 나머지 장치를 해제합니다. Control의 일반 `Dispose()`는 장치 해제를 대신하지 않습니다.

연결한 데모의 `FormClosing` 패턴을 사용합니다. 첫 종료를 취소하고 반복 요청을 막은 뒤 이벤트 루프로 제어를 돌려줍니다. 비동기 해제가 끝나면 두 번째 `Close()`를 허용합니다. `.Wait()`나 `.Result`로 UI 스레드를 막지 않습니다. 해제 실패를 로그에 기록하고 호스트가 소유한 다른 자원도 정리합니다.

| 소유 대상 | 해제 방법 |
|---|---|
| 편집기에 등록한 MCP/Remote | `await editor.DisposeDevices()`에 포함 |
| 별도로 소유한 MCP/Remote | 연결된 편집기나 Marker보다 먼저 `DisposeAsync()` 완료 대기 |
| Editor Control | 장치 해제 후 Dispose |
| 호스트 소유 Document | 모든 Viewer에서 연결을 해제한 뒤 한 번만 Dispose |
| Core | UI와 장치 수명이 끝난 후 `Core.Cleanup()` 한 번 호출 |

`await DisposeDevices()`에서 CS4008이 발생하면 컴파일러가 이전 `void` 시그니처를 참조한 것입니다. 복원된 UI 패키지와 실제 Assembly를 확인하고 해당 패키지·버전 캐시를 갱신합니다. 1.15.0을 사용할 때는 `await`를 유지합니다.

## 6. 완성된 애플리케이션 확인

100%, 150%, 200%의 시작 화면, 모니터 간 이동, DPI 변경 후 처음 여는 페이지를 확인합니다. ComboBox, 대화상자 최소 크기, 도구 모음 이미지, TreeView 행, PropertyGrid 설명과 편집기 화면을 확인합니다. 컴파일 성공만으로 DPI 레이아웃이나 GPU 렌더링을 검증할 수는 없습니다. 실행 전에 설정된 장치를 확인하고 장치를 소유하는 Sirius3 프로세스는 한 번에 하나만 실행합니다.

관련 문서: [통합 편집기](SiriusEditorUserManual.koKR.md), [다중 편집기](SiriusMultiEditorUserManual.koKR.md), [UI Config](Sirius3UIConfigUserManual.koKR.md), [WPF](WPF.koKR.md).

---
2026 Copyright (c) SpiralLAB. All rights reserved.
