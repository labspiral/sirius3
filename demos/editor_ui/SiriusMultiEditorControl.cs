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
 * Description : SiriusMultiEditorControl
 * Author : hong chan, choi / hcchoi@spirallab.co.kr (http://spirallab.co.kr)
 */

using System;
using SpiralLab.Sirius3.Localization;
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
using SpiralLab.Sirius3.Remote;
using SpiralLab.Sirius3.UI.WinForms;
using SpiralLab.Sirius3.Scanner.Rtc.SyncAxis;
using SpiralLab.Sirius3.MCP;


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
    /// </summary>
    /// <remarks>
    /// <img src="~/images/siriusmultieditorcontrol.png"/><br/>
    /// </remarks>
    [ToolboxItem(true)]
    public partial class SiriusMultiEditorControl : UserControl
    {
        #region Events
        /// <summary>
        /// Raised when after new button has pressed.
        /// <para>새 문서 버튼이 눌린 후 발생합니다.</para>
        /// </summary>
        public event Action<SiriusMultiEditorControl> OnAfterNew;
        /// <summary>
        /// Raised when after open button has pressed.
        /// <para>열기 버튼이 눌린 후 발생합니다.</para>
        /// </summary>
        public event Action<SiriusMultiEditorControl, string> OnAfterOpen;
        /// <summary>
        /// Raised when after save button has pressed.
        /// <para>저장 버튼이 눌린 후 발생합니다.</para>
        /// </summary>
        public event Action<SiriusMultiEditorControl, string> OnAfterSave;

        /// <summary>
        /// Raised before the device set is changed.
        /// <para>장치 세트가 변경되기 전에 발생합니다.</para>
        /// </summary>
        public event Action<SiriusMultiEditorControl> OnBeforeChangeDevice;
        /// <summary>
        /// Raised after the device set is changed.
        /// <para>장치 세트가 변경된 후 발생합니다.</para>
        /// </summary>
        public event Action<SiriusMultiEditorControl> OnAfterChangeDevice;
        #endregion

        #region Fields
        private IDocument document;
        private bool isEditEnabled = true;
        private IScanner[] scanners = new IScanner[DEFAULT_MAX_DEVICE_COUNTS];
        private ILaser[] lasers = new ILaser[DEFAULT_MAX_DEVICE_COUNTS];
        private IMarker[] markers = new IMarker[DEFAULT_MAX_DEVICE_COUNTS];
        private IPowerMeter[] powerMeters = new IPowerMeter[DEFAULT_MAX_DEVICE_COUNTS];

        private IDInput[] dIExt1s = new IDInput[DEFAULT_MAX_DEVICE_COUNTS];
        private IDInput[] dILaserPorts = new IDInput[DEFAULT_MAX_DEVICE_COUNTS];
        private IDOutput[] dOExt1s = new IDOutput[DEFAULT_MAX_DEVICE_COUNTS];
        private IDOutput[] dOExt2s = new IDOutput[DEFAULT_MAX_DEVICE_COUNTS];
        private IDOutput[] dOLaserPorts = new IDOutput[DEFAULT_MAX_DEVICE_COUNTS];
        private IRemote[] remotes = new IRemote[DEFAULT_MAX_DEVICE_COUNTS];

        private readonly SpiralLab.Sirius3.UI.WinForms.EditorControl editorControl1 = new SpiralLab.Sirius3.UI.WinForms.EditorControl();
        private readonly System.Windows.Forms.Timer timerStatus = new System.Windows.Forms.Timer();
        private int timerStatusColorCounts;
        const int DEFAULT_MAX_DEVICE_COUNTS = 4;
        #endregion

        #region Public Bindable Properties
        /// <summary>
        /// Maximum number of supported device sets.
        /// <para>지원되는 최대 장치 세트 수입니다.</para>
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius3")]
        [DisplayName("Max. Device")]
        [Description("Max. Device Counts")]
        public int MaxDeviceCounts
        {
            get { return maxDeviceCounts; }
            set
            {
#if DEBUG
                if (value < 1 || value > DEFAULT_MAX_DEVICE_COUNTS) // allowed 1 ~ 4 only
                    throw new ArgumentOutOfRangeException("MaxDeviceCounts must be between 1 and " + DEFAULT_MAX_DEVICE_COUNTS);
#endif
                maxDeviceCounts = value;

                // Update UI
                try
                {
                    rdDevice0.Enabled = maxDeviceCounts >= 1;
                    rdDevice1.Enabled = maxDeviceCounts >= 2;
                    rdDevice2.Enabled = maxDeviceCounts >= 3;
                    rdDevice3.Enabled = maxDeviceCounts >= 4;
                }
                catch (Exception)
                { }

                if (CurrentDeviceIndex >= maxDeviceCounts)
                    SwitchDevices(0); //reset to 0
            }
        }
        int maxDeviceCounts = 4;

        /// <summary>
        /// Gets registered scanners
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IScanner[] Scanners => scanners;
        /// <summary>
        /// Gets registered lasers
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ILaser[] Lasers => lasers;
        /// <summary>
        /// Gets registered markers
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IMarker[] Markers => markers;
        /// <summary>
        /// Gets registered power meters
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IPowerMeter[] PowerMeters => powerMeters;
        /// <summary>
        /// Gets registered DInput (Ext1)
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IDInput[] DIExt1s => dIExt1s;
        /// <summary>
        /// Gets registered DInput (Laser Port)
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IDInput[] DILaserPorts => dILaserPorts;
        /// <summary>
        /// Gets registered DOutput (Ext1)
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IDOutput[] DOExt1s => dOExt1s;
        /// <summary>
        /// Gets registered DOutput (Ext2)
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IDOutput[] DOExt2s => dOExt2s;
        /// <summary>
        /// Gets registered DOutput (Laser Port)
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IDOutput[] DOLaserPorts => dOLaserPorts;
        /// <summary>
        /// Gets registered Remotes 
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IRemote[] Remotes => remotes;

        /// <summary>
        /// Gets or sets the current device index.
        /// <para>현재 장치 인덱스를 가져오거나 설정합니다.</para>
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius3")]
        [DisplayName("Current Device")]
        [Description("Current Device Index")]
        public int CurrentDeviceIndex { get; protected set; }

        /// <summary>
        /// Gets or sets the editor name.
        /// <para>현재 편집기의 이름을 가져오거나 설정합니다.</para>
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius3")]
        [DisplayName("Alias")]
        [Description("Alias Name at Bottom")]
        public string AliasName
        {
            get { return lblAliasName.Text; }
            set { lblAliasName.Text = value; }
        }

        /// <summary>
        /// Gets or sets the current document and wires related UI/controls to it.
        /// <para>현재 문서를 가져오거나 설정하고 관련 UI/컨트롤을 연결합니다.</para>
        /// </summary>
        [Browsable(false)]
        [ReadOnly(false)]
        [Category("Sirius3")]
        [DisplayName("Document")]
        [Description("Document Instance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IDocument Document
        {
            get => document;
            set
            {
                if (document == value) return;
                if (null != Marker && Marker.IsBusy)
                {
                    SpiralLab.Sirius3.UI.WinForms.MessageBox.Show(MessageBoxLocalization.S("Document_MarkerBusy", Marker.ToString()), MessageBoxLocalization.S("Title_Error"), MessageBoxButtons.OK);
                    //throw new InvalidOperationException($"Not allowed to change document during {marker.ToString()} is busy");
                    return;
                }

                document?.ActSimulateStop(false);
                if (document != null)
                {
                    PropertyGridCtrl.SelecteObject = null;
                    document.OnNew -= Document_OnNew;
                    document.OnBeforeOpen -= Document_OnBeforeOpen;
                    document.OnAfterOpen -= Document_OnAfterOpen;
                    document.OnBeforeSave -= Document_OnBeforeSave;
                    document.OnAfterSave -= Document_OnAfterSave;
                    document.OnSimulationStarted -= Document_OnSimulationStarted;
                    document.OnSimulationEnded -= Document_OnSimulationEnded;
                }

                document = value;

                if (MarkerCtrl != null)
                    MarkerCtrl.Document = document;
                if (PropertyGridCtrl != null)
                    PropertyGridCtrl.Document = document;
                if (EditorCtrl != null)
                    EditorCtrl.Document = document;
                if (EntityPenCtrl != null)
                    EntityPenCtrl.Document = document;
                if (LayerPenCtrl != null)
                    LayerPenCtrl.Document = document;
                if (PowerMapCtrl != null)
                    PowerMapCtrl.Document = document;

                treeViewPageControl1.Page = DocumentPages.Page1;
                treeViewPageControl1.Document = document;
                treeViewPageControl2.Page = DocumentPages.Page2;
                treeViewPageControl2.Document = document;
                treeViewPageControl3.Page = DocumentPages.Page3;
                treeViewPageControl3.Document = document;
                treeViewPageControl4.Page = DocumentPages.Page4;
                treeViewPageControl4.Document = document;

                treeViewBlockControl1.Document = document;
                //treeViewWaferControl1.Document = document;
                //treeViewSubstrateControl1.Document = document;

                if (document != null)
                {
                    document.OnNew += Document_OnNew;
                    document.OnBeforeOpen += Document_OnBeforeOpen;
                    document.OnAfterOpen += Document_OnAfterOpen;
                    document.OnBeforeSave += Document_OnBeforeSave;
                    document.OnAfterSave += Document_OnAfterSave;
                    document.OnSimulationStarted += Document_OnSimulationStarted;
                    document.OnSimulationEnded += Document_OnSimulationEnded;
                    PropertyGridCtrl.SelecteObject = document.Selected;
                }
                EditorControlDispatch.Run(this, ApplyEditPermission);
            }
        }

        /// <summary>
        /// Get current view.
        /// <para>현재 뷰를 가져옵니다.</para>
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius3")]
        [DisplayName("View")]
        [Description("View Instance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IView View
        {
            get { return EditorCtrl?.View; }
        }

        /// <summary>
        /// Gets or sets the RTC(scanner) instance and wires all RTC-related controls.
        /// <para>RTC(스캐너) 인스턴스를 가져오거나 설정하고 모든 RTC 관련 컨트롤을 연결합니다.</para>
        /// </summary>
        /// <remarks>Created by <see cref="ScannerFactory"/>.</remarks>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius3")]
        [DisplayName("Scanner")]
        [Description("Scanner Instance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IScanner Scanner
        {
            get => scanners[CurrentDeviceIndex];
            private set
            {
                Document?.ActSimulateStop(false);
                if (scanners[CurrentDeviceIndex] is IRtcMoF oldMof)
                    oldMof.OnEncoderChanged -= MoF_OnEncoderChanged;

                scanners[CurrentDeviceIndex] = value;
                var scanner = scanners[CurrentDeviceIndex];

                if (lasers[CurrentDeviceIndex] != null)
                    lasers[CurrentDeviceIndex].Scanner = scanner;

                if (ScannerCtrl != null)
                    ScannerCtrl.Scanner = scanner;
                if (LaserCtrl != null)
                    LaserCtrl.Scanner = scanner;
                if (MarkerCtrl != null)
                    MarkerCtrl.Scanner = scanner;
                if (ManualCtrl != null)
                    ManualCtrl.Scanner = scanner;
                if (EditorCtrl != null)
                    EditorCtrl.Scanner = scanner;
                if (DIRtcCtrl != null)
                    DIRtcCtrl.Scanner = scanner;
                if (DORtcCtrl != null)
                    DORtcCtrl.Scanner = scanner;
                if (PowerMapCtrl != null)
                    PowerMapCtrl.Scanner = scanner;
                if (StepperCtrl != null)
                    StepperCtrl.Stepper = scanner as IRtcStepper;

                if (scanner != null)
                {
                    PropertyVisibility();
                    MenuVisibility();
                    PageVisibility();
                    var rtc = value as IRtc;
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
        /// </summary>
        /// <remarks>Created by <see cref="LaserFactory"/>.</remarks>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius3")]
        [DisplayName("Laser")]
        [Description("Laser Instance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ILaser Laser
        {
            get => lasers[CurrentDeviceIndex];
            private set
            {
                Document?.ActSimulateStop(false);
                if (lasers[CurrentDeviceIndex] != null)
                {
                    lasers[CurrentDeviceIndex].Scanner = scanners[CurrentDeviceIndex];
                    UpdatePens();
                    UpdateLaser();
                    PropertyVisibility();
                }

                if (LaserCtrl != null)
                    LaserCtrl.Laser = lasers[CurrentDeviceIndex];
                if (EditorCtrl != null)
                    EditorCtrl.Laser = lasers[CurrentDeviceIndex];
                if (MarkerCtrl != null)
                    MarkerCtrl.Laser = lasers[CurrentDeviceIndex];
                if (ManualCtrl != null)
                    ManualCtrl.Laser = lasers[CurrentDeviceIndex];
                if (PowerMeterCtrl != null)
                    PowerMeterCtrl.Laser = lasers[CurrentDeviceIndex];
                if (PowerMapCtrl != null)
                    PowerMapCtrl.Laser = lasers[CurrentDeviceIndex];
                if (EntityPenCtrl != null)
                    EntityPenCtrl.Document = document;
            }
        }

        /// <summary>
        /// Gets or sets the marker and wires all marker-dependent controls and events.
        /// <para>마커를 가져오거나 설정하고 모든 마커 종속 컨트롤 및 이벤트를 연결합니다.</para>
        /// </summary>
        /// <remarks>Created by <see cref="MarkerFactory"/>.</remarks>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius3")]
        [DisplayName("Marker")]
        [Description("Marker Instance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IMarker Marker
        {
            get => markers[CurrentDeviceIndex];
            private set
            {
                Document?.ActSimulateStop(false);
                if (markers[CurrentDeviceIndex] != null)
                {

                }

                markers[CurrentDeviceIndex] = value;

                if (MarkerCtrl != null)
                    MarkerCtrl.Marker = markers[CurrentDeviceIndex];
                if (ManualCtrl != null)
                    ManualCtrl.Marker = markers[CurrentDeviceIndex];
                if (DORtcCtrl != null)
                    DORtcCtrl.Marker = markers[CurrentDeviceIndex];
                if (EditorCtrl != null)
                    EditorCtrl.Marker = markers[CurrentDeviceIndex];
                if (PropertyGridCtrl != null)
                    PropertyGridCtrl.Marker = markers[CurrentDeviceIndex];
                if (RemoteCtrl != null)
                    RemoteCtrl.Marker = markers[CurrentDeviceIndex];
            }
        }

        /// <summary>
        /// Gets or sets the power meter and wires related control/event hooks.
        /// <para>파워 미터를 가져오거나 설정하고 관련 컨트롤/이벤트 후크를 연결합니다.</para>
        /// </summary>
        /// <remarks>Created by <see cref="PowerMeterFactory"/>.</remarks>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius3")]
        [DisplayName("PowerMeter")]
        [Description("PowerMeter Instance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IPowerMeter PowerMeter
        {
            get => powerMeters[CurrentDeviceIndex];
            private set
            {
                if (powerMeters[CurrentDeviceIndex] != null)
                {

                }

                powerMeters[CurrentDeviceIndex] = value;
                UpdateLaser();

                if (PowerMeterCtrl != null)
                    PowerMeterCtrl.PowerMeter = powerMeters[CurrentDeviceIndex];
                if (PowerMapCtrl != null)
                    PowerMapCtrl.PowerMeter = powerMeters[CurrentDeviceIndex];
                if (MarkerCtrl != null)
                    MarkerCtrl.PowerMeter = powerMeters[CurrentDeviceIndex];

                if (powerMeters[CurrentDeviceIndex] != null)
                {

                }
            }
        }

        /// <summary>
        /// Gets or sets RTC DI (Extension1) input port binding.
        /// <para>RTC DI (Extension1) 입력 포트 바인딩을 가져오거나 설정합니다.</para>
        /// </summary>
        /// <remarks>Created by <see cref="IOFactory"/>.</remarks>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius3")]
        [DisplayName("DInput")]
        [Description("IDInput Instance (Extension1 Port)")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IDInput DIExt1
        {
            get => dIExt1s[CurrentDeviceIndex];
            private set
            {
                dIExt1s[CurrentDeviceIndex] = value;
                if (DIRtcCtrl != null)
                    DIRtcCtrl.DIExt1 = dIExt1s[CurrentDeviceIndex];
            }
        }

        /// <summary>
        /// Gets or sets RTC DI (Laser) input port binding (2-bit).
        /// <para>RTC DI (레이저) 입력 포트 바인딩(2비트)을 가져오거나 설정합니다.</para>
        /// </summary>
        /// <remarks>Created by <see cref="IOFactory"/>.</remarks>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius3")]
        [DisplayName("DInput")]
        [Description("IDInput Instance (LASER Port)")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IDInput DILaserPort
        {
            get => dILaserPorts[CurrentDeviceIndex];
            private set
            {
                dILaserPorts[CurrentDeviceIndex] = value;
                if (DIRtcCtrl != null)
                    DIRtcCtrl.DILaserPort = dILaserPorts[CurrentDeviceIndex];
            }
        }

        /// <summary>
        /// Gets or sets RTC DO (Extension1) output port binding (16-bit).
        /// <para>RTC DO (Extension1) 출력 포트 바인딩(16비트)을 가져오거나 설정합니다.</para>
        /// </summary>
        /// <remarks>Created by <see cref="IOFactory"/>.</remarks>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius3")]
        [DisplayName("DOutput")]
        [Description("IDOutput Instance (EXTENSION1 Port)")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IDOutput DOExt1
        {
            get => dOExt1s[CurrentDeviceIndex];
            private set
            {
                dOExt1s[CurrentDeviceIndex] = value;
                if (DORtcCtrl != null)
                    DORtcCtrl.DOExt1 = dOExt1s[CurrentDeviceIndex];
            }
        }

        /// <summary>
        /// Gets or sets RTC DO (Extension2) output port binding (8-bit).
        /// <para>RTC DO (Extension2) 출력 포트 바인딩(8비트)을 가져오거나 설정합니다.</para>
        /// </summary>
        /// <remarks>Created by <see cref="IOFactory"/>.</remarks>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius3")]
        [DisplayName("DOutput")]
        [Description("IDOutput Instance (EXTENSION2 Port)")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IDOutput DOExt2
        {
            get => dOExt2s[CurrentDeviceIndex];
            private set
            {
                dOExt2s[CurrentDeviceIndex] = value;
                if (DORtcCtrl != null)
                    DORtcCtrl.DOExt2 = dOExt2s[CurrentDeviceIndex];
            }
        }

        /// <summary>
        /// Gets or sets RTC DO (Laser) output port binding (2-bit).
        /// <para>RTC DO (레이저) 출력 포트 바인딩(2비트)을 가져오거나 설정합니다.</para>
        /// </summary>
        /// <remarks>Created by <see cref="IOFactory"/>.</remarks>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius3")]
        [DisplayName("DOutput")]
        [Description("IDOutput Instance (LASER Port)")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IDOutput DOLaserPort
        {
            get => dOLaserPorts[CurrentDeviceIndex];
            private set
            {
                dOLaserPorts[CurrentDeviceIndex] = value;
                if (DORtcCtrl != null)
                    DORtcCtrl.DOLaserPort = dOLaserPorts[CurrentDeviceIndex];
            }
        }

        /// <summary>
        /// <see cref="IRemote">IRemote</see>
        /// </summary>
        /// <remarks>
        /// Created by <see cref="RemoteFactory">RemoteFactory</see>. <br/>
        /// </remarks>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius3")]
        [DisplayName("Remote")]
        [Description("Remote Instance")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IRemote Remote
        {
            get => remotes[CurrentDeviceIndex];
            private set
            {
                remotes[CurrentDeviceIndex] = value;
                if (RemoteCtrl != null)
                    RemoteCtrl.Remote = remotes[CurrentDeviceIndex];
                if (RemoteCtrl != null)
                    RemoteCtrl.Marker = markers[CurrentDeviceIndex];

                UpdateRemoteTabVisibility();
            }
        }

        /// <summary>
        /// Gets or sets the MCP server created for this multi-editor. The assignment enables UI control and transfers disposal responsibility to <see cref="DisposeDevices"/>.
        /// <para>이 다중 편집기를 대상으로 생성된 MCP 서버를 가져오거나 설정합니다. 지정하면 UI 제어를 사용하며 <see cref="DisposeDevices"/>가 서버 해제를 담당합니다.</para>
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [LocalizedCategory("MCP")]
        [LocalizedDisplayName("MCPServer")]
        [LocalizedDescription("MCPServer")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IMCPServer MCPServer
        {
            get => mcpServer;
            set
            {
                if (ReferenceEquals(mcpServer, value)) return;
                mcpServer = value;
                if (RemoteCtrl != null) RemoteCtrl.MCPServer = value;
                UpdateRemoteTabVisibility();
            }
        }
        private IMCPServer mcpServer;

        private void UpdateRemoteTabVisibility()
        {
            var visible = remotes[CurrentDeviceIndex] != null;
            visible |= mcpServer != null;
            if (visible)
            {
                if (!tbcMain.TabPages.Contains(tabRemote)) tbcMain.TabPages.Add(tabRemote);
            }
            else if (tbcMain.TabPages.Contains(tabRemote))
            {
                tbcMain.TabPages.Remove(tabRemote);
            }
        }

        /// <summary>
        /// Show(or hide) <see cref="LogCtrl"/> window at bottom side
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius3")]
        [DisplayName("Show Log")]
        [Description("Show(or Hide) Log Window")]
        public bool IsShowLogWindow
        {
            get { return isShowLogWindow; }
            set { ShowLogWindow(value); }
        }
        bool isShowLogWindow = true;

        /// <summary>
        /// Show(or hide) <see cref="EntityPenControl"/>, <see cref="LayerPenControl"/> and <see cref="TreeView"/> windows at left side
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius3")]
        [DisplayName("Show Left")]
        [Description("Show(or Hide) TreeView and Pen")]
        public bool IsShowTreeViewAndPen
        {
            get { return isShowTreeViewAndPen; }
            set { ShowTreeViewAndPens(value); }
        }
        bool isShowTreeViewAndPen = true;

        /// <summary>
        /// Show(or hide) <see cref="EntityPenControl"/>, <see cref="LayerPenControl"/> windows at bottom left side
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius3")]
        [DisplayName("Show Pen")]
        [Description("Show(or Hide) TreeView and Pen")]
        public bool IsShowPen
        {
            get { return isShowPen; }
            set { ShowPens(value); }
        }
        bool isShowPen = true;

        /// <summary>
        /// Show(or hide) <see cref="PropertyGridCtrl"/> window at right side
        /// </summary>
        [Browsable(true)]
        [ReadOnly(false)]
        [Category("Sirius3")]
        [DisplayName("Show Right")]
        [Description("Show(or Hide) PropertyGrid Window")]
        public bool IsPropertyGridWindow
        {
            get { return isPropertyGridWindow; }
            set { ShowPropertyWindow(value); }
        }
        bool isPropertyGridWindow = true;

        /// <summary>
        /// Get <see cref="TreeViewPageControl"/> for <see cref="IDocumentData.Pages"/>
        /// <para><see cref="IDocumentData.Pages"/>에 대한 <see cref="TreeViewPageControl"/>을 가져옵니다.</para>
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SpiralLab.Sirius3.UI.WinForms.TreeViewPageControl[] PageCtrls
        {
            get
            {
                return new SpiralLab.Sirius3.UI.WinForms.TreeViewPageControl[]
                    {
                        treeViewPageControl1,
                        treeViewPageControl2,
                        treeViewPageControl3,
                        treeViewPageControl4,
                    };
            }
        }

        /// <summary>
        /// Get <see cref="TreeViewBlockControl"/> for <see cref="IDocumentData.Blocks"/>
        /// <para><see cref="IDocumentData.Blocks"/>에 대한 <see cref="TreeViewBlockControl"/>을 가져옵니다.</para>
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SpiralLab.Sirius3.UI.WinForms.TreeViewBlockControl BlockCtrl => treeViewBlockControl1;

        ///// <summary>
        ///// Get <see cref="TreeViewWaferControl"/> for <see cref="IDocumentData.Wafers"/>
        ///// <para><see cref="IDocumentData.Wafers"/>에 대한 <see cref="TreeViewWaferControl"/>을 가져옵니다.</para>
        ///// </summary>
        //[Browsable(true)]
        //[ReadOnly(false)]
        //[Category("Sirius3")]
        //[DisplayName("SubstrateControl")]
        //[Description("TreeViewWaferControl UserControl")]
        //public SpiralLab.Sirius3.UI.WinForms.TreeViewWaferControl WaferCtrl => treeViewWaferControl1;

        ///// <summary>
        ///// Get <see cref="TreeViewSubstrateControl"/> for <see cref="IDocumentData.Substrates"/>
        ///// <para><see cref="IDocumentData.Substrates"/>에 대한 <see cref="TreeViewSubstrateControl"/>을 가져옵니다.</para>
        ///// </summary>
        //[Browsable(true)]
        //[ReadOnly(false)]
        //[Category("Sirius3")]
        //[DisplayName("SubstrateControl")]
        //[Description("TreeViewSubstrateControl UserControl")]
        //public SpiralLab.Sirius3.UI.WinForms.TreeViewSubstrateControl SubstrateCtrl => treeViewSubstrateControl1;

        /// <summary>
        /// Gets the property grid control wrapper.
        /// <para>속성 그리드 컨트롤 래퍼를 가져옵니다.</para>
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SpiralLab.Sirius3.UI.WinForms.PropertyGridControl PropertyGridCtrl => propertyGridControl1;

        /// <summary>
        /// Gets the editor (OpenGL) control wrapper.
        /// <para>편집기(OpenGL) 컨트롤 래퍼를 가져옵니다.</para>
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SpiralLab.Sirius3.UI.WinForms.EditorControl EditorCtrl => editorControl1;

        /// <summary>
        /// Gets the laser control wrapper.
        /// <para>레이저 컨트롤 래퍼를 가져옵니다.</para>
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SpiralLab.Sirius3.UI.WinForms.LaserControl LaserCtrl => laserControl1;

        /// <summary>
        /// Gets the RTC control wrapper.
        /// <para>RTC 컨트롤 래퍼를 가져옵니다.</para>
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SpiralLab.Sirius3.UI.WinForms.ScannerControl ScannerCtrl => scannerControl1;

        /// <summary>
        /// Gets the marker control wrapper.
        /// <para>마커 컨트롤 래퍼를 가져옵니다.</para>
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SpiralLab.Sirius3.UI.WinForms.MarkerControl MarkerCtrl => markerControl1;

        /// <summary>
        /// Gets the RTC DI control wrapper.
        /// <para>RTC DI 컨트롤 래퍼를 가져옵니다.</para>
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SpiralLab.Sirius3.UI.WinForms.DIRtcControl DIRtcCtrl => rtcDIControl1;

        /// <summary>
        /// Gets the RTC DO control wrapper.
        /// <para>RTC DO 컨트롤 래퍼를 가져옵니다.</para>
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SpiralLab.Sirius3.UI.WinForms.DORtcControl DORtcCtrl => rtcDOControl1;

        /// <summary>
        /// Gets the manual control wrapper.
        /// <para>수동 컨트롤 래퍼를 가져옵니다.</para>
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SpiralLab.Sirius3.UI.WinForms.ManualControl ManualCtrl => manualControl1;

        /// <summary>
        /// Gets the power meter control wrapper.
        /// <para>파워 미터 컨트롤 래퍼를 가져옵니다.</para>
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SpiralLab.Sirius3.UI.WinForms.PowerMeterControl PowerMeterCtrl => powerMeterControl1;

        /// <summary>
        /// Gets the power map control wrapper.
        /// <para>파워 맵 컨트롤 래퍼를 가져옵니다.</para>
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SpiralLab.Sirius3.UI.WinForms.PowerMapControl PowerMapCtrl => powerMapControl1;

        /// <summary>
        /// Gets the stepper control wrapper.
        /// <para>스태퍼 컨트롤 래퍼를 가져옵니다.</para>
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SpiralLab.Sirius3.UI.WinForms.StepperControl StepperCtrl => stepperControl1;

        /// <summary>
        /// Gets the entity pen control wrapper.
        /// <para>엔티티 펜 컨트롤 래퍼를 가져옵니다.</para>
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SpiralLab.Sirius3.UI.WinForms.EntityPenControl EntityPenCtrl => entityPenControl1;

        /// <summary>
        /// Gets the layer pen control wrapper.
        /// <para>레이어 펜 컨트롤 래퍼를 가져옵니다.</para>
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SpiralLab.Sirius3.UI.WinForms.LayerPenControl LayerPenCtrl => layerPenControl1;

        /// <summary>
        /// Gets the remote control wrapper.
        /// <para>리모트 컨트롤 래퍼를 가져옵니다.</para>
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SpiralLab.Sirius3.UI.WinForms.RemoteControl RemoteCtrl => remoteControl1;

        /// <summary>
        /// Gets the log control.
        /// <para>로그 컨트롤을 가져옵니다.</para>
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SpiralLab.Sirius3.UI.WinForms.LogControl LogCtrl => logControl1;
        #endregion

        #region Constructor & Form Lifecycle
        /// <summary>
        /// Initializes a new instance of <see cref="SiriusMultiEditorControl"/> and wires UI events. <br/>
        /// <see cref="SiriusMultiEditorControl"/>의 새 인스턴스를 초기화하고 UI 이벤트를 연결합니다. <br/>
        /// </summary>
        public SiriusMultiEditorControl()
        {
            InitializeComponent();
            ApplyLocalization();

            if (EditorControl.IsDesigner())
                return;

            // Embed editor control into tab page
            tabEditor.Controls.Add(editorControl1);
            editorControl1.Dock = DockStyle.Fill;
            editorControl1.Location = new Point(0, 0);
            editorControl1.Margin = new Padding(0);
            editorControl1.Name = "Editor";

            Disposed += SiriusEditorControl_Disposed;
            VisibleChanged += SiriusEditorControl_VisibleChanged;

            timerStatus.Interval = 100;
            timerStatus.Tick += TimerStatus_Tick;

            lblEncoder.DoubleClick += LblEncoder_DoubleClick;
            lblEncoder.DoubleClickEnabled = true;
            lblEncoder.MouseEnter += LblEncoder_MouseEnter;

            tbcLeft.SelectedIndexChanged += tbcLeft_SelectedIndexChanged;
            btnNew.Click += BtnNew_Click;
            btnOpen.Click += BtnOpen_Click;
            btnSave.Click += BtnSave_Click;
            btnLock.Click += BtnLock_Click;

            rdDevice0.Tag = "0";
            rdDevice0.CheckedChanged += RdDevice_CheckedChanged;
            rdDevice1.Tag = "1";
            rdDevice1.CheckedChanged += RdDevice_CheckedChanged;
            rdDevice2.Tag = "2";
            rdDevice2.CheckedChanged += RdDevice_CheckedChanged;
            rdDevice3.Tag = "3";
            rdDevice3.CheckedChanged += RdDevice_CheckedChanged;

            // Hide log window by default
            splitContainer2.Panel2Collapsed = true;
            splitContainer2.Panel2Collapsed = false;
            splitContainer2.Panel2Collapsed = true;
            btnLogWindow.Click += (_, __) =>
            {
                splitContainer2.Panel2Collapsed = !splitContainer2.Panel2Collapsed;
            };

            // Hide remote control tab by default
            tbcMain.TabPages.Remove(tabRemote);

            var doc = new DocumentBase();
            Document = doc;
        }

        private void ApplyLocalization()
        {
            btnNew.ToolTipText = MessageBoxLocalization.S("SiriusEditor_NewDocument");
            btnOpen.ToolTipText = MessageBoxLocalization.S("SiriusEditor_OpenDocument");
            ddbOpenNewOptions.ToolTipText = MessageBoxLocalization.S("SiriusEditor_DocumentOptions");
            ApplyIncludePageLocalization(mnuIncludePage1, 1);
            ApplyIncludePageLocalization(mnuIncludePage2, 2);
            ApplyIncludePageLocalization(mnuIncludePage3, 3);
            ApplyIncludePageLocalization(mnuIncludePage4, 4);
            ApplyIncludeLocalization(mnuIncludeBlocks, "SiriusEditor_Blocks", "SiriusEditor_IncludeBlocks");
            ApplyIncludeLocalization(mnuIncludeLayerPens, "SiriusEditor_LayerPens", "SiriusEditor_IncludeLayerPens");
            ApplyIncludeLocalization(mnuIncludeEntityPens, "SiriusEditor_EntityPens", "SiriusEditor_IncludeEntityPens");
            ApplyIncludeLocalization(mnuIncludeWafers, "SiriusEditor_Wafers", "SiriusEditor_IncludeWafers");
            ApplyIncludeLocalization(mnuIncludeSubstrates, "SiriusEditor_Substrates", "SiriusEditor_IncludeSubstrates");
            btnSave.ToolTipText = MessageBoxLocalization.S("SiriusEditor_SaveDocument");
            btnLock.ToolTipText = MessageBoxLocalization.S("SiriusEditor_LockEditing");
            btnLogWindow.ToolTipText = MessageBoxLocalization.S("SiriusEditor_ToggleLogWindow");
            tabBlockPage.Text = MessageBoxLocalization.S("SiriusEditor_NavBlock");
            tabEntityPen.Text = MessageBoxLocalization.S("SiriusEditor_NavEntity");
            tabLayerPen.Text = MessageBoxLocalization.S("SiriusEditor_NavLayer");
            tabEditor.Text = MessageBoxLocalization.S("SiriusEditor_NavEditor");
            tabMarker.Text = MessageBoxLocalization.S("SiriusEditor_NavMarker");
            tabManual.Text = MessageBoxLocalization.S("SiriusEditor_NavManual");
            tabScanner.Text = MessageBoxLocalization.S("SiriusEditor_NavScanner");
            tabLaser.Text = MessageBoxLocalization.S("SiriusEditor_NavLaser");
            tabDIO.Text = MessageBoxLocalization.S("SiriusEditor_NavDio");
            tabPower.Text = MessageBoxLocalization.S("SiriusEditor_NavPower");
            tabPage18.Text = MessageBoxLocalization.S("SiriusEditor_NavPowerMeter");
            tabPage19.Text = MessageBoxLocalization.S("SiriusEditor_NavPowerMap");
            tabStepper.Text = MessageBoxLocalization.S("SiriusEditor_NavStepper");
            tabRemote.Text = MessageBoxLocalization.S("SiriusEditor_NavRemote");
            tabProperty.Text = MessageBoxLocalization.S("SiriusEditor_NavProperty");
            lblAliasName.ToolTipText = MessageBoxLocalization.S("SiriusEditor_StatusName");
            lblFileName.ToolTipText = MessageBoxLocalization.S("SiriusEditor_StatusFileName");
            lblEncoder.ToolTipText = MessageBoxLocalization.S("SiriusEditor_StatusEncoder");
            lblReady.Text = MessageBoxLocalization.S("MultiBeam_ReadyStatus");
            lblReady.ToolTipText = MessageBoxLocalization.S("SiriusEditor_StatusReady");
            lblBusy.Text = MessageBoxLocalization.S("MultiBeam_BusyStatus");
            lblBusy.ToolTipText = MessageBoxLocalization.S("SiriusEditor_StatusBusy");
            lblError.Text = MessageBoxLocalization.S("MultiBeam_ErrorStatus");
            lblError.ToolTipText = MessageBoxLocalization.S("SiriusEditor_StatusError");
            lblRemote.Text = $" {Internal.RemoteStatusLocalization.Format(RemoteControlModes.Local, false)} ";
            lblRemote.ToolTipText = MessageBoxLocalization.S("SiriusEditor_StatusRemote");
        }

        private static void ApplyIncludePageLocalization(ToolStripMenuItem item, int page)
        {
            item.Text = MessageBoxLocalization.S("SiriusEditor_Page", page);
            item.ToolTipText = MessageBoxLocalization.S("SiriusEditor_IncludePage", page);
        }

        private static void ApplyIncludeLocalization(ToolStripMenuItem item, string textKey, string toolTipKey)
        {
            item.Text = MessageBoxLocalization.S(textKey);
            item.ToolTipText = MessageBoxLocalization.S(toolTipKey);
        }


        /// <summary>
        /// Registers devices for the specified device index.
        /// <para>지정된 장치 인덱스에 대한 장치를 등록합니다.</para>
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
        /// <param name="remote">The remote instance.</param>
        /// <param name="mcpServer">Optional MCP server created for this multi-editor. A non-null value is registered once and disposed before the device sets.<br/>이 다중 편집기를 대상으로 생성된 선택적 MCP 서버입니다. null이 아닌 값은 한 번 등록되며 장치 세트보다 먼저 해제됩니다.</param>
        public void RegisterDevices(int index, IScanner scanner, ILaser laser, IPowerMeter powerMeter, IDInput dIExt1, IDInput dILaserPort, IDOutput dOExt1, IDOutput dOExt2, IDOutput dOLaserPort, IMarker marker, IRemote remote = null, IMCPServer mcpServer = null)
        {
#if DEBUG
            if (MaxDeviceCounts <= index)
                throw new ArgumentOutOfRangeException(nameof(index), $"CurrentDeviceIndex must be less than {MaxDeviceCounts}.");
#endif
            scanners[index] = scanner;
            lasers[index] = laser;
            powerMeters[index] = powerMeter;
            dIExt1s[index] = dIExt1;
            dILaserPorts[index] = dILaserPort;
            dOExt1s[index] = dOExt1;
            dOExt2s[index] = dOExt2;
            dOLaserPorts[index] = dOLaserPort;
            var previousMarker = markers[index];
            markers[index] = marker;
            if (previousMarker != null && !markers.Any(value => ReferenceEquals(value, previousMarker)))
            {
                previousMarker.OnStarted -= Marker_OnStarted;
                previousMarker.OnEnded -= Marker_OnEnded;
                previousMarker.PropertyChanged -= Marker_PropertyChanged;
            }
            MultiBeamRtcControl.Markers[index] = marker;

            markers[index].OnStarted -= Marker_OnStarted;
            markers[index].OnStarted += Marker_OnStarted;
            markers[index].OnEnded -= Marker_OnEnded;
            markers[index].OnEnded += Marker_OnEnded;
            markers[index].PropertyChanged -= Marker_PropertyChanged;
            markers[index].PropertyChanged += Marker_PropertyChanged;

            remotes[index] = remote;
            if (mcpServer != null) MCPServer = mcpServer;
            marker.Ready(Document, View, scanner as IRtc, laser, powerMeter);
            EditorControlDispatch.Run(this, ApplyEditPermission);
        }

        /// <summary>
        /// Dispose all registered devices.
        /// <para>장치를 모두 해지하고 자원을 회수합니다.</para>
        /// </summary>
        public void DisposeDevices()
        {
            Document?.ActSimulateStop(false);
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

            var registeredMCPServer = MCPServer;
            MCPServer = null;
            registeredMCPServer?.Dispose();

            for (int i = 0; i < MaxDeviceCounts; i++)
            {
                remotes[i]?.Dispose();
                remotes[i] = null;

                MultiBeamRtcControl.Markers[i] = null;
                if (null != markers[i])
                {
                    markers[i].OnStarted -= Marker_OnStarted;
                    markers[i].OnEnded -= Marker_OnEnded;
                    markers[i].PropertyChanged -= Marker_PropertyChanged;
                }

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
#if DEBUG
            if (MaxDeviceCounts <= CurrentDeviceIndex)
                throw new ArgumentOutOfRangeException(nameof(index), $"CurrentDeviceIndex must be less than {MaxDeviceCounts}.");
#endif

            if (null == scanners[index] || null == lasers[index] || null == markers[index])
            {
                Logger.Log(LogLevel.Error, $"Some device is not registered yet at {index} index.");
                //throw new ArgumentOutOfRangeException(nameof(value), $"Some device is not assigned. null ?");
            }

            OnBeforeChangeDevice?.Invoke(this);

            CurrentDeviceIndex = index;
            try
            {
                var rdButton = new RadioButton[] { rdDevice0, rdDevice1, rdDevice2, rdDevice3 };
                for (int i = 0; i < rdButton.Length; i++)
                {
                    if (null == rdButton[i]) continue;
                    if (i == CurrentDeviceIndex)
                    {
                        rdButton[i].Checked = true;
                        rdButton[i].Text = $"Device {i + 1}";

                    }
                    else
                    {
                        rdButton[i].Text = ""; // $"{i + 1}";
                    }
                }
            }
            catch (Exception)
            { }

            this.Scanner = scanners[CurrentDeviceIndex];
            this.Laser = lasers[CurrentDeviceIndex];
            this.PowerMeter = powerMeters[CurrentDeviceIndex];
            this.Marker = markers[CurrentDeviceIndex];

            this.DIExt1 = dIExt1s[CurrentDeviceIndex];
            this.DILaserPort = dILaserPorts[CurrentDeviceIndex];
            this.DOExt1 = dOExt1s[CurrentDeviceIndex];
            this.DOExt2 = dOExt2s[CurrentDeviceIndex];
            this.DOLaserPort = dOLaserPorts[CurrentDeviceIndex];

            if (null != this.Marker && !this.Marker.IsBusy)
                this.Marker?.Ready(Document, View, Scanner as IRtc, Laser, PowerMeter);

            this.Remote = remotes[CurrentDeviceIndex];
            OnAfterChangeDevice?.Invoke(this);
            return true;
        }

        /// <summary>
        /// Handles the device radio button checked changed event.
        /// <para>장치 라디오 버튼 변경 이벤트를 처리합니다.</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        private void RdDevice_CheckedChanged(object sender, EventArgs e)
        {
            var btn = sender as RadioButton;
            if (null == btn) return;
            if (!btn.Checked) return;
            if (int.TryParse((string)btn.Tag, out int index))
            {
                SwitchDevices(index);
            }
        }

        /// <summary>
        /// Handles form closing; disposes timers.
        /// <para>폼 닫기를 처리하고 타이머를 해제합니다.</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        private void SiriusEditorControl_Disposed(object sender, EventArgs e)
        {
            if (document != null)
            {
                document.OnNew -= Document_OnNew;
                document.OnBeforeOpen -= Document_OnBeforeOpen;
                document.OnAfterOpen -= Document_OnAfterOpen;
                document.OnBeforeSave -= Document_OnBeforeSave;
                document.OnAfterSave -= Document_OnAfterSave;
                document.OnSimulationStarted -= Document_OnSimulationStarted;
                document.OnSimulationEnded -= Document_OnSimulationEnded;
            }
            foreach (var registeredMarker in markers)
            {
                if (registeredMarker == null) continue;
                registeredMarker.OnStarted -= Marker_OnStarted;
                registeredMarker.OnEnded -= Marker_OnEnded;
                registeredMarker.PropertyChanged -= Marker_PropertyChanged;
            }
            document?.ActSimulateStop(false);
            timerStatus.Enabled = false;
            timerStatus.Tick -= TimerStatus_Tick;
            timerStatus.Dispose();
        }

        /// <inheritdoc/>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            timerStatus.Enabled = Visible;
            UpdateMarkerStatus();
            ApplyEditPermission();
        }
        /// <summary>
        /// Enables or disables the status timer based on form visibility.
        /// <para>폼 가시성에 따라 상태 타이머를 활성화하거나 비활성화합니다.</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        private void SiriusEditorControl_VisibleChanged(object sender, EventArgs e)
        {
            timerStatus.Enabled = Visible;
            if (Visible)
            {
                SwitchDevices(CurrentDeviceIndex);
                UpdateMarkerStatus();
            }
        }
        #endregion

        #region Document Events
        private void Document_OnSimulationStarted(IDocument source, IEntity[] entities) =>
            QueueSimulationStateUpdate(source);

        private void Document_OnSimulationEnded(IDocument source) => QueueSimulationStateUpdate(source);

        private void QueueSimulationStateUpdate(IDocument source)
        {
            EditorControlDispatch.Run(this, () =>
            {
                if (ReferenceEquals(source, Document)) ApplyEditPermission();
            });
        }

        /// <summary>
        /// Called when a new document is created.
        /// <para>새 문서가 생성될 때 호출됩니다.</para>
        /// </summary>
        /// <param name="doc">The document instance.</param>
        private void Document_OnNew(IDocument doc)
        {
            if (!IsHandleCreated || IsDisposed) return;

            Invoke(new MethodInvoker(() =>
            {
                UpdatePens();
            }));
        }
        /// <summary>
        /// Called before a document open operation.
        /// <para>문서 열기 작업 전에 호출됩니다.</para>
        /// </summary>
        /// <param name="doc">The document instance.</param>
        private void Document_OnBeforeOpen(IDocument doc)
        {
            // Reserved for pre-open logic
        }

        /// <summary>
        /// Called after a document has been opened; updates pens and property grid.
        /// <para>문서가 열린 후 호출됩니다. 펜과 속성 그리드를 업데이트합니다.</para>
        /// </summary>
        /// <param name="doc">The document instance.</param>
        /// <param name="fileName">The name of the opened file.</param>
        private void Document_OnAfterOpen(IDocument doc, string fileName)
        {
            if (!IsHandleCreated || IsDisposed) return;

            Invoke(new MethodInvoker(() =>
            {
                UpdatePens();
                EntityPenCtrl.Document = document;

                lblFileName.Text = fileName;
                PropertyGridCtrl.Refresh();
            }));
        }

        /// <summary>
        /// Called before a document save operation.
        /// <para>문서 저장 작업 전에 호출됩니다.</para>
        /// </summary>
        /// <param name="doc">The document instance.</param>
        private void Document_OnBeforeSave(IDocument doc)
        {
            // Reserved for pre-save logic
        }

        /// <summary>
        /// Called after a document has been saved; updates file name label.
        /// <para>문서가 저장된 후 호출됩니다. 파일 이름 레이블을 업데이트합니다.</para>
        /// </summary>
        /// <param name="doc">The document instance.</param>
        /// <param name="fileName">The name of the saved file.</param>
        private void Document_OnAfterSave(IDocument doc, string fileName)
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
        /// Resets MoF encoder values with user confirmation.
        /// <para>사용자 확인을 통해 MoF 인코더 값을 재설정합니다.</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        private void LblEncoder_DoubleClick(object sender, EventArgs e)
        {
            if (Scanner is not IRtcMoF rtcMoF) return;

            bool isCtrlPressed = (Control.ModifierKeys & Keys.Control) == Keys.Control;

            var form = new SpiralLab.Sirius3.UI.WinForms.MessageBox(
                 MessageBoxLocalization.S("Scanner_ConfirmResetEncoders"),
                 MessageBoxLocalization.S("Title_Warning"),
                 MessageBoxButtons.YesNo);

            if (isCtrlPressed)
            {
                switch (rtcMoF.MoFMode)
                {
                    default:
                    case RtcMoFModes.XY:
                        form.TextLabel.LabelText = "OFFSET X";
                        form.TextLabel.Text = "0";
                        form.TextLabel.Visible = true;
                        form.TextLabel2.LabelText = "OFFSET Y";
                        form.TextLabel2.Text = "0";
                        form.TextLabel2.Visible = true;
                        break;
                    case RtcMoFModes.Rotary:
                        form.TextLabel.LabelText = "OFFSET ANGLE";
                        form.TextLabel.Text = "0";
                        form.TextLabel.Visible = true;
                        break;
                }

                var dialogResult = form.ShowDialog(this);
                if (dialogResult == DialogResult.Yes)
                {
                    switch (rtcMoF.MoFMode)
                    {
                        default:
                        case RtcMoFModes.XY:
                            double encX = 0, encY = 0;
                            double.TryParse(form.TextLabel.Text, out encX);
                            double.TryParse(form.TextLabel2.Text, out encY);
                            rtcMoF.CtlMoFEncoderReset(encX, encY);
                            break;
                        case RtcMoFModes.Rotary:
                            //rtcMoF.CtlMoFEncoderReset(encX, );
                            double encAngle = 0;
                            double.TryParse(form.TextLabel.Text, out encAngle);
                            rtcMoF.CtlMoFEncoderReset(encAngle);
                            break;
                    }
                }
            }
            else
            {
                var dialogResult = form.ShowDialog(this);
                if (dialogResult == DialogResult.Yes)
                {
                   rtcMoF.CtlMoFEncoderReset();
                }
            }
        }

        /// <summary>
        /// Periodic status painter for Ready/Busy/Error (and Remote if enabled).
        /// <para>준비/바쁨/오류 (및 원격이 활성화된 경우)에 대한 주기적인 상태 표시기입니다.</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        private void TimerStatus_Tick(object sender, EventArgs e)
        {
            UpdateMarkerStatus();
        }

        private void UpdateMarkerStatus()
        {
            var currentMarker = Marker;
            if (currentMarker == null)
            {
                lblReady.ForeColor = Color.White;
                lblReady.BackColor = Color.Green;
                lblBusy.ForeColor = Color.White;
                lblBusy.BackColor = Color.Olive;
                lblError.ForeColor = Color.White;
                lblError.BackColor = Color.Maroon;
                timerStatusColorCounts = 0;
                return;
            }

            // Ready
            if (currentMarker.IsReady)
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
            if (currentMarker.IsBusy)
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
            if (currentMarker.IsError)
            {
                lblError.ForeColor = Color.White;
                lblError.BackColor = Color.Red;
            }
            else
            {
                lblError.ForeColor = Color.White;
                lblError.BackColor = Color.Maroon;
            }

            if (null == Remote)
            {
                if (lblRemote.Visible)
                    lblRemote.Visible = false;
            }
            else
            {
                if (!lblRemote.Visible)
                    lblRemote.Visible = true;

                lblRemote.Text = $" {Internal.RemoteStatusLocalization.Format(Remote.ControlMode, Remote.IsConnected)} ";
                if (Remote.ControlMode == RemoteControlModes.Local)
                {
                    lblRemote.BackColor = Color.MidnightBlue;
                }
                else
                {
                    lblRemote.BackColor = Color.DodgerBlue;
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

            EditorControlDispatch.Run(this, () =>
            {
                if (ReferenceEquals(sender, Marker))
                    UpdateMarkerStatus();
            });
        }

        /// <summary>
        /// Called when marking starts; disables editing and starts progress timer.
        /// <para>마킹이 시작될 때 호출됩니다. 편집을 비활성화하고 진행 타이머를 시작합니다.</para>
        /// </summary>
        /// <param name="_marker">The marker instance.</param>
        private void Marker_OnStarted(IMarker _marker)
        {
            QueueMarkerStateUpdate(_marker);
        }

        /// <summary>
        /// Called when marking ends; re-enables editing and shows total time.
        /// <para>마킹이 종료될 때 호출됩니다. 편집을 다시 활성화하고 총 시간을 표시합니다.</para>
        /// </summary>
        /// <param name="_marker">The marker instance.</param>
        /// <param name="success">True if marking was successful, false otherwise.</param>
        /// <param name="ts">The elapsed time for the marking operation.</param>
        private void Marker_OnEnded(IMarker _marker, bool success, TimeSpan? ts)
        {
            QueueMarkerStateUpdate(_marker);
        }

        private void QueueMarkerStateUpdate(IMarker source)
        {
            EditorControlDispatch.Run(this, () =>
            {
                if (source != null && markers.Any(value => ReferenceEquals(value, source)))
                    ApplyEditPermission();
            });
        }

        private void LblEncoder_MouseEnter(object sender, EventArgs e)
        {
            lblEncoder.ToolTipText = Internal.EncoderStatusToolTip.Format(
                Scanner as IRtcMoF,
                MessageBoxLocalization.S("SiriusEditor_StatusEncoder"));
        }

        /// <summary>
        /// Called when MoF encoders change; updates encoder label text depending on MoF mode.
        /// <para>MoF 인코더가 변경될 때 호출됩니다. MoF 모드에 따라 인코더 레이블 텍스트를 업데이트합니다.</para>
        /// </summary>
        /// <param name="rtcMoF">The IRtcMoF instance.</param>
        /// <param name="encX">The X(or rotate) axis encoder count value.</param>
        /// <param name="encY">The Y axis encoder count value.</param>
        /// <param name="encXmmOrAngle">The X(or rptate) axis encoder converted to mm(or °)value.</param>
        /// <param name="encYmm">The Y axis encoder converted to mm value.</param>
        private void MoF_OnEncoderChanged(IRtcMoF rtcMoF, int encX, int encY, double encXmmOrAngle, double encYmm)
        {
            if (!stsBottom.IsHandleCreated || IsDisposed) return;

            rtcMoF.CtlMoFGetEncoderOffset(out var offsetEncX, out int offsetEncY, out double offsetEncXmmOrAngle, out double offsetEncYmm);
            try
            {
                switch (rtcMoF.MoFMode)
                {
                    default:
                    case RtcMoFModes.XY:
                        stsBottom.Invoke(new MethodInvoker(() =>
                        {
                            if (offsetEncX != 0 || offsetEncY != 0)
                                lblEncoder.Text = string.Format("ENC: {0:F3}({1:F3}), {2:F3}({3:F3})mm ({4}, {5})", encXmmOrAngle, offsetEncXmmOrAngle, encYmm, offsetEncYmm, encX, encY);
                            else
                                lblEncoder.Text = string.Format("ENC: {0:F3}, {1:F3}mm ({2}, {3})", encXmmOrAngle, encYmm, encX, encY);
                        }));
                        break;

                    case RtcMoFModes.Rotary:
                        stsBottom.Invoke(new MethodInvoker(() =>
                        {
                            if (offsetEncX != 0)
                                lblEncoder.Text = string.Format("ENC: {0:F3}({1:F3})° ({2})", encXmmOrAngle, offsetEncXmmOrAngle, encX);
                            else
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
        /// </summary>
        private void MenuVisibility()
        {
            Debug.Assert(Scanner != null);
            // Keep for future RTC-card specific UI toggles
        }

        /// <summary>
        /// Adjusts entity property visibility based on RTC capabilities.
        /// <para>RTC 기능에 따라 엔티티 속성 가시성을 조정합니다.</para>
        /// </summary>
        private void PropertyVisibility()
        {
            EntityPen.PropertyVisibility(Scanner);
            EntityPen.PropertyVisibility(Laser);
            EntityLayerPen.PropertyVisibility(Scanner);
        }
        /// <summary>
        /// Adjusts tab page visibility based on RTC capabilities.
        /// <para>RTC 기능에 따라 페이지 가시성을 조정합니다.</para>
        /// </summary>
        private void PageVisibility()
        {
            if (Scanner is IRtcSyncAxis)
            {
                if (tbcMain.TabPages.Contains(tabStepper))
                    tbcMain.TabPages.Remove(tabStepper);
            }
            else if (!tbcMain.TabPages.Contains(tabStepper))
                tbcMain.TabPages.Insert(tbcMain.TabPages.IndexOf(tabPower) + 1, tabStepper);
        }
        /// <summary>
        /// Requests editing; locks and active jobs still restrict editing.
        /// <para>편집 허용을 요청합니다. 잠금 또는 실행 중 작업의 편집 제한은 유지됩니다.</para>
        /// </summary>
        /// <param name="isEnable">True to enable; false to disable. 
        /// <para>활성화하려면 true, 비활성화하려면 false입니다.</para>
        /// </param>
        public virtual void ControlEnableOrNot(bool isEnable)
        {
            isEditEnabled = isEnable;
            EditorControlDispatch.Run(this, ApplyEditPermission);
        }

        private void ApplyEditPermission()
        {
            if (IsDisposed || Disposing || EditorCtrl == null) return;
            bool isEnable = isEditEnabled && !btnLock.Checked &&
                !markers.Any(value => value?.IsBusy == true) && Document?.IsSimulationWorking != true;
            btnNew.Enabled = isEnable;
            btnOpen.Enabled = isEnable;
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
            //WaferCtrl.Enabled = isEnable;
            //SubstrateCtrl.Enabled = isEnable;


#if DEBUG
            // Keep enables for debugging

#else
            //ManualCtrl.Enabled = isEnable;
            //ScannerCtrl.Enabled = isEnable;
            LaserCtrl.Enabled = isEnable;
            PowerMeterCtrl.Enabled = isEnable;
            PowerMapCtrl.Enabled = isEnable;
            //DORtcCtrl.Enabled = isEnable;
            EntityPenCtrl.Enabled = isEnable;
            LayerPenCtrl.Enabled = isEnable;
            //MarkerCtrl.Enabled = isEnable;
#endif

            // This button owns the requested edit-lock state, so it must remain
            // available even while simulation, marking, or the lock itself disables editing.
            tlsTop1.Enabled = true;
            btnLock.Enabled = true;
        }

        /// <summary>
        /// Update device information at entity, layer pens.
        /// <para>엔티티, 레이저 펜에 레이저 및 스캐너의 디바이스 정보를 업데이트합니다.</para>
        /// </summary>
        private void UpdatePens()
        {
            if (null != document)
            {
                foreach (var child in document.DocumentData.EntityPens.Children)
                {
                    var pen = child as EntityPen;
                    pen.Apply(scanners[CurrentDeviceIndex], lasers[CurrentDeviceIndex]);
                }

                foreach (var child in document.DocumentData.LayerPens.Children)
                {
                    var pen = child as EntityLayerPen;
                    pen.Apply(scanners[CurrentDeviceIndex], lasers[CurrentDeviceIndex]);
                }
            }
        }

        /// <summary>
        /// Update laser information 
        /// </summary>
        private void UpdateLaser()
        {
            if (powerMeters[CurrentDeviceIndex] != null)
            {
                if (powerMeters[CurrentDeviceIndex] is PowerMeterVirtual powerMeterVirtual)
                {
                    powerMeterVirtual.Laser = lasers[CurrentDeviceIndex];
                }
            }
        }

        /// <summary>
        /// Show(or hide) <see cref="LogCtrl"/> window  at bottom
        /// </summary>
        /// <param name="show"><c>True</c>: Show<br/>
        /// <c>False</c>: Hide (Default)
        /// </param>
        public void ShowLogWindow(bool show)
        {
            if (!IsHandleCreated || IsDisposed) return;
            splitContainer2.Panel2Collapsed = !show;
        }

        /// <summary>
        /// Show(or hide) <c>TreeView</c>, <see cref="EntityPenControl"/> and <see cref="LayerPenControl"/> windows at left side
        /// </summary>
        /// <param name="show"><c>True</c>: Show  (Default)<br/>
        /// <c>False</c>: Hide 
        /// </param>
        public void ShowTreeViewAndPens(bool show)
        {
            if (!IsHandleCreated || IsDisposed) return;
            splitContainer12.Panel1Collapsed = !show;
        }
        /// <summary>
        /// Show(or hide) <see cref="EntityPenControl"/> and <see cref="LayerPenControl"/> windows at left bottom side
        /// </summary>
        /// <param name="show"><c>True</c>: Show  (Default)<br/>
        /// <c>False</c>: Hide 
        /// </param>
        public void ShowPens(bool show)
        {
            if (!IsHandleCreated || IsDisposed) return;
            splitContainerLeft.Panel2Collapsed = !show;
            isShowPen = show;
        }
        /// <summary>
        /// Show(or hide) <see cref="PropertyGridControl"/> window at right side
        /// </summary>
        /// <param name="show"><c>True</c>: Show  (Default)<br/>
        /// <c>False</c>: Hide
        /// </param>
        public void ShowPropertyWindow(bool show)
        {
            if (!IsHandleCreated || IsDisposed) return;
            splitContainer123.Panel2Collapsed = !show;
        }

        #endregion

        #region Left Tab / File Buttons
        /// <summary>
        /// Switches active page/layer in the document based on the selected left tab.
        /// <para>선택된 왼쪽 탭에 따라 문서의 활성 페이지/레이어를 전환합니다.</para>
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
                    editorControl1.View.ActiveCamera.ZoomFit(Document.ActivePage?.ActiveLayer);
                    break;
                case 1:
                    Document.Page = DocumentPages.Page2;
                    Document.ActivePage = Document.DocumentData.Pages[1];
                    editorControl1.Document.ActRegen();
                    editorControl1.View.ActiveCamera.ZoomFit(Document.ActivePage?.ActiveLayer);
                    break;
                case 2:
                    Document.Page = DocumentPages.Page3;
                    Document.ActivePage = Document.DocumentData.Pages[2];
                    editorControl1.Document.ActRegen();
                    editorControl1.View.ActiveCamera.ZoomFit(Document.ActivePage?.ActiveLayer);
                    break;
                case 3:
                    Document.Page = DocumentPages.Page4;
                    Document.ActivePage = Document.DocumentData.Pages[3];
                    editorControl1.Document.ActRegen();
                    editorControl1.View.ActiveCamera.ZoomFit(Document.ActivePage?.ActiveLayer);
                    break;
                case 4:
                    Document.Page = DocumentPages.Block;
                    editorControl1.Document.ActRegen();
                    editorControl1.View.ActiveCamera.ZoomFit(Document.DocumentData.Blocks.Children.ToArray());
                    break;
                    //case :
                    //    Document.Page = DocumentPages.Wafer;
                    //    editorControl1.Document.ActRegen();
                    //    editorControl1.View.Camera.ZoomFit(Document.DocumentData.Wafers.Children.ToArray());
                    //    break;
                    //case :
                    //    Document.Page = DocumentPages.Substrate;
                    //    editorControl1.Document.ActRegen();
                    //    editorControl1.View.Camera.ZoomFit(Document.DocumentData.Substrates.Children.ToArray());
                    //    break;
            }

            editorControl1.View.DoRender();
            Cursor.Current = Cursors.Default;
        }

        /// <summary>
        /// Creates a new document according to selected include flags.
        /// <para>선택된 포함 플래그에 따라 새 문서를 생성합니다.</para>
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
        /// Opens a document from file with selected include flags.
        /// <para>선택된 포함 플래그로 파일에서 문서를 엽니다.</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        private void BtnOpen_Click(object sender, EventArgs e)
        {
            if (Document == null) return;

            using var dlg = new OpenFileDialog
            {
                Filter = SpiralLab.Sirius3.UI.Config.FileOpenFilters,
                Title = MessageBoxLocalization.S("Common_Open"),
                InitialDirectory = SpiralLab.Sirius3.Config.RecipePath,
                FileName = Document.FileName,
            };

            if (dlg.ShowDialog() != DialogResult.OK) return;

            if (Document.IsModified)
            {
                var form = new SpiralLab.Sirius3.UI.WinForms.MessageBox(
                    MessageBoxLocalization.S("Document_ConfirmOpenUnsaved"),
                    MessageBoxLocalization.S("Title_Warning"),
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

            var previousCursor = Cursor.Current;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                Document?.ActOpen(
                    dlg.FileName,
                    includeLayers,
                    includeLayers2nd,
                    includeBlocks,
                    includeEntityPens,
                    includeLayerPens,
                    includeWafers,
                    includeSubstrates);
            }
            finally
            {
                Cursor.Current = previousCursor;
            }

            OnAfterOpen?.Invoke(this, dlg.FileName);
        }

        /// <summary>
        /// Saves the current document to file.
        /// <para>현재 문서를 파일에 저장합니다.</para>
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (Document == null) return;

            using var dlg = new SaveFileDialog
            {
                Filter = SpiralLab.Sirius3.UI.Config.FileSaveFilters,
                Title = MessageBoxLocalization.S("Common_Save"),
                InitialDirectory = SpiralLab.Sirius3.Config.RecipePath,
                OverwritePrompt = true
            };

            if (dlg.ShowDialog() != DialogResult.OK) return;
            bool success;
            var previousCursor = Cursor.Current;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                success = Document.ActSave(dlg.FileName);
            }
            finally
            {
                Cursor.Current = previousCursor;
            }
#if DEBUG
            if (success && SpiralLab.Sirius3.UI.Config.IsFileSaveWithImage)
            {
                var bitmap = View?.SnapShot(Document);
                string filePath = $"{Path.GetDirectoryName(dlg.FileName)}\\{Path.GetFileNameWithoutExtension(dlg.FileName)}.bmp";
                bitmap?.Save(filePath, System.Drawing.Imaging.ImageFormat.Bmp);
                bitmap?.Dispose();
            }
#endif
            OnAfterSave?.Invoke(this, dlg.FileName);
        }

        /// <summary>
        /// Toggles allow to edit(lock) at view or not.
        /// <para>뷰에서의 편집 허용(잠금) 여부를 토글합니다.</para>
        /// </summary>
        /// <param name="sender">The source of the event. <para>이벤트 소스입니다.</para></param>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data. <para>이벤트 데이터를 포함하는 <see cref="EventArgs"/>입니다.</para></param>
        private void BtnLock_Click(object sender, EventArgs e)
        {
            ApplyEditPermission();
        }

        #endregion
    }
}
