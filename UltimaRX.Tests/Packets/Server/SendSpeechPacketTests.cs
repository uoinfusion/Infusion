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
    public class SendSpeechPacketTests
    {
        [TestMethod]
        public void Can_deserialize()
        {
            var rawPacket = FakePackets.Instantiate(new byte[]
            {
                0x1C, // packet
                0x00, 0x40, // size
                0x00, 0x00, 0x00, 0x00, // id
                0x00, 0x00, // model
                0x00, // SpeechType
                0x03, 0xB2, // color
                0x00, 0x03, // font
                // name
                0x53, 0x79, 0x73, 0x74, 0x65, 0x6D, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                // message
                0x54, 0x61, 0x72, 0x67,
                0x65, 0x74, 0x69, 0x6E, 0x67, 0x20, 0x43, 0x61, 0x6E, 0x63, 0x65, 0x6C, 0x6C, 0x65, 0x64, 0x00,
            });

            var packet = new SendSpeechPacket();
            packet.Deserialize(rawPacket);

            packet.Id.Should().Be(0);
            packet.Model.Should().Be((ModelId)0);
            packet.Type.Should().Be(SpeechType.Normal);
            packet.Color.Should().Be((Color)0x03B2);
            packet.Font.Should().Be(3);
            packet.Name.Should().Be("System");
            packet.Message.Should().Be("Targeting Cancelled");
        }
    }
}
