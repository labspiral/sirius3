# Sirius3

![sirius3_logo](https://spirallab.co.kr/sirius3/sirius3_logo3.png)

---

## Features
- Support SCANLAB's RTC controllers.
   - RTC4
   - RTC4e
   - RTC5
   - RTC6
   - RTC6e
   - XL-SCAN(RTC6 + ACS) by syncAXIS
- Support measure and profile scanner trajectory with output signals by plotted graph.
- Support powerful options.
   - MoF(Marking on the Fly)
   - 2nd head
   - 3D 
- Support Sky writing Mode 1,2,3
- Support ramp(Automatic Laser Control) controls.
   - Position dependent
   - Velocity dependent 
   - Defined-vector
- Support SCANahead and SDC(Spot Distance Control) control with RTC6.
- Support 2D, 3D scanner field correction. 
- Support many kinds of laser power controls.
   - Frequency
   - Duty cycle
   - Analog output
   - Digital output 
- Support specific laser source vendors to control with communication.
   - AdvancedOptoWave (AOPico, AOPico Precision, Fotia)
   - Coherent (Avia LX, Diamond C-Series)
   - IPG (YLP N, Type D, Type E, ULP N)
   - JPT (Type E)
   - Photonics Industry (DX, RGH AIO)
   - Spectra Physics (Hippo, Talon)
- Support many kinds of powermeters with powermap table for compensate output laser power.
   - Coherent (PowerMax)
   - Thorlabs (by OPM)
   - Ophir (by StarLab)
- Various pre-built entities.
   - Point(s), Line, Arc, Polyline, Triangle, Rectangle, Spiral, Trepan, Curve, Raster
   - Layer, Group, Block and Block insert
   - Text, SiriusText, ImageText, Circular text
   - Image, DXF, PDF
   - HPGL, ZPL (Zebra Programming Language)
   - QR code, DataMatrix, PDF417 and more barcodes
   - and various control entities
- Open source codes with editor, marker and laser source control for customization.
 
![sirius3_logo](https://spirallab.co.kr/sirius3/sirius3_slicer.png)

## Major changes
- Support 3D-printer features (slicer integration and contour extraction).
- Support multiple hatch patterns with path optimizer.
- Support multiple pages at document.
- Support scanner and layer pen properties.
- Support built-in wafer and substrate editor.
- Higher render speed by updated shader engine.
- Various rendering mode (per vertex, z depth and more)
- Freely switchable camera between orthographic and perspective views.

![sirius3_logo](https://spirallab.co.kr/sirius3/sirius3_hatch.png)
 
## Packages / DLLs
- SpiralLab.Sirius3.Dependencies — SCANLAB RTC4/5/6, syncAXIS runtime, fonts and sample data files
- SpiralLab.Sirius3 — HAL controllers
- SpiralLab.Sirius3.UI — Entities, 3D renderer and UI controls
 
## Platform targets
- `net481`
- `net8.0-windows`

## Dependencies
- SCANLAB RTC4: v2023.11.02
- SCANLAB RTC5: v2024.09.27
- SCANLAB RTC6: v.1.20.0 
- SCANLAB syncAXIS: v.1.8.2 (2023.3.9) 
- `net481` 
   - OpenTK: v3.3.3 
   - OpenTK.GLControl: v3.3.3 
- `net8.0-windows` 
   - OpenTK: v4.9.4
   - OpenTK.GLControl: v4.0.2 
   - OpenTK.Mathematics: v4.9.4 

## Getting Started
1) Install or reference: SpiralLab.Sirius3.Dependencies, SpiralLab.Sirius3, SpiralLab.Sirius3.UI
2) Create scanner/laser/powermeter, ... and assign to `SiriusEditorControl`.
3) Examples: https://github.com/labspiral/sirius3

## Outputs (runtime folders)
- **Basic**: Mandatory files for SCANLAB RTC4,5,6 and syncAXIS.
- **siriuslog**: Output log files.
- **correction**: Utilities for scanner field correction.
- **measurement**: Measurement output files. 
- **gnuplot**: Measurement viewer by gnuplot(http://gnuplot.info/download.html) program.
- **powermap**: Powermap file for compensate output laser power.
- **siriusfonts**: Sirius font files like as .lff, .cxf.
- **sample**: Sample files like as dxf, plt, stl, obj, ply and more.
- **x32**: Additional x32 dll files at runtime.
- **x64**: Additional x64 dll files at runtime.

## License
- Commercial license required for production use.
> Without a license key, the library runs in 30-minute evaluation mode per session.
- See LICENSE.txt and THIRD-PARTY-NOTICES.txt.
- Contact: hcchoi@spirallab.co.kr | https://spirallab.co.kr
- Examples: https://github.com/labspiral/sirius3

## Version history
- 0.1.0 developer preview version