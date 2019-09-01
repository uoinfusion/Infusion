using System;
using FluentAssertions;
using Infusion.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infusion.Tests.Commands
{
    [TestClass]
    public class CommandParameterParserTests
    {
        [TestMethod]
        public void Can_parse_ints_list_separated_by_space()
        {
            var parameterParser = new CommandParameterParser("1234 4321 9876");

            parameterParser.ParseInt().Should().Be(1234);
            parameterParser.ParseInt().Should().Be(4321);
            parameterParser.ParseInt().Should().Be(9876);
        }

        [TestMethod]
        public void Can_parse_ints_separated_by_multiple_spaces()
        {
            var parameterParser = new CommandParameterParser("1234   4321");

            parameterParser.ParseInt().Should().Be(1234);
            parameterParser.ParseInt().Should().Be(4321);
        }

        [TestMethod]
        public void Throws_when_epected_int_parameter_in_wrong_format()
        {
            var parameterParser = new CommandParameterParser("1asd54");

            Action action = () => parameterParser.ParseInt();
            action.Should().Throw<CommandInvocationException>();
        }

        [TestMethod]
        public void Throws_when_no_expected_in_parameter()
        {
            var parameterParser = new CommandParameterParser("1234 ");

            parameterParser.ParseInt();
            Action action = () => parameterParser.ParseInt();
            action.Should().Throw<CommandInvocationException>();
        }
    }
}
