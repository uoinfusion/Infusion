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
            var item = new Mobile(0x12345678, (ModelId) 0x4321, new Location3D(6, 5, 4), null, new Movement(Direction.East, MovementType.Run), null);
            var itemCollection = new GameObjectCollection(new Player(null, null, null));

            itemCollection.UpdateObject(item);

            item = item.UpdateHealth(111, 222);
            itemCollection.UpdateObject(item);

            item = (Mobile)itemCollection[item.Id];
            item.CurrentHealth.Should().Be(111);
            item.MaxHealth.Should().Be(222);
        }
    }
}
