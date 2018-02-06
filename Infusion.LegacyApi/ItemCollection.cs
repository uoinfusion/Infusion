using Infusion.Packets;

namespace Infusion.LegacyApi
{
    public class ItemCollection : GameObjectCollectionWrapper<Item>
    {
        internal ItemCollection(GameObjectCollection gameObjects) : base(gameObjects)
        {
        }
    }
}