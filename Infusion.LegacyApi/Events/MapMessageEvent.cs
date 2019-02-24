using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.LegacyApi.Events
{
    public sealed class MapMessageEvent : IEvent
    {
        public Location2D UpperLeft { get; }
        public Location2D LowerRight { get; }
        public int Width { get; }
        public int Height { get; }

        internal MapMessageEvent(Location2D upperLeft, Location2D lowerRight, int width, int height)
        {
            UpperLeft = upperLeft;
            LowerRight = lowerRight;
            Width = width;
            Height = height;
        }
    }
}
