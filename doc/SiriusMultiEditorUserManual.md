# SiriusMultiEditorControl User Manual

> Reference version: Sirius3 1.12.3 (public Release features)

## 1. Purpose

`SiriusMultiEditorControl` is a public WinForms control that shares one `IDocument` and editor while switching among one to four indexed Scanner/Laser/Marker device sets. It does not edit multiple recipes simultaneously; it connects the same recipe to multiple device configurations for status inspection and execution.

In public demo `editor_multiple2` and `editor_ui` you can check the initialization, device registration and UI source copy-based customization.

## 2. Initialization Order

1. Call `Core.Initialize()`.
2. Read `config*.ini` to generate Scanner, Laser, PowerMeter, DIO, Marker, and Remote for each index.
3. Set `MaxDeviceCounts` to 1 to 4.
4. Register each set as `RegisterDevices(index, ...)`.
5. Select the active set to display on the screen with `SwitchDevices(index)`.
6. When closing, stop and disable the Marker and Device and call `Core.Cleanup()`.

## 3. RegisterDevices

`RegisterDevices` connects Scanner, Laser, PowerMeter, Extension/LASER-port DInput·DOutput, Marker and Selective Remote to the same index. Inside it also calls `Ready` of that Marker so don't call again after registration and check `marker.IsReady` and logs.

The layout properties use the same index as one set.

- `Scanners[index]`
- `Lasers[index]`
- `PowerMeters[index]`
- `Markers[index]`
- DInput/Doutput and Remote Settings

## 4. SwitchDevices

`SwitchDevices(index)` re-connects the active Scanner/Laser/Marker to the sub-UI control and PropertyGrid. Document and selected objects areined, but the PropertyGrid items and status indications supported by the active device may be different.

Check whether the conversion is successful and the current index change event. If you have a running Marker, document editing or device conversion may be limited.

## 5. Safety and Cleanup

- Many devices share the same Document so don’t change process data when one Marker is Busy.
- Check KFactor, correction files, Laser Mode, PowerMax and PowerMap for each set according to the index.
- `DisposeDevices()` is an explicit action to fix a registered device. Do not confuse with the general Dispose of UI Control.
- The real Marker Start shortcut can run the selected active device.

## 6. Difference from Multi-Beam

`SiriusMultiEditorControl` is a control that converts several separate devices from the UI. Multi-Beam, which divides one laser source by two RTCs as AOM and Token, is a separate hardware structure responsible for `IRtcMultiBeam` and `RtcMultiBeamHelper`.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
