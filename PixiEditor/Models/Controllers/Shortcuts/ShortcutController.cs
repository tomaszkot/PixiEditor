using PixiEditor.Models.Controllers.Commands;
using System.Windows.Input;

namespace PixiEditor.Models.Controllers.Shortcuts
{
    public class ShortcutController
    {
        private readonly CommandController _commandController;

        public ShortcutController(CommandController controller)
        {
            _commandController = controller;
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
    }
}
