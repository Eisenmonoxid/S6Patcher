using System;
using System.IO;
using System.Windows.Forms;

namespace S6Patcher
{
    public partial class S6Patcher : Form
    {
        FileStream execStream = null;
        execID globalIdentifier;

        public S6Patcher(execID Identifier, ref FileStream Stream)
        {
            InitializeComponent();
            execStream = Stream;
            globalIdentifier = Identifier;

            if (Identifier == execID.Editor)
            {
                cbZoom.Enabled = false;
                cbAllEntities.Enabled = true;
            }
            else
            {
                BinaryReader br = new BinaryReader(execStream);
                if (Identifier == execID.OV)
                {
                    br.BaseStream.Position = 0x545400;
                    txtZoom.Text = br.ReadDouble().ToString();
                }
                else
                {
                    br.BaseStream.Position = 0xC4EC4C;
                    txtZoom.Text = br.ReadSingle().ToString();

                    br.BaseStream.Position = 0xEB83C0;
                    txtAutosave.Text = ((br.ReadDouble() / 60) / 1000).ToString();

                    cbAutosave.Enabled = true;
                }
            }
        }
        private void mainPatcher()
        {
            switch (globalIdentifier)
            {
                case execID.OV:
                    foreach (var Element in Mappings.OVMapping)
                    {
                        PatchHelpers.WriteBytesToFile(ref execStream, Element.Key, Element.Value);
                    }
                    if (cbZoom.Checked)
                    {
                        SetZoomLevel();
                    }
                    break;
                case execID.HE:
                    foreach (var Element in Mappings.HEMapping)
                    {
                        PatchHelpers.WriteBytesToFile(ref execStream, Element.Key, Element.Value);
                    }
                    if (cbZoom.Checked)
                    {
                        SetZoomLevel();
                    }
                    if (cbAutosave.Checked)
                    {
                        SetAutosaveTimer();
                    }
                    break;
                case execID.Editor:
                    foreach (var Element in Mappings.EditorMapping)
                    {
                        PatchHelpers.WriteBytesToFile(ref execStream, Element.Key, Element.Value);
                    }
                    if (cbAllEntities.Checked)
                    {   
                        PatchHelpers.WriteBytesToFile(ref execStream, 0x20615, new byte[] {0x90, 0x90, 0x90, 0x90, 0x90, 0x90});
                        PatchHelpers.WriteBytesToFile(ref execStream, 0x20629, new byte[] {0xEB, 0x10});
                    }
                    break;
                default:
                    break;
            }
            if (cbLAA.Checked) 
            {
                SetLargeAddressAwareFlag();
            }
        }
        private void btnPatch_Click(object sender, EventArgs e)
        {
            mainPatcher();

            execStream.Close();
            execStream.Dispose();

            Close();
            Dispose();
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

            autosaveTimer = autosaveTimer * 1000 * 60;
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
    }
}
