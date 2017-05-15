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
        public bool IsPassable(Location2D location)
        {
            if (Legacy.Items.Where(i => (Location2D) i.Location == location)
                .Any(i => TileData.ItemTable[(ushort) i.Type].Impassable))
                return false;

            var tiles = Map.Felucca.Tiles.GetStaticTiles(location.X, location.Y);
            if (tiles.Length == 0)
                return true;

            if (tiles.Any(t => TileData.ItemTable[t.ID].Impassable))
                return false;

            return true;
        }
    }
}
