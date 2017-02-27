using FluentAssertions;
using Infusion.Packets;
using Infusion.Packets.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infusion.Tests.Packets.Server
{
    [TestClass]
    public class UpdatePlayerPacketTests
    {
        [TestMethod]
        public void Can_deserialize()
        {
            var rawPacket = FakePackets.Instantiate(new byte[]
            {
                0x77, // packet
                0x00, 0x06, 0x62, 0x21, // PlayerId
                0x00, 0x06, // type
                0x08, 0xED, // xloc
                0x09, 0x7F, // yloc
                0x01, // zloc
                0x01, // direction
                0x09, 0x01, // color
                0x00, // status flag
                0x03, // highlight color
            });

            var packet = new UpdatePlayerPacket();
            packet.Deserialize(rawPacket);

            packet.PlayerId.Should().Be(0x00066221);
            packet.Type.Should().Be((ModelId)0x0006);
            packet.Location.X.Should().Be(0x08ed);
            packet.Location.Y.Should().Be(0x097f);
            packet.Location.Z.Should().Be(1);
            packet.Direction.Direction.Should().Be(Direction.Northeast);
            packet.Direction.Type.Should().Be(MovementType.Walk);
            packet.Color.Should().Be((Color) 0x0901);
        }
    }
}
