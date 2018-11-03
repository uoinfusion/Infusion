using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.Diagnostic
{
    public interface IDiagnosticConfiguration
    {
        string LogPath { get; }
        bool LogPacketsToFileEnabled { get; }
    }
}
