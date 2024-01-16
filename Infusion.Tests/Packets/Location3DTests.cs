using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infusion.Tests.Packets
{
    [TestClass]
    public class Location3DTests
    {
        [TestMethod]
        public void Cannot_instantiate_location_with_XYcoordinates_out_of_ushort_range()
        {
            ((Action)(() => new Location3D(-1, 0, 0))).Should().Throw<ArgumentOutOfRangeException>();
            ((Action)(() => new Location3D(0, -1, 0))).Should().Throw<ArgumentOutOfRangeException>();
            ((Action)(() => new Location3D(ushort.MaxValue + 1, 0, 0))).Should().Throw<ArgumentOutOfRangeException>();
            ((Action)(() => new Location3D(ushort.MinValue - 1, 0, 0))).Should().Throw<ArgumentOutOfRangeException>();
            ((Action)(() => new Location3D(0, ushort.MaxValue + 1, 0))).Should().Throw<ArgumentOutOfRangeException>();
            ((Action)(() => new Location3D(0, ushort.MinValue - 1, 0))).Should().Throw<ArgumentOutOfRangeException>();
        }

        [TestMethod]
        public void Cannot_instantiate_location_with_Zcoordinate_out_of_sbyte_range()
        {
            ((Action)(() => new Location3D(0, 0, sbyte.MinValue - 1))).Should().Throw<ArgumentOutOfRangeException>();
            ((Action)(() => new Location3D(0, 0, sbyte.MaxValue + 1))).Should().Throw<ArgumentOutOfRangeException>();
        }

        [TestMethod]
        public void Can_instantiate_location_wit_XYcoordinates_in_ushort_range()
        {
            ((Action)(() => new Location3D(ushort.MinValue, 10, 0))).Should().NotThrow();
            ((Action)(() => new Location3D(ushort.MaxValue, 10, 0))).Should().NotThrow();
            ((Action)(() => new Location3D(10, ushort.MinValue, 0))).Should().NotThrow();
            ((Action)(() => new Location3D(10, ushort.MaxValue, 0))).Should().NotThrow();
            ((Action)(() => new Location3D(0, 0, 0))).Should().NotThrow();
        }

        [TestMethod]
        public void Can_instantiate_location_with_Zcoordinate_in_sbyte_range()
        {
            ((Action)(() => new Location3D(0, 0, sbyte.MinValue))).Should().NotThrow();
            ((Action)(() => new Location3D(0, 0, sbyte.MaxValue))).Should().NotThrow();
        }

        [TestMethod]
        public void GetDistance_can_calculate_distance()
        {
            new Location3D(0, 0, 0).GetDistance(new Location3D(0, 0, 10)).Should().Be(10);
        }
    }
}
