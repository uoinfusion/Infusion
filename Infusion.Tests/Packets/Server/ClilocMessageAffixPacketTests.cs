using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Infusion.Packets;
using Infusion.Packets.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infusion.Tests.Packets.Server
{
    [TestClass]
    public class ClilocMessageAffixPacketTests
    {
        [TestMethod]
        public void Can_deserialize()
        {
            var rawPacket = FakePackets.Instantiate(new byte[]
            {
                0xCC, 0x00, 0x3E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 0xB2, 0x00, 0x03, 0x00, 0x07,
                0xA4, 0x8C, 0x00, 0x79, 0x73, 0x74, 0x65, 0x6D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x74, 0x68, 0x65, 0x20, 0x74, 0x61, 0x72, 0x67, 0x65, 0x74, 0x00, 0x00, 0x00,

            });
            var packet = new ClilocMessageAffixPacket();
            packet.Deserialize(rawPacket);

            packet.SpeakerId.Should().Be(new ObjectId(0));
            packet.SpeakerBody.Should().Be((ModelId)0);
            packet.Color.Should().Be((Color)0x03B2);
            packet.Font.Should().Be(0x03);
            packet.MessageId.Should().Be(new MessageId(0x0007A48C));
            packet.Name.Should().Be("ystem");
            packet.Affix.Should().Be("the target");
        }
    }
}
