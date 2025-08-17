using S6Patcher.Properties;
using S6Patcher.Source.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace S6Patcher.Source.Forms
{
    public partial class mainFrm : Form
    {
        private Patcher.Validator Validator;
        private Patcher.Patcher Patcher;

        private FileStream GlobalStream = null;
        private readonly string[] GlobalOptions = {"ExtendedKnightSelection", "UseSingleStop", "UseDowngrade", 
            "UseMilitaryRelease", "DayNightCycle", "SpecialKnightsAvailable"};

        public mainFrm()
        {
            InitializeComponent();
            this.Text = "S6Patcher - v" + Application.ProductVersion;
            Logger.Instance.Log("Startup successful! " + this.Text + " - Mono: " + Program.IsMono.ToString());
            WebHandler.Instance.CheckForUpdates(true);
        }

        private void SetControlValueFromStream(BinaryReader Reader, long Position, TextBox Control)
        {
            Reader.BaseStream.Position = Position;
            if (Control == txtResolution)
            {
                txtResolution.Text = Reader.ReadUInt32().ToString();
            }
            else if (Control == txtZoom)
            {
                if (Validator.ID == execID.HE_UBISOFT || Validator.ID == execID.HE_STEAM)
                {
                    txtZoom.Text = Reader.ReadSingle().ToString();
                }
                else
                {
                    txtZoom.Text = Reader.ReadDouble().ToString();
                }
            }
            else if (Control == txtAutosave)
            {
                txtAutosave.Text = (Reader.ReadDouble() / 60000).ToString();
            }
        }

        private void InitializeControls()
        {
            // Validator & Patcher have been successfully initialized
            BinaryReader Reader = new BinaryReader(GlobalStream);
            switch (Validator.ID)
            {
                case execID.OV:
                    SetControlValueFromStream(Reader, 0x545400, txtZoom);
                    SetControlValueFromStream(Reader, 0x2BE177, txtResolution);
                    break;
                case execID.OV_OFFSET:
                    SetControlValueFromStream(Reader, (0x545400 - 0x3F0000), txtZoom);
                    SetControlValueFromStream(Reader, (0x2BE177 - 0x3F0000), txtResolution);
                    break;
                case execID.HE_UBISOFT:
                    SetControlValueFromStream(Reader, 0xC4EC4C, txtZoom);
                    SetControlValueFromStream(Reader, 0x2D4188, txtResolution);
                    SetControlValueFromStream(Reader, 0xEB83C0, txtAutosave);
                    gbHE.Enabled = true;
                    break;
                case execID.HE_STEAM:
                    SetControlValueFromStream(Reader, 0xC4F9EC, txtZoom);
                    SetControlValueFromStream(Reader, 0x2D4D74, txtResolution);
                    SetControlValueFromStream(Reader, 0xEB95C0, txtAutosave);
                    gbHE.Enabled = true;
                    break;
                case execID.ED:
                    gbEditor.Enabled = true;
                    break;
                case execID.NONE:
                    return;
                default:
                    return;
            }

            gbAll.Enabled = true;
            txtResolution.Enabled = false;
            txtZoom.Enabled = false;
            txtAutosave.Enabled = false;
            btnPatch.Enabled = true;
            btnBackup.Enabled = true;
            txtExecutablePath.Text = GlobalStream.Name;
            btnPatch.Focus();

            ToggleEditorControls(Validator.ID != execID.ED);
        }

        private void ToggleEditorControls(bool Enable)
        {
            cbZoom.Enabled = Enable;
            lblZoomAngle.Enabled = Enable;
            lblTextureRes.Enabled = Enable;
            cbLimitedEdition.Enabled = Enable;
            gbModloader.Enabled = Enable;
            cbModloader.Enabled = Enable;
            cbScriptBugFixes.Enabled = Enable;
            cbEasyDebug.Enabled = Enable;

            if (Enable)
            {
                AskForRecommendedSettings();
            }
        }

        private void AskForRecommendedSettings()
        {
            DialogResult Result = MessageBox.Show(Resources.WelcomeMessage, "Preselect Recommended Settings ...", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (Result == DialogResult.Yes)
            {
                cbZoom.Checked = true;
                cbScriptBugFixes.Checked = true;
                cbModloader.Checked = true;
                cbBugfixMod.Checked = true;
                cbHighTextures.Checked = true;
                cbLAAFlag.Checked = true;
                cbKnightSelection.Checked = true;
                cbLimitedEdition.Checked = true;
                cbUseSingleStop.Checked = true;
                cbUseDowngrade.Checked = true;
                cbUseMilitaryRelease.Checked = true;
                cbDayNightCycle.Checked = true;
                cbSpecialKnightsAvailable.Checked = true;

                txtZoom.Text = "12000";
                txtResolution.Text = "4096";

                if (Validator.ID == execID.HE_STEAM || Validator.ID == execID.HE_UBISOFT)
                {
                    cbAutosave.Checked = true;
                    txtAutosave.Text = "30";
                }
            }
        }

        private List<string> GetPatchFeaturesByControls(List<GroupBox> Controls)
        {
            IEnumerable<CheckBox> Boxes = null;
            Controls.ForEach(Box =>
            {
                Boxes = Box.Controls.OfType<CheckBox>()
                        .Where(Element => Element.Checked)
                        .Concat(Boxes ?? Enumerable.Empty<CheckBox>());
            });

            return Boxes.Select(Element => Element.Name).ToList();
        }

        public void FinishPatchingProcess()
        {
            Logger.Instance.Log("btnPatch_Click(): Finished patching file ...");
            string StreamName = GlobalStream.Name;
            long StreamSize = GlobalStream.Length;

            ResetForm(); // Close FileStream and reset form controls
            pbProgress.Style = ProgressBarStyle.Blocks;
            pbProgress.Value = 0;
            Enabled = true;
            Focus();

            if (Program.IsMono)
            {
                MessageBox.Show(Resources.FinishedSuccess, "Finished", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Logger.Instance.Log("btnPatch_Click(): MONO found! Returning without desktop shortcut and PE Header update.");
                return;
            }

            new Patcher.CheckSumCalculator().WritePEHeaderFileCheckSum(StreamName, StreamSize);
            MessageBox.Show(Resources.FinishedSuccess, "Finished", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Logger.Instance.Log("btnPatch_Click(): Updated PEHeaderFileCheckSum! Finished.");
        }

        private void ExecutePatch(List<string> Features)
        {
            bool ModDownload = cbBugfixMod.Checked;
            Patcher.PatchByControlFeatures(Features);
            if (cbZoom.Checked)
            {
                Patcher.SetZoomLevel(txtZoom.Text);
            }
            if (cbHighTextures.Checked && Validator.ID != execID.ED) // Editor has no custom texture resolution
            {
                Patcher.SetHighResolutionTextures(txtResolution.Text);
            }
            if (cbAutosave.Checked)
            {
                Patcher.SetAutosaveTimer(txtAutosave.Text);
            }
            if (cbLAAFlag.Checked)
            {
                Patcher.SetLargeAddressAwareFlag();
            }
            if (cbScriptBugFixes.Checked)
            {
                Patcher.SetLuaScriptBugFixes();
                SetEntriesInOptionsFileByCheckBox();
            }
            if (cbEasyDebug.Checked)
            {
                Patcher.SetEasyDebug();
            }
            if (cbModloader.Checked) // Has to be last because of thread start
            {
                Patcher.SetModLoader(ModDownload);
            }

            // Finish with UI update
            if (!ModDownload)
            {
                FinishPatchingProcess();
            }
        }

        private void SetEntriesInOptionsFileByCheckBox()
        {
            Patcher.SetEntryInOptionsFile(GlobalOptions[0], cbKnightSelection.Checked);
            (from Entry in gbUserscriptOptions.Controls.OfType<CheckBox>()
                from Name in GlobalOptions
                where Name == Entry.Name.Remove(0, "cb".Length)
                select Entry).ToList().ForEach(Element =>
                {
                    Patcher.SetEntryInOptionsFile(Element.Name.Remove(0, "cb".Length), Element.Checked);
                });
        }

        private bool OpenExecutableFile(string FileName)
        {
            FileName = IOFileHandler.Instance.IsPlayLauncherExecutable(FileName);
            if (Backup.CreateBackup(FileName) == false)
            {
                Logger.Instance.Log(Resources.ErrorBackup);
                MessageBox.Show(Resources.ErrorBackup, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            GlobalStream = IOFileHandler.Instance.OpenFileStream(FileName);
            if (GlobalStream == null)
            {
                return false;
            }

            Validator = new Patcher.Validator(GlobalStream);
            if (Validator.ID == execID.NONE || Validator.IsExecutableUnpacked == false)
            {
                IOFileHandler.Instance.CloseFileStream(GlobalStream);
                string Error = Validator.IsExecutableUnpacked ? Resources.ErrorInvalidExecutable : Resources.ErrorInvalidExecutableSteam;
                Logger.Instance.Log(Error);
                MessageBox.Show(Error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            // File is valid
            IOFileHandler.Instance.InitialDirectory = Path.GetDirectoryName(GlobalStream.Name);
            try
            {
                Patcher = new Patcher.Patcher(this, Validator.ID, GlobalStream);
            }
            catch (Exception ex)
            {
                IOFileHandler.Instance.CloseFileStream(GlobalStream);
                Logger.Instance.Log(ex.ToString());
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Everything has been initialized successfully
            return true;
        }

        private void ResetForm()
        {
            IOFileHandler.Instance.CloseFileStream(GlobalStream);
            new List<GroupBox> {gbAll, gbModloader, gbHE, gbEditor}.ForEach(Control =>
            {
                Control.Enabled = false;
                foreach (var Element in Control.Controls)
                {
                    if (Element is CheckBox Box)
                    {
                        Box.Checked = false;
                    }
                    else if (Element is TextBox Text)
                    {
                        Text.Text = string.Empty;
                    }
                }
            });

            btnPatch.Enabled = false;
            btnBackup.Enabled = false;
            txtExecutablePath.Text = string.Empty;
            ActiveControl = null;

            Validator = null;
            Patcher = null;

            Logger.Instance.Log("ResetForm(): Form successfully reset!");
        }
    }
}
