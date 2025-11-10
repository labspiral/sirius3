using System;

using SpiralLab.Sirius3.Document;
using SpiralLab.Sirius3.Scanner;
using SpiralLab.Sirius3.IO;
using SpiralLab.Sirius3.Scanner.Rtc;
using SpiralLab.Sirius3.PowerMeter;
using SpiralLab.Sirius3.Laser;
using SpiralLab.Sirius3.Marker;
using SpiralLab.Sirius3.Entity;
using System.Text;
using SpiralLab.Sirius3.Entity.Hatch;
using System.Diagnostics;
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
    public partial class Form1 : Form
    {
        const int maxSerialNo = 10;
        int startingSerialNo = 1;
        int currentSerialNo = 1;

        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;

            this.btnCreateEntities.Click += BtnCreateEntities_Click;
            this.btnStartStop.Click += BtnStartStop_Click;
            this.btnStartEncoderSimulation.Click += BtnStartEncoderSimulation_Click;
            this.btnStopEncoderSimulation.Click += BtnStopEncoderSimulation_Click;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            EditorHelper.CreateDevices(out IRtc rtc, out ILaser laser, out IDInput dInExt1, out IDInput dInLaserPort, out IDOutput dOutExt1, out IDOutput dOutExt2, out IDOutput dOutLaserPort, out IPowerMeter powerMeter, out IMarker marker);

            siriusEditorControl1.Scanner = rtc;

            siriusEditorControl1.Laser = laser;

            // Set D.IN0 name
            // DIN0 을 시작 트리거로 사용할 것이므로
            // DIN.0 을 'External Trigger' 라고 명명
            dInExt1.ChannelNames[0][0] = "External Trigger";

            siriusEditorControl1.DIExt1 = dInExt1;
            siriusEditorControl1.DOExt1 = dOutExt1;
            siriusEditorControl1.DOExt2 = dOutExt2;
            siriusEditorControl1.DILaserPort = dInLaserPort;
            siriusEditorControl1.DOLaserPort = dOutLaserPort;

            siriusEditorControl1.PowerMeter = powerMeter;

            siriusEditorControl1.Marker = marker;

            marker.Ready(siriusEditorControl1.Document, siriusEditorControl1.View, rtc, laser, powerMeter);

            txtSerialNo.Text = $"{currentSerialNo}";
        }

        private void BtnCreateEntities_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            document.ActNew();

            CreateEntities();
            CreateEventHandlers();
        }

        void CreateEntities()
        {
            var document = siriusEditorControl1.Document;
            var marker = siriusEditorControl1.Marker;
            var rtc = siriusEditorControl1.Scanner as IRtc;

            // Create waiting D.IN0 ("External Trigger")
            // Condition: rising edge
            // D.IN0 트리거 신호가 High(Rising edge) 될때 까지 대기하는 제어용 객체 추가
            var waitExt16Cond = EntityFactory.CreateWaitDataExt16EdgeCond(
                0, //D.IN0 ("External Trigger")
                SignalEdges.High);
            document.ActivePage?.ActiveLayer?.AddChild(waitExt16Cond);

            // Create mof begin with encoder reset
            // 입력 엔코더(이동거리)값을 0 으로 초기화하도록 해주고
            // 이후 스캐너의 이동에 실시간 입력되는 외부 엔코더값을 추가(+) 해주는것을 시작하는
            // 제어용 MoF 시작 객체 추가
            // Also, need to MoF option at library option.
            Core.License(out var licenseInfo);
            Debug.Assert(licenseInfo.IsMoFLicensed);

            var mofBegin = EntityFactory.CreateMoFBegin(RtcMoFModes.XY, true);
            document.ActivePage?.ActiveLayer?.AddChild(mofBegin);

            // Create barcode
            // 바코드 객체 추가 및 IsAllowConvert 을 true 로 사용시 가공 직전 바코드 텍스트 데이타를 지정된 컨버터로 변경해주는 기능 사용
            // 이 예제에서는 이벤트 핸들러(IMarker 의 OnTextConvert 이벤트 핸들러)를 사용하는 방식 사용
            var barcode = EntityFactory.CreateDataMatrix("0123456789", EntityBarcode2DBase.Barcode2DCells.Dots, 5, 5);
            barcode.CellLine.DotFactor = 1;
            barcode.Name = "MyBarcode";
            barcode.IsAllowConvert = true;
            barcode.TextConverter = TextConverters.Event; // marker.OnTextConvert event will be called
            barcode.SourceText = "SERIAL NO";

            document.ActivePage?.ActiveLayer?.AddChild(barcode);

            // Create text
            // 텍스트 객체 추가 및 IsAllowConvert 을 true 로 사용시 가공 직전 바코드 텍스트 데이타를 지정된 컨버터로 변경해주는 기능 사용
            // 이 예제에서는 이벤트 핸들러(IMarker 의 OnTextConvert 이벤트 핸들러)를 사용하는 방식 사용
            var text = EntityFactory.CreateText("Arial", FontStyle.Regular, "0123456789", 2);
            text.Name = "MyText";
            text.IsAllowConvert = true;  
            text.TextConverter = TextConverters.Event; // marker.OnTextConvert event will be called
            text.SourceText = "SERIAL NO";

            text.Translate(0, -2.5);
            document.ActivePage?.ActiveLayer?.AddChild(text);

            // Create mof end with jump 0,0
            // 입력 엔코더(이동거리)에 의한 스캐너 추종이 중단(MoF end) 됨
            // 이후 실시간 입력되는 외부 엔코더처리는 중단되고 스캐너를 물리적 원점(0,0) 으로 점프
            // 제어용 MoF 끝 객체 추가
            var mofEnd = EntityFactory.CreateMoFEnd(DVec2.Zero);
            document.ActivePage?.ActiveLayer?.AddChild(mofEnd);

            // Create user event
            // 사용자 이벤트용 제어 객체 추가
            // 사용자 이벤트가 실행되면 이벤트 핸들러 (IMarker 의 OnUserEvent 이벤트 핸들러) 가 호출됨
            // 이 예제에서는 OnUserEvent 이벤트 호출시 일련번호를 증가시키는 기능을 추가함
            var userEvent = EntityFactory.CreateUserEvent(); // marker.OnUserEvent event will be called
            
            document.ActivePage?.ActiveLayer?.AddChild(userEvent);

            siriusEditorControl1.View?.DoRender();
            
            // Repeats 100 times
            // 최대 100개의 트리거 입력을 처리하기 위해
            // 레이어의 가공 반복회수(Repeats) 를 입력함
            document.ActivePage.ActiveLayer.Repeats = 100;


            Debug.Assert(rtc.IsMoF);
            var rtcMoF = rtc as IRtcMoF;
            Debug.Assert(rtcMoF != null);
            // 단위 mm 당 발생하는 엔코더 펄스의 개수가 지정되어 있어야 함
            Debug.Assert(rtcMoF.EncXCountsPerMm != 0);
            //Debug.Assert(rtcMoF.EncYCountsPerMm != 0);
        }

        private void BtnStartStop_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            var marker = siriusEditorControl1.Marker;

            if (marker.IsBusy)
            {
                // 이미 마커(IMarker)가 가공중이면 강제 중단
                marker.Stop();
                marker.Reset();
                txtSerialNo.Enabled = true;
            }
            else
            {
                // 마커(IMarker) 가공시작을 위해 
                // 초기 일련번호를 설정
                currentSerialNo = int.Parse(txtSerialNo.Text);

                marker.Reset();
                marker.Ready(siriusEditorControl1.Document);
                marker.Start(document.Page); // current page
                txtSerialNo.Enabled = false;
            }
        }

        private void BtnStartEncoderSimulation_Click(object sender, EventArgs e)
        {
            // 외부 엔코더 신호가 RTC 카드의 MOF 입력핀으로 연결되어 있으면
            // 엔코더 시뮬레이션 기능을 사용할 필요가 없음
            var rtc = siriusEditorControl1.Scanner as IRtc;

            Debug.Assert(rtc.IsMoF);
            var rtcMoF = rtc as IRtcMoF;

            // Activated simulated encoders for test purpose (x= -1, y=0 mm/s)
            // DO NOT set simulated encoder speed if ENC 0,1 has connected
            // 엔코더 입력 시뮬레이션 시작 (x 축 방향은 -1mm/s 가상 속도, y 축은 미사용)
            rtcMoF.CtlMofEncoderSpeed(-1, 0);
        }

        private void BtnStopEncoderSimulation_Click(object sender, EventArgs e)
        {
            // 외부 엔코더 신호가 RTC 카드의 MOF 입력핀으로 연결되어 있으면
            // 엔코더 시뮬레이션 기능을 사용할 필요가 없음
            var rtc = siriusEditorControl1.Scanner as IRtc;

            Debug.Assert(rtc.IsMoF);
            var rtcMoF = rtc as IRtcMoF;

            // Deactivated simulated encoders 
            // 엔코더 입력 시뮬레이션 중지 (가상 속도를 0 으로 하는것으로 중지됨)
            rtcMoF.CtlMofEncoderSpeed(0, 0);

            // Reset encoders
            rtcMoF.CtlMofEncoderReset();
        }

        void CreateEventHandlers()
        {
            var marker = siriusEditorControl1.Marker;
            
            // 바코드 및 텍스트 객체의 데이타 변경용 이벤트 핸들러 등록
            marker.OnTextConvert -= Marker_OnTextConvert;
            marker.OnTextConvert += Marker_OnTextConvert;

            // UserEvent 객체 통지용 이벤트 핸들러 등록
            marker.OnUserEvent -= Marker_OnUserEvent;
            marker.OnUserEvent += Marker_OnUserEvent;
        }

        private string Marker_OnTextConvert(IMarker marker, ITextConvertible textConvertible)
        {
            var entity = textConvertible as IEntity;

            var currentLayer = marker.WorkingSet.Layer;
            var currentLayerIndex = marker.WorkingSet.LayerIndex;
            var currentEntity = marker.WorkingSet.Entity;
            var currentEntityIndex = marker.WorkingSet.EntityIndex;
            var currentOffset = marker.WorkingSet.Offset;
            var currentOffsetIndex = marker.WorkingSet.OffsetIndex;

            // 바코드 및 텍스트 객체의 이름을 통해 변경될 데이타 리턴            
            // 가공 데이타 좌표들이 RTC 의 리스트 버퍼에 미리 추가되어야 하기에
            // 화면상의 텍스트는 이미 변경되어 있음 (버퍼 크기가 클수로 많이 변경됨)
            // 이후 트리거 (D.IN0) 발생시 리스트 버퍼의 데이타가 순차적으로 실행(가공)됨
            // RTC 리스트 버퍼에서 데이타가 처리되면 빈 리스트 버퍼 공간이 생길때 까지 대기하다가
            // 이후 텍스트 데이타 변경이 되는 방식이 반복됨

            switch (currentEntity.Name)
            {
                case "MyBarcode":
                    return $"Barcode {currentSerialNo}";
                case "MyText":
                    return $"Text {currentSerialNo}";
                default:
                    // Not modified
                    return textConvertible.SourceText;
            }
            
        }

        private bool Marker_OnUserEvent(IMarker marker, EntityUserEvent entityUserEvent)
        {
            // 일련번호 증가 및 최대값 처리
            currentSerialNo++;
            if (startingSerialNo > maxSerialNo)
                currentSerialNo = startingSerialNo;

            if (txtSerialNo.InvokeRequired)
            {
                txtSerialNo.BeginInvoke((MethodInvoker)(() => txtSerialNo.Text = $"{currentSerialNo}"));
            }
            else
            {
                txtSerialNo.Text = $"{currentSerialNo}";
            }


            return true;
        }
    }
}
