using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.Desktop
{
    internal static class PathUtilities
    {
        private static string explicitRootPath;
        private static Lazy<string> binPath = new Lazy<string>(GetBinPath);
        private static Lazy<string> rootPath = new Lazy<string>(GetRootPath);

        private static string GetBinPath()
            => Path.GetDirectoryName(new DirectoryInfo(typeof(PathUtilities).Assembly.Location).FullName);

        private static string GetRootPath()
            => new DirectoryInfo(Path.Combine(binPath.Value, "..")).FullName;

        public static string RootPath => explicitRootPath ?? binPath.Value;

        public static string GetAbsolutePath(string subpath)
            => Path.Combine(RootPath, subpath);

        public static void SetRootPath(string rootPath)
        {
            explicitRootPath = rootPath;
        }

        public static string GetSafeFilename(string filename) =>
            string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
    }
}
