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
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            EditorHelper.CreateDevices(out IRtc rtc, out ILaser laser, out IDInput dInExt1, out IDInput dInLaserPort, out IDOutput dOutExt1, out IDOutput dOutExt2, out IDOutput dOutLaserPort, out IPowerMeter powerMeter, out IMarker marker);

            // IRtcInterrupt 사용하기 위해 RTC 카드가 지원하는지 여부 확인
            var rtcInterrupt = rtc as IRtcInterrupt;
            Debug.Assert(rtcInterrupt != null);
            
            rtcInterrupt.OnInterrupt -= RtcInterrupt_OnInterrupt;
            rtcInterrupt.OnInterrupt += RtcInterrupt_OnInterrupt;

            siriusEditorControl1.Scanner = rtc;
            siriusEditorControl1.Laser = laser;
            siriusEditorControl1.DIExt1 = dInExt1;
            siriusEditorControl1.DOExt1 = dOutExt1;
            siriusEditorControl1.DOExt2 = dOutExt2;
            siriusEditorControl1.DILaserPort = dInLaserPort;
            siriusEditorControl1.DOLaserPort = dOutLaserPort;
            siriusEditorControl1.PowerMeter = powerMeter;
            siriusEditorControl1.Marker = marker;

            CreateEntities(siriusEditorControl1.Document);

            marker.Ready(siriusEditorControl1.Document, siriusEditorControl1.View, rtc, laser, powerMeter);
        }
    
        private void CreateEntities(IDocument document)
        {
            var rnd = new Random((int)DateTime.Now.Ticks);

            for (int i = 0; i < 10; i++)
            {
                var arc = EntityFactory.CreateArc(new DVec3(0, 0, 0), 5, 0, 360);
                double tx = rnd.NextDouble() * 100.0 - 50.0;
                double ty = rnd.NextDouble() * 100.0 - 50.0;
                arc.Translate(tx, ty, 0);

                // by arc.Id
                var breakPoint = EntityFactory.CreateBreakPoint(arc);
   
                // breakpoint entity before arc
                document.ActivePage?.ActiveLayer?.AddChild(breakPoint);

                // arc entity
                document.ActivePage?.ActiveLayer?.AddChild(arc); 
            }
        }

        private bool RtcInterrupt_OnInterrupt(IRtcInterrupt rtcInterrupt, long waitID)
        {
            // RTC list is excuting but paused !

            var document = siriusEditorControl1.Document;
            if (document.FindById(waitID, out var foundedEntity))
            {
                if (foundedEntity is EntityArc entityArc)
                { 
                    if (entityArc.CalcuateRealMinMax(out var min, out var max))
                    {
                        // Do something work before mark arc 
                        var realCenter = (min + max) * 0.5;
                        Thread.Sleep(2_000);

                        return true; // 'True' case 'rtcInterrupt.CtlResumePoint()' will be called.
                    }
                }
            }
            return false; 
        }

    }
}
