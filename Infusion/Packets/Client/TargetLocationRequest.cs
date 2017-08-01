using Infusion.IO;

namespace Infusion.Packets.Client
{
    internal sealed class TargetLocationRequest
    {
        public TargetLocationRequest(CursorId cursorId, Location3D location, ModelId tileType, CursorType cursorType)
        {
            var payload = new byte[19];
            var writer = new ArrayPacketWriter(payload);

            writer.WriteByte((byte) PacketDefinitions.TargetCursor.Id);
            writer.WriteByte((byte) CursorTarget.Location);
            writer.WriteUInt(cursorId.Value);
            writer.WriteByte((byte) cursorType);
            writer.WriteInt(0); // clicked on item = 0 (using location click)
            writer.WriteUShort(location.X);
            writer.WriteUShort(location.Y);
            writer.WriteByte(0); // unknown
            writer.WriteByte(location.Z);
            writer.WriteModelId(tileType);

            RawPacket = new Packet(PacketDefinitions.TargetCursor.Id, payload);
        }

        public TargetLocationRequest(CursorId cursorId, ObjectId itemId, CursorType cursorType, Location3D location,
            ModelId itemType)
        {
            var payload = new byte[19];

            var writer = new ArrayPacketWriter(payload);
            writer.WriteByte((byte) PacketDefinitions.TargetCursor.Id);
            writer.WriteByte((byte) CursorTarget.Object);
            writer.WriteUInt(cursorId.Value);
            writer.WriteByte((byte) cursorType);
            writer.WriteId(itemId);
            writer.WriteUShort(location.X);
            writer.WriteUShort(location.Y);
            writer.WriteByte(0); // unknown
            writer.WriteByte(location.Z);
            writer.WriteModelId(itemType);

            RawPacket = new Packet(PacketDefinitions.TargetCursor.Id, payload);
        }

        public Packet RawPacket { get; }
    }

    public enum CursorTarget : byte
    {
        Object = 0,
        Location = 1
    }

    public enum CursorType : byte
    {
        Neutral = 0,
        Harmful = 1,
        Helpful = 2,
        Cancel = 3
    }
}