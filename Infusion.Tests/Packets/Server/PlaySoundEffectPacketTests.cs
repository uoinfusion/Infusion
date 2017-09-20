using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Infusion.Packets.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infusion.Tests.Packets.Server
{
    [TestClass]
    public class PlaySoundEffectPacketTests
    {
        [TestMethod]
        public void Can_deserialize()
        {
            var rawPacket = FakePackets.Instantiate(new byte[]
            {
                0x54, // packet id
                0x01, // mode
                0x01, 0x3E, // sound id
                0x00, 0x00, // unknown
                0x06, 0xA5, // x
                0x04, 0xCB, // y
                0x00, 0x23, // z
            });

            var packet = new PlaySoundEffectPacket();
            packet.Deserialize(rawPacket);

            packet.Id.Should().Be((SoundId) 0x13E);
            packet.Location.Should().Be(new Location3D(0x06A5, 0x04CB, 0));
        }
    }
}
