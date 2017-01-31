using System;
using UltimaRX.IO;

namespace UltimaRX.Packets.Server
{
    public class StatusBarInfoPacket : MaterializedPacket
    {
        private Packet rawPacket;

        public uint PlayerId { get; private set; }
        public string PlayerName { get; private set; }
        public ushort CurrentHealth { get; private set; }
        public ushort MaxHealth { get; private set; }
        public ushort CurrentStamina { get; private set; }
        public ushort MaxStamina { get; private set; }
        public ushort CurrentMana { get; private set; }
        public ushort MaxMana { get; private set; }
        public uint Gold { get; private set; }
        public ushort Weight { get; private set; }
        public ushort Strength { get; private set; }
        public ushort Dexterity { get; private set; }
        public ushort Intelligence { get; private set; }

        public override Packet RawPacket => rawPacket;
        public byte Status { get; private set; }

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;
            var reader = new ArrayPacketReader(rawPacket.Payload);

            reader.Skip(3);
            PlayerId = reader.ReadUInt();
            PlayerName = reader.ReadString(30);
            CurrentHealth = reader.ReadUShort();
            MaxHealth = reader.ReadUShort();
            reader.ReadByte(); // name change flag
            var validStats = reader.ReadByte(); // status flag / valid stats

            if (validStats == 0)
                return;

            if (validStats != 1 && validStats != 7)
                throw new NotImplementedException($"unknown validStats {validStats}");

            reader.ReadByte(); // sex + race
            Strength = reader.ReadUShort();
            Dexterity = reader.ReadUShort();
            Intelligence = reader.ReadUShort();
            CurrentStamina = reader.ReadUShort();
            MaxStamina = reader.ReadUShort();
            CurrentMana = reader.ReadUShort();
            MaxMana = reader.ReadUShort();
            Gold = reader.ReadUInt();
            reader.ReadUShort();
            Weight = reader.ReadUShort();
        }
    }
}