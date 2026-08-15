# Sirius3
面向 Windows/.NET 的精密激光加工平台，集成 SCANLAB 控制、设备连接、几何处理、OpenGL 可视化、文档编辑、模拟和加工执行

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
   - 矢量文件的容差路径连接和基于内容的 Gerber/Excellon 识别
- 远程通信与动态数据
   - 用于标记控制和数据访问的 TCP/IP、Serial（RS-232）、WebSocket 及 MQTT
   - 面向文本和条码数据的事件、文件、偏移、链接实体及 C# 脚本转换
- 文档、编辑器与模拟
   - 四个文档页面，支持图层、画笔、组、块及可配置数量的 Undo/Redo
   - 稳定版 WinForms 控件，以及仅在 Debug 中启用的开发中 WPF 编辑器/查看器移植，并支持将一个文档渲染到多个视图
   - 使用屏幕固定尺寸标记、光束效果和可选碎屑效果的实时加工路径可视化
   - 面向相机及检测流程的网格化拼接图像可视化
- 开放架构
   - 可扩展的编辑器、实体、标记器、振镜、激光器、功率计和远程通信接口

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
| 库更新                        | 支持 NuGet 包管理器                      | 手动                                   |
                                                                                                              
![sirius3_hatch](https://spirallab.co.kr/sirius3/sirius3_hatch.png)
![sirius3_pod](https://spirallab.co.kr/sirius3/sirius3_pod.png)
![sirius3_slicer](https://spirallab.co.kr/sirius3/sirius3_slicer.png)
![sirius3_syncaxis](https://spirallab.co.kr/sirius3/sirius3_syncaxis.png)

## 软件包 / DLLs
- `SpiralLab.Sirius3.Dependencies` — SCANLAB RTC4/5/6, syncAXIS 运行时, 字体, 示例文件
- `SpiralLab.Sirius3` — 硬件控制 (振镜/激光/功率计等)
- `SpiralLab.Sirius3.UI` — 实体、几何处理、OpenGL 渲染及 WinForms 控件；WPF 移植目前仅用于 Debug
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
   - RTC6: 2026.3.31 v1.24.0
   - syncAXIS: v1.8.2 (2023.03.09)

- .NET
   - `net481`
      - OpenTK 3.3.3
      - Microsoft.Extensions.Logging 8.0.1
      - Microsoft.Extensions.Logging.Abstractions 8.0.3 
   - `net8.0-windows`
      - OpenTK 4.9.4
      - OpenTK.Mathematics 4.9.4
      - Microsoft.Extensions.Logging 8.0.1
      - Microsoft.Extensions.Logging.Abstractions 8.0.3 
   - `net9.0-windows`
      - OpenTK 4.9.4
      - OpenTK.Mathematics 4.9.4
      - Microsoft.Extensions.Logging 9.0.15
      - Microsoft.Extensions.Logging.Abstractions 9.0.15  
   - `net10.0-windows`
      - OpenTK 4.9.4
      - OpenTK.Mathematics 4.9.4
      - Microsoft.Extensions.Logging 10.0.7
      - Microsoft.Extensions.Logging.Abstractions 10.0.7
   - 通用软件包依赖项
      - Newtonsoft.Json 13.0.4
   - Debug WPF 开发依赖项
      - MaterialDesignThemes 5.3.2
      - OpenTK.GLWpfControl 3.3.0 / 4.3.6
   - Debug WPF 内嵌实现
      - PropertyTools.Wpf 3.1.0
      - OxyPlot.Wpf 2.2.0

## WPF

`SpiralLab.Sirius3.UI.WPF` 是仅由 Debug 构建编译的开发中移植。在完成之前，
Release 二进制文件和官方 `SpiralLab.Sirius3.UI` NuGet 包不包含 WPF 源代码、
XAML、资源、公共类型及 WPF 专用依赖项。Debug 构建提供 `ViewerControl`、
`EditorControl`、`SiriusEditorControl`、`SiriusMultiEditorControl`、原生 WPF
属性表、树以及设备/编辑面板。界面在整个进程中使用单一 Material Design 主题和适配 HiDPI 的紧凑工业布局。
WPF 属性表下方的可调整区域会显示当前属性的本地化说明。WPF 实现不使用
`WindowsFormsHost`，也不公开
WinForms UI 控件。可执行宿主必须在创建第一个窗口之前启用 Per-Monitor V2；
Release DLL 不会更改使用方进程的 DPI 模式。原有 WinForms 保持经过验证的
DpiUnaware 基线。需要 OpenGL 3.3 或更高版本；编译成功不能替代真实 DPI 与 GPU
交互验证。

```xml
<wpf:SiriusEditorControl x:Name="editor"
    xmlns:wpf="clr-namespace:SpiralLab.Sirius3.UI.WPF;assembly=SpiralLab.Sirius3.UI" />
```

分配的文档和设备仍由调用方拥有。`Unloaded` 只暂停屏幕工作，因此控件永久关闭时
应调用 `Dispose()`。仅当控件应显式释放已注册设备时调用 `DisposeDevices()`，并由
调用方单独释放已分离的文档。

## 软件包安装
- 添加引用 (建议使用 NuGet 包管理器)
   - `SpiralLab.Sirius3.Dependencies` (https://www.nuget.org/packages/SpiralLab.Sirius3.Dependencies)
   - `SpiralLab.Sirius3` (https://www.nuget.org/packages/SpiralLab.Sirius3)
   - `SpiralLab.Sirius3.UI` (https://www.nuget.org/packages/SpiralLab.Sirius3.UI)

## 快速入门
项目设置
```
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

<ItemGroup Condition="'$(TargetFramework)'=='net481' OR '$(TargetFramework)'=='net8.0-windows'">
    <PackageReference Include="Microsoft.Extensions.Logging" Version="8.0.1" />
    <PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="8.0.3" />
</ItemGroup>
	
<ItemGroup Condition="'$(TargetFramework)'=='net9.0-windows'">
    <PackageReference Include="Microsoft.Extensions.Logging" Version="9.0.15" />
    <PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="9.0.15" />
</ItemGroup>
	
<ItemGroup Condition="'$(TargetFramework)'=='net10.0-windows'">
    <PackageReference Include="Microsoft.Extensions.Logging" Version="10.0.7" />
    <PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="10.0.7" />
</ItemGroup>
	
<ItemGroup>
    <PackageReference Include="SpiralLab.Sirius3.Dependencies" Version="1.*" />
    <PackageReference Include="SpiralLab.Sirius3" Version="1.*" />
    <PackageReference Include="SpiralLab.Sirius3.UI" Version="1.*" />
    <PackageReference Include="Newtonsoft.Json" Version="13.0.4" />
</ItemGroup>
```

示例代码
```
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

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
        CreateAndExecuteMainForm();
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
        editorControl.Dock = DockStyle.Fill;
        dynamicForm.Controls.Add(editorControl);
        dynamicForm.ResumeLayout(false);

        dynamicForm.Load += (s, e) =>
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
            // Gentec-EO 的 scaleIndex 为 null 时，不更改设备当前的量程/自动量程设置。
            // 如需指定测量量程，请传入 0 到 41 范围内的值。
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
        };

       dynamicForm.FormClosing += (s, e) =>
        {
            var dlgResult = MessageBox.Show(dynamicForm, $"Do you really want to terminate program ?", "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dlgResult != DialogResult.Yes)
            {
                e.Cancel = true;
                return;
            }

            // 释放设备
            editorControl.DisposeDevices();

            // 释放文档
            editorControl.Document?.Dispose();
         
            // 清理 Sirius3 库
            SpiralLab.Sirius3.Core.Cleanup();
        };

        Application.Run(dynamicForm);
    }
}
```
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
    - Remote 选项：支持通过套接字、串行、Web、MQTT 协议进行外部通信，实现配方更改、加工控制、数据查询及修改。
- 许可政策及第三方库请参阅 [LICENSE.zhCN.txt](LICENSE.zhCN.txt)、[THIRD-PARTY-NOTICES.zhCN.txt](THIRD-PARTY-NOTICES.zhCN.txt)。
- 邮箱：hcchoi@spirallab.co.kr | https://spirallab.co.kr
> 若无许可密钥，将以仅限使用30分钟的评估模式运行。

## 版本历史
- 历史信息 [HISTORY.zhCN.md](HISTORY.zhCN.md)

## API 文档
- 参考 https://spirallab.co.kr/sirius3/doc
