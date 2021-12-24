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

        public RelayCommand ExecuteCommandCommand { get; }

        public CommandController(IServiceProvider services)
        {
            _services = services;
            ExecuteCommandCommand = new RelayCommand(x => ((Command)x).Execute());
            Commands = new();
        }

        public Command GetFromKeyCombination(Key key, ModifierKeys modifiers)
        {
            _ = Commands.TryGetValue(new KeyCombination(key, modifiers), out Command command);

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

                Commands.AddRange(commands);
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

                Commands.Add(command);

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

                    Command command = null;

                    // TODO: Implement UserInput commands
                    if (attribute is BasicAttribute basicAttr)
                    {
                        if (basicAttr is DebugAttribute debugAttr)
                        {
                            AddDebugCommand(
                                new BasicCommand(
                                    debugAttr.Name,
                                    debugAttr.Display,
                                    debugAttr.Parameter,
                                    () => (ICommand)property.GetValue(_services.GetService(type)),
                                    debugAttr.Key,
                                    debugAttr.Modifiers),
                                ref command);
                        }
                        else
                        {
                            command = new BasicCommand(
                                    basicAttr.Name,
                                    basicAttr.Display,
                                    basicAttr.Parameter,
                                    () => (ICommand)property.GetValue(_services.GetService(type)),
                                    basicAttr.Key,
                                    basicAttr.Modifiers);
                        }
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

                    if (command == null)
                    {
                        continue;
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

        [Conditional("DEBUG")]
        private static void AddDebugCommand(Command command, ref Command outputCommand)
        {
            outputCommand = command;
        }
    }
}
