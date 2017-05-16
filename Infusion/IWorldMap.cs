using Infusion.Packets;

namespace Infusion
{
    public interface IWorldMap
    {
        bool IsPassable(Location2D start, Direction direction);
    }
}