/*
 *
 * 2026 Copyright to (c)SpiralLAB. All rights reserved.
 * Description : Supported Material Design WPF SiriusEditorControl
 * Author : hong chan, choi / hcchoi@spirallab.co.kr (http://spirallab.co.kr)
 */

using System;
using SpiralLab.Sirius3.Localization;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
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
using SpiralLab.Sirius3.UI.WPF;

namespace Demos
{
    /// <summary>
    /// Supported WPF editor control that hosts <see cref="EditorControl"/> and exposes
    /// native WPF document and device management panels.
    /// <para><see cref="EditorControl"/>과 네이티브 WPF 문서 및 장치 관리 패널을 제공합니다.</para>
    /// </summary>
    public partial class SiriusEditorControl : IDisposable
    {
        #region Events
        /// <summary>
        /// Raised after New button is pressed.
        /// <para>New 버튼을 누른 후 발생합니다.</para>
        /// </summary>
        public event Action<SiriusEditorControl> OnAfterNew;
        /// <summary>
        /// Raised after a document file is opened.
        /// <para>문서 파일을 연 후 발생합니다.</para>
        /// </summary>
        public event Action<SiriusEditorControl, string> OnAfterOpen;
        /// <summary>
        /// Raised after a document file is saved.
        /// <para>문서 파일을 저장한 후 발생합니다.</para>
        /// </summary>
        public event Action<SiriusEditorControl, string> OnAfterSave;
        #endregion

        #region Dependency Properties
        /// <summary>Identifies the <see cref="AliasName"/> dependency property.</summary>
        public static readonly DependencyProperty AliasNameProperty =
            DependencyProperty.Register(nameof(AliasName), typeof(string), typeof(SiriusEditorControl),
                new PropertyMetadata(string.Empty, (d, e) =>
                {
                    if (d is SiriusEditorControl c && c.lblAliasName != null)
                        c.lblAliasName.Text = e.NewValue as string ?? string.Empty;
                }));

        /// <summary>Identifies the <see cref="Document"/> dependency property.</summary>
        public static readonly DependencyProperty DocumentProperty =
            DependencyProperty.Register(nameof(Document), typeof(IDocument), typeof(SiriusEditorControl),
                new PropertyMetadata(null, (d, e) =>
                {
                    if (d is SiriusEditorControl c)
                        c.ApplyDocument(e.OldValue as IDocument, e.NewValue as IDocument);
                }));

        /// <summary>Identifies the <see cref="Scanner"/> dependency property.</summary>
        public static readonly DependencyProperty ScannerProperty =
            DependencyProperty.Register(nameof(Scanner), typeof(IScanner), typeof(SiriusEditorControl),
                new PropertyMetadata(null, (d, e) =>
                {
                    if (d is SiriusEditorControl c)
                        c.ApplyScanner(e.OldValue as IScanner, e.NewValue as IScanner);
                }));

        /// <summary>Identifies the <see cref="Laser"/> dependency property.</summary>
        public static readonly DependencyProperty LaserProperty =
            DependencyProperty.Register(nameof(Laser), typeof(ILaser), typeof(SiriusEditorControl),
                new PropertyMetadata(null, (d, e) =>
                {
                    if (d is SiriusEditorControl c)
                        c.ApplyLaser(e.OldValue as ILaser, e.NewValue as ILaser);
                }));

        /// <summary>Identifies the <see cref="Marker"/> dependency property.</summary>
        public static readonly DependencyProperty MarkerProperty =
            DependencyProperty.Register(nameof(Marker), typeof(IMarker), typeof(SiriusEditorControl),
                new PropertyMetadata(null, (d, e) =>
                {
                    if (d is SiriusEditorControl c)
                        c.ApplyMarker(e.OldValue as IMarker, e.NewValue as IMarker);
                }));

        /// <summary>Identifies the <see cref="PowerMeter"/> dependency property.</summary>
        public static readonly DependencyProperty PowerMeterProperty =
            DependencyProperty.Register(nameof(PowerMeter), typeof(IPowerMeter), typeof(SiriusEditorControl),
                new PropertyMetadata(null, (d, e) => ((SiriusEditorControl)d).ApplyPowerMeter(e.OldValue as IPowerMeter, e.NewValue as IPowerMeter)));

        /// <summary>Identifies the <see cref="DIExt1"/> dependency property.</summary>
        public static readonly DependencyProperty DIExt1Property = RegisterDeviceProperty<IDInput>(nameof(DIExt1), (c, value) => { c.diext1 = value; if (c.DIRtcCtrl != null) c.DIRtcCtrl.DIExt1 = value; });
        /// <summary>Identifies the <see cref="DILaserPort"/> dependency property.</summary>
        public static readonly DependencyProperty DILaserPortProperty = RegisterDeviceProperty<IDInput>(nameof(DILaserPort), (c, value) => { c.dilaserport = value; if (c.DIRtcCtrl != null) c.DIRtcCtrl.DILaserPort = value; });
        /// <summary>Identifies the <see cref="DOExt1"/> dependency property.</summary>
        public static readonly DependencyProperty DOExt1Property = RegisterDeviceProperty<IDOutput>(nameof(DOExt1), (c, value) => { c.doext1 = value; if (c.DORtcCtrl != null) c.DORtcCtrl.DOExt1 = value; });
        /// <summary>Identifies the <see cref="DOExt2"/> dependency property.</summary>
        public static readonly DependencyProperty DOExt2Property = RegisterDeviceProperty<IDOutput>(nameof(DOExt2), (c, value) => { c.doext2 = value; if (c.DORtcCtrl != null) c.DORtcCtrl.DOExt2 = value; });
        /// <summary>Identifies the <see cref="DOLaserPort"/> dependency property.</summary>
        public static readonly DependencyProperty DOLaserPortProperty = RegisterDeviceProperty<IDOutput>(nameof(DOLaserPort), (c, value) => { c.dolaserport = value; if (c.DORtcCtrl != null) c.DORtcCtrl.DOLaserPort = value; });

        /// <summary>Identifies the <see cref="Remote"/> dependency property.</summary>
        public static readonly DependencyProperty RemoteProperty =
            DependencyProperty.Register(nameof(Remote), typeof(IRemote), typeof(SiriusEditorControl),
                new PropertyMetadata(null, (d, e) => ((SiriusEditorControl)d).ApplyRemote(e.OldValue as IRemote, e.NewValue as IRemote)));

        /// <summary>Identifies the <see cref="MCPServer"/> dependency property.</summary>
        public static readonly DependencyProperty MCPServerProperty =
            DependencyProperty.Register(nameof(MCPServer), typeof(IMCPServer), typeof(SiriusEditorControl),
                new PropertyMetadata(null, (d, e) => ((SiriusEditorControl)d).ApplyMCPServer(e.NewValue as IMCPServer)));

        private static DependencyProperty RegisterDeviceProperty<T>(string name, Action<SiriusEditorControl, T> apply) where T : class
        {
            return DependencyProperty.Register(name, typeof(T), typeof(SiriusEditorControl),
                new PropertyMetadata(null, (d, e) => apply((SiriusEditorControl)d, e.NewValue as T)));
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
            get => (IScanner)GetValue(ScannerProperty);
            set => SetValue(ScannerProperty, value);
        }

        /// <summary>
        /// Gets or sets the laser.
        /// <para>레이저를 가져오거나 설정합니다.</para>
        /// </summary>
        [Category("Sirius3")] [DisplayName("Laser")]
        public ILaser Laser
        {
            get => (ILaser)GetValue(LaserProperty);
            set => SetValue(LaserProperty, value);
        }

        /// <summary>
        /// Gets or sets the marker.
        /// <para>마커를 가져오거나 설정합니다.</para>
        /// </summary>
        [Category("Sirius3")] [DisplayName("Marker")]
        public IMarker Marker
        {
            get => (IMarker)GetValue(MarkerProperty);
            set => SetValue(MarkerProperty, value);
        }

        /// <summary>Gets or sets the power meter.</summary>
        [Browsable(false)]
        public IPowerMeter PowerMeter
        {
            get => (IPowerMeter)GetValue(PowerMeterProperty);
            set => SetValue(PowerMeterProperty, value);
        }

        /// <summary>Gets or sets DI Extension 1.</summary>
        [Browsable(false)]
        public IDInput DIExt1
        {
            get => (IDInput)GetValue(DIExt1Property);
            set => SetValue(DIExt1Property, value);
        }
        /// <summary>Gets or sets DI Laser Port.</summary>
        [Browsable(false)]
        public IDInput DILaserPort
        {
            get => (IDInput)GetValue(DILaserPortProperty);
            set => SetValue(DILaserPortProperty, value);
        }
        /// <summary>Gets or sets DO Extension 1.</summary>
        [Browsable(false)]
        public IDOutput DOExt1
        {
            get => (IDOutput)GetValue(DOExt1Property);
            set => SetValue(DOExt1Property, value);
        }
        /// <summary>Gets or sets DO Extension 2.</summary>
        [Browsable(false)]
        public IDOutput DOExt2
        {
            get => (IDOutput)GetValue(DOExt2Property);
            set => SetValue(DOExt2Property, value);
        }
        /// <summary>Gets or sets DO Laser Port.</summary>
        [Browsable(false)]
        public IDOutput DOLaserPort
        {
            get => (IDOutput)GetValue(DOLaserPortProperty);
            set => SetValue(DOLaserPortProperty, value);
        }

        /// <summary>Gets or sets the remote control interface.</summary>
        [Browsable(false)]
        public IRemote Remote
        {
            get => (IRemote)GetValue(RemoteProperty);
            set => SetValue(RemoteProperty, value);
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

        /// <summary>Gets or sets the assembly-owned accessory shown at the right side of the top command bar.</summary>
        internal UIElement TopToolbarAccessory
        {
            get => topAccessoryHost.Content as UIElement;
            set => topAccessoryHost.Content = value;
        }

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
        /// Initializes a new instance of <see cref="SiriusEditorControl"/>.
        /// <para><see cref="SiriusEditorControl"/> 인스턴스를 초기화합니다.</para>
        /// </summary>

        public SiriusEditorControl()
        {
            WPFThemeManager.Initialize();
            InitializeComponent();
            ApplyLocalization();

            powerReadoutUpdates = new(
                QueueReadoutUpdate, ApplyPowerReadout, update => ReferenceEquals(update.Source, powerMeter));
            encoderReadoutUpdates = new(
                QueueReadoutUpdate, ApplyEncoderReadout, update => ReferenceEquals(update.Source, encoderSource));

            if (DesignerProperties.GetIsInDesignMode(this))
                return;

            ApplyWinFormsTabIcons();

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

        private void ApplyWinFormsTabIcons()
        {
            var sourceType = typeof(global::SpiralLab.Sirius3.UI.WinForms.SiriusEditorControl);
            const string imageListResource = "imageList1.ImageStream";
            imgTabEditor.Source = WPFWindowStyleHelper.LoadWinFormsImageListImage(sourceType, imageListResource, 0, true);       // cube_24px.png
            imgTabMarker.Source = WPFWindowStyleHelper.LoadWinFormsImageListImage(sourceType, imageListResource, 24, true);     // Design.png
            imgTabManual.Source = WPFWindowStyleHelper.LoadWinFormsImageListImage(sourceType, imageListResource, 40, true);     // Voltage.png
            imgTabScanner.Source = WPFWindowStyleHelper.LoadWinFormsImageListImage(sourceType, imageListResource, 14, true);    // Video Card.png
            imgTabLaser.Source = WPFWindowStyleHelper.LoadWinFormsImageListImage(sourceType, imageListResource, 22, true);      // Processor2.png
            imgTabDio.Source = WPFWindowStyleHelper.LoadWinFormsImageListImage(sourceType, imageListResource, 28, true);        // RS-232 Male.png
            imgTabPower.Source = WPFWindowStyleHelper.LoadWinFormsImageListImage(sourceType, imageListResource, 30, true);      // Graph2.png
            imgTabStepper.Source = WPFWindowStyleHelper.LoadWinFormsImageListImage(sourceType, imageListResource, 54, true);    // Motor Symbol.png
            imgTabRemote.Source = WPFWindowStyleHelper.LoadWinFormsImageListImage(sourceType, imageListResource, 59, true);     // RJ45.png
            imgTabProperty.Source = WPFWindowStyleHelper.LoadWinFormsImageListImage(sourceType, imageListResource, 16, true);   // Property.png
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
            ApplyEditPermission();
        }

        private void ApplyScanner(IScanner oldScanner, IScanner newScanner)
        {
            if (DesignerProperties.GetIsInDesignMode(this))
                return;
            if (oldScanner is IRtcMoF oldMof)
                oldMof.OnEncoderChanged -= MoF_OnEncoderChanged;
            encoderSource = newScanner as IRtcMoF;
            encoderReadoutUpdates?.Reset();
            lblEncoder.Text = string.Empty;

            if (editorControl != null)
                editorControl.Scanner = newScanner;

            if (newScanner is IRtcMoF newMof)
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
                MarkerRegistry.Unregister(oldMarker);
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
                MarkerRegistry.Register(newMarker);
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

        /// <summary>
        /// Registers all hardware devices at once.
        /// <para>모든 하드웨어 장치를 한 번에 등록합니다.</para>
        /// </summary>
        public void RegisterDevices(
            IScanner scanner, ILaser laser, IPowerMeter powerMeter,
            IDInput dIExt1, IDInput dILaserPort,
            IDOutput dOExt1, IDOutput dOExt2, IDOutput dOLaserPort,
            IMarker marker, IRemote remote = null, IMCPServer mcpServer = null)
        {
            DIExt1      = dIExt1;
            DILaserPort = dILaserPort;
            DOExt1      = dOExt1;
            DOExt2      = dOExt2;
            DOLaserPort = dOLaserPort;

            Scanner     = scanner;
            Laser       = laser;
            PowerMeter  = powerMeter;
            Marker      = marker;
            Remote      = remote;
            if (mcpServer != null) MCPServer = mcpServer;

            if (marker != null)
            {
                MarkerRegistry.Register(marker);
                marker.Ready(Document, View, scanner as IRtc, laser, powerMeter);
            }
        }

        /// <summary>
        /// Disposes and removes all registered devices.
        /// <para>등록된 모든 장치를 해제하고 자원을 회수합니다.</para>
        /// </summary>
        /// <exception cref="AggregateException">Cleanup failures, reported after all devices have been processed.<br/>모든 장치의 해제를 시도한 후 보고되는 해제 오류입니다.</exception>
        public void DisposeDevices()
        {
            var devices = new IDisposable[] {
                MCPServer,
                Remote, Marker, DIExt1, DILaserPort,
                DOExt1, DOExt2, DOLaserPort, PowerMeter, Laser, Scanner };
            var cleanup = new WPFCleanup();
            cleanup.Run(() => Document?.ActSimulateStop(false));
            cleanup.Run(() => MarkerRegistry.Unregister(Marker));
            DetachDevices(cleanup);
            cleanup.Dispose(devices);
            cleanup.ThrowIfFailed();
        }

        internal void DetachDevices(WPFCleanup cleanup)
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

        internal void SetMultiMarkerBusy(bool isBusy)
        {
            isMultiMarkerBusy = isBusy;
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
            if (TryExecuteMarkerShortcut(e))
            {
                e.Handled = true;
                return;
            }
            if (editorControl?.IsKeyboardFocusWithin == true)
                return;
            if (WPFCommands.TryExecutePreviewShortcut(e, Document, View, Marker))
                e.Handled = true;
        }

        private bool TryExecuteMarkerShortcut(System.Windows.Input.KeyEventArgs e)
        {
            if (e == null || e.Handled ||
                System.Windows.Input.Keyboard.Modifiers != System.Windows.Input.ModifierKeys.None ||
                WPFCommands.ToGLKey(e.Key == System.Windows.Input.Key.System ? e.SystemKey : e.Key) !=
                    SpiralLab.Sirius3.UI.Config.KeyboardMarkerStart)
            {
                return false;
            }

            if (Document == null || Marker == null || Marker.IsBusy)
                return true;
            if (SpiralLab.Sirius3.UI.Config.IsShowMessageBoxWhenMarkerStart &&
                WPFMessageBox.Show(
                    System.Windows.Window.GetWindow(this),
                    MessageBoxLocalization.S("Marker_ConfirmStart", Document.Page),
                    MessageBoxLocalization.S("Title_Warning"),
                    MessageBoxButton.OKCancel,
                    MessageBoxImage.Warning) != MessageBoxResult.OK)
            {
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
        internal void PageVisibility(IScanner scanner)
        {
            if (tabStepper != null)
                tabStepper.Visibility = scanner is IRtcSyncAxis ? Visibility.Collapsed : Visibility.Visible;
        }

        #endregion
    }
}





