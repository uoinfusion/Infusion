using CommandLine;
using Infusion.Proxy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.Desktop.CommandLine
{
    internal static class Handler
    {
        public static void Handle(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(options =>
                {
                    if (!string.IsNullOrEmpty(options.RootPath) && Directory.Exists(options.RootPath))
                        PathUtilities.SetRootPath(options.RootPath);
                });
        }
    }
}
