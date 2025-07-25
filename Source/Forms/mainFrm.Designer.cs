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
            this.gbUserscriptOptions = new System.Windows.Forms.GroupBox();
            this.cbSpecialKnightsAvailable = new System.Windows.Forms.CheckBox();
            this.cbDayNightCycle = new System.Windows.Forms.CheckBox();
            this.cbUseMilitaryRelease = new System.Windows.Forms.CheckBox();
            this.cbUseDowngrade = new System.Windows.Forms.CheckBox();
            this.cbUseSingleStop = new System.Windows.Forms.CheckBox();
            this.cbLimitedEdition = new System.Windows.Forms.CheckBox();
            this.cbKnightSelection = new System.Windows.Forms.CheckBox();
            this.cbScriptBugFixes = new System.Windows.Forms.CheckBox();
            this.lblTextureRes = new System.Windows.Forms.Label();
            this.txtResolution = new System.Windows.Forms.TextBox();
            this.cbBugfixMod = new System.Windows.Forms.CheckBox();
            this.cbModloader = new System.Windows.Forms.CheckBox();
            this.btnBackup = new System.Windows.Forms.Button();
            this.btnChooseFile = new System.Windows.Forms.Button();
            this.txtExecutablePath = new System.Windows.Forms.TextBox();
            this.lblSelectFile = new System.Windows.Forms.Label();
            this.gbModloader = new System.Windows.Forms.GroupBox();
            this.pbProgress = new System.Windows.Forms.ProgressBar();
            this.gbEditor.SuspendLayout();
            this.gbHE.SuspendLayout();
            this.gbAll.SuspendLayout();
            this.gbUserscriptOptions.SuspendLayout();
            this.gbModloader.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbLAAFlag
            // 
            this.cbLAAFlag.AutoSize = true;
            this.cbLAAFlag.Location = new System.Drawing.Point(6, 47);
            this.cbLAAFlag.Name = "cbLAAFlag";
            this.cbLAAFlag.Size = new System.Drawing.Size(237, 20);
            this.cbLAAFlag.TabIndex = 0;
            this.cbLAAFlag.Text = "Activate Large Address Aware Flag";
            this.cbLAAFlag.UseVisualStyleBackColor = true;
            // 
            // cbZoom
            // 
            this.cbZoom.AutoSize = true;
            this.cbZoom.Location = new System.Drawing.Point(6, 73);
            this.cbZoom.Name = "cbZoom";
            this.cbZoom.Size = new System.Drawing.Size(131, 20);
            this.cbZoom.TabIndex = 1;
            this.cbZoom.Text = "Max. Zoom Level:";
            this.cbZoom.UseVisualStyleBackColor = true;
            this.cbZoom.CheckedChanged += new System.EventHandler(this.cbZoom_CheckedChanged);
            // 
            // txtZoom
            // 
            this.txtZoom.Enabled = false;
            this.txtZoom.Location = new System.Drawing.Point(143, 71);
            this.txtZoom.MaxLength = 8;
            this.txtZoom.Name = "txtZoom";
            this.txtZoom.Size = new System.Drawing.Size(152, 22);
            this.txtZoom.TabIndex = 2;
            // 
            // btnPatch
            // 
            this.btnPatch.Enabled = false;
            this.btnPatch.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPatch.Location = new System.Drawing.Point(12, 513);
            this.btnPatch.Name = "btnPatch";
            this.btnPatch.Size = new System.Drawing.Size(642, 47);
            this.btnPatch.TabIndex = 3;
            this.btnPatch.Text = "Patch File";
            this.btnPatch.UseVisualStyleBackColor = true;
            this.btnPatch.Click += new System.EventHandler(this.btnPatch_Click);
            // 
            // lblZoomAngle
            // 
            this.lblZoomAngle.AutoSize = true;
            this.lblZoomAngle.Location = new System.Drawing.Point(301, 74);
            this.lblZoomAngle.Name = "lblZoomAngle";
            this.lblZoomAngle.Size = new System.Drawing.Size(83, 16);
            this.lblZoomAngle.TabIndex = 4;
            this.lblZoomAngle.Text = "Default: 7200";
            // 
            // cbAutosave
            // 
            this.cbAutosave.AutoSize = true;
            this.cbAutosave.Location = new System.Drawing.Point(6, 22);
            this.cbAutosave.Name = "cbAutosave";
            this.cbAutosave.Size = new System.Drawing.Size(152, 20);
            this.cbAutosave.TabIndex = 5;
            this.cbAutosave.Text = "Autosave Time (min):";
            this.cbAutosave.UseVisualStyleBackColor = true;
            this.cbAutosave.CheckedChanged += new System.EventHandler(this.cbAutosave_CheckedChanged);
            // 
            // cbAllEntities
            // 
            this.cbAllEntities.AutoSize = true;
            this.cbAllEntities.Location = new System.Drawing.Point(6, 99);
            this.cbAllEntities.Name = "cbAllEntities";
            this.cbAllEntities.Size = new System.Drawing.Size(255, 20);
            this.cbAllEntities.TabIndex = 6;
            this.cbAllEntities.Text = "Show All Entities and Textures in Editor";
            this.cbAllEntities.UseVisualStyleBackColor = true;
            // 
            // txtAutosave
            // 
            this.txtAutosave.Enabled = false;
            this.txtAutosave.Location = new System.Drawing.Point(164, 20);
            this.txtAutosave.MaxLength = 5;
            this.txtAutosave.Name = "txtAutosave";
            this.txtAutosave.Size = new System.Drawing.Size(135, 22);
            this.txtAutosave.TabIndex = 7;
            // 
            // lblAutosave
            // 
            this.lblAutosave.AutoSize = true;
            this.lblAutosave.Location = new System.Drawing.Point(305, 23);
            this.lblAutosave.Name = "lblAutosave";
            this.lblAutosave.Size = new System.Drawing.Size(69, 16);
            this.lblAutosave.TabIndex = 8;
            this.lblAutosave.Text = "Default: 15";
            // 
            // cbDevMode
            // 
            this.cbDevMode.AutoSize = true;
            this.cbDevMode.Location = new System.Drawing.Point(6, 99);
            this.cbDevMode.Name = "cbDevMode";
            this.cbDevMode.Size = new System.Drawing.Size(274, 20);
            this.cbDevMode.TabIndex = 9;
            this.cbDevMode.Text = "Activate Development Mode Permanently";
            this.cbDevMode.UseVisualStyleBackColor = true;
            // 
            // btnAbort
            // 
            this.btnAbort.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAbort.Location = new System.Drawing.Point(12, 619);
            this.btnAbort.Name = "btnAbort";
            this.btnAbort.Size = new System.Drawing.Size(642, 47);
            this.btnAbort.TabIndex = 10;
            this.btnAbort.Text = "Exit";
            this.btnAbort.UseVisualStyleBackColor = true;
            this.btnAbort.Click += new System.EventHandler(this.btnAbort_Click);
            // 
            // cbHighTextures
            // 
            this.cbHighTextures.AutoSize = true;
            this.cbHighTextures.Location = new System.Drawing.Point(6, 21);
            this.cbHighTextures.Name = "cbHighTextures";
            this.cbHighTextures.Size = new System.Drawing.Size(186, 20);
            this.cbHighTextures.TabIndex = 12;
            this.cbHighTextures.Tag = "";
            this.cbHighTextures.Text = "High - Resolution Textures:";
            this.cbHighTextures.UseVisualStyleBackColor = true;
            this.cbHighTextures.CheckedChanged += new System.EventHandler(this.cbHighTextures_CheckedChanged);
            // 
            // cbEntityLimits
            // 
            this.cbEntityLimits.AutoSize = true;
            this.cbEntityLimits.Location = new System.Drawing.Point(6, 73);
            this.cbEntityLimits.Name = "cbEntityLimits";
            this.cbEntityLimits.Size = new System.Drawing.Size(138, 20);
            this.cbEntityLimits.TabIndex = 13;
            this.cbEntityLimits.Text = "Higher Entity Limits";
            this.cbEntityLimits.UseVisualStyleBackColor = true;
            // 
            // cbScalingPlacing
            // 
            this.cbScalingPlacing.AutoSize = true;
            this.cbScalingPlacing.Location = new System.Drawing.Point(6, 47);
            this.cbScalingPlacing.Name = "cbScalingPlacing";
            this.cbScalingPlacing.Size = new System.Drawing.Size(236, 20);
            this.cbScalingPlacing.TabIndex = 14;
            this.cbScalingPlacing.Text = "Free Scaling and Placing of Entities";
            this.cbScalingPlacing.UseVisualStyleBackColor = true;
            // 
            // cbMapBorder
            // 
            this.cbMapBorder.AutoSize = true;
            this.cbMapBorder.Location = new System.Drawing.Point(6, 21);
            this.cbMapBorder.Name = "cbMapBorder";
            this.cbMapBorder.Size = new System.Drawing.Size(181, 20);
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
            this.gbEditor.Enabled = false;
            this.gbEditor.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbEditor.Location = new System.Drawing.Point(12, 382);
            this.gbEditor.Name = "gbEditor";
            this.gbEditor.Size = new System.Drawing.Size(642, 125);
            this.gbEditor.TabIndex = 16;
            this.gbEditor.TabStop = false;
            this.gbEditor.Text = "Mapeditor";
            // 
            // gbHE
            // 
            this.gbHE.Controls.Add(this.cbAutosave);
            this.gbHE.Controls.Add(this.txtAutosave);
            this.gbHE.Controls.Add(this.lblAutosave);
            this.gbHE.Enabled = false;
            this.gbHE.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbHE.Location = new System.Drawing.Point(12, 327);
            this.gbHE.Name = "gbHE";
            this.gbHE.Size = new System.Drawing.Size(642, 49);
            this.gbHE.TabIndex = 17;
            this.gbHE.TabStop = false;
            this.gbHE.Text = "History Edition";
            // 
            // gbAll
            // 
            this.gbAll.Controls.Add(this.gbUserscriptOptions);
            this.gbAll.Controls.Add(this.cbLimitedEdition);
            this.gbAll.Controls.Add(this.cbKnightSelection);
            this.gbAll.Controls.Add(this.cbScriptBugFixes);
            this.gbAll.Controls.Add(this.lblTextureRes);
            this.gbAll.Controls.Add(this.txtResolution);
            this.gbAll.Controls.Add(this.cbHighTextures);
            this.gbAll.Controls.Add(this.cbLAAFlag);
            this.gbAll.Controls.Add(this.cbZoom);
            this.gbAll.Controls.Add(this.txtZoom);
            this.gbAll.Controls.Add(this.cbDevMode);
            this.gbAll.Controls.Add(this.lblZoomAngle);
            this.gbAll.Enabled = false;
            this.gbAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbAll.Location = new System.Drawing.Point(12, 42);
            this.gbAll.Name = "gbAll";
            this.gbAll.Size = new System.Drawing.Size(642, 203);
            this.gbAll.TabIndex = 18;
            this.gbAll.TabStop = false;
            this.gbAll.Text = "General Options";
            // 
            // gbUserscriptOptions
            // 
            this.gbUserscriptOptions.Controls.Add(this.cbSpecialKnightsAvailable);
            this.gbUserscriptOptions.Controls.Add(this.cbDayNightCycle);
            this.gbUserscriptOptions.Controls.Add(this.cbUseMilitaryRelease);
            this.gbUserscriptOptions.Controls.Add(this.cbUseDowngrade);
            this.gbUserscriptOptions.Controls.Add(this.cbUseSingleStop);
            this.gbUserscriptOptions.Enabled = false;
            this.gbUserscriptOptions.Location = new System.Drawing.Point(436, 44);
            this.gbUserscriptOptions.Name = "gbUserscriptOptions";
            this.gbUserscriptOptions.Size = new System.Drawing.Size(200, 153);
            this.gbUserscriptOptions.TabIndex = 19;
            this.gbUserscriptOptions.TabStop = false;
            this.gbUserscriptOptions.Text = "Gameplay Options";
            // 
            // cbSpecialKnightsAvailable
            // 
            this.cbSpecialKnightsAvailable.AutoSize = true;
            this.cbSpecialKnightsAvailable.Enabled = false;
            this.cbSpecialKnightsAvailable.Location = new System.Drawing.Point(6, 125);
            this.cbSpecialKnightsAvailable.Name = "cbSpecialKnightsAvailable";
            this.cbSpecialKnightsAvailable.Size = new System.Drawing.Size(164, 20);
            this.cbSpecialKnightsAvailable.TabIndex = 4;
            this.cbSpecialKnightsAvailable.Text = "Enable Special Knights";
            this.cbSpecialKnightsAvailable.UseVisualStyleBackColor = true;
            // 
            // cbDayNightCycle
            // 
            this.cbDayNightCycle.AutoSize = true;
            this.cbDayNightCycle.Location = new System.Drawing.Point(6, 99);
            this.cbDayNightCycle.Name = "cbDayNightCycle";
            this.cbDayNightCycle.Size = new System.Drawing.Size(123, 20);
            this.cbDayNightCycle.TabIndex = 3;
            this.cbDayNightCycle.Text = "Day/Night Cycle";
            this.cbDayNightCycle.UseVisualStyleBackColor = true;
            // 
            // cbUseMilitaryRelease
            // 
            this.cbUseMilitaryRelease.AutoSize = true;
            this.cbUseMilitaryRelease.Location = new System.Drawing.Point(6, 73);
            this.cbUseMilitaryRelease.Name = "cbUseMilitaryRelease";
            this.cbUseMilitaryRelease.Size = new System.Drawing.Size(163, 20);
            this.cbUseMilitaryRelease.TabIndex = 2;
            this.cbUseMilitaryRelease.Text = "Military Release Button";
            this.cbUseMilitaryRelease.UseVisualStyleBackColor = true;
            // 
            // cbUseDowngrade
            // 
            this.cbUseDowngrade.AutoSize = true;
            this.cbUseDowngrade.Location = new System.Drawing.Point(6, 47);
            this.cbUseDowngrade.Name = "cbUseDowngrade";
            this.cbUseDowngrade.Size = new System.Drawing.Size(136, 20);
            this.cbUseDowngrade.TabIndex = 1;
            this.cbUseDowngrade.Text = "Downgrade Button";
            this.cbUseDowngrade.UseVisualStyleBackColor = true;
            // 
            // cbUseSingleStop
            // 
            this.cbUseSingleStop.AutoSize = true;
            this.cbUseSingleStop.Location = new System.Drawing.Point(6, 21);
            this.cbUseSingleStop.Name = "cbUseSingleStop";
            this.cbUseSingleStop.Size = new System.Drawing.Size(135, 20);
            this.cbUseSingleStop.TabIndex = 0;
            this.cbUseSingleStop.Text = "Single Stop Button";
            this.cbUseSingleStop.UseVisualStyleBackColor = true;
            // 
            // cbLimitedEdition
            // 
            this.cbLimitedEdition.AutoSize = true;
            this.cbLimitedEdition.Location = new System.Drawing.Point(6, 177);
            this.cbLimitedEdition.Name = "cbLimitedEdition";
            this.cbLimitedEdition.Size = new System.Drawing.Size(214, 20);
            this.cbLimitedEdition.TabIndex = 17;
            this.cbLimitedEdition.Text = "Activate Limited/Special Edition";
            this.cbLimitedEdition.UseVisualStyleBackColor = true;
            // 
            // cbKnightSelection
            // 
            this.cbKnightSelection.AutoSize = true;
            this.cbKnightSelection.Enabled = false;
            this.cbKnightSelection.Location = new System.Drawing.Point(6, 151);
            this.cbKnightSelection.Name = "cbKnightSelection";
            this.cbKnightSelection.Size = new System.Drawing.Size(292, 20);
            this.cbKnightSelection.TabIndex = 16;
            this.cbKnightSelection.Text = "Enable base game Knights in Eastern Realm";
            this.cbKnightSelection.UseVisualStyleBackColor = true;
            this.cbKnightSelection.CheckedChanged += new System.EventHandler(this.cbKnightSelection_CheckedChanged);
            // 
            // cbScriptBugFixes
            // 
            this.cbScriptBugFixes.AutoSize = true;
            this.cbScriptBugFixes.Location = new System.Drawing.Point(6, 125);
            this.cbScriptBugFixes.Name = "cbScriptBugFixes";
            this.cbScriptBugFixes.Size = new System.Drawing.Size(227, 20);
            this.cbScriptBugFixes.TabIndex = 15;
            this.cbScriptBugFixes.Text = "Activate Script and Code Bugfixes";
            this.cbScriptBugFixes.UseVisualStyleBackColor = true;
            this.cbScriptBugFixes.CheckedChanged += new System.EventHandler(this.cbScriptBugFixes_CheckedChanged);
            // 
            // lblTextureRes
            // 
            this.lblTextureRes.AutoSize = true;
            this.lblTextureRes.Location = new System.Drawing.Point(346, 22);
            this.lblTextureRes.Name = "lblTextureRes";
            this.lblTextureRes.Size = new System.Drawing.Size(76, 16);
            this.lblTextureRes.TabIndex = 14;
            this.lblTextureRes.Text = "Default: 512";
            // 
            // txtResolution
            // 
            this.txtResolution.Location = new System.Drawing.Point(198, 19);
            this.txtResolution.MaxLength = 4;
            this.txtResolution.Name = "txtResolution";
            this.txtResolution.Size = new System.Drawing.Size(142, 22);
            this.txtResolution.TabIndex = 13;
            // 
            // cbBugfixMod
            // 
            this.cbBugfixMod.AutoSize = true;
            this.cbBugfixMod.Enabled = false;
            this.cbBugfixMod.Location = new System.Drawing.Point(6, 42);
            this.cbBugfixMod.Name = "cbBugfixMod";
            this.cbBugfixMod.Size = new System.Drawing.Size(219, 20);
            this.cbBugfixMod.TabIndex = 20;
            this.cbBugfixMod.Text = "Download and Install Bugfix Mod";
            this.cbBugfixMod.UseVisualStyleBackColor = true;
            // 
            // cbModloader
            // 
            this.cbModloader.AutoSize = true;
            this.cbModloader.Enabled = false;
            this.cbModloader.Location = new System.Drawing.Point(6, 19);
            this.cbModloader.Name = "cbModloader";
            this.cbModloader.Size = new System.Drawing.Size(143, 20);
            this.cbModloader.TabIndex = 18;
            this.cbModloader.Text = "Activate Modloader";
            this.cbModloader.UseVisualStyleBackColor = true;
            this.cbModloader.CheckedChanged += new System.EventHandler(this.cbModloader_CheckedChanged);
            // 
            // btnBackup
            // 
            this.btnBackup.Enabled = false;
            this.btnBackup.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBackup.Location = new System.Drawing.Point(12, 566);
            this.btnBackup.Name = "btnBackup";
            this.btnBackup.Size = new System.Drawing.Size(642, 47);
            this.btnBackup.TabIndex = 19;
            this.btnBackup.Text = "Restore Backup";
            this.btnBackup.UseVisualStyleBackColor = true;
            this.btnBackup.Click += new System.EventHandler(this.btnBackup_Click);
            // 
            // btnChooseFile
            // 
            this.btnChooseFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnChooseFile.Location = new System.Drawing.Point(548, 6);
            this.btnChooseFile.Name = "btnChooseFile";
            this.btnChooseFile.Size = new System.Drawing.Size(106, 28);
            this.btnChooseFile.TabIndex = 20;
            this.btnChooseFile.Text = "Choose File ...";
            this.btnChooseFile.UseVisualStyleBackColor = true;
            this.btnChooseFile.Click += new System.EventHandler(this.btnChooseFile_Click);
            // 
            // txtExecutablePath
            // 
            this.txtExecutablePath.Location = new System.Drawing.Point(136, 10);
            this.txtExecutablePath.Name = "txtExecutablePath";
            this.txtExecutablePath.ReadOnly = true;
            this.txtExecutablePath.Size = new System.Drawing.Size(406, 20);
            this.txtExecutablePath.TabIndex = 21;
            // 
            // lblSelectFile
            // 
            this.lblSelectFile.AutoSize = true;
            this.lblSelectFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSelectFile.Location = new System.Drawing.Point(12, 11);
            this.lblSelectFile.Name = "lblSelectFile";
            this.lblSelectFile.Size = new System.Drawing.Size(117, 16);
            this.lblSelectFile.TabIndex = 22;
            this.lblSelectFile.Text = "Select executable:";
            // 
            // gbModloader
            // 
            this.gbModloader.Controls.Add(this.cbBugfixMod);
            this.gbModloader.Controls.Add(this.cbModloader);
            this.gbModloader.Enabled = false;
            this.gbModloader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F);
            this.gbModloader.Location = new System.Drawing.Point(12, 251);
            this.gbModloader.Name = "gbModloader";
            this.gbModloader.Size = new System.Drawing.Size(642, 70);
            this.gbModloader.TabIndex = 23;
            this.gbModloader.TabStop = false;
            this.gbModloader.Text = "Modloader";
            // 
            // pbProgress
            // 
            this.pbProgress.Location = new System.Drawing.Point(12, 672);
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.Size = new System.Drawing.Size(642, 47);
            this.pbProgress.TabIndex = 24;
            // 
            // mainFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(665, 731);
            this.Controls.Add(this.pbProgress);
            this.Controls.Add(this.gbModloader);
            this.Controls.Add(this.lblSelectFile);
            this.Controls.Add(this.txtExecutablePath);
            this.Controls.Add(this.btnChooseFile);
            this.Controls.Add(this.btnBackup);
            this.Controls.Add(this.gbAll);
            this.Controls.Add(this.gbHE);
            this.Controls.Add(this.gbEditor);
            this.Controls.Add(this.btnAbort);
            this.Controls.Add(this.btnPatch);
            this.DoubleBuffered = true;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(681, 770);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(681, 770);
            this.Name = "mainFrm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "-";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.mainFrm_HelpButtonClicked);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.mainFrm_FormClosing);
            this.Load += new System.EventHandler(this.mainFrm_Load);
            this.gbEditor.ResumeLayout(false);
            this.gbEditor.PerformLayout();
            this.gbHE.ResumeLayout(false);
            this.gbHE.PerformLayout();
            this.gbAll.ResumeLayout(false);
            this.gbAll.PerformLayout();
            this.gbUserscriptOptions.ResumeLayout(false);
            this.gbUserscriptOptions.PerformLayout();
            this.gbModloader.ResumeLayout(false);
            this.gbModloader.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private System.Windows.Forms.GroupBox gbUserscriptOptions;
        private System.Windows.Forms.CheckBox cbUseMilitaryRelease;
        private System.Windows.Forms.CheckBox cbUseDowngrade;
        private System.Windows.Forms.CheckBox cbUseSingleStop;
        private System.Windows.Forms.CheckBox cbDayNightCycle;
        private System.Windows.Forms.CheckBox cbSpecialKnightsAvailable;
        private System.Windows.Forms.CheckBox cbBugfixMod;
        private System.Windows.Forms.GroupBox gbModloader;
        private System.Windows.Forms.ProgressBar pbProgress;
    }
}