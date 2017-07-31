using Infusion.Packets;

namespace Infusion.LegacyApi
{
    public class ItemCollection : GameObjectCollectionWrapper<Item>
    {
        internal ItemCollection(GameObjectCollection gameObjects) : base(gameObjects)
        {
        }

        public Item Refresh(Item item)
        {
            if (TryGet(item.Id, out Item newItemVersion))
                return newItemVersion;

            return null;
        }
    }
}