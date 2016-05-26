namespace ErwinMayerLabs.RenameVSWindowTitle
{
    partial class EditablePatternControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.txEdit = new System.Windows.Forms.TextBox();
            this.lbMore = new System.Windows.Forms.Label();
            this.ctxMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.insertTag = new System.Windows.Forms.ToolStripMenuItem();
            this.resetSep = new System.Windows.Forms.ToolStripSeparator();
            this.setToDefault = new System.Windows.Forms.ToolStripMenuItem();
            this.timerUpdate = new System.Windows.Forms.Timer(this.components);
            this.lbPreview = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ctxMenu.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txEdit
            // 
            this.txEdit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txEdit.Location = new System.Drawing.Point(3, 16);
            this.txEdit.Margin = new System.Windows.Forms.Padding(0);
            this.txEdit.Name = "txEdit";
            this.txEdit.Size = new System.Drawing.Size(485, 20);
            this.txEdit.TabIndex = 0;
            this.txEdit.TextChanged += new System.EventHandler(this.txEdit_TextChanged);
            // 
            // lbMore
            // 
            this.lbMore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lbMore.BackColor = System.Drawing.SystemColors.Info;
            this.lbMore.ForeColor = System.Drawing.SystemColors.InfoText;
            this.lbMore.Location = new System.Drawing.Point(488, 16);
            this.lbMore.Name = "lbMore";
            this.lbMore.Size = new System.Drawing.Size(20, 20);
            this.lbMore.TabIndex = 1;
            this.lbMore.Text = "...";
            this.lbMore.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lbMore.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lbMore_MouseDown);
            // 
            // ctxMenu
            // 
            this.ctxMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.insertTag,
            this.resetSep,
            this.setToDefault});
            this.ctxMenu.Name = "ctxMenu";
            this.ctxMenu.Size = new System.Drawing.Size(154, 54);
            this.ctxMenu.Opening += new System.ComponentModel.CancelEventHandler(this.ctxMenu_Opening);
            // 
            // insertTag
            // 
            this.insertTag.Name = "insertTag";
            this.insertTag.Size = new System.Drawing.Size(153, 22);
            this.insertTag.Text = "insert [{0}]";
            this.insertTag.Click += new System.EventHandler(this.insertTag_Click);
            // 
            // resetSep
            // 
            this.resetSep.Name = "resetSep";
            this.resetSep.Size = new System.Drawing.Size(150, 6);
            // 
            // setToDefault
            // 
            this.setToDefault.Name = "setToDefault";
            this.setToDefault.Size = new System.Drawing.Size(153, 22);
            this.setToDefault.Text = "reset to default";
            this.setToDefault.Click += new System.EventHandler(this.setToDefault_Click);
            // 
            // timerUpdate
            // 
            this.timerUpdate.Interval = 500;
            this.timerUpdate.Tick += new System.EventHandler(this.timerUpdate_Tick);
            // 
            // lbPreview
            // 
            this.lbPreview.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbPreview.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.lbPreview.ContextMenuStrip = this.ctxMenu;
            this.lbPreview.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lbPreview.Location = new System.Drawing.Point(4, 44);
            this.lbPreview.Name = "lbPreview";
            this.lbPreview.Size = new System.Drawing.Size(505, 13);
            this.lbPreview.TabIndex = 2;
            this.lbPreview.Text = "preview, change text to update preview";
            this.lbPreview.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lbMore_MouseDown);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txEdit);
            this.groupBox1.Controls.Add(this.lbMore);
            this.groupBox1.Controls.Add(this.lbPreview);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(511, 67);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Edit Pattern (click on preview or \"...\" to insert tags)";
            // 
            // EditablePatternControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.Name = "EditablePatternControl";
            this.Size = new System.Drawing.Size(511, 67);
            this.DoubleClick += new System.EventHandler(this.EditablePatternControl_DoubleClick);
            this.ctxMenu.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txEdit;
        private System.Windows.Forms.Label lbMore;
        private System.Windows.Forms.ContextMenuStrip ctxMenu;
        private System.Windows.Forms.ToolStripMenuItem setToDefault;
        private System.Windows.Forms.ToolStripMenuItem insertTag;
        private System.Windows.Forms.ToolStripSeparator resetSep;
        private System.Windows.Forms.Timer timerUpdate;
        private System.Windows.Forms.Label lbPreview;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}
