using System;
using System.Linq;
using System.Windows.Input;
using PixiEditor.Helpers.Extensions;

namespace PixiEditor.Models.Controllers.Shortcuts
{
    public class Shortcut
    {
        public Shortcut(Key shortcutKey, Action execute, ModifierKeys modifier = ModifierKeys.None)
        {
            ShortcutKey = shortcutKey;
            Modifier = modifier;
            ExecuteCommand = execute;
        }

        public Key ShortcutKey { get; set; }

        public ModifierKeys Modifier { get; set; }

        /// <summary>
        /// Gets all <see cref="ModifierKeys"/> as an array.
        /// </summary>
        public ModifierKeys[] Modifiers { get => Modifier.GetFlags().Except(new ModifierKeys[] { ModifierKeys.None }).ToArray(); }

        public Action ExecuteCommand { get; set; }

        public string Description { get; set; }

        public object CommandParameter { get; set; }

        public void Execute()
        {
            ExecuteCommand();
        }
    }
}