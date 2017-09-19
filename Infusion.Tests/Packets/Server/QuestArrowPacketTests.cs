using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Infusion.Packets.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infusion.Tests.Packets.Server
{
    [TestClass]
    public class QuestArrowPacketTests
    {
        [TestMethod]
        public void Can_deserialize()
        {
            var rawPacket = FakePackets.Instantiate(new byte[]
            {
                0xBA, // packet id
                0x01, // active?
                0x12, 0x14, // x
                0x0F, 0x3E, // y
            });

            var packet = new QuestArrowPacket();
            packet.Deserialize(rawPacket);

            packet.Active.Should().BeTrue();
            packet.Location.Should().Be(new Location2D(0x1214, 0x0F3E));
        }
    }
}
