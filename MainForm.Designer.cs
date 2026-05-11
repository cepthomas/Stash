namespace WinClip
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            toolStrip1 = new System.Windows.Forms.ToolStrip();
            BtnSettings = new System.Windows.Forms.ToolStripButton();
            BtnDebug = new System.Windows.Forms.ToolStripButton();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // toolStrip1
            // 
            toolStrip1.ImageScalingSize = new System.Drawing.Size(18, 18);
            toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { BtnSettings, toolStripSeparator2, BtnDebug, toolStripSeparator1 });
            toolStrip1.Location = new System.Drawing.Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new System.Drawing.Size(559, 26);
            toolStrip1.TabIndex = 16;
            toolStrip1.Text = "toolStrip1";
            // 
            // BtnSettings
            // 
            BtnSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            BtnSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            BtnSettings.Name = "BtnSettings";
            BtnSettings.Size = new System.Drawing.Size(62, 23);
            BtnSettings.Text = "Settings";
            BtnSettings.Click += Settings_Click;
            // 
            // BtnDebug
            // 
            BtnDebug.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            BtnDebug.ImageTransparentColor = System.Drawing.Color.Magenta;
            BtnDebug.Name = "BtnDebug";
            BtnDebug.Size = new System.Drawing.Size(54, 23);
            BtnDebug.Text = "Debug";
            BtnDebug.Click += Debug_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(6, 26);
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new System.Drawing.Size(6, 26);
            // 
            // MainForm
            // 
            AutoScroll = true;
            ClientSize = new System.Drawing.Size(559, 598);
            Controls.Add(toolStrip1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            Name = "MainForm";
            StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            Text = "Hoo Haa";
            WindowState = System.Windows.Forms.FormWindowState.Minimized;
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }
        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton BtnSettings;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton BtnDebug;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    }
}