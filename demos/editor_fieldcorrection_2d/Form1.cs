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
using SpiralLab.Sirius3.UI.WinForms;

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
    /// 2D Field Correction demo
    /// 2D 필드 보정 데모
    /// </summary>
    public partial class Form1 : Form
    {
        // Grid counts (Rows x Cols)
        // 그리드 개수 (행 x 열)
        int rows = 5;
        int cols = 5;

        // Effective field size (mm)
        // 유효 가공 영역 크기 (mm)
        double fieldSize;

        // Interval between grids (mm)
        // 그리드 간격 (mm)
        double rowInterval;
        double colInterval;

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
                var dlgResult = System.Windows.Forms.MessageBox.Show(this, $"Do you really want to terminate program ?", "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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
            this.btnCreateGrids.Click += BtnCreateGrids_Click;
            this.btnCorrection2D.Click += BtnCorrection2D_Click;
            this.btnSelectTable.Click += BtnSelectTable_Click;
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
        /// Create calibration grids
        /// 보정용 그리드 생성
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCreateGrids_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            document.ActNew();

            // Approx. field area from config
            // 설정 파일에서 필드 영역 크기 가져오기
            int index = 0;
            var fov = NativeMethods.ReadIni<double>(EditorHelper.ConfigFileName, $"RTC{index}", "FOV", 100.0);
            fieldSize = fov * 0.9; // Reduce for effective field only / 유효 영역만을 위해 90%로 축소

            // Calculate grid parameters
            // 그리드 파라메터 계산
            rows = 5;
            cols = 5;
            rowInterval = (double)Math.Floor(fieldSize / (rows - 1) * 1000.0f) / 1000.0f;
            colInterval = (double)Math.Floor(fieldSize / (cols - 1) * 1000.0f) / 1000.0f;

            // Calculate grid boundaries
            // 그리드 경계 계산
            var left = -colInterval * (int)(cols / 2);
            var right = colInterval * (int)(cols / 2);
            var bottom = -rowInterval * (int)(rows / 2);
            var top = rowInterval * (int)(rows / 2);

            var entities = new List<IEntity>((int)(rows * cols));

            // Create horizontal lines
            // 수평선 생성
            for (int row = 0; row < rows; row++)
            {
                var start = new DVec2(left, (bottom + rowInterval * row));
                var end = new DVec2(right, (bottom + rowInterval * row));
                var line = EntityFactory.CreateLine(start, end);
                entities.Add(line);
            }
            // Create vertical lines
            // 수직선 생성
            for (int col = 0; col < cols; col++)
            {
                var start = new DVec2((left + colInterval * col), bottom);
                var end = new DVec2((left + colInterval * col), top);
                var line = EntityFactory.CreateLine(start, end);
                entities.Add(line);
            }

            // Create group of lines
            // 선들의 그룹 생성
            var group = EntityFactory.CreateMixedGroup($"{rows}x{cols} {rowInterval}x{colInterval}mm", entities);
            document?.ActAdd(group);
        }

        /// <summary>
        /// Perform 2D Field Correction
        /// 2D 필드 보정 수행
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCorrection2D_Click(object sender, EventArgs e)
        {
            var rtc = siriusEditorControl1.Scanner as IRtc;

            // Create RtcCorrection2D instance
            // RtcCorrection2D 인스턴스 생성
            var rtcCorrection2D = new RtcCorrection2D(rtc.KFactor, rows, cols, rowInterval, colInterval, rtc.CorrectionFiles[(int)rtc.PrimaryHeadTable].FileName, string.Empty);
            double left = -colInterval * (double)(int)(cols / 2);
            double top = rowInterval * (double)(int)(rows / 2);
            
            var rnd = new Random();
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    // Input dx, dy position error (simulated with random values)
                    // dx, dy 위치 오차 입력 (랜덤 값으로 시뮬레이션)
                    rtcCorrection2D.AddRelative(row, col,
                        
                        new DVec2(left + col * colInterval, top - row * rowInterval),
                        
                        // DVec2.Zero
                        new DVec2(
                            rnd.Next(20) / 1000.0 - 0.01,
                            rnd.Next(20) / 1000.0 - 0.01
                            )                        
                        );
                }
            }

            // Show 2D Correction form
            // 2D 보정 폼 표시
            var form = new Correction2DRtcForm(rtc, rtcCorrection2D);
            form.ShowDialog();
        }

        /// <summary>
        /// Select and load correction table (*.ct5, *.ctb)
        /// 보정 테이블 선택 및 로드 (*.ct5, *.ctb)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSelectTable_Click(object sender, EventArgs e)
        {
            var rtc = siriusEditorControl1.Scanner as IRtc;

            var dlg = new OpenFileDialog();
            dlg.Title = "Open Correction File";
            dlg.InitialDirectory = SpiralLab.Sirius3.Config.CorrectionPath;
            dlg.Filter = "ct5 correction file (*.ct5)|*.ct5|ctb correction file (*.ctb)|*.ctb|All Files (*.*)|*.*";
            dlg.DefaultExt = "ct5";

            DialogResult result = dlg.ShowDialog();
            if (result != DialogResult.OK)
                return;

            bool success = true;
            var currentTable = rtc.PrimaryHeadTable;
            // Load and select correction file
            // 보정 파일 로드 및 선택
            rtc.CtlLoadCorrectionFile(currentTable, dlg.FileName);
            rtc.CtlSelectCorrection(currentTable);
            if (success)
                System.Windows.Forms.MessageBox.Show(this, $"Target correction file is load/selected at Table1");
            else
                System.Windows.Forms.MessageBox.Show(this, $"Fail to load/select correction file");
        }

    }
}
