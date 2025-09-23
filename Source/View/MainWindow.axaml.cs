using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using S6Patcher.Source.Helpers;
using S6Patcher.Source.Patcher;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace S6Patcher.Source.View
{
    public partial class MainWindow : Window
    {
        private readonly ViewHelpers ViewHelpers;

        private Patcher.Patcher Patcher = null;
        private readonly Dictionary<execID, string[]> Mapping = new()
        {
            {execID.OV, ["tiGeneral", "tiMod", "tiDev"]},
            {execID.HE_STEAM, ["tiGeneral", "tiHistory", "tiMod", "tiDev"]},
            {execID.HE_UBISOFT, ["tiGeneral", "tiHistory", "tiMod", "tiDev"]},
            {execID.ED, ["tiGeneral", "tiEditor", "tiDev", "cbZoom", "cbLimitedEdition", "cbScriptBugFixes",
                "cbEasyDebug", "txtResolution", "txtZoom"]}
        };

        public MainWindow()
        {
            InitializeComponent();
            ViewHelpers = new ViewHelpers(this);
            Backup.ShowMessage += Message => ShowMessageBox("Backup", Message);
            Title = "S6Patcher v" + Utility.GetApplicationVersion() + " - Made by Eisenmonoxid";

            DisableUI();
            GetModfileInformation();
            CheckForUpdates(true);
        }

        private void EnableUIElements(execID ID)
        {
            ViewHelpers.ViewAccessorWrapper(() =>
            {
                var Panel = this.FindControl<HeaderedContentControl>("hccMain");
                Panel.IsEnabled = true;

                ViewHelpers.GetControlsByType<CheckBox>().ToList().ForEach(Result => Result.IsEnabled = true);
                ViewHelpers.GetControlsByType<TextBox>().ToList().ForEach(Result => Result.IsEnabled = true);

                if (Mapping.TryGetValue(ID, out string[] Value))
                {
                    var Controls = ViewHelpers.GetControlsByType<TabItem>();
                    foreach (var Control in Controls.ToArray())
                    {
                        Control.IsEnabled = false;
                    }

                    List<TabItem> TabResults =
                    [.. from Entry in Value
                        from Control in Controls
                        where Control.Name == Entry
                        select Control];

                    TabResults.ForEach(Result => Result.IsEnabled = true);

                    List<Control> ControlResults =
                        [.. from Entry in Value
                            from Control in ViewHelpers.GetControlsByNames(Value)
                            where Control is not TabItem
                            select Control];

                    ControlResults.ForEach(Result => Result.IsEnabled = false);
                }

                btnPatch.IsEnabled = true;
                btnBackup.IsEnabled = true;
            });
        }

        private void DisableUI()
        {
            ViewHelpers.ViewAccessorWrapper(() =>
            {
                tcMain.SelectedIndex = 0;
                foreach (var Element in ViewHelpers.GetControlsByType<CheckBox>())
                {
                    Element.IsChecked = false;
                }

                foreach (var Element in ViewHelpers.GetControlsByType<TabItem>())
                {
                    Element.IsEnabled = false;
                }

                btnPatch.IsEnabled = false;
                btnBackup.IsEnabled = false;
                btnChoose.IsEnabled = true;
                txtPath.Text = "...";

                var Panel = this.FindControl<HeaderedContentControl>("hccMain");
                Panel.IsEnabled = false;
            });
        }

        private async void OpenFilePicker()
        {
            DisableUI();
            ResetPatcher();

            string Path = await ViewHelpers.GetFileFromFilePicker("Choose .exe file", "Settlers6", ViewHelpers.Executable);
            if (string.IsNullOrEmpty(Path))
            {
                txtPath.Text = "...";
                return;
            }

            Path = IOFileHandler.Instance.IsPlayLauncherExecutable(Path);
            txtPath.Text = Path;
            InitializePatcher(Path);
        }

        private async Task GetPathToOptionsFile()
        {
            if (UserScriptHandler.Instance.DoesUserScriptDirectoryExist())
            {
                return;
            }

            string Path = await ViewHelpers.GetFileFromFilePicker("Choose .ini file", "Options", ViewHelpers.Configuration);
            if (!string.IsNullOrEmpty(Path))
            {
                try
                {
                    Path = new DirectoryInfo(System.IO.Path.GetDirectoryName(Path)).Parent.Parent.FullName;
                }
                catch (Exception ex)
                {
                    Logger.Instance.Log(ex.ToString());
                    Path = string.Empty;
                }
            }

            UserScriptHandler.Instance.GlobalDocuments = Path;
        }

        private void ToggleUIAvailability(bool Enable)
        {
            ViewHelpers.ViewAccessorWrapper(() =>
            {
                btnChoose.IsEnabled = Enable;
                btnPatch.IsEnabled = Enable;
                btnBackup.IsEnabled = Enable;
            });
        }

        private void InitializePatcher(string Filepath)
        {
            try
            {
                Patcher = new Patcher.Patcher(Filepath);
            }
            catch (Exception ex)
            {
                ShowMessageBox("Error", ex.Message);
                return;
            }

            Patcher.ShowMessage += Message => ShowMessageBox("Error", Message);
            Patcher.PatchingFinished += Success => FinishPatching(Success);
            EnableUIElements(Patcher.GlobalID);
        }

        private void FinishPatching(bool Success)
        {
            ViewHelpers.ViewAccessorWrapper(() =>
            {
                if (Success)
                {
                    ResetPatcher(RuntimeInformation.IsOSPlatform(OSPlatform.Windows));
                    DisableUI();
                    ShowMessageBox("Success", "Patching finished successfully!");
                }
                else
                {
                    ToggleUIAvailability(true);
                    ShowMessageBox("Error", "Patching failed! Check log file for details.");
                }
            });
        }

        private async void MainPatchingTask()
        {
            ToggleUIAvailability(false);
            await GetPathToOptionsFile();

            try
            {
                PatchByFeatures();
            }
            catch (Exception ex)
            {
                ShowMessageBox("Error", ex.Message);
                Patcher.RaiseFinishedEvent(false);
            }
        }

        private void PatchByFeatures()
        {
            if (cbUpdater.IsChecked == true && Patcher.GlobalID != execID.ED)
            {
                Patcher.WriteScriptFilesToFolder();
                Patcher.SetModLoader(true);
                return;
            }

            var Features = ViewHelpers.GetSelectedFeatures();
            Features.ForEach(Element => Logger.Instance.Log("Selected Feature: " + Element));
            Patcher.PatchByControlFeatures(Features);

            if (Patcher.GlobalID != execID.ED)
            {
                Patcher.WriteScriptFilesToFolder();
            }
            if (cbHighTextures.IsChecked == true && Patcher.GlobalID != execID.ED)
            {
                Patcher.SetTextureResolution(txtResolution.Text);
            }
            if (cbZoom.IsChecked == true)
            {
                Patcher.SetZoomLevel(txtZoom.Text);
            }
            if (cbAutosave.IsChecked == true)
            {
                Patcher.SetAutosaveTimer(txtAutosave.Text);
            }
            if (cbLAAFlag.IsChecked == true)
            {
                Patcher.SetLargeAddressAwareFlag();
            }
            if (cbEasyDebug.IsChecked == true)
            {
                Patcher.SetEasyDebug();
            }
            if (cbModLoader.IsChecked == true)
            {
                Patcher.SetModLoader(cbModDownload.IsChecked == true);
            }
            else
            {
                Patcher.RaiseFinishedEvent(true);
            }
        }

        private void CheckForUpdates(bool Startup)
        {
            WebHandler.Instance.CheckForUpdatesAsync(Startup).ContinueWith((Task) =>
            {
                if (Task.Result != string.Empty)
                {
                    Logger.Instance.Log(Task.Result);
                    ShowMessageBox("Updater", Task.Result);
                }
            });
        }

        private async void GetModfileInformation()
        {
            string Size = await WebHandler.Instance.GetModfileDownloadSize();
            if (string.IsNullOrEmpty(Size))
            {
                Size = "Could not retrieve Data!";
            }
            else
            {
                Size += " KB.";
            }

            lblDownloadSize.Content = lblDownloadSize.Content.ToString().Replace("Fetching ...", Size);
        }

        protected override void OnClosing(WindowClosingEventArgs e)
        {
            ResetPatcher();
            Logger.Instance.Dispose();
            WebHandler.Instance.Dispose();
            base.OnClosing(e);
        }

        private void RestoreBackup()
        {
            ResetPatcher();
            Backup.Restore(txtPath.Text);
            DisableUI();
        }

        private void ShowMessageBox(string Title, string Message) => ViewHelpers.ShowMessageBox(Title, Message);
        private void ResetPatcher(bool FinishWithPEHeader = false)
        {
            Patcher?.Dispose(FinishWithPEHeader);
            Patcher = null;
        }

        private void btnPatch_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e) => MainPatchingTask();
        private void btnBackup_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e) => RestoreBackup();
        private void btnChoose_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e) => OpenFilePicker();
        private void btnUpdate_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e) => CheckForUpdates(false);
        private void btnExit_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e) => Close();
        private void cbUpdater_Checked(object sender, Avalonia.Interactivity.RoutedEventArgs e) => tcMain.IsEnabled = cbUpdater.IsChecked == false;
        private void cbModLoader_IsCheckedChanged(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            cbModDownload.IsEnabled = cbModLoader.IsChecked == true;
            if (cbModLoader.IsChecked == false)
            {
                cbModDownload.IsChecked = false;
            }
        }
    }
}
