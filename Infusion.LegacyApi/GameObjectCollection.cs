using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Infusion.LegacyApi
{
    internal class GameObjectCollection : IEnumerable<GameObject>
    {
        private ImmutableDictionary<ObjectId, GameObject> gameObjects = ImmutableDictionary<ObjectId, GameObject>.Empty;

        public GameObjectCollection(Player player)
        {
            Player = player;
        }

        internal Player Player { get; }

        public GameObject this[ObjectId id]
        {
            get
            {
                if (gameObjects.TryGetValue(id, out var gameObject))
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
            if (gameObjects.TryGetValue(id, out var gameObject))
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
            if (gameObjects.TryGetValue(id, out var gameObject))
            {
                gameObjects = gameObjects.Remove(id);
                OnGameObjectDeleted(gameObject);
            }
        }

        private void OnItemsLeftView(IEnumerable<GameObject> gameObjects)
        {
            var mobileLeftViewHandler = MobileLeftView;

            if (mobileLeftViewHandler != null)
            {
                foreach (var obj in gameObjects)
                {
                    if (obj is Mobile mobile)
                        mobileLeftViewHandler.Invoke(this, mobile);
                }
            }
        }

        private void OnGameObjectDeleted(GameObject gameObject)
        {
            var mobileDeletedHandler = MobileDeleted;
            if (mobileDeletedHandler != null && gameObject is Mobile mobile)
            {
                mobileDeletedHandler.Invoke(this, mobile);
            }
        }

        internal event EventHandler<Mobile> MobileLeftView;
        internal event EventHandler<Mobile> MobileDeleted;

        internal void PurgeUnreachableItems(Location2D referenceLocation, ushort reachableRange)
        {
            var itemsOutOfRange =
                gameObjects.Values.Where(obj => obj.GetDistance(referenceLocation) >= reachableRange && obj.IsOnGround)
                    .ToArray();
            OnItemsLeftView(itemsOutOfRange);
            gameObjects = gameObjects.RemoveRange(itemsOutOfRange.Select(x => x.Id));

            // to be perfectly correct, we would need to remove all nested orphaned containers as well, but this is good enough for now
            var orphanedItemIds =
                gameObjects.Values.OfType<Item>().Where(i =>
                        i.ContainerId.HasValue && i.ContainerId.Value != Player.PlayerId &&
                        !gameObjects.ContainsKey(i.ContainerId.Value))
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