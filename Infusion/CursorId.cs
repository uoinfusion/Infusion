namespace Infusion
{
    public struct CursorId
    {
        public CursorId(uint value) => Value = value;
        public uint Value { get; }

        public static explicit operator CursorId(uint value) => new CursorId(value);

        public static explicit operator uint(CursorId id) => id.Value;

        public static bool operator ==(CursorId id1, CursorId id2) => id1.Equals(id2);

        public static bool operator !=(CursorId id1, CursorId id2) => !id1.Equals(id2);

        public override bool Equals(object obj)
        {
            if (obj is CursorId)
            {
                var id = (CursorId)obj;
                return Equals(id);
            }

            return false;
        }

        public bool Equals(CursorId other) => Value == other.Value;

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => $"0x{Value:X8}";
    }

}
