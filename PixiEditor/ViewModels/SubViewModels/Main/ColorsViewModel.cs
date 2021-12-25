using PixiEditor.Helpers;
using PixiEditor.Models.Controllers;
using PixiEditor.Models.Controllers.Commands;
using SkiaSharp;
using System;
using System.Windows;
using System.Windows.Input;

namespace PixiEditor.ViewModels.SubViewModels.Main
{
    public class ColorsViewModel : SubViewModel<ViewModelMain>
    {
        [Commands.Basic("PixiEditor.Colors.SwapColors", "Swap Colors", Key.X)]
        public RelayCommand SwapColorsCommand { get; set; }

        [Commands.Basic("PixiEditor.Colors.SelectColor", "")]
        [Commands.Factory("PixiEditor.Colors.PasteFromClipboard", "Paste color from clipboard", nameof(GetFromClipboard))]
        public RelayCommand SelectColorCommand { get; set; }

        public RelayCommand RemoveSwatchCommand { get; set; }

        private SKColor primaryColor = SKColors.Black;

        public SKColor PrimaryColor // Primary color, hooked with left mouse button
        {
            get => primaryColor;
            set
            {
                Owner.BitmapManager.PrimaryColor = value;
                SetProperty(ref primaryColor, value);
            }
        }

        private SKColor secondaryColor = SKColors.White;

        public SKColor SecondaryColor
        {
            get => secondaryColor;
            set => SetProperty(ref secondaryColor, value);
        }

        public ColorsViewModel(ViewModelMain owner)
            : base(owner)
        {
            SelectColorCommand = new RelayCommand(SelectColor);
            RemoveSwatchCommand = new RelayCommand(RemoveSwatch);
            SwapColorsCommand = new RelayCommand(SwapColors);
        }

        public void SwapColors(object parameter)
        {
            var tmp = PrimaryColor;
            PrimaryColor = SecondaryColor;
            SecondaryColor = tmp;
        }

        public void AddSwatch(SKColor color)
        {
            if (!Owner.BitmapManager.ActiveDocument.Swatches.Contains(color))
            {
                Owner.BitmapManager.ActiveDocument.Swatches.Add(color);
            }
        }

        private static SKColor? GetFromClipboard()
        {
            if (Clipboard.ContainsText())
            {
                string text = Clipboard.GetText();

                if (SKColor.TryParse(text, out SKColor color))
                {
                    return color;
                }
            }

            return null;
        }

        private void RemoveSwatch(object parameter)
        {
            if (parameter is not SKColor color)
            {
                throw new ArgumentException("parameter must be a SKColor", nameof(parameter));
            }

            if (Owner.BitmapManager.ActiveDocument.Swatches.Contains(color))
            {
                Owner.BitmapManager.ActiveDocument.Swatches.Remove(color);
            }
        }

        private void SelectColor(object parameter)
        {
            var color = parameter as SKColor?;

            if (color != null)
            {
                PrimaryColor = color.Value;
            }
        }
    }
}
