using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infusion.IO;

namespace Infusion.Packets.Client
{
//            08:22:26.900 >>>> proxy -> server: RawPacket GetClientStatus, length = 10
//0x34, 0xED, 0xED, 0xED, 0xED, 0x04, 0x00, 0x05, 0xB1, 0x12,

    public class GetClientStatusRequest
    {
        public Packet RawPacket { get; }

        public GetClientStatusRequest(uint id)
        {
            var payload = new byte[10];
            var writer = new ArrayPacketWriter(payload);

            writer.WriteByte((byte)PacketDefinitions.GetClientStatus.Id);
            writer.WriteUInt(0xedededed);
            writer.WriteByte(0x04);
            writer.WriteUInt(id);

            RawPacket = new Packet(PacketDefinitions.GetClientStatus.Id, payload);
        }
    }
}
