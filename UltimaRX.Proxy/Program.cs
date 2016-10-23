using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UltimaRX.IO;
using UltimaRX.Packets;
using UltimaRX.Packets.PacketDefinitions;
using UltimaRX.Packets.PacketDefinitions.Server;

namespace UltimaRX.Proxy
{
    internal class Program
    {
        private static TcpListener listener;
        private static ServerConnection serverConnection;
        private static UltimaClientConnection clientConnection;
        private static Socket serverSocket;

        private static bool needServerReconnect;
        private static ConsoleDiagnosticPushStream serverDiagnosticPushStream;
        private static ConsoleDiagnosticPullStream serverDiagnosticPullStream;

        public static NetworkStream ClientStream { get; set; }

        public static NetworkStream ServerStream { get; set; }

        private static void Main(string[] args)
        {
            listener = new TcpListener(new IPEndPoint(IPAddress.Any, 33333));
            listener.Start();

            ClientLoop();
        }

        private static void ClientLoop()
        {
            serverDiagnosticPushStream = new ConsoleDiagnosticPushStream("proxy -> server");
            serverDiagnosticPullStream = new ConsoleDiagnosticPullStream("server -> proxy");

            serverConnection = new ServerConnection(ServerConnectionStatus.Initial, serverDiagnosticPullStream,
                serverDiagnosticPushStream);
            serverConnection.PacketReceived += ServerConnectionOnPacketReceived;

            clientConnection = new UltimaClientConnection(UltimaClientConnectionStatus.Initial,
                new ConsoleDiagnosticPullStream("client -> proxy"), new ConsoleDiagnosticPushStream("proxy -> client"));
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
            using (var memoryStream = new MemoryStream(1024))
            {
                if (packet.Id == ConnectToGameServerDefinition.Id)
                {
                    var materializedPacket = PacketDefinitionRegistry.Materialize<ConnectToGameServer>(packet);
                    materializedPacket.GameServerIp = new byte[] {0x7F, 0x00, 0x00, 0x01};
                    materializedPacket.GameServerPort = 33333;
                    packet = materializedPacket.RawPacket;
                    needServerReconnect = true;
                }

                clientConnection.Send(packet, memoryStream);
                ClientStream.Write(memoryStream.GetBuffer(), 0, (int) memoryStream.Length);
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
                        DisconnectFromServer();
                        needServerReconnect = false;
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

        private static NetworkStream ConnectToServer()
        {
            var serverEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2593);
            serverSocket = new Socket(serverEndpoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Connect(serverEndpoint);

            return new NetworkStream(serverSocket);
        }

        private static void ClientConnectionOnPacketReceived(object sender, Packet packet)
        {
            using (var memoryStream = new MemoryStream(1024))
            {
                serverConnection.Send(packet, memoryStream);
                ServerStream.Write(memoryStream.GetBuffer(), 0, (int) memoryStream.Length);
            }
        }
    }
}