# SyncAXIS Integrated Marker Control User Manual

> Reference version: Sirius3 1.16.0 (public Release features)


## 1. Overview

This guide explains how to use Sirius3 `MarkerSyncAxis` and `MarkerSyncAxisControl` with SCANLAB syncAXIS hardware for coordinated scan-head and motion-stage processing. syncAXIS combines the scanner and stage into one coordinated motion system for large-area or continuous-transport processing.

## 2. System status verification (Operation Status) - SyncAXIS

In addition to the normal Ready/Busy/Error status, the internal control status of SyncAXIS is displayed in color.
- Dark Gray (Unknown): The system is not initiated or communications failure.
- Red (Stop): a fatal error occurring in the system.
- Yellow (Warning): a state of attention, and certain conditions are not satisfied.
- Green (OK): All synchronization preparations are completed and processable.

## 3. Processing settings (Targets and Procedures)

- Mark Target (Mark Target):
  - All: Processing all entities within the drawing.
  - Selected: Only selected specific entities perform synchronization processing.
- Mark Procedure (Mark Procedure):
  - LayerFirst: Move the stage to each offset (product position), then process every Layer at that position.
  - Offset First: One layer is processed from all product locations and then passed to the next layer.

## 4. Main Action Buttons

- Start: Create and run the synchronized processing list of the scanner and stage.
- Stop: Stop all ongoing synchronization movements immediately.
- Preview: Use the guide laser to pre-display the entire range to be moved by the stage and the outer edge of the scanner processing area.
- Reset: Start the error status of the SyncAXIS controller and laser.

## 5. Simulation and Plot

SyncAXIS often uses the simulation function to predict the real path before processing.
- Measurement Plot: If this option is activated, the processing orbit data (.txt) generated in the simulation mode will be drawn into graph through the SyncAXIS Viewer immediately after processing is completed, which allows you to pre-verify the speed limit of the stage or the processing speed of the scanner.

## 6. Processing flow

1) Start the hardware in SyncAXIS mode (Confirm that the Operation Status is Green)
2) Set the drawings and offset information to be processed.
3) Use Preview to confirm that the stage travel envelope has no collision risk.
4) When you click 'Start', the stage and scanner will be synchronized in real time (10μs) and processing will begin.

## 7. Cautions

- SyncAXIS markers cannot be connected to ordinary RTC-only hardware (RTC5/6 alone).
- The SCANLAB's SyncAXIS Dungeon Key + ACS Motion Controller + ExcelliSCAN and the same system combination must be ready.
- All different environment settings according to the SyncAXIS Setup Manual must be completed in advance.
- If processing includes a path beyond the soft limit of the stage, hardware error occurs, so check it in advance through the simulation.
- The specific correction file (xml or .ct5) must be accurately charged for the synchronization accuracy of the stage and scanner.

## 8. Factory and comparison with other markers

`MarkerFactory.CreateSyncAxis(index)` creates `MarkerSyncAxis`; `CreateAuto(rtc)` selects it for `IRtcSyncAxis`. `MarkerRtc` uses ordinary RTC lists and `MarkerRtcFast` aggregates larger lists. `MarkerSyncAxis` builds and executes syncAXIS jobs according to the layer's Motion Type and cannot be substituted with an ordinary RTC marker.

The public [editor_marker](../demos/editor_marker/Form1.cs) supplies MyMarkerSyncAxis and MyMarkerRtc/MyMarkerRtcFast source. Initialize the scanner and laser, then bind those same instances through RegisterDevices/Ready. Start acceptance is not completion; observe Busy, errors and OnEnded.

Offset X/Y/Z supplies coordinate displacement, AngleZ rotates about Z and ExtensionData supplies per-location strings. Mechanical motion follows syncAXIS job, motion-type and axis-limit configuration. Offset Z alone is not an independent stage command. See [marker comparison](MarkerUserManual.md) and [Offset examples/parser](OffsetUserManual.md).

---
2026 Copyright (c) SpiralLAB. All rights reserved.
