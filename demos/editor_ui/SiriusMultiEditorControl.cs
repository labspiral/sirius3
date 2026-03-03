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
    /// Main WinForms editor control that hosts the OpenGL editor surface,
    /// device controls (Scanner/Laser/PowerMeter/IO/Marker), and document management UI.
    /// It supports multiple sets of devices for a single document.
    /// <para>To use this control, set <see cref="MaxDeviceCounts"/> (1 to 4) and register devices using <see cref="RegisterDevices"/> at specific indices. 
    /// You can switch the active device set by setting <see cref="CurrentDeviceIndex"/> (0 to <see cref="MaxDeviceCounts"/> - 1).</para>
    /// <para>OpenGL 편집기 화면, 장치 제어(스캐너/레이저/파워 미터/IO/마커) 및 문서 관리 UI를 호스팅하는 주 WinForms 편집기 컨트롤입니다.
    /// 하나의 문서에 대해 여러 개의 장치 세트(스캐너, 레이저, 파워 미터, 마커 등)를 지원합니다.
    /// 이 컨트롤을 사용하려면 <see cref="MaxDeviceCounts"/>(1~4)를 설정하고 <see cref="RegisterDevices"/>를 사용하여 특정 인덱스에 장치를 등록하십시오. 
    /// <see cref="CurrentDeviceIndex"/>(0 ~ <see cref="MaxDeviceCounts"/> - 1)를 설정하여 활성 장치 세트를 전환할 수 있습니다.</para>
    /// <para>主 WinForms 编辑器控件，托管 OpenGL 编辑器表面、设备控件（扫描仪/激光/功率计/IO/标记）和文档管理 UI。
    /// 它支持单个文档的多个设备集。
    /// 要使用此控件，请设置 <see cref="MaxDeviceCounts"/>（1 到 4）并使用 <see cref="RegisterDevices"/> 在特定索引处注册设备。 
    /// 您可以通过设置 <see cref="CurrentDeviceIndex"/>（0 到 <see cref="MaxDeviceCounts"/> - 1）来切换活动设备集。</para>
    /// </summary>
    public partial class SiriusMultiEditorControl : UserControl
    {
        #region Events
        /// <summary>
        /// Raised when after new button has pressed.
        /// <para>새 문서 버튼이 눌린 후 발생합니다.</para>
        /// <para>按下新建按钮后触发。</para>
        /// </summary>
        public event Action<SiriusMultiEditorControl> OnAfterNew;
        /// <summary>
        /// Raised when after open button has pressed.
        /// <para>열기 버튼이 눌린 후 발생합니다.</para>
        /// <para>按下打开按钮后触发。</para>
        /// </summary>
        public event Action<SiriusMultiEditorControl, string> OnAfterOpen;
        /// <summary>
        /// Raised when after save button has pressed.
        /// <para>저장 버튼이 눌린 후 발생합니다.</para>
        /// <para>按下保存按钮后触发。</para>
        /// </summary>
        public event Action<SiriusMultiEditorControl, string> OnAfterSave;

        /// <summary>
        /// Raised before the device set is changed.
        /// <para>장치 세트가 변경되기 전에 발생합니다.</para>
        /// <para>在设备集更改之前触发。</para>
        /// </summary>
        public event Action<SiriusMultiEditorControl> OnBeforeChangeDevice;
        /// <summary>
        /// Raised after the device set is changed.
        /// <para>장치 세트가 변경된 후 발생합니다.</para>
        /// <para>在设备集更改之后触发。</para>
        /// </summary>
        public event Action<SiriusMultiEditorControl> OnAfterChangeDevice;
        #endregion

        #region Fields
        private IDocument document;
        private IScanner[] scanners = new IScanner[DEFAULT_MAX_DEVICE_COUNTS];
        private ILaser[] lasers = new ILaser[DEFAULT_MAX_DEVICE_COUNTS];
        private IMarker[] markers = new IMarker[DEFAULT_MAX_DEVICE_COUNTS];
        private IPowerMeter[] powerMeters = new IPowerMeter[DEFAULT_MAX_DEVICE_COUNTS];

        private IDInput[] dIExt1s = new IDInput[DEFAULT_MAX_DEVICE_COUNTS];
        private IDInput[] dILaserPorts = new IDInput[DEFAULT_MAX_DEVICE_COUNTS];
        private IDOutput[] dOExt1s = new IDOutput[DEFAULT_MAX_DEVICE_COUNTS];
        private IDOutput[] dOExt2s = new IDOutput[DEFAULT_MAX_DEVICE_COUNTS];
        private IDOutput[] dOLaserPorts = new IDOutput[DEFAULT_MAX_DEVICE_COUNTS];

        private readonly SpiralLab.Sirius3.UI.WinForms.EditorControl editorControl1 = new SpiralLab.Sirius3.UI.WinForms.EditorControl();
        private readonly System.Windows.Forms.Timer timerStatus = new System.Windows.Forms.Timer();
        private int timerStatusColorCounts;
        const int DEFAULT_MAX_DEVICE_COUNTS = 4;
        #endregion

        #region Public Bindable Properties
        /// <summary>
        /// Maximum number of supported device sets.
        /// <para>지원되는 최대 장치 세트 수입니다.</para>
        /// <para>支持的最大设备集数。</para>
        /// </summary>
        [Category("Sirius3")]
        [DisplayName("Max. Device")]
        [Description("Max. Device Counts")]
        public int MaxDeviceCounts
        {
            get { return maxDeviceCounts; }
            set
            {
                if (value < 1 || value > DEFAULT_MAX_DEVICE_COUNTS) // allowed 1 ~ 4 only
                    return;

                maxDeviceCounts = value;

                // Update UI
                btnDevice0.Enabled = maxDeviceCounts >= 1;
                btnDevice1.Enabled = maxDeviceCounts >= 2;
                btnDevice2.Enabled = maxDeviceCounts >= 3;
                btnDevice3.Enabled = maxDeviceCounts >= 4;

                if (CurrentDeviceIndex >= maxDeviceCounts)
                    SwitchDevices(0); //reset to 0
            }
        }
        int maxDeviceCounts = 4;

        /// <summary>
        /// Gets or sets the current device index.
        /// <para>현재 장치 인덱스를 가져오거나 설정합니다.</para>
        /// <para>获取或设置当前设备索引。</para>
        /// </summary>
        public int CurrentDeviceIndex { get; protected set; }

        /// <summary>
        /// Gets or sets the editor name.
        /// <para>현재 편집기의 이름을 가져오거나 설정합니다.</para>
        /// <para>获取或设置编辑器名称。</para>
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
                    document.OnNew -= Document_OnNew;
                    document.OnBeforeOpen -= Document_OnBeforeOpen;
                    document.OnAfterOpen -= Document_OnAfterOpen;
                    document.OnBeforeSave -= Document_OnBeforeSave;
                    document.OnAfterSave -= Document_OnAfterSave;
                }

                document = value;

                MarkerCtrl.Document = document;
                PropertyGridCtrl.Document = document;
                EditorCtrl.Document = document;
                EntityPenCtrl.Document = document;
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
                    document.OnNew += Document_OnNew;
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
            get => scanners[CurrentDeviceIndex];
            private set
            {

                if (scanners[CurrentDeviceIndex] is IRtcMoF oldMof)
                    oldMof.OnEncoderChanged -= MoF_OnEncoderChanged;

                scanners[CurrentDeviceIndex] = value;

                if (lasers[CurrentDeviceIndex] != null)
                    lasers[CurrentDeviceIndex].Scanner = scanners[CurrentDeviceIndex];

                ScannerCtrl.Scanner = scanners[CurrentDeviceIndex];
                var rtc = value as IRtc;
                MarkerCtrl.Rtc = rtc;
                ManualCtrl.Rtc = rtc;
                EditorCtrl.Rtc = rtc;
                PowerMapCtrl.Rtc = rtc;

                if (scanners[CurrentDeviceIndex] != null)
                {
                    PropertyVisibility();
                    MenuVisibility();

                    if (rtc.IsMoF)
                    {
                        if (scanners[CurrentDeviceIndex] is IRtcMoF newMof)
                        {
                            newMof.OnEncoderChanged += MoF_OnEncoderChanged;
                        }
                        lblEncoder.Visible = true;
                    }
                    else
                    {
                        lblEncoder.Visible = false;
                    }
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
            get => lasers[CurrentDeviceIndex];
            private set
            {
                if (lasers[CurrentDeviceIndex] != null)
                {
                    lasers[CurrentDeviceIndex].Scanner = scanners[CurrentDeviceIndex];
                    UpdatePowerMap();
                }

                LaserCtrl.Laser = lasers[CurrentDeviceIndex];
                EditorCtrl.Laser = lasers[CurrentDeviceIndex];
                MarkerCtrl.Laser = lasers[CurrentDeviceIndex];
                ManualCtrl.Laser = lasers[CurrentDeviceIndex];
                PowerMeterCtrl.Laser = lasers[CurrentDeviceIndex];
                PowerMapCtrl.Laser = lasers[CurrentDeviceIndex];
                EntityPenCtrl.Document = document;
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
            get => markers[CurrentDeviceIndex];
            private set
            {

                if (markers[CurrentDeviceIndex] != null)
                {

                }

                markers[CurrentDeviceIndex] = value;

                MarkerCtrl.Marker = markers[CurrentDeviceIndex];
                ManualCtrl.Marker = markers[CurrentDeviceIndex];
                RtcDOCtrl.Marker = markers[CurrentDeviceIndex];
                OffsetCtrl.Marker = markers[CurrentDeviceIndex];
                EditorCtrl.Marker = markers[CurrentDeviceIndex];
                PropertyGridCtrl.Marker = markers[CurrentDeviceIndex];

                if (markers[CurrentDeviceIndex] != null)
                {
                    markers[CurrentDeviceIndex].OnStarted -= Marker_OnStarted;
                    markers[CurrentDeviceIndex].OnStarted += Marker_OnStarted;

                    markers[CurrentDeviceIndex].OnEnded -= Marker_OnEnded;
                    markers[CurrentDeviceIndex].OnEnded += Marker_OnEnded;
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
            get => powerMeters[CurrentDeviceIndex];
            private set
            {
                if (powerMeters[CurrentDeviceIndex] != null)
                {

                }

                powerMeters[CurrentDeviceIndex] = value;

                PowerMeterCtrl.PowerMeter = powerMeters[CurrentDeviceIndex];
                PowerMapCtrl.PowerMeter = powerMeters[CurrentDeviceIndex];
                MarkerCtrl.PowerMeter = powerMeters[CurrentDeviceIndex];

                if (powerMeters[CurrentDeviceIndex] != null)
                {

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
            get => dIExt1s[CurrentDeviceIndex];
            private set
            {
                dIExt1s[CurrentDeviceIndex] = value;
                RtcDICtrl.DIExt1 = dIExt1s[CurrentDeviceIndex];
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
            get => dILaserPorts[CurrentDeviceIndex];
            private set
            {
                dILaserPorts[CurrentDeviceIndex] = value;
                RtcDICtrl.DILaserPort = dILaserPorts[CurrentDeviceIndex];
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
            get => dOExt1s[CurrentDeviceIndex];
            private set
            {
                dOExt1s[CurrentDeviceIndex] = value;
                RtcDOCtrl.DOExt1 = dOExt1s[CurrentDeviceIndex];
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
            get => dOExt2s[CurrentDeviceIndex];
            private set
            {
                dOExt2s[CurrentDeviceIndex] = value;
                RtcDOCtrl.DOExt2 = dOExt2s[CurrentDeviceIndex];
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
            get => dOLaserPorts[CurrentDeviceIndex];
            private set
            {
                dOLaserPorts[CurrentDeviceIndex] = value;
                RtcDOCtrl.DOLaserPort = dOLaserPorts[CurrentDeviceIndex];
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
        public SpiralLab.Sirius3.UI.WinForms.TreeViewPageControl[] PageCtrls
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
        public SpiralLab.Sirius3.UI.WinForms.TreeViewBlockControl BlockCtrl => treeViewBlockControl1;
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
        public SpiralLab.Sirius3.UI.WinForms.TreeViewWaferControl WaferCtrl => treeViewWaferControl1;

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
        public SpiralLab.Sirius3.UI.WinForms.TreeViewSubstrateControl SubstrateCtrl => treeViewSubstrateControl1;

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
        /// <para>파워 맵 컨트롤 래퍼를 가져옵니다.</para>
        /// <para>获取功率映射控件包装器。</para>
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius3")]
        [DisplayName("PowerMapControl")]
        [Description("PowerMap UserControl")]
        public SpiralLab.Sirius3.UI.WinForms.PowerMapControl PowerMapCtrl => powerMapControl1;

        /// <summary>
        /// Gets the entity pen control wrapper.
        /// <para>엔티티 펜 컨트롤 래퍼를 가져옵니다.</para>
        /// <para>获取实体笔控件包装器。</para>
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius3")]
        [DisplayName("Entity PenUserControl")]
        [Description("Entity Pen UserControl")]
        public SpiralLab.Sirius3.UI.WinForms.EntityPenControl EntityPenCtrl => entityPenControl1;

        /// <summary>
        /// Gets the layer pen control wrapper.
        /// <para>레이어 펜 컨트롤 래퍼를 가져옵니다.</para>
        /// <para>获取图层笔控件包装器。</para>
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius3")]
        [DisplayName("Layer PenUserControl")]
        [Description("Layer Pen UserControl")]
        public SpiralLab.Sirius3.UI.WinForms.LayerPenControl LayerPenCtrl => layerPenControl1;

        /// <summary>
        /// Gets the log control.
        /// <para>로그 컨트롤을 가져옵니다.</para>
        /// <para>获取日志控件。</para>
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
        /// Initializes a new instance of <see cref="SiriusMultiEditorControl"/> and wires UI events. <br/>
        /// <see cref="SiriusMultiEditorControl"/>의 새 인스턴스를 초기화하고 UI 이벤트를 연결합니다. <br/>
        /// 初始化 <see cref="SiriusMultiEditorControl"/> 的新实例并连接 UI 事件。
        /// </summary>
        public SiriusMultiEditorControl()
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

            timerStatus.Interval = 100;
            timerStatus.Tick += TimerStatus_Tick;

            lblEncoder.DoubleClick += LblEncoder_DoubleClick;
            lblEncoder.DoubleClickEnabled = true;

            tbcLeft.SelectedIndexChanged += tbcLeft_SelectedIndexChanged;
            btnNew.Click += BtnNew_Click;
            btnOpen.Click += BtnOpen_Click;
            btnSave.Click += BtnSave_Click;
            btnLock.Click += BtnLock_Click;

            btnDevice0.Click += BtnDevice_Click;
            btnDevice1.Click += BtnDevice_Click;
            btnDevice2.Click += BtnDevice_Click;
            btnDevice3.Click += BtnDevice_Click;

            // Hide log window by default
            splitContainer2.Panel2Collapsed = true;
            splitContainer2.Panel2Collapsed = false;
            splitContainer2.Panel2Collapsed = true;
            btnLogWindow.Click += (_, __) =>
            {
                splitContainer2.Panel2Collapsed = !splitContainer2.Panel2Collapsed;
            };

            Document = new DocumentBase();
        }

        /// <summary>
        /// Registers devices for the specified device index.
        /// <para>지정된 장치 인덱스에 대한 장치를 등록합니다.</para>
        /// <para>为指定的设备索引注册设备。</para>
        /// </summary>
        /// <param name="index">The device index.</param>
        /// <param name="scanner">The scanner instance.</param>
        /// <param name="laser">The laser instance.</param>
        /// <param name="powerMeter">The power meter instance.</param>
        /// <param name="dIExt1">The digital input extension 1.</param>
        /// <param name="dILaserPort">The digital input laser port.</param>
        /// <param name="dOExt1">The digital output extension 1.</param>
        /// <param name="dOExt2">The digital output extension 2.</param>
        /// <param name="dOLaserPort">The digital output laser port.</param>
        /// <param name="marker">The marker instance.</param>
        public void RegisterDevices(int index, IScanner scanner, ILaser laser, IPowerMeter powerMeter, IDInput dIExt1, IDInput dILaserPort, IDOutput dOExt1, IDOutput dOExt2, IDOutput dOLaserPort, IMarker marker)
        {
            if (MaxDeviceCounts <= index)
                throw new ArgumentOutOfRangeException(nameof(index), $"CurrentDeviceIndex must be less than {MaxDeviceCounts}.");

            scanners[index] = scanner;
            lasers[index] = laser;
            powerMeters[index] = powerMeter;
            dIExt1s[index] = dIExt1;
            dILaserPorts[index] = dILaserPort;
            dOExt1s[index] = dOExt1;
            dOExt2s[index] = dOExt2;
            dOLaserPorts[index] = dOLaserPort;
            markers[index] = marker;
        }

        /// <summary>
        /// Dispose all registered devices.
        /// <para>장치를 모두 해지하고 자원을 회수합니다.</para>
        /// </summary>
        public void DisposeDevices()
        {
            //this.Marker?.Stop();
            //this.Marker = null;
            //this.PowerMeter = null;
            //this.DIExt1 = null;
            //this.DILaserPort = null;
            //this.DOExt1 = null;
            //this.DOExt2 = null;
            //this.DOLaserPort = null;
            //this.Laser = null;
            //this.Scanner = null;

            for (int i = 0; i < MaxDeviceCounts; i++)
            {
                markers[i]?.Dispose();
                powerMeters[i]?.Dispose();
                dIExt1s[i]?.Dispose();
                dILaserPorts[i]?.Dispose();
                dOExt1s[i]?.Dispose();
                dOExt2s[i]?.Dispose();
                dOLaserPorts[i]?.Dispose();
                lasers[i]?.Dispose();
                scanners[i]?.Dispose();
            }
        }

        /// <summary>
        /// Sets the current device index.
        /// <para>현재 장치 인덱스를 설정합니다.</para>
        /// </summary>
        /// <param name="index">Target device index. <br/>Allowed range: 0 ~ MaxDeviceCounts - 1 </param>
        public bool SwitchDevices(int index)
        {
            if (MaxDeviceCounts <= CurrentDeviceIndex)
                throw new ArgumentOutOfRangeException(nameof(index), $"CurrentDeviceIndex must be less than {MaxDeviceCounts}.");

            if (null == scanners[index] || null == lasers[index] || null == markers[index])
            {
                Logger.Log(LogLevel.Error, $"Some device is not registered yet at {index} index.");
                //throw new ArgumentOutOfRangeException(nameof(value), $"Some device is not assigned. null ?");
            }

            OnBeforeChangeDevice?.Invoke(this);

            CurrentDeviceIndex = index;
            var buttons = new ToolStripButton[] { btnDevice0, btnDevice1, btnDevice2, btnDevice3 };
            for (int i = 0; i < buttons.Length; i++)
            {
                if (null == buttons[i]) continue;
                if (i == CurrentDeviceIndex)
                {
                    buttons[i].Checked = true;
                    buttons[i].Text = $"Device {i + 1}";
                    buttons[i].DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                    //buttons[i].BackColor = Color.Orange;
                    //buttons[i].ForeColor = Color.Black;
                }
                else
                {
                    buttons[i].Checked = false;
                    buttons[i].Text = ""; // $"{i + 1}";
                    buttons[i].DisplayStyle = ToolStripItemDisplayStyle.Image;
                    //buttons[i].BackColor = Color.Empty;
                    //buttons[i].ForeColor = Color.Empty;
                }
            }

            this.Scanner = scanners[CurrentDeviceIndex];
            this.Laser = lasers[CurrentDeviceIndex];
            this.PowerMeter = powerMeters[CurrentDeviceIndex];
            this.Marker = markers[CurrentDeviceIndex];

            this.DIExt1 = dIExt1s[CurrentDeviceIndex];
            this.DILaserPort = dILaserPorts[CurrentDeviceIndex];
            this.DOExt1 = dOExt1s[CurrentDeviceIndex];
            this.DOExt2 = dOExt2s[CurrentDeviceIndex];
            this.DOLaserPort = dOLaserPorts[CurrentDeviceIndex];

            this.Marker?.Ready(Document, View, Scanner as IRtc, Laser, PowerMeter);

            OnAfterChangeDevice?.Invoke(this);
            return true;
        }

        /// <summary>
        /// Handles the device button click event.
        /// <para>장치 버튼 클릭 이벤트를 처리합니다.</para>
        /// <para>处理设备按钮单击事件。</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        private void BtnDevice_Click(object sender, EventArgs e)
        {
            var btn = sender as ToolStripButton;
            if (int.TryParse((string)btn.Tag, out int index))
            {
                SwitchDevices(index);
            }
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
            SwitchDevices(CurrentDeviceIndex);
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
            timerStatus.Tick -= TimerStatus_Tick;
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
        /// Called when a new document is created.
        /// <para>새 문서가 생성될 때 호출됩니다.</para>
        /// <para>创建新文档时调用。</para>
        /// </summary>
        /// <param name="obj">The document instance.</param>
        private void Document_OnNew(IDocument obj)
        {
            if (!IsHandleCreated || IsDisposed) return;

            Invoke(new MethodInvoker(() =>
            {
                UpdatePowerMap();
            }));
        }
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
                EntityPenCtrl.Document = document;

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
                rtcMoF.CtlMoFEncoderReset();
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

            switch (_marker.Index)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    break;
            }

            Invoke(new MethodInvoker(() =>
            {
                ControlEnableOrNot(false);
            }));
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

            switch (_marker.Index)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    break;
            }

            bool isBusy = false;
            foreach (var marker in markers)
                if (null != marker)
                    isBusy |= marker.IsBusy;

            Invoke(new MethodInvoker(() =>
            {
                ControlEnableOrNot(!isBusy && !btnLock.Checked);
                if (!isBusy)
                    EditorCtrl.Focus();
            }));
        }

        /// <summary>
        /// Called when MoF encoders change; updates encoder label text depending on MoF mode.
        /// <para>MoF 인코더가 변경될 때 호출됩니다. MoF 모드에 따라 인코더 레이블 텍스트를 업데이트합니다.</para>
        /// <para>MoF 编码器更改时调用；根据 MoF 模式更新编码器标签文本。</para>
        /// </summary>
        /// <param name="rtcMoF">The IRtcMoF instance.</param>
        /// <param name="encX">The X(or rotate) axis encoder count value.</param>
        /// <param name="encY">The Y axis encoder count value.</param>
        /// <param name="encXmmOrAngle">The X(or rptate) axis encoder converted to mm(or °)value.</param>
        /// <param name="encYmm">The Y axis encoder converted to mm value.</param>
        private void MoF_OnEncoderChanged(IRtcMoF rtcMoF, int encX, int encY, double encXmmOrAngle, double encYmm)
        {
            if (!stsBottom.IsHandleCreated || IsDisposed) return;

            try
            {
                switch (rtcMoF.MoFMode)
                {
                    default:
                    case RtcMoFModes.XY:
                        stsBottom.Invoke(new MethodInvoker(() =>
                        {
                            lblEncoder.Text = string.Format("ENC: {0:F3}, {1:F3}mm ({2}, {3})", encXmmOrAngle, encYmm, encX, encY);
                        }));
                        break;

                    case RtcMoFModes.Angular:
                        stsBottom.Invoke(new MethodInvoker(() =>
                        {
                            lblEncoder.Text = string.Format("ENC: {0:F3}° ({1})", encXmmOrAngle, encX);
                        }));
                        break;
                }
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
            EntityPen.PropertyVisibility(Scanner);
            EntityLayerPen.PropertyVisibility(Scanner);
        }

        /// <summary>
        /// Enables or disables editing-related controls when marker is busy.
        /// <para>마커가 사용 중일 때 편집 관련 컨트롤을 활성화하거나 비활성화합니다.</para>
        /// <para>当标记器忙碌时，启用或禁用与编辑相关的控件。</para>
        /// </summary>
        /// <param name="isEnable">True to enable; false to disable. <para>활성화하려면 true, 비활성화하려면 false입니다.</para> <para>如果要启用则为 true；如果要禁用则为 false。</para></param>
        public virtual void ControlEnableOrNot(bool isEnable)
        {
            if (!IsHandleCreated || IsDisposed) return;

            Invoke(new MethodInvoker(() =>
            {
                btnNew.Enabled = isEnable;
                //btnOpen.Enabled = isEnable;
                ddbOpenNewOptions.Enabled = isEnable;
                btnSave.Enabled = isEnable;

                tbcLeft.Enabled = isEnable;
                //splitContainer12.Panel1Collapsed = !isEnable;
                //splitContainer123.Panel2Collapsed = !isEnable;
                PropertyGridCtrl.Enabled = isEnable;

                EditorCtrl.IsAllowEdit = isEnable;
                foreach (var pc in PageCtrls)
                    pc.Enabled = isEnable;

                BlockCtrl.Enabled = isEnable;
                WaferCtrl.Enabled = isEnable;
                SubstrateCtrl.Enabled = isEnable;


#if DEBUG
                // Keep enables for debugging

#else
                ManualCtrl.Enabled = isEnable;
                OffsetCtrl.Enabled = isEnable;
                ScannerCtrl.Enabled = isEnable;
                LaserCtrl.Enabled = isEnable;
                PowerMeterCtrl.Enabled = isEnable;
                PowerMapCtrl.Enabled = isEnable;
                RtcDOCtrl.Enabled = isEnable;
                EntityPenCtrl.Enabled = isEnable;
                LayerPenCtrl.Enabled = isEnable;
                //MarkerCtrl.Enabled = isEnable;
#endif

            }));
        }

        /// <summary>
        /// Update laser and powermap information at entity pens.
        /// <para>엔티티 펜의 레이저 및 파워 맵 정보를 업데이트합니다.</para>
        /// <para>更新实体笔处的激光和功率映射信息。</para>
        /// </summary>
        private void UpdatePowerMap()
        {
            var powerControl = lasers[CurrentDeviceIndex] as ILaserPowerControl;

            if (null != document && null != powerControl)
            {
                foreach (var child in document.DocumentData.EntityPens.Children)
                {
                    var pen = child as EntityPen;
                    pen.PowerMax = lasers[CurrentDeviceIndex].MaxPowerWatt;
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
            bool includeEntityPens = mnuIncludeEntityPens.Checked;
            bool includeLayerPens = mnuIncludeLayerPens.Checked;
            bool includeWafers = mnuIncludeWafers.Checked;
            bool includeSubstrates = mnuIncludeSubstrates.Checked;

            Document?.ActNew(
                includePage1,
                includePage2,
                includePage3,
                includePage4,
                includeBlocks,
                includeEntityPens,
                includeLayerPens,
                includeWafers,
                includeSubstrates);

            OnAfterNew?.Invoke(this);
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
            bool includeEntityPens = mnuIncludeEntityPens.Checked;
            bool includeLayerPens = mnuIncludeLayerPens.Checked;
            bool includeWafers = mnuIncludeWafers.Checked;
            bool includeSubstrates = mnuIncludeSubstrates.Checked;

            Document?.ActOpen(
                dlg.FileName,
                includeLayers,
                includeLayers2nd,
                includeBlocks,
                includeEntityPens,
                includeLayerPens,
                includeWafers,
                includeSubstrates);

            OnAfterOpen?.Invoke(this, dlg.FileName);
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

            OnAfterSave?.Invoke(this, dlg.FileName);
        }

        /// <summary>
        /// Toggles allow to edit(lock) at view or not.
        /// <para>뷰에서의 편집 허용(잠금) 여부를 토글합니다.</para>
        /// <para>切换是否允许在视图中编辑（锁定）。</para>
        /// </summary>
        /// <param name="sender">The source of the event. <para>이벤트 소스입니다.</para> <para>事件源。</para></param>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data. <para>이벤트 데이터를 포함하는 <see cref="EventArgs"/>입니다.</para> <para>包含事件数据的 <see cref="EventArgs"/>。</para></param>
        private void BtnLock_Click(object sender, EventArgs e)
        {
            ControlEnableOrNot(!btnLock.Checked);
        }
        #endregion
    }
}
