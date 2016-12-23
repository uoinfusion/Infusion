namespace UltimaRX.Packets
{
    public struct Movement
    {
        public Movement(Direction direction, MovementType type) : this()
        {
            Direction = direction;
            Type = type;
        }

        public Direction Direction { get; }
        public MovementType Type { get; }

        public static explicit operator Movement(byte rawByte) =>
            (rawByte & 0x80) != 0
                ? new Movement((Direction) (rawByte - 0x80), MovementType.Run)
                : new Movement((Direction) rawByte, MovementType.Walk);

        public override string ToString() => $"{Type} in {Direction}";
    }
}