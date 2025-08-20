using S6Patcher.Properties;
using S6Patcher.Source.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace S6Patcher.Source.Patcher
{
    internal class Mod(Forms.mainFrm GlobalBaseForm, execID GlobalID, string GlobalDestinationDirectoryPath)
    {
        private readonly Uri GlobalDownloadURL = new(Resources.ModLink);
        private readonly string ArchiveFilePathBase = Path.Combine(GlobalDestinationDirectoryPath, "base");
        private readonly string ArchiveFilePathExtra1 = Path.Combine(GlobalDestinationDirectoryPath, "extra1");
        private readonly string BaseDirectoryPath = Path.Combine(GlobalDestinationDirectoryPath, "shr");
        private const string ArchiveFileName = "mod.bba";

        private void ExtractZipArchive(string ZipPath)
        {
            try
            {
                using ZipArchive Archive = ZipFile.OpenRead(ZipPath);
                if (GlobalID == execID.HE_UBISOFT || GlobalID == execID.HE_STEAM)
                {
                    List<ZipArchiveEntry> Entries = [.. from Entry in Archive.Entries
                                   where !Entry.FullName.Contains(ArchiveFileName)
                                   where !String.IsNullOrEmpty(Entry.Name)
                                   select Entry];

                    foreach (ZipArchiveEntry Entry in Entries)
                    {
                        string FullPath = Path.Combine(BaseDirectoryPath, Path.GetDirectoryName(Entry.FullName));
                        if (Directory.Exists(FullPath) == false)
                        {
                            Directory.CreateDirectory(FullPath);
                        }
                        Entry.ExtractToFile(Path.Combine(BaseDirectoryPath, Entry.FullName), true);
                        Logger.Instance.Log("ExtractZipArchive(): Extracted " + Path.Combine(BaseDirectoryPath, Entry.FullName));
                    }
                }
                else
                {
                    ZipArchiveEntry Entry = Archive.GetEntry("mod.bba");
                    Entry.ExtractToFile(Path.Combine(ArchiveFilePathBase, ArchiveFileName), true);
                    //Entry.ExtractToFile(Path.Combine(ArchiveFilePathExtra1, ArchiveFileName), true);
                    // Not necessary right now
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Log(ex.ToString());
                GlobalBaseForm.Invoke(new Action(() => MessageBox.Show(ex.ToString(), "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error)));
                return;
            }

            Logger.Instance.Log("ExtractZipArchive(): Successfully extracted " + ZipPath + " to " + GlobalDestinationDirectoryPath);
        }

        private void DownloadZipArchive()
        {
            bool Result = WebHandler.Instance.DownloadZipArchive(GlobalBaseForm, GlobalDownloadURL, GlobalDestinationDirectoryPath);           
            if (Result)
            {
                ExtractZipArchive(GlobalDestinationDirectoryPath + ".zip");
            }

            GlobalBaseForm.Invoke(new Action(GlobalBaseForm.FinishPatchingProcess));
        }

        public bool CreateModLoader(bool UseBugfixMod)
        {
            try
            {
                if (GlobalID == execID.HE_UBISOFT || GlobalID == execID.HE_STEAM)
                {
                    Directory.CreateDirectory(BaseDirectoryPath);
                    Directory.CreateDirectory(Path.Combine(GlobalDestinationDirectoryPath, "base", "shr"));
                    Directory.CreateDirectory(Path.Combine(GlobalDestinationDirectoryPath, "extra1", "shr"));
                    Logger.Instance.Log("SetModLoader(): Directories created.");
                }
                else
                {
                    Directory.CreateDirectory(ArchiveFilePathBase);
                    Directory.CreateDirectory(ArchiveFilePathExtra1);
                    // Always do this in case the download fails or user cancels
                    File.WriteAllBytes(Path.Combine(ArchiveFilePathBase, ArchiveFileName), Resources.mod);
                    File.WriteAllBytes(Path.Combine(ArchiveFilePathExtra1, ArchiveFileName), Resources.mod);
                    Logger.Instance.Log("SetModLoader(): Written " + ArchiveFileName + " to Paths "
                        + ArchiveFilePathBase + " and " + ArchiveFilePathExtra1);

                    string OldModLoaderPath = Path.Combine(GlobalDestinationDirectoryPath, "bba");
                    if (Directory.Exists(OldModLoaderPath))
                    {
                        Directory.Delete(OldModLoaderPath, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Log(ex.ToString());
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (UseBugfixMod)
            {
                Thread Context = new(DownloadZipArchive)
                {
                    IsBackground = true
                };
                Context.Start();
            }

            return true;
        }
    }
}
