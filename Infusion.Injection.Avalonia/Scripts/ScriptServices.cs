using Infusion.LegacyApi.Injection;
using InjectionScript.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infusion.Injection.Avalonia.Scripts
{
    public class ScriptServices : IScriptServices
    {
        private readonly InjectionRuntime injectionRuntime;
        private readonly InjectionHost injectionHost;

        public ScriptServices(InjectionRuntime injectionRuntime, InjectionHost injectionHost)
        {
            this.injectionRuntime = injectionRuntime;
            this.injectionHost = injectionHost;
        }

        public IEnumerable<string> RunningScripts { get; } = new[] { "Test", "Qwer" };
        public IEnumerable<string> AvailableScripts { get; } = new[] { "Test", "Qwer", "not_running", "and", "something", "else" };

        public event Action<string> ScriptStarted;
        public event Action<string> ScriptTerminated;

        public void Load(string scriptFileName) => injectionRuntime.Load(scriptFileName);
        public void Run(string name) => injectionRuntime.Exec(name);
        public void Terminate(string name) => injectionRuntime.Api.UO.Terminate(name);
    }
}
