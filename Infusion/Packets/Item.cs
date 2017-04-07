namespace Infusion.Packets
{
    public sealed class Item
    {
        public Item(uint id, ModelId type, ushort amount, ushort xLoc, ushort yLoc, ushort color, uint? containerId)
        {
            Type = type;
            Id = id;
            Amount = amount;
            Color = (Color) color;
            ContainerId = containerId;
            Location = new Location3D(xLoc, yLoc, 0);
        }

        public Item(uint id, ModelId type, ushort amount, Location3D location, Color? color = null,
            uint? containerId = null, Layer? layer = null, Movement? orientation = null)
        {
            Id = id;
            Type = type;
            Amount = amount;
            Location = location;
            Color = color ?? default(Color);
            ContainerId = containerId;
            Layer = layer;
            Orientation = orientation;
        }

        public ushort GetDistance(Item item)
        {
            return GetDistance(item.Location);
        }

        public ushort GetDistance(Location3D secondLocation) => GetDistance((Location2D) secondLocation);

        public ushort GetDistance(Location2D secondLocation) => secondLocation.GetDistance((Location2D) Location);

        public bool IsOnGround => !Layer.HasValue && !ContainerId.HasValue;

        public ModelId Type { get; private set; }

        public uint Id { get; private set; }

        public ushort Amount { get; private set; }

        public Location3D Location { get; private set; }

        public uint? ContainerId { get; private set; }

        public Color Color { get; private set; }

        public Movement? Orientation { get; private set; }

        public Layer? Layer { get; private set; }

        public ushort CurrentHealth { get; private set; }

        public ushort MaxHealth { get; private set; }

        public override string ToString()
        {
            return (ContainerId.HasValue)
                ? $"Id: {Id:X8}, Type: {Type}; Amount: {Amount}; Location: {Location}; Container {ContainerId.Value:X8}"
                : $"Id: {Id:X8}, Type: {Type}; Amount: {Amount}; Location: {Location}";
        }

        private Item()
        {
            
        }

        internal Item Duplicate()
        {
            return new Item()
            {
                Color = this.Color,
                CurrentHealth = this.CurrentHealth,
                Location = this.Location,
                Id = this.Id,
                MaxHealth = this.MaxHealth,
                Type = this.Type,
                Orientation = this.Orientation,
                Amount = this.Amount,
                ContainerId = this.ContainerId,
                Layer = this.Layer,
            };
        }

        public Item UpdateHealth(ushort currentHealth, ushort maxHealth)
        {
            var updatedItem = Duplicate();
            updatedItem.CurrentHealth = currentHealth;
            updatedItem.MaxHealth = maxHealth;

            return updatedItem;
        }

        public Item Ignore()
        {
            var ignoredItem = Duplicate();
            ignoredItem.Ignored = true;

            return ignoredItem;
        }

        public bool Ignored { get; private set; }

        public Item UpdateLocation(Location3D location)
        {
            var updatedItem = Duplicate();
            updatedItem.Location = location;

            return updatedItem;
        }

        public Item Update(ushort amount, Location3D location, Color color, uint? containerId)
        {
            var updatedItem = Duplicate();
            updatedItem.Amount = amount;
            updatedItem.Location = location;
            updatedItem.Color = color;
            updatedItem.ContainerId = containerId;

            return updatedItem;
        }
    }
}