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
    /// Barcode and Text entity demo
    /// 바코드 및 텍스트 엔티티 데모
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

            this.btnCreateBarcode.Click += BtnCreateBarcode_Click;
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
        /// Create barcode and text entities
        /// 바코드 및 텍스트 엔티티 생성
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCreateBarcode_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            document.ActNew();

            // Create sample entities
            // 샘플 엔티티 생성
            CreateEntities();
        }

        /// <summary>
        /// Create entities (DataMatrix and SiriusText)
        /// 엔티티 생성 (데이터매트릭스 및 시리우스 텍스트)
        /// </summary>
        void CreateEntities()
        {
            var document = siriusEditorControl1.Document;

            // Create DataMatrix 2D Barcode
            // 데이터매트릭스 2D 바코드 생성
            {
                // Various barcode types are available:
                // 다양한 바코드 타입이 제공됩니다:
                
                // 1D Barcode (Code39, Code128, etc.)
                // 1D 바코드 (Code39, Code128 등)
                //var entity = new EntityBarcode1D("0123456789", EntityBarcode1D.Barcode1DFormats.Code39, 50, 10);
                //entity.DotFactor = 5;

                // DataMatrix (Lines, Circles, Squares, Dots, etc.)
                // 데이터매트릭스 (선, 원, 사각형, 점 등)
                //var entity = new EntityDataMatrix("0123456789", EntityBarcode2DBase.Barcode2DCells.Lines, 10, 10);
                //entity.CellLine.DotFactor = 5;
                //entity.CellLine.Direction = CellLine.LineDirections.Horizontal;
                //entity.CellLine.IsZigZag = true;

                // QRCode
                // QRCode 생성
                //var entity = new EntityQRCode("0123456789", EntityBarcode2DBase.Barcode2DCells.Lines, 10, 10);
                //entity.CellLine.DotFactor = 5;
                //entity.CellLine.Direction = CellLine.LineDirections.Horizontal;
                //entity.CellLine.IsZigZag = true;

                // DataMatrix (Circles)
                // 데이터매트릭스 (원형 셀)
                //var entity = new EntityDataMatrix("0123456789", EntityBarcode2DBase.Barcode2DCells.Circles, 10, 10);
                //entity.CellCircle.DotFactor = 1;
                //entity.CellCircle.RadiusFactor = 0.95;
                //entity.CellCircle.IsZigZag = true;

                // DataMatrix (Squares)
                // 데이터매트릭스 (사각형 셀)
                //var entity = new EntityDataMatrix("0123456789", EntityBarcode2DBase.Barcode2DCells.Squares, 10, 10);
                //entity.CellSquare.DotFactor = 1;
                //entity.CellSquare.ScaleFactor = 0.95;
                //entity.CellSquare.IsZigZag = true;

                // DataMatrix (Dots)
                // 데이터매트릭스 (점형 셀)
                var entity = EntityFactory.CreateDataMatrix("0123456789", EntityBarcode2DBase.Barcode2DCells.Dots, 10, 10);
                entity.CellDot.DotFactor = 2;
                entity.IsReversed = true;

                // PDF417 with Hatch
                // PDF417 및 해치 적용
                //var entity = new EntityPDF417("0123456789", EntityBarcode2DBase.Barcode2DCells.Outline, 10, 10);
                //var hatch = HatchFactory.CreateLine(0, 0.02);
                //hatch.Joint = HatchJoints.Miter;
                //hatch.Exclude = 0.1;
                //hatch.IsZigZag = true;
                //hatch.Sort = HatchSorts.Global; //slow calculation but mark time optimized
                //entity.AddHatch(hatch);
                //entity.HatchMarkOption = HatchMarkOptions.HatchFirst;

                entity.Name = "MyBarcode";
                entity.IsAllowConvert = true;

                entity.Translate(0, -10);
                document.ActAdd(entity);
            }

            // Create Text entity
            // 텍스트 엔티티 생성
            {
                // Normal GDI Text
                // 일반 GDI 텍스트
                //var entity = new EntityText("Arial", FontStyle.Regular, "0123456789", 2);
                
                // Sirius Text (Custom Font .cxf, .lff)
                // 시리우스 텍스트 (사용자 정의 폰트 .cxf, .lff)
                var entity = new EntitySiriusText("ocra.cxf",  EntitySiriusText.LetterSpaces.Variable, 0.2, 0.5, 1, "0123456789", 2);

                entity.Name = "MyText";
                entity.IsAllowConvert = true;
                
                // Add hatch for outline types
                // 외곽선 타입에 대한 해치 추가
                // allow to hatch for cell types : outline, circle, square 
                var hatch = HatchFactory.CreateLine(90, 0.1);
                hatch.Joint = HatchJoints.Miter;
                hatch.Exclude = 0.05;
                hatch.IsZigZag = true;
                hatch.Order = HatchOrders.Descending;
                hatch.Sort = HatchSorts.Nearest; 
                entity.HatchMarkOption = HatchMarkOptions.HatchFirst;
                entity.AddHatch(hatch);

                entity.Translate(0, -12.5);
                document.ActAdd(entity);
            }
            siriusEditorControl1.View?.DoRender();
        }
       
    }
}
