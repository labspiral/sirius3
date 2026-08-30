# Liste der Demoprogramme

## beginner

Initialisiert die Sirius3-Bibliothek sowie Scanner, Laser und weitere Geräte und verbindet sie mit der Benutzeroberfläche.

## console_document

Zeigt die Verwendung von Geräten, Markern und Dokumenten in einer Konsolenanwendung.

## console_syncaxis_setup

Führt die grundlegende Einrichtung und Diagnose eines RTC6-basierten XL-SCAN-Systems mit syncAXIS aus. Das Beispiel lädt `syncAXISConfig.xml`, unterstützt Simulation und Hardwarebetrieb, steuert Follow/Unfollow und führt Bewegungs-, Kalibrierungs- und Verzögerungstests aus.

## editor_automatic_laser_control

Demonstriert Automatic Laser Control (ALC) für definierte Vektoren, Soll-/Istgeschwindigkeit und Spot Distance Control. Ausgangsdaten können gemessen und als Diagramm dargestellt werden.

## editor_barcode

Erzeugt verschiedene 2D-Barcodes, Zellkombinationen und den zugehörigen Klartext. Das Aussehen und die Bearbeitungsbedingungen werden über den Entity-Stift eingestellt.

## editor_barcode_textconvert

Ändert Barcodedaten unmittelbar vor der Bearbeitung durch Ereignisse, C#-Skripte, externe Dateien oder positionsabhängige Offsets.

## editor_dio

Verwendet die digitalen Ein- und Ausgänge der RTC-Erweiterungsports. Pins können benannt, Zustände ausgegeben und externe Startsignale verarbeitet werden.

## editor_document

Zeigt Erzeugung, Austausch und gemeinsame Darstellung von Dokumenten in Editoren und Viewern nach dem Prinzip „ein Dokument, mehrere Ansichten“.

## editor_entity

Erzeugt die von Sirius3 bereitgestellten Bearbeitungs- und Vektordaten, fügt sie Seiten hinzu und demonstriert Blöcke, Einfügungen und Gruppen.

## editor_entity_custom

Implementiert anwendungsspezifische Rhombus-, Fiducial- und Bohrungsentitäten einschließlich Eigenschaften, Regeneration, Rendering, Klonen, Schraffieren und Markieren.

## editor_fieldcorrection_2d

Erzeugt aus gemessenen Fehlern eines regelmäßigen Musters eine neue 2D-Korrekturdatei und demonstriert das Laden und Auswählen der neuen Tabelle.

## editor_fieldcorrection_3d

Führt die 3D-Kalibrierung mit `RtcCalibrationLibrary` schrittweise aus: Strahlneigung, Fokus bei Z=0, Fokuskoeffizienten A/B/C und Stretch-Korrektur im Z-Volumen.

## editor_fieldcorrection_3d_pointscloud

Erzeugt aus einer 3D-Punktwolke eine höhenabhängige Korrekturdatei. 2D-XY-Bearbeitungsdaten werden dadurch auf die Oberfläche des 3D-Netzes projiziert.

## editor_hardjump

Vergleicht normale MicroVector-Sprünge mit einem Hard Jump innerhalb eines 10-µs-Zyklus. Hard Jumps verkürzen die Sprungzeit, belasten den Scanner jedoch stärker.

## editor_hatch

Erzeugt Linien- und Polygonschraffuren für geschlossene Konturen, optimiert ihre Reihenfolge und weist einzelnen Schraffuren unterschiedliche Entity-Stifte zu.

## editor_hatch_clip

Erzeugt Schraffuren durch Ausschneiden bestimmter Bereiche.

## editor_interrupt

Unterbricht die Verarbeitung eines RTC-Listenpuffers, lässt die Anwendung eine Benutzerfunktion ausführen und setzt anschließend die weiterhin vorbereitete Liste fort.

## editor_laser_ui

Zeigt, wie ein eigenes `ILaser`-Gerät und eine zugehörige Benutzeroberfläche über `OnCreateLaserUI` integriert werden.

## editor_marker

Enthält den offenen Beispielcode für `MarkerRtc`, `MarkerRtcFast` und `MarkerSyncAxis`, damit der Markierablauf an eigene Anforderungen angepasst werden kann.

## editor_measurement_skywriting_wobbel

Kombiniert Skywriting-Modi, Wobble-Formen und eine Messfunktion zur grafischen Kontrolle der Ausgangsdaten.

## editor_mof_interrupt

Kombiniert MoF mit Listenunterbrechungen. Vor der Bearbeitung wird auf die Encoderposition gewartet, die Scannerverfolgung gestartet und nach Abschluss beendet.

## editor_mof_offsets

Bearbeitet ein Muster an bis zu 1.000 Offsetpositionen in einem virtuellen Bildfeld und zeigt zusätzlich SCANAhead-Einstellungen für RTC6 + excelliSCAN.

## editor_mof_trigger

Startet MoF nach einem externen Trigger, erzeugt dynamische Barcodedaten kurz vor der Bearbeitung und zählt ausgeführte Objekte mit freien Variablen.

## editor_mof_xy

Erzeugt für jedes Objekt eigene Encoder-Wartebedingungen für zweiachsige MoF-Anwendungen.

## editor_mof_xy_raster

Synchronisiert jede Rasterzeile von Bildern, 1D-/2D-Barcodes und ImageText mit der Encoderposition, sodass die Bearbeitung nahe der Feldmitte erfolgt.

## editor_multibeam

Verteilt eine Laserquelle über zwei AOM auf zwei Scanköpfe. Während ein Kopf springt, übernimmt der andere die Bearbeitung, um die Laserquelle besser auszulasten.

## editor_multibeam2

Stellt eine Bedienoberfläche für zwei RTC-MultiBeam-Instanzen bereit, prüft die Signalleitungen und steuert Head 1, Head 2 oder beide gemeinsam.

## editor_multiple

Erzeugt zwei vollständige Gerätesysteme, zwei Editoren und zwei Dokumente zur parallelen Bearbeitung unterschiedlicher Daten.

## editor_multiple2

Erzeugt zwei Gerätesysteme, die dasselbe Dokument über einen Editor bearbeiten.

## editor_offset

Wiederholt dieselben Daten an mehreren Positionen mit individuellen dx/dy/dz-Werten, Z-Drehung und Skalierung.

## editor_pen

Ändert Bearbeitungsbedingungen über Layer- und Entity-Stifte und demonstriert das Überschreiben ihrer Anwendung über Markerereignisse.

## editor_pen_multiple

Legt eigene Standardwerte für neu erzeugte Stifte fest und weist verschiedenen Objekten unterschiedliche Entity-Stifte zu.

## editor_points_sync_pulses_count

Synchronisiert die Laser-Einschaltdauer eines Punktes mit dem SYNC-OUT-Puls der Laserquelle und zeigt weitere verfügbare Synchronisationssignale.

## editor_powermap

Erzeugt, prüft und verwendet eine PowerMap zur Kompensation der Differenz zwischen angeforderter und gemessener Laserleistung. `my_powermap.cs` enthält den anpassbaren Ablauf.

## editor_remote

Verbindet externe Systeme über seriell, TCP/IP, WebSocket oder MQTT und liest bzw. schreibt Marker-, Offset- und Objektwerte. Das Beispiel verwendet WebSocket.

## editor_scanahead

Verwendet RTC6 SCANAhead und Auto Delays. Sprung-, Polygon-, Markier- und Laserzeiten werden automatisch berechnet und die Entity-Stift-Eigenschaften entsprechend gefiltert.

## editor_scanahead_sdc

Kombiniert SCANAhead und ALC über den Layer-Stift und setzt Spot Distance Control über den Entity-Stift.

## editor_script

Aktualisiert Entitätsdaten unmittelbar vor dem Markieren. Das Beispiel erzeugt Seriennummern und lädt oder kompiliert Skripte aus `.script`, C#-Quelldateien oder DLLs.

## editor_slicer

Lädt ein 3D-Netz, schneidet es an einer Z-Ebene in Konturen und fügt Schraffuren in die resultierenden Bereiche ein.

## editor_steppermotor

Steuert einen Schrittmotor über den Erweiterungsanschluss einer RTC-Karte.

## editor_stitchedimage

Erzeugt ein `EntityStitchedImage` aus Kameraraster, Auflösung und Sichtfeld, simuliert die Bildaufnahme pro Kachel und demonstriert Löschen und Neuaufbau.

## editor_syncaxis

Demonstriert ein XL-SCAN-System aus ACS Motion Control, excelliSCAN bzw. intelliSCAN iV und RTC6 einschließlich synchronisierter Bewegung und Motion Decomposition.

## editor_ui

Enthält den offenen UI-Quellcode für `SiriusEditorControl` und `SiriusMultiEditorControl` als Ausgangspunkt für eigene WinForms-Oberflächen.

## editor_viewer

Verknüpft ein Dokument gleichzeitig mit Editor und Viewer und zeigt das Szenario „ein Dokument, mehrere Ansichten“.

## editor_zpl

Erzeugt `EntityImageZPL` mit dem lokalen BinaryKits-Renderer und demonstriert Etikettengröße, Druckdichte, Unicode-ZPL, `^CW`-Schriftzuordnung und Fallback-Schriften.
