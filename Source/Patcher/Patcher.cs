using S6Patcher.Source.Helpers;
using S6Patcher.Source.Patcher.Mappings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace S6Patcher.Source.Patcher
{
    internal partial class Patcher
    {
        private readonly FileStream GlobalStream = null;
        private readonly MappingBase GlobalMappings = null;
        private readonly Mod GlobalMod = null;
        private readonly execID GlobalID = execID.NONE;
        private const uint GlobalOffset = 0x3F0000;

        public Patcher(execID ID, FileStream Stream)
        {
            if (ID == execID.NONE || Stream == null || Stream.CanWrite == false)
            {
                throw new ArgumentException("Erroneous arguments to Patcher ctor.");
            }

            GlobalID = ID;
            GlobalStream = Stream;
            GlobalMod = new Mod(ID, Stream);
            GlobalMappings = MappingBase.GetMappingsByID(GlobalID);

            Logger.Instance.Log("Patcher ctor(): ID: " + GlobalID.ToString() + ", Stream: " + GlobalStream.Name);
        }

        public void PatchByControlFeatures(List<string> Names)
        {
            List<MappingBase.PatchEntry> Entries = GlobalMappings.GetMapping();
            var Result = (
                from Entry in Entries
                from Name in Names
                where Name == Entry.Name
                select Entry.Mapping).ToList();

            Result.ForEach(Element =>
            {
                foreach (var Entry in Element)
                {
                    WriteBytes(Entry.Key, Entry.Value);
                    Logger.Instance.Log("PatchByControlFeatures(): Patching Element: 0x" + $"{Entry.Key:X}");
                }
            });
        }

        public void SetHighResolutionTextures(string ResolutionText)
        {
            Logger.Instance.Log("SetHighResolutionTextures(): Called with " + ResolutionText);

            uint Resolution;
            try
            {
                Resolution = Convert.ToUInt32(ResolutionText);
            }
            catch (Exception ex)
            {
                Logger.Instance.Log(ex.ToString());
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Dictionary<long, byte[]> Mapping = GlobalMappings.GetTextureResolutionMapping(Resolution);
            foreach (var Element in Mapping)
            {
                WriteBytes(Element.Key, Element.Value);
            }

            IOFileHandler.Instance.UpdateEntryInOptionsFile("[Display]", "TextureResolution", 3);
            IOFileHandler.Instance.UpdateEntryInOptionsFile("[Display]", "Terrain", 2);
        }

        public void SetAutosaveTimer(string AutosaveText)
        {
            Logger.Instance.Log("SetAutosaveTimer(): Called with " + AutosaveText);

            double Timer;
            try
            {
                Timer = Convert.ToDouble(AutosaveText);
            }
            catch (Exception ex)
            {
                Logger.Instance.Log(ex.ToString());
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Dictionary<long, byte[]> Mapping = GlobalMappings.GetAutoSaveMapping(Timer);
            foreach (var Element in Mapping)
            {
                WriteBytes(Element.Key, Element.Value);
            }
        }

        public void SetZoomLevel(string ZoomText)
        {
            Logger.Instance.Log("SetZoomLevel(): Called with " + ZoomText);

            double Level;
            float Distance;
            try
            {
                Level = Convert.ToDouble(ZoomText);
                Distance = Convert.ToSingle(ZoomText);
            }
            catch (Exception ex)
            {
                Logger.Instance.Log(ex.ToString());
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Dictionary<long, byte[]> Mapping = GlobalMappings.GetZoomLevelMapping(Level, Distance);
            foreach (var Element in Mapping)
            {
                WriteBytes(Element.Key, Element.Value);
            }
        }

        public void SetModLoader(bool UseBugFixMod)
        {
            Logger.Instance.Log("SetModLoader(): Called.");

            Dictionary<long, byte[]> Entries = GlobalMappings.GetModloaderMapping();
            foreach (var Entry in Entries)
            {
                WriteBytes(Entry.Key, Entry.Value);
            }

            GlobalMod.CreateModLoader(UseBugFixMod);
        }

        public void SetLargeAddressAwareFlag()
        {
            Logger.Instance.Log("SetLargeAddressAwareFlag(): Called.");

            // Partially adapted from:
            // https://stackoverflow.com/questions/9054469/how-to-check-if-exe-is-set-as-largeaddressaware
            const int IMAGE_FILE_LARGE_ADDRESS_AWARE = 0x20;
            BinaryReader Reader = new BinaryReader(GlobalStream);

            Reader.BaseStream.Position = 0x3C;
            Reader.BaseStream.Position = Reader.ReadInt32();

            if (Reader.ReadInt32() != 0x4550)
            {
                Logger.Instance.Log("SetLargeAddressAwareFlag(): Error - Not at expected offset!");
                return;
            }

            Reader.BaseStream.Position += 0x12;
            long CurrentPosition = Reader.BaseStream.Position;

            Int16 Flag = Reader.ReadInt16();
            if ((Flag & IMAGE_FILE_LARGE_ADDRESS_AWARE) != IMAGE_FILE_LARGE_ADDRESS_AWARE)
            {
                WriteBytes(CurrentPosition, BitConverter.GetBytes(Flag |= IMAGE_FILE_LARGE_ADDRESS_AWARE));
            }

            Logger.Instance.Log("SetLargeAddressAwareFlag(): Finished successfully.");
        }

        public void SetLuaScriptBugFixes()
        {
            Logger.Instance.Log("SetLuaScriptBugFixes(): Called.");

            UserScriptHandler.GetUserScriptDirectories().ForEach(Element =>
            {
                string CurrentPath = Path.Combine(Element, "Script");
                try
                {
                    Directory.CreateDirectory(CurrentPath);
                    for (uint i = 0; i < UserScriptHandler.ScriptFiles.Length; i++)
                    {
                        File.WriteAllBytes(Path.Combine(CurrentPath, UserScriptHandler.ScriptFiles[i]), UserScriptHandler.ScriptResources[i]);
                        Logger.Instance.Log("SetLuaScriptBugFixes(): Finished writing Scriptfile named " + UserScriptHandler.ScriptFiles[i] + " to " + CurrentPath);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Instance.Log(ex.ToString());
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            });
        }

        public void SetEntryInOptionsFile(string Entry, bool Checked)
        {
            IOFileHandler.Instance.UpdateEntryInOptionsFile("[S6Patcher]", Entry, Checked == true ? 1U : 0U);
        }

        private void WriteBytes(long Position, byte[] Bytes)
        {
            GlobalStream.Position = (GlobalID == execID.OV_OFFSET) ? Position - GlobalOffset : Position;
            try
            {
                GlobalStream.Write(Bytes, 0, Bytes.Length);
            }
            catch (Exception ex)
            {
                Logger.Instance.Log(ex.ToString());
            }
        }
    }
}
