namespace Infusion.Packets
{
    public sealed class Mobile : GameObject
    {
        public Mobile(ObjectId id, ModelId type, Location3D location, Color? color,
            Direction? orientation, MovementType? currentMovementType, Notoriety? notoriety)
            : base(id, type, location)
        {
            Color = color;
            Orientation = orientation;
            CurrentMovementType = currentMovementType;
            Notoriety = notoriety;
        }

        public Mobile(ObjectId id, ModelId type, Location3D location) : base(id, type, location)
        {
        }

        public Color? Color { get; private set; }
        public Direction? Orientation { get; private set; }
        public MovementType? CurrentMovementType { get; private set; }
        public Notoriety? Notoriety { get; private set; }

        public bool IsWalking => CurrentMovementType.HasValue && CurrentMovementType.Value == MovementType.Walk;
        public bool IsRunning => CurrentMovementType.HasValue && CurrentMovementType.Value == MovementType.Run;

        public override bool IsOnGround => true;

        public ushort CurrentHealth { get; private set; }

        public ushort MaxHealth { get; private set; }

        protected override GameObject Duplicate()
        {
            return new Mobile(Id, Type, Location)
            {
                Color = Color,
                CurrentHealth = CurrentHealth,
                MaxHealth = MaxHealth,
                Orientation = Orientation,
                Notoriety = Notoriety,
                Name = Name,
            };
        }

        internal Mobile UpdateHealth(ushort currentHealth, ushort maxHealth)
        {
            var updatedItem = (Mobile) Duplicate();
            updatedItem.CurrentHealth = currentHealth;
            updatedItem.MaxHealth = maxHealth;

            return updatedItem;
        }

        public Mobile Update(ModelId type, Location3D location, Color color, Direction? orientation, MovementType? currentMovementType, Notoriety? notoriety)
        {
            var updatedMobile = (Mobile) Duplicate();
            updatedMobile.Location = location;
            updatedMobile.Type = type;
            updatedMobile.Color = color;
            updatedMobile.Orientation = orientation;
            updatedMobile.CurrentMovementType = currentMovementType;
            updatedMobile.Notoriety = notoriety;

            return updatedMobile;
        }

        public override string ToString()
        {
            return
                $"Id: {Id}; Type: {Type}; Name: {Name}; Location: {Location}; Color: {Color}; Orientation: {Orientation}; MovmentType: {CurrentMovementType} Notiriety: {Notoriety}";
        }
    }
}