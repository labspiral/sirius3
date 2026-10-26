# ILaser and Custom Laser UI User Manual

> API reference: Sirius3 Core/UI 1.17.0 (public features)

[English](LaserUsermanual.md) | [한국어](LaserUsermanual.koKR.md) | [All guides](README.md)

## 1. ILaser and LaserBase

`SpiralLab.Sirius3.Laser.ILaser` is the common source contract used by device registration and markers. It combines identity, state, scanner association, capability information, initialization, immediate abort/reset, list-recording lifetime, property notifications and disposal.

| Member/group | Role |
|---|---|
| Index, Name, LaserType | Device identity and source type. |
| IsReady, IsBusy, IsWarning, IsError | Report actual source/driver state. |
| MaxPowerWatt, LastPowerWatt | Rated/configured maximum and last commanded power; a command value is not a power-meter measurement. |
| Scanner | Scanner used for supported RTC-timed output. |
| IsPowerControl, PowerControlMethod, PowerControlDelayTime | Advertise and configure power-control capability. |
| IsGuideControl | Advertises guide-control capability. |
| Initialize, CtlAbort, CtlReset | Initialize communication, immediately abort, and reset source state. |
| ListBegin, ListEnd | Participate in the marker's paired scanner/laser recording lifecycle. |
| SyncRoot, PropertyChanged, Dispose | Synchronization, status notification and deterministic cleanup. |

An application can implement ILaser directly. Deriving from `LaserBase` reuses standard metadata, synchronized state notification, scanner association and lifetime behavior. Its abstract Initialize/CtlAbort/CtlReset methods still require an implementation. Override list recording when the source needs its own buffered transport. Core adapters must remain independent of the UI assembly.

## 2. Optional power and guide interfaces

| Interface | Members and purpose |
|---|---|
| `ILaserPowerControl` | PowerMap, CtlPower(targetWatt, category), ListPower(targetWatt, category). |
| `ILaserGuideControl` | IsGuideOn and CtlGuide(bool). |

`CtlPower` is immediate control. `ListPower` records power changes in the active scanner/laser list; the controller applies them during execution. Both take watts and an optional mapping category. Implement the appropriate analog, digital, serial or vendor transport, and return failure when the operation cannot be performed.

PowerMap compensation maps requested optical output to a commanded source value when configured and enabled. A category identifies the measurement table, not an automatic frequency-setting command. The custom demo also has its own IsLookUpPowerMap gate. Check the map's Boolean lookup result and the adapter's selected control method. See [PowerMap](PowermapUserManual.md).

Guide control is a separate capability. Implementing the interface and advertising IsGuideControl must agree. A ready UI checkbox is not an interlock or proof that the main beam is off; implement the source's actual guide/main-output relationship.

## 3. Implement a custom source

The public [laser_source.cs](../demos/editor_laser_ui/laser_source.cs) defines `Demos.MyLaserSource : LaserBase, ILaserPowerControl, ILaserGuideControl`. It demonstrates UserDefined1 identity, rated power, optional PowerMap, analog-port selection, voltage scaling, guide state and distinct immediate/list power methods.

Use it as the adapter structure. For an actual device, implement communication and validated state transitions instead of declaring readiness from software flags alone. Initialize with output inactive, validate the configured range and selected scanner capability, and propagate transport failures. Provide bounded diagnostic logging and idempotent cleanup after partial initialization.

Set Index, Name, rated MaxPowerWatt and Scanner before initialization. Set capability flags inside the implementation only when those functions are supported. Dispose only resources owned by the adapter; an assigned scanner or shared PowerMap retains its own owner.

## 4. Create the matching WinForms panel

[LaserUIControl.cs](../demos/editor_laser_ui/LaserUIControl.cs), its [Designer](../demos/editor_laser_ui/LaserUIControl.Designer.cs) and resource file provide the public custom panel. The control receives a non-owned ILaser through LaserSource, displays its properties/status and uses capability interfaces for supported actions.

Register `UI.Config.OnCreateLaserUI` before assigning the editor's Laser or calling RegisterDevices. Its signature is `Func<ILaser, System.Windows.Forms.Control>`.

Inside the demo host, the factory can be written as:

```csharp
using SpiralLab.Sirius3.Laser;

System.Windows.Forms.Control CreateLaserPanel(ILaser source)
{
    return new Demos.LaserUIControl { LaserSource = source };
}

SpiralLab.Sirius3.UI.Config.OnCreateLaserUI += CreateLaserPanel;
// Assign/register the initialized source while the handler is attached.
// Detach the same handler when the owning host is closing.
SpiralLab.Sirius3.UI.Config.OnCreateLaserUI -= CreateLaserPanel;
```

WinForms expects one registered factory. That factory replaces built-in panel selection; returning null does not automatically select a built-in panel. Return a suitable panel for every source that the host may assign. The demo's generic panel can display the ILaser instance without requiring a cast to MyLaserSource.

## 5. Initialization and registration order

1. Initialize Core and create/initialize the scanner and related devices from the host's config/factories.
2. Create the custom laser with the correct index, rated power, scanner and validated device settings; initialize it successfully.
3. Register the panel-creation handler before the editor receives that source.
4. Register the complete device set using the custom source instance.
5. Check the resulting Marker readiness and logs. Any explicit Ready call must use that same custom laser.

`RegisterDevices` connects the marker to the editor document/view and assigned devices. A source created by a helper and subsequently replaced is still host-owned and must be disposed if no longer used. Do not rebind the marker to the discarded original source after registering a custom source.

The public [Form1.cs](../demos/editor_laser_ui/Form1.cs) shows the customization seam and factory hookup. Its illustrative default-source creation must be completed with the application's rated power, initialization and consistent registration/Ready references; do not treat the template as commissioned hardware configuration.

## 6. Native WPF customization

WPF uses `UI.Config.OnCreateLaserUIWPF`, which returns `System.Windows.Controls.Control`. Register it before assigning the source, and return a native WPF panel bound to the non-owned ILaser. A null result can continue to the built-in WPF selection. Keep WinForms and WPF handlers separate; do not return a WinForms control from the WPF event.

Built-in WPF resources are initialized by the Sirius3 controls. A custom panel owns its event subscriptions, dispatcher work and timer; it does not automatically own the assigned laser. See [WPF integration](WPF.md).

## 7. Status updates and cleanup

Use PropertyChanged to refresh source status on the panel's owning UI thread. Avoid driver probing or blocking I/O on the UI/render thread, prevent value-update reentry, and enable controls according to actual readiness/capability. Keep handler references so subscriptions can be removed.

The host must unsubscribe its static UI-creation handler and stop panel work. Await registered remote/MCP/device cleanup with editor.DisposeDevices, dispose the control/document as owned, and finish Core cleanup after those lifetimes end. Source Dispose should release only its communication/resources, without disposing a caller-owned scanner or UI.

## 8. Public source and related guides

[editor_laser_ui](../demos/editor_laser_ui/editor_laser_ui.csproj) ships its source in namespace Demos and remains a NuGet consumer. Copy the source/control/Designer/resource files as a coherent set when adapting it. Version 1.16.0 supports the common host APIs for both WinForms and WPF.

Related guides: [virtual laser](LaserVirtualUserManual.md), [pens](PenUserManual.md), [markers](MarkerUserManual.md), [WinForms](WinForms.md), [digital I/O](DigitalIOUserManual.md).

---
2026 Copyright (c) SpiralLAB. All rights reserved.
