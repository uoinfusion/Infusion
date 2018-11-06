using FluentAssertions;
using Infusion.LegacyApi.Injection;
using InjectionScript.Interpretation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.LegacyApi.Tests.Injection
{
    [TestClass]
    public class FindTypeTests
    {
        private TestServerApi serverApi;
        private FindTypeSubrutine findTypeSubrutine;
        private InfusionTestProxy testProxy;
        private InjectionHost injectionHost;

        [TestInitialize]
        public void Initialize()
        {
            testProxy = new InfusionTestProxy();
            serverApi = testProxy.ServerApi;
            findTypeSubrutine = testProxy.Api.Injection.FindTypeSubrutine;
            injectionHost = testProxy.Api.Injection;
        }

        [TestMethod]
        public void Finds_in_backpack_when_no_container()
        {
            serverApi.PlayerEntersWorld(new Location2D(1000, 1000));
            serverApi.AddNewItemToBackpack(0xEED);

            findTypeSubrutine.FindType(0xEED);

            findTypeSubrutine.FindCount.Should().Be(1);
        }

        [TestMethod]
        public void Finds_any_color_when_no_color_specified()
        {
            serverApi.PlayerEntersWorld(new Location2D(1000, 1000));

            serverApi.AddNewItemToBackpack(0xEED, 10, (Color)0x0100);
            serverApi.AddNewItemToBackpack(0xEED, 20, (Color)0x0200);

            findTypeSubrutine.FindType(0xEED);

            findTypeSubrutine.FindCount.Should().Be(2);
        }

        [TestMethod]
        public void Finds_items_with_specified_color()
        {
            serverApi.PlayerEntersWorld(new Location2D(1000, 1000));

            serverApi.AddNewItemToBackpack(0xEED, 10, (Color)0x0100);
            serverApi.AddNewItemToBackpack(0xEED, 20, (Color)0x0200);

            findTypeSubrutine.FindType(0xEED, 0x0100, -1);

            findTypeSubrutine.FindCount.Should().Be(1);
        }

        [TestMethod]
        public void Finds_any_color_when_color_is_minus_1()
        {
            serverApi.PlayerEntersWorld(new Location2D(1000, 1000));

            serverApi.AddNewItemToBackpack(0xEED, 10, (Color)0x0100);
            serverApi.AddNewItemToBackpack(0xEED, 20, (Color)0x0200);

            findTypeSubrutine.FindType(0xEED, -1, -1);

            findTypeSubrutine.FindCount.Should().Be(2);
        }

        [TestMethod]
        public void Finds_on_ground_when_container_is_1()
        {
            serverApi.PlayerEntersWorld(new Location2D(1000, 1000));

            serverApi.AddNewItemToGround(0xEED, new Location2D(1001, 1001), 20, (Color)0);

            findTypeSubrutine.FindType(0xEED, -1, 1);

            findTypeSubrutine.FindCount.Should().Be(1);
        }

        [TestMethod]
        public void Finds_in_backpack_when_container_is_minus_1()
        {
            serverApi.PlayerEntersWorld(new Location2D(1000, 1000));

            serverApi.AddNewItemToBackpack(0xEED);

            findTypeSubrutine.FindType(0xEED, -1, -1);

            findTypeSubrutine.FindCount.Should().Be(1);
        }

        [TestMethod]
        public void Finds_nearest_item_on_ground()
        {
            serverApi.PlayerEntersWorld(new Location2D(1000, 1000));

            serverApi.AddNewItemToGround(0xEED, new Location2D(1005, 1005), 20, (Color)0);
            var nearestId = serverApi.AddNewItemToGround(0xEED, new Location2D(1001, 1001), 15, (Color)0);

            findTypeSubrutine.FindType(0xEED, -1, 1);

            injectionHost.GetSerial("finditem").Should().Be(NumberConversions.Int2Hex((int)nearestId));
        }

        [TestMethod]
        public void Finds_on_ground_when_container_is_ground()
        {
            serverApi.PlayerEntersWorld(new Location2D(1000, 1000));

            serverApi.AddNewItemToGround(0xEED, new Location2D(1001, 1001), 20, (Color)0);

            findTypeSubrutine.FindType(0xEED, -1, "ground");

            findTypeSubrutine.FindCount.Should().Be(1);
        }

        [TestMethod]
        public void Finds_in_backpack_when_container_is_my()
        {
            serverApi.PlayerEntersWorld(new Location2D(1000, 1000));

            serverApi.AddNewItemToBackpack(0xEED);

            findTypeSubrutine.FindType(0xEED, -1, "my");

            findTypeSubrutine.FindCount.Should().Be(1);
        }

        [TestMethod]
        public void Ignores_nested_containers_in_backpack()
        {
            serverApi.PlayerEntersWorld(new Location2D(1000, 1000));
            var subContainerId = serverApi.AddNewItemToBackpack(0x0E75);
            serverApi.AddNewItemToContainer(0xeed, containerId: subContainerId);

            findTypeSubrutine.FindType(0xEED, -1, "my");

            findTypeSubrutine.FindCount.Should().Be(0);
        }

        [TestMethod]
        public void Finds_in_container_when_container_is_id()
        {
            serverApi.PlayerEntersWorld(new Location2D(1000, 1000));
            var subContainerId = serverApi.AddNewItemToBackpack(0x0E75);
            serverApi.AddNewItemToContainer(0xeed, containerId: subContainerId);

            findTypeSubrutine.FindType(0xEED, -1, NumberConversions.Int2Hex(subContainerId));

            findTypeSubrutine.FindCount.Should().Be(1);
        }

        [TestMethod]
        public void Ignores_nested_containers()
        {
            serverApi.PlayerEntersWorld(new Location2D(1000, 1000));
            var subContainerId = serverApi.AddNewItemToBackpack(0x0E75);
            serverApi.AddNewItemToContainer(0xeed, containerId: subContainerId);

            findTypeSubrutine.FindType(0xEED, -1, NumberConversions.Int2Hex(testProxy.Api.Me.BackPack.Id));

            findTypeSubrutine.FindCount.Should().Be(0);
        }

        [TestMethod]
        public void Findcount_resets_to_0_when_no_item_found()
        {
            serverApi.PlayerEntersWorld(new Location2D(1000, 1000));
            serverApi.AddNewItemToBackpack(0xEED);

            findTypeSubrutine.FindType(0xEED);
            findTypeSubrutine.FindCount.Should().Be(1);

            findTypeSubrutine.FindType(0xEEF);
            findTypeSubrutine.FindCount.Should().Be(0);
        }

        [TestMethod]
        public void Unsets_finditem_when_item_not_found()
        {
            serverApi.PlayerEntersWorld(new Location2D(1000, 1000));
            serverApi.AddNewItemToBackpack(0xEED);

            findTypeSubrutine.FindType(0xEED);
            findTypeSubrutine.FindCount.Should().Be(1);

            findTypeSubrutine.FindType(0xEEF);

            findTypeSubrutine.FindItem.Should().Be(0);
        }

        [TestMethod]
        public void Finds_item_inside_chebyshev_distance()
        {
            serverApi.PlayerEntersWorld(new Location2D(1000, 1000));

            // everything inside chebyshev distance
            serverApi.AddNewItemToGround(0xeed, new Location2D(999, 999));
            serverApi.AddNewItemToGround(0xeed, new Location2D(999, 1000));
            serverApi.AddNewItemToGround(0xeed, new Location2D(999, 1001));

            serverApi.AddNewItemToGround(0xeed, new Location2D(1000, 999));
            serverApi.AddNewItemToGround(0xeed, new Location2D(1000, 1000));
            serverApi.AddNewItemToGround(0xeed, new Location2D(1000, 1001));

            serverApi.AddNewItemToGround(0xeed, new Location2D(1001, 999));
            serverApi.AddNewItemToGround(0xeed, new Location2D(1001, 1000));
            serverApi.AddNewItemToGround(0xeed, new Location2D(1001, 1001));

            injectionHost.Set("finddistance", 1);
            findTypeSubrutine.FindType(0xEED, -1, "ground");

            findTypeSubrutine.FindCount.Should().Be(9);
        }

        [TestMethod]
        public void Doesnt_find_items_outside_chebyshev_distance()
        {
            serverApi.PlayerEntersWorld(new Location2D(1000, 1000));

            // everything outside chebyshev distance
            serverApi.AddNewItemToGround(0xeed, new Location2D(998, 999));
            serverApi.AddNewItemToGround(0xeed, new Location2D(998, 1000));
            serverApi.AddNewItemToGround(0xeed, new Location2D(998, 1001));

            serverApi.AddNewItemToGround(0xeed, new Location2D(1000, 997));
            serverApi.AddNewItemToGround(0xeed, new Location2D(1000, 998));
            serverApi.AddNewItemToGround(0xeed, new Location2D(1000, 1002));

            injectionHost.Set("finddistance", "1");
            findTypeSubrutine.FindType(0xEED, -1, "ground");

            findTypeSubrutine.FindCount.Should().Be(0);
        }

        [TestMethod]
        public void Distance_is_0_when_cannot_convert_set_value_to_int()
        {
            serverApi.PlayerEntersWorld(new Location2D(1000, 1000));

            serverApi.AddNewItemToGround(0xeed, new Location2D(998, 999));
            serverApi.AddNewItemToGround(0xeed, new Location2D(1000, 1000));

            injectionHost.Set("finddistance", "asdf");
            findTypeSubrutine.FindType(0xEED, -1, "ground");

            findTypeSubrutine.FindCount.Should().Be(1);
        }

        [TestMethod]
        public void All_items_on_ground_when_distance_is_negative()
        {
            serverApi.PlayerEntersWorld(new Location2D(1000, 1000));

            serverApi.AddNewItemToGround(0xeed, new Location2D(1010, 1010));
            serverApi.AddNewItemToGround(0xeed, new Location2D(1015, 1015));

            injectionHost.Set("finddistance", "-10");
            findTypeSubrutine.FindType(0xEED, -1, "ground");

            findTypeSubrutine.FindCount.Should().Be(2);
        }
    }
}
