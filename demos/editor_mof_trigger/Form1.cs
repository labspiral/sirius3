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
        const int maxSerialNo = 10;
        int startingSerialNo = 1;
        int currentSerialNo = 1;

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

            marker.Ready(siriusEditorControl1.Document, siriusEditorControl1.View, rtc, laser, powerMeter);
        }

        private void BtnCreateEntities_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            document.ActNew();

            CreateEntities();
            CreateEventHandlers();
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
            document.ActivePage?.ActiveLayer?.AddChild(mofBegin);

            // Create barcode
            var barcode = EntityFactory.CreateDataMatrix("0123456789", EntityBarcode2DBase.Barcode2DCells.Lines, 5, 10, 10);
            barcode.Name = "MyBarcode";
            barcode.IsAllowConvert = true; // marker.OnTextConvert event will be called

            document.ActivePage?.ActiveLayer?.AddChild(barcode);
          
            // Create text
            var text = EntityFactory.CreateText("Arial", FontStyle.Regular, "0123456789", 2);
            text.Name = "MyText";
            text.IsAllowConvert = true;  // marker.OnTextConvert event will be called

            text.Translate(0, -2.5);
            document.ActivePage?.ActiveLayer?.AddChild(text);

            // Create mof end
            var mofEnd = EntityFactory.CreateMoFEnd(DVec2.Zero);
            document.ActivePage?.ActiveLayer?.AddChild(mofEnd);

            // Create user event
            var userEvent = EntityFactory.CreateUserEvent();
            document.ActivePage?.ActiveLayer?.AddChild(mofEnd);

            siriusEditorControl1.View?.DoRender();
            
            // Repeats 100 times
            document.ActivePage.ActiveLayer.Repeats = 100;


            Debug.Assert(rtc.IsMoF);
            var rtcMoF = rtc as IRtcMoF;
            Debug.Assert(rtcMoF != null);
            Debug.Assert(rtcMoF.EncXCountsPerMm != 0);
            //Debug.Assert(rtcMoF.EncYCountsPerMm != 0);
        }

        private void BtnStartStop_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            var marker = siriusEditorControl1.Marker;

            if (marker.IsBusy)
            {
                marker.Stop();
                marker.Reset();
            }
            else
            {
                currentSerialNo = startingSerialNo;

                marker.Reset();
                marker.Ready(siriusEditorControl1.Document);
                var pageIndex = document.ActivePage.Index;
                marker.Start((DocumentPages)pageIndex);
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

        void CreateEventHandlers()
        {
            var marker = siriusEditorControl1.Marker;
            
            marker.OnTextConvert -= Marker_OnTextConvert;
            marker.OnTextConvert += Marker_OnTextConvert;

            marker.OnUserEvent -= Marker_OnUserEvent;
            marker.OnUserEvent += Marker_OnUserEvent;
        }

        private string Marker_OnTextConvert(IMarker marker, ITextConvertible textConvertible)
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
                    return $"Barcode {currentSerialNo}";
                case "MyText":
                    return $"Text {currentSerialNo}";
                default:
                    // Not modified
                    return textConvertible.SourceText;
            }
        }

        private bool Marker_OnUserEvent(IMarker marker, EntityUserEvent entityUserEvent)
        {
            currentSerialNo++;
            if (startingSerialNo > maxSerialNo)
                currentSerialNo = startingSerialNo;

            return true;
        }
    }
}
