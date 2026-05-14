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
using Microsoft.Extensions.Logging;
using SpiralLab.Sirius3;
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
    /// Remote control demo (TCP/IP, Serial, WebSocket, MQTT, etc.)
    /// 원격 제어 데모 (TCP/IP, 시리얼, 웹소켓, MQTT 등)
    /// </summary>
    public partial class Form1 : Form
    {

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

            // Create remote control device (Protocol defined in config.ini)
            // 원격 제어 장치 생성 (config.ini에 정의된 프로토콜 사용)
            if (EditorHelper.CreateRemote(marker, out var remote))
            {
                // Note: Running as Administrator might be required for some protocols (e.g., HTTP/WebSocket)
                // 참고: 일부 프로토콜(예: HTTP/웹소켓)의 경우 관리자 권한 실행이 필요할 수 있습니다.
                
                // remote.Start(); // Usually started inside CreateRemote helper / 보통 CreateRemote 내부에서 시작됨

                // Set remote control instance to editor control
                // 편집기 컨트롤에 원격 제어 인스턴스 설정
                siriusEditorControl1.Remote = remote;

                // Open the sample HTML client for WebSocket demo
                // 웹소켓 데모를 위한 샘플 HTML 클라이언트 열기
                var dir = AppDomain.CurrentDomain.BaseDirectory;
                string filePath = System.IO.Path.Combine(dir, "Remote_WebSocket_Demo.html");
                OpenHtml(filePath);
            }
        }

        /// <summary>
        /// Open HTML file in default browser
        /// 기본 브라우저에서 HTML 파일 열기
        /// </summary>
        /// <param name="path"></param>
        public static void OpenHtml(string path)
        {
            var psi = new ProcessStartInfo
            {
                FileName = path,           
                UseShellExecute = true     
            };

            Process.Start(psi);
        }
    }
}
