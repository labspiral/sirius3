# SpiralLab.Sirius3

SIRIUS3 library

---

## Prebuilt directories

- **spirallab.sirius3.dependencies**: Mandatory files for SCANLAB RTC4,5,6 and syncAXIS.
- **spirallab.sirius3.dll**: Hardware abstract layer for RTC, laser, DIO,... devices.
- **spirallab.sirius3.ui.dll**: Render engine and entities for UI.

## Platform targets

- `net481`
- `net8.0-windows`

## How to use

- Add **SpiralLab.Sirius3.Dependencies**, **SpiralLab.Sirius3**, **SpiralLab.Sirius3.UI** reference files into your working project
   - dotnet add package SpiralLab.Sirius3.Dependencies --version 0.1.0
   - dotnet add package SpiralLab.Sirius3 --version 1.0.0
   - dotnet add package SpiralLab.Sirius3.UI --version 1.0.0
- Create scanner, laser, powermeter devices with document, marker and assign at **SiriusEditorControl**
- More example(or demo) projects at https://github.com/labspiral/sirius3

## License

- Homgpage : http://www.spirallab.co.kr
- E-mail : hcchoi@spirallab.co.kr
- SCANLAB and RTC is trademarks of (c)SCANLAB.
- All rights reserved. 2025 Copyright to (c)SpiralLAB.

> Activated evaluation copy mode during 30 mins if without license key.
