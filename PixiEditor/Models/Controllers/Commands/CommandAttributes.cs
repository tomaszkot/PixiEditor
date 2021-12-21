using System;
using System.Windows.Input;

namespace PixiEditor.Models.Controllers.Commands
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Clarity and simplicity")]
    public static class Commands
    {
        public class BasicAttribute : CommandAttribute
        {
            public object Parameter { get; set; }

            public BasicAttribute(string name, string display, Key key = Key.None, ModifierKeys modifiers = ModifierKeys.None)
                : base(name, display, key, modifiers)
            {
            }

            public BasicAttribute(string name, string display, object parameter = null, Key key = Key.None, ModifierKeys modifiers = ModifierKeys.None)
                : base(name, display, key, modifiers)
            {
                Parameter = parameter;
            }
        }

        public class FactoryAttribute : CommandAttribute
        {
            public string FactoryName { get; set; }

            public FactoryAttribute(string name, string display, string factoryName, Key key = Key.None, ModifierKeys modifiers = ModifierKeys.None)
                : base(name, display, key, modifiers)
            {
                FactoryName = factoryName;
            }
        }

        public class UserInputAttribute : CommandAttribute
        {
            public Type InputType { get; set; }

            public UserInputAttribute(string name, string display, Type type, Key key = Key.None, ModifierKeys modifiers = ModifierKeys.None)
                : base(name, display, key, modifiers)
            {
                InputType = type;
            }
        }

        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
        public class ToolAttribute : CommandAttribute
        {
            public ToolAttribute(Key key, ModifierKeys modifiers)
                : base(key, modifiers) // Name and Display are set by CommandManager
            {
                Key = key;
                Modifiers = modifiers;
            }

            public ToolAttribute(string displayName, Key key, ModifierKeys modifiers)
                : this(key, modifiers)
            {
                Display = displayName;
            }
        }

        [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
        public abstract class CommandAttribute : Attribute
        {
            public string Name { get; set; }

            public string Display { get; set; }

            public Key Key { get; set; }

            public ModifierKeys Modifiers { get; set; }

            protected CommandAttribute(string name, string display, Key key, ModifierKeys modifiers)
            {
                Name = name;
                Display = display;
                Key = key;
                Modifiers = modifiers;
            }

            protected CommandAttribute(Key key, ModifierKeys modifiers)
            {
                Key = key;
                Modifiers = modifiers;
            }
        }
    }
}
