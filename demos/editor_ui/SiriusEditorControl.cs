using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Microsoft.Extensions.Logging;

using SpiralLab.Sirius3;
using SpiralLab.Sirius3.Document;
using SpiralLab.Sirius3.Entity;
using SpiralLab.Sirius3.Entity.Hatch;
using SpiralLab.Sirius3.IO;
using SpiralLab.Sirius3.Laser;
using SpiralLab.Sirius3.Marker;
using SpiralLab.Sirius3.PowerMeter;
using SpiralLab.Sirius3.Scanner;
using SpiralLab.Sirius3.Scanner.Rtc;
using SpiralLab.Sirius3.View;

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
    /// Main WinForms editor form that hosts the OpenGL editor surface,
    /// device controls (RTC/Laser/PowerMeter/IO), and document management UI.
    /// <para>OpenGL 편집기 화면, 장치 제어(RTC/레이저/파워 미터/IO) 및 문서 관리 UI를 호스팅하는 주 WinForms 편집기 폼입니다.</para>
    /// <para>主 WinForms 编辑器窗体，托管 OpenGL 编辑器表面、设备控件（RTC/激光/功率计/IO）和文档管理 UI。</para>
    /// </summary>
    public partial class SiriusEditorControl : UserControl
    {
        #region Fields

        private IDocument document;
        private IScanner scanner;
        private ILaser laser;
        private IMarker marker;
        private IPowerMeter powerMeter;

        private IDInput dIExt1;
        private IDInput dILaserPort;
        private IDOutput dOExt1;
        private IDOutput dOExt2;
        private IDOutput dOLaserPort;

        private readonly SpiralLab.Sirius3.UI.WinForms.EditorControl editorControl1 = new SpiralLab.Sirius3.UI.WinForms.EditorControl();
        private readonly Stopwatch timerProgressStopwatch = new Stopwatch();
        private readonly System.Windows.Forms.Timer timerProgress = new System.Windows.Forms.Timer();
        private readonly System.Windows.Forms.Timer timerStatus = new System.Windows.Forms.Timer();

        private int timerStatusColorCounts;
        private int timerProgressColorCounts;

        #endregion

        #region Public Bindable Properties
        /// <summary>
        /// Gets or sets the editor name
        /// <para>현재 편집기의 이름을 가져오거나 설정합니다. </para>
        /// </summary>
        public string AliasName
        {
            get { return lblName.Text; }
            set { lblName.Text = value; }
        }
        /// <summary>
        /// Gets or sets the current document and wires related UI/controls to it.
        /// <para>현재 문서를 가져오거나 설정하고 관련 UI/컨트롤을 연결합니다.</para>
        /// <para>获取或设置当前文档，并将相关的 UI/控件连接到它。</para>
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius3")]
        [DisplayName("Document")]
        [Description("Document")]
        public IDocument Document
        {
            get => document;
            internal set
            {
                document?.ActSimulateStop(false);
                if (document != null)
                {
                    PropertyGridCtrl.SelecteObject = null;
                    document.OnBeforeOpen -= Document_OnBeforeOpen;
                    document.OnAfterOpen -= Document_OnAfterOpen;
                    document.OnBeforeSave -= Document_OnBeforeSave;
                    document.OnAfterSave -= Document_OnAfterSave;
                }

                document = value;

                MarkerCtrl.Document = document;
                PropertyGridCtrl.Document = document;
                EditorCtrl.Document = document;
                ScannerPenCtrl.Document = document;
                LayerPenCtrl.Document = document;
                PowerMapCtrl.Document = document;

                treeViewPageControl1.Document = document;
                treeViewPageControl2.Document = document;
                //treeViewPageControl3.Document = document;
                //treeViewPageControl4.Document = document;

                treeViewBlockControl1.Document = document;
                treeViewWaferControl1.Document = document;
                treeViewSubstrateControl1.Document = document;

                treeViewPageControl1.View = editorControl1.View;
                treeViewPageControl2.View = editorControl1.View;
                //treeViewPageControl3.View = editorControl1.View;
                //treeViewPageControl4.View = editorControl1.View;

                treeViewBlockControl1.View = editorControl1.View;
                treeViewWaferControl1.View = editorControl1.View;
                treeViewSubstrateControl1.View = editorControl1.View;

                if (document != null)
                {
                    document.OnBeforeOpen += Document_OnBeforeOpen;
                    document.OnAfterOpen += Document_OnAfterOpen;
                    document.OnBeforeSave += Document_OnBeforeSave;
                    document.OnAfterSave += Document_OnAfterSave;
                    PropertyGridCtrl.SelecteObject = document.Selected;
                }
            }
        }

        /// <summary>
        /// Get current view.
        /// <para>현재 뷰를 가져옵니다.</para>
        /// <para>获取当前视图。</para>
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius3")]
        [DisplayName("View")]
        [Description("View")]
        public IView View
        {
            get { return EditorCtrl.View; }
        }

        /// <summary>
        /// Gets or sets the RTC(scanner) instance and wires all RTC-related controls.
        /// <para>RTC(스캐너) 인스턴스를 가져오거나 설정하고 모든 RTC 관련 컨트롤을 연결합니다.</para>
        /// <para>获取或设置 RTC（扫描仪）实例，并连接所有 RTC 相关控件。</para>
        /// </summary>
        /// <remarks>Created by <see cref="ScannerFactory"/>.</remarks>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius3")]
        [DisplayName("Scanner")]
        [Description("Scanner Instance")]
        public IScanner Scanner
        {
            get => scanner;
            set
            {
                if (scanner == value) return;

                if (scanner is IRtcMoF oldMof)
                    oldMof.OnEncoderChanged -= Mof_OnEncoderChanged;

                scanner = value;

                if (laser != null)
                    laser.Scanner = scanner;

                ScannerCtrl.Scanner = scanner;
                var rtc = value as IRtc;
                MarkerCtrl.Rtc = rtc;
                ManualCtrl.Rtc = rtc;
                EditorCtrl.Rtc = rtc;
                PowerMapCtrl.Rtc = rtc;

                if (scanner != null)
                {
                    PropertyVisibility();
                    MenuVisibility();

                    if (scanner is IRtcMoF newMof)
                        newMof.OnEncoderChanged += Mof_OnEncoderChanged;

                    if (rtc.KFactor <= 0)
                        throw new InvalidDataException("Kfactor value is invalid. must be assigned correctly !");                 
                }
            }
        }

        /// <summary>
        /// Gets or sets the laser and wires dependent controls and pen power mappings.
        /// <para>레이저를 가져오거나 설정하고 종속 컨트롤 및 펜 파워 매핑을 연결합니다.</para>
        /// <para>获取或设置激光器，并连接相关的控件和笔功率映射。</para>
        /// </summary>
        /// <remarks>Created by <see cref="LaserFactory"/>.</remarks>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius3")]
        [DisplayName("Laser")]
        [Description("Laser Instance")]
        public ILaser Laser
        {
            get => laser;
            set
            {
                laser = value;

                if (laser != null)
                {
                    laser.Scanner = scanner;
                    UpdatePowerMap();
                }

                LaserCtrl.Laser = laser;
                MarkerCtrl.Laser = laser;
                ManualCtrl.Laser = laser;
                PowerMeterCtrl.Laser = laser;
                PowerMapCtrl.Laser = laser;
                ScannerPenCtrl.Document = document;
            }
        }

        /// <summary>
        /// Gets or sets the marker and wires all marker-dependent controls and events.
        /// <para>마커를 가져오거나 설정하고 모든 마커 종속 컨트롤 및 이벤트를 연결합니다.</para>
        /// <para>获取或设置标记，并连接所有依赖于标记的控件和事件。</para>
        /// </summary>
        /// <remarks>Created by <see cref="MarkerFactory"/>.</remarks>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius3")]
        [DisplayName("Marker")]
        [Description("Marker Instance")]
        public IMarker Marker
        {
            get => marker;
            set
            {
                if (marker == value) return;

                if (marker != null)
                {
                    marker.OnStarted -= Marker_OnStarted;
                    marker.OnEnded -= Marker_OnEnded;
                }

                marker = value;

                MarkerCtrl.Marker = marker;
                ManualCtrl.Marker = marker;
                RtcDOCtrl.Marker = marker;
                OffsetCtrl.Marker = marker;
                EditorCtrl.Marker = marker;
                PropertyGridCtrl.Marker = marker;

                if (marker != null)
                {
                    marker.OnStarted += Marker_OnStarted;
                    marker.OnEnded += Marker_OnEnded;
                }
            }
        }

        /// <summary>
        /// Gets or sets the power meter and wires related control/event hooks.
        /// <para>파워 미터를 가져오거나 설정하고 관련 컨트롤/이벤트 후크를 연결합니다.</para>
        /// <para>获取或设置功率计，并连接相关的控制/事件挂钩。</para>
        /// </summary>
        /// <remarks>Created by <see cref="PowerMeterFactory"/>.</remarks>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius3")]
        [DisplayName("PowerMeter")]
        [Description("PowerMeter Instance")]
        public IPowerMeter PowerMeter
        {
            get => powerMeter;
            set
            {
                if (powerMeter == value) return;

                if (powerMeter != null)
                {
                    powerMeter.OnStarted -= PowerMeter_OnStarted;
                    powerMeter.OnStopped -= PowerMeter_OnStopped;
                    powerMeter.OnMeasured -= PowerMeter_OnMeasured;
                    powerMeter.OnCleared -= PowerMeter_OnCleared;
                }

                powerMeter = value;

                PowerMeterCtrl.PowerMeter = powerMeter;
                PowerMapCtrl.PowerMeter = powerMeter;
                MarkerCtrl.PowerMeter = powerMeter;

                if (powerMeter != null)
                {
                    lblPowerWatt.Text = "0.0 W";
                    powerMeter.OnStarted += PowerMeter_OnStarted;
                    powerMeter.OnStopped += PowerMeter_OnStopped;
                    powerMeter.OnMeasured += PowerMeter_OnMeasured;
                    powerMeter.OnCleared += PowerMeter_OnCleared;
                }
            }
        }

        /// <summary>
        /// Gets or sets RTC DI (Extension1) input port binding.
        /// <para>RTC DI (Extension1) 입력 포트 바인딩을 가져오거나 설정합니다.</para>
        /// <para>获取或设置 RTC DI (Extension1) 输入端口绑定。</para>
        /// </summary>
        /// <remarks>Created by <see cref="IOFactory"/>.</remarks>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius3")]
        [DisplayName("DInput")]
        [Description("IDInput Instance for RTC Extension1 Port")]
        public IDInput DIExt1
        {
            get => dIExt1;
            set
            {
                if (dIExt1 == value) return;
                dIExt1?.Dispose();
                dIExt1 = value;
                RtcDICtrl.DIExt1 = dIExt1;
            }
        }

        /// <summary>
        /// Gets or sets RTC DI (Laser) input port binding (2-bit).
        /// <para>RTC DI (레이저) 입력 포트 바인딩(2비트)을 가져오거나 설정합니다.</para>
        /// <para>获取或设置 RTC DI（激光）输入端口绑定（2 位）。</para>
        /// </summary>
        /// <remarks>Created by <see cref="IOFactory"/>.</remarks>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius3")]
        [DisplayName("DInput")]
        [Description("IDInput Instance for RTC LASER Port")]
        public IDInput DILaserPort
        {
            get => dILaserPort;
            set
            {
                if (dILaserPort == value) return;
                dILaserPort?.Dispose();
                dILaserPort = value;
                RtcDICtrl.DILaserPort = dILaserPort;
            }
        }

        /// <summary>
        /// Gets or sets RTC DO (Extension1) output port binding (16-bit).
        /// <para>RTC DO (Extension1) 출력 포트 바인딩(16비트)을 가져오거나 설정합니다.</para>
        /// <para>获取或设置 RTC DO (Extension1) 输出端口绑定（16 位）。</para>
        /// </summary>
        /// <remarks>Created by <see cref="IOFactory"/>.</remarks>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius3")]
        [DisplayName("DOutput")]
        [Description("IDOutput Instance for RTC EXTENSION1 Port")]
        public IDOutput DOExt1
        {
            get => dOExt1;
            set
            {
                if (dOExt1 == value) return;
                dOExt1?.Dispose();
                dOExt1 = value;
                RtcDOCtrl.DOExt1 = dOExt1;
            }
        }

        /// <summary>
        /// Gets or sets RTC DO (Extension2) output port binding (8-bit).
        /// <para>RTC DO (Extension2) 출력 포트 바인딩(8비트)을 가져오거나 설정합니다.</para>
        /// <para>获取或设置 RTC DO (Extension2) 输出端口绑定（8 位）。</para>
        /// </summary>
        /// <remarks>Created by <see cref="IOFactory"/>.</remarks>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius3")]
        [DisplayName("DOutput")]
        [Description("IDOutput Instance for RTC EXTENSION2 Port")]
        public IDOutput DOExt2
        {
            get => dOExt2;
            set
            {
                if (dOExt2 == value) return;
                dOExt2?.Dispose();
                dOExt2 = value;
                RtcDOCtrl.DOExt2 = dOExt2;
            }
        }

        /// <summary>
        /// Gets or sets RTC DO (Laser) output port binding (2-bit).
        /// <para>RTC DO (레이저) 출력 포트 바인딩(2비트)을 가져오거나 설정합니다.</para>
        /// <para>获取或设置 RTC DO（激光）输出端口绑定（2 位）。</para>
        /// </summary>
        /// <remarks>Created by <see cref="IOFactory"/>.</remarks>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius3")]
        [DisplayName("DOutput")]
        [Description("IDOutput Instance for RTC LASER Port")]
        public IDOutput DOLaserPort
        {
            get => dOLaserPort;
            set
            {
                if (dOLaserPort == value) return;
                dOLaserPort?.Dispose();
                dOLaserPort = value;
                RtcDOCtrl.DOLaserPort = dOLaserPort;
            }
        }

        /// <summary>
        /// Get <see cref="TreeViewPageControl"/> for <see cref="IDocumentData.Pages"/>
        /// <para><see cref="IDocumentData.Pages"/>에 대한 <see cref="TreeViewPageControl"/>을 가져옵니다.</para>
        /// <para>获取 <see cref="IDocumentData.Pages"/> 的 <see cref="TreeViewPageControl"/>。</para>
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius3")]
        [DisplayName("PageControls")]
        [Description("Array of TreeViewPageControl UserControl")]
        public SpiralLab.Sirius3.UI.WinForms.TreeViewPageControl[] PageControls
        {
            get
            {
                return new SpiralLab.Sirius3.UI.WinForms.TreeViewPageControl[]
                    {
                        treeViewPageControl1,
                        treeViewPageControl2,
                    };
            }
        }

        /// <summary>
        /// Get <see cref="TreeViewBlockControl"/> for <see cref="IDocumentData.Blocks"/>
        /// <para><see cref="IDocumentData.Blocks"/>에 대한 <see cref="TreeViewBlockControl"/>을 가져옵니다.</para>
        /// <para>获取 <see cref="IDocumentData.Blocks"/> 的 <see cref="TreeViewBlockControl"/>。</para>
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius3")]
        [DisplayName("BlockControl")]
        [Description("TreeViewBlockControl UserControl")]
        public SpiralLab.Sirius3.UI.WinForms.TreeViewBlockControl BlockControl
        {
            get
            {
                return treeViewBlockControl1;
            }
        }
        /// <summary>
        /// Get <see cref="TreeViewWaferControl"/> for <see cref="IDocumentData.Wafers"/>
        /// <para><see cref="IDocumentData.Wafers"/>에 대한 <see cref="TreeViewWaferControl"/>을 가져옵니다.</para>
        /// <para>获取 <see cref="IDocumentData.Wafers"/> 的 <see cref="TreeViewWaferControl"/>。</para>
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius3")]
        [DisplayName("SubstrateControl")]
        [Description("TreeViewWaferControl UserControl")]
        public SpiralLab.Sirius3.UI.WinForms.TreeViewWaferControl WaferControl
        {
            get
            {
                return treeViewWaferControl1;
            }
        }

        /// <summary>
        /// Get <see cref="TreeViewSubstrateControl"/> for <see cref="IDocumentData.Substrates"/>
        /// <para><see cref="IDocumentData.Substrates"/>에 대한 <see cref="TreeViewSubstrateControl"/>을 가져옵니다.</para>
        /// <para>获取 <see cref="IDocumentData.Substrates"/> 的 <see cref="TreeViewSubstrateControl"/>。</para>
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius3")]
        [DisplayName("SubstrateControl")]
        [Description("TreeViewSubstrateControl UserControl")]
        public SpiralLab.Sirius3.UI.WinForms.TreeViewSubstrateControl SubstrateControl
        {
            get
            {
                return treeViewSubstrateControl1;
            }
        }

        /// <summary>
        /// Gets the property grid control wrapper.
        /// <para>속성 그리드 컨트롤 래퍼를 가져옵니다.</para>
        /// <para>获取属性网格控件包装器。</para>
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius3")]
        [DisplayName("PropertyGridControl")]
        [Description("PropertyGrid UserControl")]
        public SpiralLab.Sirius3.UI.WinForms.PropertyGridControl PropertyGridCtrl => propertyGridControl1;

        /// <summary>
        /// Gets the editor (OpenGL) control wrapper.
        /// <para>편집기(OpenGL) 컨트롤 래퍼를 가져옵니다.</para>
        /// <para>获取编辑器（OpenGL）控件包装器。</para>
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius3")]
        [DisplayName("EditorUserControl")]
        [Description("Editor UserControl")]
        public SpiralLab.Sirius3.UI.WinForms.EditorControl EditorCtrl => editorControl1;

        /// <summary>
        /// Gets the laser control wrapper.
        /// <para>레이저 컨트롤 래퍼를 가져옵니다.</para>
        /// <para>获取激光控制器包装器。</para>
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius3")]
        [DisplayName("LaserControl")]
        [Description("Laser UserControl")]
        public SpiralLab.Sirius3.UI.WinForms.LaserControl LaserCtrl => laserControl1;

        /// <summary>
        /// Gets the RTC control wrapper.
        /// <para>RTC 컨트롤 래퍼를 가져옵니다.</para>
        /// <para>获取 RTC 控制器包装器。</para>
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius3")]
        [DisplayName("RtcUserControl")]
        [Description("Rtc UserControl")]
        public SpiralLab.Sirius3.UI.WinForms.ScannerControl ScannerCtrl => scannerControl1;

        /// <summary>
        /// Gets the marker control wrapper.
        /// <para>마커 컨트롤 래퍼를 가져옵니다.</para>
        /// <para>获取标记控制器包装器。</para>
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius3")]
        [DisplayName("MarkerControl")]
        [Description("Marker UserControl")]
        public SpiralLab.Sirius3.UI.WinForms.MarkerControl MarkerCtrl => markerControl1;

        /// <summary>
        /// Gets the offset control wrapper.
        /// <para>오프셋 컨트롤 래퍼를 가져옵니다.</para>
        /// <para>获取偏移控制器包装器。</para>
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius3")]
        [DisplayName("OffsetControl")]
        [Description("Offset UserControl")]
        public SpiralLab.Sirius3.UI.WinForms.OffsetControl OffsetCtrl => offsetControl1;

        /// <summary>
        /// Gets the RTC DI control wrapper.
        /// <para>RTC DI 컨트롤 래퍼를 가져옵니다.</para>
        /// <para>获取 RTC DI 控制器包装器。</para>
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius3")]
        [DisplayName("RtcDIControl")]
        [Description("RtcDI UserControl")]
        public SpiralLab.Sirius3.UI.WinForms.RtcDIControl RtcDICtrl => rtcDIControl1;

        /// <summary>
        /// Gets the RTC DO control wrapper.
        /// <para>RTC DO 컨트롤 래퍼를 가져옵니다.</para>
        /// <para>获取 RTC DO 控制器包装器。</para>
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius3")]
        [DisplayName("RtcDOControl")]
        [Description("RtcDO UserControl")]
        public SpiralLab.Sirius3.UI.WinForms.RtcDOControl RtcDOCtrl => rtcDOControl1;

        /// <summary>
        /// Gets the manual control wrapper.
        /// <para>수동 컨트롤 래퍼를 가져옵니다.</para>
        /// <para>获取手动控制器包装器。</para>
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius3")]
        [DisplayName("ManualControl (Customized)")]
        [Description("Manual UserControl")]
        public SpiralLab.Sirius3.UI.WinForms.ManualControl ManualCtrl => manualControl1;

        /// <summary>
        /// Gets the power meter control wrapper.
        /// <para>파워 미터 컨트롤 래퍼를 가져옵니다.</para>
        /// <para>获取功率计控制器包装器。</para>
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius3")]
        [DisplayName("PowerMeterControl")]
        [Description("PowerMeter UserControl")]
        public SpiralLab.Sirius3.UI.WinForms.PowerMeterControl PowerMeterCtrl => powerMeterControl1;

        /// <summary>
        /// Gets the power map control wrapper.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius3")]
        [DisplayName("PowerMapControl")]
        [Description("PowerMap UserControl")]
        public SpiralLab.Sirius3.UI.WinForms.PowerMapControl PowerMapCtrl => powerMapControl1;

        /// <summary>
        /// Gets the scanner pen control wrapper.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius3")]
        [DisplayName("Scanner PenUserControl")]
        [Description("Scanner Pen UserControl")]
        public SpiralLab.Sirius3.UI.WinForms.ScannerPenControl ScannerPenCtrl => scannerPenControl1;

        /// <summary>
        /// Gets the layer pen control wrapper.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius3")]
        [DisplayName("Layer PenUserControl")]
        [Description("Layer Pen UserControl")]
        public SpiralLab.Sirius3.UI.WinForms.LayerPenControl LayerPenCtrl => layerPenControl1;

        /// <summary>
        /// Gets the log control.
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius3")]
        [DisplayName("LogUserControl")]
        [Description("Log UserControl")]
        public SpiralLab.Sirius3.UI.WinForms.LogControl LogCtrl => logControl1;

        #endregion

        #region Constructor & Form Lifecycle

        /// <summary>
        /// Initializes a new instance of <see cref="SiriusEditorControl"/> and wires UI events.
        /// <para><see cref="SiriusEditorControl"/>의 새 인스턴스를 초기화하고 UI 이벤트를 연결합니다.</para>
        /// <para>初始化 <see cref="SiriusEditorControl"/> 的新实例并连接 UI 事件。</para>
        /// </summary>
        public SiriusEditorControl()
        {
            InitializeComponent();

            // Embed editor control into tab page
            tabEditor.Controls.Add(editorControl1);
            editorControl1.Dock = DockStyle.Fill;
            editorControl1.Location = new Point(0, 0);
            editorControl1.Margin = new Padding(0);
            editorControl1.Name = "Editor";

            Load += SiriusEditorControl_Load;
            Disposed += SiriusEditorControl_Disposed;
            VisibleChanged += SiriusEditorControl_VisibleChanged;

            timerProgress.Interval = 100;
            timerProgress.Tick += TimerProgress_Tick;

            timerStatus.Interval = 100;
            timerStatus.Tick += TimerStatus_Tick;

            lblEncoder.DoubleClick += LblEncoder_DoubleClick;
            lblEncoder.DoubleClickEnabled = true;

            tbcLeft.SelectedIndexChanged += tbcLeft_SelectedIndexChanged;
            btnNew.Click += BtnNew_Click;
            btnOpen.Click += BtnOpen_Click;
            btnSave.Click += BtnSave_Click;
            btnLock.Click += BtnLock_Click;

            // Hide log window by default
            splitContainer7.Panel2Collapsed = true;
            splitContainer7.Panel2Collapsed = false;
            splitContainer7.Panel2Collapsed = true;
            btnLogWindow.Click += (_, __) =>
            {
                splitContainer7.Panel2Collapsed = !splitContainer7.Panel2Collapsed;
            };

        }

        /// <summary>
        /// Initializes core components, editor surface, document, and default virtual devices.
        /// <para>핵심 구성 요소, 편집기 화면, 문서 및 기본 가상 장치를 초기화합니다.</para>
        /// <para>初始化核心组件、编辑器表面、文档和默认虚拟设备。</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        private void SiriusEditorControl_Load(object sender, EventArgs e)
        {
            Document = new DocumentBase();
            //Document.ActNew(true, true, true, true, true, true, true, true, true);
            Document.ActNew(true, true, true, true, true, false, false, false, false); //scanner layer 펜 재 생성 막기

            Marker?.Ready(Document);
        }
        /// <summary>
        /// Handles form closing; disposes timers.
        /// <para>폼 닫기를 처리하고 타이머를 해제합니다.</para>
        /// <para>处理窗体关闭；释放计时器。</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        private void SiriusEditorControl_Disposed(object sender, EventArgs e)
        {
            document?.ActSimulateStop(false);
            timerStatus.Enabled = false;
            timerProgress.Enabled = false;
            timerStatus.Tick -= TimerStatus_Tick;
            timerProgress.Tick -= TimerProgress_Tick;
        }
        /// <summary>
        /// Enables or disables the status timer based on form visibility.
        /// <para>폼 가시성에 따라 상태 타이머를 활성화하거나 비활성화합니다.</para>
        /// <para>根据窗体可见性启用或禁用状态计时器。</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        private void SiriusEditorControl_VisibleChanged(object sender, EventArgs e)
        {
            timerStatus.Enabled = Visible;
        }
        #endregion

        #region Document Events

        /// <summary>
        /// Called before a document open operation.
        /// <para>문서 열기 작업 전에 호출됩니다.</para>
        /// <para>在文档打开操作之前调用。</para>
        /// </summary>
        /// <param name="_document">The document instance.</param>
        private void Document_OnBeforeOpen(IDocument _document)
        {
            // Reserved for pre-open logic
        }

        /// <summary>
        /// Called after a document has been opened; updates pens and property grid.
        /// <para>문서가 열린 후 호출됩니다. 펜과 속성 그리드를 업데이트합니다.</para>
        /// <para>文档打开后调用；更新画笔和属性网格。</para>
        /// </summary>
        /// <param name="doc">The document instance.</param>
        /// <param name="fileName">The name of the opened file.</param>
        private void Document_OnAfterOpen(IDocument doc, string fileName)
        {
            if (!IsHandleCreated || IsDisposed) return;

            Invoke(new MethodInvoker(() =>
            {
                UpdatePowerMap();
                ScannerPenCtrl.Document = document;

                lblFileName.Text = fileName;
                PropertyGridCtrl.Refresh();
            }));
        }

        /// <summary>
        /// Called before a document save operation.
        /// <para>문서 저장 작업 전에 호출됩니다.</para>
        /// <para>在文档保存操作之前调用。</para>
        /// </summary>
        /// <param name="_document">The document instance.</param>
        private void Document_OnBeforeSave(IDocument _document)
        {
            // Reserved for pre-save logic
        }

        /// <summary>
        /// Called after a document has been saved; updates file name label.
        /// <para>문서가 저장된 후 호출됩니다. 파일 이름 레이블을 업데이트합니다.</para>
        /// <para>文档保存后调用；更新文件名标签。</para>
        /// </summary>
        /// <param name="_document">The document instance.</param>
        /// <param name="fileName">The name of the saved file.</param>
        private void Document_OnAfterSave(IDocument _document, string fileName)
        {
            if (!stsBottom.IsHandleCreated || IsDisposed) return;

            stsBottom.Invoke(new MethodInvoker(() =>
            {
                lblFileName.Text = fileName;
            }));
        }

        #endregion

        #region Status / Marker / PowerMeter UI
        /// <summary>
        /// Toggles the log screen (reserved, currently not used).
        /// <para>로그 화면을 토글합니다 (예약됨, 현재 사용되지 않음).</para>
        /// <para>切换日志屏幕（保留，当前未使用）。</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        private void LblLog_DoubleClick(object sender, EventArgs e)
        {
            // Reserved
        }

        /// <summary>
        /// Resets MoF encoder values with user confirmation.
        /// <para>사용자 확인을 통해 MoF 인코더 값을 재설정합니다.</para>
        /// <para>通过用户确认重置 MoF 编码器值。</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        private void LblEncoder_DoubleClick(object sender, EventArgs e)
        {
            if (Scanner is not IRtcMoF rtcMoF) return;

            var form = new SpiralLab.Sirius3.UI.WinForms.MessageBox(
                "Do you want to reset encoder values ?",
                "Warning",
                MessageBoxButtons.YesNo);

            var dialogResult = form.ShowDialog(this);
            if (dialogResult == DialogResult.Yes)
                rtcMoF.CtlMofEncoderReset();
        }

        /// <summary>
        /// Periodic status painter for Ready/Busy/Error (and Remote if enabled).
        /// <para>준비/바쁨/오류 (및 원격이 활성화된 경우)에 대한 주기적인 상태 표시기입니다.</para>
        /// <para>用于就绪/忙碌/错误（如果启用远程）的周期性状态绘制器。</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        private void TimerStatus_Tick(object sender, EventArgs e)
        {
            if (Marker == null) return;

            // Ready
            if (Marker.IsReady)
            {
                lblReady.ForeColor = Color.Black;
                lblReady.BackColor = Color.Lime;
            }
            else
            {
                lblReady.ForeColor = Color.White;
                lblReady.BackColor = Color.Green;
            }

            // Busy
            if (Marker.IsBusy)
            {
                timerStatusColorCounts = unchecked(timerStatusColorCounts + 1);
                if (timerStatusColorCounts % 2 == 0)
                {
                    lblBusy.BackColor = Color.Orange;
                    lblBusy.ForeColor = Color.Black;
                }
                else
                {
                    lblBusy.BackColor = Color.Olive;
                    lblBusy.ForeColor = Color.White;
                }
            }
            else
            {
                lblBusy.BackColor = Color.Olive;
                lblBusy.ForeColor = Color.White;
                timerStatusColorCounts = 0;
            }

            // Error
            if (Marker.IsError)
            {
                lblError.ForeColor = Color.White;
                lblError.BackColor = Color.Red;
            }
            else
            {
                lblError.ForeColor = Color.White;
                lblError.BackColor = Color.Maroon;
            }
        }

        /// <summary>
        /// Called when marking starts; disables editing and starts progress timer.
        /// <para>마킹이 시작될 때 호출됩니다. 편집을 비활성화하고 진행 타이머를 시작합니다.</para>
        /// <para>标记开始时调用；禁用编辑并启动进度计时器。</para>
        /// </summary>
        /// <param name="_marker">The marker instance.</param>
        private void Marker_OnStarted(IMarker _marker)
        {
            if (!IsHandleCreated || IsDisposed) return;

            Invoke(new MethodInvoker(() =>
            {
                timerProgressStopwatch.Restart();
                timerProgress.Enabled = true;
                ControlEnableOrNot(false);
            }));
        }

        /// <summary>
        /// Periodically updates elapsed marking time while marker is busy.
        /// <para>마커가 작동 중일 때 경과된 마킹 시간을 주기적으로 업데이트합니다.</para>
        /// <para>在标记器忙碌时，定期更新已用标记时间。</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        private void TimerProgress_Tick(object sender, EventArgs e)
        {
            if (!stsBottom.IsHandleCreated || IsDisposed) return;

            if ((timerProgressColorCounts++ % 2) == 0)
                lblProcessTime.ForeColor = stsBottom.ForeColor;
            else
                lblProcessTime.ForeColor = Color.Red;

            lblProcessTime.Text = $"{timerProgressStopwatch.ElapsedMilliseconds / 1000.0:F3} sec";
        }

        /// <summary>
        /// Called when marking ends; re-enables editing and shows total time.
        /// <para>마킹이 종료될 때 호출됩니다. 편집을 다시 활성화하고 총 시간을 표시합니다.</para>
        /// <para>标记结束时调用；重新启用编辑并显示总时间。</para>
        /// </summary>
        /// <param name="_marker">The marker instance.</param>
        /// <param name="success">True if marking was successful, false otherwise.</param>
        /// <param name="ts">The elapsed time for the marking operation.</param>
        private void Marker_OnEnded(IMarker _marker, bool success, TimeSpan? ts)
        {
            if (!IsHandleCreated || IsDisposed) return;

            Invoke(new MethodInvoker(() =>
            {
                timerProgressStopwatch.Stop();
                timerProgress.Enabled = false;

                lblProcessTime.Text = $"{ts.GetValueOrDefault().TotalSeconds:F3} sec";
                lblProcessTime.ForeColor = success ? stsBottom.ForeColor : Color.Red;

                if (!btnLock.Checked)
                    ControlEnableOrNot(true);
                EditorCtrl.Focus();
            }));
        }

        /// <summary>
        /// Called when MoF encoders change; updates encoder label text depending on MoF mode.
        /// <para>MoF 인코더가 변경될 때 호출됩니다. MoF 모드에 따라 인코더 레이블 텍스트를 업데이트합니다.</para>
        /// <para>MoF 编码器更改时调用；根据 MoF 模式更新编码器标签文本。</para>
        /// </summary>
        /// <param name="rtcMoF">The IRtcMoF instance.</param>
        /// <param name="encX">The X-axis encoder value.</param>
        /// <param name="encY">The Y-axis encoder value.</param>
        private void Mof_OnEncoderChanged(IRtcMoF rtcMoF, int encX, int encY)
        {
            if (!stsBottom.IsHandleCreated || IsDisposed) return;

            try
            {
                switch (rtcMoF.MoFMode)
                {
                    default:
                    case RtcMoFModes.XY:
                        rtcMoF.CtlMofGetEncoder(out _, out _, out var xMm, out var yMm);
                        stsBottom.Invoke(new MethodInvoker(() =>
                        {
                            lblEncoder.Text = string.Format("ENC: {0:F3}, {1:F3}mm ({2}, {3})", xMm, yMm, encX, encY);
                        }));
                        break;

                    case RtcMoFModes.Angular:
                        rtcMoF.CtlMofGetAngularEncoder(out _, out var angle);
                        stsBottom.Invoke(new MethodInvoker(() =>
                        {
                            lblEncoder.Text = string.Format("ENC: {0:F3}° ({1})", angle, encX);
                        }));
                        break;
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Clears power display when power meter data cleared.
        /// <para>파워 미터 데이터가 지워지면 파워 디스플레이를 지웁니다.</para>
        /// <para>清除功率计数据时清除功率显示。</para>
        /// </summary>
        /// <param name="_powerMeter">The power meter instance.</param>
        private void PowerMeter_OnCleared(IPowerMeter _powerMeter)
        {
            if (!stsBottom.IsHandleCreated || IsDisposed) return;

            stsBottom.Invoke(new MethodInvoker(() =>
            {
                lblPowerWatt.Text = "(Empty)";
            }));
        }

        /// <summary>
        /// Shows status when power measurement starts.
        /// <para>전력 측정이 시작될 때 상태를 표시합니다.</para>
        /// <para>功率测量开始时显示状态。</para>
        /// </summary>
        /// <param name="_powerMeter">The power meter instance.</param>
        private void PowerMeter_OnStarted(IPowerMeter _powerMeter)
        {
            if (!stsBottom.IsHandleCreated || IsDisposed) return;

            stsBottom.Invoke(new MethodInvoker(() =>
            {
                lblPowerWatt.Text = "Started...";
            }));
        }

        /// <summary>
        /// Reserved: when power meter stops.
        /// <para>예약됨: 파워 미터가 중지될 때.</para>
        /// <para>保留：当功率计停止时。</para>
        /// </summary>
        /// <param name="_powerMeter">The power meter instance.</param>
        private void PowerMeter_OnStopped(IPowerMeter _powerMeter)
        {
            // Reserved
        }

        /// <summary>
        /// Displays latest power value when measured.
        /// <para>측정될 때 최신 전력 값을 표시합니다.</para>
        /// <para>测量时显示最新功率值。</para>
        /// </summary>
        /// <param name="_powerMeter">The power meter instance.</param>
        /// <param name="_dt">The date and time of the measurement.</param>
        /// <param name="watt">The measured power in watts.</param>
        private void PowerMeter_OnMeasured(IPowerMeter _powerMeter, DateTime _dt, double watt)
        {
            if (!stsBottom.IsHandleCreated || IsDisposed) return;

            try
            {
                stsBottom.BeginInvoke(new MethodInvoker(() =>
                {
                    lblPowerWatt.Text = $"{watt:F3} W";
                }));
            }
            catch
            {
            }
        }

        #endregion

        #region UI Visibility / Editability

        /// <summary>
        /// Updates menu/control visibility by RTC capabilities (placeholder).
        /// <para>RTC 기능에 따라 메뉴/컨트롤 가시성을 업데이트합니다 (자리 표시자).</para>
        /// <para>根据 RTC 功能更新菜单/控件可见性（占位符）。</para>
        /// </summary>
        private void MenuVisibility()
        {
            Debug.Assert(Scanner != null);
            // Keep for future RTC-card specific UI toggles
        }

        /// <summary>
        /// Adjusts entity property visibility based on RTC capabilities.
        /// <para>RTC 기능에 따라 엔티티 속성 가시성을 조정합니다.</para>
        /// <para>根据 RTC 功能调整实体属性可见性。</para>
        /// </summary>
        private void PropertyVisibility()
        {
            Debug.Assert(Scanner != null);
            EntityScannerPen.PropertyVisibility(Scanner);
            EntityLayerPen.PropertyVisibility(Scanner);
        }


        /// <summary>
        /// Enables or disables editing-related controls when marker is busy.
        /// <para>마커가 사용 중일 때 편집 관련 컨트롤을 활성화하거나 비활성화합니다.</para>
        /// <para>当标记器忙碌时，启用或禁用与编辑相关的控件。</para>
        /// </summary>
        /// <param name="isEnable">True to enable; false to disable.</param>
        public virtual void ControlEnableOrNot(bool isEnable)
        {
            if (!IsHandleCreated || IsDisposed) return;

            Invoke(new MethodInvoker(() =>
            {
                //tlsTop1.Enabled = isEnable;
                foreach (var pc in PageControls)
                    pc.Enabled = isEnable;
                PropertyGridCtrl.Enabled = isEnable;
                BlockControl.Enabled = isEnable;
                WaferControl.Enabled = isEnable;
                SubstrateControl.Enabled = isEnable;
                //EditorCtrl.Enabled = isEnable;
                View.IsAllowEdit = isEnable;
                //RtcDICtrl.Enabled = isEnable;
                //LaserCtrl.Enabled = isEnable;
                //ScannerPenCtrl.Enabled = isEnable;
                //LayerPenCtrl.Enabled = isEnable;
                //LogCtrl.Enabled = isEnable;
                //MarkerCtrl.Enabled = isEnable;
                OffsetCtrl.Enabled = isEnable;

#if DEBUG
                // Keep enabled for debugging
#else
                ManualCtrl.Enabled = isEnable;
                ScannerCtrl.Enabled = isEnable;
                PowerMeterCtrl.Enabled = isEnable;
                PowerMapCtrl.Enabled = isEnable;
                RtcDOCtrl.Enabled = isEnable;
#endif
            }));
        }

        /// <summary>
        /// Update laser and powermap information at scanner pens
        /// </summary>
        private void UpdatePowerMap()
        {
            var powerControl = laser as ILaserPowerControl;

            if (null != document && null != powerControl)
            {
                foreach (var child in document.DocumentData.ScannerPens.Children)
                {
                    var pen = child as EntityScannerPen;
                    pen.PowerMax = laser.MaxPowerWatt;
                    pen.PowerMap = powerControl?.PowerMap;
                }

                foreach (var child in document.DocumentData.LayerPens.Children)
                {
                    // Reserved: layer pen updates (if needed)
                    _ = child as EntityLayerPen;
                }
            }
        }

        #endregion

        #region Left Tab / File Buttons

        /// <summary>
        /// Switches active page/layer in the document based on the selected left tab.
        /// <para>선택된 왼쪽 탭에 따라 문서의 활성 페이지/레이어를 전환합니다.</para>
        /// <para>根据选定的左侧选项卡切换文档中的活动页面/图层。</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        private void tbcLeft_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Document == null) return;

            Cursor.Current = Cursors.WaitCursor;
            Document.ActSelectClear();

            switch (tbcLeft.SelectedIndex)
            {
                case 0:
                    Document.Page = DocumentPages.Page1;
                    Document.ActivePage = Document.DocumentData.Pages[0];
                    editorControl1.Document.ActRegen();
                    editorControl1.View.Camera.Fit(editorControl1.View, null, new IEntity[] { Document.ActivePage?.ActiveLayer });
                    break;
                case 1:
                    Document.Page = DocumentPages.Page2;
                    Document.ActivePage = Document.DocumentData.Pages[1];
                    editorControl1.Document.ActRegen();
                    editorControl1.View.Camera.Fit(editorControl1.View, null, new IEntity[] { Document.ActivePage?.ActiveLayer });
                    break;
                //case 2:
                //    Document.Page = DocumentPages.Page3;
                //    Document.ActivePage = Document.DocumentData.Pages[2];
                //    editorControl1.Document.ActRegen();
                //    editorControl1.View.Camera.Fit(editorControl1.View, null, new IEntity[] { Document.ActivePage?.ActiveLayer });
                //    break;
                //case 3:
                //    Document.Page = DocumentPages.Page4;
                //    Document.ActivePage = Document.DocumentData.Pages[3];
                //    editorControl1.Document.ActRegen();
                //    editorControl1.View.Camera.Fit(editorControl1.View, null, new IEntity[] { Document.ActivePage?.ActiveLayer });
                //    break;
                case 2:
                    Document.Page = DocumentPages.Block;
                    editorControl1.Document.ActRegen();
                    editorControl1.View.Camera.Fit(editorControl1.View, null, Document.DocumentData.Blocks.Children.ToArray());
                    break;
                case 3:
                    Document.Page = DocumentPages.Wafer;
                    editorControl1.Document.ActRegen();
                    editorControl1.View.Camera.Fit(editorControl1.View, null, Document.DocumentData.Wafers.Children.ToArray());
                    break;
                case 4:
                    Document.Page = DocumentPages.Substrate;
                    editorControl1.Document.ActRegen();
                    editorControl1.View.Camera.Fit(editorControl1.View, null, Document.DocumentData.Substrates.Children.ToArray());
                    break;
            }

            editorControl1.View.DoRender();
            Cursor.Current = Cursors.Default;
        }

        /// <summary>
        /// Creates a new document according to selected include flags.
        /// <para>선택된 포함 플래그에 따라 새 문서를 생성합니다.</para>
        /// <para>根据选定的包含标志创建新文档。</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        private void BtnNew_Click(object sender, EventArgs e)
        {
            bool includePage1 = mnuIncludePage1.Checked;
            bool includePage2 = mnuIncludePage2.Checked;
            bool includePage3 = mnuIncludePage3.Checked;
            bool includePage4 = mnuIncludePage4.Checked;
            bool includeBlocks = mnuIncludeBlocks.Checked;
            bool includeScannerPens = mnuIncludeScannerPens.Checked;
            bool includeLayerPens = mnuIncludeLayerPens.Checked;
            bool includeWafers = mnuIncludeWafers.Checked;
            bool includeSubstrates = mnuIncludeSubstrates.Checked;

            Document?.ActNew(
                includePage1,
                includePage2,
                includePage3,
                includePage4,
                includeBlocks,
                includeScannerPens,
                includeLayerPens,
                includeWafers,
                includeSubstrates);
        }

        /// <summary>
        /// Opens a document from disk with selected include flags.
        /// <para>선택된 포함 플래그로 디스크에서 문서를 엽니다.</para>
        /// <para>使用选定的包含标志从磁盘打开文档。</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        private void BtnOpen_Click(object sender, EventArgs e)
        {
            if (Document == null) return;

            using var dlg = new OpenFileDialog
            {
                Filter = SpiralLab.Sirius3.UI.Config.FileOpenFilters,
                Title = "Open File",
                InitialDirectory = SpiralLab.Sirius3.Config.RecipePath
            };

            if (dlg.ShowDialog() != DialogResult.OK) return;

            if (Document.IsModified)
            {
                var form = new SpiralLab.Sirius3.UI.WinForms.MessageBox(
                    "Not save yet ? Do you really want to open ?",
                    "Warning",
                    MessageBoxButtons.YesNo);

                var dialogResult = form.ShowDialog(this);
                if (dialogResult != DialogResult.Yes) return;
            }

            bool includeLayers = mnuIncludePage1.Checked;
            bool includeLayers2nd = mnuIncludePage2.Checked;
            bool includeBlocks = mnuIncludeBlocks.Checked;
            bool includeScannerPens = mnuIncludeScannerPens.Checked;
            bool includeLayerPens = mnuIncludeLayerPens.Checked;
            bool includeWafers = mnuIncludeWafers.Checked;
            bool includeSubstrates = mnuIncludeSubstrates.Checked;

            Document?.ActOpen(
                dlg.FileName,
                includeLayers,
                includeLayers2nd,
                includeBlocks,
                includeScannerPens,
                includeLayerPens,
                includeWafers,
                includeSubstrates);
        }

        /// <summary>
        /// Saves the current document to disk.
        /// <para>현재 문서를 디스크에 저장합니다.</para>
        /// <para>将当前文档保存到磁盘。</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (Document == null) return;

            using var dlg = new SaveFileDialog
            {
                Filter = SpiralLab.Sirius3.UI.Config.FileSaveFilters,
                Title = "Save File",
                InitialDirectory = SpiralLab.Sirius3.Config.RecipePath,
                OverwritePrompt = true
            };

            if (dlg.ShowDialog() != DialogResult.OK) return;
            Document.ActSave(dlg.FileName);
        }

        /// <summary>
        /// Toggles allow to edit(lock) at view or not
        /// </summary>
        private void BtnLock_Click(object sender, EventArgs e)
        {
            ControlEnableOrNot(!btnLock.Checked);
        }

        #endregion
    }
}
