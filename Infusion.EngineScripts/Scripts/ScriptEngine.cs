using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Infusion.EngineScripts
{
    public sealed class ScriptEngine
    {
        private readonly IScriptEngine[] engines;
        private string scriptRootPath;

        public ScriptEngine(params IScriptEngine[] engines)
        {
            this.engines = engines;
        }

        public string ScriptRootPath
        {
            get => scriptRootPath;
            set
            {
                scriptRootPath = value;
                ForeachEngine(engine => engine.ScriptRootPath = value);
            }
        }

        public Task ExecuteInitialScript(string scriptPath, CancellationTokenSource cancellationTokenSource)
        {
            var currentRoot = Path.GetDirectoryName(scriptPath);
            Directory.SetCurrentDirectory(currentRoot);
            ScriptRootPath = currentRoot;

            return ExecuteScript(scriptPath, cancellationTokenSource);
        }

        public async Task ExecuteScript(string scriptPath, CancellationTokenSource cancellationTokenSource)
        {
            if (!File.Exists(scriptPath))
                throw new FileNotFoundException($"Script file not found: {scriptPath}", scriptPath);
            await ForeachEngineAsync(engine => engine.ExecuteScript(scriptPath, cancellationTokenSource));
        }

        public void Reset() => ForeachEngine(engine => engine.Reset());

        private void ForeachEngine(Action<IScriptEngine> engineAction)
        {
            foreach (var engine in engines)
                engineAction(engine);
        }

        private async Task ForeachEngineAsync(Func<IScriptEngine, Task> engineAction)
        {
            foreach (var engine in engines)
                await engineAction(engine);
        }
    }
}
