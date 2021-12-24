using PixiEditor.Models.Controllers.Commands;
using PixiEditor.Models.Position;
using System.Collections.Generic;
using System.Windows.Input;

namespace PixiEditor.Models.Tools.Tools
{
    [Commands.Tool(Key.Space)]
    public class MoveViewportTool : ReadonlyTool
    {
        public MoveViewportTool()
        {
            Cursor = Cursors.SizeAll;
            ActionDisplay = "Click and move to pan viewport.";
        }

        public override bool HideHighlight => true;
        public override string Tooltip => "Move viewport. (Space)";

        public override void Use(IReadOnlyList<Coordinates> pixels)
        {
            // Implemented inside Zoombox.xaml.cs
        }
    }
}
