# WPF entity editor development demo

Native WPF host for the entity samples shared with `editor_entity`. The source
uses the matching local Debug or Release libraries, both of which include the
WPF API. Supported project targets are net481, net8, net9 and net10 for Windows.

## Build

Use Visual Studio Developer PowerShell. Build Core and UI from the sibling
`sirius3lib` checkout, then build this project with the same target framework:

```powershell
msbuild D:\git\sirius3lib\SpiralLab.Sirius3\SpiralLab.Sirius3.csproj /t:Build /p:Configuration=Release /p:TargetFramework=net481
msbuild D:\git\sirius3lib\SpiralLab.Sirius3.UI\SpiralLab.Sirius3.UI.csproj /restore /t:Build /p:Configuration=Release /p:TargetFramework=net481 /p:BuildProjectReferences=false
msbuild .\wpf_editor_entity.csproj /restore /t:Build /p:Configuration=Release /p:TargetFramework=net481
```

Each target references the matching Core and UI DLLs directly from the sibling
`sirius3lib/bin/<target>` directory. GLWpfControl is already merged into the UI
DLL, so the demo does not reference or copy a separate GLWpfControl assembly.
This demo does not start an MCP server. The shared `demos/config.ini` is copied to the isolated
`bin/wpf_editor_entity/<target>` output directory.

The matching UI DLL contains the GLWpfControl implementation for both the
OpenTK3 and OpenTK4 branches. Do not add a separate GLWpfControl package or DLL
to this demo.

## Use and ownership

- Stop other Sirius3 processes and review the shared `demos/config.ini` before
  starting the demo. It may select real scanner, laser and power-meter devices.
- Use the top buttons to create entities, then select them in the view or tree.
  Property cells support direct entry, multi-selection and document Undo/Redo.
- The EditorControl `Ground` command below `CheckerMode` cycles None, Paper,
  Plastic, Steel and Marble over the existing grid.
  Every finish retains the active CheckerMode; None disables material effects only.
  It uses `IView.GroundMaterial`, affects display only and is not saved in the
  recipe. Highlights follow the view's light, and `UI.Config.IsMeshWithShadowEffect`
  controls projected shadows from triangle meshes.
- Check text/numeric/enum cells, invalid input, Reset, search and selection changes.
  Check compact layout and popup editors at 100%, 150% and 200% display scaling.
- `App` owns Core initialization/cleanup. The window owns the devices and document;
  it disposes devices, detaches/disposes the editor, then disposes the document.
  Do this in the accepted Closing path, before the GL control is detached.
  Dispose waits for owning-context cleanup; Closed/OnExit is only a fallback.
  `Unloaded` does not dispose caller-owned resources.
- Replacing the virtual configuration can enable real hardware commands.
  Do not use Start, manual emission, scanner movement or DIO in a UI-only test.

## 한국어

EditorControl의 `CheckerMode` 아래 `Ground` 버튼은
None·Paper·Plastic·Steel·Marble 재질을 순환합니다.
모든 재질은 기존 CheckerMode 격자를 유지하며 None은 재질 효과만 끕니다.
`IView.GroundMaterial`은 가공 데이터와 레시피를 변경하지 않습니다.
기존 광원을 이용하며 `UI.Config.IsMeshWithShadowEffect`가 삼각형 메시의 투영 그림자를 제어합니다.

WinForms `editor_entity`의 엔티티 생성 예제를 공유하는 네이티브 WPF 개발용
데모입니다. 동일한 구성과 타깃의 Core 및 UI를 먼저 빌드합니다.
Debug와 Release UI 모두 WPF와 MCP 공개 API를 포함하지만 이 데모는 MCP 서버를 시작하지 않습니다.
각 타깃의 GLWpfControl 구현은 `SpiralLab.Sirius3.UI.dll`에 이미 병합되어
있습니다. 이 데모에는 별도의 GLWpfControl 패키지, DLL 또는 라이선스 파일을
추가하지 않습니다.

공용 `demos/config.ini`가 출력 폴더에 복사되며 해당 설정으로 장치를 초기화합니다.
실제 장치가 선택될 수 있으므로 설정을 먼저 확인하고 다른 Sirius3 프로세스를 종료한 뒤 실행하십시오. 객체 생성·선택·속성 입력·검색·
초기화·Undo/Redo 및 화면 배율별 셀과 팝업 배치를 확인합니다. 창은 장치,
편집기, 문서 순서로 해제하며 App이 마지막에 Core를 정리합니다.
GL 컨트롤이 분리되기 전, 종료를 확정한 Closing 단계에서 정리를 완료합니다.
Dispose는 소유 컨텍스트의 정리를 기다리며 Closed/OnExit는 보조 정리 경로입니다.
설정을 실제 장치로 바꾸면 가공·수동 출력·이동·DIO 명령이 장비에 전달될 수
있으므로 UI 검증에서는 실행하지 않습니다.
