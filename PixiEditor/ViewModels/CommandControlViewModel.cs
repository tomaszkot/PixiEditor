using PixiEditor.Helpers;
using PixiEditor.Models.Controllers.Commands;
using System;
using System.Collections.ObjectModel;

namespace PixiEditor.ViewModels
{
    public class CommandControlViewModel : NotifyableObject
    {
        private readonly CommandController _commandController;

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

        public CommandControlViewModel(CommandController commandController)
        {
            _commandController = commandController ?? throw new ArgumentNullException(nameof(commandController));
            SearchResults = new();
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
