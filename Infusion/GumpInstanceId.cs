namespace Infusion
{
    public struct GumpInstanceId
    {
        public GumpInstanceId(uint value) => Value = value;
        public uint Value { get; }

        public static explicit operator GumpInstanceId(uint value) => new GumpInstanceId(value);

        public static explicit operator uint(GumpInstanceId id) => id.Value;

        public static bool operator ==(GumpInstanceId id1, GumpInstanceId id2) => id1.Equals(id2);

        public static bool operator !=(GumpInstanceId id1, GumpInstanceId id2) => !id1.Equals(id2);

        public override bool Equals(object obj)
        {
            if (obj is GumpInstanceId)
            {
                var id = (GumpInstanceId)obj;
                return Equals(id);
            }

            return false;
        }

        public bool Equals(GumpInstanceId other) => Value == other.Value;

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value.ToString("X8");
    }
}