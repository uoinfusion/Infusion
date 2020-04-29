using System;
using System.Collections.Generic;
using System.Text;

namespace Infusion
{
    public struct Tile
    {
        public Tile(ModelId type, sbyte z, Color color)
        {
            Type = type;
            Z = z;
            Color = color;
        }

        public Color Color { get; }
        public ModelId Type { get; set; }
        public sbyte Z { get; }
    }
}
