using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.Desktop.CommandLine
{
    internal sealed class Options
    {
        [Option("rootPath", Required = false)]
        public string RootPath { get; set; }
    }
}
