using PixiEditor.Models.Controllers.Commands;
using PixiEditor.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

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
            DataContext = new CommandControlViewModel(this, ViewModelMain.Current.CommandController);
            InitializeComponent();
        }

        private void Uc_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Dispatcher.BeginInvoke(() => searchTerm.Focus(), DispatcherPriority.Render);
            }
        }
    }
}
