using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Infusion.Commands;
using Infusion.Config;
using Infusion.Diagnostic;
using Infusion.Injection.Avalonia;
using Infusion.IO;
using Infusion.IO.Encryption.Login;
using Infusion.LegacyApi;
using Infusion.LegacyApi.Console;
using Infusion.Logging;
using Infusion.Packets;
using Infusion.Packets.Client;
using Infusion.Packets.Server;

namespace Infusion.Proxy
{
    public static class Program
    {
        private const string ListCommandName = "list";

        public static LogConfiguration LogConfig { get; } = new LogConfiguration();

        private static ServerPacketHandler serverPacketHandler;
        private static ClientPacketHandler clientPacketHandler;
        private static TcpListener listener;
        private static ServerConnection serverConnection;
        private static UltimaClientConnection clientConnection;
        private static Socket serverSocket;

        private static IDiagnosticPushStream serverDiagnosticPushStream;
        private static ConsoleDiagnosticPullStream serverDiagnosticPullStream;

        private static readonly object serverConnectionLock = new object();

        private static readonly object serverStreamLock = new object();
        public static ILogger Diagnostic = NullLogger.Instance;

        private static readonly RingBufferLogger packetRingBufferLogger = new RingBufferLogger(10000);

        private static IPEndPoint serverEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2593);

        private static ushort proxyLocalPort;
        private static CommandHandler commandHandler;

        internal static IConsole Console { get; set; } = new TextConsole();

        internal static IConfigBagRepository ConfigRepository { get; set; } = new MemoryConfigBagRepository();
        
        public static NetworkStream ClientStream { get; set; }

        public static NetworkStream ServerStream { get; set; }

        public static void Print(string message)
        {
            Console.WriteLine(ConsoleLineType.Information, message);
        }

        public static void Main()
        {
        }

        private static void HelpCommand(string parameters)
        {
            Console.WriteLine(ConsoleLineType.Information, commandHandler.Help(parameters));
        }

        private static void ListRunningCommands()
        {
            foreach (var command in commandHandler.RunningCommands)
            {
                if (command.Name == ListCommandName)
                    continue;

                Console.WriteLine(ConsoleLineType.Information, command.ExecutionMode == CommandExecutionMode.Background
                    ? $"{command.Name} (background)"
                    : command.Name);
            }
        }

        public static Legacy LegacyApi { get; private set; }
        public static ISoundPlayer SoundPlayer { get; set; }
        private static PacketDefinitionRegistry packetRegistry;
        private static ProxyStartConfig proxyStartConfig;

        public static void Initialize(CommandHandler commandHandler, ISoundPlayer soundPlayer)
        {
            Program.commandHandler = commandHandler;
            SoundPlayer = soundPlayer;
        }

        private static void PrintProxyLatency()
        {
            Console.Info($"Client latency:\n{clientProxyLatencyMeter}");
            Console.Info($"Server latency:\n{serverProxyLatencyMeter}");
        }

        public static Task Start(ProxyStartConfig config)
        {
            proxyStartConfig = config;

            Console.Info($"Default protocol version: {config.ProtocolVersion}");
            Console.Info($"Encryption: {config.Encryption}");
            if (config.LoginEncryptionKey.HasValue)
                Console.Info($"Encryption key {config.LoginEncryptionKey.Value}");
            packetRegistry = PacketDefinitionRegistryFactory.CreateClassicClient(proxyStartConfig.ProtocolVersion);

            serverPacketHandler = new ServerPacketHandler(packetRegistry);
            clientPacketHandler = new ClientPacketHandler(packetRegistry);
            serverPacketHandler.RegisterFilter(RedirectConnectToGameServer);
            clientPacketHandler.Subscribe(PacketDefinitions.ExtendedLoginSeed, HandleExtendedLoginSeed);

            LegacyApi = new Legacy(LogConfig, commandHandler, new UltimaServer(serverPacketHandler, SendToServer, packetRegistry), new UltimaClient(clientPacketHandler, SendToClient), Console,
                packetRegistry, ConfigRepository, new InjectionWindowHandler(), SoundPlayer);
            UO.Initialize(LegacyApi);

            commandHandler.RegisterCommand(new Command("dump", DumpPacketLog, false, true,
                "Dumps packet log - log of network communication between game client and server. Network communication logs are very useful for diagnosing issues like crashes.",
                executionMode: CommandExecutionMode.Direct));
            commandHandler.RegisterCommand(new Command("help", HelpCommand, false, true, "Shows command help."));

            commandHandler.RegisterCommand(new Command(ListCommandName, ListRunningCommands, false, true,
                "Lists running commands"));

            commandHandler.RegisterCommand(new Command("proxy-latency", PrintProxyLatency, false, true, "Shows proxy latency."));

            serverEndpoint = config.ServerEndPoint;
            return Main(config.LocalProxyPort, packetRingBufferLogger);
        }

        private static void HandleExtendedLoginSeed(ExtendedLoginSeed extendedLoginSeed)
        {
            Console.Info($"Detected client version: {extendedLoginSeed.ClientVersion}");
            var detectedProtocolVersion = PacketDefinitionRegistryFactory.GetProtocolVersion(extendedLoginSeed.ClientVersion);
            if (detectedProtocolVersion != proxyStartConfig.ProtocolVersion)
            {
                Console.Info($"Using detected protocol version {detectedProtocolVersion} instead configured version {proxyStartConfig.ProtocolVersion}.");
                PacketDefinitionRegistryFactory.CreateClassicClient(packetRegistry, detectedProtocolVersion);
            }
        }

        private static Task Main(ushort port, ILogger logger)
        {
            proxyLocalPort = port;
            listener = new TcpListener(new IPEndPoint(IPAddress.Any, proxyLocalPort));
            listener.Start();
            Console.Info($"Listening on port {proxyLocalPort}");

            return Task.Run(() => ClientLoop(logger));
        }

        public static void DumpPacketLog()
        {
            packetRingBufferLogger.Dump(Console);
        }

        public static IEnumerable<PacketLogEntry> ParsePacketLogDump()
        {
            return new PacketLogParser().Parse(packetRingBufferLogger.DumpToString());
        }

        public static void ClearPacketLog()
        {
            packetRingBufferLogger.Clear();
        }

        private static void ClientLoop(ILogger packetLogger)
        {
            var diagnosticProvider = new InfusionDiagnosticPushStreamProvider(LogConfig, Console);
            serverDiagnosticPushStream =
                new CompositeDiagnosticPushStream(new ConsoleDiagnosticPushStream(packetLogger, "proxy -> server", packetRegistry),
                    new InfusionBinaryDiagnosticPushStream(DiagnosticStreamDirection.ClientToServer, diagnosticProvider.GetStream));
            serverDiagnosticPullStream = new ConsoleDiagnosticPullStream(packetLogger, "server -> proxy", packetRegistry);

            serverConnection = new ServerConnection(ServerConnectionStatus.Initial, serverDiagnosticPullStream,
                serverDiagnosticPushStream, packetRegistry);
            serverConnection.PacketReceived += ServerConnectionOnPacketReceived;

            clientConnection = new UltimaClientConnection(UltimaClientConnectionStatus.Initial,
                new ConsoleDiagnosticPullStream(packetLogger, "client -> proxy", packetRegistry),
                new CompositeDiagnosticPushStream(new ConsoleDiagnosticPushStream(packetLogger, "proxy -> client", packetRegistry),
                    new InfusionBinaryDiagnosticPushStream(DiagnosticStreamDirection.ServerToClient, diagnosticProvider.GetStream)), packetRegistry,
                    proxyStartConfig.Encryption, proxyStartConfig.LoginEncryptionKey);
            clientConnection.PacketReceived += ClientConnectionOnPacketReceived;
            clientConnection.NewGameEncryptionStarted += ClientConnectionOnNewGameEncryptionStarted;
            clientConnection.LoginEncryptionStarted += ClientConnectionOnLoginEncryptionStarted;

            diagnosticProvider.ClientConnection = clientConnection;
            diagnosticProvider.ServerConnection = serverConnection;
            bool serverLoopStarted = false;

            lock (serverStreamLock)
            {
                ServerStream = ConnectToServer();
            }

            try
            {
                while (true)
                {
                    var client = listener.AcceptTcpClient();
                    ClientStream = client.GetStream();

                    if (!serverLoopStarted)
                    {
                        Task.Run(() => ServerLoop());
                        serverLoopStarted = true;
                    }

                    int receivedLength;
                    var receiveBuffer = new byte[65535];

                    while ((receivedLength = ClientStream.Read(receiveBuffer, 0, receiveBuffer.Length)) > 0)
                    {
#if DUMP_RAW
                        Console.Info("client -> proxy");
                        Console.Info(receiveBuffer.Take(receivedLength).Select(x => x.ToString("X2")).Aggregate((l, r) => l + " " + r));
#endif

                        var memoryStream = new MemoryStream(receiveBuffer, 0, receivedLength, false);
                        clientConnection.ReceiveBatch(new MemoryStreamToPullStreamAdapter(memoryStream), receivedLength);
                    }

                    lock (serverStreamLock)
                    {
                        DisconnectFromServer();
                        ServerStream = ConnectToServer();
                    }

                    Thread.Yield();
                }
            }
            catch (IOException ioex) when (ioex.InnerException is SocketException socex && socex.SocketErrorCode == SocketError.ConnectionReset)
            {
                Console.Error("Connection to client lost. Please restart infusion.");
                Console.Debug(socex.ToString());
            }
            catch (Exception ex)
            {
                Console.Error("Connection to client lost. Please restart infusion.");
                Console.Debug(serverDiagnosticPullStream.Flush());
                Console.Debug(ex.ToString());
                throw;
            }
            finally
            {
                diagnosticProvider.Dispose();
            }
        }

        public static void TurnOnDiagnostic()
        {
            Diagnostic = new DiagnosticLogger(Console);
        }

        public static void TurnOffDiagnostic()
        {
            Diagnostic = NullLogger.Instance;
        }

        public static void SendToClient(Packet rawPacket)
        {
            if (clientConnection == null || ClientStream == null)
                return;

            var filteredPacket = serverPacketHandler.FilterOutput(rawPacket);

            if (filteredPacket.HasValue)
            {
                lock (serverConnectionLock)
                {
                    using (var memoryStream = new MemoryStream(1024))
                    {
                        clientConnection.Send(filteredPacket.Value, memoryStream);
                        var buffer = memoryStream.GetBuffer();

#if DUMP_RAW
                        Console.Info("proxy -> client");
                        Console.Info(buffer.Take((int)memoryStream.Length).Select(x => x.ToString("X2")).Aggregate((l, r) => l + " " + r));
#endif

                        ClientStream.Write(buffer, 0, (int) memoryStream.Length);
                    }
                }
            }
        }

        private static Packet? RedirectConnectToGameServer(Packet rawPacket)
        {
            if (rawPacket.Id == PacketDefinitions.ConnectToGameServer.Id)
            {
                var packet = packetRegistry.Materialize<ConnectToGameServerPacket>(rawPacket);
                serverEndpoint = new IPEndPoint(new IPAddress(packet.GameServerIp), packet.GameServerPort);
                
                packet.GameServerIp = new byte[] {0x7F, 0x00, 0x00, 0x01};
                packet.GameServerPort = proxyLocalPort;
                rawPacket = packet.RawPacket;
            }

            return rawPacket;
        }

        private static void ServerConnectionOnPacketReceived(object sender, Packet rawPacket)
        {
            Packet? handledPacket = null;

            serverProxyLatencyMeter.Measure(rawPacket, () =>
            {
                try
                {
                    handledPacket = serverPacketHandler.HandlePacket(rawPacket);
                }
                catch (PacketMaterializationException ex)
                {
                    // just log exception and continue, do not interrupt proxy
                    Console.Error(ex.ToString());
                    DumpPacketLog();
                }
                catch (Exception ex)
                {
                    // just log exception and continue, do not interrupt proxy
                    Console.Error(ex.ToString());
                }
            });

            if (handledPacket != null)
                SendToClient(handledPacket.Value);
        }

        private static void ServerLoop()
        {
            try
            {
                Console.Info($"Connecting to {proxyStartConfig.ServerAddress}");

                while (true)
                {
                    lock (serverStreamLock)
                    {
                        if (ServerStream == null)
                            ServerStream = ConnectToServer();
                    }

                    if (ServerStream.DataAvailable)
                    {
                        try
                        {
                            serverConnection.Receive(new NetworkStreamToPullStreamAdapter(ServerStream));
                        }
                        catch (EndOfStreamException ex)
                        {
                            Console.Debug(ex.ToString());
                            // just swallow this exception, wait for the next batch
                        }
                    }
                    Thread.Sleep(1);
                }
            }
            catch (Exception ex)
            {
                Console.Error("Disconnected from server. Please, restart Infusion.");
                Console.Debug(serverDiagnosticPullStream.Flush());
                Console.Debug(ex.ToString());
                throw;
            }
        }

        private static void DisconnectFromServer()
        {
            ServerStream.Dispose();
            ServerStream = null;
            serverSocket.Dispose();
            serverSocket = null;
        }

        public static event EventHandler ConnectedToServer;

        private static NetworkStream ConnectToServer()
        {
            // localhost:
            serverSocket = new Socket(serverEndpoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Connect(serverEndpoint);
            ConnectedToServer?.Invoke(null, EventArgs.Empty);

            return new NetworkStream(serverSocket);
        }

        public static void SendToServer(Packet rawPacket)
        {
            if (serverConnection == null)
                return;

            var filteredPacket = clientPacketHandler.FilterOutput(rawPacket);
            if (filteredPacket.HasValue)
            {
                lock (serverStreamLock)
                {
                    using (var memoryStream = new MemoryStream(1024))
                    {
                        serverConnection.Send(filteredPacket.Value, memoryStream);
                        var buffer = memoryStream.GetBuffer();

#if DUMP_RAW
                        Console.Info("proxy -> server");
                        Console.Info(buffer.Take((int)memoryStream.Length).Select(x => x.ToString("X2")).Aggregate((l, r) => l + " " + r));
#endif

                        ServerStream.Write(buffer, 0, (int)memoryStream.Length);
                    }
                }
            }
        }

        private static readonly LatencyMeter clientProxyLatencyMeter = new LatencyMeter();
        private static readonly LatencyMeter serverProxyLatencyMeter = new LatencyMeter();

        private static void ClientConnectionOnNewGameEncryptionStarted(byte[] key)
        {
            serverConnection.InitNewGameEncryption(key);
        }

        private static void ClientConnectionOnLoginEncryptionStarted(uint seed, LoginEncryptionKey key)
        {
            serverConnection.InitLoginEncryption(seed, key);
        }

        private static void ClientConnectionOnPacketReceived(object sender, Packet rawPacket)
        {
            Packet? handledPacket = null;

            clientProxyLatencyMeter.Measure(rawPacket, () =>
            {
                try
                {
                    handledPacket = clientPacketHandler.HandlePacket(rawPacket);
                }
                catch (PacketMaterializationException ex)
                {
                    Console.Error(ex.ToString());
                    DumpPacketLog();
                }
                catch (Exception ex)
                {
                    Print(ex.ToString());
                }
            });

            if (handledPacket != null)
                SendToServer(handledPacket.Value);
        }

        public static void SetClientWindowHandle(Process ultimaClientProcess)
        {
            LegacyApi.ClientWindow = new LocalUltimaClientWindow(ultimaClientProcess);
        }
    }
}