# Sirius3
SCANLAB 제어, 장치 연동, 지오메트리 처리, OpenGL 시각화, 문서 편집, MCP 서버를 이용한 AI 프롬프트, 시뮬레이션 및 가공 실행을 통합한 Windows/.NET 기반 정밀 레이저 가공 플랫폼

언어: [English](README.md) · [한국어](README.koKR.md) · [简体中文](README.zhCN.md) · [日本語](README.jaJP.md) · [Deutsch](README.deDE.md)

![sirius3_logo](https://spirallab.co.kr/sirius3/sirius3_logo.png)

---

## 하이라이트
![sirius3_logo1](https://spirallab.co.kr/sirius3/sirius3_logo1.png)
![sirius3_editor](https://spirallab.co.kr/sirius3/sirius3_editor.png)

- SCANLAB RTC 제어기
   - RTC4 / RTC4e / RTC5 / RTC6 / RTC6e
   - XL-SCAN (syncAXIS 기반의 RTC6 + ACS 조합)
- 계측 및 프로파일링
   - 스캐너 운동 경로와 시그널 출력 로그를 이용한 그래프 출력 지원
   - 경로 시뮬레이션을 통한 시각화 지원
- 강력한 가공 옵션
   - 가변 폴리곤, 가변 점프 지연시간 설정 지원
   - 2nd 헤드, 3D 지원
   - MoF (Marking on the Fly) 와 확장 MoF (Fly extension) 지원
   - Sky Writing: RTC5 Mode 1–3, RTC6 Mode 1–4 지원(해당 소프트웨어·펌웨어 필요), RTC4 미지원
   - SCANAhead 를 이용한 자동 지연값(Auto delays) 지원
   - 멀티빔 (1개의 레이저 소스 + 2개의 AOM + 2개의 스캔헤드) 제어 지원
- ALC(Automatic Laser Control)
   - 벡터 정의형
      - 램프(Ramp)
   - 속도 의존형
      - 지령 속도
      - 실제 속도
   - 엔코더 의존형
      - 엔코더 속도
   - 위치 의존형
      - 거리 및 스케일 값 기반 테이블
   - 또한 SCANAhead, Encoder Speed Addition, Inverse Speed Correction, Backward Transformation, SDC+Skywriting 조합 사용 가능
- 스캐너 필드 보정
   - 2D 보정
   - 3D 보정 (기울어짐, 포커스, a,b,c 계수 및 스트레치 보정 지원)
- 레이저 파워 제어
   - 주파수, 펄스폭, 아날로그, 디지털 출력
   - 레이저 소스 벤더 지원: AdvancedOptoWave, Coherent, IPG, JPT, Photonics Industry, Spectra Physics 등
- 파워메터와 파워맵
   - Coherent (PowerMax), Thorlabs (OPM 기반), Ophir (StarLab 기반), Gentec-EO
   - 파워맵 기반의 출력 보상 지원
- 렌더링 및 지오메트리 처리
   - 한 개의 직교 카메라와 다섯 개의 원근 카메라를 제공하는 OpenGL 3.3+ 2D/3D 렌더러
   - 점, 선, 라인 스트립 및 삼각형 히트 테스트용 AABB 가속 구조
   - 폐곡선/열린 경로 진단을 포함하는 토폴로지 기반 3D 메쉬 슬라이서
   - 외곽선, 중첩 영역 및 연결된 바코드 셀을 처리하는 winding 기반 다중 해치
- 엔티티, 텍스트 및 바코드
   - 점, 선, 호, 타원, 폴리라인, 삼각형, 사각형, 십자, 나선, 트레팬, 리사주, 스플라인
   - 큐브, 구, 실린더, 원뿔, 메쉬, 레이어, 그룹, 블록 및 블록 삽입
   - Text, SiriusText, ImageText, Circular Text, 링크 텍스트 및 ZPL 렌더링 엔티티
   - 외곽선, 해치 및 Dots 셀 가공을 지원하는 1D, QR, DataMatrix, PDF417 및 Aztec 바코드
- 외부 파일 및 상호 운용성
   - Sirius3 (`.sirius3`): 문서 저장·열기·가져오기.
   - DXF/DWG: 2D CAD 도면(DWG는 ODA File Converter 필요).
   - SVG: 벡터 윤곽, 채움 및 편집 가능한 단일 구간 텍스트(복잡한 텍스트는 경로 변환 필요).
   - HPGL/PLT: 플로터 벡터 경로.
   - Gerber/Excellon: PCB 패턴과 드릴 데이터.
   - PNG/BMP/GIF/JPG(JPEG)/TIF(TIFF): 래스터 이미지.
   - PDF: 페이지를 래스터 이미지로 가져오기(암호 보호 미지원).
   - STL/OBJ/PLY/STP(STEP)/3MF: 3D 메쉬 가져오기·슬라이싱.
   - G-code (`.gcode`, `.ngc`): CNC·레이저 가공 경로.
   - CXF/LFF/EO FNT/DOT: SiriusText용 외부 폰트.
- 원격 통신 및 동적 데이터
   - 마커 제어와 데이터 접근을 위한 TCP/IP, Serial(RS-232), WebSocket 및 MQTT
   - AI 기반 편집기 등록, 문서·엔티티 편집, 시뮬레이션 및 등록 장치 제어를 지원하는 내장 HTTP MCP 서버
   - 텍스트와 바코드 데이터용 이벤트, 파일, 오프셋, 링크 엔티티 및 C# 스크립트 변환
- 문서, 편집기 및 시뮬레이션
   - 레이어, 펜, 그룹, 블록 및 개수 설정형 Undo/Redo를 갖는 4개 문서 페이지
   - WinForms와 WPF `SiriusEditorControl` / `SiriusMultiEditorControl`을 Debug 및 Release에서 모두 지원하며, 하나의 문서를 복수 뷰에 렌더링하는 기능
   - 영어, 한국어, 중국어 간체, 일본어 및 독일어 UI 리소스
   - [폰트 편집기](SpiralLab.Sirius3.UI/FontEditor/README.koKR.md): WinForms/WPF **Font Edit** 메뉴에서 독립적인 CXF/LFF/EO FNT/DOT 글리프 편집을 제공합니다. mm 벡터, 정수 DOT 셀, BBox/메트릭, Undo/Redo, DXF 가져오기, 문자열 미리보기와 설치 TTF 변환을 지원하며 Debug/Release 빌드에 항상 포함됩니다.
   - 화면 고정 크기 마커, 빔 효과 및 선택적 파편 효과를 제공하는 실시간 가공 경로 시각화
   - 카메라 및 검사 작업용 격자 기반 스티치 이미지 시각화
- 오픈 아키텍쳐
   - 확장 가능한 편집기, 엔티티, 마커, 스캐너, 레이저, 파워미터, 파워맵 및 원격 통신 인터페이스
   - 개발 및 시뮬레이션용 가상 스캐너, 레이저, 파워미터 및 마커 구현

## 주요 변경사항
|                              |                SIRIUS3                   |              SIRIUS2                  |
|:-----------------------------|:-----------------------------------------|:--------------------------------------|
| 다중 페이지                   |4개의 페이지 교차 편집 지원                 |단일 페이지 편집                        |
| 카메라                        |6개(2D + 5개 3D)의 카메라                  |단일 3D 카메라                          |
| 렌더링 속도                   |GPU 가속 OpenGL 쉐이더 엔진                |내장 쉐이더 엔진                         |
| 렌더링 모드                   |Model, PerVertex, PerNormal, ZHeightMap, Texture, TexturePerVertex          |없음                                   |
| 선택 기능                     |점/선/삼각형용 AABB 가속                   |저속                                   |
| 해치                         |winding 기반 다중 해치                     |단일 해치                               |
| 3D 메쉬 슬라이서              |STL, OBJ, PLY, STEP, 3MF 슬라이서 내장      |없음                                   |
| Gerber / Excellon            |내용 판별 기반 가져오기                     |없음                                   |
| 외부 폰트 파일                |CXF, LFF, EO FNT, DOT 파일 포맷               |커스텀 CXF, LFF 파일 포맷만 지원         |
| 펜                           |Entity 와 Layer 용 펜 속성 분리            |Entity 단일펜                           |
| MCP                          |지원                                      |없음                                   |
| 라이브러리 업데이트            |Nuget 패키지 매니저 지원                    |수동                                   |
                                                                                                              
![sirius3_hatch](https://spirallab.co.kr/sirius3/sirius3_hatch.png)
![sirius3_pod](https://spirallab.co.kr/sirius3/sirius3_pod.png)
![sirius3_slicer](https://spirallab.co.kr/sirius3/sirius3_slicer.png)
![sirius3_syncaxis](https://spirallab.co.kr/sirius3/sirius3_syncaxis.png)

## 패키지 / DLLs
- `SpiralLab.Sirius3.Dependencies` — SCANLAB RTC4/5/6, syncAXIS 런타임, 폰트, 샘플 파일들
- `SpiralLab.Sirius3` — 스캐너, 레이저, 디지털 I/O, 파워미터 및 파워맵의 하드웨어 제어와 공통 계약
- `SpiralLab.Sirius3.UI` — 엔티티, 지오메트리 처리, OpenGL 렌더링, WinForms/WPF 컨트롤 및 내장 MCP 연동
 > NuGet 패키지 관리자를 이용한 손쉬운 설치 및 업데이트가 지원됩니다.

## 대상 플랫폼
- `net481`
- `net8.0-windows`
- `net9.0-windows`
- `net10.0-windows`

## 시스템 요구사항
- Windows 10/11 (x64)
- 최소 OpenGL 3.3 버전을 지원하는 GPU 필요 (최신 드라이버 강력 권장)
- 선택한 장치에 필요한 드라이버/런타임 설치 필요. NuGet의 네이티브 파일 복사는 장치 드라이버 설치를 대신하지 않습니다.
- 빌드: `net481`은 .NET Framework 4.8.1 Developer Pack, .NET 8/9/10 대상은 해당 .NET SDK 필요
- 실행: .NET Framework 4.8.1 또는 프레임워크 종속 .NET 애플리케이션에 맞는 Windows Desktop Runtime 필요
- 개발: 공개 데모는 .NET 데스크톱 개발 워크로드가 설치된 Visual Studio 2022 이상을 사용합니다(.NET 8: 17.8+, .NET 9: 17.12+; `net481`도 지원). Visual Studio에서 .NET 10을 대상으로 개발하려면 Visual Studio 2026 (18.0+)과 호환 SDK가 필요합니다([Microsoft 호환성 안내](https://learn.microsoft.com/en-us/dotnet/core/porting/versioning-sdk-msbuild-vs)).
 
## 의존성

다음은 Sirius3 연동에 사용되는 버전과 주요 .NET 의존성입니다. 관리형 의존성은 NuGet이 복원하며, 장치별로 필요한 드라이버는 별도로 설치합니다.

- SCANLAB
   - RTC4: v2023.11.02
   - RTC5: v2024.09.27
   - RTC6: 2026.9.10 v1.25.1
   - syncAXIS: v1.8.2 (2023.03.09)

- .NET
   - `net481`: OpenTK 3.3.3
   - `net8.0-windows`, `net9.0-windows`, `net10.0-windows`: OpenTK 및 OpenTK.Mathematics 4.9.4
   - 모든 대상 프레임워크의 공통 패키지 의존성
      - Microsoft.Extensions.Logging 10.0.10
      - Microsoft.Extensions.Logging.Abstractions 10.0.10
      - Microsoft.Extensions.Logging.Debug 10.0.10
      - Newtonsoft.Json 13.0.4

## 패키지 설치
- 참조 추가 (NuGet 패키지 관리자 이용 권장)
   - [SpiralLab.Sirius3.Dependencies](https://www.nuget.org/packages/SpiralLab.Sirius3.Dependencies)
   - [SpiralLab.Sirius3](https://www.nuget.org/packages/SpiralLab.Sirius3)
   - [SpiralLab.Sirius3.UI](https://www.nuget.org/packages/SpiralLab.Sirius3.UI)

애플리케이션과 호환되는 패키지 버전을 선택하고, Dependencies 패키지의 런타임·데이터 폴더를 빌드 및 배포 결과에 유지합니다. Dependencies 패키지는 빌드 시 이 파일들을 출력 폴더로 복사합니다. 대상 프레임워크는 위 지원 목록 중 애플리케이션에 필요한 항목을 선택합니다.

## 빠른 시작

위에서 안내한 Sirius3 NuGet 패키지 3개를 설치합니다. 아래 발췌 예제는 핵심 흐름을 보여주며 장치 생성, 검증과 이벤트 연결은 `...`로 생략합니다. 시작·로드·종료 단계는 각각 호스트의 해당 이벤트에 배치합니다.

공개 데모는 `config*.ini`를 읽고 `ScannerFactory`, `LaserFactory`, `IOFactory`, `PowerMeterFactory`, `MarkerFactory`로 장치를 생성·초기화한 뒤 등록합니다. `RegisterDevices`는 장치를 편집기에 연결하고 문서·뷰를 전달하여 `marker.Ready`를 호출합니다. INI 로딩 도우미는 데모에 속하며, 호스트 애플리케이션은 자체 설정 흐름을 구성합니다.

# [WinForms](#tab/winforms)

프로젝트 설정

```xml
<PropertyGroup>
    <OutputType>WinExe</OutputType>
    <PlatformTarget>x64</PlatformTarget>
    <TargetFrameworks>net481;net8.0-windows;net9.0-windows;net10.0-windows</TargetFrameworks>
    <UseWindowsForms>true</UseWindowsForms>
    <ApplicationHighDpiMode>PerMonitorV2</ApplicationHighDpiMode>
</PropertyGroup>
```

Core를 초기화하고 편집기를 만든 뒤 초기화된 장치를 등록합니다. WinForms DPI 초기화는 이 과정 전에 수행하며 아래 HiDPI 항목을 참고합니다.

```csharp
using SpiralLab.Sirius3.MCP;
using System.Windows.Forms;

// Startup: initialize Core and create the host UI.
if (!SpiralLab.Sirius3.Core.Initialize()) return;
var form = new Form
{
    AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F),
    AutoScaleMode = AutoScaleMode.Dpi
};
var editor = new SpiralLab.Sirius3.UI.WinForms.SiriusEditorControl
{
    Dock = DockStyle.Fill
};
form.Controls.Add(editor);
...

// Load: create and initialize the scanner, laser, I/O, power meter and marker.
...
editor.RegisterDevices(rtc, laser, powerMeter,
    dInExt1, dInLaserPort, dOutExt1, dOutExt2, dOutLaserPort, marker);

// Optional MCP: start after device registration (Remote license).
var server = MCPFactory.CreateServer(0, "MCP", editor);
server.AccessMode = MCPAccessModes.All;
editor.MCPServer = server;
await server.Start();
...
```

비동기 종료 시 UI 이벤트 루프를 유지하며 해제 완료를 기다립니다.

```csharp
// FormClosing / Window.Closing: await cleanup on the owning UI thread.
var document = editor.Document;
await editor.DisposeDevices();
editor.Dispose();
document?.Dispose();
...

// Application exit: after the UI lifetime ends.
SpiralLab.Sirius3.Core.Cleanup();
```

# [WPF](#tab/wpf)

프로젝트 설정

```xml
<PropertyGroup>
    <OutputType>WinExe</OutputType>
    <PlatformTarget>x64</PlatformTarget>
    <TargetFrameworks>net481;net8.0-windows;net9.0-windows;net10.0-windows</TargetFrameworks>
    <UseWPF>true</UseWPF>
    <ApplicationManifest>app.manifest</ApplicationManifest>
</PropertyGroup>
```

WPF 편집기를 만들기 전에 Core를 초기화합니다. Window가 로드되면 초기화된 장치를 등록합니다.

```csharp
using SpiralLab.Sirius3.MCP;
using System.Windows;

// Startup: initialize Core and create the host UI.
if (!SpiralLab.Sirius3.Core.Initialize()) return;
var editor = new SpiralLab.Sirius3.UI.WPF.SiriusEditorControl();
var window = new Window { Content = editor };
...

// Loaded: create and initialize the scanner, laser, I/O, power meter and marker.
...
editor.RegisterDevices(rtc, laser, powerMeter,
    dInExt1, dInLaserPort, dOutExt1, dOutExt2, dOutLaserPort, marker);

// Optional MCP: start after device registration (Remote license).
var server = MCPFactory.CreateServer(0, "MCP", editor);
server.AccessMode = MCPAccessModes.All;
editor.MCPServer = server;
await server.Start();
...
```

비동기 종료 시 UI 이벤트 루프를 유지하며 해제 완료를 기다립니다.

```csharp
// FormClosing / Window.Closing: await cleanup on the owning UI thread.
var document = editor.Document;
await editor.DisposeDevices();
editor.Dispose();
document?.Dispose();
...

// Application exit: after the UI lifetime ends.
SpiralLab.Sirius3.Core.Cleanup();
```

---

`DisposeDevices()`는 등록된 MCP 서버와 Remote를 먼저 비동기로 해제한 뒤 장치를 정리합니다. 편집기·Document 해제가 끝나면 애플리케이션 종료 단계에서 `Core.Cleanup()`을 호출합니다. 종료 이벤트의 중복 요청 방지와 오류 처리는 상세 예제에서 확인할 수 있습니다.

할당한 문서는 호출자가 소유합니다. WPF의 `Unloaded`는 화면 작업을 일시 중지하므로, 수명이 끝날 때 장치 해제를 기다린 뒤 편집기와 문서를 명시적으로 해제합니다. 초기화 결과를 확인하고 실패하면 생성 중인 장치도 정리하며, 성공한 `Core.Initialize()`마다 UI 종료 후 `Core.Cleanup()`을 호출합니다.

전체 예제: [WinForms 가이드](https://github.com/labspiral/sirius3/blob/main/doc/WinForms.koKR.md), [WPF 가이드](https://github.com/labspiral/sirius3/blob/main/doc/WPF.koKR.md), [데모 프로젝트](DEMOS.koKR.md) · [MCP 연동 가이드](SpiralLab.Sirius3.UI/MCP/README.koKR.md).

## WinForms HiDPI

먼저 DLL/패키지를 갱신합니다. 기존 `DpiUnaware` 프로그램은 기존 동작을 유지할 수 있습니다. 모니터별 배율을 선택하려면 컨트롤 생성 전에 소비 EXE를 설정합니다. DLL은 프로세스 DPI 모드를 변경하지 않습니다. 기존 호스트 Form의 `AutoScaleMode.Font`, 글꼴과 Designer 기준값을 유지할 수 있습니다. 자체 고정 픽셀 레이아웃과 수동 배율 코드는 별도로 점검합니다.

.NET 8/9/10의 수동 `Main`에서는 `EnableVisualStyles`와 `Core.Initialize` 전에 Per-Monitor V2를 설정합니다:

```csharp
#if NET8_0_OR_GREATER
Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
#endif
Application.EnableVisualStyles();
Application.SetCompatibleTextRenderingDefault(false);
```

.NET Framework 4.8.1에서는 소비 EXE의 기존 app.config에 아래 항목을 합치고 binding redirects를 유지합니다. manifest에 Windows 10 호환성을 선언하고 컨트롤 생성 전에 `EnableVisualStyles`를 호출합니다. 충돌하는 unaware DPI 선언과 `EnableWindowsFormsHighDpiAutoResizing=false` 설정은 제거합니다.

```xml
<System.Windows.Forms.ApplicationConfigurationSection>
  <add key="DpiAwareness" value="PerMonitorV2" />
</System.Windows.Forms.ApplicationConfigurationSection>
```

`ApplicationConfiguration.Initialize()`를 사용하는 경우 EXE 프로젝트에 `<ApplicationHighDpiMode>PerMonitorV2</ApplicationHighDpiMode>`를 설정하여 초기화 경로를 하나로 유지합니다. 글꼴 크기는 pt 단위를 유지합니다. 자체 ImageList는 보관한 원본으로 DPI별 재생성하며 이미지 추가 전에 `ImageSize`/`ColorDepth`를 설정합니다. 키와 인덱스를 유지하고 모든 참조를 교체한 뒤 소유한 이전 목록만 해제합니다. WPF의 96은 논리 단위 기준이므로 레이아웃에 DPI 배율을 다시 곱하지 않습니다.

[Microsoft: .NET Framework HiDPI](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/high-dpi-support-in-windows-forms), [WinForms automatic scaling](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/forms/autoscale).

## 데모 프로그램

공개 WinForms/WPF 편집기 데모의 대상은 정확히 `net481;net8.0-windows;net9.0-windows`이며, 선택한 대상에 맞는 SDK와 Visual Studio 2022 이상이 필요합니다. 라이브러리는 `net10.0-windows`도 지원합니다. [editor_ui](https://github.com/labspiral/sirius3/tree/main/demos/editor_ui) 또는 [wpf_editor_ui](https://github.com/labspiral/sirius3/tree/main/demos/wpf_editor_ui)에서 시작할 수 있습니다.

- 프로그램 설명 [DEMOS.koKR.md](DEMOS.koKR.md) 
- 스캐너, 레이저, 파워미터, 마커 등의 장치를 생성·초기화한 뒤 SiriusEditorControl에 등록합니다.
- 예제 코드: https://github.com/labspiral/sirius3/tree/main/demos

## 라이선스
- 상업용 사용은 라이선스 구매가 필요합니다.
- 라이선스: RTC 인스턴스 개수 + [옵션]
    - MoF 옵션: 외부 엔코더를 이용한(실시간 추종 및 대기 등) Fly 가공 기능 (Processing on the fly).
    - MultiBeam 옵션: 1개의 레이저 소스 + 2개의 AOM + 2개의 스캔헤드 구성으로 점프 구간에서 레이저 빔 경로를 실시간 변경해 처리하는 기능.
    - syncAXIS 옵션: ACS 모션 제어기 + excelliSCAN 스캔헤드 구성으로 스캔헤드와 스테이지의 동기화를 이용한 대면적 가공(XL-SCAN 솔루션).
    - Remote 옵션: 소켓, 시리얼, WebSocket, MQTT 외부 통신을 통한 레시피 변경, 가공 제어 및 데이터 조회·변경을 지원하며, AI 기반 문서·엔티티 편집과 등록 장치 제어를 위한 내장 MCP 서버 기능을 포함합니다.
- 라이선스 정책 및 외부 라이브러리는 [LICENSE.koKR.txt](LICENSE.koKR.txt), [THIRD-PARTY-NOTICES.koKR.txt](THIRD-PARTY-NOTICES.koKR.txt) 참고.
- 이메일: hcchoi@spirallab.co.kr | https://spirallab.co.kr
> 라이선스키가 없으면 30분간 사용이 가능한 평가모드로 실행됩니다.

## 버전 이력
- 이력 정보 [HISTORY.koKR.md](HISTORY.koKR.md)

## API 문서
- https://spirallab.co.kr/sirius3/doc 참고
