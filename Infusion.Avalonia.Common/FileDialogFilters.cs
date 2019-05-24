using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infusion.Avalonia.Common
{
    public static class FileDialogFilters
    {
        public static FileDialogFilter Executable { get; } = new FileDialogFilter()
        {
            Name = "Executable (*.exe)",
            Extensions = new List<string> { "exe" }
        };

        public static FileDialogFilter Scripts { get; } = new FileDialogFilter()
        {
            Name = "Script files (*.cs, *.sc)",
            Extensions = new List<string> { "cs", "sc" }
        };
    }
}
