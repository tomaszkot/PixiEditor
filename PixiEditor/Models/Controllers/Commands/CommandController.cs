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
                    _commandsByKey.Add((command.Key, command.Modifiers), command);
                }
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

                CommandAttribute attribute = property.GetCustomAttribute<CommandAttribute>();

                if (attribute is null)
                {
                    continue;
                }

                Command command = new()
                {
                    Name = attribute.Name,
                    Display = attribute.Display,
                    Key = attribute.Key,
                    Modifiers = attribute.Modifiers
                };

                command.GetICommand = () =>
                {
                    var obj = _services.GetService(type);

                    return (ICommand)property.GetValue(obj);
                };

                yield return command;
            }
        }

        public class Command
        {
            public Func<ICommand> GetICommand { get; set; }

            public string Name { get; set; }

            public string Display { get; set; }

            public Key Key { get; set; }

            public ModifierKeys Modifiers { get; set; }
        }
    }
}
