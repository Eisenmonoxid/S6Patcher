using S6Patcher.Properties;
using System;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace S6Patcher.Source.Helpers
{
    public sealed class WebHandler
    {
        private static readonly WebHandler _instance = new WebHandler();
        private WebHandler() {ServicePointManager.SecurityProtocol = SecurityProtocolType.SystemDefault;}
        ~WebHandler() {GlobalClient.Dispose();}
        public static WebHandler Instance => _instance;

        private static readonly WebClientWithTimeout GlobalClient = new WebClientWithTimeout(8000) {Encoding = Encoding.UTF8};
        private bool Startup = true;
        private bool EventHandlerRegistered = false;

        public bool DownloadZipArchive(Uri DonwloadURL, string DestinationDirectoryPath)
        {
            try
            {
                GlobalClient.OpenRead(DonwloadURL);
                Int64 Size = Convert.ToInt64(GlobalClient.ResponseHeaders["Content-Length"]);
                string ConvertedSize = (Size / (float)1024).ToString("0.00");

                Logger.Instance.Log("DownloadZipArchive(): Download size: " + ConvertedSize);
                if (MessageBox.Show(Resources.ModDownloadMessage.Replace("%x", ConvertedSize), "Download Bugfix Mod ...",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return false;
                }

                GlobalClient.DownloadFile(DonwloadURL, DestinationDirectoryPath + ".zip");
                // Blocks calling thread until the download is completed
            }
            catch (Exception ex)
            {
                Logger.Instance.Log("DownloadZipArchive():\n" + ex.ToString());
                MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            Logger.Instance.Log("DownloadZipFileCompleted(): Download completed successfully.");
            return true;
        }

        public void CheckForUpdates(bool OnStartup = true)
        {
            Logger.Instance.Log("WebHandler(): CheckForUpdates() called.");
            Startup = OnStartup;

            if (!EventHandlerRegistered)
            {
                GlobalClient.DownloadStringCompleted += Client_DownloadStringCompleted;
                EventHandlerRegistered = true;
            }

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.SystemDefault;
                GlobalClient.DownloadStringAsync(new Uri(Resources.VersionFileLink));
            }
            catch (Exception ex)
            {
                Logger.Instance.Log(ex.ToString());
                if (!Startup)
                {
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Cancelled == false && e.Error == null)
            {
                if (string.Compare(Application.ProductVersion, e.Result, true) != 0)
                {
                    string Message = "A newer version of the S6Patcher is available on GitHub!\n\nCurrent Version: " + Application.ProductVersion + "\nNew Version: " + e.Result;
                    Logger.Instance.Log(Message);
                    MessageBox.Show(Message, "Update", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    string Message = "You are using the latest version of the S6Patcher!";
                    Logger.Instance.Log(Message);
                    if (!Startup)
                    {
                        MessageBox.Show(Message, "Update", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            else
            {
                string Message = "Could not retrieve update information!";
                Logger.Instance.Log(Message);
                if (!Startup)
                {
                    MessageBox.Show(Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                };
            }
        }
    }

    public class WebClientWithTimeout : WebClient
    {
        private readonly int Timeout;
        public WebClientWithTimeout(int Time = 30000)
        {
            Timeout = Time;
        }

        protected override WebRequest GetWebRequest(Uri URL)
        {
            WebRequest Request = base.GetWebRequest(URL);
            Request.Timeout = Timeout;
            return Request;
        }
    }
}
