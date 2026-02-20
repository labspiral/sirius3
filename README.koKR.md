# Sirius3
.NET 기반, 정밀 레이저 가공을 위한 올인원 플랫폼

![sirius3_logo](https://spirallab.co.kr/sirius3/sirius3_logo3.png)

---

## 하이라이트
![sirius3_editor](https://spirallab.co.kr/sirius3/sirius3_editor.png)

- SCANLAB RTC 제어기
   - RTC4 / RTC4e / RTC5 / RTC6 / RTC6e
   - XL-SCAN (syncAXIS 기반의 RTC6 + ACS 조합)
- 계측 및 프로파일링
   - 스캐너 운동 경로와 시그널 출력 로그를 이용한 그래프 출력 지원
- 강력한 가공 옵션
   - 가변 폴리곤, 점프 지연시간 설정 지원
   - MoF (Marking on the Fly), 2nd 헤드, 3D 지원
   - Sky Writing Mode 1/2/3/4
   - SCANAhead 를 이용한 지연값(Auto delays) 자동 설정 지원
- ALC(Automatic Laser Control)
   - 벡터 정의형
      - 램프(Ramp)
   - 속도 의존형
      - 지령 속도
      - 실제 속도
      - 엔코더 속도
      - 또한 SCANAhead, Encoder Speed Addition, Inverse Speed Correction, Backward Transformation, SDC+Skywriting 조합 사용 가능
   - 위치 의존형
      - 거리 및 스케일 값 기반 테이블
- 스캐너 필드 보정
   - 2D / 3D 보정
- 레이저 파워 제어
   - 주파수, 펄스폭, 아나로그, 디지털 출력
   - 레이저 소스 벤더 지원: AdvancedOptoWave, Coherent, IPG, JPT, Photonics Industry, Spectra Physics 등
- 파워메터와 파워맵
   - Coherent (PowerMax), Thorlabs (OPM 기반), Ophir (StarLab 기반)
   - 파워맵 기반의 출력 보상 지원
- 다양한 엔티티와 포맷 지원
   - 점, 선, 호, 폴리라인, 삼각형, 사각형, 나선, 트레팬, 스플라인 등
   - 레이어, 그룹, 블럭, 블럭 삽입 등
   - Text, SiriusText, ImageText, Circular Text 등
   - Image, DXF, HPGL, ZPL
   - QR, DataMatrix, PDF417 Barcodes
   - STL, OBJ, PLY 등의 3D 메쉬 포맷 
- 오픈 아키텍쳐
   - 편집기(Editor) 와 레이저 소스 제어용 코드가 오픈소스로 제공됨

## 주요 변경사항
|                              |                SIRIUS3                   |              SIRIUS2                  |
|:-----------------------------|:-----------------------------------------|:--------------------------------------|
| 다중 페이지                   |4개의 페이지 교차 편집 지원                 |단일 페이지 편집                        |
| 카메라                        |6개(2D + 5개 3D)의 카메라                  |단일 3D 카메라                          |
| 렌더링 속도                   |개선된 쉐이더 엔진                         |내장 쉐이더 엔진                         |
| 랜더링 모드                   |Model, PerVertex, Normal, ZDepth          |없음                                   |
| 선택 기능                     |개선된 알고리즘 탑재                       |저속                                   |
| 해치                         |해치 패턴 중복 적용 가능                    |단일 해치                               |
| 3D 메쉬 슬라이서              |PLY, OBJ, STL 메쉬용 슬라이서 내장          |없음                                   |
| 거버 파일 (RS-274x)          |지원                                       |없음                                   |
| 웨이퍼/기판 맵                |편집기 내장                                |없음                                   |
| 외부 폰트 파일                |CXF, LFF 파일 포맷                        |커스텀 CXF, LFF 파일 포맷만 지원         |
| 펜                           |Entity 와 Layer 용 펜 속성 분리            |Entity 단일펜                           |
| 라이브러리 업데이트           |Nuget 패키지 매니저 지원                    |수동                                   |
                                                                                                              
![sirius3_hatch](https://spirallab.co.kr/sirius3/sirius3_hatch.png)
![sirius3_pod](https://spirallab.co.kr/sirius3/sirius3_pod.png)
![sirius3_slicer](https://spirallab.co.kr/sirius3/sirius3_slicer.png)
![sirius3_syncaxis](https://spirallab.co.kr/sirius3/sirius3_syncaxis.png)

## 패키지 / DLLs
- `SpiralLab.Sirius3.Dependencies` — SCANLAB RTC4/5/6, syncAXIS 런타임, 폰트, 샘플 파일들
- `SpiralLab.Sirius3` — 하드웨어 제어 (스캐너/레이저/파워메터 등)
- `SpiralLab.Sirius3.UI` — 다양한 엔티티, 3D 렌더링 엔진, 윈폼 등 UI 컨트롤
 > NuGet 패키지 관리자를 이용한 손쉬운 설치 및 업데이트가 지원됩니다.

## 대상 플랫폼
- `net481`
- `net8.0-windows`

## 시스템 요구사항
- Windows 10/11 (x64)
- OpenGL 3.3 이상 지원의 GPU 필요
- SCANLAB 드라이버/런타임 설치 필요
 
## Dependencies
- SCANLAB
   - RTC4: v2023.11.02
   - RTC5: v2024.09.27
   - RTC6: 2025.10.30 v1.22.1
   - syncAXIS: v1.8.2 (2023.03.09)

- .NET / OpenTK
   - `net481`
      - OpenTK 3.3.3
   - `net8.0-windows`
      - OpenTK 4.9.4
      - OpenTK.Mathematics 4.9.4
   - Common
      - Newtonsoft.Json 13.0.4
      - Microsoft.Extensions.Logging 8.0.1
      - Microsoft.Extensions.Logging.Abstractions 8.0.3

## 패키지 설치
- 참조 추가 (NuGet 패키지 관리자 이용 권장)
   - `SpiralLab.Sirius3.Dependencies` (https://www.nuget.org/packages/SpiralLab.Sirius3.Dependencies)
   - `SpiralLab.Sirius3` (https://www.nuget.org/packages/SpiralLab.Sirius3)
   - `SpiralLab.Sirius3.UI` (https://www.nuget.org/packages/SpiralLab.Sirius3.UI)

- 스캐너, 레이저, 파워메터, 마커등의 장치 객체를 생성하고 SiriusEditorControl 에 연결.
- 예제 코드: https://github.com/labspiral/sirius3

## 빠른 시작
프로젝트 설정
```
<PropertyGroup Condition="'$(TargetFramework)'=='net481'">
	<DefineConstants>$(DefineConstants);OPENTK3</DefineConstants>
</PropertyGroup>

PropertyGroup Condition="'$(TargetFramework)'=='net8.0-windows'">
	<DefineConstants>$(DefineConstants);OPENTK4</DefineConstants>
</PropertyGroup>

<ItemGroup Condition="'$(TargetFramework)'=='net481'">
	<PackageReference Include="OpenTK" Version="3.3.3" />
</ItemGroup>
	
<ItemGroup Condition="'$(TargetFramework)'=='net8.0-windows'">
	<PackageReference Include="OpenTK" Version="4.9.4" />
	<PackageReference Include="OpenTK.Mathematics" Version="4.9.4" />
</ItemGroup>

<ItemGroup>
	<PackageReference Include="SpiralLab.Sirius3.Dependencies" Version="1.*" />
	<PackageReference Include="SpiralLab.Sirius3" Version="1.*" />
	<PackageReference Include="SpiralLab.Sirius3.UI" Version="1.*" />

	<PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="8.0.3" />
	<PackageReference Include="Microsoft.Extensions.Logging" Version="8.0.1" />
	<PackageReference Include="Newtonsoft.Json" Version="13.0.4" />
</ItemGroup>
```

예제 코드
```
#if OPENTK3
    using OpenTK;
    using DVec3 = OpenTK.Vector3d;
#elif OPENTK4
    using OpenTK.Mathematics;
    using DVec3 = OpenTK.Mathematics.Vector3d;
#endif

public class MainForm : Form
{
    private readonly SiriusEditorControl editor = new SiriusEditorControl();

    public MainForm()
    {
        editor.Dock = DockStyle.Fill;
        Controls.Add(editor);
        Load += (s, e) =>
        {
            // 1. 장치 생성
            var scanner =  ScannerFactory.Create ...
            scanner.Initialize();

            var laser = LaserFactory.Create ...
            laser.Initialize();

            var powerMeter = PowerMeterFactory.Create ...
            powerMeter.Initialize();

            var marker = MarkerFactory.Create ... 
            marker.Initialize();

            // 2. SiriusEditorControl 에 연결
            editor.Scanner = scanner;
            editor.Laser = laser;
            editor.PowerMeter = powerMeter;
            editor.Marker = marker;
            
            // 3. 엔티티 생성
            var line = EntityFactory.CreateLine(new DVec3(0, 0, 0), new DVec3(10, 10, 0));
            editor.Document.ActAdd(line);
          
            var text = EntityFactory.CreateText("Arial", FontStyle.Regular, "SIRIUS3", 10);
            editor.Document.ActAdd(text);
            
            // 4. 마커 준비
            marker.Ready(editor.Document, editor.View, scanner, laser, powerMeter);
        };
    }

    [STAThread]
    static void Main()
    {
        // sirius3 라이브러리 초기화
        SpiralLab.Sirius3.Core.Initialize();

        ...
        Application.Run(new MainForm());

        // sirius3 라이브러리 종료(정리)
        SpiralLab.Sirius3.Core.Cleanup();
    }
}
```

## 라이센스
- 상업용 사용은 라이센스 구매가 필요합니다.
- 라이센스: RTC 인스턴스 개수 + [옵션: MoF 혹은 syncAXIS]
- 라이센스 및 외부 라이브러리는 LICENSE.txt, THIRD-PARTY-NOTICES.txt 참고.
- 이메일: hcchoi@spirallab.co.kr | https://spirallab.co.kr
> 라이센스키가 없으면 30분간 사용이 가능한 평가모드로 실행됩니다.

## 버전 이력
- 이력 정보 [HISTORY.krKR.md](HISTORY.koKR.md)
