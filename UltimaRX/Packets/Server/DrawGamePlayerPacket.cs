using UltimaRX.IO;

namespace UltimaRX.Packets.Server
{
    public class DrawGamePlayerPacket : MaterializedPacket
    {
        private Packet rawPacket;

        public uint PlayerId { get; private set; }

        public ushort BodyType { get; private set; }

        public Location3D Location { get; private set; }

        public override Packet RawPacket => rawPacket;

        public Direction Direction { get; private set; }

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;
            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(1);

            PlayerId = reader.ReadUInt();
            BodyType = reader.ReadUShort();
            reader.Skip(1); // unknown
            reader.Skip(2); // skin color / hue
            reader.Skip(1); // flag byte

            var xloc = reader.ReadUShort();
            var yloc = reader.ReadUShort();
            reader.Skip(2); // unknown
            Direction = (Direction) reader.ReadByte();
            var zloc = reader.ReadByte();

            Location = new Location3D(xloc, yloc, zloc);
        }
    }
}