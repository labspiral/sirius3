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

        int startingSerialNo = 1;

        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;

            this.btnCreateEntities.Click += BtnCreateEntities_Click;
            this.btnStartStop.Click += BtnStartStop_Click;
            this.btnStartEncoderSimulation.Click += BtnStartEncoderSimulation_Click;
            this.btnStopEncoderSimulation.Click += BtnStopEncoderSimulation_Click;
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            EditorHelper.CreateDevices(out IRtc rtc, out ILaser laser, out IDInput dInExt1, out IDInput dInLaserPort, out IDOutput dOutExt1, out IDOutput dOutExt2, out IDOutput dOutLaserPort, out IPowerMeter powerMeter, out IMarker marker);

            siriusEditorControl1.Scanner = rtc;

            siriusEditorControl1.Laser = laser;

            // Set D.IN0 name
            dInExt1.ChannelNames[0][0] = "External Trigger";

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

        private void BtnCreateEntities_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;

            document.ActNew();
            CreateEntities();
            CreateTextConvertEventHandler();
        }

        void CreateEntities()
        {
            var document = siriusEditorControl1.Document;
            var marker = siriusEditorControl1.Marker;
            var rtc = siriusEditorControl1.Scanner as IRtc;

            // Create waiting D.IN0 ("External Trigger")
            // Condition: rising edge
            var waitExt16Cond = EntityFactory.CreateWaitDataExt16EdgeCond(
                0, //D.IN0 ("External Trigger")
                SignalEdges.High);
            document.ActivePage?.ActiveLayer?.AddChild(waitExt16Cond);

            // Create mof begin 
            // with encoder reset
            var mofBegin = EntityFactory.CreateMoFBegin(RtcMoFModes.XY, true);
            document.ActAdd(mofBegin);

            // Create barcode
            var barcode = new EntityDataMatrix("0123456789", EntityBarcode2DBase.Barcode2DCells.Lines, 5, 10, 10);
            barcode.Name = "MyBarcode";
            barcode.IsAllowConvert = true; // marker.OnTextConvert event will be called

            document.ActivePage?.ActiveLayer?.AddChild(barcode);
          
            // Create text
            var text = new EntityText("Arial", FontStyle.Regular, "0123456789", 2);
            text.Name = "MyText";
            text.IsAllowConvert = true;  // marker.OnTextConvert event will be called

            text.Translate(0, -2.5);
            document.ActivePage?.ActiveLayer?.AddChild(text);

            // Create mof end
            var mofEnd = EntityFactory.CreateMoFEnd(DVec2.Zero);
            document.ActAdd(mofEnd);

            //// Create script event
            //// 'IScript.ListEvent' script function would be called whenever marker has started
            //// By external script file (marker.ScriptFile)
            //var scriptEvent = EntityFactory.CreateScriptEvent();
            //scriptEvent.Description = "Event for increase serial no after each marks";
            //document.ActAdd(scriptEvent);

            // Repeats 100 times
            document.ActivePage.ActiveLayer.Repeats = 100;
            // or infinitely


            //// Text convert by external script file
            //// Target entities should be set as IsConvertedText = true
            //marker.ScriptFile = Path.Combine(SpiralLab.Sirius2.Winforms.Config.ScriptPath, "mof_barcode.cs");
            //Debug.Assert(null != marker.ScriptInstance);

            document.ActRegen();

            Debug.Assert(rtc.IsMoF);
            var rtcMoF = rtc as IRtcMoF;
            Debug.Assert(rtcMoF != null);
            Debug.Assert(rtcMoF.EncXCountsPerMm != 0);
            //Debug.Assert(rtcMoF.EncYCountsPerMm != 0);
        }

        private void BtnStartStop_Click(object sender, EventArgs e)
        {
            var marker = siriusEditorControl1.Marker;

            if (marker.IsBusy)
            {
                marker.Stop();
                marker.Reset();
            }
            else
            {
                //startingSerialNo = 1;
                marker.Reset();
                marker.Start();
            }
        }

        private void BtnStartEncoderSimulation_Click(object sender, EventArgs e)
        {
            var rtc = siriusEditorControl1.Scanner as IRtc;

            Debug.Assert(rtc.IsMoF);
            var rtcMoF = rtc as IRtcMoF;

            // Activated simulated encoders for test purpose (x= -1, y=0 mm/s)
            // DO NOT set simulated encoder speed if ENC 0,1 has connected
            rtcMoF.CtlMofEncoderSpeed(-1, 0);
        }

        private void BtnStopEncoderSimulation_Click(object sender, EventArgs e)
        {
            var rtc = siriusEditorControl1.Scanner as IRtc;

            Debug.Assert(rtc.IsMoF);
            var rtcMoF = rtc as IRtcMoF;

            // Deactivated simulated encoders 
            rtcMoF.CtlMofEncoderSpeed(0, 0);

            // Reset encoders
            rtcMoF.CtlMofEncoderReset();
        }


        void CreateTextConvertEventHandler()
        {
            var marker = siriusEditorControl1.Marker;

            marker.OnTextConvert += (IMarker marker, ITextConvertible textConvertible) =>
            {
                var entity = textConvertible as IEntity;

                var currentLayer = marker.WorkingSet.Layer;
                var currentLayerIndex = marker.WorkingSet.LayerIndex;
                var currentEntity = marker.WorkingSet.Entity;
                var currentEntityIndex = marker.WorkingSet.EntityIndex;
                var currentOffset = marker.WorkingSet.Offset;
                var currentOffsetIndex = marker.WorkingSet.OffsetIndex;

                switch (currentEntity.Name)
                {
                    case "MyBarcode":
                        return $"Barcode {startingSerialNo++}";
                    case "MyText":
                        return $"Text {startingSerialNo++}";
                    default:
                        // Not modified
                        return textConvertible.SourceText;
                }
            };

        }
    }
}
