namespace Infusion
{
    public struct GumpControlId
    {
        public GumpControlId(uint value) => Value = value;
        public uint Value { get; }

        public static explicit operator GumpControlId(uint value) => new GumpControlId(value);

        public static explicit operator uint(GumpControlId id) => id.Value;

        public static bool operator ==(GumpControlId id1, GumpControlId id2) => id1.Equals(id2);

        public static bool operator !=(GumpControlId id1, GumpControlId id2) => !id1.Equals(id2);

        public override bool Equals(object obj)
        {
            if (obj is GumpControlId)
            {
                var id = (GumpControlId)obj;
                return Equals(id);
            }

            return false;
        }

        public bool Equals(GumpControlId other) => Value == other.Value;

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => $"0x{Value:X8}";
    }
}