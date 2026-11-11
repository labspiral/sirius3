/*
 *
 * 2026 Copyright to (c)SpiralLAB. All rights reserved.
 * Description : Independent Material Design WPF multi-device editor
 * Author : hong chan, choi / hcchoi@spirallab.co.kr (http://spirallab.co.kr)
 */

using System;
using SpiralLab.Sirius3.Localization;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Windows.Controls.Primitives;
using Orientation = System.Windows.Controls.Orientation;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Win32;

using WPFMessageBox = System.Windows.MessageBox;
using WPFRadioButton = System.Windows.Controls.RadioButton;
using WPFOpenFileDialog = Microsoft.Win32.OpenFileDialog;
using WPFSaveFileDialog = Microsoft.Win32.SaveFileDialog;

using SpiralLab.Sirius3;
using SpiralLab.Sirius3.UI;
using SpiralLab.Sirius3.UI.WPF;
using SpiralLab.Sirius3.Document;
using SpiralLab.Sirius3.Entity;
using SpiralLab.Sirius3.IO;
using SpiralLab.Sirius3.Laser;
using SpiralLab.Sirius3.Marker;
using SpiralLab.Sirius3.PowerMeter;
using SpiralLab.Sirius3.Remote;
using SpiralLab.Sirius3.Scanner;
using SpiralLab.Sirius3.Scanner.Rtc;
using SpiralLab.Sirius3.Scanner.Rtc.SyncAxis;
using SpiralLab.Sirius3.View;
using SpiralLab.Sirius3.MCP;

namespace Demos
{
    /// <summary>
    /// Independent WPF editor that switches one document and its own panels among up to four device sets.
    /// Assigned documents and devices remain caller-owned; use DisposeDevices explicitly to release registered devices.
    /// <para>최대 4개 장치 세트가 문서와 자체 패널을 공유하는 독립 WPF 편집기입니다.
    /// 할당된 문서와 장치는 호출자가 소유하며 등록된 장치는 DisposeDevices로 명시적으로 해제합니다.</para>
    /// </summary>
    public partial class SiriusMultiEditorControl : IDisposable
    {
        private const int DeviceCapacity = 4;
        private readonly ToggleButton[] deviceButtons = new ToggleButton[DeviceCapacity];
        private readonly IScanner[] scanners = new IScanner[DeviceCapacity];
        private readonly ILaser[] lasers = new ILaser[DeviceCapacity];
        private readonly IMarker[] markers = new IMarker[DeviceCapacity];
        private readonly IPowerMeter[] powerMeters = new IPowerMeter[DeviceCapacity];
        private readonly IDInput[] diExt1s = new IDInput[DeviceCapacity];
        private readonly IDInput[] diLaserPorts = new IDInput[DeviceCapacity];
        private readonly IDOutput[] doExt1s = new IDOutput[DeviceCapacity];
        private readonly IDOutput[] doExt2s = new IDOutput[DeviceCapacity];
        private readonly IDOutput[] doLaserPorts = new IDOutput[DeviceCapacity];
        private readonly IRemote[] remotes = new IRemote[DeviceCapacity];
        private int maxDeviceCounts = DeviceCapacity;
        private bool changingSelection;


        #region Events
        /// <summary>Raised before switching the active device set.<para>활성 장치 세트를 전환하기 전에 발생합니다.</para></summary>
        public event Action<SiriusMultiEditorControl> OnBeforeChangeDevice;
        /// <summary>Raised after switching the active device set.<para>활성 장치 세트를 전환한 후 발생합니다.</para></summary>
        public event Action<SiriusMultiEditorControl> OnAfterChangeDevice;
        /// <summary>
        /// Raised after New button is pressed.
        /// <para>New 버튼을 누른 후 발생합니다.</para>
        /// </summary>
        public event Action<SiriusMultiEditorControl> OnAfterNew;
        /// <summary>
        /// Raised after a document file is opened.
        /// <para>문서 파일을 연 후 발생합니다.</para>
        /// </summary>
        public event Action<SiriusMultiEditorControl, string> OnAfterOpen;
        /// <summary>
        /// Raised after a document file is saved.
        /// <para>문서 파일을 저장한 후 발생합니다.</para>
        /// </summary>
        public event Action<SiriusMultiEditorControl, string> OnAfterSave;
        #endregion

        #region Dependency Properties
        /// <summary>Identifies the <see cref="AliasName"/> dependency property.</summary>
        private static readonly DependencyProperty AliasNameProperty =
            DependencyProperty.Register(nameof(AliasName), typeof(string), typeof(SiriusMultiEditorControl),
                new PropertyMetadata(string.Empty, (d, e) =>
                {
                    if (d is SiriusMultiEditorControl c && c.lblAliasName != null)
                        c.lblAliasName.Text = e.NewValue as string ?? string.Empty;
                }));

        /// <summary>Identifies the <see cref="Document"/> dependency property.</summary>
        private static readonly DependencyProperty DocumentProperty =
            DependencyProperty.Register(nameof(Document), typeof(IDocument), typeof(SiriusMultiEditorControl),
                new FrameworkPropertyMetadata(null,
                    (d, e) => ((SiriusMultiEditorControl)d).ApplyDocument(e.OldValue as IDocument, e.NewValue as IDocument),
                    (d, value) =>
                    {
                        var control = (SiriusMultiEditorControl)d;
                        if (control.disposed && value == null) return null;
                        return control.CanChangeDevices(nameof(Document)) ? value : control.Document;
                    }));

        /// <summary>Identifies the <see cref="Scanner"/> dependency property.</summary>
        private static readonly DependencyProperty ScannerProperty =
            DependencyProperty.Register(nameof(Scanner), typeof(IScanner), typeof(SiriusMultiEditorControl),
                new PropertyMetadata(null, (d, e) =>
                {
                    if (d is SiriusMultiEditorControl c)
                        c.ApplyScanner(e.OldValue as IScanner, e.NewValue as IScanner);
                }));

        /// <summary>Identifies the <see cref="Laser"/> dependency property.</summary>
        private static readonly DependencyProperty LaserProperty =
            DependencyProperty.Register(nameof(Laser), typeof(ILaser), typeof(SiriusMultiEditorControl),
                new PropertyMetadata(null, (d, e) =>
                {
                    if (d is SiriusMultiEditorControl c)
                        c.ApplyLaser(e.OldValue as ILaser, e.NewValue as ILaser);
                }));

        /// <summary>Identifies the <see cref="Marker"/> dependency property.</summary>
        private static readonly DependencyProperty MarkerProperty =
            DependencyProperty.Register(nameof(Marker), typeof(IMarker), typeof(SiriusMultiEditorControl),
                new PropertyMetadata(null, (d, e) =>
                {
                    if (d is SiriusMultiEditorControl c)
                        c.ApplyMarker(e.OldValue as IMarker, e.NewValue as IMarker);
                }));

        /// <summary>Identifies the <see cref="PowerMeter"/> dependency property.</summary>
        private static readonly DependencyProperty PowerMeterProperty =
            DependencyProperty.Register(nameof(PowerMeter), typeof(IPowerMeter), typeof(SiriusMultiEditorControl),
                new PropertyMetadata(null, (d, e) => ((SiriusMultiEditorControl)d).ApplyPowerMeter(e.OldValue as IPowerMeter, e.NewValue as IPowerMeter)));

        /// <summary>Identifies the <see cref="DIExt1"/> dependency property.</summary>
        private static readonly DependencyProperty DIExt1Property = RegisterDeviceProperty<IDInput>(nameof(DIExt1), (c, value) => { c.diext1 = value; if (c.DIRtcCtrl != null) c.DIRtcCtrl.DIExt1 = value; });
        /// <summary>Identifies the <see cref="DILaserPort"/> dependency property.</summary>
        private static readonly DependencyProperty DILaserPortProperty = RegisterDeviceProperty<IDInput>(nameof(DILaserPort), (c, value) => { c.dilaserport = value; if (c.DIRtcCtrl != null) c.DIRtcCtrl.DILaserPort = value; });
        /// <summary>Identifies the <see cref="DOExt1"/> dependency property.</summary>
        private static readonly DependencyProperty DOExt1Property = RegisterDeviceProperty<IDOutput>(nameof(DOExt1), (c, value) => { c.doext1 = value; if (c.DORtcCtrl != null) c.DORtcCtrl.DOExt1 = value; });
        /// <summary>Identifies the <see cref="DOExt2"/> dependency property.</summary>
        private static readonly DependencyProperty DOExt2Property = RegisterDeviceProperty<IDOutput>(nameof(DOExt2), (c, value) => { c.doext2 = value; if (c.DORtcCtrl != null) c.DORtcCtrl.DOExt2 = value; });
        /// <summary>Identifies the <see cref="DOLaserPort"/> dependency property.</summary>
        private static readonly DependencyProperty DOLaserPortProperty = RegisterDeviceProperty<IDOutput>(nameof(DOLaserPort), (c, value) => { c.dolaserport = value; if (c.DORtcCtrl != null) c.DORtcCtrl.DOLaserPort = value; });

        /// <summary>Identifies the <see cref="Remote"/> dependency property.</summary>
        private static readonly DependencyProperty RemoteProperty =
            DependencyProperty.Register(nameof(Remote), typeof(IRemote), typeof(SiriusMultiEditorControl),
                new PropertyMetadata(null, (d, e) => ((SiriusMultiEditorControl)d).ApplyRemote(e.OldValue as IRemote, e.NewValue as IRemote)));

        /// <summary>Identifies the <see cref="MCPServer"/> dependency property.</summary>
        private static readonly DependencyProperty MCPServerProperty =
            DependencyProperty.Register(nameof(MCPServer), typeof(IMCPServer), typeof(SiriusMultiEditorControl),
                new PropertyMetadata(null, (d, e) => ((SiriusMultiEditorControl)d).ApplyMCPServer(e.NewValue as IMCPServer)));

        private static DependencyProperty RegisterDeviceProperty<T>(string name, Action<SiriusMultiEditorControl, T> apply) where T : class
        {
            return DependencyProperty.Register(name, typeof(T), typeof(SiriusMultiEditorControl),
                new PropertyMetadata(null, (d, e) => apply((SiriusMultiEditorControl)d, e.NewValue as T)));
        }
        #endregion

        #region CLR Properties
        /// <summary>
        /// Gets or sets the alias name shown in the status bar.
        /// <para>상태 표시줄에 표시되는 별칭 이름을 가져오거나 설정합니다.</para>
        /// </summary>
        [Category("Sirius3")] [DisplayName("Alias")]
        public string AliasName
        {
            get => (string)GetValue(AliasNameProperty);
            set => SetValue(AliasNameProperty, value);
        }

        /// <summary>
        /// Gets or sets the current document.
        /// <para>현재 문서를 가져오거나 설정합니다.</para>
        /// </summary>
        [Category("Sirius3")] [DisplayName("Document")]
        public IDocument Document
        {
            get => (IDocument)GetValue(DocumentProperty);
            set => SetValue(DocumentProperty, value);
        }

        /// <summary>Gets the current rendering view from the inner EditorControl.</summary>
        [Browsable(false)]
        public IView View => editorControl?.View;

        /// <summary>
        /// Gets or sets the scanner.
        /// <para>스캐너를 가져오거나 설정합니다.</para>
        /// </summary>
        [Category("Sirius3")] [DisplayName("Scanner")]
        public IScanner Scanner
        {
            get => scanners[CurrentDeviceIndex];
            private set => SetValue(ScannerProperty, value);
        }

        /// <summary>
        /// Gets or sets the laser.
        /// <para>레이저를 가져오거나 설정합니다.</para>
        /// </summary>
        [Category("Sirius3")] [DisplayName("Laser")]
        public ILaser Laser
        {
            get => lasers[CurrentDeviceIndex];
            private set => SetValue(LaserProperty, value);
        }

        /// <summary>
        /// Gets or sets the marker.
        /// <para>마커를 가져오거나 설정합니다.</para>
        /// </summary>
        [Category("Sirius3")] [DisplayName("Marker")]
        public IMarker Marker
        {
            get => markers[CurrentDeviceIndex];
            private set => SetValue(MarkerProperty, value);
        }

        /// <summary>Gets or sets the power meter.</summary>
        [Browsable(false)]
        public IPowerMeter PowerMeter
        {
            get => powerMeters[CurrentDeviceIndex];
            private set => SetValue(PowerMeterProperty, value);
        }

        /// <summary>Gets or sets DI Extension 1.</summary>
        [Browsable(false)]
        public IDInput DIExt1
        {
            get => diExt1s[CurrentDeviceIndex];
            private set => SetValue(DIExt1Property, value);
        }
        /// <summary>Gets or sets DI Laser Port.</summary>
        [Browsable(false)]
        public IDInput DILaserPort
        {
            get => diLaserPorts[CurrentDeviceIndex];
            private set => SetValue(DILaserPortProperty, value);
        }
        /// <summary>Gets or sets DO Extension 1.</summary>
        [Browsable(false)]
        public IDOutput DOExt1
        {
            get => doExt1s[CurrentDeviceIndex];
            private set => SetValue(DOExt1Property, value);
        }
        /// <summary>Gets or sets DO Extension 2.</summary>
        [Browsable(false)]
        public IDOutput DOExt2
        {
            get => doExt2s[CurrentDeviceIndex];
            private set => SetValue(DOExt2Property, value);
        }
        /// <summary>Gets or sets DO Laser Port.</summary>
        [Browsable(false)]
        public IDOutput DOLaserPort
        {
            get => doLaserPorts[CurrentDeviceIndex];
            private set => SetValue(DOLaserPortProperty, value);
        }

        /// <summary>Gets or sets the remote control interface.</summary>
        [Browsable(false)]
        public IRemote Remote
        {
            get => remotes[CurrentDeviceIndex];
            private set => SetValue(RemoteProperty, value);
        }

        /// <summary>
        /// Gets or sets the MCP server created for this editor. The assignment enables UI control and transfers disposal responsibility to <see cref="DisposeDevices"/>.
        /// <para>이 편집기를 대상으로 생성된 MCP 서버를 가져오거나 설정합니다. 지정하면 UI 제어를 사용하며 <see cref="DisposeDevices"/>가 서버 해제를 담당합니다.</para>
        /// </summary>
        [LocalizedCategory("MCP")]
        [LocalizedDisplayName("MCPServer")]
        [LocalizedDescription("MCPServer")]
        public IMCPServer MCPServer
        {
            get => (IMCPServer)GetValue(MCPServerProperty);
            set => SetValue(MCPServerProperty, value);
        }

        /// <summary>Gets the inner WPF <see cref="EditorControl"/>.</summary>
        [Browsable(false)]
        public EditorControl EditorCtrl => editorControl;

        /// <summary>Gets or sets the assembly-owned accessory shown above the main navigation, aligned to its left edge.</summary>


        /// <summary>Gets the property grid control wrapper.</summary>
        [Browsable(false)]
        public PropertyGridControl PropertyGridCtrl => propertyGridControl1;
        /// <summary>Gets the native WPF page tree controls.</summary>
        [Browsable(false)]
        public TreeViewPageControl[] PageCtrls => new[] { treeViewPageControl1, treeViewPageControl2, treeViewPageControl3, treeViewPageControl4 };
        /// <summary>Gets the native WPF block tree control.</summary>
        [Browsable(false)]
        public TreeViewBlockControl BlockCtrl => treeViewBlockControl1;
        /// <summary>Gets the laser control wrapper.</summary>
        [Browsable(false)]
        public LaserControl LaserCtrl => laserControl1;
        /// <summary>Gets the RTC control wrapper.</summary>
        [Browsable(false)]
        public ScannerControl ScannerCtrl => scannerControl1;
        /// <summary>Gets the marker control wrapper.</summary>
        [Browsable(false)]
        public MarkerControl MarkerCtrl => markerControl1;
        /// <summary>Gets the RTC DI control wrapper.</summary>
        [Browsable(false)]
        public DIRtcControl DIRtcCtrl => rtcDIControl1;
        /// <summary>Gets the RTC DO control wrapper.</summary>
        [Browsable(false)]
        public DORtcControl DORtcCtrl => rtcDOControl1;
        /// <summary>Gets the manual control wrapper.</summary>
        [Browsable(false)]
        public ManualControl ManualCtrl => manualControl1;
        /// <summary>Gets the power meter control wrapper.</summary>
        [Browsable(false)]
        public PowerMeterControl PowerMeterCtrl => powerMeterControl1;
        /// <summary>Gets the power map control wrapper.</summary>
        [Browsable(false)]
        public PowerMapControl PowerMapCtrl => powerMapControl1;
        /// <summary>Gets the stepper control wrapper.</summary>
        [Browsable(false)]
        public StepperControl StepperCtrl => stepperControl1;
        /// <summary>Gets the entity pen control wrapper.</summary>
        [Browsable(false)]
        public EntityPenControl EntityPenCtrl => entityPenControl1;
        /// <summary>Gets the layer pen control wrapper.</summary>
        [Browsable(false)]
        public LayerPenControl LayerPenCtrl => layerPenControl1;
        /// <summary>Gets the remote control wrapper.</summary>
        [Browsable(false)]
        public RemoteControl RemoteCtrl => remoteControl1;
        /// <summary>Gets the log control.</summary>
        [Browsable(false)]
        public LogControl LogCtrl => logControl1;
        /// <summary>Gets or sets whether the bottom log panel is visible.</summary>
        public bool IsShowLogWindow { get => isShowLogWindow; set => ShowLogWindow(value); }
        /// <summary>Gets or sets whether the left tree-and-pen pane is visible.</summary>
        public bool IsShowTreeViewAndPen { get => isShowTreeViewAndPen; set => ShowTreeViewAndPens(value); }
        /// <summary>Gets or sets whether the pen tabs are visible inside the left pane.</summary>
        public bool IsShowPen { get => isShowPen; set => ShowPens(value); }
        /// <summary>Gets or sets whether the right property pane is visible.</summary>
        public bool IsPropertyGridWindow { get => isPropertyGridWindow; set => ShowPropertyWindow(value); }
        #endregion

        /// <summary>Gets or sets the device-set count (1–4). Changes are rejected while a registered marker is busy.
        /// <para>장치 세트 수(1~4)를 설정합니다. 등록된 마커가 가공 중이면 변경하지 않습니다.</para></summary>
        /// <exception cref="ArgumentOutOfRangeException">The count is outside 1–4.<br/>장치 수가 1~4 범위를 벗어났습니다.</exception>
        public int MaxDeviceCounts
        {
            get => maxDeviceCounts;
            set
            {
                if (value < 1 || value > DeviceCapacity) throw new ArgumentOutOfRangeException(nameof(value));
                if (value == maxDeviceCounts || !CanChangeDevices(nameof(MaxDeviceCounts)))
                    return;
                if (CurrentDeviceIndex >= value)
                {
                    Document?.ActSimulateStop(false);
                    if (!CanChangeDevices(nameof(MaxDeviceCounts)))
                        return;
                }
                maxDeviceCounts = value;
                if (CurrentDeviceIndex >= value)
                {
                    CurrentDeviceIndex = 0;
                    ApplyCurrentDevices();
                }
                RefreshSelector();
            }
        }

        /// <summary>Gets the active device-set index.</summary>
        public int CurrentDeviceIndex { get; protected set; }

        /// <summary>Gets registered scanners.</summary>
        [Browsable(false)] public IScanner[] Scanners => scanners;
        /// <summary>Gets registered lasers.</summary>
        [Browsable(false)] public ILaser[] Lasers => lasers;
        /// <summary>Gets registered markers.</summary>
        [Browsable(false)] public IMarker[] Markers => markers;
        /// <summary>Gets registered power meters.</summary>
        [Browsable(false)] public IPowerMeter[] PowerMeters => powerMeters;
        /// <summary>Gets registered Extension 1 inputs.</summary>
        [Browsable(false)] public IDInput[] DIExt1s => diExt1s;
        /// <summary>Gets registered laser-port inputs.</summary>
        [Browsable(false)] public IDInput[] DILaserPorts => diLaserPorts;
        /// <summary>Gets registered Extension 1 outputs.</summary>
        [Browsable(false)] public IDOutput[] DOExt1s => doExt1s;
        /// <summary>Gets registered Extension 2 outputs.</summary>
        [Browsable(false)] public IDOutput[] DOExt2s => doExt2s;
        /// <summary>Gets registered laser-port outputs.</summary>
        [Browsable(false)] public IDOutput[] DOLaserPorts => doLaserPorts;
        /// <summary>Gets registered remotes.</summary>
        [Browsable(false)] public IRemote[] Remotes => remotes;


        /// <summary>Gets this independently implemented multi-editor host. Use EditorCtrl for its editor surface.
        /// <para>독립적으로 구현된 다중 편집기 호스트를 가져옵니다. 편집 화면은 EditorCtrl을 사용합니다.</para></summary>
        [Browsable(false)] public SiriusMultiEditorControl EditorHost => this;

        #region Fields
        private volatile IPowerMeter powerMeter;
        private volatile IRtcMoF encoderSource;
        private enum PowerReadoutState { Started, Stopped, Measured, Cleared }
        private readonly LatestValueDispatcher<(IPowerMeter Source, PowerReadoutState State, MeasureUnits Unit, double Value)> powerReadoutUpdates;
        private readonly LatestValueDispatcher<(IRtcMoF Source, int X, int Y, double XValue, double YValue)> encoderReadoutUpdates;
        private IRemote     remote;
        private IDInput     diext1;
        private IDInput     dilaserport;
        private IDOutput    doext1;
        private IDOutput    doext2;
        private IDOutput    dolaserport;

        private readonly DispatcherTimer timerStatus    = new DispatcherTimer();
        private readonly DispatcherTimer timerProgress  = new DispatcherTimer();
        private readonly Stopwatch       swProgress     = new Stopwatch();
        private int timerStatusColorCounts;
        private int timerProgressColorCounts;
        private bool isLocked = false;
        private bool isEditEnabled = true;
        private bool isMultiMarkerBusy;
        private bool isSynchronizingPageTab;
        private bool isShowLogWindow = false;
        private bool isShowTreeViewAndPen = true;
        private bool isShowPen = true;
        private bool isPropertyGridWindow = true;
        private GridLength visibleLeftWidth = new GridLength(250);
        private GridLength visibleRightWidth = new GridLength(250);
        private GridLength visibleLogHeight = new GridLength(1, GridUnitType.Star);
        private GridLength visiblePensHeight = new GridLength(1, GridUnitType.Star);
        private bool disposed;

        #endregion
        #region Constructor
        /// <summary>
        /// Initializes a new instance of <see cref="SiriusMultiEditorControl"/>.
        /// <para><see cref="SiriusMultiEditorControl"/> 인스턴스를 초기화합니다.</para>
        /// </summary>

        public SiriusMultiEditorControl()
        {
            WPFThemeManager.Initialize();
            InitializeComponent();
            ApplyLocalization();
            topAccessoryHost.Content = CreateDeviceSelector();
            RefreshSelector();

            powerReadoutUpdates = new(
                QueueReadoutUpdate, ApplyPowerReadout, update => ReferenceEquals(update.Source, powerMeter));
            encoderReadoutUpdates = new(
                QueueReadoutUpdate, ApplyEncoderReadout, update => ReferenceEquals(update.Source, encoderSource));

            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            this.Loaded   += OnLoaded;
            this.Unloaded += OnUnloaded;
            this.PreviewKeyDown += OnPreviewKeyDown;

            timerStatus.Interval   = TimeSpan.FromMilliseconds(200);
            timerStatus.Tick       += TimerStatus_Tick;

            timerProgress.Interval = TimeSpan.FromMilliseconds(100);
            timerProgress.Tick     += TimerProgress_Tick;

            // Wire encoder label double-click
            lblEncoder.MouseLeftButtonDown += LblEncoder_MouseDoubleClick;
            lblEncoder.ToolTipOpening += LblEncoder_ToolTipOpening;

            // Start with a default document
            var doc = new DocumentBase();
            Document = doc;

        }

        private void ApplyLocalization()
        {
            btnNew.ToolTip = MessageBoxLocalization.S("SiriusEditor_NewDocument");
            btnOpen.ToolTip = MessageBoxLocalization.S("SiriusEditor_OpenDocument");
            mnuDocumentOptions.ToolTip = MessageBoxLocalization.S("SiriusEditor_DocumentOptions");
            ApplyIncludePageLocalization(mnuIncludePage1, 1);
            ApplyIncludePageLocalization(mnuIncludePage2, 2);
            ApplyIncludePageLocalization(mnuIncludePage3, 3);
            ApplyIncludePageLocalization(mnuIncludePage4, 4);
            ApplyIncludeLocalization(mnuIncludeBlocks, "SiriusEditor_Blocks", "SiriusEditor_IncludeBlocks");
            ApplyIncludeLocalization(mnuIncludeLayerPens, "SiriusEditor_LayerPens", "SiriusEditor_IncludeLayerPens");
            ApplyIncludeLocalization(mnuIncludeEntityPens, "SiriusEditor_EntityPens", "SiriusEditor_IncludeEntityPens");
            ApplyIncludeLocalization(mnuIncludeWafers, "SiriusEditor_Wafers", "SiriusEditor_IncludeWafers");
            ApplyIncludeLocalization(mnuIncludeSubstrates, "SiriusEditor_Substrates", "SiriusEditor_IncludeSubstrates");
            btnSave.ToolTip = MessageBoxLocalization.S("SiriusEditor_SaveDocument");
            btnLock.ToolTip = MessageBoxLocalization.S("SiriusEditor_LockEditing");
            btnLogWindow.ToolTip = MessageBoxLocalization.S("SiriusEditor_ToggleLogWindow");
            txtTabBlock.Text = MessageBoxLocalization.S("SiriusEditor_NavBlock");
            txtTabEntityPen.Text = MessageBoxLocalization.S("SiriusEditor_NavEntity");
            txtTabLayerPen.Text = MessageBoxLocalization.S("SiriusEditor_NavLayer");
            txtTabEditor.Text = MessageBoxLocalization.S("SiriusEditor_NavEditor");
            txtTabMarker.Text = MessageBoxLocalization.S("SiriusEditor_NavMarker");
            txtTabManual.Text = MessageBoxLocalization.S("SiriusEditor_NavManual");
            txtTabScanner.Text = MessageBoxLocalization.S("SiriusEditor_NavScanner");
            txtTabLaser.Text = MessageBoxLocalization.S("SiriusEditor_NavLaser");
            txtTabDio.Text = MessageBoxLocalization.S("SiriusEditor_NavDio");
            txtTabPower.Text = MessageBoxLocalization.S("SiriusEditor_NavPower");
            txtTabPowerMeter.Text = MessageBoxLocalization.S("SiriusEditor_NavPowerMeter");
            txtTabPowerMap.Text = MessageBoxLocalization.S("SiriusEditor_NavPowerMap");
            txtTabStepper.Text = MessageBoxLocalization.S("SiriusEditor_NavStepper");
            txtTabRemote.Text = MessageBoxLocalization.S("SiriusEditor_NavRemote");
            txtTabProperty.Text = MessageBoxLocalization.S("SiriusEditor_NavProperty");
            lblAliasName.ToolTip = MessageBoxLocalization.S("SiriusEditor_StatusName");
            lblProcessTime.ToolTip = MessageBoxLocalization.S("SiriusEditor_StatusProcessingTime");
            lblPowerWatt.ToolTip = MessageBoxLocalization.S("SiriusEditor_StatusMeasuredPower");
            lblFileName.ToolTip = MessageBoxLocalization.S("SiriusEditor_StatusFileName");
            lblEncoder.ToolTip = MessageBoxLocalization.S("SiriusEditor_StatusEncoder");
            lblReady.Text = MessageBoxLocalization.S("MultiBeam_ReadyStatus");
            lblReady.ToolTip = MessageBoxLocalization.S("SiriusEditor_StatusReady");
            lblBusy.Text = MessageBoxLocalization.S("MultiBeam_BusyStatus");
            lblBusy.ToolTip = MessageBoxLocalization.S("SiriusEditor_StatusBusy");
            lblError.Text = MessageBoxLocalization.S("MultiBeam_ErrorStatus");
            lblError.ToolTip = MessageBoxLocalization.S("SiriusEditor_StatusError");
            lblRemote.ToolTip = MessageBoxLocalization.S("SiriusEditor_StatusRemote");
            lblRemote.Text = $" {Internal.RemoteStatusLocalization.Format(RemoteControlModes.Local, false)} ";
        }

        private static void ApplyIncludePageLocalization(System.Windows.Controls.MenuItem item, int page)
        {
            item.Header = MessageBoxLocalization.S("SiriusEditor_Page", page);
            item.ToolTip = MessageBoxLocalization.S("SiriusEditor_IncludePage", page);
        }

        private static void ApplyIncludeLocalization(System.Windows.Controls.MenuItem item, string headerKey, string toolTipKey)
        {
            item.Header = MessageBoxLocalization.S(headerKey);
            item.ToolTip = MessageBoxLocalization.S(toolTipKey);
        }

        #endregion

        #region Lifecycle
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(this))
                return;
            timerStatus.Start();
            Marker?.Ready(Document, View, Scanner as IRtc, Laser, powerMeter);

            if (treeViewPageControl1 != null) treeViewPageControl1.View = View;
            if (treeViewBlockControl1 != null) treeViewBlockControl1.View = View;
            if (treeViewPageControl2 != null) treeViewPageControl2.View = View;
            if (treeViewPageControl3 != null) treeViewPageControl3.View = View;
            if (treeViewPageControl4 != null) treeViewPageControl4.View = View;
            ApplyTreeMarker(Marker);
            if (propertyGridControl1 != null) propertyGridControl1.View = View;
            if (MarkerCtrl != null) MarkerCtrl.View = View;
            RefreshMarkerState();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            timerStatus.Stop();
            timerProgress.Stop();
        }

        #endregion

        #region DP Apply Callbacks
        private void ApplyDocument(IDocument oldDoc, IDocument newDoc)
        {
            if (DesignerProperties.GetIsInDesignMode(this))
                return;
            oldDoc?.ActSimulateStop(false);
            if (oldDoc != null)
            {
                oldDoc.OnNew            -= Document_OnNew;
                oldDoc.OnBeforeOpen     -= Document_OnBeforeOpen;
                oldDoc.OnAfterOpen      -= Document_OnAfterOpen;
                oldDoc.OnBeforeSave     -= Document_OnBeforeSave;
                oldDoc.OnAfterSave      -= Document_OnAfterSave;
                oldDoc.OnPageChanged    -= Document_OnPageChanged;
                oldDoc.OnSimulationStarted -= Document_OnSimulationStarted;
                oldDoc.OnSimulationEnded -= Document_OnSimulationEnded;
            }

            if (editorControl != null)
                editorControl.Document = newDoc;

            if (newDoc != null)
            {
                newDoc.OnNew            += Document_OnNew;
                newDoc.OnBeforeOpen     += Document_OnBeforeOpen;
                newDoc.OnAfterOpen      += Document_OnAfterOpen;
                newDoc.OnBeforeSave     += Document_OnBeforeSave;
                newDoc.OnAfterSave      += Document_OnAfterSave;
                newDoc.OnPageChanged    += Document_OnPageChanged;
                newDoc.OnSimulationStarted += Document_OnSimulationStarted;
                newDoc.OnSimulationEnded += Document_OnSimulationEnded;
            }

            if (treeViewPageControl1 != null) treeViewPageControl1.Document = newDoc;
            if (treeViewBlockControl1 != null) treeViewBlockControl1.Document = newDoc;
            if (treeViewPageControl2 != null) treeViewPageControl2.Document = newDoc;
            if (treeViewPageControl3 != null) treeViewPageControl3.Document = newDoc;
            if (treeViewPageControl4 != null) treeViewPageControl4.Document = newDoc;
            // treeViewWaferControl1.Document = newDoc;
            // treeViewSubstrateControl1.Document = newDoc;
            if (entityPenControl1 != null) entityPenControl1.Document = newDoc;
            if (layerPenControl1 != null) layerPenControl1.Document = newDoc;
            if (propertyGridControl1 != null) propertyGridControl1.Document = newDoc;
            if (MarkerCtrl != null) MarkerCtrl.Document = newDoc;
            if (PowerMapCtrl != null) PowerMapCtrl.Document = newDoc;
            if (newDoc != null)
                for (var index = 0; index < DeviceCapacity; index++)
                    markers[index]?.Ready(newDoc, View, scanners[index], lasers[index], powerMeters[index]);
            ApplyEditPermission();
        }

        private void ApplyScanner(IScanner oldScanner, IScanner newScanner)
        {
            if (DesignerProperties.GetIsInDesignMode(this))
                return;
            if (oldScanner is IRtcMoF oldMof)
                oldMof.OnEncoderChanged -= MoF_OnEncoderChanged;
            encoderSource = newScanner is IRtc rtc && rtc.IsMoF ? newScanner as IRtcMoF : null;
            encoderReadoutUpdates?.Reset();
            lblEncoder.Text = encoderSource != null ? "ENC: ?" : string.Empty;

            if (editorControl != null)
                editorControl.Scanner = newScanner;

            if (encoderSource is IRtcMoF newMof)
            {
                newMof.OnEncoderChanged += MoF_OnEncoderChanged;
                lblEncoder.Visibility = Visibility.Visible;
            }
            else
            {
                lblEncoder.Visibility = Visibility.Collapsed;
            }

            if (ScannerCtrl != null) ScannerCtrl.Scanner = newScanner;
            if (LaserCtrl != null) LaserCtrl.Scanner = newScanner;
            if (MarkerCtrl != null) MarkerCtrl.Scanner = newScanner;
            if (ManualCtrl != null) ManualCtrl.Scanner = newScanner;
            if (DIRtcCtrl != null) DIRtcCtrl.Scanner = newScanner;
            if (DORtcCtrl != null) DORtcCtrl.Scanner = newScanner;
            if (PowerMapCtrl != null) PowerMapCtrl.Scanner = newScanner;
            if (StepperCtrl != null) StepperCtrl.Stepper = newScanner as IRtcStepper;
            PageVisibility(newScanner);

            if (newScanner != null)
            {
                UpdatePens(newScanner, Laser);
                PropertyVisibility(newScanner, Laser);
            }
        }

        private void ApplyLaser(ILaser oldLaser, ILaser newLaser)
        {
            if (DesignerProperties.GetIsInDesignMode(this))
                return;
            if (editorControl != null)
                editorControl.Laser = newLaser;

            if (newLaser != null)
            {
                newLaser.Scanner = Scanner;
                UpdatePens(Scanner, newLaser);
                UpdatePowerMeterLaser(newLaser);
                PropertyVisibility(Scanner, newLaser);
            }

            if (LaserCtrl != null) LaserCtrl.Laser = newLaser;
            if (MarkerCtrl != null) MarkerCtrl.Laser = newLaser;
            if (ManualCtrl != null) ManualCtrl.Laser = newLaser;
            if (PowerMeterCtrl != null) PowerMeterCtrl.Laser = newLaser;
            if (PowerMapCtrl != null) PowerMapCtrl.Laser = newLaser;
            if (EntityPenCtrl != null) EntityPenCtrl.Document = Document;
        }

        private void ApplyMarker(IMarker oldMarker, IMarker newMarker)
        {
            if (DesignerProperties.GetIsInDesignMode(this))
                return;
            if (oldMarker != null)
            {
                oldMarker.OnStarted -= Marker_OnStarted;
                oldMarker.OnEnded   -= Marker_OnEnded;
                oldMarker.PropertyChanged -= Marker_PropertyChanged;
            }

            if (editorControl != null)
                editorControl.Marker = newMarker;
            if (propertyGridControl1 != null) propertyGridControl1.Marker = newMarker;
            ApplyTreeMarker(newMarker);

            if (newMarker != null)
            {
                newMarker.OnStarted += Marker_OnStarted;
                newMarker.OnEnded   += Marker_OnEnded;
                newMarker.PropertyChanged += Marker_PropertyChanged;
            }

            if (MarkerCtrl != null) MarkerCtrl.Marker = newMarker;
            if (ManualCtrl != null) ManualCtrl.Marker = newMarker;
            if (DORtcCtrl != null) DORtcCtrl.Marker = newMarker;
            if (RemoteCtrl != null) RemoteCtrl.Marker = newMarker;
            timerProgress.Stop();
            swProgress.Reset();
            UpdateMarkerStatus();
            RefreshMarkerState();
        }

        private void ApplyPowerMeter(IPowerMeter oldPm, IPowerMeter newPm)
        {
            if (oldPm != null)
            {
                oldPm.OnStarted  -= PowerMeter_OnStarted;
                oldPm.OnStopped  -= PowerMeter_OnStopped;
                oldPm.OnMeasured -= PowerMeter_OnMeasured;
                oldPm.OnCleared  -= PowerMeter_OnCleared;
            }
            powerMeter = newPm;
            powerReadoutUpdates?.Reset();
            lblPowerWatt.Text = string.Empty;
            UpdatePowerMeterLaser(Laser);

            if (PowerMeterCtrl != null) PowerMeterCtrl.PowerMeter = newPm;
            if (PowerMapCtrl != null) PowerMapCtrl.PowerMeter = newPm;
            if (MarkerCtrl != null) MarkerCtrl.PowerMeter = newPm;

            if (newPm != null)
            {
                lblPowerWatt.Text = "0.0 W";
                newPm.OnStarted  += PowerMeter_OnStarted;
                newPm.OnStopped  += PowerMeter_OnStopped;
                newPm.OnMeasured += PowerMeter_OnMeasured;
                newPm.OnCleared  += PowerMeter_OnCleared;
            }
        }

        private void ApplyRemote(IRemote oldRemote, IRemote newRemote)
        {
            if (oldRemote != null)
                oldRemote.OnModeChanged -= Remote_OnModeChanged;

            remote = newRemote;
            if (RemoteCtrl != null) RemoteCtrl.Remote = newRemote;
            if (RemoteCtrl != null) RemoteCtrl.Marker = Marker;

            if (newRemote != null)
            {
                newRemote.OnModeChanged += Remote_OnModeChanged;
            }
            UpdateRemoteTabVisibility();
        }

        private void ApplyMCPServer(IMCPServer server)
        {
            if (RemoteCtrl != null) RemoteCtrl.MCPServer = server;
            UpdateRemoteTabVisibility();
        }

        private void UpdateRemoteTabVisibility()
        {
            var visible = remote != null;
            visible |= MCPServer != null;
            if (tabRemote != null)
                tabRemote.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }
        #endregion

        #region Public Methods
        /// <summary>Shows or hides the bottom log panel.</summary>
        public void ShowLogWindow(bool show)
        {
            if (rowLog == null || logControl1 == null)
                return;
            if (!show && rowLog.Height.Value > 0)
                visibleLogHeight = rowLog.Height;
            rowLog.Height = show ? EnsureVisibleLength(visibleLogHeight, new GridLength(1, GridUnitType.Star)) : new GridLength(0);
            if (logSplitter != null)
                logSplitter.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            logControl1.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            isShowLogWindow = show;
        }

        private void ApplyTreeMarker(IMarker marker)
        {
            if (treeViewPageControl1 != null) treeViewPageControl1.Marker = marker;
            if (treeViewPageControl2 != null) treeViewPageControl2.Marker = marker;
            if (treeViewPageControl3 != null) treeViewPageControl3.Marker = marker;
            if (treeViewPageControl4 != null) treeViewPageControl4.Marker = marker;
            if (treeViewBlockControl1 != null) treeViewBlockControl1.Marker = marker;
        }

        /// <summary>Shows or hides the complete left tree-and-pen pane.</summary>
        public void ShowTreeViewAndPens(bool show)
        {
            if (colLeft == null || colLeftSplitter == null)
                return;
            if (!show && colLeft.Width.Value > 0)
                visibleLeftWidth = colLeft.Width;
            colLeft.Width = show ? EnsureVisibleLength(visibleLeftWidth, new GridLength(250)) : new GridLength(0);
            colLeftSplitter.Width = show ? new GridLength(3) : new GridLength(0);
            isShowTreeViewAndPen = show;
        }

        /// <summary>Shows or hides the pen tabs while leaving the page and block trees visible.</summary>
        public void ShowPens(bool show)
        {
            if (rowPens == null || rowPensSplitter == null || tabPens == null)
                return;
            if (!show && rowPens.Height.Value > 0)
                visiblePensHeight = rowPens.Height;
            rowPens.Height = show ? EnsureVisibleLength(visiblePensHeight, new GridLength(1, GridUnitType.Star)) : new GridLength(0);
            rowPensSplitter.Height = show ? new GridLength(3) : new GridLength(0);
            tabPens.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            isShowPen = show;
        }

        /// <summary>Shows or hides the right property pane.</summary>
        public void ShowPropertyWindow(bool show)
        {
            if (colRight == null || colRightSplitter == null)
                return;
            if (!show && colRight.Width.Value > 0)
                visibleRightWidth = colRight.Width;
            colRight.Width = show ? EnsureVisibleLength(visibleRightWidth, new GridLength(250)) : new GridLength(0);
            colRightSplitter.Width = show ? new GridLength(3) : new GridLength(0);
            isPropertyGridWindow = show;
        }

        private static GridLength EnsureVisibleLength(GridLength value, GridLength fallback) => value.Value > 0 ? value : fallback;

        /// <summary>Registers one caller-owned device set. Registered and incoming markers must be idle.
        /// <para>호출자 소유 장치 세트를 등록합니다. 기존 마커와 새 마커가 모두 유휴 상태여야 합니다.</para></summary>
        /// <param name="index">Device-set index.<br/>장치 세트 인덱스입니다.</param>
        /// <param name="scanner">Scanner.<br/>스캐너입니다.</param>
        /// <param name="laser">Laser.<br/>레이저입니다.</param>
        /// <param name="powerMeter">Optional power meter.<br/>선택적 파워미터입니다.</param>
        /// <param name="diExt1">Extension 1 input.<br/>Extension 1 입력입니다.</param>
        /// <param name="diLaserPort">Laser-port input.<br/>레이저 포트 입력입니다.</param>
        /// <param name="doExt1">Extension 1 output.<br/>Extension 1 출력입니다.</param>
        /// <param name="doExt2">Extension 2 output.<br/>Extension 2 출력입니다.</param>
        /// <param name="doLaserPort">Laser-port output.<br/>레이저 포트 출력입니다.</param>
        /// <param name="marker">Marker.<br/>마커입니다.</param>
        /// <param name="remote">Optional remote control.<br/>선택적 외부 통신 제어입니다.</param>
        /// <param name="mcpServer">Optional MCP server created for this multi-editor. A non-null value is controlled by the Remote page and disposed before the device sets.<br/>이 다중 편집기를 대상으로 생성된 선택적 MCP 서버입니다. null이 아닌 값은 Remote 페이지에서 제어하며 장치 세트보다 먼저 해제됩니다.</param>
        /// <exception cref="ArgumentOutOfRangeException">The index is outside the configured range.<br/>인덱스가 설정 범위를 벗어났습니다.</exception>
        public void RegisterDevices(int index, IScanner scanner, ILaser laser, IPowerMeter powerMeter,
            IDInput diExt1, IDInput diLaserPort, IDOutput doExt1, IDOutput doExt2, IDOutput doLaserPort,
            IMarker marker, IRemote remote = null, IMCPServer mcpServer = null)
        {
            ValidateIndex(index);
            if (!CanChangeDevices(nameof(RegisterDevices), marker))
                return;
            var previousMarker = markers[index];
            if (previousMarker != null)
            {
                previousMarker.OnStarted -= OnMarkerStarted;
                previousMarker.OnEnded -= OnMarkerEnded;
                MarkerRegistry.Unregister(previousMarker);
            }
            scanners[index] = scanner;
            lasers[index] = laser;
            powerMeters[index] = powerMeter;
            diExt1s[index] = diExt1;
            diLaserPorts[index] = diLaserPort;
            doExt1s[index] = doExt1;
            doExt2s[index] = doExt2;
            doLaserPorts[index] = doLaserPort;
            markers[index] = marker;
            remotes[index] = remote;
            if (mcpServer != null) MCPServer = mcpServer;
            MarkerRegistry.Register(marker);
            if (marker != null)
            {
                marker.OnStarted += OnMarkerStarted;
                marker.OnEnded += OnMarkerEnded;
            }
            if (laser != null)
                laser.Scanner = scanner;
            RefreshSelector();
            if (index == CurrentDeviceIndex)
                ApplyCurrentDevices();
            else
                marker?.Ready(Document, View, scanner, laser, powerMeter);
            UpdateMarkerBusyState();
        }

        private void DisposeDevicesCore()
        {
            var cleanup = new WPFCleanup();
            var devices = new List<IDisposable>();
            cleanup.Run(() => Document?.ActSimulateStop(false));
            if (MCPServer != null) devices.Add(MCPServer);
            DetachDevices(cleanup);
            for (var index = 0; index < DeviceCapacity; index++)
            {
                devices.AddRange(new IDisposable[] { remotes[index], markers[index],
                    diExt1s[index], diLaserPorts[index], doExt1s[index], doExt2s[index],
                    doLaserPorts[index], powerMeters[index], lasers[index], scanners[index] });
                var marker = markers[index];
                cleanup.Run(() => MarkerRegistry.Unregister(marker));
                if (marker != null)
                {
                    cleanup.Run(() => marker.OnStarted -= OnMarkerStarted);
                    cleanup.Run(() => marker.OnEnded -= OnMarkerEnded);
                }
                remotes[index] = null;
                markers[index] = null;
                diExt1s[index] = null;
                diLaserPorts[index] = null;
                doExt1s[index] = null;
                doExt2s[index] = null;
                doLaserPorts[index] = null;
                powerMeters[index] = null;
                lasers[index] = null;
                scanners[index] = null;
            }
            cleanup.Dispose(devices.ToArray());
            cleanup.Run(UpdateMarkerBusyState);
            cleanup.ThrowIfFailed();
        }

        /// <summary>
        /// Asynchronously disposes the registered MCP server and remote, then disposes the remaining devices.
        /// <para>등록된 MCP 서버와 원격 연결을 비동기로 해제한 뒤 나머지 장치를 해제합니다.</para>
        /// </summary>
        /// <remarks>Call and await on the owning dispatcher before disposing the editor or document.
        /// <para>편집기 또는 문서를 해제하기 전에 소유 디스패처에서 호출하고 완료를 기다립니다.</para></remarks>
        /// <returns>A task that completes after device cleanup.<br/>장치 해제가 끝나면 완료되는 작업입니다.</returns>
        /// <exception cref="AggregateException">One or more resources failed to dispose.<br/>하나 이상의 자원 해제에 실패한 경우입니다.</exception>
        public async System.Threading.Tasks.Task DisposeDevices()
        {
            var server = MCPServer;
            MCPServer = null;
            var registeredRemotes = new System.Collections.Generic.List<IRemote>();
            for (int index = 0; index < DeviceCapacity; index++)
            {
                var remote = remotes[index];
                if (remote != null && !registeredRemotes.Exists(value => ReferenceEquals(value, remote)))
                    registeredRemotes.Add(remote);
                remotes[index] = null;
            }
            var failures = new System.Collections.Generic.List<Exception>();
            try { if (server != null) await server.DisposeAsync(); }
            catch (Exception ex) { failures.Add(ex); }
            foreach (var remote in registeredRemotes)
            {
                try { await remote.DisposeAsync(); }
                catch (Exception ex) { failures.Add(ex); }
            }
            try { DisposeDevicesCore(); }
            catch (Exception ex) { failures.Add(ex); }
            if (failures.Count > 0)
                throw new AggregateException("Device cleanup failed.", failures);
        }

        private void DetachDevices(WPFCleanup cleanup)
        {
            cleanup.Run(() => MCPServer = null);
            cleanup.Run(() => Remote = null);
            cleanup.Run(() => Marker = null);
            cleanup.Run(() => PowerMeter = null);
            cleanup.Run(() => DOLaserPort = null);
            cleanup.Run(() => DOExt2 = null);
            cleanup.Run(() => DOExt1 = null);
            cleanup.Run(() => DILaserPort = null);
            cleanup.Run(() => DIExt1 = null);
            cleanup.Run(() => Laser = null);
            cleanup.Run(() => Scanner = null);
        }
        #endregion

        #region Document Events
        private void QueueUiUpdate(Action update)
        {
            if (disposed || Dispatcher.HasShutdownStarted || Dispatcher.HasShutdownFinished)
                return;
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (!disposed)
                    update();
            }));
        }

        private void Document_OnNew(IDocument doc)
        {
            QueueUiUpdate(() =>
            {
                if (!ReferenceEquals(doc, Document)) return;
                UpdatePens(Scanner, Laser);
                ApplyEditPermission();
            });
        }

        private void Document_OnBeforeOpen(IDocument doc) { }

        private void Document_OnAfterOpen(IDocument doc, string fileName)
        {
            QueueUiUpdate(() =>
            {
                if (!ReferenceEquals(doc, Document)) return;
                UpdatePens(Scanner, Laser);
                lblFileName.Text = fileName;
                ApplyEditPermission();
            });
        }

        private void Document_OnBeforeSave(IDocument doc) { }

        private void Document_OnAfterSave(IDocument doc, string fileName)
        {
            QueueUiUpdate(() =>
            {
                if (!ReferenceEquals(doc, Document)) return;
                lblFileName.Text = fileName;
            });
        }

        private void Document_OnSimulationStarted(IDocument doc, IEntity[] entities) =>
            QueueSimulationStateUpdate(doc);

        private void Document_OnSimulationEnded(IDocument doc) => QueueSimulationStateUpdate(doc);

        private void QueueSimulationStateUpdate(IDocument doc)
        {
            QueueUiUpdate(() =>
            {
                if (ReferenceEquals(doc, Document)) ApplyEditPermission();
            });
        }
        #endregion

        #region Marker Events
        private void Marker_OnStarted(IMarker _marker)
        {
            QueueUiUpdate(() =>
            {
                if (ReferenceEquals(_marker, Marker)) RefreshMarkerState();
            });
        }

        private void Marker_OnEnded(IMarker _marker, bool success, TimeSpan? ts)
        {
            QueueUiUpdate(() =>
            {
                if (ReferenceEquals(_marker, Marker)) RefreshMarkerState(success, ts);
            });
        }

        private void RefreshMarkerState(bool? success = null, TimeSpan? elapsed = null)
        {
            if (disposed) return;
            if (Marker?.IsBusy == true)
            {
                if (!swProgress.IsRunning) swProgress.Restart();
                if (IsLoaded) timerProgress.Start();
                lblProcessTime.SetResourceReference(TextBlock.ForegroundProperty, "Sirius.Brush.Text");
            }
            else
            {
                timerProgress.Stop();
                swProgress.Stop();
                if (success.HasValue)
                {
                    lblProcessTime.Text = $"{elapsed.GetValueOrDefault().TotalSeconds:F3} sec";
                    lblProcessTime.SetResourceReference(TextBlock.ForegroundProperty,
                        success.Value ? "Sirius.Brush.Text" : "Sirius.Brush.Error");
                }
            }
            ApplyEditPermission();
        }
        #endregion

        #region PowerMeter Events
        private void QueueReadoutUpdate(Action update)
        {
            if (disposed || Dispatcher.HasShutdownStarted || Dispatcher.HasShutdownFinished)
                return;
            Dispatcher.BeginInvoke(DispatcherPriority.Background, update);
        }

        private void ApplyPowerReadout((IPowerMeter Source, PowerReadoutState State, MeasureUnits Unit, double Value) update)
        {
            if (disposed || !ReferenceEquals(update.Source, powerMeter)) return;
            switch (update.State)
            {
                case PowerReadoutState.Started: lblPowerWatt.Text = MessageBoxLocalization.S("PowerMeter_Started"); break;
                case PowerReadoutState.Stopped: lblPowerWatt.Text = MessageBoxLocalization.S("PowerMeter_Stopped"); break;
                case PowerReadoutState.Cleared: lblPowerWatt.Text = MessageBoxLocalization.S("PowerMeter_Empty"); break;
                case PowerReadoutState.Measured:
                    lblPowerWatt.Text = update.Unit == MeasureUnits.Watt ? $"{update.Value:F3} W" : $"{update.Value:F3} J";
                    break;
            }
        }

        private void PowerMeter_OnStarted(IPowerMeter _pm)
        {
            powerReadoutUpdates.Post((_pm, PowerReadoutState.Started, default, 0));
        }

        private void PowerMeter_OnStopped(IPowerMeter _pm)
        {
            powerReadoutUpdates.Post((_pm, PowerReadoutState.Stopped, default, 0));
        }

        private void PowerMeter_OnMeasured(IPowerMeter _pm, DateTime _dt, MeasureUnits unit, double wattOrJoule)
        {
            powerReadoutUpdates.Post((_pm, PowerReadoutState.Measured, unit, wattOrJoule));
        }

        private void PowerMeter_OnCleared(IPowerMeter _pm)
        {
            powerReadoutUpdates.Post((_pm, PowerReadoutState.Cleared, default, 0));
        }
        #endregion

        #region Remote Events
        private void Remote_OnModeChanged(IRemote _remote, RemoteControlModes mode) { }
        #endregion

        #region MoF Encoder
        private void LblEncoder_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            lblEncoder.ToolTip = Internal.EncoderStatusToolTip.Format(
                Scanner as IRtcMoF,
                MessageBoxLocalization.S("SiriusEditor_StatusEncoder"));
        }

        private void MoF_OnEncoderChanged(IRtcMoF rtcMoF, int encX, int encY, double encXmmOrAngle, double encYmm)
        {
            encoderReadoutUpdates.Post((rtcMoF, encX, encY, encXmmOrAngle, encYmm));
        }

        private void ApplyEncoderReadout((IRtcMoF Source, int X, int Y, double XValue, double YValue) update)
        {
            if (disposed || !ReferenceEquals(update.Source, encoderSource)) return;
            lblEncoder.Text = update.Source.MoFMode == RtcMoFModes.Rotary
                ? $"ENC: {update.XValue:F3}° ({update.X})"
                : $"ENC: {update.XValue:F3}, {update.YValue:F3}mm ({update.X}, {update.Y})";
        }

        private void LblEncoder_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount < 2) return;
            if (Scanner is not IRtcMoF rtcMoF) return;

            var result = WPFMessageBox.Show(MessageBoxLocalization.S("Scanner_ConfirmResetEncoders"), MessageBoxLocalization.S("Title_Warning"), MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
                rtcMoF.CtlMoFEncoderReset();
        }

        private void BtnLogWindow_Click(object sender, RoutedEventArgs e)
        {
            ShowLogWindow(!IsShowLogWindow);
        }
        #endregion

        #region Timers
        private void TimerStatus_Tick(object sender, EventArgs e)
        {
            UpdateMarkerStatus();
        }

        private void UpdateMarkerStatus()
        {
            var currentMarker = Marker;
            if (currentMarker == null)
            {
                timerStatusColorCounts = 0;
                WPFStatusIndicator.ApplyReady(bdReady, false);
                WPFStatusIndicator.ApplyBusy(bdBusy, false, 0);
                WPFStatusIndicator.ApplyError(bdError, false);
                return;
            }

            WPFStatusIndicator.ApplyReady(bdReady, currentMarker.IsReady);

            // Busy
            if (currentMarker.IsBusy)
            {
                timerStatusColorCounts = unchecked(timerStatusColorCounts + 1);
            }
            else
            {
                timerStatusColorCounts = 0;
            }
            WPFStatusIndicator.ApplyBusy(bdBusy, currentMarker.IsBusy, timerStatusColorCounts);

            WPFStatusIndicator.ApplyError(bdError, currentMarker.IsError);

            // Remote
            if (remote == null)
            {
                bdRemote.Visibility = Visibility.Collapsed;
            }
            else
            {
                bdRemote.Visibility = Visibility.Visible;
                bdRemote.SetResourceReference(Border.BackgroundProperty, "Sirius.Brush.Remote");
                lblRemote.Text = $" {Internal.RemoteStatusLocalization.Format(remote.ControlMode, remote.IsConnected)} ";
                if (remote.ControlMode == RemoteControlModes.Local)
                {
                    bdRemote.Opacity = remote.IsConnected ? 0.8 : 0.55;
                }
                else
                {
                    bdRemote.Opacity = remote.IsConnected ? 1.0 : 0.7;
                }
            }
        }

        private void Marker_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.PropertyName) &&
                e.PropertyName != nameof(IMarker.IsReady) &&
                e.PropertyName != nameof(IMarker.IsBusy) &&
                e.PropertyName != nameof(IMarker.IsError))
                return;

            QueueUiUpdate(() =>
            {
                if (ReferenceEquals(sender, Marker))
                    UpdateMarkerStatus();
            });
        }

        private void TimerProgress_Tick(object sender, EventArgs e)
        {
            timerProgressColorCounts = unchecked(timerProgressColorCounts + 1);
            lblProcessTime.SetResourceReference(TextBlock.ForegroundProperty,
                timerProgressColorCounts % 2 == 0 ? "Sirius.Brush.Text" : "Sirius.Brush.Error");
            lblProcessTime.Text = $"{swProgress.ElapsedMilliseconds / 1000.0:F3} sec";
        }
        #endregion

        #region Toolbar Button Handlers
        private void BtnNew_Click(object sender, RoutedEventArgs e)
        {
            if (Document == null) return;
            Document.ActNew(true, true, true, true, true, false, false, false, false);
            OnAfterNew?.Invoke(this);
        }

        private void BtnOpen_Click(object sender, RoutedEventArgs e)
        {
            if (Document == null) return;

            var dlg = new WPFOpenFileDialog
            {
                Filter = SpiralLab.Sirius3.UI.Config.FileOpenFilters,
                Title  = MessageBoxLocalization.S("Common_Open"),
                InitialDirectory = SpiralLab.Sirius3.Config.RecipePath,
                FileName = Document.FileName,
            };
            if (dlg.ShowDialog() != true) return;

            if (Document.IsModified)
            {
                var r = WPFMessageBox.Show(MessageBoxLocalization.S("Document_ConfirmOpenUnsaved"), MessageBoxLocalization.S("Title_Warning"), MessageBoxButton.YesNo);
                if (r != MessageBoxResult.Yes) return;
            }

            var previousCursor = global::System.Windows.Input.Mouse.OverrideCursor;
            try
            {
                global::System.Windows.Input.Mouse.OverrideCursor = global::System.Windows.Input.Cursors.Wait;
                if (Document.ActOpen(dlg.FileName, true, true, true, false, false, false, false))
                    OnAfterOpen?.Invoke(this, dlg.FileName);
            }
            finally
            {
                global::System.Windows.Input.Mouse.OverrideCursor = previousCursor;
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (Document == null) return;

            var dlg = new WPFSaveFileDialog
            {
                Filter = SpiralLab.Sirius3.UI.Config.FileSaveFilters,
                Title  = MessageBoxLocalization.S("Common_Save"),
                InitialDirectory = SpiralLab.Sirius3.Config.RecipePath,
                OverwritePrompt = true,
            };
            if (dlg.ShowDialog() != true) return;

            var previousCursor = global::System.Windows.Input.Mouse.OverrideCursor;
            try
            {
                global::System.Windows.Input.Mouse.OverrideCursor = global::System.Windows.Input.Cursors.Wait;
                if (Document.ActSave(dlg.FileName))
                    OnAfterSave?.Invoke(this, dlg.FileName);
            }
            finally
            {
                global::System.Windows.Input.Mouse.OverrideCursor = previousCursor;
            }
        }

        private void BtnLock_Click(object sender, RoutedEventArgs e)
        {
            isLocked = btnLock.IsChecked == true;
            ApplyEditPermission();
        }


        #endregion

        #region Helpers
        private void tabTrees_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.Source == tabTrees && !isSynchronizingPageTab)
            {
                SwitchPage(tabTrees.SelectedIndex);
            }
        }

        private void Document_OnPageChanged(IDocument source, IPage page)
        {
            if (disposed || page == null)
                return;
            QueueUiUpdate(() =>
            {
                if (!ReferenceEquals(source, Document) || source?.DocumentData?.Pages == null)
                    return;
                var index = source.DocumentData.Pages.ToList().IndexOf(source.ActivePage);
                if (index < 0 || index > 3)
                    return;
                source.Page = (DocumentPages)index;
                if (tabTrees.SelectedIndex == index)
                    return;
                isSynchronizingPageTab = true;
                try
                {
                    tabTrees.SelectedIndex = index;
                }
                finally
                {
                    isSynchronizingPageTab = false;
                }
            });
        }

        private void SwitchPage(int index)
        {
            if (Document == null) return;
            Document.ActSelectClear();

            switch (index)
            {
                case 0: Document.Page = DocumentPages.Page1; Document.ActivePage = Document.DocumentData.Pages[0]; break;
                case 1: Document.Page = DocumentPages.Page2; Document.ActivePage = Document.DocumentData.Pages[1]; break;
                case 2: Document.Page = DocumentPages.Page3; Document.ActivePage = Document.DocumentData.Pages[2]; break;
                case 3: Document.Page = DocumentPages.Page4; Document.ActivePage = Document.DocumentData.Pages[3]; break;
                case 4: Document.Page = DocumentPages.Block; break;
            }

            Document.ActRegen();
            if (index < 4)
                View?.ActiveCamera?.ZoomFit(View, new IEntity[] { Document.ActivePage?.ActiveLayer });
            else
                View?.ActiveCamera?.ZoomFit(View, Document.DocumentData.Blocks.Children.ToArray());
            View?.DoRender();
        }

        /// <summary>Requests document editing; locks and active jobs still restrict editing.
        /// <para>문서 편집 허용을 요청합니다. 잠금 또는 실행 중 작업의 편집 제한은 유지됩니다.</para></summary>
        /// <param name="isEnable">Whether editing is requested.<br/>편집 허용 요청 여부입니다.</param>
        public virtual void ControlEnableOrNot(bool isEnable)
        {
            isEditEnabled = isEnable;
            ApplyEditPermission();
        }

private void ApplyEditPermission()
        {
            if (disposed) return;
            var isEnable = isEditEnabled && !isLocked && !isMultiMarkerBusy &&
                Marker?.IsBusy != true && Document?.IsSimulationWorking != true;
            btnNew.IsEnabled  = isEnable;
            btnOpen.IsEnabled = isEnable;
            btnSave.IsEnabled = isEnable;
            if (editorControl != null)
                editorControl.IsAllowEdit = isEnable;
            if (tabTrees != null) tabTrees.IsEnabled = isEnable;
            if (propertyGridControl1 != null) propertyGridControl1.IsEnabled = isEnable;
        }

        private void OnPreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // The packaged editor owns viewport/child input, including construction
            // and grip keys that can overlap the configured marker shortcut.
            if (e.Handled || editorControl?.IsKeyboardFocusWithin == true)
                return;
            if (TryExecuteMarkerShortcut(e))
            {
                e.Handled = true;
                return;
            }
            if (WPFCommands.TryExecutePreviewShortcut(e, Document, View, Marker))
                e.Handled = true;
        }

        private bool TryExecuteMarkerShortcut(System.Windows.Input.KeyEventArgs e)
        {
            if (e == null || e.Handled || System.Windows.Input.Keyboard.Modifiers != System.Windows.Input.ModifierKeys.None)
                return false;
            var key = WPFCommands.NormKey(e);
            if (key.Key != SpiralLab.Sirius3.UI.Config.KeyboardMarkerStart)
                return false;
            if (View is IViewInteractionHost interaction && interaction.IsInteractionActive)
                return true;

            // The configured start key belongs to the editor even when the command
            // cannot start or the operator cancels the safety confirmation.
            if (Document == null || Marker == null || Marker.IsBusy)
                return true;
            if (SpiralLab.Sirius3.UI.Config.IsShowMessageBoxWhenMarkerStart)
            {
                using (var confirmation = new MessageBoxWindow(
                    MessageBoxLocalization.S("Marker_ConfirmStart", Document.Page),
                    MessageBoxLocalization.S("Title_Warning"),
                    MessageBoxLocalization.S("Common_Start"),
                    MessageBoxLocalization.S("Common_Cancel"))
                    { Owner = Window.GetWindow(this) })
                    if (confirmation.ShowDialog() != true)
                        return true;
            }

            if (Document.ActRegen() && Marker.Ready(Document))
                _ = Marker.Start(Document.Page);
            return true;
        }

        /// <summary>
        /// Detaches the document, device, timer, tree, property-grid, and OpenGL event paths.
        /// Assigned documents and devices are not disposed; use <see cref="DisposeDevices"/> explicitly for owned devices.
        /// <para>문서, 장치, 타이머, 트리, 속성 표와 OpenGL 이벤트를 해제합니다. 할당된 문서와 장치는 폐기하지 않으며 소유 장치는 <see cref="DisposeDevices"/>를 명시적으로 호출합니다.</para>
        /// <example><code language="C#">
        /// // After confirming closure, while the window can still render.
        /// // 종료를 확인한 뒤, 창에서 렌더링이 가능한 동안 호출합니다.
        /// window.Closing += (sender, args) => editor.Dispose();
        /// </code></example>
        /// </summary>
        /// <remarks>Complete this call before removing the loaded control or closing its host.
        /// <para>로드된 컨트롤을 제거하거나 호스트 창을 닫기 전에 호출을 완료합니다.</para></remarks>
        public void Dispose()
        {
            if (disposed)
                return;
            disposed = true;
            Loaded -= OnLoaded;
            Unloaded -= OnUnloaded;
            PreviewKeyDown -= OnPreviewKeyDown;
            lblEncoder.MouseLeftButtonDown -= LblEncoder_MouseDoubleClick;
            lblEncoder.ToolTipOpening -= LblEncoder_ToolTipOpening;
            timerStatus.Stop();
            timerProgress.Stop();
            timerStatus.Tick -= TimerStatus_Tick;
            timerProgress.Tick -= TimerProgress_Tick;

            var cleanup = new WPFCleanup();
            foreach (var button in deviceButtons)
                if (button != null) button.Click -= OnDeviceButtonClick;
            foreach (var marker in markers)
            {
                if (marker != null)
                {
                    cleanup.Run(() => marker.OnStarted -= OnMarkerStarted);
                    cleanup.Run(() => marker.OnEnded -= OnMarkerEnded);
                }
                cleanup.Run(() => MarkerRegistry.Unregister(marker));
            }
            cleanup.Dispose(powerReadoutUpdates, encoderReadoutUpdates);
            DetachDevices(cleanup);
            cleanup.Run(() => Document = null);
            cleanup.Dispose(propertyGridControl1, logControl1,
                treeViewPageControl1, treeViewPageControl2, treeViewPageControl3,
                treeViewPageControl4, treeViewBlockControl1, laserControl1,
                scannerControl1, markerControl1, rtcDIControl1, rtcDOControl1,
                manualControl1, powerMeterControl1, powerMapControl1,
                stepperControl1, entityPenControl1, layerPenControl1,
                remoteControl1, editorControl);
            cleanup.ThrowIfFailed();
        }

        private void UpdatePens(IScanner scanner, ILaser laser)
        {
            if (Document == null || laser == null) return;
            if (laser is not ILaserPowerControl) return;

            foreach (var child in Document.DocumentData.EntityPens.Children)
                if (child is EntityPen pen) pen.Apply(scanner, laser);
            foreach (var child in Document.DocumentData.LayerPens.Children)
                if (child is EntityLayerPen lpen) lpen.Apply(scanner, laser);
        }

        private void UpdatePowerMeterLaser(ILaser laser)
        {
            if (powerMeter is PowerMeterVirtual pmv)
                pmv.Laser = laser;
        }

        private void PropertyVisibility(IScanner scanner, ILaser laser)
        {
            if (scanner != null)
            {
                EntityPen.PropertyVisibility(scanner);
                EntityLayerPen.PropertyVisibility(scanner);
            }
            if (laser != null)
            {
                EntityPen.PropertyVisibility(laser);
            }
            propertyGridControl1?.RefreshProperties();
        }

        /// <summary>
        /// Adjusts navigation page visibility based on RTC capabilities.
        /// <para>RTC 기능에 따라 탐색 페이지 가시성을 조정합니다.</para>
        /// </summary>
        private void PageVisibility(IScanner scanner)
        {
            if (tabStepper != null)
                tabStepper.Visibility = scanner is IRtcSyncAxis ? Visibility.Collapsed : Visibility.Visible;
        }

        #endregion
        #region Device Selection
        /// <summary>
        /// Switches the active device set when every registered marker is idle; devices are retained.
        /// <para>등록된 모든 마커가 유휴 상태일 때 활성 장치 세트를 전환하며 장치를 해제하지 않습니다.</para>
        /// </summary>
        /// <param name="index">Target device-set index.<br/>대상 장치 세트 인덱스입니다.</param>
        /// <returns>Whether the switch completed.<br/>전환 완료 여부입니다.</returns>
        /// <exception cref="ArgumentOutOfRangeException">The index is outside the configured range.<br/>인덱스가 설정 범위를 벗어났습니다.</exception>
        public bool SwitchDevices(int index)
        {
            ValidateIndex(index);
            if (!CanChangeDevices(nameof(SwitchDevices)))
                return false;
            if (scanners[index] == null || lasers[index] == null || markers[index] == null)
                return false;
            OnBeforeChangeDevice?.Invoke(this);
            // User callbacks may start a peer or change the available device range.
            if (index >= MaxDeviceCounts || !CanChangeDevices(nameof(SwitchDevices)))
                return false;
            if (scanners[index] == null || lasers[index] == null || markers[index] == null)
                return false;
            Document?.ActSimulateStop(false);
            if (!CanChangeDevices(nameof(SwitchDevices)))
                return false;
            CurrentDeviceIndex = index;
            ApplyCurrentDevices();
            changingSelection = true;
            UpdateDeviceButtons();
            changingSelection = false;
            OnAfterChangeDevice?.Invoke(this);
            return true;
        }

        private void ApplyCurrentDevices()
        {
            DIExt1 = diExt1s[CurrentDeviceIndex];
            DILaserPort = diLaserPorts[CurrentDeviceIndex];
            DOExt1 = doExt1s[CurrentDeviceIndex];
            DOExt2 = doExt2s[CurrentDeviceIndex];
            DOLaserPort = doLaserPorts[CurrentDeviceIndex];
            Scanner = scanners[CurrentDeviceIndex];
            Laser = lasers[CurrentDeviceIndex];
            PowerMeter = powerMeters[CurrentDeviceIndex];
            Marker = markers[CurrentDeviceIndex];
            Remote = remotes[CurrentDeviceIndex];
            Marker?.Ready(Document, View, Scanner, Laser, PowerMeter);
            PageVisibility();
        }

        private void PageVisibility()
        {
            PageVisibility(Scanner);
        }

        private void DispatchMarkerBusyState()
        {
            if (disposed || Dispatcher.HasShutdownStarted || Dispatcher.HasShutdownFinished)
                return;
            if (Dispatcher.CheckAccess())
                UpdateMarkerBusyState();
            else
                Dispatcher.BeginInvoke(new Action(UpdateMarkerBusyState));
        }

        private void UpdateMarkerBusyState()
        {
            if (disposed) return;
            var anyBusy = markers.Any(value => value?.IsBusy == true);
            isMultiMarkerBusy = anyBusy;
            ApplyEditPermission();
            RefreshSelector();
        }

        private bool CanChangeDevices(string operation, IMarker incomingMarker = null)
        {
            if (disposed) throw new ObjectDisposedException(nameof(SiriusMultiEditorControl));
            var busyMarker = markers.FirstOrDefault(value => value?.IsBusy == true);
            if (busyMarker == null && incomingMarker?.IsBusy == true)
                busyMarker = incomingMarker;
            if (busyMarker == null)
                return true;
            Logger.Log(LogLevel.Warning, $"WPF multi-editor: {operation} rejected; marker [{busyMarker.Index}] is busy.");
            return false;
        }

        private void RefreshSelector()
        {
            changingSelection = true;
            UpdateDeviceButtons();
            changingSelection = false;
        }

        private Border CreateDeviceSelector()
        {
            var panel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(2, 2, 2, 2),
                HorizontalAlignment = global::System.Windows.HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
            };
            for (var index = 0; index < DeviceCapacity; index++)
            {
                var button = new ToggleButton
                {
                    Tag = index,
                    Height = 28,
                    MinWidth = 32,
                    Margin = new Thickness(1, 0, 1, 0),
                    Padding = new Thickness(4, 2, 4, 2),
                    VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalContentAlignment = global::System.Windows.HorizontalAlignment.Center,
                };
                button.Click += OnDeviceButtonClick;
                deviceButtons[index] = button;
                panel.Children.Add(button);
            }

            var border = new Border
            {
                BorderThickness = new Thickness(0),
                Child = panel,
                VerticalAlignment = VerticalAlignment.Stretch,
            };
            border.SetResourceReference(Border.BackgroundProperty, "Sirius.Brush.Toolbar");
            return border;
        }

        private void UpdateDeviceButtons()
        {
            var anyBusy = markers.Any(value => value?.IsBusy == true);
            for (var index = 0; index < DeviceCapacity; index++)
            {
                var button = deviceButtons[index];
                if (button == null)
                    continue;
                var isVisible = index < MaxDeviceCounts;
                var isSelected = index == CurrentDeviceIndex;
                var isEmpty = scanners[index] == null || lasers[index] == null || markers[index] == null;
                var isBusy = markers[index]?.IsBusy == true;
                button.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
                button.IsChecked = isSelected;
                button.IsEnabled = isVisible && !anyBusy && (isSelected || !isEmpty);
                button.MinWidth = isSelected ? 84 : 32;
                button.ToolTip = $"Device {index + 1}{(isEmpty ? " (empty)" : isBusy ? " (busy)" : string.Empty)}";

                var content = new StackPanel { Orientation = Orientation.Horizontal, VerticalAlignment = VerticalAlignment.Center };
                content.Children.Add(WPFWindowStyleHelper.CreateImage($"m{index + 1}.png", 18, true));
                if (isSelected)
                {
                    content.Children.Add(new TextBlock
                    {
                        Text = $"Device {index + 1}",
                        Margin = new Thickness(5, 0, 0, 0),
                        VerticalAlignment = VerticalAlignment.Center,
                        FontWeight = FontWeights.SemiBold,
                    });
                }
                button.Content = content;
            }
        }

        private void OnDeviceButtonClick(object sender, RoutedEventArgs e)
        {
            if (changingSelection || !(sender is ToggleButton button) || !(button.Tag is int index))
                return;
            if (!SwitchDevices(index))
                UpdateDeviceButtons();
        }

        private void ValidateIndex(int index)
        {
            if (index < 0 || index >= MaxDeviceCounts) throw new ArgumentOutOfRangeException(nameof(index));
        }

        private void OnMarkerStarted(IMarker value) => DispatchMarkerBusyState();

        private void OnMarkerEnded(IMarker value, bool success, TimeSpan? elapsed) => DispatchMarkerBusyState();
        #endregion
    }
}
