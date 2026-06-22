# Sirius3 版本历史

## v1.10.9 (2026.6.22)
- 新增）支持文本对象的固定全宽功能
    - 目标对象：EntityText、EntitySiriusText
    - 新增 Target width 属性
    - 当值为 0 时，将像以前一样生成最佳字号； 
    - 若大于 0，则根据 Target width 值自动调整比例
- 新增) 支持本地 ZPL 图像转换服务
    - EntityImageZPL 对象
    - 原有：支持通过 Labelry 网络服务进行在线转换
    - 变更：支持通过外部 BinaryKits 库进行离线转换
    - 默认值：更改为使用 BinaryKits
    - 可通过 UI.Config.ZPLService 更改生成服务
- 新增) 支持整体尺寸转换
    - OriginalDimension：输出原始实体的尺寸
    - ModelDimension：支持修改实体在本地空间中的尺寸（宽度、高度、深度）
    - RealDimension：输出在累积应用了所有父实体 ModelMatrix（全部运算）后的世界（Real）空间中的尺寸（宽度、高度、深度）
- 新增）支持填充对齐
    - 在 HatchLine 对象中添加 Alignment 属性
    - None：无对齐
    - Center（默认）：居中对齐
    - Fit：均匀重新计算并调整间距
- 新增）支持 GS1 格式
    - 支持对 GS1 格式中的 &lt;GS&gt; 及 (,) 分隔符进行转换处理
- 修复）图像多视图纹理渲染
    - 目标对象：EntityImage、EntityImageText、EntityImageZPL
    - 修复了使用多视图时纹理无法渲染的问题
- 已修复) EntityUniformGroup
    - 限制可转换为统一组的对象的条件限制
    - 禁止添加包含控制对象以及 ITextConvertible、IHatch 的对象
- 已修复) 2D 扫描仪校准
    - 使用 RtcCorrection2D 时，支持最多 99x99 个校准点
- 已修复) RtcCalibrationLibrary 校准
    - 添加了在使用矩阵（MatrixPrimaryInternal）时自动对测量坐标进行逆变换处理的函数 
    - 用于旋转扫描仪时自动计算原始数据的功能
     
## v1.9.0 (2026.6.1)
- 新增) 支持导入 G-code
    - 文件扩展名：.gocde 或 .ngc 文件
- 修复) 改进文本转换器 TextConverters.Offset
    - 原版：将 Offset 的 ExtensionData 值用作转换后的文本
    - 变更：当 Offset 的 ExtensionData 值为 "Entity1|Value1|Entity2|Value2;..." 形式的扩展字符串时，支持 TextConverter 解析并使用相应的键值对
    - 新增) Remote 
        - 添加 text 命令
        - 命令格式：text|数量|Name1|Text1|Name2|Text2|...;
- 更新) Ophir StarLab v4.00 
- 修复) 支持创建外部实体
    - 请参考 editor_entity_custom 演示项目
     
## v1.8.6 (2026.5.14)
- 修复) 填充图案
    - 为线段填充图案提供 HatchFills 选项
    - 改进：当条形码对象使用轮廓（Outline）单元格类型时，填充效果能正确应用
- 新增) 背景棋盘格网格大小 
    - 支持通过 IView.CheckerSize 设置大小
- 修复) 撤销、重做 
    - 在 EditorControl 中使用键盘快捷键操作时也支持撤销
    - 提升了稳定性
     
## v1.8.5 (2026.5.8)
- 新增) 支持撤销、重做功能
    - IDocument 支持 ActUndo、ActRedo 方法
    - 仅在部分名为 IDocument.Act 的函数中有效
    - 可通过 Config.IsUnReDoEnable 禁用此功能 
    - 可通过 Config.UnReDoSize 更改历史记录条数（默认值：30）
- 新增) 条形码
    - 添加 Aztec 2D 条形码
    - 添加 PLESSEY 1D 条形码
    - 支持编辑像素尺寸（Dimension）
- 新增) CreateGrid 表单
    - 支持选择生成点、圆、十字线、网格图案
- 新增) Rtc6
    - 支持飞秒激光的脉冲峰值检测
- 修复) IRtcStepper
    - 初始化和等待函数支持异步处理
- 修复) DIO 表单
    - 修正了模拟输出值显示错误
- 修复) MultiBeam
    - 修改了光栅加工时的跳转（令牌交换）区间
    - 原设置：每行跳转（ListRasterLine）时进行令牌交换；
    - 修改：改为每像素（ListRasterPixel）跳转时进行令牌交换；
- 修复) semi ocr 字体；
    - 应用光栅（Raster）加工方式
 
## v1.8.1 (2026.4.22)
- 新增) 远程功能
    - 添加了 IRemote 接口，用于支持通过外部通信更改加工方案、查询和修改对象属性、启动、停止和重置标记加工命令，以及设置加工偏移量
    - 支持串行通信
    - 支持 TCP/IP 通信
    - 支持 WebSocket 通信
    - 支持 MQTT 通信
- 新增) 支持脚本数据转换
    - 支持通过外部 C# 脚本文件实时修改处理中的文本数据;```
    - 仅在 TextConverter.SimpleScript 模式下生效;```
    - 可在 Script 文件夹中使用用户编写的 C# 脚本;```
    - 在 IMarker 中新增 ScriptInstance 项
- 新增) SEMI OCR 字体
    - 添加 .dot 字体文件
    - 支持通过 SiriusText 对象使用点阵字体
- 修复) IDocument
    - 修复 FindByName 搜索错误
- 修复) MultiBeamControl
    - 修复按钮切换状态错误
     
## v1.7.1 (2026.4.16)
- 更新) RTC6 v1.24.0 包
    - 2026-3-31 发布版本
- 修复) IMarker
    - 支持异步处理（开源代码已变更）
    - 重构为使用任务代替线程，并采用继承实现方式
- 修复) IRtcMultiBeam
    - 已完成 RTC 之间的排他性同步控制验证;
    - 已完成 SiriusEditorControl 验证;
        - 支持 2 组不同的加工数据 + 2 种不同的笔组合;
    - 已完成 SiriusMultiEditorControl 验证;
        - 支持 1 组相同的加工数据 + 1 种不同的笔组合 
- 新增) LogControl
    - 添加日志消息过滤及搜索功能
- 修复) Shader
    - 修复了在控制台环境下视图（View）中对象无法渲染的问题
    - 支持针对多个视图对象分别管理 Shader
- 修复) 3D校正 
    - 对系数A、B、C进行16位和20位分辨率处理
    - 改进了使用Correction3DRtcForm进行数据操作的功能
- 修复) 修复内存泄漏问题
- 修复）修复了通过快照保存视图图像的错误
- 修复）优化了C#脚本的执行速度
 
## v1.6.1 (2026.4.9)
- 新增) ViewerControl 
    - 添加用户控件
    - 支持在查看器和编辑器中同时渲染同一文档
    - 取消文档与单个视图之间的1:1绑定限制   
    - 支持从外部创建和修改文档
- 修复) IRtc3D
    - 支持基于 RtcCalibrationLibrary 的增强型 3D 校准流程
        - 1. 光束倾斜校准：RtcCalibrationLibrary.BeamTiltCalibration
        - 2. 2D 场校正：RtcCalibrationLibrary.XyCalibration
        - 3. z=0 处的焦距校准：RtcCalibrationLibrary.FocusCalibrationAtZ0
        - 4. 系数 A、B、C 的焦距校准：RtcCalibrationLibrary.FocusCalibrationCoeffABC
        - 5. Z 体积的拉伸校准：RtcCalibrationLibrary.StretchCalibration
    - 删除 RtcCorrection3D：改用 RtcCalibrationLibrary 代替
    - 删除 KZScale：改用 RtcCalibrationLibrary 的 Focus 补偿功能代替
    - 删除 ZOffset：改用 MatrixStack 的 Translate Z 代替
- 新增) 添加 EntityPoint 对象
- 新增) 添加 EntityBarcode1D_V2 对象
    - 与2D条形码类似，支持多种单元格类型;
    - 可组合点、线、网格等;
- 新增) 支持以下对象的顶点列表的打开和保存功能;
    - EntityPoints;
    - EntityPolyline2D;
    - EntityPolyline3D;
    - OffsetControl 用户控件
    
## v1.5.4 (2026.4.2)
- fixed) 热修复
    - 修复在设计时创建 SiriusEditorControl 用户控件时发生的异常
    - 修复在设计时创建 SiriusMultiEditorControl 用户控件时发生的异常     
 
## v1.5.3 (2026.4.1)
- fixed) IDocument
    - 添加 IDocument 与 IView 之间的相互连接设置
- added) IPowerMeter
    - 分离为功率模式、能量模式，并添加 MeasureUnits
- fixed) Rtc6
    - IsActivateAutoDelays 属性更改时通知事件
    - IsActivateAutoDelays 属性更改时处理 EntityPen、EntityLayerPen 中项目的显示（Visible）
- fixed) EntityBarcode2D
    - 更改为在以 CellDot 类型加工时，通过 EntityPen 的 Raster 项目进行处理
- fixed) IRtcMultiBeam
    - 互斥令牌（Token）处理验证完成
- fixed) IRtcCorrection2D, IRtcCorrection3D
    - 支持使用扫描头中设置的内部矩阵（旋转等）进行 Raw 数据运算处理

## v1.5.2 (2026.3.27)
- added) 支持步进电机控制
    - 添加通过 RTC5, 6 的步进端子控制外部步进电机的功能
    - 添加 IRtcStepper 接口
    - 添加 StepperControl 用户控件 UI
    - 支持步进电机的绝对、相对坐标移动
- added) 支持串口通信
    - 添加通过 RTC5, 6 的 RS232 端子的通信功能
    - 添加 IRtcSerialComm 接口
    - 添加 SerialCommControl 用户控件 UI
    - 可在激光选项卡中监控收发数据（二进制）
    - 添加 OnSerialReceived 事件
- added) Fly Extension 改进
    - 改进 RTC6 专用的 Marking on the fly 扩展功能
    - 重构 IRtcMoFExtension 接口
    - 支持 3 轴组合（X, Y, Z 或旋转轴）
    - 支持 McBSP 通信
- fixed) 用户控件 UI 重构
    - OffsetControl
    - MarkerControl
    - ScannerControl 
    - LaserControl
- fixed) 矩阵栈
    - 删除 MatrixStack 的 BaseMatrix 
    - 通过使用 IRtc.CtlMatrix, ListMatrix 提供整合支持
- fixed) 功率计
    - 修复 CoherentPowerMax、GentecEO 设备中读取功率值的错误
- added) SiriusEditorControl    
    - 导入外部 .sirius3 文件并作为当前文档的图层添加
    - 在图层对象中使用的画笔颜色会显示在树形视图中
- added) 在编辑器菜单中添加数组粘贴功能
 
## v1.4.1 (2026.3.10)
- added) 通过 Web 服务器提供文档化
    - 可使用在线网站：https://spirallab.co.kr/sirius3/doc
    - 也可通过解压 sirius3\doc\sirius3_doc_版本.zip 并运行 'start_doc.bat' 批处理文件使用
- added) 使用鼠标编辑数值
    - 支持在 PropertyGrid 中按住鼠标右键左右拖动时增加或减少数值
- fixed) Rtc6
    - 修复在使用 ListLaserOn 时，输入外部激光源的 SYNC OUT 计算脉冲个数时，等待时间被错误处理为 10 倍的 Bug
- fixed) ListLaserOn(msec)
    - 改进为在时间结束后自动插入激光关闭（Laser Off Delay）时间
- fixed) 修改部分 IRtcJumpMode 接口
- fixed) EntityPen
    - 修复 Power, PowerPercentage, PowerMapCategory 值不显示的问题
- fixed) SiriusEditorControl
    - 默认支持使用全部 4 个页面（Page）
    - 禁用 WaferMap 及 Substratemap

## v1.4.0 (2026.3.3)
- added) 添加 .net9.0-windows, .net10.0-windows 开发环境
- added) 根据外部激光源同步信号输出脉冲个数
    - 通过 LASER 连接器的 DIGITAL IN1 输入外部同步信号
    - IRtc.ListLaserOn(等待时间, 脉冲个数, 脉冲个数结束) 
    - 可通过 EntityPen 画笔的 PixelPulses, IsPixelPulsesExit 值进行设置
        - 0: 与之前相同，在像素时间内输出 LASERON
        - 1~65535: 在像素时间内等待外部同步信号达到脉冲数并输出 LASERON
        - 使用 IsPixelPulsesExit 时，若外部同步信号个数达到 PixelPulses 设置值，则立即结束并移动到下一个列表命令
- added) (实验性) IRtcMultiBeam 接口
    - 使用一个激光源 + 2 个 RTC + 2 个 AOM RF 驱动器的多束光系统
    - Rtc6MultiBeam 
- added) EntityPoints 
    - 支持通过 Sort 函数进行最短路径优化 
- added) IRtcIO 接口
- fixed) EntityWaitDataExt16Cond, EntityWaitDataExt16EdgeCond, EntityWriteDataExt16, EntityWriteDataExt16Cond
    - 将 bitmask 作为 ushort 类型处理，而非字符串
- fixed) SiriusEditorControl 控件
    - 修复在设计时添加时发生的异常
    - 修复控件在后台创建时 OpenGL 未初始化导致的异常
    - 删除控件在 Load 时强制 Document ActNew 的代码
- fixed) 许可
    - 超过最大允许实例个数或没有选项的情况
    - 之前：不可使用
    - 更改：激活 30 分钟评估模式

## v1.3.2 (2026.2.20)
- fixed) 支持 Automatic Laser Control 的扩展模式
    - 可使用 Actual Velocity + Encoder + SCANAhead + Inverse Speed Correction + Backward Transformation + SDC + SkyWriting 信号组合
    - 支持在 EntityLayerPen 属性的 PoD 列表中设置扩展模式组合
    - 添加了 EntityPoD
- fixed) EntityPen 
    - SDC 功能的 Spot distance 值设置现在由 SpotDistanceSCANa 支持。
- added) IRtcMoF 
    - 支持编码器信号异常通知事件：IRtcMoF.OnEncoderSignalError 事件
    - 支持超出虚拟区域时的通知事件：IRtcMoF.OnOutOfVirtualImageField
    - 支持编码器信号滤波设置（RTC6 专用）：使用 CtlMoFEncoderFilter 函数，在噪声较多时使用信号算术平均，或支持 4MHz 以上的高速
    - 查询编码器值时，可以分别查询绝对位置和相对位置。
    - 添加了 OnEncoderChanged 事件参数
- fixed) 将 IRtcWaitID 更名为 IRtcInterrupt

## v1.3.1 (2026.2.9)
- added) IRtcSCANAhead 接口
    - 在 EntityPen 中添加 SCANAhead 项目（Corner, End, Acc Scale）
    - 支持设置 Position(或 Trajectory) Acknowledge Limit 值（初始值：全位置范围的 0.28%）
    - 在 RTC6 + SCANAhead 使用时，表示 Trajectory ACK Limit 值
- added) IRtcWaitID 接口

## v1.3.0 (2026.2.5)
- added) EntityPolyline2D, EntityPolyline3D 
    - 添加顶点列表编辑器
- added) SiriusMultiEditorControl 控件
    - 支持单个文档 + 多设备处理
- added) EntityLayerPen 
    - 添加用于编辑画笔值和提供帮助的 UI
- replace) 删除外部 gnuplot 程序，改为内置 plot
- fixed) 解决 scanner jog 输出问题
- added) 支持导入 .dwg
    - 用户需另行安装 ODA converter (https://www.opendesign.com/guestfiles/oda_file_converter) 
    - 改进为在处理 .dwg, .dxf 文件时可以额外使用 ODA converter
- license) 许可政策变更
    - 3D 选项被删除，改为默认提供
    - syncAXIS 实例更改为可选项

## v1.2.7 (2026.1.26)
- added) 在 EntityLayerPen 中添加 Variable Delays 功能
    - Variable polygon delay: 根据弯折角度设置可变的折线延迟时间（默认：启用）
    - Variable jump delay: 根据跳跃距离设置可变的跳跃延迟时间
- fixed) 修复在 RTC6 中使用 Skywriting 时 LaserOnShift 值设置过小的问题
- fixed) Config.IsMarkArcsIntoLines 
    - True: 圆弧(EntityArc)和多段线(EntityPolyline2D)的曲线加工时，分解为直线(ListMarkTo)处理
    - False: 圆弧(EntityArc)和多段线(EntityPolyline2D)的曲线加工时，作为圆弧(ListArcTo)处理
- fixed) 修复提取 Contour 时 IsClosed 值计算错误的问题
- fixed) 支持编辑 Config.EntityPenColors, Config.LayerPenColors
- fixed) 模拟对象 ActRemove 失败的问题
 
## v1.2.6 (2026.1.21)
- added) 添加椭圆(ellipse)对象
- added) EntityLine, EntityArc, EntityPolyline2D
    - 添加 RampFactor 属性以支持 Automatic laser control (defined vector)
- added) 添加 IHatch.HatchRepeats 重复次数
- fixed) 修复 EntityPen, EntityLayerPen 值输出错误的问题
- fixed) PowerMap CtlCompensate 中测量值超出范围的情况  
    - 之前：对左、右范围进行重新测量的方式 
    - 更改：将测量数据立即更新到该区间 
- fixed) IMarker.Preview
    - 之前：显示包裹所选对象的整体矩形
    - 更改：显示所选对象的每个单独外框矩形

## v1.2.5 (2026.1.15)
- added) 添加了 ClipHelper 
- added) 按住空格键选择时，激活下层对象选择模式
- fixed) 改进 IHitTestable 射线检测
   - Config.RayHitTestPixelSize: 使用动态距离值提高检测功能
- fixed) IMarker
   - 在 MarkTargets.Selected 的情况下，改进为递归处理子对象
- updated) 更新 zxing v0.16.11
- updated) 更新 clipper2 v.2.0.0
	 
## v1.2.4 (2026.1.7)
- added) 添加快捷键
   - CTRL + R: 切换是否渲染
   - CTRL + M: 切换是否标记
   - 切换渲染、标记时，树节点的字体（或颜色）会正常更改
- added) 添加 IRtcFreeVariable.OnFreeVariableChanged 事件
   - 当 FreeVariable 值更改时发生 
- added) 添加 Config.GridCloudInterval
   - 在调用 IDocument.ActGridCloud 函数时使用
- fixed) 提高了 Gerber 文件解析器的性能（缩短时间）
- fixed) 在选择对象(hittest)时提供更详细的信息
   - IDocument.SubHitEntities 在选择组结构时返回下层对象
- added) 添加新的 ActHitTest 函数
- fixed) 修复使用 ActUngroup 时因错误的空树节点导致的异常

## v1.0.1 (2025.12.22)
- added) .chm 帮助文件
- added) ActExpand 函数
   - 支持根据距离进行路径扩大(或缩小)
- added) 支持 Gentec-EO 功率计设备
- updated) PowerMeterOphir 设备更新为使用 StarLab v3.93 版本
- fixed) hatch joints 枚举
- fixed) 修复 IDocument.FindByLayerUsedPenColors
- fixed) 处理 Marker.EntityWork 时添加日志消息
 
## v0.9.3 (2025.12.5)
- added) 缩放至适应 
   - 双击树节点时应用
   - 打开文件时应用
- added) 添加新的 TextConverters.Offset
   - 使用 Offset.ExtensionData 值
- fixed) Gerber 文件
   - added) 通过 UI.Config.IsGerberWithUniformGroup 支持高速渲染
   - fixed) 修复使用 UI.Config.IsGerberTessellation 时的细分（Tessellation）问题 
- renamed) 将 scanner pen 更名为 entity pen

## 更多历史信息请参考 HISTORY.md 文件
