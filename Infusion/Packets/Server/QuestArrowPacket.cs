using System;
using Infusion.IO;

namespace Infusion.Packets.Server
{
    internal sealed class QuestArrowPacket : MaterializedPacket
    {
        private Packet rawPacket;
        public bool Active { get; private set; }
        public Location2D Location { get; private set; }

        public override Packet RawPacket => rawPacket;

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;

            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(1);

            var activeByte = reader.ReadByte();
            if (activeByte != 0 && activeByte != 1)
                throw new NotSupportedException($"QuestArrowPacket.Active has unsupported value: {activeByte}");
            Active = activeByte != 0;

            Location = new Location2D(reader.ReadUShort(), reader.ReadUShort());
        }

        public QuestArrowPacket()
        {
        }

        public QuestArrowPacket(Location2D location, bool active)
        {
            Location = location;
            Active = active;

            var payload = new byte[6];
            var writer = new ArrayPacketWriter(payload);

            writer.WriteByte((byte)PacketDefinitions.QuestArrow.Id);
            writer.WriteByte((byte)(active ? 1 : 0));
            writer.WriteUShort((ushort)location.X);
            writer.WriteUShort((ushort)location.Y);

            rawPacket = new Packet(payload[0], payload);
        }
    }
}