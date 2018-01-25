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
    public class GraphicalEffectPacketTests
    {
        [TestMethod]
        public void Can_deserialize()
        {
            var rawPacket = FakePackets.Instantiate(new byte[]
            {
                0x70, // packet
                0x03, // direction type
                0x00, 0x04, 0x5B, 0x2A, // character id
                0x00, 0x00, 0x00, 0x00, // target id
                0x37, 0x3A, // model of the first frame
                0x0A, 0xA0, // xloc
                0x0C, 0xB3, // yloc
                0x00,       // zloc
                0x0A, 0xA0, // target xloc
                0x0C, 0xB3, // target yloc
                0x00,       // target zloc
                0x00,       // speed of animation
                0x0F,       // duration
                0x00, 0x00, // unknown
                0x01,       // adjust direction during animation
                0x00,       // explode on impact
            });

            var packet = new GraphicalEffectPacket();
            packet.Deserialize(rawPacket);

            packet.DirectionType.Should().Be(EffectDirectionType.StayWithSource);
            packet.CharacterId.Should().Be((ObjectId)0x00045B2A);
            packet.TargetId.Should().Be((ObjectId)0x00000000);
            packet.Type.Should().Be((ModelId)0x373A);
            packet.Location.Should().Be(new Location3D(0x0AA0, 0x0CB3, 0));
            packet.TargetLocation.Should().Be(new Location3D(0x0AA0, 0x0CB3, 0));
            packet.AnimationSpeed.Should().Be(0);
            packet.Duration.Should().Be(0);
            packet.AdjustDirection.Should().BeTrue();
            packet.ExplodeOnImpact.Should().BeFalse();
        }

        [TestMethod]
        public void Can_deserialize_packet_with_negative_z_coords()
        {
            var rawPacket = FakePackets.Instantiate(new byte[]
            {
                0x70, // packet
                0x03, // direction type
                0x00, 0x04, 0x5B, 0x2A, // character id
                0x00, 0x00, 0x00, 0x00, // target id
                0x37, 0x3A, // model of the first frame
                0x0A, 0xA0, // xloc
                0x0C, 0xB3, // yloc
                0xF0,       // zloc
                0x0A, 0xA0, // target xloc
                0x0C, 0xB3, // target yloc
                0xF1,       // target zloc
                0x00,       // speed of animation
                0x0F,       // duration
                0x00, 0x00, // unknown
                0x01,       // adjust direction during animation
                0x00,       // explode on impact
            });

            var packet = new GraphicalEffectPacket();
            packet.Deserialize(rawPacket);

            packet.DirectionType.Should().Be(EffectDirectionType.StayWithSource);
            packet.CharacterId.Should().Be((ObjectId)0x00045B2A);
            packet.TargetId.Should().Be((ObjectId)0x00000000);
            packet.Type.Should().Be((ModelId)0x373A);

            unchecked
            {
                packet.Location.Should().Be(new Location3D(0x0AA0, 0x0CB3, (sbyte)0xF0));
                packet.TargetLocation.Should().Be(new Location3D(0x0AA0, 0x0CB3, (sbyte)0xF1));
            }

            packet.AnimationSpeed.Should().Be(0);
            packet.Duration.Should().Be(0);
            packet.AdjustDirection.Should().BeTrue();
            packet.ExplodeOnImpact.Should().BeFalse();
        }
    }
}
