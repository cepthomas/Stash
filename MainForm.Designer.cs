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
            toolStrip1 = new System.Windows.Forms.ToolStrip();
            BtnSettings = new System.Windows.Forms.ToolStripButton();
            toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            BtnDebug = new System.Windows.Forms.ToolStripButton();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            selector = new Ephemera.IconicSelector.Selector();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // toolStrip1
            // 
            toolStrip1.ImageScalingSize = new System.Drawing.Size(18, 18);
            toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { BtnSettings, toolStripSeparator2, BtnDebug, toolStripSeparator1 });
            toolStrip1.Location = new System.Drawing.Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new System.Drawing.Size(321, 26);
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
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new System.Drawing.Size(6, 26);
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
            // selector
            // 
            selector.AllowDrop = true;
            selector.AllowExternalDrop = false;
            selector.AutoScroll = true;
            selector.Dock = System.Windows.Forms.DockStyle.Fill;
            selector.DrawFont = new System.Drawing.Font("Calibri", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            selector.ImageSize = 32;
            selector.LeftMouseClick = Ephemera.IconicSelector.MouseFunction.Click;
            selector.Location = new System.Drawing.Point(0, 26);
            selector.Name = "selector";
            selector.Pad = 4;
            selector.Size = new System.Drawing.Size(321, 572);
            selector.Spacing = 10;
            selector.TabIndex = 17;
            selector.TargetColor = System.Drawing.Color.Aqua;
            // 
            // MainForm
            // 
            AutoScroll = true;
            ClientSize = new System.Drawing.Size(321, 598);
            Controls.Add(selector);
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
        private Ephemera.IconicSelector.Selector selector;
    }
}