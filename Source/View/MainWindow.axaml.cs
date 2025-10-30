using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using S6Patcher.Source.Helpers;
using System;
using System.Collections;
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
        private readonly ViewHelpers ViewHelpers;
        private Patcher.Patcher Patcher = null;

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
            Backup.ShowMessage += Message => ShowMessageBox("Backup", Message);

            DisableUI();
            GetModfileInformation();
            ViewHelpers.CheckForUpdates(true);
        }

        private void EnableUIElements(execID ID)
        {
            ViewHelpers.ViewAccessorWrapper(() =>
            {
                var Panel = this.FindControl<HeaderedContentControl>("hccMain");
                Panel.IsEnabled = true;

                ViewHelpers.GetControlsByType<CheckBox>().ToList().ForEach(Result => Result.IsEnabled = true);
                cbModDownload.IsEnabled = false;
                
                if (ID == execID.ED)
                {
                    cbUpdater.IsEnabled = false;
                }

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

        private void ToggleUIAvailability(bool Enable)
        {
            btnChoose.IsEnabled = Enable;
            btnPatch.IsEnabled = Enable;
            btnBackup.IsEnabled = Enable;
        }

        private void InitializePatcher(string Filepath)
        {
            try
            {
                Stream Definition = Utility.GetEmbeddedResourceDefinition("S6Patcher.Definitions.Definitions.bin") ?? 
                    throw new Exception("Error: Could not load Definition file! Aborting ...");
                Patcher = new Patcher.Patcher(Filepath, Definition);
            }
            catch (Exception ex)
            {
                ShowMessageBox("Error", ex.Message);
                return;
            }

            Patcher.ShowMessage += Message => ShowMessageBox("Error", Message);
            EnableUIElements(Patcher.GlobalID);
        }

        private void FinishPatching()
        {
            ResetPatcher(RuntimeInformation.IsOSPlatform(OSPlatform.Windows));
            DisableUI();
            ShowMessageBox("Finished", "Patching finished!");
        }

        private async void MainPatchingTask()
        {
            PatchingInProgress = true;
            Logger.Instance.Log("Patching Process Started!");

            ToggleUIAvailability(false);
            await ViewHelpers.GetPathToOptionsFile();
            await PatchByFeatures();
            FinishPatching();

            Logger.Instance.Log("Patching Process Finished!");
            PatchingInProgress = false;
        }

        private async Task PatchByFeatures()
        {
            bool Download = cbModDownload.IsChecked == true || cbUpdater.IsChecked == true;
            Task Completed = Task.WhenAll(PatcherScriptFilesWrapper(), PatcherModLoaderWrapper(Download));
            if (cbUpdater.IsChecked == true)
            {
                await Completed;
                return;
            }

            var Features = ViewHelpers.GetSelectedFeatures();
            Features.ForEach(Element => Logger.Instance.Log("Selected Feature: " + Element));
            Patcher.PatchByControlFeatures([.. Features.Select(Item => Utility.Features.TryGetValue(Item, out string Value) ? Value : Item)]);

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

            await Completed;
        }

        private async Task PatcherModLoaderWrapper(bool Download)
        {
            if (Patcher.GlobalID != execID.ED && (cbModLoader.IsChecked == true || cbUpdater.IsChecked == true))
            {
                await Patcher.SetModLoader(Download);
            }
        }

        private async Task PatcherScriptFilesWrapper()
        {
            if (Patcher.GlobalID != execID.ED)
            {
                await Patcher.WriteScriptFilesToFolder();
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
                ShowMessageBox("Patching in Progress ...", "Patching is currently in progress!");
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

        private void ShowMessageBox(string Title, string Message) => ViewHelpers.ShowMessageBox(Title, Message);
        private void ResetPatcher(bool FinishWithPEHeader = false)
        {
            Patcher?.Dispose(FinishWithPEHeader);
            Patcher = null;
        }

        private void btnPatch_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e) => MainPatchingTask();
        private void btnBackup_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e) => RestoreBackup();
        private void btnChoose_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e) => OpenFilePicker();
        private void btnUpdate_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e) => ViewHelpers.CheckForUpdates(false);
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

        private void cbAutosave_IsCheckedChanged(object sender, Avalonia.Interactivity.RoutedEventArgs e) => 
            txtAutosave.IsEnabled = cbAutosave.IsChecked == true;
        private void cbZoom_IsCheckedChanged(object sender, Avalonia.Interactivity.RoutedEventArgs e) =>
            txtZoom.IsEnabled = cbZoom.IsChecked == true;
        private void cbHighTextures_IsCheckedChanged(object sender, Avalonia.Interactivity.RoutedEventArgs e) =>
            txtResolution.IsEnabled = cbHighTextures.IsChecked == true && Patcher.GlobalID != execID.ED;
    }
}
