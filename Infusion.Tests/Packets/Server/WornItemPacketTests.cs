using FluentAssertions;
using Infusion.Packets;
using Infusion.Packets.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infusion.Tests.Packets.Server
{
    [TestClass]
    public class WornItemPacketTests
    {
        [TestMethod]
        public void Can_deserialize()
        {
            var rawPacket = FakePackets.Instantiate(new byte[]
            {
                0x2E, // packet
                0x40, 0x05, 0xA4, 0x96, // item id
                0x0E, 0x7C, // type
                0x00, // unknown
                0x1D, // layer
                0x00, 0x04, 0x5B, 0x2A, // player id
                0x03, 0x84, // color
            });

            var packet = new WornItemPacket();
            packet.Deserialize(rawPacket);

            packet.ItemId.Should().Be(0x4005a496);
            packet.Type.Should().Be((ModelId) 0x0e7c);
            packet.Layer.Should().Be(Layer.BankBox);
            packet.PlayerId.Should().Be(0x00045B2A);
            packet.Color.Should().Be((Color) 0x0384);
        }
    }
}
