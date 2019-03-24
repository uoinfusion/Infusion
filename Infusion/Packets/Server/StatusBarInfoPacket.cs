using System;
using Infusion.IO;

namespace Infusion.Packets.Server
{
    internal sealed class StatusBarInfoPacket : MaterializedPacket
    {
        private Packet rawPacket;

        public ObjectId PlayerId { get; private set; }
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
        public ushort Armor { get; private set; }

        public bool CanRename { get; private set; }

        public override Packet RawPacket => rawPacket;
        public byte Status { get; private set; }

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;
            var reader = new ArrayPacketReader(rawPacket.Payload);

            reader.Skip(3);
            PlayerId = reader.ReadObjectId();
            PlayerName = reader.ReadString(30);
            CurrentHealth = reader.ReadUShort();
            MaxHealth = reader.ReadUShort();
            CanRename = reader.ReadBool();
            var validStats = reader.ReadByte(); // status flag / valid stats

            if (validStats == 0)
                return;

            if (validStats != 1 && validStats != 7 && validStats != 4 && validStats != 6 && validStats != 5)
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
            Armor = reader.ReadUShort();
            Weight = reader.ReadUShort();
        }
    }
}