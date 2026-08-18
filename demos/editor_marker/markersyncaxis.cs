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
 * Description : MyMarkerSyncAxis
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
    /// <para>SyncAXIS용 마커입니다.<br/></para>
    /// </summary>
    /// <remarks>
    /// Used with syncAXIS only. <br/>
    /// Supported useful features like as <see cref="MyMarkerSyncAxis.MarkProcedures">MarkProcedures</see> and <see cref="MarkerSyncAxis.MarkTargets">MarkTargets</see>. <br/>
    /// </remarks>
    public class MyMarkerSyncAxis
        : MarkerBase
    {
        /// <summary>
        /// Mark targets
        /// <para>마크 대상<br/></para>
        /// </summary>
        public enum MarkTargets
        {
            /// <summary>
            /// All entities
            /// <para>모든 엔티티<br/></para>
            /// </summary>
            All = 0,
            /// <summary>
            /// Selected entities
            /// <para>선택된 엔티티<br/></para>
            /// </summary>
            Selected = 1,
        }

        /// <summary>
        /// Selects whether layers or offsets are completed first while traversing a page.
        /// <para>페이지를 순회할 때 레이어와 오프셋 중 어느 쪽을 먼저 완료할지 선택합니다.<br/></para>
        /// </summary>
        public enum MarkProcedures
        {
            /// <summary>
            /// Completes every layer at the current offset, then moves to the next offset.
            /// Each layer and offset pair is generated and executed as a separate syncAXIS job.
            /// <para>현재 오프셋에서 모든 레이어를 완료한 뒤 다음 오프셋으로 이동합니다.<br/>
            /// 레이어와 오프셋의 각 조합을 별도의 syncAXIS 작업으로 생성하고 실행합니다.<br/></para>
            /// <example>
            /// <code language="C#">
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
            /// </example>
            /// <remarks>
            /// With two offsets and three layers, the order is O1-L1, O1-L2, O1-L3,
            /// O2-L1, O2-L2, O2-L3, and six syncAXIS jobs are executed. This is the default.
            /// <para>오프셋 2개와 레이어 3개라면 O1-L1, O1-L2, O1-L3,
            /// O2-L1, O2-L2, O2-L3 순서이며 syncAXIS 작업을 6번 실행합니다. 기본값입니다.<br/></para>
            /// </remarks>
            /// </summary>
            LayerFirst = 0,
            /// <summary>
            /// Completes every offset for the current layer, then moves to the next layer.
            /// All offsets of one layer are generated in a single syncAXIS job.
            /// <para>현재 레이어에서 모든 오프셋을 완료한 뒤 다음 레이어로 이동합니다.<br/>
            /// 한 레이어의 모든 오프셋을 하나의 syncAXIS 작업에 생성합니다.<br/></para>
            /// <example>
            /// <code language="C#">
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
            /// </example>
            /// <remarks>
            /// With two offsets and three layers, the order is L1-O1, L1-O2, L2-O1,
            /// L2-O2, L3-O1, L3-O2, and three syncAXIS jobs are executed.
            /// <para>오프셋 2개와 레이어 3개라면 L1-O1, L1-O2, L2-O1,
            /// L2-O2, L3-O1, L3-O2 순서이며 syncAXIS 작업을 3번 실행합니다.<br/></para>
            /// </remarks>
            /// </summary>
            OffsetFirst = 1,
        }

        /// <summary>
        /// Target entities to mark
        /// <para>마크할 대상 엔티티<br/></para>
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
                    this.NotifyPropertyChanged(nameof(MarkTarget));
            }
        }
        /// <summary>
        /// Internal <c>MarkTargets</c>
        /// <para>내부 <c>MarkTargets</c><br/></para>
        /// </summary>
        protected MarkTargets markTarget = MarkTargets.All;

        /// <summary>
        /// Mark procedure
        /// <para>마크 절차<br/></para>
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
                if (markProcedure == value)
                    return;
                if (this.IsBusy)
                {
                    Logger.Log(LogLevel.Error, $"marker [{Index}]: fail to set mark procedure as '{value}' during busy");
                    return;
                }
                var oldMarkProcedure = markProcedure;
                markProcedure = value;
                if (markProcedure != oldMarkProcedure)
                    this.NotifyPropertyChanged(nameof(MarkProcedure));
            }
        }
        /// <summary>
        /// Internal <c>MarkProcedures</c>
        /// <para>내부 <c>MarkProcedures</c><br/></para>
        /// </summary>
        protected MarkProcedures markProcedure = MarkProcedures.LayerFirst;

        /// <summary>
        /// Op status 
        /// <para>작업 상태<br/></para>
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [LocalizedCategory("Status")]
        [LocalizedDisplayName("OperationStatusColor")]
        [LocalizedDescription("OperationStatusColor")]
        public System.Drawing.Color OperationStatusColor { get; protected set; }

        /// <summary>
        /// Is plot simulation output to syncAxis viewer
        /// <para>시뮬레이션 출력을 syncAxis 뷰어로 그릴지 여부<br/></para>
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
                this.NotifyPropertyChanged(nameof(IsMeasurementPlot));
            }
        }
        /// <summary>
        /// Is plot measurement session to graph or not
        /// <para>측정 세션을 그래프로 그릴지 여부<br/></para>
        /// </summary>
        protected bool isMeasurementPlot;

        /// <summary>
        /// list of layers to mark
        /// <para>마킹할 레이어 목록<br/></para>
        /// </summary>
        protected List<EntityLayer> layers;
        private System.Windows.Forms.Timer timerStatus = new System.Windows.Forms.Timer();

        /// <summary>
        /// Target <c>IRtc</c> instance
        /// </summary>
        protected IRtc Rtc { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MyMarkerSyncAxis"/> class.
        /// <para><see cref="MyMarkerSyncAxis"/> 클래스의 새 인스턴스를 초기화합니다.<br/></para>
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
        /// <para>지정된 인덱스와 이름으로 <see cref="MyMarkerSyncAxis"/> 클래스의 새 인스턴스를 초기화합니다.<br/></para>
        /// <code>
        /// </code>
        /// </summary>
        /// <param name="index">The index of the marker. <para>마커의 인덱스입니다.</para> </param>
        /// <param name="name">The name of the marker. <para>마커의 이름입니다.</para> </param>
        public MyMarkerSyncAxis(int index, string name)
            : this()
        {
            Index = index;
            Name = name;
        }
        /// <inheritdoc/>  
        protected override void OnDisposeManaged()
        {
            DisposeStatusTimer();
        }
        /// <inheritdoc/>  
        protected override Task OnDisposeManagedAsync()
        {
            DisposeStatusTimer();
            return Task.CompletedTask;
        }

        private void DisposeStatusTimer()
        {
            if (timerStatus == null)
                return;

            timerStatus.Enabled = false;
            timerStatus.Tick -= TimerStatus_Tick;
            timerStatus.Dispose();
            timerStatus = null;
        }

        /// <summary>
        /// Handles the Tick event of the timerStatus, updating the operation status color.
        /// <para>timerStatus의 Tick 이벤트를 처리하여 작업 상태 색상을 업데이트합니다.<br/></para>
        /// <code>
        /// </code>
        /// </summary>
        /// <param name="sender">The source of the event. <para>이벤트 소스입니다.</para> <para>事件源。</para></param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data. <para>이벤트 데이터를 포함하는 <see cref="EventArgs"/> 인스턴스입니다.</para> <para>包含事件数据的 <see cref="EventArgs"/> 实例。</para></param>
        private void TimerStatus_Tick(object sender, EventArgs e)
        {
            var rtcSyncAxis = Scanner as IRtcSyncAxis;
            if (rtcSyncAxis == null)
                return;
            switch (rtcSyncAxis.OpStatus)
            {
                case OperationStatus.Unknown:
                    if (OperationStatusColor != Color.DarkGray)
                    {
                        OperationStatusColor = Color.DarkGray;
                        NotifyPropertyChanged(nameof(OperationStatusColor));
                    }
                    break;
                case OperationStatus.Red:
                    if (OperationStatusColor != Color.Red)
                    {
                        OperationStatusColor = Color.Red;
                        NotifyPropertyChanged(nameof(OperationStatusColor));
                    }
                    break;
                case OperationStatus.Yellow:
                    if (OperationStatusColor != Color.Yellow)
                    {
                        OperationStatusColor = Color.Yellow;
                        NotifyPropertyChanged(nameof(OperationStatusColor));
                    }
                    break;
                case OperationStatus.Green:
                    if (OperationStatusColor != Color.Green)
                    {
                        OperationStatusColor = Color.Green;
                        NotifyPropertyChanged(nameof(OperationStatusColor));
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
        public override bool Ready(IDocument document, IView view, IScanner scanner, ILaser laser, IPowerMeter powerMeter)//, IRemote remote)
        {
            if (IsBusy)
            {
                Logger.Log(LogLevel.Error, $"marker [{Index}]: fail to ready. marker status is busy");
                return false;
            }
            if (document == null || scanner == null || laser == null)
            {
                Logger.Log(LogLevel.Error, $"marker [{Index}]: fail to ready. document, scanner and laser are required");
                return false;
            }
            if (!(scanner is IRtcSyncAxis))
            {
                Logger.Log(LogLevel.Error, $"marker [{Index}]: fail to ready. scanner {scanner.GetType().Name} is incompatible; expected IRtcSyncAxis");
                return false;
            }

            Document = document;
            View = view;
            Scanner = scanner;
            Rtc = (IRtc)scanner;
            Laser = laser;
            PowerMeter = powerMeter;
            document.ActRegen();
            Logger.Log(LogLevel.Debug, $"marker [{Index}]: ready with doc= {document.FileName}, view= {view?.Name}, rtc= {Rtc.Name}, laser= {laser.Name}, pm= {powerMeter?.Name}");
            return true;
        }
        /// <inheritdoc/>
        public override bool Ready(IDocument document)
        {
            if (IsBusy)
            {
                Logger.Log(LogLevel.Error, $"marker [{Index}]: fail to ready. marker status is busy");
                return false;
            }
            if (document == null)
            {
                Logger.Log(LogLevel.Error, $"marker [{Index}]: fail to ready. document is required");
                return false;
            }

            Document = document;
            document.ActRegen();
            Logger.Log(LogLevel.Debug, $"marker [{Index}]: ready with doc= {document.FileName}");
            return true;
        }


        private void PreparePage(DocumentPages page)
        {
            switch (page)
            {
                case DocumentPages.Page1:
                case DocumentPages.Page2:
                case DocumentPages.Page3:
                case DocumentPages.Page4:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(page), page, "Invalid target page.");
            }

            int pageIndex = (int)page;
            var targetPage = Document.DocumentData.Pages[pageIndex];
            SnapshotLayers(targetPage);

            WorkingSet.Reset();
            WorkingSet.DocumentPage = page;
            WorkingSet.Page = targetPage;
            WorkingSet.PageIndex = pageIndex;
        }

        // Preview follows the document's active page, which may have been assigned directly without
        // updating IDocument.Page. Resolve the actual page reference before publishing the working set.
        private void PrepareActivePage()
        {
            var activePage = Document.ActivePage;
            for (int pageIndex = 0; pageIndex < Document.DocumentData.Pages.Count; pageIndex++)
            {
                if (ReferenceEquals(Document.DocumentData.Pages[pageIndex], activePage))
                {
                    PreparePage((DocumentPages)pageIndex);
                    return;
                }
            }
            throw new InvalidOperationException("The active page does not belong to the marker document.");
        }

        // The list object is worker-owned; only layer references are copied from the document.
        private void SnapshotLayers(IPage page)
        {
            layers.Clear();
            foreach (var child in page.Layers.Children)
            {
                if (child is EntityLayer layer)
                    layers.Add(layer);
            }
        }

        // Each cleanup attempt is isolated so an RTC failure never skips laser cleanup.
        private void AbortIncompleteList(IRtc rtc, ILaser laser, string operation)
        {
            bool rtcAborted = false;
            bool laserAborted = false;
            try
            {
                rtcAborted = rtc.CtlAbort();
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Warning, ex, $"marker [{Index}]: exception while aborting incomplete RTC list for {operation}");
            }
            try
            {
                laserAborted = laser.CtlAbort();
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Warning, ex, $"marker [{Index}]: exception while aborting incomplete laser list for {operation}");
            }
            if (!rtcAborted || !laserAborted)
            {
                Logger.Log(LogLevel.Warning,
                    $"marker [{Index}]: incomplete list cleanup failed for {operation}. rtc abort= {rtcAborted}, laser abort= {laserAborted}");
            }
        }

        /// <inheritdoc/>
        protected override async Task<bool> OnStarting(DocumentPages page = DocumentPages.Page1)
        {
            var rtcSyncAxis = Scanner as IRtcSyncAxis;
            if (rtcSyncAxis == null)
                return false;

            //if (rtcSyncAxis.OpStatus != OperationStatus.Green)
            //{
            //    Logger.Log(LogLevel.Error, $"marker [{Index}]: operation status is not green");
            //    return false;
            //}

            PreparePage(page);

            Logger.Log(LogLevel.Warning, $"marker [{Index}]: trying to start mark with target= {MarkTarget}, proc= {MarkProcedure}, offset(s)= {this.Offsets.Length}");
            switch (MarkProcedure)
            {
                default:
                    markerTask = Task.Run(() => this.MarkerThreadLayerFirst());
                    break;

                case MarkProcedures.OffsetFirst:
                    markerTask = Task.Run(() => this.MarkerThreadOffsetFirst());
                    break;
            }

            return await markerTask;
        }
        /// <inheritdoc/>
        protected override async Task<bool> OnPreviewing()
        {
            PrepareActivePage();

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
            if (Rtc == null || Laser == null)
                return false;

            bool rtcReset = Rtc.CtlReset();
            bool laserReset = Laser.CtlReset();
            return rtcReset && laserReset;
        }
        /// <summary>
        /// Marks each <see cref="EntityLayer"/>.
        /// <para>각 <see cref="EntityLayer"/>를 마킹합니다.<br/></para>
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
                for (int j = 0; j < layer.Children.Count; j++)
                {
                    var entity = layer.Children[j];
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
        /// Implements the marker thread for <see cref="MarkProcedures.LayerFirst"/> procedure.
        /// <para><see cref="MarkProcedures.LayerFirst"/> 절차에 대한 마커 스레드를 구현합니다.<br/></para>
        /// <code>
        /// </code>
        /// </summary>
        /// <remarks>
        /// <see cref="MarkProcedures.LayerFirst">LayerFirst</see> <br/>
        /// Move offset1 and Mark layers -> Move offset2 and Mark layers, ... <br/>
        /// </remarks>
        protected virtual bool MarkerThreadLayerFirst()
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

            WorkingSet.StartTime = DateTime.Now;
            WorkingSet.EndTime = null;
            this.NotifyStarted();
            bool success = true;
            var oldMatrixStack = (IMatrixStack<DMat4>)rtc.MatrixStack.Clone();

            try
            {
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
                        WorkingSet.LayerIndex = layerIndex;
                        WorkingSet.Layer = layer;
                        success &= NotifyBeforeLayer(layer);
                        if (!success)
                        {
                            Logger.Log(LogLevel.Error, $"marker [{Index}]: fail to mark layer at before event handler");
                            break;
                        }
                        // 레이어 펜(EntityLayerPen) 가공
                        success &= layer.Mark(this);
                        if (!success)
                            break;

                        string listOperation = $"syncAXIS layer {layerIndex}, offset {offsetIndex}, motion {WorkingSet.LayerPen.MotionType}";
                        bool listBeginAttempted = false;
                        bool listCompleted = false;
                        try
                        {
                            listBeginAttempted = true;
                            success = laser.ListBegin();
                            if (success)
                                success = rtcSyncAxis.ListBegin(WorkingSet.LayerPen.MotionType);
                            if (success)
                                success = LayerWork(offsetIndex, Offsets[offsetIndex], layerIndex, layer);
                            if (success && IsJumpToOriginAfterFinished)
                                success = rtc.ListJumpTo(DVec2.Zero);
                            if (success)
                                success = laser.ListEnd();
                            if (success)
                                success = rtc.ListEnd();
                            if (success)
                                success = rtc.ListExecute(true);
                            listCompleted = success;
                        }
                        catch (Exception ex)
                        {
                            success = false;
                            Logger.Log(LogLevel.Error, ex, $"marker [{Index}]: exception while producing RTC list for {listOperation}");
                        }
                        finally
                        {
                            if (listBeginAttempted && !listCompleted)
                                AbortIncompleteList(rtc, laser, listOperation);
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
                }
                else
                {
                    Logger.Log(LogLevel.Error, $"marker [{Index}]: mark has failed with {WorkingSet.ExecuteTime.Value.TotalSeconds:F3}s");
                }
            }
            return success;
        }
        /// <summary>
        /// Implements the marker thread for <see cref="MarkProcedures.OffsetFirst"/> procedure.
        /// <para><see cref="MarkProcedures.OffsetFirst"/> 절차에 대한 마커 스레드를 구현합니다.<br/></para>
        /// <code>
        /// </code>
        /// </summary>
        /// <remarks>
        /// <see cref="MarkProcedures.OffsetFirst">OffsetFirst</see> <br/>
        /// Mark layer1 with offset1 and offset2, ... -> Mark layer2 with offset1 and offset2, ... <br/>
        /// </remarks>
        protected virtual bool MarkerThreadOffsetFirst()
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
            WorkingSet.StartTime = DateTime.Now;
            WorkingSet.EndTime = null;

            this.NotifyStarted();
            bool success = true;
            var oldMatrixStack = (IMatrixStack<DMat4>)rtc.MatrixStack.Clone();

            try
            {
                for (int layerIndex = 0; layerIndex < layers.Count; layerIndex++)
                {
                    var layer = layers[layerIndex];
                    if (!layer.IsAllowMark)
                        continue;
                    WorkingSet.LayerIndex = layerIndex;
                    WorkingSet.Layer = layer;
                    success &= NotifyBeforeLayer(layer);
                    if (!success)
                    {
                        Logger.Log(LogLevel.Error, $"marker [{Index}]: fail to mark layer at before event handler");
                        break;
                    }
                    // 레이어 펜(EntityLayerPen) 가공
                    success &= layer.Mark(this);
                    if (!success)
                        break;

                    string listOperation = $"syncAXIS layer {layerIndex}, all offsets, motion {WorkingSet.LayerPen.MotionType}";
                    bool listBeginAttempted = false;
                    bool listCompleted = false;
                    try
                    {
                        listBeginAttempted = true;
                        success = laser.ListBegin();
                        if (success)
                            success = rtcSyncAxis.ListBegin(WorkingSet.LayerPen.MotionType);
                        if (success)
                        {
                            for (int offsetIndex = 0; offsetIndex < Offsets.Length; offsetIndex++)
                            {
                                WorkingSet.OffsetIndex = offsetIndex;
                                WorkingSet.Offset = Offsets[offsetIndex];
                                rtc.MatrixStack.Push(Offsets[offsetIndex].ToMatrix);
                                try
                                {
                                    Logger.Log(LogLevel.Debug, $"marker [{Index}]: offset index= {offsetIndex}, xyzt= {Offsets[offsetIndex].ToString()}");
                                    success = LayerWork(offsetIndex, Offsets[offsetIndex], layerIndex, layer);
                                }
                                finally
                                {
                                    rtc.MatrixStack.Pop();
                                }
                                if (!success)
                                    break;
                            }
                        }
                        if (success && IsJumpToOriginAfterFinished)
                            success = rtc.ListJumpTo(DVec2.Zero);
                        if (success)
                            success = laser.ListEnd();
                        if (success)
                            success = rtc.ListEnd();
                        if (success)
                            success = rtc.ListExecute(true);
                        listCompleted = success;
                    }
                    catch (Exception ex)
                    {
                        success = false;
                        Logger.Log(LogLevel.Error, ex, $"marker [{Index}]: exception while producing RTC list for {listOperation}");
                    }
                    finally
                    {
                        if (listBeginAttempted && !listCompleted)
                            AbortIncompleteList(rtc, laser, listOperation);
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

                    if (!success)
                        break;
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
            var rtcSyncAxis = rtc as IRtcSyncAxis;
            Debug.Assert(rtc != null);
            Debug.Assert(laser != null);
            Debug.Assert(document != null);

            bool success = true;
            success &= laserGuideControl.CtlGuide(true);
            if (!success)
                return false;

            var tuples = new List<(DVec3 realMin, DVec3 realMax)>(document.Selected.Length);
            foreach (var entity in document.Selected)
            {
                if (entity is EntityTransformBase entityTransformBase)
                {
                    if (entityTransformBase.CalculateRealMinMax(out var realMin, out var realMax))
                        tuples.Add((realMin, realMax));
                }
            }

            this.isInternalBusy = true;
            var oldMatrixStack = (IMatrixStack<DMat4>)rtc.MatrixStack.Clone();
            var oldSpeedJump = rtc.SpeedJump;
            var oldSpeedMark = rtc.SpeedMark;

            try
            {
                const string listOperation = "syncAXIS guide-laser preview, scanner-only motion";
                bool listBeginAttempted = false;
                bool listCompleted = false;
                try
                {
                    listBeginAttempted = true;
                    success = laser.ListBegin();
                    if (success)
                        success = rtcSyncAxis.ListBegin(MotionTypes.ScannerOnly);
                    if (success)
                    {
                        success = rtc.ListSpeed(SpiralLab.Sirius3.UI.Config.MarkPreviewSpeed, SpiralLab.Sirius3.UI.Config.MarkPreviewSpeed);
                        for (int j = 0; success && j < SpiralLab.Sirius3.UI.Config.MarkPreviewRepeats; j++)
                        {
                            for (int offsetIndex = 0; offsetIndex < Offsets.Length; offsetIndex++)
                            {
                                WorkingSet.Offset = Offsets[offsetIndex];
                                WorkingSet.OffsetIndex = offsetIndex;
                                rtc.MatrixStack.Push(Offsets[offsetIndex].ToMatrix);
                                try
                                {
                                    foreach (var tuple in tuples)
                                    {
                                        var realMin = tuple.realMin;
                                        var realMax = tuple.realMax;
                                        success = rtc.ListJumpTo(new DVec2(realMax.X, realMax.Y))
                                            && rtc.ListMarkTo(new DVec2(realMin.X, realMax.Y))
                                            && rtc.ListMarkTo(new DVec2(realMin.X, realMin.Y))
                                            && rtc.ListMarkTo(new DVec2(realMax.X, realMin.Y))
                                            && rtc.ListMarkTo(new DVec2(realMax.X, realMax.Y));
                                        if (!success)
                                            break;
                                    }
                                }
                                finally
                                {
                                    rtc.MatrixStack.Pop();
                                }
                                if (!success)
                                    break;
                            }
                        }
                    }
                    if (success)
                        success = rtc.ListJumpTo(DVec2.Zero);
                    if (success)
                        success = laser.ListEnd();
                    if (success)
                        success = rtc.ListEnd();
                    if (success)
                        success = rtc.ListExecute(true);
                    listCompleted = success;
                }
                catch (Exception ex)
                {
                    success = false;
                    Logger.Log(LogLevel.Error, ex, $"marker [{Index}]: exception while producing RTC list for {listOperation}");
                }
                finally
                {
                    if (listBeginAttempted && !listCompleted)
                        AbortIncompleteList(rtc, laser, listOperation);
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
    }
}
