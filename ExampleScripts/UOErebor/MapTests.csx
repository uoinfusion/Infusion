void AssertPassable(string name, int x, int y, Direction dir, bool expectedValue)
{
    if (expectedValue == UO.Map.IsPassable(new Location2D(x, y), dir))
        UO.Log($"Successs - {name}");
    else
    {
        var errorMessagePrefix = expectedValue ? "" : "im";
        var errorMessageSufix = expectedValue ? " not": "";
        UO.Console.Error($"Failed - {name} is expected to be {errorMessagePrefix}passable but it is{errorMessageSufix}");
    }
}

void AssertPassable(string name, int x, int y, Direction dir)
{
    AssertPassable(name, x, y, dir, true);
}

void AssertNotPassable(string name, int x, int y, Direction dir)
{
    AssertPassable(name, x, y, dir, false);
}

AssertNotPassable("Wall without ground under land tile", 752, 1524, Direction.West);
AssertNotPassable("Stalagnite with ground under land tile", 752, 1524, Direction.East);
AssertPassable("Free path with ground under land tile", 752, 1525, Direction.North);
AssertPassable("Log post with ground under land tile (server allows it)", 753, 1532, Direction.Southeast);
AssertNotPassable("No tile under land tile", 745, 1537, Direction.South);
AssertNotPassable("Land tile only", 783, 1540, Direction.North);
AssertPassable("Free way - south", 787, 1542, Direction.South);
AssertPassable("Free way - north", 787, 1542, Direction.Northeast);
AssertPassable("Free way - northwest", 787, 1542, Direction.Northwest);
AssertPassable("Free way - Southeast", 787, 1542, Direction.Southeast);
AssertPassable("Free way without ground", 787, 1542, Direction.North);
AssertNotPassable("tree on surface - south", 785, 1551, Direction.South);
AssertNotPassable("tree on surface - east", 784, 1552, Direction.East);
AssertNotPassable("tree on surface - north", 785, 1553, Direction.North);
AssertNotPassable("tree on surface - west", 786, 1552, Direction.West);
AssertPassable("tree on surface - Southwest (server allows it)", 786, 1552, Direction.Southwest);

AssertNotPassable("Hurka, broken", 1541, 1175, Direction.Northeast);

// Found path from 2810, 3191, 20 to 2794, 3202 is invalid.
AssertNotPassable("Hurka broken 2", 1537, 1185, Direction.Southwest);