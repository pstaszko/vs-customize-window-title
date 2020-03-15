using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace ErwinMayerLabs.RenameVSWindowTitle
{
    public static class VSocket
    {
        public class Stuff
        {
            public HttpListener listener { get; set; }
            public int port { get; set; }
        }
        public static Stuff TryBindListenerOnFreePortX()
        {
            TryBindListenerOnFreePort(out var listener, out var port);
            return new Stuff { listener = listener, port = port };
        }
        public static void Listen(EnvDTE80.DTE2 dte)
        {
            var listener = TryBindListenerOnFreePortX();
            var t = new List<string> { $"{System.Diagnostics.Process.GetCurrentProcess().Id}:{listener.port.ToString()}" };
            System.IO.File.AppendAllLines(@"C:\DEV\temp\port.txt", t);
            _Listen(dte, listener.listener);
        }
        public static void _Listen(EnvDTE80.DTE2 dte, HttpListener listener)
        {
            var ctx = listener.GetContext();
            void writeOutput(string str)
            {
                var buffer = System.Text.Encoding.UTF8.GetBytes(str);
                var response = ctx.Response;
                response.ContentLength64 = buffer.Length;
                var output = response.OutputStream;
                response.ContentEncoding = System.Text.Encoding.UTF8;
                output.Write(buffer, 0, buffer.Length);
            }
            var headerDictionary2 = ctx.Request.Headers.AllKeys.ToDictionary((xx) => xx, (y) => ctx.Request.Headers[y]);
            var parameters = ctx.Request.QueryString.AllKeys.ToDictionary((xx) => xx, (y) => ctx.Request.QueryString[y]);
            foreach (var item in headerDictionary2) {
                parameters[item.Key] = item.Value;
            }
            var ret = "No action taken";
            try {
                if (parameters.ContainsKey("cmd")) {
                    string cmd = parameters["cmd"];
                    ret = "Command not found: " + cmd;
                    switch (cmd) {
                        case "LaunchDebugger":
                            System.Diagnostics.Debugger.Launch();
                            ret = "";
                            break;
                        case "Break":
                            System.Diagnostics.Debugger.Break();
                            ret = "";
                            break;
                    }
                    if (dte is EnvDTE80.DTE2 dte2) {
                        switch (cmd) {
                            case "ExecuteCommand":
                                dte.ExecuteCommand(parameters["command"], parameters["args"]);
                                ret = "";
                                break;
                            case "WriteToOutputWindow":
                                dte.ToolWindows.OutputWindow.ActivePane.OutputString(parameters["message"]);
                                ret = "";
                                break;
                                //case "DumpSolutionInfo":
                                //    var pj = dte.Solution.Projects.Cast<EnvDTE.Project>();
                                //    ret = $"{pj.Count()} Projects";
                                //    foreach (var p in pj) {

                                //    }
                                //    ret = "";
                                //    break;
                        }

                    }
                    if (dte?.ActiveDocument is EnvDTE.Document doc) {
                        switch (cmd) {
                            case "DocumentProperties":
                                ret = "";
                                var x2 = doc.ProjectItem;
                                foreach (EnvDTE.Property prop in x2.Properties) {
                                    try {
                                        var v = prop.Value.ToString().Trim();
                                        if (string.IsNullOrWhiteSpace(v)) {
                                            ret += $"{prop.Name}: {prop.Value}\r\n";
                                        }
                                    }
                                    catch {
                                    }
                                }
                                break;
                            case "ISaved":
                                ret = doc.Saved.ToString();
                                break;
                            case "AsyncSave":
                                doc.Save();
                                ret = "";
                                break;
                            case "Save":
                                doc.Save();
                                while (!doc.Saved) {
                                    System.Threading.Thread.Sleep(10);
                                }
                                ret = doc.Saved.ToString();
                                break;
                        }
                    }
                    if (dte?.ActiveDocument?.Selection is EnvDTE.TextSelection textSelection) {
                        //object o = null;
                        object vv(VirtualPoint p)
                        {
                            return new {
                                p.Line
                                    , p.DisplayColumn
                                    , p.VirtualDisplayColumn
                                    , p.AbsoluteCharOffset
                                    , p.LineCharOffset
                            };
                        }
                        var cmds = new Dictionary<string, Action> {
                            ["ChangeCase"] = () => {
                                if (Enum.TryParse(parameters["to"], out EnvDTE.vsCaseOptions x)) {
                                    textSelection.ChangeCase(x);
                                }
                            }
                            , ["SwapAnchor"] = () => textSelection.SwapAnchor()
                            , ["GetSelectedText"] = () => ret = textSelection.Text
                            , ["Position"] = () =>
                                ret = Newtonsoft.Json.JsonConvert.SerializeObject(new {
                                    textSelection.CurrentLine
                                    , textSelection.CurrentColumn
                                    , textSelection.BottomLine
                                    , textSelection.AnchorColumn
                                    , textSelection.Mode
                                    , textSelection.Text
                                    , ActivePoint = vv(textSelection.ActivePoint)
                                    , BottomPoint = vv(textSelection.BottomPoint)
                                    , AnchorPoint = vv(textSelection.AnchorPoint)
                                    //,textSelection.ActivePoint.DisplayColumn
                                    //,textSelection.ActivePoint.VirtualDisplayColumn
                                    //,textSelection.ActivePoint.AbsoluteCharOffset
                                    //,textSelection.ActivePoint.LineCharOffset
                                    //,textSelection.AnchorPoint.Line
                                    //,textSelection.AnchorPoint.DisplayColumn
                                    //,textSelection.AnchorPoint.VirtualDisplayColumn
                                    //,textSelection.AnchorPoint.AbsoluteCharOffset
                                    //,textSelection.AnchorPoint.LineCharOffset
                                    //,textSelection.BottomPoint
                                })
                            //ret = Newtonsoft.Json.JsonConvert.SerializeObject(vv(textSelection.ActivePoint))
                            //, ["Position"] = () => ret = $"{textSelection.CurrentLine}:{textSelection.CurrentColumn}:{textSelection.ActivePoint.Line"
                            , ["GetCurrentLine"] = () => {
                                textSelection.Text = parameters["to"];
                                ret = "";
                            }
                            , ["SetSelectedText"] = () => ret = textSelection.Text
                            , ["SelectedText"] = () => {
                                ret = "";
                                var lines = System.IO.File.ReadAllLines(dte.ActiveDocument.FullName);
                                ret = lines[textSelection.CurrentLine - 1];
                            }
                        };
                        if (cmds.ContainsKey(cmd)) { cmds[cmd](); }


                    }
                } else {
                    ret = "Command not provided";
                }
            }
            catch (Exception ex) {
                ret = ex.Message;
            }
            writeOutput(ret);
            //if (dte?.ActiveDocument?.Selection is EnvDTE.TextSelection responseString) {
            //    writeOutput(responseString.Text);
            //} else {
            //    writeOutput("Nope");
            //}
            _Listen(dte, listener);
        }
        public static bool TryBindListenerOnFreePort(out HttpListener httpListener, out int port)
        {
            // IANA suggested range for dynamic or private ports
            const int MinPort = 49215;
            const int MaxPort = 65535;

            for (port = MinPort; port < MaxPort; port++) {
                httpListener = new HttpListener();
                httpListener.Prefixes.Add($"http://localhost:{port}/");
                try {
                    httpListener.Start();
                    return true;
                }
                catch {
                    // nothing to do here -- the listener disposes itself when Start throws
                }
            }


            port = 0;
            httpListener = null;
            return false;
        }
    }
}
