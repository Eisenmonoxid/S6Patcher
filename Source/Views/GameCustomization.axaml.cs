using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using MsBox.Avalonia.Enums;
using S6Patcher.Source.Patcher;
using S6Patcher.Source.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace S6Patcher.Source.Views
{
    public partial class GameCustomization : Window
    {
        private bool IsApplicationClosing;

        public GameCustomization() => InitializeComponent();
        private void btnClose_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e) => Hide();

        private void btnSave_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {

        }

        private void btnReset_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {

        }

        public void CloseForApplicationShutdown()
        {
            IsApplicationClosing = true;
            Close();
        }

        protected override void OnClosing(WindowClosingEventArgs e)
        {
            if (IsApplicationClosing)
            {
                base.OnClosing(e);
                return;
            }

            e.Cancel = true;
            Hide();
        }
    }
}
