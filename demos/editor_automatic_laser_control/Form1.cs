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
using Microsoft.Extensions.Logging;
using SpiralLab.Sirius3;
using SpiralLab.Sirius3.Mathematics;
using SpiralLab.Sirius3.Scanner.Rtc.SyncAxis;
using System.Diagnostics;

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
    /// Automatic Laser Control (ALC) demo
    /// 자동 레이저 제어 (ALC) 데모
    /// </summary>
    public partial class Form1 : Form
    {
        /// <summary>
        /// Form constructor
        /// 폼 생성자
        /// </summary>
        public Form1()
        {
            // Initialize SIRIUS3 library
            // SIRIUS3 라이브러리 초기화
            SpiralLab.Sirius3.Core.Initialize();

            InitializeComponent();
            this.Load += Form1_Load;
            this.FormClosing += (s, e) =>
            {
                var dlgResult = MessageBox.Show(this, $"Do you really want to terminate program ?", "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dlgResult != DialogResult.Yes)
                {
                    // is marker need to abort ?
                    // 마킹 중단 여부 확인 ?
                    e.Cancel = true;
                    return;
                }

                // Dispose instances 
                // 인스턴스 해제 
                siriusEditorControl1.DisposeDevices();

                // Dispose document
                // 문서 해제
                var doc = siriusEditorControl1.Document;
                siriusEditorControl1.Document = null;
                doc?.Dispose();

                // Clean up SIRIUS3 library
                // SIRIUS3 라이브러리 정리
                SpiralLab.Sirius3.Core.Cleanup();
            };

            // Attach button click events
            // 버튼 클릭 이벤트 연결
            btnDefinedVector.Click += BtnDefinedVector_Click;
            btnSetVelocity.Click += BtnSetVelocity_Click;
            btnActualVelocity.Click += BtnActualVelocity_Click;
            btnSpotDistanceControl.Click += BtnSpotDistanceControl_Click;
            btnPositionDependent.Click += BtnPositionDependent_Click;
        }

        /// <summary>
        /// Form load
        /// 폼 로드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            // Create devices
            // 장치 생성
            EditorHelper.CreateDevices(out IRtc rtc, out ILaser laser, out IDInput dInExt1, out IDInput dInLaserPort, out IDOutput dOutExt1, out IDOutput dOutExt2, out IDOutput dOutLaserPort, out IPowerMeter powerMeter, out IMarker marker);

            // Register devices to control
            // 컨트롤에 장치 등록
            siriusEditorControl1.RegisterDevices(rtc, laser, powerMeter, dInExt1, dInLaserPort, dOutExt1, dOutExt2, dOutLaserPort, marker);

            // Ready marker
            // 마커 준비
            marker.Ready(siriusEditorControl1.Document, siriusEditorControl1.View, rtc, laser, powerMeter);
        }

        /// <summary>
        /// Create test entities (Line with optional Ramp)
        /// 테스트용 엔티티 생성 (선 및 선택적 램프)
        /// </summary>
        /// <param name="withRampEntity"></param>
        private void CreateEntity(bool withRampEntity = false)
        {
            var document = siriusEditorControl1.Document;

            if (withRampEntity)
            {
                // Create Ramp Begin entity (Analog1)
                // 램프 시작 엔티티 생성 (아날로그1)
                double startingVoltage = 5.0;
                var rampBegin = EntityFactory.CreateRampBegin(AutoLaserControlSignals.Analog1, startingVoltage);
                document.ActAdd(rampBegin);
            }

            // Create test line
            // 테스트용 선 생성
            var line = EntityFactory.CreateLine(0, 0, 20, 0);
            if (withRampEntity)
            {
                // Apply ramp factors to line
                // 선에 램프 계수 적용
                line.StartRampFactor = 0.5;
                line.EndRampFactor = 2;
            }

            document.ActAdd(line);

            if (withRampEntity)
            {
                // Create Ramp End entity
                // 램프 종료 엔티티 생성
                var rampEnd = EntityFactory.CreateRampEnd();
                document.ActAdd(rampEnd);
            }

            siriusEditorControl1.View?.DoRender();
        }

        /// <summary>
        /// Defined Vector mode: Manual scaling with Ramp entities
        /// Defined Vector 모드: 램프 엔티티를 사용한 수동 스케일링
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDefinedVector_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            document.ActNew();

            // Create measurement begin for data logging
            // 데이터 로깅을 위한 측정 시작 엔티티 생성
            var begin = EntityFactory.CreateMeasurementBegin(
                10 * 1000,
                new MeasurementChannels[] {
                    MeasurementChannels.SampleX,
                    MeasurementChannels.SampleY,
                    MeasurementChannels.LaserOn,
                    MeasurementChannels.ExtAO1,
                },
                "Defined vector (scale: 0.5 -> 2.0) + analog1"
                );
            document.ActAdd(begin);

            // Create entities with ramp
            // 램프를 포함한 엔티티 생성
            CreateEntity(true);

            // Create measurement end
            // 측정 종료 엔티티 생성
            var end = EntityFactory.CreateMeasurementEnd();
            document.ActAdd(end);

            Debug.Assert(document.ActivePage.ActiveLayer.PenColor == Color.White);

            // Find layer pen for 'White'
            // 'White' 레이어 펜 찾기
            document.FindByLayerPenColor(System.Drawing.Color.White, out var layerPenWhite);

            // Disable ALC on pen (using manual Ramp entities instead)
            // 펜의 ALC 비활성화 (대신 수동 램프 엔티티 사용)
            layerPenWhite.IsALC = false;
            layerPenWhite.AlcSignal = AutoLaserControlSignals.Disabled;
            layerPenWhite.AlcMode = AutoLaserControlModes.Disabled;
            layerPenWhite.AlcByPositionTable.Clear();

            siriusEditorControl1.PropertyGridCtrl.Refresh();
        }

        /// <summary>
        /// Speed Dependent mode: Set Velocity
        /// 속도 종속 모드: 설정 속도 기준 (Set Velocity)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSetVelocity_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            document.ActNew();

            // Create measurement begin
            // 측정 시작 생성
            var begin = EntityFactory.CreateMeasurementBegin(
                10 * 1000,
                new MeasurementChannels[] 
                {
                    MeasurementChannels.SampleX,
                    MeasurementChannels.SampleY,
                    MeasurementChannels.LaserOn,
                    MeasurementChannels.ExtAO1,
                },
                "Set velocity + analog1"
                );
            document.ActivePage.ActiveLayer.AddChild(begin);

            CreateEntity();

            var end = EntityFactory.CreateMeasurementEnd();
            document.ActAdd(end);

            Debug.Assert(document.ActivePage.ActiveLayer.PenColor == Color.White);

            // Find layer pen for 'White'
            // 'White' 레이어 펜 찾기
            document.FindByLayerPenColor(System.Drawing.Color.White, out var layerPenWhite);

            // Enable ALC: Set velocity + analog output
            // ALC 활성화: 설정 속도 + 아날로그 출력
            layerPenWhite.IsALC = true;
            layerPenWhite.AlcSignal = AutoLaserControlSignals.Analog1;
            layerPenWhite.AlcMode = AutoLaserControlModes.SetVelocity;
            layerPenWhite.AlcModeExtension.Clear();
            layerPenWhite.AlcPercentage100 = 5; // 5V at 100% speed
            layerPenWhite.AlcMinValue = 4; // 4V min
            layerPenWhite.AlcMaxValue = 6; // 6V max
            layerPenWhite.AlcByPositionTable.Clear();

            siriusEditorControl1.PropertyGridCtrl.Refresh();
        }

        /// <summary>
        /// Speed Dependent mode: Actual Velocity (Frequency control)
        /// 속도 종속 모드: 실제 속도 피드백 기준 (주파수 제어)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnActualVelocity_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            document.ActNew();

            // Create measurement begin
            // 측정 시작 생성
            var begin = EntityFactory.CreateMeasurementBegin(
                10 * 1000,
                new MeasurementChannels[] 
                {
                    MeasurementChannels.SampleX,
                    MeasurementChannels.SampleY,
                    MeasurementChannels.LaserOn,
                    MeasurementChannels.OutputPeriod,
                },
                "Actual velocity + frequency"
                );
            document.ActAdd(begin);

            CreateEntity();

            var end = EntityFactory.CreateMeasurementEnd();
            document.ActAdd(end);

            Debug.Assert(document.ActivePage.ActiveLayer.PenColor == Color.White);

            // Find layer pen for 'White'
            // 'White' 레이어 펜 찾기
            document.FindByLayerPenColor(System.Drawing.Color.White, out var layerPenWhite);

            // Enable ALC: Actual velocity + frequency control
            // ALC 활성화: 실제 속도 피드백 + 주파수 제어
            layerPenWhite.IsALC = true;
            layerPenWhite.AlcSignal = AutoLaserControlSignals.Frequency;
            layerPenWhite.AlcMode = AutoLaserControlModes.ActualVelocity; // Only for iDRIVE scanner products / iDRIVE 스캐너 제품 전용
            layerPenWhite.AlcModeExtension.Clear();
            // layerPenWhite.AlcModeExtension.Add(AutoLaserControlModeExtensions.Bit.SCANAhead);
            layerPenWhite.AlcPercentage100 = 50 * 1000; // 50KHz at 100% speed
            layerPenWhite.AlcMinValue = 40 * 1000; // Lower cut off frequency : 40KHz
            layerPenWhite.AlcMaxValue = 60 * 1000; // Upper cut off frequency : 60KHz
            layerPenWhite.AlcByPositionTable.Clear();

            siriusEditorControl1.PropertyGridCtrl.Refresh();
        }

        /// <summary>
        /// Speed Dependent mode: Spot Distance Control (SDC)
        /// 속도 종속 모드: 등간격 제어 (SDC - Spot Distance Control)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSpotDistanceControl_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            document.ActNew();

            double spotDistance = 0.1;
            // Create measurement begin
            // 측정 시작 생성
            var begin = EntityFactory.CreateMeasurementBegin(
                10 * 1000,
                new MeasurementChannels[] 
                {
                    MeasurementChannels.SampleX,
                    MeasurementChannels.SampleY,
                    MeasurementChannels.LaserOn,
                    // MeasurementChannels.SpotDistance,
                },
                $"Spot distance control: {spotDistance:F3}mm"
                );
            document.ActAdd(begin);

            CreateEntity();

            var end = EntityFactory.CreateMeasurementEnd();
            document.ActAdd(end);

            Debug.Assert(document.ActivePage.ActiveLayer.PenColor == Color.White);

            // Find layer pen for 'White'
            // 'White' 레이어 펜 찾기
            document.FindByLayerPenColor(System.Drawing.Color.White, out var layerPenWhite);

            // Enable ALC: Actual velocity + spot distance control
            // ALC 활성화: 실제 속도 피드백 + 등간격 제어
            layerPenWhite.IsALC = true;
            layerPenWhite.AlcByPositionTable.Clear();
            layerPenWhite.AlcSignal = AutoLaserControlSignals.SpotDistance; // RTC6 + SCANAhead required / RTC6 + SCANAhead 필요
            layerPenWhite.AlcMode = AutoLaserControlModes.ActualVelocity;
            layerPenWhite.AlcModeExtension.Clear();

            // SCANAhead extension for excelliSCAN/intelliSCAN IV
            // excelliSCAN 헤드 및 SDC(등간격 제어) 사용 시 필수
            layerPenWhite.AlcModeExtension.Add(AutoLaserControlModeExtensions.Bit.SCANAhead);

            // Enable SDC during Sky Writing
            // Sky Writing 중 SDC 알고리즘 유지 (RTC6 전용)
            layerPenWhite.AlcModeExtension.Add(AutoLaserControlModeExtensions.Bit.SkyWritingSDC);

            // Find entity pen for 'White'
            // 'White' 엔티티 펜 찾기
            document.FindByEntityPenColor(System.Drawing.Color.White, out var entityPenWhite);
            entityPenWhite.SpotDistanceSCANa = 0.02; // 20um pitch

            siriusEditorControl1.PropertyGridCtrl.Refresh();
        }

        /// <summary>
        /// Position Dependent mode: Power scaling by distance from center
        /// 위치 종속 모드: 중심으로부터의 거리에 따른 파워 스케일링
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnPositionDependent_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            document.ActNew();

            // Create measurement begin
            // 측정 시작 생성
            var begin = EntityFactory.CreateMeasurementBegin(
                10 * 1000,
                new MeasurementChannels[] {
                    MeasurementChannels.SampleX,
                    MeasurementChannels.SampleY,
                    MeasurementChannels.LaserOn,
                    MeasurementChannels.ExtAO1,
                },
                "position dependent + analog1"
                );
            document.ActAdd(begin);

            CreateEntity();

            var end = EntityFactory.CreateMeasurementEnd();
            document.ActAdd(end);

            Debug.Assert(document.ActivePage.ActiveLayer.PenColor == Color.White);

            // Find layer pen for 'White'
            // 'White' 레이어 펜 찾기
            document.FindByLayerPenColor(System.Drawing.Color.White, out var layerPenWhite);

            // Enable ALC with position dependent table
            // ALC 활성화 및 위치 종속 테이블 설정
            layerPenWhite.IsALC = true;
            layerPenWhite.AlcSignal = AutoLaserControlSignals.Analog1;
            layerPenWhite.AlcMode = AutoLaserControlModes.SetVelocity;
            layerPenWhite.AlcModeExtension.Clear();
            layerPenWhite.AlcPercentage100 = 5; // 5V base
            layerPenWhite.AlcMinValue = 0; // 0V min
            layerPenWhite.AlcMaxValue = 10; // 10V max

            // Define Position Table: Distance(mm), Scale factor (0~4)
            // 위치 테이블 정의: 거리(mm), 스케일 계수 (0~4)
            var kvList = new List<KeyValuePair<double, double>>();
            kvList.Add(new KeyValuePair<double, double>(2, 1));     // 100% scale at 2mm
            kvList.Add(new KeyValuePair<double, double>(3, 0.8));   // 80% scale at 3mm
            kvList.Add(new KeyValuePair<double, double>(4, 0.6));   // 60% scale at 4mm
            kvList.Add(new KeyValuePair<double, double>(5, 0.5));   // 50% scale at 5mm
            kvList.Add(new KeyValuePair<double, double>(20, 0.4));  // 40% scale at 20mm
            kvList.Add(new KeyValuePair<double, double>(50, 0.1));  // 10% scale at 50mm

            // Assign position dependent table
            // 위치 종속 테이블 할당
            layerPenWhite.AlcByPositionTable = kvList;

            siriusEditorControl1.PropertyGridCtrl.Refresh();
        }
    }
}
