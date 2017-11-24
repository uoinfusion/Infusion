using System.Threading;

namespace Infusion.LegacyApi.Tests
{
    internal class InfusionTestProxy
    {
        public InfusionTestProxy()
        {
            ServerPacketHandler = new ServerPacketHandler();
            ClientPacketHandler = new ClientPacketHandler();
            Server = new UltimaServer(ServerPacketHandler, packet => { });
            Client = new UltimaClient(ClientPacketHandler, packet => { });
            EventSource = new EventJournalSource();
            CancellationTokenSource = new CancellationTokenSource();
            Cancellation = new Cancellation(() => CancellationTokenSource.Token);
        }

        public UltimaClient Client { get; }

        public UltimaServer Server { get; }

        internal ServerPacketHandler ServerPacketHandler { get; }
        internal ClientPacketHandler ClientPacketHandler { get; }
        internal EventJournalSource EventSource { get; }
        public CancellationTokenSource CancellationTokenSource { get; }
        public Cancellation Cancellation { get; }
    }
}