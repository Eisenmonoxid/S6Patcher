namespace S6Patcher.Source.Forms
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
            cbLAAFlag = new System.Windows.Forms.CheckBox();
            cbZoom = new System.Windows.Forms.CheckBox();
            txtZoom = new System.Windows.Forms.TextBox();
            btnPatch = new System.Windows.Forms.Button();
            lblZoomAngle = new System.Windows.Forms.Label();
            cbAutosave = new System.Windows.Forms.CheckBox();
            cbAllEntities = new System.Windows.Forms.CheckBox();
            txtAutosave = new System.Windows.Forms.TextBox();
            lblAutosave = new System.Windows.Forms.Label();
            cbDevMode = new System.Windows.Forms.CheckBox();
            btnAbort = new System.Windows.Forms.Button();
            cbHighTextures = new System.Windows.Forms.CheckBox();
            cbEntityLimits = new System.Windows.Forms.CheckBox();
            cbScalingPlacing = new System.Windows.Forms.CheckBox();
            cbMapBorder = new System.Windows.Forms.CheckBox();
            gbEditor = new System.Windows.Forms.GroupBox();
            gbHE = new System.Windows.Forms.GroupBox();
            gbAll = new System.Windows.Forms.GroupBox();
            cbModloader = new System.Windows.Forms.CheckBox();
            cbLimitedEdition = new System.Windows.Forms.CheckBox();
            cbKnightSelection = new System.Windows.Forms.CheckBox();
            cbScriptBugFixes = new System.Windows.Forms.CheckBox();
            lblTextureRes = new System.Windows.Forms.Label();
            txtResolution = new System.Windows.Forms.TextBox();
            btnBackup = new System.Windows.Forms.Button();
            btnChooseFile = new System.Windows.Forms.Button();
            txtExecutablePath = new System.Windows.Forms.TextBox();
            lblSelectFile = new System.Windows.Forms.Label();
            gbEditor.SuspendLayout();
            gbHE.SuspendLayout();
            gbAll.SuspendLayout();
            SuspendLayout();
            // 
            // cbLAAFlag
            // 
            cbLAAFlag.AutoSize = true;
            cbLAAFlag.Location = new System.Drawing.Point(6, 47);
            cbLAAFlag.Name = "cbLAAFlag";
            cbLAAFlag.Size = new System.Drawing.Size(139, 20);
            cbLAAFlag.TabIndex = 0;
            cbLAAFlag.Text = "Activate LAA - Flag";
            cbLAAFlag.UseVisualStyleBackColor = true;
            // 
            // cbZoom
            // 
            cbZoom.AutoSize = true;
            cbZoom.Location = new System.Drawing.Point(6, 73);
            cbZoom.Name = "cbZoom";
            cbZoom.Size = new System.Drawing.Size(131, 20);
            cbZoom.TabIndex = 1;
            cbZoom.Text = "Max. Zoom Level:";
            cbZoom.UseVisualStyleBackColor = true;
            cbZoom.CheckedChanged += cbZoom_CheckedChanged;
            // 
            // txtZoom
            // 
            txtZoom.Enabled = false;
            txtZoom.Location = new System.Drawing.Point(143, 71);
            txtZoom.MaxLength = 8;
            txtZoom.Name = "txtZoom";
            txtZoom.Size = new System.Drawing.Size(152, 22);
            txtZoom.TabIndex = 2;
            // 
            // btnPatch
            // 
            btnPatch.Enabled = false;
            btnPatch.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            btnPatch.Location = new System.Drawing.Point(12, 467);
            btnPatch.Name = "btnPatch";
            btnPatch.Size = new System.Drawing.Size(642, 47);
            btnPatch.TabIndex = 3;
            btnPatch.Text = "Patch File";
            btnPatch.UseVisualStyleBackColor = true;
            btnPatch.Click += btnPatch_Click;
            // 
            // lblZoomAngle
            // 
            lblZoomAngle.AutoSize = true;
            lblZoomAngle.Location = new System.Drawing.Point(301, 74);
            lblZoomAngle.Name = "lblZoomAngle";
            lblZoomAngle.Size = new System.Drawing.Size(96, 16);
            lblZoomAngle.TabIndex = 4;
            lblZoomAngle.Text = "Standard: 7200";
            // 
            // cbAutosave
            // 
            cbAutosave.AutoSize = true;
            cbAutosave.Location = new System.Drawing.Point(6, 22);
            cbAutosave.Name = "cbAutosave";
            cbAutosave.Size = new System.Drawing.Size(152, 20);
            cbAutosave.TabIndex = 5;
            cbAutosave.Text = "Autosave Time (min):";
            cbAutosave.UseVisualStyleBackColor = true;
            cbAutosave.CheckedChanged += cbAutosave_CheckedChanged;
            // 
            // cbAllEntities
            // 
            cbAllEntities.AutoSize = true;
            cbAllEntities.Location = new System.Drawing.Point(6, 99);
            cbAllEntities.Name = "cbAllEntities";
            cbAllEntities.Size = new System.Drawing.Size(255, 20);
            cbAllEntities.TabIndex = 6;
            cbAllEntities.Text = "Show All Entities and Textures in Editor";
            cbAllEntities.UseVisualStyleBackColor = true;
            // 
            // txtAutosave
            // 
            txtAutosave.Enabled = false;
            txtAutosave.Location = new System.Drawing.Point(164, 20);
            txtAutosave.MaxLength = 5;
            txtAutosave.Name = "txtAutosave";
            txtAutosave.Size = new System.Drawing.Size(135, 22);
            txtAutosave.TabIndex = 7;
            // 
            // lblAutosave
            // 
            lblAutosave.AutoSize = true;
            lblAutosave.Location = new System.Drawing.Point(305, 23);
            lblAutosave.Name = "lblAutosave";
            lblAutosave.Size = new System.Drawing.Size(82, 16);
            lblAutosave.TabIndex = 8;
            lblAutosave.Text = "Standard: 15";
            // 
            // cbDevMode
            // 
            cbDevMode.AutoSize = true;
            cbDevMode.Location = new System.Drawing.Point(6, 99);
            cbDevMode.Name = "cbDevMode";
            cbDevMode.Size = new System.Drawing.Size(275, 20);
            cbDevMode.TabIndex = 9;
            cbDevMode.Text = "Activate Development-Mode Permanently";
            cbDevMode.UseVisualStyleBackColor = true;
            // 
            // btnAbort
            // 
            btnAbort.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            btnAbort.Location = new System.Drawing.Point(12, 573);
            btnAbort.Name = "btnAbort";
            btnAbort.Size = new System.Drawing.Size(642, 47);
            btnAbort.TabIndex = 10;
            btnAbort.Text = "Exit";
            btnAbort.UseVisualStyleBackColor = true;
            btnAbort.Click += btnAbort_Click;
            // 
            // cbHighTextures
            // 
            cbHighTextures.AutoSize = true;
            cbHighTextures.Location = new System.Drawing.Point(6, 21);
            cbHighTextures.Name = "cbHighTextures";
            cbHighTextures.Size = new System.Drawing.Size(186, 20);
            cbHighTextures.TabIndex = 12;
            cbHighTextures.Tag = "";
            cbHighTextures.Text = "High - Resolution Textures:";
            cbHighTextures.UseVisualStyleBackColor = true;
            cbHighTextures.CheckedChanged += cbHighTextures_CheckedChanged;
            // 
            // cbEntityLimits
            // 
            cbEntityLimits.AutoSize = true;
            cbEntityLimits.Location = new System.Drawing.Point(6, 73);
            cbEntityLimits.Name = "cbEntityLimits";
            cbEntityLimits.Size = new System.Drawing.Size(138, 20);
            cbEntityLimits.TabIndex = 13;
            cbEntityLimits.Text = "Higher Entity Limits";
            cbEntityLimits.UseVisualStyleBackColor = true;
            // 
            // cbScalingPlacing
            // 
            cbScalingPlacing.AutoSize = true;
            cbScalingPlacing.Location = new System.Drawing.Point(6, 47);
            cbScalingPlacing.Name = "cbScalingPlacing";
            cbScalingPlacing.Size = new System.Drawing.Size(236, 20);
            cbScalingPlacing.TabIndex = 14;
            cbScalingPlacing.Text = "Free Scaling and Placing of Entities";
            cbScalingPlacing.UseVisualStyleBackColor = true;
            // 
            // cbMapBorder
            // 
            cbMapBorder.AutoSize = true;
            cbMapBorder.Location = new System.Drawing.Point(6, 21);
            cbMapBorder.Name = "cbMapBorder";
            cbMapBorder.Size = new System.Drawing.Size(181, 20);
            cbMapBorder.TabIndex = 15;
            cbMapBorder.Text = "Usable Black Map Border";
            cbMapBorder.UseVisualStyleBackColor = true;
            // 
            // gbEditor
            // 
            gbEditor.Controls.Add(cbMapBorder);
            gbEditor.Controls.Add(cbEntityLimits);
            gbEditor.Controls.Add(cbScalingPlacing);
            gbEditor.Controls.Add(cbAllEntities);
            gbEditor.Enabled = false;
            gbEditor.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            gbEditor.Location = new System.Drawing.Point(12, 336);
            gbEditor.Name = "gbEditor";
            gbEditor.Size = new System.Drawing.Size(642, 125);
            gbEditor.TabIndex = 16;
            gbEditor.TabStop = false;
            gbEditor.Text = "Mapeditor";
            // 
            // gbHE
            // 
            gbHE.Controls.Add(cbAutosave);
            gbHE.Controls.Add(txtAutosave);
            gbHE.Controls.Add(lblAutosave);
            gbHE.Enabled = false;
            gbHE.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            gbHE.Location = new System.Drawing.Point(12, 281);
            gbHE.Name = "gbHE";
            gbHE.Size = new System.Drawing.Size(642, 49);
            gbHE.TabIndex = 17;
            gbHE.TabStop = false;
            gbHE.Text = "History Edition";
            // 
            // gbAll
            // 
            gbAll.Controls.Add(cbModloader);
            gbAll.Controls.Add(cbLimitedEdition);
            gbAll.Controls.Add(cbKnightSelection);
            gbAll.Controls.Add(cbScriptBugFixes);
            gbAll.Controls.Add(lblTextureRes);
            gbAll.Controls.Add(txtResolution);
            gbAll.Controls.Add(cbHighTextures);
            gbAll.Controls.Add(cbLAAFlag);
            gbAll.Controls.Add(cbZoom);
            gbAll.Controls.Add(txtZoom);
            gbAll.Controls.Add(cbDevMode);
            gbAll.Controls.Add(lblZoomAngle);
            gbAll.Enabled = false;
            gbAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            gbAll.Location = new System.Drawing.Point(12, 42);
            gbAll.Name = "gbAll";
            gbAll.Size = new System.Drawing.Size(642, 233);
            gbAll.TabIndex = 18;
            gbAll.TabStop = false;
            gbAll.Text = "General Options";
            // 
            // cbModloader
            // 
            cbModloader.AutoSize = true;
            cbModloader.Location = new System.Drawing.Point(6, 203);
            cbModloader.Name = "cbModloader";
            cbModloader.Size = new System.Drawing.Size(143, 20);
            cbModloader.TabIndex = 18;
            cbModloader.Text = "Activate Modloader";
            cbModloader.UseVisualStyleBackColor = true;
            // 
            // cbLimitedEdition
            // 
            cbLimitedEdition.AutoSize = true;
            cbLimitedEdition.Location = new System.Drawing.Point(6, 177);
            cbLimitedEdition.Name = "cbLimitedEdition";
            cbLimitedEdition.Size = new System.Drawing.Size(214, 20);
            cbLimitedEdition.TabIndex = 17;
            cbLimitedEdition.Text = "Activate Limited/Special Edition";
            cbLimitedEdition.UseVisualStyleBackColor = true;
            // 
            // cbKnightSelection
            // 
            cbKnightSelection.AutoSize = true;
            cbKnightSelection.Enabled = false;
            cbKnightSelection.Location = new System.Drawing.Point(6, 151);
            cbKnightSelection.Name = "cbKnightSelection";
            cbKnightSelection.Size = new System.Drawing.Size(290, 20);
            cbKnightSelection.TabIndex = 16;
            cbKnightSelection.Text = "Enable Basegame Knights in Eastern Realm";
            cbKnightSelection.UseVisualStyleBackColor = true;
            // 
            // cbScriptBugFixes
            // 
            cbScriptBugFixes.AutoSize = true;
            cbScriptBugFixes.Location = new System.Drawing.Point(6, 125);
            cbScriptBugFixes.Name = "cbScriptBugFixes";
            cbScriptBugFixes.Size = new System.Drawing.Size(227, 20);
            cbScriptBugFixes.TabIndex = 15;
            cbScriptBugFixes.Text = "Activate Script and Code Bugfixes";
            cbScriptBugFixes.UseVisualStyleBackColor = true;
            cbScriptBugFixes.CheckedChanged += cbScriptBugFixes_CheckedChanged;
            // 
            // lblTextureRes
            // 
            lblTextureRes.AutoSize = true;
            lblTextureRes.Location = new System.Drawing.Point(346, 22);
            lblTextureRes.Name = "lblTextureRes";
            lblTextureRes.Size = new System.Drawing.Size(89, 16);
            lblTextureRes.TabIndex = 14;
            lblTextureRes.Text = "Standard: 512";
            // 
            // txtResolution
            // 
            txtResolution.Location = new System.Drawing.Point(198, 19);
            txtResolution.MaxLength = 4;
            txtResolution.Name = "txtResolution";
            txtResolution.Size = new System.Drawing.Size(142, 22);
            txtResolution.TabIndex = 13;
            // 
            // btnBackup
            // 
            btnBackup.Enabled = false;
            btnBackup.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            btnBackup.Location = new System.Drawing.Point(12, 520);
            btnBackup.Name = "btnBackup";
            btnBackup.Size = new System.Drawing.Size(642, 47);
            btnBackup.TabIndex = 19;
            btnBackup.Text = "Restore Backup";
            btnBackup.UseVisualStyleBackColor = true;
            btnBackup.Click += btnBackup_Click;
            // 
            // btnChooseFile
            // 
            btnChooseFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            btnChooseFile.Location = new System.Drawing.Point(548, 6);
            btnChooseFile.Name = "btnChooseFile";
            btnChooseFile.Size = new System.Drawing.Size(106, 28);
            btnChooseFile.TabIndex = 20;
            btnChooseFile.Text = "Choose File ...";
            btnChooseFile.UseVisualStyleBackColor = true;
            btnChooseFile.Click += btnChooseFile_Click;
            // 
            // txtExecutablePath
            // 
            txtExecutablePath.Location = new System.Drawing.Point(136, 10);
            txtExecutablePath.Name = "txtExecutablePath";
            txtExecutablePath.ReadOnly = true;
            txtExecutablePath.Size = new System.Drawing.Size(406, 23);
            txtExecutablePath.TabIndex = 21;
            // 
            // lblSelectFile
            // 
            lblSelectFile.AutoSize = true;
            lblSelectFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            lblSelectFile.Location = new System.Drawing.Point(12, 11);
            lblSelectFile.Name = "lblSelectFile";
            lblSelectFile.Size = new System.Drawing.Size(117, 16);
            lblSelectFile.TabIndex = 22;
            lblSelectFile.Text = "Select executable:";
            // 
            // mainFrm
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            AutoScroll = true;
            ClientSize = new System.Drawing.Size(665, 630);
            Controls.Add(lblSelectFile);
            Controls.Add(txtExecutablePath);
            Controls.Add(btnChooseFile);
            Controls.Add(btnBackup);
            Controls.Add(gbAll);
            Controls.Add(gbHE);
            Controls.Add(gbEditor);
            Controls.Add(btnAbort);
            Controls.Add(btnPatch);
            DoubleBuffered = true;
            HelpButton = true;
            MaximizeBox = false;
            MaximumSize = new System.Drawing.Size(681, 669);
            MinimizeBox = false;
            MinimumSize = new System.Drawing.Size(681, 669);
            Name = "mainFrm";
            ShowIcon = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "-";
            HelpButtonClicked += mainFrm_HelpButtonClicked;
            FormClosing += mainFrm_FormClosing;
            gbEditor.ResumeLayout(false);
            gbEditor.PerformLayout();
            gbHE.ResumeLayout(false);
            gbHE.PerformLayout();
            gbAll.ResumeLayout(false);
            gbAll.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
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
        private System.Windows.Forms.CheckBox cbLimitedEdition;
        private System.Windows.Forms.CheckBox cbKnightSelection;
        private System.Windows.Forms.Button btnChooseFile;
        private System.Windows.Forms.TextBox txtExecutablePath;
        private System.Windows.Forms.Label lblSelectFile;
        private System.Windows.Forms.CheckBox cbModloader;
    }
}