using System;
using System.IO;
using Microsoft.Extensions.Logging;
using SpiralLab.Sirius3;
using SpiralLab.Sirius3.MCP;
using SpiralLab.Sirius3.UI.WinForms;

namespace Demos
{
    internal static class MCPDemoHelper
    {
        public static bool CreateMCP(int index, string name, SiriusEditorControl editor, out IMCPServer server)
        {
            return CreateMCP(index, options => MCPFactory.CreateServer(index, name, editor, options), out server);
        }

        public static bool CreateMCP(int index, string name, SiriusMultiEditorControl editor, out IMCPServer server)
        {
            return CreateMCP(index, options => MCPFactory.CreateServer(index, name, editor, options), out server);
        }

        private static bool CreateMCP(int index, Func<MCPServerOptions, IMCPServer> createServer, out IMCPServer server)
        {
            server = null;
            try
            {
                var accessMode = NativeMethods.ReadIni(
                    EditorHelper.ConfigFileName, $"MCP{index}", "ACCESS", MCPAccessMode.All);
                server = createServer(CreateMCPOptions(index));
                server.AccessMode = accessMode;
                return server.Start().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, ex, $"mcp[{index}]: demo server startup failed");
                return false;
            }
        }

        private static MCPServerOptions CreateMCPOptions(int index)
        {
            var section = $"MCP{index}";
            var configFileName = EditorHelper.ConfigFileName;
            var options = new MCPServerOptions();
            options.ApplyEnvironment();

            var uri = NativeMethods.ReadIni(configFileName, section, "URI", options.HttpEndpoint.AbsoluteUri);
            if (!Uri.TryCreate(uri, UriKind.Absolute, out var endpoint)
                || (endpoint.Scheme != Uri.UriSchemeHttp && endpoint.Scheme != Uri.UriSchemeHttps))
                throw new ArgumentException($"[{section}] URI must be an absolute HTTP endpoint.", nameof(uri));
            options.HttpEndpoint = endpoint;

            var documentRoot = NativeMethods.ReadIni(configFileName, section, "DocumentationRoot", string.Empty);
            if (!string.IsNullOrWhiteSpace(documentRoot))
            {
                var configDirectory = Path.GetDirectoryName(Path.GetFullPath(configFileName));
                options.DocumentationRoot = Path.GetFullPath(Path.IsPathRooted(documentRoot)
                    ? documentRoot
                    : Path.Combine(configDirectory, documentRoot));
            }

            return options;
        }
    }
}
