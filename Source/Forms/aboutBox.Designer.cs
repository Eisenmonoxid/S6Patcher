namespace S6Patcher.Source.Forms
{
    partial class aboutBox
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblDC = new System.Windows.Forms.LinkLabel();
            this.btnCheckUpdate = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblAbout = new System.Windows.Forms.Label();
            this.lblGit = new System.Windows.Forms.LinkLabel();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.lblFeatures = new System.Windows.Forms.LinkLabel();
            this.mainPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblDC
            // 
            this.lblDC.AutoSize = true;
            this.lblDC.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDC.Location = new System.Drawing.Point(69, 39);
            this.lblDC.Name = "lblDC";
            this.lblDC.Size = new System.Drawing.Size(63, 20);
            this.lblDC.TabIndex = 0;
            this.lblDC.TabStop = true;
            this.lblDC.Text = "Discord";
            this.lblDC.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblDC_LinkClicked);
            // 
            // btnCheckUpdate
            // 
            this.btnCheckUpdate.Location = new System.Drawing.Point(224, 97);
            this.btnCheckUpdate.Name = "btnCheckUpdate";
            this.btnCheckUpdate.Size = new System.Drawing.Size(109, 44);
            this.btnCheckUpdate.TabIndex = 1;
            this.btnCheckUpdate.Text = "Check for Updates";
            this.btnCheckUpdate.UseVisualStyleBackColor = true;
            this.btnCheckUpdate.Click += new System.EventHandler(this.btnCheckUpdate_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(12, 97);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(109, 44);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblAbout
            // 
            this.lblAbout.AutoSize = true;
            this.lblAbout.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAbout.Location = new System.Drawing.Point(3, 9);
            this.lblAbout.Name = "lblAbout";
            this.lblAbout.Size = new System.Drawing.Size(178, 16);
            this.lblAbout.TabIndex = 3;
            this.lblAbout.Text = "S6Patcher by Eisenmonoxid:";
            // 
            // lblGit
            // 
            this.lblGit.AutoSize = true;
            this.lblGit.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGit.Location = new System.Drawing.Point(3, 39);
            this.lblGit.Name = "lblGit";
            this.lblGit.Size = new System.Drawing.Size(60, 20);
            this.lblGit.TabIndex = 4;
            this.lblGit.TabStop = true;
            this.lblGit.Text = "GitHub";
            this.lblGit.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblGit_LinkClicked);
            // 
            // mainPanel
            // 
            this.mainPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.mainPanel.Controls.Add(this.lblFeatures);
            this.mainPanel.Controls.Add(this.lblAbout);
            this.mainPanel.Controls.Add(this.lblGit);
            this.mainPanel.Controls.Add(this.lblDC);
            this.mainPanel.Location = new System.Drawing.Point(12, 12);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size(321, 79);
            this.mainPanel.TabIndex = 5;
            // 
            // lblFeatures
            // 
            this.lblFeatures.AutoSize = true;
            this.lblFeatures.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFeatures.Location = new System.Drawing.Point(138, 39);
            this.lblFeatures.Name = "lblFeatures";
            this.lblFeatures.Size = new System.Drawing.Size(73, 20);
            this.lblFeatures.TabIndex = 5;
            this.lblFeatures.TabStop = true;
            this.lblFeatures.Text = "Features";
            this.lblFeatures.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblFeatures_LinkClicked);
            // 
            // aboutBox
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(345, 153);
            this.ControlBox = false;
            this.Controls.Add(this.mainPanel);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnCheckUpdate);
            this.DoubleBuffered = true;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(361, 192);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(361, 192);
            this.Name = "aboutBox";
            this.Padding = new System.Windows.Forms.Padding(9);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.aboutBox_Load);
            this.mainPanel.ResumeLayout(false);
            this.mainPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.LinkLabel lblDC;
        private System.Windows.Forms.Button btnCheckUpdate;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblAbout;
        private System.Windows.Forms.LinkLabel lblGit;
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.LinkLabel lblFeatures;
    }
}
