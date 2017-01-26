using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UltimaRX.Packets.Server;

namespace UltimaRX.Tests.Packets.Server
{
    [TestClass]
    public class StatusBarInfoPacketTests
    {
        [TestMethod]
        public void Can_deserialize()
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

            packet.PlayerId.Should().Be(0x00045B2A);
            packet.PlayerName.Should().Be("Pipka");
            packet.CurrentHealth.Should().Be(0x68);
            packet.MaxHealth.Should().Be(0x68);
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
    }
}