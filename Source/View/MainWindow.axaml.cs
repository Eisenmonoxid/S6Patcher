using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using S6Patcher.Source.Helpers;
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
        private bool PatchingInProgress = false;
        private bool IsFilePickerActive = false;
        private readonly ViewHelpers ViewHelpers;
        private Patcher.Patcher MainPatcher = null;

        private readonly Dictionary<execID, string[]> Mapping = new()
        {
            {execID.OV,         ["tiGeneral", "tiMod", "tiDev"]},
            {execID.HE_STEAM,   ["tiGeneral", "tiHistory", "tiMod", "tiDev"]},
            {execID.HE_UBISOFT, ["tiGeneral", "tiHistory", "tiMod", "tiDev"]},
            {execID.ED,         ["tiGeneral", "tiEditor", "tiDev", "cbZoom", "cbLimitedEdition", "cbScriptBugFixes",
                                 "cbEasyDebug", "txtResolution", "txtZoom"]}
        };

        public MainWindow()
        {
            InitializeComponent();

            ViewHelpers = new ViewHelpers(this);
            Title = "S6Patcher v" + Utility.GetApplicationVersion() + " - Made by Eisenmonoxid";
            Backup.ShowMessage += async Message => await ShowMessageBox("Backup", Message);

            DisableUI();
            GetModfileInformation();
            ViewHelpers.CheckForUpdates(true);
        }

        private void EnableUIElements(execID ID)
        {
            ViewHelpers.ViewAccessorWrapper(() =>
            {
                var Panel = this.FindControl<HeaderedContentControl>("hccMain");
                Panel?.IsEnabled = true;

                Panel = this.FindControl<HeaderedContentControl>("hccUpdater");
                Panel?.IsEnabled = true;

                ViewHelpers.GetControlsByType<CheckBox>().ToList().ForEach(Result => Result.IsEnabled = true);
                cbModDownload.IsEnabled = false;
                
                if (ID == execID.ED)
                {
                    cbUpdater.IsEnabled = false;
                }

                if (Mapping.TryGetValue(ID, out string[] Value))
                {
                    var Names = new HashSet<string>(Value);
                    foreach (var Element in ViewHelpers.GetControlsByType<TabItem>())
                    {
                        Element.IsEnabled = Names.Contains(Element.Name);
                    }

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

                ViewHelpers.GetControlsByType<CheckBox>().ToList().ForEach(Element => Element.IsChecked = false);
                ViewHelpers.GetControlsByType<TabItem>().ToList().ForEach(Element => Element.IsEnabled = false);
                
                btnPatch.IsEnabled = false;
                btnBackup.IsEnabled = false;
                btnChoose.IsEnabled = true;
                txtPath.Text = "...";

                var Panel = this.FindControl<HeaderedContentControl>("hccMain");
                Panel?.IsEnabled = false;

                Panel = this.FindControl<HeaderedContentControl>("hccUpdater");
                Panel?.IsEnabled = false;
            });
        }

        private async void OpenFilePicker()
        {
            if (IsFilePickerActive)
            {
                return;
            }
            else
            {
                IsFilePickerActive = true;
            }

            DisableUI();
            ResetPatcher();

            string Path = await ViewHelpers.GetFileFromFilePicker("Choose .exe file", "Settlers6", ViewHelpers.Executable);
            IsFilePickerActive = false;
            
            if (string.IsNullOrEmpty(Path))
            {
                txtPath.Text = "...";
                return;
            }

            Path = IOFileHandler.Instance.IsPlayLauncherExecutable(Path);
            txtPath.Text = Path;
            await InitializePatcher(Path);
        }

        private void ToggleUIAvailability(bool Enable)
        {
            btnChoose.IsEnabled = Enable;
            btnPatch.IsEnabled = Enable;
            btnBackup.IsEnabled = Enable;
        }

        private async Task InitializePatcher(string Filepath)
        {
            Stream Definition = Utility.GetEmbeddedResourceDefinition("S6Patcher.Definitions.Definitions.bin");
            try
            {
                MainPatcher = new Patcher.Patcher(Filepath, Definition);
            }
            catch (Exception ex)
            {
                if (Definition.CanRead)
                {
                    Definition.Dispose();
                }

                await ShowMessageBox("Error", ex.Message);
                return;
            }

            MainPatcher.ShowMessage += async Message => await ShowMessageBox("Error", Message);
            MainPatcher.GlobalMod.ShowMessage += async Message => await ShowMessageBox("ModLoader", Message);
            
            EnableUIElements(MainPatcher.GlobalID);
        }

        private async Task FinishPatching()
        {
            ResetPatcher(RuntimeInformation.IsOSPlatform(OSPlatform.Windows));
            DisableUI();
            await ShowMessageBox("Finished", "Patching finished!");
        }

        private async void MainPatchingTask()
        {
            PatchingInProgress = true;
            Logger.Instance.Log("Patching Process Started!");

            List<string> Features = GetFeatures();
            bool UseBugfixMod = cbModDownload.IsChecked == true || cbUpdater.IsChecked == true;
            bool UseModLoader = cbModLoader.IsChecked == true || UseBugfixMod;
            bool DoNotUseEmbedded = rbDownload.IsChecked == true;

            ToggleUIAvailability(false);
            await ViewHelpers.GetPathToOptionsFile();
            await PatchByFeatures(Features, UseBugfixMod, UseModLoader, DoNotUseEmbedded);
            await FinishPatching();

            Logger.Instance.Log("Patching Process Finished!");
            PatchingInProgress = false;
        }

        private List<string> GetFeatures()
        {
            List<string> Features = ViewHelpers.GetSelectedFeatures();
            Features = [.. Features.Select(Item => Utility.Features.TryGetValue(Item, out string Value) ? Value : Item)];
            Features.ForEach(Element => Logger.Instance.Log("Selected Feature: " + Element));
            return Features;
        }

        private async Task PatchByFeatures(List<string> Features, bool UseBugfixMod, bool UseModLoader, bool DoNotUseEmbedded)
        {
            Task Completed = Task.WhenAll(PatcherScriptFilesWrapper(DoNotUseEmbedded), 
                PatcherModLoaderWrapper(UseBugfixMod, UseModLoader, DoNotUseEmbedded));

            if (cbUpdater.IsChecked == true)
            {
                await Completed;
                return;
            }

            MainPatcher.PatchByControlFeatures(Features);
            if (cbHighTextures.IsChecked == true && MainPatcher.GlobalID != execID.ED)
            {
                MainPatcher.SetTextureResolution(txtResolution.Text);
            }
            if (cbZoom.IsChecked == true)
            {
                MainPatcher.SetZoomLevel(txtZoom.Text);
            }
            if (cbAutosave.IsChecked == true)
            {
                MainPatcher.SetAutosaveTimer(txtAutosave.Text);
            }
            if (cbLAAFlag.IsChecked == true)
            {
                MainPatcher.SetLargeAddressAwareFlag();
            }
            if (cbEasyDebug.IsChecked == true)
            {
                MainPatcher.SetEasyDebug();
            }

            await Completed;
        }

        private async Task PatcherModLoaderWrapper(bool ModInstallation, bool UseModLoader, bool DoNotUseEmbedded)
        {
            if (MainPatcher.GlobalID != execID.ED && (UseModLoader || ModInstallation))
            {
                await MainPatcher.SetModLoader(ModInstallation, DoNotUseEmbedded);
            }
        }

        private async Task PatcherScriptFilesWrapper(bool DoNotUseEmbedded)
        {
            if (MainPatcher.GlobalID != execID.ED)
            {
                await MainPatcher.WriteScriptFilesToFolder(DoNotUseEmbedded);
            }
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
            if (PatchingInProgress)
            {
                _ = ShowMessageBox("Patching in Progress ...", "Patching is currently in progress!");
                e.Cancel = true;
                return;
            }

            ResetPatcher();
            WebHandler.Instance.Dispose();
            Logger.Instance.Dispose();
            base.OnClosing(e);
        }

        private void RestoreBackup()
        {
            ResetPatcher();
            Backup.Restore(txtPath.Text);
            DisableUI();
        }

        private async Task ShowMessageBox(string Title, string Message) => await ViewHelpers.ShowMessageBox(Title, Message);
        private void ResetPatcher(bool FinishWithPEHeader = false)
        {
            MainPatcher?.Dispose(FinishWithPEHeader);
            MainPatcher = null;
        }

        private void btnPatch_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e) => MainPatchingTask();
        private void btnBackup_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e) => RestoreBackup();
        private void btnChoose_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e) => OpenFilePicker();
        private void btnUpdate_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e) => ViewHelpers.CheckForUpdates(false);
        private void btnExit_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e) => Close();
        private void cbUpdater_Checked(object sender, Avalonia.Interactivity.RoutedEventArgs e) => tcMain.IsEnabled = cbUpdater.IsChecked == false;
        private void cbModLoader_IsCheckedChanged(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            cbModDownload.IsEnabled = (bool)cbModLoader.IsChecked;
            if (cbModLoader.IsChecked == false)
            {
                cbModDownload.IsChecked = false;
            }
        }
        private void rbDownload_IsCheckedChanged(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            cbUpdater.IsEnabled = rbDownload.IsChecked == true;
            if (rbDownload.IsChecked == false)
            {
                cbUpdater.IsChecked = false;
            }
        }

        private void cbAutosave_IsCheckedChanged(object sender, Avalonia.Interactivity.RoutedEventArgs e) => 
            txtAutosave.IsEnabled = cbAutosave.IsChecked == true;
        private void cbZoom_IsCheckedChanged(object sender, Avalonia.Interactivity.RoutedEventArgs e) =>
            txtZoom.IsEnabled = cbZoom.IsChecked == true;
        private void cbHighTextures_IsCheckedChanged(object sender, Avalonia.Interactivity.RoutedEventArgs e) =>
            txtResolution.IsEnabled = cbHighTextures.IsChecked == true && MainPatcher.GlobalID != execID.ED;
    }
}
