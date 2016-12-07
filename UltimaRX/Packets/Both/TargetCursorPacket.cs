using UltimaRX.IO;
using UltimaRX.Packets.Client;

namespace UltimaRX.Packets.Both
{
    public class TargetCursorPacket : MaterializedPacket
    {
        public CursorTarget CursorTarget { get; private set; }
        public uint CursorId { get; private set; }
        public CursorType CursorType { get; private set; }

        public int ClickedOnId { get; private set; }
        public int ClickedOnType { get; private set; }
        public Location3D Location { get; set; }

        private Packet rawPacket;

        public TargetCursorPacket(CursorTarget cursorTarget, uint cursorId, CursorType cursorType)
        {
            CursorTarget = cursorTarget;
            CursorId = cursorId;
            CursorType = cursorType;
            byte[] payload = new byte[19];
            var writer = new ArrayPacketWriter(payload);

            writer.WriteByte((byte)PacketDefinitions.TargetCursor.Id);
            writer.WriteByte((byte)cursorTarget);
            writer.WriteUInt(cursorId);
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
            reader.Position = 1;

            CursorTarget = (CursorTarget)reader.ReadByte();
            CursorId = reader.ReadUInt();
            CursorType = (CursorType)reader.ReadByte();
            ClickedOnId = reader.ReadInt();

            ushort xloc = reader.ReadUShort();
            ushort yloc = reader.ReadUShort();
            byte unknown = reader.ReadByte();
            byte zloc = reader.ReadByte();

            Location = new Location3D(xloc, yloc, zloc);

            ClickedOnType = reader.ReadUShort();
        }

        public override Packet RawPacket => rawPacket;
    }
}
