/*
 * 
 *                                                            ,--,      ,--,                              
 *             ,-.----.                                     ,---.'|   ,---.'|                              
 *   .--.--.   \    /  \     ,---,,-.----.      ,---,       |   | :   |   | :      ,---,           ,---,.  
 *  /  /    '. |   :    \ ,`--.' |\    /  \    '  .' \      :   : |   :   : |     '  .' \        ,'  .'  \ 
 * |  :  /`. / |   |  .\ :|   :  :;   :    \  /  ;    '.    |   ' :   |   ' :    /  ;    '.    ,---.' .' | 
 * ;  |  |--`  .   :  |: |:   |  '|   | .\ : :  :       \   ;   ; '   ;   ; '   :  :       \   |   |  |: | 
 * |  :  ;_    |   |   \ :|   :  |.   : |: | :  |   /\   \  '   | |__ '   | |__ :  |   /\   \  :   :  :  / 
 *  \  \    `. |   : .   /'   '  ;|   |  \ : |  :  ' ;.   : |   | :.'||   | :.'||  :  ' ;.   : :   |    ;  
 *   `----.   \;   | |`-' |   |  ||   : .  / |  |  ;/  \   \'   :    ;'   :    ;|  |  ;/  \   \|   :     \ 
 *   __ \  \  ||   | ;    '   :  ;;   | |  \ '  :  | \  \ ,'|   |  ./ |   |  ./ '  :  | \  \ ,'|   |   . | 
 *  /  /`--'  /:   ' |    |   |  '|   | ;\  \|  |  '  '--'  ;   : ;   ;   : ;   |  |  '  '--'  '   :  '; | 
 * '--'.     / :   : :    '   :  |:   ' | \.'|  :  :        |   ,/    |   ,/    |  :  :        |   |  | ;  
 *   `--'---'  |   | :    ;   |.' :   : :-'  |  | ,'        '---'     '---'     |  | ,'        |   :   /   
 *             `---'.|    '---'   |   |.'    `--''                              `--''          |   | ,'    
 *               `---`            `---'                                                        `----'   
 * 
 * 2026 Copyright to (c)SpiralLAB. All rights reserved.
 * Description : MyMarkerRtcFast 
 * Author : hong chan, choi / hcchoi@spirallab.co.kr (http://spirallab.co.kr)
 */

using System;
using System.Drawing;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Threading;
using System.ComponentModel;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Collections;
using SpiralLab.Sirius3.Scanner;
using SpiralLab.Sirius3.Laser;
using SpiralLab.Sirius3.Mathematics;
using SpiralLab.Sirius3.PowerMeter;
using SpiralLab.Sirius3.Scanner.Rtc;
using SpiralLab.Sirius3.Scanner.Rtc.SyncAxis;
using SpiralLab.Sirius3.Entity;
using SpiralLab.Sirius3.Document;
using SpiralLab.Sirius3.View;
using Microsoft.Extensions.Logging;
using SpiralLab.Sirius3.Converter;
using SpiralLab.Sirius3.Localization;
using SpiralLab.Sirius3.Marker;
using SpiralLab.Sirius3;

#if OPENTK3
using OpenTK;
using DVec2 = OpenTK.Vector2d;
using DVec3 = OpenTK.Vector3d;
using DVec4 = OpenTK.Vector4d;
using DMat3 = OpenTK.Matrix3d;
using DMat4 = OpenTK.Matrix4d;
#elif OPENTK4
using OpenTK.Mathematics;
using DVec2 = OpenTK.Mathematics.Vector2d;
using DVec3 = OpenTK.Mathematics.Vector3d;
using DVec4 = OpenTK.Mathematics.Vector4d;
using DMat3 = OpenTK.Mathematics.Matrix3d;
using DMat4 = OpenTK.Mathematics.Matrix4d;
#endif

namespace Demos
{
    /// <summary>
    /// MyMarkerRtcFast
    /// <para>
    /// Process whole <see cref="EntityLayer">EntityLayer</see>s between a pair of <see cref="IRtc.ListBegin">IRtc.ListBegin</see> and <see cref="IRtc.ListEnd">IRtc.ListEnd</see> to performance up (also. reduce total mark time). <br/>
    /// </para>
    /// <para>
    /// <see cref="IRtc.ListBegin">IRtc.ListBegin</see>과 <see cref="IRtc.ListEnd">IRtc.ListEnd</see> 사이의 전체 <see cref="EntityLayer">EntityLayer</see>를 처리하여 성능을 높입니다(총 마킹 시간도 단축). <br/>
    /// </para>
    /// <para>
    /// 在一对 <see cref="IRtc.ListBegin">IRtc.ListBegin</see> 和 <see cref="IRtc.ListEnd">IRtc.ListEnd</see> 之间处理整个 <see cref="EntityLayer">EntityLayer</see> 以提高性能（同时减少总标记时间）。<br/>
    /// </para>
    /// </summary>
    /// <remarks>
    /// Used with RTC4,4e,5,6,6e <br/>
    /// </remarks>
    public class MyMarkerRtcFast
        : MarkerBase
    {
        /// <summary>
        /// Mark targets
        /// <para>마크 대상<br/></para>
        /// <para>标记目标<br/></para>
        /// </summary>
        public enum MarkTargets
        {
            /// <summary>
            /// All entities
            /// <para>모든 엔티티<br/></para>
            /// <para>所有实体<br/></para>
            /// </summary>
            All = 0,
            /// <summary>
            /// Selected entities
            /// <para>선택된 엔티티<br/></para>
            /// <para>选定的实体<br/></para>
            /// </summary>
            Selected = 1,
        }

        /// <summary>
        /// Target entities to mark
        /// <para>마크할 대상 엔티티<br/></para>
        /// <para>要标记的目标实体<br/></para>
        /// </summary>
        /// <remarks>
        /// Default: <see cref="MarkTargets.All">MarkTargets.All</see> <br/>
        /// </remarks>
        [Browsable(true)]
        [ReadOnly(false)]
        [LocalizedCategory("Data")]
        [LocalizedDisplayName("MarkTarget")]
        [LocalizedDescription("MarkTarget")]
        public virtual MarkTargets MarkTarget
        {
            get { return markTarget; }
            set
            {
                if (this.IsBusy)
                {
                    Logger.Log(LogLevel.Error, $"marker [{Index}]: fail to set mark target during busy");
                    return;
                }
                var oldMarkTarget = markTarget;
                markTarget = value;
                if (markTarget != oldMarkTarget)
                    this.NotifyPropertyChanged();
            }
        }
        /// <summary>
        /// Internal <c>MarkTargets</c>
        /// <para>내부 <c>MarkTargets</c><br/></para>
        /// <para>内部 <c>MarkTargets</c><br/></para>
        /// </summary>
        protected MarkTargets markTarget = MarkTargets.All;

        /// <summary>
        /// <c>ListBufferTypes</c>
        /// <para><c>ListBufferTypes</c></para>
        /// <para><c>ListBufferTypes</c></para>
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [LocalizedCategory("Data")]
        [LocalizedDisplayName("ListBufferType")]
        [LocalizedDescription("ListBufferType")]
        public virtual ListBufferTypes ListBufferType
        {
            get { return listType; }
            set
            {
                if (this.IsBusy)
                {
                    Logger.Log(LogLevel.Error, $"marker [{Index}]: fail to set list type during busy");
                    return;
                }
                listType = value;
            }
        }
        private ListBufferTypes listType;

        /// <summary>
        /// Array of <see cref="MeasurementSession">MeasurementSession</see> 
        /// <para><see cref="MeasurementSession">MeasurementSession</see> 배열<br/></para>
        /// <para><see cref="MeasurementSession">MeasurementSession</see> 数组<br/></para>
        /// </summary>
        /// <remarks>
        /// Session = <see cref="EntityMeasurementBegin">EntityMeasurementBegin</see> + <see cref="EntityMeasurementEnd">EntityMeasurementEnd</see> <br/>
        /// Valid when <see cref="EntityMeasurementBegin">EntityMeasurementBegin</see> has executed. <br/>
        /// </remarks>
        [Browsable(true)]
        [ReadOnly(false)]
        [LocalizedCategory("Measurement")]
        [LocalizedDisplayName("Session")]
        [LocalizedDescription("Session")]
        [JsonIgnore]
        public virtual MeasurementSession[] Session
        {
            get { return sessionQueue.ToArray(); }
        }
        /// <summary>
        /// Queue for <see cref="MeasurementSession">MeasurementSession</see> 
        /// <para><see cref="MeasurementSession">MeasurementSession</see> 큐<br/></para>
        /// <para><see cref="MeasurementSession">MeasurementSession</see> 队列<br/></para>
        /// </summary>
        protected ConcurrentQueue<MeasurementSession> sessionQueue = new ConcurrentQueue<MeasurementSession>();
        /// <summary>
        /// Current (or last measurement session)
        /// <para>현재(또는 마지막) 측정 세션<br/></para>
        /// <para>当前（或上一个）测量会话<br/></para>
        /// </summary>
        /// <remarks>
        /// Valid when a pair of <see cref="EntityMeasurementBegin">EntityMeasurementBegin</see> and <see cref="EntityMeasurementEnd">EntityMeasurementEnd</see> has executed. <br/>
        /// Only single <see cref="MeasurementSession">MeasurementSession</see> can be exist within a <see cref="EntityLayer">EntityLayer</see>. <br/>
        /// </remarks>
        internal MeasurementSession CurrentSession { get; set; }

        /// <summary>
        /// Is plot measurement session to graph or not
        /// <para>측정 세션을 그래프로 그릴지 여부<br/></para>
        /// <para>是否将测量会话绘制到图表<br/></para>
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [LocalizedCategory("Measurement")]
        [LocalizedDisplayName("IsMeasurementPlot")]
        [LocalizedDescription("IsMeasurementPlot")]
        public virtual bool IsMeasurementPlot
        {
            get { return isMeasurementPlot; }
            set
            {
                isMeasurementPlot = value;
            }
        }
        /// <summary>
        /// Is plot measurement session to graph or not
        /// <para>측정 세션을 그래프로 그릴지 여부<br/></para>
        /// <para>是否将测量会话绘制到图表<br/></para>
        /// </summary>
        protected bool isMeasurementPlot;

        /// <summary>
        /// Max measurement time
        /// <para>최대 측정 시간<br/></para>
        /// <para>最大测量时间<br/></para>
        /// </summary>
        /// <remarks>
        /// RTC Max measurement time
        /// </remarks>
        [Browsable(true)]
        [ReadOnly(false)]
        [LocalizedCategory("Measurement")]
        [LocalizedDisplayName("MaxMeasurementTime")]
        [LocalizedDescription("MaxMeasurementTime")]
        [TypeConverter(typeof(DoubleTypeConverter))]
        public virtual double MaxMeasurementTime
        {
            get
            {
                if (null == CurrentSession || null == CurrentSession.MeasurementBegin || 0 == CurrentSession.MeasurementBegin.SamplingFrequency)
                    return 0;
                //sec
                double period = 1.0 / CurrentSession.MeasurementBegin.SamplingFrequency;
                if (Scanner is Rtc4 || Scanner is Rtc4Ethernet)
                    return 32768 * period;
                else if (Scanner is Rtc5)
                    return Math.Pow(2, 20) * period;
                else if (Scanner is Rtc6)
                    return Math.Pow(2, 24) * period;

                return 0;
            }
        }
        /// <summary>
        /// Check scanner temperature when start of mark
        /// <para>마크 시작 시 스캐너 온도 확인<br/></para>
        /// <para>标记开始时检查扫描仪温度<br/></para>
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [LocalizedCategory("Check")]
        [LocalizedDisplayName("IsCheckTempOk")]
        [LocalizedDescription("IsCheckTempOk")]
        public virtual bool IsCheckTempOk { get; set; }
        /// <summary>
        /// Check scanner power supply when start of mark
        /// <para>마크 시작 시 스캐너 전원 공급 확인<br/></para>
        /// <para>标记开始时检查扫描仪电源<br/></para>
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [LocalizedCategory("Check")]
        [LocalizedDisplayName("IsCheckPowerOk")]
        [LocalizedDescription("IsCheckPowerOk")]
        public virtual bool IsCheckPowerOk { get; set; }
        /// <summary>
        /// Check scanner position acknowledge when start of mark
        /// <para>마크 시작 시 스캐너 위치 확인<br/></para>
        /// <para>标记开始时检查扫描仪位置确认<br/></para>
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [LocalizedCategory("Check")]
        [LocalizedDisplayName("IsCheckPositionAck")]
        [LocalizedDescription("IsCheckPositionAck")]
        public virtual bool IsCheckPositionAck { get; set; }


        /// <summary>
        /// List of layers to mark
        /// <para>마킹할 레이어 목록<br/></para>
        /// <para>要标记的图层列表<br/></para>
        /// </summary>
        protected List<EntityLayer> layers;

        /// <summary>
        /// Target <c>IRtc</c> instance
        /// </summary>
        protected IRtc Rtc { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MyMarkerRtcFast"/> class.
        /// <para><see cref="MyMarkerRtcFast"/> 클래스의 새 인스턴스를 초기화합니다.<br/></para>
        /// <para>初始化 <see cref="MyMarkerRtcFast"/> 类的新实例。<br/></para>
        /// <code>
        /// </code>
        /// </summary>
        public MyMarkerRtcFast()
            : base()
        {
            listType = ListBufferTypes.Auto;
            isMeasurementPlot = true;
            markTarget = MarkTargets.All;

            IsCheckTempOk = false;
            IsCheckPowerOk = false;
            IsCheckPositionAck = false;

            layers = new List<EntityLayer>();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="MyMarkerRtcFast"/> class with the specified index and name.
        /// <para>지정된 인덱스와 이름으로 <see cref="MyMarkerRtcFast"/> 클래스의 새 인스턴스를 초기화합니다.<br/></para>
        /// <para>使用指定的索引和名称初始化 <see cref="MyMarkerRtcFast"/> 类的新实例。<br/></para>
        /// <code>
        /// </code>
        /// </summary>
        /// <param name="index">The index of the marker. <para>마커의 인덱스입니다.</para> <para>标记的索引。</para></param>
        /// <param name="name">The name of the marker. <para>마커의 이름입니다.</para> <para>标记的名称。</para></param>
        public MyMarkerRtcFast(int index, string name)
            : this()
        {
            Index = index;
            Name = name;
        }

        /// <inheritdoc/>  
        protected override void OnDisposeManaged()
        {
            // myResource?.Dispose();
        }
        /// <inheritdoc/>  
        protected override async Task OnDisposeManagedAsync()
        {
            // await myResource.StopAsync();
        }

        /// <inheritdoc/>
        public override bool Initialize()
        {
            Logger.Log(LogLevel.Information, $"marker [{Index}]: initialized");
            return true;
        }
        /// <inheritdoc/>
        public override bool Ready(IDocument document, IView view, IScanner scanner, ILaser laser, IPowerMeter powerMeter) //, IRemote remote)
        {
            if (this.IsBusy)
            {
                Logger.Log(LogLevel.Error, $"marker [{Index}]: fail to ready. marker status is busy");
                return false;
            }

            base.Document = document;
            base.View = view;
            base.Scanner = scanner;
            Rtc = scanner as IRtc;
            base.Laser = laser;
            base.PowerMeter = powerMeter;
            //base.Remote = remote;

            if (scanner is IRtcSyncAxis rtcSyncAxis)
            {
                this.Scanner = null;
                Logger.Log(LogLevel.Error, $"marker [{Index}]: assigned invalid RTC instance");
                return false;
            }
            document?.ActRegen();
            Logger.Log(LogLevel.Debug, $"marker [{Index}]: ready with doc= {document?.FileName}, view= {view?.Name}, rtc= {Rtc?.Name}, laser= {laser?.Name}, pm= {powerMeter?.Name}");//, remote= {remote?.Name}");
            return true;
        }
        /// <inheritdoc/>
        public override bool Ready(IDocument document)
        {
            if (this.IsBusy)
            {
                Logger.Log(LogLevel.Error, $"marker [{Index}]: fail to ready. marker status is busy");
                return false;
            }

            base.Document = document;
            document?.ActRegen();
            Logger.Log(LogLevel.Debug, $"marker [{Index}]: ready with doc= {document?.FileName}");
            return true;
        }

        /// <inheritdoc/>
        protected override async Task<bool> OnStarting(DocumentPages page = DocumentPages.Page1)
        {
            if (IsCheckTempOk && !Rtc.CtlGetStatus(RtcStatus.TempOK))
            {
                Logger.Log(LogLevel.Error, $"marker: {this.Name} scanner temp is no ok");
                return false;
            }
            if (IsCheckPowerOk && !Rtc.CtlGetStatus(RtcStatus.PowerOK))
            {
                Logger.Log(LogLevel.Error, $"marker: {this.Name} scanner power is not ok !");
                return false;
            }
            if (IsCheckPositionAck && !Rtc.CtlGetStatus(RtcStatus.PositionAckOK))
            {
                Logger.Log(LogLevel.Error, $"marker: {this.Name} scanner position is not acked");
                return false;
            }

            // Reset measurement session
            this.CurrentSession = null;

            // Clear measurement session queue
            while (sessionQueue.Count > 0)
                sessionQueue.TryDequeue(out var dummy);

            // Shallow copy 
            layers.Clear();
            switch (page)
            {
                case DocumentPages.Page1:
                case DocumentPages.Page2:
                case DocumentPages.Page3:
                case DocumentPages.Page4:
                    foreach (var child in Document.DocumentData.Pages[(int)page].Layers.Children)
                    {
                        var layer = child as EntityLayer;
                        layers.Add(layer);
                    }
                    break;
                default:
                    throw new Exception("Invalid target page !");
            }

            WorkingSet.Reset();
            WorkingSet.DocumentPage = page;
            WorkingSet.Page = Document.DocumentData.Pages[(int)page];
            WorkingSet.PageIndex = (int)page;

            Logger.Log(LogLevel.Warning, $"marker [{Index}]: trying to start mark with target= {MarkTarget}, offset(s)= {this.Offsets.Length}");
            markerTask = Task.Run(() => this.MarkerThreadLayers());
            return await markerTask;
        }
        /// <inheritdoc/>
        protected override async Task<bool> OnPreviewing()
        {
            if (IsCheckTempOk && !Rtc.CtlGetStatus(RtcStatus.TempOK))
            {
                Logger.Log(LogLevel.Error, $"marker: {this.Name} scanner temp is no ok");
                return false;
            }
            if (IsCheckPowerOk && !Rtc.CtlGetStatus(RtcStatus.PowerOK))
            {
                Logger.Log(LogLevel.Error, $"marker: {this.Name} scanner power is not ok !");
                return false;
            }
            if (IsCheckPositionAck && !Rtc.CtlGetStatus(RtcStatus.PositionAckOK))
            {
                Logger.Log(LogLevel.Error, $"marker: {this.Name} scanner position is not acked");
                return false;
            }
            if (null == Document.Selected || 0 == Document.Selected.Length)
            {
                Logger.Log(LogLevel.Error, $"marker [{Index}]: select target entity at first to preview");
                return false;
            }
            var laserGuideControl = Laser as ILaserGuideControl;
            if (null == laserGuideControl)
            {
                Logger.Log(LogLevel.Error, $"marker [{Index}]: laser is not supported guide control");
                return false;
            }

            if (null != markerTask && !markerTask.IsCompleted)
            {
                if (!markerTask.Wait(500))
                {
                    Logger.Log(LogLevel.Error, $"marker [{Index}]: previous works has not finished yet");
                    return false;
                }
            }

            if (null == Offsets || 0 == Offsets.Length)
                this.Offsets = new Offset[1] { Offset.Zero };

            // Shallow copy for cross-thread issue
            layers.Clear();
            foreach (var child in Document.ActivePage.Layers.Children)
            {
                var layer = child as EntityLayer;
                layers.Add(layer);
            }

            Logger.Log(LogLevel.Warning, $"marker [{Index}]: trying to start preview mark");
            markerTask = Task.Run(() => this.MarkerThreadPreview());
            return await markerTask;
        }
        /// <inheritdoc/>
        protected override async Task<bool> OnStopping()
        {
            bool success = true;


            this.isInternalBusy = false;
            return success;
        }

        /// <inheritdoc/>
        public override bool Reset()
        {
            if (null == Scanner || null == Laser)
                return false;
            bool success = true;
            success &= Rtc.CtlReset();
            success &= Laser.CtlReset();

            return success;
        }
        /// <summary>
        /// Marks each <see cref="EntityLayer"/>.
        /// <para>각 <see cref="EntityLayer"/>를 마킹합니다.<br/></para>
        /// <para>标记每个 <see cref="EntityLayer"/>。<br/></para>
        /// <code>
        /// </code>
        /// </summary>
        /// <remarks>
        /// Helpful current working sets are <see cref="IWorkingSet.OffsetIndex">OffsetIndex</see>, <see cref="IWorkingSet.Offset">Offset</see>, <see cref="IWorkingSet.LayerIndex">LayerIndex</see>, <see cref="IWorkingSet.Layer">Layer</see>. <br/>
        /// Consider as its working within async threads. <br/>
        /// </remarks> 
        /// <param name="offsetIndex">Current index of offset (0,1,2,...)</param>
        /// <param name="offset">Current <see cref="SpiralLab.Sirius3.Mathematics.Offset">Offset</see></param>
        /// <param name="layerIndex">Current layer of offset (0,1,2,...)</param>
        /// <param name="layer">Current <see cref="EntityLayer">EntityLayer</see></param>
        /// <returns><c>true</c> if the operation was successful; otherwise, <c>false</c>.</returns>
        protected virtual bool LayerWork(int offsetIndex, Offset offset, int layerIndex, EntityLayer layer)
        {
            bool success = true;
            WorkingSet.LayerIndex = layerIndex;
            WorkingSet.Layer = layer;
            for (int i = 0; i < layer.Repeats; i++)
            {
                for (int j = 0; j < layer.Children.Count(); j++)
                {
                    var entity = layer.Children.ElementAt(j);
                    WorkingSet.EntityIndex = j;
                    WorkingSet.Entity = entity;
                    if (entity is IMarkerable markerable)
                    {
                        if (!markerable.IsAllowMark)
                            continue;
                    }
                    else
                        continue;

                    switch (MarkTarget)
                    {
                        case MarkTargets.All:
                            success &= EntityWork(offsetIndex, offset, layerIndex, layer, j, entity);
                            break;
                        case MarkTargets.Selected:
                            if (entity.IsSelected)
                                success &= EntityWork(offsetIndex, offset, layerIndex, layer, j, entity);
                            else if (entity is IHasChildren<IEntity> hasChildren)
                                success &= ChildrenWorkIfSelectedRecursively(offsetIndex, offset, layerIndex, layer, j, hasChildren);
                            break;
                    }
                    if (!success)
                        break;
                }
                if (!success)
                    break;
            }
            return success;
        }
        bool ChildrenWorkIfSelectedRecursively(int offsetIndex, Offset offset, int layerIndex, EntityLayer layer, int entityIndex, IHasChildren<IEntity> hasChildren)
        {
            Debug.Assert(null != hasChildren);
            bool success = true;
            foreach (var entity in hasChildren.Children)
            {
                if (entity.IsSelected)
                    success &= EntityWork(offsetIndex, offset, layerIndex, layer, entityIndex, entity);
                else if (entity is IHasChildren<IEntity> hasChildren2)
                    success &= ChildrenWorkIfSelectedRecursively(offsetIndex, offset, layerIndex, layer, entityIndex, hasChildren2);
            }
            return success;
        }
        /// <summary>
        /// Marks each <see cref="IEntity"/>.
        /// <para>각 <see cref="IEntity"/>를 마킹합니다.<br/></para>
        /// <para>标记每个 <see cref="IEntity"/>。<br/></para>
        /// <code>
        /// </code>
        /// </summary>
        /// <remarks>
        /// Helpful current working sets are <see cref="IWorkingSet.OffsetIndex">OffsetIndex</see>, <see cref="IWorkingSet.Offset">Offset</see>, <see cref="IWorkingSet.LayerIndex">LayerIndex</see>, <see cref="IWorkingSet.Layer">Layer</see>. <br/>
        /// Consider as its working within async threads. <br/>
        /// </remarks> 
        /// <param name="offsetIndex">Current index of offset (0,1,2,...)</param>
        /// <param name="offset">Current <see cref="SpiralLab.Sirius3.Mathematics.Offset">Offset</see></param>
        /// <param name="layerIndex">Current index of layer (0,1,2,...)</param>
        /// <param name="layer">Current <see cref="EntityLayer">EntityLayer</see></param>
        /// <param name="entityIndex">Current index of entity (0,1,2,...)</param>
        /// <param name="entity">Current <see cref="IEntity">IEntity</see></param>
        /// <returns><c>true</c> if the operation was successful; otherwise, <c>false</c>.</returns>
        protected virtual bool EntityWork(int offsetIndex, Offset offset, int layerIndex, EntityLayer layer, int entityIndex, IEntity entity)
        {
            bool success = true;
            success &= NotifyBeforeEntity(entity);
            if (!success)
            {
                Logger.Log(LogLevel.Error, $"marker [{Index}]: fail to mark entity at {entity.ToString()} before event handler");
                return success;
            }
            if (entity is IMarkerable markerable)
            {
                success &= markerable.Mark(this);
            }
            if (!success)
            {
                Logger.Log(LogLevel.Error, $"marker [{Index}]: fail to mark entity at {entity.ToString()}");
                return success;
            }
            success &= NotifyAfterEntity(entity);
            if (!success)
            {
                Logger.Log(LogLevel.Error, $"marker [{Index}]: fail to mark entity at {entity.ToString()} after event handler");
                return success;
            }
            return success;
        }

        /// <summary>
        /// Implements the marker thread for processing layers.
        /// <para>레이어 처리를 위한 마커 스레드를 구현합니다.<br/></para>
        /// <para>实现处理图层的标记器线程。<br/></para>
        /// <code>
        /// </code>
        /// </summary>
        /// <remarks>        
        /// Move offset1 and Mark layers -> Move offset2 and Mark layers , ... <br/>
        /// </remarks>
        protected virtual bool MarkerThreadLayers()
        {
            var rtc = this.Rtc;
            var laser = this.Laser;
            var document = this.Document;
            var rtc3D = rtc as IRtc3D;
            var rtc2ndHead = rtc as IRtc2ndHead;
            var rtcExtension = rtc as IRtcExtension;
            var rtcAlc = rtc as IRtcAutoLaserControl;
            var rtcMoF = rtc as IRtcMoF;
            var rtcSyncAxis = rtc as IRtcSyncAxis;
            Debug.Assert(rtc != null);
            Debug.Assert(laser != null);
            Debug.Assert(document != null);
            Debug.Assert(null == rtcSyncAxis);
            this.isInternalBusy = true;

            this.NotifyStarted();
            WorkingSet.StartTime = WorkingSet.EndTime = DateTime.Now;
            bool success = true;
            var oldMatrixStack = (IMatrixStack<DMat4>)rtc.MatrixStack.Clone();
            if (null != rtcMoF && rtc.IsMoF)
            {
                rtcMoF.CtlMoFOverflowClear();
                //rtcMoF.MofAngularCenter = DVec2.Zero;
            }

            try
            {
                success &= rtc.ListBegin(ListBufferType);
                if (!success)
                    return false;
                success &= laser.ListBegin();
                if (!success)
                    return false;
                for (int offsetIndex = 0; offsetIndex < Offsets.Length; offsetIndex++)
                {
                    WorkingSet.Offset = Offsets[offsetIndex];
                    WorkingSet.OffsetIndex = offsetIndex;
                    rtc.MatrixStack.Push(Offsets[offsetIndex].ToMatrix);
                    Logger.Log(LogLevel.Debug, $"marker [{Index}]: offset index= {offsetIndex}, xyzt= {Offsets[offsetIndex].ToString()}");
                    for (int layerIndex = 0; layerIndex < layers.Count; layerIndex++)
                    {
                        var layer = layers[layerIndex];
                        if (!layer.IsAllowMark)
                            continue;
                        success &= NotifyBeforeLayer(layer);
                        if (!success)
                        {
                            Logger.Log(LogLevel.Error, $"marker [{Index}]: fail to mark layer at before event handler");
                            break;
                        }
                        success &= LayerWork(offsetIndex, Offsets[offsetIndex], layerIndex, layer);
                        if (!success)
                            break;
                        success &= NotifyAfterLayer(layer);
                        if (!success)
                        {
                            Logger.Log(LogLevel.Error, $"marker [{Index}]: fail to mark layer at after event handler");
                            break;
                        }
                    }
                    // Pop offset matrix
                    rtc.MatrixStack.Pop();
                    if (!success)
                        break;
                }

                if (success) //!rtc.CtlGetStatus(RtcStatus.Aborted))
                {
                    success &= laser.ListEnd();
                    success &= rtc.ListEnd();
                    if (success) //!rtc.CtlGetStatus(RtcStatus.Aborted))
                        success &= rtc.ListExecute(true);
                    if (success)
                    {
                        if (null != CurrentSession && !CurrentSession.IsEmpty)
                        {
                            if (CurrentSession.Save(this.Scanner as IRtcMeasurement))
                            {
                                sessionQueue.Enqueue(CurrentSession);
                            }
                        }
                    }
                }

                if (null != rtcMoF)
                {
                    if (rtc.CtlGetStatus(RtcStatus.MofOutOfRange))
                    {
                        if (rtc is Rtc4 rtc4)
                        {
                            var info = rtc4.MarkingInfo;
                            Logger.Log(LogLevel.Warning, $"marker [{Index}]: mof out of range. marking info= {info.Value}");
                        }
                        else if (rtc is Rtc5 rtc5)
                        {
                            var info = rtc5.MarkingInfo;
                            Logger.Log(LogLevel.Warning, $"marker [{Index}]: mof out of range. marking info= {info.Value}");
                        }
                        else if (rtc is Rtc6 rtc6)
                        {
                            var info = rtc6.MarkingInfo;
                            Logger.Log(LogLevel.Warning, $"marker [{Index}]: mof out of range. marking info= {info.Value}");
                        }
                    }
                }
                if (IsJumpToOriginAfterFinished)
                {
                    if (rtc.Is3D)
                    {
                        success &= rtc3D.CtlZDefocus(0);
                        success &= rtc3D.CtlMoveTo(DVec3.Zero, 500);
                    }
                    else
                    {
                        success &= rtc.CtlMoveTo(DVec2.Zero, 500);
                    }
                }
                if (IsCheckPositionAck)
                {
                    if (!rtc.CtlGetStatus(RtcStatus.PositionAckOK))
                    {
                        var positionACKLimit = rtc is IRtcRangeCheck rtcRangeCheck ? rtcRangeCheck.PositionACKLimit : 0;
                        Logger.Log(LogLevel.Error, $"marker [{Index}]: out of range trajectory error limit: {positionACKLimit:F6}mm");
                    }
                }
            }
            finally
            {
                rtc.MatrixStack = oldMatrixStack;
                WorkingSet.EndTime = DateTime.Now;
                this.isInternalBusy = false;
                this.NotifyEnded(success);
                if (success)
                {
                    Logger.Log(LogLevel.Information, $"marker [{Index}]: mark has finished with {WorkingSet.ExecuteTime.Value.TotalSeconds:F3}s");
                    if (this.IsMeasurementPlot)
                        this.NotifyPlot();
                }
                else
                {
                    Logger.Log(LogLevel.Error, $"marker [{Index}]: mark has failed with {WorkingSet.ExecuteTime.Value.TotalSeconds:F3}s");
                }
            }
            return success;
        }
        /// <summary>
        /// Implements the marker thread for previewing marks.
        /// <para>마크 미리보기를 위한 마커 스레드를 구현합니다.<br/></para>
        /// <para>实现标记预览的标记器线程。<br/></para>
        /// <code>
        /// </code>
        /// </summary>
        /// <remarks>
        /// Mark bounding box with <see cref="ILaserGuideControl">ILaserGuideControl</see>
        /// </remarks>
        protected virtual bool MarkerThreadPreview()
        {
            var rtc = this.Rtc;
            var laser = this.Laser;
            var laserGuideControl = Laser as ILaserGuideControl;
            var document = this.Document;
            var rtc3D = rtc as IRtc3D;
            var rtc2ndHead = rtc as IRtc2ndHead;
            var rtcExtension = rtc as IRtcExtension;
            var rtcAlc = rtc as IRtcAutoLaserControl;
            var rtcMoF = rtc as IRtcMoF;
            Debug.Assert(rtc != null);
            Debug.Assert(laser != null);
            Debug.Assert(document != null);
            Debug.Assert(document.Selected.Length > 0);

            bool success = true;
            success &= laserGuideControl.CtlGuide(true);
            if (!success)
                return false;

            var tuples = new List<(DVec3 realMin, DVec3 realMax)>(document.Selected.Length);
            foreach (var entity in document.Selected)
            {
                if (entity is EntityTransformBase entityTransformBase)
                {
                    if (entityTransformBase.CalcuateRealMinMax(out var realMin, out var realMax))
                        tuples.Add((realMin, realMax));
                }
            }

            this.isInternalBusy = true;
            var oldMatrixStack = (IMatrixStack<DMat4>)rtc.MatrixStack.Clone();
            var oldSpeedJump = rtc.SpeedJump;
            var oldSpeedMark = rtc.SpeedMark;

            try
            {
                success &= rtc.ListBegin(ListBufferTypes.Auto);
                if (!success)
                    return false;
                success &= laser.ListBegin();
                if (!success)
                    return false;
                success &= rtc.ListSpeed(SpiralLab.Sirius3.UI.Config.MarkPreviewSpeed, SpiralLab.Sirius3.UI.Config.MarkPreviewSpeed);
                if (!success)
                    return false;
                for (int j = 0; j < SpiralLab.Sirius3.UI.Config.MarkPreviewRepeats; j++)
                {
                    for (int offsetIndex = 0; offsetIndex < Offsets.Length; offsetIndex++)
                    {
                        try
                        {
                            // Push offset matrix
                            rtc.MatrixStack.Push(Offsets[offsetIndex].ToMatrix);

                            foreach (var tuple in tuples)
                            {
                                var realMin = tuple.realMin;
                                var realMax = tuple.realMax;
                                success &= rtc.ListJumpTo(new DVec2(realMax.X, realMax.Y));
                                success &= rtc.ListMarkTo(new DVec2(realMin.X, realMax.Y));
                                success &= rtc.ListMarkTo(new DVec2(realMin.X, realMin.Y));
                                success &= rtc.ListMarkTo(new DVec2(realMax.X, realMin.Y));
                                success &= rtc.ListMarkTo(new DVec2(realMax.X, realMax.Y));
                                if (!success)
                                    break;
                            }
                        }
                        finally
                        {
                            // Pop offset matrix
                            rtc.MatrixStack.Pop();
                        }
                        if (!success)
                            break;
                    }
                    if (!success)
                        break;
                }

                if (success)
                {
                    success &= rtc.ListJumpTo(DVec2.Zero);
                    success &= laser.ListEnd();
                    success &= rtc.ListEnd();
                    success &= rtc.ListExecute(true);
                }
            }
            finally
            {
                success &= rtc.CtlSpeed(oldSpeedJump, oldSpeedMark);
                success &= laserGuideControl.CtlGuide(false);
                rtc.MatrixStack = oldMatrixStack;
                this.isInternalBusy = false;
            }
            return success;
        }

        /// <summary>
        /// Plot measurement to graph 
        /// <para>측정 세션 데이터를 그래프로 그립니다.<br/></para>
        /// <para>将测量会话数据绘制到图表。<br/></para>
        /// </summary>
        protected virtual void NotifyPlot()
        {
            // Plot as a graph
            foreach (var session in sessionQueue)
                session.Plot();
        }
    }
}
