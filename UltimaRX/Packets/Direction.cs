using System;

namespace UltimaRX.Packets
{
    public struct Direction
    {
        public const byte North = 0x00;
        public const byte Northeast = 0x01;
        public const byte East = 0x02;
        public const byte Southeast = 0x03;
        public const byte South = 0x04;
        public const byte Southwest = 0x05;
        public const byte West = 0x06;
        public const byte Northwest = 0x07;

        private readonly byte value;

        private Direction(byte value)
        {
            this.value = value;
        }

        public static implicit operator byte(Direction direction)
        {
            return direction.value;
        }

        public static implicit operator Direction(byte value)
        {
            if (value > 0x07)
            {
                throw new ArgumentOutOfRangeException(nameof(value), $"Unknown direction number: {value}");
            }

            return new Direction(value);
        }

        public override string ToString()
        {
            switch (value)
            {
                case North:
                    return "North";
                case Northwest:
                    return "Northwest";
                case Northeast:
                    return "Northeast";
                case South:
                    return "South";
                case Southeast:
                    return "Southeast";
                case Southwest:
                    return "Southwest";
                case East:
                    return "East";
                case West:
                    return "West";
                default:
                    throw new InvalidOperationException($"Cannot convert {value} to string.");
            }
        }
    }
}