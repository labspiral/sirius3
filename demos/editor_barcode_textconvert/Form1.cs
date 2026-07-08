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
    /// Form1
    /// </summary>
    public partial class Form1 : Form
    {
        /// <summary>
        /// Form constructor
        /// 폼 생성자
        /// </summary>
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

            this.btnCreateBarcode.Click += BtnCreateBarcode_Click;
            this.btnEventHandler.Click += BtnEventHandler_Click;
            this.btnSimpleScript.Click += BtnSimpleScript_Click;
            this.btnExternalFile.Click += BtnExternalFile_Click;
            this.btnOffset.Click += BtnOffset_Click;
            this.btnLink.Click += BtnLink_Click;
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

            // Register devices to SiriusEditorControl
            // 장치들을 SiriusEditorControl에 등록
            siriusEditorControl1.RegisterDevices(rtc, laser, powerMeter, dInExt1, dInLaserPort, dOutExt1, dOutExt2, dOutLaserPort, marker);

            marker.Ready(siriusEditorControl1.Document, siriusEditorControl1.View, rtc, laser, powerMeter);
        }
      
        private void BtnCreateBarcode_Click(object sender, EventArgs e)
        {
            // Create sample entities with barcode and text 
            CreateEntities();
        }

        /// <summary>
        /// Create sample entities with barcode and text 
        /// 엔티티 생성
        /// </summary>
        void CreateEntities()
        {
            // New document
            var document = siriusEditorControl1.Document;
            document.ActNew();

            // Create datamatrix barcode entity
            {
                var entity = EntityFactory.CreateDataMatrix("0123456789", EntityBarcode2DBase.Barcode2DCells.Dots, 10, 10);
                entity.CellDot.DotFactor = 2;
                entity.IsReversed = true;
                entity.Name = "MyBarcode";
                entity.IsAllowConvert = true;
                entity.Translate(0, 10);
                document.ActAdd(entity);
            }

            // Create text entity
            {
                var entity = EntityFactory.CreateText("Arial",  FontStyle.Regular, "0123456789", 5);
                entity.Name = "MyText";
                entity.IsAllowConvert = true;
                //entity.Translate(0, 0);
                document.ActAdd(entity);
            }
        }

        private void BtnEventHandler_Click(object sender, EventArgs e)
        {
            var doc = siriusEditorControl1.Document;
            var marker = siriusEditorControl1.Marker;

            doc.FindByName("MyBarcode", out IEntity entityBarcode);
            doc.FindByName("MyText", out IEntity entityText);
            Debug.Assert(entityBarcode != null && entityText != null);

            var textConvertibleBarcode = entityBarcode as ITextConvertible;
            // set text converter as event handler
            textConvertibleBarcode.IsAllowConvert = true;
            textConvertibleBarcode.TextConverter = TextConverters.Event;

            var textConvertibleText = entityText as ITextConvertible;
            // set text converter as event handler
            textConvertibleText.IsAllowConvert = true;
            textConvertibleText.TextConverter = TextConverters.Event;

            // detach IMarker.OnTextConvert event
            marker.OnTextConvert -= Marker_OnTextConvert;
            // attach IMarker.OnTextConvert event
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
                    return $"{DateTime.Now.ToString("HH:mm:ss")}";
                case "MyText":
                    return $"{DateTime.Now.ToString("HH:mm:ss")}";
                default:
                    // Not modified
                    return textConvertible.SourceText;
            }
        }

        private void BtnSimpleScript_Click(object sender, EventArgs e)
        {
            var doc = siriusEditorControl1.Document;
            var marker = siriusEditorControl1.Marker;

            doc.FindByName("MyBarcode", out IEntity entityBarcode);
            doc.FindByName("MyText", out IEntity entityText);
            Debug.Assert(entityBarcode != null && entityText != null);

            var textConvertibleBarcode = entityBarcode as ITextConvertible;
            // set text converter as simple script
            textConvertibleBarcode.IsAllowConvert = true;
            textConvertibleBarcode.TextConverter = TextConverters.SimpleScript;
            // Script code examples: https://github.com/labspiral/sirius3/blob/main/doc/ScriptUserManual.md

            var expr1 = @"NextSerialNo(1)";
            // or 
            /*
            var expr2 = @"string prf = LotCode.Substring(0, Math.Min(LotCode.Length, 3)); 
string dt = Date(""yyMMdd"");
string tm = Time(""HHmm"");
string sn = NextSerialNo(""D5"");
string sh = Shift(""A"", ""B"", ""C"");
return $""{prf}-{dt}-{tm}-{sn}-{sh}"";
            ";
            */
            textConvertibleBarcode.SourceText = expr1;

            var textConvertibleText = entityText as ITextConvertible;
            // set text converter as event handler
            textConvertibleText.IsAllowConvert = true;
            textConvertibleText.IsAllowConvert = true;
            textConvertibleText.TextConverter = TextConverters.SimpleScript;
            // Script code examples: https://github.com/labspiral/sirius3/blob/main/doc/ScriptUserManual.md

            var expr2 = @"Time(""HH:mm:ss"")";
            
            textConvertibleText.SourceText = expr2;

            siriusEditorControl1.View?.DoRender();
            siriusEditorControl1.PropertyGridCtrl.Refresh();
        }

        private void BtnExternalFile_Click(object sender, EventArgs e)
        {
            var doc = siriusEditorControl1.Document;
            var marker = siriusEditorControl1.Marker;

            doc.FindByName("MyBarcode", out IEntity entityBarcode);
            doc.FindByName("MyText", out IEntity entityText);
            Debug.Assert(entityBarcode != null && entityText != null);

            var textConvertibleBarcode = entityBarcode as ITextConvertible;
            // set text converter as external file
            textConvertibleBarcode.IsAllowConvert = true;
            textConvertibleBarcode.TextConverter = TextConverters.File;
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.txt");
            textConvertibleBarcode.ExternalFile = filePath;

            var textConvertibleText = entityText as ITextConvertible;
            // set text converter as external file
            textConvertibleText.IsAllowConvert = true;
            textConvertibleText.TextConverter = TextConverters.File;
            textConvertibleText.ExternalFile = filePath;

            siriusEditorControl1.PropertyGridCtrl.Refresh();
        }

        private void BtnOffset_Click(object sender, EventArgs e)
        {
            var doc = siriusEditorControl1.Document;
            var marker = siriusEditorControl1.Marker;

            doc.FindByName("MyBarcode", out IEntity entityBarcode);
            doc.FindByName("MyText", out IEntity entityText);
            Debug.Assert(entityBarcode != null && entityText != null);

            var textConvertibleBarcode = entityBarcode as ITextConvertible;
            // set text converter as offset
            textConvertibleBarcode.IsAllowConvert = true;
            textConvertibleBarcode.TextConverter = TextConverters.Offset;

            var textConvertibleText = entityText as ITextConvertible;
            // set text converter as offset
            textConvertibleText.IsAllowConvert = true;
            textConvertibleText.TextConverter = TextConverters.Offset;


            var offsets = new List<Offset>();
            offsets.Add(new Offset(-10, 0) { ExtensionData = "MyBarcode|OFFSET 11|MyText|OFFSET 12" }); // EntityName|Text|...
            offsets.Add(new Offset(10, 0) { ExtensionData = "MyBarcode|OFFSET 21|MyText|OFFSET 22" });
            marker.Offsets = offsets.ToArray();

            siriusEditorControl1.PropertyGridCtrl.Refresh();
        }


        private void BtnLink_Click(object sender, EventArgs e)
        {
            var doc = siriusEditorControl1.Document;
            var marker = siriusEditorControl1.Marker;

            doc.FindByName("MyBarcode", out IEntity entityBarcode);
            doc.FindByName("MyText", out IEntity entityText);
            Debug.Assert(entityBarcode != null && entityText != null);

            var textConvertibleBarcode = entityBarcode as ITextConvertible;
            // set text converter as event handler
            textConvertibleBarcode.IsAllowConvert = true;
            textConvertibleBarcode.TextConverter = TextConverters.Event;
            // detach IMarker.OnTextConvert event
            marker.OnTextConvert -= Marker_OnTextConvert;

            // attach IMarker.OnTextConvert event
            marker.OnTextConvert += Marker_OnTextConvert;

            var textConvertibleText = entityText as ITextConvertible;
            // set text converter as link
            textConvertibleText.IsAllowConvert = true;
            textConvertibleText.TextConverter = TextConverters.Link;
            textConvertibleText.LinkEntity = "MyBarcode";


           siriusEditorControl1.PropertyGridCtrl.Refresh();
        }

    }
}
