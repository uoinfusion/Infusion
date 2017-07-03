using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infusion.Desktop.Tests
{
    [TestClass]
    public class CommandHistoryTests
    {
        [TestMethod]
        public void Can_move_to_older_command()
        {
            var history = new CommandHistory();

            history.EnterCommand("command1");
            history.EnterCommand("command2");
            history.EnterCommand("command3");

            history.GetOlder().Should().Be("command3");
            history.GetOlder().Should().Be("command2");
            history.GetOlder().Should().Be("command1");
        }

        [TestMethod]
        public void Can_move_to_newer_command()
        {
            var history = new CommandHistory();

            history.EnterCommand("command1");
            history.EnterCommand("command2");
            history.EnterCommand("command3");

            history.GetOlder().Should().Be("command3");
            history.GetOlder().Should().Be("command2");
            history.GetNewer().Should().Be("command3");
        }

        [TestMethod]
        public void Moves_to_last_command_when_new_command_entered_regardless_prior_position_in_history()
        {
            var history = new CommandHistory();

            history.EnterCommand("command1");
            history.EnterCommand("command2");
            history.EnterCommand("command3");

            history.GetOlder();
            history.GetOlder();

            history.EnterCommand("command4");

            history.GetOlder().Should().Be("command4");
            history.GetOlder().Should().Be("command3");
        }

        [TestMethod]
        public void Can_handle_older_command_request_when_there_is_no_command()
        {
            var history = new CommandHistory();

            history.GetOlder().Should().BeNull();
        }

        [TestMethod]
        public void Can_handle_newer_command_request_when_there_is_no_command()
        {
            var history = new CommandHistory();

            history.GetNewer().Should().BeNull();
        }

        [TestMethod]
        public void Returns_null_when_there_is_no_newer_command()
        {
            var history = new CommandHistory();

            history.EnterCommand("command1");
            history.GetOlder();
            history.GetNewer();
            history.GetNewer().Should().BeNull();
        }

        [TestMethod]
        public void Returns_null_when_there_is_no_older_command()
        {
            var history = new CommandHistory();

            history.EnterCommand("command1");
            history.GetOlder();
            history.GetOlder().Should().BeNull();
        }

        [TestMethod]
        public void Removes_oldest_command_from_history_when_max_history_length_is_reached()
        {
            var history = new CommandHistory(1);
            history.EnterCommand("command1");
            history.EnterCommand("command2");

            history.GetOlder().Should().Be("command2");
            history.GetOlder().Should().BeNull();
        }
    }
}
