using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UltimaRX.Packets.Server;

namespace UltimaRX.Tests.Packets.Server
{
    [TestClass]
    public class UpdateCurrentStaminaPacketTests
    {
        [TestMethod]
        public void Can_deserialize()
        {
            var rawPacket = FakePackets.Instantiate(new byte[]
            {
                0xA3, // packet
                0x00, 0x04, 0x5B, 0x2A, // player id
                0x00, 0x68, // max stamina
                0x00, 0x53 // current stamina
            });
            var packet = new UpdateCurrentStaminaPacket();
            packet.Deserialize(rawPacket);

            packet.PlayerId.Should().Be(0x00045B2A);
            packet.MaxStamina.Should().Be(0x0068);
            packet.CurrentStamina.Should().Be(0x0053);
        }
    }
}