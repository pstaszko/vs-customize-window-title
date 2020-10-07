using EnvDTE;
using EnvDTE80;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace ErwinMayerLabs.RenameVSWindowTitle
{
    public static partial class VSocket
    {
        public class ListenerAndPort
        {
            public HttpListener listener { get; set; }
            public int port { get; set; }
        }
        public static void StartListen()
        {
            var pts = new ParameterizedThreadStart((object obj) => VSocket.Listen(Globals.DTE));
            var tt = new System.Threading.Thread(pts);
            tt.Start();
        }
        public static void Listen(DTE2 dte)
        {
            int pid = 0;
            void log(string msg) => System.IO.File.AppendAllText($@"c:\dev\temp\vspidlog_{pid}.txt", (DateTime.Now.ToLongTimeString()) + " | " + msg + "\r\n");
            try
            {
                pid = System.Diagnostics.Process.GetCurrentProcess().Id;
                log("Listen called, getting port");
                ListenerAndPort TryBindListenerOnFreePortX()
                {
                    TryBindListenerOnFreePort(log, out var listenerz, out var port);
                    return new ListenerAndPort { listener = listenerz, port = port };
                }
                var listener = TryBindListenerOnFreePortX();
                log(listener.port.ToString() + " assigned");
                var v = VSocket.GetVersion();
                var t = new List<string> { $"{pid}:{listener.port}:{v}" };
                if (System.IO.Directory.Exists(@"c:\DEV")) { System.IO.Directory.CreateDirectory(@"c:\DEV"); }
                if (System.IO.Directory.Exists(@"c:\DEV\temp")) { System.IO.Directory.CreateDirectory(@"c:\DEV\temp"); }
                System.IO.File.AppendAllLines(@"C:\DEV\temp\port.txt", t);
                log("Wrote to file");
                _Listen(dte, listener.listener);
                log("Finishing listen method to file");
            }
            catch (System.Exception ex)
            {
                log("Exception in Listen: " + ex.Message);
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
        public static bool TryBindListenerOnFreePort(Action<string> log, out HttpListener httpListener, out int port)
        {
            // IANA suggested range for dynamic or private ports
            const int MinPort = 50000;
            const int MaxPort = 65535;
            for (port = MinPort; port < MaxPort; port++)
            {
                log($"Checking {port}");
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
