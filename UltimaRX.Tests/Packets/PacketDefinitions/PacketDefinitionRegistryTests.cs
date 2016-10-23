using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UltimaRX.Packets.PacketDefinitions;
using UltimaRX.Packets.PacketDefinitions.Server;

namespace UltimaRX.Tests.Packets.PacketDefinitions
{
    [TestClass]
    public class PacketDefinitionRegistryTests
    {
        [TestMethod]
        public void Can_materialize_packet()
        {
            var packet = PacketDefinitionRegistry.Materialize<ConnectToGameServer>(FakePackets.ConnectToGameServerPacket);

            packet.Should().NotBeNull();
        }
    }
}