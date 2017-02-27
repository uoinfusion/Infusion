using FluentAssertions;
using Infusion.Packets;
using Infusion.Packets.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infusion.Tests.Packets
{
    [TestClass]
    public class MoveRequestTests
    {
        [TestMethod]
        public void Can_deserialize_running_request()
        {
            var rawPacket = FakePackets.Instantiate(new byte[]
            {
                0x02, // packet
                0x80, // direction
                0x06, // sequence key
                0x00, 0x00, 0x00, 0x00 // fast walk prevention key
            });

            var packet = new MoveRequest();
            packet.Deserialize(rawPacket);
            packet.Movement.Direction.Should().Be(Direction.North);
            packet.Movement.Type.Should().Be(MovementType.Run);
            packet.SequenceKey.Should().Be(0x06);
        }

        [TestMethod]
        public void Can_deserialize_walk_request()
        {
            var rawPacket = FakePackets.Instantiate(new byte[]
            {
                0x02, // packet
                0x00, // direction
                0x06, // sequence key
                0x00, 0x00, 0x00, 0x00 // fast walk prevention key
            });

            var packet = new MoveRequest();
            packet.Deserialize(rawPacket);
            packet.Movement.Direction.Should().Be(Direction.North);
            packet.Movement.Type.Should().Be(MovementType.Walk);
            packet.SequenceKey.Should().Be(0x06);
        }
    }
}
