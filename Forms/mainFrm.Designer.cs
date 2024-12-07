namespace S6Patcher
{
    partial class mainFrm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnPatch = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnPatchHE = new System.Windows.Forms.Button();
            this.btnPatchOV = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnPatch
            // 
            this.btnPatch.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPatch.Location = new System.Drawing.Point(478, 13);
            this.btnPatch.Name = "btnPatch";
            this.btnPatch.Size = new System.Drawing.Size(120, 72);
            this.btnPatch.TabIndex = 0;
            this.btnPatch.TabStop = false;
            this.btnPatch.Text = "Patch Mapeditor";
            this.btnPatch.UseVisualStyleBackColor = true;
            this.btnPatch.Click += new System.EventHandler(this.btnPatch_Click);
            // 
            // btnClose
            // 
            this.btnClose.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Location = new System.Drawing.Point(12, 12);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(117, 73);
            this.btnClose.TabIndex = 0;
            this.btnClose.TabStop = false;
            this.btnClose.Text = "Exit";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnPatchHE
            // 
            this.btnPatchHE.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPatchHE.Location = new System.Drawing.Point(352, 13);
            this.btnPatchHE.Name = "btnPatchHE";
            this.btnPatchHE.Size = new System.Drawing.Size(120, 72);
            this.btnPatchHE.TabIndex = 0;
            this.btnPatchHE.TabStop = false;
            this.btnPatchHE.Text = "Patch History Edition";
            this.btnPatchHE.UseVisualStyleBackColor = true;
            this.btnPatchHE.Click += new System.EventHandler(this.btnPatchHE_Click);
            // 
            // btnPatchOV
            // 
            this.btnPatchOV.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPatchOV.Location = new System.Drawing.Point(222, 12);
            this.btnPatchOV.Name = "btnPatchOV";
            this.btnPatchOV.Size = new System.Drawing.Size(124, 73);
            this.btnPatchOV.TabIndex = 0;
            this.btnPatchOV.TabStop = false;
            this.btnPatchOV.Text = "Patch Original Release";
            this.btnPatchOV.UseVisualStyleBackColor = true;
            this.btnPatchOV.Click += new System.EventHandler(this.btnPatchOV_Click);
            // 
            // mainFrm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.PapayaWhip;
            this.ClientSize = new System.Drawing.Size(612, 97);
            this.ControlBox = false;
            this.Controls.Add(this.btnPatchOV);
            this.Controls.Add(this.btnPatchHE);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnPatch);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(628, 136);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(628, 136);
            this.Name = "mainFrm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "-";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnPatch;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnPatchHE;
        private System.Windows.Forms.Button btnPatchOV;
    }
}

