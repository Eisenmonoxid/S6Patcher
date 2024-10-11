using S6Patcher.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace S6Patcher
{
    public partial class S6Patcher : Form
    {
        private FileStream execStream = null;
        private execID globalIdentifier;

        public S6Patcher(execID Identifier, ref FileStream Stream)
        {
            execStream = Stream;
            globalIdentifier = Identifier;

            InitializeComponent();
            InitializeControls();
        }
        private void InitializeControls()
        {
            if (globalIdentifier == execID.Editor)
            {
                for (short it = 0; it != gbEditor.Controls.Count; it++)
                {
                    gbEditor.Controls[it].Enabled = true;
                }

                cbZoom.Enabled = false;
                lblZoomAngle.Enabled = false;
                gbHE.Enabled = false;
            }
            else
            {
                BinaryReader br = new BinaryReader(execStream);
                if (globalIdentifier == execID.OV)
                {
                    br.BaseStream.Position = 0x545400;
                    txtZoom.Text = br.ReadDouble().ToString();

                    gbHE.Enabled = false;
                    gbEditor.Enabled = false;
                }
                else
                {
                    for (short it = 0; it != gbHE.Controls.Count; it++)
                    {
                        gbHE.Controls[it].Enabled = true;
                    }

                    br.BaseStream.Position = 0xC4EC4C;
                    txtZoom.Text = br.ReadSingle().ToString();

                    br.BaseStream.Position = 0xEB83C0;
                    txtAutosave.Text = ((br.ReadDouble() / 60) / 1000).ToString();

                    gbHE.Enabled = true;
                    gbEditor.Enabled = false;
                }
            }
        }
        private void mainPatcher(execID ID, GroupBox Controls)
        {
            Mappings currentMapping = new Mappings();
            var Entries = currentMapping.InitializeMappings(ID);

            CheckBox curControl;
            for (short it = 0; it != Controls.Controls.Count; it++)
            {
                try
                {
                    curControl = (CheckBox)Controls.Controls[it];
                }
                catch
                {
                    continue;
                }
                
                if (curControl.Checked)
                {
                    foreach (var curElement in Entries)
                    {
                        if (curControl.Text == curElement.Name)
                        {
                            foreach (var DictEntry in curElement.AddressMapping)
                            {
                                PatchHelpers.WriteBytesToFile(ref execStream, DictEntry.Key, DictEntry.Value);
                            }
                        }
                    }
                }
            }
        }
        private void selectPatchVersion()
        {
            switch (globalIdentifier)
            {
                case execID.OV:
                    mainPatcher(execID.OV, gbAll);
                    if (cbZoom.Checked)
                    {
                        SetZoomLevel();
                    }
                    break;
                case execID.HE:
                    mainPatcher(execID.HE, gbAll);
                    mainPatcher(execID.HE, gbHE);
                    if (cbZoom.Checked)
                    {
                        SetZoomLevel();
                    }
                    if (cbAutosave.Checked)
                    {
                        SetAutosaveTimer();
                    }
                    if (cbMeldungsstauFix.Checked)
                    {
                        SetMeldungsstauFix();
                    }
                    break;
                case execID.Editor:
                    mainPatcher(execID.Editor, gbAll);
                    mainPatcher(execID.Editor, gbEditor);
                    break;
                default:
                    break;
            }
            if (cbLAAFlag.Checked) 
            {
                SetLargeAddressAwareFlag();
            }
        }
        private void SetAutosaveTimer()
        {
            double autosaveTimer;
            try
            {
                autosaveTimer = Convert.ToDouble(txtAutosave.Text);
            }
            catch (Exception)
            {
                return;
            }

            if (autosaveTimer == (double)0)
            {
                PatchHelpers.WriteBytesToFile(ref execStream, 0x1C5F2A, new byte[] {0xEB});
                return;
            }

            autosaveTimer = (autosaveTimer * 1000 * 60);
            PatchHelpers.WriteBytesToFile(ref execStream, 0xEB83C0, BitConverter.GetBytes(autosaveTimer));
            PatchHelpers.WriteBytesToFile(ref execStream, 0x1C5F2A, new byte[] {0x76});
        }
        private void SetZoomLevel()
        {
            float Offset = 4800;
            float TransitionFactor = 3700;
            if (globalIdentifier == execID.OV)
            {
                double zoomLevel;
                float clutterFarDistance;
                try
                {
                    zoomLevel = Convert.ToDouble(txtZoom.Text);
                    clutterFarDistance = Convert.ToSingle(txtZoom.Text);
                }
                catch (Exception)
                {
                    return;
                }

                PatchHelpers.WriteBytesToFile(ref execStream, 0x545400, BitConverter.GetBytes(zoomLevel));
                PatchHelpers.WriteBytesToFile(ref execStream, 0x2B334E, new byte[] {0xC7, 0x45, 0x64});
                PatchHelpers.WriteBytesToFile(ref execStream, 0x2B3351, BitConverter.GetBytes(clutterFarDistance + Offset));
                PatchHelpers.WriteBytesToFile(ref execStream, 0x2B3355, new byte[] {0xC7, 0x45, 0x6C});
                PatchHelpers.WriteBytesToFile(ref execStream, 0x2B3358, BitConverter.GetBytes(TransitionFactor));
                PatchHelpers.WriteBytesToFile(ref execStream, 0x2B335C, new byte[] {0x90, 0x90});
                // Restriction -5000.0
                PatchHelpers.WriteBytesToFile(ref execStream, 0x27AC99, new byte[] {0x50, 0xB8, 0x00, 0x40, 0x9C, 0xC5, 0x89, 0x81,
                        0x9C, 0x00, 0x00, 0x00, 0x58, 0xC6, 0x81, 0x98, 0x00, 0x00, 0x00, 0x01, 0xC2, 0x08, 0x00});
            }
            else
            {
                float zoomLevel;
                try
                {
                    zoomLevel = Convert.ToSingle(txtZoom.Text);
                }
                catch (Exception)
                {
                    return;
                }

                PatchHelpers.WriteBytesToFile(ref execStream, 0xC4EC4C, BitConverter.GetBytes(zoomLevel));
                PatchHelpers.WriteBytesToFile(ref execStream, 0x270311, new byte[] {0xC7, 0x45, 0xF0});
                PatchHelpers.WriteBytesToFile(ref execStream, 0x270314, BitConverter.GetBytes(zoomLevel + Offset));
                PatchHelpers.WriteBytesToFile(ref execStream, 0x270318, new byte[] {0xC7, 0x45, 0xF4});
                PatchHelpers.WriteBytesToFile(ref execStream, 0x27031B, BitConverter.GetBytes(TransitionFactor));
                PatchHelpers.WriteBytesToFile(ref execStream, 0x27031F, new byte[] {0x90});
                PatchHelpers.WriteBytesToFile(ref execStream, 0x270325, new byte[] {0x90, 0x90, 0x90});
                // Restriction -5000.0
                PatchHelpers.WriteBytesToFile(ref execStream, 0x2532F7, new byte[] {0x50, 0xB8, 0x00, 0x40, 0x9C, 0xC5, 0x89, 0x81,
                        0x9C, 0x00, 0x00, 0x00, 0x58, 0xC6, 0x81, 0x98, 0x00, 0x00, 0x00, 0x01, 0x90, 0x90});
            }
        }
        private void SetLargeAddressAwareFlag()
        {
            // Partially adapted from:
            // https://stackoverflow.com/questions/9054469/how-to-check-if-exe-is-set-as-largeaddressaware
            const int IMAGE_FILE_LARGE_ADDRESS_AWARE = 0x20;
            BinaryReader br = new BinaryReader(execStream);

            br.BaseStream.Position = 0x3C;
            br.BaseStream.Position = br.ReadInt32();

            if (br.ReadInt32() != 0x4550)
            {
                return;
            }

            br.BaseStream.Position += 0x12;
            long CurrentPosition = br.BaseStream.Position;

            Int16 Flag = br.ReadInt16();
            if ((Flag & IMAGE_FILE_LARGE_ADDRESS_AWARE) != IMAGE_FILE_LARGE_ADDRESS_AWARE)
            {
                PatchHelpers.WriteBytesToFile(ref execStream, CurrentPosition, BitConverter.GetBytes(Flag |= IMAGE_FILE_LARGE_ADDRESS_AWARE));
            }
        }
        private void SetMeldungsstauFix()
        {
            string LocalScriptFilePath = "shr\\Script\\Local\\MainMapScript\\LocalMainMapScript.lua";
            string FinalPath = Path.Combine(Directory.GetParent(Path.GetDirectoryName(execStream.Name)).FullName, LocalScriptFilePath);

            if (!File.Exists(FinalPath))
            {
                MessageBox.Show(Resources.ErrorMeldungsstauFix, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                File.WriteAllBytes(FinalPath, Resources.LocalMainMapScript);
            }
            catch (Exception)
            {
                return;
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
        private void btnPatch_Click(object sender, EventArgs e)
        {
            selectPatchVersion();
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
            string Path = execStream.Name;
            execStream.Close();
            execStream.Dispose();

            bool Result = PatchHelpers.RestoreBackup(Path);
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
