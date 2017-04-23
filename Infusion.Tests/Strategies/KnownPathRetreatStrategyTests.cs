using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Infusion.Packets;
using Infusion.Strategies;
using Microsoft.VisualStudio.TestTools.UnitTesting;
// ReSharper disable PossibleInvalidOperationException

namespace Infusion.Tests.Strategies
{
    [TestClass]
    public class KnownPathRetreatStrategyTests
    {
        [TestMethod]
        public void Stays_When_target_is_farther_than_safe_distance()
        {
            ushort safeDistance = 5;

            var strategy = new KnownPathRetreatStrategy(safeDistance);

            var direction = strategy.NextDirection(new Location3D(0, 0, 0), new Location3D(6, 0, 0));
            direction.Should().Be(null);
        }

        [TestMethod]
        public void Stays_When_target_is_exactly_in_safe_distance()
        {
            ushort safeDistance = 5;

            var strategy = new KnownPathRetreatStrategy(safeDistance);

            strategy.NextDirection(new Location3D(1, 0, 0), new Location3D(6, 0, 0)).Should().BeNull();
        }

        [TestMethod]
        public void Can_make_steps_back_according_known_approach_path_When_target_is_closer_than_safe_distance()
        {
            var startLocation = new Location2D(0, 4);
            var approachPath = new TestMap(startLocation, new[]
            {
                "....t",
                "...c.",
                ".→→↑.",
                ".↑...",
                "→↑...",
            }).Path;

            ushort safeDistance = 5;
            var strategy = new KnownPathRetreatStrategy(safeDistance, approachPath);

            var currentLocation =  new Location3D(3, 1, 0);
            var targetLocation = new Location3D(4, 0, 0);

            var direction = strategy.NextDirection(currentLocation, targetLocation);
            direction.Should().Be(Direction.South);
            currentLocation = currentLocation.LocationInDirection(direction.Value);

            direction = strategy.NextDirection(currentLocation, targetLocation);
            direction.Should().Be(Direction.West);
            currentLocation = currentLocation.LocationInDirection(direction.Value);

            direction = strategy.NextDirection(currentLocation, targetLocation);
            direction.Should().Be(Direction.West);
            currentLocation = currentLocation.LocationInDirection(direction.Value);

            direction = strategy.NextDirection(currentLocation, targetLocation);
            direction.Should().Be(Direction.South);
            currentLocation = currentLocation.LocationInDirection(direction.Value);

            direction = strategy.NextDirection(currentLocation, targetLocation);
            direction.Should().Be(Direction.South);
            currentLocation = currentLocation.LocationInDirection(direction.Value);

            strategy.NextDirection(currentLocation, targetLocation).Should().BeNull();
        }

        [TestMethod]
        public void Can_continue_in_last_direction_When_retreating_path_ends()
        {
            ushort safeDistance = 5;
            var strategy = new KnownPathRetreatStrategy(safeDistance, new[] { Direction.North, Direction.East,  });

            var currentLocation = new Location3D(5, 5, 0);
            var targetLocation = new Location3D(6, 6, 0);

            var direction = strategy.NextDirection(currentLocation, targetLocation);
            currentLocation = currentLocation.LocationInDirection(direction.Value);

            direction = strategy.NextDirection(currentLocation, targetLocation);
            currentLocation = currentLocation.LocationInDirection(direction.Value);

            direction = strategy.NextDirection(currentLocation, targetLocation);
            currentLocation = currentLocation.LocationInDirection(direction.Value);
            direction.Should().Be(Direction.South);

            direction = strategy.NextDirection(currentLocation, targetLocation);
            currentLocation = currentLocation.LocationInDirection(direction.Value);
            direction.Should().Be(Direction.South);
        }

        [TestMethod]
        public void Can_remember_reapproach_path()
        {
            ushort safeDistance = 10;
            var strategy = new KnownPathRetreatStrategy(safeDistance, new[] { Direction.South, Direction.West, Direction.North, Direction.East });

            var currentLocation = new Location3D(5, 5, 0);
            var targetLocation = new Location3D(6, 6, 0);

            var direction = strategy.NextDirection(currentLocation, targetLocation);
            currentLocation = currentLocation.LocationInDirection(direction.Value);

            direction = strategy.NextDirection(currentLocation, targetLocation);
            currentLocation = currentLocation.LocationInDirection(direction.Value);

            direction = strategy.NextDirection(currentLocation, targetLocation);
            currentLocation = currentLocation.LocationInDirection(direction.Value);

            var reapproachPath = strategy.GetReaproachPath();
            reapproachPath.Should().BeEquivalentTo(new[] {Direction.West, Direction.North, Direction.East,});
        }
    }
}
