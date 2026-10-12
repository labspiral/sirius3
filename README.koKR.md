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
   - 경로 시뮬레이션 을 통한 시각화 지원
- 강력한 가공 옵션
   - 가변 폴리곤, 가변 점프 지연시간 설정 지원
   - 2nd 헤드, 3D 지원
   - MoF (Marking on the Fly) 와 확장 MoF (Fly extension) 지원
   - Sky Writing Mode 1/2/3/4
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
   - 주파수, 펄스폭, 아나로그, 디지털 출력
   - 레이저 소스 벤더 지원: AdvancedOptoWave, Coherent, IPG, JPT, Photonics Industry, Spectra Physics 등
- 파워메터와 파워맵
   - Coherent (PowerMax), Thorlabs (OPM 기반), Ophir (StarLab 기반)
   - 파워맵 기반의 출력 보상 지원
- 렌더링 및 지오메트리 처리
   - 한 개의 직교 카메라와 다섯 개의 원근 카메라를 제공하는 OpenGL 3.3+ 2D/3D 렌더러
   - 점, 선, 라인 스트립 및 삼각형 히트 테스트용 AABB 가속 구조
   - 폐곡선/열린 경로 진단을 포함하는 토폴로지 기반 3D 메쉬 슬라이서
   - 외곽선, 중첩 영역 및 연결된 바코드 셀을 처리하는 winding 기반 다중 해치
- 엔티티, 텍스트 및 바코드
   - 점, 선, 호, 폴리라인, 삼각형, 사각형, 나선, 트레팬, 스플라인
   - 큐브, 구, 실린더, 원뿔, 메쉬, 레이어, 그룹, 블록 및 블록 삽입
   - Text, SiriusText, ImageText, Circular Text, 링크 텍스트 및 ZPL 렌더링 엔티티
   - 외곽선, 해치 및 Dots 셀 가공을 지원하는 1D, QR, DataMatrix, PDF417 및 Aztec 바코드
- 외부 파일 및 상호 운용성
   - Sirius3 문서, DXF/DWG, SVG, HPGL/PLT, Gerber/Excellon 및 G-code/NGC
   - SVG (`.svg`) 벡터 가져오기: `IDocument.ActImport` / `EntityFactory.CreateSVG`. 편집 가능한 원본 곡선을 유지하고 채움·선 두께·끝점·모서리를 mm 단위 영역으로 생성하며, 둥근 끝점과 모서리도 반영합니다.
   - SVG 해치: `UI.Config.IsSvgImportHatchEnabled`로 활성화합니다(기본 `false`). `UI.Config.SvgImportHatchInterval`로 간격을 조절합니다(기본 0.1 mm). 선/폴리곤 해치를 지원하며, 화면에 표시된 윤곽과 해치를 기준으로 클릭·드래그 선택합니다.
   - 래스터 이미지와 STL, OBJ, PLY, STP/STEP, 3MF 3D 모델
   - 3MF (`.3mf`) 메쉬 가져오기: `IDocument.ActImport` / `EntityFactory.CreateMesh`. 빌드에 포함된 여러 삼각형 메쉬 모델과 컴포넌트를 상대 배치와 파일 단위를 반영하여 하나의 mm 단위 메쉬로 가져옵니다. 저장된 슬라이서 가공 경로와 슬라이스 결과는 가져오기 대상에 포함되지 않습니다.
   - 3D 미리보기에서 제공되는 메쉬 색상과 OBJ/PLY/3MF 확산 텍스처 유지
   - G-code/NGC 가져오기는 모달 XYZ 좌표, 단위·작업 원점, 평면 원호·헬릭스, 지연 및 레이저 ON/OFF 경로를 유지합니다. `UI.Config.GCodeImportDialect`로 CNC·Laser·Marlin 규칙을 선택하고 `GCodeImportInfo.TryGet`으로 장치 설정을 적용하지 않은 원본 F/S/T와 줄 범위를 조회합니다.
   - 벡터 파일의 허용 오차 기반 경로 연결과 내용 기반 Gerber/Excellon 판별
- 원격 통신 및 동적 데이터
   - 마커 제어와 데이터 접근을 위한 TCP/IP, Serial(RS-232), WebSocket 및 MQTT
   - AI 기반 편집기 등록, 문서·엔티티 편집, 시뮬레이션 및 등록 장치 제어를 지원하는 내장 HTTP MCP 서버
   - 텍스트와 바코드 데이터용 이벤트, 파일, 오프셋, 링크 엔티티 및 C# 스크립트 변환
- 문서, 편집기 및 시뮬레이션
   - 레이어, 펜, 그룹, 블록 및 개수 설정형 Undo/Redo를 갖는 4개 문서 페이지
   - WinForms와 WPF `SiriusEditorControl` / `SiriusMultiEditorControl`을 Debug 및 Release에서 모두 지원하며, 하나의 문서를 복수 뷰에 렌더링하는 기능
   - 화면 고정 크기 마커, 빔 효과 및 선택적 파편 효과를 제공하는 실시간 가공 경로 시각화
   - 카메라 및 검사 작업용 격자 기반 스티치 이미지 시각화
- 오픈 아키텍쳐
   - 확장 가능한 편집기, 엔티티, 마커, 스캐너, 레이저, 파워미터 및 원격 통신 인터페이스
   - AI 기반 편집기 등록, 문서·엔티티 편집 및 장치 제어를 지원하는 내장 MCP 서버

## 주요 변경사항
|                              |                SIRIUS3                   |              SIRIUS2                  |
|:-----------------------------|:-----------------------------------------|:--------------------------------------|
| 다중 페이지                   |4개의 페이지 교차 편집 지원                 |단일 페이지 편집                        |
| 카메라                        |6개(2D + 5개 3D)의 카메라                  |단일 3D 카메라                          |
| 렌더링 속도                   |GPU 가속 OpenGL 쉐이더 엔진                |내장 쉐이더 엔진                         |
| 랜더링 모드                   |Model, PerVertex, Normal, ZDepth          |없음                                   |
| 선택 기능                     |점/선/삼각형용 AABB 가속                   |저속                                   |
| 해치                         |winding 기반 다중 해치                     |단일 해치                               |
| 3D 메쉬 슬라이서              |STL, OBJ, PLY, STEP, 3MF 슬라이서 내장      |없음                                   |
| Gerber / Excellon            |내용 판별 기반 가져오기                     |없음                                   |
| 외부 폰트 파일                |CXF, LFF, FNT, DOT 파일 포맷               |커스텀 CXF, LFF 파일 포맷만 지원         |
| 펜                           |Entity 와 Layer 용 펜 속성 분리            |Entity 단일펜                           |
| MCP                          |지원                                      |없음                                   |
| 라이브러리 업데이트            |Nuget 패키지 매니저 지원                    |수동                                   |
                                                                                                              
![sirius3_hatch](https://spirallab.co.kr/sirius3/sirius3_hatch.png)
![sirius3_pod](https://spirallab.co.kr/sirius3/sirius3_pod.png)
![sirius3_slicer](https://spirallab.co.kr/sirius3/sirius3_slicer.png)
![sirius3_syncaxis](https://spirallab.co.kr/sirius3/sirius3_syncaxis.png)

## 패키지 / DLLs
- `SpiralLab.Sirius3.Dependencies` — SCANLAB RTC4/5/6, syncAXIS 런타임, 폰트, 샘플 파일들
- `SpiralLab.Sirius3` — 하드웨어 제어 (스캐너/레이저/파워메터 등)
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
- SCANLAB 드라이버/런타임 설치 필요
- Visual Studio 2022 이상 버전
 
## 의존성
- SCANLAB
   - RTC4: v2023.11.02
   - RTC5: v2024.09.27
   - RTC6: 2026.8.17 v1.25.1
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
   - `SpiralLab.Sirius3.Dependencies` (https://www.nuget.org/packages/SpiralLab.Sirius3.Dependencies)
   - `SpiralLab.Sirius3` (https://www.nuget.org/packages/SpiralLab.Sirius3)
   - `SpiralLab.Sirius3.UI` (https://www.nuget.org/packages/SpiralLab.Sirius3.UI)

## 빠른 시작

위에서 안내한 Sirius3 NuGet 패키지 3개를 설치합니다. 아래 발췌 예제는 핵심 흐름을 보여주며 장치 생성, 검증과 이벤트 연결은 `...`로 생략합니다. 시작·로드·종료 단계는 각각 호스트의 해당 이벤트에 배치합니다.

# [WinForms](#tab/winforms)

프로젝트 설정

```xml
<PropertyGroup>
    <OutputType>WinExe</OutputType>
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
SpiralLab.Sirius3.Core.Initialize();
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
editor.RegisterDevices(rtc, laser, powerMeter, ..., marker);

// Optional MCP: start after device registration (Remote license).
var server = MCPFactory.CreateServer(0, "MCP", editor);
server.AccessMode = MCPAccessMode.All;
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
SpiralLab.Sirius3.Core.Initialize();
var editor = new SpiralLab.Sirius3.UI.WPF.SiriusEditorControl();
var window = new Window { Content = editor };
...

// Loaded: create and initialize the scanner, laser, I/O, power meter and marker.
...
editor.RegisterDevices(rtc, laser, powerMeter, ..., marker);

// Optional MCP: start after device registration (Remote license).
var server = MCPFactory.CreateServer(0, "MCP", editor);
server.AccessMode = MCPAccessMode.All;
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

`DisposeDevices()`는 등록된 MCP 서버와 Remote를 먼저 비동기로 해제한 뒤 장치를 정리합니다. 편집기·Document 해제가 끝나면 애플리케이션 종료 단계에서 `Core.Cleanup()`을 호출합니다. 종료 이벤트의 중복 요청 방지와 오류 처리는 상세 예제에서 확인할 수 있습니다.

전체 예제: [WinForms 가이드](https://github.com/labspiral/sirius3/blob/main/doc/WinForms.koKR.md), [WPF 가이드](https://github.com/labspiral/sirius3/blob/main/doc/WPF.koKR.md), [데모 프로젝트](DEMOS.koKR.md).

---

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
- 프로그램 설명 [DEMOS.koKR.md](DEMOS.koKR.md) 
- 스캐너, 레이저, 파워메터, 마커등의 장치 객체를 생성하고 SiriusEditorControl 에 연결.
- 예제 코드: https://github.com/labspiral/sirius3/tree/main/demos

## 라이선스
- 상업용 사용은 라이선스 구매가 필요합니다.
- 라이선스: RTC 인스턴스 개수 + [옵션]
    - MoF 옵션: 외부 엔코더를 이용한(실시간 추종 및 대기 등) Fly 가공 기능 (Processing on the fly).
    - MultiBeam 옵션: 1개의 레이저 소스 + 2개의 AOM + 2개의 스캔헤드 구성으로 점프 구간에서 레이저 빔 경로를 실시간 변경해 처리하는 기능.
    - syncAXIS 옵션: ACS 모션 제어기 + excelliSCAN 스캔헤드 구성으로 스캐헤드와 스테이지의 동기화를 이용한 대면적 가공(XL-SCAN 솔류션).
    - Remote 옵션: 소켓, 시리얼, WebSocket, MQTT 외부 통신을 통한 레시피 변경, 가공 제어 및 데이터 조회·변경을 지원하며, AI 기반 문서·엔티티 편집과 등록 장치 제어를 위한 내장 MCP 서버 기능을 포함합니다.
- 라이선스 정책 및 외부 라이브러리는 [LICENSE.koKR.txt](LICENSE.koKR.txt), [THIRD-PARTY-NOTICES.koKR.txt](THIRD-PARTY-NOTICES.koKR.txt) 참고.
- 이메일: hcchoi@spirallab.co.kr | https://spirallab.co.kr
> 라이선스키가 없으면 30분간 사용이 가능한 평가모드로 실행됩니다.

## 버전 이력
- 이력 정보 [HISTORY.krKR.md](HISTORY.koKR.md)

## API 문서
- https://spirallab.co.kr/sirius3/doc 참고
