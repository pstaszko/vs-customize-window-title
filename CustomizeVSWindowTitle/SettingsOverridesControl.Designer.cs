namespace ErwinMayerLabs.RenameVSWindowTitle
{
    public partial class SettingsOverridesControl : System.Windows.Forms.UserControl
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.btSolutionConfig = new System.Windows.Forms.Button();
            this.btGlobalConfig = new System.Windows.Forms.Button();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.btRegister = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSize = true;
            this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox1.Controls.Add(this.tableLayoutPanel2);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(412, 239);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Settings Overrides";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.btRegister, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.btSolutionConfig, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.btGlobalConfig, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.propertyGrid1, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(406, 220);
            this.tableLayoutPanel2.TabIndex = 4;
            // 
            // btSolutionConfig
            // 
            this.btSolutionConfig.Dock = System.Windows.Forms.DockStyle.Top;
            this.btSolutionConfig.Location = new System.Drawing.Point(206, 161);
            this.btSolutionConfig.Name = "btSolutionConfig";
            this.btSolutionConfig.Size = new System.Drawing.Size(197, 23);
            this.btSolutionConfig.TabIndex = 9;
            this.btSolutionConfig.Text = "open solution config";
            this.btSolutionConfig.UseVisualStyleBackColor = true;
            this.btSolutionConfig.Click += new System.EventHandler(this.btSolutionConfig_Click);
            // 
            // btGlobalConfig
            // 
            this.btGlobalConfig.Dock = System.Windows.Forms.DockStyle.Top;
            this.btGlobalConfig.Location = new System.Drawing.Point(3, 161);
            this.btGlobalConfig.Name = "btGlobalConfig";
            this.btGlobalConfig.Size = new System.Drawing.Size(197, 23);
            this.btGlobalConfig.TabIndex = 8;
            this.btGlobalConfig.Text = "open global config";
            this.btGlobalConfig.UseVisualStyleBackColor = true;
            this.btGlobalConfig.Click += new System.EventHandler(this.btGlobalConfig_Click);
            // 
            // propertyGrid1
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.propertyGrid1, 2);
            this.propertyGrid1.Location = new System.Drawing.Point(3, 3);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.propertyGrid1.Size = new System.Drawing.Size(400, 152);
            this.propertyGrid1.TabIndex = 7;
            this.propertyGrid1.ToolbarVisible = false;
            // 
            // btRegister
            // 
            this.btRegister.Dock = System.Windows.Forms.DockStyle.Top;
            this.btRegister.Location = new System.Drawing.Point(3, 190);
            this.btRegister.Name = "btRegister";
            this.btRegister.Size = new System.Drawing.Size(197, 23);
            this.btRegister.TabIndex = 10;
            this.btRegister.Text = "Register";
            this.btRegister.UseVisualStyleBackColor = true;
            this.btRegister.Click += new System.EventHandler(this.btRegister_Click);
            // 
            // SettingsOverridesControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "SettingsOverridesControl";
            this.Size = new System.Drawing.Size(412, 239);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.SettingsOverridesControl_Paint);
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button btSolutionConfig;
        private System.Windows.Forms.Button btGlobalConfig;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.Button btRegister;
    }
	
}
