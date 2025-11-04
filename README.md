# Sirius3
A .NET-based, all-in-one platform for precision laser processing and additive manufacturing. 

![sirius3_logo](https://spirallab.co.kr/sirius3/sirius3_logo3.png)

---

## Highlights
![sirius3_logo](https://spirallab.co.kr/sirius3/sirius3_slicer.png)

- SCANLAB RTC Controllers
   - RTC4 / RTC4e / RTC5 / RTC6 / RTC6e
   - XL-SCAN (RTC6 + ACS via syncAXIS)
- Measurement and Profiling
   - Log scanner trajectory and output signals with plotted graphs
- Powerful Marking Options
   - MoF (Marking on the Fly), 2nd head, 3D
   - Sky Writing Mode 1/2/3 and 4
- Ramp (Automatic Laser Control)
   – Position-dependent / Velocity-dependent / User-defined vector
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
   - Barcodes: QR, DataMatrix, PDF417 and more
   - 3D Mesh Format like as STL, OBJ, PLY
- Open Architecture
   - Editor, marker, and laser-source control code are open for customization

## Major Changes
![sirius3_logo](https://spirallab.co.kr/sirius3/sirius3_hatch.png)

- 3D-printer features: slicer integration and contour extraction
- Multiple hatch patterns with path optimizer
- Multi-page documents
- Scanner, layer pen property system
- Built-in wafer, substrate editor
- Various rendering modes (per-vertex, Z-depth, etc.)
- Faster rendering with updated shader engine
- Switchable camera: orthographic / perspective

## Packages / DLLs
- `SpiralLab.Sirius3.Dependencies` — SCANLAB RTC4/5/6, syncAXIS runtime, fonts, sample data
- `SpiralLab.Sirius3` — HAL controllers (scanner/laser/powermeter, etc.)
- `SpiralLab.Sirius3.UI` — Entities, 3D renderer, WinForms/WPF UI controls
 > Easy to update library files by NuGet package manager.

## Platform targets
- `net481`
- `net8.0-windows`

## System Requirements
- Windows 10/11 (x64)
- GPU/Driver with OpenGL 3.3 support (latest drivers strongly recommended)
- SCANLAB drivers/runtimes installed (see versions below)
 
## Dependencies
- SCANLAB
   - RTC4: v2023.11.02
   - RTC5: v2024.09.27
   - RTC6: v1.20.0
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

## Getting Started
- Add references (From NuGet package manager)
   - `SpiralLab.Sirius3.Dependencies` (https://www.nuget.org/packages/SpiralLab.Sirius3.Dependencies)
   - `SpiralLab.Sirius3` (https://www.nuget.org/packages/SpiralLab.Sirius3)
   - `SpiralLab.Sirius3.UI` (https://www.nuget.org/packages/SpiralLab.Sirius3.UI)

- Create your devices like as scanner, laser, powermeter, marker, ... and attach them to SiriusEditorControl.
- Examples: https://github.com/labspiral/sirius3

## Quick Start
```
#if NETFRAMEWORK
    #define OPENTK3
#elif NET8_0_OR_GREATER
    #define OPENTK4
#endif

#if OPENTK3
    using OpenTK;
    using DVec2 = OpenTK.Vector2d;
    using DVec3 = OpenTK.Vector3d;
    using DMat4 = OpenTK.Matrix4d;
#elif OPENTK4
    using OpenTK.Mathematics;
    using DVec2 = OpenTK.Mathematics.Vector2d;
    using DVec3 = OpenTK.Mathematics.Vector3d;
    using DMat4 = OpenTK.Mathematics.Matrix4d;
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
            var scanner = /* new Rtc4, Rtc5 or Rtc6(...), */; scanner.Initialize();
            var laser = /* new LaserVirtual(...) */; laser.Initialize();
            var powerMeter = /* new PowerMeterVirtual(...) */; powerMeter.Initialize();
            var marker = /* new MarkerRtc(...) */; marker.Initialize();

            editor.Scanner = scanner;
            editor.Laser = laser;
            editor.PowerMeter = powerMeter;
            editor.Marker = marker;
            
            // 2. Ready marker
            marker.Ready(editor.Document, editor.View, scanner, laser, powerMeter);

            // 3. Create entities
            var line = EntityFactory.Line(new DVec3(0, 0, 0), new DVec3(10, 10, 0));
            editor.Document.Add(line);
            var text = new EntityText("Arial", FontStyle.Regular, "SIRIUS3", 10);
            editor.Document.Add(text);
            
            // 4. Do laser processing
            marker.Start();
        };
    }

    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.Run(new MainForm());
    }
}
```

## License
- Commercial license required for production use.
- Contact: hcchoi@spirallab.co.kr | https://spirallab.co.kr
- See LICENSE.txt and THIRD-PARTY-NOTICES.txt.
> Without a license key, the library runs in 30-minute evaluation mode.

## Version history
* 2025.11.14 v0.8.0
  - developer preview version
