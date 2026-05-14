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
    /// Form1
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

            btnNormalJump.Click += BtnNormalJump_Click;
            btnHardJump.Click += BtnHardJump_Click;
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

            siriusEditorControl1.RegisterDevices(rtc, laser, powerMeter, dInExt1, dInLaserPort, dOutExt1, dOutExt2, dOutLaserPort, marker);

            marker.Ready(siriusEditorControl1.Document, siriusEditorControl1.View, rtc, laser, powerMeter);
        }
        private void CreateEntity()
        {
            var document = siriusEditorControl1.Document;
            var text = EntityFactory.CreateText("Arial",
                FontStyle.Regular,
                $"Aa{Environment.NewLine}01{Environment.NewLine}!@",
                10);
            text.FontHorizontalAlignment = StringAlignment.Center;
            text.FontVerticalAlignment = StringAlignment.Center;
            text.PenColor = Color.White;

            document.ActAdd(text);
        }
        private void BtnNormalJump_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            document.ActNew();

            // create measurement begin
            var begin = EntityFactory.CreateMeasurementBegin(
                10 * 1000,
                new MeasurementChannels[] {
                    MeasurementChannels.SampleX,
                    MeasurementChannels.SampleY,
                    MeasurementChannels.SampleZ,
                    MeasurementChannels.LaserOn,
                },
                "Normal Jump"
                );
            document.ActAdd(begin);

            CreateEntity();

            var end = EntityFactory.CreateMeasurementEnd();
            document.ActAdd(end);

            // Find entity pen for 'White'
            document.FindByEntityPenColor(System.Drawing.Color.White, out var entityPen);

            // Disable Hard Jump 
            entityPen.IsHardJump = false;
            
            siriusEditorControl1.PropertyGridCtrl.Refresh();
        }

        private void BtnHardJump_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            document.ActNew();

            // create measurement begin
            var begin = EntityFactory.CreateMeasurementBegin(
                10 * 1000,
                new MeasurementChannels[] {
                    MeasurementChannels.SampleX,
                    MeasurementChannels.SampleY,
                    MeasurementChannels.SampleZ,
                    MeasurementChannels.LaserOn,
                },
                "Hard Jump"
                );
            document.ActAdd(begin);

            CreateEntity();

            var end = EntityFactory.CreateMeasurementEnd();
            document.ActAdd(end);

            // Find entity pen for 'White'
            document.FindByEntityPenColor(System.Drawing.Color.White, out var entityPen);

            // Enable Hard Jump 
            entityPen.IsHardJump = true;

            siriusEditorControl1.PropertyGridCtrl.Refresh();
        }
    }
}
