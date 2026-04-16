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
using SpiralLab.Sirius3.Entity.Helper;
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

            btnPrepare.Click += BtnPrepare_Click;
            btnIntersect.Click += BtnIntersect_Click;
            btnMark.Click += BtnMark_Click;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Create devices
            EditorHelper.CreateDevices(out IRtc rtc, out ILaser laser, out IDInput dInExt1, out IDInput dInLaserPort, out IDOutput dOutExt1, out IDOutput dOutExt2, out IDOutput dOutLaserPort, out IPowerMeter powerMeter, out IMarker marker);

            siriusEditorControl1.RegisterDevices(rtc, laser, powerMeter, dInExt1, dInLaserPort, dOutExt1, dOutExt2, dOutLaserPort, marker);

            // Ready marker
            marker.Ready(siriusEditorControl1.Document, siriusEditorControl1.View, rtc, laser, powerMeter);
        }

        /// <summary>
        /// Get points by creating rectangle with winding direction
        /// </summary>
        List<DVec2> CreateRectangle(
            DVec2 center,
            double width,
            double height,
            bool ccw = true)
        {
            double hw = width * 0.5;
            double hh = height * 0.5;

            var pts = new List<DVec2>
            {
                new DVec2(center.X - hw, center.Y - hh),
                new DVec2(center.X + hw, center.Y - hh),
                new DVec2(center.X + hw, center.Y + hh),
                new DVec2(center.X - hw, center.Y + hh),
            };

            if (!ccw)
                pts.Reverse();

            return pts;
        }

        EntityPolyline2D outterPolyline2D;
        EntityPolyline2D innerPolyline2D;
        EntityPolyline2D fovPolyline2D;
        EntityPolyline2D resultPolyline2D;

        private void BtnPrepare_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;            

            // New document
            document.ActNew();

            outterPolyline2D = null;
            innerPolyline2D = null;
            fovPolyline2D = null;
            resultPolyline2D = null;

            // Create outter rectangle with ccw direction
            var outterPts = CreateRectangle(new DVec2(0, 0), 100, 100, ccw: true); 
            outterPolyline2D = new EntityPolyline2D(outterPts, true);
            outterPolyline2D.Name = "Outter";
            outterPolyline2D.IsAllowMark = false;
            document.ActivePage?.ActiveLayer?.AddChild(outterPolyline2D);

            // Create inner rectangle with cw direction (for hole)
            var innerPts = CreateRectangle(new DVec2(0, 0), 80, 80, ccw: false);  
            innerPolyline2D = new EntityPolyline2D(innerPts, true);
            innerPolyline2D.Name = "Inner";
            innerPolyline2D.IsAllowMark = false;
            document.ActivePage?.ActiveLayer?.AddChild(innerPolyline2D);

            Random rnd = new Random((int)DateTime.Now.Ticks);
            double cx = rnd.NextDouble() * 120 - 60;
            double cy = rnd.NextDouble() * 120 - 60;
            double w = rnd.NextDouble() * 60 + 5;
            double h = rnd.NextDouble() * 60 + 5;

            // Create random rectangle with random position
            var fovPts = CreateRectangle(new DVec2(cx, cy), w, h, ccw: true);   
            fovPolyline2D = new EntityPolyline2D(fovPts, true);
            fovPolyline2D.Name = "Fov";
            fovPolyline2D.IsAllowMark = false;
            fovPolyline2D.Alpha = 0.4; 
            document.ActivePage?.ActiveLayer?.AddChild(fovPolyline2D);

            siriusEditorControl1.View?.DoRender();
        }

        private void BtnIntersect_Click(object sender, EventArgs e)
        {
            // Extract contour from outter rectangle 
            var contourOutter = outterPolyline2D.ToContour(outterPolyline2D.CalculateMatriesRecursive());

            // Extract contour from inner rectangle 
            var contourInnter = innerPolyline2D.ToContour(innerPolyline2D.CalculateMatriesRecursive());

            // Extract contour from fov rectangle 
            var contourFov = fovPolyline2D.ToContour(fovPolyline2D.CalculateMatriesRecursive());

            // Outter, inner rectangles into subject and Fov rectangle into clip.
            // Do intersect 
            bool success = ClipHelper.Intersect(new List<Contour> { contourOutter, contourInnter }, contourFov, out List<Contour> closeResults, out List<Contour> openResults);
            if (!success)
                return;

            if (0 == closeResults.Count)
                return;

            var document = siriusEditorControl1.Document;

            // Delete old one
            if (null != resultPolyline2D)
            {
                document.ActRemove(resultPolyline2D);
                resultPolyline2D = null;
            }

            // Get first result from intersect results and convert into polyline 2D
            resultPolyline2D = closeResults[0].ToPolyline2D();

            Random rnd = new Random((int)DateTime.Now.Ticks);
            resultPolyline2D.Name = "Result";

            // Add hatch 
            var hatch = HatchFactory.CreateLine(0, 0.05, 0, true); 
            //var hatch = HatchFactory.CreatePolygon()
            hatch.Order = HatchOrders.Ascending; 
            hatch.Sort = HatchSorts.None; 
            hatch.ModelColor = SpiralLab.Sirius3.UI.Config.EntityPenColors[1].ToDVec3(); // Yellow pen
            resultPolyline2D.HatchMarkOption = HatchMarkOptions.HatchFirst; // Mark hatch at first

            // Add hatch at polyline 2D
            //resultPoly2D.ClearHatches();
            resultPolyline2D.AddHatch(hatch);

            // Add polyline 2D 
            document.ActivePage?.ActiveLayer?.AddChild(resultPolyline2D);
            //document.ActAdd(resultPolyline2D);

            siriusEditorControl1.View?.DoRender();
        }
        private void BtnMark_Click(object sender, EventArgs e)
        {
            if (null == resultPolyline2D)
                return;

            var marker = siriusEditorControl1.Marker;
            if (marker.IsBusy || marker.IsError)
                return;

            var document = siriusEditorControl1.Document;
            var penColor = SpiralLab.Sirius3.UI.Config.EntityPenColors[1]; // Yellow pen
            document.FindByEntityPenColor(penColor, out var entityPen);
            entityPen.JumpSpeed = 1_000; // 1 m/s
            entityPen.MarkSpeed = 1_000; // 1 m/s

            // Do mark as selected targets only
            var markerRtc = marker as MarkerRtc;
            markerRtc.MarkTarget = MarkerRtc.MarkTargets.Selected;
            
            // Select target entity
            document.ActSelect(resultPolyline2D);

            var dlgResult = MessageBox.Show(this, $"Do you really want to mark selected {resultPolyline2D.ToString()} hatch ?", "WARNING", MessageBoxButtons.YesNo);
            if (dlgResult != DialogResult.Yes)
                return;

            // Start to mark
            marker.Start();
        }
    }
}
