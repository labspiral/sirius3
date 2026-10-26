# [User Manual] Processing Route Simulation

> API reference: Sirius3 Core/UI 1.17.0 (public features)

[English](SimulationUserManual.md) | [한국어](SimulationUserManual.koKR.md) | [All guides](README.md)


## 1. Overview

Simulation visualizes the marking path and scanner motion in the OpenGL View without emitting real laser output.
Processing order, jump routes, processing speed sensation, etc. can be reviewed in advance.



## 2. Execute the simulation (IDocument.ActSimulateStart)

Control the simulation through the IDocument.

The Method.
   - ActSimulateStart(view, entities, marker, simulationSpeed)
       - view: the IView object that will be simulated out
       - entities: Entities to simulate; a selected-entity simulation is also supported
       - Markers: Markers device abstracting interface
       - SimulationSpeed: Simulation Playing Speed (Slow, Normal, Fast)

Simulation stop (ActSimulateStop)
   - Stop the running simulation immediately and restore the view to the original state.


## 3. Editor commands

Use F1 / Ctrl+F1 / Ctrl+Alt+F1 for Fast / Normal / Slow simulation, or the simulation
toolbar button and its speed modifiers. Escape stops the current simulation.
These defaults follow `UI.Config.KeyboardSimulationStart`. The viewport or tree
must own focus; text fields keep their normal editing commands.

Simulation is display-only. It is separate from Marker Preview and Marker Start,
which can move scanners or operate real hardware.

## 4. Check Simulation Status

   - IDocument.IsSimulationWorking: Returns whether the current simulation is in operation. When this state is true, the usual editing task may be limited.

   - In Sirius3 1.11.10 or later, path markers remain a fixed size on screen. Beam and optional debris effects help distinguish Jump and Mark motion.
   - In Sirius3 1.11.14 or higher, the ESC immediately stops the simulation, and the Virtual RTC Abort is repeated during the closing cleaning, or the normal Virtual Laser stop is not recorded as an error.


## 5. Public API example

```csharp
if (!document.IsSimulationWorking && document.Selected.Length > 0)
    await document.ActSimulateStart(view, document.Selected, marker,
        IDocument.SimulationSpeeds.Normal);

// Stop the active simulation when requested.
document.ActSimulateStop();
```

Pass the application's assigned view and ready marker. Simulation uses the
selected source paths and processing order; it does not alter the recipe.

## 6. Cautions

   - Entity Selection Required: One or more entities must be selected to run the simulation (see Document.Selected)
   - Device settings: Simulations use the internal markers (IMarker) logic, so the markers objects must be normally assigned.
   - Rendering performance: In Slow mode, you can check the on/off state and the jump range of the laser very accurately, but it can take a long time if the entire route is long.
