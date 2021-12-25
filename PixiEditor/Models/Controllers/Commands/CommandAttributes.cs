using System;
using System.Windows.Input;

namespace PixiEditor.Models.Controllers.Commands
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:File name should match first type name", Justification = "Clarity and simplicity")]
    public static class Commands
    {
        /// <summary>
        /// A command that always uses the same command parameter on execution
        /// </summary>
        public class BasicAttribute : CommandAttribute
        {
            public object Parameter { get; set; }

            /// <summary>
            /// A command that always uses null as the command parameter
            /// </summary>
            /// <param name="name">The name of the command. Used to identify a command. Must be unique.</param>
            /// <param name="display">The string used to represent the command.</param>
            /// <param name="key">The default shortcut key</param>
            /// <param name="modifiers">The default shortcut modifier</param>
            public BasicAttribute(string name, string display, Key key = Key.None, ModifierKeys modifiers = ModifierKeys.None)
                : base(name, display, key, modifiers)
            {
            }

            /// <summary>
            /// A command that always uses the <paramref name="parameter"/> as the command parameter
            /// </summary>
            /// <param name="name">The name of the command. Used to identify a command. Must be unique.</param>
            /// <param name="display">The string used to represent the command.</param>
            /// <param name="parameter">The command parameter</param>
            /// <param name="key">The default shortcut key</param>
            /// <param name="modifiers">The default shortcut modifier</param>
            public BasicAttribute(string name, string display, object parameter, Key key = Key.None, ModifierKeys modifiers = ModifierKeys.None)
                : base(name, display, key, modifiers)
            {
                Parameter = parameter;
            }
        }

        /// <summary>
        /// A command that generates it's command parameter using a factory. <para/> The factory must be a method and can be static
        /// </summary>
        public class FactoryAttribute : CommandAttribute
        {
            /// <summary>
            /// Gets or sets name of the factory method.
            /// </summary>
            public string FactoryName { get; set; }

            /// <summary>
            /// A command that generates it's command parameter using a factory. <para/> The factory must be a method and can be static
            /// </summary>
            /// <param name="name">The name of the command. Used to identify a command. Must be unique.</param>
            /// <param name="display">The string used to represent the command.</param>
            /// <param name="factoryName">The name of the method that should be used as a factory. Must be in the same class as the property</param>
            /// <param name="key">The default shortcut key</param>
            /// <param name="modifiers">The default shortcut modifier</param>
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

        /// <summary>
        /// A basic command that is only shown when the DEBUG conditional is met
        /// </summary>
        public class DebugAttribute : BasicAttribute
        {
            public DebugAttribute(string name, string display, object parameter = null, Key key = Key.None, ModifierKeys modifiers = ModifierKeys.None) : base(name, display, parameter, key, modifiers)
            { }
        }

        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
        public class ToolAttribute : CommandAttribute
        {
            public ToolAttribute()
                : base(Key.None, ModifierKeys.None)
            { }

            public ToolAttribute(Key key, ModifierKeys modifiers = ModifierKeys.None)
                : base(key, modifiers) // Name and Display are set by CommandManager
            {
                Key = key;
                Modifiers = modifiers;
            }

            public ToolAttribute(string displayName, Key key, ModifierKeys modifiers = ModifierKeys.None)
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
