using PixiEditor.Helpers;
using PixiEditor.Models.Controllers.Commands;
using System.Windows.Input;

namespace PixiEditor.ViewModels.SubViewModels.Main
{
    public class CommandViewModel : SubViewModel<ViewModelMain>
    {
        private bool searchWindowVisible;

        public bool SearchWindowVisible
        {
            get => searchWindowVisible;
            set => SetProperty(ref searchWindowVisible, value);
        }

        [Commands.Basic("PixiEditor.Commands.ToggleSearchWindow", "Toggle Search Window Visibilty", Key.F1)]
        public RelayCommand ToggleSearchWindowVisibilty { get; set; }

        public CommandViewModel(ViewModelMain owner)
            : base(owner)
        {
            ToggleSearchWindowVisibilty = new(_ => SearchWindowVisible = !SearchWindowVisible);
        }
    }
}
