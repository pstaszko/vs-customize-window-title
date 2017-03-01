using System;
using System.ComponentModel;
using System.Windows.Forms;
using Microsoft.VisualStudio.Shell;
using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics;
using System.IO;
using ErwinMayerLabs.RenameVSWindowTitle.Properties;

namespace ErwinMayerLabs.RenameVSWindowTitle {
    public class SupportedTagsGrid : DialogPage {
        SupportedTagsControl SupportedTagsControl;

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected override IWin32Window Window {
            get {
                this.SupportedTagsControl = new SupportedTagsControl {
                    Dock = DockStyle.Fill,
                    Location = new System.Drawing.Point(0, 0)
                };
                var sb = new StringBuilder();
                var wb = this.SupportedTagsControl.WebBrowser;
                sb.Append("<html><head><meta http-equiv=\'Content-Type\' content=\'text/html;charset=UTF-8\' /><style type=\'text/css\'>" + Resources.normalize + "</style><style type=\'text/css\'>" + Resources.style + "</style><script type=\'text/javascript\'>" + Resources.script + "</script></head>");
                sb.Append("<body oncontextmenu=\'return false;\'><h2>Supported tags</h2>");
                sb.Append("<p>Full documentation is available on <a href='#' onclick='window.external.OpenLinkInExternalBrowser(\"https://visualstudiogallery.msdn.microsoft.com/f3f23845-5b1e-4811-882f-60b7181fa6d6\");'>the Visual Studio Gallery page</a>.");
                sb.Append("<p style='font-style: italic'>Tip: clicking on a tag will copy it to the clipboard.</p>");
                sb.Append("<table><thead><tr><th>Tag</th><th>Description</th></tr></thead><tbody>");
                foreach (var tag in RenameVSWindowTitle.SupportedTags) {
                    var localizedDescription = Resources.ResourceManager.GetString("tag_" + tag.Replace(":", ""));

                    sb.AppendFormat("<tr><td onclick=\'selectText(this);copyText(\"{0}\");\'><strong>[{0}]</strong></td>", tag);
                    sb.AppendFormat("<td>{0}</td></tr>", localizedDescription);
                }
                sb.Append("</tbody></table>");

                /*
                sb.Append("<table><thead><tr><th>Misc operations</th></tr></thead><tbody>");
                foreach (var kv in new Dictionary<string,string> {
                    { "GenerateGlobalConfig", "create or update the global-override.xml for the current solution" }
                })
                {
                    sb.AppendFormat("<tr><td onclick=\'javascript:window.external.{0}()\'>{0}</td>", kv.Key);
                    sb.AppendFormat("<td>{0}</td></tr>", kv.Value);
                }
                sb.Append("</tbody></table>");    
                */
                sb.Append("</body></html>");
                wb.DocumentText = sb.ToString();
                wb.ObjectForScripting = new ScriptHelper();
                return this.SupportedTagsControl;
            }
        }
    }

    [ComVisible(true)]
    public class ScriptHelper {
        public void OpenLinkInExternalBrowser(string link) {
            try {
                ProcessStartInfo sInfo = new ProcessStartInfo(link);
                using (Process.Start(sInfo)) {
                    // Do nothing
                }
            }
            catch {
                // Do nothing
            }
        }

        public void CopyTag(string tag) {
            try {
                Clipboard.SetText("[" + tag + "]");
            }
            catch {
                // Do nothing
            }
        }
    }
}