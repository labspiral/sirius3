# Sirius3 Versionsverlauf

## v1.12.2 (2026.8.18)

- hinzugefügt) Die Entität `EntityCircularSiriusText` wurde hinzugefügt
- behoben) Das AutoKerning von SiriusText berechnet den Zeichenabstand jetzt aus dem tatsächlichen Mindestabstand der Glyphensegmente einschließlich diagonaler Abstände

## v1.12.1 (2026.8.16)

- hinzugefügt) `EntitySiriusText.IsAutoKerning` unterstützt im variablen Zeichenabstand automatisches Kerning benachbarter Glyphen anhand zwischengespeicherter Schriftkonturen
- behoben) Der feste Zeichenabstand behält bei Inhaltsänderungen die Positionen der Zeichenzellen und beide äußeren Zellränder bei, unterstützt konfigurierbare mehrsprachige Schriftmetrik-Muster und wählt Fallbacks anhand der Unicode-17-Schriftbereiche
- behoben) Verwendet auch das Ziel von `TextConverters.Link` den Konverter `Link`, wird keine Link-Kette mehr verfolgt; stattdessen wird ein Fehler protokolliert und die Markierung beendet
- geändert) Ist `scaleIndex` bei der Gentec-EO-Initialisierung `null`, werden die aktuelle Skala und Auto-Scale-Einstellung des Geräts beibehalten, statt Auto-Scale einzuschalten
- behoben) Leerer oder ungültiger Barcodetext protokolliert einen Kodierungsfehler und entfernt die vorherige Data-Matrix-, QR-, PDF417-, Aztec- oder 1D-Barcodegeometrie aus dem Editor
- hinzugefügt) `ITransformable` stellt mit `OriginalIn/Out`, `ModelIn/Out` und `RealIn/Out` Pfadanfangs- und Endpunkte in Quell-, eigenständig modelltransformierten und über alle Eltern kumulierten Weltkoordinaten bereit
- geändert) RTC-2D/3D-Korrekturdialoge, gemeinsame Validierungsfehler und Schaltflächen des benutzerdefinierten Meldungsfensters verwenden Sirius3-Ressourcen für Englisch, Koreanisch, vereinfachtes Chinesisch, Japanisch und Deutsch

## v1.11.14 (2026.8.7)

- behoben) OpenTK 3 und 4 lesen und restaurieren Polygonmodi passend zum Kontext und vermeiden Release-Speicherfehler sowie verschwindende Begrenzungsrahmen
- behoben) Die Laserpfadsimulation beendet sich mit ESC ohne PropertyGrid-Timeout oder wiederholte Abbrüche virtueller Geräte
- behoben) Das Abbrechen der F5-Startbestätigung öffnet den Dialog nicht erneut
- behoben) Lokalisierte PropertyGrid-Beschreibungen zeigen wieder zusammengehörige Einstellungen, Warnungen und Reihenfolgen auf getrennten Zeilen
- behoben) MultiBeam JumpAndShoot überlappt die Tokenfreigabe mit realen Sprüngen und gruppiert nur kurze Sprünge
- behoben) Editor- und Baum-Tastenkürzel funktionieren zuverlässig; Pfeiltasten bleiben für die Baumnavigation verfügbar
- behoben) Zoom und Verschieben bleiben während der Markierung und über Remote Desktop verfügbar, während die Auswahl gesperrt bleibt
- geändert) DXF, DWG, HPGL und PLT verwenden gemeinsam `Config.ImportMergeDistance`
- geändert) Core-/UI-Konfigurationsbezeichnungen folgen der Sprache; DXF, DWG und Gerber können Quellfarben erhalten oder auf Entity-Stiftfarben abbilden
- hinzugefügt) PropertyGrid-Suche nach Name, Kategorie und Beschreibung mit STRG+F

## v1.11.11 (2026.8.5)

- behoben) RTC6 verwendet die passende Controller-API für Status und Analog-E/A, erkennt Ethernetfehler und beendet Status-Timer ohne Rennen
- behoben) syncAXIS-Jobs löschen ihren Busy-Zustand nach Abschluss und melden Konfigurationsfehler konsistent
- behoben) Stabilere StreamParser-Verbindung, Wiederverbindung und Bereinigung ausstehender Empfangsarbeit
- behoben) Optionale Barcodekodierung, getrennte angeforderte/tatsächliche Punktdimensionen und stabile Data-Matrix-Größen

## v1.11.10 (2026.8.1)

- behoben) Barcodes bleiben in der angeforderten Größe; Bearbeitungspfad, Zusatzcodes, Schraffur und Punktzellen stimmen überein
- behoben) Schnelleres und zuverlässigeres Schneiden von 3D-Netzen mit verständlichen Warnungen
- behoben) Stabilerer AABB-Treffertest ohne Veränderung der Geometrie
- behoben) Zuverlässigere Listen- und Vorschaueditoren für Hatch, ALC und Stifte
- hinzugefügt) Laserpfadsimulation mit bildschirmfesten Markierungen, Strahl und ausblendenden Partikeln
- behoben) Vektor-, Gerber- und Excellon-Import verbindet nahe Pfade, erkennt Inhalte und überspringt nicht unterstützte Dateien sicher

## v1.11.0 (2026.7.27)

- hinzugefügt) `EntityStitchedImage` und Verwendung über `IView.StitchedImage`
- hinzugefügt) Unterstützung von `IEntityCloneable`
- behoben) Auswahlhervorhebung bestimmter Entitäten
- überarbeitet) OpenGL-Rendering in eine Renderer-Schicht verschoben und Auswahlindikatoren verbessert
- behoben) PropertyGrid-Werte außerhalb des Bereichs werden auf Min/Max begrenzt

## v1.10.14 (2026.7.10)

- hinzugefügt) `TextConverters.Link` liest Eigenschaften eines über `LinkEntity` verknüpften Objekts
- behoben) Stabileres Undo/Redo einschließlich Überschreitung von `Config.UnReDoSize`
- behoben) Stabilere OpenGL-Initialisierung auf Intel-GPUs
- behoben) `QuiteZone` bei `EntityBarcode1D_V2` gilt als linker und rechter Rand
- hinzugefügt) `UI.Config.MaxDegreeOfParallelism` begrenzt parallele Aufgaben

## v1.10.11 (2026.7.1)

- überarbeitet) Kerning, feste/variable Breiten und externe `.fnt`-Schriften für Textentitäten
- hinzugefügt) `Config.IsConvertToControllerResolution` für RTC-gerechte Ausgabeauflösung von Zeit- und Frequenzwerten
- behoben) Koreanische und konfigurierbare Fallback-Schriften für lokale ZPL-Konvertierung
- behoben) Verarbeitung mehrerer Remote-Textdatensätze
- behoben) ZoomFit bei Größe 0, Dateikonverter-Zeilenlöschung und Bearbeitung nach Undo

## v1.10.10 (2026.6.22)

- hinzugefügt) Feste Gesamtbreite für `EntityText` und `EntitySiriusText`
- hinzugefügt) Lokale ZPL-Bildkonvertierung mit BinaryKits
- hinzugefügt) `OriginalDimension`, `ModelDimension` und `RealDimension`
- hinzugefügt) Hatch-Ausrichtung None, Center und Fit
- hinzugefügt) GS1-Trennzeichenkonvertierung
- behoben) Bildtexturen in mehreren Ansichten
- behoben) Zulässige Inhalte von `EntityUniformGroup`
- behoben) 2D-Scannerkalibrierung bis 99 x 99 Punkte
- behoben) Rücktransformation gemessener Koordinaten in `RtcCalibrationLibrary`

## v1.9.0 (2026.6.1)

- hinzugefügt) Import von G-code über `.gcode`/`.ngc`
- verbessert) Erweiterte Werte in `TextConverters.Offset` und Remote-Textbefehl
- aktualisiert) Ophir StarLab v4.00
- behoben) Erstellung externer Entitäten; siehe `editor_entity_custom`

## v1.8.6 (2026.5.14)

- behoben) `HatchFills` und korrekte Hatch-Verarbeitung von Barcode-Outline-Zellen
- hinzugefügt) Konfigurierbare Hintergrundrastergröße über `IView.CheckerSize`
- behoben) Undo über Editor-Tastenkürzel und allgemeine Stabilität

## v1.8.5 (2026.5.8)

- hinzugefügt) Undo/Redo für unterstützte `IDocument.Act*`-Aktionen mit konfigurierbarer Historiengröße
- hinzugefügt) Aztec- und PLESSEY-Barcodes sowie bearbeitbare Pixelgröße
- hinzugefügt) CreateGrid für Punkte, Kreise, Kreuze und Raster
- hinzugefügt) RTC6 Pulse Picking für Femtosekundenlaser
- behoben) Asynchrone Initialisierung/Wartefunktionen für `IRtcStepper`
- behoben) Analogausgangsanzeige, MultiBeam-Tokenwechsel und Rasterverarbeitung von SEMI-OCR-Schriften

## v1.8.1 (2026.4.22)

- hinzugefügt) `IRemote` für Rezept-, Objekt-, Offset- und Markersteuerung über Serial, TCP/IP, WebSocket und MQTT
- hinzugefügt) C#-Skripte zur Echtzeitänderung von Textdaten vor der Bearbeitung
- hinzugefügt) SEMI-OCR-`.dot`-Schriften
- behoben) `IDocument.FindByName` und Umschaltzustände in MultiBeamControl

## v1.7.1 (2026.4.16)

- aktualisiert) RTC6-Paket v1.24.0
- behoben) Asynchroner `IMarker`-Ablauf mit Tasks
- behoben) Exklusive RTC-Synchronisierung für MultiBeam in Einzel- und Multi-Editor-Konfigurationen
- hinzugefügt) Filter- und Suchfunktion in LogControl
- behoben) Shaderverwaltung pro Ansicht
- behoben) 16-/20-Bit-Behandlung der 3D-Koeffizienten
- behoben) Speicherlecks, Snapshot-Speicherung und C#-Skriptausführung

## v1.6.1 (2026.4.9)

- hinzugefügt) `ViewerControl` und mehrere Ansichten eines Dokuments
- behoben) Erweiterte 3D-Kalibrierung über `RtcCalibrationLibrary`: Strahlneigung, XY, Z=0-Fokus, A/B/C und Stretch
- geändert) `RtcCorrection3D`, `KZScale` und `ZOffset` durch CalibrationLibrary bzw. MatrixStack ersetzt
- hinzugefügt) `EntityPoint`, `EntityBarcode1D_V2` und Datei-E/A für Vertexlisten

## v1.5.4 (2026.4.2)

- behoben) Design-Time-Ausnahmen von `SiriusEditorControl` und `SiriusMultiEditorControl`

## v1.5.3 (2026.3.31)

- hinzugefügt) `IView`-Verbindungseinstellungen und `IPowerMeter.MeasureUnits`
- behoben) Änderungsereignisse für RTC6 Auto Delays und Sichtbarkeit zugehöriger Stifteigenschaften
- behoben) Barcode-Punktreihenfolge, MultiBeam-Tokenbehandlung und Matrixunterstützung für 2D/3D-Korrekturdaten

## v1.5.2 (2026.3.27)

- hinzugefügt) RTC5/RTC6-Schrittmotorsteuerung über `IRtcStepper` und StepperControl
- hinzugefügt) Serielle RTC-Kommunikation über `IRtcSerialComm` und SerialCommControl
- verbessert) MoF Extension für drei Achsen und McBSP
- überarbeitet) Offset-, Marker-, Scanner- und Laser-Steuerelemente
- geändert) MatrixStack ohne BaseMatrix; Integration über `IRtc.CtlMatrix` und `ListMatrix`
- behoben) Leistungsmesser, Import externer `.sirius3`-Dateien und Anzeige von Layer-Stiftfarben
- hinzugefügt) Array-Einfügen im Editor

## v1.4.1 (2026.3.10)

- hinzugefügt) Web- und lokale API-Dokumentation
- hinzugefügt) PropertyGrid-Wertänderung durch Ziehen mit der rechten Maustaste
- behoben) RTC6-Pulswartezeit, EntityPen-Anzeige und Standardaktivierung aller vier Seiten

## v1.4.0 (2026.3.3)

- hinzugefügt) .NET 9.0-Windows und .NET 10.0-Windows
- hinzugefügt) Pulszählung über externes Lasersynchronisationssignal
- hinzugefügt) Experimentelles `IRtcMultiBeam`, `EntityPoints` und `IRtcIO`
- behoben) 16-Bit-Masken für Ext16-Entitäten und Design-Time-Stabilität
- geändert) Bei fehlender Lizenz/Option wird ein 30-minütiger Evaluierungsmodus verwendet

## v1.3.2 (2026.2.20)

- behoben) Kombinierter Extended Mode für ALC
- hinzugefügt) Spot-Distance-Einstellung für SCANAhead
- hinzugefügt) Encoderfehler-, Virtual-Field- und Filterfunktionen für `IRtcMoF`
- umbenannt) `IRtcWaitID` zu `IRtcInterrupt`

## v1.3.1 (2026.2.9)

- hinzugefügt) `IRtcSCANAhead`, SCANAhead-Stifteigenschaften und Position/Trajectory Acknowledge Limit
- hinzugefügt) `IRtcWaitID`

## v1.3.0 (2026.2.5)

- hinzugefügt) Vertexeditoren, `SiriusMultiEditorControl` und `EntityLayerPen`
- ersetzt) gnuplot durch integrierte Diagramme
- hinzugefügt) Optionale ODA-Konvertierung für DWG/DXF
- geändert) 3D ist Basisfunktion; syncAXIS ist eine Lizenzoption

## v1.2.7 (2026.1.26)

- hinzugefügt) Variable Polygon- und Sprungverzögerungen
- behoben) Skywriting-LaserOnShift, Bogenzerlegung, Konturschluss, Stiftfarben und Entfernung simulierter Entitäten

## v1.2.6 (2026.1.20)

- hinzugefügt) Ellipse, RampFactor und Hatch-Wiederholungen
- behoben) Anzeige von Stiftwerten, PowerMap-Kompensation und Vorschau mehrerer Begrenzungsrahmen

## v1.2.5 (2026.1.15)

- hinzugefügt) Kontur-Offset, Unterentitäts-Treffermodus und zusätzlicher Treffertest
- behoben) Detaillierter Treffertest und leeres Ungroup
- aktualisiert) ZXing 0.16.11 und Clipper2 2.0.0

## v1.2.4 (2026.1.7)

- hinzugefügt) Render-/Markierkürzel, FreeVariable-Ereignis und GridCloud-Abstand
- behoben) Gerber-Parsing, detaillierte Treffertests und Ungroup-Ausnahme

## v1.0.1 (2025.12.22)

- hinzugefügt) CHM-Dokumentation, Konturexpansion und Gentec-EO-Leistungsmesser
- aktualisiert) Ophir StarLab v3.93
- behoben) Hatch-Joints, Stiftsuche und Markerprotokollierung

## v0.9.3 (2025.12.5)

- hinzugefügt) ZoomFit und `TextConverters.Offset`
- behoben) Gerber-Import und Stiftzuordnung
- umbenannt) Scanner-Stift zu Entity-Stift

## v0.9.2 (2025.11.25)

- hinzugefügt) Konvertierung in Block/BlockInsert
- umbenannt) `EntityGroup` zu `EntityMixedGroup`
- behoben) Ungroup, Gruppenleistung, Gerber-Ladezeit und Speichern großer Dokumente

## v0.9.1 (2025.11.18)

- hinzugefügt) gnuplot im Dependencies-Paket und Uniform-Group-Befehl
- behoben) Uniform-Group-Rendering, Speicherlecks, Splines und sehr große Bäume
- geändert) `Core.Initialize`-Signaturen

## v0.8.2 (2025.11.11)

- behoben) HPGL-Parsing, Scanner-Stift und Stiftaktualisierung nach `ActNew`

## v0.8.0 (2025.11.7)

- Entwicklervorschau

## v0.1 (2025.03.06)

- Erstveröffentlichung
