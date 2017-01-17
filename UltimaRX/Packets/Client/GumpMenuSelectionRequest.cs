using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UltimaRX.IO;

namespace UltimaRX.Packets.Client
{
    public class GumpMenuSelectionRequest
    {
        public GumpMenuSelectionRequest(uint id, uint gumpId, uint triggerId)
        {
            var payload = new byte[23];
            var writer = new ArrayPacketWriter(payload);

            writer.WriteByte((byte)PacketDefinitions.GumpMenuSelection.Id);
            writer.WriteUShort(23);
            writer.WriteUInt(id);
            writer.WriteUInt(gumpId);
            writer.WriteUInt(triggerId);
            writer.WriteUInt(0);
            writer.WriteUInt(0);

            RawPacket = new Packet(PacketDefinitions.GumpMenuSelection.Id, payload);
        }

        public Packet RawPacket { get; }
    }
}
