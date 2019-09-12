using System;
using System.Collections.Generic;
using System.Text;

namespace Infusion
{
    public struct MapBoundary
    {
        public ushort MinX { get; }
        public ushort MinY { get; }
        public ushort MaxX { get;}
        public ushort MaxY { get; }

        public MapBoundary(ushort minX, ushort minY, ushort maxX, ushort maxY)
        {
            MinX = minX;
            MinY = minY;
            MaxX = maxX;
            MaxY = maxY;
        }
    }
}
