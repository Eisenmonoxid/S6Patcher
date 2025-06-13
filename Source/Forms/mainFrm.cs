using S6Patcher.Properties;
using S6Patcher.Source.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using static S6Patcher.Source.Helpers.Helpers;

namespace S6Patcher.Source.Forms
{
    public partial class mainFrm : Form
    {
        private FileStream GlobalStream = null;
        private readonly string[] GlobalOptions = {"ExtendedKnightSelection", "UseSingleStop", "UseDowngrade", "UseMilitaryRelease", "DayNightCycle"};
        public mainFrm()
        {
            InitializeComponent();
            this.Text = "S6Patcher - v" + Application.ProductVersion;
            Logger.Instance.Log("Startup successful! Text: " + this.Text);
        }
        private void SetControlValueFromStream(ref BinaryReader Reader, long Position, TextBox Control)
        {
            Reader.BaseStream.Position = Position;
            if (Control == txtResolution)
            {
                txtResolution.Text = Reader.ReadUInt32().ToString();
            }
            else if (Control == txtZoom)
            {
                if (CurrentID == execID.HE_UBISOFT || CurrentID == execID.HE_STEAM)
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
            BinaryReader Reader = new BinaryReader(GlobalStream);
            switch (CurrentID)
            {
                case execID.OV:
                    SetControlValueFromStream(ref Reader, 0x545400, txtZoom);
                    SetControlValueFromStream(ref Reader, 0x2BE177, txtResolution);
                    break;
                case execID.OV_OFFSET:
                    SetControlValueFromStream(ref Reader, 0x545400 - GlobalOffset, txtZoom);
                    SetControlValueFromStream(ref Reader, 0x2BE177 - GlobalOffset, txtResolution);
                    break;
                case execID.HE_UBISOFT:
                    SetControlValueFromStream(ref Reader, 0xC4EC4C, txtZoom);
                    SetControlValueFromStream(ref Reader, 0x2D4188, txtResolution);
                    SetControlValueFromStream(ref Reader, 0xEB83C0, txtAutosave);
                    gbHE.Enabled = true;
                    break;
                case execID.HE_STEAM:
                    SetControlValueFromStream(ref Reader, 0xC4F9EC, txtZoom);
                    SetControlValueFromStream(ref Reader, 0x2D4D74, txtResolution);
                    SetControlValueFromStream(ref Reader, 0xEB95C0, txtAutosave);
                    gbHE.Enabled = true;
                    break;
                case execID.ED:
                    gbEditor.Enabled = true;
                    ToggleEditorControls(false);
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

            if (CurrentID != execID.ED)
            {
                ToggleEditorControls(true);
                AskForRecommendedSettings();
            }
        }
        private void ToggleEditorControls(bool Enable)
        {
            cbZoom.Enabled = Enable;
            lblZoomAngle.Enabled = Enable;
            lblTextureRes.Enabled = Enable;
            cbLimitedEdition.Enabled = Enable;
            cbModloader.Enabled = Enable;
            cbScriptBugFixes.Enabled = Enable;
        }
        private void AskForRecommendedSettings()
        {
            DialogResult Result = MessageBox.Show(Resources.WelcomeMessage, "Preselect Recommended Settings ...", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (Result == DialogResult.Yes)
            {
                cbZoom.Checked = true;
                cbScriptBugFixes.Checked = true;
                cbModloader.Checked = true;
                cbHighTextures.Checked = true;
                cbLAAFlag.Checked = true;
                cbKnightSelection.Checked = true;
                cbLimitedEdition.Checked = true;
                cbUseSingleStop.Checked = true;
                cbUseDowngrade.Checked = true;
                cbUseMilitaryRelease.Checked = true;

                txtZoom.Text = "14000";
                txtResolution.Text = "4096";

                if (CurrentID == execID.HE_STEAM || CurrentID == execID.HE_UBISOFT)
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

            return Boxes.Select(Element => Element.Text).ToList();
        }
        private void SelectPatchFeatures()
        {
            Patcher.Patcher Patcher;
            try
            {
                Patcher = new Patcher.Patcher(CurrentID, GlobalStream);
            }
            catch (Exception ex)
            {
                Logger.Instance.Log(ex.ToString());
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            List<string> Features = GetPatchFeaturesByControls(new List<GroupBox> {gbAll, gbHE, gbEditor});
            Patcher.PatchByControlFeatures(Features);

            if (cbZoom.Checked)
            {
                Patcher.SetZoomLevel(txtZoom.Text);
            }
            if (cbHighTextures.Checked && CurrentID != execID.ED) // Editor has no custom texture resolution
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
                SetEntriesInOptionsFileByCheckBox(Patcher);
            }
            if (cbModloader.Checked)
            {
                Patcher.SetModLoader();
            }
        }

        private void SetEntriesInOptionsFileByCheckBox(Patcher.Patcher Patcher)
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

        private void ResetForm()
        {
            CloseFileStream();

            new List<GroupBox> {gbAll, gbHE, gbEditor}.ForEach(Control =>
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
            this.ActiveControl = null;

            Logger.Instance.Log("ResetForm(): Form successfully reset!");
        }

        private void CloseFileStream()
        {
            if (GlobalStream != null && GlobalStream.CanRead == true) // Close Filestream if not already done
            {
                GlobalStream.Close();
                GlobalStream.Dispose();
            }
        }
    }
}
