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

            this.btnPoints.Click += (s, e) => {
                var document = siriusEditorControl1.Document;
                points_testcase(document);
            };
            this.btnLineArc.Click += (s, e) => {
                var document = siriusEditorControl1.Document;
                line_arc_testcase(document);
            };
            this.btnTriangleRectangle.Click += (s, e) => {
                var document = siriusEditorControl1.Document;
                triangle_rectangle_testcase(document);
            };
            this.btnPolyline.Click += (s, e) => {
                var document = siriusEditorControl1.Document;
                polyline2d_testcase(document);
                polyline3d_testcase(document);
            };
            this.btnHatch.Click += (s, e) => {
                var document = siriusEditorControl1.Document;
                hatch_testcase(document);
            };
            this.btnSpline.Click += (s, e) => {
                var document = siriusEditorControl1.Document;
                bezierSpline_testcase(document);
                catmullRomSpline_testcase(document);
            };
            this.btnText.Click += (s, e) => {
                var document = siriusEditorControl1.Document;
                text_testcase(document);
            };
            this.btnImage.Click += (s, e) => {
                var document = siriusEditorControl1.Document;
                image_testcase(document);
            };
            this.btnGridCloud.Click += (s, e) => {
                var document = siriusEditorControl1.Document;
                gridcloud_testcase(document);
            };
            this.btnLines.Click += (s, e) => {
                var document = siriusEditorControl1.Document;
                large_lines_testcase(document);
            };
            this.btnBarcode.Click += (s, e) => {
                var document = siriusEditorControl1.Document;
                barcode_testcase(document);
            };
            this.btnGroup.Click += (s, e) => {
                var document = siriusEditorControl1.Document;
                groupIngroup_testcase(document);
            };
            this.btn3DMesh.Click += (s, e) => {
                var document = siriusEditorControl1.Document;
                sphere_testcase(document);
                cube_cylinder_testcase(document);
            };
            this.btnBlockInsert.Click += (s, e) => {
                var document = siriusEditorControl1.Document;
                block_insert_testcase(document);
            };
            this.btnZPL.Click += (s, e) => {
                var document = siriusEditorControl1.Document;
                zpl_testcase(document);
            };
            this.btnLissajous.Click += (s, e) => {
                var document = siriusEditorControl1.Document;
                lissajous_testcase(document);
            };
            this.btnGerber.Click += (s, e) => {
                var document = siriusEditorControl1.Document;
                gerber_testcase(document);
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

        #region Testcases (Samples)
        /// <summary>
        /// Adds a random point cloud entity.
        /// </summary>
        private void points_testcase(IDocument document)
        {
            var rng = new Random((int)DateTime.Now.Ticks);

            int VERT_COUNT = 100 + (int)(rng.NextDouble() * 100);
            var tempVerts = new List<Vector3d>(VERT_COUNT);
            for (int v = 0; v < VERT_COUNT; v++)
            {
                double x = rng.NextDouble() * 6.0 - 3.0;
                double y = rng.NextDouble() * 6.0 - 3.0;
                double z = rng.NextDouble();
                tempVerts.Add(new Vector3d(x, y, z));
            }

            var points = new EntityPoints(tempVerts)
            {
                ColorMode = EntityModelBase.ColorModes.Model,
                ModelColor = new Vector3d(rng.NextDouble() + 0.8, rng.NextDouble() * 0.5, rng.NextDouble() + 0.4)
            };

            double tx = rng.NextDouble() * 100.0 - 50.0;
            double ty = rng.NextDouble() * 100.0 - 50.0;
            double tz = rng.NextDouble() * 10.0;
            points.Translate(tx, ty, tz);

            document.ActivePage?.ActiveLayer?.AddChild(points);
        }

        /// <summary>
        /// Adds line, arc, trepan samples with random transforms.
        /// </summary>
        private void line_arc_testcase(IDocument document)
        {
            var rng = new Random((int)DateTime.Now.Ticks);

            {
                var entity = new EntityLine(new Vector3d(0, 0, 0), new Vector3d(10, 10, 1));
                document.ActivePage?.ActiveLayer?.AddChild(entity);
            }

            {
                var entity = new EntityArc(new Vector3d(0, 0, 0), 5);
                double rx = rng.NextDouble() * 10 - 5.0;
                double ry = rng.NextDouble() * 10 - 5.0;
                double rz = rng.NextDouble() * 10 - 5.0;
                entity.Rotate(rx, ry, rz);

                double tx = rng.NextDouble() * 100.0 - 50.0;
                double ty = rng.NextDouble() * 100.0 - 50.0;
                double tz = rng.NextDouble() * 100.0 - 10.0;
                entity.Translate(tx, ty, tz);

                document.ActivePage?.ActiveLayer?.AddChild(entity);
            }

            {
                var entity = new EntityTrepan(Vector3d.Zero, 5, 10, 10);
                double tx = rng.NextDouble() * 100.0 - 50.0;
                double ty = rng.NextDouble() * 100.0 - 50.0;
                double tz = rng.NextDouble() * 10.0;
                entity.Translate(tx, ty, tz);

                document.ActivePage?.ActiveLayer?.AddChild(entity);
            }
        }

        /// <summary>
        /// Adds triangle/rectangle/cross with random transforms.
        /// </summary>
        private void triangle_rectangle_testcase(IDocument document)
        {
            var rng = new Random((int)DateTime.Now.Ticks);

            {
                var entity = new EntityTriangle(new Vector3d(0, 0, 0), 3, 2);
                entity.Rotate(rng.NextDouble() * 10 - 5.0, rng.NextDouble() * 10 - 5.0, rng.NextDouble() * 10 - 5.0);
                entity.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 10.0);
                document.ActivePage?.ActiveLayer?.AddChild(entity);
            }

            {
                var entity = new EntityRectangle(new Vector3d(0, 0, 0), 4, 3);
                entity.Rotate(rng.NextDouble() * 10 - 5.0, rng.NextDouble() * 10 - 5.0, rng.NextDouble() * 10 - 5.0);
                entity.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 10.0);
                document.ActivePage?.ActiveLayer?.AddChild(entity);
            }

            {
                var entity = new EntityCross(Vector3d.Zero, 10, 10, 2);
                entity.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 10.0);
                document.ActivePage?.ActiveLayer?.AddChild(entity);
            }
        }

        /// <summary>
        /// Adds hatch examples on rectangle/cross/polylines.
        /// </summary>
        private void hatch_testcase(IDocument document)
        {
            var rng = new Random((int)DateTime.Now.Ticks);

            {
                var entity = new EntityRectangle(new Vector3d(0, 0, 0), 4, 3);
                entity.Rotate(rng.NextDouble() * 10 - 5.0, rng.NextDouble() * 10 - 5.0, rng.NextDouble() * 10 - 5.0);
                entity.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 10.0);
                entity.AddHatch(HatchFactory.CreateLine(30, 0.2));
                entity.AddHatch(HatchFactory.CreateLine(120, 0.2));
                document.ActivePage?.ActiveLayer?.AddChild(entity);
            }

            {
                var entity = new EntityCross(Vector3d.Zero, 10, 10, 2);
                entity.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 10.0);
                entity.AddHatch(HatchFactory.CreatePolygon(0.1));
                document.ActivePage?.ActiveLayer?.AddChild(entity);
            }

            {
                const int ENTITY_COUNT = 5;
                for (int i = 0; i < ENTITY_COUNT; i++)
                {
                    int VERT_COUNT = 3 + (int)(rng.NextDouble() * 5);
                    var tempVerts = new List<Vertex2D>(VERT_COUNT);
                    for (int v = 0; v < VERT_COUNT; v++)
                    {
                        double x = rng.NextDouble() * 10.0 - 5.0;
                        double y = rng.NextDouble() * 10.0 - 5.0;
                        double b = rng.NextDouble();
                        tempVerts.Add(new Vertex2D(x, y, b));
                    }

                    var poly = new EntityPolyline2D(tempVerts, true)
                    {
                        ColorMode = EntityModelBase.ColorModes.Model,
                        ModelColor = new Vector3d(rng.NextDouble() + 0.4, rng.NextDouble() * 0.5, rng.NextDouble() + 0.4)
                    };

                    poly.Rotate(rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0);
                    poly.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 10.0);
                    poly.AddHatch(HatchFactory.CreateLine(45, 0.2, 0.1));

                    document.ActivePage?.ActiveLayer?.AddChild(poly);
                }
            }
        }

        /// <summary>
        /// Adds multiple cubes and cylinders with random transforms.
        /// </summary>
        private void cube_cylinder_testcase(IDocument document)
        {
            var rng = new Random((int)DateTime.Now.Ticks);
            const int ENTITY_COUNT = 5;

            for (int i = 0; i < ENTITY_COUNT; i++)
            {
                var cube = new EntityCube(Vector3d.Zero, rng.NextDouble() * 5, rng.NextDouble() * 6, rng.NextDouble() * 2)
                {
                    ColorMode = EntityModelBase.ColorModes.Model,
                    ModelColor = new Vector3d(rng.NextDouble() + 0.8, rng.NextDouble() * 0.5, rng.NextDouble())
                };
                cube.Rotate(rng.NextDouble() * 60.0 - 30.0, rng.NextDouble() * 60.0 - 30.0, rng.NextDouble() * 60.0 - 30.0);
                cube.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 10.0);
                document.ActivePage?.ActiveLayer?.AddChild(cube);

                var cyl = new EntityCylinder(Vector3d.Zero, rng.NextDouble() * 10, rng.NextDouble() * 10)
                {
                    ColorMode = EntityModelBase.ColorModes.Model,
                    ModelColor = new Vector3d(rng.NextDouble() * 0.5, rng.NextDouble() * 0.7, rng.NextDouble() + 0.5)
                };
                cyl.Rotate((float)(rng.NextDouble() * 60.0 - 30.0), (float)(rng.NextDouble() * 60.0 - 30.0), (float)(rng.NextDouble() * 60.0 - 30.0));
                cyl.Translate((float)(rng.NextDouble() * 100.0 - 50.0), (float)(rng.NextDouble() * 100.0 - 50.0), (float)(rng.NextDouble() * 100.0 - 10.0));
                document.ActivePage?.ActiveLayer?.AddChild(cyl);
            }
        }

        /// <summary>
        /// Adds two large grid-cloud entities for a height map example.
        /// </summary>
        private void gridcloud_testcase(IDocument document)
        {
            var rng = new Random((int)DateTime.Now.Ticks);
            const int COLS = 1024;
            const int ROWS = 768;
            const double INTERVAL = 0.05;

            EntityGrids reference = null;
            EntityGrids measured = null;

            // Reference
            {
                var zDepths = new List<double>(ROWS * COLS);
                var center = new Vector2d(COLS / 2f * INTERVAL, ROWS / 2f * INTERVAL);
                double amplitude = 0.5f;
                float wavelength = 5f;
                double phaseOffset = 0f;

                for (int y = 0; y < ROWS; y++)
                {
                    for (int x = 0; x < COLS; x++)
                    {
                        var pos = new Vector2d(x * INTERVAL, y * INTERVAL);
                        double dist = (pos - center).Length;
                        double z = amplitude * Math.Sin((2 * Math.PI * dist / wavelength) + phaseOffset);
                        zDepths.Add(z);
                    }
                }

                var minZ = zDepths.Min();
                var maxZ = zDepths.Max();
                var pointsCloud = new EntityGrids(ROWS, COLS, INTERVAL, zDepths, new Vector2d(minZ + 2, maxZ + 2));
                pointsCloud.Translate(-100, 0, 2);
                document.ActivePage?.ActiveLayer?.AddChild(pointsCloud);
                reference = pointsCloud;
            }

            // Measured
            {
                var zDepths = new List<double>(ROWS * COLS);
                var center = new Vector2d(COLS / 2f * INTERVAL, ROWS / 2f * INTERVAL);
                double amplitude = 0.5f;
                double wavelength = 5f;
                double phaseOffset = 0f;

                for (int y = 0; y < ROWS; y++)
                {
                    for (int x = 0; x < COLS; x++)
                    {
                        var pos = new Vector2d(x * INTERVAL, y * INTERVAL);
                        double dist = (pos - center).Length;
                        double z = amplitude * Math.Sin((2 * Math.PI * dist / wavelength) + phaseOffset);
                        zDepths.Add(z + 0.02f);
                    }
                }

                var minZ = zDepths.Min();
                var maxZ = zDepths.Max();
                var pointsCloud = new EntityGrids(ROWS, COLS, INTERVAL, zDepths, new Vector2d(minZ + 5, maxZ + 5))
                {
                    ColorMode = EntityModelBase.ColorModes.PerVertex
                };
                pointsCloud.Translate(100, 0, 5);
                document.ActivePage?.ActiveLayer?.AddChild(pointsCloud);
                measured = pointsCloud;
            }
        }

        /// <summary>
        /// Adds random closed 2D polylines with transforms.
        /// </summary>
        private void polyline2d_testcase(IDocument document)
        {
            var rng = new Random((int)DateTime.Now.Ticks);
            const int ENTITY_COUNT = 5;

            for (int i = 0; i < ENTITY_COUNT; i++)
            {
                int VERT_COUNT = 3 + (int)(rng.NextDouble() * 5);
                var tempVerts = new List<Vertex2D>(VERT_COUNT);
                for (int v = 0; v < VERT_COUNT; v++)
                {
                    double x = rng.NextDouble() * 10.0 - 5.0;
                    double y = rng.NextDouble() * 10.0 - 5.0;
                    double b = rng.NextDouble();
                    tempVerts.Add(new Vertex2D(x, y, b));
                }

                var poly = new EntityPolyline2D(tempVerts, true)
                {
                    ColorMode = EntityModelBase.ColorModes.Model,
                    ModelColor = new Vector3d(rng.NextDouble() + 0.4, rng.NextDouble() * 0.5, rng.NextDouble() + 0.4)
                };

                poly.Rotate(rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0);
                poly.Scale(rng.NextDouble() * 2.0 + 0.5, rng.NextDouble() * 2.0 + 0.5, rng.NextDouble() * 2.0 + 0.5);
                poly.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 10.0);

                document.ActivePage?.ActiveLayer?.AddChild(poly);
            }
        }

        /// <summary>
        /// Adds random closed 3D polylines with transforms.
        /// </summary>
        private void polyline3d_testcase(IDocument document)
        {
            var rng = new Random((int)DateTime.Now.Ticks);
            const int ENTITY_COUNT = 5;

            for (int i = 0; i < ENTITY_COUNT; i++)
            {
                int VERT_COUNT = 3 + (int)(rng.NextDouble() * 5);
                var tempVerts = new List<Vector3d>(VERT_COUNT);
                for (int v = 0; v < VERT_COUNT; v++)
                {
                    double x = rng.NextDouble() * 10.0 - 5.0;
                    double y = rng.NextDouble() * 10.0 - 5.0;
                    double z = rng.NextDouble() * 10.0 - 5.0;
                    tempVerts.Add(new Vector3d(x, y, z));
                }

                var poly = new EntityPolyline3D(tempVerts, true)
                {
                    ColorMode = EntityModelBase.ColorModes.Model,
                    ModelColor = new Vector3d(rng.NextDouble() + 0.4, rng.NextDouble() * 0.5, rng.NextDouble() + 0.4)
                };

                poly.Rotate(rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0);
                poly.Scale(rng.NextDouble() * 2.0 + 0.5, rng.NextDouble() * 2.0 + 0.5, rng.NextDouble() * 2.0 + 0.5);
                poly.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 10.0);

                document.ActivePage?.ActiveLayer?.AddChild(poly);
            }
        }

        /// <summary>
        /// Adds random Bezier spline examples with transforms.
        /// </summary>
        private void bezierSpline_testcase(IDocument document)
        {
            var rng = new Random((int)DateTime.Now.Ticks);
            const int ENTITY_COUNT = 5;

            for (int i = 0; i < ENTITY_COUNT; i++)
            {
                int VERT_COUNT = 3 + (int)(rng.NextDouble() * 5);
                var tempVerts = new List<Vector3d>(VERT_COUNT);
                for (int v = 0; v < VERT_COUNT; v++)
                {
                    double x = rng.NextDouble() * 10.0 - 5.0;
                    double y = rng.NextDouble() * 10.0 - 5.0;
                    double z = rng.NextDouble() * 10.0 - 5.0;
                    tempVerts.Add(new Vector3d(x, y, z));
                }

                var spline = new EntityBezierSpline(tempVerts)
                {
                    ColorMode = EntityModelBase.ColorModes.Model,
                    ModelColor = new Vector3d(rng.NextDouble() + 0.4, rng.NextDouble() * 0.5, rng.NextDouble() + 0.4)
                };

                spline.Rotate(rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0);
                spline.Scale(rng.NextDouble() * 2.0 + 0.5, rng.NextDouble() * 2.0 + 0.5, rng.NextDouble() * 2.0 + 0.5);
                spline.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 10.0);

                document.ActivePage?.ActiveLayer?.AddChild(spline);
            }
        }

        /// <summary>
        /// Adds random Catmull-Rom spline examples with transforms.
        /// </summary>
        private void catmullRomSpline_testcase(IDocument document)
        {
            var rng = new Random((int)DateTime.Now.Ticks);
            const int ENTITY_COUNT = 5;

            for (int i = 0; i < ENTITY_COUNT; i++)
            {
                int VERT_COUNT = 5 + (int)(rng.NextDouble() * 5);
                var tempVerts = new List<Vector3d>(VERT_COUNT);
                for (int v = 0; v < VERT_COUNT; v++)
                {
                    double x = rng.NextDouble() * 10.0 - 5.0;
                    double y = rng.NextDouble() * 10.0 - 5.0;
                    double z = rng.NextDouble() * 10.0 - 5.0;
                    tempVerts.Add(new Vector3d(x, y, z));
                }

                var spline = new EntityCatmullRomSpline(tempVerts, false)
                {
                    ColorMode = EntityModelBase.ColorModes.Model,
                    ModelColor = new Vector3d(rng.NextDouble() + 0.4, rng.NextDouble() * 0.5, rng.NextDouble() + 0.4)
                };

                spline.Rotate(rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0);
                spline.Scale(rng.NextDouble() * 2.0 + 0.5, rng.NextDouble() * 2.0 + 0.5, rng.NextDouble() * 2.0 + 0.5);
                spline.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 10.0);

                document.ActivePage?.ActiveLayer?.AddChild(spline);
            }
        }

        /// <summary>
        /// Adds multiple text variants (GDI, image, circular, cxf) with transforms.
        /// </summary>
        private void text_testcase(IDocument document)
        {
            var rng = new Random((int)DateTime.Now.Ticks);

            {
                var text = new EntityText("Arial", FontStyle.Regular, $"0123456789{Environment.NewLine}AaBbFfGgHhJj{Environment.NewLine}~!@#$%^&*()_+", 10);
                text.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 10.0);
                document.ActivePage?.ActiveLayer?.AddChild(text);
            }

            {
                var text = new EntityText("Segoe UI", FontStyle.Regular, $"½ºÆÄÀÌ·²·¦{Environment.NewLine}SIRIUS3{Environment.NewLine}°³¹ßÀÚ ¹öÀü", 12);
                text.Rotate(rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0);
                text.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 10.0);
                document.ActivePage?.ActiveLayer?.AddChild(text);
            }

            {
                var text = new EntityImageText(
                    "Segoe UI",
                    FontStyle.Regular,
                    $"0123456789{Environment.NewLine}AaBbFfGgHhJj{Environment.NewLine}~!@#$%^&*()_+",
                    50, 1, true, 20);

                text.Rotate(rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0);
                text.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 10.0);
                document.ActivePage?.ActiveLayer?.AddChild(text);
            }

            {
                var text = new EntityCircularText("Segoe UI", FontStyle.Regular, TextCircularDirections.ClockWise, 30, 90,
                    $"0123456789{Environment.NewLine}AaBbFfGgHhJj{Environment.NewLine}~!@#$%^&*()_+", 5);

                text.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 10.0);
                document.ActivePage?.ActiveLayer?.AddChild(text);
            }

            {
                var text = new EntitySiriusText("romans2.cxf", EntitySiriusText.LetterSpaces.Fixed, 0, 1, 0.5,
                    $"0123456789{Environment.NewLine}AaBbFfGgHhJj{Environment.NewLine}~!@#$%^&*()_+", 10);

                text.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 10.0);
                document.ActivePage?.ActiveLayer?.AddChild(text);
            }
        }

        /// <summary>
        /// Creates a nested group containing multiple polylines and sub-groups.
        /// </summary>
        private void groupIngroup_testcase(IDocument document)
        {
            var rng = new Random((int)DateTime.Now.Ticks);
            var group = new EntityGroup { Name = "TestGroup" };

            for (int i = 0; i < 5; i++)
            {
                int VERT_COUNT = 3 + (int)(rng.NextDouble() * 5);
                var tempVerts = new List<Vertex2D>(VERT_COUNT);
                for (int v = 0; v < VERT_COUNT; v++)
                {
                    double x = rng.NextDouble() * 20.0 - 10.0;
                    double y = rng.NextDouble() * 20.0 - 10.0;
                    double b = rng.NextDouble() * 0.1;
                    tempVerts.Add(new Vertex2D(x, y, b));
                }

                var poly = new EntityPolyline2D(tempVerts, true)
                {
                    ColorMode = EntityModelBase.ColorModes.Model,
                    ModelColor = new Vector3d(rng.NextDouble() + 0.4, rng.NextDouble() * 0.5, rng.NextDouble() + 0.4)
                };

                poly.Rotate(rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0);
                poly.Scale(rng.NextDouble() * 2.0 + 0.5, rng.NextDouble() * 2.0 + 0.5, rng.NextDouble() * 2.0 + 0.5);
                poly.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 10.0);

                group.AddChild(poly);
            }

            for (int i = 0; i < 2; i++)
            {
                var subGroup = new EntityGroup(2) { Name = $"SubGroup{i}" };

                int VERT_COUNT = 5 + (int)(rng.NextDouble() * 5);
                var tempVerts = new List<Vertex2D>(VERT_COUNT);
                for (int v = 0; v < VERT_COUNT; v++)
                {
                    double x = rng.NextDouble() * 20.0 - 10.0;
                    double y = rng.NextDouble() * 20.0 - 10.0;
                    double b = rng.NextDouble() * 0.1;
                    tempVerts.Add(new Vertex2D(x, y, b));
                }

                var poly = new EntityPolyline2D(tempVerts, true)
                {
                    ColorMode = EntityModelBase.ColorModes.Model,
                    ModelColor = new Vector3d(rng.NextDouble() + 0.4, rng.NextDouble() * 0.5, rng.NextDouble() + 0.4)
                };

                poly.Rotate(rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0);
                poly.Scale(rng.NextDouble() * 2.0 + 0.5, rng.NextDouble() * 2.0 + 0.5, rng.NextDouble() * 2.0 + 0.5);
                poly.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 10.0);

                subGroup.AddChild(poly);
                group.AddChild(subGroup);
            }

            document.ActivePage?.ActiveLayer?.AddChild(group);
        }

        /// <summary>
        /// Adds a set of spheres with Z height-map coloring.
        /// </summary>
        private void sphere_testcase(IDocument document)
        {
            var rng = new Random((int)DateTime.Now.Ticks);
            const int ENTITY_COUNT = 100;
            var group = new EntityGroup();

            for (int i = 0; i < ENTITY_COUNT; i++)
            {
                var entity = new EntitySphere(new Vector3d(0, 0, 0), 3)
                {
                    Segments = 24,
                    ColorMode = EntityModelBase.ColorModes.ZHeightMap,
                    ZRange = new Vector2d(-5, 5)
                };

                entity.Rotate(rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0);
                entity.Scale(rng.NextDouble() * 2.0 + 0.5, rng.NextDouble() * 2.0 + 0.5, rng.NextDouble() * 2.0 + 0.5);
                entity.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0 + 100, rng.NextDouble() * 10.0 - 5.0);

                group.AddChild(entity);
            }

            document.ActivePage?.ActiveLayer?.AddChild(group);
        }

        /// <summary>
        /// Creates a block from an entity and inserts multiple block instances.
        /// </summary>
        private void block_insert_testcase(IDocument document)
        {
            var rng = new Random((int)DateTime.Now.Ticks);

            {
                var entity = new EntitySpiral(Vector3d.Zero, 5, 10, 2, 5, true);
                document.ActBlock(new IEntity[] { entity }, "Block1");
            }

            {
                double dx = 0;
                double dy = 0;
                List<IEntity> entities = new(3 * 5);

                for (int y = 0; y < 2; y++)
                {
                    for (int x = 0; x < 5; x++)
                    {
                        var insert = new EntityBlockInsert($"BlockInsert{x},{y}", "Block1", new Vector3d(dx, dy - 50, 0));
                        insert.Rotate(rng.NextDouble() * 60.0 - 30.0, rng.NextDouble() * 60.0 - 30.0, rng.NextDouble() * 60.0 - 30.0);
                        insert.Scale(rng.NextDouble() + 0.2, rng.NextDouble() + 0.2, rng.NextDouble() + 0.2);

                        entities.Add(insert);
                        dx += 10;
                    }
                    dx = 0;
                    dy += 11;
                }

                document.ActivePage?.ActiveLayer?.AddChildren(entities.ToArray());
            }
        }

        /// <summary>
        /// Adds an image entity if the sample image exists.
        /// </summary>
        private void image_testcase(IDocument document)
        {
            var rng = new Random((int)DateTime.Now.Ticks);
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sample\\lena.bmp");
            if (!File.Exists(fileName)) return;

            var image = new EntityImage(fileName, 10);
            image.Rotate(rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0);
            image.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 10.0);

            document.ActivePage?.ActiveLayer?.AddChild(image);
        }

        /// <summary>
        /// Adds several large line batches to test performance.
        /// </summary>
        private void large_lines_testcase(IDocument document)
        {
            // Pack 1
            {
                const int LINE_COUNT = 10000;
                const double LINE_LENGTH = 10;
                const double LINE_GAP = 0.01;
                List<Vector3d> lines = new(LINE_COUNT * 2);

                double dx = -80;
                double dy = -10;
                for (int i = 0; i < LINE_COUNT; i++)
                {
                    var start = new Vector3d(0 + dx, LINE_GAP * i + dy, 0);
                    var end = new Vector3d(LINE_LENGTH + dx, LINE_GAP * i + dy, 0);
                    lines.Add(start);
                    lines.Add(end);
                }
                var entity = new EntityLines(lines) { Alpha = 0.9f };
                document.ActivePage?.ActiveLayer?.AddChild(entity);
            }

            // Pack 2
            {
                const int LINE_COUNT = 1000;
                const double LINE_LENGTH = 5;
                const double LINE_GAP = 0.05;
                List<Vector3d> lines = new(LINE_COUNT * 2);

                double dx = 80;
                double dy = -10;
                for (int i = 0; i < LINE_COUNT; i++)
                {
                    var start = new Vector3d(0 + dx, LINE_GAP * i + dy, 0);
                    var end = new Vector3d(LINE_LENGTH + dx, LINE_GAP * i + dy, 0);
                    lines.Add(start);
                    lines.Add(end);
                }
                var entity = new EntityLines(lines);
                entity.Translate(0, 0, 1);
                document.ActivePage?.ActiveLayer?.AddChild(entity);
            }

            // Pack 3 (Z stacked)
            {
                const int LINE_COUNT = 100;
                const double LINE_LENGTH = 20;
                const double LINE_GAP = 0.05;
                List<Vector3d> lines = new(LINE_COUNT * 2);

                double dx = -10;
                double dy = -60;
                for (int i = 0; i < LINE_COUNT; i++)
                {
                    var start = new Vector3d(0 + dx, 0 + dy, LINE_GAP * i);
                    var end = new Vector3d(LINE_LENGTH + dx, 0 + dy, LINE_GAP * i);
                    lines.Add(start);
                    lines.Add(end);
                }
                var entity = new EntityLines(lines);
                entity.Translate(0, 0, 1);
                document.ActivePage?.ActiveLayer?.AddChild(entity);
            }
        }

        /// <summary>
        /// Adds 1D/2D barcode examples with transforms and hatch.
        /// </summary>
        private void barcode_testcase(IDocument document)
        {
            var rng = new Random((int)DateTime.Now.Ticks);

            {
                var entity = new EntityBarcode1D("1234567890", EntityBarcode1D.Barcode1DFormats.Code128, 5, 5, 1);
                entity.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 10.0 - 2.0);
                document.ActivePage?.ActiveLayer?.AddChild(entity);
            }

            {
                var entity = new EntityQRCode("01234567890123456789", EntityBarcode2DBase.Barcode2DCells.Lines, 5, 5, 5);
                entity.Rotate(rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0);
                entity.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 10.0 - 2.0);
                document.ActivePage?.ActiveLayer?.AddChild(entity);
            }

            {
                var entity = new EntityDataMatrix("01234567890123456789", EntityBarcode2DBase.Barcode2DCells.Dots, 5, 5, 5);
                entity.Rotate(rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0);
                entity.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 10.0 - 2.0);
                document.ActivePage?.ActiveLayer?.AddChild(entity);
            }

            {
                var entity = new EntityPDF417("01234567890123456789", EntityBarcode2DBase.Barcode2DCells.Outline, 1, 5, 5);
                entity.Rotate(rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0);
                entity.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 10.0 - 2.0);

                var hatch = HatchFactory.CreateLine(45, 0.1);
                entity.AddHatch(hatch);

                document.ActivePage?.ActiveLayer?.AddChild(entity);
            }
        }

        /// <summary>
        /// Adds a sample ZPL label as an image entity.
        /// </summary>
        private void zpl_testcase(IDocument document)
        {
            var sb = new StringBuilder();
            sb.Append("^XA");
            sb.Append("^FX Top section with logo, name and address.");
            sb.Append("^CF0,60");
            sb.Append("^FO50,50^GB100,100,100^FS");
            sb.Append("^FO75,75^FR^GB100,100,100^FS");
            sb.Append("^FO93,93^GB40,40,40^FS");
            sb.Append("^FO220,50^FDIntershipping, Inc.^FS");
            sb.Append("^CF0,30");
            sb.Append("^FO220,115^FD1000 Shipping Lane^FS");
            sb.Append("^FO220,155^FDShelbyville TN 38102^FS");
            sb.Append("^FO220,195^FDUnited States (USA)^FS");
            sb.Append("^FO50,250^GB700,3,3^FS");
            sb.Append("^FX Second section with recipient address and permit information.");
            sb.Append("^CFA,30");
            sb.Append("^FO50,300^FDJohn Doe^FS");
            sb.Append("^FO50,340^FD100 Main Street^FS");
            sb.Append("^FO50,380^FDSpringfield TN 39021^FS");
            sb.Append("^FO50,420^FDUnited States (USA)^FS");
            sb.Append("^CFA,15");
            sb.Append("^FO600,300^GB150,150,3^FS");
            sb.Append("^FO638,340^FDPermit^FS");
            sb.Append("^FO638,390^FD123456^FS");
            sb.Append("^FO50,500^GB700,3,3^FS");
            sb.Append("^FX Third section with bar code.");
            sb.Append("^BY5,2,270");
            sb.Append("^FO100,550^BC^FD12345678^FS");
            sb.Append("^FX Fourth section (the two boxes on the bottom).");
            sb.Append("^FO50,900^GB700,250,3^FS");
            sb.Append("^FO400,900^GB3,250,3^FS");
            sb.Append("^CF0,40");
            sb.Append("^FO100,960^FDCtr. X34B-1^FS");
            sb.Append("^FO100,1010^FDREF1 F00B47^FS");
            sb.Append("^FO100,1060^FDREF2 BL4H8^FS");
            sb.Append("^CF0,190");
            sb.Append("^FO470,955^FDCA^FS");
            sb.Append("^XZ");

            var zplText = sb.ToString();
            var entity = new EntityImageZPL(4 * 25.4, 6 * 25.4, zplText, EntityImageZPL.DotsPerMMs.Dots8_203DPI);
            document.ActivePage?.ActiveLayer?.AddChild(entity);
        }
        private void lissajous_testcase(IDocument document)
        {
            var entity = new EntityLissajous(DVec3.Zero, 10, 2, 12, EntityLissajous.LissajousTypes.¥ð, EntityLissajous.Directions.Cw);
            document.ActivePage?.ActiveLayer?.AddChild(entity);
        }
        /// <summary>
        /// (Optional) Demonstrates adding Gerber entities (paths are placeholders).
        /// </summary>
        private void gerber_testcase(IDocument document)
        {
            // Update the file paths below before enabling.
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sample\\LED-seven-segment.GBS");
            if (!File.Exists(fileName)) return;
            var gerber = new EntityGerber(fileName);
            document.ActivePage?.ActiveLayer?.AddChild(gerber);
        }

        #endregion
    }
}
