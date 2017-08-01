namespace Infusion.Packets
{
    public struct MessageId
    {
        public MessageId(int value) => Value = value;
        public int Value { get; }

        public static explicit operator MessageId(int value) => new MessageId(value);

        public static explicit operator int(MessageId id) => id.Value;

        public static bool operator ==(MessageId id1, MessageId id2) => id1.Equals(id2);

        public static bool operator !=(MessageId id1, MessageId id2) => !id1.Equals(id2);

        public override bool Equals(object obj)
        {
            if (obj is MessageId)
            {
                var id = (MessageId) obj;
                return Equals(id);
            }

            return false;
        }

        public bool Equals(MessageId other) => Value == other.Value;

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value.ToString("X8");
    }
}