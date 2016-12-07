namespace UltimaRX.Packets
{
    public sealed class Item
    {
        public Item(int id, ushort type, ushort amount, ushort xLoc, ushort yLoc, ushort color, int? containerId)
        {
            Type = type;
            Id = id;
            Amount = amount;
            Color = (Color) color;
            ContainerId = containerId;
            Location = new Location3D(xLoc, yLoc, 0);
        }

        public Item(int id, ushort type, ushort amount, Location3D location, Color? color = null,
            int? containerId = null, Layer? layer = null)
        {
            Id = id;
            Type = type;
            Amount = amount;
            Location = location;
            Color = color ?? default(Color);
            ContainerId = containerId;
            Layer = layer;
        }

        public ushort Type { get; }

        public int Id { get; }

        public ushort Amount { get; }

        public Location3D Location { get; }

        public int? ContainerId { get; }

        public Color Color { get; }

        public Layer? Layer { get; }

        public override string ToString()
        {
            return (ContainerId.HasValue)
                ? $"Id: {Id:X8}, Type: {Type:X4}; Amount: {Amount}; Location: {Location}; Container {ContainerId.Value:X8}"
                : $"Id: {Id:X8}, Type: {Type:X4}; Amount: {Amount}; Location: {Location}";
        }
    }
}