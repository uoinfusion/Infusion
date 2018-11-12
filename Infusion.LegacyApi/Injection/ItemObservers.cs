using Infusion.Packets;
using Infusion.Packets.Both;
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
        public ObjectId LastCorpseId { get; private set; } = 0;
        public ObjectId LastStatusId { get; private set; } = 0;
        public ObjectId LastTargetId { get; private set; } = 0;

        public ItemObservers(IServerPacketSubject serverPacketSubject, IClientPacketSubject clientPacketSubject)
        {
            serverPacketSubject.Subscribe(PacketDefinitions.ObjectInfo, HandleObjectInfoPacket);
            serverPacketSubject.Subscribe(PacketDefinitions.StatusBarInfo, HandleStatusBarInfoPacket);

            clientPacketSubject.RegisterOutputFilter(FilterSentClientPackets);
        }

        private Packet? FilterSentClientPackets(Packet rawPacket)
        {
            if (rawPacket.Id == PacketDefinitions.TargetCursor.Id)
            {
                var packet = PacketDefinitionRegistry.Materialize<TargetCursorPacket>(rawPacket);
                LastTargetId = packet.ClickedOnId;
            }

            return rawPacket;
        }

        private void HandleObjectInfoPacket(ObjectInfoPacket packet)
        {
            if (packet.Type == 0x2006)
                LastCorpseId = packet.Id;
        }

        private void HandleStatusBarInfoPacket(StatusBarInfoPacket packet)
        {
            LastStatusId = packet.PlayerId;
        }
    }
}
