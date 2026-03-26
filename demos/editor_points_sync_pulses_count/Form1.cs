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
using System.Diagnostics;
using SpiralLab.Sirius3;
using SpiralLab.Sirius3.Entity.Hatch;
using SpiralLab.Sirius3.Mathematics;


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

                // Clean up SIRIUS3 library
                SpiralLab.Sirius3.Core.Cleanup();
            };

            this.btnCreateEntities.Click += BtnCreateEntities_Click;
            this.btnStartStop.Click += BtnStartStop_Click;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            EditorHelper.CreateDevices(out IRtc rtc, out ILaser laser, out IDInput dInExt1, out IDInput dInLaserPort, out IDOutput dOutExt1, out IDOutput dOutExt2, out IDOutput dOutLaserPort, out IPowerMeter powerMeter, out IMarker marker);

            // Allowed RTC5 or 6 only
            // RTC5,6 만 지원됨
            VerifyProperCard(rtc);

            // External SYNC signal edge level
            // 외부 신호의 엣지 레벨 설정
            ConfigLASERPORT_DIN1_Pin_SignalLevel(rtc, true);

            // Addition synchronization options
            // 아래는 부가적인 동기화 기능에 대한 소개로, 사용을 하지 않아도 됩니다.
            if (rtc is Rtc5 rtc5)
            {
                ConfigOutputSynchronization(rtc5);
            }
            else if (rtc is Rtc6 rtc6)
            {
                // Choose only one thing
                // 아래 2 개중 하나의 기능만 선택 사용
                //ConfigLASER1Synchronization(rtc6);
                // or
                ConfigLASER1Synchronization(rtc6); // RTC6 only
            }

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

        /// <summary>
        /// D.IN1 pin at LASER PORT is exist RTC5,6 only
        /// <para>
        /// RTC5,6 이상에서만 LASER 커넥터에 있는 DIGITAL IN1 핀이 제공됩니다.
        /// </para>
        /// </summary>
        void VerifyProperCard(IRtc rtc)
        {
            Debug.Assert(rtc is Rtc5 || rtc is Rtc6);
        }

        /// <summary>
        /// Configure D.IN1 signal edge (falling or rising)
        /// <para>
        /// <see cref="EntityPen.PixelPulses"/> 설정된 펄스 개수만큼, DIGITAL IN1 핀에 입력되는 외부 펄스에 대해 상승 모서리에서 셀지, 하강 모서리에서 셀지를 결정합니다.
        /// 외부 펄스 신호는 RTC 보드의 LASER 커넥터에 있는 DIGITAL IN1 핀에 반드시 연결해야 합니다.
        /// LASER 커넥터의 DIGITAL IN1 핀으로 레이저 소스의 Sync Out 신호를 입력시킵니다.
        /// DIGITAL IN1 핀으로 입력받는 외부 신호는 레이저 소스 자체의 실제 발진(공진) 주기와 일치하는 마스터 클럭(Master Clock) 또는 펄스 동기(Sync) 신호여야 합니다.
        /// </para>
        /// </summary>
        void ConfigLASERPORT_DIN1_Pin_SignalLevel(IRtc rtc, bool isDIGITALIN1FallingEdge = true)
        {
            var rtcSignalLevel = rtc as IRtcSignalLevel;
            Debug.Assert(rtcSignalLevel != null);

            if (rtc is Rtc5 rtc5)
            {
                var lcs = rtc5.LaserControlSignal;
                if (isDIGITALIN1FallingEdge)
                    lcs.Remove(Rtc5LaserControlSignal.Bit.ExtSignalPulseRisingEdge);
                else
                    lcs.Add(Rtc5LaserControlSignal.Bit.ExtSignalPulseRisingEdge);
                rtc5.LaserControlSignal = lcs;
            }
            else if (rtc is Rtc6 rtc6)
            {
                var lcs = rtc6.LaserControlSignal;
                if (isDIGITALIN1FallingEdge)
                    lcs.Remove(Rtc6LaserControlSignal.Bit.ExtSignalPulseRisingEdge);
                else
                    lcs.Add(Rtc6LaserControlSignal.Bit.ExtSignalPulseRisingEdge);
                rtc6.LaserControlSignal = lcs;
            }
        }

        /// <summary>
        /// Configure Output Synchronization (sync timing for start of vector by D.IN1 pin)
        /// <para>
        /// 출력 동기화(혹은 스캐너 모션 외부 클럭 동기화) 기능 켜기
        /// DIGITAL IN1 핀으로 입력되는 외부 펄스 신호 타이밍을 사용해 스캐너의 위치를 맞춰주는 역할을 합니다.
        /// 작동 원리:
        /// 이 기능을 켜면 RTC5,6 보드는 DIGITAL IN1 핀으로 입력되는 외부 레이저 펄스 신호(마스터 클럭)를 모니터링합니다.
        /// 마킹 명령어(mark, arc, ellipse 등)가 시작될 때, 명령어의 시작 시점과 실제 첫 번째 레이저 펄스가 나오는 시점 사이의 시간 차이(위상 편이)를 계산하여 스캐너 거울의 출력 위치를 자동으로 보정합니다.
        /// 사용 목적:
        /// 프리 러닝 레이저 사용 시 레이저 펄스의 무작위한 위상 차이로 인해 선을 반복해서 그을 때 시작점이 들쭉날쭉해지는 현상(Jittery line images)을 방지하고,항상 일정한 위치에서 선이 시작되도록(flush line starts) 보장합니다.
        /// <para>
        /// </summary>
        void ConfigOutputSynchronization(IRtc rtc)
        {
            if (rtc is Rtc5 rtc5)
            {
                var lcs = rtc5.LaserControlSignal;
                lcs.Add(Rtc5LaserControlSignal.Bit.OutputSynchronization);
                rtc5.LaserControlSignal = lcs;
            }
            else if (rtc is Rtc6 rtc6)
            {
                var lcs = rtc6.LaserControlSignal;
                lcs.Add(Rtc6LaserControlSignal.Bit.OutputSynchronization);
                rtc6.LaserControlSignal = lcs;
            }
        }

        /// <summary>
        /// Configure LASER1 Synchronization (sync timing for LASER1 output signal by D.IN1 pin)
        /// <para>
        /// 펄스 동기화 모드(Pulse Synchronization) 기능 켜기 (RTC6 고유기능)
        /// 이 기능 역시 DIGITAL IN1 핀으로 입력되는 외부 펄스 신호를 사용합니다.
        /// 앞선 OutputSynchronization 스캐너의 움직임으로 동기화 하는 것과 달리, RTC6 보드가 내보내는 LASER1 펄스의 출력 타이밍 자체를 외부 클럭에 맞추는 기능입니다.
        /// LASER1 신호가 외부 클럭과 위상(Phase)까지 완벽하게 일치하게 나가도록 만듭니다.
        ///
        /// 내부에서 생성되는 개별 레이저 펄스(LASER1)의 출력 타이밍을 외부에서 들어오는 레이저의 클럭 신호에 정확하게 맞추기 위해 사용되는 기능입니다.
        /// 이 모드가 활성화되면, RTC6 보드는 지정된 LASER1 펄스를 즉시 내보내지 않고 DIGITAL IN1 핀을 통해 새로운 외부 클럭 신호 펄스가 감지될 때까지 출력을 지연(대기)시킵니다.
        /// 외부 신호가 감지되면 그에 맞춰 펄스를 출력하므로 외부 레이저 소스와의 완벽한 위상 동기화가 가능해집니다.
        /// 
        /// 주의사항: OutputSynchronization 와 함께 사용하는것은 금지됩니다. 2개중 하나의 기능만 사용하세요.
        /// </para>
        /// </summary>
        void ConfigLASER1Synchronization(Rtc6 rtc6)
        {
            rtc6.CtlLASER1Synchronization(true, 0);
        }

        private void BtnCreateEntities_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            document.ActNew();

            CreateEntities();

            EditPenValues();
        }
        void CreateEntities()
        {
            var document = siriusEditorControl1.Document;

            var rnd = new Random((int)DateTime.Now.Ticks);
            const int pointCounts = 10_000;
            var tempPoints = new List<DVec3>(pointCounts);
            for (int v = 0; v < pointCounts; v++)
            {
                double x = rnd.NextDouble() * 100.0 - 50.0;
                double y = rnd.NextDouble() * 100.0 - 50.0;
                double z = 0;
                tempPoints.Add(new DVec3(x, y, z));
            }

            var points = EntityFactory.CreatePoints(tempPoints);

            // Path optimizer for points (using TSP algorithm)
            // 경로 최적화 적용 (TSP 알고리즘을 이용한)
            points.Sort( EntityPoints.PointSorts.Global);

            var color = SpiralLab.Sirius3.UI.Config.EntityPenColors[0]; // Color.White
            points.ColorMode = EntityModelBase.ColorModes.Model;
            points.ModelColor = color.ToDVec3();

            document.ActivePage?.ActiveLayer?.AddChild(points);
            siriusEditorControl1.View?.DoRender();
        }

        void EditPenValues()
        {
            var document = siriusEditorControl1.Document;
            var marker = siriusEditorControl1.Marker;
            var rtc = siriusEditorControl1.Scanner as IRtc;

            // Find entity pen for 'White'
            // White 펜 개체 
            var entityPenColor = SpiralLab.Sirius3.UI.Config.EntityPenColors[0]; // Color.White
            document.FindByEntityPenColor(entityPenColor, out var entityPen);

            // Use Jump and shoot
            // 점프 and 가공 모드 사용
            entityPen.RasterMode = RasterModes.JumpAndShoot;

            // Pixel time: Max wait(or timeout) time
            // 최대 대기 시간(혹은 타임아웃 시간)
            entityPen.PixelTime = 100; // 100usec

            // If PixelPulses = 0,
            //   LASERON druing PixelTime (not used waiting edges from DIGITAL IN1 at LASER Connector)
            //   PixelTime 동안 LASERON 진행됨 (LASER 커넥터에 있는 DIGITAL IN1 핀 입력 대기 없음)
            //
            // If PixelPulses > 0,
            //   Counts of waiting signal edges from input DIGITAL IN1 at LASER Connector
            //   PixelTime 시간동안 대기하면서, LASER 커넥터에 있는 DIGITAL IN1 핀으로 들어오는 신호 엣지의 개수 만큼 동기화된 펄스를 출력.
            //   이 명령어가 실행되는 동안 스캐너의 위치는 이동하지 않고 제자리에 고정되며, 외부 펄스의 개수를 정확히 세어 점(Dot)을 가공하는 데 사용.
            //   PixelTime 시간 동안 외부 신호(LASER 커넥터의 DIGITAL IN1)의 엣지가 감지되는 순간 LASERON 신호가 켜집니다. 
            //   PixelTime 은 펄스를 대기하는 시간으로 사용되며, 시간내에 외부 펄스가 아예 들어오지 않으면 LASERON 신호는 켜지지 않으며,
            //   지정된 개수보다 더 많은 펄스가 들어오면 초과분은 무시됩니다. 
            //   지정된 펄스 개수가 들어오는 동안 레이저가 켜진 상태로 유지되지만, 최대 유지 시간은 PixelTime 설정 시간에 의해 제한됩니다. 
            //   외부 펄스 신호는 반드시 LASER 커넥터의 DIGITAL IN1 핀에 TTL 레벨로 공급되어야 하고, IRtcSignalLevel.CtlLaserControlSignal 을 이용해 상승(혹은 하강) 에지를 설정할 수 있습니다. 
            entityPen.PixelPulses = 5;


            // PixelPulses 사용시(0 보다 큰 경우) 목표한 외부 펄스 개수에 도달되는 즉시 대기를 종료하고 다음 명령어로 넘어가는 기능을 활성화 할지 여부입니다.
            // Break(or terminate) waiting if DIGITAL IN1 signal counts has reached PixelPulses
            entityPen.IsPixelPulsesExit = true;

            // Also, Enable hard jump or not
            // entityPen.IsHardJump = true;
            // 일반적인 점프(jump)나 마킹(mark) 명령어들이 목표점까지의 궤적을 10µs 단위의 여러 마이크로스텝(Microsteps)으로 쪼개어 부드럽게 이동하는 것과 달리,
            // Hard Jump 기능은 모든 점프를 궤적을 쪼개지 않고 단일 클럭 주기에 목표 위치로 직접 이동시킵니다. 
            // 마이크로스텝 분할 없이 사용자가 직접 위치를 제어하므로, 명령어를 연속 호출할 때 스캔 시스템의 동적 물리 한계(가속도, 속도 등)를 초과하지 않도록 궤적을 설계할 책임이 전적으로 사용자에게 있습니다. 
            // 주의사항: 일반적인 점프는 이동 후 설정된 ScannerJumpDelay 만큼 자동으로 대기하여 거울의 기계적 진동이 멈출 시간을 줍니다.
            // 하지만 IsHardJump 사용시 이전 명령어의 지연 시간은 기다리지만, 명령어 자체적으로는 새로운 스캐너 지연을 절대 발생시키지 않습니다.
            // 따라서 IsHardJump 에 의해 점프 이동 직후 곧바로 가공을 실행하면, 미러가 아직 목표 위치에 도달하지 못했거나 진동(Settling)이
            // 끝나지 않은 상태에서 레이저가 방출되어 위치가 어긋날 수 있습니다. 
            // 해결책: 미러가 완전히 정착할 수 있도록 LaserOnDelay, LaserOffDelay 지연값을 사용해 레이저 켜짐을 늦추는것을 활용해야 합니다.
            //entityPen.LaserOnDelay =
            //entityPen.LaserOffDelay =
        }

        private void BtnStartStop_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            var marker = siriusEditorControl1.Marker;
            var rtc = siriusEditorControl1.Scanner as IRtc;
          
            if (marker.IsBusy)
            {
                marker.Stop();
                marker.Reset();
            }
            else
            {
                marker.Reset();
                marker.Ready(siriusEditorControl1.Document);

                // Start to mark current page
                marker.Start(document.Page);
            }
        }
    }
}
