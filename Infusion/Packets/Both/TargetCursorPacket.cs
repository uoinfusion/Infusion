using Infusion.IO;
using Infusion.Packets.Client;

namespace Infusion.Packets.Both
{
    internal sealed class TargetCursorPacket : MaterializedPacket
    {
        public CursorTarget CursorTarget { get; private set; }
        public CursorId CursorId { get; private set; }
        public CursorType CursorType { get; private set; }

        public ObjectId ClickedOnId { get; private set; }
        public ModelId ClickedOnType { get; private set; }
        public Location3D Location { get; set; }

        private Packet rawPacket;

        public TargetCursorPacket(CursorTarget cursorTarget, CursorId cursorId, CursorType cursorType)
        {
            CursorTarget = cursorTarget;
            CursorId = cursorId;
            CursorType = cursorType;
            byte[] payload = new byte[19];
            var writer = new ArrayPacketWriter(payload);

            writer.WriteByte((byte)PacketDefinitions.TargetCursor.Id);
            writer.WriteByte((byte)cursorTarget);
            writer.WriteUInt(cursorId.Value);
            writer.WriteByte((byte)cursorType);

            rawPacket = new Packet(PacketDefinitions.TargetCursor.Id, payload);
        }

        public TargetCursorPacket()
        {
        }

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;
            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(1);

            CursorTarget = (CursorTarget)reader.ReadByte();
            CursorId = new CursorId(reader.ReadUInt());
            CursorType = (CursorType)reader.ReadByte();
            ClickedOnId = reader.ReadObjectId();

            ushort xloc = reader.ReadUShort();
            ushort yloc = reader.ReadUShort();
            byte unknown = reader.ReadByte();
            sbyte zloc = reader.ReadSByte();

            Location = new Location3D(xloc, yloc, zloc);

            ClickedOnType = reader.ReadModelId();
        }

        public override Packet RawPacket => rawPacket;
    }
}
