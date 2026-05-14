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
    /// RTC Interrupt (Break point) demo
    /// RTC 인터럽트(중단점) 데모
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

            // Check if RTC card supports IRtcInterrupt
            // RTC 카드가 IRtcInterrupt 인터페이스를 지원하는지 확인
            var rtcInterrupt = rtc as IRtcInterrupt;
            Debug.Assert(rtcInterrupt != null);
            
            // Attach interrupt event handler
            // 인터럽트 이벤트 핸들러 연결
            rtcInterrupt.OnInterrupt -= RtcInterrupt_OnInterrupt;
            rtcInterrupt.OnInterrupt += RtcInterrupt_OnInterrupt;

            // Register devices to control
            // 컨트롤에 장치 등록
            siriusEditorControl1.RegisterDevices(rtc, laser, powerMeter, dInExt1, dInLaserPort, dOutExt1, dOutExt2, dOutLaserPort, marker);

            // Create sample entities with break points
            // 중단점을 포함한 샘플 엔티티 생성
            CreateEntities(siriusEditorControl1.Document);

            // Ready marker
            // 마커 준비
            marker.Ready(siriusEditorControl1.Document, siriusEditorControl1.View, rtc, laser, powerMeter);
        }
    
        /// <summary>
        /// Create entities and insert break point control entities
        /// 엔티티 생성 및 중단점 제어 객체 삽입
        /// </summary>
        /// <param name="document"></param>
        private void CreateEntities(IDocument document)
        {
            var rnd = new Random((int)DateTime.Now.Ticks);

            for (int i = 0; i < 10; i++)
            {
                // Create random arc
                // 랜덤 원호 생성
                var arc = EntityFactory.CreateArc(new DVec3(0, 0, 0), 5, 0, 360);
                double tx = rnd.NextDouble() * 100.0 - 50.0;
                double ty = rnd.NextDouble() * 100.0 - 50.0;
                arc.Translate(tx, ty, 0);

                // Create break point using arc's ID
                // 원호의 ID를 사용하여 중단점(BreakPoint) 생성
                var breakPoint = EntityFactory.CreateBreakPoint(arc);
   
                // Add breakpoint entity before arc to pause execution
                // 가공 중단을 위해 원호 이전에 중단점 엔티티 추가
                document.ActAdd(breakPoint);

                // Add arc entity
                // 원호 엔티티 추가
                document.ActAdd(arc); 
            }
        }

        /// <summary>
        /// Event handler called when RTC list execution reaches a break point
        /// RTC 리스트 실행 중 중단점에 도달했을 때 호출되는 이벤트 핸들러
        /// </summary>
        /// <param name="rtcInterrupt"></param>
        /// <param name="waitID"></param>
        /// <returns></returns>
        private bool RtcInterrupt_OnInterrupt(IRtcInterrupt rtcInterrupt, long waitID)
        {
            // Note: RTC list execution is currently paused and waiting!
            // 참고: 현재 RTC 리스트 실행이 중단되어 대기 중인 상태입니다!

            var document = siriusEditorControl1.Document;
            // Find the entity associated with the waitID (Break point's assigned ID)
            // waitID(중단점에 할당된 ID)와 일치하는 엔티티 검색
            if (document.FindById(waitID, out var foundedEntity))
            {
                if (foundedEntity is EntityArc entityArc)
                { 
                    if (entityArc.CalcuateRealMinMax(out var min, out var max))
                    {
                        // Perform some external work before resuming marking
                        // 예: 가공 전 스테이지 이동이나 상태 확인 등 외부 작업 수행
                        var realCenter = (min + max) * 0.5;
                        Thread.Sleep(2_000); // Simulated delay / 작업 시뮬레이션을 위한 지연

                        // Return 'True' to resume list execution (rtcInterrupt.CtlResumePoint() is called internally)
                        // 'True' 리턴 시 리스트 실행을 재개합니다 (내부적으로 CtlResumePoint 호출됨)
                        return true; 
                    }
                }
            }
            // Return 'False' to keep paused or handle error
            // 'False' 리턴 시 계속 중단 상태를 유지하거나 오류 처리
            return false; 
        }

    }
}
