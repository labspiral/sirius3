# Sirius3

Eine Windows/.NET-Plattform für die präzise Laserbearbeitung. Sie verbindet SCANLAB-Steuerung, Geräteintegration, Geometrieverarbeitung, OpenGL-Visualisierung, Dokumentbearbeitung, Simulation und Markierausführung.

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
  - Ereignis-, Datei-, Offset-, Verknüpfungs- und C#-Skriptkonvertierung für Text- und Barcodedaten
- Dokumente, Editoren und Simulation
  - Vier Dokumentseiten mit Ebenen, Stiften, Gruppen, Blöcken und konfigurierbarem Undo/Redo
  - Stabile WinForms-Steuerelemente; ein Dokument kann in mehreren Ansichten gerendert werden
  - Echtzeit-Laserpfad mit bildschirmfesten Markierungen, Strahleffekt und optionalen Partikeln
  - Rasterbasierte zusammengesetzte Bildansicht für Kamera- und Prüfabläufe
- Offene Architektur
  - Erweiterbare Schnittstellen für Editor, Entitäten, Marker, Scanner, Laser, Leistungsmesser und Fernsteuerung

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
| Aktualisierung | NuGet-Paketverwaltung | Manuell |

![sirius3_hatch](https://spirallab.co.kr/sirius3/sirius3_hatch.png)
![sirius3_pod](https://spirallab.co.kr/sirius3/sirius3_pod.png)
![sirius3_slicer](https://spirallab.co.kr/sirius3/sirius3_slicer.png)
![sirius3_syncaxis](https://spirallab.co.kr/sirius3/sirius3_syncaxis.png)

## Pakete / DLLs

- `SpiralLab.Sirius3.Dependencies` - SCANLAB RTC4/5/6, syncAXIS-Laufzeit, Schriftarten und Beispieldaten
- `SpiralLab.Sirius3` - Hardwareabstraktion für Scanner, Laser, Leistungsmesser usw.
- `SpiralLab.Sirius3.UI` - Entitäten, Geometrieverarbeitung, OpenGL-Rendering und WinForms-Steuerelemente
- `SpiralLab.Sirius3.MCP` - Eingebetteter MCP-Server für laufende WinForms-Editoren, öffentliche API-Hilfe und freigegebene SCANLAB-Dokumente ([Nutzungs- und Sicherheitshinweise](SpiralLab.Sirius3.MCP/MCP.md))

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
  - RTC6: 2026.3.31 v1.24.0
  - syncAXIS: v1.8.2 (2023.03.09)
- .NET
  - OpenTK 3.3.3 für `net481`
  - OpenTK und OpenTK.Mathematics 4.9.4 für moderne Zielplattformen
  - Microsoft.Extensions.Logging 8.0.1 / 9.0.15 / 10.0.7 je nach Zielplattform
  - Microsoft.Extensions.Logging.Abstractions 8.0.3 / 9.0.15 / 10.0.7
  - Newtonsoft.Json 13.0.4

## Pakete installieren

Fügen Sie die folgenden NuGet-Pakete hinzu:

- `SpiralLab.Sirius3.Dependencies`
- `SpiralLab.Sirius3`
- `SpiralLab.Sirius3.UI`

## Schnellstart

Aktivieren Sie WinForms und die unterstützten Zielplattformen im Projekt:

```xml
<PropertyGroup>
  <OutputType>WinExe</OutputType>
  <TargetFrameworks>net481;net8.0-windows;net9.0-windows;net10.0-windows</TargetFrameworks>
  <UseWindowsForms>true</UseWindowsForms>
</PropertyGroup>
```

Initialisieren Sie `SpiralLab.Sirius3.Core`, erzeugen und initialisieren Sie Scanner, Laser, Leistungsmesser, E/A und Marker, registrieren Sie diese mit `SiriusEditorControl.RegisterDevices(...)` und geben Sie sie beim Schließen mit `DisposeDevices()` frei. Danach muss `Core.Cleanup()` aufgerufen werden. Das vollständige, kompilierbare Beispiel steht im Abschnitt [Quick Start der englischen README](README.md#quick-start).

## Demoprogramme

- Beschreibungen: [DEMOS.deDE.md](DEMOS.deDE.md)
- Beispiele: https://github.com/labspiral/sirius3/tree/main/demos

## Lizenz

- Für die kommerzielle Nutzung ist eine Lizenz erforderlich.
- Die Lizenz umfasst die Anzahl der RTC-Instanzen sowie optionale Funktionen:
  - MoF: Bearbeitung bewegter Objekte anhand externer Encoder
  - MultiBeam: eine Laserquelle, zwei AOM und zwei Scanköpfe
  - syncAXIS: XL-SCAN-Lösung aus Scanhead und ACS-Achse
  - Remote: Rezept-, Prozess- und Datenzugriff über Socket, seriell, WebSocket und MQTT
- Siehe [LICENSE.deDE.txt](LICENSE.deDE.txt) und [THIRD-PARTY-NOTICES.deDE.txt](THIRD-PARTY-NOTICES.deDE.txt).
- Kontakt: hcchoi@spirallab.co.kr | https://spirallab.co.kr

> Ohne Lizenzschlüssel läuft die Software in einem auf 30 Minuten begrenzten Evaluierungsmodus.

## Versionsverlauf

- Siehe [HISTORY.deDE.md](HISTORY.deDE.md)

## API-Dokumentation

- https://spirallab.co.kr/sirius3/doc
