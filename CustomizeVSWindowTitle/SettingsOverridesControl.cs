using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;

namespace ErwinMayerLabs.RenameVSWindowTitle
{
    public partial class SettingsOverridesControl
    {
        public SettingsOverridesControl(SettingsOverridesPageGrid optionsPage)
        {
            this.InitializeComponent();
            this.OptionsPage = optionsPage;
            this.propertyGrid1.SelectedObject = this.OptionsPage;
        }

        private void SettingsOverridesControl_Paint(object sender, PaintEventArgs e)
        {
            this.ResizeDescriptionArea(this.propertyGrid1, lines: 7);
        }

        private readonly SettingsOverridesPageGrid OptionsPage;

        private void ResizeDescriptionArea(PropertyGrid grid, int lines)
        {
            try
            {
                var info = grid.GetType().GetProperty("Controls");
                var collection = (ControlCollection)info.GetValue(grid, null);

                foreach (var control in collection)
                {
                    var type = control.GetType();

                    if ("DocComment" == type.Name)
                    {
                        const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic;
                        var field = type.BaseType.GetField("userSized", Flags);
                        field.SetValue(control, true);

                        info = type.GetProperty("Lines");
                        info.SetValue(control, lines, null);

                        grid.HelpVisible = true;
                        break;
                    }
                }
            }

            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }

        string _SolutionFp;

        public string SolutionFp
        {
            get
            {
                return this._SolutionFp;
            }
            set
            {
                this._SolutionFp = value;
                this.btSolutionConfig.Enabled = !string.IsNullOrEmpty(value);
                this.btSolutionConfig.Tag = this.btSolutionConfig.Enabled ? value + Globals.SolutionSettingsOverrideExtension : null;
            }
        }

        string _GlobalSettingsFp;

        public string GlobalSettingsFp
        {
            get
            {
                return this._GlobalSettingsFp;
            }
            set
            {
                this._GlobalSettingsFp = value;

                this.btGlobalConfig.Tag = value;
                this.btGlobalConfig.Enabled = !string.IsNullOrEmpty(value);
            }
        }


        private void btGlobalConfig_Click(object sender, EventArgs e)
        {
            PSCustomizeVSWindowTitle.CurrentPackage.UiSettingsOverridesOptions.GlobalSolutionSettingsOverridesFp = this._GlobalSettingsFp;

            if (this._GlobalSettingsFp != null)
            {
                try
                {
                    this.OpenText(this._GlobalSettingsFp, true);
                }
                catch
                {
                    MessageBox.Show("A problem occurred while trying to open/create the file. Please check that the path is well formed and retry.", "Rename Visual Studio Window Title",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            else
            {
                MessageBox.Show("Please enter a file path and retry (if the file does not exist, a sample one will be created)", "Rename Visual Studio Window Title",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void btSolutionConfig_Click(object sender, EventArgs e)
        {
            if (this.btSolutionConfig.Tag != null)
            {
                if (Globals.DTE.Solution == null)
                {
                    MessageBox.Show("Please open a solution first.", "Rename Visual Studio Window Title",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                try
                {
                    this.OpenText(this.btSolutionConfig.Tag.ToString(), false);
                }
                catch
                {
                    MessageBox.Show("A problem occurred while trying to open/create the file. Please check that the path is well formed and retry.", "Rename Visual Studio Window Title",
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void OpenText(string path, bool bGlobal)
        {
            var settings = PSCustomizeVSWindowTitle.CurrentPackage?.GetSettings(this._SolutionFp);
            if (settings == null) return;
            var sampleSln = Globals.GetExampleSolution(this._SolutionFp);
            var bIsNewFile = !File.Exists(path);
            if (bIsNewFile)
            {
                var doc = new XmlDocument();
                doc.AppendChild(doc.CreateXmlDeclaration("1.0", "utf-8", string.Empty));
                var rootNode = doc.CreateElement("PSCustomizeVSWindowTitle");
                doc.AppendChild(rootNode);
                XmlElement s;
                if (!string.IsNullOrEmpty(this._SolutionFp))
                {
                    var sn = string.IsNullOrEmpty(settings.SolutionFilePath) ? string.Empty : Path.GetFileNameWithoutExtension(settings.SolutionFilePath);

                    s = doc.CreateElement("SettingsSet");
                    if (bGlobal)
                    {
                        addAttr(doc, s, Globals.PathTag, sampleSln, false);
                    }
                    addAttr(doc, s, Globals.SolutionNameTag, settings.SolutionName, sn.Equals(settings.SolutionName));
                    addAttr(doc, s, Globals.ClosestParentDepthTag, settings.ClosestParentDepth?.ToString(), settings.ClosestParentDepth == PSCustomizeVSWindowTitle.DefaultClosestParentDepth);
                    addAttr(doc, s, Globals.FarthestParentDepthTag, settings.FarthestParentDepth?.ToString(), settings.FarthestParentDepth == PSCustomizeVSWindowTitle.DefaultFarthestParentDepth);
                    addAttr(doc, s, Globals.AppendedStringTag, settings.AppendedString, settings.AppendedString == PSCustomizeVSWindowTitle.DefaultAppendedString);
                    addAttr(doc, s, Globals.PatternIfRunningModeTag, settings.PatternIfRunningMode, settings.PatternIfRunningMode == PSCustomizeVSWindowTitle.DefaultPatternIfRunningMode);
                    addAttr(doc, s, Globals.PatternIfBreakModeTag, settings.PatternIfBreakMode, settings.PatternIfBreakMode == PSCustomizeVSWindowTitle.DefaultPatternIfBreakMode);
                    addAttr(doc, s, Globals.PatternIfDesignModeTag, settings.PatternIfDesignMode, settings.PatternIfDesignMode == PSCustomizeVSWindowTitle.DefaultPatternIfDesignMode);
                    rootNode.AppendChild(s);
                }
                //rootNode.AppendChild(doc.CreateComment("See following sample SettingsSet (remove -Example-Children from the tag name to use as is). All overrides are specified as attributes."));
                //s = doc.CreateElement("SettingsSet-Example-Children");
                //s.AppendChild(doc.CreateComment("Element-Style SettingsSet example (all overrides are specified as child elements)."));
                //if (bGlobal) {
                //    s.AppendChild(doc.CreateComment("Multiple Path child elements are allowed."));
                //}
                //if (bGlobal) addVal(doc, s, Globals.PathTag, sampleSln, false);
                //addVal(doc, s, Globals.SolutionNameTag, cfg.SolutionName, false);
                //addVal(doc, s, Globals.ClosestParentDepthTag, CustomizeVSWindowTitle.DefaultClosestParentDepth.ToString(), false);
                //addVal(doc, s, Globals.FarthestParentDepthTag, CustomizeVSWindowTitle.DefaultFarthestParentDepth.ToString(), false);
                //addVal(doc, s, Globals.AppendedStringTag, CustomizeVSWindowTitle.DefaultAppendedString, false);
                //addVal(doc, s, Globals.PatternIfRunningModeTag, CustomizeVSWindowTitle.DefaultPatternIfRunningMode, false);
                //addVal(doc, s, Globals.PatternIfBreakModeTag, CustomizeVSWindowTitle.DefaultPatternIfBreakMode, false);
                //addVal(doc, s, Globals.PatternIfDesignModeTag, CustomizeVSWindowTitle.DefaultPatternIfDesignMode, false);
                //rootNode.AppendChild(s);

                rootNode.AppendChild(doc.CreateComment("See following sample SettingsSet (remove -Example from the tag name to use as is). All overrides are specified as attributes."));
                s = doc.CreateElement("SettingsSet-Example");
                if (bGlobal)
                {
                    addAttr(doc, s, Globals.PathTag, sampleSln, false);
                }
                addAttr(doc, s, Globals.SolutionNameTag, settings.SolutionName, false);
                addAttr(doc, s, Globals.ClosestParentDepthTag, PSCustomizeVSWindowTitle.DefaultClosestParentDepth.ToString(), false);
                addAttr(doc, s, Globals.FarthestParentDepthTag, PSCustomizeVSWindowTitle.DefaultFarthestParentDepth.ToString(), false);
                addAttr(doc, s, Globals.AppendedStringTag, PSCustomizeVSWindowTitle.DefaultAppendedString, false);
                addAttr(doc, s, Globals.PatternIfRunningModeTag, PSCustomizeVSWindowTitle.DefaultPatternIfRunningMode, false);
                addAttr(doc, s, Globals.PatternIfBreakModeTag, PSCustomizeVSWindowTitle.DefaultPatternIfBreakMode, false);
                addAttr(doc, s, Globals.PatternIfDesignModeTag, PSCustomizeVSWindowTitle.DefaultPatternIfDesignMode, false);
                rootNode.AppendChild(s);

                var tmp = Path.GetTempFileName();
                var xws = new XmlWriterSettings
                {
                    NewLineOnAttributes = true,
                    CloseOutput = true,
                    ConformanceLevel = ConformanceLevel.Document,
                    Encoding = System.Text.Encoding.UTF8,
                    Indent = true,
                    IndentChars = "  ",
                    NewLineChars = Environment.NewLine,
                    NewLineHandling = NewLineHandling.Entitize
                };
                var xw = XmlWriter.Create(tmp, xws);
                doc.Save(xw);
                xw.Close();
                File.Move(tmp, path);
            }
            var dd = Globals.DTE.Application.Documents.Open(path);
            if (bIsNewFile)
            {
                dd.Saved = false;
                File.Delete(path);
            }
        }

        private static void addVal(XmlDocument doc, XmlElement S, string n, string val, bool asComment = false)
        {
            if (val == null)
                return;
            var E = doc.CreateElement(n);
            E.InnerText = val ?? string.Empty;

            if (asComment)
            {
                return;
                //S.AppendChild(doc.CreateComment(E.OuterXml));
            }
            else
            {
                S.AppendChild(E);
            }

        }
        private static void addAttr(XmlDocument doc, XmlElement S, string n, string val, bool asComment = false)
        {
            if (val == null)
                return;
            S.SetAttribute(asComment ? "__" + n : n, val);
        }

        private void btRegister_Click(object sender, EventArgs e)
        {
            VSocket.StartListen();
        }
    }
}