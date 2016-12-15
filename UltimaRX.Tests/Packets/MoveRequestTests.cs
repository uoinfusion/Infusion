using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UltimaRX.Packets.Client;

namespace UltimaRX.Tests.Packets
{
    [TestClass]
    public class MoveRequestTests
    {
        [TestMethod]
        public void Can_deserialize_running_request()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void Can_deserialize_walk_request()
        {
            var rawPacket = FakePackets.Instantiate(new byte[]
            {
                0x02, // packet
                0x80, // direction
                0x06, // sequence key
                0x00, 0x00, 0x00, 0x00 // fast walk prevention key
            });

            var packet = new MoveRequest();
            packet.SequenceKey.Should().Be(0x06);

        }
    }
}
