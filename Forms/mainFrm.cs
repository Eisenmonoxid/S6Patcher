using S6Patcher.Properties;
using System;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

namespace S6Patcher
{
    public enum execID
    {
        OV = 0,
        HE = 1,
        Editor = 2,
    }
    public partial class mainFrm : Form
    {
        public mainFrm()
        {
            InitializeComponent();
            SetTooltipSystemLanguage();

            this.Text = "S6Patcher - v" + Application.ProductVersion.Substring(0, 3) + " - \"https://github.com/Eisenmonoxid/S6Patcher\"";
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
        private void btnPatch_Click(object sender, EventArgs e)
        {
            StartPatching(execID.Editor);
        }
        private void btnPatchHE_Click(object sender, EventArgs e)
        {
            StartPatching(execID.HE);
        }
        private void btnPatchOV_Click(object sender, EventArgs e)
        {
            StartPatching(execID.OV);
        }
        private void StartPatching(execID ID)
        {
            OpenFileDialog ofd = Helpers.CreateOFDialog();
            if (ofd.ShowDialog() == DialogResult.OK && File.Exists(ofd.FileName))
            {
                if (Helpers.CreateBackup(ofd.FileName) == false)
                {
                    MessageBox.Show(Resources.ErrorBackup, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                FileStream Reader = Helpers.OpenFileStream(ofd.FileName, ID);
                if (Reader == null) {
                    MessageBox.Show(Resources.ErrorWrongVersion, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                mainPatcher PatchHandler = new mainPatcher(ID, ref Reader);
                PatchHandler.ShowDialog();

                Reader.Close();
                Reader.Dispose();

                if (PatchHandler.DialogResult == DialogResult.OK)
                {
                    MessageBox.Show(Resources.FinishedSuccess, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (PatchHandler.DialogResult == DialogResult.Cancel)
                {
                    MessageBox.Show(Resources.ErrorBackupFail, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show(Resources.ErrorNoFile, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            ofd.Dispose();
        }
        private void SetTooltipSystemLanguage()
        {
            string Language = CultureInfo.InstalledUICulture.TwoLetterISOLanguageName;
            ToolTip btnPatchTooltip = new ToolTip{ShowAlways = true, ToolTipIcon = ToolTipIcon.None, InitialDelay = 50, ReshowDelay = 100, AutoPopDelay = 32000},
            btnPatchOVTooltip = new ToolTip{ShowAlways = true, ToolTipIcon = ToolTipIcon.None, InitialDelay = 50, ReshowDelay = 100, AutoPopDelay = 32000},
            btnPatchHETooltip = new ToolTip{ShowAlways = true, ToolTipIcon = ToolTipIcon.None, InitialDelay = 50, ReshowDelay = 100, AutoPopDelay = 32000};

            if (Language == "de") // German
            {
                btnPatchTooltip.SetToolTip(btnPatch, Resources.btnPatch_GermanText);
                btnPatchHETooltip.SetToolTip(btnPatchHE, Resources.btnPatchHE_GermanText);
                btnPatchOVTooltip.SetToolTip(btnPatchOV, Resources.btnPatchOV_GermanText);

                btnPatchTooltip.ToolTipTitle = Resources.btnGermanTitle;
                btnPatchHETooltip.ToolTipTitle = Resources.btnGermanTitle;
                btnPatchOVTooltip.ToolTipTitle= Resources.btnGermanTitle;

                btnClose.Text = "Beenden";
            }
            else // English
            {
                btnPatchTooltip.SetToolTip(btnPatch, Resources.btnPatch_EnglishText);
                btnPatchHETooltip.SetToolTip(btnPatchHE, Resources.btnPatchHE_EnglishText);
                btnPatchOVTooltip.SetToolTip(btnPatchOV, Resources.btnPatchOV_EnglishText);

                btnPatchTooltip.ToolTipTitle = Resources.btnEnglishTitle;
                btnPatchHETooltip.ToolTipTitle = Resources.btnEnglishTitle;
                btnPatchOVTooltip.ToolTipTitle = Resources.btnEnglishTitle;

                btnClose.Text = "Exit";
            }
        }
    }
}