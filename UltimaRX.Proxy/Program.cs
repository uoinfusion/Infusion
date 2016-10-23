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
            var client = listener.AcceptTcpClient();

            serverConnection = new ServerConnection(ServerConnectionStatus.Initial,
                new ConsoleDiagnosticPullStream("server -> proxy"), new ConsoleDiagnosticPushStream("proxy -> server"));
            serverConnection.PacketReceived += ServerConnectionOnPacketReceived;
            var serverEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2593);
            var serverSocket = new Socket(serverEndpoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Connect(serverEndpoint);
            ServerStream = new NetworkStream(serverSocket);
            Task.Run(() => ServerLoop());

            ClientStream = client.GetStream();

            clientConnection = new UltimaClientConnection(UltimaClientConnectionStatus.Initial,
                new ConsoleDiagnosticPullStream("client -> proxy"));
            clientConnection.PacketReceived += ClientConnectionOnPacketReceived;

            while (true)
            {
                if (ClientStream.DataAvailable)
                {
                    clientConnection.ReceiveBatch(new NetworkStreamToPullStreamAdapter(ClientStream));
                }
                Thread.Yield();
            }
        }

        private static void ServerConnectionOnPacketReceived(object sender, Packet packet)
        {
            using (var memoryStream = new MemoryStream(1024))
            {
                if (packet.Id == ConnectToGameServerDefinition.Id)
                {
                    var materializedPacket = PacketDefinitionRegistry.Materialize<ConnectToGameServer>(packet);
                    materializedPacket.GameServerIp = new byte[] { 0x7F, 0x00, 0x00, 0x01 };
                    materializedPacket.GameServerPort = 33333;
                    packet = materializedPacket.RawPacket;
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
                    if (ServerStream.DataAvailable)
                    {
                        serverConnection.Receive(new NetworkStreamToPullStreamAdapter(ServerStream));
                        Thread.Yield();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine(ex);
                Console.WriteLine();
                throw;
            }
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