/*
 * 2026 Copyright to (c)SpiralLAB. All rights reserved.
 * Description : Native WPF multi-device editor control
 */

using System;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using SpiralLab.Sirius3.Document;
using SpiralLab.Sirius3.IO;
using SpiralLab.Sirius3.Laser;
using SpiralLab.Sirius3.Marker;
using SpiralLab.Sirius3.PowerMeter;
using SpiralLab.Sirius3.Remote;
using SpiralLab.Sirius3.Scanner;
using SpiralLab.Sirius3.View;
using SpiralLab.Sirius3.Localization;
using SpiralLab.Sirius3.MCP;
using SpiralLab.Sirius3.UI.WPF;
using Border = global::System.Windows.Controls.Border;
using Image = global::System.Windows.Controls.Image;
using Orientation = global::System.Windows.Controls.Orientation;
using StackPanel = global::System.Windows.Controls.StackPanel;
using TextBlock = global::System.Windows.Controls.TextBlock;
using ToggleButton = global::System.Windows.Controls.Primitives.ToggleButton;
using UserControl = global::System.Windows.Controls.UserControl;

namespace Demos
{
    /// <summary>
    /// Native WPF editor that switches one document and UI among up to four registered device sets.
    /// Registered documents and devices remain caller-owned until <see cref="DisposeDevices"/> is called explicitly.
    /// <para>최대 4개 장치 세트를 전환하는 WPF 편집기입니다. 문서와 장치는 호출자가 소유합니다.</para>
    /// </summary>
    /// <remarks>Change bindings on the UI thread, separately from application-driven marker starts.
    /// Busy changes are ignored with a warning.
    /// <para>UI 스레드에서 연결을 변경하며 외부 마커 시작과 동시에 수행하지 않습니다. 가공 중 변경 요청은 경고를 남기고 무시합니다.</para></remarks>
    public class SiriusMultiEditorControl : UserControl, IDisposable
    {
        private const int DeviceCapacity = 4;
        private readonly SiriusEditorControl editor;
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
        private bool disposed;

        /// <summary>Raised after New is completed by the inner editor.</summary>
        public event Action<SiriusMultiEditorControl> OnAfterNew;
        /// <summary>Raised after Open is completed by the inner editor.</summary>
        public event Action<SiriusMultiEditorControl, string> OnAfterOpen;
        /// <summary>Raised after Save is completed by the inner editor.</summary>
        public event Action<SiriusMultiEditorControl, string> OnAfterSave;
        /// <summary>Raised before the active device set changes.</summary>
        public event Action<SiriusMultiEditorControl> OnBeforeChangeDevice;
        /// <summary>Raised after the active device set changes.</summary>
        public event Action<SiriusMultiEditorControl> OnAfterChangeDevice;

        /// <summary>Initializes the native WPF multi-device editor.</summary>
        public SiriusMultiEditorControl()
        {
            WPFThemeManager.Initialize();
            UseLayoutRounding = true;
            SnapsToDevicePixels = true;
            editor = new SiriusEditorControl();
            editor.TopToolbarAccessory = CreateDeviceSelector();
            Content = editor;

            editor.OnAfterNew += Editor_OnAfterNew;
            editor.OnAfterOpen += Editor_OnAfterOpen;
            editor.OnAfterSave += Editor_OnAfterSave;
            RefreshSelector();
        }

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
        /// <summary>Gets or sets the status alias shown by the editor.</summary>
        public string AliasName { get => editor.AliasName; set => editor.AliasName = value; }
        /// <summary>Gets or sets the caller-owned document shared by all device sets. All registered markers must be idle.
        /// <para>모든 장치 세트가 공유하는 호출자 소유 문서를 설정합니다. 등록된 모든 마커가 유휴 상태여야 합니다.</para></summary>
        [Browsable(false)]
        public IDocument Document
        {
            get => editor.Document;
            set
            {
                if (ReferenceEquals(editor.Document, value))
                    return;
                if (!CanChangeDevices(nameof(Document)))
                    return;
                editor.Document = value;
                if (value != null)
                    for (var index = 0; index < DeviceCapacity; index++)
                        markers[index]?.Ready(value, View, scanners[index], lasers[index], powerMeters[index]);
            }
        }
        /// <summary>Gets the current rendering view.</summary>
        [Browsable(false)] public IView View => editor.View;
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

        /// <summary>Gets the active scanner.</summary>
        [Browsable(false)] public IScanner Scanner { get => scanners[CurrentDeviceIndex]; private set => scanners[CurrentDeviceIndex] = value; }
        /// <summary>Gets the active laser.</summary>
        [Browsable(false)] public ILaser Laser { get => lasers[CurrentDeviceIndex]; private set => lasers[CurrentDeviceIndex] = value; }
        /// <summary>Gets the active marker.</summary>
        [Browsable(false)] public IMarker Marker { get => markers[CurrentDeviceIndex]; private set => markers[CurrentDeviceIndex] = value; }
        /// <summary>Gets the active power meter.</summary>
        [Browsable(false)] public IPowerMeter PowerMeter { get => powerMeters[CurrentDeviceIndex]; private set => powerMeters[CurrentDeviceIndex] = value; }
        /// <summary>Gets the active Extension 1 input.</summary>
        [Browsable(false)] public IDInput DIExt1 { get => diExt1s[CurrentDeviceIndex]; private set => diExt1s[CurrentDeviceIndex] = value; }
        /// <summary>Gets the active laser-port input.</summary>
        [Browsable(false)] public IDInput DILaserPort { get => diLaserPorts[CurrentDeviceIndex]; private set => diLaserPorts[CurrentDeviceIndex] = value; }
        /// <summary>Gets the active Extension 1 output.</summary>
        [Browsable(false)] public IDOutput DOExt1 { get => doExt1s[CurrentDeviceIndex]; private set => doExt1s[CurrentDeviceIndex] = value; }
        /// <summary>Gets the active Extension 2 output.</summary>
        [Browsable(false)] public IDOutput DOExt2 { get => doExt2s[CurrentDeviceIndex]; private set => doExt2s[CurrentDeviceIndex] = value; }
        /// <summary>Gets the active laser-port output.</summary>
        [Browsable(false)] public IDOutput DOLaserPort { get => doLaserPorts[CurrentDeviceIndex]; private set => doLaserPorts[CurrentDeviceIndex] = value; }
        /// <summary>Gets the active remote interface.</summary>
        [Browsable(false)] public IRemote Remote { get => remotes[CurrentDeviceIndex]; private set => remotes[CurrentDeviceIndex] = value; }
        /// <summary>
        /// Gets or sets the MCP server created for this multi-editor. The assignment enables UI control and transfers disposal responsibility to <see cref="DisposeDevices"/>.
        /// <para>이 다중 편집기를 대상으로 생성된 MCP 서버를 가져오거나 설정합니다. 지정하면 UI 제어를 사용하며 <see cref="DisposeDevices"/>가 서버 해제를 담당합니다.</para>
        /// </summary>
        [LocalizedCategory("MCP")]
        [LocalizedDisplayName("MCPServer")]
        [LocalizedDescription("MCPServer")]
        public IMCPServer MCPServer { get => editor.MCPServer; set => editor.MCPServer = value; }
        /// <summary>Gets or sets whether the bottom log panel is visible.</summary>
        public bool IsShowLogWindow { get => editor.IsShowLogWindow; set => editor.IsShowLogWindow = value; }
        /// <summary>Gets or sets whether the left tree-and-pen pane is visible.</summary>
        public bool IsShowTreeViewAndPen { get => editor.IsShowTreeViewAndPen; set => editor.IsShowTreeViewAndPen = value; }
        /// <summary>Gets or sets whether the pen tabs are visible.</summary>
        public bool IsShowPen { get => editor.IsShowPen; set => editor.IsShowPen = value; }
        /// <summary>Gets or sets whether the property pane is visible.</summary>
        public bool IsPropertyGridWindow { get => editor.IsPropertyGridWindow; set => editor.IsPropertyGridWindow = value; }
        /// <summary>Gets the inner WPF editor.</summary>
        [Browsable(false)] public SiriusEditorControl EditorHost => editor;
        /// <summary>Gets the inner OpenGL editor control.</summary>
        [Browsable(false)] public EditorControl EditorCtrl => editor.EditorCtrl;
        /// <summary>Gets the native WPF property grid.</summary>
        [Browsable(false)] public PropertyGridControl PropertyGridCtrl => editor.PropertyGridCtrl;
        /// <summary>Gets the native WPF page trees.</summary>
        [Browsable(false)] public TreeViewPageControl[] PageCtrls => editor.PageCtrls;
        /// <summary>Gets the native WPF block tree.</summary>
        [Browsable(false)] public TreeViewBlockControl BlockCtrl => editor.BlockCtrl;
        /// <summary>Gets the native WPF laser panel.</summary>
        [Browsable(false)] public LaserControl LaserCtrl => editor.LaserCtrl;
        /// <summary>Gets the native WPF scanner panel.</summary>
        [Browsable(false)] public ScannerControl ScannerCtrl => editor.ScannerCtrl;
        /// <summary>Gets the native WPF marker panel.</summary>
        [Browsable(false)] public MarkerControl MarkerCtrl => editor.MarkerCtrl;
        /// <summary>Gets the native WPF DI panel.</summary>
        [Browsable(false)] public DIRtcControl DIRtcCtrl => editor.DIRtcCtrl;
        /// <summary>Gets the native WPF DO panel.</summary>
        [Browsable(false)] public DORtcControl DORtcCtrl => editor.DORtcCtrl;
        /// <summary>Gets the native WPF manual panel.</summary>
        [Browsable(false)] public ManualControl ManualCtrl => editor.ManualCtrl;
        /// <summary>Gets the native WPF power-meter panel.</summary>
        [Browsable(false)] public PowerMeterControl PowerMeterCtrl => editor.PowerMeterCtrl;
        /// <summary>Gets the native WPF power-map panel.</summary>
        [Browsable(false)] public PowerMapControl PowerMapCtrl => editor.PowerMapCtrl;
        /// <summary>Gets the native WPF stepper panel.</summary>
        [Browsable(false)] public StepperControl StepperCtrl => editor.StepperCtrl;
        /// <summary>Gets the native WPF entity-pen panel.</summary>
        [Browsable(false)] public EntityPenControl EntityPenCtrl => editor.EntityPenCtrl;
        /// <summary>Gets the native WPF layer-pen panel.</summary>
        [Browsable(false)] public LayerPenControl LayerPenCtrl => editor.LayerPenCtrl;
        /// <summary>Gets the native WPF remote panel.</summary>
        [Browsable(false)] public RemoteControl RemoteCtrl => editor.RemoteCtrl;
        /// <summary>Gets the native WPF log panel.</summary>
        [Browsable(false)] public LogControl LogCtrl => editor.LogCtrl;

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

        /// <summary>Switches the active device set when all registered markers are idle; neither set is disposed.
        /// <para>등록된 모든 마커가 유휴 상태일 때 활성 장치 세트를 전환하며 장치는 해제하지 않습니다.</para></summary>
        /// <param name="index">Target device-set index.<br/>대상 장치 세트 인덱스입니다.</param>
        /// <returns>Whether switching succeeded.<br/>전환 성공 여부입니다.</returns>
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
            editor.RegisterDevices(scanners[CurrentDeviceIndex], lasers[CurrentDeviceIndex], powerMeters[CurrentDeviceIndex],
                diExt1s[CurrentDeviceIndex], diLaserPorts[CurrentDeviceIndex], doExt1s[CurrentDeviceIndex],
                doExt2s[CurrentDeviceIndex], doLaserPorts[CurrentDeviceIndex], markers[CurrentDeviceIndex], remotes[CurrentDeviceIndex]);
            PageVisibility();
            // The inner editor detaches its previous marker; the multi-editor still owns that registration.
            foreach (var marker in markers)
                MarkerRegistry.Register(marker);
        }

        private void PageVisibility()
        {
            editor.PageVisibility(Scanner);
        }

        private void Editor_OnAfterNew(SiriusEditorControl value) => OnAfterNew?.Invoke(this);
        private void Editor_OnAfterOpen(SiriusEditorControl value, string fileName) => OnAfterOpen?.Invoke(this, fileName);
        private void Editor_OnAfterSave(SiriusEditorControl value, string fileName) => OnAfterSave?.Invoke(this, fileName);

        private void OnMarkerStarted(IMarker value) => DispatchMarkerBusyState();
        private void OnMarkerEnded(IMarker value, bool success, TimeSpan? elapsed) => DispatchMarkerBusyState();

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
            editor.SetMultiMarkerBusy(anyBusy);
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
            SpiralLab.Sirius3.Logger.Log(LogLevel.Warning, $"WPF multi-editor: {operation} rejected; marker [{busyMarker.Index}] is busy.");
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

        /// <inheritdoc cref="SiriusEditorControl.ControlEnableOrNot"/>
        public virtual void ControlEnableOrNot(bool isEnable) => editor.ControlEnableOrNot(isEnable);

        /// <summary>Shows or hides the bottom log panel.</summary>
        public void ShowLogWindow(bool show) => editor.ShowLogWindow(show);

        /// <summary>Shows or hides the left tree-and-pen pane.</summary>
        public void ShowTreeViewAndPens(bool show) => editor.ShowTreeViewAndPens(show);

        /// <summary>Shows or hides the pen tabs.</summary>
        public void ShowPens(bool show) => editor.ShowPens(show);

        /// <summary>Shows or hides the right property pane.</summary>
        public void ShowPropertyWindow(bool show) => editor.ShowPropertyWindow(show);

        /// <summary>Detaches and disposes all registered device sets, continuing after individual failures.
        /// <para>모든 등록 장치를 연결 해제하고 개별 실패가 발생해도 나머지 장치를 해제합니다.</para></summary>
        /// <exception cref="AggregateException">One or more cleanup operations failed.<br/>하나 이상의 해제 작업이 실패했습니다.</exception>
        public void DisposeDevices()
        {
            var cleanup = new WPFCleanup();
            var devices = new List<IDisposable>();
            cleanup.Run(() => Document?.ActSimulateStop(false));
            if (MCPServer != null) devices.Add(MCPServer);
            editor.DetachDevices(cleanup);
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

        /// <summary>Releases UI and rendering resources; assigned documents and devices remain caller-owned.
        /// <para>UI와 렌더링 자원을 해제하며 할당된 문서와 장치 소유권은 유지합니다.</para></summary>
        /// <remarks>Complete this call before removing the loaded control or closing its host.
        /// <para>로드된 컨트롤을 제거하거나 호스트 창을 닫기 전에 호출을 완료합니다.</para></remarks>
        public void Dispose()
        {
            if (disposed) return;
            disposed = true;
            foreach (var button in deviceButtons)
                if (button != null)
                    button.Click -= OnDeviceButtonClick;
            editor.OnAfterNew -= Editor_OnAfterNew;
            editor.OnAfterOpen -= Editor_OnAfterOpen;
            editor.OnAfterSave -= Editor_OnAfterSave;
            foreach (var marker in markers)
            {
                if (marker != null)
                {
                    marker.OnStarted -= OnMarkerStarted;
                    marker.OnEnded -= OnMarkerEnded;
                }
                MarkerRegistry.Unregister(marker);
            }
            editor.Dispose();
        }
    }
}
