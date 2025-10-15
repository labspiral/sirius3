using System;

using SpiralLab.Sirius3.Document;
using SpiralLab.Sirius3.Scanner;
using SpiralLab.Sirius3.IO;
using SpiralLab.Sirius3.Scanner.Rtc;
using SpiralLab.Sirius3.PowerMeter;
using SpiralLab.Sirius3.Laser;
using SpiralLab.Sirius3.Marker;
using SpiralLab.Sirius3.Motion;
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
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;

            btnPrepare.Click += BtnPrepare_Click;
            btnAddHatch1.Click += BtnAddHatch1_Click;
            btnAddHatch2.Click += BtnAddHatch2_Click;
            btnAddHatch3.Click += BtnAddHatch3_Click;
            btnHatchOrder.Click += BtnHatchOrder_Click;
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

            var stage = StageFactory.CreateVirtual(0);
            stage.Initialize();
            siriusEditorControl1.Stage = stage;

            marker.Ready(siriusEditorControl1.Document, siriusEditorControl1.EditorCtrl.View, rtc, laser, powerMeter, stage);
        }

        private void BtnPrepare_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            
            document.ActNew();
            var text = new EntityText("Arial", 
                FontStyle.Regular, 
                $"AaBbCcDdEeFfGg{Environment.NewLine}HhIiJjKkLlMmNn{Environment.NewLine}OoPpQqRrSsTt{Environment.NewLine}UuVvWwXxYyZz{Environment.NewLine}0123456789{Environment.NewLine}!@#$%^&*()-+<>", 
                10);
            text.FontHorizontalAlignment = StringAlignment.Center;
            text.FontVerticalAlignment = StringAlignment.Center;
            document.ActivePage?.ActiveLayer?.AddChild(text);
            document.ActSelect(text);
        }


        private void BtnAddHatch1_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            if (1 != document.Selected.Length)
                return;
            var entity = document.Selected[0];
            var hatchable = entity as IHatchable;
            if (null == hatchable)
                return;

            var angle = 0;
            var interval = 0.1;

            var hatch = HatchFactory.CreateLine(angle, interval);
            hatch.Exclude = 0.05;
            hatch.Sort = HatchSorts.Line;

            var index = 0; //0 means Color.White
         
            hatch.ModelColor = SpiralLab.Sirius3.UI.Config.ScannerPenColors[index].ToDVec3();
            hatchable.AddHatch(hatch);

            document.ActRegen();

            siriusEditorControl1.View?.DoRender();
        }

        private void BtnAddHatch2_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            if (1 != document.Selected.Length)
                return;
            var entity = document.Selected[0];
            var hatchable = entity as IHatchable;
            if (null == hatchable)
                return;

            var angle = 90;
            var interval = 0.1;

            var hatch = HatchFactory.CreateLine(angle, interval);
            hatch.Exclude = 0.05;
            hatch.Sort = HatchSorts.Line;

            var index = 1; //1 means Color.Yellow
            hatch.ModelColor = SpiralLab.Sirius3.UI.Config.ScannerPenColors[index].ToDVec3();
            hatchable.AddHatch(hatch);

            document.ActRegen();

            siriusEditorControl1.View?.DoRender();
        }

        private void BtnAddHatch3_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            if (1 != document.Selected.Length)
                return;
            var entity = document.Selected[0];
            var hatchable = entity as IHatchable;
            if (null == hatchable)
                return;

            var interval = 0.1;

            var hatch = HatchFactory.CreatePolygon(interval);

            var index = 2; //2 means Color.Orange
            hatch.ModelColor = SpiralLab.Sirius3.UI.Config.ScannerPenColors[index].ToDVec3();
            hatchable.AddHatch(hatch);

            document.ActRegen();

            siriusEditorControl1.View?.DoRender();
        }

        private void BtnHatchOrder_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            if (1 != document.Selected.Length)
                return;
            var entity = document.Selected[0];
            var hatchable = entity as IHatchable;
            if (null == hatchable)
                return;

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
