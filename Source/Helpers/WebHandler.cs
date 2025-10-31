using S6Patcher.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace S6Patcher.Source.Helpers
{
    public sealed class WebHandler
    {
        private WebHandler() {}
        public void Dispose() {GlobalClient.Dispose();}
        public static WebHandler Instance {get;} = new();

        private readonly HttpClient GlobalClient = new()
        {
            Timeout = TimeSpan.FromMilliseconds(8000),
            DefaultRequestHeaders = {{"User-Agent", "Other"}},
        };

        private readonly Uri GlobalMod = new(Resources.RepoBasePath + "Gamefiles/Modfiles.zip");
        private readonly Uri GlobalVersion = new(Resources.VersionFileLink);

        public async Task<string> GetModfileDownloadSize()
        {
            string DownloadSize;
            try
            {
                var Response = await GlobalClient.GetAsync(GlobalMod, HttpCompletionOption.ResponseHeadersRead);
                Response.EnsureSuccessStatusCode();
                long Size = Response.Content.Headers.ContentLength ?? 0;
                DownloadSize = (Size / (float)1024).ToString("0.00");
            }
            catch (Exception ex)
            {
                Logger.Instance.Log(ex.ToString());
                return string.Empty;
            }

            Logger.Instance.Log("Download size: " + DownloadSize);
            return DownloadSize;
        }

        public async Task<List<MemoryStream>> DownloadScriptFilesAsync(Uri[] Paths)
        {
            List<MemoryStream> Elements = [];
            foreach (var Element in Paths)
            {
                try
                {
                    var Response = await GlobalClient.GetAsync(Element, HttpCompletionOption.ResponseHeadersRead);
                    Response.EnsureSuccessStatusCode();
                    using Stream Stream = await Response.Content.ReadAsStreamAsync();
                    MemoryStream MemStream = new();
                    await Stream.CopyToAsync(MemStream);
                    MemStream.Position = 0;
                    Elements.Add(MemStream);
                }
                catch (Exception ex)
                {
                    Logger.Instance.Log(ex.ToString());
                    return null;
                }
            }

            Logger.Instance.Log("Downloaded ScriptFiles successfully.");
            return Elements;
        }

        public async Task<bool> DownloadZipArchiveAsync(string DestinationDirectoryPath)
        {
            try
            {
                var Response = await GlobalClient.GetAsync(GlobalMod, HttpCompletionOption.ResponseHeadersRead);
                Response.EnsureSuccessStatusCode();
                using Stream Stream = await Response.Content.ReadAsStreamAsync();
                using FileStream FileStream = File.Create(DestinationDirectoryPath + ".zip");
                await Stream.CopyToAsync(FileStream);
            }
            catch (Exception ex)
            {
                Logger.Instance.Log(ex.ToString());
                return false;
            }

            Logger.Instance.Log("Downloaded ModFiles successfully.");
            return true;
        }

        public async Task<string> CheckForUpdatesAsync(bool OnStartup = true)
        {
            Logger.Instance.Log("With " + OnStartup.ToString() + " called.");

            string WebVersion;
            try
            {
                WebVersion = await GlobalClient.GetStringAsync(GlobalVersion);
            }
            catch (Exception ex)
            {
                Logger.Instance.Log(ex.ToString());
                return !OnStartup ? ex.Message : string.Empty;
            }

            string LocalVersion = Utility.GetApplicationVersion();
            if (string.Compare(LocalVersion, WebVersion, true) != 0)
            {
                return "A newer version of the S6Patcher is available on GitHub!\n" +
                    "\nCurrent Version: " + LocalVersion + "\nNew Version: " + WebVersion +
                    "\n\n" + Resources.VersionFileLink[..Resources.VersionFileLink.IndexOf("/raw")];
            }
            else
            {
                return !OnStartup ? "You are using the latest version of the S6Patcher!" : string.Empty;
            }
        }
    }
}
