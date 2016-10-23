using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UltimaRX.Packets.Server;

namespace UltimaRX.Tests.Packets.PacketDefinitions.Server
{
    [TestClass]
    public class ConnectToGameServerTests
    {
        [TestMethod]
        public void Can_get_GameServerIP()
        {
            var packet = new ConnectToGameServer(FakePackets.ConnectToGameServerPacket);

            packet.GameServerIp.Should().BeEquivalentTo(new byte[] {0x7F, 0x00, 0x00, 0x01});
        }

        [TestMethod]
        public void Can_set_GameServerIp()
        {
            var packet = new ConnectToGameServer(FakePackets.ConnectToGameServerPacket);
            var gameServerIp = new byte[] {0xFF, 0xFE, 0xFD, 0xFC};

            packet.GameServerIp = gameServerIp;

            gameServerIp.Should().BeEquivalentTo(packet.GameServerIp);
            gameServerIp.Should().BeSubsetOf(packet.RawPacket.Payload);
        }

        [TestMethod]
        public void Can_get_GameServerPort()
        {
            var packet = new ConnectToGameServer(FakePackets.ConnectToGameServerPacket);

            packet.GameServerPort.Should().Be(2593);
        }

        [TestMethod]
        public void Can_set_GameServerPort()
        {
            var packet = new ConnectToGameServer(FakePackets.ConnectToGameServerPacket) {GameServerPort = 0xAABB};

            packet.GameServerPort.Should().Be(0xAABB);
            new byte[] {0xAA, 0xBB}.Should().BeSubsetOf(packet.RawPacket.Payload);
        }
    }
}