using FluentAssertions;
using Infusion.Packets;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infusion.LegacyApi.Tests
{
    [TestClass]
    public class ItemCollectionTests
    {
        [TestMethod]
        public void Can_update_item()
        {
            var item = new Item(0x12345678, (ModelId) 0x4321, 333, new Location3D(6, 5, 4));
            var itemCollection = new ItemCollection(new Player(null, null, null));

            itemCollection.UpdateItem(item);

            item = item.UpdateHealth(111, 222);
            itemCollection.UpdateItem(item);

            item = itemCollection[item.Id];
            item.CurrentHealth.Should().Be(111);
            item.MaxHealth.Should().Be(222);
        }
    }
}
