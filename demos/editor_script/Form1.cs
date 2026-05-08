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
using SpiralLab.Sirius3.Scripting;

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


            this.btnCreateText.Click += BtnCreateText_Click;
            this.btnScriptShow.Click += BtnScriptShow_Click;
            this.btnScriptSave.Click += BtnScriptSave_Click;
            this.btnScriptOpen.Click += BtnScriptOpen_Click;
            this.btnScriptRevert.Click += BtnScriptRevert_Click;

            this.btnLoadCompile.Click += BtnLoadCompile_Click;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            EditorHelper.CreateDevices(out IRtc rtc, out ILaser laser, out IDInput dInExt1, out IDInput dInLaserPort, out IDOutput dOutExt1, out IDOutput dOutExt2, out IDOutput dOutLaserPort, out IPowerMeter powerMeter, out IMarker marker);

            siriusEditorControl1.RegisterDevices(rtc, laser, powerMeter, dInExt1, dInLaserPort, dOutExt1, dOutExt2, dOutLaserPort, marker);

            marker.Ready(siriusEditorControl1.Document, siriusEditorControl1.View, rtc, laser, powerMeter);
        }

        private void BtnCreateText_Click(object sender, EventArgs e)
        {
            siriusEditorControl1.Document.ActNew();

            // Create text with semi-ocr font (double dots)
            var text = EntityFactory.CreateSiriusText("semi_ocr_double.dot",
                 EntitySiriusText.LetterSpaces.Fixed,
                 $"01234{Environment.NewLine}ABCDEF", 10);

            // Add into document
            siriusEditorControl1.Document.ActAdd(text);
            // And then config ...

            // Use simple script converter at Marker
            text.TextConverter = TextConverters.SimpleScript;

            // Script documentation : https://github.com/labspiral/sirius3/blob/main/doc/ScriptUserManual.md

            // Will be increase serial no each Marker.Start
            var expr1 = @"NextSerialNo(1)";
            /*
            var expr2 = @"string prf = LotCode.Substring(0, Math.Min(LotCode.Length, 3)); 
string dt = Date(""yyMMdd"");
string tm = Time(""HHmm"");
string sn = NextSerialNo(""D5"");
string sh = Shift(""A"", ""B"", ""C"");
return $""{prf}-{dt}-{tm}-{sn}-{sh}"";
            ";
            */
            // Expression for script format
            text.SourceText = expr1;

            // Allow text conversion
            text.IsAllowConvert = true;

            // Redraw
            siriusEditorControl1.View?.DoRender();
        }

        private void BtnScriptShow_Click(object sender, EventArgs e)
        {
            var marker = siriusEditorControl1.Marker;

            siriusEditorControl1.Document.ActSelect(marker.ScriptInstance);
        }

        private void BtnScriptOpen_Click(object sender, EventArgs e)
        {
            var marker = siriusEditorControl1.Marker;
            if (null == marker) return;
            if (null == marker || marker.IsBusy)
                return;

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Title = "Script Instance File ...";
                ofd.Filter = "script instance file (*.script)|*.script|All Files (*.*)|*.*";
                ofd.DefaultExt = "script";
                ofd.InitialDirectory = SpiralLab.Sirius3.Config.ScriptPath;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    if (ScriptSerializer.Open(ofd.FileName, out var script))
                    {
                        marker.ScriptInstance = script;
                    }
                }
            }

            BtnScriptShow_Click(sender, e);
        }

        private void BtnScriptSave_Click(object sender, EventArgs e)
        {
            var marker = siriusEditorControl1.Marker;
            if (null == marker) return;
            if (null == marker || marker.IsBusy)
                return;

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Title = "Script Instance File ...";
                sfd.Filter = "script instance file (*.script)|*.script|All Files (*.*)|*.*";
                sfd.DefaultExt = "script";
                sfd.InitialDirectory = SpiralLab.Sirius3.Config.ScriptPath;
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    ScriptSerializer.Save(sfd.FileName, marker.ScriptInstance);
                }
            }
        }

        private void BtnScriptRevert_Click(object sender, EventArgs e)
        {
            var marker = siriusEditorControl1.Marker;
            if (null == marker) return;
            if (null == marker || marker.IsBusy)
                return;

            // Revert(or reset) original(or built in) script instance
            marker.ScriptInstance = ScriptFactory.Create();
        }

        private void BtnLoadCompile_Click(object sender, EventArgs e)
        {
            LoadAndCompileScript();
        }

        void LoadAndCompileScript()
        {
            var marker = siriusEditorControl1.Marker;
            if (null == marker) return;
            if (null == marker || marker.IsBusy)
                return;

            var ofd = new OpenFileDialog();
            ofd.Title = "Script File ...";
            ofd.Filter = "script cs file (*.cs)|*.cs|script dll file (*.dll)|*.dll|All Files (*.*)|*.*";
            ofd.DefaultExt = "cs";
            ofd.InitialDirectory = SpiralLab.Sirius3.Config.ScriptPath;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                var script = ScriptFactory.Create(ofd.FileName);
                if (null != script)
                {
                    marker.ScriptInstance = script;
                    BtnScriptShow_Click(null, null);
                }
            }
        }
    }
}
