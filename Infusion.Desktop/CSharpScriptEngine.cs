using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using UltimaRX;
using UltimaRX.Proxy.InjectionApi;

namespace Infusion.Desktop
{
    public class CSharpScriptEngine
    {
        private static ScriptState<object> scriptState;

        private static readonly string[] Imports =
        {
            "using System;",
            "using System.Threading;",
            "using UltimaRX.Proxy;",
            "using UltimaRX.Packets;",
            "using UltimaRX.Proxy.InjectionApi;",
            "using UltimaRX.Packets.Parsers;",
            "using UltimaRX.Gumps;",
            "using static UltimaRX.Proxy.InjectionApi.Injection;"
        };

        private readonly IScriptOutput scriptOutput;

        private ScriptOptions scriptOptions = ScriptOptions.Default
            .WithReferences(
                typeof(ServerConnection).Assembly,
                typeof(Injection).Assembly);

        public CSharpScriptEngine(IScriptOutput scriptOutput)
        {
            this.scriptOutput = scriptOutput;
        }

        public async Task AddDefaultImports()
        {
            scriptOutput.Info("Initializing C# scripting...");
            foreach (var import in Imports)
            {
                await Execute(import);
            }
        }

        public async Task ExecuteScript(string scriptPath)
        {
            scriptOutput.Info("Loading script...");
            var scriptLines = File.ReadAllLines(scriptPath);

            string scriptDirectory = Path.GetDirectoryName(scriptPath);
            scriptOptions = scriptOptions.WithSourceResolver(
                ScriptSourceResolver.Default.WithSearchPaths(scriptDirectory));

            await Execute($"System.IO.Directory.SetCurrentDirectory(@\"{scriptDirectory}\");");

            foreach (var line in scriptLines)
            {
                await Execute(line);
            }
        }

        public async Task<object> Execute(string code)
        {
            scriptOutput.Echo(code);

            try
            {
                scriptState = scriptState == null
                    ? await CSharpScript.RunAsync(code, scriptOptions)
                    : await scriptState.ContinueWithAsync(code, scriptOptions);

                var resultText = scriptState?.ReturnValue?.ToString();
                if (!string.IsNullOrEmpty(resultText))
                {
                    scriptOutput.Result(resultText);
                    return scriptState.ReturnValue;
                }
            }
            catch (AggregateException ex)
            {
                scriptOutput.Error(ex.InnerExceptions
                    .Select(inner => inner.Message)
                    .Aggregate((l, r) => l + Environment.NewLine + r));
            }
            catch (Exception ex)
            {
                scriptOutput.Error(ex.Message);
            }

            return null;
        }
    }
}