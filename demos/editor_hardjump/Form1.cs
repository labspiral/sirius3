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

            btnNormalJump.Click += BtnNormalJump_Click;
            btnHardJump.Click += BtnHardJump_Click;
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

            document.ActivePage.ActiveLayer.AddChild(text);
            siriusEditorControl1.View?.DoRender();
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
            document.ActivePage.ActiveLayer.AddChild(begin);

            CreateEntity();

            var end = EntityFactory.CreateMeasurementEnd();
            document.ActivePage.ActiveLayer.AddChild(end);

            // Find entity pen for 'White'
            document.FindByEntityPenColor(System.Drawing.Color.White, out var entityPenWhite);

            // Disable Hard Jump 
            entityPenWhite.IsHardJump = false;
            
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
            document.ActivePage.ActiveLayer.AddChild(begin);

            CreateEntity();

            var end = EntityFactory.CreateMeasurementEnd();
            document.ActivePage.ActiveLayer.AddChild(end);

            // Find entity pen for 'White'
            document.FindByEntityPenColor(System.Drawing.Color.White, out var entityPenWhite);

            // Disable Hard Jump 
            entityPenWhite.IsHardJump = true;

            siriusEditorControl1.PropertyGridCtrl.Refresh();
        }

    }
}
