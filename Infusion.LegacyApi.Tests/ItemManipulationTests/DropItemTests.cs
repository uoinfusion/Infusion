using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.LegacyApi.Tests.ItemManipulationTests
{
    [TestClass]
    public class DropItemTests
    {
        [TestMethod]
        public void Can_drop_item_to_container_at_specific_location()
        {
            byte[] expectedPayload = new byte[]
            {
                0x08, 0x40, 0x07, 0xAF, 0xA8, 0x00, 0x8E, 0x00, 0x41, 0x00, 0x40, 0x07, 0x3D, 0x99
            };
            var testProxy = new InfusionTestProxy();
            testProxy.Api.DropItem(0x4007AFA8, 0x40073D99, new Location2D(0x8E, 0x41));

            testProxy.PacketsSentToServer.Should().HaveCount(1);
            testProxy.PacketsSentToServer.Single().Payload.ShouldBeEquivalentTo(expectedPayload);
        }
    }
}
