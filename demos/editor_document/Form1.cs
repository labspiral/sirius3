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
    /// <summary>
    /// How to use multiple <see cref="IDocument"/>s
    /// </summary>
    public partial class Form1 : Form
    {
        
        IDocument docOriginal = null;
        IDocument doc1 = null;
        IDocument doc2 = null;


        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
            this.FormClosing += (s, e) =>
            {
                var dlgResult = MessageBox.Show(this, $"Do you really want to terminate program ?", "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dlgResult != DialogResult.Yes)
                {
                    // is marker need to abort ?
                    e.Cancel = true;
                    return;
                }

                // Dispose instances 
                siriusEditorControl1.DisposeDevices();

                // Dispose documents
                siriusEditorControl1.Document = null;
                docOriginal?.Dispose();
                doc1?.Dispose();
                doc2?.Dispose();

                // Clean up SIRIUS3 library
                SpiralLab.Sirius3.Core.Cleanup();
            };

            this.btnOriginal.Click += (s, e) =>
            {
                // Revert to original document
                siriusEditorControl1.Document = docOriginal;
                viewerControl1.Document = docOriginal;

                // Redraw at Editor 
                siriusEditorControl1.View.DoRender();

                // Zomm to fit 
                viewerControl1.View.ActiveCamera?.ZoomFit(viewerControl1.View, docOriginal.ActivePage.ActiveLayer);
                // Redraw at Viewer
                viewerControl1.View.DoRender();

                // Re-assign changed document at Marker
                siriusEditorControl1.Marker.Ready(docOriginal);
            };

            this.btnDoc1.Click += (s, e) =>
            {
                if (null == doc1)
                {
                    // Create test entity
                    doc1 = DocumentFactory.CreateDefault();
                    var entity = EntityFactory.CreateText("Tahoma", FontStyle.Bold, "DOCUMENT 1", 20);
                    doc1.ActAdd(entity);
                }
                // Change to document1 
                siriusEditorControl1.Document = doc1;
                viewerControl1.Document = doc1;

                // Redraw at Editor 
                siriusEditorControl1.View.DoRender();

                // Zomm to fit 
                viewerControl1.View.ActiveCamera?.ZoomFit(viewerControl1.View, doc1.ActivePage.ActiveLayer);
                // Redraw at Viewer
                viewerControl1.View.DoRender();

                // Re-assign changed document at Marker
                siriusEditorControl1.Marker.Ready(doc1);
            };

            this.btnDoc2.Click += (s, e) =>
            {
                if (null == doc2)
                {
                    // Create test entity
                    doc2 = DocumentFactory.CreateDefault();
                    var entity = EntityFactory.CreateText("Arial", FontStyle.Bold, "DOCUMENT 2", 20);
                    doc2.ActAdd(entity);
                }
                // Change to document2 
                siriusEditorControl1.Document = doc2;
                viewerControl1.Document = doc2;

                // Redraw at Editor 
                siriusEditorControl1.View.DoRender();

                // Zomm to fit 
                viewerControl1.View.ActiveCamera?.ZoomFit(viewerControl1.View, doc2.ActivePage.ActiveLayer);
                // Redraw at Viewer
                viewerControl1.View.DoRender();

                // Re-assign changed document at Marker
                siriusEditorControl1.Marker.Ready(doc2);
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

            docOriginal = siriusEditorControl1.Document;
            viewerControl1.Document = docOriginal;

            marker.Ready(docOriginal, siriusEditorControl1.View, rtc, laser, powerMeter);
        }
    }
}
