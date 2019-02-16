using FluentAssertions;
using Infusion.LegacyApi.Injection;
using InjectionScript.Runtime;
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
        private InjectionProxy injection;

        [TestInitialize]
        public void Initialize()
        {
            injection = new InjectionProxy();
        }

        [TestMethod]
        public void Finds_in_backpack_when_no_container()
        {
            injection.ServerApi.PlayerEntersWorld(new Location2D(1000, 1000));
            injection.ServerApi.AddNewItemToBackpack(0xEED);

            injection.InjectionHost.UO.FindType(0xEED);

            injection.InjectionHost.UO.FindCount().Should().Be(1);
        }

        [TestMethod]
        public void Finds_any_color_when_no_color_specified()
        {
            injection.ServerApi.PlayerEntersWorld(new Location2D(1000, 1000));

            injection.ServerApi.AddNewItemToBackpack(0xEED, 10, (Color)0x0100);
            injection.ServerApi.AddNewItemToBackpack(0xEED, 20, (Color)0x0200);

            injection.InjectionHost.UO.FindType(0xEED);

            injection.InjectionHost.UO.FindCount().Should().Be(2);
        }

        [TestMethod]
        public void Finds_items_with_specified_color()
        {
            injection.ServerApi.PlayerEntersWorld(new Location2D(1000, 1000));

            injection.ServerApi.AddNewItemToBackpack(0xEED, 10, (Color)0x0100);
            injection.ServerApi.AddNewItemToBackpack(0xEED, 20, (Color)0x0200);

            injection.InjectionHost.UO.FindType(0xEED, 0x0100, -1);

            injection.InjectionHost.UO.FindCount().Should().Be(1);
        }

        [TestMethod]
        public void Finds_any_color_when_color_is_minus_1()
        {
            injection.ServerApi.PlayerEntersWorld(new Location2D(1000, 1000));

            injection.ServerApi.AddNewItemToBackpack(0xEED, 10, (Color)0x0100);
            injection.ServerApi.AddNewItemToBackpack(0xEED, 20, (Color)0x0200);

            injection.InjectionHost.UO.FindType(0xEED, -1, -1);

            injection.InjectionHost.UO.FindCount().Should().Be(2);
        }

        [TestMethod]
        public void Finds_on_ground_when_container_is_1()
        {
            injection.ServerApi.PlayerEntersWorld(new Location2D(1000, 1000));

            injection.ServerApi.AddNewItemToGround(0xEED, new Location2D(1001, 1001), 20, (Color)0);

            injection.InjectionHost.UO.FindType(0xEED, -1, 1);

            injection.InjectionHost.UO.FindCount().Should().Be(1);
        }

        [TestMethod]
        public void Finds_in_backpack_when_container_is_minus_1()
        {
            injection.ServerApi.PlayerEntersWorld(new Location2D(1000, 1000));

            injection.ServerApi.AddNewItemToBackpack(0xEED);

            injection.InjectionHost.UO.FindType(0xEED, -1, -1);

            injection.InjectionHost.UO.FindCount().Should().Be(1);
        }

        [TestMethod]
        public void Finds_nearest_item_on_ground()
        {
            injection.ServerApi.PlayerEntersWorld(new Location2D(1000, 1000));

            injection.ServerApi.AddNewItemToGround(0xEED, new Location2D(1005, 1005), 20, (Color)0);
            var nearestId = injection.ServerApi.AddNewItemToGround(0xEED, new Location2D(1001, 1001), 15, (Color)0);

            injection.InjectionHost.UO.FindType(0xEED, -1, 1);

            injection.InjectionHost.UO.GetSerial("finditem").Should().Be(NumberConversions.Int2Hex((int)nearestId));
        }

        [TestMethod]
        public void Finds_on_ground_when_container_is_ground()
        {
            injection.ServerApi.PlayerEntersWorld(new Location2D(1000, 1000));

            injection.ServerApi.AddNewItemToGround(0xEED, new Location2D(1001, 1001), 20, (Color)0);

            injection.InjectionHost.UO.FindType(new InjectionValue(0xEED), new InjectionValue(-1), new InjectionValue("ground"));

            injection.InjectionHost.UO.FindCount().Should().Be(1);
        }

        [TestMethod]
        public void Finds_in_backpack_when_container_is_my()
        {
            injection.ServerApi.PlayerEntersWorld(new Location2D(1000, 1000));

            injection.ServerApi.AddNewItemToBackpack(0xEED);

            injection.InjectionHost.UO.FindType(new InjectionValue(0xEED), new InjectionValue(-1), new InjectionValue("my"));

            injection.InjectionHost.UO.FindCount().Should().Be(1);
        }

        [TestMethod]
        public void Ignores_nested_containers_in_backpack()
        {
            injection.ServerApi.PlayerEntersWorld(new Location2D(1000, 1000));
            var subContainerId = injection.ServerApi.AddNewItemToBackpack(0x0E75);
            injection.ServerApi.AddNewItemToContainer(0xeed, containerId: subContainerId);

            injection.InjectionHost.UO.FindType(new InjectionValue(0xEED), new InjectionValue(-1), new InjectionValue("my"));

            injection.InjectionHost.UO.FindCount().Should().Be(0);
        }

        [TestMethod]
        public void Finds_in_container_when_container_is_id()
        {
            injection.ServerApi.PlayerEntersWorld(new Location2D(1000, 1000));
            var subContainerId = injection.ServerApi.AddNewItemToBackpack(0x0E75);
            injection.ServerApi.AddNewItemToContainer(0xeed, containerId: subContainerId);

            injection.InjectionHost.UO.FindType(new InjectionValue(0xEED), new InjectionValue(-1), new InjectionValue((int)subContainerId));

            injection.InjectionHost.UO.FindCount().Should().Be(1);
        }

        [TestMethod]
        public void Ignores_nested_containers()
        {
            injection.ServerApi.PlayerEntersWorld(new Location2D(1000, 1000));
            var subContainerId = injection.ServerApi.AddNewItemToBackpack(0x0E75);
            injection.ServerApi.AddNewItemToContainer(0xeed, containerId: subContainerId);

            injection.InjectionHost.UO.FindType(new InjectionValue(0xEED), new InjectionValue(-1), new InjectionValue((int)injection.TestProxy.Api.Me.BackPack.Id));

            injection.InjectionHost.UO.FindCount().Should().Be(0);
        }

        [TestMethod]
        public void Findcount_resets_to_0_when_no_item_found()
        {
            injection.ServerApi.PlayerEntersWorld(new Location2D(1000, 1000));
            injection.ServerApi.AddNewItemToBackpack(0xEED);

            injection.InjectionHost.UO.FindType(0xEED);
            injection.InjectionHost.UO.FindCount().Should().Be(1);

            injection.InjectionHost.UO.FindType(0xEEF);
            injection.InjectionHost.UO.FindCount().Should().Be(0);
        }

        [TestMethod]
        public void Unsets_finditem_when_item_not_found()
        {
            injection.ServerApi.PlayerEntersWorld(new Location2D(1000, 1000));
            injection.ServerApi.AddNewItemToBackpack(0xEED);

            injection.InjectionHost.UO.FindType(0xEED);
            injection.InjectionHost.UO.FindCount().Should().Be(1);

            injection.InjectionHost.UO.FindType(0xEEF);

            injection.InjectionHost.GetObject("finditem").Should().Be(0);
        }

        [TestMethod]
        public void Finds_item_inside_chebyshev_distance_specified_by_finddistance()
        {
            injection.ServerApi.PlayerEntersWorld(new Location2D(1000, 1000));

            // everything inside chebyshev distance
            injection.ServerApi.AddNewItemToGround(0xeed, new Location2D(999, 999));
            injection.ServerApi.AddNewItemToGround(0xeed, new Location2D(999, 1000));
            injection.ServerApi.AddNewItemToGround(0xeed, new Location2D(999, 1001));

            injection.ServerApi.AddNewItemToGround(0xeed, new Location2D(1000, 999));
            injection.ServerApi.AddNewItemToGround(0xeed, new Location2D(1000, 1000));
            injection.ServerApi.AddNewItemToGround(0xeed, new Location2D(1000, 1001));

            injection.ServerApi.AddNewItemToGround(0xeed, new Location2D(1001, 999));
            injection.ServerApi.AddNewItemToGround(0xeed, new Location2D(1001, 1000));
            injection.ServerApi.AddNewItemToGround(0xeed, new Location2D(1001, 1001));

            injection.InjectionHost.UO.Set("finddistance", 1);
            injection.InjectionHost.UO.FindType(new InjectionValue(0xEED), new InjectionValue(-1), new InjectionValue("ground"));

            injection.InjectionHost.UO.FindCount().Should().Be(9);
        }

        [TestMethod]
        public void Finds_item_inside_chebyshev_distance_specified_by_parameter()
        {
            injection.ServerApi.PlayerEntersWorld(new Location2D(1000, 1000));

            // everything inside chebyshev distance
            injection.ServerApi.AddNewItemToGround(0xeed, new Location2D(999, 999));
            injection.ServerApi.AddNewItemToGround(0xeed, new Location2D(999, 1000));
            injection.ServerApi.AddNewItemToGround(0xeed, new Location2D(999, 1001));

            injection.ServerApi.AddNewItemToGround(0xeed, new Location2D(1000, 999));
            injection.ServerApi.AddNewItemToGround(0xeed, new Location2D(1000, 1000));
            injection.ServerApi.AddNewItemToGround(0xeed, new Location2D(1000, 1001));

            injection.ServerApi.AddNewItemToGround(0xeed, new Location2D(1001, 999));
            injection.ServerApi.AddNewItemToGround(0xeed, new Location2D(1001, 1000));
            injection.ServerApi.AddNewItemToGround(0xeed, new Location2D(1001, 1001));

            injection.InjectionHost.UO.FindType(new InjectionValue(0xEED),
                new InjectionValue(-1),
                new InjectionValue("ground"),
                new InjectionValue(1));

            injection.InjectionHost.UO.FindCount().Should().Be(9);
        }

        [TestMethod]
        public void Finds_item_inside_zero_distance_specified_by_string_parameter()
        {
            injection.ServerApi.PlayerEntersWorld(new Location2D(1000, 1000));

            // everything inside chebyshev distance
            injection.ServerApi.AddNewItemToGround(0xeed, new Location2D(999, 999));
            injection.ServerApi.AddNewItemToGround(0xeed, new Location2D(999, 1000));
            injection.ServerApi.AddNewItemToGround(0xeed, new Location2D(999, 1001));

            injection.ServerApi.AddNewItemToGround(0xeed, new Location2D(1000, 999));
            injection.ServerApi.AddNewItemToGround(0xeed, new Location2D(1000, 1000));
            injection.ServerApi.AddNewItemToGround(0xeed, new Location2D(1000, 1001));

            injection.ServerApi.AddNewItemToGround(0xeed, new Location2D(1001, 999));
            injection.ServerApi.AddNewItemToGround(0xeed, new Location2D(1001, 1000));
            injection.ServerApi.AddNewItemToGround(0xeed, new Location2D(1001, 1001));

            injection.InjectionHost.UO.FindType(new InjectionValue("0xEED"),
                new InjectionValue("-1"),
                new InjectionValue("ground"),
                new InjectionValue("0"));

            injection.InjectionHost.UO.FindCount().Should().Be(1);
        }

        [TestMethod]
        public void Doesnt_find_items_outside_chebyshev_distance()
        {
            injection.ServerApi.PlayerEntersWorld(new Location2D(1000, 1000));

            // everything outside chebyshev distance
            injection.ServerApi.AddNewItemToGround(0xeed, new Location2D(998, 999));
            injection.ServerApi.AddNewItemToGround(0xeed, new Location2D(998, 1000));
            injection.ServerApi.AddNewItemToGround(0xeed, new Location2D(998, 1001));

            injection.ServerApi.AddNewItemToGround(0xeed, new Location2D(1000, 997));
            injection.ServerApi.AddNewItemToGround(0xeed, new Location2D(1000, 998));
            injection.ServerApi.AddNewItemToGround(0xeed, new Location2D(1000, 1002));

            injection.InjectionHost.UO.Set("finddistance", "1");
            injection.InjectionHost.UO.FindType(new InjectionValue(0xEED), new InjectionValue(-1), new InjectionValue("ground"));

            injection.InjectionHost.UO.FindCount().Should().Be(0);
        }

        [TestMethod]
        public void Distance_is_0_when_cannot_convert_set_value_to_int()
        {
            injection.ServerApi.PlayerEntersWorld(new Location2D(1000, 1000));

            injection.ServerApi.AddNewItemToGround(0xeed, new Location2D(998, 999));
            injection.ServerApi.AddNewItemToGround(0xeed, new Location2D(1000, 1000));

            injection.InjectionHost.UO.Set("finddistance", "asdf");
            injection.InjectionHost.UO.FindType(new InjectionValue(0xEED), new InjectionValue(-1), new InjectionValue("ground"));

            injection.InjectionHost.UO.FindCount().Should().Be(1);
        }

        [TestMethod]
        public void All_items_on_ground_when_distance_is_negative()
        {
            injection.ServerApi.PlayerEntersWorld(new Location2D(1000, 1000));

            injection.ServerApi.AddNewItemToGround(0xeed, new Location2D(1010, 1010));
            injection.ServerApi.AddNewItemToGround(0xeed, new Location2D(1015, 1015));

            injection.InjectionHost.UO.Set("finddistance", "-10");
            injection.InjectionHost.UO.FindType(new InjectionValue(0xEED), new InjectionValue(-1), new InjectionValue("ground"));

            injection.InjectionHost.UO.FindCount().Should().Be(2);
        }

        [TestMethod]
        public void Ignores_ignored_items()
        {
            injection.ServerApi.PlayerEntersWorld(new Location2D(1000, 1000));

            var itemId1 = injection.ServerApi.AddNewItemToGround(0xeed, new Location2D(1001, 1001));
            var itemId2 = injection.ServerApi.AddNewItemToGround(0xeed, new Location2D(1002, 1002));
            injection.ServerApi.AddNewItemToGround(0xeed, new Location2D(1003, 1003));

            injection.InjectionHost.UO.Ignore(NumberConversions.Int2Hex(itemId1));
            injection.InjectionHost.UO.Ignore(NumberConversions.Int2Hex(itemId2));

            injection.InjectionHost.UO.FindType(new InjectionValue(0xEED), new InjectionValue(-1), new InjectionValue("ground"));

            injection.InjectionHost.UO.FindCount().Should().Be(1);
        }

        [TestMethod]
        public void Can_clear_ignored_items()
        {
            injection.ServerApi.PlayerEntersWorld(new Location2D(1000, 1000));

            var itemId1 = injection.ServerApi.AddNewItemToGround(0xeed, new Location2D(1001, 1001));
            var itemId2 = injection.ServerApi.AddNewItemToGround(0xeed, new Location2D(1002, 1002));
            injection.ServerApi.AddNewItemToGround(0xeed, new Location2D(1003, 1003));

            injection.InjectionHost.UO.Ignore((int)itemId1);
            injection.InjectionHost.UO.Ignore((int)itemId2);
            injection.InjectionHost.UO.IgnoreReset();

            injection.InjectionHost.UO.FindType(new InjectionValue(0xEED), new InjectionValue(-1), new InjectionValue("ground"));

            injection.InjectionHost.UO.FindCount().Should().Be(3);
        }
    }
}
