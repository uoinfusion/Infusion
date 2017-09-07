using System.Collections.Generic;
using System.Linq;
using Infusion.Packets;

namespace Infusion.LegacyApi
{
    public static class ItemCollectionExtensions
    {
        public static IEnumerable<Item> Refresh(this IEnumerable<Item> items)
            => items.Select(i => UO.Items[i.Id]).Where(i => i != null);

        public static IEnumerable<Item> OfLayer(this IEnumerable<Item> items, Layer layer)
            => items.Where(i => i.Layer == layer);

        public static IEnumerable<Item> OfType(this IEnumerable<Item> items, ModelId type)
            => items.Where(i => i.Type == type);

        public static IEnumerable<Item> OfType(this IEnumerable<Item> items, ModelId[] types)
            => items.Where(i => types.Contains(i.Type));

        public static IEnumerable<Item> OfColor(this IEnumerable<Item> items, Color? color)
            => items.Where(i => i.Color == color);

        public static IEnumerable<Item> Matching(this IEnumerable<Item> items, ItemSpec spec)
            => items.Where(i => spec.Matches(i));

        public static IEnumerable<Item> NotMatching(this IEnumerable<Item> items, ItemSpec spec)
            => items.Where(i => !spec.Matches(i));

        public static IEnumerable<Item> OnLayer(this IEnumerable<Item> items, Layer layer) =>
            items.Where(i =>
                i.Layer.HasValue && i.Layer.Value == layer &&
                i.ContainerId.HasValue && i.ContainerId.Value == UO.Me.PlayerId);

        public static Item FirstOrDefault(this IEnumerable<Item> items) => Enumerable.FirstOrDefault(items);

        public static IEnumerable<Item> InContainer(this IEnumerable<Item> items, GameObject container)
            => items.Where(i => i.ContainerId.HasValue && i.ContainerId.Value == container.Id);

        public static IEnumerable<Item> InContainer(this IEnumerable<Item> items, ObjectId containerId)
            => items.Where(i => i.ContainerId.HasValue && i.ContainerId.Value == containerId);

        public static IEnumerable<Item> OnGround(this IEnumerable<Item> items)
            => items.Where(i => i.IsOnGround);

        public static IEnumerable<Item> OrderByDistance(this IEnumerable<Item> items)
            => items.OrderBy(i => i.GetDistance(UO.Me.Location));

        public static IEnumerable<Item> OrderByDistance(this IEnumerable<Item> items, Location3D referenceLocation)
            => items.OrderBy(i => i.GetDistance(referenceLocation));

        public static ModelId[] ToModelIds(this ushort[] ids) => ids.Select(i => (ModelId) i).ToArray();

        public static IEnumerable<Item> MaxDistance(this IEnumerable<Item> items, ushort maxDistance)
            => items.Where(i => i.GetDistance(UO.Me.Location) <= maxDistance);

        public static IEnumerable<Item> MaxDistance(this IEnumerable<Item> items, Location2D referenceLocation,
            ushort maxDistance) => items.Where(i => i.GetDistance(referenceLocation) <= maxDistance);

        public static IEnumerable<Item> MaxDistance(this IEnumerable<Item> items, Location3D referenceLocation,
            ushort maxDistance) => MaxDistance(items, (Location2D) referenceLocation, maxDistance);

        public static IEnumerable<Item> MinDistance(this IEnumerable<Item> items, Location2D referenceLocation,
            ushort minDistance) => items.Where(i => i.GetDistance(referenceLocation) >= minDistance);

        public static IEnumerable<Item> MinDistance(this IEnumerable<Item> items, ushort minDistance)
            => items.Where(i => i.GetDistance(UO.Me.Location) >= minDistance);
    }
}