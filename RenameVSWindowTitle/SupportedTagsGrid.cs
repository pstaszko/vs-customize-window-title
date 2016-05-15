using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;

namespace ErwinMayerLabs.RenameVSWindowTitle {
    public class SupportedTagsGrid : DialogPage { 
        private readonly List<string> SupportedTags  = new List<string> {
            "documentName",
            "projectName",
            "alternateSolutionName",
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

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected override IWin32Window Window {
            get {
                var supportedTagsControl = new SupportedTagsControl {
                    Dock = DockStyle.Fill,
                    Location = new System.Drawing.Point(0, 0)
                };
                var wb = supportedTagsControl.WebBrowser;
                var html = "<html><head><meta http-equiv=\'Content-Type\' content=\'text/html;charset=UTF-8\' /><style type=\'text/css\'>" + Resources.style + "</style><script type=\'text/javascript\'>" + Resources.script + "</script></head>";
                html += "<body oncontextmenu=\'return false;\'><h1>Supported tags (clicking on a tag will copy it to the clipboard)</h1><table><thead><tr><th>Tag</th><th>Description</th></tr></thead><tbody>";
                foreach (var tag in this.SupportedTags)
                {
                    string LocalizedDescription = Resources.ResourceManager.GetString("tag_"+tag);

                    html += "<tr><td onclick=\'selectText(this)\'><strong>[" + tag+ "]</strong></td>";
                    html += "<td>" + LocalizedDescription + "</td></tr>";
                }
                html += "</tbody></table></body></html>";
                wb.DocumentText = html;
                return supportedTagsControl;
            }
        }
    }
}