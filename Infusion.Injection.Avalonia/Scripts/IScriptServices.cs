using System;
using System.Collections.Generic;

namespace Infusion.Injection.Avalonia.Scripts
{
    public interface IScriptServices
    {
        IEnumerable<string> AvailableScripts { get; }
        IEnumerable<string> RunningScripts { get; }

        event Action AvailableScriptsChanged;
        event Action RunningScriptsChanged;

        void Run(string name);
        void Terminate(string name);
        void Load(string scriptFileName);
    }
}