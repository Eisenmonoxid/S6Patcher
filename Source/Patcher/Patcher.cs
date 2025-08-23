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

        public Patcher(Forms.mainFrm Base, execID ID, FileStream Stream)
        {
            if (ID == execID.NONE || Stream == null || Stream.CanWrite == false)
            {
                throw new ArgumentException("Erroneous arguments to Patcher ctor.");
            }

            GlobalID = ID;
            GlobalStream = Stream;
            GlobalMod = new Mod(Base, ID, Stream);
            GlobalMappings = MappingBase.GetMappingsByID(GlobalID);

            Logger.Instance.Log("Patcher ctor(): ID: " + GlobalID.ToString() + ", Stream: " + GlobalStream.Name);
        }

        public void PatchByControlFeatures(List<string> Names)
        {
            List<Dictionary<long, byte[]>> Results = 
                [.. from Entry in GlobalMappings.GetMapping()
                    from Name in Names
                    where Name == Entry.Name
                    select Entry.Mapping];

            foreach (var Element in Results)
            {
                WriteMappingToFile(Element);
            }
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

            WriteMappingToFile(GlobalMappings.GetTextureResolutionMapping(Resolution));
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

            WriteMappingToFile(GlobalMappings.GetAutoSaveMapping(Timer));
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

            WriteMappingToFile(GlobalMappings.GetZoomLevelMapping(Level, Distance));
        }

        public void SetModLoader(bool UseBugFixMod)
        {
            Logger.Instance.Log("SetModLoader(): Called.");
            WriteMappingToFile(GlobalMappings.GetModloaderMapping());
            GlobalMod.CreateModLoader(UseBugFixMod);
        }

        public void SetLargeAddressAwareFlag()
        {
            Logger.Instance.Log("SetLargeAddressAwareFlag(): Called.");

            // Partially adapted from:
            // https://stackoverflow.com/questions/9054469/how-to-check-if-exe-is-set-as-largeaddressaware

            const int IMAGE_FILE_LARGE_ADDRESS_AWARE = 0x20;
            BinaryReader Reader = new(GlobalStream);

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

        public void SetEntryInOptionsFile(string Entry, bool Checked) => IOFileHandler.Instance.UpdateEntryInOptionsFile("[S6Patcher]", Entry, Checked == true ? 1U : 0U);
        public void SetEasyDebug() => WriteMappingToFile(GlobalMappings.GetEasyDebugMapping());
        public void SetLuaScriptBugFixes() => IOFileHandler.Instance.CreateUserScriptFiles();

        private void WriteMappingToFile(Dictionary<long, byte[]> Mapping)
        {
            foreach (var Entry in Mapping)
            {
                WriteBytes(Entry.Key, Entry.Value);
                Logger.Instance.Log("WriteMappingToFile(): Patching Element: 0x" + $"{Entry.Key:X}");
            }
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
