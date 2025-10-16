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
            btnEventHandler.Click += BtnEventHandler_Click;
            btnMarkPage1.Click += BtnMarkPage1_Click;
            btnMarkPage2.Click += BtnMarkPage2_Click;
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

            {
                document.ActivePage = document.DocumentData.Pages[0];
                var text = EntityFactory.CreateText("Arial",
                    FontStyle.Regular,
                    $"AaBbCcDdEeFfGg{Environment.NewLine}HhIiJjKkLlMmNn{Environment.NewLine}OoPpQqRrSsTt{Environment.NewLine}UuVvWwXxYyZz{Environment.NewLine}0123456789{Environment.NewLine}!@#$%^&*()-+<>",
                    10);
                text.FontHorizontalAlignment = StringAlignment.Center;
                text.FontVerticalAlignment = StringAlignment.Center;

                var index = 0; //0 means Color.White
                text.PenColor = SpiralLab.Sirius3.UI.Config.ScannerPenColors[index];

                document.ActivePage?.ActiveLayer?.AddChild(text);
            }
            {
                document.ActivePage = document.DocumentData.Pages[1];
                var text = EntityFactory.CreateText("Arial",
                    FontStyle.Regular,
                    $"AaBbCcDdEeFfGg{Environment.NewLine}HhIiJjKkLlMmNn{Environment.NewLine}OoPpQqRrSsTt{Environment.NewLine}UuVvWwXxYyZz{Environment.NewLine}0123456789{Environment.NewLine}!@#$%^&*()-+<>",
                    10);
                text.FontHorizontalAlignment = StringAlignment.Center;
                text.FontVerticalAlignment = StringAlignment.Center;

                var index = 1; //1 means Color.Yellow
                text.PenColor = SpiralLab.Sirius3.UI.Config.ScannerPenColors[index];

                document.ActivePage?.ActiveLayer?.AddChild(text);
            }
        }

        private void BtnEventHandler_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            var marker = siriusEditorControl1.Marker;

            marker.OnMarkLayerPen -= Marker_OnMarkLayerPen;
            marker.OnMarkLayerPen += Marker_OnMarkLayerPen;

            marker.OnMarkScannerPen -= Marker_OnMarkScannerPen;
            marker.OnMarkScannerPen += Marker_OnMarkScannerPen;
        }

        private void BtnMarkPage1_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            var marker = siriusEditorControl1.Marker;

            if (marker.IsBusy)
                return;

            marker.Reset();
            marker.Ready(document);
            marker.Page = DocumentPages.Page1;
            marker.Start();
        }

        private void BtnMarkPage2_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            var marker = siriusEditorControl1.Marker;

            if (marker.IsBusy)
                return;

            marker.Reset();
            marker.Ready(document);
            marker.Page = DocumentPages.Page2;
            marker.Start();
        }

        private bool Marker_OnMarkLayerPen(IMarker marker, EntityLayerPen pen)
        {
            // mark for each layer pen
            var rtc = marker.Rtc;
            Debug.Assert(null != rtc);
            var laser = marker.Laser;
            Debug.Assert(null != laser);

            var rtc2ndHead = rtc as IRtc2ndHead;
            var rtcExtension = rtc as IRtcExtension;
            var rtcSkywriting = rtc as IRtcSkyWriting;
            var rtcWobbel = rtc as IRtcWobbel;
            var rtcAlc = rtc as IRtcAutoLaserControl;
            var rtcMoF = rtc as IRtcMoF;
            var rtcSyncAxis = rtc as IRtcSyncAxis;

            bool success = true;
            // User defined data can be saved at 'pen.ExtensionData' 

            if (null != rtcAlc && pen.IsALC)
            {
                success &= rtcAlc.CtlAlcByPositionTable(pen.AlcByPositionTable);
                switch (pen.AlcSignal)
                {
                    case AutoLaserControlSignals.ExtDO16:
                    case AutoLaserControlSignals.ExtDO8:
                        success &= rtcAlc.CtlAlc<uint>(pen.AlcSignal, pen.AlcMode, (uint)pen.AlcPercentage100, (uint)pen.AlcMinValue, (uint)pen.AlcMaxValue);
                        break;
                    default:
                        success &= rtcAlc.CtlAlc<double>(pen.AlcSignal, pen.AlcMode, pen.AlcPercentage100, pen.AlcMinValue, pen.AlcMaxValue);
                        break;
                }
            }

            if (null != rtcSyncAxis)
            {
                switch (pen.MotionType)
                {
                    case MotionTypes.StageOnly:
                        success &= rtcSyncAxis.CtlMotionType(MotionTypes.StageOnly);
                        break;
                    case MotionTypes.ScannerOnly:
                        success &= rtcSyncAxis.CtlMotionType(MotionTypes.ScannerOnly);
                        break;
                    case MotionTypes.StageAndScanner:
                        success &= rtcSyncAxis.CtlMotionType(MotionTypes.StageAndScanner);
                        success &= rtcSyncAxis.CtlBandWidth(pen.BandWidth);
                        break;
                }
            }

            return success;
        }

        private bool Marker_OnMarkScannerPen(IMarker marker, EntityScannerPen pen)
        {
            // mark for each scanner pen
            var rtc = marker.Rtc;
            Debug.Assert(null != rtc);
            var laser = marker.Laser;
            Debug.Assert(null != laser);

            var rtcExtension = rtc as IRtcExtension;
            var rtcSkywriting = rtc as IRtcSkyWriting;
            var rtcWobbel = rtc as IRtcWobbel;
            var rtcSyncAxis = rtc as IRtcSyncAxis;

            bool success = true;
            if (null != laser)
            {
                if (laser is ILaserPowerControl laserPowerControl)
                {
                    switch (laser.PowerControlMethod)
                    {
                        case PowerControlMethods.Frequency:
                        case PowerControlMethods.DutyCycle:
                            success &= laserPowerControl.ListPower(pen.Power, pen.PowerMapCategory);
                            break;
                        default:
                            success &= laserPowerControl.ListPower(pen.Power, pen.PowerMapCategory);
                            success &= rtc.ListFrequency(pen.Frequency, pen.PulseWidth);
                            break;
                    }
                }
            }
            success &= rtc.ListDelay(pen.LaserOnDelay, pen.LaserOffDelay, pen.ScannerJumpDelay, pen.ScannerMarkDelay, pen.ScannerPolygonDelay);
            success &= rtc.ListSpeed(pen.JumpSpeed, pen.MarkSpeed);
            success &= rtc.ListFirstPulseKiller(pen.LaserFpk);
            if (null != rtcExtension)
            {
                success &= rtcExtension.ListQSwitchDelay(pen.LaserQSwitchDelay);
            }
            if (null != rtcSkywriting)
            {
                double cosineLimit = Math.Cos(SpiralLab.Sirius3.Mathematics.Helper.DegToRad(pen.AngularLimit));
                if (pen.IsSkyWritingEnabled)
                    success &= rtcSkywriting.ListSkyWritingBegin(pen.SkyWritingMode, pen.LaserOnShift, pen.TimeLag, pen.Prev, pen.Post, cosineLimit);
                else
                    success &= rtcSkywriting.ListSkyWritingEnd();
            }

            if (null != rtcWobbel)
            {
                if (pen.IsWobbelEnabled)
                    success &= rtcWobbel.ListWobbelBegin(pen.WobbelParallel, pen.WobbelPerpendicular, pen.WobbelFrequency, pen.WobbelShape);
                else
                    success &= rtcWobbel.ListWobbelEnd();
            }

            if (null != rtcSyncAxis)
            {
                if (pen.MinMarkSpeed > 0)
                    success &= rtcSyncAxis.ListSpeedMinMark(pen.MinMarkSpeed);
                if (pen.ApproxBlendLimit > 0)
                    success &= rtcSyncAxis.ListApproxBlendLimit(pen.ApproxBlendLimit);
            }

            return success;
        }
    }
}
