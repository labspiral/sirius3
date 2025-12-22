# Sirius3
A .NET-Based, All-IN-ONE Platform for Precision Laser Processing.

![sirius3_logo](https://spirallab.co.kr/sirius3/sirius3_logo3.png)

---

## Highlights
![sirius3_editor](https://spirallab.co.kr/sirius3/sirius3_editor.png)

- SCANLAB RTC Controllers
   - RTC4 / RTC4e / RTC5 / RTC6 / RTC6e
   - XL-SCAN (RTC6 + ACS via syncAXIS)
- Measurement and Profiling
   - Log scanner trajectory and output signals with plotted graphs
- Powerful Marking Options
   - MoF (Marking on the Fly), 2nd head, 3D
   - Sky Writing Mode 1/2/3 and 4
- Ramp (Automatic Laser Control)
   – Position dependent 
   - Velocity(set or actual) dependent
   - Encoder speed dependent
- Scanner Field Correction
   - 2D / 3D correction 
- Laser Power Control
   - Frequency, Duty Cycle, Analog, Digital
   - Built-in vendor integrations: AdvancedOptoWave, Coherent, IPG, JPT, Photonics Industry, Spectra Physics and more
- Powermeters & Powermap
   - Coherent (PowerMax), Thorlabs (via OPM), Ophir (via StarLab)
   - Powermap-based output compensation
- Various Entities and Formats
   - Point(s), Line, Arc, Polyline, Triangle, Rectangle, Spiral, Trepan, Spline
   - Layer, Group, Block and BlockInsert
   - Text, SiriusText, ImageText, Circular Text
   - Image, DXF, HPGL, ZPL
   - QR, DataMatrix, PDF417 Barcodes
   - 3D Mesh Format like as STL, OBJ, PLY
- Open Architecture
   - Editor and laser-source control code are open for customization

## Major Changes
![sirius3_logo](https://spirallab.co.kr/sirius3/sirius3_hatch.png)

- Faster rendering with updated shader engine
- 3D-printer features: slicer integration and contour extraction
- Multiple hatch patterns with path optimizer
- Scanner, layer pen property system
- Various rendering modes (per-vertex, Z-depth, etc.)
- Multi-page documents
- Built-in wafer, substrate editor
- Built-in gerber file format(RS-274x) editor
- Switchable camera: orthographic / perspective

## Packages / DLLs
![sirius3_logo](https://spirallab.co.kr/sirius3/sirius3_slicer.png)
- `SpiralLab.Sirius3.Dependencies` — SCANLAB RTC4/5/6, syncAXIS runtime, fonts, sample data
- `SpiralLab.Sirius3` — HAL controllers (scanner/laser/powermeter, etc.)
- `SpiralLab.Sirius3.UI` — Entities, 3D renderer, WinForms UI controls
 > Easy to update library files by NuGet package manager.

## Platform targets
- `net481`
- `net8.0-windows`

## System Requirements
- Windows 10/11 (x64)
- GPU/Driver with OpenGL 3.3 support (latest drivers strongly recommended)
- SCANLAB drivers/runtimes installed (see versions below)
 
## Dependencies
![sirius3_logo](https://spirallab.co.kr/sirius3/sirius3_syncaxis.png)
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

## Install Packages
- Add references 
   - `SpiralLab.Sirius3.Dependencies` (https://www.nuget.org/packages/SpiralLab.Sirius3.Dependencies)
   - `SpiralLab.Sirius3` (https://www.nuget.org/packages/SpiralLab.Sirius3)
   - `SpiralLab.Sirius3.UI` (https://www.nuget.org/packages/SpiralLab.Sirius3.UI)

- Create your devices like as scanner, laser, powermeter, marker, ... and attach them to SiriusEditorControl.
- Examples: https://github.com/labspiral/sirius3

## Quick Start
Project settings

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

Example code

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
            // 1. Create devices 
            var scanner =  ScannerFactory.Create ...
            scanner.Initialize();

            var laser = LaserFactory.Create ...
            laser.Initialize();

            var powerMeter = PowerMeterFactory.Create ...
            powerMeter.Initialize();

            var marker = MarkerFactory.Create ... 
            marker.Initialize();

            // 2. Assign into SiriusEditorControl
            editor.Scanner = scanner;
            editor.Laser = laser;
            editor.PowerMeter = powerMeter;
            editor.Marker = marker;
            
            // 3. Create entities
            var line = EntityFactory.CreateLine(new DVec3(0, 0, 0), new DVec3(10, 10, 0));
            editor.Document.ActAdd(line);
          
            var text = EntityFactory.CreateText("Arial", FontStyle.Regular, "SIRIUS3", 10);
            editor.Document.ActAdd(text);
            
            // 4. Ready marker
            marker.Ready(editor.Document, editor.View, scanner, laser, powerMeter);
        };
    }

    [STAThread]
    static void Main()
    {
        // Initialize sirius3 library
        SpiralLab.Sirius3.Core.Initialize();

        ...
        Application.Run(new MainForm());

        // Clean-up sirius3 library
        SpiralLab.Sirius3.Core.Cleanup();
    }
}
```

## License
- Commercial license required for production use.
- See LICENSE.txt and THIRD-PARTY-NOTICES.txt.
- Contact: hcchoi@spirallab.co.kr | https://spirallab.co.kr
> Without a license key, the library runs in 30-minute evaluation mode.

## Version history
- See [HISTORY.md](HISTORY.md)

  
