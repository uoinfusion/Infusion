namespace UltimaRX.Packets
{
    internal struct Vector
    {
        public short X { get; }
        public short Y { get; }
        public byte Z { get; }

        public Vector(short x, short y, byte z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

    }
}