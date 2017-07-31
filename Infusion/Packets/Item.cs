namespace Infusion.Packets
{
    public sealed class Item : GameObject
    {
        public Item(uint id, ModelId type, ushort amount, Location3D location, Color? color, uint? containerId, Layer? layer)
            : base(id, type, location)
        {
            Amount = amount;
            Color = color;
            ContainerId = containerId;
            Layer = layer;
        }

        private Item(uint id, ModelId type, Location3D location) : base(id, type, location)
        {
        }

        public override bool IsOnGround => !Layer.HasValue && !ContainerId.HasValue;

        public ushort Amount { get; private set; }

        public uint? ContainerId { get; private set; }

        public Color? Color { get; private set; }

        public Layer? Layer { get; private set; }

        public override string ToString()
        {
            return
                $"Id: {Id:X8}; Type: {Type}; Name: {Name}; Amount: {Amount}; Location: {Location}; Color: {Color}; Container {ContainerId:X8}; Layer: {Layer}";
        }

        protected override GameObject Duplicate()
        {
            return new Item(Id, Type, Location)
            {
                Color = Color,
                Amount = Amount,
                ContainerId = ContainerId,
                Layer = Layer,
            };
        }

        public Item Update(ModelId type, ushort amount, Location3D location, Color? color, uint? containerId)
        {
            var updatedItem = (Item)Duplicate();
            updatedItem.Location = location;
            updatedItem.Type = type;
            updatedItem.Amount = amount;
            updatedItem.Color = color;
            updatedItem.ContainerId = containerId;

            return updatedItem;
        }
    }
}