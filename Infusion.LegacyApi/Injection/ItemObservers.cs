using Infusion.Packets;
using Infusion.Packets.Both;
using Infusion.Packets.Client;
using Infusion.Packets.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.LegacyApi.Injection
{
    internal sealed class ItemObservers
    {
        private readonly PacketDefinitionRegistry packetRegistry;

        public ObjectId LastCorpseId { get; private set; } = 0;
        public ObjectId LastStatusId { get; private set; } = 0;
        public ObjectId LastTargetId { get; private set; } = 0;

        public ItemObservers(IServerPacketSubject serverPacketSubject, IClientPacketSubject clientPacketSubject,
            PacketDefinitionRegistry packetRegistry)
        {
            serverPacketSubject.Subscribe(PacketDefinitions.ObjectInfo, HandleObjectInfoPacket);

            clientPacketSubject.RegisterOutputFilter(FilterSentClientPackets);
            this.packetRegistry = packetRegistry;
        }

        private Packet? FilterSentClientPackets(Packet rawPacket)
        {
            if (rawPacket.Id == PacketDefinitions.TargetCursor.Id)
            {
                var packet = packetRegistry.Materialize<TargetCursorPacket>(rawPacket);
                LastTargetId = packet.ClickedOnId;
            }
            else if (rawPacket.Id == PacketDefinitions.GetClientStatus.Id)
            {
                var packet = packetRegistry.Materialize<GetClientStatusRequest>(rawPacket);
                LastStatusId = packet.Id;
            }

            return rawPacket;
        }

        private void HandleObjectInfoPacket(ObjectInfoPacket packet)
        {
            if (packet.Type == 0x2006)
                LastCorpseId = packet.Id;
        }
    }
}
