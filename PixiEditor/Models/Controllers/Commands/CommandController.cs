using Microsoft.Extensions.DependencyInjection;
using PixiEditor.Helpers;
using PixiEditor.Models.Tools;
using PixiEditor.ViewModels.SubViewModels.Main;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Input;

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

                    Command command = new()
                    {
                        Name = attribute.Name,
                        Display = attribute.Display,
                        Key = attribute.Key,
                        Modifiers = attribute.Modifiers,
                        CommandParameter = attribute.CommandParameter
                    };

                    command.GetICommand = () =>
                    {
                        var obj = _services.GetService(type);

                        return (ICommand)property.GetValue(obj);
                    };

                    yield return command;
                }
            }
        }

        private Command GetToolCommand(Type type)
        {
            var attribute = type.GetCustomAttribute<ToolCommandAttribute>();

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

            return new Command()
            {
                Name = name,
                Display = displayName,
                Key = attribute.Key,
                Modifiers = attribute.Modifiers,
                GetICommand = () =>
                {
                    return _services.GetRequiredService<ToolsViewModel>().SelectToolCommand;
                },
                CommandParameter = type
            };
        }

        public class Command
        {
            public Func<ICommand> GetICommand { get; set; }

            public object CommandParameter { get; set; }

            public string Name { get; set; }

            public string Display { get; set; }

            public Key Key { get; set; }

            public ModifierKeys Modifiers { get; set; }
        }
    }
}
