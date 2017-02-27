using FluentAssertions;
using Infusion.Packets.Both;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infusion.Tests.Packets.Both
{
    [TestClass]
    public class CharacterMoveAckPacketTests
    {
        [TestMethod]
        public void Can_deserialize()
        {
            var rawPacket = FakePackets.Instantiate(new byte[]
            {
                0x22, // packet
                0x02, // movement sequence key
                0x41, // notoriety flag
            });
            var packet = new CharacterMoveAckPacket();
            packet.Deserialize(rawPacket);

            packet.MovementSequenceKey.Should().Be(0x02);
        }
    }
}
