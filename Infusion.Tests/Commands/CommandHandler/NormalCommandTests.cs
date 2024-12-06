using FluentAssertions;
using Infusion.Commands;
using Infusion.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace Infusion.Tests.Commands
{
    [TestClass]
    public class NormalCommandTests
    {
        private CommandHandler commandHandler;
        private CancellationToken cancellationToken;
        private RingBufferLogger logger;

        [TestInitialize]
        public void Initialize()
        {
            logger = new RingBufferLogger(16);
            commandHandler = new CommandHandler(logger);
            commandHandler.CancellationTokenCreated += (sender, token) => cancellationToken = token;
        }

        [TestMethod]
        public void Restarts_Normal_command_when_it_already_runs()
        {
            int executionCount = 0;

            var command = new TestCommand(commandHandler, "cmd1", CommandExecutionMode.Normal, () => executionCount++);
            commandHandler.RegisterCommand(command.Command);
            commandHandler.InvokeSyntax(",cmd1");
            commandHandler.InvokeSyntax(",cmd1");

            command.Finish();
            command.WaitForFinished();

            executionCount.Should().Be(1);
        }
    }
}
