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
using System.Xml;
using System.Text;

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

        public static RenameVSWindowTitle CurrentPackage;

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
            CurrentPackage=this;
            Globals.DTE = (DTE2)GetGlobalService(typeof(DTE));
            Globals.DTE.Events.DebuggerEvents.OnEnterBreakMode += this.OnIdeEvent;
            Globals.DTE.Events.DebuggerEvents.OnEnterRunMode += this.OnIdeEvent;
            Globals.DTE.Events.DebuggerEvents.OnEnterDesignMode += this.OnIdeEvent;
            Globals.DTE.Events.DebuggerEvents.OnContextChanged += this.OnIdeEvent;
            Globals.DTE.Events.SolutionEvents.AfterClosing += this.OnIdeSolutionEvent;
            Globals.DTE.Events.SolutionEvents.Opened += this.OnIdeSolutionEvent;
            Globals.DTE.Events.SolutionEvents.Renamed += this.OnIdeSolutionEvent;
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

        private void OnIdeSolutionEvent(string oldname) {
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

        // clear settings cache and update
        private void OnIdeSolutionEvent() {
            ClearSettingsCache();
            OnIdeEvent();
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
            CurrentPackage=this;

            GlobalSettings.SettingsCleared = this.OnSettingsCleared;
            SolutionSettings.SettingsCleared = this.OnSettingsCleared;

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


        private OptionPageGrid _SettingsPage = null;

        internal OptionPageGrid Settings
        {
            get
            {
                if (_SettingsPage==null)
                {
                    _SettingsPage=this.GetDialogPage(typeof(OptionPageGrid)) as OptionPageGrid;  // as is faster than cast
                    _SettingsPage.SettingsChanged+= _SettingsPage_SettingsChanged;
                }
                return _SettingsPage;
            }
        }

        private void _SettingsPage_SettingsChanged(object sender, EventArgs e)
        {
            this.OnIdeSolutionEvent();
        }

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

                SettingsSet config = GetSettings(solutionFp);

                var pattern = this.GetPattern(solutionFp, useDefaultPattern, config);
                this.ChangeWindowTitle(this.GetNewTitle(solution, pattern, config));
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

        SettingsWatcher SolutionSettings = new SettingsWatcher(false);
        SettingsWatcher GlobalSettings = new SettingsWatcher(true);
        SettingsSet CurrentConfig;

        private void ClearSettingsCache()
        {
            if (this.Settings.EnableDebugMode) WriteOutput("ClearSettingsCache.");

            SolutionSettings.Clear();
            GlobalSettings.Clear();
            CurrentConfig = null;
        }

        private void OnSettingsCleared()
        {
            CurrentConfig=null; // force reload
        }


        internal SettingsSet GetSettings(string solutionFp)
        {
            GlobalSettings.Update(this.Settings.GlobalSolutionSettingsOverridesFp);
            SolutionSettings.Update(string.IsNullOrEmpty(solutionFp) ? null : solutionFp+Globals.SolutionConfigExt);

            // config already loaded, use cache
            if (CurrentConfig!=null && CurrentConfig.SolutionFilePath==solutionFp)
            {
                return CurrentConfig;
            }

            var Settings = this.Settings;

            // init values from settings
            CurrentConfig = new SettingsSet
            {
                ClosestParentDepth= Settings.ClosestParentDepth,
                FarthestParentDepth=Settings.FarthestParentDepth,
                AppendedString=Settings.AppendedString,
                PatternIfBreakMode = Settings.PatternIfBreakMode,
                PatternIfDesignMode= Settings.PatternIfDesignMode,
                PatternIfRunningMode= Settings.PatternIfRunningMode,
            };
            if (string.IsNullOrEmpty(solutionFp))
                return CurrentConfig;

            CurrentConfig.SolutionFilePath=solutionFp;
            CurrentConfig.SolutionFileName=Path.GetFileName(solutionFp);
            CurrentConfig.SolutionName= Path.GetFileNameWithoutExtension(solutionFp);

            // no override allowed, return
            if (!this.Settings.AllowSolutionSettingsOverrides)
                return CurrentConfig;

            // check global override file
            if (GlobalSettings.TryUpdate(CurrentConfig))
            {
                return CurrentConfig;
            }

            // check solution override file
            if (SolutionSettings.TryUpdate(CurrentConfig))
            {
                return CurrentConfig;
            }
            return CurrentConfig;
        }

        private static bool isSameFile(ref DateTime? writeTime, string path)
        {
            DateTime? newTime;
            if (string.IsNullOrEmpty(path))
                newTime=null;
            else
            {
                var fi = new FileInfo(path);
                if (fi.Exists)
                    newTime=fi.LastWriteTimeUtc;
                else
                    newTime=null;
            }
            if (writeTime!=newTime)
            {
                writeTime=newTime;
                return false;
            }
            return true;
        }

        public const string DefaultDesignModePattern = "[solutionName] - [ideName]";
        public const string DefaultBreakModePattern = "[solutionName] (Debugging) - [ideName]";
        public const string DefaultRunningModePattern = "[solutionName] (Running) - [ideName]";
        public const string DefaultPatternIfDocumentButNoSolutionOpen = "[documentName] - [ideName]";
        public const string DefaultPatternIfNothingOpen = "[ideName]";
        public const string DefaultAppendedString = "*";
        public const int DefaultClosestParentDepth = 1;
        public const int DefaultFarthestParentDepth = 1;

        private string GetPattern(string solutionFp, bool useDefault, SettingsSet settingsOverride) {
            var Settings = this.Settings;
            if (string.IsNullOrEmpty(solutionFp)) {
                var document = Globals.DTE.ActiveDocument;
                var window = Globals.DTE.ActiveWindow;
                if (string.IsNullOrEmpty(document?.FullName) && string.IsNullOrEmpty(window?.Caption)) {
                    return useDefault ? DefaultPatternIfNothingOpen : Settings.PatternIfNothingOpen;
                }
                return useDefault ? DefaultPatternIfDocumentButNoSolutionOpen : Settings.PatternIfDocumentButNoSolutionOpen;
            }
            string designModePattern = null;
            string breakModePattern = null;
            string runningModePattern = null;
            if (!useDefault) {
                designModePattern = settingsOverride?.PatternIfDesignMode ?? Settings.PatternIfDesignMode;
                breakModePattern = settingsOverride?.PatternIfBreakMode ?? Settings.PatternIfBreakMode;
                runningModePattern = settingsOverride?.PatternIfRunningMode ?? Settings.PatternIfRunningMode;
            }
            if (Globals.DTE.Debugger == null || Globals.DTE.Debugger.CurrentMode == dbgDebugMode.dbgDesignMode) {
                return designModePattern ?? DefaultDesignModePattern;
            }
            if (Globals.DTE.Debugger.CurrentMode == dbgDebugMode.dbgBreakMode) {
                return breakModePattern ?? DefaultBreakModePattern;
            }
            if (Globals.DTE.Debugger.CurrentMode == dbgDebugMode.dbgRunMode) {
                return runningModePattern ?? DefaultRunningModePattern;
            }
            throw new Exception("No matching state found");
        }

        public static readonly string[] SupportedTags = new string[] {
            "documentName",
            "projectName",
            "solutionName",
            "parentX",
            "parentPath",
            "ideName",
            "configurationName",
            "platformName",
            "vsMajorVersion",
            "vsMajorVersionYear",
            "gitBranchName",
            "workspaceName",
            "workspaceOwnerName",
        };

        readonly Regex TagRegex = new Regex(@"\[([^\]]+)\]", RegexOptions.Multiline);

        internal string GetNewTitle(Solution solution, string pattern, SettingsSet cfg) {
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
                            return Globals.GetActiveConfigurationNameOrEmpty(solution) ?? string.Empty;
                        case "platformName":
                            return Globals.GetPlatformNameOrEmpty(solution) ?? string.Empty;
                        case "projectName":
                            return Globals.GetActiveProjectNameOrEmpty() ?? string.Empty;
                        case "solutionName":
                            return cfg.SolutionName ?? string.Empty;
                        case "gitBranchName":
                            Globals.UpdateGitExecFp(this.Settings.GitDirectory); // there is likely a better way to adjust the git path
                            return Globals.GetGitBranchNameOrEmpty(solution) ?? string.Empty;
                        case "workspaceName":
                            return Globals.GetWorkspaceNameOrEmpty(solution) ?? string.Empty;
                        case "workspaceOwnerName":
                            return Globals.GetWorkspaceOwnerNameOrEmpty(solution) ?? string.Empty;
                        case "documentName":
                            return Globals.GetActiveDocumentNameOrEmpty(activeDocument, activeWindow) ?? string.Empty;
                        case "vsMajorVersion":
                            return Globals.VsMajorVersion.ToString(CultureInfo.InvariantCulture);
                        case "vsMajorVersionYear":
                            return Globals.VsMajorVersionYear.ToString(CultureInfo.InvariantCulture);
                        case "ideName":
                            return this.IDEName ?? string.Empty;
                        case "path":
                            return solution?.FullName ?? string.Empty;
                        case "parentPath":
                            return GetParentPath(pathParts, cfg?.ClosestParentDepth ?? this.Settings.ClosestParentDepth, cfg?.FarthestParentDepth ?? this.Settings.FarthestParentDepth) ?? string.Empty;
                        case "parentX":
                            return "[invalid: replace X with number]";
                        case "pathX":
                            return "[invalid: replace X with number]";
                        default:
                            if (tag.StartsWith("parent")) {
                                string smartParent = SmartParent(pathParts, tag.Substring(6 /*length of "parent"*/));
                                return smartParent ?? "[invalid:" + tag + "]";
                            } else
                            if (tag.StartsWith("path")) {
                                string smartPath = SmartPath(pathParts, tag.Substring(4 /*length of "path"*/));
                                return smartPath?? "[invalid:" + tag + "]";
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
            string appendedString = cfg?.AppendedString ?? this.Settings.AppendedString;
            return pattern + " " + appendedString;
        }

        // pattern supports: 

        static char[] smartSep = new char[] { ',' };
        private static string SmartParent(string[] pathParts, string pattern)
        {
            int z = pattern.IndexOf('-');
            if (z==-1)
            {
                int n;
                if (!int.TryParse(pattern, out n))
                    return null;

                if (n >= 0 && n < pathParts.Length)
                {
                    return pathParts[pathParts.Length - n - 1]; // n=1 means direct parent
                }
                return string.Empty;
            }
            int a, e;
            if (int.TryParse(pattern.Substring(0,z), out a) && int.TryParse(pattern.Substring(z+1), out e))
            {
                return GetParentPath(pathParts, a, e);
            }
            return null;
        }

        private static string SmartPath(string[] pathParts, string pattern)
        {
            int z = pattern.IndexOf('-');
            if (z==-1)
            {
                int n;
                if (!int.TryParse(pattern, out n))
                    return null;

                if (n >= 0 && n < pathParts.Length)
                {
                    return pathParts[n]; // n=1 means direct parent
                }
                return string.Empty;
            }
            int a, e;
            if (int.TryParse(pattern.Substring(0,z), out a) && int.TryParse(pattern.Substring(z+1), out e))
            {
                return GetPathRange(pathParts, a, e);
            }
            return null;
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
            if(closestParentDepth>farthestParentDepth)
            {
                // swap if provided in wrong order
                int t = closestParentDepth;
                closestParentDepth=farthestParentDepth;
                farthestParentDepth=t;
            }
            return Path.Combine(parents.Reverse().Skip(closestParentDepth)
                .Take(farthestParentDepth - closestParentDepth + 1)
                .Reverse()
                .ToArray());
        }
       private static string GetPathRange(string[] parents, int closest, int farthest) {
            if(closest>farthest)
            {
                // swap if provided in wrong order
                int t = closest;
                closest=farthest;
                farthest=t;
            }
            return Path.Combine(parents.Skip(closest)
                .Take(farthest - closest + 1)
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

        private static void WriteOutput(string str, params object[] args) {
            try {
                Globals.InvokeOnUIThread(() => {
                    var outWindow = GetGlobalService(typeof(SVsOutputWindow)) as IVsOutputWindow;
                    var generalPaneGuid = VSConstants.OutputWindowPaneGuid.DebugPane_guid;
                    // P.S. There's also the VSConstants.GUID_OutWindowDebugPane available.
                    if (outWindow != null) {
                        IVsOutputWindowPane generalPane;
                        outWindow.GetPane(ref generalPaneGuid, out generalPane);
                        generalPane.OutputString("RenameVSWindowTitle: " + string.Format(str,args) + "\r\n");
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