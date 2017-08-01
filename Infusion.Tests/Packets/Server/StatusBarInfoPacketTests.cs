using FluentAssertions;
using Infusion.Packets;
using Infusion.Packets.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infusion.Tests.Packets.Server
{
    [TestClass]
    public class StatusBarInfoPacketTests
    {
        [TestMethod]
        public void Can_deserialize_packet_with_StatusFlag_equals_to_1()
        {
            var rawPacket = FakePackets.Instantiate(new byte[]
            {
                0x11, // packet
                0x00, 0x42, // size
                0x00, 0x04, 0x5B, 0x2A, // serial
                // name
                0x50, 0x69, 0x70, 0x6B, 0x61, 0x00, 0x61, 0x6B, 0x00, 0x00, 0x05, 0x35, 0x0B, 0x61, 0x63, 0x74,
                0x00, 0x68, 0x05, 0x35, 0x0B, 0xC4, 0xFF, 0x34, 0x0B, 0x7C, 0xFF, 0x34, 0x0B, 0x00,
                0x00, 0x68, // current health
                0x00, 0x68, // max health
                0x00, // name change flag
                0x01, // status flag
                0x00, // sex + race
                0x00, 0x72, // strength
                0x00, 0x32, // dexterity
                0x00, 0x28, // intelligence
                0x00, 0x53, // current stamina
                0x00, 0x68, // max stamina
                0x00, 0x28, // current mana
                0x00, 0x28, // max mana
                0x00, 0x00, 0x2F, 0xBE, // gold
                0x00, 0x43, // armor rating
                0x00, 0x9A // weight
            });

            var packet = new StatusBarInfoPacket();
            packet.Deserialize(rawPacket);

            packet.PlayerId.Should().Be(new ObjectId(0x00045B2A));
            packet.PlayerName.Should().Be("Pipka");
            packet.CurrentHealth.Should().Be(0x68);
            packet.MaxHealth.Should().Be(0x68);
            packet.Status.Should().Be(0);
            packet.Strength.Should().Be(0x72);
            packet.Dexterity.Should().Be(0x32);
            packet.Intelligence.Should().Be(0x28);
            packet.CurrentStamina.Should().Be(0x53);
            packet.MaxStamina.Should().Be(0x68);
            packet.CurrentMana.Should().Be(0x28);
            packet.MaxMana.Should().Be(0x28);
            packet.Gold.Should().Be(0x2FBE);
            packet.Weight.Should().Be(0x9A);
        }

        [TestMethod]
        public void Can_deserialize_packet_with_StatusFlag_equals_to_0()
        {
            var rawPacket = FakePackets.Instantiate(new byte[]
            {
                0x11, // packet
                0x00, 0x2B, // size
                0x00, 0x05, 0x75, 0xF7, // player id
                // name
                0x50, 0x74, 0x61, 0x63, 0x65, 0x6B, 0x00, 0x00, 0x00, 0x78, 0xA0, 0x7E, 0x0B, 0xAB, 0xE4, 0xA1,
                0x02, 0x88, 0x7A, 0x7E, 0x0B, 0x79, 0xA3, 0x4D, 0x00, 0x00, 0x7A, 0x7E, 0x0B, 0xB0,
                0x00, 0x64, // current health
                0x00, 0x64, // max health
                0x00, // name change flag
                0x00, // status flag
            });

            var packet = new StatusBarInfoPacket();
            packet.Deserialize(rawPacket);

            packet.PlayerId.Should().Be(new ObjectId(0x000575F7));
            packet.PlayerName.Should().Be("Ptacek");
            packet.CurrentHealth.Should().Be(0x64);
            packet.MaxHealth.Should().Be(0x64);
            packet.Status.Should().Be(0);
        }

        [TestMethod]
        public void Can_deserialize_packet_with_StatusFlag_equals_to_7()
        {
            var rawPacket = FakePackets.Instantiate(new byte[]
            {
                0x11, // packet
                0x00, 0x54, // size
                0x00, 0x04, 0x5B, 0x2A, // player id
                // name
                0x50, 0x69, 0x70, 0x6B, 0x61, 0x00, 0xFF, 0x34, 0x0B, 0x01, 0x00, 0x00, 0x00, 0x0C, 0x00, 0x35,
                0x0B, 0x38, 0x05, 0x35, 0x0B, 0x38, 0x05, 0x35, 0x0B, 0x72, 0x65, 0x67, 0x69, 0x6F,
                0x00, 0x68, // current health
                0x00, 0x68, // max health
                0x00, // name change flag
                0x07, // status flag
                0x00, // sex + race
                0x00, 0x76, // strength
                0x00, 0x32, // dexterity
                0x00, 0x28, // intelligence
                0x00, 0x26, // current stamina
                0x00, 0x68, // max stamina
                0x00, 0x28, // current mana
                0x00, 0x28, // max mana
                0x00, 0x00, 0x2E, 0x38, // gold
                0x00, 0x26, // armor rating
                0x00, 0x9C, // weight
                // ignored fields
                0x03, 0xE8, 0x00, 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x1F, 0x00, 0x1F,
            });

            var packet = new StatusBarInfoPacket();
            packet.Deserialize(rawPacket);

            packet.PlayerId.Should().Be(new ObjectId(0x00045B2A));
            packet.PlayerName.Should().Be("Pipka");
            packet.CurrentHealth.Should().Be(0x68);
            packet.MaxHealth.Should().Be(0x68);
            packet.Status.Should().Be(0);
            packet.Strength.Should().Be(0x76);
            packet.Dexterity.Should().Be(0x32);
            packet.Intelligence.Should().Be(0x28);
            packet.CurrentStamina.Should().Be(0x26);
            packet.MaxStamina.Should().Be(0x68);
            packet.CurrentMana.Should().Be(0x28);
            packet.MaxMana.Should().Be(0x28);
            packet.Gold.Should().Be(0x2E38);
            packet.Weight.Should().Be(0x9C);
        }
    }
}
