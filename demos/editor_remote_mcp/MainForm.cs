using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SpiralLab.Sirius3.IO;
using SpiralLab.Sirius3.Laser;
using SpiralLab.Sirius3.Marker;
using SpiralLab.Sirius3.MCP;
using SpiralLab.Sirius3.PowerMeter;
using SpiralLab.Sirius3.Scanner.Rtc;
using SpiralLab.Sirius3.UI.WinForms;

namespace Demos
{
    internal sealed partial class MainForm : Form
    {
        private readonly CancellationTokenSource mcpCancellationTokenSource = new CancellationTokenSource();

        private IMCPServer mcpServer;
        private bool resourcesDisposed;
        private bool shutdownInProgress;
        private bool closeAfterShutdown;

        public MainForm()
        {
            InitializeComponent();
            FormClosing += MainForm_FormClosing;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            EditorHelper.CreateDevices(
                out IRtc rtc,
                out ILaser laser,
                out IDInput dInExt1,
                out IDInput dInLaserPort,
                out IDOutput dOutExt1,
                out IDOutput dOutExt2,
                out IDOutput dOutLaserPort,
                out IPowerMeter powerMeter,
                out IMarker marker);

            editorControl.RegisterDevices(
                rtc,
                laser,
                powerMeter,
                dInExt1,
                dInLaserPort,
                dOutExt1,
                dOutExt2,
                dOutLaserPort,
                marker);

            // Create MCP server
            // AI 모델 연결을 위한 MCP(Model Context Protocol) 생성
            if (MCPDemoHelper.CreateMCP(0, "MCP", editorControl, out mcpServer) && mcpServer != null)
            {
                // Set remote control instance to editor control
                // 편집기 컨트롤에 원격 제어 인스턴스 설정
                editorControl.MCPServer = mcpServer;

                statusValueLabel.Text = $"Running (loopback HTTP, {GetAccessModeLabel()})";
                endpointTextBox.Text = mcpServer.Options.HttpEndpoint?.AbsoluteUri ?? string.Empty;
                tokenTextBox.Text = mcpServer.Options.HttpToken ?? string.Empty;
            }
            else
            {
                statusValueLabel.Text = "Disabled";
            }
        }

        private string GetAccessModeLabel()
        {
            var accessMode = mcpServer?.AccessMode ?? MCPAccessMode.All;
            return accessMode == MCPAccessMode.ReadOnly
                ? "read-only"
                : accessMode == MCPAccessMode.DocumentControl
                    ? "read/write"
                    : accessMode.ToString();
        }


        private async void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (closeAfterShutdown)
                return;

            e.Cancel = true;
            if (shutdownInProgress)
                return;

            shutdownInProgress = true;
            statusValueLabel.Text = "Stopping...";

            await DisposeRuntimeResourcesAsync();

            closeAfterShutdown = true;
            shutdownInProgress = false;
            Close();
        }


        private async Task DisposeRuntimeResourcesAsync()
        {
            if (resourcesDisposed)
                return;

            resourcesDisposed = true;
            mcpCancellationTokenSource.Cancel();

            try
            {
                var server = mcpServer;
                mcpServer = null;
                if (server != null)
                {
                    if (ReferenceEquals(editorControl.MCPServer, server))
                        editorControl.MCPServer = null;
                    await server.DisposeAsync();
                }
            }
            catch (Exception ex)
            {
                // A shutdown failure must not leave the WinForms message loop or Kestrel host alive.
                System.Diagnostics.Debug.WriteLine($"MCP server shutdown failed: {ex}");
            }
            finally
            {
                DisposeEditorResources();
            }
        }

        private void DisposeRuntimeResources()
        {
            if (resourcesDisposed)
                return;

            resourcesDisposed = true;
            mcpCancellationTokenSource.Cancel();

            try
            {
                var server = mcpServer;
                mcpServer = null;
                if (ReferenceEquals(editorControl.MCPServer, server))
                    editorControl.MCPServer = null;
                server?.Dispose();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"MCP server shutdown failed: {ex}");
            }
            finally
            {
                DisposeEditorResources();
            }
        }


        private void DisposeEditorResources()
        {
            try
            {
                editorControl.DisposeDevices();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Editor device cleanup failed: {ex}");
            }

            try
            {
                var document = editorControl.Document;
                editorControl.Document = null;
                document?.Dispose();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Editor document cleanup failed: {ex}");
            }

            try
            {
                mcpCancellationTokenSource.Dispose();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"MCP cancellation cleanup failed: {ex}");
            }
        }
    }
}
