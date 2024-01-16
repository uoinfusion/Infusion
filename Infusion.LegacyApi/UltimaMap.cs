using System;
using System.Linq;
using Ultima;

namespace Infusion.LegacyApi
{
    public class UltimaMap : IWorldMap
    {
        public bool IsPassable(Location2D start, Direction direction)
        {
            var target = start.LocationInDirection(direction);

            var targetLand = Map.Felucca.Tiles.GetLandTile(target.X, target.Y);
            var startLand = Map.Felucca.Tiles.GetLandTile(start.X, start.Y);
            if (Math.Abs(targetLand.Z - startLand.Z) > 15)
                return false;

            var tiles = Map.Felucca.Tiles.GetStaticTiles(target.X, target.Y);
            if (tiles.Length != 0)
            {
                var startSurfaceTiles = Map.Felucca.Tiles.GetStaticTiles(start.X, start.Y).Where(t => TileData.ItemTable[t.ID].Flags.HasFlag(TileFlag.Background));
                var surfaceZ = startSurfaceTiles.Any() ? startSurfaceTiles.First().Z : startLand.Z;
                var impassable = tiles.Any(t =>
                {
                    var item = TileData.ItemTable[t.ID];
                    return item.Impassable && t.Z >= surfaceZ && t.Z - item.Height <= surfaceZ;
                });
                if (impassable)
                    return false;
            }
            else
            {
                if (TileData.LandTable[targetLand.ID].Flags.HasFlag(TileFlag.Impassable))
                    return false;
            }

            var groundItems = UO.Items.OnGround().Where(i => i.Location == target && TileData.ItemTable[i.Type].Impassable);
            return !groundItems.Any();
        }
    }
}