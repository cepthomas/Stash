using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ephemera.NBagOfTricks;
using Ephemera.NBagOfUis;


namespace WinClip
{
    public partial class Dev : Form
    {
        readonly Label lblPrevious;
        readonly Label lblCurrent;
        readonly TextBox txtPreviousWin;
        readonly TextBox txtCurrentWin;
        readonly Button btnDebug;
        readonly TextViewer tvInfo;

        public Dev()
        {
            ///// Init myself.
            AutoScaleDimensions = new SizeF(8F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(752, 471);
            Text = "Dev";
            Location = new(500, 100);
            //Location = Common.Settings.FormGeometry.Location;
            //Visible = false; // doesn't work - like C:\Dev\Misc\NLab\TrayExForm - use Minimized?

            ///// Init controls.
            lblPrevious = new()
            {
                Location = new Point(537, 14),
                Size = new Size(61, 19),
                Text = "Previous"
            };
            Controls.Add(lblPrevious);

            lblCurrent = new()
            {
                Location = new Point(328, 16),
                Size = new Size(56, 19),
                Text = "Current"
            };
            Controls.Add(lblCurrent);

            txtPreviousWin = new()
            {
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(600, 12),
                ReadOnly = true,
                Size = new Size(145, 26)
            };
            Controls.Add(txtPreviousWin);

            txtCurrentWin = new()
            {
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(385, 12),
                ReadOnly = true,
                Size = new Size(145, 26)
            };
            Controls.Add(txtCurrentWin);

            btnDebug = new()
            {
                Location = new Point(108, 11),
                Size = new Size(94, 29),
                Text = "Do It!",
                UseVisualStyleBackColor = true
            };
            Controls.Add(btnDebug);

            tvInfo = new()
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(8, 46),
                MatchUseBackground = true,
                MaxText = 50000,
                Size = new Size(737, 420),
                WordWrap = true,
                BackColor = Color.Cornsilk,
                Matchers =
                [
                    new("ERR ", Color.Red),
                    new("WRN ", Color.Green),
                ]
            };
            Controls.Add(tvInfo);
        }

        /// <summary>Clean up any resources being used.</summary>
        /// <param name="disposing">True if managed resources should be disposed.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>Tell me something.</summary>
        /// <param name="s">What.</param>
        public void Tell(string s)
        {
            tvInfo.Append(s);
        }
    }
}
