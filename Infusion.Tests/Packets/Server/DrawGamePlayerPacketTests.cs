using FluentAssertions;
using Infusion.Packets;
using Infusion.Packets.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infusion.Tests.Packets.Server
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

            packet.PlayerId.Should().Be(new ObjectId(0x00000001));
            packet.BodyType.Should().Be((ModelId)0x0190);
            packet.Location.X.Should().Be(0x129C);
            packet.Location.Y.Should().Be(0x0552);
            packet.Location.Z.Should().Be(0x0A);
            packet.Color.Should().Be((Color) 0x83EA);
            packet.Movement.Type.Should().Be(MovementType.Walk);
            packet.Movement.Direction.Should().Be(Direction.South);
        }
    }
}
