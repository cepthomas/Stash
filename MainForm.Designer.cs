using System.Drawing;
using System.Windows.Forms;
using Ephemera.NBagOfUis;
using Ephemera.IconicSelector;


namespace WinClip
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code
        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            selector = new Selector();
            toolStrip1 = new ToolStrip();
            btnClear = new ToolStripButton();
            btnSettings = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            toolStrip1.SuspendLayout();
            SuspendLayout();
            //
            // selector
            //
            selector.AutoScroll = true;
            selector.BorderStyle = BorderStyle.FixedSingle;
            selector.Dock = DockStyle.Fill;
            selector.Location = new Point(0, 26);
            selector.Name = "selector";
            selector.Size = new Size(350, 353);
            selector.TabIndex = 7;
            //
            // toolStrip1
            //
            toolStrip1.ImageScalingSize = new Size(18, 18);
            toolStrip1.Items.AddRange(new ToolStripItem[] { btnClear, toolStripSeparator1, btnSettings });
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(350, 26);
            toolStrip1.TabIndex = 8;
            toolStrip1.Text = "toolStrip1";
            //
            // btnClear
            //
            btnClear.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnClear.ImageTransparentColor = Color.Magenta;
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(37, 23);
            btnClear.Text = "Clear All";
            //
            // toolStripSeparator1
            //
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 26);
            //
            // btnSettings
            //
            btnSettings.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnSettings.ImageTransparentColor = Color.Magenta;
            btnSettings.Name = "btnSettings";
            btnSettings.Size = new Size(37, 23);
            btnSettings.Text = "Settings";
            //
            // MainForm
            //
            AutoScaleDimensions = new SizeF(8F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(350, 379);
            Controls.Add(selector);
            Controls.Add(toolStrip1);
            Name = "MainForm";
            Text = "booga";
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }
        #endregion

        private Selector selector;
        private ToolStrip toolStrip1;
        private ToolStripButton btnClear;
        private ToolStripButton btnSettings;
        private ToolStripSeparator toolStripSeparator1;
    }
}
