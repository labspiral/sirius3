# Sirius3 MCP Editor Demo: AI client setup

[English](MCP.md) | [한국어](MCP.koKR.md)

`editor_remote_mcp` is a public WinForms demo that connects one Sirius3 editor to an MCP-compatible AI client. It registers the devices configured in `config.ini` and, when `[MCP0] ENABLE=1`, starts the MCP service and displays its status, endpoint, and access token.

Every supported target, including `net481`, uses authenticated Streamable HTTP. The demo reads its endpoint, access mode, and documentation root from `[MCP0]` in `config.ini`. `SIRIUS3_MCP_TOKEN` may supply a fixed token; otherwise a new token is generated for each run. HTTP listens on `http://0.0.0.0:3712/mcp` by default. Use `http://127.0.0.1:3712/mcp` from the same computer or replace the host with the server's reachable IP. STDIO is not supported.

One running demo instance represents one editor and one MCP server. Run separate instances on different HTTP ports when multiple editors are needed. Device capabilities follow `config.ini`. File open/import/save and file-backed entity factories remain unavailable unless the host adds at least one `AllowedFileRoots` entry; `[MCP0]` does not configure file roots.

Client cancellation removes requests that are still queued for the editor UI. It does not stop a marking job or undo an action that already started; use an explicit stop request and check the resulting state.

Barcode cell settings can be changed individually while preserving the other settings and Undo/Redo. For example: “Set this DataMatrix's circular cell radius factor to 0.65 and keep its other cell settings.” Invalid value types, out-of-range numbers and unsupported settings return a validation error without changing the requested property.

Entity edits use the current active layer. Missing or stale entity IDs, pen/layer/device IDs and non-writable properties return validation errors without changing the document; refresh the entity list before retrying.

## Demo server configuration

Use `[MCP0]` in `config.ini` to enable the server and configure its HTTP endpoint, access mode, and SCANLAB documentation root. The demo applies environment settings first; a configured `[MCP0] URI` overrides the endpoint environment variable. `SIRIUS3_MCP_TOKEN` may supply the fixed server token. The client's `bearer_token_env_var = "SIRIUS3_MCP_TOKEN"` reads a token from the client process; it does not configure a running server. The server and client must use matching credentials.

Permissions and the documentation root come from `config.ini`; file roots cannot be configured by this demo. The demo does not persist environment secrets.

Official setup and field definitions: [MCP](https://learn.chatgpt.com/docs/extend/mcp), [configuration reference](https://learn.chatgpt.com/docs/config-file/config-reference).

## Prepare the public demo

Use the executable produced from the public `demos/editor_remote_mcp` project or its distributed release package. Keep the executable, `config.ini`, and the supplied runtime/dependency files together. Select the output that matches the installed runtime: `net481`, `net8.0-windows7.0`, or `net9.0-windows7.0`.

Review `config.ini` before startup because it determines which scanner, laser, marker, digital I/O, and power-meter implementations the demo registers. Enable and configure the server as follows. The setup steps below require only the public demo output and do not depend on a separate library source repository.

```ini
[MCP0]
ENABLE=1
URI=http://0.0.0.0:3712/mcp
ACCESS=All
DocumentationRoot=..\..\..\doc\SCANLAB
```

Relative `DocumentationRoot` paths are resolved from the directory containing the demo's `config.ini`; an absolute path is also accepted. If `URI` is omitted, `SIRIUS3_MCP_HTTP_ENDPOINT` can supply the endpoint.

`ACCESS` accepts `ReadOnly`, `DocumentControl`, `DeviceControl`, or `All`. The access mode can also be changed from the editor's MCP PropertyGrid while the server is running; the change applies to subsequent requests.

This guide connects the WinForms `editor_remote_mcp` demo to Claude, Codex, Gemini, and GitHub Copilot. The examples were checked against the official client documentation on 2026-09-14.

## Safety first

This demo can enable document editing and registered-device control according to `[MCP0] ACCESS`. It can change the open document and operate the configured scanner, laser, digital I/O, power meter, and power map. File-backed operations additionally require a host-configured allowlist. Review every write or device request before approving it.

Only one Sirius3 process may run at a time. Close every previous Sirius3 demo, MCP client-spawned `editor_remote_mcp.exe`, and debugging session before starting another target or client.

## HTTP on every target

Connect the `net481`, .NET 8, or .NET 9 executable through Streamable HTTP in the same way. Start the demo yourself, then confirm that it is running before connecting the AI client.

```powershell
Start-Process '<path-to-editor_remote_mcp>\editor_remote_mcp.exe'
```

- Default listen endpoint: `http://0.0.0.0:3712/mcp`
- Same-computer client endpoint: `http://127.0.0.1:3712/mcp`
- Authentication: the Bearer token displayed by the demo

Change `[MCP0] URI` before starting the demo to use another endpoint. Set `SIRIUS3_MCP_TOKEN` to use a fixed token. For multiple editors, run separate demo processes on different HTTP ports.

## Connection summary

| Target | Transport | Who starts the demo? | Authentication |
|---|---|---|---|
| Every target (`net481`, `net8.0-windows7.0`, `net9.0-windows7.0`) | Streamable HTTP | Start the demo first, then start the AI client | Bearer token in `SIRIUS3_MCP_TOKEN` |

Use the same HTTP setup on every target.

## Prepare the HTTP endpoint and token

Start the HTTP demo and copy its displayed endpoint/token into the client. The following variable configures the client; changing it does not update an already running server.

The endpoint and access mode come from `config.ini`; the token may remain automatically generated or be supplied through `SIRIUS3_MCP_TOKEN`. Use a separate process and listening port for each editor.

Start the selected demo and confirm that it is running. Copy its displayed endpoint and token into the client configuration. Replace a displayed `0.0.0.0` host with `127.0.0.1` on the same computer or the server's reachable IP. The following environment variable is only a way to pass credentials to AI clients; the server does not read it.

```powershell
$endpoint = 'http://127.0.0.1:3712/mcp'
$token = Read-Host 'Token from the demo window'
$env:SIRIUS3_MCP_TOKEN = $token
```

A new options instance generates a new token. To reuse a token across restarts, the caller must retain it and assign `HttpToken`. Otherwise update the client token after restarting the demo. Tokens must not contain whitespace or control characters. Use the actual displayed URL if you change the endpoint; port 0 explicitly selects an available port.

## 1. Claude (Claude Code)

Claude Code reads project MCP configuration from `.mcp.json`. Put the following block in the directory from which you start `claude`.

### HTTP

Start the demo first, set the environment variable as shown above, and use:

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

Run `/mcp` in Claude Code and confirm that `editor_remote_mcp` is connected. Do not use `bypassPermissions` for this server.

## 2. ChatGPT desktop app / Codex

On Windows, the ChatGPT desktop app, Codex CLI, and the Codex IDE extension on the same host share `%USERPROFILE%\.codex\config.toml`. This guide edits that file directly instead of relying on a Settings screen that may differ between app versions.

1. Fully quit the ChatGPT desktop app.
2. Open `%USERPROFILE%\.codex\config.toml`. If it does not exist, create the `.codex` folder and `config.toml` file.
3. Remove or disable any existing `[mcp_servers.editor_remote_mcp]` definition.
4. Add the following HTTP configuration and save the file.
5. Restart the ChatGPT desktop app, open a new chat, and use `/mcp` to confirm the connection.

### HTTP

Start the HTTP demo first and copy the token displayed in its window. Store that token in the Windows user environment so that the ChatGPT desktop app can read it after restarting:

```powershell
[Environment]::SetEnvironmentVariable(
  'SIRIUS3_MCP_TOKEN',
  '<token-from-demo-window>',
  'User')
```

Then add this configuration to `%USERPROFILE%\.codex\config.toml`. Replace the URL if the demo displays a different endpoint:

```toml
[mcp_servers.editor_remote_mcp]
url = "http://127.0.0.1:3712/mcp"
bearer_token_env_var = "SIRIUS3_MCP_TOKEN"
default_tools_approval_mode = "writes"
```

Fully quit and restart the ChatGPT desktop app after changing the file or user environment. The token remains in the Windows user environment; remove it with `[Environment]::SetEnvironmentVariable('SIRIUS3_MCP_TOKEN', $null, 'User')` when it is no longer needed. Codex CLI users can additionally verify the shared configuration with `codex.cmd mcp list`.

## 3. Gemini (Gemini CLI)

Gemini CLI stores MCP definitions in `settings.json`. Keep tool confirmation enabled: do not pass `--trust` for this server. Use `gemini.cmd` instead of `gemini` if PowerShell blocks its npm PowerShell shim.

### HTTP

Start the demo and run the PowerShell preparation block first, then:

```powershell
gemini.cmd mcp add --scope user --transport http `
  --header "Authorization: Bearer $token" `
  editor_remote_mcp $endpoint
```

Gemini saves this expanded header in its user settings. Protect `~/.gemini/settings.json`; if the token changes, remove and add the server again. Run `gemini.cmd mcp list`, then `/mcp list` inside a new Gemini session. Leave `trust` at its default `false`.

## 4. GitHub Copilot

The primary example is GitHub Copilot Agent mode in VS Code. Open the Command Palette and run `MCP: Open User Configuration`, then add this HTTP server definition.

### HTTP

Start the demo first. This configuration asks for the bearer token once and stores it as a protected VS Code input:

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

Paste only the token value shown in the demo, without `Bearer `. Run `MCP: List Servers`, start `editor_remote_mcp`, and use Copilot Chat in Agent mode. Select the Sirius3 tools but approve document writes individually; do not permanently auto-approve device tools.

Visual Studio 2022 17.14 or later also supports GitHub Copilot MCP servers. In Copilot Chat Agent mode, open Tools, select `+`, and choose **Add custom MCP server**. Use the displayed HTTP URL. Visual Studio discovers user configuration at `%USERPROFILE%\.mcp.json`; review each tool approval. The VS Code JSON above is the reference configuration when a bearer-header input is required.

## Supported document and device operations

The connected AI client can inspect editor state; open, import, edit, and save documents; create and modify entities; preview a processing path; and control registered devices. Ask for the intended result in ordinary language. The client discovers the available tools and their arguments from the connected demo.

When the host has configured `AllowedFileRoots`, open and import files must already exist below one of those roots. Saved files must use the `.sirius3` extension in an existing allowed directory. Discarding unsaved changes and overwriting an existing file require explicit confirmation.

Device-changing operations require explicit destructive-operation confirmation. Never permanently auto-approve the general device-control capability because it can reach the configured scanner, laser, digital I/O, power meter, and power map.

## Ten usable prompt examples

These examples describe only the user's intended task in ordinary language. The AI client discovers the connected editor and selects the required Sirius3 tools and arguments, so users do not need to know internal tool names or editor IDs. Review document changes and device operations before allowing them. Before Preview or actual processing, independently confirm the registered devices and the physical system's safety; an AI response is not proof that the equipment is safe.

1. Check the connected editor's current state.

   ```text
   Tell me the current state of the connected editor. Summarize the active page, open file name, unsaved changes, entity count, and connected scanner, laser, and marker types without changing anything.
   ```

2. Open a Sirius3 document.

   ```text
   Open <allowlisted-path>\sample.sirius3 in the connected editor. If the current document has unsaved changes, do not discard them; ask me first. When finished, report the opened file name and document state.
   ```

3. Import an external drawing.

   ```text
   Import <allowlisted-path>\sample.dxf into the active layer of the current document. Keep the existing document contents, then report the newly added entities and the import result.
   ```

4. Add a rectangle.

   ```text
   Add one rectangle to the active layer, centered at X=0 mm and Y=0 mm, with a size of 10 mm by 10 mm. Name it MCP Rectangle and report the created entity.
   ```

5. Add a circle and text.

   ```text
   Add a circle to the active layer with its center at X=15 mm and Y=0 mm and a radius of 5 mm. Also add the text "Sirius3 MCP" below it, then summarize the created entities.
   ```

6. Rename an entity.

   ```text
   Find the entity named MCP Rectangle in the active layer and rename it to Main Rectangle. Verify and report the value before and after the change.
   ```

7. Save the document.

   ```text
   Save the current document as <allowlisted-path>\mcp-result.sirius3. If a file with that name already exists, ask me before overwriting it. When finished, report the saved file name and document state.
   ```

8. Preview the processing path.

   ```text
   First check the connected devices and current processing state. If the system is ready for a safe Preview, tell me which devices will be used and wait for my approval. Then run the processing-path Preview once and report the result.
   ```

9. Start processing Page1.

   ```text
   Check whether the connected laser, scanner, and current processing state are ready to process Page1. Because physical equipment may operate, wait for my explicit approval, start only once, do not retry automatically, and report the result.
   ```

10. Stop active processing.

    ```text
    Stop any processing currently running in the connected editor exactly once. Do not issue other device commands. After stopping, check whether processing has fully ended and report the resulting state.
    ```

Entity deletion, grouping, transforms, hatch editing, hit tests, point/grid clouds, and Editor simulation start/stop are also available. Preview is a processing-path operation and is separate from Editor simulation. Never treat an AI response as proof of success without the returned result and a follow-up state check.

## Troubleshooting

- `Connection refused` or `error sending request`: the HTTP demo is not running, the port differs from the configured URL, or another process owns the port.
- `401 Unauthorized`: the client did not inherit `SIRIUS3_MCP_TOKEN`, the header is malformed, or a stored token is stale. Rejected authentication is recorded as a Warning and accepted authentication as Information, with the remote address, method, and path. Restart the client after correcting it.
- `--url <URL> ... none was supplied`: `$endpoint` is empty or was set to Markdown instead of a plain URL. Re-run the exact PowerShell preparation block.
- `Transport closed`: close stale `editor_remote_mcp.exe` processes, make sure only one Sirius3 process is active, and open a fresh AI session.
- Copilot cannot see tools: use Agent mode, run `MCP: List Servers`, start/restart the server, and inspect its MCP output.

Close the AI client connection and the demo window before changing target frameworks or clients.

## Official client references

- [Claude Code: Connect Claude Code to tools via MCP](https://code.claude.com/docs/en/mcp)
- [OpenAI Codex: Model Context Protocol](https://developers.openai.com/codex/mcp/)
- [Gemini CLI: MCP servers](https://geminicli.com/docs/tools/mcp-server/)
- [VS Code: MCP configuration reference](https://code.visualstudio.com/docs/agents/reference/mcp-configuration)
- [Visual Studio: Use MCP servers with GitHub Copilot](https://learn.microsoft.com/en-us/visualstudio/ide/mcp-servers?view=visualstudio)
