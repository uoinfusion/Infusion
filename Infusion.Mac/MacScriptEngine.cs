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
using Infusion.EngineScripts;
using Infusion.LegacyApi;
using Infusion.LegacyApi.Console;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Infusion.Mac
{
    /// <summary>
    /// Replacement of Infusion.EngineScripts.CSharpScriptEngine for macOS:
    ///  - converts Windows separators in #load / #r paths (UOErebor\Specs.csx) to '/',
    ///  - finds files case-insensitively (scripts written on Windows often differ in letter case),
    ///  - redirects Windows-only script libraries (e.g. the WPF Infusion.Scripts.UOErebor.Extensions.dll)
    ///    to cross-platform versions placed next to the host.
    /// </summary>
    internal sealed class MacScriptEngine : IScriptEngine
    {
        private ScriptState<object> scriptState;
        private readonly IConsole scriptOutput;
        private ScriptOptions scriptOptions;
        private string scriptRootPath;
        private int submissionNumber;

        internal static string BinDirectory => Path.GetDirectoryName(typeof(MacScriptEngine).Assembly.Location);

        public MacScriptEngine(IConsole scriptOutput)
        {
            this.scriptOutput = scriptOutput;

            var references = new List<MetadataReference>();
            scriptOptions = ScriptOptions.Default
                .WithImports("Infusion", "Infusion.LegacyApi", "Infusion.LegacyApi.Events", "Infusion.Gumps")
                .WithReferences(
                    Assembly.Load("Infusion"),
                    Assembly.Load("System.Collections.Immutable"),
                    Assembly.Load("Infusion.LegacyApi"));

            // Compatibility replacements of Windows libraries used by scripts (System.Windows.Extensions = SoundPlayer);
            // on Windows they come with the WindowsDesktop runtime, here they are taken from the host folder.
            foreach (var compat in new[] { "System.Windows.Extensions.dll" })
            {
                var path = Path.Combine(BinDirectory, compat);
                if (File.Exists(path))
                    scriptOptions = scriptOptions.AddReferences(MetadataReference.CreateFromFile(path));
            }
        }

        public string ScriptRootPath
        {
            get => scriptRootPath;
            set
            {
                scriptRootPath = value;
                scriptOptions = scriptOptions.WithSourceResolver(new MacSourceResolver(value, scriptOutput));
            }
        }

        public void Reset() => scriptState = null;

        public async Task ExecuteScript(string scriptPath, CancellationTokenSource cancellationTokenSource)
        {
            if (!scriptPath.EndsWith(".csx", StringComparison.OrdinalIgnoreCase))
                return;

            string scriptText = File.ReadAllText(scriptPath);
            await Execute(scriptText, scriptPath, true, cancellationTokenSource);
        }

        private ScriptOptions PrepareOptions(string filePath, out bool fileExists)
        {
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
                    .WithFileEncoding(System.Text.Encoding.UTF8)
                    .WithSourceResolver(new MacSourceResolver(scriptDirectory, scriptOutput))
                    .WithMetadataResolver(new MacMetadataResolver(scriptDirectory, BinDirectory));
            }

            return scriptOptions;
        }

        /// <summary>Only compiles the script (without running it) and returns diagnostics - for --check.</summary>
        public ImmutableArray<Microsoft.CodeAnalysis.Diagnostic> Compile(string scriptPath)
        {
            ScriptRootPath = Path.GetDirectoryName(scriptPath);
            var options = PrepareOptions(scriptPath, out _);
            var script = CSharpScript.Create(File.ReadAllText(scriptPath), options);
            return script.Compile();
        }

        public Task<object> Execute(string code, string filePath, bool wholeFile, CancellationTokenSource cancellationTokenSource = null)
        {
            var options = PrepareOptions(filePath, out bool fileExists);

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
                        scriptOutput.WriteLine(ConsoleLineType.Information, $"Executing file {filePath}.");
                    else
                        scriptOutput.WriteLine(ConsoleLineType.ScriptEcho, code);

                    try
                    {
                        var watch = Stopwatch.StartNew();
                        var previousState = scriptState;
                        var token = cancellationTokenSource?.Token ?? default(CancellationToken);
                        scriptState = previousState == null
                            ? CSharpScript.RunAsync(code, options, cancellationToken: token).Result
                            : previousState.ContinueWithAsync(code, options, token).Result;

                        var resultText = scriptState?.ReturnValue?.ToString();
                        if (!string.IsNullOrEmpty(resultText))
                        {
                            scriptOutput.WriteLine(ConsoleLineType.ScriptResult, resultText);
                            result = scriptState.ReturnValue;
                            return;
                        }

                        scriptOutput.WriteLine(ConsoleLineType.Information, $"OK (in {watch.ElapsedMilliseconds} ms)");
                    }
                    catch (CompilationErrorException compilationErrorEx)
                    {
                        foreach (var diagnostic in compilationErrorEx.Diagnostics)
                            scriptOutput.WriteLine(ConsoleLineType.Error, diagnostic.ToString());
                    }
                    catch (AggregateException ex)
                    {
                        var inner = ex.Flatten().InnerExceptions;
                        if (inner.Count == 1 && inner[0] is CompilationErrorException cee)
                        {
                            foreach (var diagnostic in cee.Diagnostics)
                                scriptOutput.WriteLine(ConsoleLineType.Error, diagnostic.ToString());
                        }
                        else
                        {
                            scriptOutput.WriteLine(ConsoleLineType.Error, inner
                                .Select(x => x.ToString())
                                .Aggregate((l, r) => l + Environment.NewLine + r));
                        }
                    }
                    catch (Exception ex)
                    {
                        scriptOutput.WriteLine(ConsoleLineType.Error, ex.ToString());
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

    internal static class MacPath
    {
        /// <summary>Converts Windows paths and finds existing files regardless of letter case.</summary>
        public static string Normalize(string path)
            => string.IsNullOrEmpty(path) ? path : path.Replace('\\', '/');

        public static string FindCaseInsensitive(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath) || File.Exists(fullPath) || Directory.Exists(fullPath))
                return fullPath;

            var root = Path.GetPathRoot(fullPath);
            if (string.IsNullOrEmpty(root))
                return fullPath;

            var current = root;
            foreach (var part in fullPath.Substring(root.Length).Split('/', StringSplitOptions.RemoveEmptyEntries))
            {
                if (part == ".") continue;
                if (part == "..") { current = Path.GetDirectoryName(current) ?? current; continue; }

                var candidate = Path.Combine(current, part);
                if (!File.Exists(candidate) && !Directory.Exists(candidate) && Directory.Exists(current))
                {
                    var match = Directory.EnumerateFileSystemEntries(current)
                        .FirstOrDefault(e => string.Equals(Path.GetFileName(e), part, StringComparison.OrdinalIgnoreCase));
                    if (match != null)
                        candidate = match;
                }

                current = candidate;
            }

            return current;
        }
    }

    internal sealed class MacSourceResolver : SourceReferenceResolver
    {
        private readonly SourceFileResolver inner;
        private readonly IConsole output;
        private readonly string baseDirectory;

        public MacSourceResolver(string baseDirectory, IConsole output)
        {
            this.baseDirectory = baseDirectory;
            this.output = output;
            inner = new SourceFileResolver(ImmutableArray<string>.Empty, baseDirectory);
        }

        public override string NormalizePath(string path, string baseFilePath)
            => inner.NormalizePath(MacPath.Normalize(path), baseFilePath);

        public override string ResolveReference(string path, string baseFilePath)
        {
            path = MacPath.Normalize(path);
            output?.WriteLine(ConsoleLineType.Debug, $"Referencing {path} from {baseFilePath}");

            var resolved = inner.ResolveReference(path, baseFilePath);
            if (resolved != null)
                return resolved;

            // case-insensitive fallback
            var baseDir = !string.IsNullOrEmpty(baseFilePath) ? Path.GetDirectoryName(baseFilePath) : baseDirectory;
            var candidate = MacPath.FindCaseInsensitive(Path.GetFullPath(Path.Combine(baseDir ?? baseDirectory, path)));
            return File.Exists(candidate) ? candidate : null;
        }

        public override Stream OpenRead(string resolvedPath)
        {
            var stream = inner.OpenRead(resolvedPath);
            if (!string.Equals(Path.GetFileName(resolvedPath), "uoam.csx", StringComparison.OrdinalIgnoreCase))
                return stream;

            // macOS: user32 calls (FindWindowA/SendMessage to the UOAM window) are redirected to a TCP bridge to UOAM running in Wine
            string text;
            using (var reader = new StreamReader(stream))
                text = reader.ReadToEnd();
            text = MacScriptPatches.PatchUoam(text);
            return new MemoryStream(System.Text.Encoding.UTF8.GetBytes(text));
        }

        public override bool Equals(object other)
            => other is MacSourceResolver r && string.Equals(r.baseDirectory, baseDirectory, StringComparison.Ordinal);

        public override int GetHashCode() => baseDirectory?.GetHashCode() ?? 0;
    }

    internal sealed class MacMetadataResolver : MetadataReferenceResolver
    {
        private static readonly HashSet<string> RedirectedToBin = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Infusion.Scripts.UOErebor.Extensions.dll",
            "System.Windows.Extensions.dll",
        };

        private readonly ScriptMetadataResolver inner;
        private readonly string scriptDirectory;
        private readonly string binDirectory;

        public MacMetadataResolver(string scriptDirectory, string binDirectory)
        {
            this.scriptDirectory = scriptDirectory;
            this.binDirectory = binDirectory;
            inner = ScriptMetadataResolver.Default
                .WithSearchPaths(scriptDirectory, binDirectory)
                .WithBaseDirectory(scriptDirectory);
        }

        public override ImmutableArray<PortableExecutableReference> ResolveReference(string reference, string baseFilePath, MetadataReferenceProperties properties)
        {
            reference = MacPath.Normalize(reference);

            var fileName = Path.GetFileName(reference);
            if (RedirectedToBin.Contains(fileName))
            {
                var macVersion = Path.Combine(binDirectory, fileName);
                if (File.Exists(macVersion))
                    return ImmutableArray.Create(MetadataReference.CreateFromFile(macVersion, properties));
            }

            var result = inner.ResolveReference(reference, baseFilePath, properties);
            if (!result.IsDefaultOrEmpty)
                return result;

            // case-insensitive fallback relative to the script, then by file name next to the host
            var baseDir = !string.IsNullOrEmpty(baseFilePath) ? Path.GetDirectoryName(baseFilePath) : scriptDirectory;
            if (reference.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
            {
                var candidate = MacPath.FindCaseInsensitive(Path.GetFullPath(Path.Combine(baseDir ?? scriptDirectory, reference)));
                if (File.Exists(candidate))
                    return ImmutableArray.Create(MetadataReference.CreateFromFile(candidate, properties));

                var inBin = Path.Combine(binDirectory, fileName);
                if (File.Exists(inBin))
                    return ImmutableArray.Create(MetadataReference.CreateFromFile(inBin, properties));
            }

            return result;
        }

        public override bool ResolveMissingAssemblies => inner.ResolveMissingAssemblies;

        public override PortableExecutableReference ResolveMissingAssembly(MetadataReference definition, AssemblyIdentity referenceIdentity)
            => inner.ResolveMissingAssembly(definition, referenceIdentity);

        public override bool Equals(object other)
            => other is MacMetadataResolver r && r.scriptDirectory == scriptDirectory && r.binDirectory == binDirectory;

        public override int GetHashCode() => (scriptDirectory ?? "").GetHashCode() ^ (binDirectory ?? "").GetHashCode();
    }
}

namespace Infusion.Mac
{
    internal static class MacScriptPatches
    {
        public static string PatchUoam(string text)
        {
            var lines = text.Replace("\r\n", "\n").Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains("extern "))
                    continue;
                lines[i] = System.Text.RegularExpressions.Regex.Replace(lines[i], @"\bFindWindowA\(",
                    "Infusion.Scripts.UOErebor.Extensions.Uoam.Client.MacFindUoam(");
                lines[i] = System.Text.RegularExpressions.Regex.Replace(lines[i], @"(?<![\w.])SendMessage\(",
                    "Infusion.Scripts.UOErebor.Extensions.Uoam.Client.MacSendMessage(");
            }
            return string.Join("\n", lines);
        }
    }
}
