using System;

namespace UltimaRX.Packets
{
    public struct Location3D
    {
        public Location3D(ushort x, ushort y, byte z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public ushort X { get; }
        public ushort Y { get; }
        public byte Z { get; }

        public override string ToString()
        {
            return $"{X}, {Y}, {Z}";
        }

        private static readonly Vector NorthVector = new Vector(0, -1, 0);
        private static readonly Vector NortheastVector = new Vector(1, -1, 0);
        private static readonly Vector EastVector = new Vector(1, 0, 0);
        private static readonly Vector SoutheastVector = new Vector(1, 1, 0);
        private static readonly Vector SouthVector = new Vector(0, 1, 0);
        private static readonly Vector SouthwestVector = new Vector(-1, 1, 0);
        private static readonly Vector WestVector = new Vector(-1, 0, 0);
        private static readonly Vector NorthwestVector = new Vector(-1, -1, 0);
        private static readonly Vector NullVector = new Vector(0, 0, 0);

        public Location3D LocationInDirection(Direction direction)
        {
            Vector directionVector;

            switch (direction)
            {
                case Direction.East:
                    directionVector = EastVector;
                    break;
                case Direction.North:
                    directionVector = NorthVector;
                    break;
                case Direction.Northeast:
                    directionVector = NortheastVector;
                    break;
                case Direction.Northwest:
                    directionVector = NorthwestVector;
                    break;
                case Direction.Southeast:
                    directionVector = SoutheastVector;
                    break;
                case Direction.Southwest:
                    directionVector = SouthwestVector;
                    break;
                case Direction.South:
                    directionVector = SouthVector;
                    break;
                case Direction.West:
                    directionVector = WestVector;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), $"Unknown direction {direction}");
            }

            return new Location3D((ushort)(X + directionVector.X), (ushort)(Y + directionVector.Y), (byte)(Z + directionVector.Z));
        }
    }
}