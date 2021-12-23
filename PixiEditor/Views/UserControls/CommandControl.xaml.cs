using PixiEditor.Models.Controllers.Commands;
using PixiEditor.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PixiEditor.Views.UserControls
{
    /// <summary>
    /// Interaction logic for CommandControl.xaml
    /// </summary>
    public partial class CommandControl : UserControl
    {
        public static readonly DependencyProperty ExecuteCommandProperty =
            DependencyProperty.Register(nameof(ExecuteCommand), typeof(ICommand), typeof(CommandControl));

        public ICommand ExecuteCommand
        {
            get => (ICommand)GetValue(ExecuteCommandProperty);
            set => SetValue(ExecuteCommandProperty, value);
        }

        public CommandControl()
        {
            DataContext = new CommandControlViewModel(ViewModelMain.Current.CommandController);
            InitializeComponent();
        }
    }
}
