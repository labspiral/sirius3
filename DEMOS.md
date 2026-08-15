# List of Demo Programs

## beginner

Initializing the Sirius3 library
Initializing various devices (scanners, laser devices, etc.) and connecting to the UI

## console_document

Initializing various devices (such as scanners and laser devices) and using markers with documents in a console environment

## console_syncaxis_setup

This console demo performs low-level setup and diagnostics for an RTC6-based XL-SCAN system using syncAXIS.
It loads `syncAXISConfig.xml`, switches between simulation and hardware modes, controls Follow/Unfollow motion, moves the scanner and stage, and runs square, circle, calibration, laser-delay, and system-delay test jobs.

## editor_automatic_laser_control

This demo program automatically adjusts laser output using the Automatic Laser Control (ALC) feature. 
- The Defined Vector-based ALC function can be enabled via the Layer pen, and the start and end ramp values are set to ensure the output changes linearly at the beginning and end of machining.
- The Speed (or Velocity) Dependent-based ALC function can be enabled via the Layer pen, and you can select the minimum and maximum values, as well as the output signal and mode.
- The SDC (Spot Distance Control)-based ALC function can be enabled via the Layer pen, and the spot distance value can be set using the Entity pen.    
- It also includes a measurement function that samples the output data and visually displays the results.

## editor_barcode

You can create various types of 2D barcode objects and determine their cell combinations.
It also creates text objects containing the same data as the barcode.
The formatting of barcode and text objects is configured using the Entity pen.

## editor_barcode_textconvert

This is a demo program that demonstrates how to create 2D barcode objects and modify their data.
- Event-based: The user modifies the text just before processing via the marker’s event handler, and this text is dynamically updated and applied during processing.
- Script: The user writes and specifies C# script code. During rendering, the marker dynamically modifies the text of the barcode using the script just before rendering and applies it.
- External File: The user specifies an external file. Just before each rendering, the marker reads the first line of the text file to dynamically modify the data and apply it to the rendering. Additionally, the first line of the text file used is automatically removed.
- Offset: The user specifies text content along with multiple position (offset) values. During processing, the marker dynamically changes the text content at each offset position immediately before processing and applies it to the barcode.

## editor_dio

This is a demo program that utilizes the digital input/output (DIO) functionality of the RTC card’s expansion port.
Users can assign names to DIO pin numbers, and the program displays the system status (Ready, Processing, Error) as output.
It also implements features such as receiving an external signal (e.g., Start) to initiate processing.

## editor_document

Processed data (i.e. Recipes) is managed within an object called a Document, and various entities are created and stored within this Document.
At this point, the Editor renders the specified Document. Alongside methods for creating and replacing multiple Documents externally, this demonstrates an example of rendering a single Document via a Viewer to various editors and viewers.

## editor_entity

This example demonstrates how to generate various processed data and vector data provided by Sirius3 and add them to documents and pages.
It demonstrates how to reuse entities using Blocks and Insert, and also shows how to group and manage multiple entities.    

## wpf_editor_entity

This WPF version of `editor_entity` hosts the supported Material `SiriusEditorControl` and runs the same entity-creation samples on .NET Framework 4.8.1, .NET 8 and .NET 9 for Windows.
Its dedicated `config.ini` selects virtual scanner, laser and marker devices so the demo does not probe installed machining hardware by default.

## editor_entity_custom

This demo shows how to add application-specific entity types to Sirius3.
It implements custom rhombus, fiducial, and drill-hole entities and demonstrates property editing, geometry regeneration, rendering, cloning, hatching, marking, and insertion into a document.

## editor_fieldcorrection_2d

This is a demo program that utilizes the scanner's 2D field correction feature.
After marking patterns at regular intervals on the area to be processed, the system measures them using a measurement device (e.g., machine vision) and uses the error values to generate a new calibration file from the current one.
This compensates for nonlinear distortions in 2D space. It also demonstrates how to load the newly generated calibration file into the table and select it.

## editor_fieldcorrection_3d

This is a demo program that utilizes the scanner's 3D field calibration feature.
It demonstrates a procedural calibration method using the RtcCalibrationLibrary to leverage the scanner's 3D field calibration feature.
- Calibration of beam tilt in response to Z-axis movement
- Calibration to find the optimal focus position at Z=0
- Calibration to find the optimal focus at various positions in Z-space and determine the coefficients (A, B, C) of the equation Zout = A + Bl + Cl²
- Calibration to compensate for stretch size errors in the vertical Z-space

## editor_fieldcorrection_3d_pointscloud

This is a demonstration programme utilising the scanner’s 3D field correction function.
It generates a 3D correction file containing Z-height values by processing a 3D point cloud (a set of points constituting the surface of a 3D mesh).
Subsequently, when machining begins using 2D machining data, the laser focus automatically shifts to the 3D mesh surface at the corresponding X and Y positions.
At the application level, simply providing X and Y data results in these coordinates being projected onto the 3D surface, enabling laser machining on the 3D surface.

## editor_hardjump

All Jump and Mark commands for the scanner are processed as microvectors at a specified speed (mm/s), with commands executed at 10-microsecond intervals.
This demo program sets the jump section to a “Hard Jump” within a single cycle (10 μs) instead of processing it as a microvector.
While this drastically reduces the time required for jumps, it can place a significant burden on the scanner; therefore, it should be used appropriately depending on the scanner’s specifications and the characteristics of the processing data.

## editor_hatch

This is a demo program that generates hatching patterns for objects with various closed curve shapes.
Hatches can be created in line or polygon form, and you can set the hatch spacing, angle, and starting position.
Using path optimization (Sort) optimizes the hatching path, reducing machining time, and;
you can set different entity pens for each hatch to vary the machining conditions.

## editor_hatch_clip

This is a demo program that generates hatching patterns by clipping specific areas.

## editor_interrupt

The RTC card, as the name suggests, is a Real-Time Controller card. It pre-loads processing data into the list buffer and processes it in real time; therefore, it is difficult to insert various user functions during processing.
This demo introduces an interrupt feature where an interrupt occurs in the middle of list buffer processing, and the user application handles it before resuming processing.
This is a method where the list buffer remains in a ready-to-execute state even if an interrupt occurs, rather than stopping the list execution.

## editor_laser_ui

In Sirius3, by inheriting and implementing the ILaser interface, you can extend and use any laser device.
Additionally, you can design and implement a UI tailored to your custom laser device and integrate it directly.
This example demonstrates how to create and integrate a custom UI designed by the user using the OnCreateLaserUI event.

## editor_marker

This is the open-source code for MarkerRtc, MarkerRtcFast, and MarkerSyncAxis, which are provided within the Sirius3 library.
By modifying this open-source code and connecting it directly to Sirius3, users can customize the processing method to suit their needs.

## editor_measurement_skywriting_wobbel

This demo allows you to select various Skywriting modes (1, 2, 3, 4) using the Layer Pen and apply them to processing.
It is also a demo program that utilizes various Wobbel shapes via the Entity Pen.
Additionally, it includes a measurement function that samples output data and visually displays the results.

## editor_mof_interrupt

The MoF (Marking on the Fly) function allows the scanner to track and process a moving object in real time by inputting the encoder signal generated by the object into the RTC card.
This demo is a combined demonstration utilizing both the interrupt function of `editor_interrupt` and the MoF function.

When MoF processing begins, an interrupt occurs just before the object is processed, at which point the stage (or conveyor, etc.) is moved to the center position.
This example waits until the encoder reaches that position, then starts MoF (scanner tracking begins) and ends MoF after processing is complete.

## editor_mof_offsets

The MoF (Marking on the Fly) function allows the scanner to track and machine in real time by inputting encoder signals generated by an external moving object into the RTC card.
A virtual image field is used to perform MoF machining of a single machining pattern across a very large area.
- This method involves waiting until the encoder value corresponds to one of 1,000 offset positions, and then immediately performing machining if the condition is met.
- Here, we assume that the X-coordinate of the offset lies within the range of -200 to 0, and that the stage (or conveyor) moves to the right, 
- that is, in the direction where the scanner’s X value increases.
- Using the Entity Pen, this demonstrates not only machining conditions such as speed and power but also how to use the SCANAhead option feature in the RTC6 + excelliSCAN combination.

## editor_mof_trigger

The MoF (Marking on the Fly) function allows the scanner to process data in real time by tracking it based on encoder signals generated by an external moving object, which are input to the RTC card.
- This example demonstrates how the system waits for an external trigger at the RTC’s extended D.IN input port and immediately starts MoF processing when the trigger is received. 
- You can generate the desired barcode data immediately before each barcode processing and pre-insert it into the RTC buffer to process dynamic data.
- It also demonstrates a method of using Free Variables to accumulate the number of times the actual barcode object is processed after the trigger is received.

## editor_mof_xy

The MoF (Marking on the Fly) feature allows the RTC card to process barcodes in real time by tracking an encoder signal generated by an external moving object.
This example demonstrates how to create an encoder wait condition and configure it individually for each object.

## editor_mof_xy_raster

The MoF (Marking on the Fly) function allows the scanner to process data in real time by tracking an encoder signal generated by an external moving object and inputting it into the RTC card.
- For raster processing applications such as images, 1D and 2D barcodes, and ImageText, the encoder position is detected immediately before each line of processing.
- Once the wait is over, this feature ensures that raster processing occurs at the center of the scanner (or lens). 
- This is effective for MoF + raster processing of logos and other markings on the side of a cylinder while the cylinder is laid flat and rotated.

## editor_multibeam

This is a demo program that utilizes the Multibeam function, which splits a single laser source into a combination of two scan heads and two AOMs for processing.
- When one scan head begins a jump during machining, the laser output is turned off, 
- at which point the opposite scan head is released from standby to handle the machining, and this process repeats as the scan head returns control upon reaching the jump section. 
- Overall, this method maximizes the utilization of the laser source to increase productivity.

## editor_multibeam2

This demo provides an operating UI for a pair of RTC MultiBeam instances that share one laser source.
It checks the inter-head signal wiring, selects Head 1, Head 2, or Both mode, chooses the preparation side, and controls Ready, Start, Stop, and Reset for either head or both heads together.

## editor_multiple

This is a demo program that creates two systems (scanners, laser units, etc.) and two editors and data (documents) to process different data using a multi-head system.

## editor_multiple2

This is a demo program that creates two systems (scanners, laser devices, etc.) and one editor and data (Document) to process the same data using a multi-head system.

## editor_offset

This is a demo program that repeatedly processes the same data at multiple positions (offsets).
Offsets can be applied with different values for dx, dy, dz (movement distances), z-axis rotation angle, and scale.

## editor_pen

This demo program demonstrates how to modify machining conditions using Layer Pens and Entity Pens.
It also shows how to override the functions to which Layer Pens and Entity Pens are applied (via marker events).

## editor_pen_multiple

This demonstrates how to override settings to configure default values when Layer Pens and Entity Pens are first created.
It also demonstrates how to apply different Entity Pens to various objects to vary machining conditions.

## editor_points_sync_pulses_count

When processing dots (or pixels), a processing method is typically used in which the laser is activated (LASER ON) for a specified duration.
- The SYNC OUT signal output from the laser source is input to the RTC card and used as a synchronization signal, and;
- The time during which the laser is activated (LASER ON) for the corresponding point (or dot) is synchronized with the output pulse using the input pulse of the SYNC OUT signal.
- This is a demo program that demonstrates how to use various other available synchronization signals.

## editor_powermap

This demonstrates how to create and use a compensation map (or table) to compensate for the difference between the laser source’s output and the user’s desired actual output.
Additionally, `my_powermap.cs` is open-source code for the actual mapping, verification, and compensation procedures; users can modify this code to utilize the compensation map in their preferred manner.

## editor_remote

This is a demo program that attempts to establish a connection via remote communication (serial, TCP/IP, WebSockets, MQTT) and handles operations such as reading and writing markers, offsets, and object values through external systems.
Here, we are showing an example that uses WebSockets.

## editor_scanahead

This is a demo program that utilizes the SCANAhead feature supported by RTC6.
When the Auto delays feature is enabled, scanner jumps, polygon and mark delay times, as well as laser on/off delay times are automatically calculated and applied.
Additionally, the property lists of the Entity Pen are automatically shown or hidden to match the Auto delays feature.

## editor_scanahead_sdc

This is a demo program that utilizes the SCANAhead feature supported by RTC6.
It demonstrates how to combine SCANAhead and ALC functions using the Layer Pen and activate them, as well as how to set the SDC (Spot Distance Control) distance value using the Entity Pen.

## editor_script

This demo shows how marker scripts can update entity data immediately before marking.
It creates a SiriusText entity using `TextConverters.SimpleScript`, uses script expressions to generate serial numbers, and demonstrates inspecting, saving, restoring, loading, and compiling script instances from `.script`, C# source, or DLL files.

## editor_slicer

This is a demo program that demonstrates AM (Additive Manufacturing) functions, such as loading a 3D mesh model, slicing it along a specified Z-plane to extract contours, and inserting hatching into the resulting area.

## editor_steppermotor

This is a demo program that utilizes the control functions provided by the stepper motor expansion port on the RTC card.

## editor_stitchedimage

This demo creates an `EntityStitchedImage` from a camera grid, image resolution, and field-of-view settings.
It simulates moving the scanner to each tile and acquiring camera images from files, displays the combined result through `View.StitchedImage`, and demonstrates clearing and rebuilding the stitched image.

## editor_syncaxis

This is a demo program designed for use with the XL-SCAN system, rather than a standard RTC card.
Designed exclusively for large-area scanning systems combining ACS Motion Control + excelliSCAN (or intelliSCAN iV) + RTC6, this program demonstrates how to use the syncAXIS library for synchronized control of the scanner and motion controller, as well as the Motion Decomposition feature.

## editor_ui

This is the open-source code for the SiriusEditorControl and SiriusMultiEditorControl user controls provided within the Sirius3 library.
You can use this open-source code to design and customize the UI to your liking.

## editor_viewer

Processed data (i.e. Recipes) is managed within an object called a Document, which creates various entities and stores them within this Document.
This demonstrates how this Document is linked to an editor and a viewer for rendering. In other words, it provides an example of a ‘One Document, Multiple Views’ scenario.

## editor_zpl

This demo creates `EntityImageZPL` objects from several ZPL label samples using the offline BinaryKits renderer.
It demonstrates label size and print-density settings, Unicode ZPL data, `^CW` font mapping, and fallback font configuration for local preview and laser processing.
