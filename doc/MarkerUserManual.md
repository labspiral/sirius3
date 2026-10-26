# RTC Laser Marker Control User Manual

> API reference: Sirius3 Core/UI 1.17.0 (public features)

[English](MarkerUserManual.md) | [한국어](MarkerUserManual.koKR.md) | [All guides](README.md)


## 1. Overview

This guide explains how to use Sirius3 `MarkerRtc` and `MarkerRtcControl` for laser processing with a SCANLAB RTC controller. The Marker converts drawing entities into RTC list commands, submits those commands to the controller, and manages execution state.

## 2. Mark Targets

You can choose the range of entities to process.
- All (full): Sign all processable entities in the current document page in a row.
- Selected (optional): Mark only the specific entities that the user has chosen by the mouse.

## 3. Marking Procedure and Order

It is an important setting to determine the processing efficiency during multiple offset.
- Layer First (Layer First):
  - Method: Full Layer Processing in Offset 1 position -> Full Layer Processing in Offset 2 position...
  - Characteristics: This is the common way to move to the next product after one product is fully processed.
- Offset First (offset first):
  - Method: Layer 1 processing in all offset locations -> Layer 2 processing in all offset locations.
  - Use when a specific parameter order is important, such as during a tool change.

## 4. Hardware status and safety check (Health Checks)

You can set to automatically check the physical state of the RTC card and scanner before the processing starts.
- Check Temp: Make sure the temperature of the scanner head is normal.
- Check Power: Make sure the power supply is stable.
- Check Position Ack: verify that the scanner normally follows the trajectory commanded (within the traction range).

## 5. Main Action Buttons

- Start: Start real laser marking according to the set conditions.
- Stop: Stop the processing ongoing immediately. (RTC List Abort Command)
- Preview: Repeat the bounding box (Bounding Box) of the selected entity using the Red Pointer without shooting the real laser.
- Reset: Start the error status of the RTC card and laser source.

## 6. List Buffer and Measurement (List & Measurement)

- List Buffer: Select 'Auto' or 'Single' buffer mode. determines how to use hardware memory when processing a large amount of data.
- Measurement Plot: record real-time data, such as the scanner's location, speed, laser state, which occurs during processing, in the RTC internal memory, and visualize it as a graph after processing is very useful for processing quality analysis and debugging.

## 7. Status Indicators

- Ready: Hardware is initiated and processed ready (green).
- Busy: Current processing is ongoing or the list is running (yellow shake).
- Error: the state in which a lethal error occurred in the hardware or software (red).

## 8. Cautions

- During the processing (Busy state) you cannot change the marking procedure or the object.
- When you click 'Stop', the scanner returns to the source point (0.0) and the list being processed will be canceled.
- Before the actual processing, make sure to check the location and scope of the processing through the 'Preview' feature.

## 9. Scripts

- ScriptingInstance: Use 'ScriptFactory.Create' to create and specify external script objects.
- If you use TextConverters.SimpleScript for text and barcode objects, this script automatically supports string conversion.
- In the EditorControl, the short key (F2) allows you to verify and edit the properties of ScriptingInstance.

## 10. MarkerFactory and implementation selection

`MarkerFactory` creates an instance; it does not initialize devices, take ownership of the scanner or start a job. Choose the marker for the initialized scanner and bind the same document/view/scanner/laser/power meter through the editor's `RegisterDevices`/marker `Ready` flow.

| Factory / instance | Processing structure | Appropriate use and differences |
|---|---|---|
| CreateVirtual → MarkerVirtual | Software processing without physical RTC execution. | Device-free traversal/host development; does not validate hardware timing. |
| CreateRtc → MarkerRtc | Begins, ends and executes RTC lists at layer/offset boundaries. | General RTC4/5/6 workflow; handles layer-level ALC processing. |
| CreateRtcFast → MarkerRtcFast | Uses one continuous logical list between ListBegin and ListEnd rather than a separate pair for each layer. | Reduces submission overhead; suited to a single EntityLayer, for example MoF. Automatic buffering can still divide the native transmission. Layer-level ALC setup is not provided. |
| CreateSyncAxis → MarkerSyncAxis | Builds jobs by layer motion type through IRtcSyncAxis and coordinates laser lists. | syncAXIS scanner/stage processing; requires the compatible controller/job configuration. |
| CreateAuto(IRtc) | IRtcSyncAxis → SyncAxis; RtcVirtual → Virtual; otherwise Rtc. | Does not automatically choose RtcFast. |

```csharp
using SpiralLab.Sirius3.Marker;

// Creation only. Bind initialized devices before starting a job.
IMarker marker = MarkerFactory.CreateRtc(0);
IMarker fastMarker = MarkerFactory.CreateRtcFast(0);
IMarker syncMarker = MarkerFactory.CreateSyncAxis(0);
```

Fast is a different submission strategy, not a guarantee of a faster physical scan. Keep required per-layer setup and controller capabilities explicit when switching implementations. Do not substitute an ordinary RTC marker for a syncAXIS job marker. Factory selection alone does not commission a scanner, stage or laser.

`Start` accepting a task does not mean that machining has completed. Observe `IsBusy`, errors and `OnStarted`/`OnEnded` for lifecycle and completion. Event handler execution on the host is not a hard-real-time hardware trigger.

## 11. Public marker source

The public [editor_marker](../demos/editor_marker/editor_marker.csproj) demo supplies [Form1.cs](../demos/editor_marker/Form1.cs) and custom MyMarkerRtc, MyMarkerRtcFast and MyMarkerSyncAxis source. It is a NuGet consumer in namespace Demos. The host selects MyMarkerRtc for ordinary RTCs, MyMarkerSyncAxis for syncAXIS and the built-in virtual marker for RtcVirtual. MyMarkerRtcFast is supplied for customization but is not the default selection.

Use these sources to study traversal, list/job boundaries and hooks. Preserve initialization, capability checks, failure/abort handling and device ownership when changing them. A copied implementation must be reviewed against the installed SDK and the current library version.

## 12. Offset position and per-location data

`Offsets` adds placement through Translate.X/Y/Z (mm), AngleZ (degrees, positive counterclockwise) and Scale. ExtensionData carries per-location metadata; it does not alter coordinates. Z is a coordinate displacement, not an implicit stage-move request. Prepare offsets while idle and avoid duplicating the entity's placement transform.

With `TextConverters.Offset`, a plain ExtensionData string supplies the same value to all converted entities. `MyBarcode|A|MyText|B` assigns values by entity Name; JSON can represent values containing the separator. Delimiter names are case-insensitive, JSON keys are case-sensitive, and repeated delimiter names use the last matching pair. See [Offset](OffsetUserManual.md) for the parser, worked examples and transformation order.

Related guides: [syncAXIS](MarkerSyncaxisUserManual.md), [laser adapters](LaserUsermanual.md), [digital I/O](DigitalIOUserManual.md).

---
2026 Copyright (c) SpiralLAB. All rights reserved.
