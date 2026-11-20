using S6Patcher.Properties;
using System;
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
