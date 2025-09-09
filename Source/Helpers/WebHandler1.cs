using S6Patcher.Properties;
using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace S6Patcher.Source.Helpers
{
    public sealed class WebHandler
    {
        private WebHandler() {}
        ~WebHandler() {GlobalClient.Dispose();}
        public static WebHandler Instance {get;} = new();

        private static readonly HttpClient GlobalClient = new()
        {
            Timeout = TimeSpan.FromMilliseconds(8000),
            DefaultRequestHeaders = {{"User-Agent", "Other"}},
        };

        private static readonly Uri GlobalMod = new(Resources.ModLink);
        private static readonly Uri GlobalVersion = new(Resources.VersionFileLink);

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

            Logger.Instance.Log("GetModfileDownloadSize(): Download size: " + DownloadSize);
            return DownloadSize;
        }

        public async Task<bool> DownloadZipArchiveAsync(Forms.mainFrm BaseForm, string DestinationDirectoryPath)
        {
            DialogResult Result = (DialogResult)BaseForm.Invoke(new Func<DialogResult>(() =>
                MessageBox.Show(Resources.ModDownloadMessage.Replace("%x", GetModfileDownloadSize().Result), "Download Bugfix Mod ...",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question)));

            if (Result != DialogResult.Yes)
            {
                return false;
            }

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
                BaseForm.Invoke(new Action(() => MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)));
                return false;
            }

            Logger.Instance.Log("DownloadZipArchiveAsync(): Download completed successfully.");
            return true;
        }

        public async void CheckForUpdatesAsync(bool OnStartup = true)
        {
            Logger.Instance.Log("CheckForUpdatesAsync() with " + OnStartup.ToString() + " called.");

            string WebVersion;
            try
            {
                WebVersion = await GlobalClient.GetStringAsync(GlobalVersion);
            }
            catch (Exception ex)
            {
                Logger.Instance.Log(ex.ToString());
                if (!OnStartup)
                {
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return;
            }

            string LocalVersion = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>().Version;
            if (string.Compare(LocalVersion, WebVersion, true) != 0)
            {
                string Message = "A newer version of the S6Patcher is available on GitHub!\n\nCurrent Version: " +
                    LocalVersion + "\nNew Version: " + WebVersion;
                Logger.Instance.Log(Message);
                MessageBox.Show(Message, "Update", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                string Message = "You are using the latest version of the S6Patcher!";
                Logger.Instance.Log(Message);
                if (!OnStartup)
                {
                    MessageBox.Show(Message, "Update", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
