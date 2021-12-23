using System.Windows.Input;

namespace PixiEditor.Models.Controllers.Commands
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "That's a record")]
    public record struct KeyCombination(Key Key, ModifierKeys Modifiers)
    {
        public static KeyCombination None => new(Key.None, ModifierKeys.None);
    }
}
