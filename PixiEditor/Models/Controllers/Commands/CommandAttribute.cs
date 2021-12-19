using System;
using System.Windows.Input;

namespace PixiEditor.Models.Controllers.Commands
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CommandAttribute : Attribute
    {
        public string Name { get; set; }

        public string Display { get; set; }

        public Key Key { get; set; }

        public ModifierKeys Modifiers { get; set; }

        public CommandAttribute(string name, string displayName)
            : this(name, displayName, Key.None, ModifierKeys.None)
        {
        }

        public CommandAttribute(string name, string displayName, Key key, ModifierKeys modifiers)
        {
            Name = name;
            Display = displayName;
            Key = key;
            Modifiers = modifiers;
        }
    }
}
