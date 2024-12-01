using S6Patcher.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace S6Patcher
{
    internal partial class Patcher
    {
        public Patcher(execID ID, ref FileStream Stream, ref List<string> PatchNames)
        {
            List<PatchEntry> Entries = GetMappingsByExecID(ID);
            foreach (PatchEntry Entry in Entries)
            {
                foreach (string Element in PatchNames)
                {
                    if (Element == Entry.Name)
                    {
                        foreach (var DictEntry in Entry.AddressMapping)
                        {
                            Helpers.WriteBytes(ref Stream, DictEntry.Key, DictEntry.Value);
                        }
                    }
                }
            }
        }
        public void SetHighResolutionTextures(execID ID, ref FileStream Stream, string ResolutionText)
        {
            UInt32 Resolution;
            try
            {
                Resolution = Convert.ToUInt32(ResolutionText);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            byte[] HighResolution = BitConverter.GetBytes(Resolution);
            byte[] MediumResolution = BitConverter.GetBytes(Resolution / 2);
            byte[] LowResolution = BitConverter.GetBytes(Resolution / 4);

            if (ID == execID.OV)
            {
                Helpers.WriteBytes(ref Stream, 0x2BE177, HighResolution);
                Helpers.WriteBytes(ref Stream, 0x2BE17E, MediumResolution);
                Helpers.WriteBytes(ref Stream, 0x2BE185, LowResolution);
            }
            else // HE
            {
                Helpers.WriteBytes(ref Stream, ((Helpers.IsSteamHE) ? 0x2D4D74 : 0x2D4188), HighResolution);
                Helpers.WriteBytes(ref Stream, ((Helpers.IsSteamHE) ? 0x2D4D7B : 0x2D418F), MediumResolution);
                Helpers.WriteBytes(ref Stream, ((Helpers.IsSteamHE) ? 0x2D4D82 : 0x2D4196), LowResolution);
            }
        }
        public void SetAutosaveTimer(ref FileStream Stream, string AutosaveText)
        {
            double autosaveTimer;
            try
            {
                autosaveTimer = Convert.ToDouble(AutosaveText);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (autosaveTimer == (double)0)
            {
                Helpers.WriteBytes(ref Stream, ((Helpers.IsSteamHE) ? 0x1C6045 : 0x1C5F2A), new byte[] {0xEB});
            }
            else
            {
                Helpers.WriteBytes(ref Stream, ((Helpers.IsSteamHE) ? 0x1C6045 : 0x1C5F2A), new byte[] {0x76});
            }

            autosaveTimer = (autosaveTimer * 60000);
            Helpers.WriteBytes(ref Stream, ((Helpers.IsSteamHE) ? 0xEB95C0 : 0xEB83C0), BitConverter.GetBytes(autosaveTimer));     
        }
        public void SetZoomLevel(execID ID, ref FileStream Stream, string ZoomText)
        {
            float Offset = 4800;
            float TransitionFactor = 3700;

            if (ID == execID.OV)
            {
                double zoomLevel;
                float clutterFarDistance;
                try
                {
                    zoomLevel = Convert.ToDouble(ZoomText);
                    clutterFarDistance = Convert.ToSingle(ZoomText);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Helpers.WriteBytes(ref Stream, 0x545400, BitConverter.GetBytes(zoomLevel));
                Helpers.WriteBytes(ref Stream, 0x2B334E, new byte[] {0xC7, 0x45, 0x64});
                Helpers.WriteBytes(ref Stream, 0x2B3351, BitConverter.GetBytes(clutterFarDistance + Offset));
                Helpers.WriteBytes(ref Stream, 0x2B3355, new byte[] {0xC7, 0x45, 0x6C});
                Helpers.WriteBytes(ref Stream, 0x2B3358, BitConverter.GetBytes(TransitionFactor));
                Helpers.WriteBytes(ref Stream, 0x2B335C, new byte[] {0x90, 0x90});
                Helpers.WriteBytes(ref Stream, 0x27AC99, new byte[] {0x50, 0xB8, 0x00, 0x40, 0x9C, 0xC5, 0x89, 0x81,
                        0x9C, 0x00, 0x00, 0x00, 0x58, 0xC6, 0x81, 0x98, 0x00, 0x00, 0x00, 0x01, 0xC2, 0x08, 0x00}); // Restriction -5000.0
            }
            else // HE
            {
                float zoomLevel;
                try
                {
                    zoomLevel = Convert.ToSingle(ZoomText);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Helpers.WriteBytes(ref Stream, ((Helpers.IsSteamHE) ? 0xC4F9EC : 0xC4EC4C), BitConverter.GetBytes(zoomLevel));
                Helpers.WriteBytes(ref Stream, ((Helpers.IsSteamHE) ? 0x270E73 : 0x270311), new byte[] {0xC7, 0x45, 0xF0});
                Helpers.WriteBytes(ref Stream, ((Helpers.IsSteamHE) ? 0x270E76 : 0x270314), BitConverter.GetBytes(zoomLevel + Offset));
                Helpers.WriteBytes(ref Stream, ((Helpers.IsSteamHE) ? 0x270E7A : 0x270318), new byte[] {0xC7, 0x45, 0xF4});
                Helpers.WriteBytes(ref Stream, ((Helpers.IsSteamHE) ? 0x270E7D : 0x27031B), BitConverter.GetBytes(TransitionFactor));
                Helpers.WriteBytes(ref Stream, ((Helpers.IsSteamHE) ? 0x270E81 : 0x27031F), new byte[] {0x90});
                Helpers.WriteBytes(ref Stream, ((Helpers.IsSteamHE) ? 0x270E87 : 0x270325), new byte[] {0x90, 0x90, 0x90});
                Helpers.WriteBytes(ref Stream, ((Helpers.IsSteamHE) ? 0x2540B8 : 0x2532F7), new byte[] {0x50, 0xB8, 0x00, 0x40, 0x9C, 0xC5, 0x89, 0x81,
                        0x9C, 0x00, 0x00, 0x00, 0x58, 0xC6, 0x81, 0x98, 0x00, 0x00, 0x00, 0x01, 0x90, 0x90}); // Restriction -5000.0
            }
        }
        public void SetLargeAddressAwareFlag(ref FileStream Stream)
        {
            // Partially adapted from:
            // https://stackoverflow.com/questions/9054469/how-to-check-if-exe-is-set-as-largeaddressaware
            const int IMAGE_FILE_LARGE_ADDRESS_AWARE = 0x20;
            BinaryReader br = new BinaryReader(Stream);

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
                Helpers.WriteBytes(ref Stream, CurrentPosition, BitConverter.GetBytes(Flag |= IMAGE_FILE_LARGE_ADDRESS_AWARE));
            }
        }
        public void SetLuaScriptBugFixes()
        {
            string[] ScriptFiles = {"UserScriptLocal.lua", "EMXBinData.s6patcher"};
            List<string> Directories = Helpers.GetUserScriptDirectories();

            try
            {
                string ScriptPath = String.Empty;
                foreach (string Element in Directories)
                {
                    ScriptPath = Path.Combine(Element, "Script");
                    if (!Directory.Exists(ScriptPath))
                    {
                        Directory.CreateDirectory(ScriptPath);
                    }

                    File.WriteAllBytes(Path.Combine(ScriptPath, ScriptFiles[0]), Resources.UserScriptLocal);
                    File.WriteAllBytes(Path.Combine(ScriptPath, ScriptFiles[1]), Resources.EMXBinData);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(Resources.ErrorLuaScriptFixes + "\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}