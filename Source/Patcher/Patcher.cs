using S6Patcher.Properties;
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
        private FileStream GlobalStream = null;
        private readonly MappingBase GlobalMappings = null;
        private readonly execID GlobalID = execID.NONE;
        public Patcher(execID ID, ref FileStream Stream)
        {
            if (ID == execID.NONE || Stream == null)
            {
                throw new ArgumentException("Missing arguments to Patcher ctor.");
            }

            GlobalID = ID;
            GlobalStream = Stream;
            GlobalMappings = MappingBase.GetMappingsByID(GlobalID);

            Logger.Instance.Log("Patcher ctor(): ID: " + GlobalID.ToString() + ", Stream: " + GlobalStream.Name);
        }
        public void PatchByControlFeatures(List<string> Names)
        {
            List<MappingBase.PatchEntry> Entries = GlobalMappings.GetMapping();
            if (Entries == null)
            {
                return;
            }

            var Result = (from Entry in Entries
                         from Name in Names
                         where Name == Entry.Name
                         select Entry.Mapping).ToList();

            Result.ForEach(Element =>
            {
                foreach (var Entry in Element)
                {
                    Helpers.Helpers.WriteBytes(ref GlobalStream, Entry.Key, Entry.Value);
                    Logger.Instance.Log("PatchByControlFeatures(): Patching Element: " + Entry.Key.ToString());
                }
            });
        }
        public void SetHighResolutionTextures(string ResolutionText)
        {
            Logger.Instance.Log("SetHighResolutionTextures(): Called with " + ResolutionText);

            UInt32 Resolution;
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

            UInt32[] Mapping = GlobalMappings.GetTextureResolutionMapping();
            for (uint i = 0; i < Mapping.Length; i++)
            {
                Helpers.Helpers.WriteBytes(ref GlobalStream, Mapping[i], BitConverter.GetBytes(Resolution / Convert.ToUInt32(Math.Pow(2, i))));
            }
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

            byte[] Interval = BitConverter.GetBytes(Timer * 60000);
            UInt32[] Mapping = GlobalMappings.GetAutoSaveMapping();

            Helpers.Helpers.WriteBytes(ref GlobalStream, Mapping[0], (Timer == 0.0) ? new byte[] {0xEB} : new byte[] {0x76});
            Helpers.Helpers.WriteBytes(ref GlobalStream, Mapping[1], Interval);
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
                Helpers.Helpers.WriteBytes(ref GlobalStream, Element.Key, Element.Value);
            }
        }
        public void SetModLoader()
        {
            Logger.Instance.Log("SetModLoader(): Called.");

            Modloader Loader = new Modloader();
            Dictionary<long, byte[]> Entries = Loader.GetMappingsByID(GlobalID);
            if (Entries == null)
            {
                Logger.Instance.Log("SetModLoader(): Entries was zero!");
                return;
            }

            foreach (var DictEntry in Entries)
            {
                Helpers.Helpers.WriteBytes(ref GlobalStream, DictEntry.Key, DictEntry.Value);
            }

            char Separator = Path.DirectorySeparatorChar;
            string ModPath = Helpers.Helpers.GetRootPathFromFile(GlobalStream.Name, GlobalID);
            ModPath += (Separator + "modloader");

            if (Directory.Exists(ModPath) == false)
            {
                try
                {
                    Directory.CreateDirectory(ModPath);
                    Logger.Instance.Log("SetModLoader(): Directory created " + ModPath);
                }
                catch (Exception ex)
                {
                    Logger.Instance.Log(ex.ToString());
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                Logger.Instance.Log("SetModLoader(): Folder " + ModPath + " found! Returning ...");
                return;
            }

            if (GlobalID == execID.HE_UBISOFT || GlobalID == execID.HE_STEAM)
            {
                ModPath = ModPath + Separator + "shr";
                Directory.CreateDirectory(ModPath);
                Logger.Instance.Log("SetModLoader(): Directory created " + ModPath);
            }
            else
            {
                ModPath = ModPath + Separator + "bba" + Separator;
                Directory.CreateDirectory(ModPath);
                try
                {
                    File.WriteAllBytes(ModPath + "mod.bba", Resources.mod);
                    Logger.Instance.Log("SetModLoader(): Written mod.bba to Path " + ModPath);
                }
                catch (Exception ex)
                {
                    Logger.Instance.Log(ex.ToString());
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
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
                Helpers.Helpers.WriteBytes(ref GlobalStream, CurrentPosition, BitConverter.GetBytes(Flag |= IMAGE_FILE_LARGE_ADDRESS_AWARE));
            }
        }
        public void SetLuaScriptBugFixes()
        {
            Logger.Instance.Log("SetLuaScriptBugFixes(): Called.");

            // EMXBinData.s6patcher is the minified and compiled main menu script
            string[] ScriptFiles = {"UserScriptLocal.lua", "EMXBinData.s6patcher"};
            Helpers.Helpers.GetUserScriptDirectories().ForEach(Element =>
            {
                string ScriptPath = Path.Combine(Element, "Script");
                try
                {
                    Directory.CreateDirectory(ScriptPath);
                    File.WriteAllBytes(Path.Combine(ScriptPath, ScriptFiles[0]), Resources.UserScriptLocal);
                    File.WriteAllBytes(Path.Combine(ScriptPath, ScriptFiles[1]), Resources.EMXBinData);
                }
                catch (Exception ex)
                {
                    Logger.Instance.Log(ex.ToString());
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                Logger.Instance.Log("SetLuaScriptBugFixes(): Finished writing Scriptfiles to " + ScriptPath);
            });
        }
        public void SetKnightSelection(bool Checked)
        {
            Logger.Instance.Log("SetKnightSelection(): Called with " + Checked.ToString());
            IOFileHandler.Instance.UpdateEntryInOptionsFile("S6Patcher", "ExtendedKnightSelection", Checked);
        }
    }
}
