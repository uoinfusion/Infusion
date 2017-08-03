using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Infusion.Commands;
using Infusion.Diagnostic;
using Infusion.IO;
using Infusion.LegacyApi;
using Infusion.Logging;
using Infusion.Packets;
using Infusion.Packets.Both;
using Infusion.Packets.Client;
using Infusion.Packets.Server;
using Ultima;

namespace Infusion.Proxy
{
    public static class Program
    {
        public static Configuration Configuration { get; } = new Configuration();

        private static readonly ServerPacketHandler serverPacketHandler = new ServerPacketHandler();
        private static readonly ClientPacketHandler clientPacketHandler = new ClientPacketHandler();
        private static TcpListener listener;
        private static ServerConnection serverConnection;
        private static UltimaClientConnection clientConnection;
        private static Socket serverSocket;

        private static bool needServerReconnect;
        private static IDiagnosticPushStream serverDiagnosticPushStream;
        private static ConsoleDiagnosticPullStream serverDiagnosticPullStream;
        private static readonly SpeechFilter speechFilter = new SpeechFilter(Configuration);

        private static readonly object serverConnectionLock = new object();

        private static readonly object serverStreamLock = new object();
        public static ILogger Diagnostic = NullLogger.Instance;

        private static readonly RingBufferLogger packetRingBufferLogger = new RingBufferLogger(100);

        private static IPEndPoint serverEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2593);

        private static readonly StringList clilocDictionary = new StringList("ENU");

        private static ushort proxyLocalPort;
        private static CommandHandler commandHandler;

        internal static ILogger Console { get; set; } = new ConsoleLogger();

        public static NetworkStream ClientStream { get; set; }

        public static NetworkStream ServerStream { get; set; }

        public static void Print(string message)
        {
            Console.Info(message);
        }

        public static void Main()
        {
        }

        private static void HelpCommand(string parameters)
        {
            Console.Info(commandHandler.Help(parameters));
        }

        private static void ListRunningCommands()
        {
            foreach (var command in commandHandler.RunningCommands)
                Console.Info(command.Name);
        }

        public static void Initialize()
        {
            commandHandler = new CommandHandler(Program.Console);
            var commandObservers = new CommandHandlerObservers(clientPacketHandler, commandHandler);
            commandHandler.RegisterCommand(new Command("dump", DumpPacketLog,
                "Dumps packet log - log of network communication between game client and server. Network communication logs are very useful for diagnosing issues like crashes.",
                executionMode: CommandExecutionMode.Direct));
            commandHandler.RegisterCommand(new Command("help", HelpCommand, "Shows command help."));
            commandHandler.RegisterCommand(new Command("list", ListRunningCommands,
                "Lists running commands"));

            var legacyApi = new Legacy(Configuration, commandHandler, new UltimaServer(serverPacketHandler, SendToServer), new UltimaClient(clientPacketHandler, SendToClient), Console);
            UO.Initialize(legacyApi);
        }


        public static Task Start(IPEndPoint serverAddress, ushort localProxyPort = 33333)
        {
            serverPacketHandler.RegisterFilter(RedirectConnectToGameServer);
            serverPacketHandler.Subscribe(PacketDefinitions.SendSpeech, HandleSendSpeechPacket);
            serverPacketHandler.Subscribe(PacketDefinitions.SpeechMessage, HandleSpeechMessagePacket);
            serverPacketHandler.Subscribe(PacketDefinitions.ClilocMessage, HandleClilocMessage);
            serverPacketHandler.Subscribe(PacketDefinitions.ClilocMessageAffix, HandleClilocMessageAffix);

            serverEndpoint = serverAddress;
            return Main(localProxyPort, packetRingBufferLogger);
        }

        private static void HandleClilocMessageAffix(ClilocMessageAffixPacket packet)
        {
            var message = new SpeechMessage
            {
                Type = SpeechType.Speech,
                Message = clilocDictionary.GetString(packet.MessageId.Value) + packet.Affix,
                Name = packet.Name,
                SpeakerId = packet.SpeakerId
            };

            AddConsoleMessage(message);
        }

        private static void HandleClilocMessage(ClilocMessagePacket packet)
        {
            var message = new SpeechMessage
            {
                Type = SpeechType.Speech,
                Message = clilocDictionary.GetString(packet.MessageId.Value),
                Name = packet.Name,
                SpeakerId = packet.SpeakerId
            };

            AddConsoleMessage(message);
        }

        private static void AddConsoleMessage(SpeechMessage message)
        {
            if (speechFilter.IsPassing(message.Text))
                Console.Important(message.Text);
            else
                Console.Info(message.Text);
        }

        private static void HandleSendSpeechPacket(SendSpeechPacket packet)
        {
            var message = new SpeechMessage
            {
                Type = packet.Type,
                Message = packet.Message,
                Name = packet.Name,
                SpeakerId = packet.Id
            };

            AddConsoleMessage(message);
        }

        private static void HandleSpeechMessagePacket(SpeechMessagePacket packet)
        {
            var message = new SpeechMessage
            {
                Type = packet.Type,
                Message = packet.Message,
                Name = packet.Name,
                SpeakerId = packet.Id
            };

            AddConsoleMessage(message);
        }

        private static Task Main(ushort port, ILogger logger)
        {
            proxyLocalPort = port;
            listener = new TcpListener(new IPEndPoint(IPAddress.Any, proxyLocalPort));
            listener.Start();

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
            var diagnosticProvider = new InfusionDiagnosticPushStreamProvider(Configuration);
            serverDiagnosticPushStream =
                new CompositeDiagnosticPushStream(new ConsoleDiagnosticPushStream(packetLogger, "proxy -> server"),
                    new InfusionBinaryDiagnosticPushStream(DiagnosticStreamDirection.ClientToServer, diagnosticProvider.GetStream));
            serverDiagnosticPullStream = new ConsoleDiagnosticPullStream(packetLogger, "server -> proxy");

            serverConnection = new ServerConnection(ServerConnectionStatus.Initial, serverDiagnosticPullStream,
                serverDiagnosticPushStream);
            serverConnection.PacketReceived += ServerConnectionOnPacketReceived;

            clientConnection = new UltimaClientConnection(UltimaClientConnectionStatus.Initial,
                new ConsoleDiagnosticPullStream(packetLogger, "client -> proxy"),
                new CompositeDiagnosticPushStream(new ConsoleDiagnosticPushStream(packetLogger, "proxy -> client"),
                    new InfusionBinaryDiagnosticPushStream(DiagnosticStreamDirection.ServerToClient, diagnosticProvider.GetStream)));
            clientConnection.PacketReceived += ClientConnectionOnPacketReceived;

            Task.Run(() => ServerLoop());

            try
            {
                while (true)
                {
                    var client = listener.AcceptTcpClient();
                    ClientStream = client.GetStream();

                    int receivedLength;
                    var receiveBuffer = new byte[65535];

                    while ((receivedLength = ClientStream.Read(receiveBuffer, 0, receiveBuffer.Length)) > 0)
                    {
                        var memoryStream = new MemoryStream(receiveBuffer, 0, receivedLength, false);

                        clientConnection.ReceiveBatch(new MemoryStreamToPullStreamAdapter(memoryStream));
                    }

                    Thread.Yield();
                }
            }
            catch (Exception ex)
            {
                Console.Error(serverDiagnosticPullStream.Flush());
                Console.Error(ex.ToString());
                DumpPacketLog();
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
            lock (serverConnectionLock)
            {
                using (var memoryStream = new MemoryStream(1024))
                {
                    clientConnection.Send(rawPacket, memoryStream);
                    ClientStream.Write(memoryStream.GetBuffer(), 0, (int) memoryStream.Length);
                }
            }
        }

        private static Packet? RedirectConnectToGameServer(Packet rawPacket)
        {
            if (rawPacket.Id == PacketDefinitions.ConnectToGameServer.Id)
            {
                var packet = PacketDefinitionRegistry.Materialize<ConnectToGameServerPacket>(rawPacket);
                packet.GameServerIp = new byte[] {0x7F, 0x00, 0x00, 0x01};
                packet.GameServerPort = proxyLocalPort;
                rawPacket = packet.RawPacket;
                needServerReconnect = true;
            }

            return rawPacket;
        }

        private static void ServerConnectionOnPacketReceived(object sender, Packet rawPacket)
        {
            try
            {
                var handledPacket = serverPacketHandler.HandlePacket(rawPacket);
                if (!handledPacket.HasValue)
                    return;

                rawPacket = handledPacket.Value;
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

            SendToClient(rawPacket);
        }

        private static void ServerLoop()
        {
            try
            {
                while (true)
                {
                    if (needServerReconnect)
                    {
                        lock (serverStreamLock)
                        {
                            DisconnectFromServer();
                            needServerReconnect = false;
                            ServerStream = ConnectToServer();
                        }
                    }

                    if (ServerStream == null)
                        ServerStream = ConnectToServer();

                    if (ServerStream.DataAvailable)
                    {
                        try
                        {
                            serverConnection.Receive(new NetworkStreamToPullStreamAdapter(ServerStream));
                        }
                        catch (EndOfStreamException ex)
                        {
                            Console.Error(ex.ToString());
                            DumpPacketLog();
                            // just swallow this exception, wait for the next batch
                        }
                    }
                    Thread.Yield();
                }
            }
            catch (Exception ex)
            {
                Console.Error(serverDiagnosticPullStream.Flush());
                Console.Error(ex.ToString());
                DumpPacketLog();
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
            lock (serverStreamLock)
            {
                using (var memoryStream = new MemoryStream(1024))
                {
                    serverConnection.Send(rawPacket, memoryStream);
                    ServerStream.Write(memoryStream.GetBuffer(), 0, (int) memoryStream.Length);
                }
            }
        }

        private static void ClientConnectionOnPacketReceived(object sender, Packet rawPacket)
        {
            try
            {
                var handledPacket = clientPacketHandler.HandlePacket(rawPacket);
                if (!handledPacket.HasValue)
                    return;

                rawPacket = handledPacket.Value;
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

            SendToServer(rawPacket);
        }
    }
}