using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infusion.LegacyApi
{
    public static class GameObjectCollectionExtensions
    {
        public static IEnumerable<GameObject> OfType(this IEnumerable<GameObject> objects, ModelId type)
            => objects.Where(o => o.Type == type);

        public static IEnumerable<GameObject> OfType(this IEnumerable<GameObject> objects, ModelId[] types)
            => objects.Where(o => types.Contains(o.Type));

        public static IEnumerable<GameObject> OfColor(this IEnumerable<GameObject> gameObjects, Color? color)
            => gameObjects.Where(o =>
            {
                if (o is Item i)
                    return i.Color == color;
                if (o is Mobile m)
                    return m.Color == color;

                return false;
            });

        public static IEnumerable<GameObject> OnGround(this IEnumerable<GameObject> objects)
            => objects.Where(o => o.IsOnGround);

        public static IEnumerable<GameObject> OrderByDistance(this IEnumerable<GameObject> objects)
            => objects.OrderBy(o => o.GetDistance(UO.Me.Location));

        public static IEnumerable<GameObject> OrderByDistance(this IEnumerable<GameObject> objects, Location3D referenceLocation)
            => objects.OrderBy(o => o.GetDistance(referenceLocation));

        public static IEnumerable<GameObject> MaxDistance(this IEnumerable<GameObject> objects, ushort maxDistance)
            => objects.Where(o => o.GetDistance(UO.Me.Location) <= maxDistance);

        public static IEnumerable<GameObject> MaxDistance(this IEnumerable<GameObject> objects, Location2D referenceLocation, ushort maxDistance)
            => objects.Where(o => o.GetDistance(referenceLocation) <= maxDistance);

        public static IEnumerable<GameObject> MaxDistance(this IEnumerable<GameObject> objects, Location3D referenceLocation,
            ushort maxDistance) => MaxDistance(objects, (Location2D)referenceLocation, maxDistance);

        public static IEnumerable<GameObject> MinDistance(this IEnumerable<GameObject> objects, Location2D referenceLocation,
            ushort minDistance) => objects.Where(o => o.GetDistance(referenceLocation) >= minDistance);

        public static IEnumerable<GameObject> MinDistance(this IEnumerable<GameObject> objects, ushort minDistance)
            => objects.Where(o => o.GetDistance(UO.Me.Location) >= minDistance);


    }
}
