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
                "..T..",
                ".....",
                "..x..",
            });

            var expectedPathMap = new TestMap(map.StartLocation.Value, new[]
            {
                ".....",
                "..↑..",
                "..↑..",
            });

            var pathFinder = new PathFinder();

            var actualPath = pathFinder.FindPath(map.StartLocation.Value, map.TargetLocation.Value);
            actualPath.ShouldBeEquivalentTo(expectedPathMap.Path);
        }

        [TestMethod]
        public void Can_find_direct_south_path()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void Can_find_direct_west_path()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void Can_find_direct_east_path()
        {
            Assert.Inconclusive();
        }
    }

    public class TestMap
    {
        public List<Direction> Path { get; } = new List<Direction>();
        private readonly char[,] map;

        public TestMap(Location2D? startLocation, string[] mapString)
        {
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
                                throw new InvalidOperationException($"TargetLocation already specified, x char already at {TargetLocation.Value}");
                            TargetLocation = new Location2D(x, y);
                            break;
                        case 'x':
                            if (StartLocation.HasValue)
                                throw new InvalidOperationException($"StartLocation already specified, x char already at {StartLocation.Value}");
                            StartLocation = new Location2D(x, y);
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

        public TestMap(string[] mapString) : this(null, mapString)
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

        private char At(Location2D location) => map[location.X, location.Y];

        public Location2D? StartLocation { get; }
        public Location2D? TargetLocation { get;}
    }
}
