using System;

using SpiralLab.Sirius3.Document;
using SpiralLab.Sirius3.Scanner;
using SpiralLab.Sirius3.IO;
using SpiralLab.Sirius3.Scanner.Rtc;
using SpiralLab.Sirius3.PowerMeter;
using SpiralLab.Sirius3.Laser;
using SpiralLab.Sirius3.Marker;
using SpiralLab.Sirius3.Motion;
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
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            EditorHelper.CreateDevices(out IRtc rtc, out ILaser laser, out IDInput dInExt1, out IDInput dInLaserPort, out IDOutput dOutExt1, out IDOutput dOutExt2, out IDOutput dOutLaserPort, out IPowerMeter powerMeter, out IMarker marker);

            siriusEditorControl1.Rtc = rtc;

            siriusEditorControl1.Laser = laser;

            siriusEditorControl1.DIExt1 = dInExt1;
            siriusEditorControl1.DOExt1 = dOutExt1;
            siriusEditorControl1.DOExt2 = dOutExt2;
            siriusEditorControl1.DILaserPort = dInLaserPort;
            siriusEditorControl1.DOLaserPort = dOutLaserPort;

            siriusEditorControl1.PowerMeter = powerMeter;

            siriusEditorControl1.Marker = marker;

            var stage = StageFactory.CreateVirtual(0);
            stage.Initialize();
            siriusEditorControl1.Stage = stage;

            //siriusEditorControl1.EditorCtrl.View.FovArea = new DVec3(200, 200, 0);

            CreateBarcodes();

            CreateTextConvertEventHandler();

            marker.Ready(siriusEditorControl1.Document, siriusEditorControl1.EditorCtrl.View, rtc, laser, powerMeter, stage);
        }

      
        void CreateBarcodes()
        {
            var document = siriusEditorControl1.Document;

            {
                var entity = new EntityQRCode("01234567890123456789", EntityBarcode2DBase.Barcode2DCells.Lines, 5, 5, 5);
                entity.Name = "MyBarcode1";
                entity.Translate(-10, 0);
                document.ActivePage?.ActiveLayer?.AddChild(entity);
            }

            //{
            //    var entity = new EntityDataMatrix("01234567890123456789", EntityBarcode2DBase.Barcode2DCells.Dots, 5, 5, 5);
            //    entity.Name = "MyBarcode2";
            //    entity.Translate(0, -10);
            //    document.ActivePage?.ActiveLayer?.AddChild(entity);
            //}

            //{
            //    var entity = new EntityPDF417("01234567890123456789", EntityBarcode2DBase.Barcode2DCells.Outline, 1, 5, 5);
            //    entity.Name = "MyBarcode3";
            //    entity.Translate(0, 10);

            //    var hatch = HatchFactory.CreateLine(45, 0.1);
            //    entity.AddHatch(hatch);

            //    document.ActivePage?.ActiveLayer?.AddChild(entity);
            //}
        }

        void CreateTextConvertEventHandler()
        {
            var marker = siriusEditorControl1.Marker;

            marker.OnTextConvert += (IMarker marker, ITextConvertible textConvertible) =>
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
                    case "MyBarcode1":
                        return $"SIRIUS2 {currentOffsetIndex}";
                    case "MyBarcode2":
                        return $"SIRIUS2 {DateTime.Now.ToString("HH:mm:ss")} {currentOffsetIndex}";
                    case "MyBarcode3":
                        return $"SIRIUS2 {DateTime.Now.ToString("HH:mm:ss")} {currentOffsetIndex}";
                    default:
                        // Not modified
                        return textConvertible.SourceText;
                }
            };

        }
    }
}
