using FluentAssertions;
using Infusion.Packets;
using Infusion.Proxy.LegacyApi;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infusion.Proxy.Tests.InjectionApi
{
    [TestClass]
    public class ItemSpecTests
    {
        [TestMethod]
        public void Spec_with_ModelId_matches_Item_with_same_ModelId()
        {
            var spec = new ItemSpec(0x4444);
            var item = new Item(0x000000, 0x4444, 1, new Location3D(0, 0, 0), (Color) 0);

            spec.Matches(item).Should().BeTrue();
        }

        [TestMethod]
        public void Spec_with_ModelId_matches_same_ModelId()
        {
            var spec = new ItemSpec(0x4444);
            spec.Matches(0x4444).Should().BeTrue();
        }

        [TestMethod]
        public void Spec_with_ModelId_doesnt_match_different_ModelId()
        {
            var spec = new ItemSpec(0x4444);
            spec.Matches(0x1111).Should().BeFalse();
        }

        [TestMethod]
        public void Spec_with_ModelId_and_Color_doesnt_match_same_ModelId()
        {
            var spec = new ItemSpec(0x4444, (Color)0x5555);
            spec.Matches(0x4444).Should().BeFalse();
        }

        [TestMethod]
        public void Spec_with_ModelId_not_matching_Item_with_other_ModelId()
        {
            var spec = new ItemSpec(0x4444);
            var item = new Item(0x000000, 0x2222, 1, new Location3D(0, 0, 0), (Color) 0);

            spec.Matches(item).Should().BeFalse();
        }

        [TestMethod]
        public void Spec_with_ModelId_and_Color_matching_Item_with_same_ModelId_and_Color()
        {
            var spec = new ItemSpec(0x4444, (Color) 0x22);
            var item = new Item(0x000000, 0x4444, 1, new Location3D(0, 0, 0), (Color) 0x22);

            spec.Matches(item).Should().BeTrue();
        }

        [TestMethod]
        public void Spec_with_ModelId_and_Color_not_matching_Item_with_different_Color()
        {
            var spec = new ItemSpec(0x4444, (Color) 0x22);
            var item = new Item(0x000000, 0x4444, 1, new Location3D(0, 0, 0), (Color) 0x99);

            spec.Matches(item).Should().BeFalse();
        }

        [TestMethod]
        public void Spec_with_any_matching_subspecs_Matches_item()
        {
            var spec = new ItemSpec(new ItemSpec(0x1111), new ItemSpec(0x2222));

            var item = new Item(0x000000, 0x1111, 1, new Location3D(0, 0, 0), (Color) 0x99);

            spec.Matches(item).Should().BeTrue();
        }

        [TestMethod]
        public void Spec_with_no_matching_subspecs_Not_Matching_item()
        {
            var spec = new ItemSpec(new ItemSpec(0x1111), new ItemSpec(0x2222));

            var item = new Item(0x000000, 0x9999, 1, new Location3D(0, 0, 0), (Color) 0x99);

            spec.Matches(item).Should().BeFalse();
        }

        [TestMethod]
        public void Matching_when_any_subspecs_has_same_ModelId()
        {
            var spec = new ItemSpec(new ItemSpec(0x1111), new ItemSpec(0x2222));

            spec.Matches(0x1111).Should().BeTrue();
            spec.Matches(0x2222).Should().BeTrue();
            spec.Matches(0x3333).Should().BeFalse();
        }

        [TestMethod]
        public void Can_construct_spec_by_listing_subspecs()
        {
            var spec = new ItemSpec(0x1111).Including(new ItemSpec(0x2222), new ItemSpec(0x3333));

            spec.Matches(new Item(0x000000, 0x1111, 1, new Location3D(0, 0, 0), (Color) 0x99)).Should().BeTrue();
            spec.Matches(new Item(0x000000, 0x2222, 1, new Location3D(0, 0, 0), (Color) 0x99)).Should().BeTrue();
            spec.Matches(new Item(0x000000, 0x3333, 1, new Location3D(0, 0, 0), (Color) 0x99)).Should().BeTrue();
        }
    }
}