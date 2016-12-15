using UltimaRX.Packets.Client;

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

        public override string ToString() => $"{Type} in {Direction}";
    }
}