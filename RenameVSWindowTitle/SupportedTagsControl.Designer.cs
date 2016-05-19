namespace ErwinMayerLabs.RenameVSWindowTitle
{
    public partial class SupportedTagsControl : System.Windows.Forms.UserControl
	{
		
		//UserControl overrides dispose to clean up the component list.
		[System.Diagnostics.DebuggerNonUserCode()]protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing && components != null)
				{
					components.Dispose();
				}
			}
			finally
			{

                base.Dispose(disposing);
			}
		}
		
		//Required by the Windows Form Designer
		private System.ComponentModel.Container components = null;
		
		//NOTE: The following procedure is required by the Windows Form Designer
		//It can be modified using the Windows Form Designer.
		//Do not modify it using the code editor.
		[System.Diagnostics.DebuggerStepThrough()]private void InitializeComponent()
		{
            this.WebBrowser = new System.Windows.Forms.WebBrowser();
            this.btGlobalConfig = new System.Windows.Forms.Button();
            this.btSolutionConfig = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // WebBrowser
            // 
            this.WebBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.WebBrowser.IsWebBrowserContextMenuEnabled = false;
            this.WebBrowser.Location = new System.Drawing.Point(3, 32);
            this.WebBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.WebBrowser.Name = "WebBrowser";
            this.WebBrowser.Size = new System.Drawing.Size(406, 319);
            this.WebBrowser.TabIndex = 0;
            this.WebBrowser.WebBrowserShortcutsEnabled = false;
            // 
            // btGlobalConfig
            // 
            this.btGlobalConfig.Location = new System.Drawing.Point(3, 3);
            this.btGlobalConfig.Name = "btGlobalConfig";
            this.btGlobalConfig.Size = new System.Drawing.Size(130, 23);
            this.btGlobalConfig.TabIndex = 1;
            this.btGlobalConfig.Text = "open global config";
            this.btGlobalConfig.UseVisualStyleBackColor = true;
            this.btGlobalConfig.Click += new System.EventHandler(this.btGlobalConfig_Click);
            // 
            // btSolutionConfig
            // 
            this.btSolutionConfig.Location = new System.Drawing.Point(139, 3);
            this.btSolutionConfig.Name = "btSolutionConfig";
            this.btSolutionConfig.Size = new System.Drawing.Size(130, 23);
            this.btSolutionConfig.TabIndex = 1;
            this.btSolutionConfig.Text = "open solution config";
            this.btSolutionConfig.UseVisualStyleBackColor = true;
            this.btSolutionConfig.Click += new System.EventHandler(this.btSolutionConfig_Click);
            // 
            // SupportedTagsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btSolutionConfig);
            this.Controls.Add(this.btGlobalConfig);
            this.Controls.Add(this.WebBrowser);
            this.Name = "SupportedTagsControl";
            this.Size = new System.Drawing.Size(412, 354);
            this.ResumeLayout(false);

		}
		internal System.Windows.Forms.WebBrowser WebBrowser;
        private System.Windows.Forms.Button btGlobalConfig;
        private System.Windows.Forms.Button btSolutionConfig;
    }
	
}
