﻿using S6Patcher.Properties;
using S6Patcher.Source.Helpers;
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
        private readonly execID GlobalID = execID.NONE;
        public Patcher(execID ID, ref FileStream Stream)
        {
            if (ID == execID.NONE || Stream == null)
            {
                throw new ArgumentException("Missing arguments to Patcher ctor.");
            }

            GlobalID = ID;
            GlobalStream = Stream;

            Logger.Instance.Log("Patcher ctor(): ID: " + GlobalID.ToString() + ", Stream: " + GlobalStream.Name);
        }
        public void PatchByControlFeatures(List<string> Names)
        {
            Mappings Mappings = new Mappings();
            List<Mappings.PatchEntry> Entries = Mappings.GetMappingsByID(GlobalID);
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

            byte[] HighResolution = BitConverter.GetBytes(Resolution);
            byte[] MediumResolution = BitConverter.GetBytes(Resolution / 2);
            byte[] LowResolution = BitConverter.GetBytes(Resolution / 4);

            if (GlobalID == execID.OV || GlobalID == execID.OV_OFFSET)
            {
                Helpers.Helpers.WriteBytes(ref GlobalStream, 0x2BE177, HighResolution);
                Helpers.Helpers.WriteBytes(ref GlobalStream, 0x2BE17E, MediumResolution);
                Helpers.Helpers.WriteBytes(ref GlobalStream, 0x2BE185, LowResolution);
            }
            else if (GlobalID == execID.HE_UBISOFT)
            {
                Helpers.Helpers.WriteBytes(ref GlobalStream, 0x2D4188, HighResolution);
                Helpers.Helpers.WriteBytes(ref GlobalStream, 0x2D418F, MediumResolution);
                Helpers.Helpers.WriteBytes(ref GlobalStream, 0x2D4196, LowResolution);
            }
            else if (GlobalID == execID.HE_STEAM)
            {
                Helpers.Helpers.WriteBytes(ref GlobalStream, 0x2D4D74, HighResolution);
                Helpers.Helpers.WriteBytes(ref GlobalStream, 0x2D4D7B, MediumResolution);
                Helpers.Helpers.WriteBytes(ref GlobalStream, 0x2D4D82, LowResolution);
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
            switch (GlobalID)
            {
                case execID.HE_STEAM:
                    Helpers.Helpers.WriteBytes(ref GlobalStream, 0x1C6045, (Timer == 0.0) ? new byte[] {0xEB} : new byte[] {0x76});
                    Helpers.Helpers.WriteBytes(ref GlobalStream, 0xEB95C0, Interval);
                    break;
                case execID.HE_UBISOFT:
                    Helpers.Helpers.WriteBytes(ref GlobalStream, 0x1C5F2A, (Timer == 0.0) ? new byte[] {0xEB} : new byte[] {0x76});
                    Helpers.Helpers.WriteBytes(ref GlobalStream, 0xEB83C0, Interval);
                    break;
                default:
                    return;
            }
        }
        public void SetZoomLevel(string ZoomText)
        {
            Logger.Instance.Log("SetZoomLevel(): Called with " + ZoomText);

            float Offset = 4800;
            float TransitionFactor = 3700;

            if (GlobalID == execID.OV || GlobalID == execID.OV_OFFSET)
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
                    Logger.Instance.Log(ex.ToString());
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Helpers.Helpers.WriteBytes(ref GlobalStream, 0x545400, BitConverter.GetBytes(zoomLevel));
                Helpers.Helpers.WriteBytes(ref GlobalStream, 0x2B334E, new byte[] {0xC7, 0x45, 0x64});
                Helpers.Helpers.WriteBytes(ref GlobalStream, 0x2B3351, BitConverter.GetBytes(clutterFarDistance + Offset));
                Helpers.Helpers.WriteBytes(ref GlobalStream, 0x2B3355, new byte[] {0xC7, 0x45, 0x6C});
                Helpers.Helpers.WriteBytes(ref GlobalStream, 0x2B3358, BitConverter.GetBytes(TransitionFactor));
                Helpers.Helpers.WriteBytes(ref GlobalStream, 0x2B335C, new byte[] {0x90, 0x90});
                Helpers.Helpers.WriteBytes(ref GlobalStream, 0x27AC99, new byte[] {0x50, 0xB8, 0x00, 0x40, 0x9C, 0xC5, 0x89, 0x81,
                        0x9C, 0x00, 0x00, 0x00, 0x58, 0xC6, 0x81, 0x98, 0x00, 0x00, 0x00, 0x01, 0xC2, 0x08, 0x00}); // Restriction -5000.0
            }
            else
            {
                float zoomLevel;

                try
                {
                    zoomLevel = Convert.ToSingle(ZoomText);
                }
                catch (Exception ex)
                {
                    Logger.Instance.Log(ex.ToString());
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Helpers.Helpers.WriteBytes(ref GlobalStream, (GlobalID == execID.HE_STEAM) ? 0xC4F9EC : 0xC4EC4C, BitConverter.GetBytes(zoomLevel));
                Helpers.Helpers.WriteBytes(ref GlobalStream, (GlobalID == execID.HE_STEAM) ? 0x270E73 : 0x270311, new byte[] {0xC7, 0x45, 0xF0});
                Helpers.Helpers.WriteBytes(ref GlobalStream, (GlobalID == execID.HE_STEAM) ? 0x270E76 : 0x270314, BitConverter.GetBytes(zoomLevel + Offset));
                Helpers.Helpers.WriteBytes(ref GlobalStream, (GlobalID == execID.HE_STEAM) ? 0x270E7A : 0x270318, new byte[] {0xC7, 0x45, 0xF4});
                Helpers.Helpers.WriteBytes(ref GlobalStream, (GlobalID == execID.HE_STEAM) ? 0x270E7D : 0x27031B, BitConverter.GetBytes(TransitionFactor));
                Helpers.Helpers.WriteBytes(ref GlobalStream, (GlobalID == execID.HE_STEAM) ? 0x270E81 : 0x27031F, new byte[] {0x90});
                Helpers.Helpers.WriteBytes(ref GlobalStream, (GlobalID == execID.HE_STEAM) ? 0x270E87 : 0x270325, new byte[] {0x90, 0x90, 0x90});
                Helpers.Helpers.WriteBytes(ref GlobalStream, (GlobalID == execID.HE_STEAM) ? 0x2540B8 : 0x2532F7, new byte[] {0x50, 0xB8, 0x00, 0x40, 0x9C, 0xC5, 0x89, 0x81,
                        0x9C, 0x00, 0x00, 0x00, 0x58, 0xC6, 0x81, 0x98, 0x00, 0x00, 0x00, 0x01, 0x90, 0x90}); // Restriction -5000.0
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
            ModPath = ModPath + Separator + "modloader";
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
            List<string> Directories = Helpers.Helpers.GetUserScriptDirectories();
            try
            {
                string ScriptPath = String.Empty;
                foreach (string Element in Directories)
                {
                    ScriptPath = Path.Combine(Element, "Script");
                    if (Directory.Exists(ScriptPath) == false)
                    {
                        Directory.CreateDirectory(ScriptPath);
                        Logger.Instance.Log("SetLuaScriptBugFixes(): Created directory " + ScriptPath);
                    }

                    File.WriteAllBytes(Path.Combine(ScriptPath, ScriptFiles[0]), Resources.UserScriptLocal);
                    File.WriteAllBytes(Path.Combine(ScriptPath, ScriptFiles[1]), Resources.EMXBinData);
                    Logger.Instance.Log("SetLuaScriptBugFixes(): Written Scriptfiles to " + ScriptPath);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Log(ex.ToString());
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void SetKnightSelection(bool Checked)
        {
            Logger.Instance.Log("SetKnightSelection(): Called with " + Checked.ToString());
            IOFileHandler.Instance.UpdateEntryInOptionsFile("S6Patcher", "ExtendedKnightSelection", Checked);
        }
    }
}