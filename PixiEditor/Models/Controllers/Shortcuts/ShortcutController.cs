using Newtonsoft.Json;
using PixiEditor.Helpers;
using PixiEditor.Models.Controllers.Commands;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;

namespace PixiEditor.Models.Controllers.Shortcuts
{
    public class ShortcutController
    {
        private readonly CommandController _commandController;
        private readonly Dictionary<string, KeyCombination> _shortcuts;

        public string ShortcutFilePath { get; } = PathHelper.GetLocalData("shortcuts.json");

        public ShortcutController(CommandController controller)
        {
            _commandController = controller;
            _shortcuts = GetShortcutsFromFile();
        }

        public static bool BlockShortcutExecution { get; set; }

        public void KeyPressed(Key key, ModifierKeys modifiers)
        {
            if (!BlockShortcutExecution)
            {
                var command = _commandController.GetFromKeyCombination(key, modifiers);

                if (command == null)
                {
                    return;
                }

                command.Execute();
            }
        }

        public void UpdateShortcut(Command command, KeyCombination shortcut)
        {
            command.Shortcut = shortcut;

            _shortcuts.Remove(command.Name);
            _shortcuts.Add(command.Name, command.Shortcut);

            File.WriteAllText(ShortcutFilePath, JsonConvert.SerializeObject(_shortcuts));
        }

        public KeyCombination GetShortcut(string name)
        {
            return _shortcuts.GetValueOrDefault(name);
        }

        private Dictionary<string, KeyCombination> GetShortcutsFromFile()
        {
            if (!File.Exists(ShortcutFilePath))
            {
                return new();
            }

            try
            {
                return JsonConvert.DeserializeObject<Dictionary<string, KeyCombination>>(File.ReadAllText(ShortcutFilePath)) ?? new();
            }
            catch (JsonSerializationException)
            {
                return new();
            }
        }
    }
}
