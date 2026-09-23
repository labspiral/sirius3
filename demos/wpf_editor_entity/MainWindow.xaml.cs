using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows;
using SpiralLab.Sirius3.Document;

namespace Demos
{
    /// <summary>Material WPF host for the entity creation samples.</summary>
    public partial class Form1
    {
        private bool devicesInitialized;
        private bool editorDisposed;
        private bool initializationFailed;

        /// <summary>Initializes the WPF demo window.</summary>
        public Form1()
        {
            InitializeComponent();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (devicesInitialized || initializationFailed || editorDisposed)
                return;

            try
            {
                Form1_Load(sender, EventArgs.Empty);
                devicesInitialized = true;
            }
            catch (Exception exception)
            {
                initializationFailed = true;
                MessageBox.Show(
                    this,
                    exception.ToString(),
                    "Device initialization failed",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Close();
            }
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (!initializationFailed && MessageBox.Show(
                    this,
                    "Do you really want to terminate the program?",
                    "WARNING",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) != MessageBoxResult.Yes)
            {
                e.Cancel = true;
                return;
            }
            // Complete GL cleanup while the host can still render a context callback.
            try { DisposeEditor(); }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Sirius3 cleanup failed",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            try { DisposeEditor(); }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Sirius3 cleanup failed",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        internal void DisposeEditor()
        {
            if (editorDisposed)
                return;
            editorDisposed = true;

            var document = siriusEditorControl1.Document;
            var failures = new List<Exception>();
            try { siriusEditorControl1.DisposeDevices(); }
            catch (Exception exception) { failures.Add(exception); }
            try { siriusEditorControl1.Dispose(); }
            catch (Exception exception) { failures.Add(exception); }
            try { document?.Dispose(); }
            catch (Exception exception) { failures.Add(exception); }
            if (failures.Count > 0)
                throw new AggregateException("Editor cleanup failed.", failures);
        }

        private void Run(Action<IDocument> sample)
        {
            var document = siriusEditorControl1.Document;
            if (document != null)
                sample(document);
        }

        private void Points_Click(object sender, RoutedEventArgs e) => Run(points_testcase);

        private void LineArc_Click(object sender, RoutedEventArgs e) => Run(line_arc_testcase);

        private void TriangleRectangle_Click(object sender, RoutedEventArgs e) => Run(triangle_rectangle_testcase);

        private void Polyline_Click(object sender, RoutedEventArgs e) => Run(document =>
        {
            polyline2d_testcase(document);
            polyline3d_testcase(document);
        });

        private void Spline_Click(object sender, RoutedEventArgs e) => Run(document =>
        {
            bezierSpline_testcase(document);
            catmullRomSpline_testcase(document);
            hermiteSpline_testcase(document);
            bSpline_testcase(document);
            nurbSpline_testcase(document);
        });

        private void Text_Click(object sender, RoutedEventArgs e) => Run(text_testcase);

        private void Image_Click(object sender, RoutedEventArgs e) => Run(image_testcase);

        private void GridCloud_Click(object sender, RoutedEventArgs e) => Run(gridcloud_testcase);

        private void Lines_Click(object sender, RoutedEventArgs e) => Run(many_lines_testcase);

        private void Barcode_Click(object sender, RoutedEventArgs e) => Run(barcode_testcase);

        private void Group_Click(object sender, RoutedEventArgs e) => Run(document =>
        {
            mixed_group_testcase(document);
            uniform_group_testcase(document);
        });

        private void Mesh_Click(object sender, RoutedEventArgs e) => Run(document =>
        {
            sphere_testcase(document);
            cube_cylinder_testcase(document);
            stl_testcase(document);
            obj_testcase(document);
        });

        private void BlockInsert_Click(object sender, RoutedEventArgs e) => Run(block_insert_testcase);

        private void Zpl_Click(object sender, RoutedEventArgs e) => Run(zpl_testcase);

        private void Lissajous_Click(object sender, RoutedEventArgs e) => Run(lissajous_testcase);

        private void Spiral_Click(object sender, RoutedEventArgs e) => Run(spiral_testcase);

        private void Gerber_Click(object sender, RoutedEventArgs e) => Run(gerber_testcase);
    }
}
