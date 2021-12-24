using PixiEditor.Helpers;
using System;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;

#pragma warning disable SA1402 // File may only contain a single type
namespace PixiEditor.Models.Controllers.Commands
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public abstract class Command : NotifyableObject
    {
        private KeyCombination shortcut;

        private string DebuggerDisplay
        {
            get
            {
                StringBuilder builder = new($"{Display} ({Name})");

                if (Shortcut != KeyCombination.None)
                {
                    builder.Append(' ');
                    builder.Append(Shortcut);
                }

                return builder.ToString();
            }
        }

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

        protected Command(string name, string display, KeyCombination shortcut, Func<ICommand> getCommand)
        {
            Name = name;
            Display = display;
            Shortcut = shortcut;
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

        public override string ToString() => Display;
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

        public BasicCommand(string name, string display, object parameter, Func<ICommand> getCommand, KeyCombination shortcut = default)
            : base(name, display, shortcut, getCommand)
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

        public FactoryCommand(string name, string display, Func<Command, object> parameterFactory, Func<ICommand> getCommand, KeyCombination shortcut = default)
            : base(name, display, shortcut, getCommand)
        {
            ParameterFactory = parameterFactory;
        }

        public FactoryCommand(string name, string display, Func<object> parameterFactory, Func<ICommand> getCommand, KeyCombination shortcut = default)
            : this(name, display, _ => parameterFactory(), getCommand, shortcut)
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
