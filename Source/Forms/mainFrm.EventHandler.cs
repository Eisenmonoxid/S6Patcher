using S6Patcher.Properties;
using S6Patcher.Source.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using static S6Patcher.Source.Helpers.Helpers;

namespace S6Patcher.Source.Forms
{
    public partial class mainFrm
    {
        private void mainFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseFileStream(GlobalStream);
        }
        private void mainFrm_Load(object sender, EventArgs e)
        {
            try
            {
                using (Stream IconStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("S6Patcher.Source.Resources.favicon.ico"))
                {
                    this.Icon = new System.Drawing.Icon(IconStream);
                    this.ShowIcon = true;
                }
            }
            finally
            {
                Logger.Instance.Log("mainFrm_Load(): Form loaded successfully.");
            }
        }

        private void cbKnightSelection_CheckedChanged(object sender, EventArgs e)
        {
            /*
            cbSpecialKnightsAvailable.Enabled = cbKnightSelection.Checked;
            if (!cbKnightSelection.Checked)
            {
                cbSpecialKnightsAvailable.Checked = false;
            }
            */
            return;
        }
        private void cbModloader_CheckedChanged(object sender, EventArgs e)
        {
            cbBugfixMod.Enabled = cbModloader.Checked;
            if (!cbModloader.Checked)
            {
                cbBugfixMod.Enabled = false;
            }
        }
        private void cbZoom_CheckedChanged(object sender, EventArgs e)
        {
            txtZoom.Enabled = cbZoom.Checked;
        }
        private void cbAutosave_CheckedChanged(object sender, EventArgs e)
        {
            txtAutosave.Enabled = cbAutosave.Checked;
        }
        private void cbHighTextures_CheckedChanged(object sender, EventArgs e)
        {
            txtResolution.Enabled = cbHighTextures.Checked && (Validator.ID != execID.ED);
        }
        private void cbScriptBugFixes_CheckedChanged(object sender, EventArgs e)
        {
            cbKnightSelection.Enabled = cbScriptBugFixes.Checked;
            gbUserscriptOptions.Enabled = cbScriptBugFixes.Checked;

            if (!cbScriptBugFixes.Checked)
            {
                cbKnightSelection.Checked = false;
                gbUserscriptOptions.Controls.OfType<CheckBox>()
                    .Select(Element => {Element.Checked = false; return Element;})
                    .ToArray();
            }
        }

        private void btnPatch_Click(object sender, EventArgs e)
        {
            Enabled = false;
            pbProgress.Style = ProgressBarStyle.Marquee;
            pbProgress.MarqueeAnimationSpeed = 60;
            Logger.Instance.Log("btnPatch_Click(): Going to patch file ...");

            List<string> Features = GetPatchFeaturesByControls(new List<GroupBox> {gbAll, gbModloader, gbHE, gbEditor});
            Thread Worker = new Thread(() => ExecutePatchWrapper(Features, GlobalStream.Name, GlobalStream.Length)) // Main patching logic is executed here
            {
                IsBackground = true
            };
            Worker.Start();
        }

        private void mainFrm_HelpButtonClicked(object sender, System.ComponentModel.CancelEventArgs e)
        {
            new aboutBox().ShowDialog();
            e.Cancel = true;
        }

        private void btnAbort_Click(object sender, EventArgs e)
        {
            Logger.Instance.Log("btnAbort_Click(): Exiting application ...");
            CloseFileStream(GlobalStream);
            Environment.Exit(0);
        }

        private void btnBackup_Click(object sender, EventArgs e)
        {
            Logger.Instance.Log("btnBackup_Click(): Restoring backup ...");
            bool Result = Backup.RestoreBackup(GlobalStream, GlobalOptions);
            ResetForm();

            if (Result == false)
            {
                Logger.Instance.Log(Resources.ErrorBackupFail);
                MessageBox.Show(Resources.ErrorBackupFail, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                Logger.Instance.Log(Resources.FinishedBackup);
                MessageBox.Show(Resources.FinishedBackup, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnChooseFile_Click(object sender, EventArgs e)
        {
            ResetForm();
            OpenFileDialog ofd = IOFileHandler.Instance.CreateOFDialog("Executable file|*.exe", Environment.SpecialFolder.ProgramFiles);
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string FileName = IsPlayLauncherExecutable(ofd.FileName);
                if (Backup.CreateBackup(FileName) == false)
                {
                    Logger.Instance.Log(Resources.ErrorBackup);
                    MessageBox.Show(Resources.ErrorBackup, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                GlobalStream = IOFileHandler.Instance.OpenFileStream(FileName);
                if (GlobalStream == null)
                {
                    Logger.Instance.Log(Resources.ErrorInvalidExecutable);
                    MessageBox.Show(Resources.ErrorInvalidExecutable, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                Validator = new Patcher.Validator(GlobalStream);
                if (Validator.ID == execID.NONE || Validator.IsExecutableUnpacked == false)
                {
                    CloseFileStream(GlobalStream);
                    string Error = Validator.IsExecutableUnpacked ? Resources.ErrorInvalidExecutable : Resources.ErrorInvalidExecutableSteam;
                    Logger.Instance.Log(Error);
                    MessageBox.Show(Error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // File is valid
                IOFileHandler.Instance.InitialDirectory = Path.GetDirectoryName(GlobalStream.Name);
                try
                {
                    Patcher = new Patcher.Patcher(Validator.ID, GlobalStream);
                }
                catch (Exception ex)
                {
                    CloseFileStream(GlobalStream);
                    Logger.Instance.Log(ex.ToString());
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Everything has been initialized successfully
                InitializeControls();
            }
            else
            {
                Logger.Instance.Log(Resources.ErrorNoFile);
                MessageBox.Show(Resources.ErrorNoFile, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
