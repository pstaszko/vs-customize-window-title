using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.IO;

namespace ErwinMayerLabs.RenameVSWindowTitle
{
    public partial class EditablePatternControl : UserControl
    {
        private IWindowsFormsEditorService editorService;
        private PreviewRequiresAttribute.Requirement previewRequires;

        public EditablePatternControl(IWindowsFormsEditorService editorService)
        {
            this.editorService=editorService;
            InitializeComponent();
            txEdit.Focus();
        }

        public string Pattern
        {
            get { return txEdit.Text; }
            set
            {
                txEdit.Text=value;
                updatePreview();

            }
        }
        public string DefaultPattern { get; set; }

        public PreviewRequiresAttribute.Requirement PreviewRequires
        {
            get { return previewRequires; }
            set
            {
                previewRequires=value;
                updatePreview();
            }
        }

        private void lbMore_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button!= MouseButtons.Left)
                return;
            ctxMenu.Show(Cursor.Position);

        }

        private void setToDefault_Click(object sender, EventArgs e)
        {
            txEdit.Text=DefaultPattern;
        }

        private void insertTag_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem m = (ToolStripMenuItem)sender;
            txEdit.Paste(string.Format("[{0}]", m.Tag));
        }

        private void ctxMenu_Opening(object sender, CancelEventArgs e)
        {
            bool bHaveDefault = (DefaultPattern!=null);
            setToDefault.Visible=bHaveDefault;
            resetSep.Visible=bHaveDefault;

            insertTag.Visible=false;
            // remove old entries
            for (int i = ctxMenu.Items.Count-1; i>=0; i--)
                if (ctxMenu.Items[i].Tag is string)
                    ctxMenu.Items.RemoveAt(i);
            int z = ctxMenu.Items.IndexOf(insertTag);

            foreach (var tag in RenameVSWindowTitle.SupportedTags)
            {
                string LocalizedDescription = Resources.ResourceManager.GetString("tag_"+tag);
                var m = new ToolStripMenuItem(string.Format(insertTag.Text, tag));
                m.ToolTipText=LocalizedDescription;
                m.Tag=tag;
                m.Click+=insertTag_Click;
                ctxMenu.Items.Insert(z++, m);
            }
        }

        private void txEdit_TextChanged(object sender, EventArgs e)
        {
            timerUpdate.Stop();
            timerUpdate.Start();
        }

        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            timerUpdate.Stop();
            updatePreview();

        }

        private void updatePreview()
        {
            BackColor=SystemColors.Control;
            var solution = Globals.DTE.Solution;
            var solutionFp = solution?.FullName;

            var activeDocument = Globals.DTE.ActiveDocument;
            var activeWindow = Globals.DTE.ActiveWindow;

            switch (PreviewRequires)
            {
            case PreviewRequiresAttribute.Requirement.Document:
                if (!(solution==null || string.IsNullOrEmpty(solutionFp)))
                {
                    lbPreview.Text="please close solution to enable preview!";
                    return;
                }
                string ad = Globals.GetActiveDocumentNameOrEmpty(activeDocument, activeWindow);
                if(string.IsNullOrEmpty(ad))
                {
                    lbPreview.Text="please load any document or open any window to enable preview!";
                    return;
                }
                break;

            case PreviewRequiresAttribute.Requirement.Solution:
                if (solution==null || string.IsNullOrEmpty(solutionFp))
                {
                    lbPreview.Text="please load any solution to enable preview!";
                    return;
                }
                break;
            }
            SettingsSet  config = RenameVSWindowTitle.CurrentPackage.GetSettings(solutionFp);
            var pattern = txEdit.Text;
            lbPreview.Text="preview: "+ RenameVSWindowTitle.CurrentPackage.GetNewTitle(solution, pattern, config);
        }

        private void EditablePatternControl_DoubleClick(object sender, EventArgs e)
        {
            ;
        }
    }
}