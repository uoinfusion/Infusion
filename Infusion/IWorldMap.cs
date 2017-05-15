using Infusion.Packets;

namespace Infusion
{
    public interface IWorldMap
    {
        bool IsPassable(Location2D location);
    }
}