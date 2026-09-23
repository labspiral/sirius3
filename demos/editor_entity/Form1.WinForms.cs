using System;
using System.Windows.Forms;

namespace Demos
{
    public partial class Form1
    {
        /// <summary>
        /// Form constructor
        /// 폼 생성자
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            Load += Form1_Load;
            FormClosing += (s, e) =>
            {
                var dlgResult = MessageBox.Show(this, "Do you really want to terminate program ?", "WARNING", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dlgResult != DialogResult.Yes)
                {
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

            btnPoints.Click += (s, e) =>
            {
                var document = siriusEditorControl1.Document;
                points_testcase(document);
            };
            btnLineArc.Click += (s, e) =>
            {
                var document = siriusEditorControl1.Document;
                line_arc_testcase(document);
            };
            btnTriangleRectangle.Click += (s, e) =>
            {
                var document = siriusEditorControl1.Document;
                triangle_rectangle_testcase(document);
            };
            btnPolyline.Click += (s, e) =>
            {
                var document = siriusEditorControl1.Document;
                polyline2d_testcase(document);
                polyline3d_testcase(document);
            };
            btnSpline.Click += (s, e) =>
            {
                var document = siriusEditorControl1.Document;
                bezierSpline_testcase(document);
                catmullRomSpline_testcase(document);
                hermiteSpline_testcase(document);
                bSpline_testcase(document);
                nurbSpline_testcase(document);
            };
            btnText.Click += (s, e) =>
            {
                var document = siriusEditorControl1.Document;
                text_testcase(document);
            };
            btnImage.Click += (s, e) =>
            {
                var document = siriusEditorControl1.Document;
                image_testcase(document);
            };
            btnGridCloud.Click += (s, e) =>
            {
                var document = siriusEditorControl1.Document;
                gridcloud_testcase(document);
            };
            btnLines.Click += (s, e) =>
            {
                var document = siriusEditorControl1.Document;
                many_lines_testcase(document);
            };
            btnBarcode.Click += (s, e) =>
            {
                var document = siriusEditorControl1.Document;
                barcode_testcase(document);
            };
            btnGroup.Click += (s, e) =>
            {
                var document = siriusEditorControl1.Document;
                mixed_group_testcase(document);
                uniform_group_testcase(document);
            };
            btn3DMesh.Click += (s, e) =>
            {
                var document = siriusEditorControl1.Document;
                sphere_testcase(document);
                cube_cylinder_testcase(document);
                stl_testcase(document);
                obj_testcase(document);
            };
            btnBlockInsert.Click += (s, e) =>
            {
                var document = siriusEditorControl1.Document;
                block_insert_testcase(document);
            };
            btnZPL.Click += (s, e) =>
            {
                var document = siriusEditorControl1.Document;
                zpl_testcase(document);
            };
            btnLissajous.Click += (s, e) =>
            {
                var document = siriusEditorControl1.Document;
                lissajous_testcase(document);
            };
            btnSpiral.Click += (s, e) =>
            {
                var document = siriusEditorControl1.Document;
                spiral_testcase(document);
            };
            btnGerber.Click += (s, e) =>
            {
                var document = siriusEditorControl1.Document;
                gerber_testcase(document);
            };
        }
    }
}
