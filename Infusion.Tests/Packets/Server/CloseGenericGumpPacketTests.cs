using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Infusion.Packets.Both;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infusion.Tests.Packets.Server
{
    [TestClass]
    public class CloseGenericGumpPacketTests
    {
        [TestMethod]
        public void Can_deserialize()
        {
            var rawPacket = FakePackets.Instantiate(new byte[]
            {
                0xBF, // packet
                0x00, 0x0D, // size
                0x00, 0x04, // sub command
                0x96, 0x00, 0x05, 0x7B,
                0x00, 0x00, 0x00, 0x01
            });

            var packet = new CloseGenericGumpPacket();
            packet.Deserialize(rawPacket);

            packet.GumpId.Should().Be((GumpInstanceId)0x9600057B);
        }
    }
}
