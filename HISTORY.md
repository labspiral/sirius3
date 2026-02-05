# Sirius3 version history

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
