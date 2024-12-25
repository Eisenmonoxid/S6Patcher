using S6Patcher.Properties;
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using static S6Patcher.Helpers;

namespace S6Patcher
{
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
            StartPatching(execID.ED);
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
            OpenFileDialog ofd = IOFileHandler.Instance.CreateOFDialog();
            if (ofd.ShowDialog() == DialogResult.OK && File.Exists(ofd.FileName))
            {
                if (IOFileHandler.Instance.CreateBackup(ofd.FileName) == false)
                {
                    MessageBox.Show(Resources.ErrorBackup, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                FileStream Stream = IOFileHandler.Instance.OpenFileStream(ofd.FileName, ID);
                if (Stream == null) {
                    MessageBox.Show(Resources.ErrorWrongVersion, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (IsValidExecutable(ref Stream, ID) == false)
                {
                    Stream.Close();
                    Stream.Dispose();
                    MessageBox.Show(Resources.ErrorWrongVersion, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                mainPatcher PatchHandler = new mainPatcher(ID, ref Stream);
                PatchHandler.ShowDialog();

                if (Stream.CanRead == true) // Was Stream already closed?
                {
                    Stream.Close();
                    Stream.Dispose();
                }

                HandlePatcherReturnValue(PatchHandler.DialogResult);
            }
            else
            {
                MessageBox.Show(Resources.ErrorNoFile, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void HandlePatcherReturnValue(DialogResult Result)
        {
            if (Result == DialogResult.OK)
            {
                MessageBox.Show(Resources.FinishedSuccess, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (Result == DialogResult.Cancel)
            {
                MessageBox.Show(Resources.ErrorBackupFail, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (Result == DialogResult.Retry)
            {
                MessageBox.Show(Resources.FinishedBackup, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (Result == DialogResult.Abort)
            {
                MessageBox.Show(Resources.AbortedMessage, "Abort", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        public bool IsValidExecutable(ref FileStream Reader, execID Identifier, Int64 Offset = 0x0)
        {
            string ExpectedVersion = "1, 71, 4289, 0";
            UInt32[] Mapping = {0x6ECADC, 0xF531A4, 0x6D06A8};
            byte[] Result = new byte[30];

            Reader.Position = (Mapping[(char)Identifier] - Offset);
            Reader.Read(Result, 0, 30);

            string Version = Encoding.Unicode.GetString(Result).Substring(0, ExpectedVersion.Length);
            if (Version == ExpectedVersion)
            {
                IsSteamOV = false;
                IsSteamHE = false;
                return true;
            }
            else if (Offset == 0x0 && Identifier == execID.OV) // Try again with offset applied
            {
                if (IsValidExecutable(ref Reader, Identifier, 0x3F0000) == true)
                {
                    IsSteamOV = true;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (Offset == 0x0 && Identifier == execID.HE) // Check for Steam HE
            {
                if (IsValidExecutable(ref Reader, Identifier, -0x1400) == true)
                {
                    IsSteamHE = true;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                MessageBox.Show("Erwartet/Expected: " + ExpectedVersion + "\nGelesen/Read: " + Version.ToString(), "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
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