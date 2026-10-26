# Sirius3 MCP Editor Demo: AI client setup

> API reference: Sirius3 Core/UI 1.17.0 (public features)

[English](MCP.md) | [한국어](MCP.koKR.md) | [All guides](README.md)

`editor_remote_mcp` is a public WinForms demo that connects one Sirius3 editor to an MCP-compatible AI client. It registers the devices configured in `config.ini` and starts the MCP service from its startup code and displays its status, endpoint, and access token.

Every supported target, including `net481`, uses authenticated Streamable HTTP. The demo reads its endpoint, access mode, and documentation root from `[MCP0]` in `config.ini`. `SIRIUS3_MCP_TOKEN` may supply a fixed token; otherwise a new token is generated for each run. HTTP listens on `http://0.0.0.0:3712/mcp` by default. Use `http://127.0.0.1:3712/mcp` from the same computer or replace the host with the server's reachable IP. STDIO is not supported.

One running demo instance represents one editor and one MCP server. For several editors on one computer, host them in one Sirius3 application and assign distinct server ports. Close the previous demo before starting another instance. Device capabilities follow `config.ini`. File open/import/save and file-backed entity factories default to `AllowedFileRoots = { "*" }`, allowing every local or network path accessible to the host process. The host may clear or replace `AllowedFileRoots` before server creation; an empty list disables file access. `[MCP0]` does not configure file roots.

Client cancellation removes requests that are still queued for the editor UI. It does not stop a marking job or undo an action that already started; use an explicit stop request and check the resulting state.

Barcode cell settings can be changed individually while preserving the other settings and Undo/Redo. For example: “Set this DataMatrix's circular cell radius factor to 0.65 and keep its other cell settings.” Invalid value types, out-of-range numbers and unsupported settings return a validation error without changing the requested property.

Entity edits use the current active layer. Missing or stale entity IDs, pen/layer/device IDs and non-writable properties return validation errors without changing the document; refresh the entity list before retrying.

## Demo server configuration

Use `[MCP0]` in `config.ini` to configure the HTTP endpoint, access mode, and SCANLAB documentation root. The current `MCPDemoHelper` starts the server without reading `ENABLE`; that key does not disable this demo server. The demo applies environment settings first; a configured `[MCP0] URI` overrides the endpoint environment variable. `SIRIUS3_MCP_TOKEN` may supply the fixed server token. The client's `bearer_token_env_var = "SIRIUS3_MCP_TOKEN"` reads a token from the client process; it does not configure a running server. The server and client must use matching credentials.

Permissions and the documentation root come from `config.ini`; file roots cannot be configured by this demo. The demo does not persist environment secrets.

Official setup and field definitions: [MCP](https://learn.chatgpt.com/docs/extend/mcp), [configuration reference](https://learn.chatgpt.com/docs/config-file/config-reference).

## Prepare the public demo

Use the executable produced from the public `demos/editor_remote_mcp` project or its distributed release package. Keep the executable, `config.ini`, and the supplied runtime/dependency files together. Select the output that matches the installed runtime: `net481`, `net8.0-windows7.0`, or `net9.0-windows7.0`.

Review `config.ini` before startup because it determines which scanner, laser, marker, digital I/O, and power-meter implementations the demo registers. Configure the server as follows. The setup steps below require only the public demo output and do not depend on a separate library source repository.

```ini
[MCP0]
URI=http://0.0.0.0:3712/mcp
ACCESS=All
DocumentationRoot=..\..\..\doc\SCANLAB
```

Relative `DocumentationRoot` paths are resolved from the directory containing the demo's `config.ini`; an absolute path is also accepted. If `URI` is omitted, `SIRIUS3_MCP_HTTP_ENDPOINT` can supply the endpoint.

`ACCESS` accepts `ReadOnly`, `DocumentControl`, `DeviceControl`, or `All`. The access mode can also be changed from the editor's MCP PropertyGrid while the server is running; the change applies to subsequent requests.

This guide connects the WinForms `editor_remote_mcp` demo to Claude, Codex, Gemini, and GitHub Copilot. HTTP configuration fields were checked against the official client references on 2026-10-19. Client UI labels may differ by installed version.

## HiDPI, WPF hosting, and shutdown

The WinForms demo uses Per-Monitor V2 and a 96-DPI `AutoScaleMode.Dpi` main form. Follow [WinForms](WinForms.md) for the modern .NET startup property and the separate `net481` config/manifest settings. WPF applications use the same authenticated HTTP API through the WPF `MCPFactory.CreateServer` overload; see [WPF](WPF.md).

In 1.15.0, await `IMCPServer.DisposeAsync()` before releasing its editor binding. If assigned to the editor's `MCPServer` property, `await editor.DisposeDevices()` handles the registered server and remote first, then the devices. Keep the UI event loop alive until cleanup finishes. The public demo cancels the first close, awaits cleanup, then closes again; do not replace this with a blocking wait.

## Safety first

This demo can enable document editing and registered-device control according to `[MCP0] ACCESS`. It can change the open document and operate the configured scanner, laser, digital I/O, power meter, and power map. File-backed operations follow the host's file-access policy; the default wildcard allows host-accessible paths, specific roots restrict them, and an empty list disables file access. Review every write or device request before approving it.

Only one Sirius3 process may run at a time. Close every previous Sirius3 demo, MCP client-spawned `editor_remote_mcp.exe`, and debugging session before starting another target or client.

## HTTP on every target

Connect the `net481`, .NET 8, or .NET 9 executable through Streamable HTTP in the same way. Start the demo yourself, then confirm that it is running before connecting the AI client.

```powershell
Start-Process '<path-to-editor_remote_mcp>\editor_remote_mcp.exe'
```

- Default listen endpoint: `http://0.0.0.0:3712/mcp`
- Same-computer client endpoint: `http://127.0.0.1:3712/mcp`
- Authentication: the Bearer token displayed by the demo

Change `[MCP0] URI` before starting the demo to use another endpoint. Set `SIRIUS3_MCP_TOKEN` to use a fixed token. For multiple editors on one computer, use one application with separate editor/server instances and distinct ports.

## Connection summary

| Target | Transport | Who starts the demo? | Authentication |
|---|---|---|---|
| Every target (`net481`, `net8.0-windows7.0`, `net9.0-windows7.0`) | Streamable HTTP | Start the demo first, then start the AI client | Bearer token in `SIRIUS3_MCP_TOKEN` |

Use the same HTTP setup on every target.

## Prepare the HTTP endpoint and token

Start the HTTP demo and copy its displayed endpoint/token into the client. The following variable configures the client; changing it does not update an already running server.

The endpoint and access mode come from `config.ini`; the token may remain automatically generated or be supplied through `SIRIUS3_MCP_TOKEN`. Use distinct listening ports for multiple servers within one Sirius3 application.

Start the selected demo and confirm that it is running. Copy its displayed endpoint and token into the client configuration. Replace a displayed `0.0.0.0` host with `127.0.0.1` on the same computer or the server's reachable IP. Setting the following variable in the client process does not change a running server. The demo also reads `SIRIUS3_MCP_TOKEN` from its own environment during startup through `ApplyEnvironment()`.

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

The shared configuration for the same Codex host is also documented by [OpenAI Docs](https://learn.chatgpt.com/docs/extend/mcp). The Codex HTTP fields and shared configuration location were rechecked on 2026-10-19.

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

With specific `AllowedFileRoots`, open/import files must exist under an allowed root. The default `"*"` also allows other host-accessible local and network paths; an empty list disables file access. Saved files must use the `.sirius3` extension in an existing allowed directory. Discarding unsaved changes and overwriting an existing file require explicit confirmation.

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

## Edit grips and source vertices

Move includes position and supported X/Y/Z rotations; Vertex exposes source
vertices/controls, and Size exposes dimensions/model scaling and scale reset.
Inspect an object's Move, Vertex and Size grips, then change a source vertex,
native size or rotation with Undo/Redo. Supported shapes, barcodes and raster
objects retain native Width/Height/radius parameters and existing model scale.
Coordinates use the object's original local frame; world positions include
object and parent transforms. Requests use exact coordinates, without screen
picking or camera-dependent magnetic/grid snapping.

Polyline2D/3D and spline source vertices can be inserted or removed by index.
Insertion uses an interior fraction of a source/control-polygon segment;
polyline arcs preserve Bulge, while spline control insertion can change shape.
Minimum point counts and related spline data are retained. Each accepted edit
is one Undo/Redo action. Locked, busy or simulating documents reject changes.
Prepare pending text/image source changes in the editor before grip operations.

```text
Inspect this object's editing grips. Increase its native width by 25% while keeping its model scale and opposite edge fixed. Verify the dimensions, undo, and verify the original values.
```

```text
Insert a vertex halfway along the first source segment of this polyline. Check its arc and machining order, then undo and confirm the original vertices return.
```

## Clip selected entities

Clipping follows each entity's supported geometry. Partial cuts of spatial meshes
or stitched images are unsupported. A layer containing such an intersected source
fails as a whole and keeps the original document; choose supported sources instead.

Clip supports paths or closed regions for entities implementing `IClippable`, including barcodes, text, spirals and block inserts. Existing HatchLine/HatchPolygon machining paths can be included or excluded. Results are added to the document with Undo/Redo; source removal is optional.

Select an EntityLayer alone when clipping its child data. Mixing a layer with its children, other entities or another layer is rejected.

```text
Clip the selected objects to the world-coordinate rectangle from (-5, -5) to (5, 5). Create closed regions with EvenOdd fill and outer counterclockwise/hole clockwise winding. Include existing hatch paths, keep the originals, and check the resulting objects.
```

## Requests for new imports, contours and PDF pages

SVG, 3MF, PDF and offline G-code are prepared on a worker and added to the active layer with Undo/Redo. File operations require host-configured allowed roots. Import settings and power-map interpolation are shared by every editor in the process and are outside document Undo/Redo. Import-setting changes do not alter existing paths.

- SVG: “Read the current import settings, enable automatic line hatching for new SVG imports at 0.1 mm, and import <allowed-path>\logo.svg as editable vectors. Keep the existing document contents and verify the result.”
- 3MF: “Import <allowed-path>\part.3mf into the active layer. Check millimeter units and relative component placement, and report any unsupported materials or non-mesh data.”
- G-code: “Set the dialect for new imports to Laser and helical-arc tolerance to 0.001 mm, then import <allowed-path>\path.gcode as offline paths. Report source line ranges and F/S/T attributes. Do not send commands to physical devices.”
- PDF: “Add page 2 of <allowed-path>\label.pdf as an image at 20 mm height and 150 DPI. Check the page count and applied values. Change to page 3, then undo and verify that the previous image and page number return.”
- Contour: “Add an open contour with two independently owned lines from (0, 0) to (10, 0), then to (10, 10). Keep existing entities and verify the connection.”
- Power map: “Read the current power-map interpolation mode and change the process-wide setting to Pchip. Do not mark, measure or issue device output; verify the applied setting only.”

PDF page numbers start at 1 and resolution is 36–600 DPI. Page changes require the original PDF under an allowed root; failure retains the previous image. Full source paths are neither returned nor editable. SVG supports solid fill/stroke outlines, excluding text, images, masks and filters. G-code F/S/T are source metadata, not pen/device settings. `.nc`/`.tap` retain the existing PCB import route.

Font Editor public API help is searchable. The separate font editor's glyph editing, TTF conversion and save sessions are not MCP connection targets.

## Troubleshooting

- `Connection refused` or `error sending request`: the HTTP demo is not running, the port differs from the configured URL, or another process owns the port.
- `401 Unauthorized`: the client did not inherit `SIRIUS3_MCP_TOKEN`, the header is malformed, or a stored token is stale. Rejected authentication is recorded as a Warning and accepted authentication as Information, with the remote address, method, and path. Restart the client after correcting it.
- `--url <URL> ... none was supplied`: `$endpoint` is empty or was set to Markdown instead of a plain URL. Re-run the exact PowerShell preparation block.
- `Transport closed`: close stale `editor_remote_mcp.exe` processes, make sure only one Sirius3 process is active, and open a fresh AI session.
- Copilot cannot see tools: use Agent mode, run `MCP: List Servers`, start/restart the server, and inspect its MCP output.

Close the AI client connection and the demo window before changing target frameworks or clients.

## Official client references

- [Claude Code: Connect Claude Code to tools via MCP](https://code.claude.com/docs/en/mcp)
- [OpenAI Codex: Model Context Protocol](https://learn.chatgpt.com/docs/extend/mcp)
- [Gemini CLI: MCP servers](https://geminicli.com/docs/tools/mcp-server/)
- [VS Code: MCP configuration reference](https://code.visualstudio.com/docs/agents/reference/mcp-configuration)
- [Visual Studio: Use MCP servers with GitHub Copilot](https://learn.microsoft.com/en-us/visualstudio/ide/mcp-servers?view=visualstudio)

## Document structure and font updates

Ask to inspect the page/layer structure, processing pen settings and named blocks before changing their public properties. Pen changes use document actions and Undo/Redo; they do not send commands to a device.

Ask to create a polyline using positions, bulges and ramp factors, or inspect hit-test coordinates and vertex indexes. Vertex positions are in mm; ramp factors range from 0 to 4 and default to 1.

Ask which Sirius font files are available, then reload one after external editing. Font operations require DocumentControl and follow the same file-access policy; a specific-root policy must include the font directory. Reloading affects matching live text in every editor in this process and is outside document Undo/Redo. Font Editor saves already refresh automatically.

## File access and Font Editor

| `AllowedFileRoots` | File access |
|---|---|
| Default `"*"` | Any path accessible to the host process, including network shares |
| Specific folders | Those folders and descendants only |
| Empty list | Disabled |

The mode, file existence, extension, destination and overwrite checks still apply.
The host sets this policy before creating the server; Windows permissions still
limit access. Use [Font Editor](FontEditorUserManual.md) for interactive glyph
editing and TTF conversion; it is not an MCP editor binding.

## Registered device capabilities and Script data

Ask which operations the connected scanner, laser and marker actually support,
or inspect cached meter readings, calibration points and marker progress without
starting a device. Entity queries also provide prepared vertices, hatch settings,
cached converted text, contour topology and model-space slices in bounded pages.
Slice Z uses mm after the entity's model transform and excludes parent transforms.

Ask to change the registered script's lot/operator data, scalar variables or
serial counters. These changes are outside document Undo/Redo and affect later
text conversion or marking. Serial helpers retain the script's own increment
policy. C# evaluation, source edits and .cs/.dll loading are unavailable.

Device mutations need DeviceControl and operator confirmation; busy-state checks
remain active. Ask to save or open calibration data only in allowed folders,
confirming replacement or overwriting first. A successful command submission
does not establish physical completion; inspect device status and events.
