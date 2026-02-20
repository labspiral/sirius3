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

        const int MaxDeviceCount = 2;

        private IRtc[] rtcs = new IRtc[MaxDeviceCount];
        private ILaser[] lasers = new ILaser[MaxDeviceCount];
        private IDInput[] dInExt1s = new IDInput[MaxDeviceCount];
        private IDInput[] dInLaserPorts = new IDInput[MaxDeviceCount];
        private IDOutput[] dOutExt1s = new IDOutput[MaxDeviceCount];
        private IDOutput[] dOutExt2s = new IDOutput[MaxDeviceCount];
        private IDOutput[] dOutLaserPorts = new IDOutput[MaxDeviceCount];
        private IPowerMeter[] powerMeters = new IPowerMeter[MaxDeviceCount];
        private IMarker[] markers = new IMarker[MaxDeviceCount];

        public Form1()
        {
            InitializeComponent();

            this.Load += Form1_Load;
            this.Disposed += Form1_Disposed;
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            // Also, must be equipped multiple instances at sirius 3 library option.
            //Core.License(out var licenseInfo);
            //Debug.Assert(licenseInfo.RtcLicenseMax == MaxDeviceCount);

            siriusMultiEditorControl1.MaxDeviceCounts = MaxDeviceCount;

            for (int index= 0; index < MaxDeviceCount; index++)
            {
                EditorHelper.CreateDevices(out rtcs[index], out lasers[index], out dInExt1s[index], out dInLaserPorts[index], out dOutExt1s[index], out dOutExt2s[index], out dOutLaserPorts[index], out powerMeters[index], out markers[index], index);
                siriusMultiEditorControl1.RegisterDevices(index, rtcs[index], lasers[index], powerMeters[index], dInExt1s[index], dInLaserPorts[index], dOutExt1s[index], dOutExt2s[index], dOutLaserPorts[index], markers[index]);
            }

            // 0 (first device set) by default
            siriusMultiEditorControl1.SwitchDevices(0);
        }

        private void Form1_Disposed(object sender, EventArgs e)
        {
            EditorHelper.DestroyDevices(siriusMultiEditorControl1);
        }
    }
}
