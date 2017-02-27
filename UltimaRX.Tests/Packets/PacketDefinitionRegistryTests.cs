using FluentAssertions;
using Infusion.Packets;
using Infusion.Packets.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infusion.Tests.Packets
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