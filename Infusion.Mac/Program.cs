using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Infusion.Commands;
using Infusion.EngineScripts;
using Infusion.LegacyApi;
using Infusion.LegacyApi.Console;
using Infusion.Proxy;
using Infusion.Utilities;
using Microsoft.CodeAnalysis;
using Ultima;

namespace Infusion.Mac
{
    /// <summary>
    /// Console host for Infusion on macOS. It does the same as Infusion.Desktop on Windows
    /// (proxy, client launch, scripts, logs, profiles), but without WPF: the console is the terminal
    /// and commands (,reload, ,help...) are typed there.
    /// </summary>
    internal static class Program
    {
        private static InfusionProxy proxy;
        private static MacConsole console;
        private static FileConsole fileConsole;
        private static MacProfile profile;
        private static ScriptEngine scriptEngine;
        private static MacScriptEngine csharpEngine;
        private static string scriptFileName;
        private static Process clientProcess;

        private sealed class Options
        {
            public string Profile;
            public string ClientPath;     // ClassicUO for macOS (ClassicUO.bin.osx) or any other client executable
            public string UOPath;         // folder with Ultima Online data files (*.mul)
            public bool NoClient;
            public string Check;          // only compile a script and print errors
            public string Root;
            public string Server;
        }

        public static int Main(string[] args)
        {
            // script compatibility libraries (see Compat/) are not listed in deps.json - look for them next to the host
            System.Runtime.Loader.AssemblyLoadContext.Default.Resolving += (ctx, name) =>
            {
                var candidate = Path.Combine(MacScriptEngine.BinDirectory, name.Name + ".dll");
                return File.Exists(candidate) ? ctx.LoadFromAssemblyPath(candidate) : null;
            };

            var o = ParseArgs(args);
            if (o == null)
                return 1;

            if (!string.IsNullOrEmpty(o.Root))
                PathUtilities.SetRootPath(Path.GetFullPath(o.Root));

            if (!string.IsNullOrEmpty(o.Check))
                return CheckScript(o.Check);

            return Run(o);
        }

        private static Options ParseArgs(string[] args)
        {
            var o = new Options();
            for (int i = 0; i < args.Length; i++)
            {
                string Next() => i + 1 < args.Length ? args[++i] : throw new ArgumentException($"Missing value for {args[i]}");
                switch (args[i])
                {
                    case "--profile": o.Profile = Next(); break;
                    case "--client": o.ClientPath = Next(); break;
                    case "--uo": o.UOPath = Next(); break;
                    case "--root": o.Root = Next(); break;
                    case "--server": o.Server = Next(); break;
                    case "--no-client": o.NoClient = true; break;
                    case "--check": o.Check = Next(); break;
                    case "-h":
                    case "--help":
                        System.Console.WriteLine("Infusion for macOS\n" +
                            "  --profile <name|id>    profile from <root>/profiles (default: the 'selected' one)\n" +
                            "  --client <path>        ClassicUO for macOS (ClassicUO.bin.osx)\n" +
                            "  --uo <path>            folder with Ultima Online data files (*.mul)\n" +
                            "  --root <path>          Infusion root folder (scripts, profiles, logs)\n" +
                            "  --server <host,port>   override the server from the profile\n" +
                            "  --no-client            don't start the client, only the proxy\n" +
                            "  --check <script.csx>   only compile the script and print errors");
                        return null;
                    default:
                        System.Console.WriteLine($"Unknown parameter {args[i]}, use --help");
                        return null;
                }
            }

            return o;
        }

        private static int CheckScript(string path)
        {
            var testConsole = new MacConsole(null) { Filter = ConsoleFilter.NoDebug };
            var engine = new MacScriptEngine(testConsole);
            var full = Path.GetFullPath(path);
            var watch = Stopwatch.StartNew();
            var diagnostics = engine.Compile(full);
            var errors = diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error).ToArray();
            foreach (var d in errors)
                System.Console.WriteLine(d.ToString());
            System.Console.WriteLine($"{errors.Length} errors, {diagnostics.Count(d => d.Severity == DiagnosticSeverity.Warning)} warnings ({watch.ElapsedMilliseconds} ms)");
            return errors.Length == 0 ? 0 : 2;
        }

        private static int Run(Options o)
        {
            proxy = new InfusionProxy();
            fileConsole = new FileConsole(proxy.LogConfig, new CircuitBreaker(HandleFileLoggingException));
            console = new MacConsole(fileConsole);
            proxy.Console = console;

            var commandHandler = new CommandHandler(console);
            proxy.Initialize(commandHandler, new MacSoundPlayer(), new LegacyApi.Injection.NullInjectionWindow());

            var version = typeof(InfusionProxy).Assembly.GetName().Version;
            console.Important($"Infusion {version} (macOS)");
            console.Info($"Infusion root path {PathUtilities.RootPath}");

            var profilesPath = PathUtilities.GetAbsolutePath("profiles");
            profile = MacProfile.Find(profilesPath, o.Profile);
            if (profile == null)
            {
                console.Error($"Profile '{o.Profile}' not found in {profilesPath}. Available: {string.Join(", ", MacProfile.ListNames(profilesPath))}");
                Thread.Sleep(500);
                return 1;
            }

            if (!string.IsNullOrEmpty(o.Server))
                profile.ServerOverride = o.Server;
            console.Info($"Profile: {profile.Name}");
            if (string.IsNullOrEmpty(profile.ServerEndpoint))
            {
                console.Error($"Profile '{profile.Name}' has no server. Use --profile <name> or --server <host,port>. Available: {string.Join(", ", MacProfile.ListNames(profilesPath))}");
                Thread.Sleep(500);
                return 1;
            }
            proxy.ConfigRepository = new ProfileConfigRepository(profile, console);

            try
            {
                Launch(o);
            }
            catch (Exception ex)
            {
                console.Error(ex.InnerException?.Message ?? ex.Message);
                console.Debug(ex.ToString());
                Thread.Sleep(500);
                return 1;
            }

            csharpEngine = new MacScriptEngine(console);
            scriptEngine = new ScriptEngine(csharpEngine, new InjectionScriptEngine(UO.Injection, console));

            RegisterCommands();

            if (!string.IsNullOrEmpty(profile.InitialScriptFileName))
                Load(profile.InitialScriptFileName);

            proxy.LegacyApi.LoginConfirmed += HandleLoginConfirmed;

            System.Console.CancelKeyPress += (s, e) => { Shutdown(); };
            AppDomain.CurrentDomain.ProcessExit += (s, e) => SaveState();

            InputLoop(commandHandler);
            Shutdown();
            return 0;
        }

        private static void Launch(Options o)
        {
            var clientPath = o.ClientPath;
            var uoPath = o.UOPath;

            // Same as on Windows: without an Ultima Online installation the client folder is used.
            if (string.IsNullOrEmpty(Files.RootDir) || !Directory.Exists(Files.RootDir))
            {
                if (!string.IsNullOrEmpty(uoPath))
                {
                    console.Info($"Using Ultima Online files from {uoPath}.");
                    Files.SetMulPath(uoPath);
                }
                else if (!string.IsNullOrEmpty(clientPath))
                {
                    var dir = Path.GetDirectoryName(clientPath);
                    console.Info($"Client path is {clientPath}. Assuming Ultima Online files are in {dir}.");
                    Files.SetMulPath(dir);
                }
            }

            var serverEndPoint = profile.ResolveServerEndpoint().Result;
            var proxyPort = MacProfile.GetFreeLocalPort();

            proxy.Start(new ProxyStartConfig
            {
                ServerAddress = profile.ServerEndpoint,
                ServerEndPoint = serverEndPoint,
                LocalProxyPort = proxyPort,
                ProtocolVersion = profile.ProtocolVersion,
                Encryption = profile.EncryptionSetup,
                LoginEncryptionKey = profile.GetEncryptionKey(),
            });

            if (o.NoClient)
            {
                console.Important($"Client is not started. Connect ClassicUO to 127.0.0.1, port {proxyPort}.");
                return;
            }

            if (string.IsNullOrEmpty(clientPath) || !File.Exists(clientPath))
            {
                console.Error($"Client not found: '{clientPath}'. Start ClassicUO manually and connect it to 127.0.0.1, port {proxyPort}.");
                return;
            }

            var info = new ProcessStartInfo(clientPath)
            {
                WorkingDirectory = Path.GetDirectoryName(clientPath),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
            };
            info.ArgumentList.Add("-ip"); info.ArgumentList.Add("127.0.0.1");
            info.ArgumentList.Add("-port"); info.ArgumentList.Add(proxyPort.ToString());
            if (!string.IsNullOrEmpty(profile.UserName))
            {
                info.ArgumentList.Add("-username"); info.ArgumentList.Add(profile.UserName);
            }
            if (!string.IsNullOrEmpty(profile.Password))
            {
                info.ArgumentList.Add("-password"); info.ArgumentList.Add(profile.Password);
            }

            console.Info($"Starting {clientPath} -ip 127.0.0.1 -port {proxyPort} -username {profile.UserName} -password <censored>");

            clientProcess = Process.Start(info);
            if (clientProcess == null)
            {
                console.Error($"Cannot start {clientPath}.");
                return;
            }

            // client output (FNA messages) goes to a file, so it doesn't flood the Infusion console
            try
            {
                var logDir = Path.Combine(Path.GetDirectoryName(clientPath), "Logs");
                Directory.CreateDirectory(logDir);
                var clientLog = new StreamWriter(Path.Combine(logDir, "client-console.log"), false) { AutoFlush = true };
                var logLock = new object();
                clientProcess.OutputDataReceived += (s, e) => { if (e.Data != null) lock (logLock) clientLog.WriteLine(e.Data); };
                clientProcess.ErrorDataReceived += (s, e) => { if (e.Data != null) lock (logLock) clientLog.WriteLine(e.Data); };
                clientProcess.BeginOutputReadLine();
                clientProcess.BeginErrorReadLine();
            }
            catch (Exception ex)
            {
                console.Debug("Cannot redirect client output: " + ex.Message);
            }

            clientProcess.EnableRaisingEvents = true;
            clientProcess.Exited += (s, e) =>
            {
                console.Important("The client has exited. Use ,exit (or Ctrl+C) to close Infusion.");
            };

            proxy.LegacyApi.ClientWindow = new MacClientWindow(clientProcess);
        }

        private static void RegisterCommands()
        {
            UO.CommandHandler.RegisterCommand(new Command("reload", () => Reload(), false, true,
                "Reloads an initial script file."));
            UO.CommandHandler.RegisterCommand(new Command("load", path => Load(path), false, true,
                "Loads a script file."));
            UO.CommandHandler.RegisterCommand(new Command("edit", () => Edit(), false, true,
                "Opens the initial script in Visual Studio Code (if installed) or in the default text editor."));
            UO.CommandHandler.RegisterCommand(new Command("cls", () => console.Clear(), false, true,
                "Clears console content."));
            UO.CommandHandler.RegisterCommand(new Command("console-show-toggle", () =>
                console.Filter = console.Filter == ConsoleFilter.All ? ConsoleFilter.NoDebug : ConsoleFilter.All, false, true));
            UO.CommandHandler.RegisterCommand(new Command("console-show-speechonly", () => console.Filter = ConsoleFilter.SpeechOnly, false, true));
            UO.CommandHandler.RegisterCommand(new Command("console-show-game", () => console.Filter = ConsoleFilter.Game, false, true));
            UO.CommandHandler.RegisterCommand(new Command("console-show-all", () => console.Filter = ConsoleFilter.All, false, true));
            UO.CommandHandler.RegisterCommand(new Command("console-show-nodebug", () => console.Filter = ConsoleFilter.NoDebug, false, true));
            UO.CommandHandler.RegisterCommand(new Command("exit", () => { Shutdown(); Environment.Exit(0); }, false, true,
                "Exits Infusion."));
        }

        private static void HandleFileLoggingException(Exception ex)
        {
            console.Error($"Error while writing logs to disk. Please, check that Infusion can write to {proxy.LogConfig.LogPath}.");
            console.Debug(ex.ToString());
        }

        private static void HandleLoginConfirmed()
        {
            var logPath = PathUtilities.GetAbsolutePath(Path.Combine("logs", proxy.LegacyApi.ServerName ?? "server",
                profile.UserName ?? "account", proxy.LegacyApi.SelectedCharacterName ?? "char") + Path.DirectorySeparatorChar);
            proxy.LogConfig.SetDefaultLogPath(logPath);
        }

        private static void Load(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                console.Error("Missing script file name.");
                return;
            }

            fileName = MacPath.Normalize(fileName);
            string name = Path.GetFileName(fileName);
            var scriptPath = Path.GetDirectoryName(fileName);

            if (string.IsNullOrEmpty(scriptPath))
                scriptPath = PathUtilities.GetAbsolutePath("scripts");
            else if (!Path.IsPathRooted(scriptPath))
                scriptPath = Path.Combine(PathUtilities.GetAbsolutePath("scripts"), scriptPath);

            scriptFileName = MacPath.FindCaseInsensitive(Path.Combine(scriptPath, name));
            scriptEngine.ScriptRootPath = Path.GetDirectoryName(scriptFileName);

            Reload();
        }

        private static void Reload() => _ = ReloadAsync();

        private static async Task ReloadAsync()
        {
            if (string.IsNullOrEmpty(scriptFileName))
            {
                console.Error("Initial script is not set. Use ,load <script.csx>");
                return;
            }

            if (!File.Exists(scriptFileName))
            {
                console.Error($"Script file doesn't exist: '{scriptFileName}'");
                return;
            }

            SaveState();
            profile.FixOptions();

            UO.CommandHandler.BeginTerminate(true);
            UO.CommandHandler.UnregisterAllPublic();

            scriptEngine.Reset();
            try
            {
                using var tokenSource = new CancellationTokenSource();
                await scriptEngine.ExecuteScript(scriptFileName, tokenSource);
            }
            catch (Exception ex)
            {
                console.Error(ex.ToString());
            }
        }

        private static readonly string[] VsCodeCliCandidates =
        {
            "/Applications/Visual Studio Code.app/Contents/Resources/app/bin/code",
            "/usr/local/bin/code",
            "/opt/homebrew/bin/code",
        };

        private static void Edit()
        {
            if (string.IsNullOrEmpty(scriptFileName) || !File.Exists(scriptFileName))
            {
                console.Error("Initial script is not set.");
                return;
            }

            // Visual Studio Code opens the whole scripts folder (so #load-ed files are at hand),
            // otherwise the default text editor.
            try
            {
                var home = Environment.GetEnvironmentVariable("HOME") ?? "";
                var codeCli = VsCodeCliCandidates
                    .Append(Path.Combine(home, "Applications/Visual Studio Code.app/Contents/Resources/app/bin/code"))
                    .FirstOrDefault(File.Exists);
                if (codeCli != null)
                {
                    var psi = new ProcessStartInfo("/bin/bash") { UseShellExecute = false };
                    psi.ArgumentList.Add(codeCli);
                    psi.ArgumentList.Add(Path.GetDirectoryName(scriptFileName));
                    psi.ArgumentList.Add(scriptFileName);
                    Process.Start(psi);
                    return;
                }
                Process.Start("/usr/bin/open", new[] { "-t", scriptFileName });
            }
            catch (Exception ex) { console.Error(ex.Message); }
        }

        private static readonly object saveLock = new object();

        private static void SaveState()
        {
            lock (saveLock)
            {
                try { proxy?.LegacyApi?.Config?.Save(); } catch { }
                profile?.Save(console);
            }
        }

        private static int shutdownDone;

        private static void Shutdown()
        {
            if (Interlocked.Exchange(ref shutdownDone, 1) == 1)
                return;
            SaveState();
            fileConsole?.Dispose();
        }

        private static void InputLoop(CommandHandler commandHandler)
        {
            console.Info("Type commands here (e.g. ,reload or ,help). Text without a comma is said in game. Ctrl+C exits.");

            while (true)
            {
                string line;
                try { line = System.Console.ReadLine(); }
                catch { line = null; }

                if (line == null)
                {
                    // stdin is closed (started without a terminal) - just wait while the client runs
                    while (clientProcess != null && !clientProcess.HasExited)
                        Thread.Sleep(1000);
                    return;
                }

                line = line.Trim();
                if (line.Length == 0)
                    continue;

                if (commandHandler.IsInvocationSyntax(line))
                {
                    if (line != ",cls")
                        console.Debug(line);
                    Task.Run(() =>
                    {
                        try { commandHandler.InvokeSyntax(line); }
                        catch (Exception ex) { console.Error(ex.ToString()); }
                    });
                }
                else if (UO.IsLoginConfirmed)
                {
                    UO.Say(line);
                }
                else
                {
                    console.Error("Cannot say anything, character is not in game yet.");
                }
            }
        }
    }
}
