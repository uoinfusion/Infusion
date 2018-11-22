using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infusion.Desktop.Scripts
{
    public interface IScriptEngine
    {
        string ScriptRootPath { get; set; }
        void Reset();
        Task ExecuteScript(string scriptPath, CancellationTokenSource cancellationTokenSource);
    }
}
