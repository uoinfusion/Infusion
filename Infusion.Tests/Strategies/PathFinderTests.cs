using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Infusion.Packets;
using Infusion.Strategies;
using Microsoft.VisualStudio.TestTools.UnitTesting;

// ReSharper disable PossibleInvalidOperationException

namespace Infusion.Tests.Strategies
{
    [TestClass]
    public class PathFinderTests
    {
        [TestMethod]
        public void Can_find_direct_north_path()
        {
            var map = new TestMap(new[]
            {
                ".T.",
                ".S.",
                "...",
            });

            var pathFinder = new PathFinder(map);

            var actualPath = pathFinder.FindPath(map.StartLocation.Value, map.TargetLocation.Value);
            AssertPath(actualPath, 1, map);
        }

        [TestMethod]
        public void Can_find_direct_northwest_path()
        {
            var map = new TestMap(new[]
            {
                "T..",
                ".S.",
                "...",
            });

            var pathFinder = new PathFinder(map);

            var actualPath = pathFinder.FindPath(map.StartLocation.Value, map.TargetLocation.Value);
            AssertPath(actualPath, 1, map);
        }

        [TestMethod]
        public void Can_find_direct_west()
        {
            var map = new TestMap(new[]
            {
                "...",
                "TS.",
                "...",
            });

            var pathFinder = new PathFinder(map);

            var actualPath = pathFinder.FindPath(map.StartLocation.Value, map.TargetLocation.Value);
            AssertPath(actualPath, 1, map);
        }

        [TestMethod]
        public void Can_find_direct_southwest_path()
        {
            var map = new TestMap(new[]
            {
                "...",
                ".S.",
                "T..",
            });

            var pathFinder = new PathFinder(map);

            var actualPath = pathFinder.FindPath(map.StartLocation.Value, map.TargetLocation.Value);
            AssertPath(actualPath, 1, map);
        }


        [TestMethod]
        public void Can_find_direct_south_path()
        {
            var map = new TestMap(new[]
            {
                "...",
                ".S.",
                ".T.",
            });

            var pathFinder = new PathFinder(map);

            var actualPath = pathFinder.FindPath(map.StartLocation.Value, map.TargetLocation.Value);
            AssertPath(actualPath, 1, map);
        }

        [TestMethod]
        public void Can_find_direct_southeast_path()
        {
            var map = new TestMap(new[]
            {
                "...",
                ".S.",
                "..T",
            });

            var pathFinder = new PathFinder(map);

            var actualPath = pathFinder.FindPath(map.StartLocation.Value, map.TargetLocation.Value);
            AssertPath(actualPath, 1, map);
        }

        [TestMethod]
        public void Can_find_direct_east_path()
        {
            var map = new TestMap(new[]
            {
                "...",
                ".ST",
                "...",
            });

            var pathFinder = new PathFinder(map);

            var actualPath = pathFinder.FindPath(map.StartLocation.Value, map.TargetLocation.Value);
            AssertPath(actualPath, 1, map);
        }

        [TestMethod]
        public void Can_find_direct_northeast_path()
        {
            var map = new TestMap(new[]
            {
                "..T",
                ".S.",
                "...",
            });

            var pathFinder = new PathFinder(map);

            var actualPath = pathFinder.FindPath(map.StartLocation.Value, map.TargetLocation.Value);
            AssertPath(actualPath, 1, map);
        }

        [TestMethod]
        public void Can_find_direct_diagonal_multistep_path()
        {
            var map = new TestMap(new[]
            {
                "S.......",
                "........",
                ".......T",
            });

            var pathFinder = new PathFinder(map);

            var actualPath = pathFinder.FindPath(map.StartLocation.Value, map.TargetLocation.Value);
            AssertPath(actualPath, 7, map);
        }

        [TestMethod]
        public void Can_find_path_constrained_by_vertical_wall()
        {
            var map = new TestMap(new[]
            {
                "S#....",
                ".#....",
                ".#....",
                ".....T",
            });

            var pathFinder = new PathFinder(map);

            var actualPath = pathFinder.FindPath(map.StartLocation.Value, map.TargetLocation.Value);
            AssertPath(actualPath, 8, map);
        }

        [TestMethod]
        public void Can_find_path_constrained_by_horizontal_wall()
        {
            var map = new TestMap(new[]
            {
                "S.....",
                "#####.",
                "......",
                ".....T",
            });

            var pathFinder = new PathFinder(map);

            var actualPath = pathFinder.FindPath(map.StartLocation.Value, map.TargetLocation.Value);
            AssertPath(actualPath, 8, map);
        }

        [TestMethod]
        public void Can_return_null_when_start_on_island()
        {
            var map = new TestMap(new[]
            {
                "S#....",
                "##....",
                "......",
                ".....T",
            });

            var pathFinder = new PathFinder(map);

            var actualPath = pathFinder.FindPath(map.StartLocation.Value, map.TargetLocation.Value);
            actualPath.Should().BeNull();
        }

        [TestMethod]
        public void Can_return_null_when_target_on_island_in_the_middle_of_infinite_map()
        {
            var map = new TestMap(true, new[]
            {
                "S......",
                ".......",
                "..###..",
                "..#T#..",
                "..###..",
                "......."
            });

            var pathFinder = new PathFinder(map);

            var actualPath = pathFinder.FindPath(map.StartLocation.Value, map.TargetLocation.Value);
            actualPath.Should().BeNull();
        }

        [TestMethod]
        public void Can_find_constrained_diagonal_path()
        {
            var map = new TestMap(new[]
            {
                "S#.",
                "#..",
                "..T",
            });

            var pathFinder = new PathFinder(map);

            var actualPath = pathFinder.FindPath(map.StartLocation.Value, map.TargetLocation.Value);
            AssertPath(actualPath, 2, map);
        }

        [TestMethod]
        public void Can_find_vertical_tricky_path()
        {
            var map = new TestMap(new[]
            {
                "S....",
                "...#.",
                "...#.",
                "...#T",
            });

            var pathFinder = new PathFinder(map);

            var actualPath = pathFinder.FindPath(map.StartLocation.Value, map.TargetLocation.Value);
            AssertPath(actualPath, 7, map);
        }

        [TestMethod]
        public void Can_find_path_through_maze()
        {
            var map = new TestMap(new[]
            {
                "S.......",
                ".######.",
                ".#......",
                "##.#####",
                "........",
                ".#.####.",
                ".#.#....",
                ".#######",
                ".......T",
            });

            var pathFinder = new PathFinder(map);

            var actualPath = pathFinder.FindPath(map.StartLocation.Value, map.TargetLocation.Value);
            AssertPath(actualPath, 23, map);
        }

        [TestMethod]
        public void Can_find_long_direct_path()
        {
            var map = new InfiniteEmptyMap();

            var pathFinder = new PathFinder(map);

            var actualPath = pathFinder.FindPath(new Location2D(100, 100), new Location2D(0, 0));

            actualPath.Should().NotBeNull();
            actualPath.Length.Should().Be(100);
        }

        [TestMethod]
        public void Can_find_path_to_a_place_in_specific_distance_from_target()
        {
            var map = new TestMap(new[]
            {
                "S..",
                "...",
                "..T",
            });

            var pathFinder = new PathFinder(map);

            var actualPath = pathFinder.FindPath(map.StartLocation.Value, map.TargetLocation.Value, 1);
            actualPath.Should().NotBeNull();
            actualPath.Length.Should().Be(1);
        }

        private void AssertPath(Direction[] actualPath, int maxSteps, TestMap map)
        {
            actualPath.Should().NotBeNull();
            map.StartLocation.Should().NotBeNull();
            map.TargetLocation.Should().NotBeNull();
            actualPath.Length.Should().BeLessOrEqualTo(maxSteps);

            var location = map.StartLocation.Value;
            foreach (var direction in actualPath)
                location = location.LocationInDirection(direction);

            map.TargetLocation.ShouldBeEquivalentTo(location);
        }
    }

    public class InfiniteEmptyMap : IWorldMap
    {
        public bool IsPassable(Location2D start, Direction direction) => true;
    }

    public class TestMap : IWorldMap
    {
        private char outOfRangeChar;
        public List<Direction> Path { get; } = new List<Direction>();
        private readonly char[,] map;

        public TestMap(bool infinite, Location2D? startLocation, string[] mapString)
        {
            outOfRangeChar = infinite ? '.' : '#';
            StartLocation = startLocation;

            ushort y = 0;
            ushort maxX = 0;

            map = new char[mapString.Max(x => x.Length), mapString.Length];

            foreach (var line in mapString)
            {
                ushort x = 0;
                if (maxX < x)
                    maxX = x;

                foreach (var ch in line)
                {
                    switch (ch)
                    {
                        case 'T':
                            if (TargetLocation.HasValue)
                                throw new InvalidOperationException($"TargetLocation already specified, T char already at {TargetLocation.Value}");
                            TargetLocation = new Location2D(x, y);
                            map[x, y] = '.';
                            break;
                        case 'S':
                            if (StartLocation.HasValue)
                                throw new InvalidOperationException($"StartLocation already specified, S char already at {StartLocation.Value}");
                            StartLocation = new Location2D(x, y);
                            map[x, y] = '.';
                            break;
                        default:
                            map[x, y] = ch;
                            break;
                    }

                    x++;
                }
                y++;
            }


            BuildPath();
        }

        public TestMap(bool infinite, string[] mapString) : this(infinite, null, mapString)
        {
        }

        public TestMap(string[] mapString) : this(false, null, mapString)
        {
        }

        private void BuildPath()
        {
            var currentLocation = StartLocation.Value;
            char currentChar = At(currentLocation);
            while (IsPath(currentChar))
            {
                switch (currentChar)
                {
                    case '←':
                        Path.Add(Direction.West);
                        currentLocation = currentLocation.LocationInDirection(Direction.West);
                        break;
                    case '→':
                        Path.Add(Direction.East);
                        currentLocation = currentLocation.LocationInDirection(Direction.East);
                        break;
                    case '↑':
                        Path.Add(Direction.North);
                        currentLocation = currentLocation.LocationInDirection(Direction.North);
                        break;
                    case '↓':
                        Path.Add(Direction.South);
                        currentLocation = currentLocation.LocationInDirection(Direction.South);
                        break;
                    case '↖':
                        Path.Add(Direction.Northwest);
                        currentLocation = currentLocation.LocationInDirection(Direction.Northwest);
                        break;
                    case '↗':
                        Path.Add(Direction.Northeast);
                        currentLocation = currentLocation.LocationInDirection(Direction.Northeast);
                        break;
                    case '↘':
                        Path.Add(Direction.Southeast);
                        currentLocation = currentLocation.LocationInDirection(Direction.Southeast);
                        break;
                    case '↙':
                        Path.Add(Direction.Southwest);
                        currentLocation = currentLocation.LocationInDirection(Direction.Southwest);
                        break;
                }

                currentChar = At(currentLocation);
            }
        }

        private bool IsPath(char ch) => "←→↑↓↖↗↘↙".Contains(ch);

        private char At(Location2D location) => location.X < map.GetLength(0) && location.Y < map.GetLength(1) ? map[location.X, location.Y] : outOfRangeChar;

        public Location2D? StartLocation { get; }
        public Location2D? TargetLocation { get;}

        public bool IsPassable(Location2D start, Direction direction) => At(start) == '.' || At(start) == 'S' || At(start) == 'T';
    }
}
