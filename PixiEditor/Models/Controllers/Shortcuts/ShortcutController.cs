using PixiEditor.Models.Controllers.Commands;
using System.Collections.ObjectModel;
using System.Linq;
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

        public ObservableCollection<ShortcutGroup> ShortcutGroups { get; init; }

        public Shortcut LastShortcut { get; private set; }

        public void KeyPressed(Key key, ModifierKeys modifiers)
        {
            if (!BlockShortcutExecution)
            {
                var command = _commandController.GetFromKeyCombination(key, modifiers);

                if (command == null)
                {
                    return;
                }

                var icommand = command.GetICommand();

                if (!icommand.CanExecute(null))
                {
                    return;
                }

                icommand.Execute(null);

                LastShortcut = new Shortcut(command.Key, command.GetICommand());

                //Shortcut[] shortcuts = ShortcutGroups.SelectMany(x => x.Shortcuts).ToList().FindAll(x => x.ShortcutKey == key).ToArray();
                //if (shortcuts.Length < 1)
                //{
                //    return;
                //}

                //shortcuts = shortcuts.OrderByDescending(x => x.Modifier).ToArray();
                //for (int i = 0; i < shortcuts.Length; i++)
                //{
                //    if (modifiers.HasFlag(shortcuts[i].Modifier))
                //    {
                //        shortcuts[i].Execute();
                //        LastShortcut = shortcuts[i];
                //        break;
                //    }
                //}
            }
        }
    }
}
