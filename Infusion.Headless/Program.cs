using Infusion.Commands;
using Infusion.EngineScripts;
using Infusion.IO.Encryption.Login;
using Infusion.LegacyApi;
using Infusion.LegacyApi.Console;
using Infusion.Logging;
using Infusion.Proxy;
using System;
using System.Net;

namespace Infusion.Headless
{
    class Program
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
            var proxy = new InfusionProxy();
            proxy.Initialize(commandHandler, new NullSoundPlayer());

            proxy.Start(new ProxyStartConfig()
            {
                ServerAddress = "127.0.0.1,2593",
                ServerEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2593),
                LocalProxyPort = 60000,
                ProtocolVersion = new Version("3.0.0"),
                Encryption = EncryptionSetup.EncryptedServer,
                LoginEncryptionKey = LoginEncryptionKey.FromVersion(new Version(3, 0, 6)),
            });

            var headlessClient = new HeadlessClient(Console, new HeadlessStartConfig()
            {
                Encryption = EncryptionSetup.Autodetect,
                ProtocolVersion = new Version("3.0.0"),
                ServerAddress = "127.0.0.1,60000",
                ServerEndPoint = new IPEndPoint(new IPAddress(new byte[] { 127, 0, 0, 1 }), 60000),
                ShardName = "Erebor",
                AccountName = "diblik",
                Password = "password",
            });

            CSharpScriptEngine = new CSharpScriptEngine(Console);
            ScriptEngine = new ScriptEngine(CSharpScriptEngine, new InjectionScriptEngine(UO.Injection, Console));

            headlessClient.Connect();
            proxy.DumpPacketLog();

            System.Console.ReadLine();
        }
    }
}
