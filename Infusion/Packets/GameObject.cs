namespace Infusion.Packets
{
    public abstract class GameObject
    {
        protected GameObject(uint id, ModelId type, Location3D location)
        {
            Id = id;
            Type = type;
            Location = location;
        }

        public ModelId Type { get; protected set; }

        public uint Id { get; }

        public string Name { get; private set; }

        protected abstract GameObject Duplicate();

        internal GameObject UpdateName(string name)
        {
            var updatedItem = Duplicate();

            updatedItem.Name = name;

            return updatedItem;
        }

        public abstract bool IsOnGround { get; }

        public ushort GetDistance(GameObject item)
        {
            return GetDistance(item.Location);
        }

        public ushort GetDistance(Location3D secondLocation)
        {
            return GetDistance((Location2D)secondLocation);
        }

        public ushort GetDistance(Location2D secondLocation)
        {
            return secondLocation.GetDistance(Location);
        }


        public Location3D Location { get; protected set; }
    }
}
