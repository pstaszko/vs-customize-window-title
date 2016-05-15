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
        [DisplayName("Farthest parent folder depth")]
        [Description("Distance of the farthest parent folder to be shown. 1 will only show the folder of the opened projet/solution file, before the project/folder name. Default: 1.")]
        public int FarthestParentDepth { get; set; } = 1;

        [Category("General")]
        [DisplayName("Closest parent folder depth")]
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
        [DisplayName("Solution in design mode")]
        [Description("Default: [parentPath]\\[solutionName] - [ideName]. See 'Supported tags' section on the left or the documentation on the Visual Studio Gallery for more guidance.")]
        public string PatternIfDesignMode { get; set; } = "[parentPath]\\[solutionName] - [ideName]";

        [Category("Patterns")]
        [DisplayName("Solution in break mode")]
        [Description("Default: [parentPath]\\[solutionName] (Debugging) - [ideName]. The appended string parameter will be added at the end automatically to identify that the title is being improved. See 'Supported tags' section on the left or the documentation on the Visual Studio Gallery for more guidance.")]
        public string PatternIfBreakMode { get; set; } = "[parentPath]\\[solutionName] (Debugging) - [ideName]";

        [Category("Patterns")]
        [DisplayName("Solution in running mode")]
        [Description("Default: [parentPath]\\[solutionName] (Running) - [ideName]. The appended string parameter will be added at the end automatically to identify that the title is being improved. See 'Supported tags' section on the left or the documentation on the Visual Studio Gallery for more guidance.")]
        public string PatternIfRunningMode { get; set; } = "[parentPath]\\[solutionName] (Running) - [ideName]";

        [Category("Patterns")]
        [DisplayName("Appended string")]
        [Description("String to be added at the end of the title to identify that it has been rewritten. If not default and Always rewrite titles, the detection of concurrent instances with the same title may not work. Default: '*'.")]
        public string AppendedString { get; set; } = "*";

        [Category("Patterns")]
        [DisplayName("Allow solution-specific settings overrides")]
        [Description("Default: False. If true, will use settings overrides from any existing 'MySolution.sln.rncfg' file found in the same directory as the MySolution.sln file. The file should contain a valid JSON dictionary. Supported \"key\":valueType are: 'FarthestParentDepth':int, 'ClosestParentDepth':int, 'PatternIfDesignMode':str, 'PatternIfBreakMode':str, 'PatternIfRunningMode':str, 'AppendedString':str. Such overrides will only be loaded once upon opening the solution.")]
        public bool AllowSolutionSettingsOverrides { get; set; } = false;

        [Category("Patterns")]
        [DisplayName("Global solution-specific settings overrides file")]
        [Description("Default: Empty. Path of an existing file that should contain a JSON array of JSON dictionaries in the same format as defined above. Each dictionary should contain an additional 'SolutionFilePath':str key value pair, specifying to which solution file (full or partial path matching the end of the actual path) each override should be applied. This takes precedence over any .sln.rncfg file that may exist.")]
        public string GlobalSolutionSettingsOverridesFp { get; set; } = null;

        [Category("git")]
        [DisplayName("Git directory")]
        [Description("Default: Empty. Search windows PATH for git if empty.")]
        public string GitDirectory { get; set; } = "";

        [Category("Debug")]
        [DisplayName("Enable debug mode")]
        [Description("Set to true to activate debug output to Output window.")]
        public bool EnableDebugMode { get; set; }
    }
}