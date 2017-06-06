using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infusion.IO;

namespace Infusion.Packets.Both
{
    public class CloseGenericGumpPacket
    {
        public uint GumpId { get; }

        public CloseGenericGumpPacket(uint gumpId)
        {
            GumpId = gumpId;

            byte[] payload = new byte[13];
            var writer = new ArrayPacketWriter(payload);

            writer.WriteByte((byte)PacketDefinitions.GeneralInformationPacket.Id);
            writer.WriteUShort(13); // length
            writer.WriteUShort(4); // subcommand
            writer.WriteUInt(gumpId);
            writer.WriteUInt(0);

            RawPacket = new Packet(PacketDefinitions.GeneralInformationPacket.Id, payload);
        }

        public Packet RawPacket { get; }
    }
}
