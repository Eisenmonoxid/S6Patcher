using S6Patcher.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace S6Patcher.Source.Utilities
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

        private readonly Uri GlobalVersion = new(Resources.VersionFileLink);

        private async Task<HttpResponseMessage> GetHttpResponse(Uri Path)
        {
            try
            {
                var Response = await GlobalClient.GetAsync(Path, HttpCompletionOption.ResponseHeadersRead);
                Response.EnsureSuccessStatusCode();
                return Response;
            }
            catch (Exception ex)
            {
                ErrorTracking.Increment();
                Logger.Instance.Log(ex.ToString());
                return null;
            }
        }

        public async Task<List<MemoryStream>> DownloadFilesAsync(Uri[] Paths)
        {
            var Tasks = Paths.Select(async Path =>
            {
                Logger.Instance.Log("Downloading Repository File: " + Path.ToString());

                HttpResponseMessage Response = await GetHttpResponse(Path);
                if (Response == null)
                {
                    Logger.Instance.Log("Failed to download Repository File: " + Path.ToString());
                    return null;
                }

                await using var Stream = await Response.Content.ReadAsStreamAsync();
                var Memory = new MemoryStream();
                await Stream.CopyToAsync(Memory);
                Memory.Seek(0, SeekOrigin.Begin);
                return Memory;
            });

            var Finished = await Task.WhenAll(Tasks);
            var FinalList = Finished.Where(Element => Element != null).ToList();
            Logger.Instance.Log("Downloaded Repository File successfully. Size: " + FinalList.Count);

            return FinalList;
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
