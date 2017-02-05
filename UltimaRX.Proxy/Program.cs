using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UltimaRX.IO;
using UltimaRX.Packets;
using UltimaRX.Packets.Both;
using UltimaRX.Packets.Client;
using UltimaRX.Packets.Parsers;
using UltimaRX.Packets.Server;
using UltimaRX.Proxy.InjectionApi;
using UltimaRX.Proxy.Logging;

// TODO: lumberjacking skill increase
//12/3/2016 10:03:18 PM >>>> server -> proxy: RawPacket SendSkills, length = 11
//0x3A, 0x00, 0x0B, 0xFF, 0x00, 0x2C, 0x00, 0x0A, 0x00, 0x0A, 0x00, 

namespace UltimaRX.Proxy
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

        private static readonly object ServerConnectionLock = new object();

        private static readonly object ServerStreamLock = new object();

        public static ILogger Console { get; set; } = new ConsoleLogger();
        public static ILogger Diagnostic = NullLogger.Instance;

        private static readonly RingBufferLogger PacketRingBufferLogger = new RingBufferLogger(1000);

        private static IPEndPoint serverEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2593);

        public static readonly ServerPacketHandler ServerPacketHandler = new ServerPacketHandler();
        public static readonly ClientPacketHandler ClientPacketHandler = new ClientPacketHandler();

        public static NetworkStream ClientStream { get; set; }

        public static NetworkStream ServerStream { get; set; }

        public static void Print(string message)
        {
            Console.WriteLine(message);
        }

        public static void Main()
        {
            Injection.Initialize();
            Main(33333, new ConsoleLogger());
        }

        public static Task Start(IPEndPoint serverAddress, int localProxyPort = 33333)
        {
            Injection.Initialize();

            ServerPacketHandler.RegisterFilter(RedirectConnectToGameServer);
            ServerPacketHandler.Subscribe(PacketDefinitions.SendSpeech, HandleSendSpeechPacket);
            ServerPacketHandler.Subscribe(PacketDefinitions.SpeechMessage, HandleSpeechMessagePacket);

            serverEndpoint = serverAddress;
            return Task.Run(() => Main(localProxyPort, PacketRingBufferLogger));
        }

        private static void HandleSendSpeechPacket(SendSpeechPacket packet)
        {
            var message = (packet.Name ?? string.Empty) + ": " + packet.Message;
            Console.WriteLine(message);
        }

        private static void HandleSpeechMessagePacket(SpeechMessagePacket packet)
        {
            var message = (packet.Name ?? string.Empty) + ": " + packet.Message;
            Console.WriteLine(message);
        }

        private static void Main(int port, ILogger logger)
        {
            listener = new TcpListener(new IPEndPoint(IPAddress.Any, port));
            listener.Start();

            ClientLoop(logger);
        }

        public static void DumpPacketLog()
        {
            PacketRingBufferLogger.Dump(Console);
        }

        public static IEnumerable<PacketLogEntry> ParsePacketLogDump()
            => new PacketLogParser().Parse(PacketRingBufferLogger.DumpToString());

        public static void ClearPacketLog()
        {
            PacketRingBufferLogger.Clear();
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
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(serverDiagnosticPullStream.Flush());
                System.Console.WriteLine();
                System.Console.WriteLine(ex);
                System.Console.WriteLine();

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
            lock (ServerConnectionLock)
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
                packet.GameServerPort = 33334;
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                // just log exception and continue, do not interrupt proxy
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
                        lock (ServerStreamLock)
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
                        serverConnection.Receive(new NetworkStreamToPullStreamAdapter(ServerStream));
                    }
                    Thread.Yield();
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(serverDiagnosticPullStream.Flush());
                System.Console.WriteLine();
                System.Console.WriteLine(ex);
                System.Console.WriteLine();
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

        private static NetworkStream ConnectToServer()
        {
            // localhost:
            serverSocket = new Socket(serverEndpoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Connect(serverEndpoint);

            return new NetworkStream(serverSocket);
        }

        public static void SendToServer(Packet rawPacket)
        {
            lock (ServerStreamLock)
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
            catch (Exception ex)
            {
                Print(ex.ToString());
                throw;
            }

            SendToServer(rawPacket);
        }
    }
}