# Sirius3
面向 Windows/.NET 的精密激光加工平台，集成 SCANLAB 控制、设备连接、几何处理、OpenGL 可视化、文档编辑、通过内置 MCP 服务器进行 AI 提示、模拟和加工执行

语言：[English](README.md) · [한국어](README.koKR.md) · [简体中文](README.zhCN.md) · [日本語](README.jaJP.md) · [Deutsch](README.deDE.md)

![sirius3_logo](https://spirallab.co.kr/sirius3/sirius3_logo.png)

---

## 亮点
![sirius3_logo1](https://spirallab.co.kr/sirius3/sirius3_logo1.png)
![sirius3_editor](https://spirallab.co.kr/sirius3/sirius3_editor.png)

- SCANLAB RTC 控制器
   - RTC4 / RTC4e / RTC5 / RTC6 / RTC6e
   - XL-SCAN (基于 syncAXIS 的 RTC6 + ACS 组合)
- 测量与分析
   - 支持利用振镜运动路径和信号输出日志进行图形输出
   - 支持实时模拟加工路径
- 强大的加工选项
   - 支持设置可变多边形、可变跳跃延迟时间
   - 支持双头、3D
   - 支持 MoF (Marking on the Fly) 和扩展 MoF (Fly extension)
   - Sky Writing Mode 1/2/3/4
   - 支持利用 SCANAhead 的自动延迟值 (Auto delays)
   - 支持多束光 (1 个激光源 + 2 个 AOM + 2 个振镜扫描头) 控制
- ALC (Automatic Laser Control)
   - 矢量定义型
      - 渐变 (Ramp)
   - 速度依赖型
      - 指令速度
      - 实际速度
   - 编码器依赖型
      - 编码器速度
   - 位置依赖型
      - 基于距离及缩放值的表格
   - 此外还可使用 SCANAhead, Encoder Speed Addition, Inverse Speed Correction, Backward Transformation, SDC+Skywriting 的组合
- 振镜场校正
   - 2D 校正
   - 3D 校正 (支持焦点及 Z 空间拉伸补偿)
- 激光功率控制
   - 频率、脉冲宽度、模拟、数字输出
   - 激光源厂商支持：AdvancedOptoWave, Coherent, IPG, JPT, Photonics Industry, Spectra Physics 等
- 功率计和功率映射 (PowerMap)
   - Coherent (PowerMax), Thorlabs (基于 OPM), Ophir (基于 StarLab)
   - 支持基于功率映射的输出补偿
- 渲染与几何处理
   - 基于 OpenGL 3.3+ 的 2D/3D 渲染器，提供一个正交相机和五个透视相机
   - 用于点、线、线带和三角形命中测试的 AABB 加速结构
   - 带闭合/开放轮廓诊断的拓扑感知三维网格切片
   - 基于绕组规则处理轮廓、嵌套区域和相邻条码单元的多重填充
- 实体、文本与条码
   - 点、线、弧、多段线、三角形、矩形、螺旋、Trépannage 和样条曲线
   - 立方体、球体、圆柱体、圆锥体、网格、图层、组、块和块插入
   - Text、SiriusText、ImageText、Circular Text、链接文本及 ZPL 渲染实体
   - 支持轮廓、填充和点阵单元加工的一维、QR、DataMatrix、PDF417 及 Aztec 条码
- 文件导入与互操作
   - Sirius3 文档、DXF/DWG、SVG、HPGL/PLT、Gerber/Excellon 及 G-code/NGC
   - SVG（`.svg`）矢量导入：`IDocument.ActImport` / `EntityFactory.CreateSVG`。保留可编辑的原始曲线，并以毫米生成填充、描边宽度、端点及连接区域，包括圆头和圆角。
   - SVG 填充线：通过 `UI.Config.IsSvgImportHatchEnabled` 启用（默认 `false`），通过 `UI.Config.SvgImportHatchInterval` 调整间距（默认 0.1 mm）。支持直线/多边形填充线；点击和拖动选择基于显示的轮廓和填充线。
   - 光栅图像以及 STL、OBJ、PLY、STP/STEP、3MF 三维模型
   - 3MF（`.3mf`）网格导入：`IDocument.ActImport` / `EntityFactory.CreateMesh`。按相对位置和文件单位，将构建中的多个三角网格模型及组件合并为一个毫米单位的网格。已保存的切片器加工路径和切片结果不属于导入范围。
   - 在三维预览中保留可用的网格颜色及 OBJ/PLY/3MF 漫反射纹理
   - G-code/NGC 导入保留模态 XYZ 坐标、单位和工作原点、平面圆弧和螺旋线、停留及激光 ON/OFF 路径。通过 `UI.Config.GCodeImportDialect` 选择 CNC、Laser 或 Marlin 规则，通过 `GCodeImportInfo.TryGet` 查询源 F/S/T 和行范围，不应用设备设置。
   - 矢量文件的容差路径连接和基于内容的 Gerber/Excellon 识别
- 远程通信与动态数据
   - 用于标记控制和数据访问的 TCP/IP、Serial（RS-232）、WebSocket 及 MQTT
   - 内置 HTTP MCP 服务器，支持 AI 辅助注册编辑器、编辑文档与实体、运行模拟以及控制已注册设备
   - 面向文本和条码数据的事件、文件、偏移、链接实体及 C# 脚本转换
- 文档、编辑器与模拟
   - 四个文档页面，支持图层、画笔、组、块及可配置数量的 Undo/Redo
   - Debug 和 Release 均支持 WinForms 与 WPF `SiriusEditorControl` / `SiriusMultiEditorControl`，并支持将一个文档渲染到多个视图
   - [字体编辑器](SpiralLab.Sirius3.UI/FontEditor/README.md)：通过 WinForms/WPF **Font Edit** 菜单独立编辑 CXF/LFF/EO FNT/DOT 字形，支持 mm 矢量、整数 DOT 单元格、BBox/度量、撤销/重做、DXF 导入、文本预览与已安装 TTF 转换。Debug 和 Release 构建均始终包含。
   - 使用屏幕固定尺寸标记、光束效果和可选碎屑效果的实时加工路径可视化
   - 面向相机及检测流程的网格化拼接图像可视化
- 开放架构
   - 可扩展的编辑器、实体、标记器、振镜、激光器、功率计和远程通信接口
   - 内置 MCP 服务器，支持 AI 辅助注册编辑器、编辑文档与实体以及控制设备

## 主要变更事项
|                              |                SIRIUS3                   |              SIRIUS2                  |
|:-----------------------------|:-----------------------------------------|:--------------------------------------|
| 多页面支持                    | 支持 4 个页面的交叉编辑                   | 单页面编辑                             |
| 摄像头                        | 6 个 (2D + 5 个 3D) 摄像头                | 单个 3D 摄像头                         |
| 渲染速度                      | GPU 加速 OpenGL 着色器引擎                | 内置着色器引擎                         |
| 渲染模式                      | Model, PerVertex, Normal, ZDepth          | 无                                    |
| 选择功能                      | 点/线/三角形 AABB 加速                    | 低速                                  |
| 填充 (Hatch)                  | 基于绕组规则的多重填充                    | 单个填充                               |
| 3D 网格切片机                 | 内置 STL、OBJ、PLY、STEP、3MF 网格切片机 | 无                                    |
| Gerber / Excellon            | 基于内容识别的导入                        | 无                                    |
| 外部字体文件                  | CXF, LFF, FNT, DOT 文件格式               | 仅支持自定义 CXF, LFF 文件格式         |
| 画笔 (Pen)                    | 分离 Entity 和 Layer 的画笔属性           | Entity 单一画笔                        |
| MCP                          | 支持                                      | 无                                    |
| 库更新                        | 支持 NuGet 包管理器                      | 手动                                   |
                                                                                                              
![sirius3_hatch](https://spirallab.co.kr/sirius3/sirius3_hatch.png)
![sirius3_pod](https://spirallab.co.kr/sirius3/sirius3_pod.png)
![sirius3_slicer](https://spirallab.co.kr/sirius3/sirius3_slicer.png)
![sirius3_syncaxis](https://spirallab.co.kr/sirius3/sirius3_syncaxis.png)

## 软件包 / DLLs
- `SpiralLab.Sirius3.Dependencies` — SCANLAB RTC4/5/6, syncAXIS 运行时, 字体, 示例文件
- `SpiralLab.Sirius3` — 硬件控制 (振镜/激光/功率计等)
- `SpiralLab.Sirius3.UI` — 实体、几何处理、OpenGL 渲染、WinForms/WPF 控件及内置 MCP 集成
 > 支持通过 NuGet 包管理器进行便捷的安装及更新。

## 目标平台
- `net481`
- `net8.0-windows`
- `net9.0-windows`
- `net10.0-windows`

## 系统要求
- Windows 10/11 (x64)
- GPU/驱动程序至少支持 OpenGL 3.3（强烈建议使用最新驱动程序）
- 需要安装 SCANLAB 驱动程序/运行时
- Visual Studio 2022 或更高版本
 
## 依赖项
- SCANLAB
   - RTC4: v2023.11.02
   - RTC5: v2024.09.27
   - RTC6: 2026.8.17 v1.25.1
   - syncAXIS: v1.8.2 (2023.03.09)

- .NET
   - `net481`: OpenTK 3.3.3
   - `net8.0-windows`, `net9.0-windows`, `net10.0-windows`: OpenTK 和 OpenTK.Mathematics 4.9.4
   - 所有目标框架的通用软件包依赖项
      - Microsoft.Extensions.Logging 10.0.10
      - Microsoft.Extensions.Logging.Abstractions 10.0.10
      - Microsoft.Extensions.Logging.Debug 10.0.10
      - Newtonsoft.Json 13.0.4
## 软件包安装
- 添加引用 (建议使用 NuGet 包管理器)
   - `SpiralLab.Sirius3.Dependencies` (https://www.nuget.org/packages/SpiralLab.Sirius3.Dependencies)
   - `SpiralLab.Sirius3` (https://www.nuget.org/packages/SpiralLab.Sirius3)
   - `SpiralLab.Sirius3.UI` (https://www.nuget.org/packages/SpiralLab.Sirius3.UI)

## 快速入门

安装上文列出的三个 Sirius3 NuGet 软件包。以下片段展示主要流程；设备创建、验证和事件绑定使用 `...` 省略。请将启动、加载和关闭步骤分别放入宿主对应的事件中。

# [WinForms](#tab/winforms)

项目设置

```xml
<PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFrameworks>net481;net8.0-windows;net9.0-windows;net10.0-windows</TargetFrameworks>
    <UseWindowsForms>true</UseWindowsForms>
    <ApplicationHighDpiMode>PerMonitorV2</ApplicationHighDpiMode>
</PropertyGroup>
```

初始化 Core，创建编辑器，然后注册已初始化的设备。WinForms 的 DPI 初始化应先于这些步骤；请参阅下方 HiDPI 部分。

```csharp
using SpiralLab.Sirius3.MCP;
using System.Windows.Forms;

// Startup: initialize Core and create the host UI.
SpiralLab.Sirius3.Core.Initialize();
var form = new Form
{
    AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F),
    AutoScaleMode = AutoScaleMode.Dpi
};
var editor = new SpiralLab.Sirius3.UI.WinForms.SiriusEditorControl
{
    Dock = DockStyle.Fill
};
form.Controls.Add(editor);
...

// Load: create and initialize the scanner, laser, I/O, power meter and marker.
...
editor.RegisterDevices(rtc, laser, powerMeter, ..., marker);

// Optional MCP: start after device registration (Remote license).
var server = MCPFactory.CreateServer(0, "MCP", editor);
server.AccessMode = MCPAccessMode.All;
editor.MCPServer = server;
await server.Start();
...
```

异步关闭时保持 UI 事件循环运行，并等待资源清理完成：

```csharp
// FormClosing / Window.Closing: await cleanup on the owning UI thread.
var document = editor.Document;
await editor.DisposeDevices();
editor.Dispose();
document?.Dispose();
...

// Application exit: after the UI lifetime ends.
SpiralLab.Sirius3.Core.Cleanup();
```

# [WPF](#tab/wpf)

项目设置

```xml
<PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFrameworks>net481;net8.0-windows;net9.0-windows;net10.0-windows</TargetFrameworks>
    <UseWPF>true</UseWPF>
    <ApplicationManifest>app.manifest</ApplicationManifest>
</PropertyGroup>
```

创建 WPF 编辑器前初始化 Core，并在 Window 加载时注册已初始化的设备。

```csharp
using SpiralLab.Sirius3.MCP;
using System.Windows;

// Startup: initialize Core and create the host UI.
SpiralLab.Sirius3.Core.Initialize();
var editor = new SpiralLab.Sirius3.UI.WPF.SiriusEditorControl();
var window = new Window { Content = editor };
...

// Loaded: create and initialize the scanner, laser, I/O, power meter and marker.
...
editor.RegisterDevices(rtc, laser, powerMeter, ..., marker);

// Optional MCP: start after device registration (Remote license).
var server = MCPFactory.CreateServer(0, "MCP", editor);
server.AccessMode = MCPAccessMode.All;
editor.MCPServer = server;
await server.Start();
...
```

异步关闭时保持 UI 事件循环运行，并等待资源清理完成：

```csharp
// FormClosing / Window.Closing: await cleanup on the owning UI thread.
var document = editor.Document;
await editor.DisposeDevices();
editor.Dispose();
document?.Dispose();
...

// Application exit: after the UI lifetime ends.
SpiralLab.Sirius3.Core.Cleanup();
```

`DisposeDevices()` 先等待已注册的 MCP 服务器和远程连接释放，再清理设备。完成编辑器和 Document 清理后，在应用退出阶段调用 `Core.Cleanup()`。详细示例包含关闭事件的重复请求保护和错误处理。

完整示例：[WinForms 指南（英语）](https://github.com/labspiral/sirius3/blob/main/doc/WinForms.md)、[WPF 指南（英语）](https://github.com/labspiral/sirius3/blob/main/doc/WPF.md)、[演示项目](DEMOS.zhCN.md)。

---

## WinForms HiDPI

首先更新 DLL/软件包。现有 `DpiUnaware` 应用可以保留原有行为。要启用按监视器缩放，请在创建控件前配置消费 EXE；DLL 不会更改进程 DPI 模式。现有宿主窗体可以保留 `AutoScaleMode.Font`、字体和设计器基准值。请另行检查自定义固定像素布局及手动缩放。

对于使用手动 `Main` 的 .NET 8/9/10 应用，请在 `EnableVisualStyles` 和 `Core.Initialize` 之前选择 Per-Monitor V2：

```csharp
#if NET8_0_OR_GREATER
Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
#endif
Application.EnableVisualStyles();
Application.SetCompatibleTextRenderingDefault(false);
```

对于 .NET Framework 4.8.1，请将以下节合并到 EXE 的现有 app.config，保留绑定重定向，在 manifest 中声明 Windows 10 兼容性，并在创建控件前调用 `EnableVisualStyles`。移除冲突的 unaware DPI 声明及 `EnableWindowsFormsHighDpiAutoResizing=false` 设置。

```xml
<System.Windows.Forms.ApplicationConfigurationSection>
  <add key="DpiAwareness" value="PerMonitorV2" />
</System.Windows.Forms.ApplicationConfigurationSection>
```

使用 `ApplicationConfiguration.Initialize()` 时，请在 EXE 项目中设置 `<ApplicationHighDpiMode>PerMonitorV2</ApplicationHighDpiMode>`，保持单一初始化路径。字体大小仍以 pt 为单位。应用拥有的 ImageList 应根据保留的源图像按 DPI 重建，在添加图像前设置 `ImageSize`/`ColorDepth`，保留键和索引，替换全部引用后仅释放自己拥有的旧列表。WPF 的 96 是逻辑单位基准，不要再次对布局乘以 DPI 缩放。

[Microsoft: .NET Framework HiDPI](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/high-dpi-support-in-windows-forms), [WinForms automatic scaling](https://learn.microsoft.com/en-us/dotnet/desktop/winforms/forms/autoscale).

## 演示程序
- 程序说明 [DEMOS.zhCN.md](DEMOS.zhCN.md) 
- 创建振镜、激光、功率计、标记器等设备对象并连接到 SiriusEditorControl。
- 示例代码: https://github.com/labspiral/sirius3/tree/main/demos

## 许可证
- 商业用途需购买许可证。
- 许可证：RTC 实例数量 + [选项]
    - MoF 选项：利用外部编码器（实时跟踪及待机等）实现的飞行加工功能（Processing on the fly）。
    - MultiBeam 选项：由 1 个激光源 + 2 个 AOM + 2 个扫描头组成的配置，可在跳跃区间实时更改激光束路径进行加工的功能。
    - syncAXIS 选项：采用 ACS 运动控制器 + excelliSCAN 扫描头配置，利用扫描头与工作台的同步实现大面积加工（XL-SCAN 解决方案）。
    - Remote 选项：支持通过套接字、串行、WebSocket 和 MQTT 外部通信进行配方更改、加工控制及数据查询与修改，并包含用于 AI 辅助文档/实体编辑和已注册设备控制的内置 MCP 服务器功能。
- 许可政策及第三方库请参阅 [LICENSE.zhCN.txt](LICENSE.zhCN.txt)、[THIRD-PARTY-NOTICES.zhCN.txt](THIRD-PARTY-NOTICES.zhCN.txt)。
- 邮箱：hcchoi@spirallab.co.kr | https://spirallab.co.kr
> 若无许可密钥，将以仅限使用30分钟的评估模式运行。

## 版本历史
- 历史信息 [HISTORY.zhCN.md](HISTORY.zhCN.md)

## API 文档
- 参考 https://spirallab.co.kr/sirius3/doc
