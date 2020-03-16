using EnvDTE;
using EnvDTE80;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace ErwinMayerLabs.RenameVSWindowTitle
{
    public static class VSocket
    {
        public class ListenerAndPort
        {
            public HttpListener listener { get; set; }
            public int port { get; set; }
        }
        public static void Listen(DTE2 dte)
        {
            ListenerAndPort TryBindListenerOnFreePortX()
            {
                TryBindListenerOnFreePort(out var listenerz, out var port);
                return new ListenerAndPort { listener = listenerz, port = port };
            }
            var listener = TryBindListenerOnFreePortX();
            var t = new List<string> { $"{System.Diagnostics.Process.GetCurrentProcess().Id}:{listener.port.ToString()}" };
            System.IO.File.AppendAllLines(@"C:\DEV\temp\port.txt", t);
            _Listen(dte, listener.listener);
        }
        public static object expandPoint(VirtualPoint p) => new {
            p.Line
            , p.DisplayColumn
            , p.VirtualDisplayColumn
            , p.AbsoluteCharOffset
            , p.LineCharOffset
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
            var parameters = ctx.Request.QueryString.AllKeys.ToDictionary(k => k, v => ctx.Request.QueryString[v]);
            foreach (var item in headerDictionary2) {
                parameters[item.Key] = item.Value;
            }
            writeOutput(processParameters(dte, parameters));
            _Listen(dte, listener);
        }

        private static string processParameters(DTE2 dte, Dictionary<string, string> parameters)
        {
            var ret = "No action taken";
            try {
                if (parameters.ContainsKey("cmd")) {
                    string cmd = parameters["cmd"];
                    var acts = new Dictionary<string, Action>();
                    var func = new Dictionary<string, Func<string>>();
                    acts["LaunchDebugger"] = () => System.Diagnostics.Debugger.Launch();
                    acts["Break"] = () => System.Diagnostics.Debugger.Break();
                    if (dte is EnvDTE80.DTE2 dte2) {
                        acts["ExecuteCommand"] = () => dte.ExecuteCommand(parameters["command"], parameters["args"]);
                        acts["WriteToOutputWindow"] = () => dte.ToolWindows.OutputWindow.ActivePane.OutputString(parameters["message"]);
                    }
                    if (dte?.ActiveDocument is Document doc) {
                        func["DocumentProperties"] = () => {
                            var retx = "";
                            var x2 = doc.ProjectItem;
                            foreach (Property prop in x2.Properties) {
                                try {
                                    var v = prop.Value.ToString().Trim();
                                    if (!string.IsNullOrWhiteSpace(v)) {
                                        retx += $"{prop.Name}: {v}\r\n";
                                    }
                                }
                                catch { }
                            }
                            return retx;
                        };
                        func["Save"] = () => {
                            doc.Save();
                            while (!doc.Saved) {
                                System.Threading.Thread.Sleep(10);
                            }
                            return doc.Saved.ToString();
                        };
                        func["AsyncSave"] = () => doc.Save().ToString();
                        acts["ISaved"] = () => doc.Saved.ToString();
                    }
                    if (dte?.ActiveDocument?.Selection is TextSelection textSelection) {
                        acts["ChangeCase"] = () => {
                            if (Enum.TryParse(parameters["to"], out EnvDTE.vsCaseOptions x)) {
                                textSelection.ChangeCase(x);
                            }
                        };
                        acts["SwapAnchor"] = () => textSelection.SwapAnchor();
                        func["GetSelectedText"] = () => textSelection.Text;
                        func["Position"] = () =>
                                Newtonsoft.Json.JsonConvert.SerializeObject(new {
                                    textSelection.CurrentLine
                                    , textSelection.CurrentColumn
                                    , textSelection.BottomLine
                                    , textSelection.AnchorColumn
                                    , textSelection.Mode
                                    , textSelection.Text
                                    , ActivePoint = expandPoint(textSelection.ActivePoint)
                                    , BottomPoint = expandPoint(textSelection.BottomPoint)
                                    , AnchorPoint = expandPoint(textSelection.AnchorPoint)
                                });
                    }
                    if (func.ContainsKey(cmd)) { ret = func[cmd](); }
                    if (acts.ContainsKey(cmd)) { acts[cmd](); }
                } else {
                    ret = "Command not provided";
                }
            }
            catch (Exception ex) {
                ret = ex.Message;
            }

            return ret;
        }

        public static bool TryBindListenerOnFreePort(out HttpListener httpListener, out int port)
        {
            // IANA suggested range for dynamic or private ports
            const int MinPort = 50000;
            const int MaxPort = 65535;
            for (port = MinPort; port < MaxPort; port++) {
                httpListener = new HttpListener();
                httpListener.Prefixes.Add($"http://localhost:{port}/");
                try {
                    httpListener.Start();
                    return true;
                }
                catch {
                }
            }
            port = 0;
            httpListener = null;
            return false;
        }
    }
}
