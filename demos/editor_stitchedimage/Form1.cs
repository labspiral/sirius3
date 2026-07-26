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
    /// Stitched image demo
    /// 스티치 이미지 데모
    /// </summary>
    public partial class Form1 : Form
    {


        MyCamera myCamera = new MyCamera(0, "TestCamera");



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

            this.btnCreateStitchedImage.Click += BtnCreateStitchedImage_Click;
            this.btnGrabImage.Click += BtnGrabImage_Click;
            this.btnClear.Click += BtnClear_Click;
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


        private void BtnCreateStitchedImage_Click(object sender, EventArgs e)
        {
            var view = siriusEditorControl1.View;

            // Create stitched image entity
            // 스티치 이미지 개체 생성
            var stitched = new EntityStitchedImage(
                myCamera, 
                MyCamera.Rows, MyCamera.Cols,
                MyCamera.WidthPixels, MyCamera.HeightPixels,
                MyCamera.FovWidth, MyCamera.FovHeight);

            view.StitchedImage = stitched;
        }


        private void BtnGrabImage_Click(object sender, EventArgs e)
        {
            Debug.Assert(myCamera != null);

            var view = siriusEditorControl1.View;
            var scanner = siriusEditorControl1.Scanner;

            // Simulate continous grabbing an image from a camera 
            // 카메라 이미지 취득을 외부 파일 로딩으로 시뮬레이션해서 테스트
            myCamera.MoveAndGrabs(view.StitchedImage, scanner);
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            var view = siriusEditorControl1.View;
            Debug.Assert(view.StitchedImage != null);

            // Clear stitched image
            // 스티치 이미지 초기화
            view.StitchedImage.ClearImages();
        }

    }
}
