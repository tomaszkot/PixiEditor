using PixiEditor.Helpers;
using PixiEditor.Models.Controllers.Commands;
using PixiEditor.Models.Dialogs;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace PixiEditor.ViewModels.SubViewModels.Main
{
    public class DebugViewModel : SubViewModel<ViewModelMain>
    {
        [Commands.Debug("PixiEditor.Debug.OpenTempDirectory", "Open Temp Directory", "%Temp%/PixiEditor")]
        [Commands.Debug("PixiEditor.Debug.OpenLocalAppDataDirectory", "Open Local AppData Directory", "%LocalAppData%/PixiEditor")]
        [Commands.Debug("PixiEditor.Debug.OpenRoamingAppDataDirectory", "Open Roaming AppData Directory", "%AppData%/PixiEditor")]
        public RelayCommand OpenFolderCommand { get; set; }

        [Commands.Debug("PixiEditor.Debug.OpenInstallDirectory", "Open Installation Directory")]
        public RelayCommand OpenInstallLocationCommand { get; set; }

        [Commands.Debug("PixiEditor.Debug.DeleteUserPreferences", "Delete User Preferences (Roaming AppData)")]
        [Commands.Debug("PixiEditor.Debug.DeleteEditorData", "Delete Editor Data (Local AppData)")]
        public RelayCommand DeleteFileCommand { get; set; }

        public DebugViewModel(ViewModelMain owner)
            : base(owner)
        {
            OpenFolderCommand = new RelayCommand(OpenFolder);
            OpenInstallLocationCommand = new RelayCommand(OpenInstallLocation);
            DeleteFileCommand = new RelayCommand(DeleteFile);
        }

        public static void OpenFolder(object parameter)
        {
            OpenShellExecute((string)parameter);
        }

        public static void OpenInstallLocation(object parameter)
        {
            OpenShellExecute(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
        }

        public static void DeleteFile(object parameter)
        {
            string file = Environment.ExpandEnvironmentVariables(parameter as string);
            if (!File.Exists(file))
            {
                NoticeDialog.Show($"File {parameter} does not exist\n(Full Path: {file})");
                return;
            }

            if (ConfirmationDialog.Show($"Are you sure you want to delete {parameter}?\nThis data will be lost for all installations.\n(Full Path: {file})", "Are you sure?") == Models.Enums.ConfirmationType.Yes)
            {
                File.Delete(file);
            }
        }

        private static void OpenShellExecute(string path)
        {
            ProcessStartInfo startInfo = new (Environment.ExpandEnvironmentVariables(path));

            startInfo.UseShellExecute = true;

            Process.Start(startInfo);
        }
    }
}