using S6Patcher.Source.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace S6Patcher.Source.Forms
{
    public partial class mainFrm : Form
    {
        private FileStream GlobalStream = null;
        public mainFrm()
        {
            InitializeComponent();
            this.Text = "S6Patcher - v" + Application.ProductVersion;
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
                if (Helpers.Helpers.CurrentID == execID.HE_UBISOFT || Helpers.Helpers.CurrentID == execID.HE_STEAM)
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
            switch (Helpers.Helpers.CurrentID)
            {
                case execID.OV:
                    SetControlValueFromStream(ref Reader, 0x545400, txtZoom);
                    SetControlValueFromStream(ref Reader, 0x2BE177, txtResolution);
                    break;
                case execID.OV_OFFSET:
                    SetControlValueFromStream(ref Reader, 0x545400 - Helpers.Helpers.GlobalOffset, txtZoom);
                    SetControlValueFromStream(ref Reader, 0x2BE177 - Helpers.Helpers.GlobalOffset, txtResolution);
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
                    cbZoom.Enabled = false;
                    lblZoomAngle.Enabled = false;
                    lblTextureRes.Enabled = false;
					cbLimitedEdition.Enabled = false;
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
        }
        private List<string> GetPatchFeaturesByControls(List<GroupBox> Controls)
        {
            List<string> CheckedFeatures = new List<string>();
            CheckBox curControl;

            foreach (GroupBox Box in Controls)
            {
                for (ushort it = 0; it != Box.Controls.Count; it++)
                {
                    try
                    {
                        curControl = (CheckBox)Box.Controls[it];
                    }
                    catch
                    {
                        continue;
                    }

                    if (curControl.Checked)
                    {
                        CheckedFeatures.Add(curControl.Text);
                    }
                }
            }

            return CheckedFeatures;
        }
        private void SelectPatchFeatures()
        {
            Patcher.Patcher Patcher;
            try
            {
                Patcher = new Patcher.Patcher(Helpers.Helpers.CurrentID, ref GlobalStream);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            List<string> Features = GetPatchFeaturesByControls(new List<GroupBox> {gbAll, gbHE, gbEditor});
            Patcher.PatchByControlFeatures(Features);

            if (cbZoom.Checked)
            {
                Patcher.SetZoomLevel(txtZoom.Text);
            }
            if (cbHighTextures.Checked && Helpers.Helpers.CurrentID != execID.ED) // Editor has no custom texture resolution
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
                Patcher.SetKnightSelection(cbKnightSelection.Checked);
            }
        }
        private void CloseFileStream()
        {
            if (GlobalStream != null && GlobalStream.CanRead == true) // Close Filestream if not already done
            {
                GlobalStream.Close();
                GlobalStream.Dispose();
            }
        }
        private void ResetForm()
        {
            CloseFileStream();
            foreach (var Control in new List<GroupBox> {gbAll, gbHE, gbEditor})
            {
                Control.Enabled = false;

                CheckBox curCheckBox;
                TextBox curTextBox;
                foreach (var Element in Control.Controls)
                {
                    try
                    {
                        curCheckBox = (CheckBox)Element;
                        curCheckBox.Checked = false;
                    }
                    catch {};
                    try
                    {
                        curTextBox = (TextBox)Element;
                        curTextBox.Text = String.Empty;
                    }
                    catch {};
                }
            }

            btnPatch.Enabled = false;
            btnBackup.Enabled = false;
            txtExecutablePath.Text = String.Empty;
        }
    }
}
