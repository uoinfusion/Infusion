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
            uint? containerId = null, Layer? layer = null, Movement? orientation = null, Notoriety? notoriety = null, string name = null)
        {
            Id = id;
            Type = type;
            Amount = amount;
            Location = location;
            Color = color ?? default(Color);
            ContainerId = containerId;
            Layer = layer;
            Orientation = orientation;
            Notoriety = notoriety;
            Name = name;
        }

        private Item()
        {
        }

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

        public Notoriety? Notoriety { get; set; }

        public string Name { get; private set; }

        public ushort GetDistance(Item item)
        {
            return GetDistance(item.Location);
        }

        public ushort GetDistance(Location3D secondLocation)
        {
            return GetDistance((Location2D) secondLocation);
        }

        public ushort GetDistance(Location2D secondLocation)
        {
            return secondLocation.GetDistance(Location);
        }

        public override string ToString()
        {
            return
                $"Id: {Id:X8}; Type: {Type}; Name: {Name}; Amount: {Amount}; Location: {Location}; Color: {Color}; Container {ContainerId:X8}; Notoriety: {Notoriety}; Orientation: {Orientation}; Layer: {Layer}";
        }

        internal Item Duplicate()
        {
            return new Item
            {
                Notoriety = Notoriety,
                Color = Color,
                CurrentHealth = CurrentHealth,
                Location = Location,
                Id = Id,
                MaxHealth = MaxHealth,
                Type = Type,
                Orientation = Orientation,
                Amount = Amount,
                ContainerId = ContainerId,
                Layer = Layer,
                Name = Name
            };
        }

        public Item UpdateHealth(ushort currentHealth, ushort maxHealth)
        {
            var updatedItem = Duplicate();
            updatedItem.CurrentHealth = currentHealth;
            updatedItem.MaxHealth = maxHealth;

            return updatedItem;
        }

        public Item UpdateName(string name)
        {
            var updatedItem = Duplicate();

            updatedItem.Name = name;

            return updatedItem;
        }

        public Item UpdateLocation(Location3D location)
        {
            var updatedItem = Duplicate();
            updatedItem.Location = location;

            return updatedItem;
        }

        public Item Update(ModelId type, ushort amount, Location3D location, Color color, uint? containerId,
            Notoriety? notoriety = null)
        {
            var updatedItem = Duplicate();
            updatedItem.Type = type;
            updatedItem.Amount = amount;
            updatedItem.Location = location;
            updatedItem.Color = color;
            updatedItem.ContainerId = containerId;

            if (notoriety.HasValue)
                updatedItem.Notoriety = notoriety;

            return updatedItem;
        }
    }
}