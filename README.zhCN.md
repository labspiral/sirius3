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
   - Sirius3 文档、DXF/DWG、HPGL/PLT、Gerber/Excellon 及 G-code/NGC
   - 光栅图像以及 STL、OBJ、PLY、STP/STEP 三维模型
   - 在三维预览中保留可用的网格颜色及 OBJ/PLY 漫反射纹理
   - 矢量文件的容差路径连接和基于内容的 Gerber/Excellon 识别
- 远程通信与动态数据
   - 用于标记控制和数据访问的 TCP/IP、Serial（RS-232）、WebSocket 及 MQTT
   - 内置 HTTP MCP 服务器，支持 AI 辅助注册编辑器、编辑文档与实体、运行模拟以及控制已注册设备
   - 面向文本和条码数据的事件、文件、偏移、链接实体及 C# 脚本转换
- 文档、编辑器与模拟
   - 四个文档页面，支持图层、画笔、组、块及可配置数量的 Undo/Redo
   - Debug 和 Release 均支持 WinForms 与 WPF `SiriusEditorControl` / `SiriusMultiEditorControl`，并支持将一个文档渲染到多个视图
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
| 3D 网格切片机                 | 内置 STL、OBJ、PLY、STEP 网格切片机       | 无                                    |
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
   - RTC6: 2026.6.19 v1.25.0
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

# [WinForms](#tab/winforms)

项目设置
```xml
<PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFrameworks>net481;net8.0-windows;net9.0-windows;net10.0-windows</TargetFrameworks>
    <UseWindowsForms>true</UseWindowsForms>
</PropertyGroup>

<PropertyGroup Condition="'$(TargetFramework)'=='net481'">
	<DefineConstants>$(DefineConstants);OPENTK3</DefineConstants>
</PropertyGroup>
<PropertyGroup Condition="'$(TargetFramework)'!='net481'">
	<DefineConstants>$(DefineConstants);OPENTK4</DefineConstants>
</PropertyGroup>

<ItemGroup Condition="'$(TargetFramework)'=='net481'">
	<PackageReference Include="OpenTK" Version="3.3.3" />
</ItemGroup>
<ItemGroup Condition="'$(TargetFramework)'!='net481'">
	<PackageReference Include="OpenTK" Version="4.9.4" />
	<PackageReference Include="OpenTK.Mathematics" Version="4.9.4" />
</ItemGroup>

<ItemGroup>
    <PackageReference Include="Microsoft.Extensions.Logging" Version="10.0.10" />
    <PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="10.0.10" />
    <PackageReference Include="Microsoft.Extensions.Logging.Debug" Version="10.0.10" />
    <PackageReference Include="SpiralLab.Sirius3.Dependencies" Version="1.*" />
    <PackageReference Include="SpiralLab.Sirius3" Version="1.*" />
    <PackageReference Include="SpiralLab.Sirius3.UI" Version="1.*" />
    <PackageReference Include="Newtonsoft.Json" Version="13.0.4" />
</ItemGroup>
```

示例代码
```csharp
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

using SpiralLab.Sirius3.MCP;
using SpiralLab.Sirius3.IO;
using SpiralLab.Sirius3.Laser;
using SpiralLab.Sirius3.Marker;
using SpiralLab.Sirius3.PowerMap;
using SpiralLab.Sirius3.PowerMeter;
using SpiralLab.Sirius3.Scanner;
using SpiralLab.Sirius3.Scanner.Rtc;

#if OPENTK3
    using OpenTK;
    using DVec3 = OpenTK.Vector3d;
#elif OPENTK4
    using OpenTK.Mathematics;
    using DVec3 = OpenTK.Mathematics.Vector3d;
#endif

static class Program
{
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // 初始化 Sirius3 库
        SpiralLab.Sirius3.Core.Initialize();
        // 创建 WinForm
        try { CreateAndExecuteMainForm(); }
        finally { SpiralLab.Sirius3.Core.Cleanup(); }
    }

    static void CreateAndExecuteMainForm()
    {
        // 创建动态窗体并添加 SiriusEditorControl
        Form dynamicForm = new Form();
        dynamicForm.SuspendLayout();
        dynamicForm.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        dynamicForm.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        dynamicForm.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
        dynamicForm.Text = "DEMO - (c)SpiralLab";
        dynamicForm.Size = new Size(1600, 1200);
        dynamicForm.StartPosition = FormStartPosition.CenterScreen;
        var editorControl = new SpiralLab.Sirius3.UI.WinForms.SiriusEditorControl();
        IMCPServer mcpServer = null;
        bool startupFailed = false;
        bool shutdownStarted = false;
        bool closeAfterShutdown = false;
        editorControl.Dock = DockStyle.Fill;
        dynamicForm.Controls.Add(editorControl);
        dynamicForm.ResumeLayout(false);

        dynamicForm.Load += async (s, e) =>
        {
            // 创建并初始化设备后注册到 EditorControl
            bool success = true;

            // 振镜控制
            string correctionFile = "cor_1to1.ct5";
            string correctionPath = Path.Combine(SpiralLab.Sirius3.Config.CorrectionPath, correctionFile);
            const double fov = 100.0;
            var kfactor = Math.Pow(2, 20) / fov;
            var index = 0;
            var rtc = ScannerFactory.CreateRtc5(index, kfactor, LaserModes.Yag1, RtcSignalLevels.ActiveHigh, RtcSignalLevels.ActiveHigh, correctionPath);
            success &= rtc.Initialize();
            rtc.CtlFrequency(50 * 1000, 2);
            rtc.CtlSpeed(100, 100);

            // 数字输入输出控制
            var dIExt1 = IOFactory.CreateInputExtension1(rtc); success &= dIExt1.Initialize();
            var dOExt1 = IOFactory.CreateOutputExtension1(rtc); success &= dOExt1.Initialize();
            var dOExt2 = IOFactory.CreateOutputExtension2(rtc); success &= dOExt2.Initialize();
            var dILaserPort = IOFactory.CreateInputLaserPort(rtc); success &= dILaserPort.Initialize();
            var dOLaserPort = IOFactory.CreateOutputLaserPort(rtc); success &= dOLaserPort.Initialize();

            // 功率计控制
            double laserMaxPower = 20;
            var powerMeter = PowerMeterFactory.CreateVirtual(index, laserMaxPower);
            //var powerMeter = PowerMeterFactory.CreateCoherentPowerMax(index, 4);
            //var powerMeter = PowerMeterFactory.CreateGentecEO(index, 3, scaleIndex: null);
            success &= powerMeter.Initialize();

            // 激光控制
            var laser = LaserFactory.CreateVirtualDutyCycle(index, laserMaxPower, 0, 100);
            //var laser = LaserFactory.Create ...
            success &= laser.Initialize();
            laser.Scanner = rtc;

            // 功率映射 (PowerMap)
            var powerMap = PowerMapFactory.CreateDefault(index, "default");
            powerMap.Reset1to1("10000", laserMaxPower);
            laser.PowerMap = powerMap;

            // 标记器 (Marker)
            var marker = MarkerFactory.CreateRtc(index);
            //var marker = MarkerFactory.CreateRtcFast(index);
            //var marker = MarkerFactory.CreateSyncAxis(index);
            success &= marker.Initialize();

            Debug.Assert(success);

            // 注册设备
            editorControl.RegisterDevices(rtc, laser, powerMeter, dIExt1, dILaserPort, dOExt1, dOExt2, dOLaserPort, marker);

            // 启动经过身份验证的 HTTP MCP 服务器。
            try
            {
                mcpServer = MCPFactory.CreateServer(0, "MCP", editorControl);
                mcpServer.AccessMode = MCPAccessMode.All;
                if (!await mcpServer.Start())
                    throw new InvalidOperationException("Failed to start the MCP server.");
                if (!shutdownStarted)
                    dynamicForm.Text = $"{dynamicForm.Text} + MCP ({mcpServer.Options.HttpEndpoint})";
            }
            catch (Exception ex)
            {
                startupFailed = true;
                Trace.WriteLine(ex);
                if (!shutdownStarted) dynamicForm.Close();
            }
        };

        dynamicForm.FormClosing += async (s, e) =>
        {
            if (closeAfterShutdown) return;
            e.Cancel = true;
            if (shutdownStarted) return;
            if (!startupFailed && MessageBox.Show(dynamicForm, "Do you really want to terminate the program?",
                "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            shutdownStarted = true;
            await Task.Yield(); // 先返回 FormClosing，以便 UI 处理待执行的 MCP 工作。
            try
            {
                if (mcpServer != null)
                    await mcpServer.DisposeAsync();
            }
            catch (Exception ex) { Trace.WriteLine(ex); }
            finally
            {
                mcpServer = null;
                editorControl.MCPServer = null;
                var document = editorControl.Document;
                editorControl.Document = null;
                try { await editorControl.DisposeDevices(); }
                catch (Exception ex) { Trace.WriteLine(ex); }
                try { document?.Dispose(); }
                catch (Exception ex) { Trace.WriteLine(ex); }
                closeAfterShutdown = true;
                dynamicForm.Close();
            }
        };

        Application.Run(dynamicForm);
    }
}
```

# [WPF](#tab/wpf)

项目设置：

```xml
<PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFrameworks>net481;net8.0-windows;net9.0-windows;net10.0-windows</TargetFrameworks>
    <UseWPF>true</UseWPF>
    <UseWindowsForms>true</UseWindowsForms>
</PropertyGroup>

<PropertyGroup Condition="'$(TargetFramework)'=='net481'">
	<DefineConstants>$(DefineConstants);OPENTK3</DefineConstants>
</PropertyGroup>
<PropertyGroup Condition="'$(TargetFramework)'!='net481'">
	<DefineConstants>$(DefineConstants);OPENTK4</DefineConstants>
</PropertyGroup>

<ItemGroup Condition="'$(TargetFramework)'=='net481'">
	<PackageReference Include="OpenTK" Version="3.3.3" />
</ItemGroup>
<ItemGroup Condition="'$(TargetFramework)'!='net481'">
	<PackageReference Include="OpenTK" Version="4.9.4" />
	<PackageReference Include="OpenTK.Mathematics" Version="4.9.4" />
</ItemGroup>

<ItemGroup>
    <PackageReference Include="Microsoft.Extensions.Logging" Version="10.0.10" />
    <PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="10.0.10" />
    <PackageReference Include="Microsoft.Extensions.Logging.Debug" Version="10.0.10" />
    <PackageReference Include="SpiralLab.Sirius3.Dependencies" Version="1.*" />
    <PackageReference Include="SpiralLab.Sirius3" Version="1.*" />
    <PackageReference Include="SpiralLab.Sirius3.UI" Version="1.*" />
    <PackageReference Include="Newtonsoft.Json" Version="13.0.4" />
</ItemGroup>
```

`Program.cs`：

```csharp
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using SpiralLab.Sirius3.IO;
using SpiralLab.Sirius3.Laser;
using SpiralLab.Sirius3.Marker;
using SpiralLab.Sirius3.MCP;
using SpiralLab.Sirius3.PowerMap;
using SpiralLab.Sirius3.PowerMeter;
using SpiralLab.Sirius3.Scanner;
using SpiralLab.Sirius3.Scanner.Rtc;
using SpiralLab.Sirius3.UI.WPF;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        var app = new System.Windows.Application();
        SiriusEditorControl editor = null;
        IMCPServer mcpServer = null;
        var coreInitialized = false;
        var editorDisposed = false;

        async Task DisposeEditorResourcesAsync()
        {
            var document = editor.Document;
            try { await editor.DisposeDevices(); }
            finally { try { editor.Dispose(); } finally { document?.Dispose(); } }
        }

        async Task DisposeEditorAsync()
        {
            if (editorDisposed || editor == null) return;
            editorDisposed = true;
            var server = mcpServer;
            mcpServer = null;
            try
            {
                if (server != null)
                    await server.DisposeAsync();
            }
            finally
            {
                editor.MCPServer = null;
                await DisposeEditorResourcesAsync();
            }
        }

        try
        {
            SpiralLab.Sirius3.Core.Initialize();
            coreInitialized = true;
            editor = new SiriusEditorControl();

            const int index = 0;
            const double fieldSize = 100.0;
            const double laserMaxPower = 20.0;
            var scanner = ScannerFactory.CreateVirtual(
                index,
                Math.Pow(2, 20) / fieldSize,
                LaserModes.Yag1,
                RtcSignalLevels.ActiveHigh,
                RtcSignalLevels.ActiveHigh,
                null);
            var dIExt1 = IOFactory.CreateInputExtension1(scanner);
            var dILaserPort = IOFactory.CreateInputLaserPort(scanner);
            var dOExt1 = IOFactory.CreateOutputExtension1(scanner);
            var dOExt2 = IOFactory.CreateOutputExtension2(scanner);
            var dOLaserPort = IOFactory.CreateOutputLaserPort(scanner);
            var powerMeter = PowerMeterFactory.CreateVirtual(index, laserMaxPower);
            var laser = LaserFactory.CreateVirtualDutyCycle(index, laserMaxPower, 0, 100);
            var powerMap = PowerMapFactory.CreateDefault(index, "default");
            var marker = MarkerFactory.CreateVirtual(index);

            powerMap.Reset1to1("10000", laserMaxPower);
            laser.Scanner = scanner;
            laser.PowerMap = powerMap;

            var success = scanner.Initialize();
            success &= scanner.CtlFrequency(50 * 1000, 2);
            success &= scanner.CtlSpeed(100, 100);
            success &= dIExt1.Initialize();
            success &= dILaserPort.Initialize();
            success &= dOExt1.Initialize();
            success &= dOExt2.Initialize();
            success &= dOLaserPort.Initialize();
            success &= powerMeter.Initialize();
            success &= laser.Initialize();
            success &= marker.Initialize();

            editor.RegisterDevices(
                scanner, laser, powerMeter,
                dIExt1, dILaserPort,
                dOExt1, dOExt2, dOLaserPort,
                marker);

            if (!success)
                throw new InvalidOperationException("虚拟设备初始化失败。");

            var window = new Window
            {
                Title = "Sirius3 WPF Editor",
                Width = 1400,
                Height = 900,
                Content = editor,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
            };

            bool shutdownStarted = false;
            bool closeAfterShutdown = false;
            window.Loaded += async (_, __) =>
            {
                try
                {
                    mcpServer = MCPFactory.CreateServer(0, "MCP", editor);
                    mcpServer.AccessMode = MCPAccessMode.All;
                    if (!await mcpServer.Start())
                        throw new InvalidOperationException("Failed to start the MCP server.");
                    if (!shutdownStarted)
                        window.Title = $"{window.Title} + MCP ({mcpServer.Options.HttpEndpoint})";
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex);
                    if (!shutdownStarted) window.Close();
                }
            };

            // 在 MCP 关闭完成前保持 WPF 调度器和 GL 上下文运行。
            window.Closing += async (_, args) =>
            {
                if (closeAfterShutdown) return;
                args.Cancel = true;
                if (shutdownStarted) return;
                shutdownStarted = true;
                await Task.Yield();
                try { await DisposeEditorAsync(); }
                catch (Exception ex) { Trace.WriteLine(ex); }
                finally { closeAfterShutdown = true; window.Close(); }
            };
            app.Run(window);
        }
        finally
        {
            try
            {
                // 若在 Loaded 之前启动失败，MCP 服务器尚未创建。
                if (!editorDisposed && mcpServer == null && editor != null)
                    DisposeEditorResourcesAsync().GetAwaiter().GetResult();
            }
            finally
            {
                if (coreInitialized)
                    SpiralLab.Sirius3.Core.Cleanup();
            }
        }
    }
}
```

两个宿主都会取消首次关闭请求，并在 UI 消息循环运行时等待 `IMCPServer.DisposeAsync()`。随后释放编辑器资源，在消息循环返回后调用 `Core.Cleanup()`。MCP 运行期间不要在 WinForms 或 WPF UI 线程上直接调用 `IMCPServer.Dispose()`。

释放编辑器或文档之前，始终调用 `await editor.DisposeDevices()`。它会先关闭已注册的 MCP 服务器和远程连接，再释放其余设备。
使用原有 `void DisposeDevices()` 的应用必须修改关闭处理程序以等待返回的任务，并重新编译。

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
