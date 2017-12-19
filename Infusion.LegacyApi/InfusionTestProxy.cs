using System.Collections.Generic;
using Infusion.Commands;
using Infusion.Logging;
using Infusion.Packets;

namespace Infusion.LegacyApi
{
    public class InfusionTestProxy
    {
        private readonly List<Packet> packetsSentToClient = new List<Packet>();
        private readonly List<Packet> packetsSentToServer = new List<Packet>();

        public InfusionTestProxy()
        {
            ServerPacketHandler = new ServerPacketHandler();
            ClientPacketHandler = new ClientPacketHandler();
            Server = new UltimaServer(ServerPacketHandler, packet => { packetsSentToServer.Add(packet); });
            Client = new UltimaClient(ClientPacketHandler, packet => { packetsSentToClient.Add(packet); });

            var logger = new NullLogger();
            Api = new Legacy(new Configuration(), new CommandHandler(logger), Server, Client, logger);
        }

        public IEnumerable<Packet> PacketsSentToClient => packetsSentToClient;
        public IEnumerable<Packet> PacketsSentToServer => packetsSentToServer;

        public Packet? PacketReceivedFromServer(Packet packet) => ServerPacketHandler.HandlePacket(packet);
        public Packet? PacketReceivedFromClient(Packet packet) => ClientPacketHandler.HandlePacket(packet);

        public Legacy Api { get; }

        public UltimaClient Client { get; }

        internal UltimaServer Server { get; }

        internal ServerPacketHandler ServerPacketHandler { get; }
        internal ClientPacketHandler ClientPacketHandler { get; }
    }
}