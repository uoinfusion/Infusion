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

        public IEnumerable<Item> FindTypeAll(ModelId type) => items.Values.Where(i => i.Type == type);

        public Item FindType(ModelId type) => FindTypeAll(type).FirstOrDefault();

        public Item FindType(params ModelId[] types) => FindTypeAll(types).FirstOrDefault();

        public Item FindTypeOnGround(params ModelId[] types) => FindTypeAll(types).FirstOrDefault(i => i.IsOnGround);

        public Item InContainer(Item container)
            => items.Values.First(i => i.ContainerId.HasValue && i.ContainerId.Value == container.Id);

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