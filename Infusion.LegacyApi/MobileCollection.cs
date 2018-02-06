using Infusion.Packets;

namespace Infusion.LegacyApi
{
    public class MobileCollection : GameObjectCollectionWrapper<Mobile>
    {
        internal MobileCollection(GameObjectCollection gameObjects) : base(gameObjects)
        {
        }
    }
}