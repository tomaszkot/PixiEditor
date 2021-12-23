using PixiEditor.Helpers;
using PixiEditor.Helpers.Extensions;
using System.Text;
using System.Windows.Input;

namespace PixiEditor.Models.Controllers.Commands
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "That's a record")]
    public record struct KeyCombination(Key Key, ModifierKeys Modifiers)
    {
        public static KeyCombination None => new(Key.None, ModifierKeys.None);

        public override string ToString()
        {
            StringBuilder builder = new();

            foreach (ModifierKeys modifier in Modifiers.GetFlags())
            {
                if (modifier == ModifierKeys.None) continue;

                string key = modifier switch
                {
                    ModifierKeys.Control => "Ctrl",
                    _ => modifier.ToString()
                };

                builder.Append(key);
                builder.Append(" + ");
            }

            if (Key != Key.None)
            {
                builder.Append(InputKeyHelpers.GetCharFromKey(Key));
            }

            return builder.ToString();
        }
    }
}
