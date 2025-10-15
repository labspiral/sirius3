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

        System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();


        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
            this.FormClosed += Form1_FormClosed;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            EditorHelper.CreateDevices(out IRtc rtc, out ILaser laser, out IDInput dInExt1, out IDInput dInLaserPort, out IDOutput dOutExt1, out IDOutput dOutExt2, out IDOutput dOutLaserPort, out IPowerMeter powerMeter, out IMarker marker);

            siriusEditorControl1.Scanner = rtc;

            siriusEditorControl1.Laser = laser;

            RenameDIOs(dInExt1, dOutExt1);

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

            CreateSampleData();

            marker.Ready(siriusEditorControl1.Document, siriusEditorControl1.EditorCtrl.View, rtc, laser, powerMeter, stage);

            // Enable timer for update D.IO status
            timer.Interval = 100; //10hz
            timer.Tick += Timer_Tick;
            timer.Enabled = true;

            dInExt1.OnChanged += DIExt1_OnChanged;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            timer.Stop();
            timer.Dispose();
        }

        void RenameDIOs(IDInput dInExt1, IDOutput dOutExt1)
        {
            dInExt1.ChannelNames = new string[1][] {
                new string[16] {
                    "Start1",
                    "Start2",
                    "Stop",
                    "Reset",
                    "D04",
                    "D05",
                    "D06",
                    "D07",
                    "D08",
                    "D09",
                    "D10",
                    "D11",
                    "D12",
                    "D13",
                    "D14",
                    "ExternalFile",
                }};

            dOutExt1.ChannelNames = new string[1][] {
                new string[16] {
                    "Ready",
                    "Busy",
                    "Error",
                    "D03",
                    "D04",
                    "D05",
                    "D06",
                    "D07",
                    "D08",
                    "D09",
                    "D10",
                    "D11",
                    "D12",
                    "D13",
                    "D14",
                    "D15",
                }};
        }

        void CreateSampleData()
        {
            var document = siriusEditorControl1.Document;
            var entity = new EntityText("Tahoma", FontStyle.Regular, $"Hello{Environment.NewLine}你好{Environment.NewLine}안녕{Environment.NewLine}こんにちは{Environment.NewLine}Hola{Environment.NewLine}Xin chào{Environment.NewLine}Здравствуйте", 2);
            entity.FontHorizontalAlignment = StringAlignment.Center;
            entity.FontVerticalAlignment = StringAlignment.Center;

            var hatch = HatchFactory.CreateLine(90, 0.02);
            entity.AddHatch(hatch);

            document.ActivePage?.ActiveLayer?.AddChild(entity);
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            if (this.IsDisposed || !this.IsHandleCreated)
                return;
            UpdateDIOStatus();
        }

        void UpdateDIOStatus()
        {
            var dInExt1 = siriusEditorControl1.DIExt1;
            var dOutExt1 = siriusEditorControl1.DOExt1;
            var marker = siriusEditorControl1.Marker;

            dInExt1?.Update();

            if (null != dOutExt1)
            {
                //D.Out
                if (marker.IsReady)
                    dOutExt1.OutOn(0);
                else
                    dOutExt1.OutOff(0);

                if (marker.IsBusy)
                    dOutExt1.OutOn(1);
                else
                    dOutExt1.OutOff(1);
                if (marker.IsError)
                    dOutExt1.OutOn(2);
                else
                    dOutExt1.OutOff(2);

                dOutExt1.Update();
            }
        }
        private void DIExt1_OnChanged(SpiralLab.Sirius3.IO.IDInput dInput, int bitNo, SignalEdges edge)
        {
            var document = siriusEditorControl1.Document;
            var marker = siriusEditorControl1.Marker;
            if (null == marker)
                return;
         
            switch (bitNo)
            {
                case 0: //start page1
                    if (edge == SignalEdges.High)
                    {
                        Logger.Log(LogLevel.Information , "trying to start mark for page1 by external dio");
                        marker.Ready(document);
                        marker.Page = DocumentPages.Page1;
                        marker.Start();
                    }
                    break;
                case 1: //start page2
                    if (edge == SignalEdges.High)
                    {
                        Logger.Log(LogLevel.Information, "trying to start mark for page2 by external dio");
                        marker.Ready(document);
                        marker.Page = DocumentPages.Page1;
                        marker.Start();
                    }
                    break;
                case 2: //stop
                    if (edge == SignalEdges.High)
                    {
                        Logger.Log(LogLevel.Information, "trying to stop marker by external dio");
                        marker.Stop();
                    }
                    break;
                case 3: //reset
                    if (edge == SignalEdges.High)
                    {
                        Logger.Log(LogLevel.Information, "trying to reset marker by external dio");
                        marker.Reset();
                    }
                    break;
                case 15: // open external file and try to mark
                    if (edge == SignalEdges.High)
                    {
                        Logger.Log(LogLevel.Information, "trying to mark field corretion by external dio");
                        string fileName = Path.Combine(SpiralLab.Sirius3.UI.Config.RecipePath, "cal_100mm_5x5.sirius3");
                        if (!File.Exists(fileName))
                        {
                            Logger.Log(LogLevel.Error, $"file not found: {fileName}");
                            break;
                        }
                        if (!document.ActOpen(fileName))
                        {
                            Logger.Log(LogLevel.Error, $"fail to open external file: {fileName}");
                            break;
                        }

                        marker.Ready(document);
                        marker.Start();
                    }
                    break;
            }
        }
    }
}
