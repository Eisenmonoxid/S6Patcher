﻿using IWshRuntimeLibrary;
using Microsoft.Win32.SafeHandles;
using S6Patcher.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace S6Patcher.Source.Helpers
{
    public enum execID
    {
        NONE = 0,
        OV = 1,
        OV_OFFSET = 2,
        HE_UBISOFT = 3,
        HE_STEAM = 4,
        ED = 5
    };

    public static class Helpers
    {
        public static readonly uint GlobalOffset = 0x3F0000;
        public static execID CurrentID = execID.NONE;
        public static string Mono_DocumentsPath = string.Empty;
        public static void WriteBytes(FileStream Stream, long Position, byte[] Bytes)
        {
            Stream.Position = (CurrentID == execID.OV_OFFSET) ? Position - GlobalOffset : Position;
            try
            {
                Stream.Write(Bytes, 0, Bytes.Length);
            }
            catch (Exception ex)
            {
                Logger.Instance.Log(ex.ToString());
            }
        }
        public static List<string> GetUserScriptDirectories()
        {
            // <Documents>/THE SETTLERS - Rise of an Empire/Script/UserScriptGlobal.lua && UserScriptLocal.lua are always loaded by the game when a map is started!
            // EMXBinData.s6patcher is the minified and precompiled MainMenuUserScript.lua
            string DocumentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (Program.IsMono)
            {
                if (Mono_DocumentsPath == string.Empty)
                {
                    MessageBox.Show(Resources.MonoOptionsFile, "Select Options.ini file ...", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    OpenFileDialog ofd = IOFileHandler.Instance.CreateOFDialog("Configuration file|*.ini", Environment.SpecialFolder.MyDocuments);

                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        DirectoryInfo Info = new DirectoryInfo(ofd.FileName);
                        Mono_DocumentsPath = Info.Parent.Parent.Parent.FullName;
                    }
                    else
                    {
                        Logger.Instance.Log("GetUserScriptDirectories(): User did not select a file! Aborting ...");
                        ofd.Dispose();
                        return new List<string>();
                    }
                    ofd.Dispose();
                }

                DocumentsPath = Mono_DocumentsPath;
            }

            return Directory.GetDirectories(DocumentsPath)
                .Where(Element => Element.Contains("Aufstieg eines") || Element.Contains("Rise of an"))
                .Select(Element => {Element = Path.Combine(DocumentsPath, Element); return Element;})
                .ToList();
        }
        public static uint SetCurrentExecutableID(FileStream Stream)
        {
            Logger.Instance.Log("SetCurrentExecutableID(): Called with Stream: " + Stream.Name);

            string ExpectedVersion = "1, 71, 4289, 0";
            int Size = Encoding.Unicode.GetByteCount(ExpectedVersion) + 1;
            byte[] Result = new byte[Size];

            Dictionary<UInt32, execID> Mapping = new Dictionary<UInt32, execID>()
            {
                {0x6ECADC, execID.OV},
                {0x2FCADC, execID.OV_OFFSET},
                {0xF531A4, execID.HE_UBISOFT},
                {0xF545A4, execID.HE_STEAM},
                {0x6D06A8, execID.ED},
            };

            foreach (var Element in Mapping)
            {
                if (Stream.Length < Element.Key)
                {
                    Logger.Instance.Log("SetCurrentExecutableID(): " + Element.Value.ToString() + " Stream Length smaller than Mapping Index: " + Stream.Length.ToString());
                    continue;
                }

                Stream.Position = Element.Key;
                Stream.Read(Result, 0, Result.Length);
                string Version = Encoding.Unicode.GetString(Result).Substring(0, ExpectedVersion.Length);
                Logger.Instance.Log("SetCurrentExecutableID(): Read from File: " + Version);

                if (Version == ExpectedVersion)
                {
                    if (Element.Value == execID.OV || Element.Value == execID.HE_STEAM)
                    {
                        if (IsSteamExecutableValid(Stream, Element.Value) == false)
                        {
                            CurrentID = execID.NONE;
                            Logger.Instance.Log("SetCurrentExecutableID(): Steam Executable has not been unpacked! Aborting ...");
                            return 2;
                        }
                    }

                    CurrentID = Element.Value;
                    Logger.Instance.Log("SetCurrentExecutableID(): Valid executable! execID: " + CurrentID.ToString());
                    return 0;
                };
            }

            CurrentID = execID.NONE;
            Logger.Instance.Log("SetCurrentExecutableID(): NO valid executable was found!");
            return 1;
        }
        public static bool IsSteamExecutableValid(FileStream Stream, execID ID)
        {
            byte[] Identifier = new byte[] {0x84, 0xC0};
            UInt32[] Addresses = new UInt32[] {0x00C044, 0x1E0F08}; // 0 = OV, 1 = Steam HE

            byte[] Result = new byte[Identifier.Length];
            Stream.Position = Addresses[ID == execID.OV ? 0 : 1];
            Stream.Read(Result, 0, Result.Length);

            return Identifier.SequenceEqual(Result);
        }
        public static void CreateDesktopShortcut(string Filepath, string LinkName, string Arguments)
        {
            string Link = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            Link += @"\" + LinkName + " - Patched.lnk";

            try
            {
                WshShell Shell = new WshShell();
                IWshShortcut PatchedShortcut = (IWshShortcut)Shell.CreateShortcut(Link);
                PatchedShortcut.Description = "Launches patched " + LinkName;
                PatchedShortcut.TargetPath = Filepath;
                PatchedShortcut.Arguments = Arguments;
                PatchedShortcut.Save();
            }
            catch (Exception ex)
            {
                Logger.Instance.Log(ex.ToString());
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Logger.Instance.Log("CreateDesktopShortcut(): Creating shortcut successful! Link: " + Link);
        }
        public static string GetRootDirectory(string Filepath, uint Depth)
        {
            Logger.Instance.Log("GetRootPathFromFile(): Called with Depth: " + Depth.ToString() + " and Input: " + Filepath);

            DirectoryInfo Info = new DirectoryInfo(Path.GetDirectoryName(Filepath));
            for (; Depth > 0; Depth--)
            {
                Info = Info.Parent;
            }

            Logger.Instance.Log("GetRootPathFromFile(): Returning Path: " + Info.FullName);
            return Info.FullName;
        }
        public static string IsPlayLauncherExecutable(string Filepath)
        {
            string CurrentPath = Path.GetFileNameWithoutExtension(Filepath);
            if (CurrentPath.StartsWith("Play"))
            {
                Logger.Instance.Log("IsPlayLauncherExecutable(): Found Launcher!");

                CurrentPath = Path.GetDirectoryName(Filepath);
                CurrentPath = Path.Combine(CurrentPath, CurrentPath.Contains("Eastern") ? "extra1" : "base", "bin", "Settlers6.exe");

                if (System.IO.File.Exists(CurrentPath) == true)
                {
                    Logger.Instance.Log("IsPlayLauncherExecutable(): Redirecting Path to " + CurrentPath);
                    Filepath = CurrentPath;
                }
            }

            return Filepath;
        }

        [DllImport("imagehlp.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CheckSumMappedFile(SafeMemoryMappedViewHandle BaseAddress, uint FileLength, ref uint HeaderSum, ref uint CheckSum);

        public static uint UpdatePEHeaderFileCheckSum(string Path, long Size)
        {
            Logger.Instance.Log("UpdatePEHeaderFileCheckSum(): Called.");

            uint CheckSum = 0x0;
            uint HeaderSum = 0x0;
            using (MemoryMappedFile Mapping = MemoryMappedFile.CreateFromFile(Path))
            {
                using (MemoryMappedViewAccessor View = Mapping.CreateViewAccessor())
                {
                    CheckSumMappedFile(View.SafeMemoryMappedViewHandle, (uint)Size, ref HeaderSum, ref CheckSum);
                };
            };

            Logger.Instance.Log("UpdatePEHeaderFileCheckSum(): Finished successfully. New CheckSum: " + CheckSum.ToString());
            return CheckSum;
        }
    }
}
