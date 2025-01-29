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
            lblDC = new System.Windows.Forms.LinkLabel();
            btnCheckUpdate = new System.Windows.Forms.Button();
            btnClose = new System.Windows.Forms.Button();
            lblAbout = new System.Windows.Forms.Label();
            lblGit = new System.Windows.Forms.LinkLabel();
            mainPanel = new System.Windows.Forms.Panel();
            lblFeatures = new System.Windows.Forms.LinkLabel();
            mainPanel.SuspendLayout();
            SuspendLayout();
            // 
            // lblDC
            // 
            lblDC.AutoSize = true;
            lblDC.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            lblDC.Location = new System.Drawing.Point(69, 39);
            lblDC.Name = "lblDC";
            lblDC.Size = new System.Drawing.Size(63, 20);
            lblDC.TabIndex = 0;
            lblDC.TabStop = true;
            lblDC.Text = "Discord";
            lblDC.LinkClicked += lblDC_LinkClicked;
            // 
            // btnCheckUpdate
            // 
            btnCheckUpdate.Location = new System.Drawing.Point(224, 97);
            btnCheckUpdate.Name = "btnCheckUpdate";
            btnCheckUpdate.Size = new System.Drawing.Size(109, 44);
            btnCheckUpdate.TabIndex = 1;
            btnCheckUpdate.Text = "Check for Updates";
            btnCheckUpdate.UseVisualStyleBackColor = true;
            btnCheckUpdate.Click += btnCheckUpdate_Click;
            // 
            // btnClose
            // 
            btnClose.Location = new System.Drawing.Point(12, 97);
            btnClose.Name = "btnClose";
            btnClose.Size = new System.Drawing.Size(109, 44);
            btnClose.TabIndex = 2;
            btnClose.Text = "Close";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += btnClose_Click;
            // 
            // lblAbout
            // 
            lblAbout.AutoSize = true;
            lblAbout.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            lblAbout.Location = new System.Drawing.Point(3, 9);
            lblAbout.Name = "lblAbout";
            lblAbout.Size = new System.Drawing.Size(178, 16);
            lblAbout.TabIndex = 3;
            lblAbout.Text = "S6Patcher by Eisenmonoxid:";
            // 
            // lblGit
            // 
            lblGit.AutoSize = true;
            lblGit.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            lblGit.Location = new System.Drawing.Point(3, 39);
            lblGit.Name = "lblGit";
            lblGit.Size = new System.Drawing.Size(60, 20);
            lblGit.TabIndex = 4;
            lblGit.TabStop = true;
            lblGit.Text = "GitHub";
            lblGit.LinkClicked += lblGit_LinkClicked;
            // 
            // mainPanel
            // 
            mainPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            mainPanel.Controls.Add(lblFeatures);
            mainPanel.Controls.Add(lblAbout);
            mainPanel.Controls.Add(lblGit);
            mainPanel.Controls.Add(lblDC);
            mainPanel.Location = new System.Drawing.Point(12, 12);
            mainPanel.Name = "mainPanel";
            mainPanel.Size = new System.Drawing.Size(321, 79);
            mainPanel.TabIndex = 5;
            // 
            // lblFeatures
            // 
            lblFeatures.AutoSize = true;
            lblFeatures.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            lblFeatures.Location = new System.Drawing.Point(138, 39);
            lblFeatures.Name = "lblFeatures";
            lblFeatures.Size = new System.Drawing.Size(73, 20);
            lblFeatures.TabIndex = 5;
            lblFeatures.TabStop = true;
            lblFeatures.Text = "Features";
            lblFeatures.LinkClicked += lblFeatures_LinkClicked;
            // 
            // aboutBox
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            AutoScroll = true;
            ClientSize = new System.Drawing.Size(345, 153);
            ControlBox = false;
            Controls.Add(mainPanel);
            Controls.Add(btnClose);
            Controls.Add(btnCheckUpdate);
            DoubleBuffered = true;
            MaximizeBox = false;
            MaximumSize = new System.Drawing.Size(361, 192);
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(361, 192);
            Name = "aboutBox";
            Padding = new System.Windows.Forms.Padding(9);
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "About";
            Load += aboutBox_Load;
            mainPanel.ResumeLayout(false);
            mainPanel.PerformLayout();
            ResumeLayout(false);
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
