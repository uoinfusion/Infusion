using Infusion.Packets;

namespace Infusion.LegacyApi
{
    public class MobileCollection : GameObjectCollectionWrapper<Mobile>
    {
        internal MobileCollection(GameObjectCollection gameObjects) : base(gameObjects)
        {
        }

        public Mobile Refresh(Mobile mobile)
        {
            if (mobile == null)
                return null;

            if (TryGet(mobile.Id, out Mobile newItemVersion))
                return newItemVersion;

            return null;
        }
    }
}