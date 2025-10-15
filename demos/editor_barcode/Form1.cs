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

            siriusEditorControl1.Scanner = rtc;

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

            CreateBarcodes();

            CreateTextConvertEventHandler();

            marker.Ready(siriusEditorControl1.Document, siriusEditorControl1.EditorCtrl.View, rtc, laser, powerMeter, stage);
        }

        void CreateBarcodes()
        {
            var document = siriusEditorControl1.Document;

            {
                //var entity = new EntityBarcode1D("0123456789", EntityBarcode1D.Barcode1DFormats.Code39, 5, 50, 10);

                //var entity = new EntityDataMatrix("0123456789", EntityBarcode2DBase.Barcode2DCells.Lines, 5, 10, 10);
                //entity.CellLine.Direction = CellLine.LineDirections.Horizontal;
                //entity.CellLine.IsZigZag = true;

                //var entity = new EntityQRCode("0123456789", EntityBarcode2DBase.Barcode2DCells.Lines, 5, 10, 10);
                //entity.CellLine.Direction = CellLine.LineDirections.Horizontal;
                //entity.CellLine.IsZigZag = true;

                //var entity = new EntityDataMatrix("0123456789", EntityBarcode2DBase.Barcode2DCells.Circles, 1, 10, 10);
                //entity.CellCircle.RadiusFactor = 0.9;
                //entity.CellCircle.IsZigZag = true;

                //var entity = new EntityDataMatrix("0123456789", EntityBarcode2DBase.Barcode2DCells.Squares, 1, 10, 10);
                //entity.CellSquare.ScaleFactor = 0.9;
                //entity.CellSquare.IsZigZag = true;

                var entity = new EntityDataMatrix("0123456789", EntityBarcode2DBase.Barcode2DCells.Dots, 5, 10, 10);
                entity.CellDot.Direction = CellDot.DotDirections.Horizontal;
                entity.CellDot.IsZigZag = true;
                entity.CellDot.IsReversed = true;

                //var entity = new EntityPDF417("0123456789", EntityBarcode2DBase.Barcode2DCells.Outline, 1, 10, 10);
                //var hatch = HatchFactory.CreateLine(0, 0.02);
                //hatch.Joint = HatchJoints.Miter;
                //hatch.Exclude = 0.05;
                //hatch.IsZigZag = true;
                //hatch.Sort = HatchSorts.Global; //slow calculation but mark time optimized
                //entity.AddHatch(hatch);
                //entity.HatchMarkOption = HatchMarkOptions.HatchFirst;

                entity.Name = "MyBarcode";
                entity.IsAllowConvert = true;

                entity.Translate(0, -10);
                document.ActivePage?.ActiveLayer?.AddChild(entity);
            }
            {
                //var entity = new EntityText("Arial", FontStyle.Regular, "0123456789", 2);
                var entity = new EntitySiriusText("ocra.cxf",  EntitySiriusText.LetterSpaces.Variable, 0.2, 0.5, 1, "0123456789", 2);

                entity.Name = "MyText";
                entity.IsAllowConvert = true;
                
                // allow to hatch for cell types : outline, circle, square 
                var hatch = HatchFactory.CreateLine(0, 0.02);
                hatch.Joint = HatchJoints.Miter;
                hatch.Exclude = 0.05;
                hatch.IsZigZag = true;
                hatch.Order = HatchOrders.Descending;
                hatch.Sort = HatchSorts.Line; // fast calculation, line by line but mark time is not optimzed
                //hatch.Sort = HatchSorts.Global; //slow calculation but mark time optimized
                entity.HatchMarkOption = HatchMarkOptions.HatchFirst;
                entity.AddHatch(hatch);

                entity.Translate(0, -12.5);
                document.ActivePage?.ActiveLayer?.AddChild(entity);
            }

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
                    case "MyBarcode":
                    case "MyText":
                        return $"SIRIUS2 {DateTime.Now.ToString("HH:mm:ss")} {currentOffsetIndex}";
                    default:
                        // Not modified
                        return textConvertible.SourceText;
                }
            };

        }
    }
}
