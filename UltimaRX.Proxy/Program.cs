using System.Net;
using System.Net.Sockets;
using System.Threading;
using UltimaRX.IO;

namespace UltimaRX.Proxy
{
    internal class Program
    {
        private static TcpListener listener;

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
            ClientStream = client.GetStream();

            var clientConnection = new UltimaClientConnection(UltimaClientConnectionStatus.Initial, new ConsoleDiagnosticStream());

            clientConnection.ReceiveBatch(new NetworkStreamToPullStreamAdapter(ClientStream));
        }
    }
}