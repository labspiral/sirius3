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
 * Description : MyMarkerRtc 
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
using OpenTK.Graphics.OpenGL;

namespace Demos
{
    /// <summary>
    /// MyMarkerRtc
    /// <para>RTC용 마커입니다.<br/></para>
    /// </summary>
    /// <remarks>
    /// Used with RTC4,4e,5,6,6e <br/>
    /// Used with <see cref="IRtc.ListBegin">IRtc.ListBegin</see> and <see cref="IRtc.ListEnd">IRtc.ListEnd</see> at each <see cref="EntityLayer">EntityLayer</see>. <br/>
    /// Supported useful features like as <see cref="MyMarkerRtc.MarkProcedures"/> and <see cref="MyMarkerRtc.MarkTargets"/>. <br/>
    /// </remarks>
    public class MyMarkerRtc
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
            /// Each layer and offset pair is generated and executed as a separate RTC list.
            /// <para>현재 오프셋에서 모든 레이어를 완료한 뒤 다음 오프셋으로 이동합니다.<br/>
            /// 레이어와 오프셋의 각 조합을 별도의 RTC 리스트로 생성하고 실행합니다.<br/></para>
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
            /// O2-L1, O2-L2, O2-L3, and six RTC lists are executed. This is the default.
            /// <para>오프셋 2개와 레이어 3개라면 O1-L1, O1-L2, O1-L3,
            /// O2-L1, O2-L2, O2-L3 순서이며 RTC 리스트를 6번 실행합니다. 기본값입니다.<br/></para>
            /// </remarks>
            /// </summary>
            LayerFirst = 0,
            /// <summary>
            /// Completes every offset for the current layer, then moves to the next layer.
            /// All offsets of one layer are generated in a single RTC list.
            /// <para>현재 레이어에서 모든 오프셋을 완료한 뒤 다음 레이어로 이동합니다.<br/>
            /// 한 레이어의 모든 오프셋을 하나의 RTC 리스트에 생성합니다.<br/></para>
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
            /// L2-O2, L3-O1, L3-O2, and three RTC lists are executed.
            /// <para>오프셋 2개와 레이어 3개라면 L1-O1, L1-O2, L2-O1,
            /// L2-O2, L3-O1, L3-O2 순서이며 RTC 리스트를 3번 실행합니다.<br/></para>
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
        /// Notify <see cref="INotifyPropertyChanged.PropertyChanged">PropertyChanged</see> event. <br/>
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
        /// Notify <see cref="INotifyPropertyChanged.PropertyChanged">PropertyChanged</see> event. <br/>
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
        /// <c>ListBufferTypes</c>
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
                this.NotifyPropertyChanged(nameof(ListBufferType));
            }
        }
        private ListBufferTypes listType;

        /// <summary>
        /// Array of <see cref="MeasurementSession">MeasurementSession</see> 
        /// <para><see cref="MeasurementSession">MeasurementSession</see> 배열<br/></para>
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
        /// </summary>
        protected ConcurrentQueue<MeasurementSession> sessionQueue = new ConcurrentQueue<MeasurementSession>();
        /// <summary>
        /// Current (or last measurement session)
        /// <para>현재(또는 마지막) 측정 세션<br/></para>
        /// </summary>
        /// <remarks>
        /// Valid when a pair of <see cref="EntityMeasurementBegin">EntityMeasurementBegin</see> and <see cref="EntityMeasurementEnd">EntityMeasurementEnd</see> has executed. <br/>
        /// Only single <see cref="MeasurementSession">MeasurementSession</see> can be exist within a <see cref="EntityLayer">EntityLayer</see>. <br/>
        /// </remarks>
        internal MeasurementSession CurrentSession { get; set; }

        /// <summary>
        /// Is plot measurement session to graph or not
        /// <para>측정 세션을 그래프로 그릴지 여부<br/></para>
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
        /// </summary>
        protected List<EntityLayer> layers;

        /// <summary>
        /// Target <c>IRtc</c> instance
        /// </summary>
        protected IRtc Rtc { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MyMarkerRtc"/> class.
        /// <para><see cref="MyMarkerRtc"/> 클래스의 새 인스턴스를 초기화합니다.<br/></para>
        /// <code>
        /// </code>
        /// </summary>
        public MyMarkerRtc()
            : base()
        {
            listType = ListBufferTypes.Auto;
            isMeasurementPlot = true;
            markTarget = MarkTargets.All;
            markProcedure = MarkProcedures.LayerFirst;

            IsCheckTempOk = false;
            IsCheckPowerOk = false;
            IsCheckPositionAck = false;

            layers = new List<EntityLayer>();
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="MyMarkerRtc"/> class with the specified index and name.
        /// <para>지정된 인덱스와 이름으로 <see cref="MyMarkerRtc"/> 클래스의 새 인스턴스를 초기화합니다.<br/></para>
        /// <code>
        /// </code>
        /// </summary>
        /// <param name="index">The index of the marker. <para>마커의 인덱스입니다.</para> </param>
        /// <param name="name">The name of the marker. <para>마커의 이름입니다.</para> </param>
        public MyMarkerRtc(int index, string name)
            : this()
        {
            Index = index;
            Name = name;
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
            if (!(scanner is IRtc) || scanner is IRtcSyncAxis)
            {
                Logger.Log(LogLevel.Error, $"marker [{Index}]: fail to ready. scanner {scanner.GetType().Name} is incompatible; expected a non-syncAXIS IRtc");
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
            if (IsCheckTempOk && !Rtc.CtlGetStatus(RtcStatus.TemperatureOK))
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

            if (IsCheckTempOk && !Rtc.CtlGetStatus(RtcStatus.TemperatureOK))
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

            if (null == Offsets || 0 == Offsets.Length)
                this.Offsets = new Offset[1] { Offset.Zero };

            PrepareActivePage();

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
            WorkingSet.StartTime = DateTime.Now;
            WorkingSet.EndTime = null;
            this.NotifyStarted();
            bool success = true;
            var oldMatrixStack = (IMatrixStack<DMat4>)rtc.MatrixStack.Clone();
            if (null != rtcMoF && rtc.IsMoF)
            {
                rtcMoF.CtlMoFOverflowClear();
                //rtcMoF.MofAngularCenter = DVec2.Zero;
            }

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
                        string listOperation = $"layer {layerIndex}, offset {offsetIndex}";
                        bool listBeginAttempted = false;
                        bool listCompleted = false;
                        try
                        {
                            listBeginAttempted = true;
                            success = rtc.ListBegin(ListBufferType);
                            if (success)
                                success = laser.ListBegin();
                            if (success)
                                success = LayerWork(offsetIndex, Offsets[offsetIndex], layerIndex, layer);
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
                        if (!success)
                            break;

                        if (null != rtcAlc && WorkingSet.LayerPen.IsALC)
                        {
                            success &= rtcAlc.CtlAlcByPositionTable(null);
                            success &= rtcAlc.CtlAlc<double>(AutoLaserControlSignals.Disabled, AutoLaserControlModes.Disabled, AutoLaserControlModeExtensions.Empty);
                        }
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

                if (null != rtcMoF)
                {
                    if (rtc.CtlGetStatus(RtcStatus.MoFOutOfRange))
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
            WorkingSet.StartTime = DateTime.Now;
            WorkingSet.EndTime = null;
            this.NotifyStarted();
            bool success = true;
            var oldMatrixStack = (IMatrixStack<DMat4>)rtc.MatrixStack.Clone();
            if (null != rtcMoF && rtc.IsMoF)
            {
                rtcMoF.CtlMoFOverflowClear();
                //rtcMoF.MofAngularCenter = DVec2.Zero;
            }

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
                    string listOperation = $"layer {layerIndex}, all offsets";
                    bool listBeginAttempted = false;
                    bool listCompleted = false;
                    try
                    {
                        listBeginAttempted = true;
                        success = rtc.ListBegin(ListBufferType);
                        if (success)
                            success = laser.ListBegin();
                        if (success)
                        {
                            for (int offsetIndex = 0; offsetIndex < Offsets.Length; offsetIndex++)
                            {
                                try
                                {
                                    WorkingSet.Offset = Offsets[offsetIndex];
                                    WorkingSet.OffsetIndex = offsetIndex;
                                    rtc.MatrixStack.Push(Offsets[offsetIndex].ToMatrix);
                                    Logger.Log(LogLevel.Debug, $"marker [{Index}]: offset index= {offsetIndex}, xyzt= {Offsets[offsetIndex].ToString()}");
                                    success = LayerWork(offsetIndex, Offsets[offsetIndex], layerIndex, layer);
                                    if (!success)
                                        break;
                                }
                                finally
                                {
                                    rtc.MatrixStack.Pop();
                                }
                            }

                            if (success && IsJumpToOriginAfterFinished)
                            {
                                if (rtc.Is3D)
                                    success = rtc3D.ListZDefocus(0) && rtc3D.ListJumpTo(DVec3.Zero);
                                else
                                    success = rtc.ListJumpTo(DVec2.Zero);
                            }
                        }
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
                    if (success)
                    {
                        if (null != CurrentSession && !CurrentSession.IsEmpty)
                        {
                            if (CurrentSession.Save(this.Scanner as IRtcMeasurement))
                                sessionQueue.Enqueue(CurrentSession);
                        }
                    }

                    if (null != rtcAlc && WorkingSet.LayerPen.IsALC)
                    {
                        success &= rtcAlc.CtlAlcByPositionTable(null);
                        success &= rtcAlc.CtlAlc<uint>(AutoLaserControlSignals.Disabled, AutoLaserControlModes.Disabled, AutoLaserControlModeExtensions.Empty, 0, 0, 0);
                    }
                    if (!success)
                        break;
                    success &= NotifyAfterLayer(layer);
                    if (!success)
                    {
                        Logger.Log(LogLevel.Error, $"marker [{Index}]: fail to mark layer at after event handler");
                        break;
                    }
                }

                if (null != rtcMoF)
                {
                    if (rtc.CtlGetStatus(RtcStatus.MoFOutOfRange))
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
                const string listOperation = "guide-laser preview";
                bool listBeginAttempted = false;
                bool listCompleted = false;
                try
                {
                    listBeginAttempted = true;
                    success = rtc.ListBegin(ListBufferTypes.Auto);
                    if (success)
                        success = laser.ListBegin();
                    if (success)
                    {
                        success = rtc.ListSpeed(SpiralLab.Sirius3.UI.Config.MarkPreviewSpeed, SpiralLab.Sirius3.UI.Config.MarkPreviewSpeed);
                        for (int j = 0; success && j < SpiralLab.Sirius3.UI.Config.MarkPreviewRepeats; j++)
                        {
                            for (int offsetIndex = 0; offsetIndex < Offsets.Length; offsetIndex++)
                            {
                                try
                                {
                                    WorkingSet.Offset = Offsets[offsetIndex];
                                    WorkingSet.OffsetIndex = offsetIndex;
                                    rtc.MatrixStack.Push(Offsets[offsetIndex].ToMatrix);

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

        /// <summary>
        /// Plots the measurement session data to a graph.
        /// <para>측정 세션 데이터를 그래프로 그립니다.<br/></para>
        /// <code>
        /// </code>
        /// </summary>
        protected virtual void NotifyPlot()
        {
            // Plot as a graph
            foreach (var session in sessionQueue)
                session.Plot();
        }
    }
}
