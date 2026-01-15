# Sirius3 version history

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
