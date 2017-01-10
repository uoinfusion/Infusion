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
    public class AddItemToContainerPacketTests
    {
        [TestMethod]
        public void Can_deserialize()
        {
            var rawPacket = FakePackets.Instantiate(new byte[]
            {
                0x25, // packet
                0x40, 0x06, 0x40, 0x87, // object id
                0x1B, 0xDD, // object type
                0x00, // item id offset?
                0x00, 0x02, // amount
                0x00, 0x72, // xloc
                0x00, 0x74, // yloc
                0x40, 0x02, 0x43, 0x33, // container id
                0x00, 0x00, // color
            });

            var packet = new AddItemToContainerPacket();
            packet.Deserialize(rawPacket);

            packet.ItemId.Should().Be(0x40064087);
            packet.Type.Should().Be((ModelId)0x1BDD);
            packet.Amount.Should().Be(2);
            packet.Location.X.Should().Be(0x72);
            packet.Location.Y.Should().Be(0x74);
            packet.ContainerId.Should().Be(0x40024333);
            packet.Color.Should().Be((Color) 0);
        }
    }
}
