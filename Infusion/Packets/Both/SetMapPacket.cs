using Infusion.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infusion.Packets.Both
{
    internal sealed class SetMapPacket
    {
        private Packet rawPacket;

        public Packet RawPacket => rawPacket;

        public byte MapId { get; set; }

        public SetMapPacket()
        {
        }

        public SetMapPacket(byte mapId)
        {
            MapId = mapId;
            byte[] payload = new byte[6];

            var writer = new ArrayPacketWriter(payload);
            writer.WriteByte(0xBF);
            writer.WriteUShort(6);
            writer.WriteUShort(8);
            writer.WriteByte(mapId);
            rawPacket = new Packet(PacketDefinitions.GeneralInformationPacket.Id, payload);
        }

        public void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;

            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(5);
            MapId = reader.ReadByte();
        }
    }
}
