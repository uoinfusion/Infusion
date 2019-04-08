using System;
using System.IO;
using System.Net;
using System.Threading;
using Infusion.Commands;
using Infusion.EngineScripts;
using Infusion.LegacyApi;
using Infusion.LegacyApi.Console;
using Infusion.Logging;
using Infusion.Proxy;
using Microsoft.Extensions.CommandLineUtils;

namespace Infusion.Launcher.NetCore
{
    class Launcher
    {
        internal static IConsole Console { get; set; } = new TextConsole();
        internal static ScriptEngine ScriptEngine { get; private set; }
        public static ILogger Diagnostic;
        public static CSharpScriptEngine CSharpScriptEngine { get; private set; }
        private static readonly IConsole scriptOutput = Console;
        static readonly CommandHandler commandHandler = new CommandHandler(Diagnostic);
        private static string ScriptFileName { get; set; }

        static void Main(string[] args)
        {
            var app = new CommandLineApplication();
            Program.Initialize(commandHandler);
            if (app != null)
            {
                app.Name = "Infusion Laucher NetCore";
            }

            app.HelpOption("-?|-h|--help");

            var serverAdress = app.Option("-l|--login",
                    "Ultima online login server you what to play.",
                    CommandOptionType.SingleValue);

            var protocolVersion = app.Option("-p|--protocol",
                    "Ultima online server protocol",
                    CommandOptionType.SingleValue);

            var scriptPath = app.Option("-s|--script",
                    "Full path to your startup script",
                    CommandOptionType.SingleValue);

            app.OnExecute(() =>
            {
                if (serverAdress.HasValue() && protocolVersion.HasValue())
                {
                    string[] loginServer = serverAdress.Value().Split(",");
                    Program.Start(new ProxyStartConfig()
                    {
                        ServerAddress = "127.0.0.1",
                        ServerEndPoint = new IPEndPoint(IPAddress.Parse(loginServer[0]), int.Parse(loginServer[1])),
                        LocalProxyPort = 60000,
                        ProtocolVersion = new Version(protocolVersion.Value()),

                    });

                    UO.CommandHandler.RegisterCommand(new Command("reload", () => Reload(), false, true,
                        "Reloads an initial script file."));

                    CSharpScriptEngine = new CSharpScriptEngine(Console);
                    ScriptEngine = new ScriptEngine(CSharpScriptEngine, new InjectionScriptEngine(UO.Injection, Console));
                   
                }
                else
                {
                    app.ShowHint();
                }
                if (scriptPath.HasValue())
                {
                    Load(scriptPath.Value());
                }
                return 0;
            });

            app.Execute(args);
            System.Console.ReadLine();
        }

        private static void Load(string scriptFileName)
        {
            string name = Path.GetFileName(scriptFileName);
            var scriptPath = Path.GetDirectoryName(scriptFileName);

            ScriptFileName = Path.Combine(scriptPath, name);

            ScriptFileName = scriptFileName;
            ScriptEngine.ScriptRootPath = scriptPath;

            Reload();
        }

        private async static void Reload()
        {
            UO.CommandHandler.BeginTerminate(true);
            UO.CommandHandler.UnregisterAllPublic();

            ScriptEngine.Reset();
            using (var tokenSource = new CancellationTokenSource())
            {
                await ScriptEngine.ExecuteScript(ScriptFileName, tokenSource);
            }
        }
    }
}
