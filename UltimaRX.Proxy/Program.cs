using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UltimaRX.IO;
using UltimaRX.Packets;
using UltimaRX.Packets.Client;
using UltimaRX.Packets.Server;
using UltimaRX.Proxy.Logging;

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
        private static byte currentSequenceNumber;
        private static int moveRequestsFromProxy;
        private static Direction lastWalkDirection;

        private static readonly RingBufferLogger PacketRingBufferLogger = new RingBufferLogger(new ConsoleLogger(), 1000);

        public static NetworkStream ClientStream { get; set; }

        public static NetworkStream ServerStream { get; set; }

        public static void Say(string message)
        {
            var packet = new SpeechRequest
            {
                Type = SpeechType.Normal,
                Text = message,
                Font = 0x02b2,
                Color = 0x0003,
                Language = "ENU"
            };

            ClientConnectionOnPacketReceived(null, packet.RawPacket);
        }

        public static void Test()
        {
            Say("test");
        }

        public static void Walk(Direction direction)
        {
            var packet = new MoveRequest
            {
                Direction = direction,
                SequenceNumber = ++currentSequenceNumber
            };

            lastWalkDirection = direction;
            moveRequestsFromProxy++;

            ClientConnectionOnPacketReceived(null, packet.RawPacket);
        }

        public static void MovePlayer(Direction direction)
        {
            var packet = new MovePlayer
            {
                Direction = direction
            };

            ServerConnectionOnPacketReceived(null, packet.RawPacket);
        }

        public static void SetWeather(WeatherType type, byte numberOfEffects)
        {
            var packet = new SetWeather
            {
                Type = type,
                NumberOfEffects = numberOfEffects,
                Temperature = 25
            };

            ServerConnectionOnPacketReceived(null, packet.RawPacket);
        }

        public static void SetOverallLightLevel(byte level)
        {
            var packet = new OverallLightLevel
            {
                Level = level
            };

            ServerConnectionOnPacketReceived(null, packet.RawPacket);
        }

        public static void Main()
        {
            Main(33333, new ConsoleLogger());
        }

        public static Task Start(IPEndPoint serverAddress, int localProxyPort = 33333)
        {
            serverEndpoint = serverAddress;
            return Task.Run(() => Main(localProxyPort, PacketRingBufferLogger));
        }

        private static void Main(int port, ILogger logger)
        {
            listener = new TcpListener(new IPEndPoint(IPAddress.Any, port));
            listener.Start();

            ClientLoop(logger);
        }

        public static void DumpCommunication()
        {
            PacketRingBufferLogger.Dump();
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

        private static void ServerConnectionOnPacketReceived(object sender, Packet packet)
        {
            lock (ServerConnectionLock)
            {
                using (var memoryStream = new MemoryStream(1024))
                {
                    if (packet.Id == PacketDefinitions.ConnectToGameServer.Id)
                    {
                        var materializedPacket = PacketDefinitionRegistry.Materialize<ConnectToGameServer>(packet);
                        materializedPacket.GameServerIp = new byte[] {0x7F, 0x00, 0x00, 0x01};
                        materializedPacket.GameServerPort = 33333;
                        packet = materializedPacket.RawPacket;
                        needServerReconnect = true;
                    }
                    else if (packet.Id == PacketDefinitions.CharacterMoveAck.Id)
                    {
                        if (moveRequestsFromProxy > 0)
                        {
                            var movePlayerPacket = new MovePlayer
                            {
                                Direction = lastWalkDirection
                            };

                            packet = movePlayerPacket.RawPacket;
                            moveRequestsFromProxy--;
                        }
                    }

                    clientConnection.Send(packet, memoryStream);
                    ClientStream.Write(memoryStream.GetBuffer(), 0, (int) memoryStream.Length);
                }
            }
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
                Console.WriteLine(serverDiagnosticPullStream.Flush());
                Console.WriteLine();
                Console.WriteLine(ex);
                Console.WriteLine();
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

        private static IPEndPoint serverEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2593);

        private static NetworkStream ConnectToServer()
        {
            // localhost:
            serverSocket = new Socket(serverEndpoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Connect(serverEndpoint);

            return new NetworkStream(serverSocket);
        }

        private static void ClientConnectionOnPacketReceived(object sender, Packet packet)
        {
            lock (ServerStreamLock)
            {
                using (var memoryStream = new MemoryStream(1024))
                {
                    if (packet.Id == PacketDefinitions.MoveRequest.Id)
                    {
                        var moveRequestPacket = PacketDefinitionRegistry.Materialize<MoveRequest>(packet);
                        currentSequenceNumber = moveRequestPacket.SequenceNumber;
                    }

                    serverConnection.Send(packet, memoryStream);
                    ServerStream.Write(memoryStream.GetBuffer(), 0, (int) memoryStream.Length);
                }
            }
        }
    }
}