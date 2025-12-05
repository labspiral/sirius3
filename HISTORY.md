# Sirius3 version history

## v0.9.3 (2025.11.5)
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
