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
using SpiralLab.Sirius3.UI.WinForms;
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

        const int editorCount = 2;
        SiriusEditorControl[] EditorControls = new SiriusEditorControl[editorCount];

        public Form1()
        {
            InitializeComponent();

            EditorControls[0] = siriusEditorControl1;
            EditorControls[1] = siriusEditorControl2;

            this.Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // must be called onload event at editor (view)
            tabControl1.SelectedIndex = 1;
            tabControl1.SelectedIndex = 0;

            // Also, need to 2 instances at library option.
            //Core.License(out var licenseInfo);
            //Debug.Assert(licenseInfo.RtcLicenseMax == 2);

            for (int i = 0; i < editorCount; i++)
            {
                EditorHelper.CreateDevices(out IRtc rtc, out ILaser laser, out IDInput dInExt1, out IDInput dInLaserPort, out IDOutput dOutExt1, out IDOutput dOutExt2, out IDOutput dOutLaserPort, out IPowerMeter powerMeter, out IMarker marker, i);

                EditorControls[i].Scanner = rtc;

                EditorControls[i].Laser = laser;

                EditorControls[i].DIExt1 = dInExt1;
                EditorControls[i].DOExt1 = dOutExt1;
                EditorControls[i].DOExt2 = dOutExt2;
                EditorControls[i].DILaserPort = dInLaserPort;
                EditorControls[i].DOLaserPort = dOutLaserPort;

                EditorControls[i].PowerMeter = powerMeter;

                EditorControls[i].Marker = marker;

                marker.Ready(EditorControls[i].Document, EditorControls[i].View, rtc, laser, powerMeter);

            }
        }     
    }
}
