using PixiEditor.Helpers;
using PixiEditor.Models.Controllers.Commands;
using System.Windows.Input;

namespace PixiEditor.ViewModels.SubViewModels.Main
{
    public class ViewportViewModel : SubViewModel<ViewModelMain>
    {
        [Commands.Basic("PixiEditor.Viewport.ZoomIn", "Zoom in", 1, Key.OemPlus, ModifierKeys.None)]
        [Commands.Basic("PixiEditor.Viewport.ZoomOut", "Zoom out", -1, Key.OemMinus, ModifierKeys.None)]
        public RelayCommand ZoomCommand { get; set; }

        public RelayCommand ToggleGridLinesCommand { get; set; }

        private bool gridLinesEnabled = false;

        public bool GridLinesEnabled
        {
            get => gridLinesEnabled;
            set
            {
                gridLinesEnabled = value;
                RaisePropertyChanged(nameof(GridLinesEnabled));
            }
        }

        public ViewportViewModel(ViewModelMain owner)
            : base(owner)
        {
            ZoomCommand = new RelayCommand(ZoomViewport);
            ToggleGridLinesCommand = new RelayCommand(ToggleGridLines);
        }

        private void ToggleGridLines(object parameter)
        {
            GridLinesEnabled = !GridLinesEnabled;
        }

        private void ZoomViewport(object parameter)
        {
            double zoom = (int)parameter;
            if (Owner.BitmapManager.ActiveDocument is not null)
            {
                Owner.BitmapManager.ActiveDocument.ZoomViewportTrigger.Execute(this, zoom);
            }
        }
    }
}
