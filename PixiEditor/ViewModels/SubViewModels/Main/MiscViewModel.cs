using PixiEditor.Helpers;
using PixiEditor.Models.Controllers.Commands;
using PixiEditor.Views.Dialogs;
using System.Diagnostics;

namespace PixiEditor.ViewModels.SubViewModels.Main
{
    public class MiscViewModel : SubViewModel<ViewModelMain>
    {
        [Commands.Basic("PixiEditor.Misc.Hyperlink.PixiEditorProject", "Open project website (https://pixieditor.net)", "https://pixieditor.net")]
        [Commands.Basic("PixiEditor.Misc.Hyperlink.GitHub", "Open GitHub repository (https://github.com/PixiEditor/PixiEditor)", "https://github.com/PixiEditor/PixiEditor")]
        [Commands.Basic("PixiEditor.Misc.Hyperlink.Documentation", "Open documentation (https://github.com/PixiEditor/PixiEditor)", "https://github.com/PixiEditor/PixiEditor")]
        public RelayCommand OpenHyperlinkCommand { get; set; }

        [Commands.Basic("PixiEditor.Preferences.OpenSettingsWindow", "Open Settings Window")]
        public RelayCommand OpenSettingsWindowCommand { get; set; }

        public RelayCommand OpenShortcutWindowCommand { get; set; }

        [Commands.Basic("PixiEditor.Misc.OpenHelloThereWindow", "Open Startup Window")]
        public RelayCommand OpenHelloThereWindowCommand { get; set; }

        public ShortcutPopup ShortcutPopup { get; set; }

        public MiscViewModel(ViewModelMain owner)
            : base(owner)
        {
            OpenHyperlinkCommand = new RelayCommand(OpenHyperlink);
            OpenSettingsWindowCommand = new RelayCommand(OpenSettingsWindow);
            OpenShortcutWindowCommand = new RelayCommand(OpenShortcutWindow);
            OpenHelloThereWindowCommand = new RelayCommand(OpenHelloThereWindow);

            ShortcutPopup = new ShortcutPopup(owner.ShortcutController);
        }

        private void OpenSettingsWindow(object parameter)
        {
            SettingsWindow settings = new SettingsWindow();
            settings.Show();
        }

        private void OpenHyperlink(object parameter)
        {
            if (parameter == null)
            {
                return;
            }

            var url = (string)parameter;
            var processInfo = new ProcessStartInfo()
            {
                FileName = url,
                UseShellExecute = true
            };
            Process.Start(processInfo);
        }

        private void OpenShortcutWindow(object parameter)
        {
            ShortcutPopup.Show();
        }

        private void OpenHelloThereWindow(object parameter)
        {
            new HelloTherePopup(Owner.FileSubViewModel).Show();
        }
    }
}