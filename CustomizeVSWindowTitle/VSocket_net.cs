using EnvDTE;
using EnvDTE80;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace ErwinMayerLabs.RenameVSWindowTitle
{
    public static partial class VSocket
    {
        public class ListenerAndPort
        {
            public HttpListener listener { get; set; }
            public int port { get; set; }
        }
        public static void Listen(DTE2 dte)
        {
            try
            {
                ListenerAndPort TryBindListenerOnFreePortX()
                {
                    TryBindListenerOnFreePort(out var listenerz, out var port);
                    return new ListenerAndPort { listener = listenerz, port = port };
                }
                var listener = TryBindListenerOnFreePortX();
                var v = VSocket.GetVersion();
                var t = new List<string> { $"{System.Diagnostics.Process.GetCurrentProcess().Id}:{listener.port.ToString()}:{v}" };
                System.IO.File.AppendAllLines(@"C:\DEV\temp\port.txt", t);
                _Listen(dte, listener.listener);
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }
        public static object expandPoint(VirtualPoint p) => new
        {
            p.Line,
            p.DisplayColumn,
            p.VirtualDisplayColumn,
            p.AbsoluteCharOffset,
            p.LineCharOffset
        };
        public static void _Listen(DTE2 dte, HttpListener listener)
        {
            var ctx = listener.GetContext();
            void writeOutput(string str)
            {
                var buffer = Encoding.UTF8.GetBytes(str);
                var response = ctx.Response;
                response.ContentLength64 = buffer.Length;
                var output = response.OutputStream;
                response.ContentEncoding = System.Text.Encoding.UTF8;
                output.Write(buffer, 0, buffer.Length);
            }
            var headerDictionary2 = ctx.Request.Headers.AllKeys.ToDictionary(k => k, v => ctx.Request.Headers[v]);
            var parameters = ctx.Request.QueryString.AllKeys.Where(x => x != null).ToDictionary(k => k, v => ctx.Request.QueryString[v]);
            foreach (var item in headerDictionary2)
            {
                parameters[item.Key] = item.Value;
            }
            writeOutput(processParameters(dte, parameters));
            _Listen(dte, listener);
        }
        public static bool TryBindListenerOnFreePort(out HttpListener httpListener, out int port)
        {
            // IANA suggested range for dynamic or private ports
            const int MinPort = 50000;
            const int MaxPort = 65535;
            for (port = MinPort; port < MaxPort; port++)
            {
                httpListener = new HttpListener();
                httpListener.Prefixes.Add($"http://localhost:{port}/");
                try
                {
                    httpListener.Start();
                    return true;
                }
                catch
                {
                }
            }
            port = 0;
            httpListener = null;
            return false;
        }
    }
}
