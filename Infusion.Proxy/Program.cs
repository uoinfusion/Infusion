using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Infusion.IO;
using Infusion.Packets;
using Infusion.Packets.Both;
using Infusion.Packets.Client;
using Infusion.Packets.Parsers;
using Infusion.Packets.Server;
using Infusion.Proxy.LegacyApi;
using Infusion.Proxy.Logging;

namespace Infusion.Proxy
{
    public static class Program
    {
        private static TcpListener listener;
        private static ServerConnection serverConnection;
        private static UltimaClientConnection clientConnection;
        private static Socket serverSocket;

        private static bool needServerReconnect;
        private static ConsoleDiagnosticPushStream serverDiagnosticPushStream;
        private static ConsoleDiagnosticPullStream serverDiagnosticPullStream;

        private static readonly Configuration configuration = new Configuration();
        private static readonly SpeechFilter speechFilter = new SpeechFilter(configuration);

        private static readonly object serverConnectionLock = new object();

        private static readonly object serverStreamLock = new object();

        public static ILogger Console { get; set; } = new ConsoleLogger();
        public static ILogger Diagnostic = NullLogger.Instance;

        private static readonly RingBufferLogger packetRingBufferLogger = new RingBufferLogger(1000);

        private static IPEndPoint serverEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2593);

        public static readonly ServerPacketHandler ServerPacketHandler = new ServerPacketHandler();
        public static readonly ClientPacketHandler ClientPacketHandler = new ClientPacketHandler();

        public static NetworkStream ClientStream { get; set; }

        public static NetworkStream ServerStream { get; set; }

        public static void Print(string message)
        {
            Console.Info(message);
        }

        public static void Main()
        {
            Legacy.Configuration = configuration;
            Legacy.Initialize();
            Main(33333, new ConsoleLogger());
        }

        public static Task Start(IPEndPoint serverAddress, ushort localProxyPort = 33333)
        {
            Legacy.Configuration = configuration;
            Legacy.Initialize();

            ServerPacketHandler.RegisterFilter(RedirectConnectToGameServer);
            ServerPacketHandler.Subscribe(PacketDefinitions.SendSpeech, HandleSendSpeechPacket);
            ServerPacketHandler.Subscribe(PacketDefinitions.SpeechMessage, HandleSpeechMessagePacket);

            serverEndpoint = serverAddress;
            return Main(localProxyPort, packetRingBufferLogger);
        }

        private static void HandleSendSpeechPacket(SendSpeechPacket packet)
        {
            var message = new SpeechMessage()
            {
                Type = packet.Type,
                Message = packet.Message,
                Name = packet.Name,
                SpeakerId = packet.Id,
            };

            if (speechFilter.IsPassing(message.Text))
                Console.Important(message.Text);
            else
                Console.Info(message.Text);
        }

        private static void HandleSpeechMessagePacket(SpeechMessagePacket packet)
        {
            var message = new SpeechMessage()
            {
                Type = packet.Type,
                Message = packet.Message,
                Name = packet.Name,
                SpeakerId = packet.Id,
            };

            if (speechFilter.IsPassing(message.Text))
                Console.Important(message.Text);
            else
                Console.Info(message.Text);
        }

        private static ushort proxyLocalPort;

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
            => new PacketLogParser().Parse(packetRingBufferLogger.DumpToString());

        public static void ClearPacketLog()
        {
            packetRingBufferLogger.Clear();
        }

        private static void ClientLoop(ILogger packetLogger)
        {
            serverDiagnosticPushStream = new ConsoleDiagnosticPushStream(packetLogger, "proxy -> server");
            serverDiagnosticPullStream = new ConsoleDiagnosticPullStream(packetLogger, "server -> proxy");

            serverConnection = new ServerConnection(ServerConnectionStatus.Initial, serverDiagnosticPullStream,
                serverDiagnosticPushStream);
            serverConnection.PacketReceived += ServerConnectionOnPacketReceived;

            clientConnection = new UltimaClientConnection(UltimaClientConnectionStatus.Initial,
                new ConsoleDiagnosticPullStream(packetLogger, "client -> proxy"),
                new ConsoleDiagnosticPushStream(packetLogger, "proxy -> client"));
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
                var handledPacket = HandleServerPacket(rawPacket);
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

        private static Packet? HandleServerPacket(Packet rawPacket)
        {
            var filteredPacket = ServerPacketHandler.Filter(rawPacket);
            if (!filteredPacket.HasValue)
                return null;
            rawPacket = filteredPacket.Value;

            if (rawPacket.Id == PacketDefinitions.AddMultipleItemsInContainer.Id)
            {
                ServerPacketHandler.Publish<AddMultipleItemsInContainerPacket>(rawPacket);
            }
            else if (rawPacket.Id == PacketDefinitions.AddItemToContainer.Id)
            {
                ServerPacketHandler.Publish<AddItemToContainerPacket>(rawPacket);
            }
            else if (rawPacket.Id == PacketDefinitions.DeleteObject.Id)
            {
                ServerPacketHandler.Publish<DeleteObjectPacket>(rawPacket);
            }
            else if (rawPacket.Id == PacketDefinitions.ObjectInfo.Id)
            {
                ServerPacketHandler.Publish<ObjectInfoPacket>(rawPacket);
            }
            else if (rawPacket.Id == PacketDefinitions.DrawObject.Id)
            {
                ServerPacketHandler.Publish<DrawObjectPacket>(rawPacket);
            }
            else if (rawPacket.Id == PacketDefinitions.TargetCursor.Id)
            {
                ServerPacketHandler.Publish<TargetCursorPacket>(rawPacket);
            }
            else if (rawPacket.Id == PacketDefinitions.CharacterMoveAck.Id)
            {
                ServerPacketHandler.Publish<CharacterMoveAckPacket>(rawPacket);
            }
            else if (rawPacket.Id == PacketDefinitions.CharMoveRejection.Id)
            {
                ServerPacketHandler.Publish<CharMoveRejectionPacket>(rawPacket);
            }
            else if (rawPacket.Id == PacketDefinitions.DrawGamePlayer.Id)
            {
                ServerPacketHandler.Publish<DrawGamePlayerPacket>(rawPacket);
            }
            else if (rawPacket.Id == PacketDefinitions.SpeechMessage.Id)
            {
                ServerPacketHandler.Publish<SpeechMessagePacket>(rawPacket);
            }
            else if (rawPacket.Id == PacketDefinitions.SendSpeech.Id)
            {
                ServerPacketHandler.Publish<SendSpeechPacket>(rawPacket);
            }
            else if (rawPacket.Id == PacketDefinitions.CharacterLocaleAndBody.Id)
            {
                ServerPacketHandler.Publish<CharLocaleAndBodyPacket>(rawPacket);
            }
            else if (rawPacket.Id == PacketDefinitions.UpdatePlayer.Id)
            {
                ServerPacketHandler.Publish<UpdatePlayerPacket>(rawPacket);
            }
            else if (rawPacket.Id == PacketDefinitions.UpdateCurrentHealth.Id)
            {
                ServerPacketHandler.Publish<UpdateCurrentHealthPacket>(rawPacket);
            }
            else if (rawPacket.Id == PacketDefinitions.UpdateCurrentStamina.Id)
            {
                ServerPacketHandler.Publish<UpdateCurrentStaminaPacket>(rawPacket);
            }
            else if (rawPacket.Id == PacketDefinitions.SendGumpMenuDialog.Id)
            {
                ServerPacketHandler.Publish<SendGumpMenuDialogPacket>(rawPacket);
            }
            else if (rawPacket.Id == PacketDefinitions.WornItem.Id)
            {
                ServerPacketHandler.Publish<WornItemPacket>(rawPacket);
            }
            else if (rawPacket.Id == PacketDefinitions.StatusBarInfo.Id)
            {
                ServerPacketHandler.Publish<StatusBarInfoPacket>(rawPacket);
            }
            else if (rawPacket.Id == PacketDefinitions.SendSkills.Id)
            {
                ServerPacketHandler.Publish<SendSkillsPacket>(rawPacket);
            }

            return rawPacket;
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
                    {
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
                var filteredPacket = ClientPacketHandler.Filter(rawPacket);
                if (!filteredPacket.HasValue)
                    return;
                rawPacket = filteredPacket.Value;

                if (rawPacket.Id == PacketDefinitions.MoveRequest.Id)
                {
                    ClientPacketHandler.Publish<MoveRequest>(rawPacket);
                }
                else if (rawPacket.Id == PacketDefinitions.SpeechRequest.Id)
                {
                    ClientPacketHandler.Publish<SpeechRequest>(rawPacket);
                }
                else if (rawPacket.Id == PacketDefinitions.TargetCursor.Id)
                {
                    ClientPacketHandler.Publish<TargetCursorPacket>(rawPacket);
                }
                else if (rawPacket.Id == PacketDefinitions.GumpMenuSelection.Id)
                {
                    ClientPacketHandler.Publish<GumpMenuSelectionRequest>(rawPacket);
                }
                else if (rawPacket.Id == PacketDefinitions.DoubleClick.Id)
                {
                    ClientPacketHandler.Publish<DoubleClickRequest>(rawPacket);
                }

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