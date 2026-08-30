# PropertyGrid Control & Entity Inspector User Manual

> Reference version: Sirius3 1.12.3 (public Release features)

## 1. Role of PropertyGrid

PropertyGrid displays and edits the selected Page, Layer, Entity, EntityPen, EntityLayerPen, or device settings from the editor or TreeView. A committed value change updates the Document, regenerates affected geometry when required, and refreshes the editor.

## 2. Selection and Display

- Choose one object: Show all public editing properties of that object
- Select multiple objects: Show only common properties and select one change to apply to the whole object
- Layer/Pen Choice: Show Layer or Processing Conditions instead of Object Shape
- Device Change: Properties that are not supported by the connected RTC·Laser are displayed as hidden or read only

When describing an object or comparing values, first select the object in TreeView or Editor to make sure PropertyGrid is filled.

## 3. Property Search

From Sirius3 1.11.14 you can search for properties **Name, Category, Description**.

- `CTRL+F`: Go to the search window
- Search word enter: Show only matching properties
- Clear button: Delete the search conditions at once

The unseen properties during search may not be deleted but excluded from the filter. check the device support and search word together.

## 4. Main Categories

- Basic: name, ID, processing/rendering permitted
- Transform: position, rotation, distribution, ModelMatrix
- Geometry/Text/Hatch: size, height, text, hatch
- Laser: Power, PowerMax, PowerMapCategory, Frequency, PulseWidth
- Scanner: Mark/Jump Speed, Laser/Scanner Delay, Hard Jump
- Raster: RasterMode, PixelTime, PixelPeriod, PixelPulses
- Wobbel: Shape, Frequency, Parallel/Perpendicular Extension
- Layer Advanced: Sky Writing, Variable Delay, ALC, SCANAhead
- syncAXIS: MotionType, Bandwidth, etc.

## 5. Changes in value and scope.

Number Editor can adjust values outside the permissible range to the minimum/maximum values of the property. Do not assume that the input values remain the same, but check the display values and logs after editing is completed.

When you activate `UI.Config.IsConvertToControllerResolution`, the time and frequency values of EntityPen/EntityLayerPen may be displayed according to the application resolution of the RTC connected, which is separate from the coordinate KFactor conversion.

## 6. Localized Descriptions

Core and UI Config, PropertyGrid names, classifications and descriptions use the language resource you have chosen. From Sirius3 1.11.14, the relevant settings, notes and application orders are displayed again in several lines. After changing to another language, open PropertyGrid again or update your choice to check the display.

## 7. Restrictions During Marking

If the Marker is Busy, it will limit the editing of the shape and process conditions. extension, reduction and screen movements are possible but the selection and data changes are locked. F5 can lead to the real Marker Start, so use it after checking the work area and the laser safety state.

---
2026 Copyright (c) SpiralLAB. All rights reserved.
