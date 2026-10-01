# Sirius3 WinForms integration guide

> Reference version: Sirius3 1.15.0 (public Release features)

[English](WinForms.md) | [한국어](WinForms.koKR.md) | [WPF](WPF.md)

## 1. Packages and demo projects

Install the published `SpiralLab.Sirius3`, `SpiralLab.Sirius3.UI`, and `SpiralLab.Sirius3.Dependencies` NuGet packages. Version 1.15.0 adds WinForms HiDPI support and asynchronous remote/MCP cleanup. Keep the Dependencies package's runtime files with the application output.

The public demos use Visual Studio 2022 or later and target `net481;net8.0-windows;net9.0-windows`. The library also supports `net10.0-windows`; that target is outside the current public demo configuration.

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
    <PackageReference Include="SpiralLab.Sirius3" Version="1.15.0" />
    <PackageReference Include="SpiralLab.Sirius3.UI" Version="1.15.0" />
    <PackageReference Include="SpiralLab.Sirius3.Dependencies" Version="1.15.0" />
  </ItemGroup>
</Project>
```

| Demo | Entry files and purpose |
|---|---|
| [beginner](../demos/beginner/Form1.cs) | `Program.cs`, `Form1.cs`, `Form1.Designer.cs`: basic editor and explicit device creation; inspect its scanner factory before running it |
| [editor_ui](../demos/editor_ui/Form1.cs) | `Form1` for one device set, `Form2` for multiple sets; editable control sources in namespace `Demos` |
| [editor_multiple2](../demos/editor_multiple2/Form1.cs) | Indexed device registration and switching |
| [editor_viewer](../demos/editor_viewer/Form1.cs) | Separate viewer windows sharing a document |
| [editor_remote_mcp](../demos/editor_remote_mcp/MainForm.cs) | Authenticated HTTP MCP server and asynchronous shutdown |

Package controls use namespace `SpiralLab.Sirius3.UI.WinForms`. The source examples in `editor_ui` use `Demos` and still consume NuGet packages.

## 2. Configure DPI before creating forms

On .NET 8/9, `ApplicationHighDpiMode=PerMonitorV2` is applied through the generated `ApplicationConfiguration.Initialize()`. Call it before constructing any form or control. Set the UI culture before creating localized controls.

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

This entry point owns Core initialization and cleanup. Its form must finish asynchronous device cleanup before allowing closure; do not duplicate Core initialization or cleanup in the form.

For `net481`, add the following section to the executable's `App.config`:

```xml
<configuration>
  <System.Windows.Forms.ApplicationConfigurationSection>
    <add key="DpiAwareness" value="PerMonitorV2" />
  </System.Windows.Forms.ApplicationConfigurationSection>
</configuration>
```

The executable manifest must declare Windows 10 compatibility with `supportedOS` ID `{8e0f7a12-bfb3-4fe8-b9a5-48fd50a15a9a}`. Keep the Framework DPI selection in app.config instead of adding a competing DPI declaration to that manifest. An ordinary unsigned demo uses `uiAccess="false"`.

The public demos already share these Framework settings through [Directory.Build.targets](../demos/Directory.Build.targets), [WinForms.App.net481.config](../demos/WinForms.App.net481.config), and [WinForms.app.manifest](../demos/WinForms.app.manifest). Existing project-specific manifests are retained. When copying one demo outside this directory, copy or recreate those settings too. The Framework-only configuration section must not be added to a modern .NET application's config. See Microsoft's [Framework HiDPI setup](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/high-dpi-support-in-windows-forms).

## 3. Main form design and custom layout

The main form's generated design file uses a 96-DPI baseline:

```csharp
this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
```

Change both values together: `(6F, 13F)` is the previous font-based baseline. Apply the same DPI baseline to additional forms and dynamically created viewer windows. Set `Dock` and `Anchor` for resizable editor panels; use `TableLayoutPanel` or `FlowLayoutPanel` for custom buttons and fields. Size selection dialogs from their content and give resizable dialogs a usable minimum size.

Keep font sizes in logical points. Sirius3 scales its owned toolbar images, tabs, TreeView rows, PropertyGrid description area, and custom value editors. Do not multiply those metrics by the display scale again. Custom owner-drawn UI still needs its own current-DPI measurements.

Process DPI awareness belongs to the executable. Referencing the UI DLL does not enable it for the host. Existing products with font-based forms should review their own layout before changing their scaling baseline.

## 4. Register documents and devices

Initialize Core before constructing the editor. Read `config*.ini` and use the demo factories in [helper.cs](../demos/helper.cs) to create and initialize the scanner, laser, power meter, digital I/O, and marker. The `beginner` demo creates its devices explicitly instead. Use virtual device factories when checking UI without equipment.

After successful device initialization, register the complete set on the owning UI thread. In the following fragment, every device variable is an initialized instance owned by the host:

```csharp
editor.RegisterDevices(rtc, laser, powerMeter,
    diExt1, diLaserPort, doExt1, doExt2, doLaserPort, marker);
if (!marker.IsReady)
    throw new InvalidOperationException("Marker is not ready.");
```

`RegisterDevices` connects the document and view and calls `Marker.Ready`; check its resulting state and logs rather than repeating `Ready`. For `SiriusMultiEditorControl`, register each set with `RegisterDevices(index, ...)` and select it with `SwitchDevices(index)`.

Select an entity in the editor or TreeView before changing its properties. Use document actions to preserve Undo/Redo and geometry regeneration. Shared documents have one disposal owner; do not dispose a document while another viewer still uses it.

## 5. MCP and asynchronous shutdown

Create MCP after device registration. `MCPFactory.CreateServer` constructs the server; `await server.Start()` starts authenticated HTTP. Configure `AccessMode` explicitly, and assign an editor-owned server to `editor.MCPServer`. See [MCP setup](MCP.md) for client configuration and [remote protocols](RemoteProtocolManual.md) for socket, serial, WebSocket, and MQTT commands. Remote/MCP availability follows the license options.

In 1.15.0, `DisposeDevices()` returns `Task`. Call and await it on the owning UI thread before releasing the editor or document. It first awaits the registered MCP server and remote connections, then disposes the remaining devices. A normal control `Dispose()` does not replace device cleanup.

Use the `FormClosing` pattern in the linked demos: cancel the first close, guard against repeated requests, yield back to the event loop, await cleanup, then allow a second `Close()`. Do not block the UI thread with `.Wait()` or `.Result`. Log cleanup failures and continue releasing host-owned resources.

| Ownership | Cleanup |
|---|---|
| MCP/remote registered on the editor | Included in `await editor.DisposeDevices()` |
| Separately owned MCP/remote | Await `DisposeAsync()` before releasing its editor or marker |
| Editor control | Dispose after device cleanup |
| Host-owned document | Detach from all viewers and dispose once |
| Core | Call `Core.Cleanup()` once after the UI/device lifetime ends |

If `await DisposeDevices()` gives CS4008, the compiler resolved an older `void` signature. Verify the restored UI package and assembly, then refresh the affected package/version. Keep the `await` when consuming 1.15.0.

## 6. Check the resulting application

Check cold startup at 100%, 150%, and 200%, movement between monitors, and pages first opened after a DPI change. Inspect combo boxes, dialog minimum sizes, toolbar images, TreeView rows, PropertyGrid descriptions, and the editor viewport. Compilation alone does not verify DPI layout or GPU rendering. Review the configured devices before launching a demo; only one Sirius3 process should own them at a time.

Related guides: [integrated editor](SiriusEditorUserManual.md), [multi-editor](SiriusMultiEditorUserManual.md), [UI configuration](Sirius3UIConfigUserManual.md), [WPF](WPF.md).

---
2026 Copyright (c) SpiralLAB. All rights reserved.
