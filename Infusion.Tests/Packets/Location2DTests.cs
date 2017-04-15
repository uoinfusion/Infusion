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
    }
}
