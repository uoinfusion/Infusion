using Infusion.IO;

namespace Infusion.Packets.Server
{
    public class UpdateCurrentHealthPacket : MaterializedPacket
    {
        private Packet rawPacket;
        public ObjectId PlayerId { get; private set; }

        public ushort MaxHealth { get; private set; }
        public ushort CurrentHealth { get; private set; }

        public override Packet RawPacket => rawPacket;

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;
            var reader = new ArrayPacketReader(rawPacket.Payload);

            reader.Skip(1);
            PlayerId = reader.ReadObjectId();
            MaxHealth = reader.ReadUShort();
            CurrentHealth = reader.ReadUShort();
        }
    }
}