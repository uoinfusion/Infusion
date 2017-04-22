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
    public class ContainsFilterTests
    {
        [TestMethod]
        public void Can_pass_text_without_specified_text()
        {
            new ContainsFilter("text").IsPassing("without bla bla bla").Should().BeTrue();
        }

        [TestMethod]
        public void Can_filter_out_text_with_specified_text()
        {
            new ContainsFilter("text").IsPassing("without text bla bla bla").Should().BeFalse();
        }
    }
}
