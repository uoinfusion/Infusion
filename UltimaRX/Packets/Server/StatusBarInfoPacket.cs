using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public ushort  MaxStamina { get; private set; }
        public ushort CurrentMana { get; private set; }
        public ushort MaxMana { get; private set; }
        public uint Gold { get; private set; }
        public ushort Weight { get; private set; }
        public ushort Strength { get; private set; }
        public ushort Dexterity { get; private set; }
        public ushort Intelligence { get; private set; }

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
            byte validStats = reader.ReadByte(); // status flag / valid stats
            if (validStats == 0)
                throw new NotImplementedException("validStats is 0");

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

        public override Packet RawPacket => rawPacket;
    }
}
