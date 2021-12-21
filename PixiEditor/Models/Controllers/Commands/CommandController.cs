using Microsoft.Extensions.DependencyInjection;
using PixiEditor.Helpers;
using PixiEditor.Models.Tools;
using PixiEditor.ViewModels.SubViewModels.Main;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Input;
using static PixiEditor.Models.Controllers.Commands.Commands;

namespace PixiEditor.Models.Controllers.Commands
{
    public class CommandController
    {
        private readonly IServiceProvider _services;
        private List<Command> _commands;
        private Dictionary<(Key, ModifierKeys), Command> _commandsByKey;

        public CommandController(IServiceProvider services)
        {
            _services = services;
            _commands = new();
            _commandsByKey = new();
        }

        public Command GetFromKeyCombination(Key key, ModifierKeys modifiers)
        {
            _ = _commandsByKey.TryGetValue((key, modifiers), out Command command);

            return command;
        }

        public void Init()
        {
            InitTools();

            foreach (Type type in Assembly.GetAssembly(typeof(CommandController)).GetTypes())
            {
                var service = _services.GetService(type);

                if (service == null)
                {
                    continue;
                }

                var commands = GetCommands(type);

                _commands.AddRange(commands);

                foreach (var command in commands)
                {
                    if (command.Key == Key.None && command.Modifiers == ModifierKeys.None)
                    {
                        continue;
                    }

                    _commandsByKey.Add((command.Key, command.Modifiers), command);
                }
            }
        }

        private void InitTools()
        {
            foreach (Tool tool in _services.GetServices<Tool>())
            {
                var command = GetToolCommand(tool.GetType());

                if (command == null)
                {
                    continue;
                }

                _commands.Add(command);
                _commandsByKey.Add((command.Key, command.Modifiers), command);

                continue;
            }
        }

        private IEnumerable<Command> GetCommands(Type type)
        {
            foreach (var property in type.GetProperties())
            {
                if (!property.CanRead || !property.PropertyType.IsAssignableTo(typeof(ICommand)))
                {
                    continue;
                }

                var attributes = property.GetCustomAttributes<CommandAttribute>();

                foreach (var attribute in attributes)
                {
                    if (attribute is null)
                    {
                        continue;
                    }

                    Command command;

                    // TODO: Implement UserInput commands
                    if (attribute is BasicAttribute basicAttr)
                    {
                        command = new BasicCommand(
                            basicAttr.Name,
                            basicAttr.Display,
                            basicAttr.Parameter,
                            () => (ICommand)property.GetValue(_services.GetService(type)),
                            basicAttr.Key,
                            basicAttr.Modifiers);
                    }
                    else if (attribute is FactoryAttribute factoryAttr)
                    {
                        MethodInfo method = type.GetMethod(factoryAttr.FactoryName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

                        Func<Command, object> factory;

                        if (method.IsStatic)
                        {
                            factory = _ => method.Invoke(null, null);
                        }
                        else
                        {
                            factory = _ => method.Invoke(_services.GetService(type), null);
                        }

                        command = new FactoryCommand(
                            factoryAttr.Name,
                            factoryAttr.Display,
                            factory,
                            () => (ICommand)property.GetValue(_services.GetService(type)),
                            factoryAttr.Key,
                            factoryAttr.Modifiers);
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }

                    yield return command;
                }
            }
        }

        private Command GetToolCommand(Type type)
        {
            var attribute = type.GetCustomAttribute<ToolAttribute>();

            if (attribute == null)
            {
                return null;
            }

            string displayName = attribute.Display;

            if (displayName == null)
            {
                displayName = ToolHelpers.GetToolDisplayName(type);
            }

            displayName = $"Select {displayName} Tool";

            string name = $"PixiEditor.Tools.{ToolHelpers.GetToolName(type)}";

            return new BasicCommand(
                name,
                displayName,
                type,
                () => _services.GetRequiredService<ToolsViewModel>().SelectToolCommand,
                attribute.Key,
                attribute.Modifiers);
        }

        public abstract class Command
        {
            public string Name { get; set; }

            public string Display { get; set; }

            public Key Key { get; set; }

            public ModifierKeys Modifiers { get; set; }

            public Func<ICommand> GetCommand { get; init; }

            protected Command(string name, string display, Key key, ModifierKeys modifiers, Func<ICommand> getCommand)
            {
                Name = name;
                Display = display;
                Key = key;
                Modifiers = modifiers;
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

        public class BasicCommand : Command
        {
            public object Parameter { get; set; }

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
            public Func<Command, object> ParameterFactory { get; set; }

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
}
