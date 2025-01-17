using S6Patcher.Properties;
using S6Patcher.Source.Helpers;
using System;
using System.Diagnostics;
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
            OpenLink(Resources.GithubMainLink, (LinkLabel)sender);
        }
        private void lblDC_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenLink(Resources.DiscordMainLink, (LinkLabel)sender);
        }
        private void lblFeatures_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenLink(Resources.FeaturesLink, (LinkLabel)sender);
        }
        private void OpenLink(string Link, LinkLabel CurrentLabel)
        {
            var Info = new ProcessStartInfo(Link);
            Process.Start(Info);
            CurrentLabel.LinkVisited = true;
        }
    }
}
