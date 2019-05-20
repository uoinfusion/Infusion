using System;
using System.Collections.Generic;
using Infusion.Commands;
using Infusion.Config;
using Infusion.LegacyApi.Cliloc;
using Infusion.LegacyApi.Console;
using Infusion.LegacyApi.Injection;
using Infusion.LegacyApi.Keywords;
using Infusion.Logging;
using Infusion.Packets;
using InjectionScript.Runtime;

namespace Infusion.LegacyApi
{
    public class InfusionTestProxy
    {
        private readonly List<Packet> packetsSentToClient = new List<Packet>();
        private readonly List<Packet> packetsSentToServer = new List<Packet>();

        internal InfusionTestProxy(PacketDefinitionRegistry packetRegistry = null)
            : this(new RealTimeSource(), packetRegistry)
        {
        }

        internal void PacketReceivedFromServer(object compressed) => throw new NotImplementedException();

        internal InfusionTestProxy(ITimeSource timeSource, PacketDefinitionRegistry packetRegistry = null)
        {
            packetRegistry = packetRegistry ?? PacketDefinitionRegistryFactory.CreateClassicClient();
            ServerPacketHandler = new ServerPacketHandler(PacketDefinitionRegistryFactory.CreateClassicClient());
            ClientPacketHandler = new ClientPacketHandler(PacketDefinitionRegistryFactory.CreateClassicClient());
            Server = new UltimaServer(ServerPacketHandler, packet =>
            {
                var filteredPacket = ClientPacketHandler.FilterOutput(packet);
                if (filteredPacket.HasValue)
                    packetsSentToServer.Add(filteredPacket.Value);
            }, packetRegistry);
            Client = new UltimaClient(ClientPacketHandler, packet =>
            {
                var filteredPacket = ServerPacketHandler.FilterOutput(packet);
                if (filteredPacket.HasValue)
                    packetsSentToClient.Add(filteredPacket.Value);
            });

            var console = new NullConsole();
            Api = new Legacy(new LogConfiguration(), new CommandHandler(console), Server, Client, console, packetRegistry,
                timeSource, ClilocSource, KeywordSource, new MemoryConfigBagRepository(), new NullInjectionWindow(), new NullSoundPlayer());
            UO.Initialize(Api);
            ServerApi = new TestServerApi(PacketReceivedFromServer, Api);
        }

        public IEnumerable<Packet> PacketsSentToClient => packetsSentToClient;
        public IEnumerable<Packet> PacketsSentToServer => packetsSentToServer;

        public Packet? PacketReceivedFromServer(Packet packet) => ServerPacketHandler.HandlePacket(packet);
        public Packet? PacketReceivedFromServer(byte[] payload) => ServerPacketHandler.HandlePacket(new Packet(payload[0], payload));
        public Packet? PacketReceivedFromClient(Packet packet) => ClientPacketHandler.HandlePacket(packet);
        public Packet? PacketReceivedFromClient(byte[] payload) => ClientPacketHandler.HandlePacket(new Packet(payload[0], payload));

        public Legacy Api { get; }

        public UltimaClient Client { get; }

        internal UltimaServer Server { get; }

        internal ServerPacketHandler ServerPacketHandler { get; }
        internal ClientPacketHandler ClientPacketHandler { get; }
        public TestServerApi ServerApi { get; }
        internal MemoryClilocSource ClilocSource { get; } = new MemoryClilocSource();
        internal MemoryKeywordSource KeywordSource { get; } = new MemoryKeywordSource();
    }
}