﻿namespace SettlersEditorUpdater
{
    partial class S6Patcher
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
            this.cbLAA = new System.Windows.Forms.CheckBox();
            this.cbZoom = new System.Windows.Forms.CheckBox();
            this.txtZoom = new System.Windows.Forms.TextBox();
            this.btnPatch = new System.Windows.Forms.Button();
            this.lblZoomAngle = new System.Windows.Forms.Label();
            this.cbAutosave = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // cbLAA
            // 
            this.cbLAA.AutoSize = true;
            this.cbLAA.Location = new System.Drawing.Point(13, 13);
            this.cbLAA.Name = "cbLAA";
            this.cbLAA.Size = new System.Drawing.Size(91, 20);
            this.cbLAA.TabIndex = 0;
            this.cbLAA.Text = "LAA - Flag";
            this.cbLAA.UseVisualStyleBackColor = true;
            // 
            // cbZoom
            // 
            this.cbZoom.AutoSize = true;
            this.cbZoom.Location = new System.Drawing.Point(13, 39);
            this.cbZoom.Name = "cbZoom";
            this.cbZoom.Size = new System.Drawing.Size(134, 20);
            this.cbZoom.TabIndex = 1;
            this.cbZoom.Text = "Max. Zoom Level:";
            this.cbZoom.UseVisualStyleBackColor = true;
            this.cbZoom.CheckedChanged += new System.EventHandler(this.cbZoom_CheckedChanged);
            // 
            // txtZoom
            // 
            this.txtZoom.Location = new System.Drawing.Point(186, 37);
            this.txtZoom.MaxLength = 8;
            this.txtZoom.Name = "txtZoom";
            this.txtZoom.Size = new System.Drawing.Size(100, 22);
            this.txtZoom.TabIndex = 2;
            // 
            // btnPatch
            // 
            this.btnPatch.Location = new System.Drawing.Point(12, 109);
            this.btnPatch.Name = "btnPatch";
            this.btnPatch.Size = new System.Drawing.Size(365, 37);
            this.btnPatch.TabIndex = 3;
            this.btnPatch.Text = "Patch";
            this.btnPatch.UseVisualStyleBackColor = true;
            this.btnPatch.Click += new System.EventHandler(this.btnPatch_Click);
            // 
            // lblZoomAngle
            // 
            this.lblZoomAngle.AutoSize = true;
            this.lblZoomAngle.Location = new System.Drawing.Point(292, 40);
            this.lblZoomAngle.Name = "lblZoomAngle";
            this.lblZoomAngle.Size = new System.Drawing.Size(85, 16);
            this.lblZoomAngle.TabIndex = 4;
            this.lblZoomAngle.Text = "Normal: 7200";
            // 
            // cbAutosave
            // 
            this.cbAutosave.AutoSize = true;
            this.cbAutosave.Enabled = false;
            this.cbAutosave.Location = new System.Drawing.Point(13, 65);
            this.cbAutosave.Name = "cbAutosave";
            this.cbAutosave.Size = new System.Drawing.Size(154, 20);
            this.cbAutosave.TabIndex = 5;
            this.cbAutosave.Text = "Deactivate Autosave";
            this.cbAutosave.UseVisualStyleBackColor = true;
            // 
            // S6Patcher
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(385, 154);
            this.ControlBox = false;
            this.Controls.Add(this.cbAutosave);
            this.Controls.Add(this.lblZoomAngle);
            this.Controls.Add(this.btnPatch);
            this.Controls.Add(this.txtZoom);
            this.Controls.Add(this.cbZoom);
            this.Controls.Add(this.cbLAA);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(403, 201);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(403, 201);
            this.Name = "S6Patcher";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "S6Patcher";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cbLAA;
        private System.Windows.Forms.CheckBox cbZoom;
        private System.Windows.Forms.TextBox txtZoom;
        private System.Windows.Forms.Button btnPatch;
        private System.Windows.Forms.Label lblZoomAngle;
        private System.Windows.Forms.CheckBox cbAutosave;
    }
}