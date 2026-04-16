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
            this.FormClosing += (s, e) =>
            {
                var dlgResult = MessageBox.Show(this, $"Do you really want to terminate program ?", "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dlgResult != DialogResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }
                // Dispose instances 
                siriusEditorControl1.DisposeDevices();


                // Dispose document
                var doc = siriusEditorControl1.Document;
                siriusEditorControl1.Document = null;
                doc?.Dispose();

                // Clean up SIRIUS3 library
                SpiralLab.Sirius3.Core.Cleanup();
            };

            btnPrepare.Click += BtnPrepare_Click;
            btnPen.Click += BtnPen_Click;

            // Override default entity pen values
            SpiralLab.Sirius3.UI.Config.OnCreateEntityPen += Config_OnCreateEntityPen;
            // Override default layer pen values
            SpiralLab.Sirius3.UI.Config.OnCreateLayerPen += Config_OnCreateLayerPen;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            EditorHelper.CreateDevices(out IRtc rtc, out ILaser laser, out IDInput dInExt1, out IDInput dInLaserPort, out IDOutput dOutExt1, out IDOutput dOutExt2, out IDOutput dOutLaserPort, out IPowerMeter powerMeter, out IMarker marker);

            siriusEditorControl1.RegisterDevices(rtc, laser, powerMeter, dInExt1, dInLaserPort, dOutExt1, dOutExt2, dOutLaserPort, marker);

            marker.Ready(siriusEditorControl1.Document, siriusEditorControl1.View, rtc, laser, powerMeter);
        }

        private EntityLayerPen Config_OnCreateLayerPen(IDocument document, System.Drawing.Color color)
        {
            EntityLayerPen pen = new EntityLayerPen();

            pen.Name = color.ToKnownColor().ToString();
            pen.PenColor = color; //Config.EntityPenColors[0]; //default 'white'
            pen.Description = color.ToString();

            pen.IsALC = false;
            pen.AlcSignal = AutoLaserControlSignals.Disabled;
            pen.AlcMode = AutoLaserControlModes.Disabled;
            pen.AlcModeExtension = AutoLaserControlModeExtensions.Empty;
            pen.AlcPercentage100 = 0;
            pen.AlcMinValue = 0;
            pen.AlcMaxValue = 0;
            pen.AlcByPositionTable.Clear();

            pen.IsSkyWritingEnabled = false;
            pen.SkyWritingMode = SkyWritingModes.Mode3;
            pen.TimeLag = 250;
            pen.LaserOnShift = 0;
            pen.Prev = 2000 * 0.15;
            pen.Post = 2000 * 0.1;
            pen.AngularLimit = 90;

            pen.MotionType = MotionTypes.StageAndScanner;
            pen.BandWidth = 2;

            pen.IsVariablePolygonDelay = true;
            pen.VariablePolygonDelayEdgeLevel = 150; // < PolygonDelay * 2(or 1.5)

            pen.IsVariableJumpDelay = false;
            pen.VariableJumpDelayMin = 50;
            pen.VariableJumpDelayLimitLength = 0.1;

            return pen;
        }

        private EntityPen Config_OnCreateEntityPen(IDocument document, System.Drawing.Color color)
        {
            EntityPen pen = new EntityPen();

            pen.Name = color.ToKnownColor().ToString();
            pen.PenColor = color; //Config.EntityPenColors[0]; //default 'white'
            pen.Description = color.ToString();

            pen.Power = 1;
            pen.Frequency = 50 * 1000;
            pen.PulseWidth = 2;

            pen.LaserOnDelay = 0;
            pen.LaserOffDelay = 0;
            pen.ScannerJumpDelay = 250;
            pen.ScannerMarkDelay = 150;
            pen.ScannerPolygonDelay = 100;

            pen.JumpSpeed = 500; //syncAXIS?
            pen.MarkSpeed = 500;
            pen.IsHardJump = false;

            pen.RasterMode = RasterModes.JumpAndShoot;
            pen.RasterDirection = EntityPen.RasterDirections.Horizontal;
            pen.IsRasterZigZag = true;
            pen.PixelTime = 100;
            pen.PixelPulses = 0;
            pen.IsPixelPulsesExit = true;
            pen.PixelPeriod = 200;
            pen.PixelChannel = ExtensionChannels.ExtAO2;

            pen.LaserOnShiftSCANa = 0;
            pen.LaserOffShiftSCANa = 0;
            pen.CornerScaleSCANa = 100;
            pen.EndScaleSCANa = 100;
            pen.AccScaleSCANa = 100;
            pen.SpotDistanceSCANa = 0.02;

            pen.IsWobbelEnabled = false;
            pen.WobbelFrequency = 100;
            pen.WobbelPerpendicular = 0.5;
            pen.WobbelParallel = 0.5;
            pen.WobbelShape = WobbelShapes.Ellipse;

            pen.MinMarkSpeed = 0;
            pen.ApproxBlendLimit = 0;

            return pen;
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
