using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace S6Patcher
{
    public partial class mainPatcher : Form
    {
        private FileStream GlobalStream = null;
        private execID GlobalID;

        public mainPatcher(execID ID, ref FileStream Stream)
        {
            InitializeComponent();
            InitializeControls(ID, ref Stream);

            GlobalStream = Stream;
            GlobalID = ID;

            this.Text = "S6Patcher - V" + Application.ProductVersion + " - \"https://github.com/Eisenmonoxid/S6Patcher\"";
        }
        private void InitializeControls(execID ID, ref FileStream Stream)
        {
            if (ID == execID.Editor)
            {
                for (ushort it = 0; it != gbEditor.Controls.Count; it++)
                {
                    gbEditor.Controls[it].Enabled = true;
                }

                cbZoom.Enabled = false;
                lblZoomAngle.Enabled = false;
                gbHE.Enabled = false;
                txtResolution.Enabled = false;
                lblTextureRes.Enabled = false;
            }
            else
            {
                BinaryReader Reader = new BinaryReader(Stream);
                if (ID == execID.OV)
                {
                    Reader.BaseStream.Position = 0x545400;
                    txtZoom.Text = Reader.ReadDouble().ToString();

                    Reader.BaseStream.Position = 0x2BE177;
                    txtResolution.Text = Reader.ReadUInt32().ToString();

                    gbHE.Enabled = false;
                    gbEditor.Enabled = false;
                }
                else
                {
                    for (short it = 0; it != gbHE.Controls.Count; it++)
                    {
                        gbHE.Controls[it].Enabled = true;
                    }

                    Reader.BaseStream.Position = (Helpers.IsSteamHE) ? 0xC4F9EC : 0xC4EC4C;
                    txtZoom.Text = Reader.ReadSingle().ToString();

                    Reader.BaseStream.Position = (Helpers.IsSteamHE) ? 0x2D4D74 : 0x2D4188;
                    txtResolution.Text = Reader.ReadUInt32().ToString();

                    Reader.BaseStream.Position = (Helpers.IsSteamHE) ? 0xEB95C0 : 0xEB83C0;
                    txtAutosave.Text = (Reader.ReadDouble() / 60000).ToString();

                    gbHE.Enabled = true;
                    gbEditor.Enabled = false;
                    txtAutosave.Enabled = false;
                }
            }
        }
        private Patcher GetPatcherForControls(execID ID, ref FileStream Stream, List<GroupBox> Controls)
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

            Patcher Patcher = new Patcher(ID, ref Stream, ref CheckedFeatures);
            return Patcher;
        }
        private void SelectPatchVersion(execID ID, ref FileStream Stream)
        {
            Patcher Patcher = GetPatcherForControls(ID, ref Stream, new List<GroupBox> {gbAll, gbHE, gbEditor});
            if (cbZoom.Checked)
            {
                Patcher.SetZoomLevel(ID, ref Stream, txtZoom.Text);
            }
            if (cbHighTextures.Checked && ID != execID.Editor) // Editor has no custom texture resolution
            {
                Patcher.SetHighResolutionTextures(ID, ref Stream, txtResolution.Text);
            }
            if (cbAutosave.Checked)
            {
                Patcher.SetAutosaveTimer(ref Stream, txtAutosave.Text);
            }
            if (cbLAAFlag.Checked)
            {
                Patcher.SetLargeAddressAwareFlag(ref Stream);
            }
            if (cbScriptBugFixes.Checked)
            {
                Patcher.SetLuaScriptBugFixes();
            }
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
            if (cbHighTextures.Checked && GlobalID != execID.Editor)
            {
                txtResolution.Enabled = true;
            }
            else
            {
                txtResolution.Enabled = false;
            }
        }
        private void btnPatch_Click(object sender, EventArgs e)
        {
            SelectPatchVersion(GlobalID, ref GlobalStream);
            DialogResult = DialogResult.OK;

            Close();
            Dispose();
        }
        private void btnAbort_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Abort;

            Close();
            Dispose();
        }
        private void btnBackup_Click(object sender, EventArgs e)
        {
            string Path = GlobalStream.Name;
            GlobalStream.Close();
            GlobalStream.Dispose();

            bool Result = Helpers.RestoreBackup(Path);
            if (Result == false)
            {
                DialogResult = DialogResult.Cancel;
            }
            else
            {
                DialogResult = DialogResult.Abort;
            }

            Close();
            Dispose();
        }
    }
}
