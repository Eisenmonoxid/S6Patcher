namespace S6Patcher
{
    partial class mainPatcher
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
            this.cbLAAFlag = new System.Windows.Forms.CheckBox();
            this.cbZoom = new System.Windows.Forms.CheckBox();
            this.txtZoom = new System.Windows.Forms.TextBox();
            this.btnPatch = new System.Windows.Forms.Button();
            this.lblZoomAngle = new System.Windows.Forms.Label();
            this.cbAutosave = new System.Windows.Forms.CheckBox();
            this.cbAllEntities = new System.Windows.Forms.CheckBox();
            this.txtAutosave = new System.Windows.Forms.TextBox();
            this.lblAutosave = new System.Windows.Forms.Label();
            this.cbDevMode = new System.Windows.Forms.CheckBox();
            this.btnAbort = new System.Windows.Forms.Button();
            this.cbHighTextures = new System.Windows.Forms.CheckBox();
            this.cbEntityLimits = new System.Windows.Forms.CheckBox();
            this.cbScalingPlacing = new System.Windows.Forms.CheckBox();
            this.cbMapBorder = new System.Windows.Forms.CheckBox();
            this.gbEditor = new System.Windows.Forms.GroupBox();
            this.gbHE = new System.Windows.Forms.GroupBox();
            this.gbAll = new System.Windows.Forms.GroupBox();
            this.cbScriptBugFixes = new System.Windows.Forms.CheckBox();
            this.lblTextureRes = new System.Windows.Forms.Label();
            this.txtResolution = new System.Windows.Forms.TextBox();
            this.btnBackup = new System.Windows.Forms.Button();
            this.gbEditor.SuspendLayout();
            this.gbHE.SuspendLayout();
            this.gbAll.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbLAAFlag
            // 
            this.cbLAAFlag.AutoSize = true;
            this.cbLAAFlag.Location = new System.Drawing.Point(5, 47);
            this.cbLAAFlag.Name = "cbLAAFlag";
            this.cbLAAFlag.Size = new System.Drawing.Size(117, 17);
            this.cbLAAFlag.TabIndex = 0;
            this.cbLAAFlag.Text = "Activate LAA - Flag";
            this.cbLAAFlag.UseVisualStyleBackColor = true;
            // 
            // cbZoom
            // 
            this.cbZoom.AutoSize = true;
            this.cbZoom.Location = new System.Drawing.Point(5, 74);
            this.cbZoom.Name = "cbZoom";
            this.cbZoom.Size = new System.Drawing.Size(111, 17);
            this.cbZoom.TabIndex = 1;
            this.cbZoom.Text = "Max. Zoom Level:";
            this.cbZoom.UseVisualStyleBackColor = true;
            this.cbZoom.CheckedChanged += new System.EventHandler(this.cbZoom_CheckedChanged);
            // 
            // txtZoom
            // 
            this.txtZoom.Enabled = false;
            this.txtZoom.Location = new System.Drawing.Point(164, 72);
            this.txtZoom.MaxLength = 8;
            this.txtZoom.Name = "txtZoom";
            this.txtZoom.Size = new System.Drawing.Size(152, 20);
            this.txtZoom.TabIndex = 2;
            // 
            // btnPatch
            // 
            this.btnPatch.Location = new System.Drawing.Point(11, 367);
            this.btnPatch.Name = "btnPatch";
            this.btnPatch.Size = new System.Drawing.Size(449, 37);
            this.btnPatch.TabIndex = 3;
            this.btnPatch.Text = "Patch";
            this.btnPatch.UseVisualStyleBackColor = true;
            this.btnPatch.Click += new System.EventHandler(this.btnPatch_Click);
            // 
            // lblZoomAngle
            // 
            this.lblZoomAngle.AutoSize = true;
            this.lblZoomAngle.Location = new System.Drawing.Point(322, 75);
            this.lblZoomAngle.Name = "lblZoomAngle";
            this.lblZoomAngle.Size = new System.Drawing.Size(80, 13);
            this.lblZoomAngle.TabIndex = 4;
            this.lblZoomAngle.Text = "Standard: 7200";
            // 
            // cbAutosave
            // 
            this.cbAutosave.AutoSize = true;
            this.cbAutosave.Enabled = false;
            this.cbAutosave.Location = new System.Drawing.Point(5, 22);
            this.cbAutosave.Name = "cbAutosave";
            this.cbAutosave.Size = new System.Drawing.Size(125, 17);
            this.cbAutosave.TabIndex = 5;
            this.cbAutosave.Text = "Autosave Time (min):";
            this.cbAutosave.UseVisualStyleBackColor = true;
            this.cbAutosave.CheckedChanged += new System.EventHandler(this.cbAutosave_CheckedChanged);
            // 
            // cbAllEntities
            // 
            this.cbAllEntities.AutoSize = true;
            this.cbAllEntities.Enabled = false;
            this.cbAllEntities.Location = new System.Drawing.Point(6, 99);
            this.cbAllEntities.Name = "cbAllEntities";
            this.cbAllEntities.Size = new System.Drawing.Size(210, 17);
            this.cbAllEntities.TabIndex = 6;
            this.cbAllEntities.Text = "Show All Entities and Textures in Editor";
            this.cbAllEntities.UseVisualStyleBackColor = true;
            // 
            // txtAutosave
            // 
            this.txtAutosave.Enabled = false;
            this.txtAutosave.Location = new System.Drawing.Point(181, 20);
            this.txtAutosave.MaxLength = 5;
            this.txtAutosave.Name = "txtAutosave";
            this.txtAutosave.Size = new System.Drawing.Size(135, 20);
            this.txtAutosave.TabIndex = 7;
            // 
            // lblAutosave
            // 
            this.lblAutosave.AutoSize = true;
            this.lblAutosave.Location = new System.Drawing.Point(322, 23);
            this.lblAutosave.Name = "lblAutosave";
            this.lblAutosave.Size = new System.Drawing.Size(68, 13);
            this.lblAutosave.TabIndex = 8;
            this.lblAutosave.Text = "Standard: 15";
            // 
            // cbDevMode
            // 
            this.cbDevMode.AutoSize = true;
            this.cbDevMode.Location = new System.Drawing.Point(5, 100);
            this.cbDevMode.Name = "cbDevMode";
            this.cbDevMode.Size = new System.Drawing.Size(222, 17);
            this.cbDevMode.TabIndex = 9;
            this.cbDevMode.Text = "Activate Development-Mode Permanently";
            this.cbDevMode.UseVisualStyleBackColor = true;
            // 
            // btnAbort
            // 
            this.btnAbort.Location = new System.Drawing.Point(10, 453);
            this.btnAbort.Name = "btnAbort";
            this.btnAbort.Size = new System.Drawing.Size(449, 37);
            this.btnAbort.TabIndex = 10;
            this.btnAbort.Text = "Abort";
            this.btnAbort.UseVisualStyleBackColor = true;
            this.btnAbort.Click += new System.EventHandler(this.btnAbort_Click);
            // 
            // cbHighTextures
            // 
            this.cbHighTextures.AutoSize = true;
            this.cbHighTextures.Location = new System.Drawing.Point(6, 21);
            this.cbHighTextures.Name = "cbHighTextures";
            this.cbHighTextures.Size = new System.Drawing.Size(154, 17);
            this.cbHighTextures.TabIndex = 12;
            this.cbHighTextures.Tag = "";
            this.cbHighTextures.Text = "High - Resolution Textures:";
            this.cbHighTextures.UseVisualStyleBackColor = true;
            this.cbHighTextures.CheckedChanged += new System.EventHandler(this.cbHighTextures_CheckedChanged);
            // 
            // cbEntityLimits
            // 
            this.cbEntityLimits.AutoSize = true;
            this.cbEntityLimits.Enabled = false;
            this.cbEntityLimits.Location = new System.Drawing.Point(6, 73);
            this.cbEntityLimits.Name = "cbEntityLimits";
            this.cbEntityLimits.Size = new System.Drawing.Size(121, 17);
            this.cbEntityLimits.TabIndex = 13;
            this.cbEntityLimits.Text = "Higher Entity - Limits";
            this.cbEntityLimits.UseVisualStyleBackColor = true;
            // 
            // cbScalingPlacing
            // 
            this.cbScalingPlacing.AutoSize = true;
            this.cbScalingPlacing.Enabled = false;
            this.cbScalingPlacing.Location = new System.Drawing.Point(6, 47);
            this.cbScalingPlacing.Name = "cbScalingPlacing";
            this.cbScalingPlacing.Size = new System.Drawing.Size(193, 17);
            this.cbScalingPlacing.TabIndex = 14;
            this.cbScalingPlacing.Text = "Free Scaling and Placing of Entities";
            this.cbScalingPlacing.UseVisualStyleBackColor = true;
            // 
            // cbMapBorder
            // 
            this.cbMapBorder.AutoSize = true;
            this.cbMapBorder.Enabled = false;
            this.cbMapBorder.Location = new System.Drawing.Point(6, 21);
            this.cbMapBorder.Name = "cbMapBorder";
            this.cbMapBorder.Size = new System.Drawing.Size(147, 17);
            this.cbMapBorder.TabIndex = 15;
            this.cbMapBorder.Text = "Usable Black Map Border";
            this.cbMapBorder.UseVisualStyleBackColor = true;
            // 
            // gbEditor
            // 
            this.gbEditor.Controls.Add(this.cbMapBorder);
            this.gbEditor.Controls.Add(this.cbEntityLimits);
            this.gbEditor.Controls.Add(this.cbScalingPlacing);
            this.gbEditor.Controls.Add(this.cbAllEntities);
            this.gbEditor.Location = new System.Drawing.Point(11, 223);
            this.gbEditor.Name = "gbEditor";
            this.gbEditor.Size = new System.Drawing.Size(448, 125);
            this.gbEditor.TabIndex = 16;
            this.gbEditor.TabStop = false;
            this.gbEditor.Text = "Mapeditor";
            // 
            // gbHE
            // 
            this.gbHE.Controls.Add(this.cbAutosave);
            this.gbHE.Controls.Add(this.txtAutosave);
            this.gbHE.Controls.Add(this.lblAutosave);
            this.gbHE.Location = new System.Drawing.Point(12, 168);
            this.gbHE.Name = "gbHE";
            this.gbHE.Size = new System.Drawing.Size(448, 49);
            this.gbHE.TabIndex = 17;
            this.gbHE.TabStop = false;
            this.gbHE.Text = "History Edition";
            // 
            // gbAll
            // 
            this.gbAll.Controls.Add(this.cbScriptBugFixes);
            this.gbAll.Controls.Add(this.lblTextureRes);
            this.gbAll.Controls.Add(this.txtResolution);
            this.gbAll.Controls.Add(this.cbHighTextures);
            this.gbAll.Controls.Add(this.cbLAAFlag);
            this.gbAll.Controls.Add(this.cbZoom);
            this.gbAll.Controls.Add(this.txtZoom);
            this.gbAll.Controls.Add(this.cbDevMode);
            this.gbAll.Controls.Add(this.lblZoomAngle);
            this.gbAll.Location = new System.Drawing.Point(12, 12);
            this.gbAll.Name = "gbAll";
            this.gbAll.Size = new System.Drawing.Size(447, 150);
            this.gbAll.TabIndex = 18;
            this.gbAll.TabStop = false;
            this.gbAll.Text = "General Options";
            // 
            // cbScriptBugFixes
            // 
            this.cbScriptBugFixes.AutoSize = true;
            this.cbScriptBugFixes.Location = new System.Drawing.Point(5, 127);
            this.cbScriptBugFixes.Name = "cbScriptBugFixes";
            this.cbScriptBugFixes.Size = new System.Drawing.Size(187, 17);
            this.cbScriptBugFixes.TabIndex = 15;
            this.cbScriptBugFixes.Text = "Activate Script and Code Bugfixes";
            this.cbScriptBugFixes.UseVisualStyleBackColor = true;
            // 
            // lblTextureRes
            // 
            this.lblTextureRes.AutoSize = true;
            this.lblTextureRes.Location = new System.Drawing.Point(345, 22);
            this.lblTextureRes.Name = "lblTextureRes";
            this.lblTextureRes.Size = new System.Drawing.Size(74, 13);
            this.lblTextureRes.TabIndex = 14;
            this.lblTextureRes.Text = "Standard: 512";
            // 
            // txtResolution
            // 
            this.txtResolution.Enabled = false;
            this.txtResolution.Location = new System.Drawing.Point(197, 19);
            this.txtResolution.MaxLength = 4;
            this.txtResolution.Name = "txtResolution";
            this.txtResolution.Size = new System.Drawing.Size(142, 20);
            this.txtResolution.TabIndex = 13;
            // 
            // btnBackup
            // 
            this.btnBackup.Location = new System.Drawing.Point(11, 410);
            this.btnBackup.Name = "btnBackup";
            this.btnBackup.Size = new System.Drawing.Size(449, 37);
            this.btnBackup.TabIndex = 19;
            this.btnBackup.Text = "Restore Backup";
            this.btnBackup.UseVisualStyleBackColor = true;
            this.btnBackup.Click += new System.EventHandler(this.btnBackup_Click);
            // 
            // mainPatcher
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.PapayaWhip;
            this.ClientSize = new System.Drawing.Size(471, 509);
            this.ControlBox = false;
            this.Controls.Add(this.btnBackup);
            this.Controls.Add(this.gbAll);
            this.Controls.Add(this.gbHE);
            this.Controls.Add(this.gbEditor);
            this.Controls.Add(this.btnAbort);
            this.Controls.Add(this.btnPatch);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(487, 548);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(487, 548);
            this.Name = "mainPatcher";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "-";
            this.TopMost = true;
            this.gbEditor.ResumeLayout(false);
            this.gbEditor.PerformLayout();
            this.gbHE.ResumeLayout(false);
            this.gbHE.PerformLayout();
            this.gbAll.ResumeLayout(false);
            this.gbAll.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox cbLAAFlag;
        private System.Windows.Forms.CheckBox cbZoom;
        private System.Windows.Forms.TextBox txtZoom;
        private System.Windows.Forms.Button btnPatch;
        private System.Windows.Forms.Label lblZoomAngle;
        private System.Windows.Forms.CheckBox cbAutosave;
        private System.Windows.Forms.CheckBox cbAllEntities;
        private System.Windows.Forms.TextBox txtAutosave;
        private System.Windows.Forms.Label lblAutosave;
        private System.Windows.Forms.CheckBox cbDevMode;
        private System.Windows.Forms.Button btnAbort;
        private System.Windows.Forms.CheckBox cbHighTextures;
        private System.Windows.Forms.CheckBox cbEntityLimits;
        private System.Windows.Forms.CheckBox cbScalingPlacing;
        private System.Windows.Forms.CheckBox cbMapBorder;
        private System.Windows.Forms.GroupBox gbEditor;
        private System.Windows.Forms.GroupBox gbHE;
        private System.Windows.Forms.GroupBox gbAll;
        private System.Windows.Forms.Button btnBackup;
        private System.Windows.Forms.Label lblTextureRes;
        private System.Windows.Forms.TextBox txtResolution;
        private System.Windows.Forms.CheckBox cbScriptBugFixes;
    }
}