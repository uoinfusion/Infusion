using System.Collections.Generic;
using System.Linq;
using UltimaRX.Packets;

namespace UltimaRX.Proxy.InjectionApi
{
    public static class ItemCollectionExtensions
    {
        public static IEnumerable<Item> OfType(this IEnumerable<Item> items, ModelId type)
            => items.Where(i => i.Type == type);

        public static IEnumerable<Item> OfType(this IEnumerable<Item> items, ModelId[] types)
            => items.Where(i => types.Contains(i.Type));

        public static IEnumerable<Item> OfColor(this IEnumerable<Item> items, Color color)
            => items.Where(i => i.Color == color);

        public static IEnumerable<Item> OnLayer(this IEnumerable<Item> items, Layer layer)
            => items.Where(i => i.Layer.HasValue && i.Layer.Value == layer);

        public static Item First(this IEnumerable<Item> items) => items.FirstOrDefault();

        public static IEnumerable<Item> InContainer(this IEnumerable<Item> items, Item container)
            => items.Where(i => i.ContainerId.HasValue && i.ContainerId.Value == container.Id);

        public static IEnumerable<Item> OnGround(this IEnumerable<Item> items)
            => items.Where(i => i.IsOnGround);

        public static IEnumerable<Item> OrderByDistance(this IEnumerable<Item> items, Location3D referenceLocation)
            => items.OrderBy(i => i.GetDistance(referenceLocation));

        public static ModelId[] ToModelIds(this ushort[] ids) => ids.Select(i => (ModelId) i).ToArray();
    }
}