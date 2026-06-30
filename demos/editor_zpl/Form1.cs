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

            this.btnZPL1.Click += BtnZPL1_Click;
            this.btnZPL2.Click += BtnZPL2_Click;
            this.btnZPL3.Click += BtnZPL3_Click;

            this.btnFontLoader.Click += BtnFontLoader_Click;
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

            siriusEditorControl1.RegisterDevices(rtc, laser, powerMeter, dInExt1, dInLaserPort, dOutExt1, dOutExt2, dOutLaserPort, marker);

            marker.Ready(siriusEditorControl1.Document, siriusEditorControl1.View, rtc, laser, powerMeter);
        }
      
        private void BtnZPL1_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            document.ActNew();

            var zplText = @"^XA
^FX Top section with logo, name and address.
^CF0,60

^FO50,50^GB100,100,100^FS
^FO75,75^FR^GB100,100,100^FS
^FO93,93^GB40,40,40^FS
^FO220,50^FDIntershipping, Inc.^FS
^CF0,30

^FO220,115^FD1000 Shipping Lane^FS
^FO220,155^FDShelbyville TN 38102^FS
^FO220,195^FDUnited States (USA)^FS
^FO50,250^GB700,3,3^FS
^FX Second section with recipient address and permit information.
^CFA,30

^FO50,300^FDJohn Doe^FS
^FO50,340^FD100 Main Street^FS
^FO50,380^FDSpringfield TN 39021^FS
^FO50,420^FDUnited States (USA)^FS
^CFA,15

^FO600,300^GB150,150,3^FS
^FO638,340^FDPermit^FS
^FO638,390^FD123456^FS
^FO50,500^GB700,3,3^FS
^FX Third section with bar code.
^BY5,2,270
^FO100,550^BC^FD12345678^FS
^FX Fourth section (the two boxes on the bottom).

^FO50,900^GB700,250,3^FS
^FO400,900^GB3,250,3^FS
^CF0,40

^FO100,960^FDCtr. X34B-1^FS
^FO100,1010^FDREF1 F00B47^FS
^FO100,1060^FDREF2 BL4H8^FS
^CF0,190

^FO470,955^FDCA^FS
^XZ";

            var entity = EntityFactory.CreateImageZPL(4 * 25.4, 6 * 25.4, zplText, EntityImageZPL.DotsPerMMs.Dots8_203DPI);
            entity.AlignmentXY = AlignmentXYs.MiddleCenter;
            document.ActAdd(entity);

            siriusEditorControl1.View?.DoRender();
        }

        private void BtnZPL2_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            document.ActNew();

            string zplText = @"^XA
^CI28
^PW576
^LL640
^LH0,0

^CWZ,E:KFONT.TTF

^FO35,35
^AZN,32,32
^FD[대기번호]^FS

^FO0,80
^A0N,110,95
^FB576,1,0,C,0
^FD006^FS

^FO0,205
^AZN,34,34
^FB576,1,0,C,0
^FD[주 문 서]^FS

^FO35,285
^GB506,1,1^FS

^FO90,305
^AZN,31,31
^FD상 품 명^FS

^FO405,305
^AZN,31,31
^FD수량^FS

^FO35,350
^AZN,30,30
^FD[포장](아이스)아메리카노^FS

^FO410,350
^AZN,30,30
^FD1 개^FS

^FO55,385
^AZN,28,28
^FD└▶[포장]600ml^FS

^FO55,420
^AZN,28,28
^FD└▶[포장]시그니처원두^FS

^FO55,455
^AZN,28,28
^FD└▶[포장]기본(2샷)^FS

^FO35,505
^GB506,1,1^FS

^FO125,530
^AZN,32,32
^FD총주문금액 :^FS

^FO380,530
^AZN,32,32
^FD2,000 원^FS

^FO35,575
^AZN,30,30
^FD2026/06/28 09:37^FS

^XZ";

            var entity = EntityFactory.CreateImageZPL(3 * 25.4, 3.2 * 25.4, zplText, EntityImageZPL.DotsPerMMs.Dots8_203DPI);
            entity.AlignmentXY = AlignmentXYs.MiddleCenter;
            document.ActAdd(entity);

            siriusEditorControl1.View?.DoRender();
        }

        private void BtnZPL3_Click(object sender, EventArgs e)
        {
            var document = siriusEditorControl1.Document;
            document.ActNew();

            string zplText = @"^XA

d^CI28
^PW400
^LL600
^LH0,0

Horizontal Rule
^FO20,100^GB360,1,1^FS

Header
^FO20,30^A0N,30,30^FB360,1,C,0^FDSTORE RECEIPT^FS
^FO20,70^A0N,20,20^FB360,1,C,0^FD123 Shopping St, New York^FS

Info
^FO20,120^A0N,20,20^FDDate: 2026-06-30^FS
^FO20,145^A0N,20,20^FDTime: 10:54 AM^FS
^FO20,170^A0N,20,20^FDReceipt #: 987654321^FS

Horizontal Rule
^FO20,205^GB360,1,1^FS

Items Header
^FO20,220^A0N,20,20^FDItem^FS
^FO320,220^A0N,20,20^FDPrice^FS

Item 1
^FO20,250^A0N,20,20^FDWireless Mouse^FS
^FO320,250^A0N,20,20^FD$25.00^FS

Item 2
^FO20,285^A0N,20,20^FDKeyboard^FS
^FO320,285^A0N,20,20^FD$45.00^FS

Horizontal Rule
^FO20,320^GB360,1,1^FS

Totals
^FO20,335^A0N,20,20^FDSubtotal:^FS
^FO320,335^A0N,20,20^FD$70.00^FS

^FO20,365^A0N,20,20^FDTax (8.25%):^FS
^FO320,365^A0N,20,20^FD$5.78^FS

^FO20,395^A0N,25,25^FB150,1,L,0^FDTOTAL:^FS
^FO250,395^A0N,25,25^FB130,1,R,0^FD$75.78^FS

Horizontal Rule
^FO20,435^GB360,1,1^FS

Barcode
^FO60,455^BY2,3^BCN,60,Y,N,N^FD987654321^FS

Footer
^FO20,550^A0N,20,20^FB360,1,C,0^FDTHANK YOU FOR SHOPPING!^FS

^XZ
";

            var entity = EntityFactory.CreateImageZPL(2 * 25.4, 3 * 25.4, zplText, EntityImageZPL.DotsPerMMs.Dots8_203DPI);
            entity.AlignmentXY = AlignmentXYs.MiddleCenter;
            document.ActAdd(entity);

            siriusEditorControl1.View?.DoRender();
        }

        private void BtnFontLoader_Click(object sender, EventArgs e)
        {
            // BinaryKits: offline library (BinaryKits 라이브러리: 오프라인 지원)
            SpiralLab.Sirius3.UI.Config.ZPLService = SpiralLab.Sirius3.UI.Config.ZPLServices.BinaryKits;

            // Used when the ZPL font identifier is '0'. Multiple candidates can be separated with ';', '|', or ','. 
            // ZPL 폰트 식별자가 '0'일 때 사용됩니다. 여러 후보는 ';', '|', ','로 구분할 수 있습니다.
            SpiralLab.Sirius3.UI.Config.ZPLBinaryKitsDefaultFont = "Arial Narrow;Arial;Helvetica";

            SpiralLab.Sirius3.UI.Config.ZPLBinaryKitsFonts.Clear();
            SpiralLab.Sirius3.UI.Config.ZPLBinaryKitsFonts["K"] = "Malgun Gothic;Microsoft YaHei UI;Noto Sans CJK KR";
            SpiralLab.Sirius3.UI.Config.ZPLBinaryKitsFonts["A"] = "Consolas;Cascadia Mono;D2Coding;Noto Sans Mono CJK KR;Noto Sans Mono CJK SC";
        }

    }
}
