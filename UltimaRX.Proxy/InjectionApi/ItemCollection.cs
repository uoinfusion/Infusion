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

        public IEnumerable<Item> FindTypeAll(ushort type) => FindTypeAll((ModelId) type);

        public IEnumerable<Item> FindTypeAll(ModelId type) => items.Values.Where(i => i.Type == type);

        public Item FindType(ushort type) => FindType((ModelId) type);

        public Item FindType(ModelId type) => FindTypeAll(type).FirstOrDefault();

        public Item FindType(params ushort[] types) => FindType(ToModelIds(types));

        public Item FindType(params ModelId[] types) => FindTypeAll(types).FirstOrDefault();

        public Item FindTypeOnGround(params ushort[] types) => FindTypeOnGround(ToModelIds(types));

        public Item FindTypeOnGround(params ModelId[] types) => FindTypeAll(types).FirstOrDefault(i => i.IsOnGround);

        public Item InContainer(Item container)
            => items.Values.FirstOrDefault(i => i.ContainerId.HasValue && i.ContainerId.Value == container.Id);

        public IEnumerable<Item> FindTypeAll(params ushort[] types) => FindTypeAll(ToModelIds(types));

        internal static ModelId[] ToModelIds(ushort[] types)
        {
            return types.Select(t => (ModelId) t).ToArray();
        }

        public IEnumerable<Item> FindTypeAll(params ModelId[] types) => items.Values.Where(i => types.Contains(i.Type));

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