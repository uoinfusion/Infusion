using FluentAssertions;
using Infusion.TextFilters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infusion.Tests.TextFilters
{
    [TestClass]
    public class JustNumberFilterTests
    {
        [TestMethod]
        public void Can_filter_out_text_containing_just_name_and_number()
        {
            new JustNumberFilter().IsPassing("Diblik: 1234").Should().BeFalse();
        }

        [TestMethod]
        public void Can_pass_text_containing_name_and_number_and_something_more()
        {
            new JustNumberFilter().IsPassing("Diblik: 1234 and something more").Should().BeTrue();
        }

        [TestMethod]
        public void Can_pass_text_containing_name_and_something_else_than_number()
        {
            new JustNumberFilter().IsPassing("Diblik: something else than number").Should().BeTrue();
        }

        [TestMethod]
        public void Can_pass_text_not_containing_name()
        {
            new JustNumberFilter().IsPassing("this doesn't contain name").Should().BeTrue();
        }
    }
}
