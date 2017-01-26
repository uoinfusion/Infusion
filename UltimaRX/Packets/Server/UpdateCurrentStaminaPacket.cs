using UltimaRX.IO;

namespace UltimaRX.Packets.Server
{
    public class UpdateCurrentStaminaPacket : MaterializedPacket
    {
        private Packet rawPacket;
        public uint PlayerId { get; private set; }

        public ushort MaxStamina { get; private set; }

        public ushort CurrentStamina { get; private set; }

        public override Packet RawPacket => rawPacket;

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;
            var reader = new ArrayPacketReader(rawPacket.Payload);
            reader.Skip(1);

            PlayerId = reader.ReadUInt();
            MaxStamina = reader.ReadUShort();
            CurrentStamina = reader.ReadUShort();
        }
    }
}