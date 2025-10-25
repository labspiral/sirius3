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
    public partial class Form1 : Form
    {
        int rows = 5;
        int cols = 5;
        double fieldSize;
        double rowInterval;
        double colInterval;

        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
            this.btnCreateGrids.Click += BtnCreateGrids_Click;
            this.btnCorrection2D.Click += BtnCorrection2D_Click;
            this.btnSelectTable.Click += BtnSelectTable_Click;
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

        private void BtnCreateGrids_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            document.ActNew();

            // approx. field area 
            int index = 0;
            var fov = NativeMethods.ReadIni<double>(EditorHelper.ConfigFileName, $"RTC{index}", "FOV", 100.0);
            fieldSize = fov * 0.9; //reduce for effective field only

            //create grids
            rows = 5;
            cols = 5;
            rowInterval = (double)Math.Floor(fieldSize / (rows - 1) * 1000.0f) / 1000.0f;
            colInterval = (double)Math.Floor(fieldSize / (cols - 1) * 1000.0f) / 1000.0f;

            var left = -colInterval * (int)(cols / 2);
            var right = colInterval * (int)(cols / 2);
            var bottom = -rowInterval * (int)(rows / 2);
            var top = rowInterval * (int)(rows / 2);

            var entities = new List<IEntity>((int)(rows * cols));

            for (int row = 0; row < rows; row++)
            {
                var start = new DVec2(left, (bottom + rowInterval * row));
                var end = new DVec2(right, (bottom + rowInterval * row));
                var line = EntityFactory.CreateLine(start, end);
                entities.Add(line);
            }
            for (int col = 0; col < cols; col++)
            {
                var start = new DVec2((left + colInterval * col), bottom);
                var end = new DVec2((left + colInterval * col), top);
                var line = EntityFactory.CreateLine(start, end);
                entities.Add(line);
            }

            var group = EntityFactory.CreateGroup($"{rows}x{cols} {rowInterval}x{colInterval}mm", entities);
            document?.ActivePage?.ActiveLayer?.AddChild(group);
            siriusEditorControl1.View?.DoRender();
        }

        private void BtnCorrection2D_Click(object sender, EventArgs e)
        {
            var rtc = siriusEditorControl1.Scanner as IRtc;

            var rtcCorrection2D = new RtcCorrection2D(rtc.KFactor, rows, cols, rowInterval, colInterval, rtc.CorrectionFiles[(int)rtc.PrimaryHeadTable].FileName, string.Empty);
            double left = -colInterval * (double)(int)(cols / 2);
            double top = rowInterval * (double)(int)(rows / 2);
            
            var rand = new Random();
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    // input dx, dy position error
                    rtcCorrection2D.AddRelative(row, col,
                        
                        new DVec2(left + col * colInterval, top - row * rowInterval),
                        
                        //DVec2.Zero
                        new DVec2(
                            rand.Next(20) / 1000.0 - 0.01,
                            rand.Next(20) / 1000.0 - 0.01
                            )                        
                        );
                }
            }

            var form = new RtcCorrection2DForm(rtc, rtcCorrection2D);
            form.ShowDialog();
        }

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
            rtc.CtlLoadCorrectionFile(currentTable, dlg.FileName);
            rtc.CtlSelectCorrection(currentTable);
            if (success)
                System.Windows.Forms.MessageBox.Show($"Target correction file is load/selected at Table1");
            else
                System.Windows.Forms.MessageBox.Show($"Fail to load/select correction file");
        }

    }
}
