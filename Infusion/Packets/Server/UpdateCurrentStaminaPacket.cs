using Infusion.IO;

namespace Infusion.Packets.Server
{
    internal sealed class UpdateCurrentStaminaPacket : MaterializedPacket
    {
        private Packet rawPacket;
        public ObjectId PlayerId { get; private set; }

        public ushort MaxStamina { get; private set; }

        public ushort CurrentStamina { get; private set; }

        public override Packet RawPacket => rawPacket;

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;
            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(1);

            PlayerId = reader.ReadObjectId();
            MaxStamina = reader.ReadUShort();
            CurrentStamina = reader.ReadUShort();
        }
    }
}