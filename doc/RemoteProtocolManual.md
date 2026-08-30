# Remote Control Protocol Manual

> Reference version: Sirius3 1.12.3 (public Release features)


This document is a specification of the communication protocol for external devices or programs to control the system through the remote control function in the Sirius3 library.

## 1. Basic communication.

- Protocol of support:
  - TCP/IP Server (Default: 50001)
  - RS-232 Serial
  - WebSocket Server (Web Environment Standard Support)
  - MQTT Client (IoT and Remote Distributed Control)
  
- Separator: | (pipe, default)
- Terminator (Terminator): ; (SemiColon) (Default)
- The basic response:
  - The success: OK
  - The failure: NG
  
- Caution: Every command must end with the terminator (`;`). Separate the command and parameters with the separator (`|`).
- When using WebSocket, you can run doc\Remote_WebSocket_Demo.html in your web browser to test communication.


## 2. Marking and control mode.

- Control Mode (Control Mode):
  - Local: PC user controls directly. In the remote control command, the writing/controll behavior related command returns NG.
  - Remote: allows remote client control. Direct manipulation on the PC is predominantly limited.


## 3. Push Notification

When the status of the markers is changed, it immediately sends a message from the server to the client (following is not required).

- When marking starts: Status|Started;
- Marking ended: Status|Ended|{Result}|{Elapsed};
  - OK or NG
  - {Elapsed}: takes time (second units, example: 1.234)


## 4. Main Command List


## 4.1 Mark control (Marker control)

- Start marking: marker|start;
  - Answer: OK; (which means that the work has been accepted, processing ends confirmed by automatic notification)
- Stop marking: marker|stop;
  - The answer is OK;
- Reset errors: marker|reset;
  - Reply: OK; or NG;

## 4.2 Control mode viewing (ControlMode)

- Current control mode view: controlmode;
  - Response: OK; followed by ControlMode|{Mode};
  - {Mode}: Local (local control) or Remote (distance control)
  
## 4.3 Status view (Status)

- Current status: status;
  - Response: Status|{State};
  - {State}: Error (Error), Busy (in action), Ready (ready), NotReady (ready)

## 4.4 Recipe Control (Recipe)

- Open recipe: recipe|{FileName or Number};
  The recipe.|C:\Recipes\ProductA.sirius3; or recipe|10;
  - Reply: OK; or NG;
  - When using the number (Number), try to find and open the 'laser.sirius3' file in the 'recipe\number' directory.
- Check the current recipe: recipe;
  - Response: OK; followed by Recipe|{FileName};

## 4.5 Offset control (offset)

Set offset arrangements (coordinates and rotating) set on the markers.

- **Batch Set Set Set: `offset|{Count}|{X1}|{Y1}|{Z1}|{Angle1}|...|{Xn}|{Yn}|{Zn}|{Angle n};`
  - `{Count}`: The total number of offset set
  - `{X}, {Y}, {Z}, {Angle}`: Translation and rotation (Rotate Z) information.This set is repeated as much as `{Count}`.
  - Reply: `OK;` or `NG;`
- ** Offset viewing**: `offset;`
  - Answer: `OK;` after `Offset|{Count}|{X1}|{Y1}|{Z1}|{Angle1}|...;`
  - Returns the coordinate information of all offset set currently.
- **Attention**: When you run the `offset` command, the outset layout of the markers will be re-started, so the `text` command will reset all the extension data (ExtensionData) that you have previously set up.

## 4.6 Dynamic Text Data Control (Text)

Set or view individual extension data (`ExtensionData`) assigned to each offset index.
- **Attention**: If the `TextConverter` property of an entity such as text (or barcode) is set to `Offset`, this extension will automatically find and exchange the value corresponding to the entity name.
- * * * * * * * * * * * * * * * * * * * * `offset` The command must be advanced** and offset information must be executed after defined. also offset number ( `Count` and must be the same.
- **Settings of text data setup**: `text|{Count}|{Text1}|...|{Text n};`
  - `{Count}`: Number of offset set (it must match `{Count}` in the previously set `offset` command)
  - `{Text}`: Simple string or Key-Value pair (`Name|Value`).
    - **Caution**: Only the Key|Value separator format is supported.
  - Answer: `OK;` or `NG;` (`NG;` returns if the outset numbers disagree)
- ** Text data viewing**: `text;`
  - Answer: `OK;` after `Text|{Count}|{Text1}|...;`
  - Returns the extension data of all offset set currently.

*Supply for use *:

1. **Offset and text data individual settings**:
   - `offset|1|10|20|0|0;`
   - `text|1|EntityName|Hello;`
2. ** 2 offset and each text data offset locations individually**:
   - `offset|2|0|0|0|0|10|10|0|0;`
   - `text|2|EntityName1|Text1|EntityName2|Text2;`
3. * Text data viewing *:
   - `text;`
   - (Response: `Text|1|EntityName|Hello;`)
   - (Response: `Text|2|EntityName1|Text1|EntityName2|Text2;`)

## 4.7 Select and Deselect Entities

Select the name: select|{Count}|{Name1}|{Name2},...;
  and select.|1|Circle_1;
  - Reply: OK; or NG;
- Selected by: deselect
  - The answer is OK;
- Selected objects: select
  - Reply: OK; then Select|{Count}|{Name1}|...;

## 4.8 Layer, Entity, EntityPen, and LayerPen Property Control

You can read or write the values of a particular property.The property name can distinguish the English vocabulary.

- Properties read: {target}|{name}|{propertyName};
  The entity|Circle_1|Radius;
  - Reply: OK; after Entity|Circle_1|Radius|{Value};
The title: {target}|{name}|{propertyName}|{Value};
  The entity|Circle_1|Radius|20.0;
  by entitypen|White|Power|50.0;
  - Reply: OK; or NG;
- Support Properties List View: {target}|{name}|properties;
  by entitypen|White|properties;
  - Response: After OK;, `{propertyName}|{currentValue};` entries are sent continuously, separated by newlines.

## 4.9 Scanner field correction (Field correction)

- 2D correction data application: fieldcorrection|{Rows}|{Cols}|{Interval}|{ErrX1}|{ErrY1}|...;
  - Reply: OK; or NG;
  - The scanner correction window is displayed and it will not respond until the window is closed.


## 5. Common Responses

Command does not distinguish the speaker but the name of the object, the values act by distinguishing the speaker.
Command for immediate success, failure or failure is OK; or NG; responding,
Reactions containing data, such as state searches or offset searches, follow the dedicated format according to the command.
The unmotionary results of the marking task are delivered through the automatic notification of item 3, so we do not recommend a separate polling loop.


## 6. Developer Extension Guide

Sirius3 library users use the `RegisterCommandHandler` method in IRemote instances
You can extend your default protocol or redirect the behavior of the existing bitin command.

6.1 Command Handler Registration and Priority
- Registration method: `remote.RegisterCommandHandler("command_keyword", handler_delegate);`
- The Priority:
  1. Registered Handlers – the first to check, and if there is, it will be ignored and run by the Biltin logic.
  2. Built-in Commands (Built-in Commands) - Run only if there is no registered handler.
- Keyword matching: don't distinguish the speaker.

6.2 Code Examples (C#)

```csharp
// Example 1: completely new custom command registration ("MyCmd,Param1,Param2;")
remote.RegisterCommandHandler("MyCmd", async (r, tokens) =>
{
    // tokens[0]: Command Keywords (“MyCmd”)
    if (tokens.Length < 3) 
    {
        await r.Send($"{UI.Config.RemoteNG}{UI.Config.RemoteTerminator}");
        return true; 
    }
    string p1 = tokens[1];
    string p2 = tokens[2];
    
    // The user’s logic...
    Console.WriteLine($"Custom command received: {p1}, {p2}");
    
    await r.Send($"{UI.Config.RemoteOk}{UI.Config.RemoteTerminator}");
    return true; // The true return order is considered to be processed.
});

// Example 2: Biltin Command Overlay (e.g. Pass the 'recipe' Command)
remote.RegisterCommandHandler("recipe", async (r, tokens) =>
{
    if (tokens.Length < 2) return false;
    
    bool success = true;

    // user logic.
    var doc = Marker.Document;
    // Use the receiptName to process the receipt file open.
    string recipeName = tokens[1];
    string recipePath  = Path.Combine(Config.RecipePath, recipeName);

    success &= doc.ActOpen(recipePath);
    
    if (success)
    {
      // Processing Successful
      await r.Send($"{UI.Config.RemoteOk}{UI.Config.RemoteTerminator}");
    }
    else
    {
      // Processing Failure
      await r.Send($"{UI.Config.RemoteNG}{UI.Config.RemoteTerminator}");
    }

     return true; // Returns true so that the Biltin logic does not run
});
```

6.3 Main Bilt In Command Communication Test Examples (Raw Protocol)
- Change recipe: recipe|laser.sirius3;
- Layer View / Hidden: Layer|Default|IsVisible|False;
Change of location: entity|Circle_1|Center|10.0|20.0;
All posts tagged: entity|Circle_1|properties;
- Pen Power Change: entitypen|White|Power|50.0;
1 Offset Setup: Offset|1|10|0|0|0; (Xc 10mm move)
- 1 offset and text data settings: offset|1|0|0|0|0|Text_1|Value_1;
- Only change the text of a specific offset: offset|0|Text_1|NewValue;
Selection of Selection: Selection|2|Circle_1|Rect_1; (a selection of two objects)
- Selected by: deselect


---
2026 Copyright (c) SpiralLAB. All rights reserved.
