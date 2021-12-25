using PixiEditor.Models.Controllers.Commands;
using PixiEditor.Models.Controllers.Shortcuts;
using System;
using System.Collections.ObjectModel;

namespace PixiEditor.ViewModels
{
    public class ShortcutEditorViewModel : ViewModelBase
    {
        private readonly CommandController _commandController;
        private readonly ShortcutController _shortcutController;
        private string searchTerm;

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

        public ShortcutEditorViewModel()
        {
            SearchResults = new();
            _commandController = ViewModelMain.Current.CommandController;
            _shortcutController = ViewModelMain.Current.ShortcutController;
        }

        private void UpdateSearchResults()
        {
            SearchResults.Clear();

            if (string.IsNullOrWhiteSpace(SearchTerm))
            {
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
        }
    }
}
