using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using Avalonia.VisualTree;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace S6Patcher.Source.View
{
    public class ViewHelpers(Window Window)
    {
        public FilePickerFileType Executable {get;} = new("Executable file | *.exe") {Patterns = ["*.exe"]};
        public FilePickerFileType Configuration {get;} = new("Configuration file | *.ini") {Patterns = ["*.ini"]};

        public List<string> GetSelectedFeatures() => [.. GetControlsByType<CheckBox>()
            .Where(Box => Box.IsChecked == true)
            .Select(Box => Box.Name).Distinct()];

        public async void ShowMessageBox(string Title, string Message)
        {
            if (Design.IsDesignMode)
            {
                return;
            }

            if (Dispatcher.UIThread.CheckAccess() == false)
            {
                Dispatcher.UIThread.Post(() => ShowMessageBox(Title, Message));
                return;
            }

            var Box = MessageBoxManager.GetMessageBoxStandard(Title, Message, ButtonEnum.Ok,
                Icon.Warning, WindowStartupLocation.CenterOwner);
            await Box.ShowWindowDialogAsync(Window);
        }

        public List<Control> GetControlsByNames(string[] Names)
        {
            List<Control> Found = [.. Names
                .Select(Element => Window.FindControl<Control>(Element))
                .Where(Control => Control != null)];

            return Found;
        }

        public IEnumerable<T> GetControlsByType<T>() where T : Control => Window.GetVisualDescendants().OfType<T>();

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

        public void ViewAccessorWrapper(System.Action Action)
        {
            if (Dispatcher.UIThread.CheckAccess() == false)
            {
                Dispatcher.UIThread.Post(Action);
                return;
            }

            Action();
        }
    }
}
