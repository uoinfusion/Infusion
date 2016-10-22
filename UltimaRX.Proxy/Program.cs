using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UltimaRX.IO;
using UltimaRX.Packets;

namespace UltimaRX.Proxy
{
    internal class Program
    {
        private static TcpListener listener;
        private static ServerConnection serverConnection;

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
            var serverEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2593);
            var serverSocket = new Socket(serverEndpoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Connect(serverEndpoint);
            ServerStream = new NetworkStream(serverSocket);
            Task.Run(() => ServerLoop());

            ClientStream = client.GetStream();

            var clientConnection = new UltimaClientConnection(UltimaClientConnectionStatus.Initial,
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

        private static void ServerLoop()
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