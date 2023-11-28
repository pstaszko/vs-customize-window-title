using CSFramework;
using EnvDTE;
using EnvDTE80;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace VSocketExtension
{
    public static partial class VSocketQQ
    {
        public class ListenerAndPort
        {
            public HttpListener listener { get; set; }
            public int port { get; set; }
        }
        public static void StartListen()
        {
            var pts = new ParameterizedThreadStart((object obj) => VSocketQQ.Listen(VSocketExtension.Globals.DTE));
            var tt = new System.Threading.Thread(pts);
            tt.Start();
        }
        public static void Listen(DTE2 dte)
        {
            int pid = 0;
            //System.Diagnostics.Debugger.Launch();
            //System.Diagnostics.Debugger.Break();
            void log(string msg) => System.IO.File.AppendAllText($@"c:\dev\temp\VSPIDLOG_{pid}.txt", (DateTime.Now.ToLongTimeString()) + " | " + msg + "\r\n");
            try {
                pid = System.Diagnostics.Process.GetCurrentProcess().Id;
                log("Listen called, getting port");
                ListenerAndPort TryBindListenerOnFreePortX()
                {
                    TryBindListenerOnFreePort(log, out var listenerz, out var port);
                    return new ListenerAndPort { listener = listenerz, port = port };
                }
                var listener = TryBindListenerOnFreePortX();
                log(listener.port.ToString() + " assigned");
                var version = VSocketQQ.GetVersion();
                var t = new List<string> { $"{pid}:{listener.port}:{version}" };
                if (System.IO.Directory.Exists(@"c:\DEV")) { System.IO.Directory.CreateDirectory(@"c:\DEV"); }
                if (System.IO.Directory.Exists(@"c:\DEV\temp")) { System.IO.Directory.CreateDirectory(@"c:\DEV\temp"); }
                System.IO.File.AppendAllLines(@"C:\DEV\temp\port.txt", t);
                log("Wrote to file");
                var c = new Connect(dte);
                //c.SE();
                _ListenWS(c, dte, listener.listener, log);
                _Listen(c, dte, listener.listener);
                log("Finishing listen method to file");

            } catch (System.Exception ex) {
                log("Exception in Listen: " + ex.Message);
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }
        public static object expandPoint(VirtualPoint p) => new {
            p.Line,
            p.DisplayColumn,
            p.VirtualDisplayColumn,
            p.AbsoluteCharOffset,
            p.LineCharOffset
        };
        public static void _ListenWS(Connect c, DTE2 dte, HttpListener listener, Action<string> log)
        {
            var onMessage = SharedFramework.Extensions.MakeAction(((string x) => {
                //Flatinum.WebSockets.SendToClientNow("Got: " + x);
                //Flatinum.WebSockets.SendToClientNow(UniversalExtensions.Extensions.NewtonSerialize(d));
                var parameters = UniversalExtensions.Extensions.NewtonDeserialize<Dictionary<string, string>>(x);
                var msg = processParameters(c, dte, parameters);
                if (msg.isNotEmpty()) {
                    Flatinum.WebSockets.SendToClientNow(msg);
                }
                log(x);
            }));
            Flatinum.WebSockets.ConnectCS("ws://localhost:1880/ws/vs", null, onMessage);
        }
        public static void _Listen(Connect c, DTE2 dte, HttpListener listener)
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
            foreach (var item in headerDictionary2) {
                parameters[item.Key] = item.Value;
            }
            writeOutput(processParameters(c, dte, parameters));
            _Listen(c, dte, listener);
        }
        public static bool TryBindListenerOnFreePort(Action<string> log, out HttpListener httpListener, out int port)
        {
            // IANA suggested range for dynamic or private ports
            const int MinPort = 50160;
            const int MaxPort = 65535;
            for (port = MinPort; port < MaxPort; port++) {
                log($"Checking {port}");
                httpListener = new HttpListener();
                httpListener.Prefixes.Add($"http://127.0.0.1:{port}/");
                try {
                    httpListener.Start();
                    return true;
                } catch {
                }
            }
            port = 0;
            httpListener = null;
            return false;
        }
    }
    public class Connect
    {
        private DTE2 _applicationObject;
        //private AddIn _addInInstance;

        public Connect(DTE2 application)
        {
            _applicationObject = (DTE2)application;
            //_addInInstance = (AddIn)addInInst;
        }

        private void ExpandServers()
        {
            EnvDTE.Window serverExplorerToolwindow;
            EnvDTE.UIHierarchy hierarchy;

            try {
                serverExplorerToolwindow = GetToolwindow(EnvDTE.Constants.vsWindowKindServerExplorer);
                //var x = new FSSClass.myProgsGen.progsSingleton();
                FSSClass.AHKUtils.SendInput("{right}");
                if (serverExplorerToolwindow != null) {
                    hierarchy = (EnvDTE.UIHierarchy)serverExplorerToolwindow.Object;

                    //ShowNodes(hierarchy.UIHierarchyItems);
                    var getNode = getNodeByName("Servers", hierarchy.UIHierarchyItems);
                    if (getNode != null) {
                        getNode.Select(vsUISelectionType.vsUISelectionTypeSelect);
                        //getNode.Collection.Expanded = true;
                        //var c = getNode.Collection.Count;
                        foreach (UIHierarchyItem item in getNode.Collection) {
                            if (item.Name == "Data Connections") {
                                //item.Select(vsUISelectionType.vsUISelectionTypeSelect);
                                //item.Collection.Expanded = true;
                                if (item.Collection.Expanded) {
                                    //getNodeByName
                                }
                            }

                        }
                        //getNode.Collection.Item(0).Select(vsUISelectionType.vsUISelectionTypeExtend);
                    }
                }
            } catch (Exception ex) {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        private EnvDTE.Window GetToolwindow(string windowKind)
        {
            EnvDTE.Window toolWindow = null;

            foreach (EnvDTE.Window window in _applicationObject.Windows) {
                try {
                    if (window.ObjectKind == windowKind) {
                        toolWindow = window;
                        break;
                    }

                } catch (Exception) {

                }
            }
            return toolWindow;
        }

        private UIHierarchyItem getNodeByName(string name, UIHierarchyItems hierarchyItems)
        {
            foreach (UIHierarchyItem hierarchyItem in hierarchyItems) {
                if (hierarchyItem.Name == name) {
                    return hierarchyItem;
                }
            }
            return null;
        }
        private void ShowNodes(UIHierarchyItems hierarchyItems)
        {
            foreach (UIHierarchyItem hierarchyItem in hierarchyItems) {
                //System.Windows.Forms.MessageBox.Show(hierarchyItem.Name);

                // Enter in recursion
                ShowNodes(hierarchyItem.UIHierarchyItems);
            }
        }

        internal void SE()
        {
            ExpandServers();

        }
    }
}