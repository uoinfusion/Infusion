using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infusion.Packets;
using Infusion.Strategies;
using Ultima;

namespace Infusion.Proxy.LegacyApi
{
    public class UltimaMap : IWorldMap
    {
        public bool IsPassable(Location2D start, Direction direction)
        {
            var target = start.LocationInDirection(direction);

            if (Legacy.Items.Where(i => (Location2D) i.Location == target)
                .Any(i => TileData.ItemTable[(ushort) i.Type].Impassable))
                return false;

            var tiles = Map.Felucca.Tiles.GetStaticTiles(target.X, target.Y);
            if (tiles.Length != 0)
            {
                if (tiles.Any(t => TileData.ItemTable[t.ID].Impassable))
                    return false;

                //bool bridge = tiles.Any(t =>
                //{
                //    var data = TileData.ItemTable[t.ID];
                //    return data.Bridge || data.Surface;
                //});

                //if (bridge)
                //    return true; // just override land checking
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
