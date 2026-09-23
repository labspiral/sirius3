# Sirius3

Eine Windows/.NET-Plattform für die präzise Laserbearbeitung. Sie verbindet SCANLAB-Steuerung, Geräteintegration, Geometrieverarbeitung, OpenGL-Visualisierung, Dokumentbearbeitung, KI-Prompts über einen eingebetteten MCP-Server, Simulation und Markierausführung.

[English](README.md) | [한국어](README.koKR.md) | [简体中文](README.zhCN.md) | [日本語](README.jaJP.md) | Deutsch

![sirius3_logo](https://spirallab.co.kr/sirius3/sirius3_logo.png)

---

## Highlights

![sirius3_logo1](https://spirallab.co.kr/sirius3/sirius3_logo1.png)
![sirius3_editor](https://spirallab.co.kr/sirius3/sirius3_editor.png)

- SCANLAB-RTC-Controller
  - RTC4 / RTC4e / RTC5 / RTC6 / RTC6e
  - XL-SCAN (RTC6 + ACS über syncAXIS)
- Messung und Profilierung
  - Aufzeichnung von Scannertrajektorien und Ausgangssignalen mit Diagrammen
  - Echtzeitvisualisierung von Bearbeitungspfaden
- Leistungsfähige Markieroptionen
  - Variable Polygon- und Sprungverzögerungen
  - Zweiter Kopf und 3D
  - MoF (Marking on the Fly) und MoF Extension
  - Sky Writing Mode 1/2/3/4
  - Automatische Verzögerungen mit SCANAhead
  - MultiBeam (eine Laserquelle + zwei AOM + zwei Scanköpfe)
- ALC (Automatic Laser Control) / Pulse on Demand
  - Definierter Vektor mit Rampen
  - Abhängigkeit von Soll- oder Istgeschwindigkeit
  - Abhängigkeit von Encodergeschwindigkeit
  - Positionsabhängige Tabellen nach Abstand und Skalierungsfaktor
  - Kombinationen aus SCANAhead, Encoder Speed Addition, Inverse Speed Correction, Backward Transformation und SDC + Skywriting
- Scanner-Feldkorrektur
  - 2D-Korrektur
  - 3D-Korrektur für Neigung, Fokus, Koeffizienten A/B/C und Stretch-Faktoren
- Laserleistungssteuerung
  - Frequenz, Tastgrad, analoge und digitale Ausgänge
  - Integrationen unter anderem für AdvancedOptoWave, Coherent, IPG, JPT, Photonics Industry und Spectra Physics
- Leistungsmesser und PowerMap
  - Coherent PowerMax, Thorlabs über OPM und Ophir über StarLab
  - Ausgangskompensation anhand einer PowerMap
- Rendering und Geometrieverarbeitung
  - OpenGL-3.3+-Renderer für 2D/3D mit einer orthografischen und fünf perspektivischen Kameras
  - AABB-Beschleunigung für Trefferprüfungen von Punkten, Linien, Linienzügen und Dreiecken
  - Topologiebewusstes Schneiden von 3D-Netzen mit Diagnose offener und geschlossener Konturen
  - Winding-basierte Mehrfachschraffuren für Außenkonturen, verschachtelte Bereiche und verbundene Barcodezellen
- Entitäten, Text und Barcodes
  - Punkte, Linien, Bögen, Polylinien, Dreiecke, Rechtecke, Spiralen, Trepanierbahnen und Splines
  - Würfel, Kugeln, Zylinder, Kegel, Netze, Ebenen, Gruppen, Blöcke und Blockeinfügungen
  - Text, SiriusText, ImageText, kreisförmiger Text, verknüpfter Text und ZPL-Entitäten
  - 1D-, QR-, DataMatrix-, PDF417- und Aztec-Barcodes mit Kontur-, Schraffur- und Punktzellenbearbeitung
- Dateiimport und Interoperabilität
  - Sirius3-Dokumente, DXF/DWG, HPGL/PLT, Gerber/Excellon und G-code/NGC
  - Rasterbilder sowie STL-, OBJ-, PLY- und STP/STEP-3D-Modelle
  - Toleranzbasiertes Verbinden von Vektorpfaden und inhaltsbasierte Gerber/Excellon-Erkennung
- Fernkommunikation und dynamische Daten
  - TCP/IP, seriell (RS-232), WebSocket und MQTT für Markersteuerung und Datenzugriff
  - Eingebetteter HTTP-MCP-Server für KI-gestützte Editorregistrierung, Dokument- und Entitätsbearbeitung, Simulation sowie Steuerung registrierter Geräte
  - Ereignis-, Datei-, Offset-, Verknüpfungs- und C#-Skriptkonvertierung für Text- und Barcodedaten
- Dokumente, Editoren und Simulation
  - Vier Dokumentseiten mit Ebenen, Stiften, Gruppen, Blöcken und konfigurierbarem Undo/Redo
  - WinForms- und WPF-`SiriusEditorControl` / `SiriusMultiEditorControl` werden in Debug und Release vollständig unterstützt; ein Dokument kann in mehreren Ansichten gerendert werden
  - Echtzeit-Laserpfad mit bildschirmfesten Markierungen, Strahleffekt und optionalen Partikeln
  - Rasterbasierte zusammengesetzte Bildansicht für Kamera- und Prüfabläufe
- Offene Architektur
  - Erweiterbare Schnittstellen für Editor, Entitäten, Marker, Scanner, Laser, Leistungsmesser und Fernsteuerung
  - Eingebetteter MCP-Server für KI-gestützte Editorregistrierung, Dokument- und Entitätsbearbeitung sowie Gerätesteuerung

## Wichtige Unterschiede zu Sirius2

| Funktion | SIRIUS3 | SIRIUS2 |
|:--|:--|:--|
| Dokumentseiten | 4 Seiten | Einzelnes Dokument |
| Kameras | 6 integrierte Kameras | Perspektivkamera |
| Rendering | GPU-beschleunigte OpenGL-Shader | Integrierte Shader-Engine |
| Treffertest | AABB-beschleunigt | Langsam |
| Schraffur | Winding-basierte Mehrfachschraffur | Einfache Schraffur |
| 3D-Netzschnitt | STL, OBJ, PLY, STEP | Nicht vorhanden |
| Gerber / Excellon | Inhaltsbasierte Erkennung | Nicht vorhanden |
| Stifte | Getrennte Entity- und Layer-Stifte | Ein Entity-Stift |
| MCP | Unterstützt | Nicht vorhanden |
| Aktualisierung | NuGet-Paketverwaltung | Manuell |

![sirius3_hatch](https://spirallab.co.kr/sirius3/sirius3_hatch.png)
![sirius3_pod](https://spirallab.co.kr/sirius3/sirius3_pod.png)
![sirius3_slicer](https://spirallab.co.kr/sirius3/sirius3_slicer.png)
![sirius3_syncaxis](https://spirallab.co.kr/sirius3/sirius3_syncaxis.png)

## Pakete / DLLs

- `SpiralLab.Sirius3.Dependencies` - SCANLAB RTC4/5/6, syncAXIS-Laufzeit, Schriftarten und Beispieldaten
- `SpiralLab.Sirius3` - Hardwareabstraktion für Scanner, Laser, Leistungsmesser usw.
- `SpiralLab.Sirius3.UI` - Entitäten, Geometrieverarbeitung, OpenGL-Rendering, WinForms/WPF-Steuerelemente und eingebettete MCP-Integration

Die Installation und Aktualisierung erfolgt über die NuGet-Paketverwaltung.

## Zielplattformen

- `net481`
- `net8.0-windows`
- `net9.0-windows`
- `net10.0-windows`

## Systemanforderungen

- Windows 10/11 (x64)
- GPU und Treiber mit mindestens OpenGL 3.3; aktuelle Treiber werden dringend empfohlen
- Installierte SCANLAB-Treiber und -Laufzeiten
- Visual Studio 2022 oder neuer

## Abhängigkeiten

- SCANLAB
  - RTC4: v2023.11.02
  - RTC5: v2024.09.27
  - RTC6: 2026.6.19 v1.25.0
  - syncAXIS: v1.8.2 (2023.03.09)
- .NET
  - OpenTK 3.3.3 für `net481`
  - OpenTK und OpenTK.Mathematics 4.9.4 für moderne Zielplattformen
  - Microsoft.Extensions.Logging 10.0.10 für alle Zielplattformen
  - Microsoft.Extensions.Logging.Abstractions 10.0.10 für alle Zielplattformen
  - Microsoft.Extensions.Logging.Debug 10.0.10 für alle Zielplattformen
  - Newtonsoft.Json 13.0.4
## Pakete installieren

Fügen Sie die folgenden NuGet-Pakete hinzu:

- `SpiralLab.Sirius3.Dependencies`
- `SpiralLab.Sirius3`
- `SpiralLab.Sirius3.UI`

## Schnellstart

# [WinForms](#tab/winforms)

Projekteinstellungen:

```xml
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

Beispielcode:

```csharp
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

        // Sirius3-Bibliothek initialisieren
        SpiralLab.Sirius3.Core.Initialize();

        // WinForms erstellen
        CreateAndExecuteMainForm();
    }

    static void CreateAndExecuteMainForm()
    {
        // Dynamisches Formular erstellen und SiriusEditorControl hinzufügen
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
            // Geräte erstellen, initialisieren und bei EditorControl registrieren
            bool success = true;

            // Scanner-Steuerung
            string correctionFile = "cor_1to1.ct5";
            string correctionPath = Path.Combine(SpiralLab.Sirius3.Config.CorrectionPath, correctionFile);
            const double fov = 100.0;
            var kfactor = Math.Pow(2, 20) / fov;
            var index = 0;
            var rtc = ScannerFactory.CreateRtc5(index, kfactor, LaserModes.Yag1, RtcSignalLevels.ActiveHigh, RtcSignalLevels.ActiveHigh, correctionPath);
            success &= rtc.Initialize();
            rtc.CtlFrequency(50 * 1000, 2);
            rtc.CtlSpeed(100, 100);

            // Digitale E/A-Steuerung
            var dIExt1 = IOFactory.CreateInputExtension1(rtc); success &= dIExt1.Initialize();
            var dOExt1 = IOFactory.CreateOutputExtension1(rtc); success &= dOExt1.Initialize();
            var dOExt2 = IOFactory.CreateOutputExtension2(rtc); success &= dOExt2.Initialize();
            var dILaserPort = IOFactory.CreateInputLaserPort(rtc); success &= dILaserPort.Initialize();
            var dOLaserPort = IOFactory.CreateOutputLaserPort(rtc); success &= dOLaserPort.Initialize();

            // Leistungsmesser-Steuerung
            double laserMaxPower = 20;
            var powerMeter = PowerMeterFactory.CreateVirtual(index, laserMaxPower);
            //var powerMeter = PowerMeterFactory.CreateCoherentPowerMax(index, 4);
            //var powerMeter = PowerMeterFactory.CreateGentecEO(index, 3, scaleIndex: null);
            success &= powerMeter.Initialize();

            // Laser-Steuerung
            var laser = LaserFactory.CreateVirtualDutyCycle(index, laserMaxPower, 0, 100);
            //var laser = LaserFactory.Create ...
            success &= laser.Initialize();
            laser.Scanner = rtc;

            // Leistungskennfeld
            var powerMap = PowerMapFactory.CreateDefault(index, "default");
            powerMap.Reset1to1("10000", laserMaxPower);
            laser.PowerMap = powerMap;

            // Marker
            var marker = MarkerFactory.CreateRtc(index);
            //var marker = MarkerFactory.CreateRtcFast(index);
            //var marker = MarkerFactory.CreateSyncAxis(index);
            success &= marker.Initialize();

            Debug.Assert(success);

            // Geräte registrieren
            editorControl.RegisterDevices(rtc, laser, powerMeter, dIExt1, dILaserPort, dOExt1, dOExt2, dOLaserPort, marker);

            // Authentifizierten HTTP-MCP-Server starten
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

            // MCP-Server vor Editor, Dokument und Geräten freigeben
            mcpServer?.Dispose();
            mcpServer = null;

            // Geräte freigeben
            editorControl.DisposeDevices();

            // Dokument freigeben
            editorControl.Document?.Dispose();

            // Sirius3-Bibliothek bereinigen
            SpiralLab.Sirius3.Core.Cleanup();
        };

        Application.Run(dynamicForm);
    }
}
```

# [WPF](#tab/wpf)

Verwenden Sie für ein WPF-Projekt die folgenden Einstellungen:

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
                throw new InvalidOperationException("Die virtuellen Geräte konnten nicht initialisiert werden.");

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

            // MCP-Server und Editor während Closing freigeben, solange WPF-Fenster und GL-Kontext bestehen.
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

## Demoprogramme

- Beschreibungen: [DEMOS.deDE.md](DEMOS.deDE.md)
- Beispiele: https://github.com/labspiral/sirius3/tree/main/demos

## Lizenz

- Für die kommerzielle Nutzung ist eine Lizenz erforderlich.
- Die Lizenz umfasst die Anzahl der RTC-Instanzen sowie optionale Funktionen:
  - MoF: Bearbeitung bewegter Objekte anhand externer Encoder
  - MultiBeam: eine Laserquelle, zwei AOM und zwei Scanköpfe
  - syncAXIS: XL-SCAN-Lösung aus Scanhead und ACS-Achse
  - Remote: Rezept-, Prozess- und Datenzugriff über Socket, seriell, WebSocket und MQTT sowie der eingebettete MCP-Server für KI-gestützte Dokument-/Entitätsbearbeitung und die Steuerung registrierter Geräte
- Siehe [LICENSE.deDE.txt](LICENSE.deDE.txt) und [THIRD-PARTY-NOTICES.deDE.txt](THIRD-PARTY-NOTICES.deDE.txt).
- Kontakt: hcchoi@spirallab.co.kr | https://spirallab.co.kr

> Ohne Lizenzschlüssel läuft die Software in einem auf 30 Minuten begrenzten Evaluierungsmodus.

## Versionsverlauf

- Siehe [HISTORY.deDE.md](HISTORY.deDE.md)

## API-Dokumentation

- https://spirallab.co.kr/sirius3/doc
