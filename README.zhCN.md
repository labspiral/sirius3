# Sirius3
基于 .NET 的精密激光加工全能平台

![sirius3_logo](https://spirallab.co.kr/sirius3/sirius3_logo3.png)

---

## 亮点
![sirius3_editor](https://spirallab.co.kr/sirius3/sirius3_editor.png)

- SCANLAB RTC 控制器
   - RTC4 / RTC4e / RTC5 / RTC6 / RTC6e
   - XL-SCAN (基于 syncAXIS 的 RTC6 + ACS 组合)
- 测量与分析
   - 支持利用振镜运动路径和信号输出日志进行图形输出
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
- 支持多种实体和格式
   - 点、线、弧、多段线、三角形、矩形、螺旋、Trépannage、样条曲线等
   - 图层、组、块、块插入等
   - Text, SiriusText, ImageText, Circular Text 等
   - Image, DXF, HPGL, ZPL
   - QR, DataMatrix, PDF417 条码
   - STL, OBJ, PLY 等 3D 网格格式 
- 文档及页面
   - 支持多文档和页面
   - 支持将一个文档渲染到多个视图目标
- 开放架构
   - 用于编辑器 (Editor)、标记器 (Marker) 和激光源控制的代码以开源形式提供

## 主要变更事项
|                              |                SIRIUS3                   |              SIRIUS2                  |
|:-----------------------------|:-----------------------------------------|:--------------------------------------|
| 多页面支持                    | 支持 4 个页面的交叉编辑                   | 单页面编辑                             |
| 摄像头                        | 6 个 (2D + 5 个 3D) 摄像头                | 单个 3D 摄像头                         |
| 渲染速度                      | 改进的着色器引擎                         | 内置着色器引擎                         |
| 渲染模式                      | Model, PerVertex, Normal, ZDepth          | 无                                    |
| 选择功能                      | 搭载改进的算法                           | 低速                                   |
| 填充 (Hatch)                  | 填充模式可重复应用                       | 单个填充                               |
| 3D 网格切片机                 | 内置 PLY, OBJ, STL 网格切片机             | 无                                    |
| Gerber 文件 (RS-274x)        | 支持                                     | 无                                    |
| 晶圆/基板映射 (Map)           | 编辑器内置                               | 无                                    |
| 外部字体文件                  | CXF, LFF, FNT 文件格式                    | 仅支持自定义 CXF, LFF 文件格式         |
| 画笔 (Pen)                    | 分离 Entity 和 Layer 的画笔属性           | Entity 单一画笔                        |
| 库更新                        | 支持 NuGet 包管理器                      | 手动                                   |
                                                                                                              
![sirius3_hatch](https://spirallab.co.kr/sirius3/sirius3_hatch.png)
![sirius3_pod](https://spirallab.co.kr/sirius3/sirius3_pod.png)
![sirius3_slicer](https://spirallab.co.kr/sirius3/sirius3_slicer.png)
![sirius3_syncaxis](https://spirallab.co.kr/sirius3/sirius3_syncaxis.png)

## 软件包 / DLLs
- `SpiralLab.Sirius3.Dependencies` — SCANLAB RTC4/5/6, syncAXIS 运行时, 字体, 示例文件
- `SpiralLab.Sirius3` — 硬件控制 (振镜/激光/功率计等)
- `SpiralLab.Sirius3.UI` — 多种实体, 3D 渲染引擎, WinForms 等 UI 控件
 > 支持通过 NuGet 包管理器进行便捷的安装及更新。

## 目标平台
- `net481`
- `net8.0-windows`
- `net9.0-windows`
- `net10.0-windows`

## 系统要求
- Windows 10/11 (x64)
- 需要支持 OpenGL 3.3 或更高版本的 GPU
- 需要安装 SCANLAB 驱动程序/运行时
- Visual Studio 2022 或更高版本
 
## 依赖项
- SCANLAB
   - RTC4: v2023.11.02
   - RTC5: v2024.09.27
   - RTC6: 2025.10.30 v1.22.1
   - syncAXIS: v1.8.2 (2023.03.09)

- .NET / OpenTK
   - `net481`
      - OpenTK 3.3.3
   - `net8.0-windows`
   - `net9.0-windows`
   - `net10.0-windows`
      - OpenTK 4.9.4
      - OpenTK.Mathematics 4.9.4
   - Common
      - Newtonsoft.Json 13.0.4
      - Microsoft.Extensions.Logging 8.0.1
      - Microsoft.Extensions.Logging.Abstractions 8.0.3

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
<PropertyGroup Condition="'$(TargetFramework.StartsWith(`net8.0-windows`))' OR '$(TargetFramework.StartsWith(`net9.0-windows`))' OR '$(TargetFramework.StartsWith(`net10.0-windows`))'">
	<DefineConstants>$(DefineConstants);OPENTK4</DefineConstants>
</PropertyGroup>

<ItemGroup Condition="'$(TargetFramework)'=='net481'">
	<PackageReference Include="OpenTK" Version="3.3.3" />
</ItemGroup>

<ItemGroup Condition="'$(TargetFramework.StartsWith(`net8.0-windows`))' OR '$(TargetFramework.StartsWith(`net9.0-windows`))' OR '$(TargetFramework.StartsWith(`net10.0-windows`))'">
	<PackageReference Include="OpenTK" Version="4.9.4" />
	<PackageReference Include="OpenTK.Mathematics" Version="4.9.4" />
</ItemGroup>

<ItemGroup>
	<PackageReference Include="SpiralLab.Sirius3.Dependencies" Version="1.*" />
	<PackageReference Include="SpiralLab.Sirius3" Version="1.*" />
	<PackageReference Include="SpiralLab.Sirius3.UI" Version="1.*" />
	<PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="8.0.3" />
	<PackageReference Include="Microsoft.Extensions.Logging" Version="8.0.1" />
	<PackageReference Include="Newtonsoft.Json" Version="13.0.4" />
</ItemGroup>
```

示例代码
```
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

        // 创建 WinForm
        CreateAndExecuteMainForm();
    }

    public CreateAndExecuteMainForm()
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
            // 初始化 Sirius3 库
            SpiralLab.Sirius3.Core.Initialize();

            // 创建并初始化设备后注册到 EditorControl
            bool success = true;

            // 振镜控制
            string correctionFile = "cor_1to1.ct5";
            string correctionPath = Path.Combine(SpiralLab.Sirius3.Config.CorrectionPath, correctionFile);
            const var fov = 100.0;
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
            //var powerMeter = PowerMeterFactory.CreateGentecEO(index, 3);
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
            var dlgResult = MessageBox.Show(this, $"Do you really want to terminate program ?", "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dlgResult != DialogResult.Yes)
            {
                //editorControl.Marker?.Stop();
                e.Cancel = true;
                return;
            }

            // 释放文档
            editorControl.Document?.Dispose();
            // 释放设备
            editorControl.DisposeDevices();
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

## 许可
- 商业使用需要购买许可。
- 许可：RTC 实例数量 + [选项：MoF, MultiBeam 或 syncAXIS]
- 许可及外部库请参考 LICENSE.zhCN.txt, THIRD-PARTY-NOTICES.zhCN.txt。
- 电子邮件：hcchoi@spirallab.co.kr | https://spirallab.co.kr
> 如果没有许可密钥，将以可使用 30 分钟的评估模式运行。

## 版本历史
- 历史信息 [HISTORY.zhCN.md](HISTORY.zhCN.md)

## API 文档
- 参考 https://spirallab.co.kr/sirius3/doc
