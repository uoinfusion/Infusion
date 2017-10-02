namespace Infusion
{
    public sealed class Item : GameObject
    {
        public Item(ObjectId id, ModelId type, ushort amount, Location3D location, Color? color, ObjectId? containerId, Layer? layer)
            : base(id, type, location)
        {
            Amount = amount;
            Color = color;
            ContainerId = containerId;
            Layer = layer;
        }

        private Item(ObjectId id, ModelId type, Location3D location) : base(id, type, location)
        {
        }

        public override bool IsOnGround => !Layer.HasValue && !ContainerId.HasValue;

        public ushort Amount { get; private set; }

        public ObjectId? ContainerId { get; private set; }

        public Color? Color { get; private set; }

        public Layer? Layer { get; private set; }

        public override string ToString()
        {
            string canModifyNameText = CanRename ? " (modifiable)" : string.Empty;

            return
                $"Id: {Id}; Type: {Type}; Name: {Name}{canModifyNameText}; Amount: {Amount}; Location: {Location}; Color: {Color}; Container {ContainerId}; Layer: {Layer}";
        }

        protected override GameObject Duplicate()
        {
            return new Item(Id, Type, Location)
            {
                Color = Color,
                Amount = Amount,
                ContainerId = ContainerId,
                Layer = Layer,
                Name = Name,
                CanRename =  CanRename,
            };
        }

        public Item Update(ModelId type, ushort amount, Location3D location, Color? color, ObjectId? containerId)
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