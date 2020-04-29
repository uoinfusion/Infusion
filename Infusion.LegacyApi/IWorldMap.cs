namespace Infusion.LegacyApi
{
    public interface IWorldMap
    {
        bool CanWalk(Location2D start, Direction direction);
        bool LineOfSight(Location3D start, Location3D target);
        TileCollection TilesAt(Location2D location);
        TileCollection TilesAt(int x, int y);
    }
}