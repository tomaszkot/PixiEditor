using PixiEditor.Helpers;
using PixiEditor.Models.Controllers;
using PixiEditor.Models.Controllers.Commands;
using System.Linq;

namespace PixiEditor.ViewModels.SubViewModels.Main
{
    public class ClipboardViewModel : SubViewModel<ViewModelMain>
    {
        [Command("PixiEditor.Copy", "Copy to Clipboard")]
        public RelayCommand CopyCommand { get; set; }

        [Command("PixiEditor.Duplicate", "Duplicate")]
        public RelayCommand DuplicateCommand { get; set; }

        [Command("PixiEditor.Cut", "Cut to Clipboard")]
        public RelayCommand CutCommand { get; set; }

        [Command("PixiEditor.Paste", "Paste from Clipboard")]
        public RelayCommand PasteCommand { get; set; }

        public ClipboardViewModel(ViewModelMain owner)
            : base(owner)
        {
            CopyCommand = new RelayCommand(Copy);
            DuplicateCommand = new RelayCommand(Duplicate, Owner.SelectionSubViewModel.SelectionIsNotEmpty);
            CutCommand = new RelayCommand(Cut, Owner.SelectionSubViewModel.SelectionIsNotEmpty);
            PasteCommand = new RelayCommand(Paste, CanPaste);
        }

        public void Duplicate(object parameter)
        {
            Copy(null);
            Paste(null);
        }

        public void Cut(object parameter)
        {
            Copy(null);
            Owner.BitmapManager.BitmapOperations.DeletePixels(
                new[] { Owner.BitmapManager.ActiveDocument.ActiveLayer },
                Owner.BitmapManager.ActiveDocument.ActiveSelection.SelectedPoints.ToArray());
        }

        public void Paste(object parameter)
        {
            ClipboardController.PasteFromClipboard();
        }

        private bool CanPaste(object property)
        {
            return Owner.DocumentIsNotNull(null) && ClipboardController.IsImageInClipboard();
        }

        private void Copy(object parameter)
        {
            ClipboardController.CopyToClipboard(Owner.BitmapManager.ActiveDocument);
        }
    }
}