using PixiEditor.Helpers;
using PixiEditor.Models.Controllers.Commands;
using PixiEditor.Views.UserControls;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PixiEditor.ViewModels
{
    public class CommandControlViewModel : ViewModelBase
    {
        private readonly CommandController _commandController;
        private readonly CommandControl _control;

        private string searchTerm;
        private int selectedCommand;

        public string SearchTerm
        {
            get => searchTerm;
            set
            {
                if (SetProperty(ref searchTerm, value))
                {
                    UpdateSearchResults();
                }
            }
        }

        public ObservableCollection<Command> SearchResults { get; }

        public int SelectedCommand { get => selectedCommand; set => SetProperty(ref selectedCommand, value); }

        public RelayCommand SearchBoxKeyDownCommand { get; set; }

        public RelayCommand ExecuteCommand { get; set; }

        public CommandControlViewModel(CommandControl control, CommandController commandController)
        {
            SearchBoxKeyDownCommand = new(SearchBoxKeyDown);
            ExecuteCommand = new(ExeCommand);
            _control = control;
            _commandController = commandController ?? throw new ArgumentNullException(nameof(commandController));
            SearchResults = new();
        }

        private void ExeCommand(object paramter)
        {
            var command = paramter as Command;

            command.Execute();
            _control.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void SearchBoxKeyDown(object parameter)
        {
            var args = parameter as KeyEventArgs;

            if (_commandController.Commands["PixiEditor.Commands.ToggleSearchWindow"].Shortcut == new KeyCombination(args.Key, Keyboard.Modifiers))
            {
                _control.Visibility = System.Windows.Visibility.Collapsed;
            }

            if (SearchResults.Count == 0)
            {
                return;
            }

            args.Handled = HandleTextBoxKey(args.Key);
        }

        private bool HandleTextBoxKey(Key key)
        {
            int index = SelectedCommand;

            if (key == Key.Down)
            {
                index++;
            }
            else if (key == Key.Up)
            {
                index--;
            }
            else if (key == Key.Enter)
            {
                ExeCommand(SearchResults[SelectedCommand]);
                return false;
            }
            else
            {
                return false;
            }

            if (index >= SearchResults.Count)
            {
                index = 0;
            }
            else if (index < 0)
            {
                index = SearchResults.Count - 1;
            }

            SelectedCommand = index;

            return true;
        }

        private void UpdateSearchResults()
        {
            SearchResults.Clear();

            if (string.IsNullOrWhiteSpace(SearchTerm))
            {
                SelectedCommand = -1;
                return;
            }

            foreach (var command in _commandController.Commands)
            {
                if (command.Display.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
                {
                    SearchResults.Add(command);
                }

                if (SearchResults.Count > 8)
                {
                    break;
                }
            }

            if (SearchResults.Count > 0 && SelectedCommand == -1)
            {
                SelectedCommand = 0;
            }

            SelectedCommand = Math.Min(SearchResults.Count - 1, SelectedCommand);
        }
    }
}
