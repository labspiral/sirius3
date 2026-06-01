using System;
using System.Text;
using Microsoft.Extensions.Logging;

using SpiralLab.Sirius3.Document;
using SpiralLab.Sirius3.Scanner;
using SpiralLab.Sirius3.IO;
using SpiralLab.Sirius3.Scanner.Rtc;
using SpiralLab.Sirius3.PowerMeter;
using SpiralLab.Sirius3.Laser;
using SpiralLab.Sirius3.Marker;
using SpiralLab.Sirius3.Entity;
using SpiralLab.Sirius3.Entity.Hatch;
using SpiralLab.Sirius3.Mathematics;
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
    /// <summary>
    /// 3D Points Cloud calibration demo
    /// 3D 포인트 클라우드 보정 데모
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

            btnLoad3DModel.Click += BtnLoad3DModel_Click;
            btnGridCloud.Click += BtnGridCloud_Click;
            btnPointsCloudCalibrationAndApply.Click += BtnPointsCloudCalibrationAndApply_Click;
            btnRevertFieldCorrection.Click += BtnRevertFieldCorrection_Click;
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

            // Need to option for 3D
            Debug.Assert(rtc is IRtc3D);
            Debug.Assert(rtc.Is3D);

            var inputCtFileName = rtc.CorrectionFiles[(int)rtc.PrimaryHeadTable].FileName;
            Debug.Assert(inputCtFileName.Contains("D3_"));

            siriusEditorControl1.RegisterDevices(rtc, laser, powerMeter, dInExt1, dInLaserPort, dOutExt1, dOutExt2, dOutLaserPort, marker);

            marker.Ready(siriusEditorControl1.Document, siriusEditorControl1.View, rtc, laser, powerMeter);
        }
     
        /// <summary>
        /// Load 3D STL model
        /// 3D STL 모델 로드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLoad3DModel_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            document.ActNew();

            // Load sample STL file
            // 샘플 STL 파일 로드
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sample\\stl\\Nefertiti_face.stl");
            if (!File.Exists(fileName)) 
                return;

            // Import 3D mesh
            // 3D 메쉬 임포트
            document.ActImport(fileName, out var mesh);
            //or
            //EntityFactory.CreateMesh(fileName, out var mesh);
            //document.ActAdd(mesh);

            document.ActSelect(mesh);
        }

        /// <summary>
        /// Extract Grid Cloud (Points and Normals) from mesh
        /// 메쉬로부터 그리드 클라우드(포인트 및 법선) 추출
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnGridCloud_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            if (1 != document.Selected.Length)
                return;
            var entity = document.Selected[0];
            var mesh = entity as EntityMesh;
            if (null == mesh) 
                return;

            // Option 1: Extract pointscloud by each vertex
            // 방식 1: 각 정점(Vertex)을 기준으로 포인트 클라우드 추출
            //if (!document.ActPointCloud(mesh, -DVec3.UnitZ, out var rayOriginOffset, out List<DVec3> vertices)
            //    return;

            // Option 2: Extract pointscloud by using fixed grids interval
            // 방식 2: 고정된 그리드 간격을 사용하여 포인트 클라우드 추출
            // Smaller value cause performance drop during 'RtcCalibrationLibrary.PointsCloudCalibration'
            // 간격이 너무 작으면 보정 계산 시 성능이 저하될 수 있음
            const double interval = 1;// 0.3; 

            if (!document.ActGridCloud(mesh, interval, out DVec3[] vertices, out DVec3[] normals))
                return;

            // Create pointscloud as points entity with normals
            // 포인트 및 법선 정보를 포함한 포인트 엔티티 생성
            //var points = new EntityPoints(vertices);
            var points = new EntityPoints(vertices, normals);
            
            // Calculate dimensions and translate for visualization
            // 크기 계산 및 시각화를 위한 이동
            mesh.CalculateRealMinMax(out var realMin, out var realMax);
            double width = realMax.X - realMin.X;
            double height = realMax.Y - realMin.Y;
            points.Translate(0, -height, 0);

            // Not allow hit test for large point cloud
            // 대량의 포인트 클라우드에 대해 히트 테스트 비활성화
            points.IsAllowHitTest = false;

            // Add points entity to layer
            // 레이어에 포인트 엔티티 추가
            document.ActAdd(points);

            // Create and prepare entity for mark hover pointscloud
            // 포인트 클라우드 가공 확인을 위한 샘플 텍스트 생성
            var text = new EntitySiriusText("ocra.cxf", EntitySiriusText.LetterSpaces.Variable, 0.2, 0.5, 1, "AaBbGg 012", 10);
            text.Translate(0, -height, 0);
            document.ActAdd(text);

            document.ActSelect(points);
        }

        /// <summary>
        /// Perform 3D Points Cloud Calibration and apply to RTC
        /// 3D 포인트 클라우드 보정 수행 및 RTC 카드에 적용
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnPointsCloudCalibrationAndApply_Click(object sender, EventArgs e)
        {
            var rtc = siriusEditorControl1.Scanner as IRtc;
            Debug.Assert(rtc.Is3D); 

            var document = siriusEditorControl1.Document;
            if (1 != document.Selected.Length)
                return;
            var entity = document.Selected[0];
            var points = entity as EntityPoints;
            if (null == points)
                return;

            // Calculate vertices and normals with all transformations applied
            // 모든 변환이 적용된 정점 및 법선 계산
            if (!points.CalculateTransformedVerticesAndNormals(out var vertices, out var normals))
                return;

            // Prepare new correction file path
            // 새로운 보정 파일 경로 준비
            var inputCtFileName = rtc.CorrectionFiles[(int)rtc.PrimaryHeadTable].FileName;
            string dirName = Path.GetDirectoryName(inputCtFileName);
            string fileName = Path.GetFileNameWithoutExtension(inputCtFileName);
            var newCtFileName = Path.Combine(dirName, $"{fileName}_PointsCloud.ct5");
            try
            {
                Cursor = Cursors.WaitCursor;
                // Perform 3D Points Cloud Calibration
                // 3D 포인트 클라우드 보정 수행
                // Note: This is a heavy calculation process if too many points exist
                // 참고: 포인트가 많을 경우 계산량이 많은 무거운 작업입니다
                if (!RtcCalibrationLibrary.PointsCloudCalibration(vertices.ToArray(), inputCtFileName, null, newCtFileName, out var returnCode))
                    return;
            }
            finally
            {
                Cursor = Cursors.Default;
            }

            // Load and select the new 3D correction file
            // 생성된 3D 보정 파일을 로드하고 선택
            LoadAndSelectCorrectionFile(rtc, newCtFileName);
            // Remove calibration points from document after successful apply
            // 성공적으로 적용된 후 문서에서 보정용 포인트 제거
            document.ActRemove(points);
        }

        /// <summary>
        /// Revert to original field correction table
        /// 원본 필드 보정 테이블로 복구
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnRevertFieldCorrection_Click(object sender, EventArgs e)
        {
            var rtc = siriusEditorControl1.Scanner as IRtc;
            // Revert to default Table 1
            // 기본 Table 1로 복구
            RevertCorrectionFile(rtc);
        }

        /// <summary>
        /// Helper to load and select 3D correction file
        /// 보정 파일 로드 및 선택 헬퍼
        /// </summary>
        /// <param name="rtc"></param>
        /// <param name="newCtFileName"></param>
        /// <returns></returns>
        private bool LoadAndSelectCorrectionFile(IRtc rtc, string newCtFileName)
        {
            bool success = true;
            CorrectionTables targetTable = CorrectionTables.None;
            // Target table depends on RTC card version
            // 대상 테이블은 RTC 카드 버전에 따라 다름
            switch (rtc.RtcCard)
            {
                case RtcCards.Rtc5:
                    targetTable = CorrectionTables.Table4;
                    // Load and select new correction table
                    // 새로운 보정 테이블 로드 및 선택
                    success &= rtc.CtlLoadCorrectionFile(targetTable, newCtFileName);
                    success &= rtc.CtlSelectCorrection(targetTable, targetTable);
                    break;
                case RtcCards.Rtc6:
                    targetTable = CorrectionTables.Table8;
                    success &= rtc.CtlLoadCorrectionFile(targetTable, newCtFileName);
                    success &= rtc.CtlSelectCorrection(targetTable, targetTable);
                    break;
                default:
                    throw new InvalidOperationException();
            }
            var rtc3D = rtc as IRtc3D;
            if (success)
            {
                // Apply 3D coefficients and show result
                // 3D 계수 적용 및 결과 표시
                var coeff = rtc3D.CoeffABC;
                var stretchFactor = rtc3D.StretchFactor;
                MessageBox.Show(this, $"New 3D calibration has applied: {newCtFileName} at {targetTable}{Environment.NewLine}Coefficient A,B,C: {coeff}{Environment.NewLine}Stretch factor: {rtc3D.StretchFactor}", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
                MessageBox.Show(this, $"Fail to load and select 3D calibration: {newCtFileName} at {targetTable}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            return success;
        }

        /// <summary>
        /// Helper to revert to default Table 1
        /// 기본 Table 1로 복구 헬퍼
        /// </summary>
        /// <param name="rtc"></param>
        /// <returns></returns>
        private bool RevertCorrectionFile(IRtc rtc)
        {
            bool success = true;
            switch (rtc.RtcCard)
            {
                case RtcCards.Rtc5:
                case RtcCards.Rtc6:
                    // Revert to original Table 1
                    // 원본 Table 1로 복구
                    success &= rtc.CtlSelectCorrection(CorrectionTables.Table1);
                    break;
                default:
                    throw new InvalidOperationException();
            }
            if (success)
                MessageBox.Show(this, $"3D calibration has reset to original(or default) correction table", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return success;
        }
    }
}
