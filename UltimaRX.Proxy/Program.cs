using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UltimaRX.IO;
using UltimaRX.Packets;

namespace UltimaRX.Proxy
{
    internal class Program
    {
        private static TcpListener listener;
        private static ServerConnection serverConnection;

        public static NetworkStream ClientStream { get; set; }

        private static void Main(string[] args)
        {
            listener = new TcpListener(new IPEndPoint(IPAddress.Any, 33333));
            listener.Start();

            while (true)
            {
                ListenerLoop();
                Thread.Yield();
            }
        }

        private static void ListenerLoop()
        {
            var client = listener.AcceptTcpClient();

            serverConnection = new ServerConnection(ServerConnectionStatus.Initial, new ConsoleDiagnosticPullStream("Receive ServerConnection"), new ConsoleDiagnosticPushStream("Send ServerConnection"));
            var serverEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2593);
            var serverSocket = new Socket(serverEndpoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Connect(serverEndpoint);
            SocketStream = new NetworkStream(serverSocket);

            ClientStream = client.GetStream();

            var clientConnection = new UltimaClientConnection(UltimaClientConnectionStatus.Initial,
                new ConsoleDiagnosticPullStream("ClientConnection"));
            clientConnection.PacketReceived += ClientConnectionOnPacketReceived;

            clientConnection.ReceiveBatch(new NetworkStreamToPullStreamAdapter(ClientStream));
        }

        public static NetworkStream SocketStream { get; set; }

        private static void ClientConnectionOnPacketReceived(object sender, Packet packet)
        {
            using (var memoryStream = new MemoryStream(1024))
            {
                serverConnection.Send(packet, memoryStream);
                SocketStream.Write(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
            }
        }
    }
}