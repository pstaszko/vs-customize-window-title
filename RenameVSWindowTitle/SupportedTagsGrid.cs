using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using System;

namespace ErwinMayerLabs.RenameVSWindowTitle {
    public class SupportedTagsGrid : DialogPage { 
        private readonly List<string> SupportedTags  = new List<string> {
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

        SupportedTagsControl supportedTagsControl;

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected override IWin32Window Window {
            get {
                supportedTagsControl = new SupportedTagsControl {
                    Dock = DockStyle.Fill,
                    Location = new System.Drawing.Point(0, 0)
                };
                StringBuilder sb = new StringBuilder();
                var wb = supportedTagsControl.WebBrowser;
                sb.Append("<html><head><meta http-equiv=\'Content-Type\' content=\'text/html;charset=UTF-8\' /><style type=\'text/css\'>" + Resources.style + "</style></head>");
                sb.Append("<body oncontextmenu=\'return false;\'><h1>Supported tags (clicking on a tag will copy it to the clipboard)</h1>");
                sb.Append("<table><thead><tr><th>Tag</th><th>Description</th></tr></thead><tbody>");
                foreach (var tag in this.SupportedTags)
                {
                    string LocalizedDescription = Resources.ResourceManager.GetString("tag_"+tag);

                    sb.AppendFormat("<tr><td onclick=\'javascript:window.external.CopyTag(\"{0}\")\'><strong>[{0}]</strong></td>", tag);
                    sb.AppendFormat("<td>{0}</td></tr>", LocalizedDescription);
                }
                sb.Append("</tbody></table>");

                /*
                sb.Append("<table><thead><tr><th>Misc operations</th></tr></thead><tbody>");
                foreach (var kv in new Dictionary<string,string> {
                    { "CopyRNX", "Copy example rnxcfg for current solution" },
                    { "CopyGlobalXml", "Copy global-override.xml for current solution (with attribute-style overrides)" },
                    { "CopyGlobalCompactXml", "Copy global-override.xml for current solution (with element-style overrides)" },
                    { "CreateRNXCFG", "create or update the .rnxcfg for the current solution" },
                    { "GenerateGlobalConfig", "create or update the global-override.xml for the current solution" }
                })
                {
                    sb.AppendFormat("<tr><td onclick=\'javascript:window.external.{0}()\'>{0}</td>", kv.Key);
                    sb.AppendFormat("<td>{0}</td></tr>", kv.Value);
                }
                sb.Append("</tbody></table>");                
                sb.Append("</body></html>");
                */
                wb.DocumentText = sb.ToString();
                wb.ObjectForScripting = new ScriptHelper();
                return supportedTagsControl;
            }
        }

        protected override void OnActivate(CancelEventArgs e)
        {
            base.OnActivate(e);
            var solution = Globals.DTE.Solution;
            supportedTagsControl.SolutionPath=solution?.FullName;

            if (RenameVSWindowTitle.CurrentPackage!=null)
            {
                //  if no global settingsfile defined, use default file name
                string gfp = RenameVSWindowTitle.CurrentPackage.Settings.GlobalSolutionSettingsOverridesFp;
                if(string.IsNullOrEmpty(gfp))
                    gfp=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "RenameVSWindowTitle-global.xml");

                supportedTagsControl.GlobalSettingsPath=gfp; // if opened, the opening will set the global settings path
            }
            else
            {
                supportedTagsControl.GlobalSettingsPath=null;
            }
        }

    }

    [ComVisible(true)]
    public class ScriptHelper
    {
        public void CopyTag(string tag)
        {
            try
            {
                Clipboard.SetText("["+tag+"]");
            }
            catch { }
        }

        public void CopyRNX()
        {
            string text=string.Format(@"<? xml version=""1.0"" encoding=""utf-8"" ?>
 <RenameVSWindowTitle>
 <Solution
    AlternateSolutionName=""Tschakka SLN""
    FarthestParentDepth=""3""
    ClosestParentDepth=""1""
    PatternIfDesignMode=""[alternateSolutionName] - [ideName] XXY 0:[parent0] 1:[parent1] 2:[parent2] 3:[parent3] 4:[parent4] 5:[parent5] 6:[parent6]  -- p:[parentPath]  -- git:[gitBranchName]  Tschakka""
    PatternIfBreakMode=""[alternateSolutionName] (Debugging) - [ideName] Tschakka""
    PatternIfRunningMode=""[alternateSolutionName] (Running) - [ideName] Tschakka""
    AppendedString=""$""
/>
</RenameVSWindowTitle>", "");

            // ToDo: create XmlDocument
            //  add elements/attributes for all overridable values
            // always add AlternateSolutionName, but empty 
            try
            {
                Clipboard.SetText(text);
            }
            catch { }
        }
        public void CopyGlobalXml()
        {
            // almost like CopyRNX, but add Path
        }
        public void CopyGlobalCompactXml()
        {
            // 
// <Solution>
//    <Path>Xyz.sln</Path>
//    <AlternateSolutionName>Tschakka SLN</AlternateSolutionName>
//    ...
//</Solution>
        }

        public void CreateRNXCFG()
        {

        }
        public void CreateGlobalConfig()
        { }
    }
}