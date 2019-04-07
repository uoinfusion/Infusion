using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Infusion.Commands;
using Infusion.EngineScripts;
using Infusion.LegacyApi;
using Infusion.LegacyApi.Console;
using Infusion.Logging;
using Infusion.Proxy;

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

        static void Main(string[] args)
        {
            Program.Initialize(commandHandler);
            Program.Start(new ProxyStartConfig()
            {
                ServerAddress = "127.0.0.1",
                ServerEndPoint = new IPEndPoint(IPAddress.Parse("40.86.213.160"), 2593),
                LocalProxyPort = 60000,
                ProtocolVersion = new Version(3, 0, 0),

            });

            UO.CommandHandler.RegisterCommand(new Command("reload",() => Reload(), false, true,
                "Reloads an initial script file."));

            CSharpScriptEngine = new CSharpScriptEngine(Console);
            ScriptEngine = new ScriptEngine(CSharpScriptEngine, new InjectionScriptEngine(UO.Injection, Console));

            ScriptEngine.Reset();
            ScriptEngine.ExecuteScript("../scripts/startup.csx", new CancellationTokenSource());

            System.Console.ReadLine();
        }

        public async static void Launch()
        {
             await Proxy();
        }

        internal static Task Proxy()
        {
            return Task.Run(() =>
            {
                var proxyTask = Program.Start(new ProxyStartConfig()
                {
                    ServerAddress = "127.0.0.1",
                    ServerEndPoint = new IPEndPoint(IPAddress.Parse("40.86.213.160"), 2593),
                    LocalProxyPort = 60000,
                    ProtocolVersion = new Version(3, 0, 0),

                });
            });
        }

        private async static void Reload()
        {
            UO.CommandHandler.BeginTerminate(true);
            UO.CommandHandler.UnregisterAllPublic();

            ScriptEngine.Reset();
            using (var tokenSource = new CancellationTokenSource())
            {
                await ScriptEngine.ExecuteScript("../startup.csx", tokenSource);
            }
        }
    }
}
