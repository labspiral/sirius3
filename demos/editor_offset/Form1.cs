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
            this.btnMark4Offset.Click += BtnMark4Offset_Click;
            this.btnMark4OffsetWithRotate.Click += BtnMark4OffsetWithRotate_Click;
            this.btnMark4OffsetWithChangeData.Click += BtnMark4OffsetWithChangeData_Click;
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
            var marker = siriusEditorControl1.Marker;

            document.ActNew();

            // create entity
            var entity = EntityFactory.CreateDataMatrix("SIRIUS3", EntityBarcode2DBase.Barcode2DCells.Lines, 1, 10, 10);
            entity.CellLine.Direction = CellLine.LineDirections.Horizontal;

            entity.Name = "MyBarcode";
            entity.IsAllowConvert = true;

            document.ActivePage?.ActiveLayer?.AddChild(entity);

            // assign marker ended event handler
            marker.OnEnded += Marker_OnEnded;
        }
    
        private void BtnMark4Offset_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            var marker = siriusEditorControl1.Marker;

            if (marker.IsBusy)
                return;

            // 4 offsets
            //
            // -20,20            20,20
            //            +
            // -20,-20           20,-20
            //
            var offsets = new List<Offset>(4);
            offsets.Add(new Offset(-20, 20));
            offsets.Add(new Offset(20, 20));
            offsets.Add(new Offset(-20, -20));
            offsets.Add(new Offset(20, -20));

            marker.Offsets = offsets.ToArray();

            marker.Reset();
            marker.Ready(document);
            //set target page as current 
            marker.Page = document.Page;
            marker.Start();
        }

        private void BtnMark4OffsetWithRotate_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            var marker = siriusEditorControl1.Marker;

            if (marker.IsBusy)
                return;

            // 4 offsets
            //
            // -20,20 (0)             20,20 (90 angle)
            //                    +
            // -20,-20 (180 angle)   20,-20 (270 angle)
            //
            var offsets = new List<Offset>(4);

            //var offset = Offset.Zero;
            //offset.Scale = new DVec3(1, 1, 1);
            //offset.Translate = new DVec3(-20, 20, 0);
            //offsets.Add(offset)

            // or simply
            offsets.Add(new Offset(-20, 20, 0));
            offsets.Add(new Offset(20, 20, 90));
            offsets.Add(new Offset(-20, -20, 180));
            offsets.Add(new Offset(20, -20, 270));

            // assign offsets to marker
            marker.Offsets = offsets.ToArray();

            marker.Reset();
            marker.Ready(document);
            //set target page as current 
            marker.Page = document.Page;
            marker.Start();
        }

        private void BtnMark4OffsetWithChangeData_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            var marker = siriusEditorControl1.Marker;

            if (marker.IsBusy)
                return;

            // add 4 offsets
            //
            // -20,20            20,20
            //            +
            // -20,-20           20,-20
            //
            var offsets = new List<Offset>(4);
            offsets.Add(new Offset(-20, 20));
            offsets.Add(new Offset(20, 20));
            offsets.Add(new Offset(-20, -20));
            offsets.Add(new Offset(20, -20));
            // assign offsets to marker
            marker.Offsets = offsets.ToArray();

            // attach event handler for text convert 
            marker.OnTextConvert -= Marker_OnTextConvert;
            marker.OnTextConvert += Marker_OnTextConvert;

            marker.Reset();
            marker.Ready(document);
            //set target page as current 
            marker.Page = document.Page;
            marker.Start();
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
                    return $"SIRIUS3 {DateTime.Now.ToString("HH:mm:ss")} {currentOffsetIndex + 1}";
                default:
                    // Not modified
                    return textConvertible.SourceText;
            }
        }

        private void Marker_OnEnded(IMarker marker, bool success, TimeSpan? timeSpan)
        {
            //detach event handler after marker has ended
            marker.OnTextConvert -= Marker_OnTextConvert;
        }

    }
}
