namespace UltimaRX.Packets
{
    public struct Color
    {
        public Color(ushort id)
        {
            Id = id;
        }

        public static explicit operator Color(ushort id)
        {
            return new Color(id);
        }

        public ushort Id { get; }
    }
}