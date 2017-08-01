using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Infusion.Packets;

namespace Infusion.LegacyApi
{
    public class GameObjectCollectionWrapper<T> : IEnumerable<T> 
        where T : GameObject
        
    {
        private readonly GameObjectCollection gameObjects;

        internal GameObjectCollectionWrapper(GameObjectCollection gameObjects)
        {
            this.gameObjects = gameObjects;
        }

        public T this[ObjectId id]
        {
            get
            {
                if (gameObjects.TryGet(id, out GameObject gameObject))
                    return gameObject as T;

                return null;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return gameObjects.OfType<T>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return gameObjects.OfType<T>().GetEnumerator();
        }

        public bool TryGet(ObjectId id, out T item)
        {
            if (gameObjects.TryGet(id, out GameObject gameObject))
            {
                if (gameObject is T result)
                {
                    item = result;
                    return true;
                }
            }

            item = null;
            return false;
        }

        public T Get(ObjectId id)
        {
            if (gameObjects.TryGet(id, out GameObject gameObject))
            {
                if (gameObject is T item)
                    return item;
            }

            throw new InvalidOperationException($"Item {id} not found.");
        }

        public override string ToString()
        {
            if (!gameObjects.Any())
                return string.Empty;

            return gameObjects.OfType<T>().Select(i => i.ToString()).Aggregate((l, r) => l + Environment.NewLine + r);
        }
    }
}