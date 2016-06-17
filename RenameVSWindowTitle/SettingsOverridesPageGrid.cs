using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;

namespace ErwinMayerLabs.RenameVSWindowTitle {
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [CLSCompliant(false), ComVisible(true)]
    public class SettingsOverridesPageGrid : DialogPage {
        [Category("Solution-specific overrides")]
        [DisplayName("Enable solution-specific overrides")]
        [Description("Default: False. If true, will try to use settings overrides from any 'MySolution.sln.rn.xml' file present in the same directory as the MySolution.sln file. " +
            "The file should be in xml format containing a single RenameVSWindowTitle/SettingsSet element, which attributes can be the following: " +
            "SolutionName, FarthestParentDepth, ClosestParentDepth, PatternIfDesignMode, PatternIfBreakMode, PatternIfRunningMode, AppendedString.")]
        [DefaultValue(false)]
        public bool AllowSolutionSettingsOverrides { get; set; } = false;

        [Category("Settings overrides")]
        [DisplayName("Global settings overrides file path")]
        [Description("Default: Empty. If not empty, will try to use settings overrides from the file specified that should be in xml format containing multiple RenameVSWindowTitle/SettingsSet elements as for .sln.rn.xml files, with the addition of a Path attribute or child node(s) to specify to which solution path or solution name each SettingsSet is applicable (wildcards are supported). Overrides from this global file take precedence over those found in .sln.rn.xml files.")]
        [Editor(typeof(FilePickerEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [FilePicker(false, null, "Xml files (*.xml)|*.xml|All files (*.*)|*.*", 1)]
        [DefaultValue(null)]
        public string GlobalSolutionSettingsOverridesFp { get; set; }

        SettingsOverridesControl SettingsOverridesControl;

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected override IWin32Window Window {
            get {
                this.SettingsOverridesControl = new SettingsOverridesControl(optionsPage: this) {
                    Dock = DockStyle.Fill,
                    Location = new System.Drawing.Point(0, 0)
                };
                return this.SettingsOverridesControl;
            }
        }

        public event EventHandler SettingsChanged;

        protected override void OnApply(PageApplyEventArgs e) {
            base.OnApply(e);
            if (e.ApplyBehavior != ApplyKind.Apply)
                return;
            this.SettingsChanged?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnActivate(CancelEventArgs e) {
            base.OnActivate(e);
            var solution = Globals.DTE.Solution;
            this.SettingsOverridesControl.SolutionFp = solution?.FullName;

            if (RenameVSWindowTitle.CurrentPackage != null) {
                //  if no global settingsfile defined, use default file name
                string gfp = RenameVSWindowTitle.CurrentPackage.SettingsOverrides.GlobalSolutionSettingsOverridesFp;
                if (string.IsNullOrEmpty(gfp))
                    gfp = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "RenameVSWindowTitle-global.xml");

                this.SettingsOverridesControl.GlobalSettingsFp = gfp; // if opened, the opening will set the global settings path
            }
            else {
                this.SettingsOverridesControl.GlobalSettingsFp = null;
            }
        }

    }
}