using Infusion.Packets;

namespace Infusion.LegacyApi
{
    public class CorpseCollection : GameObjectCollectionWrapper<Corpse>
    {
        internal CorpseCollection(GameObjectCollection gameObjects) : base(gameObjects)
        {
        }
    }
}