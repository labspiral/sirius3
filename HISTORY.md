# Sirius3 version history

## v1.13.0 (2026.8.31)

- updated) Updated the SCANLAB RTC6 dependency to Software Package 1.25.0, selected firmware automatically for the board revision during initialization, and exposed `RtcRevision` values 0 for Revision 1 and 1 for Revision 2
- added) Added the `EntityNURBSSurface`, `EntityTorus`, `EntityPlane`, and `EntityPyramid` 3D surface meshes and their factory methods, with WinForms editor support for creating and editing 3D meshes and splines and previewing slices
- added) Added Fixed character-cell width and glyph-width fitting to linear and circular Sirius/GDI text and `EntityImageText`, keeping character positions stable for whitespace and missing glyphs as well
- changed) Circular text now lays out line breaks as concentric lines, with more stable text baselines and logical bounds and improved transparent-margin calculation for `EntityImageText`
- fixed) 1D and 2D barcodes now keep `Width` and `Height` as independent maximum machining bounds and fit the encoded matrix inside them without exceeding the requested size
- fixed) Multi-pulse point marking by `EntityPoint` and `EntityPoints` now applies `EntityPen.IsPixelPulsesExit` to the RTC command
- fixed) The RTC 2D correction grid now accepts dx/dy separated by whitespace or a comma and again colors cells according to the error distance

## v1.12.2 (2026.8.18)

- added) Added the `EntityCircularSiriusText` entity
- fixed) SiriusText AutoKerning now calculates character spacing from the actual shortest distance between glyph segments, including diagonal distances

## v1.12.1 (2026.8.16)

- added) `EntitySiriusText.IsAutoKerning` now supports automatic kerning of adjacent glyphs from cached font outlines in Variable character-spacing mode
- fixed) Fixed character-spacing mode now preserves character-cell positions and both end-cell margins when content changes, supports configurable multilingual font-metric samples, and selects fallbacks using Unicode 17 script ranges
- fixed) When the target of `TextConverters.Link` also uses `Link`, marking now records an error and stops instead of following a consecutive link chain
- changed) Gentec-EO initialization now leaves the device's current scale and auto-scale setting unchanged when `scaleIndex` is `null`, instead of enabling auto-scale
- fixed) Empty or invalid barcode text now records an encoding error and clears the previous Data Matrix, QR, PDF417, Aztec, or 1D barcode shape from the editor
- added) `ITransformable` provides path start and end points for source, own model-transformed, and parent-accumulated world coordinates through `OriginalIn/Out`, `ModelIn/Out`, and `RealIn/Out`
- changed) RTC 2D/3D correction dialogs, shared validation errors, and custom message-box buttons now use Sirius3 resources for English, Korean, Simplified Chinese, Japanese, and German

## v1.11.14 (2026.8.7)

- fixed) OpenTK 3 and 4 now read and restore polygon modes safely for their context type, preventing Release-mode memory access failures, disappearing bound boxes, and an incorrect OpenTK 3 debug assertion
- fixed) Laser-path simulation now stops promptly with ESC without a PropertyGrid timeout, repeated virtual-RTC aborts during cleanup, or a normal virtual-laser abort being reported as an error
- fixed) Cancelling the F5 marker-start confirmation from the editor surface or a focused tree view no longer opens the same dialog a second time
- fixed) PropertyGrid descriptions again show related settings, cautions, and setup order on separate lines instead of dropping them during localization
- fixed) In multi-beam Both-mode JumpAndShoot, token-release waits now overlap real jumps while only cheaper short jumps are kept in small batches
- fixed) Editor and tree-view shortcuts, including CTRL+R, CTRL+M, camera, simulation, and marker keys, now work reliably while every arrow-key combination remains available for tree navigation
- fixed) Editor zoom and panning remain available during marking and remote-desktop use while selection stays locked
- changed) DXF, DWG, HPGL, and PLT path imports now share the user-configurable endpoint merge distance `Config.ImportMergeDistance`
- changed) Core and UI configuration labels now follow the selected language, and DXF, DWG, and Gerber imports can preserve source colors or map them to the nearest entity-pen color
- added) PropertyGrid properties can now be searched by name, category, or description, with CTRL+F focus and one-click clearing

## v1.11.11 (2026.8.5)

- fixed) RTC6 now reads status and analog I/O through the correct controller API, detects Ethernet connection errors correctly, and shuts down without racing its status timers
- fixed) syncAXIS jobs now clear their busy state reliably after completion and report configuration errors consistently
- fixed) StreamParser connection, reconnection, and shutdown are more stable, including safe cleanup of pending receive work
- fixed) Barcode encoding is now optional, requested dot dimensions are preserved with the actual matrix size available separately, and Data Matrix shape switching no longer shrinks the requested size

## v1.11.10 (2026.8.1)

- fixed) Barcodes now stay within the requested size, and their machining paths, supplemental codes, hatching, and dot-cell simulation line up correctly
- fixed) 3D mesh slicing is faster and more reliable, with clearer warnings for broken or incomplete meshes
- fixed) AABB hit testing is more stable and faster, without changing the entity geometry
- fixed) Hatch, ALC, pen, and other list editors now update their values and previews more reliably
- added) Laser-path simulation now uses fixed-size markers, a visible beam, and lightweight fading debris
- fixed) Vector, Gerber, and Excellon imports now join nearby paths automatically, detect file contents, and skip unsupported files safely

## v1.11.0 (2026.7.27)

- added) Added the EntityStitchedImage object
    - Can be used as IView.StitchedImage
- added) Added support for the IEntityCloneable interface
- fixed) Issue where some objects were not highlighted in bold when selecting entities
- refactor) Separated entity OpenGL rendering and improved selection indicators
    - Moved OpenGL calls to the renderer layer
- fixed) PropertyGrid input values
    - When values exceed the input range, resize them to fit between the maximum and minimum values instead of displaying a warning
     
## v1.10.14 (2026.7.10)

- added) TextConverters.Link
    - TextConverter retrieves the property values of a linked object using the LinkEntity name and converts them to text.
- fixed) Improved stability of Undo and Redo.
    - fixed an exception that occurred when performing Redo beyond the Config.UnReDoSize limit.
- fixed) Improved stability of OpenGL initialization
    - "Not Responding" Issue When Using Intel GPU
- fixed) EntityBarcode1D_V2
    - The "QuiteZone" value is now treated as left and right margins
- added) UI.Config.MaxDegreeOfParallelism
    - Supports limiting the maximum number of tasks used during parallel processing (default: 50% of the number of logical processors)
 
## v1.10.11 (2026.7.1)

- refactoring) Text
    - EntityText, EntityImageText, EntityCircularText: Applied kerning; center alignment applied when "Fixed" is used
    - EntitySiriusText: Center alignment applied when using "Fixed." Support for external binary formats (.fnt font files) added
    - EntityImageText: Support for setting the total width using TargetWidthPixels. Support for variable and fixed widths
    - EntityCircularText: Supports variable and fixed widths
- added) Config.IsConvertToControllerResolution
    - Determines whether the values set for EntityPen and EntityLayerPen (time, frequency, etc.) should be output as the actual converted values for the RTC controller
    - False: Default (outputs the values entered by the user as-is)
    - True: Values are converted to match the RTC controller’s control resolution
- fixed) EntityImageZPL
    - Support for Korean fonts when converting using BinaryKits 
    - Support for changing the conversion font via Config.ZPLBinaryKitsFonts
- fixed) Remote
    - Fixed a bug in handling multiple data entries with the text command
    - Example 1) text|1|Text_1|ABCD123;
    - Example 2) text|2|Text_1|ABCD1|Text_2|ABCD2|Text_1|ABCD3|Text_2|ABCD4;
- fixed) Bug
    - Issue where ZoomFit did not work for objects with a size of 0
    - Issue where the first line of data was not deleted when using TextConverters.File
    - Issue where an exception occurred when editing object properties after an Undo
     
## v1.10.10 (2026.6.22)

- added: Support for fixing the full width of text objects
    - Target objects: EntityText, EntitySiriusText
    - Added the "Target width" property
    - When set to 0, the text is generated at the optimal font size as before; 
    - If set to a value greater than 0, the scale is automatically adjusted based on the `Target width` value.
- added) Support for local ZPL image conversion.
    - `EntityImageZPL` object.
    - Previous: Supported online conversion via the Labelry web service.
    - Change: Supports offline conversion using the external BinaryKits library.
    - Default: Changed to use BinaryKits;
    - The generation service can be changed via UI.Config.ZPLService;
- added) Support for full-size conversion;
    - OriginalDimension: Outputs the size of the original entity;
    - ModelDimension: Supports changing the entity’s size (width, height, depth) in local space;
    - RealDimension: Outputs the dimensions (width, height, depth) in world (Real) space after applying the cumulative (full) ModelMatrix of all parent entities
- added) Hatch alignment support
    - Added Alignment to the HatchLine object
    - None: No alignment
    - Center (Default): Center alignment
    - Fit: Recalculates and adjusts spacing to be uniform
- added) GS1 format support
    - Support for converting &lt;GS&gt; and (,) delimiters in the GS1 format
- fixed) Image multi-view texture rendering
    - Affected objects: EntityImage, EntityImageText, EntityImageZPL
    - Fixed an issue where textures were not rendered when using multiple views
- fixed) EntityUniformGroup
    - Added constraints on objects that can be converted to uniform groups
    - Prohibited the addition of control objects and objects containing ITextConvertible or IHatch
- fixed) 2D scanner calibration
    - Supports up to 99x99 calibration points when using RtcCorrection2D
- fixed) RtcCalibrationLibrary calibration
    - Added a function to automatically perform inverse transformation on measured coordinates when using a matrix (MatrixPrimaryInternal) 
    - Functionality for automatically calculating original data when using a rotated scanner
     
## v1.9.0 (2026.6.1)

- added) Support for importing G-code
    - File extensions: .gocde or .ngc files
- fixed) Improvements to the TextConverter's TextConverters.Offset
    - Previous: Used the ExtensionData value of Offset as the converted text
    - Changed: When the ExtensionData value of Offset is an extended string in the format "Entity1|Value1|Entity2|Value2;...", TextConverter now supports parsing and using the corresponding keys and values
    - Added) Remote
        - added text command
        - command format: text|count|Name1|Text1|Name2|Text2|...;
- updated) Ophir StarLab v4.00 
- fixed) Support for creating external entities
    - See the editor_entity_custom demo project
     
## v1.8.6 (2026.5.14)

- fixed) Hatch
    - Added the HatchFills option for line hatch
    - Improved: Hatching is now applied correctly when using the Outline cell type for barcode objects
- added) Background checker grid size
    - Supports size configuration via IView.CheckerSize
- fixed) Undo, Redo
    - Undo is now supported when using keyboard shortcuts in EditorControl
    - Improved stability
     
## v1.8.5 (2026.5.8)

- added) Undo and Redo support
    - Support for ActUndo and ActRedo in IDocument
    - Valid only for certain functions named IDocument.Act
    - Can be disabled via Config.IsUnReDoEnable
    - The number of history entries (default: 30) can be changed via Config.UnReDoSize
- added) Barcodes
    - Added Aztec 2D barcodes
    - Added PLESSEY 1D barcodes
    - Support for editing pixel size (Dimension)
- added) CreateGrid form
    - Support for creating dots, circles, crosshairs, and grid patterns
- added) Rtc6
    - Added support for pulse picking for femtosecond lasers
- fixed) IRtcStepper
    - Added support for asynchronous processing in initialization and wait functions
- fixed) DIO Form
    - Fixed an error in displaying analog output values
- fixed) MultiBeam
    - Changed the jump (token exchange) interval during raster machining
    - Previous: Token exchange upon jump (ListRasterLine) at the end of each line
    - Changed: Token exchange upon jump (ListRasterPixel) between each pixel
- fixed) semi ocr font
    - Applied raster machining method
 
## v1.8.1 (2026.4.22)

- added) Remote
    - Added the IRemote interface to support recipe changes, object property queries and modifications, and commands to start, stop, and reset marker processing via external communication, as well as the setting of processing offsets
    - Serial communication supported
    - TCP/IP communication supported
    - WebSocket communication supported
    - MQTT communication supported
- added) Script
    - Support for real-time modification of text data during machining using external C# script files
    - Works with TextConverter.SimpleScript
    - User-written C# scripts can be used in the Script folder
    - ScriptInstance at IMarker 
- added) SEMI OCR fonts
    - Added .dot font files
    - Support for dot fonts using the SiriusText 
- fixed) IDocument
    - Fixed a search error in FindByName
- fixed) MultiBeamControl
    - Fixed an error with button toggle states
     
## v1.7.1 (2026.4.16)

- updated) RTC6 v1.24.0 package
    - Release version: March 31, 2026
- fixed) IMarker
    - Added support for asynchronous processing (open-source changes)
    - Refactored to use tasks instead of threads and implemented via inheritance
- fixed) IRtcMultiBeam
    - Verification of exclusive synchronization control between RTCs completed
    - SiriusEditorControl verification completed
        - Supports 2 different processing data sets + 2 different pen combinations
    - SiriusMultiEditorControl verification completed
        - 1 identical processing data set + 1 different pen combination
- added) LogControl
    - Added log message filtering and search functionality;
- fixed) Shader;
    - Issue where objects were not rendered in the View in the console environment;
    - Support for managing Shaders per target in multiple views;
- fixed) Correction 3D 
    - 16-bit and 20-bit resolution processing for coefficients A, B, and C
    - Improvements to data manipulation using Correction3DRtcForm
- fixed) memory leak
- fixed) Fixed a bug related to saving view images via Snapshot
- fixed) Improved C# script execution speed
 
## v1.6.1 (2026.4.9)

- added) ViewerControl 
    - Added a user control 
    - Support for rendering a single document simultaneously in the viewer and editor 
    - Removed the restriction on a 1:1 connection between a Document and a single View 
    - Support for creating and modifying Documents externally 
- fixed) IRtc3D
    - Support for enhanced 3D calibration procedures based on RtcCalibrationLibrary
        - 1. Beam tilt calibration: RtcCalibrationLibrary.BeamTiltCalibration
        - 2. 2D field correction: RtcCalibrationLibrary.XyCalibration
        - 3. Focus calibration at z=0: RtcCalibrationLibrary.FocusCalibrationAtZ0
        - 4. Focus calibration for coefficients A, B, C: RtcCalibrationLibrary.FocusCalibrationCoeffABC
        - 5. Stretch calibration for Z volume: RtcCalibrationLibrary.StretchCalibration
    - RtcCorrection3D removed: Replaced by RtcCalibrationLibrary;
    - KZScale removed: Replaced by Focus compensation in RtcCalibrationLibrary;
    - ZOffset removed: Replaced by Translate Z in MatrixStack;
- added) EntityPoint object added;
- added) EntityBarcode1D_V2 object added
    - Supports various cell types, similar to 2D barcodes:
    - Combinations of dots, lines, hatches, etc. are possible:
- added) Support for opening and saving vertex lists for the following objects:
    - EntityPoints:
    - EntityPolyline2D:
    - EntityPolyline3D:
    - OffsetControl user control
     
## v1.5.4 (2026.4.2)

- Fixed) Hotfix
    - exception occurring when creating a SiriusEditorControl user control at design time
    - exception occurring when creating a SiriusMultiEditorControl user control at design time

## v1.5.3 (2026.3.31)

- fixed) IDocument
    - added IView connection settings
- added) IPowerMeter
    - added MeasureUnits for power and energy measurement modes
- fixed) Rtc6
    - event notification when the IsActivateAutoDelays property is changed
    - items in EntityPen and EntityLayerPen are set to visible when the IsActivateAutoDelays property is changed 
- fixed) EntityBarcode2D
    - changed marking order via EntityPen when processing CellDot
- fixed) IRtcMultiBeam
    - verification of exclusive token handling completed
- fixed) IRtcCorrection2D, IRtcCorrection3D
    - support added for processing raw data using the internal matrix (rotation, etc.) set in the scan head
     
## v1.5.2 (2026.3.27)

- added) Stepper motor control support
    - added controlling external stepper motors via the stepper port on the RTC5, RTC6
    - added the IRtcStepper interface
    - added the StepperControl user control UI
    - support for absolute and relative movement of stepper motors
- added) Serial communication support
    - added serial communication via the RS232 port on RTC5, RTC6
    - added the IRtcSerialComm interface
    - added the SerialCommControl user control UI
    - ability to monitor transmitted and received data (binary) in the Laser tab
    - added OnSerialReceived event
- Added) Fly Extension improvements
    - improvements to the Marking on the fly extension for RTC6
    - refactored the IRtcMoFExtension interface
    - support for 3-axis combinations (X, Y, Z or rotational axes)
    - added support for McBSP communication
- Fixed) Refactored user control UI
    - OffsetControl
    - MarkerControl
    - ScannerControl
    - LaserControl
- Fixed) Matrix stack
    - removed BaseMatrix from MatrixStack 
    - integrated support via IRtc.CtlMatrix and ListMatrix
- fixed) PowerMeter
    - fixed an error reading power values from CoherentPowerMax and GentecEO devices
- fixed) SiriusEditorControl
    - import external .sirius3 files and add(or merge) them as layers to the current document
    - layer pen color use on the layer node is displayed in the tree view
- added) paste array at editorcontrol

## v1.4.1 (2026.3.10)

- added) documentation provided via web server
    - online website: https://spirallab.co.kr/sirius3/doc available
    - alternatively, extract the sirius3\doc\sirius3_doc_{version}.zip archive and run the 'start_doc.bat' batch file
- added) Value editing using the mouse
    - Supports increasing or decreasing values by dragging left or right while holding down the right mouse button in the PropertyGrid
- fixed) Rtc6
    - When using ListLaserOn, the SYNC OUT from an external laser source is input to count pulses, but the wait time is processed as 10 times longer.
- fixed) EntityPen
    - Issue where Power, PowerPercentage, and PowerMapCategory values were not visible
- fixed) SiriusEditorControl
    - All 4 pages are usable by default
    - WaferMap and Substratemap are deactivated
    
## v1.4.0 (2026.3.3)

- added) .NET 9.0-Windows, .NET 10.0-Windows development environments
- added) Pulse count output via synchronisation signal from external laser source
    - External synchronisation signal input via DIGITAL IN1 on LASER connector
    - IRtc.ListLaserOn(wait time, pulse count, pulse count exit) 
    - Can be set via the PixelPulses and IsPixelPulsesExit values of the EntityPen pen
        - 0: Output LASERON for the pixel duration as before
        - 1~65535: Output LASERON for the pixel duration while waiting for the external synchronisation signal for the specified number of pulses
        - When using IsPixelPulsesExit, immediately terminates when the external synchronisation signal count reaches the PixelPulses setting value and proceeds to the next list command
- added) (experimental) IRtcMultiBeam interface
    - Multi-beam system utilising one laser source + two RTCs + two AOM RF drivers
    - Rtc6MultiBeam
- added) EntityPoints
    - Supports shortest path optimization 
- added) IRtcIO interface
- fixed) EntityWaitDataExt16Cond, EntityWaitDataExt16EdgeCond, EntityWriteDataExt16, EntityWriteDataExt16Cond
    - Bitmask as ushort type instead of string
- fixed) SiriusEditorControl control
    - Exception occurring when adding at design time
    - Fixed exception occurring when control is created in control behind, due to OpenGL not initialised
    - Removed code forcing Document to ActNew at control load time
- fixed) Licence
    - When exceeding maximum allowed instances or lacking options
    - Prev: Unusable
    - Changed: Activates 30-minute evaluation mode
     
## v1.3.2 (2026.2.20)

- fixed) Support for Extended Mode in Automatic Laser Control
    - Actual Velocity + Encoder + SCANAhead + Inverse Speed Correction + Backward Transformation + SDC + SkyWriting signal combinations now usable
    - Support added for setting Extended Mode combinations in the PoD list within the EntityLayerPen properties
    - Added EntityPoD
- fixed) EntityPen
    - Support added for setting Spot distance values for SDC functionality in SpotDistanceSCANa.
- added) IRtcMoF 
    - Support added for encoder signal error notification events: IRtcMoF.OnEncoderSignalError event
    - Supports notification event when exiting virtual image field: IRtcMoF.OnOutOfVirtualImageField
    - Supports encoder signal filter settings (RTC6 exclusive)
        - Use CtlMoFEncoderFilter function to apply arithmetic mean to signals in noisy conditions or support high speeds above 4MHz
    - When querying encoder values, both absolute and relative positions can be retrieved separately.
    - modified argument to OnEncoderChanged event
- fixed) renamed IRtcWaitID to IRtcInterrupt
 
## v1.3.1 (2026.2.9)

- added) IRtcSCANAhead interface
    - Added SCANAhead-specific items (Corner, End, Acc Scale) to EntityPen
    - Supports setting Position(or Trajectory) Acknowledge Limit value (default: 0.28% of total position range)
    - When using RTC6 + SCANAhead, works as Trajectory ACK Limit value
- added) IRtcWaitID interface
 
## v1.3.0 (2026.2.5)

- added) EntityPolyline2D and EntityPolyline3D
    - vertex list editor 
- added) SiriusMultiEditorControl control
    - support for processing a single document across multiple devices 
- added) EntityLayerPen
    - Added UI to edit pen values and provide assistance
- replace) removed gnuplot and replaced with built-in plot functionality
- fixed) invalid scanner jog output form 
- added) support for ODA converter programme
    - improved to enable the additional use of the ODA converter when processing .dwg and .dxf files
    - ODA converter requires separate installation by the user (https://www.opendesign.com/guestfiles/oda_file_converter) 
- licence) Licence policy changes
    - 3D option removed and changed to basic
    - syncAXIS instance changed to an option feature

## v1.2.7 (2026.1.26)

- added) Added Variable Delays to EntityLayerPen
    - Variable polygon delay: Set variable polygon delay time based on the angle of the bend (Default: Enabled)
    - Variable jump delay: Set variable jump delay time based on the jump distance
- fixed) RTC7
    - invalid LaserOnShift value for Skywriting 
- fixed) Config.IsMarkArcsIntoLines
    - True: Arcs (EntityArc) and polylines (EntityPolyline2D) are processed by decomposing into lines (ListMarkTo)
    - False: Arcs (EntityArc) and polylines (EntityPolyline2D) are processed by decomposing into lines  (ListArcTo)
- fixed) Contour 
   - IsClosed value was incorrectly calculated during contour extraction
- fixed) Editable Config.EntityPenColors and Config.LayerPenColors
- fixed) ActRemove failure for simulated entities
 
## v1.2.6 (2026.1.20)

- added) Ellipse entity
- added) EntityLine, EntityArc, EntityPolyline2D
	- Added RampFactor property for Automatic laser control(defined vector) support
- added) IHatch.HatchRepeats for hatch repeats
- fixed) Invalid EntityPen, EntityLayerPen values are shown
- fixed) PowerMap CtlCompensate routine modification
	- Prev: Re-measurement method for left/right ranges 
	- Changed: Immediately updates measured data 
- fixed) IMarker.Preview
   - Prev: Displayed a single bounding rectangle around selected objects
   - Changed: Displays all individual bounding rectangles for selected objects
	 
## v1.2.5 (2026.1.15)

- added) ClipHelper to intersect 
- added) activate sub-entity hit mode if spacebar has pressed
- fixed) improve rayhit test for IHitTestable 
   - Config.RayHitTestPixelSize: hittest with dynamic threshold distance
- fixed) IMarker 
   - do recursive marks for child entity if MarkTargets.Selected 
- updated) zxing v0.16.11
- updated) clipper2 v.2.0.0

## v1.2.4 (2026.1.7)

- added) shortcuts
   - CTRL + R: toggle allow to render
   - CTRL + M: toggle allow to mark
   - change node font (or color) when toggle allow to render or mark 
- added) IRtcFreeVariable.OnFreeVariableChanged event
   - raised when FreeVariable value has changed
- added) Config.GridCloudInterval
   - used when IDocument.ActGridCloud has called
- fixed) speed up for parse gerber file
- fixed) hittest with more detail information
   - IDocument.SubHitEntities
- added) another ActHitTest function 
- fixed) invalid exception when do ActUngroup by empty node

## v1.0.1 (2025.12.22)

- added) .chm documentation files
- added) ActExpand 
   - expand(or shrink) contours by distance
- added) Gentec-EO powermeter device support
- updated) PowerMeterOphir by StarLab v3.93
- fixed) enum for hatch joints 
- fixed) IDocument.FindByLayerUsedPenColors
- fixed) more log message for Marker.EntityWork 
 
## v0.9.3 (2025.12.5)

- added) zoom to fit 
   - mouse double click at treeview
   - after file has opened
- added) new TextConverters.Offset 
   - used with Offset.ExtensionData 
- fixed) gerber file
   - added) UI.Config.IsGerberWithUniformGroup option for higher render speed
   - fixed) UI.Config.IsGerberTessellation option for invalid tessellation 
- renamed) scanner pen to entity pen

## v0.9.2 (2025.11.25)

- added) convert to block and block insert at menu
- renamed) EntityGroup to EntityMixedGroup
- fixed) ActUngroup bug
- fixed) improve performance for ActMixedGroup, ActUniformGroup 
- fixed) improve loading time for import gerber file
- fixed) stackoverflow exception when save file

## v0.9.1 (2025.11.18)

- added) include 'gnuplot' program at Spirallab.Sirius3.Dependencies package
- added) create uniform group button at editor
- fixed) invalid render issue at EntityUniformGroup 
- fixed) memory leaks
- fixed) invalid spline vertices
- fixed) out of memory if too many node items has created
- changed) Core.Initialize signatures
	 
## v0.8.2 (2025.11.11)

- fixed) fail to parse HPGL format
- fixed) scanner pen is not applied
- fixed) refresh scanner/layer pen object when do ActNew
	 
## v0.8.0 (2025.11.7)

- Developer preview version
  
## v0.1 (2025.03.06)

- Initial release 
