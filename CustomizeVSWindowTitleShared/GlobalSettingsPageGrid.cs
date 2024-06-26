using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using ErwinMayerLabs.RenameVSWindowTitle.Resolvers;
using Microsoft.VisualStudio.Shell;

//http://stackoverflow.com/questions/24291249/dialogpage-string-array-not-persisted
//http://www.codeproject.com/Articles/351172/CodeStash-a-journey-into-the-dark-side-of-Visual-S

namespace ErwinMayerLabs.RenameVSWindowTitle
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ComVisible(true)]
    public class GlobalSettingsPageGrid : DialogPage
    {
        [Category("General")]
        [DisplayName("Always rewrite titles")]
        [Description("Default: true. If true, will rewrite titles even if no conflict with other instances is detected. Conflicts may not be detected if patterns have been changed from the default.")]
        [DefaultValue(true)]
        public bool AlwaysRewriteTitles { get; set; } = true;

        [Category("General")]
        [DisplayName("Support for compact mode")]
        [Description("Default: true. If true, will also rewrite the compact window title when compact mode is enabled (without \"- [ideName]\" if present).")]
        [DefaultValue(true)]
        public bool RewriteCompactTitle { get; set; } = true;

        [Category("General")]
        [DisplayName("Farthest parent folder depth")]
        [Description("Default: 1. Distance of the farthest parent folder to be shown. 1 will only show the folder of the opened project/solution file, before the project/folder name")]
        [DefaultValue(PSCustomizeVSWindowTitle.DefaultFarthestParentDepth)]
        public int FarthestParentDepth { get; set; } = PSCustomizeVSWindowTitle.DefaultFarthestParentDepth;

        [Category("General")]
        [DisplayName("Closest parent folder depth")]
        [Description("Default: 1. Distance of the closest parent folder to be shown. 1 will only show the folder of the opened project/solution file, before the project/folder name.")]
        [DefaultValue(PSCustomizeVSWindowTitle.DefaultClosestParentDepth)]
        public int ClosestParentDepth { get; set; } = PSCustomizeVSWindowTitle.DefaultClosestParentDepth;

        [Category("General")]
        [DisplayName("Rewrite timer")]
        [Description("Default: 5000ms. Period to recheck if title may need to be rewritten, in case a relevant event wasn't detected.")]
        [DefaultValue(PSCustomizeVSWindowTitle.DefaultResetTitleTimerMsPeriod)]
        public int ResetTitleTimerMsPeriod { get; set; } = PSCustomizeVSWindowTitle.DefaultResetTitleTimerMsPeriod;

        [Category("Patterns")]
        [DisplayName("No document or solution open")]
        [Description("Default: [ideName]. See 'Supported Tags' section on the left for more guidance.")]
        [DefaultValue(PSCustomizeVSWindowTitle.DefaultPatternIfNothingOpen)]
        [Editor(typeof(EditablePatternEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string PatternIfNothingOpen { get; set; } = PSCustomizeVSWindowTitle.DefaultPatternIfNothingOpen;

        [Category("Patterns")]
        [DisplayName("Document (no solution) open")]
        [Description("Default: [documentName] - [ideName]. See 'Supported tags' section on the left for more guidance.")]
        [DefaultValue(PSCustomizeVSWindowTitle.DefaultPatternIfDocumentButNoSolutionOpen)]
        [Editor(typeof(EditablePatternEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [PreviewRequires(PreviewRequiresAttribute.Requirement.Document)]
        public string PatternIfDocumentButNoSolutionOpen { get; set; } = PSCustomizeVSWindowTitle.DefaultPatternIfDocumentButNoSolutionOpen;

        [Category("Patterns")]
        [DisplayName("Solution in design mode")]
        [Description("Default: [solutionName] [documentPath] (Design) [gitBranchName] [documentUnsaved] | Visual Studio. See 'Supported tags' section on the left for more guidance.")]
        [DefaultValue("[solutionName] [documentPath] (Design) [gitBranchName] [documentUnsaved] | Visual Studio")]
        [Editor(typeof(EditablePatternEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [PreviewRequires(PreviewRequiresAttribute.Requirement.Solution)]
        public string PatternIfDesignMode { get; set; } = "[solutionName] [documentPath] (Design) [gitBranchName] [documentUnsaved] | Visual Studio";

        [Category("Patterns")]
        [DisplayName("Solution in break mode")]
        [Description("Default: [solutionName] [documentPath] (Break) [gitBranchName] [documentUnsaved] | Visual Studio. The appended string parameter will be added at the end automatically to identify that the title is being improved. See 'Supported tags' section on the left for more guidance.")]
        [DefaultValue("[solutionName] [documentPath] (Break) [gitBranchName] [documentUnsaved] | Visual Studio")]
        [Editor(typeof(EditablePatternEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [PreviewRequires(PreviewRequiresAttribute.Requirement.Solution)]
        public string PatternIfBreakMode { get; set; } = "[solutionName] [documentPath] (Break) [gitBranchName] [documentUnsaved] | Visual Studio";

        [Category("Patterns")]
        [DisplayName("Solution in running mode")]
        [Description("Default: [solutionName] [documentPath] (Debugging) [gitBranchName] [documentUnsaved] | Visual Studio. The appended string parameter will be added at the end automatically to identify that the title is being improved. See 'Supported tags' section on the left for more guidance.")]
        [DefaultValue("[solutionName] [documentPath] (Debugging) [gitBranchName] [documentUnsaved] | Visual Studio")]
        [Editor(typeof(EditablePatternEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [PreviewRequires(PreviewRequiresAttribute.Requirement.Solution)]
        public string PatternIfRunningMode { get; set; } = "[solutionName] [documentPath] (Debugging) [gitBranchName] [documentUnsaved] | Visual Studio";

        [Category("Patterns")]
        [DisplayName("Appended string")]
        [Description("Default: '\t' (tab). String to be added at the end of the title to identify that it has been rewritten. If not default or '*', and \"Always rewrite titles\" is false, the detection of concurrent instances with the same title may not work.")]
        [DefaultValue(PSCustomizeVSWindowTitle.DefaultAppendedString)]
        public string AppendedString { get; set; } = PSCustomizeVSWindowTitle.DefaultAppendedString;

        [Category("Source control")]
        [DisplayName("Git binaries directory")]
        [Description("Default: Empty. Search windows PATH for git if empty.")]
        [Editor(typeof(FilePickerEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [FilePicker(true, GitInfo.GitExecFn, "Git executable(git.exe)|git.exe|All files(*.*)|*.*", 1)]
        [DefaultValue("")]
        public string GitDirectory { get; set; } = "";

        [Category("Source control")]
        [DisplayName("Hg binaries directory")]
        [Description("Default: Empty. Search windows PATH for hg if empty.")]
        [Editor(typeof(FilePickerEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [FilePicker(true, HgBranchNameResolver.HgExecFn, "Hg executable(hg.exe)|hg.exe|All files(*.*)|*.*", 1)]
        [DefaultValue("")]
        public string HgDirectory { get; set; } = "";

        [Category("Source control")]
        [DisplayName("SVN binaries directory")]
        [Description("Default: Empty. Search windows PATH for svn if empty.")]
        [Editor(typeof(FilePickerEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [FilePicker(true, SvnResolver.SvnExecFn, "SVN executable(svn.exe)|svn.exe|All files(*.*)|*.*", 1)]
        [DefaultValue("")]
        public string SvnDirectory { get; set; } = "";

        [Category("Source control")]
        [DisplayName("SVN directory separator")]
        [Description("Default: '/'. Specify the character used to separate the SVN directories.")]
        [DefaultValue("/")]
        public string SvnDirectorySeparator { get; set; } = "/";

        [Category("Debug")]
        [DisplayName("Enable debug mode")]
        [Description("Default: false. Set to true to activate debug output to Output window.")]
        [DefaultValue(false)]
        public bool EnableDebugMode { get; set; } = false;

        public event EventHandler SettingsChanged;

        protected override void OnApply(PageApplyEventArgs e)
        {
            base.OnApply(e);
            if (e.ApplyBehavior != ApplyKind.Apply)
                return;
            this.SettingsChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}