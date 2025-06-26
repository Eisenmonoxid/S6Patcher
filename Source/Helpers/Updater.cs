using S6Patcher.Properties;
using System;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace S6Patcher.Source.Helpers
{
    internal class Updater
    {
        private readonly bool OnStartup;
        public Updater(bool OnStartup = true)
        {
            this.OnStartup = OnStartup;
        }
        public void CheckForUpdates()
        {
            Logger.Instance.Log("CheckForUpdates() called.");
            using (WebClient Client = new WebClient())
            {
                Client.Encoding = Encoding.UTF8;
                Client.DownloadStringCompleted += Client_DownloadStringCompleted;

                try
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.SystemDefault;
                    Client.DownloadStringAsync(new Uri(Resources.VersionFileLink));
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
                    if (!OnStartup)
                    {
                        MessageBox.Show(Message, "Update", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            else
            {
                string Message = "Could not retrieve update information!";
                Logger.Instance.Log(Message);
                if (!OnStartup)
                {
                    MessageBox.Show(Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                };
            }
        }
    }
}
