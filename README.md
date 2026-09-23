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
   - Native Sirius3 documents, DXF/DWG, HPGL/PLT, Gerber/Excellon, and G-code/NGC
   - Raster images and STL, OBJ, PLY, STP/STEP 3D models
   - Tolerance-based path joining for vector files and content-based Gerber/Excellon detection
- Remote Communication and Dynamic Data
   - TCP/IP, Serial (RS-232), WebSocket, and MQTT endpoints for marker control and data access
   - Embedded HTTP MCP server for AI-assisted editor registration, document/entity editing, simulation, and registered-device control
   - Event, file, offset, linked-entity, and C# script conversion for text and barcode data
- Documents, Editors and Simulation
   - Four document pages with layers, pens, groups, blocks, and configurable Undo/Redo
   - Both WinForms and WPF `SiriusEditorControl` / `SiriusMultiEditorControl` are supported in Debug and Release; one document can be rendered to multiple views
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
| 3D Mesh with slicer          |Built-in slicer for STL, OBJ, PLY, STEP   |None                                   |
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
   - RTC6: 2026.6.19 v1.25.0
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

# [WinForms](#tab/winforms)

Project settings
```
<PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFrameworks>net481;net8.0-windows;net9.0-windows;net10.0-windows</TargetFrameworks>
    <UseWindowsForms>true</UseWindowsForms>
</PropertyGroup>

<PropertyGroup Condition="'$(TargetFramework)'=='net481'">
	<DefineConstants>$(DefineConstants);OPENTK3</DefineConstants>
</PropertyGroup>
<PropertyGroup Condition="'$(TargetFramework)'!='net481'">
	<DefineConstants>$(DefineConstants);OPENTK4</DefineConstants>
</PropertyGroup>

<ItemGroup Condition="'$(TargetFramework)'=='net481'">
	<PackageReference Include="OpenTK" Version="3.3.3" />
</ItemGroup>
<ItemGroup Condition="'$(TargetFramework)'!='net481'">
	<PackageReference Include="OpenTK" Version="4.9.4" />
	<PackageReference Include="OpenTK.Mathematics" Version="4.9.4" />
</ItemGroup>

<ItemGroup>
    <PackageReference Include="Microsoft.Extensions.Logging" Version="10.0.10" />
    <PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="10.0.10" />
    <PackageReference Include="Microsoft.Extensions.Logging.Debug" Version="10.0.10" />
    <PackageReference Include="SpiralLab.Sirius3.Dependencies" Version="1.*" />
    <PackageReference Include="SpiralLab.Sirius3" Version="1.*" />
    <PackageReference Include="SpiralLab.Sirius3.UI" Version="1.*" />
    <PackageReference Include="Newtonsoft.Json" Version="13.0.4" />
</ItemGroup>
```

Example code
```
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using SpiralLab.Sirius3.MCP;
using SpiralLab.Sirius3.IO;
using SpiralLab.Sirius3.Laser;
using SpiralLab.Sirius3.Marker;
using SpiralLab.Sirius3.PowerMap;
using SpiralLab.Sirius3.PowerMeter;
using SpiralLab.Sirius3.Scanner;
using SpiralLab.Sirius3.Scanner.Rtc;

#if OPENTK3
    using OpenTK;
    using DVec3 = OpenTK.Vector3d;
#elif OPENTK4
    using OpenTK.Mathematics;
    using DVec3 = OpenTK.Mathematics.Vector3d;
#endif

static class Program
{
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // Initialize sirius3 library
        SpiralLab.Sirius3.Core.Initialize();
        // Create winforms
        CreateAndExecuteMainForm();    
    }

    static void CreateAndExecuteMainForm()
    {
        // Create a form and add SiriusEditorControl to the form
        Form dynamicForm = new Form();
        dynamicForm.SuspendLayout();
        dynamicForm.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        dynamicForm.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        dynamicForm.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        dynamicForm.Text = "DEMO - (c)SpiralLab";
        dynamicForm.Size = new Size(1600, 1200);
        dynamicForm.StartPosition = FormStartPosition.CenterScreen;
        var editorControl = new SpiralLab.Sirius3.UI.WinForms.SiriusEditorControl();
        IMCPServer mcpServer = null;
        editorControl.Dock = DockStyle.Fill;
        dynamicForm.Controls.Add(editorControl);
        dynamicForm.ResumeLayout(false);

        dynamicForm.Load += (s, e) =>
        {
            // Create devices and initialize them, then register to editor control
            bool success = true;

            // Scanner control
            string correctionFile = "cor_1to1.ct5";
            string correctionPath = Path.Combine(SpiralLab.Sirius3.Config.CorrectionPath, correctionFile);
            const double fov = 100.0;
            var kfactor = Math.Pow(2, 20) / fov;
            var index = 0;
            var rtc = ScannerFactory.CreateRtc5(index, kfactor, LaserModes.Yag1, RtcSignalLevels.ActiveHigh, RtcSignalLevels.ActiveHigh, correctionPath);
            success &= rtc.Initialize();
            rtc.CtlFrequency(50 * 1000, 2);
            rtc.CtlSpeed(100, 100);

            // DIO control
            var dIExt1 = IOFactory.CreateInputExtension1(rtc); success &= dIExt1.Initialize();
            var dOExt1 = IOFactory.CreateOutputExtension1(rtc); success &= dOExt1.Initialize();
            var dOExt2 = IOFactory.CreateOutputExtension2(rtc); success &= dOExt2.Initialize();
            var dILaserPort = IOFactory.CreateInputLaserPort(rtc); success &= dILaserPort.Initialize();
            var dOLaserPort = IOFactory.CreateOutputLaserPort(rtc); success &= dOLaserPort.Initialize();

            // Powermeter control
            double laserMaxPower = 20;
            var powerMeter = PowerMeterFactory.CreateVirtual(index, laserMaxPower);
            //var powerMeter = PowerMeterFactory.CreateCoherentPowerMax(index, 4);
            //var powerMeter = PowerMeterFactory.CreateGentecEO(index, 3, scaleIndex: null);
            success &= powerMeter.Initialize();

            // Laser control
            var laser = LaserFactory.CreateVirtualDutyCycle(index, laserMaxPower, 0, 100);
            //var laser = LaserFactory.Create ...
            success &= laser.Initialize();
            laser.Scanner = rtc;

            // Powermap
            var powerMap = PowerMapFactory.CreateDefault(index, "default");
            powerMap.Reset1to1("10000", laserMaxPower);
            laser.PowerMap = powerMap;

            // Marker
            var marker = MarkerFactory.CreateRtc(index);
            //var marker = MarkerFactory.CreateRtcFast(index);
            //var marker = MarkerFactory.CreateSyncAxis(index);
            success &= marker.Initialize();

            Debug.Assert(success);

            // Register devices
            editorControl.RegisterDevices(rtc, laser, powerMeter, dIExt1, dILaserPort, dOExt1, dOExt2, dOLaserPort, marker);

            // Start the authenticated HTTP MCP server.
            mcpServer = MCPFactory.CreateServer(0, "MCP", editorControl);
            mcpServer.AccessMode = MCPAccessMode.All;
            if (!mcpServer.Start().GetAwaiter().GetResult())
                throw new InvalidOperationException("Failed to start the MCP server.");
            dynamicForm.Text = $"{dynamicForm.Text} + MCP ({mcpServer.Options.HttpEndpoint})";
        };

        dynamicForm.FormClosing += (s, e) =>
        {
            var dlgResult = MessageBox.Show(dynamicForm, $"Do you really want to terminate program ?", "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dlgResult != DialogResult.Yes)
            {
                e.Cancel = true;
                return;
            }

            // Dispose the MCP server before the editor, document, and devices.
            mcpServer?.Dispose();
            mcpServer = null;

            // Dispose devices
            editorControl.DisposeDevices();

            // Dispose document
            editorControl.Document?.Dispose();
          
            // Clean-up sirius3 library
            SpiralLab.Sirius3.Core.Cleanup();
        };

        Application.Run(dynamicForm);
    }
}
```

# [WPF](#tab/wpf)

Project settings:

```xml
<PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFrameworks>net481;net8.0-windows;net9.0-windows;net10.0-windows</TargetFrameworks>
    <UseWPF>true</UseWPF>
    <UseWindowsForms>true</UseWindowsForms>
</PropertyGroup>
```

`Program.cs`:

```csharp
using System;
using System.Windows;
using SpiralLab.Sirius3.IO;
using SpiralLab.Sirius3.Laser;
using SpiralLab.Sirius3.Marker;
using SpiralLab.Sirius3.MCP;
using SpiralLab.Sirius3.PowerMap;
using SpiralLab.Sirius3.PowerMeter;
using SpiralLab.Sirius3.Scanner;
using SpiralLab.Sirius3.Scanner.Rtc;
using SpiralLab.Sirius3.UI.WPF;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        var app = new Application();
        SiriusEditorControl editor = null;
        IMCPServer mcpServer = null;
        var coreInitialized = false;
        var editorDisposed = false;

        void DisposeEditor()
        {
            if (editorDisposed || editor == null)
                return;
            editorDisposed = true;

            var document = editor.Document;
            try { mcpServer?.Dispose(); }
            finally
            {
                mcpServer = null;
                try { editor.DisposeDevices(); }
                finally
                {
                    try { editor.Dispose(); }
                    finally { document?.Dispose(); }
                }
            }
        }

        try
        {
            SpiralLab.Sirius3.Core.Initialize();
            coreInitialized = true;
            WPFThemeManager.Initialize();

            editor = new SiriusEditorControl();

            const int index = 0;
            const double fieldSize = 100.0;
            const double laserMaxPower = 20.0;
            var scanner = ScannerFactory.CreateVirtual(
                index,
                Math.Pow(2, 20) / fieldSize,
                LaserModes.Yag1,
                RtcSignalLevels.ActiveHigh,
                RtcSignalLevels.ActiveHigh,
                null);
            var dIExt1 = IOFactory.CreateInputExtension1(scanner);
            var dILaserPort = IOFactory.CreateInputLaserPort(scanner);
            var dOExt1 = IOFactory.CreateOutputExtension1(scanner);
            var dOExt2 = IOFactory.CreateOutputExtension2(scanner);
            var dOLaserPort = IOFactory.CreateOutputLaserPort(scanner);
            var powerMeter = PowerMeterFactory.CreateVirtual(index, laserMaxPower);
            var laser = LaserFactory.CreateVirtualDutyCycle(index, laserMaxPower, 0, 100);
            var powerMap = PowerMapFactory.CreateDefault(index, "default");
            var marker = MarkerFactory.CreateVirtual(index);

            powerMap.Reset1to1("10000", laserMaxPower);
            laser.Scanner = scanner;
            laser.PowerMap = powerMap;

            var success = scanner.Initialize();
            success &= scanner.CtlFrequency(50 * 1000, 2);
            success &= scanner.CtlSpeed(100, 100);
            success &= dIExt1.Initialize();
            success &= dILaserPort.Initialize();
            success &= dOExt1.Initialize();
            success &= dOExt2.Initialize();
            success &= dOLaserPort.Initialize();
            success &= powerMeter.Initialize();
            success &= laser.Initialize();
            success &= marker.Initialize();

            editor.RegisterDevices(
                scanner, laser, powerMeter,
                dIExt1, dILaserPort,
                dOExt1, dOExt2, dOLaserPort,
                marker);

            if (!success)
                throw new InvalidOperationException("Failed to initialize virtual devices.");

            var window = new Window
            {
                Title = "Sirius3 WPF Editor",
                Width = 1400,
                Height = 900,
                Content = editor,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
            };

            mcpServer = MCPFactory.CreateServer(0, "MCP", editor);
            mcpServer.AccessMode = MCPAccessMode.All;
            if (!mcpServer.Start().GetAwaiter().GetResult())
                throw new InvalidOperationException("Failed to start the MCP server.");
            window.Title = $"{window.Title} + MCP ({mcpServer.Options.HttpEndpoint})";

            // Dispose the MCP server and editor while the WPF window and GL context still exist.
            window.Closing += (_, __) => DisposeEditor();
            app.Run(window);
        }
        finally
        {
            try { DisposeEditor(); }
            finally
            {
                if (coreInitialized)
                    SpiralLab.Sirius3.Core.Cleanup();
            }
        }
    }
}
```

---

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
