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
                // Dispose document
                var doc = siriusEditorControl1.Document;
                siriusEditorControl1.Document = null;
                doc?.Dispose();

                // Dispose instances 
                siriusEditorControl1.DisposeDevices();

                // Clean up SIRIUS3 library
                SpiralLab.Sirius3.Core.Cleanup();
            };

            btnHome.Click += BtnHome_Click;
            btnMove.Click += BtnMove_Click;
            btnStop.Click += BtnStop_Click;
            btnPrepare.Click += BtnPrepare_Click;
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            EditorHelper.CreateDevices(out IRtc rtc, out ILaser laser, out IDInput dInExt1, out IDInput dInLaserPort, out IDOutput dOutExt1, out IDOutput dOutExt2, out IDOutput dOutLaserPort, out IPowerMeter powerMeter, out IMarker marker);

            ConfigStepperMotor(rtc);

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

        private void ConfigStepperMotor(IRtc rtc)
        {
            // Must be support STEPPER PORT at RTC card
            // RTC5, RTC6 only
            var rtcStepper = rtc as IRtcStepper;
            Debug.Assert(rtcStepper != null);

            rtcStepper.StepperScaleUnits = new StepperUnits[2]
            {
                StepperUnits.MilliMeter,
                StepperUnits.MilliMeter,
            };

            //// for Rotary motor
            //rtcStepper.StepperScaleUnits = new StepperUnits[2]
            //{
            //    StepperUnits.Degree,
            //    StepperUnits.Degree,
            //};

            rtcStepper.StepperScaleFactors = new double[2]
            {
                4000, // 4000 steps/mm
                4000, // 4000 steps/mm
            };

            //// for Rotary motor
            //rtcStepper.StepperScaleFactors = new double[2]
            //{
            //    3600, // 3600 steps/rev
            //    3600, // 3600 steps/rev
            //};

            rtcStepper.OnStepperInitialized -= RtcStepper_OnStepperInitialized;
            rtcStepper.OnStepperInitialized += RtcStepper_OnStepperInitialized;
        }

        private void RtcStepper_OnStepperInitialized(IRtcStepper rtcStepper, uint motorNo, bool initialized)
        {
            if (initialized) 
                MessageBox.Show(this, "Complete to Search Home", "Stepper", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show(this, "Fail to Search Home", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void BtnHome_Click(object sender, EventArgs e)
        {
            var rtcStepper = siriusEditorControl1.Scanner as IRtcStepper;
            btnHome.Enabled = btnMove.Enabled  = false;

            Task.Run(() =>
            {
                double vel1 = 10;
                double vel2 = 1;
                double tol1 = 10;
                double tol2 = 1;
                double pos1 = 0; // mm or deg

                var timeOutSec = SpiralLab.Sirius3.UI.Config.StepperReferenceRunTimeOut; // default: 30s

                try
                {
                    rtcStepper.CtlStepperReferenceRun(StepperAxes.Axis1, StepperDirections.Negativity, vel1, tol1, vel2, tol2, pos1, timeOutSec);
                    
                }
                finally
                {
                    if (!this.IsDisposed && this.IsHandleCreated)
                    {
                        this.BeginInvoke(new MethodInvoker(delegate ()
                        {
                            btnHome.Enabled = btnMove.Enabled = true;
                        }));
                    }
                }
            });
        }

        private void BtnMove_Click(object sender, EventArgs e)
        {
            var rtcStepper = siriusEditorControl1.Scanner as IRtcStepper;

            double vel = 10;
            StepperMoveTypes moveType = StepperMoveTypes.Absolute;
            double pos = 10; //mm or deg

            rtcStepper.CtlStepperVelocity(vel, -1);
            rtcStepper.CtlStepperMove(StepperAxes.Axis1, moveType, pos, 0);
        }

        private void BtnStop_Click(object sender, EventArgs e)
        {
            var rtcStepper = siriusEditorControl1.Scanner as IRtcStepper;
            rtcStepper.CtlStepperMoveStop(StepperCombinedAxes.Axis1);
        }


        private void BtnPrepare_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            var marker = siriusEditorControl1.Marker;
            var rtcStepper = siriusEditorControl1.Scanner as IRtcStepper;

            //siriusEditorControl1.Document.ActNew();

            {
                double pos = 10; //mm or deg
                double vel = 5; // mm/s or deg/s
                var stepperMove = EntityFactory.CreateStepperMove(StepperAxes.Axis1, StepperMoveTypes.Absolute, pos, vel);
                document.ActivePage?.ActiveLayer?.AddChild(stepperMove);

                var stepperWait = EntityFactory.CreateStepperWait(StepperCombinedAxes.Axis1, 2_000);
                document.ActivePage?.ActiveLayer?.AddChild(stepperWait);

                var rect = EntityFactory.CreateRectangle(0, 0, 10, 10);
                document.ActivePage?.ActiveLayer?.AddChild(rect);
            }

            {
                double pos = 20; //mm or deg
                double vel = 5; // mm/s or deg/s
                var stepperMove = EntityFactory.CreateStepperMove(StepperAxes.Axis1, StepperMoveTypes.Absolute, pos, vel);
                document.ActivePage?.ActiveLayer?.AddChild(stepperMove);

                var stepperWait = EntityFactory.CreateStepperWait(StepperCombinedAxes.Axis1, 2_000);
                document.ActivePage?.ActiveLayer?.AddChild(stepperWait);

                var circle = EntityFactory.CreateArc(0, 0, 5, 0, 360);
                document.ActivePage?.ActiveLayer?.AddChild(circle);
            }


            siriusEditorControl1.View?.DoRender();

            //marker.Ready(siriusEditorControl1.Document);
            //marker.Start();
        }
    }
}
