using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Infusion.Packets;

namespace Infusion.Proxy.LegacyApi
{
    public class ItemCollection : IEnumerable<Item>
    {
        internal Player Player { get; }

        private ImmutableDictionary<uint, Item> items = ImmutableDictionary<uint, Item>.Empty;

        public ItemCollection(Player player)
        {
            Player = player;
        }

        public Item this[uint id] => items[id];

        public IEnumerator<Item> GetEnumerator()
        {
            return items.Values.Where(i => !i.Ignored).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return items.Values.Where(i => !i.Ignored).GetEnumerator();
        }

        public bool TryGet(uint id, out Item item)
        {
            return items.TryGetValue(id, out item);
        }

        public Item Get(uint id)
        {
            Item item;
            if (items.TryGetValue(id, out item))
                return item;

            return null;
        }

        public Item RefreshItem(Item item)
        {
            Item newItemVersion;
            if (items.TryGetValue(item.Id, out newItemVersion))
            {
                return newItemVersion;
            }

            return null;
        }

        internal void UpdateItem(Item item)
        {
            AddItem(item);
        }

        internal void AddItem(Item item)
        {
            items = items.SetItem(item.Id, item);
        }

        internal void RemoveItem(uint id)
        {
            items = items.Remove(id);
        }

        internal void PurgeUnreachableItems(Location2D referencePosition, ushort reachableRange)
        {
            var itemIdsOutOfRange =
                this.MinDistance(referencePosition, reachableRange).OnGround().Select(i => i.Id).ToArray();
            items = items.RemoveRange(itemIdsOutOfRange);

            // to be perfectly correct, we would need to remove all nested orphaned containers as well, but this is good enough for now
            var orphanedItemIds =
                items.Values.Where(i => i.ContainerId.HasValue && i.ContainerId.Value != Player.PlayerId  && !items.ContainsKey(i.ContainerId.Value))
                    .Select(i => i.Id);

            items = items.RemoveRange(orphanedItemIds);
        }

        internal static ModelId[] ToModelIds(ushort[] types)
        {
            return types.Select(t => (ModelId) t).ToArray();
        }

        internal void AddItemRange(IEnumerable<Item> items)
        {
            foreach (var item in items)
            {
                AddItem(item);
            }
        }

        public override string ToString()
        {
            if (items.IsEmpty)
                return string.Empty;

            return items.Select(i => i.ToString()).Aggregate((l, r) => l + Environment.NewLine + r);
        }
    }
}