using FluentAssertions;
using Infusion.LegacyApi.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.LegacyApi.Tests
{
    [TestClass]
    public class MapTests
    {
        [TestMethod]
        public void Publishes_event_When_map_message_packet_received()
        {
            var testProxy = new InfusionTestProxy();
            var journal = testProxy.Api.CreateEventJournal();

            testProxy.PacketReceivedFromServer(new byte[]
            {
                0x90, 0x40, 0x09, 0x73, 0x5C,
                0x13, 0x9D,
                0x10, 0xD4, 0x08, 0x44,
                0x12, 0x3C, 0x09, 0xAC,
                0x00, 0xC8,
                0x00, 0xC8,
            });

            journal.OfType<MapMessageEvent>().Any().Should().BeTrue();
            var ev = journal.OfType<MapMessageEvent>().First();

            ev.UpperLeft.Should().Be(new Location2D(0x10D4, 0x0844));
            ev.LowerRight.Should().Be(new Location2D(0x123C, 0x09AC));
            ev.Width.Should().Be(0x00C8);
            ev.Height.Should().Be(0x00C8);
        }
    }
}
