using Infusion.IO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infusion.Packets.Server
{
    internal class EnableLockedClientFeaturesPacket : MaterializedPacket
    {
        protected Packet rawPacket;

        public uint Flags { get; set; }

        public override Packet RawPacket => rawPacket;

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;
            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(1);

            Flags = reader.ReadUShort();
        }

        public virtual Packet Serialize()
        {
            var payload = new byte[3];
            var writer = new ArrayPacketWriter(payload);

            writer.WriteByte((byte)PacketDefinitions.EnableLockedClientFeatures.Id);
            writer.WriteUShort((ushort)Flags);

            rawPacket = new Packet(PacketDefinitions.EnableLockedClientFeatures.Id, payload);

            return rawPacket;
        }
    }
}
