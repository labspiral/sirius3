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
using SpiralLab.Sirius3.UI.WinForms;


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
                var dlgResult = System.Windows.Forms.MessageBox.Show(this, $"Do you really want to terminate program ?", "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
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

            this.btn3DCorrection.Click += (s, e) =>
            {
                using var form = new Correction3DRtcForm(siriusEditorControl1.Marker);
                form.ShowDialog();
            };

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

    }
}
