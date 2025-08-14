using S6Patcher.Properties;
using S6Patcher.Source.Helpers;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace S6Patcher.Source.Patcher
{
    internal class Mod
    {
        private readonly Uri GlobalDownloadURL = new Uri(Resources.ModLink);
        private readonly string GlobalDestinationDirectoryPath;
        private readonly string ArchiveFilePathBase;
        private readonly string ArchiveFilePathExtra1;
        private readonly string BaseDirectoryPath;
        private const string ArchiveFileName = "mod.bba";

        private readonly execID GlobalID = execID.NONE;
        private readonly FileStream GlobalStream = null;
        private readonly Forms.mainFrm GlobalBaseForm;

        public Mod(Forms.mainFrm Base, execID ID, FileStream Stream)
        {
            GlobalBaseForm = Base;
            GlobalID = ID;
            GlobalStream = Stream;
            GlobalDestinationDirectoryPath = GetModloaderPath();
            ArchiveFilePathBase = Path.Combine(GlobalDestinationDirectoryPath, "base");
            ArchiveFilePathExtra1 = Path.Combine(GlobalDestinationDirectoryPath, "extra1");
            BaseDirectoryPath = Path.Combine(GlobalDestinationDirectoryPath, "shr");
        }

        private void ExtractZipArchive(string ZipPath)
        {
            try
            {
                using (ZipArchive Archive = ZipFile.OpenRead(ZipPath))
                {
                    if (GlobalID == execID.HE_UBISOFT || GlobalID == execID.HE_STEAM)
                    {
                        var Entries = (from Entry in Archive.Entries
                                      where !Entry.FullName.Contains(ArchiveFileName)
                                      where !String.IsNullOrEmpty(Entry.Name)
                                      select Entry);

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
                        Entry.ExtractToFile(Path.Combine(ArchiveFilePathExtra1, ArchiveFileName), true);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Log(ex.ToString());
                GlobalBaseForm.Invoke(new Action(() => MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)));
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

            GlobalBaseForm.Invoke(new Action(() => GlobalBaseForm.FinishPatchingProcess()));
        }

        public string GetModloaderPath()
        {
            uint Depth = (GlobalID == execID.OV || GlobalID == execID.OV_OFFSET) ? 2U : 3U;
            return IOFileHandler.Instance.GetRootDirectory(GlobalStream.Name, Depth) + Path.DirectorySeparatorChar + "modloader";
        }

        public void CreateModLoader(bool UseBugfixMod)
        {
            try
            {
                if (GlobalID == execID.HE_UBISOFT || GlobalID == execID.HE_STEAM)
                {
                    Directory.CreateDirectory(BaseDirectoryPath);
                    Logger.Instance.Log("SetModLoader(): Directory created " + BaseDirectoryPath);
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
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.Log(ex.ToString());
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (UseBugfixMod)
            {
                Thread Context = new Thread(() => DownloadZipArchive())
                {
                    IsBackground = true
                };
                Context.Start();
            }
        }
    }
}
