using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.Collections.Generic;

// The PackageRegistration attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class
// is a package.
//
// The InstalledProductRegistration attribute is used to register the information needed to show this package
// in the Help/About dialog of Visual Studio.

namespace ErwinMayerLabs.RenameVSWindowTitle {
    [PackageRegistration(UseManagedResourcesOnly = true), InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400), ProvideAutoLoad(VSConstants.UICONTEXT.NoSolution_string), ProvideMenuResource("Menus.ctmenu", 1), Guid(GuidList.guidRenameVSWindowTitle2PkgString)]
    [ProvideOptionPage(typeof(OptionPageGrid), "Rename VS Window Title", "Rules", 0, 0, true)]
    [ProvideOptionPage(typeof(SupportedTagsGrid), "Rename VS Window Title", "Supported tags", 101, 1000, true)]
    public sealed class RenameVSWindowTitle : Package {
        private string IDEName;

        private System.Windows.Forms.Timer ResetTitleTimer;
        //Private VersionSpecificAssembly As Assembly

        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require
        /// any Visual Studio service because at this point the package object is created but
        /// not sited yet inside Visual Studio environment. The place to do all the other
        /// initialization is the Initialize method.
        /// </summary>
        public RenameVSWindowTitle() {
            Globals.DTE = (DTE2)GetGlobalService(typeof(DTE));
            Globals.DTE.Events.DebuggerEvents.OnEnterBreakMode += this.OnIdeEvent;
            Globals.DTE.Events.DebuggerEvents.OnEnterRunMode += this.OnIdeEvent;
            Globals.DTE.Events.DebuggerEvents.OnEnterDesignMode += this.OnIdeEvent;
            Globals.DTE.Events.DebuggerEvents.OnContextChanged += this.OnIdeEvent;
            Globals.DTE.Events.SolutionEvents.AfterClosing += this.OnIdeEvent;
            Globals.DTE.Events.SolutionEvents.Opened += this.OnIdeEvent;
            Globals.DTE.Events.SolutionEvents.Renamed += this.OnIdeEvent;
            Globals.DTE.Events.WindowEvents.WindowCreated += this.OnIdeEvent;
            Globals.DTE.Events.WindowEvents.WindowClosing += this.OnIdeEvent;
            Globals.DTE.Events.WindowEvents.WindowActivated += this.OnIdeEvent;
            Globals.DTE.Events.DocumentEvents.DocumentOpened += this.OnIdeEvent;
            Globals.DTE.Events.DocumentEvents.DocumentClosing += this.OnIdeEvent;
        }

        private void OnIdeEvent(Window gotfocus, Window lostfocus) {
            this.OnIdeEvent();
        }

        private void OnIdeEvent(Document document) {
            this.OnIdeEvent();
        }

        private void OnIdeEvent(Window window) {
            this.OnIdeEvent();
        }

        private void OnIdeEvent(string oldname) {
            this.OnIdeEvent();
        }

        private void OnIdeEvent(dbgEventReason reason) {
            this.OnIdeEvent();
        }

        private void OnIdeEvent(dbgEventReason reason, ref dbgExecutionAction executionaction) {
            this.OnIdeEvent();
        }

        private void OnIdeEvent(Process newProc, Program newProg, EnvDTE.Thread newThread, StackFrame newStkFrame) {
            this.OnIdeEvent();
        }

        private void OnIdeEvent() {
            if (this.Settings.EnableDebugMode) {
                WriteOutput("Debugger context changed. Updating title.");
            }
            this.UpdateWindowTitleAsync(this, EventArgs.Empty);
        }

        ///'''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
        // Overriden Package Implementation

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initilaization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize() {
            base.Initialize();
            //Every 5 seconds, we check the window titles in case we missed an event.
            this.ResetTitleTimer = new System.Windows.Forms.Timer { Interval = 5000 };
            this.ResetTitleTimer.Tick += this.UpdateWindowTitleAsync;
            this.ResetTitleTimer.Start();
        }

        protected override void Dispose(bool disposing) {
            this.ResetTitleTimer.Dispose();
            base.Dispose(disposing: disposing);
        }

        #endregion

        private OptionPageGrid Settings => (OptionPageGrid)this.GetDialogPage(typeof(OptionPageGrid));

        private string GetIDEName(string str) {
            try {
                var m = new Regex(@"^(.*) - (" + Globals.DTE.Name + ".*) " + Regex.Escape(this.Settings.AppendedString) + "$", RegexOptions.RightToLeft).Match(str);
                if (!m.Success) {
                    m = new Regex(@"^(.*) - (" + Globals.DTE.Name + @".* \(.+\)) \(.+\)$", RegexOptions.RightToLeft).Match(str);
                }
                if (!m.Success) {
                    m = new Regex(@"^(.*) - (" + Globals.DTE.Name + ".*)$", RegexOptions.RightToLeft).Match(str);
                }
                if (!m.Success) {
                    m = new Regex(@"^(" + Globals.DTE.Name + ".*)$", RegexOptions.RightToLeft).Match(str);
                }
                if (m.Success && m.Groups.Count >= 2) {
                    if (m.Groups.Count >= 3) {
                        return m.Groups[2].Captures[0].Value;
                    }
                    if (m.Groups.Count >= 2) {
                        return m.Groups[1].Captures[0].Value;
                    }
                }
                else {
                    if (this.Settings.EnableDebugMode) {
                        WriteOutput("IDE name (" + Globals.DTE.Name + ") not found: " + str + ".");
                    }
                    return null;
                }
            }
            catch (Exception ex) {
                if (this.Settings.EnableDebugMode) {
                    WriteOutput("GetIDEName Exception: " + str + (". Details: " + ex));
                }
                return null;
            }
            return "";
        }

        private string GetVSSolutionName(string str) {
            try {
                var m = new Regex(@"^(.*)\\(.*) - (" + Globals.DTE.Name + ".*) " + Regex.Escape(this.Settings.AppendedString) + "$", RegexOptions.RightToLeft).Match(str);
                if (m.Success && m.Groups.Count >= 4) {
                    var name = m.Groups[2].Captures[0].Value;
                    var state = this.GetVSState(str);
                    return name.Substring(0, name.Length - (string.IsNullOrEmpty(state) ? 0 : state.Length + 3));
                }
                m = new Regex("^(.*) - (" + Globals.DTE.Name + ".*) " + Regex.Escape(this.Settings.AppendedString) + "$", RegexOptions.RightToLeft).Match(str);
                if (m.Success && m.Groups.Count >= 3) {
                    var name = m.Groups[1].Captures[0].Value;
                    var state = this.GetVSState(str);
                    return name.Substring(0, name.Length - (string.IsNullOrEmpty(state) ? 0 : state.Length + 3));
                }
                m = new Regex("^(.*) - (" + Globals.DTE.Name + ".*)$", RegexOptions.RightToLeft).Match(str);
                if (m.Success && m.Groups.Count >= 3) {
                    var name = m.Groups[1].Captures[0].Value;
                    var state = this.GetVSState(str);
                    return name.Substring(0, name.Length - (string.IsNullOrEmpty(state) ? 0 : state.Length + 3));
                }
                if (this.Settings.EnableDebugMode) {
                    WriteOutput("VSName not found: " + str + ".");
                }
                return null;
            }
            catch (Exception ex) {
                if (this.Settings.EnableDebugMode) {
                    WriteOutput("GetVSName Exception: " + str + (". Details: " + ex));
                }
                return null;
            }
        }

        private string GetVSState(string str) {
            try {
                var m = new Regex(@" \((.*)\) - (" + Globals.DTE.Name + ".*) " + Regex.Escape(this.Settings.AppendedString) + "$", RegexOptions.RightToLeft).Match(str);
                if (!m.Success) {
                    m = new Regex(@" \((.*)\) - (" + Globals.DTE.Name + ".*)$", RegexOptions.RightToLeft).Match(str);
                }
                if (m.Success && m.Groups.Count >= 3) {
                    return m.Groups[1].Captures[0].Value;
                }
                if (this.Settings.EnableDebugMode) {
                    WriteOutput("VSState not found: " + str + ".");
                }
                return null;
            }
            catch (Exception ex) {
                if (this.Settings.EnableDebugMode) {
                    WriteOutput("GetVSState Exception: " + str + (". Details: " + ex));
                }
                return null;
            }
        }

        private void UpdateWindowTitleAsync(object state, EventArgs e) {
            if (this.IDEName == null && Globals.DTE.MainWindow != null) {
                this.IDEName = this.GetIDEName(Globals.DTE.MainWindow.Caption);
            }
            if (this.IDEName == null) {
                return;
            }
            System.Threading.Tasks.Task.Factory.StartNew(this.UpdateWindowTitle);
        }

        private readonly object UpdateWindowTitleLock = new object();

        private void UpdateWindowTitle() {
            if (!Monitor.TryEnter(this.UpdateWindowTitleLock)) {
                return;
            }
            try {
                var useDefaultPattern = true;
                if (this.Settings.AlwaysRewriteTitles) {
                    useDefaultPattern = false;
                }
                else {
                    Globals.VSMultiInstanceInfo info;
                    Globals.GetVSMultiInstanceInfo(out info);
                    if (info.nb_instances_same_solution >= 2) {
                        useDefaultPattern = false;
                    }
                    else {
                        var currentInstance = System.Diagnostics.Process.GetCurrentProcess();
                        var vsInstances = System.Diagnostics.Process.GetProcessesByName("devenv");
                        if (vsInstances.Length >= 2) {
                            //Check if multiple instances of devenv have identical original names. If so, then rewrite the title of current instance (normally the extension will run on each instance so no need to rewrite them as well). Otherwise do not rewrite the title.
                            //The best would be to get the EnvDTE.DTE object of the other instances, and compare the solution or project names directly instead of relying on window titles (which may be hacked by third party software as well). But using moniker it will only work if they are launched with the same privilege.
                            var currentInstanceName = Path.GetFileNameWithoutExtension(Globals.DTE.Solution.FullName);
                            if (string.IsNullOrEmpty(currentInstanceName) || (from vsInstance in vsInstances
                                                                              where vsInstance.Id != currentInstance.Id
                                                                              select this.GetVSSolutionName(vsInstance.MainWindowTitle)).Any(vsInstanceName => vsInstanceName != null && currentInstanceName == vsInstanceName)) {
                                useDefaultPattern = false;
                            }
                        }
                    }
                }
                var solution = Globals.DTE.Solution;
                var solutionFp = solution?.FullName;
                Dictionary<string, object> settingsOverride = this.GetSettingsOverride(solutionFp: solutionFp);
                var pattern = this.GetPattern(solutionFp: solutionFp, useDefault: useDefaultPattern, settingsOverride: settingsOverride);
                this.ChangeWindowTitle(this.GetNewTitle(solution: solution, pattern: pattern, settingsOverride: settingsOverride));
            }
            catch (Exception ex) {
                try {
                    if (this.Settings.EnableDebugMode) {
                        WriteOutput("UpdateWindowTitle exception: " + ex);
                    }
                }
                catch {
                    // ignored
                }
            }
            finally {
                Monitor.Exit(this.UpdateWindowTitleLock);
            }
        }

        private string GetPattern(string solutionFp, bool useDefault, Dictionary<string, object> settingsOverride) {
            if (string.IsNullOrEmpty(solutionFp)) {
                var document = Globals.DTE.ActiveDocument;
                var window = Globals.DTE.ActiveWindow;
                if (string.IsNullOrEmpty(document?.FullName) && string.IsNullOrEmpty(window?.Caption)) {
                    return useDefault ? "[ideName]" : this.Settings.PatternIfNothingOpen;
                }
                return useDefault ? "[documentName] - [ideName]" : this.Settings.PatternIfDocumentButNoSolutionOpen;
            }
            var designModePattern = "[solutionName] - [ideName]";
            var breakModePattern = "[solutionName] (Debugging) - [ideName]";
            var runningModePattern = "[solutionName] (Running) - [ideName]";
            if (!useDefault) {
                designModePattern = this.Settings.PatternIfDesignMode;
                breakModePattern = this.Settings.PatternIfBreakMode;
                runningModePattern = this.Settings.PatternIfRunningMode;
                if (settingsOverride != null) {
                    string v;
                    if (TryGetSettingValue(settings: settingsOverride, key: "PatternIfDesignMode", value: out v)) {
                        designModePattern = v;
                    }
                    if (TryGetSettingValue(settings: settingsOverride, key: "PatternIfBreakMode", value: out v)) {
                        breakModePattern = v;
                    }
                    if (TryGetSettingValue(settings: settingsOverride, key: "PatternIfRunningMode", value: out v)) {
                        runningModePattern = v;
                    }
                }
            }
            if (Globals.DTE.Debugger == null || Globals.DTE.Debugger.CurrentMode == dbgDebugMode.dbgDesignMode) {
                return designModePattern;
            }
            if (Globals.DTE.Debugger.CurrentMode == dbgDebugMode.dbgBreakMode) {
                return breakModePattern;
            }
            if (Globals.DTE.Debugger.CurrentMode == dbgDebugMode.dbgRunMode) {
                return runningModePattern;
            }
            throw new Exception("No matching state found");
        }

        readonly Regex TagRegex = new Regex(@"\[([^\]]+)\]", RegexOptions.Multiline);

        private string GetNewTitle(Solution solution, string pattern, Dictionary<string, object> settingsOverride) {
            var activeDocument = Globals.DTE.ActiveDocument;
            var activeWindow = Globals.DTE.ActiveWindow;
            var solutionFp = solution?.FullName;
            if (activeDocument == null && string.IsNullOrEmpty(solutionFp)) {
                var window = Globals.DTE.ActiveWindow;
                if (window == null || window.Caption == Globals.DTE.MainWindow.Caption) {
                    return this.IDEName;
                }
            }
            var path = string.Empty;

            if (!string.IsNullOrEmpty(solutionFp)) {
                path = solutionFp;
            }
            else if (activeDocument != null) {
                path = activeDocument.FullName;
            }

            var pathParts = this.SplitPath(path);

            pattern = this.TagRegex.Replace(pattern, match => {
                try {
                    var tag = match.Groups[1].Value;
                    try {
                        switch (tag) {
                            case "configurationName":
                                return Globals.GetActiveConfigurationNameOrEmpty(solution) ?? "";
                            case "platformName":
                                return Globals.GetPlatformNameOrEmpty(solution) ?? "";
                            case "projectName":
                                return Globals.GetActiveProjectNameOrEmpty() ?? "";
                            case "solutionName":
                                return Globals.GetSolutionNameOrEmpty(solution) ?? "";
                            case "gitBranchName":
                                Globals.UpdateGitExecFp(this.Settings.GitDirectory); // there is likely a better way to adjust the git path
                                return Globals.GetGitBranchNameOrEmpty(solution) ?? "";
                            case "workspaceName":
                                return Globals.GetWorkspaceNameOrEmpty(solution) ?? "";
                            case "workspaceOwnerName":
                                return Globals.GetWorkspaceOwnerNameOrEmpty(solution) ?? "";
                            case "documentName":
                                return Globals.GetActiveDocumentNameOrEmpty(activeDocument, activeWindow) ?? "";
                            case "vsMajorVersion":
                                return Globals.VsMajorVersion.ToString(CultureInfo.InvariantCulture);
                            case "vsMajorVersionYear":
                                return Globals.VsMajorVersionYear.ToString(CultureInfo.InvariantCulture);
                            case "ideName":
                                return this.IDEName ?? "";
                            case "parentPath":
                                var closestParentDepth = this.Settings.ClosestParentDepth;
                                var farthestParentDepth = this.Settings.FarthestParentDepth;
                                if (settingsOverride != null) {
                                    int v;
                                    if (TryGetSettingValue(settings: settingsOverride, key: "ClosestParentDepth", value: out v)) {
                                        closestParentDepth = v;
                                    }
                                    if (TryGetSettingValue(settings: settingsOverride, key: "FarthestParentDepth", value: out v)) {
                                        farthestParentDepth = v;
                                    }
                                }
                                return GetParentPath(pathParts, closestParentDepth: closestParentDepth, farthestParentDepth: farthestParentDepth) ?? "";
                            default:
                                if (tag.StartsWith("parent")) {
                                    int n;
                                    if (int.TryParse(tag.Substring(6 /*length of parent*/), out n)) {
                                        if (n >= 0 && n <= pathParts.Length) {
                                            return pathParts[pathParts.Length - n - 1]; // n=1 means direct parent
                                        }
                                        return string.Empty;
                                    }
                                    return "[invalid:" + tag + "]";
                                }
                                break;
                        }
                        return match.Value;
                    }
                    catch (Exception ex) {
                        if (this.Settings.EnableDebugMode) {
                            WriteOutput("ReplaceTag (" + tag + ") failed: " + ex);
                        }
                        throw;
                    }
                }
                catch {
                    return "";
                }
            });
            var appendedString = this.Settings.AppendedString;
            if (settingsOverride != null) {
                string v;
                if (TryGetSettingValue(settings: settingsOverride, key: "AppendedString", value: out v)) {
                    appendedString = v;
                }
            }
            return pattern + " " + appendedString;
        }

        private string[] SplitPath(string path) {
            if (string.IsNullOrEmpty(path)) {
                return new string[0];
            }

            var root = Path.GetPathRoot(path);
            var parts = new List<string>();
            if (!string.IsNullOrEmpty(root)) {
                parts.Add(root);
            }
            parts.AddRange(path.Substring(root.Length).Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries));
            return parts.ToArray();
        }

        private static string GetParentPath(string[] parents, int closestParentDepth, int farthestParentDepth) {
            return Path.Combine(parents.Reverse().Skip(closestParentDepth)
                .Take(farthestParentDepth - closestParentDepth + 1)
                .Reverse()
                .ToArray());
        }

        private void ChangeWindowTitle(string title) {
            try {
                Globals.BeginInvokeOnUIThread(() => {
                    try {
                        System.Windows.Application.Current.MainWindow.Title = Globals.DTE.MainWindow.Caption;
                        if (System.Windows.Application.Current.MainWindow.Title != title) {
                            System.Windows.Application.Current.MainWindow.Title = title;
                        }
                    }
                    catch (Exception) {
                        // ignored
                    }
                });
            }
            catch (Exception ex) {
                if (this.Settings.EnableDebugMode) {
                    WriteOutput("ChangeWindowTitle failed: " + ex);
                }
            }
        }

        private static readonly System.Web.Script.Serialization.JavaScriptSerializer JsonSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();

        private readonly Dictionary<string, Dictionary<string, object>> SettingsOverridesCache = new Dictionary<string, Dictionary<string, object>>();

        private Dictionary<string, object> GetSettingsOverride(string solutionFp) {
            if (string.IsNullOrWhiteSpace(solutionFp) || !this.Settings.AllowSolutionSettingsOverrides && (string.IsNullOrWhiteSpace(this.Settings.GlobalSolutionSettingsOverridesFp) || !File.Exists(this.Settings.GlobalSolutionSettingsOverridesFp))) {
                return null;
            }
            Dictionary<string, object> settings;
            //this.SettingsOverridesCache.Clear();
            if (this.SettingsOverridesCache.TryGetValue(solutionFp, out settings)) {
                return settings;
            }
            try {
                if (File.Exists(solutionFp)) {
                    if (!string.IsNullOrWhiteSpace(this.Settings.GlobalSolutionSettingsOverridesFp) && File.Exists(this.Settings.GlobalSolutionSettingsOverridesFp)) {
                        var lines = JsonSerializer.Deserialize<Dictionary<string, object>[]>(File.ReadAllText(this.Settings.GlobalSolutionSettingsOverridesFp));
                        foreach (var line in lines) {
                            object sln;
                            if (line.TryGetValue("SolutionFilePath", out sln) && solutionFp.EndsWith(sln as string ?? "")) {
                                this.SettingsOverridesCache.Add(solutionFp, line);
                                return line;
                            }
                        }
                    }
                    var fp = solutionFp + ".rncfg";
                    if (this.Settings.AllowSolutionSettingsOverrides && File.Exists(fp)) {
                        settings = JsonSerializer.Deserialize<Dictionary<string, object>>(File.ReadAllText(fp));
                        this.SettingsOverridesCache.Add(solutionFp, settings);
                        return settings;
                    }
                }
            }
            catch (Exception ex) {
                if (this.Settings.EnableDebugMode) {
                    WriteOutput("Failed fetching settings overrides: " + ex);
                }
            }
            this.SettingsOverridesCache.Add(solutionFp, null);
            return null;
        }

        private Dictionary<string, object> GetSettingsOverrideOld(string solutionFp) {
            if (string.IsNullOrWhiteSpace(solutionFp) || !this.Settings.AllowSolutionSettingsOverrides && (string.IsNullOrWhiteSpace(this.Settings.GlobalSolutionSettingsOverridesFp) || !File.Exists(this.Settings.GlobalSolutionSettingsOverridesFp))) {
                return null;
            }
            Dictionary<string, object> settings;
            if (this.SettingsOverridesCache.TryGetValue(solutionFp, out settings)) {
                return settings;
            }
            try {
                if (File.Exists(solutionFp)) {
                    var fp = solutionFp + ".rncfg";
                    if (File.Exists(this.Settings.GlobalSolutionSettingsOverridesFp)) {
                        var lines = File.ReadAllLines(this.Settings.GlobalSolutionSettingsOverridesFp);
                        foreach (var line in lines) {
                            settings = (Dictionary<string, object>)JsonSerializer.DeserializeObject(line);
                            object sln;
                            if (settings.TryGetValue("SolutionFilePath", out sln) && solutionFp.EndsWith(sln as string ?? "")) {
                                this.SettingsOverridesCache.Add(solutionFp, settings);
                                return settings;
                            }
                        }
                    }
                    if (File.Exists(fp)) {
                        settings = (Dictionary<string, object>)JsonSerializer.DeserializeObject(File.ReadAllText(fp));
                        this.SettingsOverridesCache.Add(solutionFp, settings);
                        return settings;
                    }
                }
            }
            catch (Exception ex) {
                if (this.Settings.EnableDebugMode) {
                    WriteOutput("Failed fetching settings overrides: " + ex);
                }
            }
            this.SettingsOverridesCache.Add(solutionFp, null);
            return null;
        }

        private static bool TryGetSettingValue<T>(Dictionary<string, object> settings, string key, out T value) {
            value = default(T);
            try {
                object v;
                if (settings.TryGetValue(key, out v)) {
                    value = (T)v;
                    return true;
                }
            }
            catch {
                // do nothing
            }
            return false;
        }

        private static void WriteOutput(string str) {
            try {
                Globals.InvokeOnUIThread(() => {
                    var outWindow = GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;
                    var generalPaneGuid = VSConstants.OutputWindowPaneGuid.DebugPane_guid;
                    // P.S. There's also the VSConstants.GUID_OutWindowDebugPane available.
                    if (outWindow != null) {
                        IVsOutputWindowPane generalPane;
                        outWindow.GetPane(ref generalPaneGuid, out generalPane);
                        generalPane.OutputString("RenameVSWindowTitle: " + str + "\r\n");
                        generalPane.Activate();
                    }
                });
            }
            catch {
                // ignored
            }
        }
    }
}