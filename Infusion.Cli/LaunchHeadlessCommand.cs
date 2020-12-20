using Infusion.Commands;
using Infusion.EngineScripts;
using Infusion.IO.Encryption.Login;
using Infusion.LegacyApi;
using Infusion.LegacyApi.Console;
using Infusion.Proxy;
using System;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Infusion.Cli
{
    internal sealed class LaunchHeadlessCommand
    {
        private readonly IConsole console = new TextConsole();
        private readonly CommandHandler commandHandler;
        private readonly string scriptFileName;
        private readonly InfusionProxy infusionProxy;

        private readonly string serverAddress;
        private readonly int serverPort;
        private readonly int proxyPort;
        private readonly Version clientVersion;
        private readonly bool encrypt;

        private readonly string accountName;
        private readonly string accountPassword;
        private readonly string shardName;
        private readonly string characterName;

        private readonly string pipeName;

        public LaunchHeadlessCommand(LaunchHeadlessOptions options)
        {
            commandHandler = new CommandHandler(console);

            infusionProxy = new InfusionProxy();

            pipeName = options.PipeName;
            scriptFileName = options.ScriptFileName;

            serverAddress = options.ServerAddress;
            serverPort = options.ServerPort;
            proxyPort = options.ProxyPort;
            encrypt = options.Encrypt;
            clientVersion = options.ClientVersion;
            accountName = options.AccountName;
            accountPassword = options.AccountPassword;
            shardName = options.ShardName;
            characterName = options.CharacterName;
        }

        public void StartClient()
        {
            infusionProxy.Initialize(commandHandler, new NullSoundPlayer(), new LegacyApi.Injection.NullInjectionWindow());

            var resolvedServerAddress = Dns.GetHostEntry(serverAddress)
                .AddressList.First(a => a.AddressFamily == AddressFamily.InterNetwork);

            infusionProxy.Start(new ProxyStartConfig()
            {
                ServerAddress = $"{serverAddress},{serverPort}",
                ServerEndPoint = new IPEndPoint(resolvedServerAddress, serverPort),
                LocalProxyPort = (ushort)proxyPort,
                ProtocolVersion = clientVersion,
                Encryption = encrypt ? EncryptionSetup.EncryptedServer : EncryptionSetup.Autodetect,
                LoginEncryptionKey = LoginEncryptionKey.FromVersion(clientVersion),
            });

            var csharpScriptEngine = new CSharpScriptEngine(console);
            var scriptEngine = new ScriptEngine(csharpScriptEngine, new InjectionScriptEngine(UO.Injection, console));

            var headlessClient = new HeadlessClient(infusionProxy.LegacyApi, console, new HeadlessStartConfig()
            {
                Encryption = EncryptionSetup.Autodetect,
                ClientVersion = clientVersion,
                ServerAddress = $"127.0.0.1,{proxyPort}",
                ServerEndPoint = new IPEndPoint(new IPAddress(new byte[] { 127, 0, 0, 1 }), proxyPort),
                ShardName = shardName,
                AccountName = accountName,
                Password = accountPassword,
                CharacterName = characterName
            });

            headlessClient.Connect();

            if (!string.IsNullOrEmpty(scriptFileName))
            {
                _ = ExecuteInitialScript(PathUtilities.GetAbsolutePath(scriptFileName), scriptEngine, infusionProxy.LegacyApi.Console);
            }
        }

        private async Task ExecuteInitialScript(string scriptFileName, ScriptEngine scriptEngine, IConsole console)
        {
            try
            {
                await scriptEngine.ExecuteInitialScript(PathUtilities.GetAbsolutePath(scriptFileName), new CancellationTokenSource());
            }
            catch (Exception ex)
            {
                console.Error(ex.ToString());
            }
        }

        public void ListenPipeCommands()
        {
            while (true)
            {
                using (var pipeServer = new NamedPipeServerStream(pipeName, PipeDirection.In))
                {
                    pipeServer.WaitForConnection();
                    var ss = new StreamString(pipeServer);
                    var command = ss.ReadString();

                    if (commandHandler.IsInvocationSyntax(command))
                        commandHandler.InvokeSyntax(command);
                    else
                        infusionProxy.LegacyApi.Say(command);
                }
            }
        }
    }
}
