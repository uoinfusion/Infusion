using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltimaRX.IO;

namespace UltimaRX.Packets.Client
{
    public class DoubleClickRequest
    {
        public DoubleClickRequest(uint objectId)
        {
            byte[] payload = new byte[5];

            var writer = new ArrayPacketWriter(payload);
            writer.WriteByte((byte)PacketDefinitions.DoubleClick.Id);
            writer.WriteUInt(objectId);

            RawPacket = new Packet(PacketDefinitions.DoubleClick.Id, payload);
        }

        public Packet RawPacket { get; }
    }
}
