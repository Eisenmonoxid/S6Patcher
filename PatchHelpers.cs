﻿using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace S6Patcher
{
    internal class PatchHelpers
    {
        public static bool ApplyOffset = false;
        public static void WriteBytesToFile(ref FileStream Stream, long position, byte[] replacementBytes)
        {
            if (ApplyOffset == false)
            {
                Stream.Position = position;
            }
            else
            {
                Stream.Position = (position - 0x3F0000);
            }
            
            try
            {
                Stream.Write(replacementBytes, 0, replacementBytes.Length);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        public static FileStream OpenFileStream(string filePath, execID Identifier)
        {
            FileStream Stream;
            try
            {
                Stream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return null;
            }

            if (CheckExecVersion(ref Stream, Identifier) == false)
            {
                Stream.Close();
                Stream.Dispose();
                return null;
            }

            return Stream;
        }
        public static OpenFileDialog CreateOFDialog()
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                CheckFileExists = true,
                ShowHelp = false,
                CheckPathExists = true,
                DereferenceLinks = true,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                Multiselect = false,
                ShowReadOnly = false,
            };

            return ofd;
        }
        public static bool CreateBackup(string Filepath)
        {
            string FileName = Path.GetFileNameWithoutExtension(Filepath);
            string DirectoryPath = Path.GetDirectoryName(Filepath);
            string FinalPath = Path.Combine(DirectoryPath, FileName + "_BACKUP.exe");

            if (File.Exists(FinalPath) == false)
            {
                try
                {
                    File.Copy(Filepath, FinalPath, false);
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return true;
        }
        public static bool CheckExecVersion(ref FileStream Reader, execID Identifier, bool UseOffset = false)
        {
            string ExpectedVersion = "1, 71, 4289, 0";
            UInt32[] Mapping = {0x6ECADC, 0xF531A4, 0x6D06A8};
            byte[] Result = new byte[30];

            if (UseOffset == true)
            {
                UInt32 Offset = 0x3F0000;
                UInt32 ShiftedPosition = (Mapping[(char)Identifier] - Offset);
                Reader.Position = ShiftedPosition;
            }
            else
            {
                Reader.Position = Mapping[(char)Identifier];
            }

            Reader.Read(Result, 0, 30);

            string Version = Encoding.Unicode.GetString(Result).Substring(0, ExpectedVersion.Length);
            if (Version == ExpectedVersion)
            {
                return true;
            }
            else if (UseOffset == false && Identifier == execID.OV) // Try again with offset applied
            {
                if (CheckExecVersion(ref Reader, Identifier, true) == true)
                {
                    ApplyOffset = true;
                    return true;
                }
                else
                { 
                    return false; 
                }
            }
            else
            {
                MessageBox.Show("Erwartet/Expected: " + ExpectedVersion + "\nGelesen/Read: " + Version, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
        }
    }
}
