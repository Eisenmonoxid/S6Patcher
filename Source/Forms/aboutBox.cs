using S6Patcher.Properties;
using S6Patcher.Source.Helpers;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace S6Patcher.Source.Forms
{
    partial class aboutBox : Form
    {
        public aboutBox()
        {
            InitializeComponent();
            lblAbout.Text = "S6Patcher by " + Application.CompanyName + " - Version " + Application.ProductVersion;
        }
        private void btnCheckUpdate_Click(object sender, EventArgs e)
        {
            Updater Updater = new Updater();
            Updater.CheckForUpdates();
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }
        private void lblGit_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Clipboard.SetText(Resources.GithubMainLink);
            }
            else
            {
                OpenLink(Resources.GithubMainLink, (LinkLabel)sender);
            }
        }
        private void lblDC_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Clipboard.SetText(Resources.DiscordMainLink);
            }
            else
            {
                OpenLink(Resources.DiscordMainLink, (LinkLabel)sender);
            }
        }
        private void lblFeatures_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                Clipboard.SetText(Resources.FeaturesLink);
            }
            else
            {
                OpenLink(Resources.FeaturesLink, (LinkLabel)sender);
            }
        }
        private void OpenLink(string Link, LinkLabel CurrentLabel)
        {
            Logger.Instance.Log("OpenLink(): " + Link);
            ProcessStartInfo Info = new ProcessStartInfo(Link)
            {
                UseShellExecute = true
            };

            try
            {
                Process.Start(Info);
            }
            catch (Exception ex)
            {
                Logger.Instance.Log(ex.ToString());
                MessageBox.Show(ex.ToString());
            }

            this.WindowState = FormWindowState.Minimized;
            CurrentLabel.LinkVisited = true;
        }
        private void aboutBox_Load(object sender, EventArgs e)
        {
            try
            {
                using (Stream IconStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("S6Patcher.Source.Resources.favicon.ico"))
                {
                    this.Icon = new System.Drawing.Icon(IconStream);
                    this.ShowIcon = true;
                }
            }
            catch {}
        }
    }
}
