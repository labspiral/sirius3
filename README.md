# Sirius3
A Windows/.NET platform for precision laser processing that combines SCANLAB control, device integration, geometry processing, OpenGL visualization, document editing, AI prompting through an embedded MCP server, simulation, and marking execution.

Languages: [English](README.md) · [한국어](README.koKR.md) · [简体中文](README.zhCN.md) · [日本語](README.jaJP.md) · [Deutsch](README.deDE.md)

![sirius3_logo](https://spirallab.co.kr/sirius3/sirius3_logo.png)

---

## Highlights
![sirius3_logo1](https://spirallab.co.kr/sirius3/sirius3_logo1.png)
![sirius3_editor](https://spirallab.co.kr/sirius3/sirius3_editor.png)

- SCANLAB RTC Controllers
   - RTC4 / RTC4e / RTC5 / RTC6 / RTC6e
   - XL-SCAN (RTC6 + ACS via syncAXIS)
- Measurement and Profiling
   - Log scanner trajectory and output signals with plotted graphs
   - Visualize machining paths with real-time simulation
- Powerful Marking Options
   - Variable Polygon and Variable Jump Delays 
   - 2nd head, 3D
   - MoF (Marking on the Fly) and MoF Extension(aka. Fly extension)
   - Sky Writing Mode 1/2/3 or 4
   - Auto Delays by SCANAhead
   - MultiBeam (1 Laser source + 2 AOM + 2 ScanHead)
- ALC(Automatic Laser Control) or Pulse on Demand
   - Defined vector 
      - Ramp
   - Speed dependent 
      - Set Velocity
      - Actual Velocity
   - Encoder dependent
      - Encoder Speed 
   - Position dependent 
      - Table by distance and scale factor
   - Also, SCANAhead, Encoder Speed Addition, Inverse Speed Correction, Backward Transformation, SDC+Skywriting combinations available
- Scanner Field Correction
   - 2D correction 
   - 3D correction for tilt, focus, coefficient a,b,c and stretch factors
- Laser Power Control
   - Frequency, Duty Cycle, Analog, Digital
   - Built-in vendor integrations: AdvancedOptoWave, Coherent, IPG, JPT, Photonics Industry, Spectra Physics and more
- Powermeters & Powermap
   - Coherent (PowerMax), Thorlabs (via OPM), Ophir (via StarLab)
   - Powermap-based output compensation
- Rendering and Geometry Processing
   - OpenGL 3.3+ 2D/3D renderer with one orthographic and five perspective cameras
   - AABB acceleration for point, line, line-strip, and triangle hit testing
   - Topology-aware 3D mesh slicing with closed/open contour diagnostics
   - Winding-aware multiple hatches for outlines, nested regions, and connected barcode cells
- Entities, Text and Barcodes
   - Point(s), Line, Arc, Polyline, Triangle, Rectangle, Spiral, Trepan, Spline
   - Cube, Sphere, Cylinder, Cone, Mesh, Layer, Group, Block and BlockInsert
   - Text, SiriusText, ImageText, Circular Text, linked text, and ZPL rendering entities
   - 1D, QR, DataMatrix, PDF417, and Aztec barcodes with outline, hatch, and dot-cell processing
- File Import and Interoperability
   - Native Sirius3 documents, DXF/DWG, SVG, HPGL/PLT, Gerber/Excellon, and G-code/NGC
   - SVG (`.svg`) vector import: `IDocument.ActImport` / `EntityFactory.CreateSVG`. Retains editable native curves and generates fill, stroke-width, cap and join regions in millimeters, including rounded ends and corners.
   - SVG hatching: enable `UI.Config.IsSvgImportHatchEnabled` (default `false`) and set spacing with `UI.Config.SvgImportHatchInterval` (default 0.1 mm). Supports line/polygon hatching; click and drag selection follows displayed outlines and hatches.
   - Raster images and STL, OBJ, PLY, STP/STEP, 3MF 3D models
   - 3MF (`.3mf`) mesh import: `IDocument.ActImport` / `EntityFactory.CreateMesh`. Combines built triangle-mesh models and components into one mesh, applying relative placement and converting source units to millimeters. Stored slicer toolpaths and slice results are outside the import scope.
   - Available mesh colors and OBJ/PLY/3MF diffuse textures are preserved for 3D preview
   - G-code/NGC import preserves modal XYZ positioning, units/work origins, planar arcs and helices, dwell and laser ON/OFF paths. `UI.Config.GCodeImportDialect` selects CNC, Laser or Marlin rules; `GCodeImportInfo.TryGet` exposes source F/S/T and line ranges without applying device settings.
   - Tolerance-based path joining for vector files and content-based Gerber/Excellon detection
- Remote Communication and Dynamic Data
   - TCP/IP, Serial (RS-232), WebSocket, and MQTT endpoints for marker control and data access
   - Embedded HTTP MCP server for AI-assisted editor registration, document/entity editing, simulation, and registered-device control
   - Event, file, offset, linked-entity, and C# script conversion for text and barcode data
- Documents, Editors and Simulation
   - Four document pages with layers, pens, groups, blocks, and configurable Undo/Redo
   - Both WinForms and WPF `SiriusEditorControl` / `SiriusMultiEditorControl` are supported in Debug and Release; one document can be rendered to multiple views
   - [Font Editor](SpiralLab.Sirius3.UI/FontEditor/README.md): independent CXF/LFF/EO FNT/DOT glyph editing from the WinForms/WPF **Font Edit** menu; mm vectors, integer DOT cells, BBox/metrics, Undo/Redo, DXF import, text preview and installed-TTF conversion. Always included in Debug and Release.
   - Real-time laser-path visualization with screen-sized markers, beam effects, and optional debris
   - Grid-based stitched-image visualization for camera and inspection workflows
- Open Architecture
   - Extensible editor, entity, marker, scanner, laser, power-meter, and remote interfaces
   - Embedded MCP server for AI-assisted editor registration, document/entity editing, and device control

## Major Changes
|                              |                SIRIUS3                   |              SIRIUS2                  |
|:-----------------------------|:-----------------------------------------|:--------------------------------------|
| Multiple page                |4 Pages                                   |No Page / Single Document              |
| Camera                       |6 Built-in Cameras                        |Perspective                            |
| Render speed                 |GPU-accelerated OpenGL shader engine       |Built-in shader engine                 |
| Render mode                  |Model, PerVertex, Normal, ZDepth          |None                                   |
| HitTest speed                |AABB-accelerated point/line/triangle tests|Slow                                   |
| Hatch                        |Winding-aware multiple hatches            |Single Hatch                           |
| 3D Mesh with slicer          |Built-in slicer for STL, OBJ, PLY, STEP, 3MF |None                                |
| Gerber / Excellon            |Content-detected import                   |None                                   |
| Font file                    |General CXF, LFF, FNT, DOT formats        |Customized CXF, LFF formats            |
| Pen                          |Pens for Entity and Layer                 |Entity Pen                             |
| MCP                          |Supported                                 |None                                   |
| Library update               |By Nuget Package Manager                  |Manual                                 |
                                                                                                              
![sirius3_hatch](https://spirallab.co.kr/sirius3/sirius3_hatch.png)
![sirius3_pod](https://spirallab.co.kr/sirius3/sirius3_pod.png)
![sirius3_slicer](https://spirallab.co.kr/sirius3/sirius3_slicer.png)
![sirius3_syncaxis](https://spirallab.co.kr/sirius3/sirius3_syncaxis.png)

## Packages / DLLs
- `SpiralLab.Sirius3.Dependencies` — SCANLAB RTC4/5/6, syncAXIS runtime, fonts, sample data
- `SpiralLab.Sirius3` — HAL controllers (scanner/laser/powermeter, etc.)
- `SpiralLab.Sirius3.UI` — Entities, geometry processing, OpenGL rendering, WinForms/WPF controls, and embedded MCP integration
 > Easy to update library files by NuGet package manager.

## Platform targets
- `net481`
- `net8.0-windows`
- `net9.0-windows`
- `net10.0-windows`

## System Requirements
- Windows 10/11 (x64)
- GPU/Driver with minimum OpenGL 3.3 support (latest drivers strongly recommended)
- SCANLAB drivers/runtimes installed (see versions below)
- Visual Studio 2022 or higher version

## Dependencies
- SCANLAB
   - RTC4: v2023.11.02
   - RTC5: v2024.09.27
   - RTC6: 2026.8.17 v1.25.1
   - syncAXIS: v1.8.2 (2023.03.09)

- .NET
   - `net481`: OpenTK 3.3.3
   - `net8.0-windows`, `net9.0-windows`, `net10.0-windows`: OpenTK and OpenTK.Mathematics 4.9.4
   - Common package dependencies for all target frameworks
      - Microsoft.Extensions.Logging 10.0.10
      - Microsoft.Extensions.Logging.Abstractions 10.0.10
      - Microsoft.Extensions.Logging.Debug 10.0.10
      - Newtonsoft.Json 13.0.4
## Install Packages
- Add references 
   - `SpiralLab.Sirius3.Dependencies` (https://www.nuget.org/packages/SpiralLab.Sirius3.Dependencies)
   - `SpiralLab.Sirius3` (https://www.nuget.org/packages/SpiralLab.Sirius3)
   - `SpiralLab.Sirius3.UI` (https://www.nuget.org/packages/SpiralLab.Sirius3.UI)
   
## Quick Start

Install the three Sirius3 NuGet packages listed above. These excerpts show the main flow; `...` leaves out device creation, validation, and event wiring. Place the startup, load, and closing steps in the corresponding host events.

# [WinForms](#tab/winforms)

Project settings

```xml
<PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFrameworks>net481;net8.0-windows;net9.0-windows;net10.0-windows</TargetFrameworks>
    <UseWindowsForms>true</UseWindowsForms>
    <ApplicationHighDpiMode>PerMonitorV2</ApplicationHighDpiMode>
</PropertyGroup>
```

Initialize Core, create the editor, then register initialized devices. WinForms DPI initialization runs before these steps; see the HiDPI section below.

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

During asynchronous closing, keep the UI event loop running while cleanup finishes:

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

Project settings

```xml
<PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFrameworks>net481;net8.0-windows;net9.0-windows;net10.0-windows</TargetFrameworks>
    <UseWPF>true</UseWPF>
    <ApplicationManifest>app.manifest</ApplicationManifest>
</PropertyGroup>
```

Initialize Core before constructing the WPF editor. Register initialized devices when the Window loads.

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

During asynchronous closing, keep the UI event loop running while cleanup finishes:

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

`DisposeDevices()` awaits the registered MCP server and remote connections before releasing devices. Finish editor/document cleanup before the application calls `Core.Cleanup()`. The detailed examples show the closing-event guards and error handling.

Full examples: [WinForms guide](https://github.com/labspiral/sirius3/blob/main/doc/WinForms.md), [WPF guide](https://github.com/labspiral/sirius3/blob/main/doc/WPF.md), [demo projects](DEMOS.md).

---

## WinForms HiDPI

Update the DLL/package first. Existing `DpiUnaware` applications can retain their behavior. To opt into per-monitor scaling, configure the consuming EXE before creating any controls; the DLL does not change process DPI awareness. Existing host forms may retain their `AutoScaleMode.Font`, font and designer baseline. Review custom fixed-pixel layout and manual scaling separately.

For .NET 8/9/10 applications with a manual `Main`, select Per-Monitor V2 before `EnableVisualStyles` and `Core.Initialize`:

```csharp
#if NET8_0_OR_GREATER
Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
#endif
Application.EnableVisualStyles();
Application.SetCompatibleTextRenderingDefault(false);
```

For .NET Framework 4.8.1, merge this section into the EXE's existing app.config, preserve binding redirects, declare Windows 10 compatibility in its manifest, and call `EnableVisualStyles` before creating controls. Remove conflicting unaware DPI declarations and `EnableWindowsFormsHighDpiAutoResizing=false`.

```xml
<System.Windows.Forms.ApplicationConfigurationSection>
  <add key="DpiAwareness" value="PerMonitorV2" />
</System.Windows.Forms.ApplicationConfigurationSection>
```

If using `ApplicationConfiguration.Initialize()`, set `<ApplicationHighDpiMode>PerMonitorV2</ApplicationHighDpiMode>` in the EXE project instead of adding another bootstrap call. Font sizes remain in points. Rebuild application-owned ImageLists from retained source images at each DPI, set `ImageSize`/`ColorDepth` before adding images, preserve keys and indices, and replace references before disposing only owned old lists. WPF uses 96-DPI logical units and already applies display density; do not multiply WPF layout by DPI again.

[Microsoft: .NET Framework HiDPI](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/high-dpi-support-in-windows-forms), [WinForms automatic scaling](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/forms/autoscale).

## Demo Programs
- See [DEMOS.md](DEMOS.md) 
- Create your devices like as scanner, laser, powermeter, marker, ... and attach them to SiriusEditorControl.
- Examples: https://github.com/labspiral/sirius3/tree/main/demos
 
## License
- A license must be purchased for commercial use.
- License: Number of RTC instances + [Options]
    - MoF Option: Fly processing functionality (real-time tracking, standby, etc.) using an external encoder.
    - MultiBeam Option: A configuration consisting of 1 laser source + 2 AOMs + 2 scan heads, enabling real-time modification of the laser beam path during jump sections.
    - syncAXIS Option: A large-area processing solution (XL-SCAN Solution) utilizing synchronization between the scan head and stage via an ACS motion controller and excelliSCAN scan head configuration.
    - Remote Option: Supports recipe changes, processing control, and data read/write through socket, serial, WebSocket, and MQTT communication, and includes the embedded MCP server for AI-assisted document/entity editing and registered-device control.
- For license policies and third-party libraries, refer to [LICENSE.txt](LICENSE.txt) and [THIRD-PARTY-NOTICES.txt](THIRD-PARTY-NOTICES.txt).
- Email: hcchoi@spirallab.co.kr | https://spirallab.co.kr
> If no license key is provided, the software will run in evaluation mode, which is limited to 30 minutes of use.

## Version history
- See [HISTORY.md](HISTORY.md)

## API documentation
- See https://spirallab.co.kr/sirius3/doc
