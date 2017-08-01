using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Infusion.Packets;

namespace Infusion.LegacyApi
{
    internal class GameObjectCollection : IEnumerable<GameObject>
    {
        internal Player Player { get; }

        private ImmutableDictionary<ObjectId, GameObject> gameObjects = ImmutableDictionary<ObjectId, GameObject>.Empty;

        public GameObjectCollection(Player player)
        {
            Player = player;
        }

        public GameObject this[ObjectId id]
        {
            get
            {
                if (gameObjects.TryGetValue(id, out GameObject gameObject))
                    return gameObject;

                return null;
            }
        }

        public IEnumerator<GameObject> GetEnumerator()
        {
            return gameObjects.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return gameObjects.Values.GetEnumerator();
        }

        public bool TryGet(ObjectId id, out GameObject gameObject)
        {
            return gameObjects.TryGetValue(id, out gameObject);
        }

        public GameObject Get(ObjectId id)
        {
            if (gameObjects.TryGetValue(id, out GameObject gameObject))
                return gameObject;

            return null;
        }

        public GameObject RefreshObject(GameObject item)
        {
            GameObject newItemVersion;
            if (gameObjects.TryGetValue(item.Id, out newItemVersion))
            {
                return newItemVersion;
            }

            return null;
        }

        public void UpdateObject(GameObject gameObject)
        {
            AddObject(gameObject);
        }

        internal void AddObject(GameObject gameObject)
        {
            gameObjects = gameObjects.SetItem(gameObject.Id, gameObject);
        }

        internal void RemoveItem(ObjectId id)
        {
            gameObjects = gameObjects.Remove(id);
        }

        internal void PurgeUnreachableItems(Location2D referenceLocation, ushort reachableRange)
        {
            var itemIdsOutOfRange =
                gameObjects.Values.Where(obj => obj.GetDistance(referenceLocation) >= reachableRange && obj.IsOnGround).Select(i => i.Id).ToArray();
            gameObjects = gameObjects.RemoveRange(itemIdsOutOfRange);

            // to be perfectly correct, we would need to remove all nested orphaned containers as well, but this is good enough for now
            var orphanedItemIds =
                gameObjects.Values.OfType<Item>().Where(i => i.ContainerId.HasValue && i.ContainerId.Value != Player.PlayerId  && !gameObjects.ContainsKey(i.ContainerId.Value))
                    .Select(i => i.Id);

            gameObjects = gameObjects.RemoveRange(orphanedItemIds);
        }

        internal static ModelId[] ToModelIds(ushort[] types)
        {
            return types.Select(t => (ModelId) t).ToArray();
        }

        internal void AddItemRange(IEnumerable<Item> items)
        {
            foreach (var item in items)
            {
                AddObject(item);
            }
        }

        public override string ToString()
        {
            if (gameObjects.IsEmpty)
                return string.Empty;

            return gameObjects.Select(i => i.ToString()).Aggregate((l, r) => l + Environment.NewLine + r);
        }
    }
}