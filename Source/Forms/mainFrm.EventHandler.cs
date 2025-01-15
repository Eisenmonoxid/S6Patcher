using S6Patcher.Properties;
using S6Patcher.Source.Forms;
using System;
using System.IO;
using System.Windows.Forms;

namespace S6Patcher
{
    public partial class mainFrm
    {
        private void mainFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseFileStream();
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
            if (cbHighTextures.Checked && Helpers.CurrentID != execID.ED)
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
            }
            else
            {
                cbKnightSelection.Checked = false;
                cbKnightSelection.Enabled = false;
            }
        }
        private void btnPatch_Click(object sender, EventArgs e)
        {
            SelectPatchFeatures();
            MessageBox.Show(Resources.FinishedSuccess, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ResetForm();
        }
        private void mainFrm_HelpButtonClicked(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Form Box = new aboutBox();
            Box.ShowDialog();
            e.Cancel = true;
        }
        private void btnAbort_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
        private void btnBackup_Click(object sender, EventArgs e)
        {
            bool Result = IOFileHandler.Instance.RestoreBackup(ref GlobalStream);
            if (Result == false)
            {
                MessageBox.Show(Resources.ErrorBackupFail, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(Resources.FinishedBackup, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            ResetForm();
        }
        private void btnChooseFile_Click(object sender, EventArgs e)
        {
            ResetForm();
            OpenFileDialog ofd = IOFileHandler.Instance.CreateOFDialog();
            if (ofd.ShowDialog() == DialogResult.OK && File.Exists(ofd.FileName))
            {
                if (IOFileHandler.Instance.CreateBackup(ofd.FileName) == false)
                {
                    MessageBox.Show(Resources.ErrorBackup, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                GlobalStream = IOFileHandler.Instance.OpenFileStream(ofd.FileName);
                if (GlobalStream == null)
                {
                    MessageBox.Show(Resources.ErrorWrongVersion, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                bool ValidExecutable = Helpers.SetCurrentExecutableID(ref GlobalStream);
                if (ValidExecutable == false)
                {
                    CloseFileStream();
                    MessageBox.Show(Resources.ErrorWrongVersion, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // File is valid
                txtExecutablePath.Text = GlobalStream.Name;
                InitializeControls();
            }
            else
            {
                MessageBox.Show(Resources.ErrorNoFile, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
