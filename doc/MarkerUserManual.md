# RTC Laser Marker Control User Manual

> Reference version: Sirius3 1.12.3 (public Release features)


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

---
2026 Copyright (c) SpiralLAB. All rights reserved.
