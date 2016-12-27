using System;

namespace UltimaRX.Packets
{
    public struct Vector
    {
        public static readonly Vector NorthVector = new Vector(0, -1, 0);
        public static readonly Vector NortheastVector = new Vector(1, -1, 0);
        public static readonly Vector EastVector = new Vector(1, 0, 0);
        public static readonly Vector SoutheastVector = new Vector(1, 1, 0);
        public static readonly Vector SouthVector = new Vector(0, 1, 0);
        public static readonly Vector SouthwestVector = new Vector(-1, 1, 0);
        public static readonly Vector WestVector = new Vector(-1, 0, 0);
        public static readonly Vector NorthwestVector = new Vector(-1, -1, 0);
        public static readonly Vector NullVector = new Vector(0, 0, 0);
 
        public short X { get; }
        public short Y { get; }
        public byte Z { get; }

        public Vector RemoveZ()
        {
            if (Z == 0)
                return this;

            return new Vector(X, Y, 0);
        }

        public Vector Normalize()
        {
            if ((X == 0 || X == 1 || X == -1) &&
                (Y == 0 || Y == 1 || Y == -1) &&
                (Z == 0 || Z == 1))
            {
                return this;
            }

            short normalizedX;
            short normalizedY;
            byte normalizedZ;

            if (X == 0)
                normalizedX = 0;
            else if (X > 0)
                normalizedX = 1;
            else
                normalizedX = -1;

            if (Y == 0)
                normalizedY = 0;
            else if (Y > 0)
                normalizedY = 1;
            else
                normalizedY = -1;

            if (Z == 0)
                normalizedZ = 0;
            else
                normalizedZ = 1;

            return new Vector(normalizedX, normalizedY, normalizedZ);
        }

        public Direction ToDirection()
        {
            var normalized = Normalize();
            normalized = new Vector(normalized.X, normalized.Y, 0);

            if (normalized == EastVector)
                return Direction.East;
            if (normalized == NorthVector)
                return Direction.North;
            if (normalized == NortheastVector)
                return Direction.Northeast;
            if (normalized == NorthwestVector)
                return Direction.Northwest;
            if (normalized == SoutheastVector)
                return Direction.Southeast;
            if (normalized == SouthwestVector)
                return Direction.Southwest;
            if (normalized == SouthVector)
                return Direction.South;
            if (normalized == WestVector)
                return Direction.West;

            throw new InvalidOperationException($"Cannot convert vector {this} to direction.");
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Vector)) return false;

            var otherVector = (Vector) obj;

            return Equals(otherVector);
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

        public bool Equals(Vector otherVector) => X == otherVector.X && Y == otherVector.Y && Z == otherVector.Z;

        public static bool operator ==(Vector vector1, Vector vector2) => vector1.Equals(vector2);

        public static bool operator !=(Vector vector1, Vector vector2) => !vector1.Equals(vector2);

        public Vector(short x, short y, byte z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override string ToString() => $"({X}, {Y}, {Z})";
    }
}