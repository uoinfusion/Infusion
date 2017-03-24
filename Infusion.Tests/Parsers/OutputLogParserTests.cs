using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Infusion.Parsers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infusion.Tests.Parsers
{
    [TestClass]
    public class OutputLogParserTests
    {
        [TestMethod]
        public void Can_provide_time_and_rest_of_the_line_to_processor()
        {
            var processor = new TestProcessor();
            var parser = new OutputLogParser("4:21:21 PM: Skill Lumberjacking increased by 0.1 %, currently it is 88.6 %", processor);
            parser.Parse();

            processor.Result.Should().Be("04:21:21 - Skill Lumberjacking increased by 0.1 %, currently it is 88.6 %");
        }

        [TestMethod]
        public void Can_parse_multiple_lines_with_unix_eol()
        {
            var processor = new TestProcessor();
            var parser = new OutputLogParser("4:21:21 PM: x\n4:21:21 PM: y", processor);
            parser.Parse();

            processor.Result.Should().Be($"04:21:21 - x{Environment.NewLine}04:21:21 - y");
        }

        [TestMethod]
        public void Can_parse_multiple_lines_with_dos_eol()
        {
            var processor = new TestProcessor();
            var parser = new OutputLogParser("4:21:21 PM: x\r\n4:21:21 PM: y", processor);
            parser.Parse();

            processor.Result.Should().Be($"04:21:21 - x{Environment.NewLine}04:21:21 - y");
        }

        [TestMethod]
        public void Skips_lines_without_colon()
        {
            var processor = new TestProcessor();
            var parser = new OutputLogParser("some random text withotu colon", processor);
            parser.Parse();

            processor.Result.Should().Be(string.Empty);
        }

        [TestMethod]
        public void Skips_lines_wihtout_valid_time_before_colon()
        {
            var processor = new TestProcessor();
            var parser = new OutputLogParser("some random text with colon: but without valid time", processor);
            parser.Parse();

            processor.Result.Should().Be(string.Empty);
        }

        private class TestProcessor : IOutputLogProcessor
        {
            private readonly StringBuilder builder = new StringBuilder();

            public string Result => builder.ToString().Trim();

            public void Process(DateTimeOffset time, string line)
            {
                builder.AppendLine($"{time:hh:mm:ss} - {line}");
            }
        }
    }
}
