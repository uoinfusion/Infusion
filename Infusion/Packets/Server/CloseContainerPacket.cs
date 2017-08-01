using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infusion.IO;

namespace Infusion.Packets.Server
{
    internal sealed class CloseContainerPacket
    {
        public Packet RawPacket { get; private set; }

        public CloseContainerPacket(ObjectId containerId)
        {
            byte[] payload = new byte[13];

            var writer = new ArrayPacketWriter(payload);
            writer.WriteByte((byte)PacketDefinitions.GeneralInformationPacket.Id);
            writer.WriteUShort(13);
            writer.WriteUShort(0x16);
            writer.WriteUInt(0x0C);
            writer.WriteId(containerId);

            RawPacket = new Packet(PacketDefinitions.GeneralInformationPacket.Id, payload);
        }
    }
}
