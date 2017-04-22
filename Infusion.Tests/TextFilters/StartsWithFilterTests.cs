using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Infusion.TextFilters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infusion.Tests.TextFilters
{
    [TestClass]
    public class StartsWithFilterTests
    {
        [TestMethod]
        public void Can_pass_text_without_specified_text_at_begining()
        {
            new StartsWithFilter("text").IsPassing("without bla bla bla").Should().BeTrue();
        }

        [TestMethod]
        public void Can_filter_out_text_with_text_at_begining()
        {
            new StartsWithFilter("text").IsPassing("text bla bla bla").Should().BeFalse();
        }
    }
}
