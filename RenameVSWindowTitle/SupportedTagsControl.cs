using System;
using System.IO;
using System.Xml;

namespace ErwinMayerLabs.RenameVSWindowTitle {
    public partial class SupportedTagsControl {
        public SupportedTagsControl() {
            this.InitializeComponent();
        }

        public string SolutionPath {
            get
            {
                return btSolutionConfig.Tag.ToString();
            }
            set
            {
                btSolutionConfig.Enabled=!string.IsNullOrEmpty(value);
                btSolutionConfig.Tag=btSolutionConfig.Enabled ? value + Globals.SolutionConfigExt : null;
            }
        }

        public string GlobalSettingsPath {
            get
            {
                return btGlobalConfig.Tag.ToString();
            }
            set
            {
                btGlobalConfig.Tag=value;
                btGlobalConfig.Enabled=!string.IsNullOrEmpty(value);
            }
        }
       

        private void btGlobalConfig_Click(object sender, System.EventArgs e)
        {

            if (btGlobalConfig.Tag!=null)
                OpenText(btGlobalConfig.Tag.ToString());
        }

        private void btSolutionConfig_Click(object sender, System.EventArgs e)
        {
            if (btSolutionConfig.Tag!=null)
                OpenText(btSolutionConfig.Tag.ToString());
        }

        private void OpenText(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.AppendChild(doc.CreateXmlDeclaration("1.0", "utf-8", string.Empty));
                    XmlElement RN = doc.CreateElement("RenameVSWindowTitle");
                    doc.AppendChild(RN);

                    XmlElement S = doc.CreateElement("SettingsSet");
                    RN.AppendChild(S);
                    string val = "xxx";
                    addVal(doc, S, Globals.TagPath, btSolutionConfig.Tag.ToString(), false);
                    addVal(doc, S, Globals.TagSolutionName, val, true);
                    addVal(doc, S, Globals.TagClosestParentDepth, val, true);
                    addVal(doc, S, Globals.TagFarthestParentDepth, val, true);
                    addVal(doc, S, Globals.TagAppendedString, val, true);
                    addVal(doc, S, Globals.TagPatternIfRunningMode, val, true);
                    addVal(doc, S, Globals.TagPatternIfBreakMode, val, true);
                    addVal(doc, S, Globals.TagPatternIfDesignMode, val, true);

                    S = doc.CreateElement("SettingsSet");
                    addAttr(doc, S, Globals.TagPath, btSolutionConfig.Tag.ToString(), false);
                    addAttr(doc, S, Globals.TagSolutionName, val, true);
                    addAttr(doc, S, Globals.TagClosestParentDepth, val, true);
                    addAttr(doc, S, Globals.TagFarthestParentDepth, val, true);
                    addAttr(doc, S, Globals.TagAppendedString, val, true);
                    addAttr(doc, S, Globals.TagPatternIfRunningMode, val, true);
                    addAttr(doc, S, Globals.TagPatternIfBreakMode, val, true);
                    addAttr(doc, S, Globals.TagPatternIfDesignMode, val, true);

                    RN.AppendChild(doc.CreateComment("or use short attribute form (can be mixed)"+S.OuterXml));

                    doc.Save(path);
                }
                var dd = Globals.DTE.Application.Documents.Open(path);
                dd.Saved=false;
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
            E.InnerText=val;

            if (asCommemt)
            {
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

            S.SetAttribute(asCommemt ? "__"+n: n, val);

        }

    }
}