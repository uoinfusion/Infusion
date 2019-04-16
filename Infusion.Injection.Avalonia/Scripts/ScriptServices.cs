using Infusion.Commands;
using Infusion.LegacyApi.Injection;
using InjectionScript.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infusion.Injection.Avalonia.Scripts
{
    public class ScriptServices : IScriptServices
    {
        private readonly InjectionRuntime injectionRuntime;
        private readonly InjectionHost injectionHost;

        public event Action AvailableScriptsChanged
        {
            add => injectionHost.ScriptLoaded += value;
            remove => injectionHost.ScriptLoaded -= value;
        }

        public event Action RunningScriptsChanged
        {
            add => injectionHost.RunningCommandsChanged += value;
            remove => injectionHost.RunningCommandsChanged -= value;
        }

        public ScriptServices(InjectionRuntime injectionRuntime, InjectionHost injectionHost)
        {
            this.injectionRuntime = injectionRuntime;
            this.injectionHost = injectionHost;
        }

        public IEnumerable<string> RunningScripts => injectionHost.RunningCommands;
        public IEnumerable<string> AvailableScripts => injectionRuntime.Metadata.Subrutines.Select(x => x.Name);

        public void Load(string scriptFileName) => injectionRuntime.Load(scriptFileName);
        public void Run(string name) => injectionHost.ExecSubrutine(name);
        public void Terminate(string name) => injectionRuntime.Api.UO.Terminate(name);
    }
}
