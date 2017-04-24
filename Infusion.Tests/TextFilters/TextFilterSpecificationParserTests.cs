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
    public class TextFilterSpecificationParserTests
    {
        [TestMethod]
        public void Can_parse_StartsWith_specification()
        {
            TextFilterSpecificationParser.Parse("starts with*")
                .As<StartsWithFilter>()
                .ProhibitedText.Should()
                .Be("starts with");
        }

        [TestMethod]
        public void Can_parse_EndsWith_specification()
        {
            TextFilterSpecificationParser.Parse("*ends with")
                .As<EndsWithFilter>()
                .ProhibitedText.Should()
                .Be("ends with");
        }

        [TestMethod]
        public void Can_parse_Contains_specification()
        {
            TextFilterSpecificationParser.Parse("*contains*")
                .As<ContainsFilter>()
                .ProhibitedText.Should()
                .Be("contains");
        }

        [TestMethod]
        public void Can_parse_Name_specification()
        {
            TextFilterSpecificationParser.Parse("{name}")
                .Should().BeOfType<NameFilter>();
        }

        [TestMethod]
        public void Can_parse_any_text_as_Contains()
        {
            TextFilterSpecificationParser.Parse("any text")
                .As<ContainsFilter>()
                .ProhibitedText.Should()
                .Be("any text");
        }

        [TestMethod]
        public void Can_parse_JustNumber_filter()
        {
            TextFilterSpecificationParser.Parse("{number}")
                .Should().BeOfType<JustNumberFilter>();
        }
    }
}
