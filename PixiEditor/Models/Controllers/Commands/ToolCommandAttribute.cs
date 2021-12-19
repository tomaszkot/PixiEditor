using System;
using System.Windows.Input;

namespace PixiEditor.Models.Controllers.Commands
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ToolCommandAttribute : Attribute
    {
        public string Display { get; set; }

        public Key Key { get; set; }

        public ModifierKeys Modifiers { get; set; }

        public ToolCommandAttribute(Key key, ModifierKeys modifiers)
        {
            Key = key;
            Modifiers = modifiers;
        }

        public ToolCommandAttribute(string displayName, Key key, ModifierKeys modifiers)
            : this(key, modifiers)
        {
            Display = displayName;
        }
    }
}
