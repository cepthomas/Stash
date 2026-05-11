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
        System.ComponentModel.IContainer components = null;

        Label lblPrevious;
        Label lblCurrent;
        TextBox txtPreviousWin;
        TextBox txtCurrentWin;
        Button btnClear;
        Button btnDebug;

        TextViewer tvInfo;


        public Dev()
        {
            ///// Init myself.
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 19F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(752, 471);
            Text = "Dev";
            Location = new(500, 100);
            //Location = Common.Settings.FormGeometry.Location;
            //Visible = false; // doesn't work - like C:\Dev\Misc\NLab\TrayExForm - use Minimized?







            ///// Init controls.
            lblPrevious = new System.Windows.Forms.Label();
            lblPrevious.Location = new System.Drawing.Point(537, 14);
            lblPrevious.Size = new System.Drawing.Size(61, 19);
            lblPrevious.Text = "Previous";
            Controls.Add(lblPrevious);

            lblCurrent = new System.Windows.Forms.Label();
            lblCurrent.Location = new System.Drawing.Point(328, 16);
            lblCurrent.Size = new System.Drawing.Size(56, 19);
            lblCurrent.Text = "Current";
            Controls.Add(lblCurrent);

            txtPreviousWin = new System.Windows.Forms.TextBox();
            txtPreviousWin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            txtPreviousWin.Location = new System.Drawing.Point(600, 12);
            txtPreviousWin.ReadOnly = true;
            txtPreviousWin.Size = new System.Drawing.Size(145, 26);
            Controls.Add(txtPreviousWin);

            txtCurrentWin = new System.Windows.Forms.TextBox();
            txtCurrentWin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            txtCurrentWin.Location = new System.Drawing.Point(385, 12);
            txtCurrentWin.ReadOnly = true;
            txtCurrentWin.Size = new System.Drawing.Size(145, 26);
            Controls.Add(txtCurrentWin);


            btnDebug = new System.Windows.Forms.Button();
            btnDebug.Location = new System.Drawing.Point(108, 11);
            btnDebug.Size = new System.Drawing.Size(94, 29);
            btnDebug.Text = "Do It!";
            btnDebug.UseVisualStyleBackColor = true;
            Controls.Add(btnDebug);

            tvInfo = new Ephemera.NBagOfUis.TextViewer();
            tvInfo.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tvInfo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            tvInfo.Location = new System.Drawing.Point(8, 46);
            tvInfo.MatchUseBackground = true;
            tvInfo.MaxText = 50000;
            tvInfo.Size = new System.Drawing.Size(737, 420);
            tvInfo.WordWrap = true;
            tvInfo.BackColor = Color.Cornsilk;
            tvInfo.Matchers =
            [
                new("ERR ", Color.Red),
                new("WRN ", Color.Green),
            ];
            Controls.Add(tvInfo);
        }

        /// <summary>Clean up any resources being used.</summary>
        /// <param name="disposing">True if managed resources should be disposed.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>Tell me something.</summary>
        /// <param name="disposing">This.</param>
        public void Tell(string s)
        {
            tvInfo.Append(s);
        }
    }
}
