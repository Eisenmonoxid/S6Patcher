using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using S6Patcher.Source.Helpers;
using S6Patcher.Source.Patcher;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace S6Patcher.Source.View
{
    public class ViewHelpers(Window Window)
    {
        public FilePickerFileType Executable {get;} = new("Executable file | *.exe") {Patterns = ["*.exe"]};
        public FilePickerFileType Configuration {get;} = new("Configuration file | *.ini") {Patterns = ["*.ini"]};

        public List<string> GetSelectedFeatures() => [.. GetControlsByType<CheckBox>()
            .Where(Box => Box.IsChecked == true && Box.IsEnabled == true)
            .Select(Box => Box.Name).Distinct()];

        public void ShowMessageBox(string Title, string Message)
        {
            if (Design.IsDesignMode)
            {
                return;
            }

            ViewAccessorWrapper(async () =>
            {
                var Box = MessageBoxManager.GetMessageBoxStandard(Title, Message, ButtonEnum.Ok,
                    Icon.Warning, WindowStartupLocation.CenterOwner);
                await Box.ShowWindowDialogAsync(Window);
            });
        }

        public List<Control> GetControlsByNames(string[] Names) => [.. Names
                .Select(Window.FindControl<Control>)
                .Where(Control => Control != null)];
                
        public IEnumerable<T> GetControlsByType<T>() where T : Control => Window.GetLogicalDescendants().OfType<T>();

        public async Task<string> GetFileFromFilePicker(string Title, string Name, FilePickerFileType Type)
        {
            var Level = TopLevel.GetTopLevel(Window);
            if (Level?.StorageProvider == null)
            {
                return string.Empty;
            }

            var File = await Level.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                AllowMultiple = false,
                SuggestedFileName = Name,
                Title = Title,
                FileTypeFilter = [Type]
            });

            return File.Count > 0 ? File[0].Path.LocalPath : string.Empty;
        }

        public async Task GetPathToOptionsFile()
        {
            if (UserScriptHandler.Instance.DoesUserScriptDirectoryExist())
            {
                return;
            }

            string Path = await GetFileFromFilePicker("Choose Options.ini file", "Options", Configuration);
            if (!string.IsNullOrEmpty(Path))
            {
                try
                {
                    var dirInfo = new DirectoryInfo(System.IO.Path.GetDirectoryName(Path));
                    if (dirInfo?.Parent?.Parent != null)
                    {
                        Path = dirInfo.Parent.Parent.FullName;
                    }
                    else
                    {
                        Logger.Instance.Log("Directory structure not as expected!");
                        Path = string.Empty;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Instance.Log(ex.ToString());
                }

                UserScriptHandler.Instance.GlobalDocuments = Path;
            }
        }

        public async void CheckForUpdates(bool Startup)
        {
            string result = await WebHandler.Instance.CheckForUpdatesAsync(Startup);
            if (!string.IsNullOrEmpty(result))
            {
                Logger.Instance.Log(result);
                ShowMessageBox("Updater", result);
            }
        }

        public void ViewAccessorWrapper(Action Action)
        {
            if (!Dispatcher.UIThread.CheckAccess())
            {
                Dispatcher.UIThread.InvokeAsync(Action);
                return;
            }

            Action();
        }
    }
}
