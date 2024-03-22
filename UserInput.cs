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

                    cbAutosave.Enabled = true;
                }
            }

            txtZoom.Enabled = false;
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
                        PatchHelpers.WriteBytesToFile(ref execStream, 0x1C5F2A, new byte[] {0xEB});
                    }
                    break;
                case execID.Editor:
                    foreach (var Element in Mappings.EditorMapping)
                    {
                        PatchHelpers.WriteBytesToFile(ref execStream, Element.Key, Element.Value);
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
            if (execStream != null)
            {
                execStream.Close();
                execStream.Dispose();
            }

            Close();
            Dispose();
        }
        private void SetZoomLevel()
        {
            if (globalIdentifier == execID.OV)
            {
                PatchHelpers.WriteBytesToFile(ref execStream, 0x545400, BitConverter.GetBytes(Convert.ToDouble(txtZoom.Text)));
            }
            else
            {
                PatchHelpers.WriteBytesToFile(ref execStream, 0xC4EC4C, BitConverter.GetBytes(Convert.ToSingle(txtZoom.Text)));
            }       
            // 0x545400 -> 7200 | 0x53f150 -> 1800 - OV
            // 0xC4EC4C -> 7200 | 0xC4E008 -> 1800 - HE
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
    }
}
