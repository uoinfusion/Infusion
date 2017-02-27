using Infusion.IO;

namespace Infusion.Packets.Server
{
    public class AddItemToContainerPacket : MaterializedPacket
    {
        public uint ItemId { get; private set; }
        public ModelId Type { get; private set; }
        public ushort Amount { get; private set; }
        public Location2D Location { get; private set; }
        public uint ContainerId { get; private set; }
        public Color Color { get; private set; }

        public override Packet RawPacket => rawPacket;

        private Packet rawPacket;

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;

            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(1);

            ItemId = reader.ReadUInt();
            Type = reader.ReadModelId();
            reader.Skip(1);
            Amount = reader.ReadUShort();
            Location = new Location2D(reader.ReadUShort(), reader.ReadUShort());
            ContainerId = reader.ReadUInt();
            Color = (Color) reader.ReadUShort();
        }
    }
}