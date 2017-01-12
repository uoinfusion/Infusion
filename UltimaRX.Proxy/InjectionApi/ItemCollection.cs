using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using UltimaRX.Packets;

namespace UltimaRX.Proxy.InjectionApi
{
    public class ItemCollection : IEnumerable<Item>
    {
        private ImmutableDictionary<uint, Item> items = ImmutableDictionary<uint, Item>.Empty;

        public Item this[uint id] => items[id];

        public bool TryGet(uint id, out Item item)
        {
            return items.TryGetValue(id, out item);
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

        public IEnumerator<Item> GetEnumerator()
        {
            return items.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return items.Values.GetEnumerator();
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