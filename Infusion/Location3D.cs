using System;

namespace Infusion
{
    public struct Location3D
    {
        public Location3D(int x, int y, int z)
        {
            Coordinate.CheckCoordValue(nameof(x), x, 0, ushort.MaxValue);
            Coordinate.CheckCoordValue(nameof(y), y, 0, ushort.MaxValue);
            Coordinate.CheckCoordValue(nameof(z), z, sbyte.MinValue, sbyte.MaxValue);

            X = x;
            Y = y;
            Z = z;
        }

        public static implicit operator Location2D(Location3D location) =>
            new Location2D(location.X, location.Y);

        public static Vector operator -(Location3D location1, Location3D location2) =>
            new Vector((short) (location1.X - location2.X), (short) (location1.Y - location2.Y),
                (byte) (location1.Z - location2.Z));

        public override bool Equals(object obj)
        {
            if (obj is Location3D otherLocation)
            {
                return Equals(otherLocation);
            }

            return false;
        }

        public bool Equals(Location3D other) => X == other.X && Y == other.Y && Z == other.Z;

        public static bool operator ==(Location3D location1, Location3D location2) => location1.Equals(location2);

        public static bool operator !=(Location3D location1, Location3D location2) => !location1.Equals(location2);

        public ushort GetDistance(Location3D secondLocation)
        {
            var vector = this - secondLocation;

            var distance = Math.Sqrt(Math.Pow(vector.X, 2) + Math.Pow(vector.Y, 2) + Math.Pow(vector.Z, 2));

            return (ushort)distance;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode*397) ^ Y.GetHashCode();
                hashCode = (hashCode*397) ^ Z.GetHashCode();
                return hashCode;
            }
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public Location3D WithZ(byte z) => new Location3D(X, Y, z);

        public override string ToString() => $"{X}, {Y}, {Z}";

        public Location3D LocationInDirection(Direction direction)
        {
            Vector directionVector;

            if (direction == Direction.East)
                directionVector = Vector.EastVector;
            else if (direction == Direction.North)
                directionVector = Vector.NorthVector;
            else if (direction == Direction.Northeast)
                directionVector = Vector.NortheastVector;
            else if (direction == Direction.Northwest)
                directionVector = Vector.NorthwestVector;
            else if (direction == Direction.Southeast)
                directionVector = Vector.SoutheastVector;
            else if (direction == Direction.Southwest)
                directionVector = Vector.SouthwestVector;
            else if (direction == Direction.South)
                directionVector = Vector.SouthVector;
            else if (direction == Direction.West)
                directionVector = Vector.WestVector;
            else
                throw new ArgumentOutOfRangeException(nameof(direction), $"Unknown direction {direction}");

            return new Location3D(X + directionVector.X, Y + directionVector.Y, Z + directionVector.Z);
        }
    }
}