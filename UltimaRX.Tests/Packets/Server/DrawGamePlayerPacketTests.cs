using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UltimaRX.Packets;
using UltimaRX.Packets.Server;

namespace UltimaRX.Tests.Packets.Server
{
    [TestClass]
    public class DrawGamePlayerPacketTests
    {
        [TestMethod]
        public void Can_deserialize()
        {
            var rawPacket = FakePackets.Instantiate(new byte[]
            {
                0x20, // packet
                0x00, 0x00, 0x00, 0x01, // id
                0x01, 0x90, // body type
                0x00, // unknown
                0x83, 0xEA, // skin color hue
                0x00, // flag byte
                0x12, 0x9C, // xloc
                0x05, 0x52, // yloc
                0x00, 0x00, // unknown
                0x04, // direction
                0x0A, // zloc
            });

            var packet = new DrawGamePlayerPacket();
            packet.Deserialize(rawPacket);

            packet.PlayerId.Should().Be(0x00000001);
            packet.BodyType.Should().Be(0x0190);
            packet.Location.X.Should().Be(0x129C);
            packet.Location.Y.Should().Be(0x0552);
            packet.Location.Z.Should().Be(0x0A);
            packet.Movement.Should().Be(Direction.South);
        }
    }
}
