using Infusion.Packets;

namespace Infusion.LegacyApi
{
    public class MobileCollection : GameObjectCollectionWrapper<Mobile>
    {
        internal MobileCollection(GameObjectCollection gameObjects) : base(gameObjects)
        {
        }

        public Mobile Refresh(Mobile item)
        {
            if (TryGet(item.Id, out Mobile newItemVersion))
                return newItemVersion;

            return null;
        }
    }
}