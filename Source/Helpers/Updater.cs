using S6Patcher.Properties;
using System;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Windows.Forms;

namespace S6Patcher.Source.Helpers
{
    internal class Updater
    {
        public async void CheckForUpdates()
        {
            Logger.Instance.Log("CheckForUpdates() called.");
            using WebClient Client = new();
            string Result = String.Empty;
            string Version = Application.ProductVersion;

            try
            {
                Result = await Client.DownloadStringTaskAsync(new Uri(Resources.VersionFileLink));
            }
            catch (Exception ex)
            {
                Logger.Instance.Log(ex.ToString());
                MessageBox.Show("Could not retrieve update information!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.Equals(Version, Result, StringComparison.CurrentCultureIgnoreCase) == false)
            {
                string Message = "A new version is available on GitHub!\n\nCurrent Version: " + Version + "\nNew Version: " + Result;
                Logger.Instance.Log(Message);
                MessageBox.Show(Message, "Update", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                string Message = "You are using the latest version of the S6Patcher!";
                Logger.Instance.Log(Message);
                MessageBox.Show(Message, "Update", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
