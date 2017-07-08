using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Infusion.Packets;
using Infusion.Proxy.LegacyApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infusion.Proxy.Tests.InjectionApi
{
    [TestClass]
    public class ItemCollectionTests
    {
        [TestMethod]
        public void Can_update_item()
        {
            var item = new Item(0x12345678, (ModelId) 0x4321, 333, new Location3D(6, 5, 4));
            var itemCollection = new ItemCollection(new Player(null));

            itemCollection.UpdateItem(item);

            item = item.UpdateHealth(111, 222);
            itemCollection.UpdateItem(item);

            item = itemCollection[item.Id];
            item.CurrentHealth.Should().Be(111);
            item.MaxHealth.Should().Be(222);
        }
    }
}
