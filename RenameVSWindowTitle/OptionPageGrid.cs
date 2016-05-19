using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

//http://stackoverflow.com/questions/24291249/dialogpage-string-array-not-persisted
//http://www.codeproject.com/Articles/351172/CodeStash-a-journey-into-the-dark-side-of-Visual-S

namespace ErwinMayerLabs.RenameVSWindowTitle {
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [CLSCompliant(false), ComVisible(true)]
    public class OptionPageGrid : DialogPage {
        [Category("General")]
        [DisplayName("Always rewrite titles")]
        [Description("If set to true, will rewrite titles even if no conflict with other instances is detected. Default: true. Conflicts may not be detected if patterns have been changed from the default.")]
        public bool AlwaysRewriteTitles { get; set; } = true;

        [Category("General")]
        [DisplayName("Farthest parent folder depth (override as FarthestParentDepth)")]
        [Description("Distance of the farthest parent folder to be shown. 1 will only show the folder of the opened projet/solution file, before the project/folder name. Default: 1.")]
        public int FarthestParentDepth { get; set; } = 1;

        [Category("General")]
        [DisplayName("Closest parent folder depth (override as ClosestParentDepth)")]
        [Description("Distance of the closest parent folder to be shown. 1 will only show the folder of the opened projet/solution file, before the project/folder name. Default: 1.")]
        public int ClosestParentDepth { get; set; } = 1;

        [Category("Patterns")]
        [DisplayName("No document or solution open")]
        [Description("Default: [ideName]. See 'Supported Tags' section on the left or the documentation on the Visual Studio Gallery for more guidance.")]
        public string PatternIfNothingOpen { get; set; } = "[ideName]";

        [Category("Patterns")]
        [DisplayName("Document (no solution) open")]
        [Description("Default: [documentName] - [ideName]. See 'Supported tags' section on the left or the documentation on the Visual Studio Gallery for more guidance.")]
        public string PatternIfDocumentButNoSolutionOpen { get; set; } = "[documentName] - [ideName]";

        [Category("Patterns")]
        [DisplayName("Solution in design mode (override as PatternIfDesignMode)")]
        [Description("Default: [parentPath]\\[solutionName] - [ideName]. See 'Supported tags' section on the left or the documentation on the Visual Studio Gallery for more guidance.")]
        public string PatternIfDesignMode { get; set; } = "[parentPath]\\[solutionName] - [ideName]";

        [Category("Patterns")]
        [DisplayName("Solution in break mode (override as PatternIfBreakMode)")]
        [Description("Default: [parentPath]\\[solutionName] (Debugging) - [ideName]. The appended string parameter will be added at the end automatically to identify that the title is being improved. See 'Supported tags' section on the left or the documentation on the Visual Studio Gallery for more guidance.")]
        public string PatternIfBreakMode { get; set; } = "[parentPath]\\[solutionName] (Debugging) - [ideName]";

        [Category("Patterns")]
        [DisplayName("Solution in running mode (override as PatternIfRunningMode)")]
        [Description("Default: [parentPath]\\[solutionName] (Running) - [ideName]. The appended string parameter will be added at the end automatically to identify that the title is being improved. See 'Supported tags' section on the left or the documentation on the Visual Studio Gallery for more guidance.")]
        public string PatternIfRunningMode { get; set; } = "[parentPath]\\[solutionName] (Running) - [ideName]";

        [Category("Patterns")]
        [DisplayName("Appended string (override as AppendedString)")]
        [Description("String to be added at the end of the title to identify that it has been rewritten. If not default and Always rewrite titles, the detection of concurrent instances with the same title may not work. Default: '*'.")]
        public string AppendedString { get; set; } = "*";

        [Category("Settings-Overrides")]
        [DisplayName("Allow solution-specific settings overrides")]
        [Description("Default: False. If true, will use settings overrides from any existing 'MySolution.sln.rnxcfg' file to be placed in the same directory as the MySolution.sln file. The file is in xml format containing a single RenameVSWindowTitle/Solution element with settings overrides.")]
        public bool AllowSolutionSettingsOverrides { get; set; } = false;

        [Category("Settings-Overrides")]
        [DisplayName("Global solution-specific settings overrides file path")]
        [Description("Default: Empty. The file is in xml format containing multiple RenameVSWindowTitle/Solution elements with settings overrides. Use Path to filter by full solution path or name only.")]
        public string GlobalSolutionSettingsOverridesFp { get; set; } = null;

        [Category("Git")]
        [DisplayName("Git binaries directory")]
        [Description("Default: Empty. Search windows PATH for git if empty.")]
        public string GitDirectory { get; set; } = "";

        [Category("Debug")]
        [DisplayName("Enable debug mode")]
        [Description("Set to true to activate debug output to Output window.")]
        public bool EnableDebugMode { get; set; }

        public event EventHandler SettingsChanged;

        protected override void OnApply(PageApplyEventArgs e)
        {
            base.OnApply(e);
            if (e.ApplyBehavior!= ApplyKind.Apply)
                return;
            var settingsChanged = SettingsChanged;
            if (settingsChanged!=null)
                settingsChanged(this, EventArgs.Empty);
        }
    }
}