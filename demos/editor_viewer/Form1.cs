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
    /// Viewer demo (Multiple Views and Various Entities)
    /// 뷰어 데모 (다중 뷰 및 다양한 엔티티)
    /// </summary>
    public partial class Form1 : Form
    {
        // Counter for created views
        // 생성된 뷰 개수 카운터
        static int viewCounts = 0;

        /// <summary>
        /// Form constructor
        /// 폼 생성자
        /// </summary>
        public Form1()
        {
            // Initialize SIRIUS3 library
            // SIRIUS3 라이브러리 초기화
            SpiralLab.Sirius3.Core.Initialize();

            InitializeComponent();
            this.Load += Form1_Load;
            this.FormClosing += (s, e) =>
            {
                var dlgResult = MessageBox.Show(this, $"Do you really want to terminate program ?", "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dlgResult != DialogResult.Yes)
                {
                    // is marker need to abort ?
                    // 마킹 중단 여부 확인 ?
                    e.Cancel = true;
                    return;
                }

                // Dispose instances 
                // 인스턴스 해제 
                siriusEditorControl1.DisposeDevices();

                // Dispose document
                // 문서 해제
                var doc = siriusEditorControl1.Document;
                siriusEditorControl1.Document = null;
                doc?.Dispose();

                // Clean up SIRIUS3 library
                // SIRIUS3 라이브러리 정리
                SpiralLab.Sirius3.Core.Cleanup();
            };

            // Event handler to create additional viewer windows
            // 추가 뷰어 창 생성을 위한 이벤트 핸들러
            this.btnCreateView.Click += (s, e) => {
                var document = siriusEditorControl1.Document;
                
                var dynamicForm = new Form();
                dynamicForm.SuspendLayout();
                dynamicForm.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
                dynamicForm.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
                dynamicForm.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                dynamicForm.Text = "ViewerControl - (c)SpiralLab";
                dynamicForm.Size = new Size(800, 600);
                dynamicForm.StartPosition = FormStartPosition.WindowsDefaultLocation;

                // Create a ViewerControl (Read-only view)
                // ViewerControl 생성 (읽기 전용 뷰)
                var viewerControl = new SpiralLab.Sirius3.UI.WinForms.ViewerControl();
                viewerControl.Document = document; // Link to same document / 동일 문서 연결
                viewerControl.AliasName = $"MyView{++viewCounts}";
                viewerControl.Dock = DockStyle.Fill;
                viewerControl.Show();
                dynamicForm.Controls.Add(viewerControl);
                dynamicForm.ResumeLayout(false);
                dynamicForm.TopLevel = true;
                dynamicForm.Show();
            };

            // Attach test case handlers
            // 테스트 케이스 핸들러 연결
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
                bSpline_testcase(document);
                nurbSpline_testcase(document);
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

        /// <summary>
        /// Form load
        /// 폼 로드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            // Create devices
            // 장치 생성
            EditorHelper.CreateDevices(out IRtc rtc, out ILaser laser, out IDInput dInExt1, out IDInput dInLaserPort, out IDOutput dOutExt1, out IDOutput dOutExt2, out IDOutput dOutLaserPort, out IPowerMeter powerMeter, out IMarker marker);

            // Register devices to control
            // 컨트롤에 장치 등록
            siriusEditorControl1.RegisterDevices(rtc, laser, powerMeter, dInExt1, dInLaserPort, dOutExt1, dOutExt2, dOutLaserPort, marker);

            // Ready marker
            // 마커 준비
            marker.Ready(siriusEditorControl1.Document, siriusEditorControl1.View, rtc, laser, powerMeter);
        }
  

        #region Testcases (Samples)
        /// <summary>
        /// Adds a random point cloud entity.
        /// 랜덤 포인트 클라우드 엔티티 추가
        /// </summary>
        private void points_testcase(IDocument document)
        {
            var rnd = new Random((int)DateTime.Now.Ticks);

            int VERT_COUNT = 100 + (int)(rnd.NextDouble() * 100);
            var tempVerts = new List<DVec3>(VERT_COUNT);
            for (int v = 0; v < VERT_COUNT; v++)
            {
                double x = rnd.NextDouble() * 6.0 - 3.0;
                double y = rnd.NextDouble() * 6.0 - 3.0;
                double z = rnd.NextDouble();
                tempVerts.Add(new DVec3(x, y, z));
            }

            // Create Points entity
            // 포인트(Points) 엔티티 생성
            var points = EntityFactory.CreatePoints(tempVerts);
            points.ColorMode = EntityModelBase.ColorModes.Model;
            points.ModelColor = new DVec3(rnd.NextDouble() + 0.8, rnd.NextDouble() * 0.5, rnd.NextDouble() + 0.4);

            double tx = rnd.NextDouble() * 100.0 - 50.0;
            double ty = rnd.NextDouble() * 100.0 - 50.0;
            double tz = rnd.NextDouble() * 10.0;
            points.Translate(tx, ty, tz);

            document.ActAdd(points);
            siriusEditorControl1.View?.DoRender();
        }

        /// <summary>
        /// Adds line, arc, trepan samples with random transforms.
        /// 선, 호, 트레판 샘플 추가
        /// </summary>
        private void line_arc_testcase(IDocument document)
        {
            var rnd = new Random((int)DateTime.Now.Ticks);

            // Create Line
            // 선(Line) 생성
            {
                var entity = EntityFactory.CreateLine(new DVec3(0, 0, 0), new DVec3(10, 10, 1));
                document.ActAdd(entity);
            }

            // Create Arc
            // 호(Arc) 생성
            {
                var entity =  EntityFactory.CreateArc(new DVec3(0, 0, 0), 5, 0, 360);
                double rx = rnd.NextDouble() * 10 - 5.0;
                double ry = rnd.NextDouble() * 10 - 5.0;
                double rz = rnd.NextDouble() * 10 - 5.0;
                entity.Rotate(rx, ry, rz);

                double tx = rnd.NextDouble() * 100.0 - 50.0;
                double ty = rnd.NextDouble() * 100.0 - 50.0;
                double tz = rnd.NextDouble() * 100.0 - 10.0;
                entity.Translate(tx, ty, tz);

                document.ActAdd(entity);
            }

            // Create Trepan (Helix mark)
            // 트레판(Trepan, 나선 마킹) 생성
            {
                var entity = EntityFactory.CreateTrepan(new DVec3(0, 0, 0), 5, 10, 10);
                double tx = rnd.NextDouble() * 100.0 - 50.0;
                double ty = rnd.NextDouble() * 100.0 - 50.0;
                double tz = rnd.NextDouble() * 10.0;
                entity.Translate(tx, ty, tz);

                document.ActAdd(entity);
            }
            siriusEditorControl1.View?.DoRender();
        }

        /// <summary>
        /// Adds triangle/rectangle/cross with random transforms.
        /// 삼각형, 사각형, 십자가 샘플 추가
        /// </summary>
        private void triangle_rectangle_testcase(IDocument document)
        {
            var rnd = new Random((int)DateTime.Now.Ticks);

            // Create Triangle
            // 삼각형(Triangle) 생성
            {
                var entity = EntityFactory.CreateTriangle(new DVec3(0, 0, 0), 3, 2);
                entity.Rotate(rnd.NextDouble() * 10 - 5.0, rnd.NextDouble() * 10 - 5.0, rnd.NextDouble() * 10 - 5.0);
                entity.Translate(rnd.NextDouble() * 100.0 - 50.0, rnd.NextDouble() * 100.0 - 50.0, rnd.NextDouble() * 100.0 - 10.0);
                document.ActAdd(entity);
            }

            // Create Rectangle
            // 사각형(Rectangle) 생성
            {
                var entity = EntityFactory.CreateRectangle(new DVec3(0, 0, 0), 4, 3);
                entity.Rotate(rnd.NextDouble() * 10 - 5.0, rnd.NextDouble() * 10 - 5.0, rnd.NextDouble() * 10 - 5.0);
                entity.Translate(rnd.NextDouble() * 100.0 - 50.0, rnd.NextDouble() * 100.0 - 50.0, rnd.NextDouble() * 100.0 - 10.0);
                document.ActAdd(entity);
            }

            // Create Cross
            // 십자가(Cross) 생성
            {
                var entity = EntityFactory.CreateCross(new DVec3(0, 0, 0), 10, 10, 2);
                entity.Translate(rnd.NextDouble() * 100.0 - 50.0, rnd.NextDouble() * 100.0 - 50.0, rnd.NextDouble() * 10.0);
                document.ActAdd(entity);
            }
            siriusEditorControl1.View?.DoRender();
        }

        /// <summary>
        /// Adds random closed 2D polylines with transforms.
        /// 랜덤 2D 폴리라인 샘플 추가
        /// </summary>
        private void polyline2d_testcase(IDocument document)
        {
            var rnd = new Random((int)DateTime.Now.Ticks);
            const int ENTITY_COUNT = 5;

            for (int i = 0; i < ENTITY_COUNT; i++)
            {
                int VERT_COUNT = 3 + (int)(rnd.NextDouble() * 5);
                var tempVerts = new List<Vertex2D>(VERT_COUNT);
                for (int v = 0; v < VERT_COUNT; v++)
                {
                    double x = rnd.NextDouble() * 10.0 - 5.0;
                    double y = rnd.NextDouble() * 10.0 - 5.0;
                    double b = rnd.NextDouble(); // Bulge (Arc) factor / 벌지(호) 계수
                    tempVerts.Add(new Vertex2D(x, y, b));
                }

                // Create Polyline 2D
                // 2D 폴리라인 생성
                var poly = EntityFactory.CreatePolyline2D(tempVerts, true);
                poly.ColorMode = EntityModelBase.ColorModes.Model;
                poly.ModelColor = new DVec3(rnd.NextDouble() + 0.4, rnd.NextDouble() * 0.5, rnd.NextDouble() + 0.4);
                poly.Rotate(rnd.NextDouble() * 10.0 - 5.0, rnd.NextDouble() * 10.0 - 5.0, rnd.NextDouble() * 10.0 - 5.0);
                poly.Scale(rnd.NextDouble() * 2.0 + 0.5, rnd.NextDouble() * 2.0 + 0.5, rnd.NextDouble() * 2.0 + 0.5);
                poly.Translate(rnd.NextDouble() * 100.0 - 50.0, rnd.NextDouble() * 100.0 - 50.0, rnd.NextDouble() * 100.0 - 10.0);

                document.ActAdd(poly);
            }
            siriusEditorControl1.View?.DoRender();
        }

        /// <summary>
        /// Adds random closed 3D polylines with transforms.
        /// 랜덤 3D 폴리라인 샘플 추가
        /// </summary>
        private void polyline3d_testcase(IDocument document)
        {
            var rnd = new Random((int)DateTime.Now.Ticks);
            const int ENTITY_COUNT = 5;

            for (int i = 0; i < ENTITY_COUNT; i++)
            {
                int VERT_COUNT = 3 + (int)(rnd.NextDouble() * 5);
                var tempVerts = new List<DVec3>(VERT_COUNT);
                for (int v = 0; v < VERT_COUNT; v++)
                {
                    double x = rnd.NextDouble() * 10.0 - 5.0;
                    double y = rnd.NextDouble() * 10.0 - 5.0;
                    double z = rnd.NextDouble() * 10.0 - 5.0;
                    tempVerts.Add(new DVec3(x, y, z));
                }

                // Create Polyline 3D
                // 3D 폴리라인 생성
                var poly = EntityFactory.CreatePolyline3D(tempVerts, true);
                poly.ColorMode = EntityModelBase.ColorModes.Model;
                poly.ModelColor = new DVec3(rnd.NextDouble() + 0.4, rnd.NextDouble() * 0.5, rnd.NextDouble() + 0.4);
                poly.Rotate(rnd.NextDouble() * 10.0 - 5.0, rnd.NextDouble() * 10.0 - 5.0, rnd.NextDouble() * 10.0 - 5.0);
                poly.Scale(rnd.NextDouble() * 2.0 + 0.5, rnd.NextDouble() * 2.0 + 0.5, rnd.NextDouble() * 2.0 + 0.5);
                poly.Translate(rnd.NextDouble() * 100.0 - 50.0, rnd.NextDouble() * 100.0 - 50.0, rnd.NextDouble() * 100.0 - 10.0);

                document.ActAdd(poly);
            }
            siriusEditorControl1.View?.DoRender();
        }

        /// <summary>
        /// Adds random Bezier spline examples with transforms.
        /// 랜덤 베지에 스플라인 샘플 추가
        /// </summary>
        private void bezierSpline_testcase(IDocument document)
        {
            var rnd = new Random((int)DateTime.Now.Ticks);
            const int ENTITY_COUNT = 5;

            for (int i = 0; i < ENTITY_COUNT; i++)
            {
                double radius = 10.0; // Base radius / 기본 반지름
                double z = 0.0;

                // Build control points for circle
                // 원형을 위한 제어점 생성
                var ctrl = BuildBezierCircleControls(radius, z);

                // Create Bezier Spline
                // 베지에 스플라인 생성
                var spline = EntityFactory.CreateBezierSpline(ctrl);
                spline.ColorMode = EntityModelBase.ColorModes.Model;
                spline.ModelColor = new DVec3(rnd.NextDouble() + 0.4,
                                              rnd.NextDouble() * 0.5,
                                              rnd.NextDouble() + 0.4);

                spline.Translate(rnd.NextDouble() * 100.0 - 50.0,
                                 rnd.NextDouble() * 100.0 - 50.0,
                                 rnd.NextDouble() * 100.0 - 10.0);

                document.ActAdd(spline);
            }

            siriusEditorControl1.View?.DoRender();
        }

        /// <summary>
        /// Bezier circle control points generator
        /// 베지에 원형 제어점 생성기
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        List<DVec3> BuildBezierCircleControls(double radius, double z = 0.0)
        {
            double K = 4.0 / 3.0 * Math.Tan(Math.PI / 8.0); // ≈0.5522847498
            double R = radius;

            var pts = new List<DVec3>();

            // 4 quadrants with 4 segments each
            // 4개 사분면 각각 4개 세그먼트
            DVec3 p0 = new DVec3(R, 0, z);
            DVec3 p1 = new DVec3(R, K * R, z);
            DVec3 p2 = new DVec3(K * R, R, z);
            DVec3 p3 = new DVec3(0, R, z);

            DVec3 p4 = new DVec3(-K * R, R, z);
            DVec3 p5 = new DVec3(-R, K * R, z);
            DVec3 p6 = new DVec3(-R, 0, z);

            DVec3 p7 = new DVec3(-R, -K * R, z);
            DVec3 p8 = new DVec3(-K * R, -R, z);
            DVec3 p9 = new DVec3(0, -R, z);

            DVec3 p10 = new DVec3(K * R, -R, z);
            DVec3 p11 = new DVec3(R, -K * R, z);
            DVec3 p12 = new DVec3(R, 0, z); // Closed / 닫힘

            pts.Add(p0);  pts.Add(p1);  pts.Add(p2);  pts.Add(p3);
            pts.Add(p4);  pts.Add(p5);  pts.Add(p6);
            pts.Add(p7);  pts.Add(p8);  pts.Add(p9);
            pts.Add(p10); pts.Add(p11); pts.Add(p12);

            return pts;
        }

        /// <summary>
        /// Adds random Catmull-Rom spline examples with transforms.
        /// 랜덤 캣멀-롬 스플라인 샘플 추가
        /// </summary>
        private void catmullRomSpline_testcase(IDocument document)
        {
            var rnd = new Random((int)DateTime.Now.Ticks);
            const int ENTITY_COUNT = 5;

            for (int i = 0; i < ENTITY_COUNT; i++)
            {
                int CTRL_COUNT = 10;
                double radius = 10.0;
                double z = 0.0;

                var verts = BuildCirclePoints(CTRL_COUNT, radius, z);

                // Create Catmull-Rom Spline
                // 캣멀-롬 스플라인 생성
                var spline = EntityFactory.CreateCatmullRomSpline(verts, true);
                spline.ColorMode = EntityModelBase.ColorModes.Model;
                spline.ModelColor = new DVec3(rnd.NextDouble() + 0.4,
                                              rnd.NextDouble() * 0.5,
                                              rnd.NextDouble() + 0.4);

                spline.Translate(rnd.NextDouble() * 100.0 - 50.0,
                                 rnd.NextDouble() * 100.0 - 50.0,
                                 rnd.NextDouble() * 100.0 - 10.0);

                document.ActAdd(spline);
            }

            siriusEditorControl1.View?.DoRender();           
        }

        private static List<DVec3> BuildCirclePoints(int count, double radius, double z = 0.0)
        {
            var verts = new List<DVec3>(count);
            for (int i = 0; i < count; i++)
            {
                double t = 2.0 * Math.PI * i / count;
                double x = radius * Math.Cos(t);
                double y = radius * Math.Sin(t);
                verts.Add(new DVec3(x, y, z));
            }
            return verts;
        }

        /// <summary>
        /// Adds random Hermite spline examples with transforms.
        /// 랜덤 에르미트 스플라인 샘플 추가
        /// </summary>
        private void hermiteSpline_testcase(IDocument document)
        {
            var rnd = new Random((int)DateTime.Now.Ticks);
            const int ENTITY_COUNT = 5;

            for (int i = 0; i < ENTITY_COUNT; i++)
            {
                int CTRL_COUNT = 36;
                double radius = 10.0;
                double z = 0.0;

                BuildCirclePointsAndTangents(CTRL_COUNT, radius, z, out var verts, out var tangents);

                // Create Hermite Spline (Points + Tangents)
                // 에르미트 스플라인 생성 (점 + 접선 방향)
                var spline = EntityFactory.CreateHermiteSpline(verts, tangents, true);
                spline.ColorMode = EntityModelBase.ColorModes.Model;
                spline.ModelColor = new DVec3(rnd.NextDouble() + 0.4,
                                              rnd.NextDouble() * 0.5,
                                              rnd.NextDouble() + 0.4);

                spline.Translate(rnd.NextDouble() * 100.0 - 50.0,
                                 rnd.NextDouble() * 100.0 - 50.0,
                                 rnd.NextDouble() * 100.0 - 10.0);

                document.ActAdd(spline);
            }

            siriusEditorControl1.View?.DoRender();
        }

        // Hermite helper: point + tangent
        void BuildCirclePointsAndTangents(int count, double radius, double z, out List<DVec3> points, out List<DVec3> tangents)
        {
            points = new List<DVec3>(count);
            tangents = new List<DVec3>(count);
            double dTheta = 2.0 * Math.PI / count;
            double scale = dTheta; 

            for (int i = 0; i < count; i++)
            {
                double theta = dTheta * i;
                double cos = Math.Cos(theta);
                double sin = Math.Sin(theta);

                points.Add(new DVec3(radius * cos, radius * sin, z));
                tangents.Add(new DVec3(-radius * sin * scale, radius * cos * scale, 0.0));
            }
        }

        /// <summary>
        /// Adds random B-Spline examples with transforms.
        /// 랜덤 B-스플라인 샘플 추가
        /// </summary>
        private void bSpline_testcase(IDocument document)
        {
            var rnd = new Random((int)DateTime.Now.Ticks);
            const int ENTITY_COUNT = 5;

            for (int i = 0; i < ENTITY_COUNT; i++)
            {
                double radius = 10.0;
                double z = 0.0;

                BuildNurbsCircle(radius, z, out var ctrl, out var weights, out var knots, out int degree);

                // Create B-Spline
                // B-스플라인 생성
                var spline = EntityFactory.CreateBSpline(ctrl, knots, degree, false);

                spline.ColorMode = EntityModelBase.ColorModes.Model;
                spline.ModelColor = new DVec3(rnd.NextDouble() + 0.4, rnd.NextDouble() * 0.5, rnd.NextDouble() + 0.4);
                spline.Translate(rnd.NextDouble() * 100.0 - 50.0, rnd.NextDouble() * 100.0 - 50.0, rnd.NextDouble() * 100.0 - 10.0);

                document.ActAdd(spline);
            }

            siriusEditorControl1.View?.DoRender();
        }

        /// <summary>
        /// Adds random NURBS spline examples with transforms.
        /// 랜덤 넙스(NURBS) 스플라인 샘플 추가
        /// </summary>
        private void nurbSpline_testcase(IDocument document)
        {
            var rnd = new Random((int)DateTime.Now.Ticks);
            const int ENTITY_COUNT = 5;

            for (int i = 0; i < ENTITY_COUNT; i++)
            {
                double radius = 10.0;
                double z = 0.0;

                BuildNurbsCircle(radius, z, out var ctrl, out var weights, out var knots, out int degree);

                // Create NURBS Spline (Non-Uniform Rational B-Spline)
                // 넙스(NURBS) 스플라인 생성
                var spline = EntityFactory.CreateNURBSpline(ctrl, knots, weights, degree, false);

                spline.ColorMode = EntityModelBase.ColorModes.Model;
                spline.ModelColor = new DVec3(rnd.NextDouble() + 0.4, rnd.NextDouble() * 0.5, rnd.NextDouble() + 0.4);
                spline.Translate(rnd.NextDouble() * 100.0 - 50.0, rnd.NextDouble() * 100.0 - 50.0, rnd.NextDouble() * 100.0 - 10.0);

                document.ActAdd(spline);
            }

            siriusEditorControl1.View?.DoRender();
        }

        void BuildNurbsCircle(double radius, double z, out List<DVec3> controlPoints, out List<double> weights, out List<double> knots, out int degree)
        {
            degree = 2;              // quadratic
            double R = radius;
            double w = Math.Sqrt(0.5); // = 1 / sqrt(2)

            controlPoints = new List<DVec3>
            {
                new DVec3( R,  0, z),   
                new DVec3( R,  R, z),   
                new DVec3( 0,  R, z),   
                new DVec3(-R,  R, z),   
                new DVec3(-R,  0, z),   
                new DVec3(-R, -R, z),   
                new DVec3( 0, -R, z),   
                new DVec3( R, -R, z),   
                new DVec3( R,  0, z),   
            };

            weights = new List<double> { 1.0, w, 1.0, w, 1.0, w, 1.0, w, 1.0 };
            knots = new List<double> { 0.0, 0.0, 0.0, 0.25, 0.25, 0.5, 0.5, 0.75, 0.75, 1.0, 1.0, 1.0 };
        }

        /// <summary>
        /// Adds multiple text variants (GDI, image, circular, cxf) with transforms.
        /// 다양한 텍스트 샘플 추가 (GDI, 이미지, 원형, CXF 등)
        /// </summary>
        private void text_testcase(IDocument document)
        {
            var rnd = new Random((int)DateTime.Now.Ticks);

            // GDI Text
            // GDI 텍스트 생성
            {
                var text = EntityFactory.CreateText("Arial", FontStyle.Regular, $"0123456789{Environment.NewLine}AaBbFfGgHhJj{Environment.NewLine}~!@#$%^&*()_+", 10);
                text.Translate(rnd.NextDouble() * 100.0 - 50.0, rnd.NextDouble() * 100.0 - 50.0, rnd.NextDouble() * 10.0);
                document.ActAdd(text);
            }

            // Korean GDI Text
            // 한글 GDI 텍스트 생성
            {
                var text = EntityFactory.CreateText("Segoe UI", FontStyle.Regular, $"스파이럴랩{Environment.NewLine}SIRIUS3{Environment.NewLine}개발자 버전", 12);
                text.Rotate(rnd.NextDouble() * 10.0 - 5.0, rnd.NextDouble() * 10.0 - 5.0, rnd.NextDouble() * 10.0 - 5.0);
                text.Translate(rnd.NextDouble() * 100.0 - 50.0, rnd.NextDouble() * 100.0 - 50.0, rnd.NextDouble() * 100.0 - 10.0);
                document.ActAdd(text);
            }

            // Image based Text
            // 이미지 텍스트 생성
            {
                var text = EntityFactory.CreateImageText("Segoe UI", FontStyle.Regular, true, $"0123456789{Environment.NewLine}AaBbFfGgHhJj{Environment.NewLine}~!@#$%^&*()_+", 50, 1, 20);
                text.Rotate(rnd.NextDouble() * 10.0 - 5.0, rnd.NextDouble() * 10.0 - 5.0, rnd.NextDouble() * 10.0 - 5.0);
                text.Translate(rnd.NextDouble() * 100.0 - 50.0, rnd.NextDouble() * 100.0 - 50.0, rnd.NextDouble() * 100.0 - 10.0);
                document.ActAdd(text);
            }

            // Circular Text
            // 원형 텍스트 생성
            {
                var text = EntityFactory.CreateCircularText("Segoe UI", FontStyle.Regular, TextCircularDirections.ClockWise, 30, 90, $"0123456789{Environment.NewLine}AaBbFfGgHhJj{Environment.NewLine}~!@#$%^&*()_+", 5);
                text.Translate(rnd.NextDouble() * 100.0 - 50.0, rnd.NextDouble() * 100.0 - 50.0, rnd.NextDouble() * 100.0 - 10.0);
                document.ActAdd(text);
            }

            // Sirius CXF Text (Custom Plot Font)
            // 시리우스 전용 폰트(.cxf) 텍스트 생성
            {
                var text = EntityFactory.CreateSiriusText("romans2.cxf", EntitySiriusText.LetterSpaces.Fixed, $"0123456789{Environment.NewLine}AaBbFfGgHhJj{Environment.NewLine}~!@#$%^&*()_+", 10);
                text.Translate(rnd.NextDouble() * 100.0 - 50.0, rnd.NextDouble() * 100.0 - 50.0, rnd.NextDouble() * 100.0 - 10.0);
                document.ActAdd(text);
            }
            siriusEditorControl1.View?.DoRender();
        }

        /// <summary>
        /// Creates a nested group containing multiple polylines and sub-groups.
        /// 혼합 그룹(Mixed Group) 샘플 추가 (중첩 그룹 포함)
        /// </summary>
        private void mixed_group_testcase(IDocument document)
        {
            var rnd = new Random((int)DateTime.Now.Ticks);
            List<IEntity> list = new List<IEntity>();

            for (int i = 0; i < 5; i++)
            {
                int VERT_COUNT = 3 + (int)(rnd.NextDouble() * 5);
                var tempVerts = new List<Vertex2D>(VERT_COUNT);
                for (int v = 0; v < VERT_COUNT; v++)
                {
                    double x = rnd.NextDouble() * 20.0 - 10.0;
                    double y = rnd.NextDouble() * 20.0 - 10.0;
                    double b = rnd.NextDouble() * 0.1;
                    tempVerts.Add(new Vertex2D(x, y, b));
                }

                var poly = EntityFactory.CreatePolyline2D(tempVerts, true);
                poly.ColorMode = EntityModelBase.ColorModes.Model;
                poly.ModelColor = new DVec3(rnd.NextDouble() + 0.4, rnd.NextDouble() * 0.5, rnd.NextDouble() + 0.4);

                poly.Rotate(rnd.NextDouble() * 10.0 - 5.0, rnd.NextDouble() * 10.0 - 5.0, rnd.NextDouble() * 10.0 - 5.0);
                poly.Scale(rnd.NextDouble() * 2.0 + 0.5, rnd.NextDouble() * 2.0 + 0.5, rnd.NextDouble() * 2.0 + 0.5);
                poly.Translate(rnd.NextDouble() * 100.0 - 50.0, rnd.NextDouble() * 100.0 - 50.0, rnd.NextDouble() * 100.0 - 10.0);
                list.Add(poly);
            }

            for (int i = 0; i < 2; i++)
            {
                var subGroup = new EntityMixedGroup(2) { Name = $"SubGroup{i}" };
                int VERT_COUNT = 5 + (int)(rnd.NextDouble() * 5);
                var tempVerts = new List<Vertex2D>(VERT_COUNT);
                for (int v = 0; v < VERT_COUNT; v++)
                {
                    double x = rnd.NextDouble() * 20.0 - 10.0;
                    double y = rnd.NextDouble() * 20.0 - 10.0;
                    double b = rnd.NextDouble() * 0.1;
                    tempVerts.Add(new Vertex2D(x, y, b));
                }
                var poly = EntityFactory.CreatePolyline2D(tempVerts, true);
                poly.Translate(rnd.NextDouble() * 100.0 - 50.0, rnd.NextDouble() * 100.0 - 50.0, rnd.NextDouble() * 100.0 - 10.0);
                subGroup.AddChild(poly);
                list.Add(subGroup);
            }
            // Create Mixed Group
            // 혼합 그룹 생성
            var group = EntityFactory.CreateMixedGroup("TestGroup", list);
            group.Translate(rnd.NextDouble() * 100.0 - 50.0, rnd.NextDouble() * 100.0 - 50.0, rnd.NextDouble() * 5);

            document.ActAdd(group);
            siriusEditorControl1.View?.DoRender();
        }

        /// <summary>
        /// Adds a large number of uniform entities into a group for performance test.
        /// 균일 그룹(Uniform Group) 샘플 추가 (대량 데이터 성능 테스트용)
        /// </summary>
        private void uniform_group_testcase(IDocument document)
        {
            var rnd = new Random((int)DateTime.Now.Ticks);
            const int ENTITY_COUNT = 100;

            List<EntityModelBase> entities = new List<EntityModelBase>(ENTITY_COUNT);
            for (int i = 0; i < ENTITY_COUNT; i++)
            {
                int VERT_COUNT = 3 + (int)(rnd.NextDouble() * 5);
                var tempVerts = new List<Vertex2D>(VERT_COUNT);
                for (int v = 0; v < VERT_COUNT; v++)
                {
                    tempVerts.Add(new Vertex2D(rnd.NextDouble() * 10.0 - 5.0, rnd.NextDouble() * 10.0 - 5.0, rnd.NextDouble()));
                }

                var poly = EntityFactory.CreatePolyline2D(tempVerts, true);
                poly.ColorMode = EntityModelBase.ColorModes.Model;
                poly.ModelColor = new DVec3(rnd.NextDouble() + 0.4, rnd.NextDouble() * 0.5, rnd.NextDouble() + 0.4);
                poly.Translate(rnd.NextDouble() * 100.0 - 50.0, rnd.NextDouble() * 100.0 - 50.0, rnd.NextDouble() * 100.0 - 10.0);
                entities.Add(poly);
            }
            // Create Uniform Group
            // 균일 그룹 생성
            var group = EntityFactory.CreateUniformGroup("Group", entities);
            document.ActAdd(group);

            siriusEditorControl1.View?.DoRender();
        }

        /// <summary>
        /// Adds a set of spheres with Z height-map coloring.
        /// 구(Sphere) 샘플 추가 (Z-높이 맵 색상 적용)
        /// </summary>
        private void sphere_testcase(IDocument document)
        {
            var rnd = new Random((int)DateTime.Now.Ticks);
            const int ENTITY_COUNT = 100;
            var list = new List<EntitySphere>(ENTITY_COUNT);

            for (int i = 0; i < ENTITY_COUNT; i++)
            {
                var entity = EntityFactory.CreateSphere(new DVec3(0, 0, 0), 3);
                entity.Segments = 24;
                entity.ColorMode = EntityModelBase.ColorModes.ZHeightMap; // Z-Height Map coloring / Z-높이에 따른 색상 적용
                entity.ZRange = new DVec2(-5, 5);

                entity.Translate(rnd.NextDouble() * 100.0 - 50.0, rnd.NextDouble() * 100.0 - 50.0 + 100, rnd.NextDouble() * 10.0 - 5.0);
                list.Add(entity);
            }

            var group = EntityFactory.CreateUniformGroup("Group", list);
            group.Translate(rnd.NextDouble() * 100.0 - 50.0, rnd.NextDouble() * 100.0 - 50.0 + 100, rnd.NextDouble() * 2);
            
            document.ActAdd(group);
            siriusEditorControl1.View?.DoRender();
        }

        /// <summary>
        /// Adds multiple cubes and cylinders with random transforms.
        /// 큐브 및 실린더 샘플 추가
        /// </summary>
        private void cube_cylinder_testcase(IDocument document)
        {
            var rnd = new Random((int)DateTime.Now.Ticks);
            const int ENTITY_COUNT = 5;

            for (int i = 0; i < ENTITY_COUNT; i++)
            {
                // Create Cube
                // 큐브(Cube) 생성
                var cube = EntityFactory.CreateCube(DVec3.Zero, rnd.NextDouble() * 5, rnd.NextDouble() * 6, rnd.NextDouble() * 2);
                cube.ColorMode = EntityModelBase.ColorModes.Model;
                cube.ModelColor = new DVec3(rnd.NextDouble() + 0.8, rnd.NextDouble() * 0.5, rnd.NextDouble());
                cube.Rotate(rnd.NextDouble() * 60.0 - 30.0, rnd.NextDouble() * 60.0 - 30.0, rnd.NextDouble() * 60.0 - 30.0);
                cube.Translate(rnd.NextDouble() * 100.0 - 50.0, rnd.NextDouble() * 100.0 - 50.0, rnd.NextDouble() * 100.0 - 10.0);
                document.ActAdd(cube);

                // Create Cylinder
                // 실린더(Cylinder) 생성
                var cyl = EntityFactory.CreateCylinder(DVec3.Zero, rnd.NextDouble() * 10, rnd.NextDouble() * 10);
                cyl.ColorMode = EntityModelBase.ColorModes.Model;
                cyl.ModelColor = new DVec3(rnd.NextDouble() * 0.5, rnd.NextDouble() * 0.7, rnd.NextDouble() + 0.5);
                cyl.Rotate((rnd.NextDouble() * 60.0 - 30.0), (rnd.NextDouble() * 60.0 - 30.0), (rnd.NextDouble() * 60.0 - 30.0));
                cyl.Translate((rnd.NextDouble() * 100.0 - 50.0), (rnd.NextDouble() * 100.0 - 50.0), (rnd.NextDouble() * 100.0 - 10.0));
                document.ActAdd(cyl);
            }
            siriusEditorControl1.View?.DoRender();
        }

        /// <summary>
        /// Load and add 3D STL mesh.
        /// 3D STL 메쉬 로드 및 추가
        /// </summary>
        private void stl_testcase(IDocument document)
        {
            var rnd = new Random((int)DateTime.Now.Ticks);
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sample\\stl\\11_-_Main_Fan_1.stl");
            if (!File.Exists(fileName)) 
                return;

            // Create Mesh from file
            // 파일에서 메쉬 생성
            bool success = EntityFactory.CreateMesh(fileName, out var mesh);
            Debug.Assert(success);

            mesh.Rotate(rnd.NextDouble() * 10.0 - 5.0, rnd.NextDouble() * 10.0 - 5.0, rnd.NextDouble() * 10.0 - 5.0);
            mesh.Translate(rnd.NextDouble() * 100.0 - 50.0, rnd.NextDouble() * 100.0 - 50.0, 0);
            document.ActAdd(mesh);
            siriusEditorControl1.View?.DoRender();
        }

        /// <summary>
        /// Load and add 3D OBJ mesh.
        /// 3D OBJ 메쉬 로드 및 추가
        /// </summary>
        private void obj_testcase(IDocument document)
        {
            var rnd = new Random((int)DateTime.Now.Ticks);
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sample\\obj\\teapot.obj");
            if (!File.Exists(fileName)) 
                return;

            bool success = EntityFactory.CreateMesh(fileName, out var mesh);
            Debug.Assert(success);

            mesh.Rotate(rnd.NextDouble() * 10.0 - 5.0, rnd.NextDouble() * 10.0 - 5.0, rnd.NextDouble() * 10.0 - 5.0);
            mesh.Translate(rnd.NextDouble() * 100.0 - 50.0, rnd.NextDouble() * 100.0 - 50.0, 0);
            document.ActAdd(mesh);
            siriusEditorControl1.View?.DoRender();
        }

        /// <summary>
        /// Adds two large grid-cloud entities for a height map example.
        /// 대규모 그리드 클라우드(높이 맵) 샘플 추가
        /// </summary>
        private void gridcloud_testcase(IDocument document)
        {
            var rnd = new Random((int)DateTime.Now.Ticks);
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

            // Create Grid Cloud
            // 그리드 클라우드(Grid Cloud) 생성
            var pointsCloud = EntityFactory.CreateGridCloud(ROWS, COLS, INTERVAL, zDepths, new DVec2(zDepths.Min(), zDepths.Max()));
            pointsCloud.Translate(rnd.NextDouble() * 100.0 - 50.0, rnd.NextDouble() * 100.0 - 50.0, 0);

            document.ActAdd(pointsCloud);
            siriusEditorControl1.View?.DoRender();
        }

        /// <summary>
        /// Creates a block from an entity and inserts multiple block instances.
        /// 블록 생성 및 삽입 샘플 추가
        /// </summary>
        private void block_insert_testcase(IDocument document)
        {
            var rnd = new Random((int)DateTime.Now.Ticks);
            const string masterBlockName = "Block1";

            // Create block if not exist
            // 블록이 없으면 생성
            if (!document.FindByBlockName(masterBlockName, out _))
            {
                var entity = EntityFactory.CreateSpiral(DVec3.Zero, 5, 2, 5, EntitySpiral.SpiralTypes.Archimedean, true);
                document.ActBlock(new IEntity[] { entity }, masterBlockName, out _);
            }
            
            double dx = 0;
            double dy = 0;
            List<IEntity> entities = new List<IEntity>(2 * 5);

            for (int y = 0; y < 2; y++)
            {
                for (int x = 0; x < 5; x++)
                {
                    // Create Block Insert (Reference to block)
                    // 블록 삽입(블록 참조) 생성
                    var insert = EntityFactory.CreateBlockInsert($"BlockInsert{x},{y}", masterBlockName, new DVec3(dx, dy - 50, 0));
                    insert.Scale(rnd.NextDouble() + 0.2, rnd.NextDouble() + 0.2, rnd.NextDouble() + 0.2);
                    insert.Translate(rnd.NextDouble() * 5.0, rnd.NextDouble() * 5.0, 0);
                    insert.Rotate(rnd.NextDouble() * 60.0 - 30.0, rnd.NextDouble() * 60.0 - 30.0, rnd.NextDouble() * 60.0 - 30.0);

                    entities.Add(insert);
                    dx += 10;
                }
                dx = 0;
                dy += 11;
            }

            document.ActAdd(entities.ToArray());
            siriusEditorControl1.View?.DoRender();
        }

        /// <summary>
        /// Adds an image entity if the sample image exists.
        /// 이미지 엔티티 샘플 추가
        /// </summary>
        private void image_testcase(IDocument document)
        {
            var rnd = new Random((int)DateTime.Now.Ticks);
            var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sample\\image\\lena.bmp");
            if (!File.Exists(fileName)) return;

            // Create Image
            // 이미지(Image) 생성
            var image = EntityFactory.CreateImage(fileName, 10);
            image.Rotate(rnd.NextDouble() * 10.0 - 5.0, rnd.NextDouble() * 10.0 - 5.0, rnd.NextDouble() * 10.0 - 5.0);
            image.Translate(rnd.NextDouble() * 100.0 - 50.0, rnd.NextDouble() * 100.0 - 50.0, rnd.NextDouble() * 100.0 - 10.0);

            document.ActAdd(image);
            siriusEditorControl1.View?.DoRender();
        }

        /// <summary>
        /// Adds several large line batches to test performance.
        /// 대량의 선(Lines) 데이터 샘플 추가 (성능 테스트용)
        /// </summary>
        private void many_lines_testcase(IDocument document)
        {
            var rnd = new Random((int)DateTime.Now.Ticks);

            const int LINE_COUNT = 10_000; // 10,000 lines / 1만개 선
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
            // Create Lines (Optimized for massive line segments)
            // 대량의 선 마킹에 최적화된 Lines 엔티티 생성
            var entity = EntityFactory.CreateLines(lines);
            entity.Alpha = 0.9;
            entity.Translate(rnd.NextDouble() * 100.0 - 50.0, rnd.NextDouble() * 100.0 - 50.0, rnd.NextDouble() * 5);
            document.ActAdd(entity);

            siriusEditorControl1.View?.DoRender();
        }

        /// <summary>
        /// Adds barcode examples with transforms and hatch.
        /// 바코드 샘플 추가 (변환 및 해치 포함)
        /// </summary>
        private void barcode_testcase(IDocument document)
        {
            var rnd = new Random((int)DateTime.Now.Ticks);

            // PDF417 with Hatch
            // PDF417 및 해치 생성
            {
                var entity = EntityFactory.CreatePDF417("01234567890123456789", EntityBarcode2DBase.Barcode2DCells.Outline, 5, 5);
                entity.Rotate(rnd.NextDouble() * 10.0 - 5.0, rnd.NextDouble() * 10.0 - 5.0, rnd.NextDouble() * 10.0 - 5.0);
                entity.Translate(rnd.NextDouble() * 100.0 - 50.0, rnd.NextDouble() * 100.0 - 50.0, rnd.NextDouble() * 10.0 - 2.0);

                var hatch = HatchFactory.CreateLine(45, 0.1);
                entity.AddHatch(hatch);
                document.ActAdd(entity);
            }
            siriusEditorControl1.View?.DoRender();
        }

        /// <summary>
        /// Adds a sample ZPL label as an image entity.
        /// ZPL 라벨 샘플 추가 (이미지 엔티티로 변환)
        /// </summary>
        private void zpl_testcase(IDocument document)
        {
            var rnd = new Random((int)DateTime.Now.Ticks);

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

            // Create Image from ZPL command
            // ZPL 명령으로부터 이미지 엔티티 생성
            var entity = EntityFactory.CreateImageZPL(4 * 25.4, 6 * 25.4, zplText, EntityImageZPL.DotsPerMMs.Dots8_203DPI);
            entity.Translate(rnd.NextDouble() * 100.0 - 50, rnd.NextDouble() * 100.0 - 50, 0);
            document.ActAdd(entity);
            siriusEditorControl1.View?.DoRender();
        }

        /// <summary>
        /// Adds Lissajous curve sample.
        /// 리사주 곡선(Lissajous) 샘플 추가
        /// </summary>
        private void lissajous_testcase(IDocument document)
        {
            var rnd = new Random((int)DateTime.Now.Ticks);
            
            // Create Lissajous curve
            // 리사주 곡선 생성
            var entity = EntityFactory.CreateLissajous(DVec3.Zero, 10, 2, 12, EntityLissajous.LissajousTypes.π, EntityLissajous.Directions.Cw);
            entity.Translate(rnd.NextDouble() * 100.0 - 50, rnd.NextDouble() * 100.0 - 50, 0);
            document.ActAdd(entity);
            siriusEditorControl1.View?.DoRender();
        }

        /// <summary>
        /// Adds Archimedean and Classic spiral samples.
        /// 아르키메데스 및 클래식 나선 샘플 추가
        /// </summary>
        private void spiral_testcase(IDocument document)
        {
            var rnd = new Random((int)DateTime.Now.Ticks);

            // Classic Spiral
            // 클래식 나선 생성
            {
                var entity = EntityFactory.CreateSpiralClassic(DVec3.Zero, 10, 8, 2, 10, true);
                entity.Translate(rnd.NextDouble() * 100.0 - 50, rnd.NextDouble() * 100.0 - 50, 0);
                document.ActAdd(entity);
            }

            // Archimedean Spiral
            // 아르키메데스 나선 생성
            {
                var entity = EntityFactory.CreateSpiral(DVec3.Zero, 10, 2, 12, EntitySpiral.SpiralTypes.Archimedean, true);
                entity.Translate(rnd.NextDouble() * 100.0 - 50, rnd.NextDouble() * 100.0 - 50, 0);
                document.ActAdd(entity);
            }
            siriusEditorControl1.View?.DoRender();
        }

        /// <summary>
        /// (Optional) Demonstrates adding Gerber entities (PCB paths).
        /// 거버(Gerber, PCB 경로) 엔티티 샘플 추가
        /// </summary>
        private void gerber_testcase(IDocument document)
        {
            var rnd = new Random((int)DateTime.Now.Ticks);

            // LED Seven Segment Gerber
            // LED 7세그먼트 거버 로드
            {
                var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sample\\gerber\\LED-seven-segment.GBS");
                if (!File.Exists(fileName)) 
                    return;
                bool success = EntityFactory.CreateGerber(fileName, SpiralLab.Sirius3.UI.Config.EntityPenColors[0], out var gerber);
                Debug.Assert(success);
                gerber.Translate(rnd.NextDouble() * 100.0 - 50, rnd.NextDouble() * 100.0 - 50, 0);
                document.ActAdd(gerber);
            }

            // TRF7960 EVM Gerber
            // TRF7960 EVM 거버 로드
            {
                var fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sample\\gerber\\TRF7960_EVM (REV A).TOP");
                if (!File.Exists(fileName)) 
                    return;
                bool success = EntityFactory.CreateGerber(fileName, SpiralLab.Sirius3.UI.Config.EntityPenColors[1], out var gerber);
                Debug.Assert(success);
                gerber.Translate(rnd.NextDouble() * 100.0 - 50, rnd.NextDouble() * 100.0 - 50, 0);
                document.ActAdd(gerber);
            }
            siriusEditorControl1.View?.DoRender();
        }
        #endregion
    }
}
