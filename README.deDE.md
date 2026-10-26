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
  - Sirius3-Dokumente, DXF/DWG, SVG, HPGL/PLT, Gerber/Excellon und G-code/NGC
  - SVG-Vektorimport (`.svg`): `IDocument.ActImport` / `EntityFactory.CreateSVG`. Behält editierbare Originalkurven bei und erzeugt Füll-, Strichbreiten-, End- und Verbindungsregionen in Millimetern, einschließlich runder Enden und Ecken.
  - SVG-Schraffur: Aktivierung mit `UI.Config.IsSvgImportHatchEnabled` (Standard `false`), Abstand über `UI.Config.SvgImportHatchInterval` (Standard 0.1 mm). Linien-/Polygonschraffur wird unterstützt; Klick- und Ziehauswahl verwendet die angezeigten Konturen und Schraffuren.
  - Rasterbilder sowie STL-, OBJ-, PLY-, STP/STEP- und 3MF-3D-Modelle
  - 3MF-Netzimport (`.3mf`): `IDocument.ActImport` / `EntityFactory.CreateMesh`. Fasst die Dreiecksnetzmodelle und Komponenten des Builds unter Berücksichtigung relativer Platzierung und Quelldateieinheiten zu einem Netz in Millimetern zusammen. Gespeicherte Slicer-Bearbeitungspfade und Schnittergebnisse gehören nicht zum Importumfang.
  - Verfügbare Netzfarben und OBJ-/PLY-/3MF-Diffustexturen bleiben in der 3D-Vorschau erhalten
  - G-code/NGC-Import bewahrt modale XYZ-Koordinaten, Einheiten/Arbeitsursprünge, ebene Bögen/Helices, Verweilzeiten und Laser-ON/OFF-Pfade. `UI.Config.GCodeImportDialect` wählt CNC-, Laser- oder Marlin-Regeln; `GCodeImportInfo.TryGet` liefert Quellwerte F/S/T und Zeilenbereiche, ohne Geräteeinstellungen anzuwenden.
  - Toleranzbasiertes Verbinden von Vektorpfaden und inhaltsbasierte Gerber/Excellon-Erkennung
- Fernkommunikation und dynamische Daten
  - TCP/IP, seriell (RS-232), WebSocket und MQTT für Markersteuerung und Datenzugriff
  - Eingebetteter HTTP-MCP-Server für KI-gestützte Editorregistrierung, Dokument- und Entitätsbearbeitung, Simulation sowie Steuerung registrierter Geräte
  - Ereignis-, Datei-, Offset-, Verknüpfungs- und C#-Skriptkonvertierung für Text- und Barcodedaten
- Dokumente, Editoren und Simulation
  - Vier Dokumentseiten mit Ebenen, Stiften, Gruppen, Blöcken und konfigurierbarem Undo/Redo
  - WinForms- und WPF-`SiriusEditorControl` / `SiriusMultiEditorControl` werden in Debug und Release vollständig unterstützt; ein Dokument kann in mehreren Ansichten gerendert werden
  - [Schrifteditor](SpiralLab.Sirius3.UI/FontEditor/README.md): unabhängige CXF/LFF/EO-FNT/DOT-Glyphenbearbeitung über **Font Edit** in WinForms/WPF; mm-Vektoren, ganzzahlige DOT-Zellen, BBox/Metriken, Undo/Redo, DXF-Import, Textvorschau und Konvertierung installierter TTF-Schriften. In Debug und Release immer enthalten.
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
| 3D-Netzschnitt | STL, OBJ, PLY, STEP, 3MF | Nicht vorhanden |
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
  - RTC6: 2026.8.17 v1.25.1
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

Installieren Sie die drei oben aufgeführten Sirius3-NuGet-Pakete. Die Ausschnitte zeigen den grundlegenden Ablauf; Geräteerstellung, Validierung und Ereignisbindung sind mit `...` verkürzt. Ordnen Sie Start, Laden und Beenden den jeweiligen Host-Ereignissen zu.

# [WinForms](#tab/winforms)

Projekteinstellungen

```xml
<PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFrameworks>net481;net8.0-windows;net9.0-windows;net10.0-windows</TargetFrameworks>
    <UseWindowsForms>true</UseWindowsForms>
    <ApplicationHighDpiMode>PerMonitorV2</ApplicationHighDpiMode>
</PropertyGroup>
```

Initialisieren Sie Core, erstellen Sie den Editor und registrieren Sie initialisierte Geräte. Die WinForms-DPI-Initialisierung erfolgt vorher; siehe den HiDPI-Abschnitt unten.

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

Halten Sie beim asynchronen Schließen die UI-Ereignisschleife aktiv, bis die Freigabe abgeschlossen ist:

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

Projekteinstellungen

```xml
<PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFrameworks>net481;net8.0-windows;net9.0-windows;net10.0-windows</TargetFrameworks>
    <UseWPF>true</UseWPF>
    <ApplicationManifest>app.manifest</ApplicationManifest>
</PropertyGroup>
```

Initialisieren Sie Core vor dem Erstellen des WPF-Editors und registrieren Sie die initialisierten Geräte beim Laden des Fensters.

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

Halten Sie beim asynchronen Schließen die UI-Ereignisschleife aktiv, bis die Freigabe abgeschlossen ist:

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

`DisposeDevices()` wartet zuerst auf die Freigabe registrierter MCP-Server und Remote-Verbindungen und gibt danach die Geräte frei. Nach Editor- und Dokumentfreigabe ruft die Anwendung beim Beenden `Core.Cleanup()` auf. Vollständige Beispiele zeigen den Schutz vor mehrfachen Schließanforderungen und die Fehlerbehandlung.

Vollständige Beispiele: [WinForms-Leitfaden (Englisch)](https://github.com/labspiral/sirius3/blob/main/doc/WinForms.md), [WPF-Leitfaden (Englisch)](https://github.com/labspiral/sirius3/blob/main/doc/WPF.md), [Demoprojekte](DEMOS.deDE.md).

---

## WinForms HiDPI

Aktualisieren Sie zuerst DLL/Paket. Bestehende `DpiUnaware`-Anwendungen können ihr bisheriges Verhalten beibehalten. Für eine Skalierung pro Monitor konfigurieren Sie die verwendende EXE vor dem Erstellen von Steuerelementen. Die DLL ändert den DPI-Modus des Prozesses nicht. Vorhandene Hostformulare können `AutoScaleMode.Font`, Schriftart und Designer-Basiswerte beibehalten. Prüfen Sie eigene feste Pixelmaße und manuelle Skalierung gesondert.

Bei .NET 8/9/10 mit manuellem `Main` wählen Sie Per-Monitor V2 vor `EnableVisualStyles` und `Core.Initialize`:

```csharp
#if NET8_0_OR_GREATER
Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
#endif
Application.EnableVisualStyles();
Application.SetCompatibleTextRenderingDefault(false);
```

Bei .NET Framework 4.8.1 ergänzen Sie die vorhandene app.config der EXE um diesen Abschnitt, behalten Bindungsumleitungen bei, deklarieren Windows-10-Kompatibilität im Manifest und rufen `EnableVisualStyles` vor dem Erstellen von Steuerelementen auf. Entfernen Sie widersprüchliche unaware-DPI-Deklarationen und `EnableWindowsFormsHighDpiAutoResizing=false`.

```xml
<System.Windows.Forms.ApplicationConfigurationSection>
  <add key="DpiAwareness" value="PerMonitorV2" />
</System.Windows.Forms.ApplicationConfigurationSection>
```

Mit `ApplicationConfiguration.Initialize()` setzen Sie `<ApplicationHighDpiMode>PerMonitorV2</ApplicationHighDpiMode>` im EXE-Projekt und behalten einen einzigen Initialisierungspfad bei. Schriftgrößen bleiben in pt. Erstellen Sie eigene ImageLists je DPI aus aufbewahrten Originalbildern neu; setzen Sie `ImageSize`/`ColorDepth` vor dem Hinzufügen von Bildern, behalten Schlüssel und Indizes bei und ersetzen Sie alle Verweise, bevor Sie eigene alte Listen freigeben. WPF verwendet 96 als Basis logischer Einheiten; multiplizieren Sie das Layout nicht erneut mit dem DPI-Faktor.

[Microsoft: .NET Framework HiDPI](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/high-dpi-support-in-windows-forms), [WinForms automatic scaling](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/forms/autoscale).

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
