using PixiEditor.Helpers.Extensions;
using System;

namespace PixiEditor.Helpers
{
    public static class ToolHelpers
    {
        public static string GetToolName(Type tool) =>
            tool.Name.Replace("Tool", string.Empty);

        public static string GetToolDisplayName(Type tool) =>
            GetToolName(tool).AddSpacesBeforeUppercaseLetters();

        public static string GetImagePath(Type tool) =>
            $"/Images/Tools/{GetToolName(tool)}Image.png";
    }
}
