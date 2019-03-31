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
    public class ContainerHandlingTests
    {
        [TestMethod]
        public void ObjectInfo_takes_out_item_from_previous_container()
        {
            var testProxy = new InfusionTestProxy();

            var containerId = (ObjectId)0x400B2683;

            testProxy.ServerApi.ObjectInfo(containerId, 0x2009, 1, new Location3D(0x123, 0x321, -1), (Color)0x1020);
            var id = testProxy.ServerApi.AddNewItemToContainer(0x1BFB, 1, new Location2D(1, 2), containerId, (Color)0x0593);
            testProxy.ServerApi.ObjectInfo(id, 0x1BFB, 1, new Location3D(0x123, 0x321, -1), (Color)0x0593);

            var testedItem = testProxy.Api.Items[id];
            testedItem.Should().NotBeNull();
            testedItem.ContainerId.Should().BeNull();
        }
    }
}
