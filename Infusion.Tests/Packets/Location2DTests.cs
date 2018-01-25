using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Infusion.Packets;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infusion.Tests.Packets
{
    [TestClass]
    public class Location2DTests
    {
        [TestMethod]
        public void Can_parse_two_numbers_separated_bycomma()
        {
            var location = Location2D.Parse("1234, 4321");

            location.Should().Be(new Location2D(1234, 4321));
        }

        [TestMethod]
        public void Cannot_instantiate_location_with_coordinates_out_of_ushort_range()
        {
            ((Action)(() => new Location2D(-1, 0))).ShouldThrow<ArgumentOutOfRangeException>();
            ((Action)(() => new Location2D(0, -1))).ShouldThrow<ArgumentOutOfRangeException>();
            ((Action)(() => new Location2D(ushort.MaxValue + 1, 0))).ShouldThrow<ArgumentOutOfRangeException>();
            ((Action)(() => new Location2D(ushort.MinValue - 1, 0))).ShouldThrow<ArgumentOutOfRangeException>();
            ((Action)(() => new Location2D(0, ushort.MaxValue + 1))).ShouldThrow<ArgumentOutOfRangeException>();
            ((Action)(() => new Location2D(0, ushort.MinValue - 1))).ShouldThrow<ArgumentOutOfRangeException>();
        }

        [TestMethod]
        public void Can_instantiate_location_wit_coordinates_in_ushort_range()
        {
            ((Action)(() => new Location2D(ushort.MinValue, 10))).ShouldNotThrow();
            ((Action)(() => new Location2D(ushort.MaxValue, 10))).ShouldNotThrow();
            ((Action)(() => new Location2D(10, ushort.MinValue))).ShouldNotThrow();
            ((Action)(() => new Location2D(10, ushort.MaxValue))).ShouldNotThrow();
        }
    }
}
