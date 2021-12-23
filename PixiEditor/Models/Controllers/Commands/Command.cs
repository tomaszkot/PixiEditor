using PixiEditor.Helpers;
using System;
using System.Windows.Input;

#pragma warning disable SA1402 // File may only contain a single type
namespace PixiEditor.Models.Controllers.Commands
{
    public abstract class Command : NotifyableObject
    {
        private KeyCombination shortcut;

        public string Name { get; init; }

        public string Display { get; init; }

        public KeyCombination Shortcut
        {
            get => shortcut;
            set
            {
                KeyCombination old = shortcut;
                if (SetProperty(ref shortcut, value))
                {
                    ShortcutChanged?.Invoke(this, new CommandShortcutChangedEventArgs() { OldShortcut = old, NewShortcut = value });
                }
            }
        }

        public event CommandShortcutChanged ShortcutChanged;

        public Func<ICommand> GetCommand { get; init; }

        protected Command(string name, string display, Key key, ModifierKeys modifiers, Func<ICommand> getCommand)
        {
            Name = name;
            Display = display;
            Shortcut = new(key, modifiers);
            GetCommand = getCommand;
        }

        public virtual void Execute()
        {
            OnExecute(GetCommand());
        }

        protected virtual void OnExecute(ICommand command)
        {
            throw new NotImplementedException();
        }
    }

    public delegate void CommandShortcutChanged(Command command, CommandShortcutChangedEventArgs args);

    public class CommandShortcutChangedEventArgs : EventArgs
    {
        public KeyCombination OldShortcut { get; init; }

        public KeyCombination NewShortcut { get; init; }
    }

    public class BasicCommand : Command
    {
        public object Parameter { get; init; }

        public BasicCommand(string name, string display, object parameter, Func<ICommand> getCommand, Key key = Key.None, ModifierKeys modifiers = ModifierKeys.None)
            : base(name, display, key, modifiers, getCommand)
        {
            Parameter = parameter;
        }

        protected override void OnExecute(ICommand command)
        {
            if (command.CanExecute(Parameter))
            {
                command.Execute(Parameter);
            }
        }
    }

    public class FactoryCommand : Command
    {
        public Func<Command, object> ParameterFactory { get; init; }

        public FactoryCommand(string name, string display, Func<Command, object> parameterFactory, Func<ICommand> getCommand, Key key = Key.None, ModifierKeys modifiers = ModifierKeys.None)
            : base(name, display, key, modifiers, getCommand)
        {
            ParameterFactory = parameterFactory;
        }

        public FactoryCommand(string name, string display, Func<object> parameterFactory, Func<ICommand> getCommand, Key key = Key.None, ModifierKeys modifiers = ModifierKeys.None)
            : this(name, display, _ => parameterFactory(), getCommand, key, modifiers)
        {
        }

        protected override void OnExecute(ICommand command)
        {
            object parameter = ParameterFactory(this);

            if (command.CanExecute(parameter))
            {
                command.Execute(parameter);
            }
        }
    }
}
