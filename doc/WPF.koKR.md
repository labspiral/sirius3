# Sirius3 WPF 통합 가이드

> API 기준: Sirius3 Core/UI 1.17 (공개 기능)

[English](WPF.md) | [한국어](WPF.koKR.md) | [WinForms](WinForms.koKR.md) | [전체 안내서](README.koKR.md)

## 1. 패키지와 데모 프로젝트

WPF 편집기 컨트롤은 `SpiralLab.Sirius3.UI`에 포함됩니다. 공개 NuGet 패키지 `SpiralLab.Sirius3`, `SpiralLab.Sirius3.UI`, `SpiralLab.Sirius3.Dependencies`에서 호환되는 버전을 설치합니다. 배포 UI Assembly는 WinForms와 WPF를 모두 제공합니다. 네이티브 WPF 편집기와 장치 패널을 제공하며, 1.15.0은 Remote/MCP 비동기 해제와 Mesh 텍스처 렌더링도 지원합니다.

공개 WPF 데모는 Visual Studio 2022 이상과 `net481;net8.0-windows;net9.0-windows`를 사용합니다. 라이브러리는 `net10.0-windows`도 지원하지만 현재 데모 대상에는 포함되지 않습니다.

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFrameworks>net481;net8.0-windows;net9.0-windows</TargetFrameworks>
    <UseWPF>true</UseWPF>
    <LangVersion>latest</LangVersion>
    <ApplicationManifest>app.manifest</ApplicationManifest>
    <AutoGenerateBindingRedirects>true</AutoGenerateBindingRedirects>
    <GenerateBindingRedirectsOutputType>true</GenerateBindingRedirectsOutputType>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="SpiralLab.Sirius3" Version="1.*" />
    <PackageReference Include="SpiralLab.Sirius3.UI" Version="1.*" />
    <PackageReference Include="SpiralLab.Sirius3.Dependencies" Version="1.*" />
  </ItemGroup>
</Project>
```

| 데모 | 진입 파일과 용도 |
|---|---|
| [wpf_editor_ui](../demos/wpf_editor_ui/MainWindow.xaml) | `App.xaml.cs`, `MainWindow.xaml`, `MainWindow.xaml.cs`, `Demos` 네임스페이스의 편집 가능한 통합 편집기 소스 |
| [wpf_editor_multiple](../demos/wpf_editor_multiple/MainWindow.xaml.cs) | 여러 편집 화면과 장치·Document 수명 관리 |
| [wpf_editor_multiple2](../demos/wpf_editor_multiple2/MainWindow.xaml.cs) | `SiriusMultiEditorControl`의 인덱스별 장치 세트 |
| [wpf_editor_entity](../demos/wpf_editor_entity/MainWindow.xaml.cs) | 개체 생성과 WPF Document 편집 흐름 |

Dependencies 패키지의 런타임 파일도 출력과 함께 유지합니다. 설치된 런타임을 선택하고 실행 전 `config*.ini`를 확인합니다. 설정된 장치를 소유하는 Sirius3 프로세스는 한 번에 하나만 실행합니다.

공개 API는 SVG 벡터와 3MF 메쉬 가져오기, `IDocument.ActClip` 클리핑, 전역 `Config.PowerMapInterpolationMode`의 Linear/PCHIP 보간을 제공합니다. 공개 데모는 `Version="1.*"` 참조를 유지하므로 업데이트 후 NuGet을 다시 복원하고 실제 해석된 버전을 확인합니다. 자세한 사용법은 [문서 가져오기](DocumentUserManual.koKR.md), [클리핑](ClippingUserManual.koKR.md), [PowerMap](PowermapUserManual.koKR.md)을 참고합니다.

`wpf_editor_ui` 소스를 복사할 때 `SiriusEditorControl.xaml`/`.xaml.cs`, `SiriusMultiEditorControl.cs`와 공통 `EditorControlSupport.cs`를 함께 유지합니다. 공개 소스는 `Demos` 네임스페이스와 NuGet 참조를 유지하며 라이브러리의 내부 메서드 대신 공개 API를 사용합니다.

## 2. DPI 모드와 레이아웃

실행 파일은 Window를 만들기 전에 Per-Monitor V2를 선언합니다. Windows 10 호환성 선언을 포함한 [데모 manifest](../demos/wpf_editor_ui/app.manifest) 전체를 복사합니다. DPI 설정은 다음과 같습니다.

```xml
<application xmlns="urn:schemas-microsoft-com:asm.v3">
  <windowsSettings>
    <dpiAware xmlns="http://schemas.microsoft.com/SMI/2005/WindowsSettings">true/pm</dpiAware>
    <dpiAwareness xmlns="http://schemas.microsoft.com/SMI/2016/WindowsSettings">PerMonitorV2,system</dpiAwareness>
  </windowsSettings>
</application>
```

WPF 레이아웃은 논리적인 1인치당 96단위의 장치 독립 단위를 사용합니다. `Grid`, 자동 행·열, 정렬과 여백으로 배치를 표현합니다. `AutoScaleMode`, `AutoScaleDimensions`, `ApplicationHighDpiMode`는 WinForms 설정이며 WPF Window의 DPI 설정에 사용하지 않습니다. WinForms 전용 Framework 애플리케이션 설정 섹션을 WPF에 복사하지 않습니다.

필요한 곳에 `UseLayoutRounding="True"`, `SnapsToDevicePixels="True"`를 적용합니다. Sirius3 WPF OpenGL Surface는 XAML 레이아웃과 별도로 Framebuffer 크기를 관리합니다. 모니터 DPI를 보정하려고 ScaleTransform을 추가 적용하지 않습니다. 편집기 화면, 네이티브 대화상자, 모니터 변경 후 처음 표시되는 페이지를 확인합니다.

## 3. 편집기 배치

패키지 컨트롤은 `SpiralLab.Sirius3.UI.WPF` 네임스페이스를 사용합니다.

```xml
<Window x:Class="MyApp.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:sirius="clr-namespace:SpiralLab.Sirius3.UI.WPF;assembly=SpiralLab.Sirius3.UI"
        Title="Sirius3 Editor" Width="1280" Height="900"
        UseLayoutRounding="True" SnapsToDevicePixels="True">
  <Grid>
    <sirius:SiriusEditorControl x:Name="editor" />
  </Grid>
</Window>
```

공개 소스 `wpf_editor_ui`는 `Demos` 네임스페이스를 사용합니다. 소스 복사본을 수정할 때는 데모의 로컬 XAML 네임스페이스를 사용합니다. Sirius3 컨트롤은 필요한 테마 리소스를 자체 초기화하므로 패키지 사용자가 `WPFThemeManager.Initialize()`를 별도로 호출할 필요는 없습니다.

Window와 편집기를 만들기 전에 `Core.Initialize()`를 호출합니다. `InitializeComponent()` 전에 UI Culture를 선택합니다. Core 초기화 성공 후 Window와 장치 해제가 끝나면 `Core.Cleanup()`을 한 번 호출합니다. 공개 `App.xaml.cs`와 `MainWindow.xaml.cs`에서 Application과 Window의 수명 관리 분담을 확인할 수 있습니다.

## 4. 장치와 편집 흐름

`config*.ini`를 읽고 공개 [helper.cs](../demos/helper.cs)의 Factory 흐름으로 장치를 생성하고 초기화합니다. 장비 없이 UI를 확인할 때는 가상 Scanner, Laser와 PowerMeter를 사용합니다. `Loaded`에서는 초기화 여부를 검사해 페이지가 다시 로드되어도 장치 세트를 중복 생성하지 않습니다.

초기화에 성공한 전체 장치 세트를 Window의 Dispatcher에서 등록합니다. 다음 코드의 변수는 초기화를 마친 호스트 소유 장치입니다.

```csharp
editor.RegisterDevices(rtc, laser, powerMeter,
    diExt1, diLaserPort, doExt1, doExt2, doLaserPort, marker);
if (!marker.IsReady)
    throw new InvalidOperationException("Marker is not ready.");
```

등록 시 Document와 View를 연결하고 `Marker.Ready`를 호출합니다. 중복 호출하지 않고 Ready 상태와 로그를 확인합니다. 다중 편집기는 `MaxDeviceCounts`를 설정하고 `RegisterDevices(index, ...)`로 등록한 뒤 `SwitchDevices(index)`를 호출합니다.

편집기, TreeView와 PropertyGrid는 선택 상태와 Document Action을 공유합니다. 개체를 선택한 뒤 속성을 편집합니다. EntityPen은 개별 경로 조건을, EntityLayerPen은 레이어 실행 단계의 조건을 설정합니다. Undo/Redo는 Document Action을 통해 적용합니다.

Scanner UI의 Multi-Beam 메뉴는 지정된 Scanner가 `IRtcMultiBeam`일 때만 표시됩니다. Laser UI는 가상·제조사별 기본 패널이나 등록한 사용자 WPF 패널을 선택합니다. 네이티브 WPF 커스터마이징에는 `UI.Config.OnCreateLaserUIWPF`, `OnCreateScannerUIWPF`, `OnCreateMarkerUIWPF`를 사용합니다. WinForms 이벤트는 반환 컨트롤 형식이 다릅니다.

마우스와 키보드 명령은 포커스 위치에 따라 적용됩니다. TreeView에 포커스가 있으면 방향키는 트리 탐색에 사용합니다. Marker Start 단축키는 설정된 장비를 동작시킬 수 있습니다. [편집기 단축키](EditorUserManual.koKR.md)와 [TreeView 단축키](TreeviewUserManual.koKR.md)를 참고합니다.

## 5. WPF에서 MCP 사용

같은 `SpiralLab.Sirius3.MCP` API가 WPF 편집기 오버로드를 제공합니다. 장치를 등록한 후 서버를 생성합니다. 초기화된 편집기에 대해 비동기 UI 스레드 메서드에서 다음을 실행합니다.

```csharp
var options = new SpiralLab.Sirius3.MCP.MCPServerOptions
{
    HttpEndpoint = new Uri("http://127.0.0.1:3712/mcp")
};
var server = SpiralLab.Sirius3.MCP.MCPFactory.CreateServer(0, "MCP", editor, options);
server.AccessMode = SpiralLab.Sirius3.MCP.MCPAccessMode.All;
editor.MCPServer = server;
if (!await server.Start())
    throw new InvalidOperationException("MCP startup failed.");
```

옵션은 인증 토큰을 생성합니다. 실제 서버 endpoint와 token을 AI 클라이언트에 설정합니다. 시작 전에 서버를 편집기에 지정하므로 실패·종료 경로에서 편집기를 통해 해제할 수 있습니다. 사용 여부는 Remote 라이선스 옵션을 따릅니다. HTTP 클라이언트 설정은 [MCP.koKR.md](MCP.koKR.md)를 참고합니다. 해당 데모 호스트는 WinForms이며 전송과 인증 방식은 WPF에도 적용됩니다.

## 6. 비동기 종료와 소유권

`Unloaded`는 화면 갱신과 타이머를 일시 중지하며 장치를 최종 해제하지 않습니다. [MainWindow.xaml.cs](../demos/wpf_editor_ui/MainWindow.xaml.cs)의 비동기 `Closing` 패턴으로 소유 Window를 종료합니다.

1. 첫 종료를 취소하고 중복 해제 요청을 막습니다.
2. 이벤트 루프로 제어를 돌려준 뒤 Dispatcher에서 `editor.DisposeDevices()`의 완료를 기다립니다.
3. 편집기의 UI/OpenGL 자원을 해제하고 호스트 소유 Document를 한 번 해제합니다.
4. 최종 종료를 허용합니다. Application은 Window 수명이 끝나면 `Core.Cleanup()`을 호출합니다.

1.15.0의 `DisposeDevices()`는 `Task`를 반환합니다. 단일·다중 편집기 모두 등록된 MCP/Remote를 비동기로 해제한 후 장치를 해제합니다. 별도 소유 서버나 Remote는 연결된 편집기나 Marker를 해제하기 전에 `DisposeAsync()`를 직접 호출하고 완료를 기다립니다. `.Wait()`나 `.Result`로 Dispatcher를 막지 않습니다.

WPF 컨트롤은 지정된 Document나 장치를 자동으로 폐기하지 않습니다. 여러 Window가 Document를 공유하면 모든 사용자의 연결을 해제한 뒤 소유 호스트가 폐기합니다. 실패를 로그에 기록하고 나머지 소유 자원도 정리합니다.

## 7. 렌더링과 확인

WinForms와 WPF는 같은 Document/Entity 모델을 사용하며 1.15.0의 Mesh 재질과 확산 텍스처를 지원합니다. OBJ는 참조 MTL·이미지 파일, PLY 텍스처는 UV 데이터와 유효한 이미지 참조가 필요합니다. 이 표현은 시각화용이며 실제 가공은 설정된 경로와 Pen을 사용합니다. [Entity](EntityUserManual.koKR.md)와 [렌더링](ViewUserManual.koKR.md)을 참고합니다.


대표 DPI 배율의 시작 화면과 모니터 이동, TreeView 포커스, 카메라 조작, 가상·제조사 패널 선택, Remote 요청 중 종료를 확인합니다. 컴파일 성공만으로 GPU, 모니터, 라이선스나 장치 동작을 검증할 수는 없습니다.

관련 문서: [통합 편집기](SiriusEditorUserManual.koKR.md), [다중 편집기](SiriusMultiEditorUserManual.koKR.md), [UI Config](Sirius3UIConfigUserManual.koKR.md), [WinForms](WinForms.koKR.md).

## 개체 Grip과 Font Editor

뷰포트에서 Tab/Shift+Tab로 Move·Vertex·Scale을 순환하고 V/Shift+V으로 Grip을
탐색합니다. G는 자석 스냅, F/Ctrl+F는 화면 맞춤이며 편집은 Undo/Redo로 복원합니다.
포커스와 격자 규칙은 [편집기](EditorUserManual.koKR.md)를 참고하십시오. 우클릭 메뉴의
**Font Edit**는 별도의 폰트 파일 편집을 엽니다. [Font Editor](FontEditorUserManual.koKR.md)를 참고하십시오.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
