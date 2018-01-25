using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infusion.IO;

namespace Infusion.Packets.Both
{
    internal sealed class CloseGenericGumpPacket
    {
        public GumpTypeId GumpTypeId { get; private set; }

        public CloseGenericGumpPacket()
        {
        }

        public CloseGenericGumpPacket(GumpTypeId gumpTypeId)
        {
            GumpTypeId = gumpTypeId;

            byte[] payload = new byte[13];
            var writer = new ArrayPacketWriter(payload);

            writer.WriteByte((byte)PacketDefinitions.GeneralInformationPacket.Id);
            writer.WriteUShort(13); // length
            writer.WriteUShort(4); // subcommand
            writer.WriteUInt(gumpTypeId.Value);
            writer.WriteUInt(0);

            rawPacket = new Packet(PacketDefinitions.GeneralInformationPacket.Id, payload);
        }

        public void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;

            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(5);
            GumpTypeId = (GumpTypeId)reader.ReadUInt();
        }

        private Packet rawPacket;

        public Packet RawPacket => rawPacket;
    }
}
