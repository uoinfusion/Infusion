using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infusion.IO;

namespace Infusion.Packets.Client
{
    internal sealed class GetClientStatusRequest
    {
        public Packet RawPacket { get; }

        public GetClientStatusRequest(ObjectId id)
        {
            var payload = new byte[10];
            var writer = new ArrayPacketWriter(payload);

            writer.WriteByte((byte)PacketDefinitions.GetClientStatus.Id);
            writer.WriteUInt(0xedededed);
            writer.WriteByte(0x04);
            writer.WriteId(id);

            RawPacket = new Packet(PacketDefinitions.GetClientStatus.Id, payload);
        }
    }
}
