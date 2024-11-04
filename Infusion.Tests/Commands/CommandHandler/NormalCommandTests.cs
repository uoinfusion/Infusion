using FluentAssertions;
using Infusion.Commands;
using Infusion.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading;

namespace Infusion.Tests.Commands
{
    [TestClass]
    public class NormalCommandTests
    {
        private CommandHandler commandHandler;
        private CancellationToken cancellationToken;
        private RingBufferLogger logger;

        private void DoSomeCancellableAction()
        {
            while (!cancellationToken.IsCancellationRequested)
                Thread.Yield();
        }

        [TestInitialize]
        public void Initialize()
        {
            logger = new RingBufferLogger(16);
            commandHandler = new CommandHandler(logger);
            commandHandler.CancellationTokenCreated += (sender, token) => cancellationToken = token;
        }

        [TestMethod]
        public void Can_terminate_parent_command_executing_Normal_nested_command()
        {
            var nestedCommand = new TestCommand(commandHandler, "nested", CommandExecutionMode.Normal, () => DoSomeCancellableAction());
            commandHandler.RegisterCommand(nestedCommand.Command);

            var parentCommand = new TestCommand(commandHandler, "parent", () => commandHandler.InvokeSyntax(",nested"));
            commandHandler.RegisterCommand(parentCommand.Command);

            commandHandler.InvokeSyntax(",parent");
            parentCommand.WaitForInitialization();

            commandHandler.Terminate("parent");

            nestedCommand.Finish();
            parentCommand.Finish();

            parentCommand.WaitForFinished().Should().BeTrue();

            commandHandler.RunningCommands.Should().BeEmpty();
        }

        [TestMethod]
        public void Can_execute_nested_Normal_command_from_Normal_parent_on_same_thread()
        {
            int nestedCommandThreadId = -1;
            int parentCommandThreadId = -1;

            var nestedCommandExecuted = false;
            var nestedCommand = new TestCommand(commandHandler, "nested", CommandExecutionMode.Normal, () =>
            {
                nestedCommandThreadId = Thread.CurrentThread.ManagedThreadId;
                nestedCommandExecuted = true;
            });
            commandHandler.RegisterCommand(nestedCommand.Command);
            var parentCommand = new TestCommand(commandHandler, "cmd1", CommandExecutionMode.Normal, () =>
            {
                parentCommandThreadId = Thread.CurrentThread.ManagedThreadId;
                commandHandler.InvokeSyntax(",nested");
            });
            commandHandler.RegisterCommand(parentCommand.Command);

            commandHandler.InvokeSyntax(",cmd1");
            parentCommand.WaitForInitialization();
            nestedCommand.WaitForAdditionalAction();

            nestedCommandExecuted.Should().BeTrue();
            commandHandler.RunningCommands.Select(c => c.Name).Should().Contain("cmd1");
            commandHandler.RunningCommands.Select(c => c.Name).Should().NotContain("nested");
            nestedCommandThreadId.Should().NotBe(-1);
            parentCommandThreadId.Should().NotBe(-1);
            nestedCommandThreadId.Should().Be(parentCommandThreadId);

            nestedCommand.Finish();
            parentCommand.Finish();
        }

        [TestMethod]
        public void Can_execute_nested_Normal_command_from_Direct_parent()
        {
            var nestedCommandExecuted = false;
            var nestedCommand = new TestCommand(commandHandler, "nested", CommandExecutionMode.Normal,
                () =>
                {
                    nestedCommandExecuted = true;
                    commandHandler.RunningCommands.Select(c => c.Name).Should().Contain("parent");
                    commandHandler.RunningCommands.Select(c => c.Name).Should().NotContain("nested");
                });
            commandHandler.RegisterCommand(nestedCommand.Command);
            var parentCommand = new TestCommand(commandHandler, "parent", CommandExecutionMode.Direct,
                () => commandHandler.InvokeSyntax(",nested"));
            commandHandler.RegisterCommand(parentCommand.Command);

            nestedCommand.Finish();
            parentCommand.Finish();
            commandHandler.InvokeSyntax(",parent");

            nestedCommandExecuted.Should().BeTrue();
        }

        [TestMethod]
        public void Can_list_Normal_commands_executing_Normal_nested_command_multipletimes()
        {
            var nestedCommand = new TestCommand(commandHandler, "nested", CommandExecutionMode.Normal, () => { });
            commandHandler.RegisterCommand(nestedCommand.Command);
            var command = new TestCommand(commandHandler, "cmd1", () => commandHandler.InvokeSyntax(",nested"));
            commandHandler.RegisterCommand(command.Command);

            for (int i = 0; i < 100; i++)
            {
                nestedCommand.Reset();
                command.Reset();

                commandHandler.InvokeSyntax(",cmd1");
                nestedCommand.WaitForAdditionalAction();

                commandHandler.RunningCommands.Select(x => x.Name).Should().Contain("cmd1");
                commandHandler.RunningCommands.Select(x => x.Name).Should().NotContain("nested");

                nestedCommand.Finish();
                command.Finish();
                command.WaitForFinished();

                commandHandler.RunningCommands.Should().BeEmpty();
            }
        }

        [TestMethod]
        public void Cannot_execute_same_Normal_commands_in_parallel()
        {
            int executionCount = 0;

            var command = new TestCommand(commandHandler, "cmd1", () => executionCount++);
            commandHandler.RegisterCommand(command.Command);
            commandHandler.InvokeSyntax(",cmd1");
            commandHandler.InvokeSyntax(",cmd1");

            command.Finish();
            command.WaitForFinished();

            executionCount.Should().Be(1);
        }
    }
}
