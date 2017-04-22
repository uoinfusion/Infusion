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
    public class EndsWithFilterTests
    {
        [TestMethod]
        public void Can_pass_text_without_specified_text_at_end()
        {
            new EndsWithFilter("text").IsPassing("without bla bla bla").Should().BeTrue();
        }

        [TestMethod]
        public void Can_filter_out_text_with_text_at_end()
        {
            new EndsWithFilter("text").IsPassing("bla bla bla text").Should().BeFalse();
        }

    }
}
