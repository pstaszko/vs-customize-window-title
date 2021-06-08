using EnvDTE;
using EnvDTE80;
using System;
using System.Collections.Generic;
using System.Text;

namespace ErwinMayerLabs.RenameVSWindowTitle
{
    public static partial class VSocket
    {
        public static string GetVersion()
        {
            var x = System.Reflection.Assembly.GetExecutingAssembly();
            return $"{DateTime.Now.Subtract((new System.IO.FileInfo(x.Location)).LastWriteTime).TotalSeconds} Seconds old | {x.Location} | {x.FullName}";
        }
        //public static Lazy<Document> ActiveDocument = new Lazy<Document>(GetActiveDocument)

        //public static void NewCollapse(StringBuilder sb)
        //{
        //    System.IO.FileInfo fi = new System.IO.FileInfo(ActiveDocument.Value.FullName);
        //    ActiveDocument.Value.Selection.GotoLine(1, true);
        //    // Dim objCursorTextPoint As EnvDTE.TextPoint = GetCursorTextPoint()
        //    int line;
        //    try
        //    {
        //        DTE.ExecuteCommand("Edit.CollapseAllOutlining");
        //    }
        //    // _DTE.ExecuteCommand("Edit.ToggleOutliningExpansion")
        //    catch
        //    {
        //    }

        //    var expandClass = true;
        //    if (expandClass)
        //    {
        //        if (fi.Extension.psEqualsAny(
        //        {
        //            ".cshtml"
        //        }))
        //{
        //        }
        //else
        //        {
        //            DTE.ExecuteCommand(@"Edit.find ""^\s*(public|private|protected)?\s*\b(class|module)"" /regex");
        //            line = DTE.GetCurrentLine();
        //            log.Debug($"NewCollapse line: {line}");
        //            DTE.ExecuteCommand("Edit.Goto " + (line + 1));
        //        }
        //    }
        //    else
        //    {
        //        line = DTE.GetCurrentLine();
        //        DTE.ExecuteCommand("Edit.Goto " + line);
        //    }
        //    sb.Append("done");
        //}

        private static string processParameters(Connect c, DTE2 dte, Dictionary<string, string> parameters)
        {
            var ret = "No action taken";
            try
            {
                if (parameters.ContainsKey("cmd"))
                {
                    string cmd = parameters["cmd"];
                    var acts = new Dictionary<string, Action>();
                    var fns = new Dictionary<string, Func<string>>();
                    acts["LaunchDebugger"] = () => System.Diagnostics.Debugger.Launch();
                    acts["NewCollapse_JumpBack"] = () =>
                    {
                        var dtex = VSCMD.VSCMD.DTEWrapper.Init(dte);
                        VSCMD.VSCMD.Commands.DTE = dtex;
                        VSCMD.VSCMD.Commands.NewCollapse_JumpBack(new StringBuilder());
                    };
                    acts["Break"] = () => System.Diagnostics.Debugger.Break();
                    fns["Version"] = GetVersion;
                    if (dte is DTE2 dte2)
                    {

                        fns["ActiveSolutionProjects"] = () =>
                        {
                            //System.Diagnostics.Debugger.Launch();
                            //System.Diagnostics.Debugger.Break();

                            //Microsoft.VisualStudio.ProjectSystem.VS.Implementation.Package.Automation.OAProject x = null;
                            var n = new List<string>();
                            foreach (var item in dte.ActiveSolutionProjects as IEnumerable<object>)
                            {
                                var prop = item.GetType().GetProperty("FullName");
                                if (prop != null)
                                {
                                    n.Add(prop.GetValue(item).ToString());
                                }
                            }
                            return string.Join("\r\n", n);
                        };
                        var x = dte.ActiveSolutionProjects;
                        fns["GetActiveProjects"] = () =>
                        {
                            var l = new List<string>();
                            foreach (Project project in dte.ActiveSolutionProjects as System.Array)
                            {
                                l.Add(project.FullName);
                            }
                            return string.Join("\r\n", l);
                        };
                        fns["GetSolution"] = () => dte.Solution.FullName;
                        acts["UnloadProject"] = () =>
                        {
                            foreach (Project project in dte.ActiveSolutionProjects as System.Array)
                            {

                            }
                            string commandName = parameters["SolutionName"];
                            string commandArgs = parameters["args"];
                            dte.ExecuteCommand(commandName, commandArgs);
                        };
                        acts["ExecuteCommand"] = () =>
                        {
                            string commandName = parameters["command"];
                            string commandArgs = parameters["args"];
                            dte.ExecuteCommand(commandName, commandArgs);
                        };

                        acts["SE"] = () => {
                            c.SE();
                        };
                        acts["WriteToOutputWindow"] = () => dte.ToolWindows.OutputWindow.ActivePane.OutputString(parameters["message"]);
                    }
                    try
                    {
                        if (dte?.ActiveDocument is Document doc)
                        {
                            fns["DocumentProperties"] = () =>
                            {
                                var retx = "";
                                var x2 = doc.ProjectItem;
                                foreach (Property prop in x2.Properties)
                                {
                                    try
                                    {
                                        var v = prop.Value.ToString().Trim();
                                        if (!string.IsNullOrWhiteSpace(v))
                                        {
                                            retx += $"{prop.Name}: {v}\r\n";
                                        }
                                    }
                                    catch { }
                                }
                                return retx;
                            };
                            fns["Save"] = () =>
                            {
                                doc.Save();
                                while (!doc.Saved)
                                {
                                    System.Threading.Thread.Sleep(10);
                                }
                                return doc.Saved.ToString();
                            };
                            fns["AsyncSave"] = () => doc.Save().ToString();
                            acts["IsSaved"] = () => doc.Saved.ToString();
                        }
                        if (dte?.ActiveDocument?.Selection is TextSelection textSelection)
                        {
                            acts["InsertLineAbove"] = () =>
                            {
                                var i = textSelection.CurrentColumn;
                                dte.ExecuteCommand("Edit.LineOpenAbove");
                            };
                            acts["InsertLineBelow"] = () => { };
                            acts["ChangeCase"] = () =>
                            {
                                if (Enum.TryParse(parameters["to"], out EnvDTE.vsCaseOptions x))
                                {
                                    textSelection.ChangeCase(x);
                                }
                            };
                            acts["StartOfLine"] = () => textSelection.StartOfLine(vsStartOfLineOptions.vsStartOfLineOptionsFirstText);
                            acts["MoveToLineAndOffset"] = () =>
                            {
                                var line = int.Parse(parameters["line"]);
                                var offset = int.Parse(parameters["offset"]);
                                var extend = bool.Parse(parameters["extend"]);
                                textSelection.MoveToLineAndOffset(line, offset, extend);
                            };
                            acts["SwapAnchor"] = () => textSelection.SwapAnchor();
                            acts["SelectLine"] = () => textSelection.SelectLine();
                            fns["GetSelectedText"] = () => textSelection.Text;
                            fns["Position"] = () =>
                                    Newtonsoft.Json.JsonConvert.SerializeObject(new
                                    {
                                        textSelection.CurrentLine,
                                        textSelection.CurrentColumn,
                                        textSelection.BottomLine,
                                        textSelection.AnchorColumn,
                                        textSelection.Mode,
                                        textSelection.Text,
                                        dte.ActiveDocument.Path,
                                        dte.ActiveDocument.FullName,
                                        ActivePoint = expandPoint(textSelection.ActivePoint),
                                        BottomPoint = expandPoint(textSelection.BottomPoint),
                                        AnchorPoint = expandPoint(textSelection.AnchorPoint)
                                    });
                        }
                    }
                    catch 
                    {
                    }
                    if (fns.ContainsKey(cmd)) { ret = fns[cmd](); }
                    if (acts.ContainsKey(cmd)) { acts[cmd](); }
                }
                else
                {
                    ret = "Command not provided";
                }
            }
            catch (Exception ex)
            {
                ret = ex.Message;
            }
            return ret;
        }
    }
}
