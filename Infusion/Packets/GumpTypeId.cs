namespace Infusion.Packets
{
    public struct GumpTypeId
    {
        public GumpTypeId(uint value) => Value = value;
        public uint Value { get; }

        public static explicit operator GumpTypeId(uint value) => new GumpTypeId(value);

        public static explicit operator uint(GumpTypeId id) => id.Value;

        public static bool operator ==(GumpTypeId id1, GumpTypeId id2) => id1.Equals(id2);

        public static bool operator !=(GumpTypeId id1, GumpTypeId id2) => !id1.Equals(id2);

        public override bool Equals(object obj)
        {
            if (obj is GumpTypeId)
            {
                var id = (GumpTypeId)obj;
                return Equals(id);
            }

            return false;
        }

        public bool Equals(GumpTypeId other) => Value == other.Value;

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value.ToString("X8");
    }
}