# Sirius3 IScript Developer Manual (C# Scripting Integration)

> Reference version: Sirius3 1.12.3 (public Release features)

This guide explains the `IScript` interface at the core of Sirius3 dynamic text and shows developers how to extend it with application-specific behavior.

---

## 1. Introduction

`IScript` defines the "Global Running Environment" for processing text data that must be changed in real-time during laser marking.When you set `TextConverter` to `SimpleScript`, all C# script expressions run within the `IScript` environment.

- Key roles: serial number management, date/time formating, external database collaboration, production history logging.
- **Basic Implementation**: The `SpiralLab.Sirius3.Scripting.Script` class is implementing all the built-in features, and developers can inherit them and extend their features unlimitedly.

---

## 2. Key Properties and Functions Integrated API (Core API)

`IScript` provides all the built-in properties and functions required for laser processing and context tracking. When creating a script, you can call all the items below immediately.

### 2.1 Marking Lifecycle Event
| Method | Explanation |
|:---|:---|
| `OnStarted(IMarker)` | It is called when the marking process begins. you can override in the custom script to put the initialization logic. |
| `OnEnded(IMarker, bool, TimeSpan?)` | It is called when the marking process is completed. it is useful for the success and the time it takes to be transferred and to leave the logs. |

### 2.2 Identifiers and Process Context (Properties)
| The Property (Property) | Type | Explanation | Examples (input and results) |
|:---|:---|:---|:---|
| `Marker` | `IMarker` | See the current Marker instances. | `Marker.Name` -> `"Rtc5_0"` |
| `LotCode` | `string` | The LOT code of the product. | `$"Lot: {LotCode}"` -> `"Lot: LOT-A123"` |
| `LineName` | `string` | Name of production line. | `LineName` -> `"LINE_01"` |
| `Message` | `string` | Communication of the system. | `Message` -> `"Ready"` |
| `DeviceID` | `string` | The device identifier. | `DeviceID` -> `"DEV_842"` |
| `OperatorID` | `string` | the employee identification. | `OperatorID` -> `"OP_KIM"` |
| `PartNo` | `string` | Part number of product. | `PartNo` -> `"PN-9988"` |
| `CustomerID` | `string` | Customer identification. | `CustomerID` -> `"SAMSUNG"` |
| `EquipmentID` | `string` | Identifiers of equipment. | `EquipmentID` -> `"EQ-01"` |

### 2.3 Serial Number Control
| Method | Type of Return | Explanation | Examples (input and results) |
|:---|:---|:---|:---|
| `NextSerialNo(inc)` | `int` | Currently reboot and reboot. | `NextSerialNo()` -> `1` (internal value changed to 2) |
| `NextSerialNo(fmt, inc)` | `string` | Currently formate, return and increase the series. | `NextSerialNo("D4", 2)` -> `"0001"` (internal value changed to 3) |
| `NextNamedSerialNo(name, inc, start)` | `int` | Returns / Increase the named counter. | `NextNamedSerialNo("BOX", 1, 100)` -> `100` |
| `NextNamedSerialNo(name, fmt, inc, start)`| `string` | Formate the named counter and return / increase it. | `NextNamedSerialNo("BOX", "D2")` -> `"01"` |
| `ResetSerialNo(value)` | `void` | Reset the basic serial number. | `ResetSerialNo(1)` (No return value, change to property value 1) |
| `ResetNamedSerialNo(name, value)` | `void` | Repair the named counter. | `ResetNamedSerialNo("BOX", 1)` (No return value) |

### 2.4 Date, Time, and Shift
| Method / Method | Type of Return | Explanation | Examples (input and results) |
|:---|:---|:---|:---|
| `Date(format)` | `string` | Current date (Basic `yyyy-MM-dd`) | `Date("yyMMdd")` -> `"260419"` |
| `Time(format)` | `string` | Current time (Basic `HH:mm:ss`) | `Time("HH:mm")` -> `"15:30"` |
| `AmPm(am, pm)` | `string` | Returns morning (`am`) or afternoon (`pm`) | `AmPm("AM", "PM")` -> `"PM"` |
| `DayNight(day, night)`| `string` | 8 to 20 hours per week (`day`), other nights (`night`) | `DayNight("D", "N")` -> `"D"` |
| `Shift(s1, s2, s3)` | `string` | Returns one of three shifts at 8-hour boundaries (06:00, 14:00, 22:00) | `Shift("A", "B", "C")` -> `"B"` |
| `Shift2(s1, s2)` | `string` | Returns one of two shifts at 12-hour boundaries (08:00, 20:00) | `Shift2("Day", "Night")` -> `"Day"` |
| `WeekOfYear()` | `int` | Return to the current ISO parking. | `WeekOfYear()` -> `16` |
| `DayOfYear()` | `int` | Current return. | `DayOfYear()` -> `109` |
| `Year`, `Month` | `int` | Current Year, Month (1-12) | `Month` -> `4` |
| `Day`, `Hour` | `int` | Current day (1-31), hour (0-23) (continuous) | `Hour` -> `15` |

### 2.5 Utilities and data storage
| Method | Explanation | Examples (input and results) |
|:---|:---|:---|
| `Set(key, value)` | Save the custom data that needs to beined. | `Set("Count", 10)` (No return value) |
| `Get(key)` | Get data stored in `Set`. | `(int)Get("Count")` -> `10` |
| `Pad(input, len, char)`| Fill `char` in the front of `input` and match the total length of `len`. | `Pad("7", 3, '0')` -> `"007"` |

---

## 3. Custom Implementation

Developers can inherit the `Script` class to create their own business logic.

### 3.1 Basic Structure
```csharp
using SpiralLab.Sirius3.Scripting;

public class MyProductionScript : Script
{
    // Properties Definition (Automatically exposed to the Sirius3 UI Properties Window)
    public string FactoryName { get; set; } = "Seoul_01";

    // Custom Logic Definition (Callable from SourceText)
    public string CalculateMagicCode() {
        return FactoryName + "-" + DateTime.Now.Ticks.ToString().Substring(10);
    }
}
```

---

## 4. Public Demo

`demos/editor_script` of the public storage is a standard example.

- `Form1.cs`: Create `EntitySiriusText` and set `TextConverter = TextConverters.SimpleScript`, `SourceText = "NextSerialNo(1)"`, `IsAllowConvert = true`.
- `Marker.ScriptInstance`: It is a `IScript` instance currently available by the Marker.
- `ScriptFactory.Create(fileName)`: Open `.cs` or built `.dll` Create Script.
- `ScriptSerializer.Open/Save`: Open and save the `.script` settings file.

Do not repeat tasks that may take long or fail, such as CSV, Database, and Network Access, directly on a SourceText line. prepare the necessary data before the Marker starts and implement the Script Method to work short and decisively.

---

## 5. Integration with UI (UI Integration)

How to use customized `.cs` files or built `.dll` files are as follows:

1. **Scripts registration**: assign `IScript` instances to `Marker.ScriptInstance`.
   - The default implementation uses a separate ScriptInstance for each Marker.
   - Do not assign the same instances to multiple markers at the same time.
2. **Use of the Properties**: The `public` properties defined in the inherited class will automatically appear in the Properties Grid of the Sirius3 UI, and it is possible to change in real time during the marking.
3. **Real-time reflection**: `NotifyPropertyChanged` is called when the serial number increases or the data changes, so the UI is updated immediately.

---

## 6. Developer Tips (Tips)

- *Straed Safety**: Marking is high-speed, so when writing files or network communication, you must consider non-motion processing or lock.
- **Compilation time**: User Script should be loaded/compiled in advance when the Marker is not Busy and check the error.
- **Lifetime Cycle**: `OnStarted`, Text Conversion, `OnEnded` uses the ScriptInstance of the same Marker.
- **Smart Assessment**: With `{ }` in `SourceText`, it is internally converted to `$""`, making it possible to create very intuitive codes.

---

## 7. Advanced Multi-line Scripts (Advanced Multi-line Script)

When dealing with complex conditions in the field, there are five practical scripts that can be written directly in the `SourceText` entrance box (Multi-line Editor) in the properties window and applied immediately.

### 7.1 Complex 2D barcode data combination (multi-field)
It is used when combining multiple process information to compress it into a single string in the DataMatrix or QR code, which is a multi-frame containing semicolon, so it is automatically recognized and run.
```csharp
string prf = LotCode.Substring(0, Math.Min(LotCode.Length, 3)); // Three front seats.
string dt = Date("yyMMdd");
string tm = Time("HHmm");
string sn = NextSerialNo("D5");
string sh = Shift("A", "B", "C");

return $"{prf}-{dt}-{tm}-{sn}-{sh}";
```

### 7.2 Automatic Daily Serial Reset at Midnight
Each time the date changes, you automatically control the serial number to start back from 1 using `Set` and `Get` to remember the last marking date.
```csharp
string today = Day.ToString();
string lastDay = Get("LastDay") as string;

// If the date is changed, the counter re-set.
if (today != lastDay) {
    ResetSerialNo(1);
    Set("LastDay", today);
}

return $"SN-{NextSerialNo("D4")}";
```

### 7.3 Multi-independent counter (box and individual product combination)
When the product counter reaches a specific figure, the product counter is a double counting logic that resets and increases the box counter.
```csharp
int itemMax = 10; // 10 items in a box.
int item = NextSerialNo(); 

// If you fill 10 products.
if (item > itemMax) {
    ResetSerialNo(1);
    item = NextSerialNo();
    NextNamedSerialNo("BoxCounter"); // Box Counter 1
}

// Import the "BoxCounter" count value (not increased)
int box = NextNamedSerialNo("BoxCounter", 0);

return $"BOX:{box:D3}-ITEM:{item:D2}";
```

### 7.4 Conditional Text by Day/Night Shift and Day of Week
Apply differently the identification characters that are marked according to the time zone and day not a simple date.
```csharp
string dn = DayNight("DAY", "NIGHT");
string dw = DateTime.Now.DayOfWeek.ToString().Substring(0, 3).ToUpper(); // MON, TUE...

// If you are working at night and weekend, add a special mark.
if (dn == "NIGHT" && (dw == "SAT" || dw == "SUN")) {
    return $"[W-NGT] {LotCode}-{NextSerialNo("D4")}";
}

return $"[{dw}-{dn}] {LotCode}-{NextSerialNo("D4")}";
```

### 7.5 Special alarm returns when the target volume is reached
When you have reached the set target production volume, the employee is displayed a visual alarm (or text that causes a stop of marking).
```csharp
int target = 500; // The target number.
int current = (int)(Get("ProduceCount") ?? 0);
current++;
Set("ProduceCount", current);

if (current >= target) {
    // Objective Objective Specific Screenshots
    return "★★★ TARGET REACHED ★★★";
}

// Screenshots together and text.
return $"Lot:{LotCode} [{current}/{target}]";
```

---
2026 Copyright (c) SpiralLAB. All rights reserved.
