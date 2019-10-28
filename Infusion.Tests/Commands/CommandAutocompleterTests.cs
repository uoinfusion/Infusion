using FluentAssertions;
using Infusion.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infusion.Tests.Commands
{
    [TestClass]
    public class CommandAutocompleterTests
    {
        [TestMethod]
        public void Can_return_just_one_name_for_exact_match()
        {
            var completer = new CommandAutocompleter(() => new[] { "info" });

            var result = completer.Autocomplete(",info");

            result.PotentialCommandNames.Should().ContainSingle("info");
        }

        [TestMethod]
        public void Can_return_just_one_name_for_exact_match_prefixed_with_whitespace()
        {
            var completer = new CommandAutocompleter(() => new[] { "info" });

            var result = completer.Autocomplete("  ,info");

            result.PotentialCommandNames.Should().ContainSingle("info");
        }

        [TestMethod]
        public void Can_return_names_starting_with_text_on_command_line()
        {
            var completer = new CommandAutocompleter(() => new[]
                {"info", "reload", "refresh", "recallto", "recallhome", "help"});

            var result = completer.Autocomplete(",re");

            result.PotentialCommandNames.Should().BeEquivalentTo("reload", "refresh", "recallto", "recallhome");
        }

        [TestMethod]
        public void Can_return_command_names_when_no_command_starting_with_text_on_command_line()
        {
            var completer = new CommandAutocompleter(() => new[]
                {"info", "reload", "refresh", "recallto", "recallhome", "help"});

            var result = completer.Autocomplete(",xxx");

            result.PotentialCommandNames.Should().BeEquivalentTo("info", "reload", "refresh", "recallto", "recallhome", "help");
        }

        [TestMethod]
        public void Can_return_all_command_names_when_no_command_name_after_leading_comma()
        {
            var completer = new CommandAutocompleter(() => new[]
                {"info", "reload", "refresh", "recallto", "recallhome", "help"});

            var result = completer.Autocomplete(",");

            result.PotentialCommandNames.Should().BeEquivalentTo("info", "reload", "refresh", "recallto", "recallhome", "help");
        }

        [TestMethod]
        public void Can_return_empty_list_when_no_command_invocation_syntax_on_command_line()
        {
            var completer = new CommandAutocompleter(() => new[]
                {"info", "reload", "refresh", "recallto", "recallhome", "help"});

            var result = completer.Autocomplete("something different");

            result.PotentialCommandNames.Should().BeEmpty();
        }

        [TestMethod]
        public void Can_return_empty_list_when_no_text_on_command_line()
        {
            var completer = new CommandAutocompleter(() => new[]
                {"info", "reload", "refresh", "recallto", "recallhome", "help"});

            var result = completer.Autocomplete(string.Empty);

            result.PotentialCommandNames.Should().BeEmpty();
        }

        [TestMethod]
        public void Can_return_empty_list_when_command_on_command_line_contains_parameters()
        {
            var completer = new CommandAutocompleter(() => new[]
                {"info", "reload", "refresh", "recallto", "recallhome", "help"});

            var result = completer.Autocomplete(",reload some parameter");

            result.PotentialCommandNames.Should().BeEmpty();
        }

        [TestMethod]
        public void Returns_autocompleted_command_line_when_multiple_potential_command_names()
        {
            var completer = new CommandAutocompleter(() => new[]
                {"asdf1", "asdf2", "asdf3", "asdf4"});

            var result = completer.Autocomplete(",as");

            result.IsAutocompleted.Should().BeTrue();
            result.AutocompletedCommandLine.Should().Be(",asdf");
        }

        [TestMethod]
        public void Returns_autocompleted_command_line_when_single_potential_command_name()
        {
            var completer = new CommandAutocompleter(() => new[]
                {"asdf1",});

            var result = completer.Autocomplete(",a");

            result.IsAutocompleted.Should().BeTrue();
            result.AutocompletedCommandLine.Should().Be(",asdf1 ");
        }

        [TestMethod]
        public void Returns_autocompleted_command_line_when_exact_match()
        {
            var completer = new CommandAutocompleter(() => new[]
                {"asdf1",});

            var result = completer.Autocomplete(",asdf1");

            result.IsAutocompleted.Should().BeTrue();
            result.AutocompletedCommandLine.Should().Be(",asdf1 ");
        }

        [TestMethod]
        public void Returns_no_autocompleted_command_when_no_potential_command_name()
        {
            var completer = new CommandAutocompleter(() => new[]
                {"asdf1",});

            var result = completer.Autocomplete(",xx");

            result.IsAutocompleted.Should().BeFalse();
            result.AutocompletedCommandLine.Should().BeNull();
        }

        [TestMethod]
        public void Returns_no_autocompleted_command_for_one_space()
        {
            var completer = new CommandAutocompleter(() => new[]
                {"asdf1",});

            var result = completer.Autocomplete(" ");

            result.IsAutocompleted.Should().BeFalse();
            result.AutocompletedCommandLine.Should().BeNull();
        }
    }
}