using Infusion.IO;

namespace Infusion.Packets.Client
{
    public class GumpMenuSelectionRequest : MaterializedPacket
    {
        public GumpMenuSelectionRequest()
        {
            
        }

        public GumpMenuSelectionRequest(uint id, uint gumpId, uint triggerId)
        {
            Id = id;
            GumpId = gumpId;
            TriggerId = triggerId;

            var payload = new byte[23];
            var writer = new ArrayPacketWriter(payload);

            writer.WriteByte((byte)PacketDefinitions.GumpMenuSelection.Id);
            writer.WriteUShort(23);
            writer.WriteUInt(id);
            writer.WriteUInt(gumpId);
            writer.WriteUInt(triggerId);
            writer.WriteUInt(0);
            writer.WriteUInt(0);

            rawPacket = new Packet(PacketDefinitions.GumpMenuSelection.Id, payload);
        }

        private Packet rawPacket;

        public override Packet RawPacket => rawPacket;

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;
            var reader = new ArrayPacketReader(rawPacket.Payload);

            reader.Skip(3);
            Id = reader.ReadUInt();
            GumpId = reader.ReadUInt();
            TriggerId = reader.ReadUInt();
        }

        public uint TriggerId { get; private set; }

        public uint GumpId { get; private set; }

        public uint Id { get; private set; }
    }
}
