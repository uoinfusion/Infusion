using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Infusion.Packets;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infusion.LegacyApi.Tests.ItemManipulationTests
{
    [TestClass]
    public class LayerHandlingTests
    {
        [TestMethod]
        public void Sets_layer_when_item_worn()
        {
            var wornPacket = new Packet(0x2E, new byte[]
            {
                0x2E, // packet id
                0x40, 0x04, 0xF1, 0x9C, // item id
                0x0F, 0x51, 0x00,
                0x02, // layer
                0x00, 0x06, 0x39, 0x0E, 0x07, 0x63,
            });
            var testProxy = new InfusionTestProxy();

            testProxy.PacketReceivedFromServer(wornPacket);

            testProxy.Api.Items[0x4004F19C].Layer.HasValue.Should().BeTrue();
            testProxy.Api.Items[0x4004F19C].Layer.Should().Be(Layer.TwoHandedWeapon);
        }

        [TestMethod]
        public void Resets_layer_when_added_to_container()
        {
            var wornPacket = new Packet(0x2E, new byte[]
            {
                0x2E, 0x40, 0x04, 0xF1, 0x9C, 0x0F, 0x51, 0x00, 0x02, 0x00, 0x06, 0x39, 0x0E, 0x07, 0x63,
            });
            var testProxy = new InfusionTestProxy();

            testProxy.PacketReceivedFromServer(wornPacket);

            var addToContainerPacket = new Packet(0x25, new byte[]
            {
                0x25, 0x40, 0x04, 0xF1, 0x9C, 0x13, 0xF8, 0x00, 0x00, 0x01, 0x00, 0x36, 0x00, 0x7F, 0x40, 0x05,
                0x67, 0x56, 0x00, 0xED,
            });

            testProxy.PacketReceivedFromServer(addToContainerPacket);

            testProxy.Api.Items[0x4004F19C].Layer.HasValue.Should().BeFalse();
        }
    }
}
