using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ultima;

namespace Infusion.LegacyApi
{
    public class TileCollection : IEnumerable<Tile>
    {
        private IEnumerable<HuedTile> rawStore;

        internal TileCollection(IEnumerable<HuedTile> tiles)
        {
            this.rawStore = tiles;
        }

        public TileCollection Matching(TileSpec spec)
            => new TileCollection(rawStore.Where(x => spec.Matches(x)));

        public TileCollection OfType(ModelId type)
            => new TileCollection(rawStore.Where(x => x.ID == type.Value));

        public TileCollection OfColor(Color color)
            => new TileCollection(rawStore.Where(x => x.Hue == color.Id));

        public TileCollection Height(int z)
            => new TileCollection(rawStore.Where(x => x.Z == z));

        public TileCollection MaxHeight(int z)
            => new TileCollection(rawStore.Where(x => x.Z <= z));

        public TileCollection MinHeight(int z)
            => new TileCollection(rawStore.Where(x => x.Z >= z));

        private IEnumerator<Tile> Enumerator
            => rawStore
                .Select(x => new Tile(x.ID, (sbyte)x.Z, (Color)x.Hue))
                .GetEnumerator();

        public IEnumerator<Tile> GetEnumerator() => Enumerator;
        IEnumerator IEnumerable.GetEnumerator() => Enumerator;
    }
}
