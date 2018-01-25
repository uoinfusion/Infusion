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
            ((Action)(() => new Location3D(-1, 0, 0))).ShouldThrow<ArgumentOutOfRangeException>();
            ((Action)(() => new Location3D(0, -1, 0))).ShouldThrow<ArgumentOutOfRangeException>();
            ((Action)(() => new Location3D(ushort.MaxValue + 1, 0, 0))).ShouldThrow<ArgumentOutOfRangeException>();
            ((Action)(() => new Location3D(ushort.MinValue - 1, 0, 0))).ShouldThrow<ArgumentOutOfRangeException>();
            ((Action)(() => new Location3D(0, ushort.MaxValue + 1, 0))).ShouldThrow<ArgumentOutOfRangeException>();
            ((Action)(() => new Location3D(0, ushort.MinValue - 1, 0))).ShouldThrow<ArgumentOutOfRangeException>();
        }

        [TestMethod]
        public void Cannot_instantiate_location_with_Zcoordinate_out_of_sbyte_range()
        {
            ((Action)(() => new Location3D(0, 0, sbyte.MinValue - 1))).ShouldThrow<ArgumentOutOfRangeException>();
            ((Action)(() => new Location3D(0, 0, sbyte.MaxValue + 1))).ShouldThrow<ArgumentOutOfRangeException>();
        }

        [TestMethod]
        public void Can_instantiate_location_wit_XYcoordinates_in_ushort_range()
        {
            ((Action)(() => new Location3D(ushort.MinValue, 10, 0))).ShouldNotThrow();
            ((Action)(() => new Location3D(ushort.MaxValue, 10, 0))).ShouldNotThrow();
            ((Action)(() => new Location3D(10, ushort.MinValue, 0))).ShouldNotThrow();
            ((Action)(() => new Location3D(10, ushort.MaxValue, 0))).ShouldNotThrow();
            ((Action)(() => new Location3D(0, 0, 0))).ShouldNotThrow();
        }

        [TestMethod]
        public void Can_instantiate_location_with_Zcoordinate_in_sbyte_range()
        {
            ((Action)(() => new Location3D(0, 0, sbyte.MinValue))).ShouldNotThrow();
            ((Action)(() => new Location3D(0, 0, sbyte.MaxValue))).ShouldNotThrow();
        }
    }
}
