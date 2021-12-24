using Microsoft.Extensions.DependencyInjection;
using PixiEditor.Helpers;
using PixiEditor.Models.Tools;
using PixiEditor.ViewModels.SubViewModels.Main;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Input;
using static PixiEditor.Models.Controllers.Commands.Commands;

namespace PixiEditor.Models.Controllers.Commands
{
    public class CommandController
    {
        private readonly IServiceProvider _services;

        public CommandCollection Commands { get; }

        public CommandController(IServiceProvider services)
        {
            _services = services;
            Commands = new();
        }

        public Command GetFromKeyCombination(Key key, ModifierKeys modifiers)
        {
            _ = Commands.TryGetValue(new KeyCombination(key, modifiers), out Command command);

            return command;
        }

        public void Init(Func<string, KeyCombination> getShortcut)
        {
            InitTools(getShortcut);

            foreach (Type type in Assembly.GetAssembly(typeof(CommandController)).GetTypes())
            {
                var service = _services.GetService(type);

                if (service == null)
                {
                    continue;
                }

                var commands = GetCommands(type, getShortcut);

                Commands.AddRange(commands);
            }
        }

        private void InitTools(Func<string, KeyCombination> getShortcut)
        {
            foreach (Tool tool in _services.GetServices<Tool>())
            {
                var command = GetToolCommand(tool.GetType(), getShortcut);

                if (command == null)
                {
                    continue;
                }

                Commands.Add(command);

                continue;
            }
        }

        private IEnumerable<Command> GetCommands(Type type, Func<string, KeyCombination> getShortcut)
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

                    KeyCombination shortcut = GetShortcutOrDefault(getShortcut(attribute.Name), new(attribute.Key, attribute.Modifiers));

                    // TODO: Implement UserInput commands
                    if (attribute is BasicAttribute basicAttr)
                    {
                        command = FromBasicAttribute(basicAttr, property, shortcut);
                    }
                    else if (attribute is FactoryAttribute factoryAttr)
                    {
                        command = FromFactoryAttribute(factoryAttr, property, shortcut);
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }

                    if (command == null)
                    {
                        continue;
                    }

                    yield return command;
                }
            }

            Command FromBasicAttribute(BasicAttribute attribute, PropertyInfo property, KeyCombination shortcut)
            {
                Command command = null;

                if (attribute is DebugAttribute debugAttr)
                {
                    AddDebugCommand(
                        new BasicCommand(
                            debugAttr.Name,
                            debugAttr.Display,
                            debugAttr.Parameter,
                            () => (ICommand)property.GetValue(_services.GetService(type)),
                            shortcut),
                        ref command);
                }
                else
                {
                    command = new BasicCommand(
                            attribute.Name,
                            attribute.Display,
                            attribute.Parameter,
                            () => (ICommand)property.GetValue(_services.GetService(type)),
                            shortcut);
                }

                return command;
            }

            Command FromFactoryAttribute(FactoryAttribute attribute, PropertyInfo property, KeyCombination shortcut)
            {
                MethodInfo method = type.GetMethod(attribute.FactoryName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

                if (method == null)
                {
                    throw new NotImplementedException($"The factory '{attribute.FactoryName}' has not been implemented in '{type.FullName}'\nCommand: {attribute.Name}");
                }

                Func<Command, object> factory;

                if (method.IsStatic)
                {
                    factory = _ => method.Invoke(null, null);
                }
                else
                {
                    factory = _ => method.Invoke(_services.GetService(type), null);
                }

                return new FactoryCommand(
                    attribute.Name,
                    attribute.Display,
                    factory,
                    () => (ICommand)property.GetValue(_services.GetService(type)),
                    shortcut);
            }
        }

        private Command GetToolCommand(Type type, Func<string, KeyCombination> getShortcut)
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

            KeyCombination shortcut = GetShortcutOrDefault(getShortcut(name), new(attribute.Key, attribute.Modifiers));

            return new BasicCommand(
                name,
                displayName,
                type,
                () => _services.GetRequiredService<ToolsViewModel>().SelectToolCommand,
                shortcut);
        }

        [Conditional("DEBUG")]
        private static void AddDebugCommand(Command command, ref Command outputCommand)
        {
            outputCommand = command;
        }

        private static KeyCombination GetShortcutOrDefault(KeyCombination shortcut, KeyCombination defaultValue)
        {
            if (shortcut == KeyCombination.None)
            {
                return defaultValue;
            }

            return shortcut;
        }
    }
}
