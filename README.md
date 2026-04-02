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
   - Variable Polygon(Jump) Delay 
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
|                              |                SIRIUS3                   |              SIRIUS2                  |
|:-----------------------------|:-----------------------------------------|:--------------------------------------|
| Multiple page                |4 Pages                                   |No Page / Single Document              |
| Camera                       |6 Built-in Cameras                        |Perspective                            |
| Render speed                 |Fastest by updated shader engine          |Faster                                 |
| Render mode                  |Model, PerVertex, Normal, ZDepth          |None                                   |
| HitTest speed                |Faster by AABBTree                        |Slow                                   |
| Hatch                        |Multiple Hatches                          |Single Hatch                           |
| 3D Mesh with slicer          |Built-in Slicer for PLY, OBJ, STL         |None                                   |
| Gerber file(RS-274x)         |Supported                                 |None                                   |
| Wafer/Substrate map          |Built-in editor                           |None                                   |
| Font file                    |General CXF, LFF, FNT formats             |Customized CXF, LFF formats            |
| Pen                          |Pens for Entity and Layer                 |Entity Pen                             |
| Library update               |By Nuget Package Manager                  |Manual                                 |
                                                                                                              
![sirius3_hatch](https://spirallab.co.kr/sirius3/sirius3_hatch.png)
![sirius3_pod](https://spirallab.co.kr/sirius3/sirius3_pod.png)
![sirius3_slicer](https://spirallab.co.kr/sirius3/sirius3_slicer.png)
![sirius3_syncaxis](https://spirallab.co.kr/sirius3/sirius3_syncaxis.png)

## Packages / DLLs
- `SpiralLab.Sirius3.Dependencies` — SCANLAB RTC4/5/6, syncAXIS runtime, fonts, sample data
- `SpiralLab.Sirius3` — HAL controllers (scanner/laser/powermeter, etc.)
- `SpiralLab.Sirius3.UI` — Entities, 3D renderer, WinForms UI controls
 > Easy to update library files by NuGet package manager.

## Platform targets
- `net481`
- `net8.0-windows`
- `net9.0-windows`
- `net10.0-windows`

## System Requirements
- Windows 10/11 (x64)
- GPU/Driver with OpenGL 3.3 support (latest drivers strongly recommended)
- SCANLAB drivers/runtimes installed (see versions below)
 
## Dependencies
- SCANLAB
   - RTC4: v2023.11.02
   - RTC5: v2024.09.27
   - RTC6: 2025.10.30 v1.22.1
   - syncAXIS: v1.8.2 (2023.03.09)

- .NET / OpenTK
   - `net481`
      - OpenTK 3.3.3
   - `net8.0-windows`
   - `net9.0-windows` 
   - `net10.0-windows`
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
   
## Quick Start
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
<PropertyGroup Condition="'$(TargetFramework.StartsWith(`net8.0-windows`))' OR '$(TargetFramework.StartsWith(`net9.0-windows`))' OR '$(TargetFramework.StartsWith(`net10.0-windows`))'">
	<DefineConstants>$(DefineConstants);OPENTK4</DefineConstants>
</PropertyGroup>

<ItemGroup Condition="'$(TargetFramework)'=='net481'">
	<PackageReference Include="OpenTK" Version="3.3.3" />
</ItemGroup>

<ItemGroup Condition="'$(TargetFramework.StartsWith(`net8.0-windows`))' OR '$(TargetFramework.StartsWith(`net9.0-windows`))' OR '$(TargetFramework.StartsWith(`net10.0-windows`))'">
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

static class Program
{
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // Create winforms
        CreateAndExecuteMainForm();    
    }

    public CreateAndExecuteMainForm()
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
        editorControl.Dock = DockStyle.Fill;
        dynamicForm.Controls.Add(editorControl);
        dynamicForm.ResumeLayout(false);

        dynamicForm.Load += (s, e) =>
        {
            // Initialize sirius3 library
            SpiralLab.Sirius3.Core.Initialize();

            // Create devices and initialize them, then register to editor control
            bool success = true;

            // Scanner control
            string correctionFile = "cor_1to1.ct5";
            string correctionPath = Path.Combine(SpiralLab.Sirius3.Config.CorrectionPath, correctionFile);
            const var fov = 100.0;
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
            //var powerMeter = PowerMeterFactory.CreateGentecEO(index, 3);
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
        };

        dynamicForm.FormClosing += (s, e) =>
        {
            var dlgResult = MessageBox.Show(this, $"Do you really want to terminate program ?", "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dlgResult != DialogResult.Yes)
            {
                e.Cancel = true;
                return;
            }
            // Dispose devices
            editorControl.DisposeDevices();

            // Clean-up sirius3 library
            SpiralLab.Sirius3.Core.Cleanup();
        };

        Application.Run(dynamicForm);
    }
}
```

## Demo Programs
- See [DEMOS.md](DEMOS.md) 
- Create your devices like as scanner, laser, powermeter, marker, ... and attach them to SiriusEditorControl.
- Examples: https://github.com/labspiral/sirius3/tree/main/demos
 
## License
- Commercial license required for production use.
- License : RTC instance count + [Options: MoF, MultiBeam or syncAXIS]
- See LICENSE.txt and THIRD-PARTY-NOTICES.txt.
- Contact: hcchoi@spirallab.co.kr | https://spirallab.co.kr
> Without a license key, the library runs in 30-minute evaluation mode.

## Version history
- See [HISTORY.md](HISTORY.md)

## API documentation
- See https://spirallab.co.kr/sirius3/doc
