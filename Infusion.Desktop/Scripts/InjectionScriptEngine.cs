using System;
using System.Threading;
using System.Threading.Tasks;
using Infusion.LegacyApi.Injection;

namespace Infusion.Desktop.Scripts
{
    public class InjectionScriptEngine : IScriptEngine
    {
        private readonly InjectionHost injection;

        public InjectionScriptEngine(InjectionHost injection)
        {
            this.injection = injection;
        }

        public string ScriptRootPath { get; set; }

        public Task ExecuteScript(string scriptPath, CancellationTokenSource cancellationTokenSource)
        {
            if (scriptPath.EndsWith(".sc", StringComparison.OrdinalIgnoreCase))
            {
                return Task.Run(() =>
                {
                    injection.LoadScript(scriptPath);
                });
            }
            else
                return Task.CompletedTask;
        }

        public void Reset() { }
    }
}
