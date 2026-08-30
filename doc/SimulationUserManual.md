# [User Manual] Processing Route Simulation

> Reference version: Sirius3 1.12.3 (public Release features)


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


## 3. EditorControl UI and Shortcuts

You can regulate or control the simulation speed through the toolbar button and key combination at the EditorControl top.

Toolbar button: btnSimulation (Icon format)
   - Basic click: Start the simulation or stop if it is already in operation.
   - Speed Control: The speed is determined by the key you are pressing when you click the button.
       - Click (Basic): Run at Fast (Fast) speed
       - Ctrl + Click: Run at Normal (Normal) speed
       - Ctrl+Alt+Click: Run to Slow

Keyboard and control.
   - Simulation Start: Usually run through the toolbar button.
   - Cancellation and cancellation (ESC key):
       - When the simulation is ongoing, if you press the ESC key, the simulation will immediately be stopped.
       - EditorControl is designed to detect key entries internally to call Document.ActSimulateStop().


## 4. Check Simulation Status

   - IDocument.IsSimulationWorking: Returns whether the current simulation is in operation. When this state is true, the usual editing task may be limited.

   - In Sirius3 1.11.10 or later, path markers remain a fixed size on screen. Beam and optional debris effects help distinguish Jump and Mark motion.
   - In Sirius3 1.11.14 or higher, the ESC immediately stops the simulation, and the Virtual RTC Abort is repeated during the closing cleaning, or the normal Virtual Laser stop is not recorded as an error.


## 5. Usage Example (C#)

Simulation start code.

1 // Start simulation at normal speed for selected entities
   2 if (!document.IsSimulationWorking)
   3 {
   4     var selected = document.Selected;
   5     if (selected.Length > 0)
   6     {
   7         await document.ActSimulateStart(view, selected, marker, IDocument.SimulationSpeeds.Normal);
   8     }
   9 }

Simulation stop code.

1/Stop the simulation running.
   2 document.ActSimulateStop();
   3 



## 6. Cautions

   - Entity Selection Required: One or more entities must be selected to run the simulation (see Document.Selected)
   - Device settings: Simulations use the internal markers (IMarker) logic, so the markers objects must be normally assigned.
   - Rendering performance: In Slow mode, you can check the on/off state and the jump range of the laser very accurately, but it can take a long time if the entire route is long.
