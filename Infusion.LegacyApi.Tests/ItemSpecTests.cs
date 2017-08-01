using FluentAssertions;
using Infusion.Packets;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infusion.LegacyApi.Tests
{
    [TestClass]
    public class ItemSpecTests
    {
        [TestMethod]
        public void Spec_with_ModelId_matches_Item_with_same_ModelId()
        {
            var spec = new ItemSpec(0x4444);
            var item = new Item(new ObjectId(0), 0x4444, 1, new Location3D(0, 0, 0), null, null, null);

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
            var item = new Item(new ObjectId(0), 0x2222, 1, new Location3D(0, 0, 0), (Color)0, null, null);

            spec.Matches(item).Should().BeFalse();
        }

        [TestMethod]
        public void Spec_with_ModelId_and_Color_matching_Item_with_same_ModelId_and_Color()
        {
            var spec = new ItemSpec(0x4444, (Color)0x22);
            var item = new Item(new ObjectId(0), 0x4444, 1, new Location3D(0, 0, 0), (Color)0x22, null, null);

            spec.Matches(item).Should().BeTrue();
        }

        [TestMethod]
        public void Spec_with_ModelId_and_Color_not_matching_Item_with_different_Color()
        {
            var spec = new ItemSpec(0x4444, (Color)0x22);
            var item = new Item(new ObjectId(0), 0x4444, 1, new Location3D(0, 0, 0), (Color)0x99, null, null);

            spec.Matches(item).Should().BeFalse();
        }

        [TestMethod]
        public void Spec_with_any_matching_subspecs_Matches_item()
        {
            var spec = new ItemSpec(new ItemSpec(0x1111), new ItemSpec(0x2222));

            var item = new Item(new ObjectId(0), 0x1111, 1, new Location3D(0, 0, 0), (Color)0x99, null, null);

            spec.Matches(item).Should().BeTrue();
        }

        [TestMethod]
        public void Spec_with_no_matching_subspecs_Not_Matching_item()
        {
            var spec = new ItemSpec(new ItemSpec(0x1111), new ItemSpec(0x2222));

            var item = new Item(new ObjectId(0), 0x9999, 1, new Location3D(0, 0, 0), (Color)0x99, null, null);

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

            spec.Matches(new Item(new ObjectId(0), 0x1111, 1, new Location3D(0, 0, 0), (Color)0x99, null, null)).Should().BeTrue();
            spec.Matches(new Item(new ObjectId(0), 0x2222, 1, new Location3D(0, 0, 0), (Color)0x99, null, null)).Should().BeTrue();
            spec.Matches(new Item(new ObjectId(0), 0x3333, 1, new Location3D(0, 0, 0), (Color)0x99, null, null)).Should().BeTrue();
        }

        [TestMethod]
        public void Spec_with_subspecs_is_least_specific()
        {
            var withSubspecs = new ItemSpec(0x1111).Including(new ItemSpec(0x2222), new ItemSpec(0x3333));
            var withType = new ItemSpec(0x2222);
            var withTypeAndColor = new ItemSpec(0x3333, (Color)0x0010);

            withSubspecs.Specificity.Should().BeLessThan(withType.Specificity);
            withSubspecs.Specificity.Should().BeLessThan(withTypeAndColor.Specificity);
        }

        [TestMethod]
        public void Spec_with_type_and_color_is_most_specific()
        {
            var withSubspecs = new ItemSpec(0x1111).Including(new ItemSpec(0x2222), new ItemSpec(0x3333));
            var withType = new ItemSpec(0x2222);
            var withTypeAndColor = new ItemSpec(0x3333, (Color)0x0010);

            withTypeAndColor.Specificity.Should().BeGreaterThan(withSubspecs.Specificity);
            withTypeAndColor.Specificity.Should().BeGreaterThan(withType.Specificity);
        }
    }
}