using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Infusion.Commands;
using Infusion.LegacyApi;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using RoslynPad.UI;

namespace Infusion.Desktop
{
    public class CSharpScriptEngine : IScriptEngine
    {
        private static ScriptState<object> scriptState;

        private readonly IScriptOutput scriptOutput;

        private ScriptOptions scriptOptions;
        private string scriptRootPath;

        public string ScriptRootPath
        {
            get { return scriptRootPath; }
            set
            {
                scriptRootPath = value;
                scriptOptions =
                    scriptOptions.WithSourceResolver(new SourceFileResolver(ImmutableArray<string>.Empty, value));
            }
        }

        public CSharpScriptEngine(IScriptOutput scriptOutput)
        {
            this.scriptOutput = scriptOutput;
            scriptOptions = ScriptOptions.Default
                .WithImports("System.Linq", "Infusion.LegacyApi", "Infusion.Packets", "Infusion.Gumps", "Infusion.Packets")
                .WithReferences(
                Assembly.Load("Infusion"),
                Assembly.Load("Infusion.LegacyApi"));
        }

        public async Task ExecuteScript(string scriptPath)
        {
            scriptOutput.Info($"Loading script: {scriptPath}");
            string scriptText = File.ReadAllText(scriptPath);

            string scriptDirectory = Path.GetDirectoryName(scriptPath);
            scriptOptions = scriptOptions.WithSourceResolver(
                    ScriptSourceResolver.Default.WithSearchPaths(scriptDirectory))
                .WithMetadataResolver(ScriptMetadataResolver.Default.WithSearchPaths(scriptDirectory));

            Directory.SetCurrentDirectory(scriptDirectory);

            await Execute(scriptText, false);
        }

        private static int submissionNumber = 0;

        public void Reset()
        {
            scriptState = null;
        }

        public Task<object> Execute(string code, bool echo = true, CancellationTokenSource cancellationTokenSource = null)
        {
            submissionNumber++;
            string commandName = $"submission{submissionNumber}";

            return Task.Run(() =>
            {
                object result = null;

                var command = new Command(commandName, () =>
                {
                    if (echo)
                        scriptOutput.Echo(code);

                    try
                    {
                        var previousState = scriptState;
                        scriptState = previousState == null
                            ? CSharpScript.RunAsync(code, scriptOptions, cancellationToken: cancellationTokenSource?.Token ?? default(CancellationToken))
                                .Result
                            : previousState.ContinueWithAsync(code, scriptOptions, cancellationTokenSource?.Token ?? default(CancellationToken))
                                .Result;

                        var resultText = scriptState?.ReturnValue?.ToString();
                        if (!string.IsNullOrEmpty(resultText))
                        {
                            scriptOutput.Result(resultText);
                            result = scriptState.ReturnValue;
                            return;
                        }

                        scriptOutput.Info("OK");
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

                    result = null;
                }, executionMode: CommandExecutionMode.Direct);

                UO.CommandHandler.RegisterCommand(command);
                UO.CommandHandler.Invoke("," + commandName, cancellationTokenSource);
                UO.CommandHandler.Unregister(command);

                return result;
            }, cancellationTokenSource?.Token ?? default(CancellationToken));
        }
    }
}