using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
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
using Microsoft.CodeAnalysis.Text;
using RoslynPad.UI;

namespace Infusion.Desktop
{
    public class CSharpScriptEngine : IScriptEngine
    {
        private class Resolver : SourceFileResolver
        {
            private readonly IScriptOutput output;

            public Resolver(ScriptSourceResolver resolver, IScriptOutput output) : base(resolver.SearchPaths, resolver.BaseDirectory)
            {
                this.output = output;
            }

            public override string ResolveReference(string path, string baseFilePath)
            {
                output.Debug($"Referencing {path} from {baseFilePath}");

                return base.ResolveReference(path, baseFilePath);
            }
        }

        private ScriptState<object> scriptState;

        private readonly IScriptOutput scriptOutput;

        private ScriptOptions scriptOptions;
        private string scriptRootPath;

        public string ScriptRootPath
        {
            get => scriptRootPath;
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
                .WithImports("Infusion", "Infusion.LegacyApi", "Infusion.LegacyApi.Events", "Infusion.Gumps")
                .WithReferences(
                Assembly.Load("Infusion"),
                Assembly.Load("System.Collections.Immutable"),
                Assembly.Load("Infusion.LegacyApi"));
        }

        public async Task ExecuteScript(string scriptPath, CancellationTokenSource cancellationTokenSource)
        {
            string scriptText = File.ReadAllText(scriptPath);

            await Execute(scriptText, scriptPath, true, cancellationTokenSource);
        }

        private int submissionNumber = 0;

        public void Reset()
        {
            scriptState = null;
        }

        public Task<object> Execute(string code, string filePath, bool wholeFile, CancellationTokenSource cancellationTokenSource = null)
        {
            string binDirectory = Path.GetDirectoryName(GetType().Assembly.Location);

            bool fileExists;
            string scriptDirectory;
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                scriptDirectory = ScriptRootPath;
                fileExists = false;
            }
            else
            {
                scriptDirectory = Path.GetDirectoryName(filePath);
                fileExists = true;
            }

            if (!string.IsNullOrEmpty(scriptDirectory))
            {
                scriptOptions = scriptOptions
                    .WithEmitDebugInformation(true)
                    .WithFilePath(filePath)
                    .WithFileEncoding(System.Text.Encoding.Default)
                    .WithSourceResolver(new Resolver(ScriptSourceResolver.Default
                        .WithSearchPaths(scriptDirectory)
                        .WithBaseDirectory(scriptDirectory), scriptOutput))
                    .WithMetadataResolver(ScriptMetadataResolver.Default
                        .WithSearchPaths(scriptDirectory, binDirectory)
                        .WithBaseDirectory(scriptDirectory));
            }

            if (!string.IsNullOrEmpty(ScriptRootPath))
                Directory.SetCurrentDirectory(ScriptRootPath);

            submissionNumber++;
            string commandName = $"submission{submissionNumber}";

            return Task.Run(() =>
            {
                object result = null;

                var command = new Command(commandName, () =>
                {
                    if (wholeFile && fileExists)
                        scriptOutput.Info($"Executing file {filePath}.");
                    else
                        scriptOutput.Echo(code);

                    try
                    {
                        var watch = Stopwatch.StartNew();
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

                        scriptOutput.Info($"OK (in {watch.ElapsedMilliseconds} ms)");
                    }
                    catch (CompilationErrorException compilationErrorEx)
                    {
                        foreach (var diagnostic in compilationErrorEx.Diagnostics)
                        {
                            scriptOutput.Error(diagnostic.ToString());
                        }
                    }
                    catch (AggregateException ex)
                    {
                        scriptOutput.Error(ex.InnerExceptions
                            .Select(inner => inner.ToString())
                            .Aggregate((l, r) => l + Environment.NewLine + r));
                    }
                    catch (Exception ex)
                    {
                        scriptOutput.Error(ex.ToString());
                    }

                    result = null;
                }, executionMode: CommandExecutionMode.Direct);

                UO.CommandHandler.RegisterCommand(command);
                UO.CommandHandler.InvokeSyntax("," + commandName, null, cancellationTokenSource);
                UO.CommandHandler.Unregister(command);

                return result;
            }, cancellationTokenSource?.Token ?? default(CancellationToken));
        }
    }
}

