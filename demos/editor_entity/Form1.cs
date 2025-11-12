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
           
            this.btnSpline.Click += (s, e) => {
                var document = siriusEditorControl1.Document;
                bezierSpline_testcase(document);
                catmullRomSpline_testcase(document);
                hermiteSpline_testcase(document);
                nurbsSpline_testcase(document);
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
                many_lines_testcase(document);
            };
            this.btnBarcode.Click += (s, e) => {
                var document = siriusEditorControl1.Document;
                barcode_testcase(document);
            };
            this.btnGroup.Click += (s, e) => {
                var document = siriusEditorControl1.Document;
                mixed_group_testcase(document);
                uniform_group_testcase(document);
            };
            this.btn3DMesh.Click += (s, e) => {
                var document = siriusEditorControl1.Document;
                sphere_testcase(document);
                cube_cylinder_testcase(document);
                stl_testcase(document);
                obj_testcase(document);
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
            this.btnSpiral.Click += (s, e) => {
                var document = siriusEditorControl1.Document;
                spiral_testcase(document);
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
            var tempVerts = new List<DVec3>(VERT_COUNT);
            for (int v = 0; v < VERT_COUNT; v++)
            {
                double x = rng.NextDouble() * 6.0 - 3.0;
                double y = rng.NextDouble() * 6.0 - 3.0;
                double z = rng.NextDouble();
                tempVerts.Add(new DVec3(x, y, z));
            }

            var points = EntityFactory.CreatePoints(tempVerts);
            points.ColorMode = EntityModelBase.ColorModes.Model;
            points.ModelColor = new DVec3(rng.NextDouble() + 0.8, rng.NextDouble() * 0.5, rng.NextDouble() + 0.4);

            double tx = rng.NextDouble() * 100.0 - 50.0;
            double ty = rng.NextDouble() * 100.0 - 50.0;
            double tz = rng.NextDouble() * 10.0;
            points.Translate(tx, ty, tz);

            document.ActivePage?.ActiveLayer?.AddChild(points);
            siriusEditorControl1.View?.DoRender();
        }

        /// <summary>
        /// Adds line, arc, trepan samples with random transforms.
        /// </summary>
        private void line_arc_testcase(IDocument document)
        {
            var rng = new Random((int)DateTime.Now.Ticks);

            {
                var entity = EntityFactory.CreateLine(new DVec3(0, 0, 0), new DVec3(10, 10, 1));
                document.ActivePage?.ActiveLayer?.AddChild(entity);
            }

            {
                var entity =  EntityFactory.CreateArc(new DVec3(0, 0, 0), 5, 0, 360);
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
                var entity = EntityFactory.CreateTrepan(new DVec3(0, 0, 0), 5, 10, 10);
                double tx = rng.NextDouble() * 100.0 - 50.0;
                double ty = rng.NextDouble() * 100.0 - 50.0;
                double tz = rng.NextDouble() * 10.0;
                entity.Translate(tx, ty, tz);

                document.ActivePage?.ActiveLayer?.AddChild(entity);
            }
            siriusEditorControl1.View?.DoRender();
        }

        /// <summary>
        /// Adds triangle/rectangle/cross with random transforms.
        /// </summary>
        private void triangle_rectangle_testcase(IDocument document)
        {
            var rng = new Random((int)DateTime.Now.Ticks);

            {
                var entity = EntityFactory.CreateTriangle(new DVec3(0, 0, 0), 3, 2);
                entity.Rotate(rng.NextDouble() * 10 - 5.0, rng.NextDouble() * 10 - 5.0, rng.NextDouble() * 10 - 5.0);
                entity.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 10.0);
                document.ActivePage?.ActiveLayer?.AddChild(entity);
            }

            {
                var entity = EntityFactory.CreateRectangle(new DVec3(0, 0, 0), 4, 3);
                entity.Rotate(rng.NextDouble() * 10 - 5.0, rng.NextDouble() * 10 - 5.0, rng.NextDouble() * 10 - 5.0);
                entity.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 10.0);
                document.ActivePage?.ActiveLayer?.AddChild(entity);
            }

            {
                var entity = EntityFactory.CreateCross(new DVec3(0, 0, 0), 10, 10, 2);
                entity.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 10.0);
                document.ActivePage?.ActiveLayer?.AddChild(entity);
            }
            siriusEditorControl1.View?.DoRender();
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

                var poly = EntityFactory.CreatePolyline2D(tempVerts, true);
                poly.ColorMode = EntityModelBase.ColorModes.Model;
                poly.ModelColor = new DVec3(rng.NextDouble() + 0.4, rng.NextDouble() * 0.5, rng.NextDouble() + 0.4);
                poly.Rotate(rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0);
                poly.Scale(rng.NextDouble() * 2.0 + 0.5, rng.NextDouble() * 2.0 + 0.5, rng.NextDouble() * 2.0 + 0.5);
                poly.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 10.0);

                document.ActivePage?.ActiveLayer?.AddChild(poly);
            }
            siriusEditorControl1.View?.DoRender();
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
                var tempVerts = new List<DVec3>(VERT_COUNT);
                for (int v = 0; v < VERT_COUNT; v++)
                {
                    double x = rng.NextDouble() * 10.0 - 5.0;
                    double y = rng.NextDouble() * 10.0 - 5.0;
                    double z = rng.NextDouble() * 10.0 - 5.0;
                    tempVerts.Add(new DVec3(x, y, z));
                }

                var poly = EntityFactory.CreatePolyline3D(tempVerts, true);
                poly.ColorMode = EntityModelBase.ColorModes.Model;
                poly.ModelColor = new DVec3(rng.NextDouble() + 0.4, rng.NextDouble() * 0.5, rng.NextDouble() + 0.4);
                poly.Rotate(rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0);
                poly.Scale(rng.NextDouble() * 2.0 + 0.5, rng.NextDouble() * 2.0 + 0.5, rng.NextDouble() * 2.0 + 0.5);
                poly.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 10.0);

                document.ActivePage?.ActiveLayer?.AddChild(poly);
            }
            siriusEditorControl1.View?.DoRender();
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
                var tempVerts = new List<DVec3>(VERT_COUNT);
                for (int v = 0; v < VERT_COUNT; v++)
                {
                    double x = rng.NextDouble() * 10.0 - 5.0;
                    double y = rng.NextDouble() * 10.0 - 5.0;
                    double z = rng.NextDouble() * 10.0 - 5.0;
                    tempVerts.Add(new DVec3(x, y, z));
                }

                var spline = EntityFactory.CreateBezierSpline(tempVerts);
                spline.ColorMode = EntityModelBase.ColorModes.Model;
                spline.ModelColor = new DVec3(rng.NextDouble() + 0.4, rng.NextDouble() * 0.5, rng.NextDouble() + 0.4);

                spline.Rotate(rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0);
                spline.Scale(rng.NextDouble() * 2.0 + 0.5, rng.NextDouble() * 2.0 + 0.5, rng.NextDouble() * 2.0 + 0.5);
                spline.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 10.0);

                document.ActivePage?.ActiveLayer?.AddChild(spline);
            }
            siriusEditorControl1.View?.DoRender();
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
                var tempVerts = new List<DVec3>(VERT_COUNT);
                for (int v = 0; v < VERT_COUNT; v++)
                {
                    double x = rng.NextDouble() * 10.0 - 5.0;
                    double y = rng.NextDouble() * 10.0 - 5.0;
                    double z = rng.NextDouble() * 10.0 - 5.0;
                    tempVerts.Add(new DVec3(x, y, z));
                }

                var spline = EntityFactory.CreateCatmullRomSpline(tempVerts);
                spline.ColorMode = EntityModelBase.ColorModes.Model;
                spline.ModelColor = new DVec3(rng.NextDouble() + 0.4, rng.NextDouble() * 0.5, rng.NextDouble() + 0.4);

                spline.Rotate(rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0);
                spline.Scale(rng.NextDouble() * 2.0 + 0.5, rng.NextDouble() * 2.0 + 0.5, rng.NextDouble() * 2.0 + 0.5);
                spline.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 10.0);

                document.ActivePage?.ActiveLayer?.AddChild(spline);
            }
            siriusEditorControl1.View?.DoRender();
        }
        private void hermiteSpline_testcase(IDocument document)
        {
            var rng = new Random((int)DateTime.Now.Ticks);
            const int ENTITY_COUNT = 5;

            for (int i = 0; i < ENTITY_COUNT; i++)
            {
                int VERT_COUNT = 5 + (int)(rng.NextDouble() * 5); // 5~9
                var verts = new List<DVec3>(VERT_COUNT);

                // 랜덤 정점 생성 (-5~5)
                for (int v = 0; v < VERT_COUNT; v++)
                {
                    double x = rng.NextDouble() * 10.0 - 5.0;
                    double y = rng.NextDouble() * 10.0 - 5.0;
                    double z = rng.NextDouble() * 10.0 - 5.0;
                    verts.Add(new DVec3(x, y, z));
                }

                // C1 연속성을 위한 탄젠트: 중앙은 중앙차분, 끝점은 전/후방차분
                var tangents = new List<DVec3>(VERT_COUNT);
                double tension = 0.5; // 0~1: 값이 작을수록 부드럽게
                for (int v = 0; v < VERT_COUNT; v++)
                {
                    DVec3 t;
                    if (v == 0)
                        t = verts[1] - verts[0];
                    else if (v == VERT_COUNT - 1)
                        t = verts[VERT_COUNT - 1] - verts[VERT_COUNT - 2];
                    else
                        t = 0.5 * (verts[v + 1] - verts[v - 1]);

                    tangents.Add(tension * t);
                }

                var spline = EntityFactory.CreateHermiteSpline(verts, tangents);
                spline.ColorMode = EntityModelBase.ColorModes.Model;
                spline.ModelColor = new DVec3(rng.NextDouble() + 0.4, rng.NextDouble() * 0.5, rng.NextDouble() + 0.4);

                spline.Rotate(rng.NextDouble() * 10.0 - 5.0,
                              rng.NextDouble() * 10.0 - 5.0,
                              rng.NextDouble() * 10.0 - 5.0);
                spline.Scale(rng.NextDouble() * 2.0 + 0.5,
                             rng.NextDouble() * 2.0 + 0.5,
                             rng.NextDouble() * 2.0 + 0.5);
                spline.Translate(rng.NextDouble() * 100.0 - 50.0,
                                 rng.NextDouble() * 100.0 - 50.0,
                                 rng.NextDouble() * 100.0 - 10.0);

                document.ActivePage?.ActiveLayer?.AddChild(spline);
            }

            siriusEditorControl1.View?.DoRender();
        }
        private void nurbsSpline_testcase(IDocument document)
        {
            var rng = new Random((int)DateTime.Now.Ticks);
            const int ENTITY_COUNT = 5;

            for (int i = 0; i < ENTITY_COUNT; i++)
            {
                int degree = 3; // cubic
                int CTRL_COUNT = Math.Max(6, 6 + (int)(rng.NextDouble() * 5)); // 6~10 (degree+1 보다 크게)
                var ctrl = new List<DVec3>(CTRL_COUNT);
                var weights = new List<double>(CTRL_COUNT);

                // 랜덤 제어점 (-5~5)
                for (int c = 0; c < CTRL_COUNT; c++)
                {
                    double x = rng.NextDouble() * 10.0 - 5.0;
                    double y = rng.NextDouble() * 10.0 - 5.0;
                    double z = rng.NextDouble() * 10.0 - 5.0;
                    ctrl.Add(new DVec3(x, y, z));

                    // 가중치(양수): 0.5 ~ 2.0
                    weights.Add(0.5 + rng.NextDouble() * 1.5);
                }

                // 균일 클램프 노트 벡터 (길이 = nCtrlPts + degree + 1)
                var knots = BuildUniformClampedKnots(CTRL_COUNT, degree);

                var spline = EntityFactory.CreateNURBSpline(ctrl, knots, weights, degree);
                spline.ColorMode = EntityModelBase.ColorModes.Model;
                spline.ModelColor = new DVec3(rng.NextDouble() + 0.4, rng.NextDouble() * 0.5, rng.NextDouble() + 0.4);

                spline.Rotate(rng.NextDouble() * 10.0 - 5.0,
                              rng.NextDouble() * 10.0 - 5.0,
                              rng.NextDouble() * 10.0 - 5.0);
                spline.Scale(rng.NextDouble() * 2.0 + 0.5,
                             rng.NextDouble() * 2.0 + 0.5,
                             rng.NextDouble() * 2.0 + 0.5);
                spline.Translate(rng.NextDouble() * 100.0 - 50.0,
                                 rng.NextDouble() * 100.0 - 50.0,
                                 rng.NextDouble() * 100.0 - 10.0);

                document.ActivePage?.ActiveLayer?.AddChild(spline);
            }

            siriusEditorControl1.View?.DoRender();

            /// <summary>
            /// Open/Clamped uniform knot vector.
            /// Length = nCtrlPts + degree + 1
            /// Start 'degree' zeros, end 'degree' max, interiors uniformly spaced.
            /// </summary>
            List<double> BuildUniformClampedKnots(int nCtrlPts, int degree)
            {
                if (nCtrlPts < degree + 1)
                    throw new ArgumentException("nCtrlPts must be >= degree + 1.");

                int knotCount = nCtrlPts + degree + 1; // standard open knot vector length
                int interiorCount = knotCount - 2 * (degree + 1); // number of interior knots

                var knots = new List<double>(knotCount);

                // 앞쪽 클램프
                for (int i = 0; i <= degree; i++)
                    knots.Add(0.0);

                // 내부 균일 분포
                if (interiorCount > 0)
                {
                    for (int i = 1; i <= interiorCount; i++)
                        knots.Add((double)i / (interiorCount + 1));
                }

                // 뒤쪽 클램프
                for (int i = 0; i <= degree; i++)
                    knots.Add(1.0);

                return knots;
            }
        }

        /// <summary>
        /// Adds multiple text variants (GDI, image, circular, cxf) with transforms.
        /// </summary>
        private void text_testcase(IDocument document)
        {
            var rng = new Random((int)DateTime.Now.Ticks);

            {
                var text = EntityFactory.CreateText("Arial", FontStyle.Regular, $"0123456789{Environment.NewLine}AaBbFfGgHhJj{Environment.NewLine}~!@#$%^&*()_+", 10);
                text.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 10.0);
                document.ActivePage?.ActiveLayer?.AddChild(text);
            }

            {
                var text = EntityFactory.CreateText("Segoe UI", FontStyle.Regular, $"스파이럴랩{Environment.NewLine}SIRIUS3{Environment.NewLine}개발자 버전", 12);
                text.Rotate(rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0);
                text.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 10.0);
                document.ActivePage?.ActiveLayer?.AddChild(text);
            }

            {
                var text = EntityFactory.CreateImageText("Segoe UI",
                    FontStyle.Regular,
                    true,
                    $"0123456789{Environment.NewLine}AaBbFfGgHhJj{Environment.NewLine}~!@#$%^&*()_+",
                    50, 1, 20);
                text.Rotate(rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0);
                text.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 10.0);
                document.ActivePage?.ActiveLayer?.AddChild(text);
            }

            {
                var text = EntityFactory.CreateCircularText("Segoe UI", 
                    FontStyle.Regular, TextCircularDirections.ClockWise, 30, 90,
                    $"0123456789{Environment.NewLine}AaBbFfGgHhJj{Environment.NewLine}~!@#$%^&*()_+", 5);

                text.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 10.0);
                document.ActivePage?.ActiveLayer?.AddChild(text);
            }

            {
                var text = EntityFactory.CreateSiriusText("romans2.cxf", //or .lff
                    EntitySiriusText.LetterSpaces.Fixed, 
                    $"0123456789{Environment.NewLine}AaBbFfGgHhJj{Environment.NewLine}~!@#$%^&*()_+", 10);

                text.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 10.0);
                document.ActivePage?.ActiveLayer?.AddChild(text);
            }
            siriusEditorControl1.View?.DoRender();
        }

        /// <summary>
        /// Creates a nested group containing multiple polylines and sub-groups.
        /// </summary>
        private void mixed_group_testcase(IDocument document)
        {
            var rng = new Random((int)DateTime.Now.Ticks);
            List<IEntity> list = new List<IEntity>();

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

                var poly = EntityFactory.CreatePolyline2D(tempVerts, true);
                poly.ColorMode = EntityModelBase.ColorModes.Model;
                poly.ModelColor = new DVec3(rng.NextDouble() + 0.4, rng.NextDouble() * 0.5, rng.NextDouble() + 0.4);

                poly.Rotate(rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0);
                poly.Scale(rng.NextDouble() * 2.0 + 0.5, rng.NextDouble() * 2.0 + 0.5, rng.NextDouble() * 2.0 + 0.5);
                poly.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 10.0);
                list.Add(poly);
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

                var poly = EntityFactory.CreatePolyline2D(tempVerts, true);
                poly.ColorMode = EntityModelBase.ColorModes.Model;
                poly.ModelColor = new DVec3(rng.NextDouble() + 0.4, rng.NextDouble() * 0.5, rng.NextDouble() + 0.4);

                poly.Rotate(rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0);
                poly.Scale(rng.NextDouble() * 2.0 + 0.5, rng.NextDouble() * 2.0 + 0.5, rng.NextDouble() * 2.0 + 0.5);
                poly.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 10.0);

                subGroup.AddChild(poly);
                list.Add(subGroup);
            }
            var group = EntityFactory.CreateGroup("TestGroup", list);
            group.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 5);

            document.ActivePage?.ActiveLayer?.AddChild(group);
            siriusEditorControl1.View?.DoRender();
        }

        private void uniform_group_testcase(IDocument document)
        {
            var rng = new Random((int)DateTime.Now.Ticks);
            const int ENTITY_COUNT = 100; //large amount

            List<EntityModelBase> entities = new List<EntityModelBase>(ENTITY_COUNT);
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

                var poly = EntityFactory.CreatePolyline2D(tempVerts, true);
                poly.ColorMode = EntityModelBase.ColorModes.Model;
                poly.ModelColor = new DVec3(rng.NextDouble() + 0.4, rng.NextDouble() * 0.5, rng.NextDouble() + 0.4);

                poly.Rotate(rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0);
                poly.Scale(rng.NextDouble() * 2.0 + 0.5, rng.NextDouble() * 2.0 + 0.5, rng.NextDouble() * 2.0 + 0.5);
                poly.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 10.0);

                entities.Add(poly);
            }
            var group = EntityFactory.CreateUniformGroup("Group", entities);
            document.ActivePage?.ActiveLayer?.AddChild(group);

            siriusEditorControl1.View?.DoRender();
        }

        /// <summary>
        /// Adds a set of spheres with Z height-map coloring.
        /// </summary>
        private void sphere_testcase(IDocument document)
        {
            var rng = new Random((int)DateTime.Now.Ticks);
            const int ENTITY_COUNT = 100;

            var list = new List<EntitySphere>(ENTITY_COUNT);

            for (int i = 0; i < ENTITY_COUNT; i++)
            {
                var entity = EntityFactory.CreateSphere(new DVec3(0, 0, 0), 3);
                entity.Segments = 24;
                entity.ColorMode = EntityModelBase.ColorModes.ZHeightMap;
                entity.ZRange = new DVec2(-5, 5);

                entity.Rotate(rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0);
                entity.Scale(rng.NextDouble() * 2.0 + 0.5, rng.NextDouble() * 2.0 + 0.5, rng.NextDouble() * 2.0 + 0.5);
                entity.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0 + 100, rng.NextDouble() * 10.0 - 5.0);

                list.Add(entity);
            }

            var group = EntityFactory.CreateUniformGroup("Group", list);
            group.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0 + 100, rng.NextDouble() * 2);
            
            document.ActivePage?.ActiveLayer?.AddChild(group);
            siriusEditorControl1.View?.DoRender();
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
                var cube = EntityFactory.CreateCube(DVec3.Zero, rng.NextDouble() * 5, rng.NextDouble() * 6, rng.NextDouble() * 2);
                cube.ColorMode = EntityModelBase.ColorModes.Model;
                cube.ModelColor = new DVec3(rng.NextDouble() + 0.8, rng.NextDouble() * 0.5, rng.NextDouble());
                cube.Rotate(rng.NextDouble() * 60.0 - 30.0, rng.NextDouble() * 60.0 - 30.0, rng.NextDouble() * 60.0 - 30.0);
                cube.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 10.0);
                document.ActivePage?.ActiveLayer?.AddChild(cube);

                var cyl = EntityFactory.CreateCylinder(DVec3.Zero, rng.NextDouble() * 10, rng.NextDouble() * 10);
                cyl.ColorMode = EntityModelBase.ColorModes.Model;
                cyl.ModelColor = new DVec3(rng.NextDouble() * 0.5, rng.NextDouble() * 0.7, rng.NextDouble() + 0.5);
                cyl.Rotate((rng.NextDouble() * 60.0 - 30.0), (rng.NextDouble() * 60.0 - 30.0), (rng.NextDouble() * 60.0 - 30.0));
                cyl.Translate((rng.NextDouble() * 100.0 - 50.0), (rng.NextDouble() * 100.0 - 50.0), (rng.NextDouble() * 100.0 - 10.0));
                document.ActivePage?.ActiveLayer?.AddChild(cyl);
            }
            siriusEditorControl1.View?.DoRender();
        }

        private void stl_testcase(IDocument document)
        {
            var rng = new Random((int)DateTime.Now.Ticks);
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sample\\stl\\11_-_Main_Fan_1.stl");
            if (!File.Exists(fileName)) 
                return;

            bool success = EntityFactory.CreateMesh(fileName, out var mesh);
            Debug.Assert(success);

            mesh.Rotate(rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0);
            mesh.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, 0);
            document.ActivePage?.ActiveLayer?.AddChild(mesh);
            siriusEditorControl1.View?.DoRender();
        }

        private void obj_testcase(IDocument document)
        {
            var rng = new Random((int)DateTime.Now.Ticks);
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sample\\obj\\teapot.obj");
            if (!File.Exists(fileName)) 
                return;

            bool success = EntityFactory.CreateMesh(fileName, out var mesh);
            Debug.Assert(success);

            mesh.Rotate(rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0);
            mesh.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, 0);
            document.ActivePage?.ActiveLayer?.AddChild(mesh);
            siriusEditorControl1.View?.DoRender();
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
            
            var zDepths = new List<double>(ROWS * COLS);
            var center = new DVec2(COLS / 2.0 * INTERVAL, ROWS / 2.0 * INTERVAL);
            const double amplitude = 0.5;
            const double wavelength = 5;

            for (int y = 0; y < ROWS; y++)
            {
                for (int x = 0; x < COLS; x++)
                {
                    var pos = new DVec2(x * INTERVAL, y * INTERVAL);
                    double dist = (pos - center).Length;
                    double z = amplitude * Math.Sin((2 * Math.PI * dist / wavelength));
                    zDepths.Add(z);
                }
            }

            var minZ = zDepths.Min();
            var maxZ = zDepths.Max();

            var pointsCloud = EntityFactory.CreateGridCloud(ROWS, COLS, INTERVAL, zDepths, new DVec2(minZ, maxZ));
            pointsCloud.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, 0);

            document.ActivePage?.ActiveLayer?.AddChild(pointsCloud);
            siriusEditorControl1.View?.DoRender();
        }

        /// <summary>
        /// Creates a block from an entity and inserts multiple block instances.
        /// </summary>
        private void block_insert_testcase(IDocument document)
        {
            var rng = new Random((int)DateTime.Now.Ticks);
            const string masterBlockName = "Block1";

            if (!document.FindByBlockName(masterBlockName, out _))
            {
                var entity = EntityFactory.CreateSpiral(DVec3.Zero, 5, 2, 5, EntitySpiral.SpiralTypes.Archimedean, true);
                document.ActBlock(new IEntity[] { entity }, masterBlockName);
            }
            double dx = 0;
            double dy = 0;
            List<IEntity> entities = new List<IEntity>(2 * 5);

            for (int y = 0; y < 2; y++)
            {
                for (int x = 0; x < 5; x++)
                {
                    var insert = EntityFactory.CreateBlockInsert($"BlockInsert{x},{y}", masterBlockName, new DVec3(dx, dy - 50, 0));
                    insert.Scale(rng.NextDouble() + 0.2, rng.NextDouble() + 0.2, rng.NextDouble() + 0.2);
                    insert.Translate(rng.NextDouble() * 5.0, rng.NextDouble() * 5.0, 0);
                    insert.Rotate(rng.NextDouble() * 60.0 - 30.0, rng.NextDouble() * 60.0 - 30.0, rng.NextDouble() * 60.0 - 30.0);

                    entities.Add(insert);
                    dx += 10;
                }
                dx = 0;
                dy += 11;
            }

            document.ActivePage?.ActiveLayer?.AddChildren(entities.ToArray());
            siriusEditorControl1.View?.DoRender();
        }

        /// <summary>
        /// Adds an image entity if the sample image exists.
        /// </summary>
        private void image_testcase(IDocument document)
        {
            var rng = new Random((int)DateTime.Now.Ticks);
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sample\\image\\lena.bmp");
            if (!File.Exists(fileName)) return;

            var image = EntityFactory.CreateImage(fileName, 10);
            image.Rotate(rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0);
            image.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 10.0);

            document.ActivePage?.ActiveLayer?.AddChild(image);
            siriusEditorControl1.View?.DoRender();
        }

        /// <summary>
        /// Adds several large line batches to test performance.
        /// </summary>
        private void many_lines_testcase(IDocument document)
        {
            var rng = new Random((int)DateTime.Now.Ticks);

            const int LINE_COUNT = 10_000; //large amount of lines
            const double LINE_LENGTH = 10;
            const double LINE_GAP = 0.01;
            List<DVec3> lines = new(LINE_COUNT * 2);

            double dx = -80;
            double dy = -10;
            for (int i = 0; i < LINE_COUNT; i++)
            {
                var start = new DVec3(0 + dx, LINE_GAP * i + dy, 0);
                var end = new DVec3(LINE_LENGTH + dx, LINE_GAP * i + dy, 0);
                lines.Add(start);
                lines.Add(end);
            }
            var entity = EntityFactory.CreateLines(lines);
            entity.Alpha = 0.9;
            entity.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 5);
            document.ActivePage?.ActiveLayer?.AddChild(entity);

            siriusEditorControl1.View?.DoRender();
        }

        /// <summary>
        /// Adds 1D/2D barcode examples with transforms and hatch.
        /// </summary>
        private void barcode_testcase(IDocument document)
        {
            var rng = new Random((int)DateTime.Now.Ticks);

            {
                var entity = EntityFactory.CreateBarcode("1234567890", EntityBarcode1D.Barcode1DFormats.Code128, 5, 1);
                entity.DotFactor = 5;
                entity.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 10.0 - 2.0);
                document.ActivePage?.ActiveLayer?.AddChild(entity);
            }

            {
                var entity = EntityFactory.CreateQRCode("01234567890123456789", EntityBarcode2DBase.Barcode2DCells.Lines, 5, 5);
                entity.CellLine.DotFactor = 5;
                entity.Rotate(rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0);
                entity.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 10.0 - 2.0);
                document.ActivePage?.ActiveLayer?.AddChild(entity);
            }

            {
                var entity = EntityFactory.CreateDataMatrix("01234567890123456789", EntityBarcode2DBase.Barcode2DCells.Dots, 5, 5);
                entity.CellDot.DotFactor = 5;
                entity.Rotate(rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0);
                entity.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 10.0 - 2.0);
                document.ActivePage?.ActiveLayer?.AddChild(entity);
            }

            {
                var entity = EntityFactory.CreatePDF417("01234567890123456789", EntityBarcode2DBase.Barcode2DCells.Outline, 5, 5);
                entity.Rotate(rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0, rng.NextDouble() * 10.0 - 5.0);
                entity.Translate(rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 100.0 - 50.0, rng.NextDouble() * 10.0 - 2.0);

                var hatch = HatchFactory.CreateLine(45, 0.1);
                entity.AddHatch(hatch);
                document.ActivePage?.ActiveLayer?.AddChild(entity);
            }
            siriusEditorControl1.View?.DoRender();
        }

        /// <summary>
        /// Adds a sample ZPL label as an image entity.
        /// </summary>
        private void zpl_testcase(IDocument document)
        {
            var rng = new Random((int)DateTime.Now.Ticks);

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

            var entity = EntityFactory.CreateImageZPL(4 * 25.4, 6 * 25.4, zplText, EntityImageZPL.DotsPerMMs.Dots8_203DPI);
            entity.Translate(rng.NextDouble() * 100.0 - 50, rng.NextDouble() * 100.0 - 50, 0);
            document.ActivePage?.ActiveLayer?.AddChild(entity);
            siriusEditorControl1.View?.DoRender();
        }

        private void lissajous_testcase(IDocument document)
        {
            var rng = new Random((int)DateTime.Now.Ticks);
            
            var entity = EntityFactory.CreateLissajous(DVec3.Zero, 10, 2, 12, EntityLissajous.LissajousTypes.π, EntityLissajous.Directions.Cw);
            entity.Translate(rng.NextDouble() * 100.0 - 50, rng.NextDouble() * 100.0 - 50, 0);
            document.ActivePage?.ActiveLayer?.AddChild(entity);
            siriusEditorControl1.View?.DoRender();
        }

        private void spiral_testcase(IDocument document)
        {
            var rng = new Random((int)DateTime.Now.Ticks);

            {
                var entity = EntityFactory.CreateSpiralClassic(DVec3.Zero, 10, 8, 2, 10, true);
                entity.Translate(rng.NextDouble() * 100.0 - 50, rng.NextDouble() * 100.0 - 50, 0);
                document.ActivePage?.ActiveLayer?.AddChild(entity);
            }

            {
                var entity = EntityFactory.CreateSpiral(DVec3.Zero, 10, 2, 12, EntitySpiral.SpiralTypes.Archimedean, true);
                entity.Translate(rng.NextDouble() * 100.0 - 50, rng.NextDouble() * 100.0 - 50, 0);
                document.ActivePage?.ActiveLayer?.AddChild(entity);
            }
            siriusEditorControl1.View?.DoRender();
        }

        /// <summary>
        /// (Optional) Demonstrates adding Gerber entities (paths are placeholders).
        /// </summary>
        private void gerber_testcase(IDocument document)
        {
            var rng = new Random((int)DateTime.Now.Ticks);

            {
                var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sample\\gerber\\LED-seven-segment.GBS");
                if (!File.Exists(fileName)) 
                    return;
                bool success = EntityFactory.CreateGerber(fileName, SpiralLab.Sirius3.UI.Config.ScannerPenColors[0], out var gerber);
                Debug.Assert(success);
                gerber.Translate(rng.NextDouble() * 100.0 - 50, rng.NextDouble() * 100.0 - 50, 0);
                document.ActivePage?.ActiveLayer?.AddChild(gerber);
            }

            {
                var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sample\\gerber\\TRF7960_EVM (REV A).TOP");
                if (!File.Exists(fileName)) 
                    return;
                bool success = EntityFactory.CreateGerber(fileName, SpiralLab.Sirius3.UI.Config.ScannerPenColors[1], out var gerber);
                Debug.Assert(success);
                gerber.Translate(rng.NextDouble() * 100.0 - 50, rng.NextDouble() * 100.0 - 50, 0);
                document.ActivePage?.ActiveLayer?.AddChild(gerber);
            }

            // or by path
            //{
            //    var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sample\\gerber\\");
            //    bool success = EntityFactory.CreateGerbers(filePath, "*.*", out EntityGerber[] gerberEntities);
            //    Debug.Assert(success);
            //    document.ActivePage?.ActiveLayer?.AddChildren(gerberEntities);
            //}
            siriusEditorControl1.View?.DoRender();
        }
        #endregion
    }
}
