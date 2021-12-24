using System;
using System.IO;
using System.Linq;
using static System.Environment;

namespace PixiEditor.Helpers
{
    public static class PathHelper
    {
        public static Guid SessionGuid { get; } = Guid.NewGuid();

        public static string GetLocalData(params string[] paths)
        {
            string basePath = GetBasePath(SpecialFolder.LocalApplicationData);
            return Path.Combine(Enumerable.Prepend(paths, basePath).ToArray());
        }

        public static string GetRomaingData(params string[] paths)
        {
            string basePath = GetBasePath(SpecialFolder.LocalApplicationData);
            return Path.Combine(Enumerable.Prepend(paths, basePath).ToArray());
        }

        public static string GetTempData(params string[] paths)
        {
            string basePath = Path.Combine(Path.GetTempPath(), "PixiEditor");
            return Path.Combine(Enumerable.Prepend(paths, basePath).ToArray());
        }

        public static string GetSessionData(params string[] paths)
        {
            string basePath = GetTempData(SessionGuid.ToString());
            return Path.Combine(Enumerable.Prepend(paths, basePath).ToArray());
        }

        private static string GetBasePath(SpecialFolder folder) =>
            Path.Combine(GetFolderPath(folder), "PixiEditor");
    }
}
