using System.Collections.Generic;
using System.Linq;
using Infusion.Packets;

namespace Infusion.LegacyApi
{
    public static class MobileCollectionExtensions
    {
        public static IEnumerable<Mobile> MaxDistance(this IEnumerable<Mobile> mobiles, Location2D referenceLocation,
            ushort maxDistance) => mobiles.Where(i => i.GetDistance(referenceLocation) <= maxDistance);

        public static IEnumerable<Mobile> MaxDistance(this IEnumerable<Mobile> mobiles, Location3D referenceLocation,
            ushort maxDistance) => MaxDistance(mobiles, (Location2D) referenceLocation, maxDistance);

        public static IEnumerable<Mobile> MinDistance(this IEnumerable<Mobile> mobiles, Location2D referenceLocation,
            ushort minDistance) => mobiles.Where(i => i.GetDistance(referenceLocation) >= minDistance);

        public static IEnumerable<Mobile> MinDistance(this IEnumerable<Mobile> mobiles, ushort minDistance)
            => mobiles.Where(i => i.GetDistance(UO.Me.Location) >= minDistance);

        public static IEnumerable<Mobile> Refresh(this IEnumerable<Mobile> mobiles)
            => mobiles.Select(i => UO.Mobiles[i.Id]).Where(i => i != null);

        public static IEnumerable<Mobile> OfType(this IEnumerable<Mobile> mobiles, ModelId type)
            => mobiles.Where(i => i.Type == type);

        public static IEnumerable<Mobile> OfType(this IEnumerable<Mobile> mobiles, ModelId[] types)
            => mobiles.Where(i => types.Contains(i.Type));

        public static IEnumerable<Mobile> OfColor(this IEnumerable<Mobile> mobiles, Color? color)
            => mobiles.Where(i => i.Color == color);

        public static IEnumerable<Mobile> OrderByDistance(this IEnumerable<Mobile> mobiles)
            => mobiles.OrderBy(i => i.GetDistance(UO.Me.Location));

        public static IEnumerable<Mobile> OrderByDistance(this IEnumerable<Mobile> mobiles,
            Location3D referenceLocation)
            => mobiles.OrderBy(i => i.GetDistance(referenceLocation));

        public static IEnumerable<Mobile> MaxDistance(this IEnumerable<Mobile> mobiles, ushort maxDistance)
            => mobiles.Where(i => i.GetDistance(UO.Me.Location) <= maxDistance);

        public static IEnumerable<Mobile> NotMatching(this IEnumerable<Mobile> items, MobileSpec spec)
            => items.Where(i => !spec.Matches(i));
        public static IEnumerable<Mobile> Matching(this IEnumerable<Mobile> items, MobileSpec spec)
            => items.Where(i => spec.Matches(i));

        public static Mobile FirstOrDefault(this IEnumerable<Mobile> mobiles) => Enumerable.FirstOrDefault(mobiles);

    }
}