using System;

namespace UltimaRX.Packets
{
    public struct Direction
    {
        public static readonly Direction North = (Direction) 0x00;
        public static readonly Direction Northeast = (Direction) 0x01;
        public static readonly Direction East = (Direction) 0x02;
        public static readonly Direction Southeast = (Direction) 0x03;
        public static readonly Direction South = (Direction) 0x04;
        public static readonly Direction Southwest = (Direction) 0x05;
        public static readonly Direction West = (Direction) 0x06;
        public static readonly Direction Northwest = (Direction) 0x07;

        private readonly byte value;

        private Direction(byte value)
        {
            this.value = value;
        }

        public static bool operator ==(Direction direction1, Direction direction2)
        {
            return direction1.Equals(direction2);
        }

        public static bool operator !=(Direction direction1, Direction direction2)
        {
            return !direction1.Equals(direction2);
        }

        public static explicit operator byte(Direction direction)
        {
            return direction.value;
        }

        public static explicit operator Direction(byte value)
        {
            if (value > 0x07)
            {
                throw new ArgumentOutOfRangeException(nameof(value), $"Unknown direction number: {value}");
            }

            return new Direction(value);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Direction))
                return false;

            var otherDirection = (Direction) obj;
            return Equals(otherDirection);
        }

        public bool Equals(Direction other)
        {
            return value == other.value;
        }

        public override int GetHashCode()
        {
            return value.GetHashCode();
        }

        public override string ToString()
        {
            if (this == North)
                return "North";
            if (this == Northwest)
                return "Northwest";
            if (this == Northeast)
                return "Northeast";
            if (this == South)
                return "South";
            if (this == Southeast)
                return "Southeast";
            if (this == Southwest)
                return "Southwest";
            if (this == East)
                return "East";
            if (this == West)
                return "West";
            throw new InvalidOperationException($"Cannot convert {value} to string.");
        }
    }
}