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
using SpiralLab.Sirius3.Mathematics;


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
            this.btnCreateBarcode.Click += BtnCreateBarcode_Click;
            this.btnEventHandler.Click += BtnEventHandler_Click;
            this.btnSimpleScript.Click += BtnSimpleScript_Click;
            this.btnExternalFile.Click += BtnExternalFile_Click;
            this.btnOffset.Click += BtnOffset_Click;
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

        private void BtnCreateBarcode_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            document.ActNew();

            CreateEntities();
        }

        void CreateEntities()
        {
            var document = siriusEditorControl1.Document;
            document.ActNew();

            var entity = EntityFactory.CreateDataMatrix("0123456789", EntityBarcode2DBase.Barcode2DCells.Dots, 10, 10);
            entity.CellDot.DotFactor = 2;
            entity.CellDot.Direction = CellDot.DotDirections.Horizontal;
            entity.CellDot.IsZigZag = true;
            entity.CellDot.IsReversed = true;

            entity.Name = "MyBarcode";
            entity.IsAllowConvert = true;
            entity.TextConverter = TextConverters.Event; // used event handler by default 

            entity.Translate(0, -10);
            document.ActivePage?.ActiveLayer?.AddChild(entity);
          
            siriusEditorControl1.View?.DoRender();
        }

        private void BtnEventHandler_Click(object sender, EventArgs e)
        {
            var layer = siriusEditorControl1.Document.ActivePage.ActiveLayer;

            foreach(var entity in layer.Children)
            {
                if (entity is ITextConvertible textConvertible)
                {
                    // set text converter as event handler
                    textConvertible.IsAllowConvert = true;
                    textConvertible.TextConverter = TextConverters.Event;
                }
            }
            var marker = siriusEditorControl1.Marker;
            marker.OnTextConvert -= Marker_OnTextConvert;
            
            // now attach IMarker.OnTextConvert event
            marker.OnTextConvert += Marker_OnTextConvert;
        }

        private string Marker_OnTextConvert(IMarker marker, ITextConvertible textConvertible)
        {
            var entity = textConvertible as IEntity;
            var currentLayer = marker.WorkingSet.Layer;
            var currentLayerIndex = marker.WorkingSet.LayerIndex;
            var currentEntity = marker.WorkingSet.Entity;
            var currentEntityIndex = marker.WorkingSet.EntityIndex;
            var currentOffset = marker.WorkingSet.Offset;
            var currentOffsetIndex = marker.WorkingSet.OffsetIndex;

            switch (currentEntity.Name)
            {
                case "MyBarcode":
                    return $"EVENT {DateTime.Now.ToString("HH:mm:ss")}";
                default:
                    // Not modified
                    return textConvertible.SourceText;
            }
        }

        private void BtnSimpleScript_Click(object sender, EventArgs e)
        {
            var layer = siriusEditorControl1.Document.ActivePage.ActiveLayer;

            foreach (var entity in layer.Children)
            {
                if (entity is ITextConvertible textConvertible)
                {
                    // set text converter as event handler
                    textConvertible.IsAllowConvert = true;
                    textConvertible.TextConverter = TextConverters.SimpleScript;
                    textConvertible.SourceText = @"$""SCRIPT {DateTime.Now.ToString(""HH:mm:ss"")}""";
                }
            }

            siriusEditorControl1.View?.DoRender();
            siriusEditorControl1.PropertyGridCtrl.Refresh();
        }


        private void BtnExternalFile_Click(object sender, EventArgs e)
        {
            var layer = siriusEditorControl1.Document.ActivePage.ActiveLayer;

            foreach (var entity in layer.Children)
            {
                if (entity is ITextConvertible textConvertible)
                {
                    // set text converter as event handler
                    textConvertible.IsAllowConvert = true;
                    textConvertible.TextConverter = TextConverters.File;
                    
                    var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.txt");
                    textConvertible.ExternalFile = filePath;
                }
            }

            siriusEditorControl1.PropertyGridCtrl.Refresh();
        }

        private void BtnOffset_Click(object sender, EventArgs e)
        {
            var layer = siriusEditorControl1.Document.ActivePage.ActiveLayer;
            var marker = siriusEditorControl1.Marker;

            foreach (var entity in layer.Children)
            {
                if (entity is ITextConvertible textConvertible)
                {
                    // set text converter as event handler
                    textConvertible.IsAllowConvert = true;
                    textConvertible.TextConverter = TextConverters.Offset;
                }
            }

            var offsets = new List<Offset>();
            offsets.Add(new Offset(-10, 0) { ExtensionData = "OFFSET 1" });
            offsets.Add(new Offset(10, 0) { ExtensionData = "OFFSET 2" });
            marker.Offsets = offsets.ToArray();

            siriusEditorControl1.PropertyGridCtrl.Refresh();
        }

    }
}
