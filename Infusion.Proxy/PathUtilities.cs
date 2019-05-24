using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.Proxy
{
    public static class PathUtilities
    {
        private static string explicitRootPath;
        private static Lazy<string> binPath = new Lazy<string>(GetBinPath);

        private static string GetBinPath()
            => Path.GetDirectoryName(new DirectoryInfo(Path.Combine(typeof(PathUtilities).Assembly.Location, "..")).FullName);

        public static string RootPath
            => explicitRootPath ?? binPath.Value;

        public static string GetAbsolutePath(string subpath)
            => Path.Combine(RootPath, subpath);

        public static void SetRootPath(string rootPath)
            => explicitRootPath = rootPath;

        public static string GetSafeFilename(string filename)
            => string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
    }
}
