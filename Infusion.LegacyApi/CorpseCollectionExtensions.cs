using System.Collections.Generic;
using System.Linq;
using Infusion.Packets;

namespace Infusion.LegacyApi
{
    public static class CorpseCollectionExtensions
    {
        public static IEnumerable<Corpse> Refresh(this IEnumerable<Corpse> items)
            => items.Select(i => UO.Corpses[i.Id]).Where(i => i != null);

        public static IEnumerable<Corpse> OfLayer(this IEnumerable<Corpse> items, Layer layer)
            => items.Where(i => i.Layer == layer);

        public static IEnumerable<Corpse> OfType(this IEnumerable<Corpse> items, ModelId type)
            => items.Where(i => i.Type == type);

        public static IEnumerable<Corpse> OfType(this IEnumerable<Corpse> items, ModelId[] types)
            => items.Where(i => types.Contains(i.Type));

        public static IEnumerable<Corpse> OfColor(this IEnumerable<Corpse> items, Color? color)
            => items.Where(i => i.Color == color);

        public static IEnumerable<Corpse> Matching(this IEnumerable<Corpse> items, ItemSpec spec)
            => items.Where(i => spec.Matches(i));

        public static IEnumerable<Corpse> NotMatching(this IEnumerable<Corpse> items, ItemSpec spec)
            => items.Where(i => !spec.Matches(i));

        public static Item FirstOrDefault(this IEnumerable<Corpse> items) => Enumerable.FirstOrDefault(items);

        public static IEnumerable<Corpse> OrderByDistance(this IEnumerable<Corpse> items)
            => items.OrderBy(i => i.GetDistance(UO.Me.Location));

        public static IEnumerable<Corpse> OrderByDistance(this IEnumerable<Corpse> items, Location3D referenceLocation)
            => items.OrderBy(i => i.GetDistance(referenceLocation));

        public static ModelId[] ToModelIds(this ushort[] ids) => ids.Select(i => (ModelId) i).ToArray();

        public static IEnumerable<Corpse> MaxDistance(this IEnumerable<Corpse> items, ushort maxDistance)
            => items.Where(i => i.GetDistance(UO.Me.Location) <= maxDistance);

        public static IEnumerable<Corpse> MaxDistance(this IEnumerable<Corpse> items, Location2D referenceLocation,
            ushort maxDistance) => items.Where(i => i.GetDistance(referenceLocation) <= maxDistance);

        public static IEnumerable<Corpse> MaxDistance(this IEnumerable<Corpse> items, Location3D referenceLocation,
            ushort maxDistance) => MaxDistance(items, (Location2D) referenceLocation, maxDistance);

        public static IEnumerable<Corpse> MinDistance(this IEnumerable<Corpse> items, Location2D referenceLocation,
            ushort minDistance) => items.Where(i => i.GetDistance(referenceLocation) >= minDistance);

        public static IEnumerable<Corpse> MinDistance(this IEnumerable<Corpse> items, ushort minDistance)
            => items.Where(i => i.GetDistance(UO.Me.Location) >= minDistance);
    }
}