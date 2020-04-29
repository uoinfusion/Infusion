using System;
using System.Collections.Generic;
using System.Linq;
using Ultima;

namespace Infusion.LegacyApi
{
    public class UltimaMap : IWorldMap
    {
        private readonly Map rawMap;

        public TileCollection TilesAt(Location2D location)
            => new TileCollection(rawMap.Tiles.GetStaticTiles(location.X, location.Y));
        public TileCollection TilesAt(int x, int y)
            => new TileCollection(rawMap.Tiles.GetStaticTiles(x, y));

        internal UltimaMap(Map rawMap)
        {
            this.rawMap = rawMap;
        }

        public bool CanWalk(Location2D start, Direction direction)
        {
            var target = start.LocationInDirection(direction);

            var tiles = rawMap.Tiles.GetStaticTiles(target.X, target.Y);
            if (tiles.Length != 0)
            {
                if (tiles.Any(t => TileData.ItemTable[t.ID].Impassable))
                    return false;
            }

            var targetLand = rawMap.Tiles.GetLandTile(target.X, target.Y);
            if (TileData.LandTable[targetLand.ID].Flags.HasFlag(TileFlag.Impassable))
                return false;

            var startLand = rawMap.Tiles.GetLandTile(start.X, start.Y);
            if (Math.Abs(targetLand.Z - startLand.Z) > 10)
                return false;

            return true;
        }

        private static int[] m_InvalidLandTiles = new int[] { 0x244 };

        public bool LineOfSight(Location3D org, Location3D dest)
        {
            var start = org;
            var end = dest;

            if (org.X > dest.X || (org.X == dest.X && org.Y > dest.Y) || (org.X == dest.X && org.Y == dest.Y && org.Z > dest.Z))
            {
                var swap = org;
                org = dest;
                dest = swap;
            }

            double rise, run, zslp;
            double sq3d;
            double x, y, z;
            int xd, yd, zd;
            int ix, iy, iz;
            int height;
            bool found;
            Location3D p;
            var path = new List<Location3D>();
            TileFlag flags;

            if (org == dest)
            {
                return true;
            }

            if (path.Count > 0)
            {
                path.Clear();
            }

            xd = dest.X - org.X;
            yd = dest.Y - org.Y;
            zd = dest.Z - org.Z;
            zslp = Math.Sqrt(xd * xd + yd * yd);
            if (zd != 0)
            {
                sq3d = Math.Sqrt(zslp * zslp + zd * zd);
            }
            else
            {
                sq3d = zslp;
            }

            rise = ((float)yd) / sq3d;
            run = ((float)xd) / sq3d;
            zslp = ((float)zd) / sq3d;
            y = org.Y;
            z = org.Z;
            x = org.X;
            while (NumberBetween(x, dest.X, org.X, 0.5) && NumberBetween(y, dest.Y, org.Y, 0.5) &&
                   NumberBetween(z, dest.Z, org.Z, 0.5))
            {
                ix = (int)Math.Round(x);
                iy = (int)Math.Round(y);
                iz = (int)Math.Round(z);
                if (path.Count > 0)
                {
                    p = path.Last();

                    if (p.X != ix || p.Y != iy || p.Z != iz)
                    {
                        path.Add(new Location3D(ix, iy, iz));
                    }
                }
                else
                {
                    path.Add(new Location3D(ix, iy, iz));
                }
                x += run;
                y += rise;
                z += zslp;
            }

            if (path.Count == 0)
            {
                return true; //<--should never happen, but to be safe.
            }

            p = path.Last();

            if (p != dest)
            {
                path.Add(dest);
            }

            Location3D pTop = org, pBottom = dest;
            FixPoints(ref pTop, ref pBottom);

            int pathCount = path.Count;
            int endTop = end.Z + 1;

            for (int i = 0; i < pathCount; ++i)
            {
                var point = path[i];
                int pointTop = point.Z + 1;

                var landTile = rawMap.Tiles.GetLandTile(point.X, point.Y);
                int landZ = 0, landAvg = 0, landTop = 0;
                GetAverageZ(point.X, point.Y, ref landZ, ref landAvg, ref landTop);

                if (landZ <= pointTop && landTop >= point.Z &&
                    (point.X != end.X || point.Y != end.Y || landZ > endTop || landTop < end.Z) && !landTile.IsIgnored())
                {
                    return false;
                }

                var statics = rawMap.Tiles.GetStaticTiles(point.X, point.Y, true);

                bool contains = false;
                int ltID = landTile.ID;

                for (int j = 0; !contains && j < m_InvalidLandTiles.Length; ++j)
                {
                    contains = (ltID == m_InvalidLandTiles[j]);
                }

                for (int j = 0; j < statics.Length; ++j)
                {
                    var t = statics[j];

                    var id = TileData.ItemTable[t.ID];

                    flags = id.Flags;
                    height = id.CalcHeight;

                    if (t.Z <= pointTop && t.Z + height >= point.Z && (flags & (TileFlag.Window | TileFlag.NoShoot)) != 0)
                    {
                        if (point.X == end.X && point.Y == end.Y && t.Z <= endTop && t.Z + height >= end.Z)
                        {
                            continue;
                        }

                        return false;
                    }
                }
            }
            
            return true;
        }

        public static bool NumberBetween(double num, int bound1, int bound2, double allowance)
        {
            if (bound1 > bound2)
            {
                int i = bound1;
                bound1 = bound2;
                bound2 = i;
            }

            return (num < bound2 + allowance && num > bound1 - allowance);
        }

        public static void FixPoints(ref Location3D top, ref Location3D bottom)
        {
            if (bottom.X < top.X)
            {
                int swap = top.X;
                top.X = bottom.X;
                bottom.X = swap;
            }

            if (bottom.Y < top.Y)
            {
                int swap = top.Y;
                top.Y = bottom.Y;
                bottom.Y = swap;
            }

            if (bottom.Z < top.Z)
            {
                int swap = top.Z;
                top.Z = bottom.Z;
                bottom.Z = swap;
            }
        }

        public int GetAverageZ(int x, int y)
        {
            int z = 0, avg = 0, top = 0;

            GetAverageZ(x, y, ref z, ref avg, ref top);

            return avg;
        }

        public void GetAverageZ(int x, int y, ref int z, ref int avg, ref int top)
        {
            int zTop = rawMap.Tiles.GetLandTile(x, y).Z;
            int zLeft = rawMap.Tiles.GetLandTile(x, y + 1).Z;
            int zRight = rawMap.Tiles.GetLandTile(x + 1, y).Z;
            int zBottom = rawMap.Tiles.GetLandTile(x + 1, y + 1).Z;

            z = zTop;
            if (zLeft < z)
            {
                z = zLeft;
            }
            if (zRight < z)
            {
                z = zRight;
            }
            if (zBottom < z)
            {
                z = zBottom;
            }

            top = zTop;
            if (zLeft > top)
            {
                top = zLeft;
            }
            if (zRight > top)
            {
                top = zRight;
            }
            if (zBottom > top)
            {
                top = zBottom;
            }

            if (Math.Abs(zTop - zBottom) > Math.Abs(zLeft - zRight))
            {
                avg = FloorAverage(zLeft, zRight);
            }
            else
            {
                avg = FloorAverage(zTop, zBottom);
            }
        }

        private static int FloorAverage(int a, int b)
        {
            int v = a + b;

            if (v < 0)
            {
                --v;
            }

            return (v / 2);
        }
    }

    internal static class UltimaMapExtensions
    {
        public static bool IsIgnored(this Ultima.Tile tile) { return (tile.ID == 2 || tile.ID == 0x1DB || (tile.ID >= 0x1AE && tile.ID <= 0x1B5)); } 

    }
}