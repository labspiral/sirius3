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

            btnPrepare.Click += BtnPrepare_Click;
            btnPen.Click += BtnPen_Click;
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

        private void BtnPrepare_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            document.ActNew();

            // create measurement begin
            var begin = EntityFactory.CreateMeasurementBegin(
                10 * 1000, //10KHz
                new MeasurementChannels[] {
                    MeasurementChannels.LaserOn,
                    MeasurementChannels.SampleX,
                    MeasurementChannels.SampleY,
                    MeasurementChannels.PulseLength,
                },
                @"X,Y, LaserON, PulseLength(usec)"
                );
            document.ActivePage?.ActiveLayer?.AddChild(begin);


            double width = 50;
            double height = 40;

            // --------1------>.
            // |               |
            // 4               2
            // |               |
            // .<------3--------

            var top = EntityFactory.CreateLine(-width / 2.0, height / 2.0, width / 2.0, height / 2.0);
            top.Name = "Top";
            top.LineWidth = 3;
            top.PenColor = SpiralLab.Sirius3.UI.Config.EntityPenColors[0]; //White
            document.ActivePage?.ActiveLayer?.AddChild(top);

            var right = EntityFactory.CreateLine(width / 2.0, height / 2.0, width / 2.0, -height / 2.0);
            right.Name = "Right";
            right.LineWidth = 3;
            right.PenColor = SpiralLab.Sirius3.UI.Config.EntityPenColors[1]; //Yellow
            document.ActivePage?.ActiveLayer?.AddChild(right);

            var bottom = EntityFactory.CreateLine(width / 2.0, -height / 2.0, -width / 2.0, -height / 2.0);
            bottom.Name = "Bottom";
            bottom.LineWidth = 3;
            bottom.PenColor = SpiralLab.Sirius3.UI.Config.EntityPenColors[2]; //Orange
            document.ActivePage?.ActiveLayer?.AddChild(bottom);

            var left = EntityFactory.CreateLine(-width / 2.0, -height / 2.0, -width / 2.0, height / 2.0);
            left.Name = "Left";
            left.LineWidth = 3;
            left.PenColor = SpiralLab.Sirius3.UI.Config.EntityPenColors[3]; //Red
            document.ActivePage?.ActiveLayer?.AddChild(left);


            // create measurement end
            var end = EntityFactory.CreateMeasurementEnd();
            document.ActivePage?.ActiveLayer?.AddChild(end);

            siriusEditorControl1.View?.DoRender();
        }

        private void BtnPen_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            var laser = siriusEditorControl1.Laser;

            {
                var penColor = SpiralLab.Sirius3.UI.Config.EntityPenColors[0]; // White
                document.FindByEntityPenColor(penColor, out var entityPen);
                entityPen.Power = laser.MaxPowerWatt * 0.25;
                entityPen.JumpSpeed = 1_000; 
                entityPen.MarkSpeed = 100; 
            }

            {
                var penColor = SpiralLab.Sirius3.UI.Config.EntityPenColors[1]; //Yellow
                document.FindByEntityPenColor(penColor, out var entityPen);
                entityPen.Power = laser.MaxPowerWatt * 0.5;
                entityPen.JumpSpeed = 1_000; 
                entityPen.MarkSpeed = 500; 
            }


            {
                var penColor = SpiralLab.Sirius3.UI.Config.EntityPenColors[2]; //Orange
                document.FindByEntityPenColor(penColor, out var entityPen);
                entityPen.Power = laser.MaxPowerWatt * 0.75;
                entityPen.JumpSpeed = 1_000;
                entityPen.MarkSpeed = 1_000;
            }


            {
                var penColor = SpiralLab.Sirius3.UI.Config.EntityPenColors[3]; //Red
                document.FindByEntityPenColor(penColor, out var entityPen);
                entityPen.Power = laser.MaxPowerWatt * 1;
                entityPen.JumpSpeed = 1_000;
                entityPen.MarkSpeed = 2_000;
            }
        }
    }
}
