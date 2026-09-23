# Sirius3 MCP Editor 데모: AI 클라이언트 연결 방법

[English](MCP.md) | [한국어](MCP.koKR.md)

`editor_remote_mcp`는 하나의 Sirius3 편집기를 MCP 호환 AI 클라이언트에 연결하는 공개 WinForms 데모입니다. `config.ini`에 설정된 장치를 등록하고, `[MCP0] ENABLE=1`이면 MCP 서비스를 시작하여 창에 상태와 endpoint, access token을 표시합니다.

`net481`을 포함한 모든 지원 타깃은 인증된 Streamable HTTP를 사용합니다. endpoint, 접근 모드와 문서 루트는 `config.ini`의 `[MCP0]`에서 읽습니다. `SIRIUS3_MCP_TOKEN`으로 고정 토큰을 전달하지 않으면 실행마다 새 토큰을 생성합니다. 기본 수신 주소는 `http://0.0.0.0:3712/mcp`입니다. 같은 PC의 클라이언트는 `http://127.0.0.1:3712/mcp`, 다른 PC는 서버의 실제 IP를 사용합니다. STDIO는 지원하지 않습니다. 실행 중인 데모 하나는 편집기 하나와 MCP 서버 하나를 나타냅니다.

파일 열기·가져오기·저장과 파일 기반 엔티티 팩터리는 호스트가 `AllowedFileRoots`를 하나 이상 추가해야 사용할 수 있습니다. `[MCP0]`은 파일 루트를 설정하지 않습니다.

클라이언트 취소는 편집기 UI에서 아직 대기 중인 요청만 취소합니다. 이미 시작한 가공을 중단하거나 작업을 되돌리지는 않습니다. 중단을 명시적으로 요청한 뒤 결과 상태를 확인하십시오.

바코드 셀 설정은 나머지 값과 Undo/Redo를 유지하면서 개별 변경할 수 있습니다. 예: “이 DataMatrix의 원형 셀 반지름 비율을 0.65로 바꾸고 다른 셀 설정은 유지해 줘.” 잘못된 자료형, 범위를 벗어난 숫자, 지원하지 않는 설정은 해당 속성을 변경하지 않고 검증 오류로 반환합니다.

엔티티 편집은 현재 활성 레이어를 대상으로 합니다. 없거나 오래된 엔티티 ID, 펜·레이어·장치 ID 및 변경할 수 없는 속성은 문서를 변경하지 않고 검증 오류로 반환합니다. 다시 시도하기 전에 엔티티 목록을 갱신하십시오.

## 데모 서버 설정

`config.ini`의 `[MCP0]`에서 서버 활성화, HTTP endpoint, 접근 모드와 SCANLAB 문서 루트를 설정합니다. 데모는 환경 변수를 먼저 적용하며 `[MCP0] URI`가 지정되면 환경 변수의 endpoint보다 우선합니다. 고정 서버 토큰은 `SIRIUS3_MCP_TOKEN`으로 전달할 수 있습니다. 클라이언트의 `bearer_token_env_var = "SIRIUS3_MCP_TOKEN"`은 클라이언트 프로세스에서 토큰을 읽으며 실행 중인 서버를 설정하지 않습니다. 서버와 클라이언트의 인증 값이 같아야 합니다.

권한과 문서 루트는 `config.ini`에서 설정하며 이 데모에서는 파일 루트를 설정할 수 없습니다. 데모는 환경 변수의 비밀 값을 저장하지 않습니다.

공식 설정과 필드 정의: [MCP](https://learn.chatgpt.com/docs/extend/mcp), [구성 참조](https://learn.chatgpt.com/docs/config-file/config-reference).

## 공개 데모 준비

공개 `demos/editor_remote_mcp` 프로젝트에서 생성한 실행 파일이나 배포된 릴리스 패키지를 사용합니다. 실행 파일, `config.ini`, 함께 제공된 런타임 및 종속 파일을 같은 배포 구조에 유지하십시오. 아래 연결 절차에는 공개 데모 출력만 필요하며 별도의 라이브러리 소스 저장소를 사용하지 않습니다.

시작 전에 `config.ini`를 확인하십시오. 이 파일에 따라 데모가 등록할 스캐너, 레이저, 마커, 디지털 I/O, 파워미터 구현이 결정됩니다. 서버는 다음과 같이 활성화하고 설정합니다.

```ini
[MCP0]
ENABLE=1
URI=http://0.0.0.0:3712/mcp
ACCESS=All
DocumentationRoot=..\..\..\doc\SCANLAB
```

상대 `DocumentationRoot` 경로는 데모의 `config.ini`가 있는 폴더를 기준으로 해석하며 절대 경로도 사용할 수 있습니다. `URI`를 생략하면 `SIRIUS3_MCP_HTTP_ENDPOINT` 환경 변수로 주소를 지정할 수 있습니다.

`ACCESS`에는 `ReadOnly`, `DocumentControl`, `DeviceControl`, `All` 중 하나를 지정합니다. 서버 실행 중에도 편집기의 MCP PropertyGrid에서 접근 모드를 변경할 수 있으며 이후 요청부터 적용됩니다.

이 문서는 WinForms `editor_remote_mcp` 데모를 Claude, Codex, Gemini, GitHub Copilot에 연결하는 방법을 설명합니다. 예제는 2026-09-14 기준 각 클라이언트의 공식 문서와 대조했습니다.

## 먼저 확인할 안전 사항

이 데모는 `[MCP0] ACCESS`에 따라 문서 편집과 등록된 장치 제어를 허용할 수 있습니다. 열린 문서를 변경하고 설정된 스캐너, 레이저, 디지털 I/O, 파워미터, 파워맵을 동작시킬 수 있습니다. 파일 기반 작업에는 호스트가 별도로 지정한 허용 루트도 필요합니다. 쓰기 또는 장치 요청은 승인 전에 매번 검토하십시오.

Sirius3 프로세스는 한 번에 하나만 실행해야 합니다. 다른 대상 프레임워크나 클라이언트를 시작하기 전에 이전 Sirius3 데모, 클라이언트가 실행한 `editor_remote_mcp.exe`, 디버깅 세션을 모두 종료하십시오.

## 모든 타깃의 HTTP

`net481`, .NET 8 또는 .NET 9 실행 파일을 모두 같은 방식의 Streamable HTTP로 연결합니다. 데모를 직접 시작하고, AI 클라이언트를 연결하기 전에 실행 상태를 확인합니다.

```powershell
Start-Process '<editor_remote_mcp 경로>\editor_remote_mcp.exe'
```

- 기본 수신 endpoint: `http://0.0.0.0:3712/mcp`
- 같은 PC의 클라이언트 endpoint: `http://127.0.0.1:3712/mcp`
- 인증: 데모 창에 표시되는 Bearer token

endpoint를 변경하려면 데모를 시작하기 전에 `[MCP0] URI`를 수정합니다. 고정 토큰을 사용하려면 `SIRIUS3_MCP_TOKEN`을 설정합니다. 여러 편집기를 연결할 때는 데모를 별도 프로세스로 실행하고 서로 다른 HTTP 포트를 사용하십시오.

## 연결 요약

| 대상 | 전송 방식 | 데모 실행 주체 | 인증 |
|---|---|---|---|
| 모든 타깃 (`net481`, `net8.0-windows7.0`, `net9.0-windows7.0`) | Streamable HTTP | 데모를 먼저 실행한 후 AI 클라이언트 실행 | `SIRIUS3_MCP_TOKEN`의 Bearer 토큰 |

모든 타깃에서 같은 HTTP 설정을 사용합니다.

## HTTP endpoint와 토큰 준비

HTTP 데모를 시작하고 표시된 endpoint/token을 클라이언트에 설정합니다. 아래 변수는 클라이언트 설정용이며 변경해도 이미 실행 중인 서버에는 반영되지 않습니다.

endpoint와 접근 모드는 `config.ini`에서 정하고, 토큰은 자동 생성 값을 사용하거나 `SIRIUS3_MCP_TOKEN`으로 전달할 수 있습니다. 편집기마다 별도 프로세스와 수신 포트를 사용하십시오.

선택한 데모를 시작하고 실행 상태를 확인합니다. 창에 표시된 endpoint와 token을 클라이언트 설정에 복사합니다. 표시된 호스트가 `0.0.0.0`이면 같은 PC에서는 `127.0.0.1`, 다른 PC에서는 서버의 실제 IP로 바꿉니다. 아래 환경 변수는 AI 클라이언트에 자격 증명을 전달하는 용도이며 서버는 이 변수를 읽지 않습니다.

```powershell
$endpoint = 'http://127.0.0.1:3712/mcp'
$token = Read-Host 'Token from the demo window'
$env:SIRIUS3_MCP_TOKEN = $token
```

새 옵션 인스턴스는 새 토큰을 생성합니다. 재시작 후 같은 토큰을 사용하려면 호출자가 보관한 값을 `HttpToken`에 대입합니다. 그렇지 않으면 데모 재시작 후 클라이언트 토큰을 갱신합니다. 토큰에는 공백이나 제어 문자를 포함할 수 없습니다. endpoint를 바꾸면 실제 표시된 URL을 사용하며, 포트 0을 명시하면 사용 가능한 포트를 선택합니다.

## 1. Claude (Claude Code)

Claude Code는 프로젝트의 `.mcp.json`에서 MCP 설정을 읽습니다. `claude`를 실행할 디렉터리에 다음 설정을 넣으십시오.

### HTTP

데모를 먼저 실행하고 위 환경 변수 준비 명령을 수행한 뒤 다음 설정을 사용합니다.

```json
{
  "mcpServers": {
    "editor_remote_mcp": {
      "type": "http",
      "url": "http://127.0.0.1:3712/mcp",
      "headers": {
        "Authorization": "Bearer ${SIRIUS3_MCP_TOKEN}"
      }
    }
  }
}
```

Claude Code에서 `/mcp`를 실행해 `editor_remote_mcp`가 연결되었는지 확인하십시오. 이 서버에는 `bypassPermissions`를 사용하지 마십시오.

## 2. ChatGPT 데스크톱 앱 / Codex

Windows용 ChatGPT 데스크톱 앱, Codex CLI, Codex IDE 확장은 같은 호스트에서 `%USERPROFILE%\.codex\config.toml`을 공유합니다. 이 문서에서는 앱 버전에 따라 달라질 수 있는 설정 화면 대신 해당 파일을 직접 편집합니다.

1. ChatGPT 데스크톱 앱을 완전히 종료합니다.
2. `%USERPROFILE%\.codex\config.toml`을 엽니다. 파일이 없으면 `.codex` 폴더와 `config.toml` 파일을 만듭니다.
3. 기존 `[mcp_servers.editor_remote_mcp]` 정의가 있으면 제거하거나 비활성화합니다.
4. 아래 HTTP 설정을 추가하고 파일을 저장합니다.
5. ChatGPT 데스크톱 앱을 다시 시작하고 새 대화에서 `/mcp`를 입력해 연결을 확인합니다.

### HTTP

먼저 HTTP 데모를 실행하고 창에 표시된 토큰을 복사합니다. ChatGPT 데스크톱 앱이 재시작 후 읽을 수 있도록 토큰을 Windows 사용자 환경 변수에 저장합니다.

```powershell
[Environment]::SetEnvironmentVariable(
  'SIRIUS3_MCP_TOKEN',
  '<데모 창에 표시된 토큰>',
  'User')
```

그런 다음 `%USERPROFILE%\.codex\config.toml`에 다음 설정을 추가합니다. 데모 창에 다른 endpoint가 표시되면 URL을 바꿉니다.

```toml
[mcp_servers.editor_remote_mcp]
url = "http://127.0.0.1:3712/mcp"
bearer_token_env_var = "SIRIUS3_MCP_TOKEN"
default_tools_approval_mode = "writes"
```

파일이나 사용자 환경 변수를 변경한 뒤 ChatGPT 데스크톱 앱을 완전히 종료하고 다시 시작합니다. 토큰은 Windows 사용자 환경 변수에 남으므로 더 이상 필요하지 않으면 `[Environment]::SetEnvironmentVariable('SIRIUS3_MCP_TOKEN', $null, 'User')`로 제거합니다. Codex CLI 사용자는 `codex.cmd mcp list`로 공유 설정을 추가 확인할 수 있습니다.

## 3. Gemini (Gemini CLI)

Gemini CLI는 MCP 정의를 `settings.json`에 저장합니다. 이 서버에는 `--trust`를 전달하지 말고 도구 확인을 유지하십시오. PowerShell이 npm PowerShell shim을 차단하면 `gemini` 대신 `gemini.cmd`를 사용합니다.

### HTTP

데모를 실행하고 PowerShell 준비 블록을 먼저 수행한 뒤 다음 명령을 실행합니다.

```powershell
gemini.cmd mcp add --scope user --transport http `
  --header "Authorization: Bearer $token" `
  editor_remote_mcp $endpoint
```

Gemini는 확장된 헤더 값을 사용자 설정에 저장합니다. `~/.gemini/settings.json`을 보호하고 토큰이 바뀌면 서버를 제거한 뒤 다시 추가하십시오. `gemini.cmd mcp list`를 실행하고 새 Gemini 세션에서 `/mcp list`로 확인합니다. `trust`는 기본값 `false`로 유지하십시오.

## 4. GitHub Copilot

기본 예제는 VS Code의 GitHub Copilot Agent 모드입니다. 명령 팔레트에서 `MCP: Open User Configuration`을 실행하고 다음 서버 정의를 유지하십시오.

### HTTP

데모를 먼저 실행하십시오. 다음 설정은 Bearer 토큰을 한 번 물어보고 VS Code의 보호된 입력값으로 저장합니다.

```json
{
  "inputs": [
    {
      "type": "promptString",
      "id": "sirius3-mcp-token",
      "description": "Sirius3 MCP bearer token",
      "password": true
    }
  ],
  "servers": {
    "editor_remote_mcp": {
      "type": "http",
      "url": "http://127.0.0.1:3712/mcp",
      "headers": {
        "Authorization": "Bearer ${input:sirius3-mcp-token}"
      }
    }
  }
}
```

데모에 표시된 토큰 값만 붙여 넣고 `Bearer `는 넣지 마십시오. `MCP: List Servers`에서 `editor_remote_mcp`를 시작한 뒤 Copilot Chat을 Agent 모드로 사용합니다. Sirius3 도구를 선택하되 문서 쓰기는 매번 승인하고 장치 도구를 영구 자동 승인하지 마십시오.

Visual Studio 2022 17.14 이상도 GitHub Copilot MCP를 지원합니다. Copilot Chat의 Agent 모드에서 Tools, `+`, **Add custom MCP server**를 차례로 선택하고 화면에 표시된 HTTP URL을 사용하십시오. Visual Studio는 `%USERPROFILE%\.mcp.json`의 사용자 설정도 검색합니다. 각 도구 승인을 검토해야 하며 Bearer 헤더 입력이 필요할 때는 위 VS Code JSON을 기준으로 사용할 수 있습니다.

## 지원되는 문서 및 장치 작업

연결된 AI 클라이언트는 편집기 상태 확인, 문서 열기·가져오기·편집·저장, 개체 생성과 수정, 가공 경로 Preview, 등록된 장치 제어를 수행할 수 있습니다. 사용자는 원하는 결과를 일반적인 말로 요청하면 되고, 클라이언트가 연결된 데모에서 사용할 수 있는 도구와 인자를 확인합니다.

호스트가 `AllowedFileRoots`를 설정한 경우, 열기와 가져오기에 사용할 파일은 그 루트 아래에 실제로 있어야 합니다. 저장 파일은 허용된 기존 폴더에 `.sirius3` 확장자로 지정해야 합니다. 저장하지 않은 변경을 버리거나 기존 파일을 덮어쓰려면 명시적인 확인이 필요합니다.

장치 상태를 변경하는 작업에는 위험 작업에 대한 명시적인 확인이 필요합니다. 일반 장치 제어 기능은 설정된 스캐너, 레이저, 디지털 I/O, 파워미터, 파워맵에 접근할 수 있으므로 영구 자동 승인하지 마십시오.

## 바로 사용할 수 있는 프롬프트 예제 10가지

아래 예제는 사용자가 원하는 작업만 일반적인 말로 설명합니다. AI 클라이언트가 연결된 편집기를 확인하고 필요한 Sirius3 도구와 인자를 스스로 선택하므로, 사용자는 내부 도구명이나 편집기 ID를 알 필요가 없습니다. 문서 변경 요청과 장치 작업은 실행 전에 내용을 검토하십시오. 특히 Preview와 실제 가공을 실행하기 전에는 등록된 장치와 실제 시스템의 안전 상태를 사용자가 직접 확인해야 합니다. AI 답변은 장비 안전을 보증하지 않습니다.

1. 연결된 편집기의 현재 상태 확인

   ```text
   연결된 편집기의 현재 상태를 알려줘. 활성 페이지, 열린 파일 이름, 저장하지 않은 변경 여부, 개체 수, 연결된 스캐너·레이저·마커 종류를 요약하고 아무것도 변경하지 마.
   ```

2. Sirius3 문서 열기

   ```text
   연결된 편집기에서 <허용된-경로>\sample.sirius3 파일을 열어줘. 저장하지 않은 변경이 있으면 기존 내용을 버리지 말고 먼저 나에게 확인해. 완료되면 열린 파일 이름과 문서 상태를 알려줘.
   ```

3. 외부 도면 가져오기

   ```text
   현재 문서의 활성 레이어로 <허용된-경로>\sample.dxf 파일을 가져와줘. 기존 문서 내용은 유지하고, 완료되면 새로 추가된 개체와 가져오기 결과를 알려줘.
   ```

4. 사각형 개체 추가

   ```text
   활성 레이어에 중심이 X=0 mm, Y=0 mm이고 크기가 10 mm x 10 mm인 사각형을 하나 추가해줘. 이름은 MCP Rectangle로 지정하고, 추가된 개체의 정보를 알려줘.
   ```

5. 원과 텍스트 개체 추가

   ```text
   활성 레이어에 중심이 X=15 mm, Y=0 mm이고 반지름이 5 mm인 원을 추가해줘. 그 아래에는 "Sirius3 MCP"라는 텍스트도 추가하고, 생성된 개체들을 요약해줘.
   ```

6. 개체 이름 변경

   ```text
   활성 레이어에서 이름이 MCP Rectangle인 개체를 찾아 이름을 Main Rectangle로 변경해줘. 변경 전후 값을 확인해서 알려줘.
   ```

7. 문서 저장

   ```text
   현재 문서를 <허용된-경로>\mcp-result.sirius3 파일로 저장해줘. 같은 이름의 파일이 이미 있으면 덮어쓰기 전에 먼저 나에게 확인하고, 완료되면 저장된 파일 이름과 문서 상태를 알려줘.
   ```

8. 가공 경로 Preview

   ```text
   연결된 장치와 현재 가공 상태를 먼저 확인해줘. 안전하게 Preview할 준비가 되었으면 어떤 장치를 사용할지 알려주고 내 승인을 기다린 다음, 가공 경로 Preview를 한 번 실행하고 결과를 보고해.
   ```

9. Page1 가공 시작

   ```text
   Page1 가공을 시작할 준비가 되었는지 연결된 레이저, 스캐너와 현재 가공 상태를 확인해줘. 실제 장비가 동작할 수 있으므로 내 명시적인 승인을 기다린 다음 한 번만 시작하고, 자동으로 재시도하지 말고 결과를 알려줘.
   ```

10. 진행 중인 가공 중지

    ```text
    연결된 편집기에서 진행 중인 가공을 즉시 한 번만 중지해줘. 다른 장치 명령은 실행하지 말고, 중지 후 가공이 완전히 종료되었는지 상태를 확인해서 알려줘.
    ```

개체 삭제, 그룹/그룹 해제, 변환, 해치 편집, hit-test, point/grid cloud, Editor 시뮬레이션 시작/종료도 사용할 수 있습니다. Preview는 가공 경로 작업이며 Editor 시뮬레이션과 별개입니다. 반환 결과와 후속 상태 확인 없이 AI 응답만으로 성공했다고 판단하지 마십시오.

## 문제 해결

- `Connection refused` 또는 `error sending request`: HTTP 데모가 실행되지 않았거나, 포트가 URL과 다르거나, 다른 프로세스가 포트를 사용 중입니다.
- `401 Unauthorized`: 클라이언트가 `SIRIUS3_MCP_TOKEN`을 상속하지 못했거나 헤더 형식이 잘못되었거나 저장된 토큰이 오래된 값입니다. 인증 거부는 Warning, 인증 성공은 Information으로 원격 주소·메서드·경로와 함께 기록됩니다. 수정 후 클라이언트를 다시 시작하십시오.
- `--url <URL> ... none was supplied`: `$endpoint`가 비어 있거나 일반 URL 대신 Markdown으로 설정되었습니다. 위 PowerShell 준비 블록을 그대로 다시 실행하십시오.
- `Transport closed`: 남아 있는 `editor_remote_mcp.exe`를 종료하고 Sirius3 프로세스가 하나뿐인지 확인한 뒤 새 AI 세션을 여십시오.
- Copilot에서 도구가 보이지 않음: Agent 모드를 사용하고 `MCP: List Servers`에서 서버를 시작/재시작한 뒤 MCP 출력을 확인하십시오.

대상 프레임워크나 AI 클라이언트를 바꾸기 전에 AI 연결과 데모 창을 모두 종료하십시오.

## 공식 클라이언트 문서

- [Claude Code: MCP 도구 연결](https://code.claude.com/docs/en/mcp)
- [OpenAI Codex: Model Context Protocol](https://developers.openai.com/codex/mcp/)
- [Gemini CLI: MCP 서버](https://geminicli.com/docs/tools/mcp-server/)
- [VS Code: MCP 설정 참조](https://code.visualstudio.com/docs/agents/reference/mcp-configuration)
- [Visual Studio: GitHub Copilot에서 MCP 서버 사용](https://learn.microsoft.com/en-us/visualstudio/ide/mcp-servers?view=visualstudio)
