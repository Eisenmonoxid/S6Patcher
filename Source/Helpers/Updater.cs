using S6Patcher.Properties;
using System;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace S6Patcher.Source.Helpers
{
    internal class Updater
    {
        public void CheckForUpdates()
        {
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
                    MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    MessageBox.Show("A new version is available on GitHub!\n\nCurrent Version: " + Application.ProductVersion + "\nNew Version: " + e.Result, "Update", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("You are using the latest version of the S6Patcher!", "Update", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Error: Could not retrieve update information!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
