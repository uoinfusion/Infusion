using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using UltimaRX.Packets;

namespace UltimaRX.Proxy
{
    public class ItemCollection : IEnumerable<Item>
    {
        private ImmutableDictionary<int, Item> items = ImmutableDictionary<int, Item>.Empty;

        internal void AddItem(Item item)
        {
            items = items.SetItem(item.Id, item);
        }

        internal void RemoveItem(int id)
        {
            items = items.Remove(id);
        }

        public IEnumerable<Item> FindTypeAll(ushort type) => items.Values.Where(i => i.Type == type);

        public Item FindType(ushort type) => FindTypeAll(type).FirstOrDefault();

        public Item FindType(ushort[] types) => FindTypeAll(types).FirstOrDefault();

        public IEnumerable<Item> FindTypeAll(ushort[] types) => items.Values.Where(i => types.Contains(i.Type));

        internal void AddItemRange(IEnumerable<Item> items)
        {
            foreach (var item in items)
            {
                AddItem(item);
            }
        }

        public IEnumerator<Item> GetEnumerator()
        {
            return items.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return items.Values.GetEnumerator();
        }

        public override string ToString()
        {
            return items.Select(i => i.ToString()).Aggregate((l, r) => l + Environment.NewLine + r);
        }
    }
}