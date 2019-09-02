using Infusion.Commands;
using Infusion.EngineScripts;
using Infusion.LegacyApi;
using Infusion.LegacyApi.Console;
using Infusion.Logging;
using Infusion.Proxy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Infusion.Cli
{
    internal sealed class LaunchCommand
    {
        private IConsole console  = new TextConsole();
        private ScriptEngine scriptEngine;
        private ILogger diagnostic;
        private CSharpScriptEngine csharpScriptEngine;
        private readonly CommandHandler commandHandler;
        private string scriptFileName;

        private readonly string serverAddress;
        private readonly Version protocolVersion;

        public LaunchCommand(string serverAddress, Version protocolVersion, string scriptFileName)
        {
            this.serverAddress = serverAddress;
            this.protocolVersion = protocolVersion;
            this.scriptFileName = scriptFileName;

            diagnostic = console;
            commandHandler = new CommandHandler(diagnostic);
        }

        public void Execute()
        {
            var proxy = new InfusionProxy();
            proxy.Initialize(commandHandler, new NullSoundPlayer(), new LegacyApi.Injection.NullInjectionWindow());

            string[] loginServer = serverAddress.Split(",");

            var resolvedServerAddress = Dns.GetHostEntry(loginServer[0])
                .AddressList.First(a => a.AddressFamily == AddressFamily.InterNetwork);

            proxy.Start(new ProxyStartConfig()
            {
                ServerAddress = serverAddress,
                ServerEndPoint = new IPEndPoint(resolvedServerAddress, int.Parse(loginServer[1])),
                LocalProxyPort = 60000,
                ProtocolVersion = protocolVersion,
                Encryption = EncryptionSetup.Autodetect,
            });

            UO.CommandHandler.RegisterCommand(new Command("reload", () => Reload(), false, true,
                "Reloads an initial script file."));

            csharpScriptEngine = new CSharpScriptEngine(console);
            scriptEngine = new ScriptEngine(csharpScriptEngine, new InjectionScriptEngine(UO.Injection, console));

            if (!string.IsNullOrEmpty(scriptFileName))
            {
                Load(scriptFileName);
            }

            System.Console.ReadLine();
        }

        private void Load(string scriptFileName)
        {
            string name = Path.GetFileName(scriptFileName);
            var scriptPath = Path.GetDirectoryName(scriptFileName);

            scriptFileName = Path.Combine(scriptPath, name);

            this.scriptFileName = scriptFileName;
            scriptEngine.ScriptRootPath = scriptPath;

            Reload();
        }

        private async void Reload()
        {
            UO.CommandHandler.BeginTerminate(true);
            UO.CommandHandler.UnregisterAllPublic();

            scriptEngine.Reset();
            using (var tokenSource = new CancellationTokenSource())
            {
                await scriptEngine.ExecuteScript(scriptFileName, tokenSource);
            }
        }
    }
}
