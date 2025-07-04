using S6Patcher.Properties;
using S6Patcher.Source.Helpers;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using static S6Patcher.Source.Helpers.Helpers;

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
            finally
            {
                Logger.Instance.Log("mainFrm_Load(): Form loaded successfully.");
            }
        }

        private void cbKnightSelection_CheckedChanged(object sender, EventArgs e)
        {
            cbSpecialKnightsAvailable.Enabled = cbKnightSelection.Checked;
            if (!cbKnightSelection.Checked)
            {
                cbSpecialKnightsAvailable.Checked = false;
            }
        }
        private void cbModloader_CheckedChanged(object sender, EventArgs e)
        {
            btnBugfixMod.Enabled = cbModloader.Checked;
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
            txtResolution.Enabled = cbHighTextures.Checked && (CurrentID != execID.ED);
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

        private void btnBugfixMod_Click(object sender, EventArgs e)
        {
            // Start Thread, download and install mod files
            // Patcher.Mod Mod = new Patcher.Mod(String.Empty, String.Empty);
            return;
        }

        private void btnPatch_Click(object sender, EventArgs e)
        {
            Logger.Instance.Log("btnPatch_Click(): Going to patch file ...");

            string Name = GlobalStream.Name;
            SelectPatchFeatures(); // execute patching
            ResetForm();

            Logger.Instance.Log("btnPatch_Click(): Finished patching file ...");

            if (Program.IsMono)
            {
                Logger.Instance.Log("btnPatch_Click(): MONO found! Returning without desktop shortcut.");
                MessageBox.Show(Resources.FinishedMono, "Finished", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult Result;
            Result = MessageBox.Show(Resources.FinishedSuccess, "Finished", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (Result == DialogResult.Yes)
            {
                if (CurrentID == execID.HE_UBISOFT || CurrentID == execID.HE_STEAM && !Name.Contains("extra1"))
                {
                    CreateDesktopShortcut(Name, Path.GetFileNameWithoutExtension(Name), String.Empty);
                    CreateDesktopShortcut(Name, Path.GetFileNameWithoutExtension(Name) + " - The Eastern Realm", "-extra1");
                }
                else
                {
                    if (Name.Contains("extra1"))
                    {
                        CreateDesktopShortcut(Name, Path.GetFileNameWithoutExtension(Name) + " - The Eastern Realm", "-extra1");
                    }
                    else
                    {
                        CreateDesktopShortcut(Name, Path.GetFileNameWithoutExtension(Name), String.Empty);
                    }
                }
            }
        }

        private void mainFrm_HelpButtonClicked(object sender, System.ComponentModel.CancelEventArgs e)
        {
            new aboutBox().ShowDialog();
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
            OpenFileDialog ofd = IOFileHandler.Instance.CreateOFDialog("Executable file|*.exe", Environment.SpecialFolder.ProgramFiles);
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string FileName = IsPlayLauncherExecutable(ofd.FileName);
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

                uint ValidExecutable = SetCurrentExecutableID(GlobalStream);
                if (ValidExecutable != 0)
                {
                    CloseFileStream();
                    string Error = (ValidExecutable == 1) ? Resources.ErrorInvalidExecutable : Resources.ErrorInvalidExecutableSteam;
                    Logger.Instance.Log(Error);
                    MessageBox.Show(Error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
