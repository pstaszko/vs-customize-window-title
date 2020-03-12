using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace ErwinMayerLabs.RenameVSWindowTitle
{
    public static class SocketBorp
    {
        public static void Mxain()
        {
            var web = new HttpListener();
            web.Prefixes.Add("http://localhost:8181/");
            Console.WriteLine("Listening..");
            web.Start();
            Console.WriteLine(web.GetContext());
            var context = web.GetContext();
            var response = context.Response;
            const string responseString = "<html><body>Hello world</body></html>";
            var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            var output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            Console.WriteLine(output);
            output.Close();
            web.Stop();
            //Console.ReadKey();
        }
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
            System.IO.File.AppendAllLines(@"c:\temp\port.txt", t);
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
                        switch (cmd) {
                            case "ChangeCase":
                                if (Enum.TryParse(parameters["to"], out EnvDTE.vsCaseOptions x)) {
                                    textSelection.ChangeCase(x);
                                }
                                break;
                            case "Position":
                                ret = $"{textSelection.CurrentLine}:{textSelection.CurrentColumn}";
                                break;
                            case "GetCurrentLine":
                                ret = "";
                                var lines = System.IO.File.ReadAllLines(dte.ActiveDocument.FullName);
                                ret = lines[textSelection.CurrentLine - 1];
                                break;
                            case "SetSelectedText":
                                textSelection.Text = parameters["to"];
                                ret = "";
                                break;
                            case "SelectedText":
                                ret = textSelection.Text;
                                break;
                            //case "a":
                            //    var ap = textSelection.ActivePoint;
                            //    var sb = new StringBuilder();
                            //    ret = $"{ap.Line};{ap.DisplayColumn};{ap.LineLength}";
                            //    break;
                            default:
                                break;
                        }
                    }
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
