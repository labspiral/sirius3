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
    /// MyMarkerSyncAxis
    /// </summary>
    /// <remarks>
    /// Used with syncAXIS only. <br/>
    /// Supported useful features like as <see cref="MarkerSyncAxis.MarkProcedures">MarkProcedures</see> and <see cref="MarkerSyncAxis.MarkTargets">MarkTargets</see>. <br/>
    /// </remarks>
    public class MyMarkerSyncAxis 
        : MarkerBase
    {
        /// <summary>
        /// Mark targets
        /// </summary>
        public enum MarkTargets
        {
            /// <summary>
            /// All entities
            /// </summary>
            All = 0,
            /// <summary>
            /// Selected entities
            /// </summary>
            Selected = 1,
        }

        /// <summary>
        /// Mark procedures
        /// </summary>
        public enum MarkProcedures
        {
            /// <summary>
            /// Order of marks: Mark Page1(s) at Offset1 -> Mark Page1(s) at Offset2, ...
            /// <code>
            /// //Pseudo codes
            /// for (int i = 0; i &lt; Offsets.Length; i++)
            /// {
            ///     for (int j = 0; j &lt; Layers.Count; j++)
            ///     {
            ///         Rtc.ListBegin();
            ///         Laser.ListBegin();
            ///         ...
            ///         LayerWork(i, Offsets[i], j, Layers[j]);
            ///         ...
            ///         Laser.ListEnd();
            ///         Rtc.ListEnd();
            ///         Rtc.ListExecute(true);
            ///         ...
            ///     }
            /// }
            /// </code>
            /// <remarks>
            /// Default: <c>MarkProcedures.LayerFirst</c>
            /// </remarks>
            /// </summary>
            LayerFirst = 0,
            /// <summary>
            /// Order of marks: Mark Page2 at Offset(s) -> Mark Page2 at Offset(s), ... 
            /// <code>
            /// //Pseudo codes
            /// for (int j = 0; j &lt; Layers.Count; j++)
            /// {
            ///     Rtc.ListBegin();        
            ///     Laser.ListBegin();
            ///     for (int i = 0; i &lt; Offsets.Length; i++)
            ///     {
            ///         ...
            ///         LayerWork(i, Offsets[i], j, layer);
            ///         ...
            ///     }
            ///     Laser.ListEnd();
            ///     Rtc.ListEnd();
            ///     Rtc.ListExecute(true);
            /// }
            /// </code>
            /// </summary>
            OffsetFirst = 1,
        }

        /// <summary>
        /// Target entities to mark
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
        /// </summary>
        protected MarkTargets markTarget = MarkTargets.All;

        /// <summary>
        /// Mark procedure
        /// </summary>
        /// <remarks>
        /// Default: <see cref="MarkProcedures.LayerFirst">MarkProcedures.LayerFirst</see> <br/>
        /// </remarks>
        [Browsable(true)]
        [ReadOnly(false)]
        [LocalizedCategory("Data")]
        [LocalizedDisplayName("MarkProcedure")]
        [LocalizedDescription("MarkProcedure")]
        public virtual MarkProcedures MarkProcedure
        {
            get { return markProcedure; }
            set
            {
                if (this.IsBusy)
                {
                    Logger.Log(LogLevel.Error, $"marker [{Index}]: fail to set mark procedure during busy");
                    return;
                }
                var oldMarkProcedure = markProcedure;
                markProcedure = value;
                if (markProcedure != oldMarkProcedure)
                    this.NotifyPropertyChanged();
            }
        }
        /// <summary>
        /// Internal <c>MarkProcedures</c>
        /// </summary>
        protected MarkProcedures markProcedure = MarkProcedures.LayerFirst;

        /// <summary>
        /// Op status 
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [LocalizedCategory("Status")]
        [LocalizedDisplayName("OperationStatusColor")]
        [LocalizedDescription("OperationStatusColor")]
        public System.Drawing.Color OperationStatusColor { get; protected set; }

        /// <summary>
        /// Is plot simulation output to syncAxis viewer
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
                if (!File.Exists(SpiralLab.Sirius3.Config.SyncAxisViewerProgramPath))
                {
                    MessageBox.Show($"syncaxis viewer program is not exist at '{SpiralLab.Sirius3.Config.MeasurementGNUPlotProgramPath}'", "Warning", MessageBoxButtons.OK);
                    return;
                }
                isMeasurementPlot = value;
            }
        }
        /// <summary>
        /// Is plot measurement session to graph or not
        /// </summary>
        protected bool isMeasurementPlot;

        /// <summary>
        /// Internal marker thread 
        /// </summary>
        protected Thread thread;
        /// <summary>
        /// list of layers to mark
        /// </summary>
        protected List<EntityLayer> layers;
        System.Windows.Forms.Timer timerStatus = new System.Windows.Forms.Timer();
        private bool disposed = false;


        /// <summary>
        /// Initializes a new instance of the <see cref="MyMarkerSyncAxis"/> class.
        /// <para><see cref="MyMarkerSyncAxis"/> 클래스의 새 인스턴스를 초기화합니다.</para>
        /// <para>初始化 <see cref="MyMarkerSyncAxis"/> 类的新实例。</para>
        /// <code>
        /// </code>
        /// </summary>
        public MyMarkerSyncAxis()
              : base()
        {
            isMeasurementPlot = true;
            IsJumpToOriginAfterFinished = true;
            markTarget = MarkTargets.All;
            markProcedure = MarkProcedures.LayerFirst;

            OperationStatusColor = Color.DarkGray;
            timerStatus.Interval = 100;
            timerStatus.Tick += TimerStatus_Tick;
            timerStatus.Enabled = true;

            layers = new List<EntityLayer>();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="MyMarkerSyncAxis"/> class with the specified index and name.
        /// <para>지정된 인덱스와 이름으로 <see cref="MyMarkerSyncAxis"/> 클래스의 새 인스턴스를 초기화합니다.</para>
        /// <para>使用指定的索引和名称初始化 <see cref="MyMarkerSyncAxis"/> 类的新实例。</para>
        /// <code>
        /// </code>
        /// </summary>
        /// <param name="index">The index of the marker.</param>
        /// <param name="name">The name of the marker.</param>
        public MyMarkerSyncAxis(int index, string name)
            : this()
        {
            Index = index;
            Name = name;
        }
        /// <summary>
        /// Finalizes an instance of the <see cref="MyMarkerSyncAxis"/> class.
        /// <para><see cref="MyMarkerSyncAxis"/> 클래스의 인스턴스를 종료합니다.</para>
        /// <para>终止 <see cref="MyMarkerSyncAxis"/> 类的一个实例。</para>
        /// <code>
        /// </code>
        /// </summary>
        ~MyMarkerSyncAxis()
        {
            this.Dispose(false);
        }

        /// <inheritdoc/>  
        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    //this.Stop();
                    timerStatus.Enabled = false;
                    timerStatus.Tick -= TimerStatus_Tick;
                }
                this.disposed = true;
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Handles the Tick event of the timerStatus, updating the operation status color.
        /// <para>timerStatus의 Tick 이벤트를 처리하여 작업 상태 색상을 업데이트합니다.</para>
        /// <para>处理 timerStatus 的 Tick 事件，更新操作状态颜色。</para>
        /// <code>
        /// </code>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void TimerStatus_Tick(object sender, EventArgs e)
        {
            var rtcSyncAxis = Rtc as IRtcSyncAxis;
            if (rtcSyncAxis == null)
                return;
            switch(rtcSyncAxis.OpStatus)
            {
                case OperationStatus.Unknown:
                    if (OperationStatusColor != Color.DarkGray)
                    {
                        OperationStatusColor = Color.DarkGray;
                        NotifyPropertyChanged("OperationStatusColor");
                    }
                    break;
                case OperationStatus.Red:
                    if (OperationStatusColor != Color.Red)
                    {
                        OperationStatusColor = Color.Red;
                        NotifyPropertyChanged("OperationStatusColor");
                    }
                    break;
                case OperationStatus.Yellow:
                    if (OperationStatusColor != Color.Yellow)
                    {
                        OperationStatusColor = Color.Yellow;
                        NotifyPropertyChanged("OperationStatusColor");
                    }
                    break;
                case OperationStatus.Green:
                    if (OperationStatusColor != Color.Green)
                    {
                        OperationStatusColor = Color.Green;
                        NotifyPropertyChanged("OperationStatusColor");
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override bool Initialize()
        {
            Logger.Log(LogLevel.Information, $"marker [{Index}]: initialized");
            return true;
        }
        /// <inheritdoc/>
        public override bool Ready(IDocument document, IView view, IRtc rtc, ILaser laser, IPowerMeter powerMeter)
        {
            if (this.IsBusy)
            {
                Logger.Log(LogLevel.Error, $"marker [{Index}]: fail to ready. marker status is busy");
                return false;
            }

            base.Document = document;
            base.View = view;
            base.Rtc = rtc;
            base.Laser = laser;
            base.PowerMeter = powerMeter;

            if (rtc is Rtc4 || rtc is Rtc4Ethernet || rtc is Rtc5 || rtc is Rtc6 || rtc is Rtc6Ethernet)
            {
                this.Rtc = null;
                Logger.Log(LogLevel.Error, $"marker [{Index}]: assigned invalid RTC instance");
                return false;
            }
            document?.ActRegen();
            Logger.Log(LogLevel.Debug, $"marker [{Index}]: ready with doc= {document?.FileName}, view= {view?.Name}, rtc= {rtc?.Name}, laser= {laser?.Name}, pm= {powerMeter?.Name}");
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
        public override bool Start(DocumentPages page = DocumentPages.Page1)
        {
            if (Document == null || Rtc == null || Laser == null)
            {
                Logger.Log(LogLevel.Error, $"marker [{Index}]: document, rtc, laser is not assigned");
                return false;
            }
            if (!Document.IsReady || Document.IsSimulationWorking)
            {
                Logger.Log(LogLevel.Error, $"marker [{Index}]: document is not ready or simulating now");
                return false;
            }
            if (this.IsBusy)
            {
                Logger.Log(LogLevel.Error, $"marker [{Index}]: busy now !");
                return false;
            }
            if (this.IsError)
            {
                Logger.Log(LogLevel.Error, $"marker [{Index}]: has a error. reset at first");
                return false;
            }
            if (!this.IsReady)
            {
                Logger.Log(LogLevel.Error, $"marker [{Index}]: is not ready yet");
                return false;
            }

            var rtc = this.Rtc;            
            var laser = this.Laser;
            var doc = this.Document;

            if (rtc.CtlGetStatus(RtcStatus.Busy))
            {
                Logger.Log(LogLevel.Error, $"marker [{Index}]: busy now !");
                return false;
            }
            if (!rtc.CtlGetStatus(RtcStatus.NoError))
            {
                Logger.Log(LogLevel.Error, $"marker [{Index}]: rtc has a internal error. reset at first");
                return false;
            }

            if (laser.IsError)
            {
                Logger.Log(LogLevel.Error, $"marker [{Index}]: laser has a error status. reset at first");
                return false;
            }

            if (null != thread)
            {
                if (!this.thread.Join(500))
                {
                    Logger.Log(LogLevel.Error, $"marker [{Index}]: previous works has not finished yet");
                    return false;
                }
            }

            if (null == Offsets || 0 == Offsets.Length)
                this.Offsets = new Offset[1] { Offset.Zero };

            Logger.Log(LogLevel.Warning, $"marker [{Index}]: trying to start mark with target= {MarkTarget}, proc= {MarkProcedure}, offset(s)= {this.Offsets.Length}");

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
            WorkingSet.Reset();
            WorkingSet.DocumentPage = page;
            WorkingSet.Page = Document.DocumentData.Pages[(int)page];
            WorkingSet.PageIndex = (int)page;

            switch (MarkProcedure)
            {
                default:
                    this.thread = new Thread(this.MarkerThreadLayerFirst);
                    break;
                case MarkProcedures.OffsetFirst:
                    this.thread = new Thread(this.MarkerThreadOffsetFirst);
                    break;
            }
            this.thread.Name = $"MarkerSyncAxis [{Index}]: {this.Name}";
            this.thread.Start();
            return true;

        }
        /// <inheritdoc/>
        public override bool Preview()
        {
            if (Document == null || Rtc == null || Laser == null)
            {
                Logger.Log(LogLevel.Error, $"marker [{Index}]: document, rtc, laser is not assigned");
                return false;
            }
            if (this.IsBusy)
            {
                Logger.Log(LogLevel.Error, $"marker [{Index}]: busy now !");
                return false;
            }
            if (this.IsError)
            {
                Logger.Log(LogLevel.Error, $"marker [{Index}]: has a error. reset at first");
                return false;
            }
            if (!this.IsReady)
            {
                Logger.Log(LogLevel.Error, $"marker [{Index}]: is not ready yet");
                return false;
            }

            if (Rtc.CtlGetStatus(RtcStatus.Busy))
            {
                Logger.Log(LogLevel.Error, $"marker [{Index}]: busy now !");
                return false;
            }
            if (!Rtc.CtlGetStatus(RtcStatus.NoError))
            {
                Logger.Log(LogLevel.Error, $"marker [{Index}]: rtc has a internal error. reset at first");
                return false;
            }
            if (Laser.IsError)
            {
                Logger.Log(LogLevel.Error, $"marker [{Index}]: laser has a error status. reset at first");
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

            if (null != thread)
            {
                if (!this.thread.Join(500))
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
            this.thread = new Thread(this.MarkerThreadPreview);
            this.thread.Name = $"Marker: {this.Name}";
            this.thread.Start();
            return true;
        }
        /// <inheritdoc/>
        public override bool Stop()
        {
            if (null == Rtc || null == Laser)
                return false;
            bool success = true;
            success &= Rtc.CtlAbort();
            success &= Laser.CtlAbort();

            if (null != thread)
            {
                var sw = Stopwatch.StartNew();
                do
                {
                    Application.DoEvents();
                    if (this.thread.Join(0))
                    {
                        thread = null;
                        break;
                    }
                    if (sw.ElapsedMilliseconds > 500)
                    {
                        success = false;
                        Logger.Log(LogLevel.Error, $"marker [{Index}]: waiting for stop but timed out");
                        break; // Timed out
                    }
                }
                while (true);
            }
            this.isInternalBusy = false;
            return success;
        }
        /// <inheritdoc/>
        public override bool Reset()
        {
            if (null == Rtc || null == Laser)
                return false;
            bool success = true;
            success &= Rtc.CtlReset();
            success &= Laser.CtlReset();

            return success;
        }
        /// <summary>
        /// Marks each <see cref="EntityLayer"/>.
        /// <para>각 <see cref="EntityLayer"/>를 마킹합니다.</para>
        /// <para>标记每个 <see cref="EntityLayer"/>。</para>
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
        /// <summary>
        /// Marks each <see cref="IEntity"/>.
        /// <para>각 <see cref="IEntity"/>를 마킹합니다.</para>
        /// <para>标记每个 <see cref="IEntity"/>。</para>
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
                Logger.Log(LogLevel.Error, $"marker [{Index}]: fail to mark entity at before event handler"); 
                return success;
            }
            if (entity is IMarkerable markerable)
            {
                success &= markerable.Mark(this);
            }
  
            if (!success)
            {
                Logger.Log(LogLevel.Error, $"marker [{Index}]: fail to mark entity at after event handler"); 
                return success;
            }
            success &= NotifyAfterEntity(entity);
            return success;
        }
        /// <summary>
        /// Implements the marker thread for <see cref="MarkProcedures.LayerFirst"/> procedure.
        /// <para><see cref="MarkProcedures.LayerFirst"/> 절차에 대한 마커 스레드를 구현합니다.</para>
        /// <para>实现 <see cref="MarkProcedures.LayerFirst"/> 过程的标记器线程。</para>
        /// <code>
        /// </code>
        /// </summary>
        /// <remarks>
        /// <see cref="MarkProcedures.LayerFirst">LayerFirst</see> <br/>
        /// Move offset1 and Mark layers -> Move offset2 and Mark layers, ... <br/>
        /// </remarks>
        protected virtual void MarkerThreadLayerFirst()
        {
            var rtc = this.Rtc;
            var laser = this.Laser;
            var document = this.Document;
            var rtcSyncAxis = rtc as IRtcSyncAxis;
            Debug.Assert(rtc != null);
            Debug.Assert(laser != null);
            Debug.Assert(document != null);
            Debug.Assert(null != rtcSyncAxis);
            this.isInternalBusy = true;
            this.NotifyStarted();
            WorkingSet.StartTime = WorkingSet.EndTime = DateTime.Now;
            bool success = true;     
            var oldMatrixStack = (IMatrixStack<DMat4>)rtc.MatrixStack.Clone();
            for (int i = 0; i < Offsets.Length; i++)
            {
                WorkingSet.Offset = Offsets[i];
                WorkingSet.OffsetIndex = i;
                rtc.MatrixStack.Push(Offsets[i].ToMatrix);
                Logger.Log(LogLevel.Debug, $"marker [{Index}]: offset index= {i}, xyzt= {Offsets[i].ToString()}");
                for (int j = 0; j < layers.Count; j++)
                {
                    var layer = layers[j];
                    if (!layer.IsAllowMark)
                        continue;
                    success &= base.NotifyBeforeLayer(layer);
                    if (!success)
                    {
                        Logger.Log(LogLevel.Error, $"marker [{Index}]: fail to mark layer at before event handler"); 
                        break;
                    }

                    success &= layer.Mark(this);
                    if (!success)
                        break;

                    success &= laser.ListBegin();
                    success &= rtcSyncAxis.ListBegin(WorkingSet.LayerPen.MotionType);
                    success &= LayerWork(i, Offsets[i], j, layer);
                    if (success && IsJumpToOriginAfterFinished)
                        success &= rtc.ListJumpTo(DVec2.Zero);

                    if (success) 
                    {
                        success &= laser.ListEnd();
                        success &= rtc.ListEnd();
                        if (success)
                            success &= rtc.ListExecute(true);
                    }
                    if (!success)
                        break;
                    success &= NotifyAfterLayer(layer);
                    if (!success)
                    {
                        Logger.Log(LogLevel.Error, $"marker [{Index}]: fail to mark layer at after event handler");
                        break;
                    }
                    if (success)
                    {
                        if (this.IsMeasurementPlot)
                        {
                            if (rtcSyncAxis.IsSimulationMode)
                            {
                                string simulatedFileName = Path.Combine(SpiralLab.Sirius3.Config.SyncAxisSimulateFilePath, rtcSyncAxis.SimulationFileName);
                                SyncAxisViewerHelper.Plot(simulatedFileName);
                            }
                        }
                    }
                }
                rtc.MatrixStack.Pop(); 
                if (!success)
                    break;
            }

            rtc.MatrixStack = oldMatrixStack;
            WorkingSet.EndTime = DateTime.Now;
            this.isInternalBusy = false;
            this.NotifyEnded(success);
            if (success)
            {
                Logger.Log(LogLevel.Information, $"marker [{Index}]: mark has finished with {WorkingSet.ExecuteTime.Value.TotalSeconds:F3}s");
            }
            else
            {
                Logger.Log(LogLevel.Error, $"marker [{Index}]: mark has failed with {WorkingSet.ExecuteTime.Value.TotalSeconds:F3}s");
            }
        }
        /// <summary>
        /// Implements the marker thread for <see cref="MarkProcedures.OffsetFirst"/> procedure.
        /// <para><see cref="MarkProcedures.OffsetFirst"/> 절차에 대한 마커 스레드를 구현합니다.</para>
        /// <para>实现 <see cref="MarkProcedures.OffsetFirst"/> 过程的标记器线程。</para>
        /// <code>
        /// </code>
        /// </summary>
        /// <remarks>
        /// <see cref="MarkProcedures.OffsetFirst">OffsetFirst</see> <br/>
        /// Mark layer1 with offset1 and offset2, ... -> Mark layer2 with offset1 and offset2, ... <br/>
        /// </remarks>
        protected virtual void MarkerThreadOffsetFirst()
        {
            var rtc = this.Rtc;
            var laser = this.Laser;
            var document = this.Document;
            var rtcSyncAxis = rtc as IRtcSyncAxis;
            Debug.Assert(rtc != null);
            Debug.Assert(laser != null);
            Debug.Assert(document != null);
            Debug.Assert(null != rtcSyncAxis);
            this.isInternalBusy = true;
            WorkingSet.StartTime = WorkingSet.EndTime = DateTime.Now;
            this.NotifyStarted();
            bool success = true;
            var oldMatrixStack = (IMatrixStack<DMat4>)rtc.MatrixStack.Clone();
            for (int j = 0; j < layers.Count; j++)
            {
                var layer = layers[j];
                if (!layer.IsAllowMark)
                    continue;
                success &= base.NotifyBeforeLayer(layer);
                if (!success)
                {
                    Logger.Log(LogLevel.Error, $"marker [{Index}]: fail to mark layer at before event handler"); 
                    break;
                }

                success &= layer.Mark(this);
                if (!success)
                    break;

                success &= laser.ListBegin();
                success &= rtcSyncAxis.ListBegin(WorkingSet.LayerPen.MotionType);

                for (int i = 0; i < Offsets.Length; i++)
                {
                    WorkingSet.OffsetIndex = i;
                    WorkingSet.Offset = Offsets[i];
                    rtc.MatrixStack.Push(Offsets[i].ToMatrix);
                    Logger.Log(LogLevel.Debug, $"marker [{Index}]: offset index= {i}, xyzt= {Offsets[i].ToString()}");
                    success &= LayerWork(i, Offsets[i], j, layer);
                    rtc.MatrixStack.Pop();
                    if (!success)
                        break;
                }
                if (IsJumpToOriginAfterFinished)
                    success &= rtc.ListJumpTo(DVec2.Zero);
                success &= laser.ListEnd();
                success &= rtc.ListEnd();
                if (success)
                    success &= rtc.ListExecute(true);
                if (!success)
                    break;

                success &= NotifyAfterLayer(layer);
                if (!success)
                {
                    Logger.Log(LogLevel.Error, $"marker [{Index}]: fail to mark layer at after event handler");
                    break;
                }
                if (success)
                {
                    if (this.IsMeasurementPlot)
                    {
                        if (rtcSyncAxis.IsSimulationMode)
                        {
                            string simulatedFileName = Path.Combine(SpiralLab.Sirius3.Config.SyncAxisSimulateFilePath, rtcSyncAxis.SimulationFileName);
                            SyncAxisViewerHelper.Plot(simulatedFileName);
                        }
                    }
                }
               
                if (!success)
                    break;
            }

            rtc.MatrixStack = oldMatrixStack;
            WorkingSet.EndTime = DateTime.Now;
            this.isInternalBusy = false;
            this.NotifyEnded(success);
            if (success)
            {
                Logger.Log(LogLevel.Information, $"marker [{Index}]: mark has finished with {WorkingSet.ExecuteTime.Value.TotalSeconds:F3}s");
            }
            else
            {
                Logger.Log(LogLevel.Error, $"marker [{Index}]: mark has failed with {WorkingSet.ExecuteTime.Value.TotalSeconds:F3}s");
            }
        }
        /// <summary>
        /// Implements the marker thread for previewing marks.
        /// <para>마크 미리보기를 위한 마커 스레드를 구현합니다.</para>
        /// <para>实现标记预览的标记器线程。</para>
        /// <code>
        /// </code>
        /// </summary>
        /// <remarks>
        /// Mark bounding box with <see cref="ILaserGuideControl">ILaserGuideControl</see>
        /// </remarks>
        protected virtual void MarkerThreadPreview()
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
            double realWidth;
            double realHeight;
            double realDepth;
            DVec3 realCenter;
            if (EntityTransformBase.CalcuateRealMinMax(document.Selected, out var realMin, out var realMax))
            {
                realWidth = realMax.X - realMin.X;
                realHeight = realMax.Y - realMin.Y;
                realDepth = realMax.Z - realMin.Z;
                realCenter = (realMin + realMax) / 2.0f;
            }
            else
                Debug.Assert(false); // "invalid real min/max");

            bool success = true;
            success &= laserGuideControl.CtlGuide(true);
            if (!success)
                return;
            this.isInternalBusy = true;

            var oldMatrixStack = (IMatrixStack<DMat4>)rtc.MatrixStack.Clone();
            var oldSpeedJump = rtc.SpeedJump;
            var oldSpeedMark = rtc.SpeedMark;
            success &= rtc.ListBegin(ListBufferTypes.Auto);
            success &= laser.ListBegin();
            success &= rtc.ListSpeed(SpiralLab.Sirius3.UI.Config.MarkPreviewSpeed, SpiralLab.Sirius3.UI.Config.MarkPreviewSpeed);
            for (int j = 0; j < SpiralLab.Sirius3.UI.Config.MarkPreviewRepeats; j++)
            {
                for (int i = 0; i < Offsets.Length; i++)
                {
                    try
                    {
                        // Push offset matrix
                        rtc.MatrixStack.Push(Offsets[i].ToMatrix);
                        // Rectangle by bouding box 
                        // 2 1
                        // 3 4
                        success &= rtc.ListJumpTo(new DVec2(realMax.X, realMax.Y));
                        success &= rtc.ListMarkTo(new DVec2(realMin.X, realMax.Y));
                        success &= rtc.ListMarkTo(new DVec2(realMin.X, realMin.Y));
                        success &= rtc.ListMarkTo(new DVec2(realMax.X, realMin.Y));
                        success &= rtc.ListMarkTo(new DVec2(realMax.X, realMax.Y));
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
            success &= rtc.CtlSpeed(oldSpeedJump, oldSpeedMark);
            success &= laserGuideControl.CtlGuide(false);
            rtc.MatrixStack = oldMatrixStack;
            this.isInternalBusy = false;
        }
    }
}
