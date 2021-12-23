using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace PixiEditor.Models.Controllers.Commands
{
    public class CommandCollection : ICollection<Command>
    {
        private readonly Dictionary<KeyCombination, Command> _commandsByShortcut;
        private readonly Dictionary<string, Command> _commandsByName;

        public Command this[KeyCombination key] { get => _commandsByShortcut[key]; set => _commandsByShortcut[key] = value; }

        public Command this[string key] { get => _commandsByName[key]; set => _commandsByName[key] = value; }

        public ICollection<KeyCombination> Keys => _commandsByShortcut.Keys;

        public ICollection<Command> Values => _commandsByShortcut.Values;

        public int Count => _commandsByName.Count;

        public bool IsReadOnly => false;

        public CommandCollection()
        {
            _commandsByShortcut = new Dictionary<KeyCombination, Command>();
            _commandsByName = new Dictionary<string, Command>();
        }

        public void Add(Command command)
        {
            if (command.Shortcut != KeyCombination.None)
            {
                _commandsByShortcut.Add(command.Shortcut, command);
            }

            _commandsByName.Add(command.Name, command);

            command.ShortcutChanged += Command_ShortcutChanged;
        }

        public void AddRange(IEnumerable<Command> commands)
        {
            foreach (Command command in commands)
            {
                Add(command);
            }
        }

        public bool ContainsKey(KeyCombination keyCombination) => _commandsByShortcut.ContainsKey(keyCombination);

        public bool ContainsKey(string name) => _commandsByName.ContainsKey(name);

        public bool Contains(Command command) => _commandsByName.ContainsKey(command.Name);

        public void Clear()
        {
            _commandsByShortcut.Clear();
            _commandsByName.Clear();
        }

        public bool Remove(KeyCombination keyCombination)
        {
            if (_commandsByShortcut.Remove(keyCombination, out Command command))
            {
                _commandsByName.Remove(command.Name);
                return true;
            }

            return false;
        }

        public bool Remove(string name)
        {
            if (_commandsByName.Remove(name, out Command command))
            {
                _commandsByShortcut.Remove(command.Shortcut);
                return true;
            }

            return false;
        }

        public bool TryGetValue(KeyCombination keyCombination, out Command command)
        {
            return _commandsByShortcut.TryGetValue(keyCombination, out command);
        }

        public bool TryGetValue(string name, out Command command)
        {
            return _commandsByName.TryGetValue(name, out command);
        }

        public void CopyTo(Command[] array, int arrayIndex)
        {
            _commandsByName.Values.CopyTo(array, arrayIndex);
        }

        public bool Remove(Command item)
        {
            return Remove(item.Name);
        }

        public IEnumerator<Command> GetEnumerator() => _commandsByName.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private void Command_ShortcutChanged(Command command, CommandShortcutChangedEventArgs args)
        {
            if (args.OldShortcut != KeyCombination.None)
            {
                _commandsByShortcut.Remove(args.OldShortcut);
            }

            _commandsByShortcut.Add(command.Shortcut, command);
        }
    }
}
