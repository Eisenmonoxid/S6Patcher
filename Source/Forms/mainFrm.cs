using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace S6Patcher
{
    public partial class mainFrm : Form
    {
        private FileStream GlobalStream = null;
        public mainFrm()
        {
            InitializeComponent();
            this.Text = "S6Patcher - v" + Application.ProductVersion.Substring(0, 3) + " - \"github.com/Eisenmonoxid/S6Patcher\"";

            Helpers.CheckForUpdates();
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
                txtZoom.Text = Reader.ReadDouble().ToString();
            }
            else if (Control == txtAutosave)
            {
                txtAutosave.Text = (Reader.ReadDouble() / 60000).ToString();
            }
        }
        private void InitializeControls()
        {
            BinaryReader Reader = new BinaryReader(GlobalStream);
            switch (Helpers.CurrentID)
            {
                case execID.OV:
                    SetControlValueFromStream(ref Reader, 0x545400, txtZoom);
                    SetControlValueFromStream(ref Reader, 0x2BE177, txtResolution);
                    break;
                case execID.OV_OFFSET:
                    SetControlValueFromStream(ref Reader, 0x545400 - Helpers.GlobalOffset, txtZoom);
                    SetControlValueFromStream(ref Reader, 0x2BE177 - Helpers.GlobalOffset, txtResolution);
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
        private Patcher GetPatchFeaturesByControls(List<GroupBox> Controls)
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

            Patcher Patcher = new Patcher(Helpers.CurrentID, ref GlobalStream, ref CheckedFeatures);
            return Patcher;
        }
        private void SelectPatchFeatures()
        {
            Patcher Patcher = GetPatchFeaturesByControls(new List<GroupBox> {gbAll, gbHE, gbEditor});
            if (cbZoom.Checked)
            {
                Patcher.SetZoomLevel(Helpers.CurrentID, ref GlobalStream, txtZoom.Text);
            }
            if (cbHighTextures.Checked && Helpers.CurrentID != execID.ED) // Editor has no custom texture resolution
            {
                Patcher.SetHighResolutionTextures(Helpers.CurrentID, ref GlobalStream, txtResolution.Text);
            }
            if (cbAutosave.Checked)
            {
                Patcher.SetAutosaveTimer(Helpers.CurrentID, ref GlobalStream, txtAutosave.Text);
            }
            if (cbLAAFlag.Checked)
            {
                Patcher.SetLargeAddressAwareFlag(ref GlobalStream);
            }
            if (cbScriptBugFixes.Checked)
            {
                Patcher.SetLuaScriptBugFixes();
                Patcher.SetKnightSelection(cbKnightSelection.Checked);
            }
        }
    }
}
