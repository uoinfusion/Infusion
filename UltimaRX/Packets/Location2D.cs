namespace UltimaRX.Packets
{
    public struct Location2D
    {
        public Location2D(ushort x, ushort y)
        {
            X = x;
            Y = y;
        }

        public ushort X { get; }
        public ushort Y { get; }
    }
}