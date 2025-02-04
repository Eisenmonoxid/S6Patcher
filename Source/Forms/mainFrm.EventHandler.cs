using S6Patcher.Properties;
using S6Patcher.Source.Helpers;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace S6Patcher.Source.Forms
{
    public partial class mainFrm
    {
        private void mainFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseFileStream();
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
            catch {}
        }
        private void cbZoom_CheckedChanged(object sender, EventArgs e)
        {
            if (cbZoom.Checked)
            {
                txtZoom.Enabled = true;
            }
            else
            {
                txtZoom.Enabled = false;
            }
        }
        private void cbAutosave_CheckedChanged(object sender, EventArgs e)
        {
            if (cbAutosave.Checked)
            {
                txtAutosave.Enabled = true;
            }
            else
            {
                txtAutosave.Enabled = false;
            }
        }
        private void cbHighTextures_CheckedChanged(object sender, EventArgs e)
        {
            if (cbHighTextures.Checked && Helpers.Helpers.CurrentID != execID.ED)
            {
                txtResolution.Enabled = true;
            }
            else
            {
                txtResolution.Enabled = false;
            }
        }
        private void cbScriptBugFixes_CheckedChanged(object sender, EventArgs e)
        {
            if (cbScriptBugFixes.Checked)
            {
                cbKnightSelection.Enabled = true;
                gbUserscriptOptions.Enabled = true;
            }
            else
            {
                cbKnightSelection.Checked = false;
                cbKnightSelection.Enabled = false;
                gbUserscriptOptions.Enabled = false;

                gbUserscriptOptions.Controls.OfType<CheckBox>()
                    .Select(Element => {Element.Checked = false; return Element;})
                    .ToArray();
            }
        }
        private void btnPatch_Click(object sender, EventArgs e)
        {
            Logger.Instance.Log("btnPatch_Click(): Going to patch file ...");

            string Name = GlobalStream.Name;
            SelectPatchFeatures(); // execute patching
            ResetForm();

            Logger.Instance.Log("btnPatch_Click(): Finished patching file ...");

            DialogResult Result;
            Result = MessageBox.Show(Resources.FinishedSuccess, "Success", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (Result == DialogResult.Yes)
            {
                Helpers.Helpers.CreateDesktopShortcut(Name);
            }         
        }
        private void mainFrm_HelpButtonClicked(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Form Box = new aboutBox();
            Box.ShowDialog();
            e.Cancel = true;
        }
        private void btnAbort_Click(object sender, EventArgs e)
        {
            Logger.Instance.Log("btnAbort_Click(): Exiting application ...");
            CloseFileStream();
            Environment.Exit(0);
        }
        private void btnBackup_Click(object sender, EventArgs e)
        {
            Logger.Instance.Log("btnBackup_Click(): Restoring backup ...");
            bool Result = IOFileHandler.Instance.RestoreBackup(GlobalStream, GlobalOptions);
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
            OpenFileDialog ofd = IOFileHandler.Instance.CreateOFDialog();
            if (ofd.ShowDialog() == DialogResult.OK && File.Exists(ofd.FileName))
            {
                string FileName = Helpers.Helpers.IsPlayLauncherExecutable(ofd.FileName);
                if (IOFileHandler.Instance.CreateBackup(FileName) == false)
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

                bool ValidExecutable = Helpers.Helpers.SetCurrentExecutableID(GlobalStream);
                if (ValidExecutable == false)
                {
                    CloseFileStream();
                    Logger.Instance.Log(Resources.ErrorInvalidExecutable);
                    MessageBox.Show(Resources.ErrorInvalidExecutable, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // File is valid
                txtExecutablePath.Text = GlobalStream.Name;
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
