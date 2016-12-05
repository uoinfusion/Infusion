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
    }
}