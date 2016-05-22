using System;
using System.IO;
using System.Xml;

namespace ErwinMayerLabs.RenameVSWindowTitle {
    public partial class SupportedTagsControl {
        public SupportedTagsControl() {
            this.InitializeComponent();
        }
        string solutionPath;

        public string SolutionPath {
            get
            {
                return solutionPath;
            }
            set
            {
                solutionPath=value;
                btSolutionConfig.Enabled=!string.IsNullOrEmpty(value);
                btSolutionConfig.Tag=btSolutionConfig.Enabled ? value + Globals.SolutionConfigExt : null;
            }
        }

        string globalSettingsPath;

        public string GlobalSettingsPath {
            get
            {
                return globalSettingsPath;
            }
            set
            {
                globalSettingsPath=value;

                btGlobalConfig.Tag=value;
                btGlobalConfig.Enabled=!string.IsNullOrEmpty(value);
            }
        }
       

        private void btGlobalConfig_Click(object sender, System.EventArgs e)
        {
            RenameVSWindowTitle.CurrentPackage.Settings.GlobalSolutionSettingsOverridesFp=globalSettingsPath;


            if (globalSettingsPath!=null)
                OpenText(globalSettingsPath, true);
        }

        private void btSolutionConfig_Click(object sender, System.EventArgs e)
        {
            if (btSolutionConfig.Tag!=null)
                OpenText(btSolutionConfig.Tag.ToString(), false);
        }

        private void OpenText(string path, bool bGlobal)
        {
            SettingsSet cfg = RenameVSWindowTitle.CurrentPackage?.GetSettings(solutionPath);
           string sampleSln = string.IsNullOrEmpty(solutionPath) ? Path.Combine(Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments), @"SampleDir\SampleDir2\SampleDir3\SampleDir4\Sample.sln") : solutionPath;
            try
            {
                bool bIsNewFile = !File.Exists(path);
                XmlElement S;
                if (bIsNewFile)
                {
                    XmlDocument doc = new XmlDocument();
                    doc.AppendChild(doc.CreateXmlDeclaration("1.0", "utf-8", string.Empty));
                    XmlElement RN = doc.CreateElement("RenameVSWindowTitle");
                    doc.AppendChild(RN);
                    if (!string.IsNullOrEmpty(solutionPath))
                    {

                        string sn = string.IsNullOrEmpty(cfg.SolutionFilePath) ? string.Empty : Path.GetFileNameWithoutExtension(cfg.SolutionFilePath);

                        S = doc.CreateElement("SettingsSet");
                        if (bGlobal) addVal(doc, S, Globals.TagPath, sampleSln, false);
                        addVal(doc, S, Globals.TagSolutionName, cfg.SolutionName, sn.Equals(cfg.SolutionName));
                        addVal(doc, S, Globals.TagClosestParentDepth, cfg.ClosestParentDepth?.ToString(), cfg.ClosestParentDepth==RenameVSWindowTitle.DefaultClosestParentDepth);
                        addVal(doc, S, Globals.TagFarthestParentDepth, cfg.FarthestParentDepth?.ToString(), cfg.FarthestParentDepth==RenameVSWindowTitle.DefaultFarthestParentDepth);
                        addVal(doc, S, Globals.TagAppendedString, cfg.AppendedString, cfg.AppendedString==RenameVSWindowTitle.DefaultAppendedString);
                        addVal(doc, S, Globals.TagPatternIfRunningMode, cfg.PatternIfRunningMode, cfg.PatternIfRunningMode==RenameVSWindowTitle.DefaultRunningModePattern);
                        addVal(doc, S, Globals.TagPatternIfBreakMode, cfg.PatternIfBreakMode, cfg.PatternIfBreakMode==RenameVSWindowTitle.DefaultBreakModePattern);
                        addVal(doc, S, Globals.TagPatternIfDesignMode, cfg.PatternIfDesignMode, cfg.PatternIfDesignMode==RenameVSWindowTitle.DefaultDesignModePattern);
                        RN.AppendChild(S);
                    }
                    RN.AppendChild(doc.CreateComment("See following sample SettingsSet's. Note both forms can be mixed."));

                    S = doc.CreateElement("SettingsSet-Example");
                    S.AppendChild(doc.CreateComment("Element-Style SettingsSet example (all overrides are specified as child elements)."));
                    if(bGlobal)
                        S.AppendChild(doc.CreateComment("Multiple Path child elements are allowed."));

                    if (bGlobal) addVal(doc, S, Globals.TagPath, sampleSln, false);
                    addVal(doc, S, Globals.TagSolutionName, cfg.SolutionName, false);
                    addVal(doc, S, Globals.TagClosestParentDepth, RenameVSWindowTitle.DefaultClosestParentDepth.ToString(), false);
                    addVal(doc, S, Globals.TagFarthestParentDepth, RenameVSWindowTitle.DefaultFarthestParentDepth.ToString(), false);
                    addVal(doc, S, Globals.TagAppendedString, RenameVSWindowTitle.DefaultAppendedString, false);
                    addVal(doc, S, Globals.TagPatternIfRunningMode, RenameVSWindowTitle.DefaultRunningModePattern, false);
                    addVal(doc, S, Globals.TagPatternIfBreakMode, RenameVSWindowTitle.DefaultBreakModePattern, false);
                    addVal(doc, S, Globals.TagPatternIfDesignMode, RenameVSWindowTitle.DefaultDesignModePattern, false);
                    RN.AppendChild(S);

                    S = doc.CreateElement("SettingsSet-Example");
                    S.AppendChild(doc.CreateComment("Attribute-Style SettingsSet example (all overrides are specified as attributes)"));
                    if (bGlobal) addAttr(doc, S, Globals.TagPath, sampleSln, false);
                    addAttr(doc, S, Globals.TagSolutionName, cfg.SolutionName, false);
                    addAttr(doc, S, Globals.TagClosestParentDepth, RenameVSWindowTitle.DefaultClosestParentDepth.ToString(), false);
                    addAttr(doc, S, Globals.TagFarthestParentDepth, RenameVSWindowTitle.DefaultFarthestParentDepth.ToString(), false);
                    addAttr(doc, S, Globals.TagAppendedString, RenameVSWindowTitle.DefaultAppendedString, false);
                    addAttr(doc, S, Globals.TagPatternIfRunningMode, RenameVSWindowTitle.DefaultRunningModePattern, false);
                    addAttr(doc, S, Globals.TagPatternIfBreakMode, RenameVSWindowTitle.DefaultBreakModePattern, false);
                    addAttr(doc, S, Globals.TagPatternIfDesignMode, RenameVSWindowTitle.DefaultDesignModePattern, false);
                    RN.AppendChild(S);

                    string tmp = Path.GetTempFileName();
                    XmlWriterSettings xws = new XmlWriterSettings();
                    xws.NewLineOnAttributes=true;
                    xws.CloseOutput=true;
                    xws.ConformanceLevel= ConformanceLevel.Document;
                    xws.Encoding=System.Text.Encoding.UTF8;
                    xws.Indent=true;
                    xws.IndentChars="  ";
                    xws.NewLineChars=Environment.NewLine;
                    xws.NewLineHandling= NewLineHandling.Entitize;
                    var xw = XmlWriter.Create(tmp, xws);
                    doc.Save(xw);
                    xw.Close();
                    File.Move(tmp, path);
                }
                var dd = Globals.DTE.Application.Documents.Open(path);
                if (bIsNewFile)
                {
                    dd.Saved=false;
                    File.Delete(path);
                }
            }
            catch (Exception x)
            {
            }
        }

        private static void addVal(XmlDocument doc, XmlElement S, string n, string val, bool asCommemt = false)
        {
            if (val==null)
                return;
            XmlElement E = doc.CreateElement(n);
            E.InnerText=val ?? string.Empty;

            if (asCommemt)
            {
                return;
                S.AppendChild(doc.CreateComment(E.OuterXml));
            }
            else
            {
                S.AppendChild(E);
            }

        }
        private static void addAttr(XmlDocument doc, XmlElement S, string n, string val, bool asCommemt = false)
        {
            if (val==null)
                return;

            S.SetAttribute(asCommemt ? "__"+n: n, val ?? string.Empty);

        }

    }
}