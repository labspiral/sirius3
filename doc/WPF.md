# Sirius3 WPF integration guide

> API reference: Sirius3 Core/UI 1.17.0 (public features)

[English](WPF.md) | [한국어](WPF.koKR.md) | [WinForms](WinForms.md) | [All guides](README.md)

## 1. Packages and demo projects

WPF editor controls ship in `SpiralLab.Sirius3.UI`; install compatible published `SpiralLab.Sirius3`, `SpiralLab.Sirius3.UI`, and `SpiralLab.Sirius3.Dependencies` packages. Both WinForms and WPF are available in the distributed UI assembly. Sirius3 1.17.0 provides native WPF editor/device panels, Grip editing and PDF page import, together with asynchronous remote/MCP cleanup and mesh texture rendering.

The public WPF demos require Visual Studio 2022 or later and target `net481;net8.0-windows;net9.0-windows`. The library additionally supports `net10.0-windows`, which is not a current demo target.

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

| Demo | Entry files and purpose |
|---|---|
| [wpf_editor_ui](../demos/wpf_editor_ui/MainWindow.xaml) | `App.xaml.cs`, `MainWindow.xaml`, `MainWindow.xaml.cs`; editable integrated editor sources in namespace `Demos` |
| [wpf_editor_multiple](../demos/wpf_editor_multiple/MainWindow.xaml.cs) | Multiple editor views and their device/document lifetimes |
| [wpf_editor_multiple2](../demos/wpf_editor_multiple2/MainWindow.xaml.cs) | Indexed device sets in a `SiriusMultiEditorControl` |
| [wpf_editor_entity](../demos/wpf_editor_entity/MainWindow.xaml.cs) | Entity creation and the WPF document editing workflow |

Keep the Dependencies package's runtime files with the output. Select an installed runtime and review `config*.ini` before launch. One Sirius3 process should own the configured devices at a time.

The public APIs provide SVG vector and 3MF mesh import, `IDocument.ActClip` clipping, and global Linear/PCHIP interpolation through `Config.PowerMapInterpolationMode`. Public demos retain `Version="1.*"` references; restore NuGet packages after an update and check the resolved version. See [document import](DocumentUserManual.md), [clipping](ClippingUserManual.md), and [PowerMap](PowermapUserManual.md).

When copying `wpf_editor_ui`, keep `SiriusEditorControl.xaml`/`.xaml.cs`, `SiriusMultiEditorControl.cs`, and shared `EditorControlSupport.cs` together. Public source keeps namespace `Demos` and NuGet references, and uses public APIs in place of library-internal methods.

## 2. DPI awareness and layout

The executable declares Per-Monitor V2 before creating windows. Copy the complete [demo manifest](../demos/wpf_editor_ui/app.manifest), including its Windows 10 compatibility declaration. Its DPI settings include:

```xml
<application xmlns="urn:schemas-microsoft-com:asm.v3">
  <windowsSettings>
    <dpiAware xmlns="http://schemas.microsoft.com/SMI/2005/WindowsSettings">true/pm</dpiAware>
    <dpiAwareness xmlns="http://schemas.microsoft.com/SMI/2016/WindowsSettings">PerMonitorV2,system</dpiAwareness>
  </windowsSettings>
</application>
```

WPF layout uses device-independent units, with 96 units per logical inch. Use `Grid`, automatic rows/columns, alignment, and margins to express layout. `AutoScaleMode`, `AutoScaleDimensions`, and `ApplicationHighDpiMode` are WinForms settings and are not used to configure WPF windows. Do not copy the WinForms-specific Framework application-configuration section into a WPF application.

Use `UseLayoutRounding="True"` and `SnapsToDevicePixels="True"` where appropriate for crisp layout. The Sirius3 WPF OpenGL surface handles its framebuffer dimensions separately from XAML layout. Avoid applying another scale transform to compensate for monitor DPI. Check the editor viewport, native dialogs, and pages first displayed after monitor changes.

## 3. Host an editor

Package controls use namespace `SpiralLab.Sirius3.UI.WPF`:

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

The public-source `wpf_editor_ui` controls use namespace `Demos`; use its local XAML namespace when customizing those copies. Sirius3 controls initialize their required theme resources themselves; a package consumer does not need to call `WPFThemeManager.Initialize()` separately.

Call `Core.Initialize()` before creating the Window and editor. Set the UI culture before `InitializeComponent()`. Pair successful Core initialization with one `Core.Cleanup()` after window/device cleanup. The public `App.xaml.cs` and `MainWindow.xaml.cs` show this application/window ownership split.

## 4. Devices and editing workflow

Read `config*.ini` and create and initialize the devices through the public [helper.cs](../demos/helper.cs) factory flow. Use virtual scanner, laser, and power-meter implementations when checking UI without equipment. In `Loaded`, guard initialization so unloading and reloading a page does not create another device set.

After successful initialization, register the whole set on the window Dispatcher. The following fragment assumes initialized, host-owned device instances:

```csharp
editor.RegisterDevices(rtc, laser, powerMeter,
    diExt1, diLaserPort, doExt1, doExt2, doLaserPort, marker);
if (!marker.IsReady)
    throw new InvalidOperationException("Marker is not ready.");
```

Registration connects the document and view and calls `Marker.Ready`. Check readiness and logs rather than calling it again. For the multi-editor, set `MaxDeviceCounts`, register each set with `RegisterDevices(index, ...)`, then call `SwitchDevices(index)`.

The editor, TreeView, and PropertyGrid share selection and document actions. Select an entity before editing it. EntityPen and EntityLayerPen have different roles: entity conditions control individual paths; layer conditions apply to the layer execution stage. Undo/Redo operates through document actions.

The Scanner UI shows Multi-Beam navigation only for an assigned `IRtcMultiBeam`. The Laser UI selects built-in virtual/vendor panels or a registered custom WPF panel. Use `UI.Config.OnCreateLaserUIWPF`, `OnCreateScannerUIWPF`, and `OnCreateMarkerUIWPF` for native WPF customization; WinForms customization events return different control types.

Mouse and keyboard commands follow focus. Directional keys remain available for tree navigation when the TreeView has focus. Marker Start shortcuts can operate configured equipment; see [editor shortcuts](EditorUserManual.md) and [TreeView shortcuts](TreeviewUserManual.md).

## 5. MCP on WPF

The same `SpiralLab.Sirius3.MCP` API provides a WPF editor overload. Register devices before creating the server. In an asynchronous UI-thread method, with an initialized editor:

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

The options generate an authentication token; configure the AI client with the server's effective endpoint and token. The server is assigned before startup so the host's failure/closing path can dispose it through the editor. Availability follows the Remote license option. Use [MCP.md](MCP.md) for HTTP client settings; its demo host is WinForms, while transport and authentication also apply to WPF.

## 6. Asynchronous shutdown and ownership

`Unloaded` pauses display/timer work; it is not final device disposal. Close the owning Window through the asynchronous `Closing` pattern in [MainWindow.xaml.cs](../demos/wpf_editor_ui/MainWindow.xaml.cs):

1. Cancel the first close and reject repeated cleanup requests.
2. Yield back to the event loop and await `editor.DisposeDevices()` on the Dispatcher.
3. Dispose the editor's UI/OpenGL resources, then dispose the host-owned document once.
4. Allow the final close. The application calls `Core.Cleanup()` after the Window lifetime ends.

In 1.15.0, `DisposeDevices()` returns `Task`; both single and multi-editor controls await registered MCP/remote cleanup before releasing devices. Separately owned servers or remotes require their own `await DisposeAsync()` before their bound editor or marker is released. Do not block the Dispatcher with `.Wait()` or `.Result`.

WPF controls do not automatically dispose assigned documents or devices. If multiple windows share a document, detach every user before the owning host disposes it. Log failures and continue cleanup of remaining owned resources.

## 7. Rendering and validation

WinForms and WPF use the same document/entity model and support 1.15.0 Mesh materials and diffuse textures. OBJ imports need the referenced MTL/image files; PLY texture imports need UV data and a valid texture reference. These appearances are for visualization; machining still uses configured paths and pens. See [entities](EntityUserManual.md) and [rendering](ViewUserManual.md).


Check cold startup and monitor transitions at representative DPI scales, tree focus, camera interaction, virtual/vendor panel selection, and closing while remote work is pending. Compilation alone does not verify GPU, monitor, license, or device behavior.

Related guides: [integrated editor](SiriusEditorUserManual.md), [multi-editor](SiriusMultiEditorUserManual.md), [UI configuration](Sirius3UIConfigUserManual.md), [WinForms](WinForms.md).

## Entity grips and Font Editor

The viewport supports Move/Vertex/Size with Tab and reverse cycling with Shift+Tab,
V/Shift+V grip navigation, G magnetic snapping and F/Ctrl+F Fit. Editing is
Undo/Redo-safe. Move includes whole-entity movement and supported axis rotation; Size edits dimensions or model scale. See [Editor](EditorUserManual.md) for input focus and grid rules.
Open **Font Edit** from the context menu for the separate font-file workflow;
see [Font Editor](FontEditorUserManual.md).

See the [editor guide](EditorUserManual.md) for current Grip modes, curve centers and keyboard input. [Font Editor](FontEditorUserManual.md) covers conversion; [Document](DocumentUserManual.md) covers PDF pages and page switching.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
