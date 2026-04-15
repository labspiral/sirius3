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
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
            this.FormClosing += (s, e) =>
            {
                var dlgResult = MessageBox.Show(this, $"Do you really want to terminate program ?", "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dlgResult != DialogResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }

                // Dispose instances 
                siriusEditorControl1.DisposeDevices();

                // Dispose document
                var doc = siriusEditorControl1.Document;
                siriusEditorControl1.Document = null;
                doc?.Dispose();

                // Clean up SIRIUS3 library
                SpiralLab.Sirius3.Core.Cleanup();
            };

            btnDefinedVector.Click += BtnDefinedVector_Click;

            btnSetVelocity.Click += BtnSetVelocity_Click;
            btnActualVelocity.Click += BtnActualVelocity_Click;
            btnSpotDistanceControl.Click += BtnSpotDistanceControl_Click;

            btnPositionDependent.Click += BtnPositionDependent_Click;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            EditorHelper.CreateDevices(out IRtc rtc, out ILaser laser, out IDInput dInExt1, out IDInput dInLaserPort, out IDOutput dOutExt1, out IDOutput dOutExt2, out IDOutput dOutLaserPort, out IPowerMeter powerMeter, out IMarker marker);

            siriusEditorControl1.Scanner = rtc;
            siriusEditorControl1.Laser = laser;
            siriusEditorControl1.DIExt1 = dInExt1;
            siriusEditorControl1.DOExt1 = dOutExt1;
            siriusEditorControl1.DOExt2 = dOutExt2;
            siriusEditorControl1.DILaserPort = dInLaserPort;
            siriusEditorControl1.DOLaserPort = dOutLaserPort;
            siriusEditorControl1.PowerMeter = powerMeter;
            siriusEditorControl1.Marker = marker;

            marker.Ready(siriusEditorControl1.Document, siriusEditorControl1.View, rtc, laser, powerMeter);
        }


        private void CreateEntity(bool withRampEntity = false)
        {
            var document = siriusEditorControl1.Document;

            if (withRampEntity)
            {
                double startingVoltage = 5.0;
                var rampBegin = EntityFactory.CreateRampBegin(AutoLaserControlSignals.Analog1, startingVoltage);
                document.ActivePage.ActiveLayer.AddChild(rampBegin);
            }

            var line = EntityFactory.CreateLine(0, 0, 20, 0);
            if (withRampEntity)
            {
                line.StartRampFactor = 0.5;
                line.EndRampFactor = 2;
            }

            document.ActivePage.ActiveLayer.AddChild(line);

            if (withRampEntity)
            {
                var rampEnd = EntityFactory.CreateRampEnd();
                document.ActivePage.ActiveLayer.AddChild(rampEnd);
            }

            siriusEditorControl1.View?.DoRender();
        }

        private void BtnDefinedVector_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            document.ActNew();

            // create measurement begin
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
            document.ActivePage.ActiveLayer.AddChild(begin);

            CreateEntity(true);

            var end = EntityFactory.CreateMeasurementEnd();
            document.ActivePage.ActiveLayer.AddChild(end);

            Debug.Assert(document.ActivePage.ActiveLayer.PenColor == Color.White);

            // Find layer pen for 'White'
            document.FindByLayerPenColor(System.Drawing.Color.White, out var layerPenWhite);

            //Set veloticy + analog output
            layerPenWhite.IsALC = false;
            layerPenWhite.AlcSignal = AutoLaserControlSignals.Disabled;
            layerPenWhite.AlcMode = AutoLaserControlModes.Disabled;
            layerPenWhite.AlcByPositionTable.Clear();

            siriusEditorControl1.PropertyGridCtrl.Refresh();
        }

        private void BtnSetVelocity_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            document.ActNew();

            // create measurement begin
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
            document.ActivePage.ActiveLayer.AddChild(end);

            Debug.Assert(document.ActivePage.ActiveLayer.PenColor == Color.White);

            // Find layer pen for 'White'
            document.FindByLayerPenColor(System.Drawing.Color.White, out var layerPenWhite);

            //Set veloticy + analog output
            layerPenWhite.IsALC = true;
            layerPenWhite.AlcSignal = AutoLaserControlSignals.Analog1;
            layerPenWhite.AlcMode = AutoLaserControlModes.SetVelocity;
            layerPenWhite.AlcModeExtension.Clear();
            layerPenWhite.AlcPercentage100 = 5; //5V
            layerPenWhite.AlcMinValue = 4; // 4V
            layerPenWhite.AlcMaxValue = 6; //6V
            layerPenWhite.AlcByPositionTable.Clear();

            siriusEditorControl1.PropertyGridCtrl.Refresh();
        }

        private void BtnActualVelocity_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            document.ActNew();

            // create measurement begin
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
            document.ActivePage.ActiveLayer.AddChild(begin);

            CreateEntity();

            var end = EntityFactory.CreateMeasurementEnd();
            document.ActivePage.ActiveLayer.AddChild(end);

            Debug.Assert(document.ActivePage.ActiveLayer.PenColor == Color.White);

            // Find layer pen for 'White'
            document.FindByLayerPenColor(System.Drawing.Color.White, out var layerPenWhite);

            // Actual velocity + frequency
            layerPenWhite.IsALC = true;
            layerPenWhite.AlcSignal = AutoLaserControlSignals.Frequency;
            layerPenWhite.AlcMode = AutoLaserControlModes.ActualVelocity; // Only for iDRIVE scanner products 
            layerPenWhite.AlcModeExtension.Clear();
            //layerPenWhite.AlcModeExtension.Add(AutoLaserControlModeExtensions.Bit.SCANAhead);
            layerPenWhite.AlcPercentage100 = 50 * 1000; //50KHz
            layerPenWhite.AlcMinValue = 40 * 1000; //Lower cut off frequency : 40KHz
            layerPenWhite.AlcMaxValue = 60 * 1000; //Upper cut off frequency : 60KHz
            layerPenWhite.AlcByPositionTable.Clear();

            siriusEditorControl1.PropertyGridCtrl.Refresh();
        }

        private void BtnSpotDistanceControl_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            document.ActNew();

            double spotDistance = 0.1;
            // create measurement begin
            var begin = EntityFactory.CreateMeasurementBegin(
                10 * 1000,
                new MeasurementChannels[] 
                {
                    MeasurementChannels.SampleX,
                    MeasurementChannels.SampleY,
                    MeasurementChannels.LaserOn,
                    //MeasurementChannels.SpotDistance,
                },
                $"Spot distance control: {spotDistance:F3}mm"
                );
            document.ActivePage.ActiveLayer.AddChild(begin);

            CreateEntity();

            var end = EntityFactory.CreateMeasurementEnd();
            document.ActivePage.ActiveLayer.AddChild(end);

            Debug.Assert(document.ActivePage.ActiveLayer.PenColor == Color.White);

            // Find layer pen for 'White'
            document.FindByLayerPenColor(System.Drawing.Color.White, out var layerPenWhite);

            // Actual velocity + spot distance control
            layerPenWhite.IsALC = true;
            layerPenWhite.AlcByPositionTable.Clear();
            layerPenWhite.AlcSignal = AutoLaserControlSignals.SpotDistance; //RTC6 + SCANAhead
            layerPenWhite.AlcMode = AutoLaserControlModes.ActualVelocity;
            layerPenWhite.AlcModeExtension.Clear();

            // excelliSCAN 헤드 및 SDC(등간격 제어) 사용 시 필수.
            // Tracking Error 대신 프리뷰 타임(Preview Time)을 기반으로 제어.
            layerPenWhite.AlcModeExtension.Add(AutoLaserControlModeExtensions.Bit.SCANAhead);

            // Sky Writing 중 SDC 유지. 스카이 라이팅(가감속 구간) 동작 중에도 SDC 알고리즘을 유지.
            // 벡터의 시작과 끝부분에서도 펄스 간격을 정밀하게 유지합니다. SCANahead 활성화가 필요. (RTC6 전용).
            layerPenWhite.AlcModeExtension.Add(AutoLaserControlModeExtensions.Bit.SkyWritingSDC);

            // 엔코더 속도 합산. 스캐너 속도에 엔코더 속도를 벡터 합산.
            // 이동하는 물체를 가공하는 MoF(Marking On-the-Fly) 공정에서 사용.
            //layerPenWhite.AlcModeExtension.Add(AutoLaserControlModeExtensions.Bit.EncoderSpeedAddition); 

            // 역 속도 보정. F-Theta 렌즈 왜곡으로 인해 발생하는 위치별 선속도 차이를 보정.
            // 보정 테이블을 사용하여 각속도를 실제 필드 상의 속도로 변환.
            //layerPenWhite.AlcModeExtension.Add(AutoLaserControlModeExtensions.Bit.InverseSpeedCorrection); 

            // 역 좌표 변환. 좌표 변환(회전, 행렬 등)이 적용된 경우, 피드백 속도를 역변환.
            // 레이저 제어가 변환 전의 원본 도면 속도를 기준으로 수행되도록 보장. (RTC6 전용)
            //layerPenWhite.AlcModeExtension.Add(AutoLaserControlModeExtensions.Bit.BackwardTransformation); 

            // Find entity pen for 'White'
            document.FindByEntityPenColor(System.Drawing.Color.White, out var entityPenWhite);
            entityPenWhite.SpotDistanceSCANa = 0.02; //20um

            siriusEditorControl1.PropertyGridCtrl.Refresh();
        }

        private void BtnPositionDependent_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            document.ActNew();

            // create measurement begin
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
            document.ActivePage.ActiveLayer.AddChild(begin);

            CreateEntity();

            var end = EntityFactory.CreateMeasurementEnd();
            document.ActivePage.ActiveLayer.AddChild(end);

            Debug.Assert(document.ActivePage.ActiveLayer.PenColor == Color.White);

            // Find layer pen for 'White'
            document.FindByLayerPenColor(System.Drawing.Color.White, out var layerPenWhite);

            // Mode Disable + analog output 
            layerPenWhite.IsALC = true;
            layerPenWhite.AlcSignal = AutoLaserControlSignals.Analog1;
            layerPenWhite.AlcMode = AutoLaserControlModes.SetVelocity;
            layerPenWhite.AlcModeExtension.Clear();
            layerPenWhite.AlcPercentage100 = 5; //5V
            layerPenWhite.AlcMinValue = 0; // 0V
            layerPenWhite.AlcMaxValue = 10; //10V

            // Distance(or radius) (mm), scale (0~4)
            var kvList = new List<KeyValuePair<double, double>>();
            kvList.Add(new KeyValuePair<double, double>(2, 1));
            kvList.Add(new KeyValuePair<double, double>(3, 0.8));
            kvList.Add(new KeyValuePair<double, double>(4, 0.6));
            kvList.Add(new KeyValuePair<double, double>(5, 0.5));
            kvList.Add(new KeyValuePair<double, double>(20, 0.4));
            kvList.Add(new KeyValuePair<double, double>(50, 0.1));

            // Position dependent
            layerPenWhite.AlcByPositionTable = kvList;

            siriusEditorControl1.PropertyGridCtrl.Refresh();
        }
    }
}
