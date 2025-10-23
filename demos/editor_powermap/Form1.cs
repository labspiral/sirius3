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
using SpiralLab.Sirius3.PowerMap;
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

            this.btnPowerMap.Click += BtnPowerMap_Click;
            this.btnPowerVerify.Click += BtnPowerVerify_Click;
            this.btnPowerCompensate.Click += BtnPowerCompensate_Click;
            this.btnStop.Click += BtnStop_Click;
            this.btnReset.Click += BtnReset_Click;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            EditorHelper.CreateDevices(out IRtc rtc, out ILaser laser, out IDInput dInExt1, out IDInput dInLaserPort, out IDOutput dOutExt1, out IDOutput dOutExt2, out IDOutput dOutLaserPort, out IPowerMeter powerMeter, out IMarker marker);

            siriusEditorControl1.Scanner = rtc;

            // Replace powermap at laser as my powermap 
            ReplacePowerMap(laser, rtc, powerMeter);

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

        bool ReplacePowerMap(ILaser laser, IScanner scanner, IPowerMeter powerMeter, int index = 0)
        {
            var powerControl = laser as ILaserPowerControl;
            Debug.Assert(null != powerControl);

            bool success = true;
            var powerMap = new MyPowerMap(index, $"MAP{index}");

            powerMap.Rtc = scanner as IRtc;
            powerMap.Laser = laser;
            powerMap.PowerMeter = powerMeter;

            powerMap.OnOpened += EditorHelper.PowerMap_OnMappingOpened;
            powerMap.OnSaved += EditorHelper.PowerMap_OnMappingSaved;

            var powerMapFile = NativeMethods.ReadIni<string>(EditorHelper.ConfigFileName, $"LASER{index}", "POWERMAP_FILE", string.Empty);
            var powerMapFullPath = Path.Combine(SpiralLab.Sirius3.Config.PowerMapPath, powerMapFile);
            if (File.Exists(powerMapFullPath))
                success &= PowerMapSerializer.Open(powerMapFullPath, powerMap);
            else
            {
                //reset as 1 to 1 if you want
                //powerMap.Reset1to1("10000", laser.MaxPowerWatt);
                //powerMap.Reset1to1("50000", laser.MaxPowerWatt);
            }
            if (null != powerControl)
            {
                powerControl.PowerMap = powerMap;
                // Enable lookup powermap table 
                powerMap.IsEnableLookUp = true;
            }

            return success;
        }

        private void BtnPowerMap_Click(object sender, EventArgs e)
        {
            var laser = siriusEditorControl1.Laser;
            var powerControl = laser as ILaserPowerControl;
            Debug.Assert(null != powerControl);
            var powerMap = powerControl.PowerMap;

            using (var form = new SpiralLab.Sirius3.UI.WinForms.PowerMapCategoriesForm())
            {
                //form.Categories.Add(new CategoryValue("10000"));
                // ...
                //form.MinWatt = 0;
                //form.MaxWatt = laser.MaxPowerWatt;
                //form.Steps = 10;
                if (DialogResult.OK == form.ShowDialog(this))
                {
                    //= form.Categories
                    powerMap.CtlMapping(
                        new string[] { "10000" },  //10KHz
                        new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10}  //1W ~ 10W
                        );
                }
            }
        }

        private void BtnPowerVerify_Click(object sender, EventArgs e)
        {
            var laser = siriusEditorControl1.Laser;
            var powerControl = laser as ILaserPowerControl;
            Debug.Assert(null != powerControl);
            var powerMap = powerControl.PowerMap;

            using (var form = new SpiralLab.Sirius3.UI.WinForms.PowerMapVerifyForm())
            {
                //form.CategoryWatts.Add(new CategoryWattValue("10000", 2));
                if (DialogResult.OK == form.ShowDialog(this))
                {
                    //= form.CategoryWatts
                    powerMap.CtlVerify(new KeyValuePair<string, double>[] {
                       new KeyValuePair<string, double>
                       (
                           "10000", //10KHz
                            2       //2W
                       )});
                }
            }
        }

        private void BtnPowerCompensate_Click(object sender, EventArgs e)
        {
            var laser = siriusEditorControl1.Laser;
            var powerControl = laser as ILaserPowerControl;
            Debug.Assert(null != powerControl);
            var powerMap = powerControl.PowerMap;

            using (var form = new SpiralLab.Sirius3.UI.WinForms.PowerMapCompensateForm())
            {
                //form.CategoryWatts.Add(new CategoryWattValue("10000", 2));
                if (DialogResult.OK == form.ShowDialog(this))
                {
                    //= form.CategoryWatts
                    powerMap.CtlCompensate(new KeyValuePair<string, double>[] {
                       new KeyValuePair<string, double>
                       (
                           "10000", //10KHz
                            2       //2W
                       )});
                }
            }
        }
        private void BtnStop_Click(object sender, EventArgs e)
        {
            var laser = siriusEditorControl1.Laser;
            var powerControl = laser as ILaserPowerControl;
            Debug.Assert(null != powerControl);
            var powerMap = powerControl.PowerMap;

            powerMap.CtlStop();
        }
        private void BtnReset_Click(object sender, EventArgs e)
        {
            var laser = siriusEditorControl1.Laser;
            var powerControl = laser as ILaserPowerControl;
            Debug.Assert(null != powerControl);
            var powerMap = powerControl.PowerMap;

            powerMap.CtlReset();
        }
 
    }
}
