using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion
{
    public struct SoundId
    {
        public SoundId(ushort value) => Value = value;
        public ushort Value { get; }

        public static implicit operator SoundId(ushort value) => new SoundId(value);

        public static implicit operator ushort(SoundId id) => id.Value;

        public static bool operator ==(SoundId id1, SoundId id2) => id1.Equals(id2);

        public static bool operator !=(SoundId id1, SoundId id2) => !id1.Equals(id2);

        public override bool Equals(object obj)
        {
            if (obj is SoundId id)
            {
                return Equals(id);
            }

            return false;
        }

        public bool Equals(SoundId other) => Value == other.Value;

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => $"0x{Value:X4}";

    }
}
