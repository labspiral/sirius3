# Digital Input, Output and Controller Trigger User Manual

> API reference: Sirius3 Core/UI 1.17.0 (public features)

[English](DigitalIOUserManual.md) | [한국어](DigitalIOUserManual.koKR.md) | [All guides](README.md)

## 1. Interfaces and implementations

`IDInput` and `IDOutput` are the digital I/O contracts. `DInputBase` and `DOutputBase` supply buffered channels, names and common bit access. Concrete RTC adapters connect those buffers to the assigned scanner; the adapter does not own the scanner.

| Implementation | Port / usable bits |
|---|---|
| `RtcDInputExtension1Port` | EXTENSION 1 input, 16 bits |
| `RtcDInputLaserPort` | LASER-port input, 2 bits |
| `RtcDOutputExtension1Port` | EXTENSION 1 output, 16 bits |
| `RtcDOutputExtension2Port` | EXTENSION 2 output, 8 bits |
| `RtcDOutputLaserPort` | LASER-port output, 2 bits |
| `DInputVirtual` / `DOutputVirtual` | Software I/O without a physical port |

Use `IOFactory.CreateInputExtension1Port`, `CreateInputLaserPort`, `CreateOutputExtension1Port`, `CreateOutputExtension2Port`, `CreateOutputLaserPort`, `CreateVirtualInput` and `CreateVirtualOutput`. Initialize the scanner before an RTC adapter, initialize the adapter, then register the same instances with the editor. The built-in adapters use channel 0; validate bit numbers against the physical port width.

## 2. Buffers, polling and output submission

| Operation | Behavior |
|---|---|
| Input `Initialize()` | Captures an initial input snapshot. It does not report an already-HIGH bit as a new edge. |
| Input `Update()` | Reads hardware, updates the buffer and compares old/new states. |
| Input `IsAOn/IsAOff`, `GetChannel` | Reads the buffered state; does not itself sample the hardware. B-state methods are the inverse logic convention. |
| Output `OutOn/OutOff`, `SetChannel` | Changes the local output buffer and marks it for writing. |
| Output `Update()` | Flushes a modified buffer to the RTC port; otherwise attempts supported readback. |
| Output `IsOutOn/IsOutOff`, `GetChannel` | Reads buffered/readback state; does not prove that an external device received the output. |

A successful buffered bit edit is different from successful hardware submission. Check Update results and device logs. Full-word output updates can conflict with controller-list writes to the same port; define which component owns each output and serialize overlapping writes.

## 3. Rename channels with ChannelNames

`ChannelNames` is `string[][]`: use `ChannelNames[channelIndex][bitIndex]`. It supplies display labels, not hardware routing or bit renumbering. `Name` names the device/port itself. Match each row length to its usable bit count.

```csharp
using SpiralLab.Sirius3.IO;

if (input.ChannelNames != null && input.ChannelNames.Length > 0 &&
    input.ChannelNames[0] != null && input.ChannelNames[0].Length >= 2)
{
    var labels = (string[])input.ChannelNames[0].Clone();
    labels[0] = "Start sensor";
    labels[1] = "Part present";
    input.ChannelNames[0] = labels;
}
if (output.ChannelNames != null && output.ChannelNames.Length > 0 &&
    output.ChannelNames[0] != null && output.ChannelNames[0].Length > 0)
{
    var labels = (string[])output.ChannelNames[0].Clone();
    labels[0] = "Acknowledgement";
    output.ChannelNames[0] = labels;
}
```

Apply names before binding the editor. For an already visible panel, call its `UpdateExtension1PortNames` or `UpdateLaserPortNames` with the correct 16-bit or 2-bit label row; the output panel also has its Extension 2 naming route. Changing the array does not automatically notify every already-created label.

Control-entity bit names use the UI's own name mapping; the numeric BitPosition/bit mask remains authoritative. Renaming one adapter's ChannelNames does not change a wait entity's selected bit. Persist application labels in the host's configuration and reapply them when creating devices. See [editor_dio](../demos/editor_dio/Form1.cs).

## 4. DInput.OnChanged is a polled notification

The event is `Action<IDInput, int, SignalEdges>`. The sender is the input object; the bit number identifies the changed channel/bit (the base encodes it as `1000 * channel + bit`). With the built-in single-channel ports it is the bit number on channel 0. High reports an observed LOW→HIGH change; Low reports HIGH→LOW.

Hardware adapters compare their previous buffered word with the new Update snapshot. The event runs synchronously on the thread that called Update, after that adapter releases its data lock. It is not a hardware interrupt, timestamped pulse record or real-time trigger. A HIGH pulse that rises and falls between two samples can be entirely missed. UI scheduling, driver reads and a slow handler add latency; reducing a timer interval does not provide a deterministic deadline.

Integrated WinForms panels normally request 100-ms updates; standalone WinForms controls start at 50 ms. WPF panels normally request 100 ms and perform device reads on a worker task. Visibility/loading and host settings affect polling. The input object itself does not create a guaranteed periodic sampler.

Keep handlers short, dispatch UI changes to the owning framework and unsubscribe on teardown. Do not interpret event delivery time as the physical edge time or use a PC event handler for precisely timed laser/scanner sequencing. Software `DInputTriggerA/B` filtering also follows host sampling/timing.

```csharp
using (var simulatedInput = IOFactory.CreateVirtualInput(0))
{
    simulatedInput.Initialize();
    System.Action<IDInput, int, SignalEdges> handler = (port, bit, edge) =>
        System.Diagnostics.Trace.WriteLine($"{port.Name}: bit {bit}, {edge}");
    simulatedInput.OnChanged += handler;
    try
    {
        simulatedInput.OnByForcily(0);
        simulatedInput.OffByForcily(0);
    }
    finally { simulatedInput.OnChanged -= handler; }
}
```

Virtual forced-state methods notify immediately when they change a bit. This is useful for event wiring, and differs from physical-input polling.

## 5. Controller list conditions and Edge waiting

`IRtcConditionalIO` records conditions into a prepared RTC list. They are evaluated by the controller when that list executes. This keeps timing out of the Windows event loop.

| Entity | Role |
|---|---|
| `EntityWaitDataExt16Cond` | Level wait for all required LOW/HIGH mask conditions together. |
| `EntityWaitDataExt16EdgeCond` | Preferred LOW→HIGH or HIGH→LOW wait on one EXTENSION 1 bit. |
| `EntityWriteDataExt16Cond` | Conditional EXTENSION 1 output write; it does **not** wait for an input change. |

Edge wait uses `ListReadBitExtDI16EdgeWaitUntil`. Rising waits for LOW first, then HIGH; falling waits for HIGH first, then LOW. If the bit already has the final level when the command starts, it waits for a new opposite-level/target-level sequence. Pass `SignalEdges.High` for rising and `SignalEdges.Low` for falling.

These waits sample input levels in controller execution. A pulse must last long enough to be acquired and observed at the relevant command; the helper does not latch an arbitrary pulse that occurred before it was armed. A PLC handshake that holds a request until acknowledgement is preferable to an unverified short pulse.

Level waits use conditional absolute jumps on RTC4/4e and relative jumps on RTC5/6. The current RTC4/4e dedicated edge method returns success without recording a wait. On RTC4, use the two-level-wait composition below; the dedicated edge example applies to RTC5/6. Virtual does not wait on physical inputs. Confirm the assigned controller, installed SDK and available capability. syncAXIS does not use these RTC control entities. An unsupported scanner can skip an entity and return success, so a Boolean entity result alone does not establish that a physical wait occurred.

## 6. Edge wait followed by acknowledgement

The following constructs an RTC5/6 recipe fragment only. It does not initialize equipment, insert it into a document or start marking. DI0 is the request; DO0 is the acknowledged output.

```csharp
using SpiralLab.Sirius3.Entity;

IEntity[] triggerSequence =
{
    EntityFactory.CreateWaitDataExt16EdgeCond(0, SignalEdges.High),
    EntityFactory.CreateWriteDataExt16Cond(0, 0, 0x0001, SignalEdges.High)
};
```

The zero input masks make the write unconditional once execution reaches it. The Edge wait supplies the ordering. Place these control entities before the intended process entities in the actual layer traversal. After processing, add the application's verified acknowledgement-reset/handshake sequence. Dispose unattached fragments or transfer them through normal document actions.

The equivalent single-bit rising sequence is `CreateWaitDataExt16Cond(0x0001, 0)` followed by `CreateWaitDataExt16Cond(0, 0x0001)`. Reverse the levels for falling. The dedicated Edge entity expresses that intent more directly.

Waits have no automatic timeout in these entity contracts. The host/controller stopping procedure must remain available if the signal never arrives. Put waits at a deliberate point with laser output inactive; list submission is not completion.

## 7. Sixteen-bit masks and condition evaluation

LOW/HIGH refer to input signal levels, not the lower/upper word of a uint. The RTC6 manual's lower-16-bit rule means that bits 0–15 of each argument are evaluated.

| Call path | Argument order |
|---|---|
| CreateWriteDataExt16Cond / CreateWaitDataExt16Cond | LOW, HIGH, output (write only). |
| IRtcConditionalIO.ListWriteMaskExtDO16If / ListReadMaskExtDI16WaitUntil | HIGH, LOW, output (write only). |
| SDK set_io_cond_list / clear_io_cond_list | Mask1=HIGH, Mask0=LOW, output mask. |

For example, set DO4 HIGH if DI0 is HIGH and DI1–3 are LOW:

```csharp
var entity = EntityFactory.CreateWriteDataExt16Cond(
    0x000E, 0x0001, 0x0010, SignalEdges.High);
```

Entity.Mark passes HIGH=0x0001 and LOW=0x000E to the RTC interface, and the SDK receives that same HIGH, LOW order. The condition is `(input & high) == high && ((~input) & low) == low`. Two zero input masks make the write unconditional. Zero output-mask bits leave their outputs unchanged.

## 8. Ownership and related workflows

Register the same I/O instances with the editor and retain their scanner for their complete lifetime. Finish registered remote/MCP/device cleanup through `await editor.DisposeDevices()` before disposing controls/documents and calling Core.Cleanup. Standalone hosts own their polling task, event subscriptions and devices.

For precise input-driven processing, prepare the full list and let controller waits/conditions release subsequent commands. For an external hardware start, use the controller's separately documented external-start configuration; `DInput.OnChanged` calling a software Start is a host trigger with Windows latency.

Related guides: [markers](MarkerUserManual.md), [MoF](MarkingOnTheFlyUserManual.md), [entities](EntityUserManual.md), [WinForms](WinForms.md), [WPF](WPF.md).

---
2026 Copyright (c) SpiralLAB. All rights reserved.
