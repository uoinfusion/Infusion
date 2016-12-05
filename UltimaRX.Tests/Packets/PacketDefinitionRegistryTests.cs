using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UltimaRX.Packets;
using UltimaRX.Packets.Server;

namespace UltimaRX.Tests.Packets
{
    [TestClass]
    public class PacketDefinitionRegistryTests
    {
        [TestMethod]
        public void Can_materialize_packet()
        {
            var packet = PacketDefinitionRegistry.Materialize<ConnectToGameServerPacket>(FakePackets.ConnectToGameServerPacket);

            packet.Should().NotBeNull();
        }
    }
}