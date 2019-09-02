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

namespace Infusion.Headless
{
    public class HeadlessInfusion
    {
        private readonly IConsole console = new TextConsole();
        private readonly CommandHandler commandHandler;
        private readonly string scriptFileName;
        private readonly InfusionProxy infusionProxy;

        private readonly string serverAddress;
        private readonly int serverPort;
        private readonly int proxyPort;
        private readonly Version protocolVersion;
        private readonly Version clientVersion;

        private readonly string accountName;
        private readonly string accountPassword;
        private readonly string shardName;
        private readonly string characterName;

        private readonly string pipeName;

        public HeadlessInfusion(HeadlessOptions options)
        {
            commandHandler = new CommandHandler(console);

            infusionProxy = new InfusionProxy();

            pipeName = options.PipeName;
            scriptFileName = options.ScriptFileName;

            serverAddress = options.ServerAddress;
            serverPort = options.ServerPort;
            proxyPort = options.ProxyPort;
            protocolVersion = options.ProtocolVersion;
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
                ProtocolVersion = protocolVersion,
                Encryption = EncryptionSetup.EncryptedServer,
                LoginEncryptionKey = LoginEncryptionKey.FromVersion(clientVersion),
            });

            var csharpScriptEngine = new CSharpScriptEngine(console);
            var scriptEngine = new ScriptEngine(csharpScriptEngine, new InjectionScriptEngine(UO.Injection, console));

            var headlessClient = new HeadlessClient(console, new HeadlessStartConfig()
            {
                Encryption = EncryptionSetup.Autodetect,
                ProtocolVersion = protocolVersion,
                ServerAddress = $"127.0.0.1,{proxyPort}",
                ServerEndPoint = new IPEndPoint(new IPAddress(new byte[] { 127, 0, 0, 1 }), proxyPort),
                ShardName = shardName,
                AccountName = accountName,
                Password = accountPassword,
                CharacterName = characterName
            });

            headlessClient.Connect();

            _ = scriptEngine.ExecuteScript(PathUtilities.GetAbsolutePath(scriptFileName), new CancellationTokenSource());
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
