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
    /// Hatch entity demo
    /// 해치(Hatch) 엔티티 데모
    /// </summary>
    public partial class Form1 : Form
    {

        /// <summary>
        /// Form constructor
        /// 폼 생성자
        /// </summary>
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
            btnPrepare.Click += BtnPrepare_Click;
            btnAddHatch1.Click += BtnAddHatch1_Click;
            btnAddHatch2.Click += BtnAddHatch2_Click;
            btnAddHatch3.Click += BtnAddHatch3_Click;
            btnHatchOrder.Click += BtnHatchOrder_Click;
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
        /// Prepare test entities (Text)
        /// 테스트용 엔티티 준비 (텍스트)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnPrepare_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;            
            document.ActNew();

            // Create sample text
            // 샘플 텍스트 생성
            var text = new EntityText("Arial", 
                FontStyle.Regular, 
                $"AaBbCcDdEeFfGg{Environment.NewLine}HhIiJjKkLlMmNn{Environment.NewLine}OoPpQqRrSsTt{Environment.NewLine}UuVvWwXxYyZz{Environment.NewLine}0123456789{Environment.NewLine}!@#$%^&*()-+<>", 
                10);
            text.FontHorizontalAlignment = StringAlignment.Center;
            text.FontVerticalAlignment = StringAlignment.Center;
            
            // Add and select entity
            // 엔티티 추가 및 선택
            document.ActAdd(text);
            document.ActSelect(text);
        }

        /// <summary>
        /// Add Line Hatch (Horizontal)
        /// 라인 해치 추가 (수평 방향)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAddHatch1_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            if (1 != document.Selected.Length)
                return;
            var entity = document.Selected[0];
            var hatchable = entity as IHatchable;
            if (null == hatchable)
                return;

            var angle = 0; // 0 degree / 0 도
            var interval = 0.1; // 0.1mm interval / 0.1mm 간격

            // Create line hatch
            // 라인 해치 생성
            var hatch = HatchFactory.CreateLine(angle, interval);
            hatch.Exclude = 0.05; // Distance from edge / 외곽선으로부터의 거리
            hatch.Sort = HatchSorts.None;

            var index = 0; // 0 means Color.White / 0번: 흰색 펜
         
            hatch.ModelColor = SpiralLab.Sirius3.UI.Config.EntityPenColors[index].ToDVec3();
            // Add hatch to entity
            // 엔티티에 해치 추가
            hatchable.AddHatch(hatch);
            // or document.ActAddHatch(hatchable, hatch);

            // Regenerate hatch paths
            // 해치 경로 재생성
            document.ActRegen();
            siriusEditorControl1.View?.DoRender();
        }

        /// <summary>
        /// Add Line Hatch (Vertical)
        /// 라인 해치 추가 (수직 방향)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAddHatch2_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            if (1 != document.Selected.Length)
                return;
            var entity = document.Selected[0];
            var hatchable = entity as IHatchable;
            if (null == hatchable)
                return;

            var angle = 90; // 90 degree / 90 도
            var interval = 0.1; // 0.1mm interval / 0.1mm 간격

            // Create line hatch
            // 라인 해치 생성
            var hatch = HatchFactory.CreateLine(angle, interval);
            hatch.Exclude = 0.05;
            hatch.Sort = HatchSorts.None;

            var index = 1; // 1 means Color.Yellow / 1번: 노란색 펜
            hatch.ModelColor = SpiralLab.Sirius3.UI.Config.EntityPenColors[index].ToDVec3();
            hatchable.AddHatch(hatch);
            // or document.ActAddHatch(hatchable, hatch);
            document.ActRegen();

            siriusEditorControl1.View?.DoRender();
        }

        /// <summary>
        /// Add Polygon Hatch
        /// 폴리곤 해치 추가
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAddHatch3_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            if (1 != document.Selected.Length)
                return;
            var entity = document.Selected[0];
            var hatchable = entity as IHatchable;
            if (null == hatchable)
                return;

            var interval = 0.1; // 0.1mm interval / 0.1mm 간격

            // Create polygon hatch (concentric)
            // 폴리곤 해치 생성 (동심원 형태)
            var hatch = HatchFactory.CreatePolygon(interval);

            var index = 2; // 2 means Color.Orange / 2번: 오렌지색 펜
            hatch.ModelColor = SpiralLab.Sirius3.UI.Config.EntityPenColors[index].ToDVec3();
            hatchable.AddHatch(hatch);
            // or document.ActAddHatch(hatchable, hatch);
            document.ActRegen();

            siriusEditorControl1.View?.DoRender();
        }

        /// <summary>
        /// Switch Hatch Marking Order (First -> Last -> Only)
        /// 해치 마킹 순서 전환 (First -> Last -> Only)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnHatchOrder_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            if (1 != document.Selected.Length)
                return;
            var entity = document.Selected[0];
            var hatchable = entity as IHatchable;
            if (null == hatchable)
                return;

            // Toggle HatchMarkOption
            // 해치 마킹 옵션 토글
            switch(hatchable.HatchMarkOption)
            {
                case HatchMarkOptions.HatchLast:
                    hatchable.HatchMarkOption = HatchMarkOptions.HatchFirst;
                    break;
                case HatchMarkOptions.HatchFirst:
                    hatchable.HatchMarkOption = HatchMarkOptions.HatchOnly;
                    break;
                case HatchMarkOptions.HatchOnly:
                    hatchable.HatchMarkOption = HatchMarkOptions.HatchLast;
                    break;
            }

            siriusEditorControl1.View?.DoRender();
        }
    }
}
