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

            var tiles = Map.Felucca.Tiles.GetStaticTiles(target.X, target.Y);
            if (tiles.Length != 0)
            {
                if (tiles.Any(t => TileData.ItemTable[t.ID].Impassable))
                    return false;
            }

            var targetLand = Map.Felucca.Tiles.GetLandTile(target.X, target.Y);
            if (TileData.LandTable[targetLand.ID].Flags.HasFlag(TileFlag.Impassable))
                return false;

            var startLand = Map.Felucca.Tiles.GetLandTile(start.X, start.Y);
            if (Math.Abs(targetLand.Z - startLand.Z) > 10)
                return false;

            return true;
        }
    }
}