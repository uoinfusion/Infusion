using System;
using System.Collections.Generic;
using Infusion.IO;

namespace Infusion.Packets.Both
{
    internal sealed class SendSkillsPacket : MaterializedPacket
    {
        public SkillValue[] Values { get; private set; }

        public override void Deserialize(Packet rawPacket)
        {
            this.rawPacket = rawPacket;
            var reader = new ArrayPacketReader(rawPacket.Payload);

            reader.ReadByte();
            ushort packetSize = reader.ReadUShort();
            byte type = reader.ReadByte();

            var values = new List<SkillValue>();

            ushort skillNumber;
            ushort value;
            ushort unmodifiedValue;
            bool isLocked = false;

            switch (type)
            {
                case 0x00:
                case 0x02:
                case 0xDF:
                    while (reader.Position < packetSize && (skillNumber = reader.ReadUShort() ) != 0)
                    {
                        value = reader.ReadUShort();
                        unmodifiedValue = reader.ReadUShort();
                        isLocked = reader.ReadBool();
                        ushort cap = 0;
                        if (type == 0x02 || type == 0xDF)
                            cap = reader.ReadUShort();

                        values.Add(new SkillValue((Skill)skillNumber , value, unmodifiedValue, cap, isLocked));
                    }
                    break;
                case 0xFF:
                    skillNumber = reader.ReadUShort();
                    var skill = skillNumber != 0 ? (Skill)(skillNumber + 1) : Skill.Alchemy;
                    value = reader.ReadUShort();
                    unmodifiedValue = reader.ReadUShort();
                    isLocked = reader.ReadBool();
                    values.Add(new SkillValue(skill, value, unmodifiedValue,0, isLocked));
                    break;
                default:
                    throw new NotImplementedException($"Unknown type {type} of SendSkills packet.");
            }


            Values = values.ToArray();
        }

        private Packet rawPacket;

        public override Packet RawPacket => rawPacket;
    }

    public struct SkillValue : IEquatable<SkillValue>
    {
        public Skill Skill { get; }
        public ushort UnmodifiedValue { get; }
        public decimal UnmodifiedPercentage => decimal.Divide(UnmodifiedValue, 10);

        public ushort Value { get; }
        public decimal Percentage => decimal.Divide(Value, 10);

        public ushort Cap { get; }
        public decimal CapPercentage => decimal.Divide(Cap, 10);

        public bool Lock { get; }

        public SkillValue(Skill skill, ushort value, ushort unmodifiedValue, ushort cap, bool isLocked)
        {
            Skill = skill;
            Value = value;
            UnmodifiedValue = unmodifiedValue;
            Cap = cap;
            Lock = isLocked;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is SkillValue))
                return false;

            return Equals((SkillValue)obj);
        }

        public override string ToString() => (Percentage == UnmodifiedPercentage)
            ? $"{Skill}: {Percentage:F1} %"
            : $"{Skill}: {Percentage:F1} % / {UnmodifiedPercentage:F1} %";

        public bool Equals(SkillValue other)
        {
            return Skill == other.Skill && Value == other.Value && UnmodifiedValue == other.UnmodifiedValue;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) Skill;
                hashCode = (hashCode * 397) ^ Value.GetHashCode();
                hashCode = (hashCode * 397) ^ UnmodifiedValue.GetHashCode();
                return hashCode;
            }
        }
    }
}
