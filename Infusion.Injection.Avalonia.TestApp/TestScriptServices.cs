using Infusion.Injection.Avalonia.Scripts;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infusion.Injection.Avalonia.TestApp
{
    public class TestScriptServices : IScriptServices
    {
        public IEnumerable<string> RunningScripts { get; } = new[] { "Test", "Qwer" };
        public IEnumerable<string> AvailableScripts { get; } = new[] { "Test", "Qwer", "not_running", "and", "something", "else" };

        public event Action<string> ScriptStarted;
        public event Action<string> ScriptTerminated;

        public void Load(string scriptFileName) { }
        public void Run(string name) { }
        public void Terminate(string name) { }
    }

}
