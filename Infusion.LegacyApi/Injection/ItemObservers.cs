using Infusion.Packets;
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
        private Legacy legacyApi;
        public ObjectId LastCorpseId { get; internal set; } = 0;

        public ItemObservers(IServerPacketSubject serverPacketSubject, Legacy legacyApi)
        {
            this.legacyApi = legacyApi;
            serverPacketSubject.Subscribe(PacketDefinitions.ObjectInfo, HandleObjectInfoPacket);
        }

        private void HandleObjectInfoPacket(ObjectInfoPacket packet)
        {
            if (packet.Type == 0x2006)
                LastCorpseId = packet.Id;
        }
    }
}
